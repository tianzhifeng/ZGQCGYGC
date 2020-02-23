using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

using Config;
using Config.Logic;
using MvcAdapter;
using Project.Logic.Domain;
using Project.Logic;



namespace Project.Areas.Monitor.Controllers
{
    public class CostAnalyzeController : ProjectController
    {
        public ActionResult CostTab()
        {
            ViewBag.ProjectInfoID = this.GetQueryString("ProjectInfoID");
            return View();
        }

        public JsonResult GetList()
        {
            //            string projectInfoID = this.Request["ProjectInfoID"];
            //            string belongYear = this.Request["BelongYear"];
            //            if (String.IsNullOrEmpty(belongYear)) { belongYear = DateTime.Now.Year.ToString(); }
            //            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            //            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("没有找到指定的项目对象");
            //            //var marketDBContext = Formula.FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            //            //var project = marketDBContext.S_I_Project.FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
            //            //if (project == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目对象");
            //            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            //            //var mainDt = db.ExecuteDataTable("select distinct SubjectName,SubjectCode from S_FC_CostInfo where ProjectID='" + project.ID + "'");

            //            string sql = @"select SubjectName,SubjectCode,Sum(CostValue) as CostValue,
            //BelongYear,BelongMonth,ProjectID from S_FC_CostInfo where ProjectID='{0}' and BelongYear='{1}'
            //group by SubjectName,SubjectCode,BelongYear,BelongMonth,ProjectID ";
            //            var dt = db.ExecuteDataTable(String.Format(sql, projectInfo.ID, belongYear));
            //            var result = new Dictionary<string, object>();
            //            var resultDt = new DataTable();
            //            resultDt.Columns.Add("SubjectName");
            //            resultDt.Columns.Add("SubjectCode");
            //            resultDt.Columns.Add("ProjectID");
            //            resultDt.Columns.Add("BelongYear", typeof(string));
            //            resultDt.Columns.Add("SumCostValue",typeof(decimal));
            //            for (int i = 1; i <= 12; i++)
            //            {
            //                var field =  i.ToString() + "Month";
            //                if (!resultDt.Columns.Contains(field))
            //                    resultDt.Columns.Add(field, typeof(decimal));
            //            }

            //            string series = string.Empty;
            //            string serieFields = string.Empty;
            //            foreach (DataRow row in mainDt.Rows)
            //            {
            //                var item = resultDt.NewRow();
            //                item["SubjectName"] = row["SubjectName"];
            //                item["ProjectID"] = project.ID;
            //                item["SubjectCode"] = row["SubjectCode"];
            //                item["BelongYear"] = belongYear;
            //                var sumValue = 0m;
            //                for (int i = 1; i <= 12; i++)
            //                {
            //                    var field =  i.ToString() + "Month";
            //                    if (resultDt.Columns.Contains(field))
            //                    {
            //                        var monthRows = dt.Select(" SubjectCode='" + row["SubjectCode"].ToString() + "' and BelongMonth='" + i + "' ");
            //                        if (monthRows.Length == 0)
            //                            item[field] = 0;
            //                        else
            //                        {
            //                            var costValue = monthRows[0]["CostValue"] == null || monthRows[0]["CostValue"] == DBNull.Value ? 0m : Convert.ToDecimal(monthRows[0]["CostValue"]);
            //                            item[field] = costValue;
            //                            sumValue += costValue;
            //                        }
            //                    }
            //                }
            //                item["SumCostValue"] = sumValue;
            //                resultDt.Rows.Add(item);
            //            }
            var data = new Dictionary<string, object>();
            //data.SetValue("data", resultDt);
            //data.SetValue("chartData", this.GetYearChartData(belongYear, project.ID));
            //data.SetValue("pieChartData", this.GetPieChartData(belongYear, project.ID));
            return Json(data);
        }

        public Dictionary<string, object> GetYearChartData(string belongYear, string projectID)
        {
            string sql = @"select Sum(CostValue) as CostValue,
BelongYear,BelongMonth,ProjectID  from S_FC_CostInfo where ProjectID='{0}' 
and BelongYear='{1}' group by BelongYear,BelongMonth,ProjectID";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var dt = db.ExecuteDataTable(String.Format(sql, projectID, belongYear));
            var dataSource = new DataTable();
            dataSource.Columns.Add("BelongYear", typeof(string));
            dataSource.Columns.Add("BelongMonth", typeof(string));
            dataSource.Columns.Add("CostValue", typeof(decimal));
            dataSource.Columns.Add("SumCostValue", typeof(decimal));
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
                    value = monthRows[0]["CostValue"] == null || monthRows[0]["CostValue"] == DBNull.Value ? 0m : Convert.ToDecimal(monthRows[0]["CostValue"]);
                }
                sumValue += value;
                row["SumCostValue"] = sumValue;
                row["CostValue"] = value;
                dataSource.Rows.Add(row);
            }
            #region 生成图表
            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "成本金额");
            y1.Lable.SetValue("format", "{value}元");
            yAxies.Add(y1);

            var serDefines = new List<Series>();


            var costSer = new Series { Name = "实际成本", Field = "CostValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            var sumCostSer = new Series { Name = "累计成本", Field = "SumCostValue", Type = "spline", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            serDefines.Add(costSer);
            serDefines.Add(sumCostSer);

            var chart = HighChartHelper.CreateColumnXYChart(belongYear + "年度成本分析", "", dataSource, "BelongMonth", yAxies, serDefines, null);
            return chart;
            #endregion
        }

        public Dictionary<string, object> GetPieChartData(string belongYear, string projectID)
        {
            string sql = @"select * from (select SubjectName,SubjectCode,Sum(CostValue) as CostValue,
BelongYear,ProjectID from dbo.S_FC_CostInfo
group by SubjectName,SubjectCode,BelongYear,ProjectID) tableInfo where 1=1";
            if (!String.IsNullOrEmpty(belongYear))
                sql += " and BelongYear='" + belongYear + "'";
            if (!String.IsNullOrEmpty(projectID))
                sql += " and ProjectID='" + projectID + "'";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var dt = db.ExecuteDataTable(sql);
            var pieChart = HighChartHelper.CreatePieChart(belongYear + "成本科目分布", "成本金额（元）", dt, "SubjectName", "CostValue");

            var chartOption = pieChart.Render();
            var credits = new Dictionary<string, object>();
            credits.SetValue("enabled", false);
            chartOption.SetValue("credits", credits);
            return chartOption;
        }

        public JsonResult GetAllList()
        {
            //            string projectInfoID = this.Request["ProjectInfoID"];
            //            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            //            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("没有找到指定的项目对象");
            //            //var marketDBContext = Formula.FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            //            //var project = marketDBContext.S_I_Project.FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
            //            if (project == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目对象");
            //            string sql = @"select SubjectName,SubjectCode,Sum(CostValue) as CostValue,ProjectID
            //from dbo.S_FC_CostInfo where ProjectID='{0}'
            //group by SubjectName,SubjectCode,ProjectID";
            //            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            //            var mainDt = db.ExecuteDataTable(String.Format(sql,project.ID));
            var result = new Dictionary<string, object>();
            //            result["data"] = mainDt;
            //            var pieChart = HighChartHelper.CreatePieChart("项目成本分析", "成本金额（元）", mainDt, "SubjectName", "CostValue");
            //            var chartData = pieChart.Render();
            //            var credits = new Dictionary<string, object>();
            //            credits.SetValue("enabled", false);
            //            chartData.SetValue("credits", credits);
            //            result["pieChartData"] = chartData;
            return Json(result);
        }
    }
}
