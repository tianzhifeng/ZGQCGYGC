using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using MvcAdapter;
using Formula.Helper;
using Formula;
using Base.Logic.BusinessFacade;
using System.Web.Security;
using Config;
using System.Data;
using System.Text;
using System.IO;
using System.Web.WebPages;

namespace Base.Areas.Auth.Controllers
{
    public class OrgUserController : BaseController<S_A_Org, S_A__OrgUser, S_A_User>
    {
        public override JsonResult GetTree()
        {
            string sql = "select * from S_A_Org where IsDeleted<>'1' order by SortIndex";

            SearchCondition cnd = new SearchCondition();
            if (!string.IsNullOrEmpty(Request["CorpID"]))
                cnd.Add("FullID", QueryMethod.InLike, Request["CorpID"]);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(sql, cnd);
            return Json(dt);
        }

        public override JsonResult AppendRelation()
        {
            var userIds = GetValues(Request["RelationData"], "ID");
            var orgfullid = Request["NodeFullID"];
            var cdept = entities.Set<S_A_Org>().FirstOrDefault(o => o.FullID == orgfullid);
            for (int i = 0; i < orgfullid.Split('.').Length; i++)
            {
                if (cdept.Type == "Post")
                {
                    cdept = entities.Set<S_A_Org>().FirstOrDefault(o => o.ID == cdept.ParentID);
                }
                else
                {
                    break;
                }
            }
            if (cdept == null)
            {
                throw new Exception("请选择部门");
            }
            var users = entities.Set<S_A_User>().Where(c => userIds.Contains(c.ID)).ToList();
            users.Update(c =>
            {
                c.IsDeleted = "0";
                c.ModifyTime = DateTime.Now;
                if (c.DeptID.IsEmpty())
                {
                    c.DeptName = cdept.Name;
                    c.DeptID = cdept.ID;
                    c.DeptFullID = cdept.FullID;
                }
            });
            var result = base.AppendRelation();


            return result;
        }

        #region 重载 保存关联数据（用户）

