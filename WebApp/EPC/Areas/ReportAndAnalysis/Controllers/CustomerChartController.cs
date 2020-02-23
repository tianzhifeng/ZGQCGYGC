using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Collections;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using MvcAdapter;
using System.ComponentModel;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class CustomerChartController : EPCController
    {
        public ActionResult CustomerAnalysis()
        {
            ViewBag.StartYear = DateTime.Now.Year;
            List<object> yearEnums = new List<object>();
            for (int i = 0; i < 8; i++)
            {
                var year = DateTime.Now.Year - i;
                yearEnums.Add(new { value = year, text = year });
            }
            ViewBag.YearEnum = JsonHelper.ToJson(yearEnums);
            var monthEnums = new List<object>();
            for (int i = 1; i <= 12; i++)
                monthEnums.Add(new { value = i, text = i });
            ViewBag.MonthEnum = JsonHelper.ToJson(monthEnums);
            return View();
        }

        public JsonResult GetList()
        {
            var field = CustomerAnlysisType.ReceiptValue.ToString();
            string queryData = this.Request["QueryData"];
            if (!String.IsNullOrEmpty(queryData))
            {
                var query = JsonHelper.ToObject(queryData);
                if (!String.IsNullOrEmpty(query.GetValue("AnlysisValue")))
                    field = query.GetValue("AnlysisValue");
            }
            var sql = this.createSQL();
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            var result = new Dictionary<string, object>();
            result["data"] = dt;
            result["chartData"] = createChartOption(dt, field);
            return Json(result);
        }

        private string createSQL()
        {
            string sql = @"select top 20 * from (select S_M_CustomerInfo.ID CustomerID, Name,SerialNumber,
isnull(ReceiptValue,0) as ReceiptValue,
isnull(ContractValue,0) as ContractValue,
isnull(UnContractValue,0) as UnContractValue,
isnull(ReceivableValue,0) as ReceivableValue,isnull(RemainContractValue,0) as RemainContractValue
 from S_M_CustomerInfo left join 
(select Sum(ReceiptValue) as ReceiptValue,CustomerInfo from dbo.S_M_Receipt
where ReceiptDate>='{1}' and ReceiptDate<='{2}' 
group by CustomerInfo) ReceiptValueInfo
on ReceiptValueInfo.CustomerInfo = S_M_CustomerInfo.ID
left join (select Sum(ContractRMBValue) as ContractValue,PartyA from S_M_ContractInfo
where SignDate is not null and SignDate>='{1}' and SignDate<='{2}' 
group by PartyA) SignContractInfo
on SignContractInfo.PartyA = S_M_CustomerInfo.ID
left join (select Sum(ContractRMBValue) as UnContractValue,PartyA from S_M_ContractInfo
where SignDate is null
group by PartyA) UnSignContractInfo
on UnSignContractInfo.PartyA = S_M_CustomerInfo.ID
left join (select Sum(isnull(SumInvoiceValue,0)-isnull(SumReceiptValue,0)-isnull(SumBadDebtValue,0))
as ReceivableValue,PartyA from S_M_ContractInfo group by PartyA) ReceivableValueInfo
on ReceivableValueInfo.PartyA = S_M_CustomerInfo.ID
left join (select Sum(isnull(ContractRMBValue,0)-isnull(SumReceiptValue,0)-isnull(SumBadDebtValue,0))
as RemainContractValue ,PartyA from S_M_ContractInfo where SignDate is not null group by PartyA) RemainContractValueInfo
on RemainContractValueInfo.PartyA = S_M_CustomerInfo.ID
) TableInfo order by {0} desc";

            string queryData = this.Request["QueryData"];
            var startDate = new DateTime(DateTime.Now.Year, 1, 1).ToShortDateString();
            var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1).ToShortDateString();
            string queryValue = "ReceiptValue";
            if (!String.IsNullOrEmpty(queryData))
            {
                var query = JsonHelper.ToObject(queryData);
                if (!String.IsNullOrEmpty(query.GetValue("LastYear")))
                {

                }
                var lastYear = String.IsNullOrEmpty(query.GetValue("LastYear")) ? 1 : Convert.ToInt32(query.GetValue("LastYear"));
                var endYear = DateTime.Now;
                var startYear = DateTime.Now.Year - lastYear + 1;
                startDate = new DateTime(startYear, 1, 1).ToShortDateString();
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1).ToShortDateString();
                if (!String.IsNullOrEmpty(query.GetValue("AnlysisValue")))
                    queryValue = query.GetValue("AnlysisValue");
            }
            sql = String.Format(sql, queryValue, startDate, endDate, "Yes");
            return sql;

        }

        private Dictionary<string, object> createChartOption(DataTable dt, string field)
        {
            FillDataSourceWithChart(dt, field);
            var result = new Dictionary<string, object>();
            var chart = new Dictionary<string, object>();
            chart.SetValue("zoomType", "xy");
            result.SetValue("chart", chart);
            var title = new Dictionary<string, object>();
            title.SetValue("text", "");
            result.SetValue("title", title);
            var xAxisis = new List<Dictionary<string, object>>();
            var xAxis = new Dictionary<string, object>();
            var categories = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
                categories[i] = dt.Rows[i]["Name"].ToString();
            xAxis.SetValue("categories", categories);
            xAxisis.Add(xAxis);
            result.SetValue("xAxis", xAxisis);
            result.SetValue("credits", new { enabled = false });
            var yAxisis = new List<Dictionary<string, object>>();
            yAxisis.Add(this.CreateYAxis("#4572A7", EnumBaseHelper.GetEnumDescription(typeof(CustomerAnlysisType), field), "{value}", false));
            yAxisis.Add(this.CreateYAxis("#89A54E", "", "{value} %", true));
            result.SetValue("yAxis", yAxisis);
            var tooltip = new Dictionary<string, object>();
            tooltip.SetValue("shared", true);
            result.SetValue("tooltip", tooltip);

            var series = new List<Dictionary<string, object>>();
            series.Add(this.CreateSeries(dt, field, EnumBaseHelper.GetEnumDescription(typeof(CustomerAnlysisType), field), "column", "元", "#4572A7"));
            series.Add(this.CreateSeries(dt, "Rate", "累计比例", "spline", "%", "#89A54E", 1));
            result.SetValue("series", series);
            return result;
        }

        private Dictionary<string, object> CreateSeries(DataTable dt, string field, string name, string type, string toolTipUnit, string color, int yAxis = 0)
        {
            var result = new Dictionary<string, object>();
            result.SetValue("name", name);
            result.SetValue("color", color);
            result.SetValue("type", type);
            if (yAxis > 0)
                result.SetValue("yAxis", yAxis);
            var tooltip = new Dictionary<string, object>();
            tooltip.SetValue("valueSuffix", toolTipUnit);
            result.SetValue("tooltip", tooltip);
            var data = new ArrayList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data.Add(dt.Rows[i][field]);
            }
            result.SetValue("data", data);
            return result;
        }

        private Dictionary<string, object> CreateYAxis(string color, string text, string format, bool opposite = false)
        {
            var result = new Dictionary<string, object>();
            var yAxisLabels = new Dictionary<string, object>();
            var yAxisLabelsStyle = new Dictionary<string, object>();
            yAxisLabelsStyle.SetValue("color", color);
            yAxisLabelsStyle.SetValue("format", format);
            yAxisLabels.SetValue("style", yAxisLabelsStyle);
            result.SetValue("labels", yAxisLabels);
            var yAxisTitle = new Dictionary<string, object>();
            var yAxisTitleStyle = new Dictionary<string, object>();
            yAxisTitleStyle.SetValue("color", color);
            yAxisTitle.SetValue("text", text);
            yAxisTitle.SetValue("style", yAxisTitleStyle);
            result.SetValue("title", yAxisTitle);
            if (opposite)
                result.SetValue("opposite", opposite);
            return result;
        }

        private void FillDataSourceWithChart(DataTable dt, string field)
        {
            dt.Columns.Add("Rate", typeof(decimal));
            var obj = dt.Compute("Sum(" + field + ")", "");
            var sumValue = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
            var sumRate = 0M;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (sumValue == 0)
                {
                    dt.Rows[i]["Rate"] = 0;
                    continue;
                }
                var value = dt.Rows[i][field] == null || dt.Rows[i][field] == DBNull.Value ? 0 : Convert.ToDecimal(dt.Rows[i][field]);
                var currentRate = Math.Round(value / sumValue * 100, 4);
                var rate = Math.Round(value / sumValue * 100, 4) + sumRate;
                sumRate += currentRate;
                dt.Rows[i]["Rate"] = Math.Round(rate, 2);
            }
        }
    }

    public enum CustomerAnlysisType
    {
        [Description("实际收款额")]
        ReceiptValue,
        [Description("已签合同额")]
        ContractValue,
        [Description("财务应收款")]
        ReceivableValue,
        [Description("合同余额")]
        RemainContractValue,
        [Description("待签合同额")]
        UnContractValue
    }
}
