using Config;
using Formula;
using MvcAdapter;
using Newtonsoft.Json;
using Project.Logic;
using Project.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic;

namespace Project.Areas.Selector.Controllers
{
    public class WBSMultiUserSelectController : BaseController
    {
        //
        // GET: /Selector/WBSMultiUserSelect/

        private DbContext _entities = null;
        protected override System.Data.Entity.DbContext entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = FormulaHelper.GetEntities<ProjectEntities>();
                }
                return _entities; 
            }
        }

        public ActionResult WBSMultiUserSelect()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.entities.Set<S_I_ProjectInfo>().Find(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目对象");
            //string wbsTypes = this.Request["WBSTypes"];
            //var mode = BaseConfigFO.GetMode(projectInfo.ModeCode);
            //if (mode == null) throw new Formula.Exceptions.BusinessException("编码为【" + projectInfo.ModeCode + "】的管理模式定义不存在");
            //var roleDefines  = new List<S_T_WBSStructRole>();
            //foreach (var wbsType in wbsTypes.Split(','))
            //{
            //    var wbsStructNode = mode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == wbsType);
            //    if (wbsStructNode == null) throw new Formula.Exceptions.BusinessException("未找到WBS节点定义，请确认【" + projectInfo.ModeCode + "】管理模式中存在【" + wbsType + "】的WBS定义");
            //    var _roleDefines = wbsStructNode.S_T_WBSStructRole.Where(d => d.RoleCode != ProjectRole.ProjectManager.ToString()).OrderBy(d => d.SortIndex).ToList();
            //    foreach (var _roleDefine in _roleDefines)
            //    {
            //        if (!roleDefines.Any(a => a.RoleCode == _roleDefine.RoleCode && a.RoleName == _roleDefine.RoleName))
            //            roleDefines.Add(_roleDefine);
            //    }
            //}
            var roleCodes = this.GetQueryString("RoleCodes").Split(',');
            ViewBag.RoleDefines =BaseConfigFO.GetRoleDefineList().Where(a=>roleCodes.Contains(a.RoleCode)).OrderBy(a=>a.SortIndex).ToList();
            
            ViewBag.ProjectClass = projectInfo.ProjectClass;
            ViewBag.ProjectLevel = projectInfo.ProjectLevel;
            ViewBag.MajorCodes = GetQueryString("MajorCodes");
            return View();
        }

        public JsonResult GetOrgUserList(string OrgID, QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }

            string sql = GetScopeSql(OrgID, "");

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            qb.Fields = "distinct ID,Code,Name,SortIndex,WorkNo,Sex,Phone,MobilePhone,Email,Address,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX";

            if (ConfigurationManager.AppSettings.AllKeys.Contains("UserAptitude") && ConfigurationManager.AppSettings["UserAptitude"].ToLower().Equals("true"))
            {
                var majorValues = this.GetQueryString("MajorCodes");
                if (!string.IsNullOrEmpty(majorValues))
                    qb.Add("MajorValue", QueryMethod.In, majorValues);
                //启用资质
                qb.Fields += ",MajorValue,RoleCode,MajorName,RoleName";
            }
            if (!string.IsNullOrEmpty(Request["CorpID"]))
            {
                qb.Add("FullID", QueryMethod.InLike, Request["CorpID"]);
            }

            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        public JsonResult GetProjectUserList(string ProjectInfoID, QueryBuilder qb)
        {
            FormulaHelper.GetEntities<BaseConfigEntities>();
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var sql = @"select distinct sysuser.ID,sysuser.Name,RoleName,RoleCode,MajorValue,MajorName,WorkNo,sysuser.DeptName,sysuser.DeptID  from S_W_OBSUser obs
left join " + SQLHelper.CreateSqlHelper(ConnEnum.Base).DbName + @"..S_A_User sysuser on sysuser.ID = obs.UserID
where ProjectInfoID ='" + ProjectInfoID + "' and MajorValue !='' and MajorValue is not null";
            if (ConfigurationManager.AppSettings.AllKeys.Contains("UserAptitude") && ConfigurationManager.AppSettings["UserAptitude"].ToLower().Equals("true"))
            {
                var majorValues = this.GetQueryString("MajorCodes");
                if (!string.IsNullOrEmpty(majorValues))
                    qb.Add("MajorValue", QueryMethod.In, majorValues);
            }
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        private string GetScopeSql(string orgIDs, string roleIDs)
        {
            string field = string.Format("S_A_User.ID,S_A_User.Code,case when '{0}'='EN' then isnull(S_A_User.NameEN,S_A_User.Name) else S_A_User.Name end as Name,S_A_User.SortIndex, WorkNo,Sex,Phone,MobilePhone,Email,Address,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX,FullID", FormulaHelper.GetCurrentLGID());
            var result = string.Empty;
            if (string.IsNullOrWhiteSpace(orgIDs) && string.IsNullOrWhiteSpace(roleIDs))
                result= string.Format("select {0} from S_A_User join S_A__OrgUser on UserID=S_A_User.ID join S_A_Org on S_A_Org.ID=OrgID where S_A_User.IsDeleted<>'1'", field);
            else if (string.IsNullOrWhiteSpace(roleIDs) == true)
            {
                string orgStr = "";
                if (!string.IsNullOrEmpty(orgIDs) && ConfigurationManager.AppSettings.AllKeys.Contains("UserSelectOnlyCurrent") && ConfigurationManager.AppSettings["UserSelectOnlyCurrent"].ToLower().Equals("true"))
                    orgStr = string.Format(" and DeptID in ('{0}') ", orgIDs.Replace(",", "','"));
                else
                    orgStr = string.Format(" and OrgID in('{0}')", orgIDs.Replace(",", "','"));

                result= string.Format("select {1} from S_A_User join S_A__OrgUser on UserID=S_A_User.ID join S_A_Org on S_A_Org.ID=OrgID where S_A_User.IsDeleted<>'1' {0} ", orgStr, field);
            }
            else
            {

                string userIDs = FormulaHelper.GetService<IRoleService>().GetUserIDsInRoles(roleIDs, orgIDs);
                //2018-1-30 剥离项目角色选人功能
                var prjRoleUser = PrjRoleExt.GetRoleUserIDs(roleIDs, orgIDs);
                if (!string.IsNullOrEmpty(prjRoleUser))
                    userIDs = (prjRoleUser + "," + userIDs).Trim(',');

                //Oracle的in查询长度不能超过1000
                var arr = userIDs.Split(',');
                string where = "";
                var i = 0;
                while (i * 1000 < arr.Length)
                {
                    where += string.Format(" or S_A_User.ID in('{0}')", string.Join("','", arr.Skip(i * 1000).Take(1000)));
                    i++;
                }
                where = where.Substring(4);
                result = string.Format("select {1} from S_A_User join S_A__OrgUser on UserID=S_A_User.ID join S_A_Org on S_A_Org.ID=OrgID where S_A_User.IsDeleted<>'1' and {0}", where, field);
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("UserAptitude") && ConfigurationManager.AppSettings["UserAptitude"].ToLower().Equals("true"))
            {
                //启用资质
                var projectInfoID = this.GetQueryString("ProjectInfoID");
                var projectLevel = this.GetQueryString("ProjectLevel");
                var projectClass = this.GetQueryString("ProjectClass");
                string hrBaseName = SQLHelper.CreateSqlHelper(ConnEnum.HR).DbName;
                var aptitudeSql = @"select distinct UserID,UserName,Major MajorValue,Position RoleCode from {0}..S_D_UserAptitude where Major!='Project' {1}
union
select distinct UserID,UserName,Major MajorValue,Position RoleCode from {0}..S_D_UserAptitudeApply where Major!='Project' {2}";
                var userAptString = " and ProjectClass='" + projectClass + "' and AptitudeLevel>=" + projectLevel;
                aptitudeSql = string.Format(aptitudeSql, hrBaseName, userAptString, userAptString + " and ProjectInfoID='" + projectInfoID + "'");
                result = string.Format(@"select UserDt.*,AptDt.MajorValue,AptDt.RoleCode,majorDef.Name MajorName,roleDef.RoleName from ({0}) UserDt 
                left join ({1}) AptDt on AptDt.UserID = UserDt.ID and AptDt.RoleCode!='Project'
                left join {2}..S_D_WBSAttrDefine majorDef on majorDef.Type='Major' and majorDef.Code = AptDt.MajorValue
                left join {2}..S_D_RoleDefine roleDef on roleDef.RoleCode = AptDt.RoleCode
                where aptDt.RoleCode is not null", result, aptitudeSql, SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).DbName);
            }
            return result;
        }

        private string GetAptitudeSql(string aptitude)
        {
            string hrBaseName = SQLHelper.CreateSqlHelper(ConnEnum.HR).DbName;
            string field = string.Format("S_A_User.ID,S_A_User.Code,case when '{0}'='EN' then isnull(S_A_User.NameEN,S_A_User.Name) else S_A_User.Name end as Name,S_A_User.SortIndex, WorkNo,Sex,Phone,MobilePhone,Email,Address,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX", FormulaHelper.GetCurrentLGID());

            var aptiParam = JsonConvert.DeserializeObject<AptitudeParam>(aptitude);
            string whereStr = "";
            if (!string.IsNullOrEmpty(aptiParam.Major))
                whereStr += " and Major='" + aptiParam.Major + "'";
            if (!string.IsNullOrEmpty(aptiParam.ProjectClass))
                whereStr += " and ProjectClass='" + aptiParam.ProjectClass + "'";
            if (!string.IsNullOrEmpty(aptiParam.Position))
                whereStr += " and Position='" + aptiParam.Position + "'";
            if (aptiParam.AptitudeLevel != 0)
                whereStr += " and AptitudeLevel>=" + aptiParam.AptitudeLevel.ToString();
            string whereApplyStr = whereStr;
            if (!string.IsNullOrEmpty(aptiParam.ProjectInfoID))
                whereApplyStr += " and ProjectInfoID='" + aptiParam.ProjectInfoID + "'";

            string sql = string.Format(@"select {0} from S_A_User join (
select distinct UserID from {1}..S_D_UserAptitude where 1=1 {2}
union
select distinct UserID from {1}..S_D_UserAptitudeApply where 1=1 {3})
b on S_A_User.ID=b.UserID", field, hrBaseName, whereStr, whereApplyStr);

            return sql;
        }

        struct AptitudeParam
        {
            public string Major;
            public string ProjectClass;
            public string Position;
            public int AptitudeLevel;
            public string ProjectInfoID;
        }
    }
}