        public override JsonResult SaveRelationData()
        {
            var user = UpdateEntity<S_A_User>();
            string nodeFullID = Request["NodeFullID"];

            if (entities.Set<S_A_User>().Count(c => c.Code == user.Code && c.ID != user.ID) > 0)
                throw new Exception("用户账号不能重复");

            if (string.IsNullOrEmpty(user.DeptID) && !string.IsNullOrEmpty(nodeFullID))
            {
                string[] orgIDs = nodeFullID.Split('.');
                var orgs = entities.Set<S_A_Org>().Where(c => orgIDs.Contains(c.ID)).ToDictionary<S_A_Org, string>(d => d.ID);
                for (var i = orgIDs.Length; i > 0; i--)
                {
                    var itemOrg = orgs[orgIDs[i - 1]];
                    if ((itemOrg.Type ?? "none") != "Post")
                    {
                        user.DeptID = itemOrg.ID;
                        user.DeptFullID = itemOrg.FullID;
                        user.DeptName = itemOrg.Name;
                        break;
                    }
                }
                for (var j = orgIDs.Length; j > 0; j--)
                {
                    var itemOrg = orgs[orgIDs[j - 1]];
                    if ((itemOrg.Type ?? "none").IndexOf("Company") > -1)
                    {
                        user.CorpID = itemOrg.ID;
                        user.CorpName = itemOrg.Name;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Code.ToLower(), "SHA1");
            }
            user.ModifyTime = DateTime.Now;

            return base.JsonSaveRelationData<S_A_Org, S_A__OrgUser, S_A_User>(user);
        }

        #endregion

        #region 重载 获取关联数据列表（用户）

        public override JsonResult GetRelationList(QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }

            string nodeFullID = Request["NodeFullID"];
            string nodeID = Request["NodeFullID"];
            if (string.IsNullOrEmpty(nodeID))
                return Json("");
            nodeID = nodeID.Split('.').Last();

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select ID,Code,Name,WorkNo,DeptName,CorpName,SortIndex from S_A_User join S_A__OrgUser on ID=UserID where IsDeleted='0' and OrgID='{0}'", nodeID);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);

            string userIDs = string.Join("','", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());

            //部门名称
            sql = "select UserID,Name as OrgName from S_A__OrgUser join S_A_Org on S_A_Org.ID=OrgID where UserID in('{0}') and S_A_Org.Type like '%Dept'";
            sql = string.Format(sql, userIDs);
            var dtOrgName = sqlHelper.ExecuteDataTable(sql).AsEnumerable();

            //系统角色名称
            sql = "select UserID,Name as RoleName from S_A__RoleUser join S_A_Role on ID=RoleID where UserID in('{0}')";
            sql = string.Format(sql, userIDs);
            var dtRoleName = sqlHelper.ExecuteDataTable(sql).AsEnumerable();

            //组织角色名称
            sql = @"
select  a.UserID
,case when sum(Src)>2 then '★' when sum(Src)=2 then '☆'  else '' end +a.RoleName+'('+MAX(OrgName)+')' as RoleName 
from
(
select UserID,S_A_Role.Name as RoleName,2 as Src,S_A_Org.Name as OrgName from S_A__OrgRole
join S_A__OrgUser on S_A__OrgRole.OrgID=S_A__OrgUser.OrgID
join S_A_Org on S_A_Org.ID=S_A__OrgRole.OrgID
join S_A_Role on S_A_Role.ID=S_A__OrgRole.RoleID
where UserID in('{0}')
union
select UserID, S_A_Role.Name as RoleName,1 as Src,S_A_Org.Name as OrgName from S_A__OrgRoleUser
join S_A_Org on S_A_Org.ID=OrgID
join S_A_Role on S_A_Role.ID=RoleID
where UserID in('{0}')
) a 
group by a.UserID, a.RoleName,a.OrgName
";
            if (Config.Constant.IsOracleDb)
                sql = sql.Replace("+", "||");

            sql = string.Format(sql, userIDs);
            var dtOrgRole = sqlHelper.ExecuteDataTable(sql).AsEnumerable();

            dt.Columns.Add("DeptNames");
            dt.Columns.Add("UserRoleName");
            dt.Columns.Add("OrgRoleName");

            foreach (DataRow row in dt.Rows)
            {
                string userID = row["ID"].ToString();
                row["DeptNames"] = string.Join(",", dtOrgName.Where(c => c["UserID"].ToString() == userID).Select(c => c["OrgName"]).ToArray());
                row["UserRoleName"] = string.Join(",", dtRoleName.Where(c => c["UserID"].ToString() == userID).Select(c => c["RoleName"]).ToArray());
                row["OrgRoleName"] = string.Join(",", dtOrgRole.Where(c => c["UserID"].ToString() == userID).Select(c => c["RoleName"]).ToArray());
            }

            GridData gridData = new GridData(dt);
            gridData.total = qb.TotolCount;
            return Json(gridData);
        }

        #endregion

        #region 重载 删除关联（用户）

        public override JsonResult DeleteRelation()
        {
            #region 设置当前部门

            string nodeFullID = Request["NodeFullID"];
            string relationData = Request["RelationData"];
            var userids = GetValues(relationData, "ID");
            var users = entities.Set<S_A_User>().Where(c => userids.Contains(c.ID)).ToArray();
            foreach (var user in users)
            {
                if (user.DeptFullID != null && user.DeptFullID.StartsWith(nodeFullID))
                {
                    user.DeptID = "";
                    user.DeptName = "";
                    user.DeptFullID = "";
                }
            }
            #endregion

            return base.DeleteRelation();
        }

        #endregion

        #region 作废部门

        public ActionResult AbortOrg(string fullID)
        {
            AuthFO authBF = FormulaHelper.CreateFO<AuthFO>();
            authBF.AbortOrg(fullID);
            return Json("");
        }
        #endregion

        #region 恢复部门
        public ActionResult RecoverOrg(string nodeID)
        {
            AuthFO authBF = FormulaHelper.CreateFO<AuthFO>();
            authBF.RecoverOrg(nodeID);
            return Json("");
        }

        #endregion

        #region 删除部门

        public override JsonResult DeleteNode()
        {
            AuthFO authBF = FormulaHelper.CreateFO<AuthFO>();
            authBF.DeleteOrg(Request["FullID"]);
            return Json("");
        }

        #endregion

        #region 组织的角色

        public JsonResult GetOrgRole(string nodeFullID)
        {
            return base.JsonGetRelationAll<S_A_Org, S_A__OrgRole, S_A_Role>(nodeFullID);
        }

        public JsonResult SetOrgRole(string nodeFullID, string relationData, string fullRelation)
        {
            if (!string.IsNullOrEmpty(nodeFullID))
            {
                var org = this.entities.Set<S_A_Org>().FirstOrDefault(a => a.FullID == nodeFullID);
                if (org != null)
                {
                    var date = DateTime.Now;
                    org.S_A__OrgUser.Update(a => a.S_A_User.ModifyTime = date);
                }
            }
            return base.JsonSetRelation<S_A_Org, S_A__OrgRole, S_A_Role>(nodeFullID, relationData, fullRelation);
        }

