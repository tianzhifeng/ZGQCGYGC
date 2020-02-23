using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic.Domain;
using Formula;
using EPC.Logic;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class MeunDefineController : InfrastructureController<S_C_Meun>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            qb.Add("MeunType", Formula.QueryMethod.Equal, "Root");
            return base.GetList(qb);
        }

        protected override void AfterGetMode(S_C_Meun entity, bool isNew)
        {
            if (isNew)
            {
                entity.MeunType = this.GetQueryString("MeunType");
                entity.MeunDefineType = this.GetQueryString("MeunDefineType");
            }
        }

        public override JsonResult Save()
        {
            var define = this.UpdateEntity<S_C_Meun>();
            if (this.entities.Entry<S_C_Meun>(define).State == System.Data.EntityState.Detached || this.entities.Entry<S_C_Meun>(define).State == System.Data.EntityState.Added)
            {
                if (define.MeunType == "Root")
                {
                    S_C_Meun.AddRoot(define);
                }
                else
                {
                    var parent = this.GetEntityByID<S_C_Meun>(define.ParentID);
                    if (parent == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + define.ParentID + "】的定义节点，无法增加SpaceDefine子节点");
                    parent.AddChild(define);
                }
            }
            else
            {
                if (define.MeunType == "Root" && !String.IsNullOrEmpty(define.NavWBSType))
                {
                    this.entities.Set<S_C_Meun>().Where(c => c.FullID.StartsWith(define.FullID) && c.ID != define.ID).Update(c => c.NavWBSType = define.NavWBSType);
                }
            }
            this.entities.SaveChanges();
            return Json(define);
        }

        protected override void BeforeDelete(List<S_C_Meun> entityList)
        {
            foreach (var item in entityList)
            {
                item.Delete();
            }
        }

        public JsonResult GetDefineTree(string ID)
        {
            var nodes = this.entities.Set<S_C_Meun>().Where(d => d.FullID.StartsWith(ID)).OrderBy(d => d.SortIndex).ToList();
            return Json(nodes);
        }

        #region 菜单授权
        public JsonResult GetRoleList(QueryBuilder qb)
        {
            string roleType = this.Request["roleType"];
            qb.PageSize = 0;
            if (String.IsNullOrEmpty(roleType)) roleType = EPC.Logic.RoleType.ProjectRoleType.ToString();
            if (roleType == EPC.Logic.RoleType.ProjectRoleType.ToString())
            {
                qb.SortField = "SortIndex";
                qb.SortOrder = "asc";
                string sql = "select ID,RoleCode,RoleName,'ProjectRoleType' as RoleType,SortIndex from S_T_RoleDefine";
                var list = this.SqlHelper.ExecuteGridData(sql, qb);
                return Json(list);
            }
            else
            {
                var service = FormulaHelper.GetService<IRoleService>();
                var list = service.GetSysRoles().AsQueryable().Where(qb).ToList();
                var data = FormulaHelper.CollectionToListDic<Config.Role>(list);
                foreach (var item in data)
                {
                    item["RoleName"] = item["Name"].ToString();
                    item["RoleCode"] = item["Code"].ToString();
                    item["RoleType"] = EPC.Logic.RoleType.SysRoleType.ToString();
                }
                var gridData = new GridData(data);
                return Json(gridData);
            }
        }

        public JsonResult GetSpaceList(string ModeID, string roleCode)
        {
            string sql = @"select S_C_Meun.*,S_C_MenuAuth.IsDefault,
case when S_C_MenuAuth.ID is null then 'False' else 'True' end as HasAuth
from S_C_Meun left join S_C_MenuAuth  on S_C_MenuAuth.MeunID=S_C_Meun.ID
and RoleKey='{1}' where MeunType='Root' and ModeID='{0}' order by SortIndex";
            var sqldb = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = sqldb.ExecuteDataTable(String.Format(sql, ModeID, roleCode));
            var data = new GridData(dt);
            return Json(data);
        }

        public JsonResult GetSpaceTree(string RootID, string roleCode)
        {
            string sql = @"select S_C_Meun.*,
case when S_C_MenuAuth.AuthType <>'{3}' or S_C_MenuAuth.AuthType is null then 'False' else 'True' end as FullControlAuth,
case when S_C_MenuAuth.AuthType <>'{4}' or S_C_MenuAuth.AuthType is null then 'False' else 'True' end as CurrentFullControlAuth,
case when S_C_MenuAuth.AuthType <>'{2}' or S_C_MenuAuth.AuthType is null then 'False' else 'True' end as ViewAuth 
from S_C_Meun left join S_C_MenuAuth  
on S_C_MenuAuth.MeunID=S_C_Meun.ID
and RoleKey='{1}' where FullID like '{0}%' order by SortIndex";
            sql = String.Format(sql, RootID, roleCode, SpaceAuthType.View.ToString(),
                SpaceAuthType.FullControl.ToString(), SpaceAuthType.CurrentFullControl.ToString());
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure).ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult RemoveAuth(string RoleCode, string SpaceDefineID, string RoleType)
        {
            var spaceDefine = this.entities.Set<S_C_Meun>().FirstOrDefault(d => d.ID == SpaceDefineID);
            if (spaceDefine == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + SpaceDefineID + "】的空间定义对象，无法移除权限");
            spaceDefine.ClearAuth(RoleCode);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SetAuth(string RoleCode, string SpaceDefineID, string RoleType, string authType = "FullControl")
        {
            var spaceDefine = this.entities.Set<S_C_Meun>().FirstOrDefault(d => d.ID == SpaceDefineID);
            if (spaceDefine == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + SpaceDefineID + "】的空间定义对象，无法设置权限");
            if (RoleType == EPC.Logic.RoleType.ProjectRoleType.ToString())
                spaceDefine.SetProjectRole(RoleCode, authType);
            else
                spaceDefine.SetSysRole(RoleCode, authType);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SetDefault(string RoleCode, string SpaceDefineID)
        {
            var spaceDefine = this.entities.Set<S_C_Meun>().FirstOrDefault(d => d.ID == SpaceDefineID);
            if (spaceDefine == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【" + SpaceDefineID + "】的空间定义对象，无法设置权限");
            if (spaceDefine.MeunType != "Root") throw new Formula.Exceptions.BusinessValidationException("只有根菜单才能设置默认");
            var auth = spaceDefine.S_C_MenuAuth.FirstOrDefault(d => d.RoleKey == RoleCode);
            if (auth == null) { throw new Formula.Exceptions.BusinessValidationException("必须拥有权限才能设置默认"); }

            var authRootList = this.entities.Set<S_C_MenuAuth>().Where(d => d.RoleKey == RoleCode && d.S_C_Meun.ModeID == spaceDefine.ModeID
                && d.S_C_Meun.MeunType == "Root").ToList();
            foreach (var item in authRootList)
            {
                item.IsDefault = false.ToString();
            }
            auth.IsDefault = true.ToString();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var sourceNode = this.GetEntityByID<S_C_Meun>(sourceID);
            if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的节点，无法移动");
            if (dragAction.ToLower() == "add")
            {
                var target = this.GetEntityByID<S_C_Meun>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                sourceNode.ParentID = target.ID;
                sourceNode.FullID = target.FullID + "." + sourceNode.ID;
                var maxSortIndex = target.Children.Max(c => c.SortIndex);
                sourceNode.SortIndex = maxSortIndex + 1;
            }
            else if (dragAction.ToLower() == "before")
            {
                var target = this.GetEntityByID<S_C_Meun>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                this.entities.Set<S_C_Meun>().Where(c => c.ParentID == target.ParentID && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 0.001);
                if (target.Parent == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标的上级节点，移动失败");
                sourceNode.ParentID = target.ParentID;
                sourceNode.FullID = target.Parent.FullID + "." + sourceNode.ID;
                sourceNode.SortIndex = target.SortIndex - 0.001;
            }
            else if (dragAction.ToLower() == "after")
            {
                var target = this.GetEntityByID<S_C_Meun>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");
                if (target.Parent == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标的上级节点，移动失败");
                this.entities.Set<S_C_Meun>().Where(c => c.ParentID == target.ParentID && c.SortIndex > target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 0.001);
                sourceNode.ParentID = target.ParentID;
                sourceNode.FullID = target.Parent.FullID + "." + sourceNode.ID;
                sourceNode.SortIndex = target.SortIndex + 0.001;
            }
            this.entities.SaveChanges();
            return Json(sourceNode);
        }

        #endregion
    }
}
