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
    public class BidResultAnalysisController : EPCController
    {
        public JsonResult GetAnalysisList()
        {
            string queryData = this.Request["QueryData"];
            var lastYear = 5;
            if (!String.IsNullOrEmpty(queryData))
            {
                var query = JsonHelper.ToObject(queryData);
                if (!String.IsNullOrEmpty(query.GetValue("LastYear")))
                    lastYear = Convert.ToInt32(query.GetValue("LastYear"));
            }
            var startYear = DateTime.Now.Year - 5 + 1;
            var startDate = new DateTime(startYear, 1, 1);
            var endDate = new DateTime(DateTime.Now.Year, 12, 31);

            var dt = CreateTable();
            var sql = @"select count(0) as BidCount,BusinessType from S_M_BidResult
where MakingTime>='{0}' and MakingTime<='{1}'
group by Year(MakingTime),BusinessType";
            var bidInfoDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, startDate.ToString(), endDate.ToString()));

            sql = @"select count(0) as BidCount,BusinessType from S_M_BidResult
where IsBid='{2}' and MakingTime>='{0}' and MakingTime<='{1}'
group by Year(MakingTime),BusinessType";
            var inbidInfoDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, startDate.ToString(), endDate.ToString(), "是"));

            foreach (DataRow row in dt.Rows)
            {
                var bidCount = 0m; var inbidCount = 0m; var scale = 0m;
                var bidRows = bidInfoDt.Select("BusinessType='" + row["Value"].ToString() + "'");
                if (bidRows.Length > 0)
                {
                    bidCount = bidRows[0]["BidCount"] == null || bidRows[0]["BidCount"] == DBNull.Value ? 0m : Convert.ToDecimal(bidRows[0]["BidCount"]);
                }
                var inbidRows = inbidInfoDt.Select("BusinessType='" + row["Value"].ToString() + "'");
                if (inbidRows.Length > 0)
                {
                    inbidCount = inbidRows[0]["BidCount"] == null || inbidRows[0]["BidCount"] == DBNull.Value ? 0m : Convert.ToDecimal(inbidRows[0]["BidCount"]);
                }

                if (bidCount > 0)
                {
                    scale = Math.Round(inbidCount / bidCount*100, 2);
                }
                row["BidCount"] = bidCount;
                row["InBidCount"] = inbidCount;
                row["Scale"] = scale;
            }

            var result = new Dictionary<string, object>();
            result.SetValue("data", dt);
            #region 生成图表
            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "个");
            y1.Lable.SetValue("format", "{value}个");
            var y2 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y2.TitleInfo.SetValue("text", "中标率");
            y2.Lable.SetValue("format", "{value}%"); y2.opposite = true;
            yAxies.Add(y1);
            yAxies.Add(y2);
            var serDefines = new List<Series>();
            var contractValueSer = new Series { Name = "投标数", Field = "BidCount", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            contractValueSer.Tooltip.SetValue("valueSuffix", "个");
            serDefines.Add(contractValueSer);
            var unContractValueSer = new Series { Name = "中标数", Field = "InBidCount", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            unContractValueSer.Tooltip.SetValue("valueSuffix", "个");
            serDefines.Add(unContractValueSer);

            var contractComplateRateSer = new Series { Name = "中标率", Field = "Scale", Type = "spline", yAxis = 1, Tooltip = new Dictionary<string, object>() };
            contractComplateRateSer.Tooltip.SetValue("valueSuffix", "%");
            serDefines.Add(contractComplateRateSer);
            string title = "投标结果分析";
          
            var chart = HighChartHelper.CreateColumnXYChart(title, "", dt, "text", yAxies, serDefines, null);
            result.SetValue("chartData", chart);
            #endregion

            return Json(result);

        }

        private DataTable CreateTable()
        {
            var dt = EnumBaseHelper.GetEnumTable("Base.ProjectClass");
            dt.Columns.Add("BidCount", typeof(decimal));
            dt.Columns.Add("InBidCount", typeof(decimal));
            dt.Columns.Add("Scale", typeof(decimal));
            return dt;
        }
    }
}
