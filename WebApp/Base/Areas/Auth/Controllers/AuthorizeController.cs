using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula;
using Config;
using System.Data;
using MvcAdapter;
using Base.Logic;
using Base.Logic.BusinessFacade;

namespace Base.Areas.Auth.Controllers
{
    public class AuthorizeController : BaseController
    {
        #region 展示

        public JsonResult GetOrgTree()
        {
            string sql = "select * from S_A_Org order by SortIndex";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            SearchCondition cnd = new SearchCondition();
            if (!string.IsNullOrEmpty(Request["CorpID"]))
                cnd.Add("FullID", QueryMethod.InLike, Request["CorpID"]);

            var dt = sqlHelper.ExecuteDataTable(sql, cnd);
            return Json(dt);
        }

        public JsonResult GetRoleList(QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CorpID"]))
                qb.Add("CorpID", QueryMethod.In, Request["CorpID"]);

            var result = entities.Set<S_A_Role>().WhereToGridData(qb);
            return Json(result);
        }

        public JsonResult GetUserList(QueryBuilder qb)
        {
            string nodeFullID = Request["NodeFullID"];
            if (string.IsNullOrEmpty(nodeFullID))
            {
                string corpID = Request["CorpID"];
                if (!string.IsNullOrEmpty(corpID))
                {
                    var result = entities.Set<S_A_User>().Where(c => c.IsDeleted != "1" && c.CorpID == corpID).WhereToGridData(qb);
                    return Json(result);
                }
                else
                {
                    var result = entities.Set<S_A_User>().Where(c => c.IsDeleted != "1").WhereToGridData(qb);
                    return Json(result);
                }
            }

            string getUserListSQL = "select b.ID,b.Code,b.Name,b.WorkNo,b.DeptName,b.SortIndex from(select * from S_A__OrgUser where S_A__OrgUser.OrgID='{0}') a join S_A_User b on a.UserID=b.ID And b.IsDeleted<>'1' order by SortIndex";
            var nodeID = nodeFullID.Split('.').Last();
            string sql = string.Format(getUserListSQL, nodeID);
            SQLHelper sqlHelp = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var data = sqlHelp.ExecuteGridData(sql, qb);
            return Json(data);
        }

        #region 组织授权展示

        public JsonResult GetResTreeByOrgID(string orgID)
        {
            DataTable dt = GetOrgAuthTree(Config.Constant.MenuRooID, orgID);
            return Json(dt);
        }

        public JsonResult GetRuleTreeByOrgID(string orgID)
        {
            DataTable dt = GetOrgAuthTree(Config.Constant.RuleRootID, orgID);
            return Json(dt);
        }

        private DataTable GetOrgAuthTree(string rootID, string orgID)
        {
            string sql = @"
select a.ID,a.ParentID,a.FullID,a.Code,a.Name,a.Type,a.SortIndex
,case when ResID is null then 'false' else 'true' end Checked
from (select * from S_A_Res where (FullID like '{0}%' {3}) {2}) a
left join 
(select ResID from S_A_Res join S_A__OrgRes on (FullID like '{0}%' {3}) and ResID=ID and OrgID='{1}') b
on a.ID=b.ResID order by SortIndex
";
            sql = string.Format(sql, rootID, orgID, Auth_PowerDiscrete, string.Format(" or Code = '{0}'", "Mobile"));
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            //子公司权限
            DealCompanyAuth(dt);
            return dt;
        }

        #endregion

        #region 角色授权展示

        public JsonResult GetResTreeByRoleID(string roleID)
        {
            DataTable dt = GetRuleAuthTree(Config.Constant.MenuRooID, roleID);
            return Json(dt);
        }

        public JsonResult GetRuleTreeByRoleID(string roleID)
        {
            DataTable dt = GetRuleAuthTree(Config.Constant.RuleRootID, roleID);
            return Json(dt);
        }

        private DataTable GetRuleAuthTree(string rootID, string roleID)
        {
            string sql = @"
select a.ID,a.ParentID,a.FullID,a.Code,a.Name,a.Type,a.SortIndex
,case when ResID is null then 'false' else 'true' end Checked
from (select * from S_A_Res where (FullID like '{0}%' {3}) {2}) a
left join 
(select ResID from S_A_Res join S_A__RoleRes on (FullID like '{0}%' {3}) and ResID=ID and RoleID='{1}') b
on a.ID=b.ResID order by SortIndex
";
            sql = string.Format(sql, rootID, roleID, Auth_PowerDiscrete, string.Format(" or Code = '{0}'", "Mobile"));
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            //子公司权限
            DealCompanyAuth(dt);
            return dt;
        }

