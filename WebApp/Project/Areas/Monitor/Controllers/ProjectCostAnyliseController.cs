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

namespace Project.Areas.Monitor.Controllers
{
    public class ProjectCostAnyliseController : ProjectController
    {
        public ActionResult ProjectCostAnylise()
        {
            //绘制数据过滤标签
            var tab = new Tab();
            //项目费用科目
            var dt = EnumBaseHelper.GetEnumTable(typeof(CBSType));
            var cbsTypeCategory = CategoryFactory.GetCategory(typeof(CBSType), "费用科目", "CBSType", true);
            cbsTypeCategory.SetDefaultItem();
            cbsTypeCategory.Multi = false;
            tab.Categories.Add(cbsTypeCategory);
            //年份
            var yearCategory = CategoryFactory.GetYearCategory("BelongYear");
            yearCategory.SetDefaultItem();
            yearCategory.Multi = false;
            tab.Categories.Add(yearCategory);
            //月份
            var monthCategory = CategoryFactory.GetMonthCategory("BelongMonth");
            yearCategory.SetDefaultItem();
            monthCategory.Multi = false;
            tab.Categories.Add(monthCategory);
            //季度
            var quarterCategory = CategoryFactory.GetQuarterCategory("BelongQuarter");
            yearCategory.SetDefaultItem();
            quarterCategory.Multi = false;
            tab.Categories.Add(quarterCategory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCostList(QueryBuilder qb)
        {
            //费用科目查询
            var cbsType = "";
            var whereCBSType = " and isnull(ParentID,'')=''";
            foreach (var item in qb.Items)
            {
                if (item.Field == "CBSType")
                {
                    cbsType = item.Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(cbsType))
                whereCBSType = " and isnull(ParentID,'')!='' and CBSType='" + cbsType + "'";
            //年份查询
            var belongYear = "";
            var whereBelong = " ";
            foreach (var item in qb.Items)
            {
                if (item.Field == "BelongYear")
                {
                    belongYear = item.Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(belongYear))
                whereBelong = " and BelongYear='" + belongYear + "'";
            //月份查询
            var belongMonth = "";
            foreach (var item in qb.Items)
            {
                if (item.Field == "BelongMonth")
                {
                    belongMonth = item.Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(belongMonth))
                whereBelong += " and BelongMonth='" + belongMonth + "'";
            //季度查询
            var belongQuarter = "";
            foreach (var item in qb.Items)
            {
                if (item.Field == "BelongQuarter")
                {
                    belongQuarter = item.Value.ToString();
                }
            }
            if (!string.IsNullOrEmpty(belongQuarter))
                whereBelong += " and BelongQuarter='" + belongQuarter + "'";

            //获取列表数据
            var projectInfoID = GetQueryString("ProjectInfoID");
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var sql = @"select CBSCode,CBSName,sum(CBSValue) as CBSValue,sum(CostValue) as CostValue from 
                        (
                        select CBSCode,CBSName,CBSValue,
                        (select sum(CostValue) from S_C_CBSCost as c
                        inner join S_C_CBSNode as n on c.CBSNodeID=n.ID
                        where n.FullID like '%'+node.ID+'%' {0}) as CostValue
                        from S_C_CBSNode as node
                        where CBSNodeType in ('CBS','Major') and ProjectInfoID='" + projectInfoID + @"' {1}
                        ) as t
                        group by CBSCode,CBSName";
            var resultDt = sqlHelper.ExecuteDataTable(string.Format(sql, whereBelong, whereCBSType));
            //图表数据
            var result = new Dictionary<string, object>();
            result.SetValue("data", resultDt);
            var series = "";
            var serieFields = "";
            var chartDt = new DataTable();
            chartDt.Columns.Add("CBSType");
            //预算金额
            var cbsRow = chartDt.NewRow();
            cbsRow["CBSType"] = "预算金额";
            chartDt.Rows.Add(cbsRow);
            //实耗成本
            var costRow = chartDt.NewRow();
            costRow["CBSType"] = "实耗成本";
            chartDt.Rows.Add(costRow);
            if (resultDt.Rows.Count > 0)
            {
                foreach (DataRow resultDr in resultDt.Rows)
                {
                    series += resultDr["CBSName"] + ",";
                    serieFields += resultDr["CBSCode"] + ",";
                    chartDt.Columns.Add(resultDr["CBSCode"].ToString(), typeof(decimal));
                    cbsRow[resultDr["CBSCode"].ToString()] = resultDr["CBSValue"];
                    costRow[resultDr["CBSCode"].ToString()] = resultDr["CostValue"];
                }
                var chartData = HighChartHelper.CreateColumnChart("", chartDt, "CBSType", series.Trim(',').Split(','), serieFields.Trim(',').Split(','));
                result.SetValue("chartData", chartData.Render());
                return Json(result);
            }
            else
            {
                return Json("");
            }
        }
    }
}
