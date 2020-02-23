using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Logic.Domain;
using Project.Logic;
using System.Data;
using Formula.Helper;
using MvcAdapter;
using Formula;
using Config;

namespace Project.Areas.Extend.Controllers
{
    public class MyProjectController : ProjectController
    {
        public ActionResult List()
        {
            var state = ProjectCommoneState.Finish.ToString() + "," +
           ProjectCommoneState.Pause.ToString() + "," +
           ProjectCommoneState.Terminate.ToString();
            //当前用户参与的项目
            var list = (from project in entities.Set<S_I_ProjectInfo>().Where(p => state.IndexOf(p.State) < 0)
                        where (from rbs in entities.Set<S_W_RBS>().Where(p => p.UserID == Formula.FormulaHelper.UserID)
                               select rbs.ProjectInfoID).Contains(project.ID)
                        select project).ToList();
            var prjIDs = string.Join(",", list.Select(c => c.ID.ToString()));

            return View();
        }
        public JsonResult GetMyProjectList(MvcAdapter.QueryBuilder qb)
        {
            qb.SortField = "DefaultType";
            qb.SortOrder = "desc";
            var state = ProjectCommoneState.Finish.ToString() + "," +
            ProjectCommoneState.Pause.ToString() + "," +
            ProjectCommoneState.Terminate.ToString();
            //当前用户参与的项目
            string sql = @"select * from (select S_I_ProjectInfo.*, S_I_UserDefaultProjectInfo.ID as DefaultType from S_I_UserDefaultProjectInfo with(nolock)
                    inner join S_I_ProjectInfo with(nolock) on S_I_UserDefaultProjectInfo.ProjectInfoID=S_I_ProjectInfo.ID
                    where S_I_UserDefaultProjectInfo.UserID='" + CurrentUserInfo.UserID + "' ";
            sql = sql + " union all select *,'2' as DefaultType from S_I_ProjectInfo with(nolock) where ID in (select ProjectInfoID from S_W_RBS with(nolock) where UserID='" + CurrentUserInfo.UserID + "') and State not in ('" + state.Replace(",", "','") + "') and ID not in (select ProjectInfoID from S_I_UserDefaultProjectInfo with(nolock) where UserID='" + CurrentUserInfo.UserID + "')) a ";

            var dt = this.SqlHelper.ExecuteDataTable(sql, qb);
            var json = JsonHelper.ToJson(dt);
            List<S_I_ProjectInfo> list = JsonHelper.ToObject<List<S_I_ProjectInfo>>(json);
            var prjIDs = string.Join(",", list.Select(c => c.ID.ToString()));

            var rbsList = (from rbs in entities.Set<S_W_RBS>().Where(p => p.UserID == Formula.FormulaHelper.UserID && prjIDs.IndexOf(p.ProjectInfoID) >= 0)
                           select rbs).ToList();
            var wbsSql = "select * from S_W_WBS with(nolock) where ProjectInfoID in ('" + prjIDs.Replace(",", "','") + "')";
            var wbsList = this.SqlHelper.ExecuteList<S_W_WBS>(wbsSql);

            var phaseDt = Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Phase);

            List<ProjectInfo> prjList = new List<ProjectInfo>();

