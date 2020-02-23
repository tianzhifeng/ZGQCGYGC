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
    public class ProjectTaskWorkController : ProjectController
    {

        public ActionResult ProjectPlanDetail()
        {
            ViewBag.MajorJson = JsonHelper.ToJson(Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Major));
            return View();
        }

        public ActionResult ProjectFinishDetail()
        {
            ViewBag.MajorJson = JsonHelper.ToJson(Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Major));
            return View();
        }

        public ActionResult ProjectTaskWork()
        {
            ViewBag.MajorJson = JsonHelper.ToJson(Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Major));
            ViewBag.DefaultYear = DateTime.Now.Year;
            ViewBag.DefaultMonth = DateTime.Now.Month;
            return View();
        }

        public JsonResult GetList(QueryBuilder qb, string Year, string Season, string Month)
        {
            var data = new Dictionary<string, object>();
            var result = new List<Dictionary<string, object>>();
            var sqlHelper =  SQLHelper.CreateSqlHelper(ConnEnum.Project);
            string allTaskSql = @"select S_I_ProjectInfo.ID,S_I_ProjectInfo.Name,S_I_ProjectInfo.Code,S_I_ProjectInfo.ChargeUserName,isnull(AllCount,0) as AllCount,isnull(AllFinishedCount,0) as  AllFinishedCount 
from (select count(0) as AllCount,ProjectInfoID from S_W_TaskWork group by ProjectInfoID) TaskInfo left join S_I_ProjectInfo on  TaskInfo.ProjectInfoID= S_I_ProjectInfo.ID 
left join (select  count(0) as AllFinishedCount,ProjectInfoID from S_W_TaskWork where State='Finish' group by ProjectInfoID) FinishTaskInfo on S_I_ProjectInfo.ID=FinishTaskInfo.ProjectInfoID ";
            var dt = sqlHelper.ExecuteDataTable(allTaskSql, qb);

            string currentPlanSql = @"select count(0)as CurrentTaskInfoCount,ProjectInfoID from S_W_TaskWork
where 1=1  {0} group by ProjectInfoID";
            string currentFinishSql = @"select count(0)as CurrentTaskInfoCount,ProjectInfoID from S_W_TaskWork
where State='Finish'  {0} group by ProjectInfoID";
            var currentFinishDt = sqlHelper.ExecuteDataTable(String.Format(currentFinishSql, this.getTimewhereStr("Fact", Year, Season, Month)));
            var currentPlanDt = sqlHelper.ExecuteDataTable(String.Format(currentPlanSql,this.getTimewhereStr("Plan",Year,Season,Month)));

            var json = JsonHelper.ToJson(dt);
            result = JsonHelper.ToList(json);
            foreach (var item in result)
            {
                var allCount = String.IsNullOrEmpty(item.GetValue("AllCount")) ? 0 : Convert.ToDouble(item.GetValue("AllCount"));
                var allFinish = String.IsNullOrEmpty(item.GetValue("AllFinishedCount")) ? 0 : Convert.ToDouble(item.GetValue("AllFinishedCount"));
                item.SetValue("AllFinishedPercent", allCount == 0 ? 0 : Math.Round(allFinish / allCount, 2) * 100);
                var currentPlan = currentPlanDt.Compute("sum(CurrentTaskInfoCount)", " ProjectInfoID='" + item.GetValue("ID") + "'");
                var currentFinish = currentFinishDt.Compute("sum(CurrentTaskInfoCount)", " ProjectInfoID='" + item.GetValue("ID") + "'");
                double plan = 0; double finish = 0;
                if (currentPlan != null && currentPlan != DBNull.Value)
                    plan = Convert.ToDouble(currentPlan);
                if (currentFinish != null && currentFinish != DBNull.Value)
                    finish = Convert.ToDouble(currentFinish);
                item.SetValue("PeriodCount", plan);
                item.SetValue("PeriodFinished", finish);
                item.SetValue("PeriodPercent", plan == 0 ? 0 : Math.Round(finish / plan, 2) * 100);
            }
            var dataGrid = new GridData(result);
            return Json(result);
        }

        public JsonResult GetProjectDetail(string ProjectInfoID, string Year, string Season, string Month)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            var majors = projectInfo.GetMajors();
            var allMajorTaskList = this.entities.Set<S_W_TaskWork>().Where(d => d.ProjectInfoID == ProjectInfoID).
                 GroupBy(d => new { MajorValue = d.MajorValue }).Select(d => new { MajorValue = d.Key.MajorValue, AllTaskCount = d.Count() });
            var allFinishTaskList = this.entities.Set<S_W_TaskWork>().Where(d => d.ProjectInfoID == ProjectInfoID&&d.State=="Finish").
                  GroupBy(d => new { MajorValue = d.MajorValue }).Select(d => new { MajorValue = d.Key.MajorValue, AllTaskCount = d.Count() });
            var spec = this.getTimeSpec("Plan", Year, Season, Month);
            var express = spec.GetExpression<S_W_TaskWork>();
            var planTaskList =this.entities.Set<S_W_TaskWork>().Where(d => d.ProjectInfoID == ProjectInfoID).Where(express).GroupBy(d => new { MajorValue = d.MajorValue }).Select(d => new { MajorValue = d.Key.MajorValue, AllTaskCount = d.Count() });
            
            spec = this.getTimeSpec("Fact", Year, Season, Month);
            express = spec.GetExpression<S_W_TaskWork>();
            var finishTaskList = this.entities.Set<S_W_TaskWork>().Where(d => d.ProjectInfoID == ProjectInfoID&&d.State=="Finish").Where(express).GroupBy(d => new { MajorValue = d.MajorValue }).Select(d => new { MajorValue = d.Key.MajorValue, AllTaskCount = d.Count() });

            foreach (var major in majors)
            {
                var majorValue = major.GetValue("Value");
                var allTask = allMajorTaskList.FirstOrDefault(d => d.MajorValue == majorValue);
                var allFinishTask = allFinishTaskList.FirstOrDefault(d => d.MajorValue == majorValue);
                var planTask = planTaskList.FirstOrDefault(d => d.MajorValue == majorValue);
                var finishTask = finishTaskList.FirstOrDefault(d => d.MajorValue == majorValue);
                major.SetValue("AllCount", allTask == null ? 0 : allTask.AllTaskCount);
                major.SetValue("AllFinishedCount", allFinishTask == null ? 0 : allFinishTask.AllTaskCount);
                if (allTask == null || allTask.AllTaskCount == 0 || allFinishTask == null)
                    major.SetValue("AllFinishedPercent", 0);
                else
                    major.SetValue("AllFinishedPercent", Math.Round(Convert.ToDouble(allFinishTask.AllTaskCount) / Convert.ToDouble(allTask.AllTaskCount), 2) * 100);
                major.SetValue("PeriodCount", planTask == null ? 0 : planTask.AllTaskCount);
                major.SetValue("PeriodFinished", finishTask == null ? 0 : finishTask.AllTaskCount);

                if (planTask == null || planTask.AllTaskCount == 0 || finishTask == null)
                    major.SetValue("PeriodPercent", 0);
                else
                    major.SetValue("PeriodPercent", Math.Round(Convert.ToDouble(finishTask.AllTaskCount) / Convert.ToDouble(planTask.AllTaskCount), 2) * 100);
            }
            var data = new GridData(majors);
            return Json(data);
        }

        public JsonResult GetFinishDetail(string ProjectInfoID, string Year, string Season, string Month)
        {
            var spec = this.getTimeSpec("Fact", Year, Season, Month);
            var express = spec.GetExpression<S_W_TaskWork>();
            var list = this.entities.Set<S_W_TaskWork>().Where(d => d.ProjectInfoID == ProjectInfoID && d.State == "Finish").Where(express).ToList();
            var data = new GridData(list);
            return Json(data);
        }

        public JsonResult GetPlanDetail(string ProjectInfoID, string Year, string Season, string Month)
        {
            var spec = this.getTimeSpec("Plan", Year, Season, Month);
            var express = spec.GetExpression<S_W_TaskWork>();
            var list = this.entities.Set<S_W_TaskWork>().Where(d => d.ProjectInfoID == ProjectInfoID).Where(express).ToList();
            var data = new GridData(list);
            return Json(data);
        }

        private string getTimewhereStr(string type,string year,string season,string month)
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
    }
}
