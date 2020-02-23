using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using EPC.Logic.Domain;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class CostAnalysisController : EPCController
    {
        public ActionResult Index()
        {
            List<dynamic> res = new List<dynamic>();
            int count = 5;
            int nowYear = DateTime.Now.Year;
            for (int i = 0; i < count; i++)
            {
                res.Add(new { text = (nowYear - i) + "年", value = i });
            }
            ViewBag.Years = JsonHelper.ToJson(res);
            return View();
        }

        //public JsonResult GetLastYear()
        //{
        //    List<dynamic> res = new List<dynamic>();
        //    int count = 5;
        //    int nowYear = DateTime.Now.Year;
        //    for (int i = 0; i < count; i++)
        //    {
        //        res.Add(new { text = (nowYear - i), value = i });
        //    }
        //    return Json(res, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetTreegridResult()
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");

            #region 招投标
            //获取S_M_BidOffer最低版本的id
            string tmpSqlOfS_M_BidOffer = "SELECT top 1 ID FROM S_M_BidOffer where FlowPhase = 'End' and EngineeringInfoID = '"
                + engineeringInfoID + "'  ORDER BY CAST(VersionNumber AS DECIMAL)";
            var tableRes = SqlHelper.ExecuteDataTable(tmpSqlOfS_M_BidOffer);
            string bidOfferID = "";
            if (tableRes != null && tableRes.Rows.Count == 1)
            {
                bidOfferID = tableRes.Rows[0][0].ToString();
            }

            //cbs
            var zeroBidQueryCBS = entities.Set<S_M_BidOffer_CBS>().Where(a => a.BidOfferID == bidOfferID);

            var zeroBidQuery = zeroBidQueryCBS.Select(a => new
            {
                CBSID = a.CBSID,
                ZeroTender = a.TotalValue
            });
            #endregion


            #region 预算
            //获取S_I_BudgetInfo最低版本的id
            string tmpSqlOfS_I_BudgetInfo = "SELECT top 1 ID FROM S_I_BudgetInfo where FlowPhase = 'End' and EngineeringInfoID = '"
                + engineeringInfoID + "'  ORDER BY CAST(VersionNumber AS DECIMAL)";
            var tableRes2 = SqlHelper.ExecuteDataTable(tmpSqlOfS_I_BudgetInfo);
            string budgetInfoID = "";
            if (tableRes != null && tableRes2.Rows.Count == 1)
            {
                budgetInfoID = tableRes2.Rows[0][0].ToString();
            }

            //查找S_I_BudgetInfo通过的cbs及金额
            //S_I_BudgetInfo_Detail完整包括从工程节点到最低阶子节点
            var zeroBudgetInfo = entities.Set<S_I_BudgetInfo_Detail>().Where(a => a.BudgetInfoID == budgetInfoID);

            var zeroBudgetQuery = zeroBudgetInfo.Select(a => new
            {
                CBSID = a.CBSID,
                // Name = a.Name,
                ZeroBudget = a.TotalValue
            });
            #endregion

            var results = from a in entities.Set<S_I_CBS>().Where(a => a.EngineeringInfoID == engineeringInfoID)
                          join b in zeroBidQuery
                          on a.ID equals b.CBSID into tmp1
                          from dept1 in tmp1.DefaultIfEmpty()
                          join c in zeroBudgetQuery
                          on a.ID equals c.CBSID into tmp2
                          from dept2 in tmp2.DefaultIfEmpty()
                          select new
                          {
                              EngineeringInfoID = a.EngineeringInfoID,
                              CBSParentID = a.ParentID,
                              CBSID = a.ID,
                              Name = a.Name,
                              Tender = a.Tender ?? 0,
                              ZeroTender = (dept1.ZeroTender ?? 0),
                              ZeroBudget = (dept2.ZeroBudget ?? 0),
                              Budget = a.Budget ?? 0,
                              Contract = a.Contract ?? 0,
                              Settle = a.Settle ?? 0
                          };

            return Json(results.ToList());
        }

        private Dictionary<string, object> GetTreegridData(string engineeringInfoID)
        {
            return null;
        }

        public JsonResult GetChartData(int year, string engineeringInfoID)
        {
            #region 查询
            var costResult = entities.Set<S_I_CBS_Cost>().Where(a => a.CostDate.Value.Year == year && a.EngineeringInfoID == engineeringInfoID);
            List<decimal> dMonthCost = new List<decimal>(); //当月 
            List<decimal> dMonthTotalCost = new List<decimal>();//当月累计

            //预算
            //var budgetResult = entities.Set<S_I_BudgetInfo>().Where(a => a.FlowPhase == "End" && a.);

            decimal tmpMonthTotal = 0;
            for (int i = 1; i <= 12; i++)
            {
                decimal tmp = costResult.Where(a => a.CostDate.Value.Month == i).Sum(a => a.TotalValue) ?? 0;
                tmp /= 10000;
                tmpMonthTotal += tmp;
                dMonthCost.Add(tmp);
                dMonthTotalCost.Add(tmpMonthTotal);
            }

            #endregion

            #region 绘图
            Dictionary<string, object> result = new Dictionary<string, object>();

            #region plotOptions
            Dictionary<string, object> plotOptions = new Dictionary<string, object>();
            result.SetValue("plotOptions", plotOptions);
            Dictionary<string, object> column = new Dictionary<string, object>();
            plotOptions.SetValue("column", column);

            column.SetValue("pointWidth", 25);

            #endregion

            #region title
            result.SetValue("title", "");
            #endregion

            #region chart
            Dictionary<string, object> chart = new Dictionary<string, object>();
            result.SetValue("chart", chart);

            chart.SetValue("height", 200);
            #endregion

            #region xAxis categories
            Dictionary<string, object> xAxis = new Dictionary<string, object>();
            List<string> categories = new List<string>();
            xAxis.SetValue("categories", categories);
            result.SetValue("xAxis", xAxis);

            categories.Add("一月");
            categories.Add("二月");
            categories.Add("三月");
            categories.Add("四月");
            categories.Add("五月");
            categories.Add("六月");
            categories.Add("七月");
            categories.Add("八月");
            categories.Add("九月");
            categories.Add("十月");
            categories.Add("十一月");
            categories.Add("十二月");
            #endregion

            #region yAxis
            Dictionary<string, object> y1 = new Dictionary<string, object>();
            Dictionary<string, object> y2 = new Dictionary<string, object>();
            Dictionary<string, object> title1 = new Dictionary<string, object>();
            Dictionary<string, object> title2 = new Dictionary<string, object>();
            result.SetValue("yAxis", new List<Dictionary<string, object>>() { y1, y2 });


            y1.SetValue("title", title1);
            y2.SetValue("title", title2);
            y2.SetValue("opposite", true);
            title1.SetValue("text", "(万元)");
            title2.SetValue("text", "(万元)");
            #endregion

            #region series
            //当月支出
            Dictionary<string, object> monthCost = new Dictionary<string, object>();
            //当月累计支出
            Dictionary<string, object> monthTotalCost = new Dictionary<string, object>();
            //预算线
            Dictionary<string, object> budgetCost = new Dictionary<string, object>();
            result.SetValue("series", new List<Dictionary<string, object>>() { monthCost, monthTotalCost
                //, budgetCost 
            });

            monthCost.SetValue("name", "当月支出");
            monthCost.SetValue("type", "column");
            monthCost.SetValue("color", "#73BDEE");
            monthCost.SetValue("width", 200);
            monthCost.SetValue("data", dMonthCost);

            monthTotalCost.SetValue("name", "当月累计支出");
            monthTotalCost.SetValue("type", "spline");
            monthTotalCost.SetValue("yAxis", 1);
            monthTotalCost.SetValue("color", "#FEBF2C");
            monthTotalCost.SetValue("data", dMonthTotalCost);

            budgetCost.SetValue("name", "预算线");
            budgetCost.SetValue("type", "spline");
            budgetCost.SetValue("yAxis", 1);
            budgetCost.SetValue("color", "#B93636");
            Dictionary<string, object> marker = new Dictionary<string, object>();
            marker.SetValue("radius", 0);
            budgetCost.SetValue("marker", marker);
            List<int> budgetCostValue = new List<int>() { 150, 73, 20, 200, 73, 20, 150, 73, 20, 150, 73, 20 };
            budgetCost.SetValue("data", budgetCostValue);
            #endregion
            #endregion

            return Json(result); ;
        }
    }
}
