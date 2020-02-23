using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Collections;
using Formula.Helper;
using Formula;
using MvcAdapter;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class DeptAnalysisController : EPCController
    {
        public ActionResult DeptIndicatorInfo()
        {
            ViewBag.ServerYear = DateTime.Now.Year;
            return View();
        }

        public ActionResult ContractView()
        {
            var tab = new Tab();
            var deptCategory = CategoryFactory.GetCategory("System.ManDept", "部门", "ProductionDept");
            deptCategory.SetDefaultItem();
            tab.Categories.Add(deptCategory);

            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 7);
            yearCategory.SetDefaultItem();
            tab.Categories.Add(yearCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public ActionResult ProjectView()
        {
            var tab = new Tab();
            var deptCategory = CategoryFactory.GetCategory("System.ManDept", "部门", "ChargerDept");
            deptCategory.SetDefaultItem();
            tab.Categories.Add(deptCategory);

            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 7);
            yearCategory.SetDefaultItem();
            tab.Categories.Add(yearCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetContractList(QueryBuilder qb, string Year)
        {
            var receiptSQL = @"SELECT  ISNULL(SUM(ReceiptInfo.ReceiptValue), 0) AS Value ,
                                        ProductionDept
                                FROM    ( SELECT    S_M_Receipt.* ,
                                                    YEAR(ReceiptDate) AS BelongYear,
                                                    ProductionDept ,
                                                    ProductionDeptName
                                          FROM      S_M_Receipt
                                                    LEFT JOIN S_M_ContractInfo ON ContractInfo = S_M_ContractInfo.ID
                                        ) ReceiptInfo
                                WHERE   1 = 1 {0}
                                GROUP BY ProductionDept;";
            var invoiceSQL = @"SELECT  ISNULL(SUM(ReceiptInfo.InvoiceValue), 0) AS Value ,
                                        ProductionDept
                                FROM    ( SELECT    S_M_Invoice.* ,
                                                    YEAR(InvoiceDate) AS BelongYear,
                                                    ProductionDept ,
                                                    ProductionDeptName
                                          FROM      S_M_Invoice
                                                    LEFT JOIN S_M_ContractInfo ON ContractInfo = S_M_ContractInfo.ID
                                        ) ReceiptInfo
                                WHERE   State = 'Normal' {0}
                                GROUP BY ProductionDept;";
            var contractSQL = @"SELECT  ISNULL(SUM(ContractRMBValue), 0) AS Value ,
                                        ISNULL(SUM(SumBadDebtValue), 0) AS BadValue ,
                                        YEAR(SignDate) AS BelongYear,
                                        ProductionDept
                                FROM    S_M_ContractInfo
                                WHERE   ContractState='Sign' {0}
                                GROUP BY ProductionDept;";

            string innerWhere = string.Empty;
            if (!String.IsNullOrEmpty(Year) && Year.ToLowerInvariant() != "all")
                innerWhere += " and BelongYear in ('" + Year.Replace(",", "','") + "')";
            receiptSQL = String.Format(receiptSQL, innerWhere);
            invoiceSQL = String.Format(invoiceSQL, innerWhere);
            contractSQL = String.Format(contractSQL, innerWhere);
            var receiptDt = this.SqlHelper.ExecuteDataTable(receiptSQL);
            var invoiceDt = this.SqlHelper.ExecuteDataTable(invoiceSQL);
            var contractDt = this.SqlHelper.ExecuteDataTable(contractSQL);

            var deptField = "ProductionDept";
            var deptNameField = "ProductionDeptName";
            var resultDt = CreateDefaultTable(qb, deptField, deptNameField);
            foreach (DataRow item in resultDt.Rows)
            {
                var receiptRow = receiptDt.Select("ProductionDept='" + item[deptField] + "'").FirstOrDefault();
                if (receiptRow != null)
                    item["ReceiptValue"] = receiptRow["Value"];
                var invoiceRow = invoiceDt.Select("ProductionDept='" + item[deptField] + "'").FirstOrDefault();
                if (invoiceRow != null)
                    item["InvoiceValue"] = invoiceRow["Value"];
                var contractRow = contractDt.Select("ProductionDept='" + item[deptField] + "'").FirstOrDefault();
                if (contractRow != null)
                {
                    item["ContractValue"] = contractRow["Value"];
                    item["BadDebtValue"] = contractRow["BadValue"];
                }
                item["RemainValue"] = Convert.ToDecimal(item["ContractValue"]) -
                    Convert.ToDecimal(item["ReceiptValue"]) - Convert.ToDecimal(item["BadDebtValue"]);
                item["ReciveableValue"] = Convert.ToDecimal(item["InvoiceValue"]) -
                    Convert.ToDecimal(item["ReceiptValue"]) - Convert.ToDecimal(item["BadDebtValue"]);
            }

            var result = new Dictionary<string, object>();
            result.SetValue("data", resultDt);
            var series = "合同总额,收款总额,开票总额,坏账总额,应收款总额,剩余合同额";
            var serieFields = "ContractValue,ReceiptValue,InvoiceValue,BadDebtValue,ReciveableValue,RemainValue";
            var chartData = HighChartHelper.CreateColumnChart("", resultDt, deptNameField, series.Split(','), serieFields.Split(','));
            result.SetValue("chartData", chartData.Render());
            return Json(result);
        }

        public JsonResult GetProjectList(QueryBuilder qb, string Year)
        {
            var receiptSQL = @"SELECT isnull(Sum(RelationValue),0) as Value,ChargerDept FROM (
                                select * FROM(
                                select RelationValue,ChargerDept,YEAR(PlanDate) AS BelongYear
                                from S_M_Receipt_PlanRelation left join S_M_Receipt on S_M_ReceiptID = S_M_Receipt.ID
                                left join S_M_ContractInfo_ReceiptObj on ReceiptObjID = S_M_ContractInfo_ReceiptObj.ID
                                left join S_I_Engineering on  S_M_ContractInfo_ReceiptObj.ProjectInfo= S_I_Engineering.ID
                                )midTable
                                WHERE 1=1 {0})result
                                group by ChargerDept";
            var invoiceSQL = @"select isnull(Sum(RelationValue),0) as Value,ChargerDept FROM(
                                SELECT * FROM (SELECT RelationValue,ChargerDept,YEAR(InvoiceDate) AS BelongYear,S_M_Invoice.State
                                from S_M_Invoice_ReceiptObjRelation left join S_M_Invoice on S_M_InvoiceID = S_M_Invoice.ID
                                left join S_M_ContractInfo_ReceiptObj on S_M_ReceiptObjID = S_M_ContractInfo_ReceiptObj.ID
                                left join S_I_Engineering on ProjectInfo = S_I_Engineering.ID
                                )midtable
                                where State = 'Normal' {0}
                                )result
                                group by ChargerDept";
            var contractSQL = @"select isnull(Sum(ReceiptValue),0) as Value,isnull(Sum(FactBadValue),0) as BadValue,ChargerDept FROM
                                (
                                SELECT * FROM (  
                                SELECT ReceiptValue,FactBadValue,S_I_Engineering.ChargerDept,YEAR(SignDate) AS BelongYear,ContractState
                                FROM S_M_ContractInfo_ReceiptObj
                                left join S_I_Engineering on ProjectInfo = S_I_Engineering.ID
                                left join S_M_ContractInfo on S_M_ContractInfo_ReceiptObj.S_M_ContractInfoID = S_M_ContractInfo.ID
                                )midTable
                                where ContractState='Sign' {0}
                                )result
                                group by ChargerDept";

            string innerWhere = string.Empty;
            if (!String.IsNullOrEmpty(Year) && Year.ToLowerInvariant() != "all")
                innerWhere += " and BelongYear in ('" + Year.Replace(",", "','") + "')";
            receiptSQL = String.Format(receiptSQL, innerWhere);
            invoiceSQL = String.Format(invoiceSQL, innerWhere);
            contractSQL = String.Format(contractSQL, innerWhere);
            var receiptDt = this.SqlHelper.ExecuteDataTable(receiptSQL);
            var invoiceDt = this.SqlHelper.ExecuteDataTable(invoiceSQL);
            var contractDt = this.SqlHelper.ExecuteDataTable(contractSQL);

            var resultDt = CreateDefaultTable(qb);
            foreach (DataRow item in resultDt.Rows)
            {
                var receiptRow = receiptDt.Select("ChargerDept='" + item["ChargerDept"] + "'").FirstOrDefault();
                if (receiptRow != null)
                    item["ReceiptValue"] = receiptRow["Value"];
                var invoiceRow = invoiceDt.Select("ChargerDept='" + item["ChargerDept"] + "'").FirstOrDefault();
                if (invoiceRow != null)
                    item["InvoiceValue"] = invoiceRow["Value"];
                var contractRow = contractDt.Select("ChargerDept='" + item["ChargerDept"] + "'").FirstOrDefault();
                if (contractRow != null)
                {
                    item["ContractValue"] = contractRow["Value"];
                    item["BadDebtValue"] = contractRow["BadValue"];
                }
                item["RemainValue"] = Convert.ToDecimal(item["ContractValue"]) -
                    Convert.ToDecimal(item["ReceiptValue"]) - Convert.ToDecimal(item["BadDebtValue"]);
                item["ReciveableValue"] = Convert.ToDecimal(item["InvoiceValue"]) -
                    Convert.ToDecimal(item["ReceiptValue"]) - Convert.ToDecimal(item["BadDebtValue"]);
            }

            var result = new Dictionary<string, object>();
            result.SetValue("data", resultDt);
            var series = "合同总额,收款总额,开票总额,坏账总额,应收款总额,剩余合同额";
            var serieFields = "ContractValue,ReceiptValue,InvoiceValue,BadDebtValue,ReciveableValue,RemainValue";
            var chartData = HighChartHelper.CreateColumnChart("", resultDt, "ChargerDeptName", series.Split(','), serieFields.Split(','));
            result.SetValue("chartData", chartData.Render());
            return Json(result);
        }

        private DataTable CreateDefaultTable(QueryBuilder qb, string deptField = "ChargerDept", string deptNameField = "ChargerDeptName")
        {
            string sql = @"select * from (select ID as " + deptField + ",Name as " + deptNameField + @",SortIndex,0 as ContractValue,
0 as ReceiptValue,0 as InvoiceValue,0 as BadDebtValue,0 as RemainValue,0 as ReciveableValue
from S_A_Org where  Type='ManufactureDept' ) TableInfo ";
            var baseSQLHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = baseSQLHelper.ExecuteDataTable(sql, qb);
            return dt;
        }

        #region 各部门目标完成情况统计表

        public JsonResult GetDeptIndicatorList(string anlysisValue = "ReceiptValue")
        {
            var dt = EnumBaseHelper.GetEnumTable("System.ManDept");
            dt.Columns.Add("ContractKPI", typeof(decimal));
            dt.Columns.Add("ContractValue", typeof(decimal));
            dt.Columns.Add("ContractComplateRate", typeof(decimal));
            dt.Columns.Add("UnContractValue", typeof(decimal));
            dt.Columns.Add("ContractKPIRemain", typeof(decimal));
            dt.Columns.Add("ReceiptKPI", typeof(decimal));
            dt.Columns.Add("ReceiptValue", typeof(decimal));
            dt.Columns.Add("RecepitComplateRate", typeof(decimal));
            dt.Columns.Add("CanReceiptValue", typeof(decimal));
            dt.Columns.Add("RemaintContractValue", typeof(decimal));
            dt.Columns.Add("ReceiptKPIRemain", typeof(decimal));
            dt.Columns.Add("TimeRate", typeof(decimal));

            var belongYear = String.IsNullOrEmpty(GetQueryString("BelongYear")) ? DateTime.Now.Year : Convert.ToInt32(GetQueryString("BelongYear"));
            var sql = "select * from S_KPI_IndicatorOrg where IndicatorType = 'YearIndicator' and BelongYear= '{0}'";
            var indicatorDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, belongYear));

            sql = @"SELECT  ISNULL(SUM(ReceiptInfo.ReceiptValue), 0) AS Value ,
                            ProductionDept,ProductionDeptName
                    FROM    ( SELECT    ReceiptValue,
                                        ProductionDept,ProductionDeptName
                              FROM      S_M_Receipt 
                                        LEFT JOIN S_M_ContractInfo ON ContractInfo = S_M_ContractInfo.ID
		                      WHERE YEAR(ReceiptDate)='{0}'
                            ) ReceiptInfo
                    WHERE   1 = 1
                    GROUP BY ProductionDept,ProductionDeptName; ";
            var receiptDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, belongYear));

            sql = @"  select isnull(Sum(isnull(ReceiptValue,0)-isnull(FactReceiptValue,0)),0)  as DataValue,ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName 
                      FROM S_M_ContractInfo_ReceiptObj
                      left join S_M_ContractInfo on S_M_ContractInfo_ReceiptObj.S_M_ContractInfoID=S_M_ContractInfo.ID
                      group by ProductionDept,ProductionDeptName";
            var canReceiptDt = this.SqlHelper.ExecuteDataTable(sql);

            sql = @" select Sum(isnull(ContractRMBValue,0)-isnull(SumReceiptValue,0)) as DataValue,ProductionDept as ChargeDeptID,
                     ProductionDeptName as ChargeDeptName 
                     from S_M_ContractInfo where ContractState='Sign' group by ProductionDept,ProductionDeptName";
            var remainContractDt = this.SqlHelper.ExecuteDataTable(sql);

            sql = @" SELECT isnull(Sum(ContractRMBValue),0) as DataValue,ChargeDeptID,ChargeDeptName,BelongYear,BelongMonth,BelongQuarter
                     FROM
                    (select ContractRMBValue,ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName,
                    YEAR(SignDate) AS BelongYear,MONTH(SignDate) AS BelongMonth,DATEPART(QUARTER,SignDate) AS BelongQuarter
                    from S_M_ContractInfo 
                    WHERE ContractState='Sign' and YEAR(SignDate)='{0}')result
                    GROUP by BelongYear,BelongQuarter,BelongMonth,ChargeDeptID,ChargeDeptName ";
            var contractDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, belongYear));

            sql = @"select isnull(Sum(ContractRMBValue),0) as DataValue,ChargeDeptID,ChargeDeptName,BelongYear,BelongMonth,BelongQuarter
                    FROM(
                    SELECT ContractRMBValue,ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName,
                    YEAR(SignDate) AS BelongYear,MONTH(SignDate) AS BelongMonth,DATEPART(QUARTER,SignDate) AS BelongQuarter
                    FROM dbo.S_M_ContractInfo
                    WHERE ContractState!='Sign' OR ContractState IS NULL
                    )resulttable
                    GROUP BY ChargeDeptID,ChargeDeptName,BelongYear,BelongMonth,BelongQuarter";
            var UncCntractDt = this.SqlHelper.ExecuteDataTable(sql);

            var sumReceptKpi = 0m; var sumReceiptValue = 0m; var sumContractKpi = 0m; var sumContractValue = 0m;
            foreach (DataRow row in dt.Rows)
            {
                var obj = indicatorDt.Compute("Sum(ReceiptValue)", "OrgID='" + row["value"] + "'");
                var receptkpi = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ReceiptKPI"] = receptkpi;
                sumReceptKpi += receptkpi;
                obj = receiptDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                var recepitValue = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ReceiptValue"] = recepitValue;
                sumReceiptValue += recepitValue;
                row["RecepitComplateRate"] = receptkpi == 0 ? 100 : Math.Round(recepitValue / receptkpi * 100, 2);

                obj = canReceiptDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                row["CanReceiptValue"] = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["RemaintContractValue"] = remainContractDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                row["ReceiptKPIRemain"] = receptkpi - recepitValue;
                //row["TimeRate"] = Math.Round(Convert.ToDecimal(DateTime.Now.DayOfYear) / 365 * 100);

                obj = indicatorDt.Compute("Sum(ContractValue)", "OrgID='" + row["value"] + "'");
                var contractKpi = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ContractKPI"] = contractKpi;
                sumContractKpi += contractKpi;

                obj = contractDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                var value = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ContractValue"] = value;
                sumContractValue += value;
                row["ContractComplateRate"] = contractKpi == 0 ? 100 : Math.Round(value / contractKpi * 100, 2);

                obj = UncCntractDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                row["UnContractValue"] = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ContractKPIRemain"] = contractKpi - value;
            }
            var result = new Dictionary<string, object>();
            result.SetValue("data", dt);
            var sumData = new Dictionary<string, object>();
            sumData.SetValue("RecepitComplateRate", sumReceptKpi == 0 ? 100 : Math.Round(sumReceiptValue / sumReceptKpi * 100, 2));
            sumData.SetValue("ContractComplateRate", sumContractKpi == 0 ? 100 : Math.Round(sumReceiptValue / sumContractKpi * 100, 2));
            result.SetValue("sumData", sumData);

            #region 生成图表
            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "金额");
            y1.Lable.SetValue("format", "{value}元");
            var y2 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y2.TitleInfo.SetValue("text", "完成率");
            y2.Lable.SetValue("format", "{value}%"); y2.opposite = true;
            yAxies.Add(y1);
            yAxies.Add(y2);

            var serDefines = new List<Series>();
            if (anlysisValue == AnlysisValue.ReceiptValue.ToString())
            {
                var ReceiptKPISer = new Series { Name = "收款目标", Field = "ReceiptKPI", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
                ReceiptKPISer.Tooltip.SetValue("valueSuffix", "元");
                serDefines.Add(ReceiptKPISer);

                var ReceiptValueSer = new Series { Name = "已收款", Field = "ReceiptValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
                ReceiptValueSer.Tooltip.SetValue("valueSuffix", "元");
                serDefines.Add(ReceiptValueSer);

                var CanReceiptValueSer = new Series { Name = "经营应收款", Field = "CanReceiptValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
                CanReceiptValueSer.Tooltip.SetValue("valueSuffix", "元");
                serDefines.Add(CanReceiptValueSer);

                var RecepitComplateRateSer = new Series { Name = "收款完成率", Field = "RecepitComplateRate", Type = "spline", yAxis = 1, Tooltip = new Dictionary<string, object>() };
                RecepitComplateRateSer.Tooltip.SetValue("valueSuffix", "%");
                serDefines.Add(RecepitComplateRateSer);
            }
            else
            {
                var contractKPISer = new Series { Name = "合同目标", Field = "ContractKPI", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
                contractKPISer.Tooltip.SetValue("valueSuffix", "元");
                serDefines.Add(contractKPISer);

                var contractValueSer = new Series { Name = "已签订金额", Field = "ContractValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
                contractValueSer.Tooltip.SetValue("valueSuffix", "元");
                serDefines.Add(contractValueSer);

                var unContractValueSer = new Series { Name = "待签约金额", Field = "UnContractValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
                unContractValueSer.Tooltip.SetValue("valueSuffix", "元");
                serDefines.Add(unContractValueSer);

                var contractComplateRateSer = new Series { Name = "合同完成率", Field = "ContractComplateRate", Type = "spline", yAxis = 1, Tooltip = new Dictionary<string, object>() };
                contractComplateRateSer.Tooltip.SetValue("valueSuffix", "%");
                serDefines.Add(contractComplateRateSer);

            }

            //var TimeRateSer = new Series { Name = "时间", Field = "TimeRate", Type = "spline", yAxis = 1, Tooltip = new Dictionary<string, object>() };
            //TimeRateSer.Tooltip.SetValue("valueSuffix", "%");
            //serDefines.Add(TimeRateSer);
            string title = belongYear + "年各部门收款完成情况";
            if (anlysisValue == AnlysisValue.ContractValue.ToString())
            {
                title = belongYear + "年各部门合同完成情况";
            }
            var chart = HighChartHelper.CreateColumnXYChart(title, "", dt, "text", yAxies, serDefines, null);
            result.SetValue("chart", chart);
            #endregion

            return Json(result);
        }

        #endregion

        #region 部门收款情况统计表

        public JsonResult GetDeptReceiptList()
        {

            var dt = EnumBaseHelper.GetEnumTable("System.ManDept");
            dt.Columns.Add("ReceiptKPI", typeof(decimal));
            dt.Columns.Add("ReceiptValue", typeof(decimal));
            dt.Columns.Add("RecepitComplateRate", typeof(decimal));
            dt.Columns.Add("CanReceiptValue", typeof(decimal));
            dt.Columns.Add("RemaintContractValue", typeof(decimal));
            dt.Columns.Add("ReceiptKPIRemain", typeof(decimal));
            dt.Columns.Add("TimeRate", typeof(decimal));

            var belongYear = String.IsNullOrEmpty(GetQueryString("BelongYear")) ? DateTime.Now.Year : Convert.ToInt32(GetQueryString("BelongYear"));
            var sql = "select * from S_KPI_IndicatorOrg where IndicatorType = 'YearIndicator' and BelongYear= '{0}'";
            var indicatorDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, belongYear));

            sql = @"SELECT  ISNULL(SUM(ReceiptInfo.ReceiptValue), 0) AS Value ,
                            ProductionDept,ProductionDeptName
                    FROM    ( SELECT    S_M_Receipt.* ,
					                    YEAR(ReceiptDate) AS BelongYear,
                                        ProductionDept ,
                                        ProductionDeptName
                              FROM      S_M_Receipt 
                                        LEFT JOIN S_M_ContractInfo ON ContractInfo = S_M_ContractInfo.ID
		                      WHERE YEAR(ReceiptDate)='{0}'
                            ) ReceiptInfo
                    WHERE   1 = 1
                    GROUP BY ProductionDept,ProductionDeptName;";
            var receiptDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, belongYear));

            sql = @" select isnull(Sum(isnull(ReceiptValue,0)-isnull(FactReceiptValue,0)),0)  as DataValue,
ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName 
from dbo.S_M_ContractInfo_ReceiptObj left join dbo.S_M_ContractInfo 
on S_M_ContractInfo_ReceiptObj.S_M_ContractInfoID=S_M_ContractInfo.ID
  group by ProductionDept,ProductionDeptName";
            var canReceiptDt = this.SqlHelper.ExecuteDataTable(sql);

            sql = @"select Sum(isnull(ContractRMBValue,0)-isnull(SumReceiptValue,0)) as DataValue,ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName 
 from S_M_ContractInfo where ContractState='Sign' group by ProductionDept,ProductionDeptName";
            var remainContractDt = this.SqlHelper.ExecuteDataTable(sql);
            var sumKpi = 0m; var sumReceiptValue = 0m;
            foreach (DataRow row in dt.Rows)
            {
                var obj = indicatorDt.Compute("Sum(ReceiptValue)", "OrgID='" + row["value"] + "'");
                var kpi = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ReceiptKPI"] = kpi;
                sumKpi += kpi;
                obj = receiptDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                var recepitValue = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ReceiptValue"] = recepitValue;
                sumReceiptValue += recepitValue;
                row["RecepitComplateRate"] = kpi == 0 ? 100 : Math.Round(recepitValue / kpi * 100, 2);

                obj = canReceiptDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                row["CanReceiptValue"] = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["RemaintContractValue"] = remainContractDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                row["ReceiptKPIRemain"] = kpi - recepitValue;
                row["TimeRate"] = Math.Round(Convert.ToDecimal(DateTime.Now.DayOfYear) / 365 * 100);
            }
            var result = new Dictionary<string, object>();
            result.SetValue("data", dt);
            var sumData = new Dictionary<string, object>();
            sumData.SetValue("RecepitComplateRate", sumKpi == 0 ? 100 : Math.Round(sumReceiptValue / sumKpi * 100, 2));
            result.SetValue("sumData", sumData);

            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "收款金额");
            y1.Lable.SetValue("format", "{value}元");
            var y2 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y2.TitleInfo.SetValue("text", "完成率");
            y2.Lable.SetValue("format", "{value}%"); y2.opposite = true;
            yAxies.Add(y1);
            yAxies.Add(y2);

            var serDefines = new List<Series>();
            var ReceiptKPISer = new Series { Name = "收款目标", Field = "ReceiptKPI", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            ReceiptKPISer.Tooltip.SetValue("valueSuffix", "元");
            serDefines.Add(ReceiptKPISer);

            var ReceiptValueSer = new Series { Name = "已收款", Field = "ReceiptValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            ReceiptValueSer.Tooltip.SetValue("valueSuffix", "元");
            serDefines.Add(ReceiptValueSer);

            var CanReceiptValueSer = new Series { Name = "经营应收款", Field = "CanReceiptValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            CanReceiptValueSer.Tooltip.SetValue("valueSuffix", "元");
            serDefines.Add(CanReceiptValueSer);

            var RecepitComplateRateSer = new Series { Name = "完成率", Field = "RecepitComplateRate", Type = "spline", yAxis = 1, Tooltip = new Dictionary<string, object>() };
            RecepitComplateRateSer.Tooltip.SetValue("valueSuffix", "%");
            serDefines.Add(RecepitComplateRateSer);

            var chart = HighChartHelper.CreateColumnXYChart(belongYear + "年各部门收款情况", "", dt, "text", yAxies, serDefines, null);
            result.SetValue("chart", chart);
            return Json(result);
        }

        #endregion

        #region 部门合同情况统计
        public JsonResult GetDeptContractList()
        {
            var dt = EnumBaseHelper.GetEnumTable("System.ManDept");
            dt.Columns.Add("ContractKPI", typeof(decimal));
            dt.Columns.Add("ContractValue", typeof(decimal));
            dt.Columns.Add("ContractComplateRate", typeof(decimal));
            dt.Columns.Add("UnContractValue", typeof(decimal));
            dt.Columns.Add("ContractKPIRemain", typeof(decimal));
            dt.Columns.Add("TimeRate", typeof(decimal));

            var belongYear = String.IsNullOrEmpty(GetQueryString("BelongYear")) ? DateTime.Now.Year : Convert.ToInt32(GetQueryString("BelongYear"));
            var sql = "select * from S_KPI_IndicatorOrg where IndicatorType = 'YearIndicator' and BelongYear= '{0}'";
            var indicatorDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, belongYear));

            sql = @"select isnull(Sum(ContractRMBValue),0) as DataValue,ChargeDeptID,ChargeDeptName,BelongYear,BelongMonth,BelongQuarter
                    FROM(
                    SELECT ContractRMBValue,ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName,
                    YEAR(SignDate) AS BelongYear,MONTH(SignDate) AS BelongMonth,DATEPART(QUARTER,SignDate) AS BelongQuarter
                    FROM dbo.S_M_ContractInfo
                    WHERE ContractState='Sign'  AND YEAR(SignDate)='{0}'
                    )resulttable
                    GROUP BY ChargeDeptID,ChargeDeptName,BelongYear,BelongMonth,BelongQuarter ";
            var contractDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, belongYear));

            sql = @"select isnull(Sum(ContractRMBValue),0) as DataValue,ChargeDeptID,ChargeDeptName,BelongYear,BelongMonth,BelongQuarter
                    FROM(
                    SELECT ContractRMBValue,ProductionDept as ChargeDeptID,ProductionDeptName as ChargeDeptName,
                    YEAR(SignDate) AS BelongYear,MONTH(SignDate) AS BelongMonth,DATEPART(QUARTER,SignDate) AS BelongQuarter
                    FROM dbo.S_M_ContractInfo
                    WHERE ContractState!='Sign' OR ContractState IS NULL
                    )resulttable
                    GROUP BY ChargeDeptID,ChargeDeptName,BelongYear,BelongMonth,BelongQuarter";
            var UncCntractDt = this.SqlHelper.ExecuteDataTable(sql);

            var sumKpi = 0m; var sumValue = 0m;
            foreach (DataRow row in dt.Rows)
            {
                var obj = indicatorDt.Compute("Sum(ContractValue)", "OrgID='" + row["value"] + "'");
                var kpi = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ContractKPI"] = kpi;
                sumKpi += kpi;
                obj = contractDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                var value = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ContractValue"] = value;
                sumValue += value;
                row["ContractComplateRate"] = kpi == 0 ? 100 : Math.Round(value / kpi * 100, 2);

                obj = UncCntractDt.Compute("Sum(DataValue)", "ChargeDeptID='" + row["value"] + "'");
                row["UnContractValue"] = obj == null || obj == DBNull.Value ? 0 : Convert.ToDecimal(obj);
                row["ContractKPIRemain"] = kpi - value;
                row["TimeRate"] = Math.Round(Convert.ToDecimal(DateTime.Now.DayOfYear) / 365 * 100);
            }
            var result = new Dictionary<string, object>();
            result.SetValue("data", dt);
            var sumData = new Dictionary<string, object>();
            sumData.SetValue("ContractComplateRate", sumKpi == 0 ? 100 : Math.Round(sumValue / sumKpi * 100, 2));
            result.SetValue("sumData", sumData);

            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "合同金额");
            y1.Lable.SetValue("format", "{value}元");
            var y2 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y2.TitleInfo.SetValue("text", "完成率");
            y2.Lable.SetValue("format", "{value}%"); y2.opposite = true;
            yAxies.Add(y1);
            yAxies.Add(y2);

            var serDefines = new List<Series>();
            var ReceiptKPISer = new Series { Name = "合同目标", Field = "ContractKPI", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            ReceiptKPISer.Tooltip.SetValue("valueSuffix", "元");
            serDefines.Add(ReceiptKPISer);

            var ReceiptValueSer = new Series { Name = "已签订金额", Field = "ContractValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            ReceiptValueSer.Tooltip.SetValue("valueSuffix", "元");
            serDefines.Add(ReceiptValueSer);

            var CanReceiptValueSer = new Series { Name = "待签约金额", Field = "UnContractValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            CanReceiptValueSer.Tooltip.SetValue("valueSuffix", "元");
            serDefines.Add(CanReceiptValueSer);

            var RecepitComplateRateSer = new Series { Name = "完成率", Field = "ContractComplateRate", Type = "spline", yAxis = 1, Tooltip = new Dictionary<string, object>() };
            RecepitComplateRateSer.Tooltip.SetValue("valueSuffix", "%");
            serDefines.Add(RecepitComplateRateSer);

            //var TimeRateSer = new Series { Name = "时间", Field = "TimeRate", Type = "spline", yAxis = 1, Tooltip = new Dictionary<string, object>() };
            //TimeRateSer.Tooltip.SetValue("valueSuffix", "%");
            //serDefines.Add(TimeRateSer);

            var chart = HighChartHelper.CreateColumnXYChart(belongYear + "年各部门合同情况", "", dt, "text", yAxies, serDefines, null);
            result.SetValue("chart", chart);
            return Json(result);
        }
        #endregion

        public JsonResult GetDeptMonthReceiptList()
        {
            var dt = EnumBaseHelper.GetEnumTable("System.ManDept");
            for (int i = 1; i <= 12; i++)
            {
                var field = i + "_Month";
                dt.Columns.Add(field, typeof(decimal));
            }
            dt.Columns.Add("TotalValue", typeof(decimal));
            var belongYear = String.IsNullOrEmpty(this.GetQueryString("BelongYear")) ? DateTime.Now.Year : Convert.ToInt32(this.GetQueryString("BelongYear"));
            var sql = @"SELECT  ISNULL(SUM(ReceiptInfo.ReceiptValue), 0) AS Value ,
                                BelongMonth,BelongYear,ProductionDept
                        FROM    ( SELECT    ReceiptValue,
					                        YEAR(ReceiptDate) AS BelongYear,
					                        MONTH(ReceiptDate) AS BelongMonth,
                                            ProductionDept
                                  FROM      S_M_Receipt 
                                            LEFT JOIN S_M_ContractInfo ON ContractInfo = S_M_ContractInfo.ID
		                          WHERE YEAR(ReceiptDate)='{0}'
                                ) ReceiptInfo
                        WHERE   1 = 1
                        GROUP BY ProductionDept,BelongMonth,BelongYear; ";
            var receiptDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, belongYear));
            foreach (DataRow row in dt.Rows)
            {
                var sumValue = 0m;
                for (int i = 1; i <= 12; i++)
                {
                    if (belongYear == DateTime.Now.Year && i > DateTime.Now.Month)
                    { row[i + "_Month"] = DBNull.Value; continue; }
                    var monthData = receiptDt.Select(" ProductionDept='" + row["value"] + "' and BelongMonth = '" + i + "'").FirstOrDefault();
                    if (monthData == null)
                    {
                        row[i + "_Month"] = 0;
                        continue;
                    }
                    row[i + "_Month"] = monthData["ReceiptValue"];
                    sumValue += Convert.ToDecimal(monthData["ReceiptValue"]);
                }
                row["TotalValue"] = sumValue;
            }
            return Json(dt);
        }

        public JsonResult GetDeptMonthContractList()
        {
            var dt = EnumBaseHelper.GetEnumTable("System.ManDept");
            for (int i = 1; i <= 12; i++)
            {
                var field = i + "_Month";
                dt.Columns.Add(field, typeof(decimal));
            }
            dt.Columns.Add("TotalValue", typeof(decimal));
            var belongYear = String.IsNullOrEmpty(this.GetQueryString("BelongYear")) ? DateTime.Now.Year : Convert.ToInt32(this.GetQueryString("BelongYear"));
            var sql = @"select isnull(Sum(ContractRMBValue),0) as ContractValue,BelongMonth,BelongYear
                         FROM (
                        SELECT ContractRMBValue,MONTH(SignDate) AS BelongMonth,YEAR(SignDate) AS BelongYear,ProductionDept 
                        FROM dbo.S_M_ContractInfo
                        where YEAR(SignDate)='{0}' and ContractState='Sign' )resulttable
                        group by ProductionDept,BelongMonth,BelongYear ";
            var receiptDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, belongYear));
            foreach (DataRow row in dt.Rows)
            {
                var sumValue = 0m;
                for (int i = 1; i <= 12; i++)
                {
                    if (belongYear == DateTime.Now.Year && i > DateTime.Now.Month)
                    { row[i + "_Month"] = DBNull.Value; continue; }
                    var monthData = receiptDt.Select(" ProductionDept='" + row["value"] + "' and BelongMonth = '" + i + "'").FirstOrDefault();
                    if (monthData == null)
                    {
                        row[i + "_Month"] = 0;
                        continue;
                    }
                    row[i + "_Month"] = monthData["ContractValue"];
                    sumValue += Convert.ToDecimal(monthData["ContractValue"]);
                }
                row["TotalValue"] = sumValue;
            }
            return Json(dt);
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            this.FillQueryBuilderFilter<S_I_Engineering>(qb);
            var qbItem_date = qb.Items.FirstOrDefault(a => a.Field == "Date");
            if (qbItem_date != null)
            {
                qb.Add("CreateDate", QueryMethod.GreaterThanOrEqual, GetQueryString("Date"));
                qb.Items.Remove(qbItem_date);
            }

            string sql = @"select * from (SELECT pinfo.ID,pinfo.Name,pinfo.SerialNumber,pinfo.ProjectScale,pinfo.ProjectClass,
c.Industry,pinfo.CustomerInfo,pinfo.CustomerInfoName,
pinfo.FlowPhase,pinfo.ChargerDeptName,pinfo.ChargerDept,pinfo.ChargerUserName
,pinfo.ChargerUser,pinfo.BusinessChargerName
,pinfo.BusinessCharger,pinfo.OtherDeptName,pinfo.OtherDept,c.Country,isnull(c.City,'') City
,pinfo.State,pinfo.Remark,pinfo.Attachment,pinfo.CreateDate,isnull(c.Province,'') Province FROM dbo.S_I_Engineering pinfo
left join dbo.S_M_CustomerInfo c on pinfo.CustomerInfo = c.ID) aa ";
            GridData grid = this.SqlHelper.ExecuteGridData(sql, qb);

            return Json(grid);

        }

    }
}
