using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Project.Logic.Domain;
using MvcAdapter;
using Formula;
using Project.Logic;
using Formula.Helper;
using Config;


namespace Project.Areas.ProjectGroup.Controllers
{
    public class PersonnelLoadController : ProjectController
    {
        //
        // GET: /ProjectGroup/PersonnelLoad/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetTree(QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string sql = string.Format(@"select ID,Name,ParentID,Type,FullID from S_A_Org where
               Type!='{0}' order by SortIndex ",
                OrgType.Post.ToString());
            return Json(sqlHelper.ExecuteDataTable(sql), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserList(QueryBuilder qb)
        {
            var dt = new DataTable();
            SQLHelper ProjectsqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            SQLHelper BasesqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            string OrgID = Request["OrgID"];
            if (!string.IsNullOrEmpty(OrgID))
            {
                string usersql = @"select S_A_User.ID,S_A_User.Name as UserName,S_A_User.WorkNo,S_A_User.SortIndex, 
S_A_Org.ID as DeptID,S_A_Org.Name as DeptName,S_A_Org.FullID as DeptFullID, 
S_A_Org.Code as DeptCode from S_A_User
left join S_A_Org on S_A_Org.ID=S_A_User.DeptID";

                if (OrgID != Config.Constant.OrgRootID)
                {
                    usersql += " where S_A_User.DeptID='" + OrgID + "'";
                }

                string projectsql = @"select * from (select  distinct ProjectInfoID,UserID from S_W_OBSUser) as OBSUser
left join S_I_ProjectInfo as ProjectInfo on OBSUser.ProjectInfoID=ProjectInfo.ID";

                var userdt = BasesqlHelper.ExecuteDataTable(usersql, qb);
                var projectdt = ProjectsqlHelper.ExecuteDataTable(projectsql);
                foreach (DataRow item in userdt.Rows)
                {
                    var listItem = new Dictionary<string, object>();
                    var UserID = GetVal(item["ID"]);
                    var UserName = GetVal(item["UserName"]);
                    var WorkNo = GetVal(item["WorkNo"]);
                    var DeptID = GetVal(item["DeptID"]);
                    var DeptCode = GetVal(item["DeptCode"]);
                    var DeptName = GetVal(item["DeptName"]);
                    var DeptFullID = GetVal(item["DeptFullID"]);
                    listItem["UserID"] = UserID;
                    listItem["UserName"] = UserName;
                    listItem["WorkNo"] = WorkNo;
                    listItem["DeptID"] = DeptID;
                    listItem["DeptCode"] = DeptCode;
                    listItem["DeptName"] = DeptName;
                    listItem["DeptFullID"] = DeptFullID;
                    listItem["Plan"] = GetProjectNum(projectdt, UserID, ProjectCommoneState.Plan);
                    listItem["Execute"] = GetProjectNum(projectdt, UserID, ProjectCommoneState.Execute);
                    listItem["Pause"] = GetProjectNum(projectdt, UserID, ProjectCommoneState.Pause);
                    listItem["Terminate"] = GetProjectNum(projectdt, UserID, ProjectCommoneState.Terminate);
                    listItem["Finish"] = GetProjectNum(projectdt, UserID, ProjectCommoneState.Finish);
                    result.Add(listItem);
                }
            }
            var gridData = new GridData(result);
            gridData.total = qb.TotolCount;
            return Json(gridData);
        }

        private string GetVal(object obj)
        {
            try
            {
                return obj.ToString();
            }
            catch (Formula.Exceptions.BusinessException)
            {
                return "";
            }
        }

        private string GetProjectNum(DataTable projectdt, string userID, ProjectCommoneState state)
        {
            var num = projectdt.Compute("count(ID)", "UserID='" + userID + "' AND State='" + state.ToString() + "'");
            if (num != null)
                return num.ToString();
            else
                return "0";
        }

        public JsonResult GetProjectList(QueryBuilder qb)
        {
            var userID = GetQueryString("UserID");
            var state = GetQueryString("State");
            var dept = GetQueryString("DeptID");
            string parentsql = @"select distinct S_I_ProjectInfo.ID as ID, 
S_I_ProjectInfo.Code as ProjectInfoCode, S_I_ProjectInfo.Name as ProjectInfoName,
S_I_ProjectInfo.State as State, '' as TaskNoticeID,'' as TaskNoticeTmplCode,'' as MajorName,'' as RoleName
from  S_W_OBSUser 
left join S_I_ProjectInfo on S_W_OBSUser.ProjectInfoID=S_I_ProjectInfo.ID
where UserID='" + userID+"' ";
            if (!String.IsNullOrEmpty(dept))
                parentsql += " and  S_W_OBSUser.DeptID ='" + dept + "'";
            if(!String.IsNullOrEmpty(state))
            {
                parentsql += " and  S_I_ProjectInfo.State in ('"+state.Replace(",","','")+"')";
            }
            var wherestr = qb.GetWhereString(false);
            if (!string.IsNullOrEmpty(wherestr))
                parentsql += " and " + wherestr;
            var maindt = this.SqlHelper.ExecuteDataTable(parentsql, qb);
            var obsUsersql = @"SELECT  S_W_OBSUser.*  FROM S_W_OBSUser
left join S_I_ProjectInfo on S_W_OBSUser.ProjectInfoID=S_I_ProjectInfo.ID
where  UserID='" + userID + "' ";
            //S_I_ProjectInfo.State='" + state + "' and
            if (!String.IsNullOrEmpty(dept))
                obsUsersql += " and  S_W_OBSUser.DeptID ='" + dept + "'";
            if (!String.IsNullOrEmpty(state))
            {
                obsUsersql += " and  S_I_ProjectInfo.State in ('" + state.Replace(",", "','") + "')";
            }
            var obsUserdt = this.SqlHelper.ExecuteDataTable(obsUsersql);
            var taskNoticeList = FormulaHelper.GetEntities<ProjectEntities>().Set<T_CP_TaskNotice>().ToList();

            foreach (DataRow item in maindt.Rows)
            {
                var MajorNames = string.Empty;
                var RoleNames = string.Empty;
                var projectInfoId = item["ID"].ToString();
                var rows = obsUserdt.Select("ProjectInfoID='" + projectInfoId + "'");
                foreach (DataRow obsuser in rows)
                {
                    var MajorName = GetVal(obsuser["MajorName"]);
                    var RoleName = GetVal(obsuser["RoleName"]);
                    if (!string.IsNullOrEmpty(MajorName) && !MajorNames.Contains(MajorName))
                        MajorNames += MajorName + ",";
                    if (!string.IsNullOrEmpty(RoleName) && !RoleNames.Contains(RoleName))
                        RoleNames += RoleName + ",";
                }
                item["MajorName"] = MajorNames.TrimEnd(',');
                item["RoleName"] = RoleNames.TrimEnd(',');
                var taskNotice = taskNoticeList.FirstOrDefault(s => s.ProjectInfoID.Contains(projectInfoId));
                if (taskNotice != null)
                {
                    item["TaskNoticeID"] = taskNotice.ID;
                    item["TaskNoticeTmplCode"] = taskNotice.TmplCode;
                }
            }

            GridData gridData = new GridData(maindt);
            gridData.total = qb.TotolCount;
            return Json(gridData);


        }

        public JsonResult GetActivity(QueryBuilder qb)
        {
            var userID = GetQueryString("UserID");
            var projectinfoID = GetQueryString("ProjectInfoID");
            qb.Add("OwnerUserID", QueryMethod.Equal, userID);
            qb.Add("ProjectInfoID", QueryMethod.Equal, projectinfoID);
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            return Json(entities.Set<S_W_Activity>().Where((SearchCondition)qb));
        }

        #region 个人任务

        public ActionResult MainTab()
        {
            ViewBag.ProjectInfoID = GetQueryString("ProjectInfoID");
            ViewBag.UserID = GetQueryString("UserID");
            return View();
        }

        public JsonResult GetPersonTaskList(QueryBuilder qb, string ProjectInfoID, string UserID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null)
            {
                throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
            }
            qb.PageSize = 0;
            qb.DefaultSort = false;
            var enumType = FlowTraceDefineNodeType.FlowNode.ToString();
            var defineList = projectInfo.ProjectMode.S_T_FlowTraceDefine.Where(a => a.Type == enumType).OrderBy(a => a.SortIndex).ToList();
            if (defineList.Count == 0) return Json("");
            string sqlIDs = string.Empty;
            int n = 0;
            foreach (var item in defineList)
            {
                sqlIDs += " select id from {0} where {1}={2}";
                sqlIDs = string.Format(sqlIDs, item.TableName, "ProjectInfoID", "'" + projectInfo.ID + "'");
                if (n != defineList.Count - 1)
                    sqlIDs += " union";
                n++;
            }
            n = 0;
            var dt = this.SqlHelper.ExecuteDataTable(sqlIDs);
            string whereStrIDs = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                whereStrIDs += "'" + row["id"].ToString() + "'";
                if (n != dt.Rows.Count - 1)
                    whereStrIDs += ",";
                n++;
            }
            if (!string.IsNullOrEmpty(whereStrIDs))
                whereStrIDs = " and FormInstanceID in (" + whereStrIDs + ")";
            //任务
            string sql = @"select S_WF_InsTaskExec.ID as ID
,S_WF_InsTaskExec.ID as TaskExecID
,S_WF_InsTask.ID as TaskID
,S_WF_InsTaskExec.Type as TaskExecType
,S_WF_InsTask.InsDefStepID as StepID
,S_WF_InsTask.InsFlowID as FlowID
,S_WF_InsTask.Type as TaskType
,S_WF_InsTask.Urgency
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserIDs
,SendTaskUserNames
,S_WF_InsTaskExec.TaskUserID
,S_WF_InsTaskExec.TaskUserName
,S_WF_InsTask.Status as Status
,S_WF_InsTask.CreateTime as CreateTime
,S_WF_InsTaskExec.ExecTime
,FormInstanceID
,FlowName
,FlowCategory
,FlowSubCategory
,S_WF_InsDefFlow.TableName
,S_WF_InsFlow.CreateUserName FlowStartUserName
,(case when S_WF_InsTaskExec.ExecTime is not null then 'T' else 'F' end) IsFinish
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
from S_WF_InsTaskExec
join S_WF_InsTask on ExecUserID='{0}' 
and S_WF_InsTask.Type in('Normal','Inital') and S_WF_InsTask.ID=InsTaskID
join S_WF_InsFlow on S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
join S_WF_InsDefFlow on InsDefFlowID=S_WF_InsDefFlow.ID
join S_WF_InsDefStep on InsDefStepID = S_WF_InsDefStep.ID
left join S_WF_DefSubForm on SubFormID=S_WF_DefSubForm.ID
where 1=1 {1}";
            sql = string.Format(sql, UserID, whereStrIDs);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            var taskDt = sqlHelper.ExecuteDataTable(sql, qb, false);
            taskDt.Columns.Add("SortIndex", typeof(int));
            foreach (DataRow row in taskDt.Rows)
            {
                var tablename = row["TableName"].ToString();
                var sortIndex = 0;
                var def = defineList.FirstOrDefault(a => a.TableName == tablename);
                if (def != null) sortIndex = def.SortIndex;
                row["SortIndex"] = sortIndex;
            }
            return Json(taskDt);
        }

        #endregion
    }
}