        #endregion

        #region 用户授权展示

        public JsonResult GetResTreeByUserID(string userID)
        {
            DataTable dt = GetUserAuthTree(Config.Constant.MenuRooID, userID);


            return Json(dt);
        }

        public JsonResult GetRuleTreeByUserID(string userID)
        {
            DataTable dt = GetUserAuthTree(Config.Constant.RuleRootID, userID);
            return Json(dt);
        }

        private DataTable GetUserAuthTree(string rootID, string userID)
        {
            string getUserResTreeSql = @"
select a.ID,a.ParentID,a.FullID,a.Code,a.Name,a.Type,a.SortIndex
,b.Src
,case when c.DenyAuth is null then NULL else c.DenyAuth end DenyAuth
 from( select * from S_A_Res where (S_A_Res.FullID like '{0}%' {3}) {2}) a
left join (
select ID,MAX(Src) as Src from (
--组织权限
select S_A_Res.ID,S_A_Org.Name+'(组织)'  as Src from S_A__OrgUser 
join S_A__OrgRes on S_A__OrgRes.OrgID=S_A__OrgUser.OrgID 
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
where (S_A_Res.FullID like '{0}%' {3}) and UserID='{1}' 
union
--系统角色权限
select S_A_Res.ID,S_A_Role.Name+'(角色)'  as Src from S_A__RoleUser 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__RoleUser.RoleID 
join S_A_Res on S_A_Res.ID=ResID 
join S_A_Role on S_A_Role.ID=S_A__RoleUser.RoleID
where (S_A_Res.FullID like '{0}%' {3}) and UserID='{1}'
union
--继承自组织的角色权限
select S_A_Res.ID,S_A_Role.Name+'(角色)' as Src from S_A__OrgUser 
join S_A__OrgRole on S_A__OrgRole.OrgID=S_A__OrgUser.OrgID
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
join S_A_Role on S_A_Role.ID=S_A__OrgRole.RoleID
join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
where (S_A_Res.FullID like '{0}%' {3}) and UserID='{1}' --and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%'+S_A_User.PrjID+'%')
union
--组织角色权限
select S_A_Res.ID,S_A_Role.Name+'(角色)' as Src from S_A__OrgRoleUser
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRoleUser.RoleID
join S_A_Res on S_A_Res.ID=S_A__RoleRes.ResID
join S_A_Org on S_A_Org.ID=S_A__OrgRoleUser.OrgID
join S_A_Role on S_A_Role.ID=S_A__OrgRoleUser.RoleID
join S_A_User on S_A_User.ID=S_A__OrgRoleUser.UserID
where (S_A_Res.FullID like '{0}%' {3}) and UserID='{1}' and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%'+S_A_User.PrjID+'%')
union
--用户权限
select S_A_Res.ID,S_A_User.Name+'(用户)' as Src from S_A__UserRes
join S_A_Res on S_A_Res.ID=ResID
join S_A_User on S_A_User.ID=S_A__UserRes.UserID
where (S_A_Res.FullID like '{0}%' {3}) and UserID='{1}' 

) dt1 group by ID

) b on a.ID=b.ID

left join (select ResID,DenyAuth from S_A__UserRes join S_A_Res on (S_A_Res.FullID like '{0}%' {3}) and S_A_Res.ID=ResID where UserID='{1}' ) c on c.ResID=a.ID
order by SortIndex
";
            string sql = string.Format(getUserResTreeSql
                , rootID
                , userID
                , Auth_PowerDiscrete
                , string.Format(" or S_A_Res.Code = '{0}'", "Mobile")
                );

            if (Config.Constant.IsOracleDb)
                sql = sql.Replace("+", "||");

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());

            foreach (DataRow row in dt.Rows)
            {
                if (row["Src"].ToString() != "")
                    row["Name"] = string.Format("<span>{0}<a href='#' style='text-decoration: underline;color:blue;' onclick='ViewDetail(\"{2}\",\"{3}\");'>√</a></span>", row["Name"], row["Src"], userID, row["ID"]);

                string denyAuth = row["DenyAuth"].ToString();
                if (denyAuth == "NULL" || denyAuth == "")
                    row["Name"] = "<img src = '/CommonWebResource/Theme/Default/MiniUI/icons/unchecked.gif' id = " + "'" + row["ID"] + "'" + " />" + row["Name"].ToString();
                else if (denyAuth == "0")
                    row["Name"] = "<img src = '/CommonWebResource/Theme/Default/MiniUI/icons/checked.gif' id = " + row["ID"] + " />" + row["Name"].ToString();
                else if (denyAuth == "1")
                    row["Name"] = "<img src = '/CommonWebResource/Theme/Default/MiniUI/icons/DenyAuth.gif' id = " + row["ID"] + " />" + row["Name"].ToString();


            }

