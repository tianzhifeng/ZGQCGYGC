using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using MvcAdapter;
using Project.Logic.Domain;
using Project.Logic;
using Formula.Helper;


namespace Project.Areas.ProjectGroup.Controllers
{
    public class CostInfoController : ProjectController
    {
        public JsonResult GetList()
        {
            string belongYear = this.Request["BelongYear"];
            if (String.IsNullOrEmpty(belongYear)) { belongYear = DateTime.Now.Year.ToString(); }
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var depts = EnumBaseHelper.GetEnumDef("System.Org").EnumItem;
            string sql = @"select Sum(CostValue) as CostValue,
BelongYear,BelongMonth,ProjectDeptID, ProjectDeptName from S_FC_CostInfo where  BelongYear='{0}'
group by BelongYear,BelongMonth,ProjectDeptID,ProjectDeptName ";
            var dt = db.ExecuteDataTable(String.Format(sql, belongYear));
            var result = new Dictionary<string, object>();
            var resultDt = new DataTable();
            resultDt.Columns.Add("DeptName");
            resultDt.Columns.Add("DeptID");
            resultDt.Columns.Add("BelongYear", typeof(string));
            resultDt.Columns.Add("SumCostValue", typeof(decimal));
            for (int i = 1; i <= 12; i++)
            {
                var field = i.ToString() + "Month";
                if (!resultDt.Columns.Contains(field))
                    resultDt.Columns.Add(field, typeof(decimal));
            }

            string series = string.Empty;
            string serieFields = string.Empty;
            foreach (var dept in depts)
            {
                var item = resultDt.NewRow();
                item["DeptName"] = dept.Name;
                item["DeptID"] = dept.Code;
                item["BelongYear"] = belongYear;
                var sumValue = 0m;
                for (int i = 1; i <= 12; i++)
                {
                    var field = i.ToString() + "Month";
                    if (resultDt.Columns.Contains(field))
                    {
                        var monthRows = dt.Select(" ProjectDeptID='" + dept.Code + "' and BelongMonth='" + i + "' ");
                        if (monthRows.Length == 0)
                            item[field] = 0;
                        else
                        {
                            var costValue = monthRows[0]["CostValue"] == null || monthRows[0]["CostValue"] == DBNull.Value ? 0m : Convert.ToDecimal(monthRows[0]["CostValue"]);
                            item[field] = costValue;
                            sumValue += costValue;
                        }
                    }
                }
                item["SumCostValue"] = sumValue;
                resultDt.Rows.Add(item);
            }
            var data = new Dictionary<string, object>();
            data.SetValue("data", resultDt);
            data.SetValue("chartData", this.GetYearChartData(belongYear));
            data.SetValue("pieChartData", this.GetPieChartData(belongYear));
            return Json(data);
        }

        public Dictionary<string, object> GetYearChartData(string belongYear)
        {
            string sql = @"select Sum(CostValue) as CostValue,
BelongYear,BelongMonth  from S_FC_CostInfo where BelongYear='{0}' group by BelongYear,BelongMonth";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var dt = db.ExecuteDataTable(String.Format(sql, belongYear));
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

        public Dictionary<string, object> GetPieChartData(string belongYear)
        {
            string sql = @"select * from (select SubjectName,SubjectCode,Sum(CostValue) as CostValue,
BelongYear from dbo.S_FC_CostInfo
group by SubjectName,SubjectCode,BelongYear) tableInfo where 1=1";
            if (!String.IsNullOrEmpty(belongYear))
                sql += " and BelongYear='" + belongYear + "'";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var dt = db.ExecuteDataTable(sql);
            var pieChart = HighChartHelper.CreatePieChart(belongYear + "成本科目分布", "科目费用", dt, "SubjectName", "CostValue");
            var chartOption = pieChart.Render();
            var credits = new Dictionary<string, object>();
            credits.SetValue("enabled", false);
            chartOption.SetValue("credits", credits);
            return chartOption;
        }
    }
}
