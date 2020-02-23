using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Reflection;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Config;
using Config.Logic;
using Project.Logic.Domain;
using Project.Logic;
using System.Linq.Expressions;
using Formula.DynConditionObject;

namespace Project.Areas.Monitor.Controllers
{
    public class TaskWorkExecuteAnyliseController : ProjectController
    {
        public ActionResult TaskWorkExecuteAnylise()
        {
            string projectInfoID = GetQueryString("ProjectInfoID");
            var projectInfo = this.entities.Set<S_I_ProjectInfo>().SingleOrDefault(d => d.ID == projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象");
            ViewBag.DefaultYear = DateTime.Now.Year;
            ViewBag.DefaultMonth = DateTime.Now.Month;
            var majors = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID && d.WBSType == "Major").
                Select(c => new { MajorName = c.Name, MajorValue = c.WBSValue, SortIndex = c.SortIndex, }).Distinct().OrderBy(d => d.SortIndex)
                .Select(c => new { value = c.MajorValue, text = c.MajorName }).ToList();
            ViewBag.MajorJson = JsonHelper.ToJson(majors);
            return View();
        }

        public JsonResult GetList(string Year, string Season, string Month,string MajorValue)
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象");

            //获取所有项目的工作卷册信息
            var allTaskWorkList = this.entities.Set<S_W_TaskWork>().Where(d => d.ProjectInfoID == projectInfoID).ToList();

            //获取项目所有已完成的卷册信息
            var allFinishTaskWorkList = allTaskWorkList.Where(d => d.ProjectInfoID == projectInfoID && d.State == "Finish").ToList();

            //根据查询条件，获取所有卷册信息（计划年，月，季度，专业）
            var spec = new Specifications();
            spec.AndAlso("ProjectInfoID", projectInfoID, QueryMethod.Equal);
            if (!String.IsNullOrEmpty(Year))
                spec.AndAlso("PlanYear", Convert.ToInt32(Year), QueryMethod.Equal);
            if (!String.IsNullOrEmpty(Season))
                spec.AndAlso("PlanSeason", Convert.ToInt32(Season), QueryMethod.Equal);
            if (!String.IsNullOrEmpty(Month))
                spec.AndAlso("PlanMonth", Convert.ToInt32(Month), QueryMethod.Equal);
            var planTaskWorkList = this.entities.Set<S_W_TaskWork>().Where(spec.GetExpression<S_W_TaskWork>()).ToList();

            //根据查询条件获得所有完成的卷册信息
            spec = new Specifications();
            spec.AndAlso("ProjectInfoID", projectInfoID, QueryMethod.Equal);
            spec.AndAlso("State", ProjectCommoneState.Finish.ToString(), QueryMethod.Equal);
            if (!String.IsNullOrEmpty(Year))
                spec.AndAlso("FactYear", Convert.ToInt32(Year), QueryMethod.Equal);
            if (!String.IsNullOrEmpty(Season))
                spec.AndAlso("FactSeason", Convert.ToInt32(Season), QueryMethod.Equal);
            if (!String.IsNullOrEmpty(Month))
                spec.AndAlso("FactMonth", Convert.ToInt32(Month), QueryMethod.Equal);
            var finsihTaskWorkList = this.entities.Set<S_W_TaskWork>().Where(spec.GetExpression<S_W_TaskWork>()).ToList();
         
            var currentUnFinishTaskWorkList = planTaskWorkList.Where(d => d.State != "Finish").ToList();

            var majors = projectInfo.GetMajors(MajorValue);
            var result = new Dictionary<string, object>();
            var listData = new List<Dictionary<string, object>>();
            foreach (var major in majors)
            {
                string majorValue = major.GetValue("Value");
                var itemData = new Dictionary<string, object>();
                var majorTaskCount = allTaskWorkList.Count(d => d.MajorValue == majorValue);
                var majorFinishTaskCount = allFinishTaskWorkList.Count(d => d.MajorValue == majorValue);
                var planFinishTaskCount = planTaskWorkList.Count(d => d.MajorValue == majorValue);
                var currentFinishTaskCount = finsihTaskWorkList.Count(d => d.MajorValue == majorValue);
                var delayUnDone = currentUnFinishTaskWorkList.Count(d => d.MajorValue == majorValue);
                itemData.SetValue("MajorName", major.GetValue("Name"));
                itemData.SetValue("MajorValue", major.GetValue("Value"));
                itemData.SetValue("AllCount", majorTaskCount);
                itemData.SetValue("AllFinishedCount", majorFinishTaskCount);
                itemData.SetValue("AllFinishedPercent", majorTaskCount == 0 ? 0 : Math.Round(Convert.ToDouble(majorFinishTaskCount) / Convert.ToDouble(majorTaskCount), 2) * 100);
                itemData.SetValue("AllDelayUndone", delayUnDone);
                itemData.SetValue("PeriodCount", planFinishTaskCount);
                itemData.SetValue("PeriodFinished", currentFinishTaskCount);
                itemData.SetValue("PeriodPercent", planFinishTaskCount == 0 ? 0 : Math.Round(Convert.ToDouble(currentFinishTaskCount) / Convert.ToDouble(planFinishTaskCount), 2) * 100);
                listData.Add(itemData);
            }
            result.SetValue("data", listData);
            result.SetValue("ColumnChart", this.createColumnChart(majors, planTaskWorkList, finsihTaskWorkList));
            result.SetValue("AngularChart", this.createAngularChart(allTaskWorkList, allFinishTaskWorkList));
            return Json(result);
        }

