using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;

namespace Project.Areas.Basic.Controllers
{
    public class WorkloadAnalysisController : ProjectController
    {
        static string[] colors = new string[] { "#168C8C", "#F59E00", "#B5E6D9", "#B4D9C4", "#BCD97E", "#99C53B" };
        public ActionResult WorkloadAnalysis()
        {
            ViewBag.ProjectInfoID = GetQueryString("ProjectInfoID");
            return View();
        }

        public JsonResult GetTree(string ProjectInfoID, bool IsShowTaskWork)
        {
            var result = new List<Dictionary<string, object>>();
            var CBSList = this.entities.Set<S_C_CBS>().Where(a => a.ProjectInfoID == ProjectInfoID).ToList();
            var CBSBudgetList = this.entities.Set<S_C_CBS_Budget>().Where(a => a.ProjectInfoID == ProjectInfoID && String.IsNullOrEmpty(a.ParentID)).ToList();
            foreach (var cbs in CBSList)
            {
                var item = cbs.ToDic();
                item.SetValue("VirtualID", cbs.ID);
                item.SetValue("WBSType", cbs.NodeType);
                if (cbs.NodeType == CBSNodeType.Category.ToString())
                    item.SetValue("WBSType", "SubProject");
                item.SetValue("Code", "");
                if (cbs.SummaryBudgetQuantity == null)
                    item.SetValue("SummaryBudgetQuantity", "0");
                if (cbs.SummaryCostQuantity == null)
                    item.SetValue("SummaryCostQuantity", "0");

                item.SetValue("Rate", Math.Round((decimal)((cbs.SummaryCostQuantity == null ? 0 : cbs.SummaryCostQuantity) / cbs.Quantity * 100), 2));
                result.Add(item);
            }
            if (IsShowTaskWork)
            {
                foreach (var cbsBudget in CBSBudgetList)
                {
                    var item = cbsBudget.ToDic();
                    item.SetValue("VirtualID", cbsBudget.ID);
                    item.SetValue("ParentID", cbsBudget.CBSID);
                    item.SetValue("WBSType", "Work");
                    var parent = CBSList.FirstOrDefault(a => a.ID == cbsBudget.CBSID);
                    if (parent != null)
                        item.SetValue("ParentCode", parent.Code);
                    item.SetValue("SummaryBudgetQuantity", cbsBudget.Quantity);
                    if (cbsBudget.SummaryCostQuantity == null)
                        item.SetValue("SummaryCostQuantity", "0");
                    item.SetValue("Rate", Math.Round((decimal)((cbsBudget.SummaryCostQuantity == null ? 0 : cbsBudget.SummaryCostQuantity) / cbsBudget.Quantity * 100), 2));
                    result.Add(item);
                }
            }
            return Json(result);
        }

        public JsonResult GetPieChartData(string ProjectInfoID, string ID, string Type = null)
        {
            if (String.IsNullOrEmpty(Type)) { }
            var sql = @"
select '已结算' ItemName,ISnull(SummaryCostQuantity,0) ItemValue from {2} where
ProjectInfoID='{0}' {1}
union 
select '未结算' ItemName,Quantity - ISnull(SummaryCostQuantity,0) ItemValue from {2} where
ProjectInfoID='{0}' {1}
";
            var whereStr = "";
            if (!String.IsNullOrEmpty(ID))
                whereStr = String.Format("and ID='{0}'", ID);
            else
                whereStr = "and NodeType = 'Root'";
            if (String.IsNullOrEmpty(Type))
                sql = String.Format(sql, ProjectInfoID, whereStr, "S_C_CBS");
            else
                sql = String.Format(sql, ProjectInfoID, whereStr, "S_C_CBS_Budget");
            var data = this.SqlHelper.ExecuteDataTable(sql);

            var result = new Dictionary<string, object>();
            var chat = HighChartHelper.CreatePieChart("项目定额工时结算情况分析", "项目定额工时结算情况分析", data, "ItemName", "ItemValue").Render();
            var plot = (Dictionary<string, object>)chat["plotOptions"];
            var pie = (Dictionary<string, object>)plot["pie"];
            var dataLabels = (Dictionary<string, object>)pie["dataLabels"];
            dataLabels.SetValue("format", "<b>{point.name}</b>: {point.percentage:.1f} %");
            pie.SetValue("dataLabels", dataLabels);
            plot.SetValue("pie", pie);
            chat.SetValue("plotOptions", plot);
            result["chartData"] = chat;

            return Json(result);
        }

        public JsonResult GetXYChartData(string ProjectInfoID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("找不到指定的项目信息。");
            var wbsMajor = projectInfo.S_W_WBS.Where(a => a.WBSType == "Major").ToList();
            var majorNames = wbsMajor.OrderBy(a => a.SortIndex).Select(a => a.Name).ToArray();
            var majorValues = wbsMajor.OrderBy(a => a.SortIndex).Select(a => a.Code).ToArray();

            var sqlList = new List<string>();
            var sqlItem = @"
select '工时总数' ItemName,ISnull(Quantity,0) ItemValue,Code,Name from S_C_CBS where
ProjectInfoID='{0}' and NodeType = 'Major' and Code = '{1}'
union 
select '已结算' ItemName,ISnull(SummaryCostQuantity,0) ItemValue,Code,Name from S_C_CBS where
ProjectInfoID='{0}' and NodeType = 'Major' and Code = '{1}'
";
            foreach (var major in majorValues)
                sqlList.Add(string.Format(sqlItem, ProjectInfoID, major));

            var result = new Dictionary<string, object>();
            if (sqlList.Count > 0)
            {
                var sql = String.Join("union\n", sqlList);
                var data = this.SqlHelper.ExecuteDataTable(sql);

                var dataSource = new DataTable();
                dataSource.Columns.Add("Name", typeof(string));
                dataSource.Columns.Add("工时总数", typeof(decimal));
                dataSource.Columns.Add("已结算", typeof(decimal));
                string[] array = new string[2] { "工时总数", "已结算" };

                foreach (var major in majorNames)
                {
                    var row = dataSource.NewRow();
                    row["Name"] = major;
                    var total = 0m; var finish = 0m;
                    if (data.Rows.Count > 0)
                    {
                        total = Convert.ToDecimal(data.Select("Name='" + major + "' and ItemName='工时总数'")[0]["ItemValue"]);
                        finish = Convert.ToDecimal(data.Select("Name='" + major + "' and ItemName='已结算'")[0]["ItemValue"]); 
                    }
                    row["工时总数"] = total;
                    row["已结算"] = finish;
                    dataSource.Rows.Add(row);
                }

                var chat = HighChartHelper.CreateColumnChart("各专业定额工时分析", dataSource, "Name", array, array).Render();
                var chart = (Dictionary<string, object>)chat["chart"];
                chart.SetValue("type", "bar");
                chat.SetValue("chart", chart);

                result["chartData"] = chat;
            }
            return Json(result);
        }
    }
}
