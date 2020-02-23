using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Config.Logic;
using DocSystem.Logic.Domain;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class DBSConfigController : BaseConfigController<S_T_DBSDefine>
    {
        public override ActionResult Edit()
        {
            var dbsTypeList = EnumBaseHelper.GetEnumDef(typeof(DBSType)).EnumItem.ToList();
            var dbsType = new List<Dictionary<string, object>>();
            foreach (var item in dbsTypeList)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", item.Code);
                dic.SetValue("text", item.Name);
                dbsType.Add(dic);
            }
            this.ViewBag.DBSType = JsonHelper.ToJson(dbsType);
            return this.View();
        }

        public ActionResult TreeList()
        {
            var dbsTypeList = EnumBaseHelper.GetEnumDef(typeof(DBSType)).EnumItem.ToList();
            var dbsType = new List<Dictionary<string, object>>();
            foreach (var item in dbsTypeList)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", item.Code);
                dic.SetValue("text", item.Name);
                dbsType.Add(dic);
            }
            this.ViewBag.DBSType = JsonHelper.ToJson(dbsType);
            return this.View();
        }

        protected override void BeforeSave(S_T_DBSDefine entity, bool isNew)
        {
            var parent = this.entities.Set<S_T_DBSDefine>().FirstOrDefault(d => d.ID == entity.ParentID);
            if (isNew)
            {
                if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + entity.ParentID + "】的父节点对象，无法保存");
                parent.AddChild(entity);
            }
            var docConfigEntities = FormulaHelper.GetEntities<DocConfigEntities>();
            if (parent != null)
            {
                foreach (var _id in parent.FullID.Split('.'))
                {
                    var item = this.GetEntityByID<S_T_DBSDefine>(_id);
                    if (item != null && !string.IsNullOrEmpty(item.ArchiveFolder))
                    {
                        var nodeConfig = docConfigEntities.Set<S_DOC_Node>().FirstOrDefault(a => a.ID == item.ArchiveFolder);
                        if (nodeConfig == null) throw new Formula.Exceptions.BusinessException("检测到父节点【" + item.Name + "】未找到归档目录【" + item.ArchiveFolderName + "】");
                        if (nodeConfig.IsFreeNode == DocSystem.Logic.Domain.TrueOrFalse.True.ToString())
                            throw new Exception("检测到父节点【" + item.Name + "】的归档目录【" + item.ArchiveFolderName + "】为自由节点，其下层不能有其他项目资料节点");
                    }
                }
            }
            if (!string.IsNullOrEmpty(entity.ArchiveFolder))
            {
                var nodeConfig = docConfigEntities.Set<S_DOC_Node>().FirstOrDefault(a => a.ID == entity.ArchiveFolder);
                if (nodeConfig == null) throw new Formula.Exceptions.BusinessException("未找到归档目录【" + entity.ArchiveFolderName + "】");
                if (nodeConfig.IsFreeNode == DocSystem.Logic.Domain.TrueOrFalse.True.ToString() && entity.Children.Count > 0)
                    throw new Exception("归档目录【" + entity.ArchiveFolderName + "】为自由节点，下层不能有其他项目资料节点");
            }
        }

        protected override void AfterSave(S_T_DBSDefine entity, bool isNew)
        {
            string dbsType = DBSType.Cloud.ToString();
            if (!isNew && entity.DBSType == dbsType)
            {
                var childrens = entity.AllChildren;
                if (childrens.Count != 0)
                    foreach (var children in childrens)
                        children.DBSType = dbsType;
                entities.SaveChanges();
            }
            //更新项目库dbs表的归档目录字段
            var projectDB = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            string sql = @"update S_D_DBS set ArchiveFolder='{1}',ArchiveFolderName='{2}' where ConfigDBSID='{0}'";
            sql = string.Format(sql, entity.ID, entity.ArchiveFolder, entity.ArchiveFolderName);
            projectDB.ExecuteNonQuery(sql);
        }

        protected override void BeforeDelete(List<S_T_DBSDefine> entityList)
        {
            foreach (var item in entityList)
                item.Delete();
        }

        public JsonResult GetAuthList(string DBSDefineID, string RoleType)
        {
            string sql = @"select S_D_RoleDefine.RoleCode,S_D_RoleDefine.RoleName,
case when AuthType <>'{0}' or AuthType is null then 'False' else 'True' end as FullControlAuth,
case when AuthType <>'{1}' or AuthType is null then 'False' else 'True' end as WriteAuth,
case when AuthType <>'{2}' or AuthType is null then 'False' else 'True' end as ViewAuth
from S_D_RoleDefine left join (select * from S_T_DBSSecurity where DBSDefineID='{3}' and RoleType='{4}') DBSSecurity
on DBSSecurity.RoleCode=S_D_RoleDefine.RoleCode";
            if (RoleType != Project.Logic.RoleType.ProjectRoleType.ToString())
            {
                sql = @"select RoleTB.Code RoleCode,RoleTB.Name RoleName,
case when AuthType <>'{0}' or AuthType is null then 'False' else 'True' end as FullControlAuth,
case when AuthType <>'{1}' or AuthType is null then 'False' else 'True' end as WriteAuth,
case when AuthType <>'{2}' or AuthType is null then 'False' else 'True' end as ViewAuth
from (select * from " + SQLHelper.CreateSqlHelper(ConnEnum.Base.ToString()).DbName + "..S_A_Role where type='" + RoleType.Replace("Type", "") + @"') RoleTB 
left join (select * from S_T_DBSSecurity where DBSDefineID='{3}' and RoleType='{4}') DBSSecurity
on DBSSecurity.RoleCode=RoleTB.Code
";

            }
            sql = String.Format(sql, FolderAuthType.FullControl.ToString(), FolderAuthType.ReadAndWrite.ToString(), FolderAuthType.ReadOnly.ToString(), DBSDefineID, RoleType);
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            var dt = sqlHelper.ExecuteDataTable(sql);
            var data = new GridData(dt);
            return Json(data);
        }

        public JsonResult SetAuth(string RoleCode, string RoleName, string DBSDefineID, string authType, string RoleType)
        {
            var dbs = this.entities.Set<S_T_DBSDefine>().SingleOrDefault(d => d.ID == DBSDefineID);
            if (dbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【"+DBSDefineID+"】的DBS对象，无法设置权限");
            dbs.SetAuth(RoleCode, RoleName, authType, RoleType);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RemoveAuth(string RoleCode, string DBSDefineID)
        {
            this.entities.Set<S_T_DBSSecurity>().Delete(d => d.RoleCode == RoleCode && d.DBSDefineID == DBSDefineID);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult InhertAuth(string DBSNodeID)
        {
            var dbs = this.GetEntityByID<S_T_DBSDefine>(DBSNodeID);
            if (dbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + DBSNodeID + "】的DBS对象，无法进行权限继承操作");
            dbs.InhertParentAuth();
            this.entities.SaveChanges();
            return Json("");
        }
    }
}