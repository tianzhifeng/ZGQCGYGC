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
using System.Text;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class PublishInfoController : ProjectController
    {
        public JsonResult GetList()
        {
            string belongYear = this.Request["BelongYear"];
            if (String.IsNullOrEmpty(belongYear)) { belongYear = DateTime.Now.Year.ToString(); }
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var depts = EnumBaseHelper.GetEnumDef("System.ManDept").EnumItem;
            string sql = @"select Sum(ToA1) ToA1,ChargeDeptID,ChargeDeptName,BelongYear,BelongMonth,BelongQuarter from S_EP_PublishInfo
left join S_I_ProjectInfo on S_EP_PublishInfo.ProjectInfoID=S_I_ProjectInfo.ID
where ChargeDeptID in ('" + string.Join("','", depts.Select(a => a.Code).ToArray()) + @"') and PublishTime is not null
group by ChargeDeptID,ChargeDeptName,BelongYear,BelongMonth,BelongQuarter  ";
            var dt = db.ExecuteDataTable(String.Format(sql, belongYear));
            var result = new Dictionary<string, object>();
            var resultDt = new DataTable();
            resultDt.Columns.Add("DeptName");
            resultDt.Columns.Add("DeptID");
            resultDt.Columns.Add("BelongYear", typeof(string));
            resultDt.Columns.Add("SumToA1", typeof(decimal));
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
                        var monthRows = dt.Select(" ChargeDeptID='" + dept.Code + "' and BelongMonth='" + i + "'  and BelongYear ='" + belongYear + "'");
                        if (monthRows.Length == 0)
                            item[field] = 0;
                        else
                        {
                            var costValue = monthRows[0]["ToA1"] == null || monthRows[0]["ToA1"] == DBNull.Value ? 0m : Convert.ToDecimal(monthRows[0]["ToA1"]);
                            item[field] = costValue;
                            sumValue += costValue;
                        }
                    }
                }
                item["SumToA1"] = sumValue;
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
            var depts = EnumBaseHelper.GetEnumDef("System.ManDept").EnumItem;
            string sql = @"select Sum(ToA1) as ToA1,
BelongYear,BelongMonth  from S_EP_PublishInfo 
left join S_I_ProjectInfo on S_EP_PublishInfo.ProjectInfoID=S_I_ProjectInfo.ID
where ChargeDeptID in ('" + string.Join("','", depts.Select(a => a.Code).ToArray()) + @"') and PublishTime is not null and BelongYear='{0}' group by BelongYear,BelongMonth";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = db.ExecuteDataTable(String.Format(sql, belongYear));
            var dataSource = new DataTable();
            dataSource.Columns.Add("BelongYear", typeof(string));
            dataSource.Columns.Add("BelongMonth", typeof(string));
            dataSource.Columns.Add("ToA1", typeof(decimal));
            dataSource.Columns.Add("SumToA1", typeof(decimal));
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
                    value = monthRows[0]["ToA1"] == null || monthRows[0]["ToA1"] == DBNull.Value ? 0m : Convert.ToDecimal(monthRows[0]["ToA1"]);
                }
                sumValue += value;
                row["SumToA1"] = sumValue;
                row["ToA1"] = value;
                dataSource.Rows.Add(row);
            }
            #region 生成图表
            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "折合A1数");
            y1.Lable.SetValue("format", "{value}张");
            yAxies.Add(y1);

            var serDefines = new List<Series>();


            var costSer = new Series { Name = "当月出图", Field = "ToA1", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            var sumCostSer = new Series { Name = "累计出图", Field = "SumToA1", Type = "spline", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            serDefines.Add(costSer);
            serDefines.Add(sumCostSer);

            var chart = HighChartHelper.CreateColumnXYChart(belongYear + "年度出图分析", "", dataSource, "BelongMonth", yAxies, serDefines, null);
            return chart;
            #endregion
        }

        public Dictionary<string, object> GetPieChartData(string belongYear)
        {
            var depts = EnumBaseHelper.GetEnumDef("System.ManDept").EnumItem;
            string sql = @"select * from (select Sum(ToA1) SumToA1,ChargeDeptID,ChargeDeptName,BelongYear from S_EP_PublishInfo
left join S_I_ProjectInfo on S_EP_PublishInfo.ProjectInfoID=S_I_ProjectInfo.ID
where PublishTime is not null
group by ChargeDeptID,ChargeDeptName,BelongYear ) tableInfo where ChargeDeptID in ('" + string.Join("','", depts.Select(a => a.Code).ToArray()) + @"') ";
            if (!String.IsNullOrEmpty(belongYear))
                sql += " and BelongYear='" + belongYear + "'";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var dt = db.ExecuteDataTable(sql);
            var pieChart = HighChartHelper.CreatePieChart(belongYear + "各部门出图", "出图量", dt, "ChargeDeptName", "SumToA1");
            var chartOption = pieChart.Render();
            var credits = new Dictionary<string, object>();
            credits.SetValue("enabled", false);
            chartOption.SetValue("credits", credits);
            return chartOption;
        }

    }
}
