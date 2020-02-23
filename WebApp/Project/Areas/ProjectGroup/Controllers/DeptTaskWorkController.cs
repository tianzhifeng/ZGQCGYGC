using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Reflection;
using System.Linq.Expressions;
using Formula;
using Formula.Helper;
using Formula.DynConditionObject;
using MvcAdapter;
using Config;
using Config.Logic;
using Project.Logic.Domain;
using Project.Logic;


namespace Project.Areas.ProjectGroup.Controllers
{
    public class DeptTaskWorkController : ProjectController
    {
        public ActionResult PlanDetail()
        {
            ViewBag.MajorJson = JsonHelper.ToJson(Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Major));
            return View();
        }

        public ActionResult FinishDetail()
        {
            ViewBag.MajorJson = JsonHelper.ToJson(Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Major));
            return View();
        }

        public ActionResult DeptTaskWork()
        {
            ViewBag.MajorJson = JsonHelper.ToJson(Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Major));
            ViewBag.DefaultYear = DateTime.Now.Year;
            ViewBag.DefaultMonth = DateTime.Now.Month;
            return View();
        }

        public JsonResult GetList(string Year, string Season, string Month)
        {
            IOrgService orgService = FormulaHelper.GetService<IOrgService>();
            var result = new List<Dictionary<string, object>>();
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            string sql = @" select TaskInfo.ChargeDeptID,TaskInfo.ChargeDeptName,AllCount,isnull(AllFinishedCount,0) as AllFinishedCount from (select ChargeDeptID,ChargeDeptName,Count(0) as AllCount from S_W_TaskWork
group by ChargeDeptID,ChargeDeptName) TaskInfo left join (select ChargeDeptID,Count(0) as AllFinishedCount from S_W_TaskWork where State='Finish' 
group by ChargeDeptID) FinishTaskInfo on TaskInfo.ChargeDeptID= FinishTaskInfo.ChargeDeptID ";
            var dt = sqlHelper.ExecuteDataTable(sql);

            string currentPlanSql = @"select count(0)as CurrentTaskInfoCount,ChargeDeptID from S_W_TaskWork where 1=1  {0} group by ChargeDeptID";
            string currentFinishSql = @"select count(0)as CurrentTaskInfoCount,ChargeDeptID from S_W_TaskWork where State='Finish'  {0} group by ChargeDeptID";

            var currentFinishDt = sqlHelper.ExecuteDataTable(String.Format(currentFinishSql, this.getTimewhereStr("Fact", Year, Season, Month)));
            var currentPlanDt = sqlHelper.ExecuteDataTable(String.Format(currentPlanSql, this.getTimewhereStr("Plan", Year, Season, Month)));
            var enumService = FormulaHelper.GetService<IEnumService>();

            var depts = enumService.GetEnumTable("System.ManDept");
            foreach (DataRow org in depts.Rows)
            {
                var deptItem = this.createDefaultItem(org);
                var row = dt.AsEnumerable().FirstOrDefault(d => d["ChargeDeptID"].ToString() == org["Value"].ToString());
                if (row != null)
                {
                    var allCount = Convert.ToDouble(row["AllCount"]);
                    var allFinish = Convert.ToDouble(row["AllFinishedCount"]);
                    var currentPlan = currentPlanDt.Compute("sum(CurrentTaskInfoCount)", " ChargeDeptID='" + org["Value"].ToString() + "'");
                    var currentFinish = currentFinishDt.Compute("sum(CurrentTaskInfoCount)", " ChargeDeptID='" + org["Value"].ToString() + "'");
                    deptItem.SetValue("AllCount", allCount);
                    deptItem.SetValue("AllFinishedCount", allFinish);
                    deptItem.SetValue("AllFinishedPercent", allCount == 0 ? 0 : Math.Round(allFinish / allCount, 2) * 100);
                    double plan = 0; double finish = 0;
                    if (currentPlan != null && currentPlan != DBNull.Value)
                        plan = Convert.ToDouble(currentPlan);
                    if (currentFinish != null && currentFinish != DBNull.Value)
                        finish = Convert.ToDouble(currentFinish);
                    deptItem.SetValue("PeriodCount", plan);
                    deptItem.SetValue("PeriodFinished", finish);
                    deptItem.SetValue("PeriodPercent", plan == 0 ? 0 : Math.Round(finish / plan, 2) * 100);
                }
                result.Add(deptItem);
            }
            //var orgList = orgService.GetChildOrgs(Config.Constant.OrgRootID, OrgType.ManufactureDept);
            //foreach (var org in orgList)
            //{
            //    var deptItem = this.createDefaultItem(org);
            //    var row = dt.AsEnumerable().FirstOrDefault(d => d["ChargeDeptID"].ToString() == org.ID);
            //    if (row != null)
            //    {
            //        var allCount = Convert.ToDouble(row["AllCount"]);
            //        var allFinish = Convert.ToDouble(row["AllFinishedCount"]);                    
            //        var currentPlan = currentPlanDt.Compute("sum(CurrentTaskInfoCount)", " ChargeDeptID='" + org.ID + "'");
            //        var currentFinish = currentFinishDt.Compute("sum(CurrentTaskInfoCount)", " ChargeDeptID='" + org.ID + "'");
            //        deptItem.SetValue("AllCount", allCount);
            //        deptItem.SetValue("AllFinishedCount", allFinish);
            //        deptItem.SetValue("AllFinishedPercent", allCount == 0 ? 0 : Math.Round(allFinish / allCount, 2) * 100);
            //        double plan = 0; double finish = 0;
            //        if (currentPlan != null && currentPlan != DBNull.Value)
            //            plan = Convert.ToDouble(currentPlan);
            //        if (currentFinish != null && currentFinish != DBNull.Value)
            //            finish = Convert.ToDouble(currentFinish);
            //        deptItem.SetValue("PeriodCount", plan);
            //        deptItem.SetValue("PeriodFinished", finish);
            //        deptItem.SetValue("PeriodPercent", plan == 0 ? 0 : Math.Round(finish / plan, 2) * 100);
            //    }
            //    result.Add(deptItem);
            //}
            var dataGrid = new GridData(result);         
            return Json(result);
        }

        public JsonResult GetCurrentPlanDetail(string DeptID, string Year, string Season, string Month)
        {
            string sql = @"select S_W_TaskWork.*,S_I_ProjectInfo.Name as ProjectInfoName,S_I_ProjectInfo.Code as ProjectInfoCode from S_W_TaskWork
left join S_I_ProjectInfo on S_W_TaskWork.ProjectInfoID=S_I_ProjectInfo.ID where S_W_TaskWork.ChargeDeptID='" + DeptID + "' ";
            sql += this.getTimewhereStr("Plan", Year, Season, Month);
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = sqlHelper.ExecuteDataTable(sql);
            var dataGrid = new GridData(dt);
            return Json(dataGrid);
        }

        public JsonResult GetCurrentFinishDetail(string DeptID, string Year, string Season, string Month)
        {
            string sql = @"select S_W_TaskWork.*,S_I_ProjectInfo.Name as ProjectInfoName,S_I_ProjectInfo.Code as ProjectInfoCode from S_W_TaskWork
left join S_I_ProjectInfo on S_W_TaskWork.ProjectInfoID=S_I_ProjectInfo.ID where S_W_TaskWork.ChargeDeptID='" + DeptID + "' and S_W_TaskWork.State='Finish' ";
            sql += this.getTimewhereStr("Fact", Year, Season, Month);
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = sqlHelper.ExecuteDataTable(sql);
            var dataGrid = new GridData(dt);
            return Json(dataGrid);
        }

        private string getTimewhereStr(string type, string year, string season, string month)
        {
            string result = string.Empty;
            var yearField = type + "Year";
            var seasonField = type + "Season";
            var monthField = type + "Month";
            if (!String.IsNullOrEmpty(year))
                result += " and " + yearField + "=" + year;
            if (!String.IsNullOrEmpty(season))
                result += " and " + seasonField + "=" + season;
            if (!String.IsNullOrEmpty(month))
                result += " and " + monthField + "=" + month;
            return result;
        }

        private Specifications getTimeSpec(string type, string year, string season, string month)
        {
            var yearField = type + "Year";
            var seasonField = type + "Season";
            var monthField = type + "Month";
            var spec = new Specifications();
            if (!String.IsNullOrEmpty(year))
                spec.AndAlso(yearField, year, QueryMethod.Equal);
            if (!String.IsNullOrEmpty(season))
                spec.AndAlso(seasonField, season, QueryMethod.Equal);
            if (!String.IsNullOrEmpty(month))
                spec.AndAlso(monthField, month, QueryMethod.Equal);
            return spec;
        }

        private Dictionary<string, object> createDefaultItem(DataRow org)
        {
            var deptItem = new Dictionary<string, object>();
            deptItem.SetValue("ID", org["Value"].ToString());
            deptItem.SetValue("Name", org["Text"].ToString());
            deptItem.SetValue("AllCount", "0");
            deptItem.SetValue("AllFinishedCount", "0");
            deptItem.SetValue("AllFinishedPercent", "0");
            deptItem.SetValue("PeriodCount", "0");
            deptItem.SetValue("PeriodFinished", "0");
            deptItem.SetValue("PeriodPercent", "0");
            return deptItem;
        }
    }
}