            //子公司权限
            DealCompanyAuth(dt);

            return dt;
        }

        public JsonResult GetUserAuthDetail(string userID, string resID)
        {
            string sql = @"
--组织权限
select S_A_Res.ID,S_A_Org.Name+'(组织)' as Src from S_A__OrgUser 
join S_A__OrgRes on S_A__OrgRes.OrgID=S_A__OrgUser.OrgID 
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
where S_A_Res.ID = '{0}' and UserID='{1}' 
union
--系统角色权限
select S_A_Res.ID,S_A_Role.Name+'(角色)' as Src from S_A__RoleUser 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__RoleUser.RoleID 
join S_A_Res on S_A_Res.ID=ResID 
join S_A_Role on S_A_Role.ID=S_A__RoleUser.RoleID
where S_A_Res.ID = '{0}' and UserID='{1}'
union
--继承自组织的角色权限
select S_A_Res.ID,S_A_Role.Name+'(角色)' as Src from S_A__OrgUser 
join S_A__OrgRole on S_A__OrgRole.OrgID=S_A__OrgUser.OrgID
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A_Res on S_A_Res.ID=ResID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
join S_A_Role on S_A_Role.ID=S_A__OrgRole.RoleID
join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
where S_A_Res.ID = '{0}' and UserID='{1}' --and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%'+S_A_User.PrjID+'%')
union
--组织角色权限
select S_A_Res.ID,S_A_Role.Name+'(角色)' as Src from S_A__OrgRoleUser
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRoleUser.RoleID
join S_A_Res on S_A_Res.ID=S_A__RoleRes.ResID
join S_A_Org on S_A_Org.ID=S_A__OrgRoleUser.OrgID
join S_A_Role on S_A_Role.ID=S_A__OrgRoleUser.RoleID
join S_A_User on S_A_User.ID=S_A__OrgRoleUser.UserID
where S_A_Res.ID = '{0}' and UserID='{1}' and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%'+S_A_User.PrjID+'%')
union
--用户权限
select S_A_Res.ID,S_A_User.Name+'(用户)' as Src from S_A__UserRes
join S_A_Res on S_A_Res.ID=ResID
join S_A_User on S_A_User.ID=S_A__UserRes.UserID
where S_A_Res.ID = '{0}' and UserID='{1}' 
";
            sql = string.Format(sql, resID, userID);

            if (Config.Constant.IsOracleDb)
                sql = sql.Replace("+", "||");

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            return Json(dt);
        }

        #endregion

        #endregion

        #region 授权

