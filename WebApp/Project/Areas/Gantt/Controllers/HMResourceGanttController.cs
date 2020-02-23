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
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;

namespace Project.Areas.Gantt.Controllers
{
    public class HMResourceGanttController : ProjectController
    {
        public ActionResult ResourceGantt()
        {
            var orgService = FormulaHelper.GetService<IOrgService>();
            var depts =orgService.GetChildOrgs(Config.Constant.OrgRootID, OrgType.ManufactureDept);
            ViewBag.Depts = JsonHelper.ToJson(depts);
            return View();
        }
        [ValidateInput(false)]
        public JsonResult GetList(QueryBuilder qb)
        {
            string deptID = this.Request["DeptID"];
            if (String.IsNullOrEmpty(deptID)) deptID = this.CurrentUserInfo.UserOrgID;
            string sql = @"select distinct S_A_User.ID,S_A_User.Name, S_A_User.SortIndex from S_A_User left join S_A__OrgUser on S_A_User.ID=S_A__OrgUser.UserID
left join S_A_Org on S_A_Org.ID= S_A__OrgUser.OrgID where S_A_User.IsDeleted=0 and S_A_Org.ID='{0}'";
            sql += " Order By SortIndex ";
            sql = String.Format(sql, deptID);
            string projectSQL = @"select distinct S_I_ProjectInfo.ID,S_I_ProjectInfo.Name,S_I_ProjectInfo.PlanStartDate,S_I_ProjectInfo.PlanFinishDate,RBS.UserID,
RBS.UserName from S_I_ProjectInfo inner join (select * from S_W_RBS where UserDeptID='{0}') RBS on S_I_ProjectInfo.ID=RBS.ProjectInfoID 
where S_I_ProjectInfo.State <> '{1}' and  S_I_ProjectInfo.PlanStartDate is not null and S_I_ProjectInfo.PlanFinishDate is not null";
            projectSQL = String.Format(projectSQL, deptID, ProjectCommoneState.Finish.ToString());
            var userDt = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(sql, qb);
            var projectDt = SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteDataTable(projectSQL);
            var userList = new List<Dictionary<string, object>>();
            foreach (DataRow user in userDt.Rows)
            {
                var userDic = new Dictionary<string, object>();
                userDic.SetValue("UID", user["ID"]);
                userDic.SetValue("Name", user["Name"]);
                var projects = projectDt.Select("UserID='"+user["ID"]+"'");
                var projectDicList = new List<Dictionary<string, object>>();
                foreach (var project in projects)
                {
                    var node = new Dictionary<string, object>();
                    node.SetValue("UID", project["ID"]);
                    node.SetValue("Name", project["Name"]);
                    node.SetValue("Start", project["PlanStartDate"]);
                    node.SetValue("Finish", project["PlanFinishDate"]);
                    //node.SetValue("PercentComplete", 0);
                    //node.SetValue("Duration", 2 * 24 * 3600);
                    projectDicList.Add(node);
                }
                userDic.SetValue("Tasks", projectDicList);
                userList.Add(userDic);
            }
            var data = new Dictionary<string, object>();
            data.SetValue("Total", qb.TotolCount);
            data.SetValue("CurrentPage", qb.PageIndex);
            data.SetValue("data", userList);
            return Json(data);
        }
    }
}
