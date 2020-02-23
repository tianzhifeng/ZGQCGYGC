using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;
using Newtonsoft.Json;
using Formula.ImportExport;

namespace EPC.Areas.ExpenseControl.Controllers
{
    public class ComparisonController : EPCController
    {
        public ActionResult Comparison()
        {
            string sourceID = this.Request["SourceID"];
            string targetID = this.Request["TargetID"];
            var source = this.GetEntityByID<S_I_BudgetInfo>(sourceID);
            var target = this.GetEntityByID<S_I_BudgetInfo>(targetID);
            if (source == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到要对比的预算信息");
            }
            if (target == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到要对比的预算信息");
            }
            ViewBag.TargetBudget = target;
            ViewBag.SourceBudget = source;
            return View();
        }

        public JsonResult GetTreeList(string ListIDs, string ShowDetail)
        {
            var versionList = ListIDs.Split(',');
            var result = new DataTable();
            result.Columns.Add("ID");
            result.Columns.Add("ParentID");
            result.Columns.Add("Name");
            result.Columns.Add("NodeType");
            result.Columns.Add("SortIndex");
            result.Columns.Add("Diff");
            foreach (var item in versionList)
            {
                result.Columns.Add(item + "_Quantity", typeof(decimal));
                result.Columns.Add(item + "_UnitPrice", typeof(decimal));
                result.Columns.Add(item + "_TotalValue", typeof(decimal));
                result.Columns.Add(item + "_Branding");
            }

            string sql = @"select CBSID as ID,Name,Code,CBSParentID as ParentID,SortIndex,NodeType,BudgetInfoID,Branding,
UnitPrice,Quantity,TotalValue from S_I_BudgetInfo_Detail where BudgetInfoID in ('{0}') ";
            if (String.IsNullOrEmpty(ShowDetail) || ShowDetail.ToLower() != true.ToString().ToLower())
            {
                sql += " and NodeType!='Detail'";
            }
            var dataTable = this.SqlHelper.ExecuteDataTable(String.Format(sql, ListIDs.Replace(",", "','")));
            foreach (DataRow row in dataTable.Rows)
            {
                if (row["BudgetInfoID"] == null || row["BudgetInfoID"] == DBNull.Value || String.IsNullOrEmpty(row["BudgetInfoID"].ToString()))
                    continue;
                var budgetID = row["BudgetInfoID"].ToString();
                var resultRows = result.Select("ID='" + row["ID"] + "'");
                if (resultRows.Length == 0)
                {
                    var resultRow = result.NewRow();
                    resultRow["ID"] = row["ID"];
                    resultRow["ParentID"] = row["ParentID"];
                    resultRow["Name"] = row["Name"];
                    resultRow["NodeType"] = row["NodeType"];
                    resultRow["SortIndex"] = row["SortIndex"];
                    resultRow["Diff"] = false.ToString();
                    resultRow[budgetID + "_Quantity"] = row["Quantity"];
                    resultRow[budgetID + "_UnitPrice"] = row["UnitPrice"];
                    resultRow[budgetID + "_TotalValue"] = row["TotalValue"];
                    resultRow[budgetID + "_Branding"] = row["Branding"];
                    result.Rows.Add(resultRow);
                }
                else
                {
                    var resultRow = resultRows.FirstOrDefault();
                    resultRow[budgetID + "_Quantity"] = row["Quantity"];
                    resultRow[budgetID + "_UnitPrice"] = row["UnitPrice"];
                    resultRow[budgetID + "_TotalValue"] = row["TotalValue"];
                    resultRow[budgetID + "_Branding"] = row["Branding"];
                }
            }
            foreach (DataRow item in result.Rows)
            {
                var lastTotalValue = -1m;
                for (int i = 0; i < versionList.Length; i++)
                {
                    var field = versionList[i] + "_TotalValue";
                    var totalValue = item[field] == null || item[field] == DBNull.Value ? 0m : Convert.ToDecimal(item[field]);
                    if (totalValue != lastTotalValue && (lastTotalValue >= 0))
                    {
                        item["Diff"] = true.ToString();
                        break;
                    }
                    lastTotalValue = totalValue;
                }
            }
            var rows = result.Select("", "SortIndex asc");
            var resultDt = result.Clone();
            resultDt.Clear();
            foreach (var item in rows)
            {
                resultDt.ImportRow(item);
            }
            return Json(resultDt);
        }
    }
}
