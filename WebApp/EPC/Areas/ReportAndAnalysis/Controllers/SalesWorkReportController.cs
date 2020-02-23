using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula.Helper;
using Formula;
using MvcAdapter;
using Config;
using Config.Logic;
using System.Collections;
using EPC.Logic;
using System.ComponentModel;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class SalesWorkReportController : EPCController
    {

        public ActionResult List()
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = startDate.AddMonths(1);
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            var def = EnumBaseHelper.GetEnumDef("EPC.SaleWorkContent");
            var items = def.EnumItem.ToList();
            ViewBag.Columns = items;
            return View();
        }

        public JsonResult GetAnalysisList()
        {
            string queryData = this.Request["QueryData"];
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = startDate.AddMonths(1);
            if (!String.IsNullOrEmpty(queryData))
            {
                var query = JsonHelper.ToObject(queryData);
                if (!String.IsNullOrEmpty(query.GetValue("StartDate")))
                    startDate = Convert.ToDateTime(query.GetValue("StartDate"));
                if (!String.IsNullOrEmpty(query.GetValue("EndDate")))
                    endDate = Convert.ToDateTime(query.GetValue("EndDate"));
            }
            var dt = CreateTable();
            var sql = @"select Count(0) as WorkCount,UserInfo,ContactType from dbo.S_M_BusinessTrace
where RegisterDate>='{0}' and RegisterDate<='{1}'
group by UserInfo,ContactType";
            var dataDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, startDate.ToString(), endDate.ToString()));

            foreach (DataRow row in dt.Rows)
            {
                var totalValue = 0m;
                var rows = dataDt.Select("UserInfo='" + row["UserID"].ToString() + "'");
                foreach (DataRow item in rows)
                {
                    if (item["ContactType"] == null || item["ContactType"] == DBNull.Value || String.IsNullOrEmpty(item["ContactType"].ToString()))
                        continue;
                    var field = item["ContactType"].ToString() + "_Count";
                    var value = item["WorkCount"] == null || item["WorkCount"] == DBNull.Value ? 0m : Convert.ToDecimal(item["WorkCount"]);
                    row[field] = value;
                    totalValue += value;
                }
                row["TotalValue"] = totalValue;
            }

            var result = new Dictionary<string, object>();
            result.SetValue("data", dt);
            #region 生成图表

            var series = string.Empty; var serieFields = string.Empty;
            var def = EnumBaseHelper.GetEnumDef("EPC.SaleWorkContent");
            var items = def.EnumItem.ToList();
            foreach (var item in items)
            {
                series += item.Name + ",";
                serieFields += item.Code + "_Count,";
            }
            series = series.TrimEnd(',');
            serieFields = serieFields.TrimEnd(',');
            var columChart = HighChartHelper.CreateColumnChart("销售人员工作分析", dt, "UserName", series.Split(','), serieFields.Split(','));
            result.SetValue("chartData", columChart.Render());
            #endregion
            return Json(result);
        }

        private DataTable CreateTable()
        {
            var dt = EnumBaseHelper.GetEnumTable("EPC.SaleWorkContent");
            var result = new DataTable();
            result.Columns.Add("UserID");
            result.Columns.Add("UserName");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var fieldName = dt.Rows[i]["value"].ToString() + "_Count";
                result.Columns.Add(fieldName, typeof(decimal));
            }
            result.Columns.Add("TotalValue", typeof(decimal));
            var user = EnumBaseHelper.GetEnumTable("EPC.Sales");
            foreach (DataRow item in user.Rows)
            {
                var row = result.NewRow();
                row["UserID"] = item["value"];
                row["UserName"] = item["text"];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var fieldName = dt.Rows[i]["value"].ToString() + "_Count";
                    row[fieldName] = 0;
                }
                result.Rows.Add(row);
            }
            return result;
        }
    }
}
