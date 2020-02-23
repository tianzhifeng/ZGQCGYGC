using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using EPC.Logic.Domain;
using System.Data;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class QualityStatisticsController : EPCController
    {
        public ActionResult Project()
        {
            List<dynamic> res = new List<dynamic>();
            int count = 5;
            int nowYear = DateTime.Now.Year;
            for (int i = 0; i < count; i++)
            {
                res.Add(new { text = (nowYear - i) + "年", value = nowYear - i });
            }
            ViewBag.Years = JsonHelper.ToJson(res);
            ViewBag.CurrentYear = System.DateTime.Now.Year;
            ViewBag.CurrentMonth = System.DateTime.Now.Month;
            string engineeringID = GetQueryString("EngineeringInfoID");
            var engineering = this.GetEntityByID<S_I_Engineering>(engineeringID);
            if (engineering != null)
            {
                var qbsStruct = engineering.Mode.S_C_QBSStruct.Where(a => a.NodeType != "Root").OrderBy(a => a.SortIndex).ToList();
                var qbsEnum = qbsStruct.Select(a => new { text = a.Name, value = a.NodeType }).ToList();
                ViewBag.QBSEnum = JsonHelper.ToJson(qbsEnum);
            }
            return View();
        }

        public JsonResult GetQualityList()
        {
            Dictionary<string, object> results = new Dictionary<string, object>();
            var enumDef = EnumBaseHelper.GetEnumDef("EPC.ProblemType");
            string engineeringID = GetQueryString("EngineeringInfoID");
            var year = string.IsNullOrEmpty(GetQueryString("Year")) ? System.DateTime.Now.Year : int.Parse(GetQueryString("Year"));
            var month = string.IsNullOrEmpty(GetQueryString("Month")) ? System.DateTime.Now.Month : int.Parse(GetQueryString("Month"));
            var thisMonthF = new DateTime(year, month, 1);
            var nextMonthF = month == 12 ? new DateTime(year + 1, 1, 1) : new DateTime(year, month + 1, 1);
            var dataList = entities.Set<S_C_RectifySheet_RectifyProblems>().Where(a => a.EngineeringInfo == engineeringID).ToList();
            var dataTable = new DataTable();
            dataTable.Columns.Add("ID");
            dataTable.Columns.Add("TypeName");
            dataTable.Columns.Add("QuestionCount");
            dataTable.Columns.Add("TotalQuestionCount");
            dataTable.Columns.Add("ClosedCount");
            dataTable.Columns.Add("TotalClosedCount");
            dataTable.Columns.Add("For");
            foreach (var item in enumDef.EnumItem)
            {
                var row = dataTable.NewRow();
                row["ID"] = "";
                row["TypeName"] = item.Code;
                row["QuestionCount"] = dataList.Count(a => a.ProType == item.Code && a.OpenDate >= thisMonthF && a.OpenDate < nextMonthF);
                row["TotalQuestionCount"] = dataList.Count(a => a.ProType == item.Code && a.OpenDate < nextMonthF);
                row["ClosedCount"] = dataList.Count(a => a.ProType == item.Code && a.RectifyState == "Closed" && a.CloseDate >= thisMonthF && a.CloseDate < nextMonthF);
                row["TotalClosedCount"] = dataList.Count(a => a.ProType == item.Code && a.RectifyState == "Closed" && a.OpenDate < nextMonthF);
                row["For"] = dataList.Count(a => a.ProType == item.Code && a.RectifyState != "Closed" && a.OpenDate < nextMonthF);
                dataTable.Rows.Add(row);
            }
            results.SetValue("data", dataTable);
            results.SetValue("chartData", GetQualityCharts(dataTable, enumDef.EnumItem.ToList()));
            return Json(results);
        }

        private Dictionary<string, object> GetQualityCharts(DataTable dataTable, List<EnumItemInfo> items)
        {
            #region 绘图
            Dictionary<string, object> result = new Dictionary<string, object>();

            #region plotOptions
            Dictionary<string, object> plotOptions = new Dictionary<string, object>();
            result.SetValue("plotOptions", plotOptions);
            Dictionary<string, object> column = new Dictionary<string, object>();
            plotOptions.SetValue("column", column);

            column.SetValue("pointWidth", 15);

            #endregion

            #region title
            var titile = new Dictionary<string, object>(); titile.SetValue("text", "项目质量分析");
            result.SetValue("title", titile);
            #endregion

            #region chart
            Dictionary<string, object> chart = new Dictionary<string, object>();
            result.SetValue("chart", chart);

            chart.SetValue("height", 300);
            #endregion

            #region xAxis categories
            Dictionary<string, object> xAxis = new Dictionary<string, object>();
            List<string> categories = new List<string>();
            xAxis.SetValue("categories", categories);
            result.SetValue("xAxis", xAxis);

            var closeList = new List<int>();
            var toCloseList = new List<int>();
            foreach (var item in items)
            {
                categories.Add(item.Name);
                closeList.Add(int.Parse(dataTable.Select("TypeName = '" + item.Code + "'")[0]["TotalClosedCount"].ToString()));
                toCloseList.Add(int.Parse(dataTable.Select("TypeName = '" + item.Code + "'")[0]["For"].ToString()));
            }
            #endregion

            #region yAxis
            Dictionary<string, object> y1 = new Dictionary<string, object>();
            Dictionary<string, object> title1 = new Dictionary<string, object>();
            result.SetValue("yAxis", new List<Dictionary<string, object>>() { y1 });


            y1.SetValue("title", title1);
            title1.SetValue("text", "(个)");
            #endregion

            #region series
            //待关闭
            Dictionary<string, object> forClose = new Dictionary<string, object>();
            //已关闭
            Dictionary<string, object> closed = new Dictionary<string, object>();
            result.SetValue("series", new List<Dictionary<string, object>>() { forClose, closed });

            forClose.SetValue("name", "待关闭");
            forClose.SetValue("type", "column");
            forClose.SetValue("color", "#73BDEE");
            forClose.SetValue("width", 200);
            forClose.SetValue("data", toCloseList);

            closed.SetValue("name", "已关闭");
            closed.SetValue("type", "column");
            closed.SetValue("color", "#F59E00");
            closed.SetValue("width", 200);
            closed.SetValue("data", closeList);

            #endregion
            #endregion

            var credits = new Dictionary<string, object>();
            credits.SetValue("enabled", false);
            result.SetValue("credits", credits);

            return result;
        }

        public JsonResult GetCheckList()
        {
            Dictionary<string, object> results = new Dictionary<string, object>();
            string engineeringID = GetQueryString("EngineeringInfoID");
            var engineering = this.GetEntityByID<S_I_Engineering>(engineeringID);
            var year = string.IsNullOrEmpty(GetQueryString("Year")) ? System.DateTime.Now.Year : int.Parse(GetQueryString("Year"));
            var month = string.IsNullOrEmpty(GetQueryString("Month")) ? System.DateTime.Now.Month : int.Parse(GetQueryString("Month"));
            var thisMonthF = new DateTime(year, month, 1);
            var nextMonthF = month == 12 ? new DateTime(year + 1, 1, 1) : new DateTime(year, month + 1, 1);
            var dataList = entities.Set<S_Q_QBS>().Where(a => a.EngineeringInfoID == engineeringID).ToList();
            var dataTable = new DataTable();
            dataTable.Columns.Add("ID");
            dataTable.Columns.Add("TypeName");
            dataTable.Columns.Add("MonthCheckCount");
            dataTable.Columns.Add("TotalCheckCount");
            dataTable.Columns.Add("MonthPassCount");
            dataTable.Columns.Add("TotalPassCount");
            dataTable.Columns.Add("MonthNoPassCount");
            dataTable.Columns.Add("TotalNoPassCount");
            dataTable.Columns.Add("OneTimePassRate");

            var acceptances = entities.Set<T_Q_QBSAcceptance>().Where(a => a.EngineeringInfoID == engineeringID).ToList();
            var untilMonthAcceptances = acceptances.Where(a => a.CheckDate < nextMonthF).ToList();
            var thisMonthAcceptances = acceptances.Where(a => a.CheckDate >= thisMonthF && a.CheckDate < nextMonthF).ToList();
            var untilMonthList = new List<T_Q_QBSAcceptance_Detail>();
            var thisMonthList = new List<T_Q_QBSAcceptance_Detail>();
            foreach (var thisMonthAcceptance in thisMonthAcceptances)
                foreach (var detail in thisMonthAcceptance.T_Q_QBSAcceptance_Detail.ToList())
                    thisMonthList.Add(detail);
            foreach (var untilMonthAcceptance in untilMonthAcceptances)
                foreach (var detail in untilMonthAcceptance.T_Q_QBSAcceptance_Detail.ToList())
                    untilMonthList.Add(detail);
            var qbsStruct = engineering.Mode.S_C_QBSStruct.Where(a => a.NodeType != "Root").OrderBy(a => a.SortIndex).ToList();
            foreach (var item in qbsStruct)
            {
                var row = dataTable.NewRow();
                row["ID"] = "";
                row["TypeName"] = item.NodeType;

                row["MonthCheckCount"] = thisMonthList.Where(a => a.NodeType == item.NodeType).Select(a => a.QBSID).Distinct().Count();
                row["TotalCheckCount"] = untilMonthList.Where(a => a.NodeType == item.NodeType).Select(a => a.QBSID).Distinct().Count();
                row["MonthPassCount"] = thisMonthList.Where(a => a.NodeType == item.NodeType && decimal.Parse(a.Result) >= 60).Select(a => a.QBSID).Distinct().Count();
                row["TotalPassCount"] = untilMonthList.Where(a => a.NodeType == item.NodeType && decimal.Parse(a.Result) >= 60).Select(a => a.QBSID).Distinct().Count();
                row["MonthNoPassCount"] = int.Parse(row["MonthCheckCount"].ToString()) - int.Parse(row["MonthPassCount"].ToString());
                row["TotalNoPassCount"] = int.Parse(row["TotalCheckCount"].ToString()) - int.Parse(row["TotalPassCount"].ToString());

                var oneTimeCount = dataList.Count(a => a.NodeType == item.NodeType && a.CheckCount == 1 && a.State == "Finish");
                var checkCount = dataList.Count(a => a.NodeType == item.NodeType && a.CheckCount >= 1);
                row["OneTimePassRate"] = checkCount == 0 ? 0 : Math.Round(1.0 * oneTimeCount / checkCount * 100, 2);
                dataTable.Rows.Add(row);
            }
            results.SetValue("data", dataTable);
            results.SetValue("chartData", GetCheckListCharts(dataTable, qbsStruct));
            return Json(results);
        }

        private Dictionary<string, object> GetCheckListCharts(DataTable dataTable, List<S_C_QBSStruct> items)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            #region plotOptions
            Dictionary<string, object> plotOptions = new Dictionary<string, object>();
            result.SetValue("plotOptions", plotOptions);
            Dictionary<string, object> column = new Dictionary<string, object>();
            plotOptions.SetValue("column", column);

            column.SetValue("pointWidth", 15);

            #endregion

            #region title
            var titile = new Dictionary<string, object>(); titile.SetValue("text", "工程验收分析");
            result.SetValue("title", titile);
            #endregion

            #region chart
            Dictionary<string, object> chart = new Dictionary<string, object>();
            result.SetValue("chart", chart);

            chart.SetValue("height", 300);
            #endregion

            #region xAxis categories
            Dictionary<string, object> xAxis = new Dictionary<string, object>();
            List<string> categories = new List<string>();
            xAxis.SetValue("categories", categories);
            result.SetValue("xAxis", xAxis);

            var checkList = new List<int>();
            var passList = new List<int>();
            var toPassList = new List<int>();
            foreach (var item in items)
            {
                categories.Add(item.Name);
                checkList.Add(int.Parse(dataTable.Select("TypeName = '" + item.NodeType + "'")[0]["TotalCheckCount"].ToString()));
                passList.Add(int.Parse(dataTable.Select("TypeName = '" + item.NodeType + "'")[0]["TotalPassCount"].ToString()));
                toPassList.Add(int.Parse(dataTable.Select("TypeName = '" + item.NodeType + "'")[0]["TotalNoPassCount"].ToString()));
            }
            #endregion

            #region yAxis
            Dictionary<string, object> y1 = new Dictionary<string, object>();
            Dictionary<string, object> title1 = new Dictionary<string, object>();
            result.SetValue("yAxis", new List<Dictionary<string, object>>() { y1 });


            y1.SetValue("title", title1);
            title1.SetValue("text", "(个)");
            #endregion

            #region series
            Dictionary<string, object> check = new Dictionary<string, object>();
            Dictionary<string, object> pass = new Dictionary<string, object>();
            Dictionary<string, object> toPass = new Dictionary<string, object>();
            result.SetValue("series", new List<Dictionary<string, object>>() { check, pass, toPass });

            check.SetValue("name", "验收数量");
            check.SetValue("type", "column");
            check.SetValue("color", "#73BDEE");
            check.SetValue("width", 200);
            check.SetValue("data", checkList);

            pass.SetValue("name", "合格数量");
            pass.SetValue("type", "column");
            pass.SetValue("color", "#F59E00");
            pass.SetValue("width", 200);
            pass.SetValue("data", passList);

            toPass.SetValue("name", "不合格数量");
            toPass.SetValue("type", "column");
            toPass.SetValue("color", "#BA55D3");
            toPass.SetValue("width", 200);
            toPass.SetValue("data", toPassList);

            #endregion

            var credits = new Dictionary<string, object>();
            credits.SetValue("enabled", false);
            result.SetValue("credits", credits);

            return result;
        }
    }
}