            var showAllMajor = true;
            if (!string.IsNullOrEmpty(GetQueryString("ShowAllMajor")))
                showAllMajor = GetQueryString("ShowAllMajor").ToLower() == "true";
            foreach (S_I_ProjectInfo item in list)
            {
                var majorType = WBSNodeType.Major.ToString();
                var majorCodes = string.Join(",", wbsList.Where(d => d.ProjectInfoID == item.ID && d.WBSType == majorType).Select(d => d.MajorCode).Distinct());
                var joinMajorCodes = string.Join(",", rbsList.Where(p => p.ProjectInfoID == item.ID && p.MajorValue != "" && p.MajorValue != null).Select(p => p.MajorValue).Distinct());
                prjList.Add(new ProjectInfo
                {
                    ID = item.ID,
                    Name = item.Name,
                    Code = item.Code,
                    PlanStartDate = item.PlanStartDate == null ? "" : item.PlanStartDate.Value.ToShortDateString(),
                    ChargeUserName = item.ChargeUserName,
                    Phase = GetEnum(phaseDt, item.PhaseValue),
                    Status = EnumBaseHelper.GetEnumDescription(typeof(ProjectCommoneState), item.State),
                    Role = string.Join(",", rbsList.Where(p => p.ProjectInfoID == item.ID).Select(c => c.RoleName.ToString()).Distinct()),
                    //haveMonitorPort = rbsList.Exists(p => p.ProjectInfoID == item.ID && p.WBSType == "Project" && string.IsNullOrEmpty(p.MajorValue)),
                    //haveDesignPort = string.IsNullOrEmpty(majorCodes) ? false : true,
                    Major = showAllMajor ? majorCodes : joinMajorCodes,
                });
            }

            GridData data = new GridData(prjList);
            data.total = qb.TotolCount;
            return Json(data);
        }

        /// <summary>
        /// 获取用户最近进入过的项目信息（默认只记录最近进入的5个项目信息）
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>返回用户最近进入过的5个项目信息</returns>
        public List<S_I_ProjectInfo> GetUserProjectList(string userID)
        {
            string sql = @"select S_I_ProjectInfo.* from S_I_UserDefaultProjectInfo
inner join S_I_ProjectInfo on S_I_UserDefaultProjectInfo.ProjectInfoID=S_I_ProjectInfo.ID
where S_I_UserDefaultProjectInfo.UserID='" + userID + "' order by S_I_UserDefaultProjectInfo.ID desc";
            var list = SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteList<S_I_ProjectInfo>(sql);
            return list;
        }

        public JsonResult GetUserInfo(string ProjectInfoID)
        {
            var list = new List<OBSUserViewModel>();
            var obsUser = entities.Set<S_W_OBSUser>().Where(d => d.ProjectInfoID == ProjectInfoID).ToList();
            var users = obsUser.Select(d => new { UserName = d.UserName, UserID = d.UserID }).Distinct();
            foreach (var user in users)
            {
                var ogUser = FormulaHelper.GetUserInfoByID(user.UserID);
                var roleName = "";
                var userRoles = obsUser.Where(d => d.UserID == user.UserID).Select(d => new { UserName = d.UserName, UserID = d.UserID, RoleCode = d.RoleCode, RoleName = d.RoleName }).Distinct().ToList();
                foreach (var obs in userRoles)
                {
                    roleName += obs.RoleName + ",";
                }
                var obsUserView = new OBSUserViewModel();
                obsUserView.UserID = user.UserID;
                obsUserView.UserName = user.UserName;
                if (ogUser != null)
                {
                    obsUserView.DeptName = ogUser.UserOrgName;
                    obsUserView.Phone = ogUser.MobilePhone;
                }
                obsUserView.OBSName = roleName.TrimEnd(',');
                list.Add(obsUserView);
            }

            return Json(list);
        }
        private string GetEnum(DataTable dt, string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            var rows = dt.Select("value='" + value + "'");
            if (rows.Count() == 0)
                return "";

            return rows[0]["text"].ToString();
        }

        public JsonResult SetTopProject(string ProjectInfoID)
        {
            var projectFO = new Project.Logic.ProjectInfoFO();
            projectFO.SetUserProjectList(CurrentUserInfo.UserID, ProjectInfoID);

            return Json("");
        }
    }

    public class ProjectInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string PlanStartDate { get; set; }
        public string ChargeUserName { get; set; }
        public string Phase { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public int DesignTasksCount { get; set; }//设计任务 
        public int AuditTasksCount { get; set; }//校对审核 
        public int CooperationPlanCount { get; set; }//专业提资 
        public int CoSignQueryCount { get; set; }//会审会签
        public bool haveMonitorPort { get; set; }//是否拥有管理入口
        public bool haveDesignPort { get; set; }//是否拥有设计入口
        public string Major { get; set; }
    }

}