        public JsonResult SaveOrgRes(string orgID, string resIDs)
        {

            var s_A_Res = entities.Set<S_A_Res>().Where(x => x.Code == "Mobile" && x.ParentID == "").FirstOrDefault();
            if (s_A_Res != null)
            {
                entities.Set<S_A__OrgRes>().Delete(c => c.OrgID == orgID && (c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID) || c.S_A_Res.FullID.StartsWith(s_A_Res.ID)));
            }
            else
            {
                entities.Set<S_A__OrgRes>().Delete(c => c.OrgID == orgID && (c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID)));
            }

            foreach (string item in resIDs.Split(','))
            {
                if (item == "") continue;
                S_A__OrgRes orgRes = new S_A__OrgRes();
                orgRes.OrgID = orgID;
                orgRes.ResID = item;
                entities.Set<S_A__OrgRes>().Add(orgRes);
            }
            //记录安全审计日志
            string orgName = entities.Set<S_A_Org>().SingleOrDefault(c => c.ID == orgID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => resIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("组织授权（菜单）", orgName, resNames);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveOrgRule(string orgID, string ruleIDs)
        {
            var s_A_Res = entities.Set<S_A_Res>().Where(x => x.Code == "Mobile" && x.ParentID == "").FirstOrDefault();
            if (s_A_Res != null)
            {
                entities.Set<S_A__OrgRes>().Delete(c => c.OrgID == orgID && (c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID) || c.S_A_Res.FullID.StartsWith(s_A_Res.ID)));
            }
            else
            {
                entities.Set<S_A__OrgRes>().Delete(c => c.OrgID == orgID && (c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID)));
            }

            foreach (string item in ruleIDs.Split(','))
            {
                if (item == "") continue;
                S_A__OrgRes orgRes = new S_A__OrgRes();
                orgRes.OrgID = orgID;
                orgRes.ResID = item;
                entities.Set<S_A__OrgRes>().Add(orgRes);
            }
            //记录安全审计日志
            string orgName = entities.Set<S_A_Org>().SingleOrDefault(c => c.ID == orgID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => ruleIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("组织授权（对象）", orgName, resNames);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveRoleRes(string roleID, string resIDs)
        {
            var s_A_Res = entities.Set<S_A_Res>().Where(x => x.Code == "Mobile" && x.ParentID == "").FirstOrDefault();
            if (s_A_Res != null)
            {
                entities.Set<S_A__RoleRes>().Delete(c => c.RoleID == roleID && (c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID) || c.S_A_Res.FullID.StartsWith(s_A_Res.ID)));
            }
            else
            {
                entities.Set<S_A__RoleRes>().Delete(c => c.RoleID == roleID && (c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID)));
            }

            foreach (string item in resIDs.Split(','))
            {
                if (item == "") continue;
                S_A__RoleRes roleRes = new S_A__RoleRes();
                roleRes.RoleID = roleID;
                roleRes.ResID = item;
                entities.Set<S_A__RoleRes>().Add(roleRes);
            }
            //记录安全审计日志
            string roleName = entities.Set<S_A_Role>().SingleOrDefault(c => c.ID == roleID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => resIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("角色授权（菜单）", roleName, resNames);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveRoleRule(string roleID, string ruleIDs)
        {
            entities.Set<S_A__RoleRes>().Delete(c => c.RoleID == roleID && c.S_A_Res.FullID.StartsWith(Config.Constant.RuleRootID));

            foreach (string item in ruleIDs.Split(','))
            {
                if (item == "") continue;
                S_A__RoleRes roleRes = new S_A__RoleRes();
                roleRes.RoleID = roleID;
                roleRes.ResID = item;
                entities.Set<S_A__RoleRes>().Add(roleRes);
            }

            //记录安全审计日志
            string roleName = entities.Set<S_A_Role>().SingleOrDefault(c => c.ID == roleID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => ruleIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("角色授权（对象）", roleName, resNames);

            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveUserRes(string userID, string checkedIDs, string denyAuthIDs)
        {
            var s_A_Res = entities.Set<S_A_Res>().Where(x => x.Code == "Mobile" && x.ParentID == "").FirstOrDefault();
            if (s_A_Res != null)
            {
                entities.Set<S_A__UserRes>().Delete(c => c.UserID == userID && (c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID) || c.S_A_Res.FullID.StartsWith(s_A_Res.ID)));
            }
            else
            {
                entities.Set<S_A__UserRes>().Delete(c => c.UserID == userID && (c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID)));
            }

            checkedIDs = checkedIDs.Trim('"');
            denyAuthIDs = denyAuthIDs.Trim('"');

            foreach (string item in checkedIDs.Split(','))
            {
                if (item == "") continue;
                S_A__UserRes userRes = new S_A__UserRes();
                userRes.UserID = userID;
                userRes.ResID = item;
                userRes.DenyAuth = "0";
                entities.Set<S_A__UserRes>().Add(userRes);
            }
            foreach (string item in denyAuthIDs.Split(','))
            {
                if ("" == item) continue;
                S_A__UserRes userRes = new S_A__UserRes();
                userRes.UserID = userID;
                userRes.ResID = item;
                userRes.DenyAuth = "1";
                entities.Set<S_A__UserRes>().Add(userRes);
            }
            //记录安全审计日志
            string UserName = entities.Set<S_A_User>().SingleOrDefault(c => c.ID == userID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => checkedIDs.Contains(c.ID)).Select(c => c.Name));
            string denyNames = string.Join(",", entities.Set<S_A_Res>().Where(c => denyAuthIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("用户授权（菜单）", UserName, resNames);
            AuthFO.Log("用户授权（菜单-否定）", UserName, denyNames);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveUserRule(string userID, string checkedIDs, string denyAuthIDs)
        {
            var s_A_Res = entities.Set<S_A_Res>().Where(x => x.Code == "Mobile" && x.ParentID == "").FirstOrDefault();
            if (s_A_Res != null)
            {
                entities.Set<S_A__UserRes>().Delete(c => c.UserID == userID && (c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID) || c.S_A_Res.FullID.StartsWith(s_A_Res.ID)));
            }
            else
            {
                entities.Set<S_A__UserRes>().Delete(c => c.UserID == userID && (c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID)));
            }

            checkedIDs = checkedIDs.Trim('"');
            denyAuthIDs = denyAuthIDs.Trim('"');

            foreach (string item in checkedIDs.Split(','))
            {
                if (item == "") continue;
                S_A__UserRes userRes = new S_A__UserRes();
                userRes.UserID = userID;
                userRes.ResID = item;
                userRes.DenyAuth = "0";
                entities.Set<S_A__UserRes>().Add(userRes);
            }
            foreach (string item in denyAuthIDs.Split(','))
            {
                if (item == "") continue;
                S_A__UserRes userRes = new S_A__UserRes();
                userRes.UserID = userID;
                userRes.ResID = item;
                userRes.DenyAuth = "1";
                entities.Set<S_A__UserRes>().Add(userRes);
            }

            //记录安全审计日志
            string UserName = entities.Set<S_A_User>().SingleOrDefault(c => c.ID == userID).Name;
            string resNames = string.Join(",", entities.Set<S_A_Res>().Where(c => checkedIDs.Contains(c.ID)).Select(c => c.Name));
            string denyNames = string.Join(",", entities.Set<S_A_Res>().Where(c => denyAuthIDs.Contains(c.ID)).Select(c => c.Name));
            AuthFO.Log("用户授权（对象）", UserName, resNames);
            AuthFO.Log("用户授权（对象-否定）", UserName, denyNames);

            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 查询

        #region 按人员

        public JsonResult GetResTreeView(string userID)
        {
            return GetResTreeByUserID(userID);
        }

        public JsonResult GetRuleTreeView(string userID)
        {
            return GetRuleTreeByUserID(userID);
        }

        #endregion

        #region 按菜单入口

        public JsonResult GetResTree()
        {
            //string sql = string.Format("select * from S_A_Res where FullID like '{0}%'", Config.Constant.MenuRooID);

            //if (!string.IsNullOrEmpty(Request["CorpID"]))
            //{
            //    var authLevel = entities.Set<S_A_AuthLevel>().SingleOrDefault(c => c.UserID == FormulaHelper.UserID);
            //    var menuIds = string.Join("','", authLevel.MenuRootFullID.Split(',', '.').Distinct());
            //    sql += string.Format(" and ID in('{0}')", menuIds);
            //}

            //SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            //var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            //return Json(dt);           
            string sql = string.Format("select ID,ParentID,FullID,Code,Name,Type,SortIndex from S_A_Res where (FullID like '{0}%')", Config.Constant.MenuRooID);
            if (System.Configuration.ConfigurationManager.AppSettings["Auth_PowerDiscrete"] == "True")
            {
                sql += string.Format(" and FullID not like '{0}%'", Config.Constant.SystemMenuFullID);
            }
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(sql);
            DealCompanyAuth(dt);
            return Json(dt);
        }

        public JsonResult GetRuleTree()
        {
            //string sql = string.Format("select * from S_A_Res where FullID like '{0}%'", Config.Constant.RuleRootID);


            //if (!string.IsNullOrEmpty(Request["CorpID"]))
            //{
            //    var authLevel = entities.Set<S_A_AuthLevel>().SingleOrDefault(c => c.UserID == FormulaHelper.UserID);
            //    var menuIds = string.Join("','", authLevel.RuleRootFullID.Split(',', '.').Distinct());
            //    sql += string.Format(" and ID in('{0}')", menuIds);
            //}


            //SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            //var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            //return Json(dt);

            string sql = string.Format("select ID,ParentID,FullID,Code,Name,Type,SortIndex from S_A_Res where (FullID like '{0}%')", Config.Constant.RuleRootID);
            if (System.Configuration.ConfigurationManager.AppSettings["Auth_PowerDiscrete"] == "True")
            {
                sql += string.Format(" and FullID not like '{0}%'", Config.Constant.SystemMenuFullID);
            }
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(sql);
            DealCompanyAuth(dt);
            return Json(dt);
        }


        public JsonResult GetResUserList(string resID, QueryBuilder qb)
        {



            string sql = @"
select ID,WorkNo,Code,Name,Sex,Phone,MobilePhone,RTX,DeptName,CorpID from (
--直接授权给用户的
select S_A_User.* from S_A__UserRes
join S_A_User on S_A_User.ID=UserID
where ResID='{0}'
--系统角色的
union
select S_A_User.* from S_A__RoleRes
join S_A__RoleUser on S_A__RoleUser.RoleID=S_A__RoleRes.RoleID
join S_A_User on S_A_User.ID=S_A__RoleUser.UserID
where S_A__RoleRes.ResID='{0}'
--组织的
union
select S_A_User.* from S_A__OrgRes
join S_A__OrgUser on S_A__OrgUser.OrgID=S_A__OrgRes.OrgID
join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
where S_A__OrgRes.ResID='{0}'
--继承自组织的组织角色的
union
select S_A_User.* from S_A__RoleRes
join S_A__OrgRole on S_A__OrgRole.RoleID=S_A__RoleRes.RoleID
join S_A__OrgUser on S_A__OrgUser.OrgID=S_A__OrgRole.OrgID
join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
join S_A_Org on S_A_Org.ID=S_A__OrgUser.OrgID
where S_A__RoleRes.ResID='{0}' --and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%' + S_A_User.PrjID+'%')
--组织角色权限
union
select S_A_User.* from S_A__RoleRes
join S_A__OrgRoleUser on S_A__OrgRoleUser.RoleID=S_A__RoleRes.RoleID
join S_A_User on S_A_User.ID=S_A__OrgRoleUser.UserID
join S_A_Org on S_A_Org.ID=S_A__OrgRoleUser.OrgID
where S_A__RoleRes.ResID='{0}' --and (S_A_Org.FullID like '%'+S_A_User.DeptID+'%' or S_A_Org.FullID like '%' + S_A_User.PrjID+'%')
) table1

left join S_A__UserRes on table1.ID=S_A__UserRes.UserID and S_A__UserRes.ResID='{0}'
where S_A__UserRes.DenyAuth is null or S_A__UserRes.DenyAuth='0'
";

            sql = string.Format(sql, resID);

            if (!string.IsNullOrEmpty(Request["CorpID"]))
                sql = string.Format("select * from ({0}) dt where CorpID in('{1}')", sql, Request["CorpID"].Replace(",", "','"));

            if (Config.Constant.IsOracleDb)
                sql = sql.Replace("+", "||");

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var result = sqlHelper.ExecuteGridData(sql, qb);
            return Json(result);
        }


        #endregion

        #endregion


        private string _Auth_PowerDiscrete = "";
        private string Auth_PowerDiscrete
        {
            get
            {
                if (_Auth_PowerDiscrete == "")
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["Auth_PowerDiscrete"] == "True")
                    {
                        _Auth_PowerDiscrete = string.Format(" and S_A_Res.FullID not like '{0}%'", Config.Constant.SystemMenuFullID);
                    }
                    else
                    {
                        _Auth_PowerDiscrete = " ";
                    }
                }
                return _Auth_PowerDiscrete;
            }
        }


        public void DealCompanyAuth(DataTable dt)
        {
            //去掉分级授权菜单
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i]["FullID"].ToString().StartsWith(Config.Constant.SystemMenuCompanyFullID))
                    dt.Rows.RemoveAt(i);
            }
            //如果开启了子公司权限
            //子公司权限
            if (System.Configuration.ConfigurationManager.AppSettings["CorpAuthEnabled"] == "True")
            {
                var authFO = new AuthFO();
                if (authFO.IsSepAdmin(FormulaHelper.UserID))
                {
                    var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

                    //DealCompanyAuth多个地方公用，因此有不同取值
                    var corpID = Request["CorpID"];
                    if (string.IsNullOrEmpty(Request["CorpID"]))
                        corpID = FormulaHelper.GetUserInfo().AdminCompanyID;

                    var _dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_A_AuthCompany where CompanyID='{0}' ", corpID));

                    var ids = _dt.AsEnumerable().Select(c => c["ResID"].ToString());

                    var result = dt.AsEnumerable().Where(c => ids.Contains(c["ID"].ToString()));
                    if (result.Count() > 0)
                    {
                        //dt = result.CopyToDataTable();
                        for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        {
                            if (ids.Contains(dt.Rows[i]["ID"].ToString()) == false)
                                dt.Rows.RemoveAt(i);
                        }
                    }
                    else
                        dt.Rows.Clear();
                }
            }
        }
    }
}
