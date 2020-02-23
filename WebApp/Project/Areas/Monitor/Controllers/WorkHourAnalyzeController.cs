using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using MvcAdapter;
using Project.Logic.Domain;
using Project.Logic;
using System.Data;


namespace Project.Areas.Monitor.Controllers
{
    public class WorkHourAnalyzeController : ProjectController
    {
        public ActionResult Tab()
        {
            ViewBag.ProjectInfoID = this.GetQueryString("ProjectInfoID");
            return View();
        }

        public JsonResult GetList()
        {
            string belongYear = this.Request["BelongYear"];
            if (String.IsNullOrEmpty(belongYear)) belongYear = DateTime.Now.Year.ToString();
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(this.Request["ProjectInfoID"]);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
            string sql = @"select distinct Name,WBSValue,ProjectInfoID,SortIndex from S_W_WBS where ProjectInfoID='{0}' and WBSType='Major' order by SortIndex";
            var resultDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, projectInfo.ID));
            for (int i = 1; i <= 12; i++)
            {
                var field = i + "Month";
                resultDt.Columns.Add(field, typeof(decimal));
            }
            resultDt.Columns.Add("SumWorkHour", typeof(decimal));

            sql = @"select Sum(WorkHourValue) as WorkHourValue,BelongYear,BelongMonth,MajorCode,MajorName from S_W_UserWorkHour
where ProjectID='{0}' and BelongYear= '{1}' group by BelongYear,BelongMonth,MajorCode,MajorName ";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            var workHourDt = db.ExecuteDataTable(String.Format(sql, projectInfo.ID, belongYear));
            foreach (DataRow row in resultDt.Rows)
            {
                var sumValue = 0m;
                for (int i = 1; i <= 12; i++)
                {
                    var field = i + "Month";
                    var value = 0m;
                    var workHourRows = workHourDt.Select("BelongMonth='" + i.ToString() + "' and MajorCode='" + row["WBSValue"] + "'");
                    if (workHourRows.Length > 0)
                    {
                        value = workHourRows[0]["WorkHourValue"] == null || workHourRows[0]["WorkHourValue"] == DBNull.Value ? 0m : Convert.ToDecimal(workHourRows[0]["WorkHourValue"]);
                    }
                    row[field] = value;
                    sumValue += value;
                }
                row["SumWorkHour"] = sumValue;
            }
            var result = new Dictionary<string, object>();
            result.SetValue("data", resultDt);
            result.SetValue("chartData", this.GetYearChartData(belongYear, projectInfo.ID));
            result.SetValue("pieChartData", this.GetPieChartData(belongYear, projectInfo.ID));
            return Json(result);
        }

        public JsonResult GetAllList()
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(this.Request["ProjectInfoID"]);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
            string sql = @"select distinct Name,WBSValue,ProjectInfoID,SortIndex from S_W_WBS where ProjectInfoID='{0}' and WBSType='Major' order by SortIndex";
            var resultDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, projectInfo.ID));
            resultDt.Columns.Add("SumWorkHour", typeof(decimal));
            sql = @"select Sum(WorkHourValue) as WorkHourValue,MajorCode,MajorName from S_W_UserWorkHour
where ProjectID='{0}' group by MajorCode,MajorName ";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            var workHourDt = db.ExecuteDataTable(String.Format(sql, projectInfo.ID));
            foreach (DataRow row in resultDt.Rows)
            {
                var value = 0m;
                var workHourRows = workHourDt.Select(" MajorCode='" + row["WBSValue"] + "'");
                if (workHourRows.Length > 0)
                {
                    value = workHourRows[0]["WorkHourValue"] == null || workHourRows[0]["WorkHourValue"] == DBNull.Value ? 0m : Convert.ToDecimal(workHourRows[0]["WorkHourValue"]);
                }
                row["SumWorkHour"] = value;
            }
            var result = new Dictionary<string, object>();
            result.SetValue("data", resultDt);
            result.SetValue("pieChartData", GetPieChartData("", projectInfo.ID));
            return Json(result);
        }

        public Dictionary<string, object> GetYearChartData(string belongYear, string projectID)
        {
            string sql = @"select Sum(WorkHourValue) as WorkHourValue,BelongYear,BelongMonth from S_W_UserWorkHour
where ProjectID='{0}' and BelongYear='{1}' group by BelongYear,BelongMonth";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            var dt = db.ExecuteDataTable(String.Format(sql, projectID, belongYear));
            var dataSource = new DataTable();
            dataSource.Columns.Add("BelongYear", typeof(string));
            dataSource.Columns.Add("BelongMonth", typeof(string));
            dataSource.Columns.Add("WorkHourValue", typeof(decimal));
            dataSource.Columns.Add("SumWorkHourValue", typeof(decimal));
            var sumValue = 0m;
            for (int i = 1; i <= 12; i++)
            {
                var row = dataSource.NewRow();
                row["BelongMonth"] = i + "月";
                row["BelongYear"] = belongYear;
                var value = 0m;
                var monthRows = dt.Select("BelongMonth = '" + i.ToString() + "'");
                if (monthRows.Length > 0)
                {
                    value = monthRows[0]["WorkHourValue"] == null || monthRows[0]["WorkHourValue"] == DBNull.Value ? 0m : Convert.ToDecimal(monthRows[0]["WorkHourValue"]);
                }
                sumValue += value;
                row["WorkHourValue"] = value;
                row["SumWorkHourValue"] = sumValue;
                dataSource.Rows.Add(row);
            }
            #region 生成图表
            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "");
            y1.Lable.SetValue("format", "{value}工时");
            yAxies.Add(y1);

            var serDefines = new List<Series>();


            var costSer = new Series { Name = "当月工时", Field = "WorkHourValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            var sumCostSer = new Series { Name = "累计工时", Field = "SumWorkHourValue", Type = "spline", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            serDefines.Add(costSer);
            serDefines.Add(sumCostSer);

            var chart = HighChartHelper.CreateColumnXYChart(belongYear + "年度项目工时分析", "", dataSource, "BelongMonth", yAxies, serDefines, null);
            return chart;
            #endregion
        }

        public Dictionary<string, object> GetPieChartData(string belongYear, string projectID)
        {
            string sql = @"select * from (select Sum(WorkHourValue) as WorkHourValue,ProjectID,BelongYear,MajorCode,MajorName from S_W_UserWorkHour
group by BelongYear,MajorCode,MajorName,ProjectID ) tableInfo where 1=1";
            if (!String.IsNullOrEmpty(belongYear))
                sql += " and BelongYear='" + belongYear + "'";
            if (!String.IsNullOrEmpty(projectID))
                sql += " and ProjectID='" + projectID + "'";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            var dt = db.ExecuteDataTable(sql);
            var pieChart = HighChartHelper.CreatePieChart(belongYear + "专业工时分布", "工时", dt, "MajorName", "WorkHourValue");
            var chartOption = pieChart.Render();
            var credits = new Dictionary<string, object>();
            credits.SetValue("enabled", false);
            chartOption.SetValue("credits", credits);
            return chartOption;
        }
    }
}