        private Dictionary<string, object> createColumnChart(List<Dictionary<string, object>> Majors,
            List<S_W_TaskWork> PlanTaskWorkList, List<S_W_TaskWork> FinishTaskWorkList)
        {
            var columnChart = new Dictionary<string, object>();
            var chartPty = new Dictionary<string, object>();
            chartPty.SetValue("caption", "各专业卷册完成情况分析");
            chartPty.SetValue("rotatevalues", "1");
            chartPty.SetValue("placevaluesinside", "1");
            chartPty.SetValue("showvalues", "0");
            chartPty.SetValue("showlabels", "1");
            chartPty.SetValue("bgcolor", "FFFFFF");
            chartPty.SetValue("showBorder", "0");
            columnChart.SetValue("chart", chartPty);
            var categories = new List<Dictionary<string, object>>();
            var category = new Dictionary<string, object>();
            var lables = new List<Dictionary<string, object>>();
            var dataSet = new List<Dictionary<string, object>>();
            foreach (var major in Majors)
            {
                var label = new Dictionary<string, object>();
                label.SetValue("label", major.GetValue("Name"));
                lables.Add(label);
            }
            category.SetValue("category", lables);
            categories.Add(category);
            columnChart.SetValue("categories", categories);

            var plandata = new Dictionary<string, object>();
            plandata.SetValue("color", colors[0]);
            var planDataList = new List<Dictionary<string, object>>();
            foreach (var major in Majors)
            {
                var plan = new Dictionary<string, object>();
                var majorValue = major.GetValue("Value");
                var planCount = PlanTaskWorkList.Count(d => d.MajorValue == majorValue);
                plan.SetValue("value", planCount);
                planDataList.Add(plan);
            }
            plandata.SetValue("seriesname", "计划完成");
            plandata.SetValue("data", planDataList);
            dataSet.Add(plandata);

            var finishdata = new Dictionary<string, object>();
            finishdata.SetValue("color", colors[1]);
            var finishDataList = new List<Dictionary<string, object>>();
            foreach (var major in Majors)
            {
                var finish = new Dictionary<string, object>();
                var majorValue = major.GetValue("Value");
                var finishCount = FinishTaskWorkList.Count(d => d.MajorValue == majorValue);
                finish.SetValue("value", finishCount);
                finishDataList.Add(finish);
            }
            finishdata.SetValue("seriesname", "实际完成");
            finishdata.SetValue("data", finishDataList);
            dataSet.Add(finishdata);
            columnChart.SetValue("dataset", dataSet);
            return columnChart;        
        }

        private Dictionary<string, object> createAngularChart(List<S_W_TaskWork> allTaskWorkList, List<S_W_TaskWork> allFinishTaskWorkList)
        {
            var result = new Dictionary<string, object>();
            var chartpty = new Dictionary<string, object>();
            chartpty.SetValue("caption", "卷册完成情况分析");
            chartpty.SetValue("bgcolor", "FFFFFF");
            chartpty.SetValue("showBorder", "0");
            chartpty.SetValue("managevalueoverlapping", "1");
            chartpty.SetValue("autoaligntickvalues", "1");
            chartpty.SetValue("fillangle", "45");
            chartpty.SetValue("upperlimit", allTaskWorkList.Count);
            chartpty.SetValue("lowerlimit", 0);
            chartpty.SetValue("showgaugeborder", 0);
            chartpty.SetValue("showvalue", "1");

            var colorrange = new Dictionary<string, object>();
            var colors = new List<Dictionary<string, object>>();
            var finishColor = new Dictionary<string,object>();
            finishColor.SetValue("minvalue",0);
            finishColor.SetValue("maxvalue",allFinishTaskWorkList.Count);
            finishColor.SetValue("code", "8BBA00");
            colors.Add(finishColor);
            var remianColor  =new Dictionary<string,object>();
            remianColor.SetValue("minvalue", allFinishTaskWorkList.Count);
            remianColor.SetValue("maxvalue", allTaskWorkList.Count);
            remianColor.SetValue("code", "F6BD0F");
            colors.Add(remianColor);
            colorrange.SetValue("color", colors);
            var dials = new Dictionary<string, object>();
            var dialList = new List<Dictionary<string, object>>();
         
            var dial = new Dictionary<string, object>();
            dial.SetValue("value", allFinishTaskWorkList.Count);
            dial.SetValue("borderalpha", "0");
            dial.SetValue("bgcolor", "#000000");
            dial.SetValue("basewidth", "20");
            dial.SetValue("topwidth", "1");
            dial.SetValue("radius", "130");
            dialList.Add(dial);
            dials.SetValue("dial", dialList);
            result.SetValue("dials", dials);
            result.SetValue("chart", chartpty);
            result.SetValue("colorrange", colorrange);
            return result;
        }

        string[] colors = new string[] { "F6BD0F", "AFD8F8", "8BBA00" };
    }
}