        #endregion

        #region 组织的权限

        public JsonResult GetOrgList(string nodeFullID)
        {
            return Json(entities.Set<S_A_Org>().Where(c => c.FullID.StartsWith(nodeFullID)));
        }

        public JsonResult GetOrgRes(string nodeFullID)
        {
            return base.JsonGetRelationAll<S_A_Org, S_A__OrgRes, S_A_Res>(nodeFullID);
        }

        public JsonResult SetOrgRes(string nodeFullID, string relationData, string fullRelation)
        {
            string nodeID = nodeFullID.Split('.').Last();
            entities.Set<S_A__OrgRes>().Delete(c => c.OrgID == nodeID && c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID));
            entities.SaveChanges();
            return base.JsonAppendRelation<S_A_Org, S_A__OrgRes, S_A_Res>(nodeFullID, relationData, "False");
        }

        public JsonResult SetOrgRule(string nodeFullID, string relationData, string fullRelation)
        {
            string nodeID = nodeFullID.Split('.').Last();
            entities.Set<S_A__OrgRes>().Delete(c => c.OrgID == nodeID && c.S_A_Res.FullID.StartsWith(Config.Constant.RuleRootID));
            entities.SaveChanges();
            return base.JsonAppendRelation<S_A_Org, S_A__OrgRes, S_A_Res>(nodeFullID, relationData, "False");
        }

        #endregion

        #region 人员的角色

        public JsonResult GetUserRole(string nodeFullID)
        {
            return base.JsonGetRelationAll<S_A_User, S_A__RoleUser, S_A_Role>(nodeFullID);
        }

        public JsonResult SetUserRole(string nodeFullID, string relationData, string fullRelation)
        {
            return base.JsonSetRelation<S_A_User, S_A__RoleUser, S_A_Role>(nodeFullID, relationData, fullRelation);
        }

        #endregion

        #region 人员的权限

        public JsonResult GetUserRes(string nodeFullID)
        {
            AuthFO authBF = FormulaHelper.CreateFO<AuthFO>();

            return Json(authBF.GetResByUserID(nodeFullID));
        }

        #endregion

        #region 作废的组织查看

        public ActionResult AbortTree()
        {
            return View();
        }

