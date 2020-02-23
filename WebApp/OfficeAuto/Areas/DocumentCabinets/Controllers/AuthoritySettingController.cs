using System.Configuration;
using Config;
using Formula;
using Formula.Helper;
using MvcAdapter;
using OfficeAuto.Logic;
using OfficeAuto.Logic.Domain;
using System;
using System.Linq;
using System.Web.Mvc;
using Config.Logic;

namespace OfficeAuto.Areas.DocumentCabinets.Controllers
{
    public class AuthoritySettingController : BaseController<S_F_DocumentFileAuthority>
    {
        /// <summary>
        /// 取消作废
        /// </summary>
        /// <param name="id">节点ID</param>
        public JsonResult DoValidNode(string id)
        {
            entities.Set<S_F_DocumentInfo>().Where(i => i.ID == id).Update(i => i.IsValid = "T");
            entities.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public override ActionResult Edit()
        {
            ViewBag.RoleType = JsonHelper.ToJson(EnumBaseHelper.GetEnumTable(typeof(enumGroupUserType)));
            var documentInfoId = GetQueryString("DocumentInfoID");
            if (!String.IsNullOrWhiteSpace(documentInfoId))
            {
                ViewBag.DocumentInfoID = documentInfoId;
            }
            var level = GetQueryString("Level");
            if (!String.IsNullOrWhiteSpace(level))
            {
                ViewBag.Level = level;
            }

            return base.Edit();
        }

        public override ActionResult List()
        {
            ViewBag.RoleType = JsonHelper.ToJson(EnumBaseHelper.GetEnumTable(typeof(enumGroupUserType)));
            var documentInfoId = GetQueryString("DocumentInfoID");
            if (!String.IsNullOrWhiteSpace(documentInfoId))
            {
                var entity = entities.Set<S_F_DocumentInfo>().FirstOrDefault(i => i.ID == documentInfoId);
                if (entity != null)
                {
                    ViewBag.IsInherit = entity.IsInherit;
                }
            }
            return base.List();
        }

        public override ActionResult Tree()
        {
            var entity = entities.Set<S_F_DocumentInfo>().FirstOrDefault();
            if (entity == null)
            {
                //初始化，新建顶级节点
                entity = GetEntity<S_F_DocumentInfo>(null);
                entity.FullPathID = entity.ID;
                entity.Name = "资料柜";
                entity.Code = "0001";
                entity.SortIndex = 0;
                entity.Level = 0;
                entity.IsInherit = 0;
                entity.IsValid = "T";
                entity.IsDeleted = 0;
                entities.Set<S_F_DocumentInfo>().Add(entity);
                entities.SaveChanges();
            }
            return View();
        }

        public override JsonResult GetTree()
        {
            var query = entities.Set<S_F_DocumentInfo>().Where(i => i.IsDeleted == 0).ToList();
            var list = query.Select(documentInfo => new
            {
                documentInfo.ID,
                documentInfo.ParentID,
                documentInfo.FullPathID,
                documentInfo.Name,
                documentInfo.Code,
                documentInfo.SortIndex,
                documentInfo.Level,
                documentInfo.IsInherit,
                documentInfo.IsValid,
                IconType = "mini-tree-folder"
            }).ToList();
            return Json(list);
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            var documentInfoId = GetQueryString("DocumentInfoID");
            qb.SortField = "IsParentAuth";
            qb.SortOrder = "desc";
            SQLHelper sqlBase = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            SQLHelper sqlOA = SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);
            var sql = string.Format(@"SELECT *,(
CASE WHEN a.RoleType='User' THEN (SELECT u.Name FROM S_A_User u WHERE u.ID=a.RoleCode)
WHEN a.RoleType='SysRole' OR a.RoleType='OrgRole' THEN (SELECT r.Name FROM S_A_Role r WHERE r.ID = a.RoleCode)
WHEN a.RoleType='Org' THEN (SELECT o.Name FROM S_A_Org o WHERE o.ID = a.RoleCode) END) AS Name,
(CASE WHEN a.AuthType='ReadOnly' THEN 1 ELSE 0 END) AS ReadOnly,
(CASE WHEN a.AuthType='CanWrite' THEN 1 ELSE 0 END) AS CanWrite,
(CASE WHEN a.AuthType='FullControl' THEN 1 ELSE 0 END) AS FullControl 
FROM  {0}.dbo.S_F_DocumentFileAuthority a
WHERE DocumentInfoID='{1}'", sqlOA.DbName, documentInfoId);

            var db = sqlBase.ExecuteDataTable(sql, qb);
            return Json(db);
        }