        public JsonResult GetAbortTree(string rootFullID)
        {
            if (rootFullID == null)
                rootFullID = "";

            var orgs = entities.Set<S_A_Org>().Where(c => c.FullID.StartsWith(rootFullID)).OrderBy(c => c.SortIndex);

            foreach (var org in orgs)
            {
                if (org.IsDeleted == "1")
                {
                    var ss = (org.DeleteTime == null) ? "" : org.DeleteTime.Value.ToShortDateString();
                    org.Name = "<b title ='作废日期：" + ss + "'>" + org.Name + "<font color='red' >(已作废)</font></b>";
                }
            }
            return Json(orgs, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region 设置人员的组织角色

        public JsonResult SetOrgRoleUser(string OrgID, string RoleIDs, string UserIDs)
        {
            var userIds = UserIDs.Split(',');
            foreach (var userID in userIds)
            {
                entities.Set<S_A__OrgRoleUser>().Delete(c => c.OrgID == OrgID && c.UserID == userID);

                foreach (var roleID in RoleIDs.Split(','))
                {
                    if (roleID == "") continue;
                    S_A__OrgRoleUser orgRoleUser = new S_A__OrgRoleUser();
                    orgRoleUser.UserID = userID;
                    orgRoleUser.OrgID = OrgID;
                    orgRoleUser.RoleID = roleID;
                    entities.Set<S_A__OrgRoleUser>().Add(orgRoleUser);

                }
            }
            var users = this.entities.Set<S_A_User>().Where(a => userIds.Contains(a.ID)).ToList();
            users.Update(a => a.ModifyTime = DateTime.Now);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetRoleByUserAndOrg(string orgID, string userID)
        {
            var list = entities.Set<S_A__OrgRoleUser>().Where(c => c.OrgID == orgID && c.UserID == userID).Select(c => new { ID = c.RoleID });
            return Json(list);
        }

        #endregion

        #region 设置人员系统角色

        public JsonResult SetRoleUser(string RoleIDs, string UserIDs)
        {
            var userIds = UserIDs.Split(',');
            foreach (var userID in userIds)
            {
                entities.Set<S_A__RoleUser>().Delete(c => c.UserID == userID);

                foreach (var roleID in RoleIDs.Split(','))
                {
                    if (roleID == "") continue;
                    var roleUser = new S_A__RoleUser();
                    roleUser.UserID = userID;     
                    roleUser.RoleID = roleID;
                    entities.Set<S_A__RoleUser>().Add(roleUser);
                }
            }
            var users = this.entities.Set<S_A_User>().Where(a => userIds.Contains(a.ID)).ToList();
            users.Update(a => a.ModifyTime = DateTime.Now);
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 设置当前部门

        public JsonResult SetCurrentOrg(string userIDs, string deptID)
        {
            var org = entities.Set<S_A_Org>().SingleOrDefault(c => c.ID == deptID);
            string companyID = "";
            string companyName = "";

            //逆序判断类型是否为集团/公司
            string[] orgIDs = org.FullID.Split('.');
            var orgs = entities.Set<S_A_Org>().Where(c => orgIDs.Contains(c.ID)).ToDictionary<S_A_Org, string>(d => d.ID);

            for (var i = orgIDs.Length; i > 0; i--)
            {
                if ((orgs[orgIDs[i - 1]].Type ?? "none").IndexOf("Company") > -1)
                {
                    companyID = orgs[orgIDs[i - 1]].ID;
                    companyName = orgs[orgIDs[i - 1]].Name;
                    break;
                }
            }

            //移除人员和部门的关系，后面再加入
            var orgUser = entities.Set<S_A__OrgUser>().Where(c => userIDs.Contains(c.UserID) && orgIDs.Contains(c.OrgID));
            foreach (var item in orgUser)
                entities.Set<S_A__OrgUser>().Remove(item);

            foreach (string userID in userIDs.Split(','))
            {
                var user = entities.Set<S_A_User>().SingleOrDefault(c => c.ID == userID);
                if (org != null)
                {
                    user.DeptID = org.ID;
                    user.DeptName = org.Name;
                    user.DeptFullID = org.FullID;
                }
                else
                {
                    user.DeptID = "";
                    user.DeptName = "";
                    user.DeptFullID = "";
                }

                user.CorpID = companyID;
                user.CorpName = companyName;
                user.ModifyTime = DateTime.Now;
                //用户放入当前部门
                foreach (var orgItem in orgs)
                    entities.Set<S_A__OrgUser>().Add(new S_A__OrgUser { UserID = userID, OrgID = orgItem.Key });
            }
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 导出Sql

        //菜单导出Sql
        public FileResult SqlFile(string orgID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("USE [{0}] \n", sqlHelper.DbName);

            string sql = string.Format("select * from S_A_Org where ID in('{0}')", orgID);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            var fullID = dt.Rows[0]["FullID"].ToString();
            var parentID = dt.Rows[0]["ParentID"].ToString();
            if (!string.IsNullOrEmpty(parentID))
            {
                sb.AppendFormat(@"
--父组织不存在，不能导入
if not exists (select ID from S_A_Org where ID='{0}')
begin
    select '父组织不存在，不能导入'
	return
end
", parentID);
            }

            sb.AppendLine();
            dt = sqlHelper.ExecuteDataTable("select * from S_A_Org where FullID like '" + fullID + "%'");
            sb.Append(SQLHelper.CreateUpdateSql("S_A_Org", dt));

            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(sb.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", "OrgSQL.sql");
        }


        #endregion

        public JsonResult AuthTrans(string srcUserID, string targetUserID)
        {
            var src = entities.Set<S_A__UserRes>().Where(c => c.UserID == srcUserID).ToList();
            var target = entities.Set<S_A__UserRes>().Where(c => c.UserID == targetUserID).ToList();
            foreach (var item in src)
            {
                if (target.Count(c => c.ResID == item.ResID) == 0)
                {
                    entities.Set<S_A__UserRes>().Add(new S_A__UserRes() { UserID = targetUserID, ResID = item.ResID, DenyAuth = "0" });
                }
            }
            entities.SaveChanges();
            var targetUser = FormulaHelper.GetUserInfoByID(targetUserID);
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            string sql = string.Format("update S_WF_InsTaskExec set ExecUserID='{0}',ExecUserName='{2}' where ExecTime is null and ExecUserID='{1}'", targetUserID, srcUserID, targetUser.UserName);
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }
    }
}