        public override JsonResult SaveList()
        {
            var documentInfoId = GetQueryString("DocumentInfoID");
            var listData = JsonHelper.ToList(Request["ListData"]);
            var entity = entities.Set<S_F_DocumentInfo>().FirstOrDefault(i => i.ID == documentInfoId);
            if (entity == null)
                new Formula.Exceptions.BusinessException("未找到ID为【" + documentInfoId + "】的文件夹节点！");
            var nodes = this.entities.Set<S_F_DocumentInfo>().Where(a => a.FullPathID.StartsWith(entity.FullPathID)).ToList();
            var ids = nodes.Select(a => a.ID).ToList();
            var authList = this.entities.Set<S_F_DocumentFileAuthority>().Where(a => ids.Contains(a.DocumentInfoID)).ToList();
            var auths = UpdateList<S_F_DocumentFileAuthority>(listData);

            //强制权限继承
            var forceInherit = ConfigurationManager.AppSettings["ForceInherit"];
            if (forceInherit == null || forceInherit.ToLower() != "false")
            {
                foreach (var child in nodes.Where(a => a.ID != documentInfoId))
                {
                    foreach (var item in auths)
                    {
                        var auth = authList.FirstOrDefault(a => a.DocumentInfoID == child.ID &&
                            a.RoleType == item.RoleType && a.RoleCode == item.RoleCode);
                        if (auth == null)
                        {
                            if (item._state == "removed" || item._state == "deleted") continue;
                            auth = new S_F_DocumentFileAuthority();
                            auth.ID = FormulaHelper.CreateGuid();
                            auth.DocumentInfoID = child.ID;
                            auth.AuthType = item.AuthType;
                            auth.RoleType = item.RoleType;
                            auth.RoleCode = item.RoleCode;
                            auth.IsParentAuth = 1;
                            entities.Set<S_F_DocumentFileAuthority>().Add(auth);
                            authList.Add(auth);
                        }
                        else
                        {
                            auth.AuthType = item.AuthType;
                            if (item._state == "removed" || item._state == "deleted")
                                entities.Set<S_F_DocumentFileAuthority>().Remove(auth);
                        }
                    }
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        #region 选择角色

        public JsonResult GetOrgRoleList(QueryBuilder qb)
        {
            string sql = string.Format("select ID,Code,Name,Type,Description from S_A_Role where GroupID='{0}' and Type='OrgRole'", Request["GroupID"]);
            if (string.IsNullOrEmpty(Request["GroupID"]))
                sql = "select ID,Code,Name,Type,Description from S_A_Role where Type='OrgRole'";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return Json(sqlHelper.ExecuteDataTable(sql, (SearchCondition)qb));
        }

        public JsonResult GetSysRoleList(QueryBuilder qb)
        {
            string sql = string.Format("select ID,Code,Name,Type,Description from S_A_Role where GroupID='{0}' and Type='SysRole'", Request["GroupID"]);
            if (string.IsNullOrEmpty(Request["GroupID"]))
                sql = "select ID,Code,Name,Type,Description from S_A_Role where Type='SysRole'";



            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return Json(sqlHelper.ExecuteDataTable(sql, qb));
        }

        #endregion

        #region 继承权限

        public JsonResult DoInherit(string documentInfoId)
        {
            var entity = entities.Set<S_F_DocumentInfo>().FirstOrDefault(i => i.ID == documentInfoId);
            if (entity != null)
            {
                var authList = entities.Set<S_F_DocumentFileAuthority>().Where(i => i.DocumentInfoID == documentInfoId).ToList();
                //删除原有继承的权限设置
                entities.Set<S_F_DocumentFileAuthority>().Delete(i => i.DocumentInfoID == documentInfoId && i.IsParentAuth == 1);
                authList.RemoveWhere(a => a.IsParentAuth == 1);
                //entities.SaveChanges();

                //设置继承的权限
                entities.Set<S_F_DocumentInfo>().Where(i => i.ID == documentInfoId && i.IsInherit == 0).Update(i => i.IsInherit = 1);
                var parentAuthList = entities.Set<S_F_DocumentFileAuthority>().Where(i => i.DocumentInfoID == entity.ParentID);
                foreach (S_F_DocumentFileAuthority authority in parentAuthList)
                {
                    var auth = authList.FirstOrDefault(a => a.RoleType == authority.RoleType && a.RoleCode == authority.RoleCode);
                    if (auth == null)
                    {
                        auth = new S_F_DocumentFileAuthority();
                        auth.ID = FormulaHelper.CreateGuid();
                        auth.DocumentInfoID = entity.ID;
                        auth.AuthType = authority.AuthType;
                        auth.RoleType = authority.RoleType;
                        auth.RoleCode = authority.RoleCode;
                        auth.IsParentAuth = 1;
                        entities.Set<S_F_DocumentFileAuthority>().Add(auth);
                    }
                    else
                    {
                        auth.AuthType = authority.AuthType;
                        auth.IsParentAuth = 1;
                    }
                }
                entities.SaveChanges();

                //强制权限继承
                var forceInherit = ConfigurationManager.AppSettings["ForceInherit"];
                if (forceInherit != null && forceInherit.ToLower() == "true")
                {
                    DocumentCabinetsFO.InheritAuthority(documentInfoId);
                }
            }

            return Json("");
        }

        #endregion
    }
}
