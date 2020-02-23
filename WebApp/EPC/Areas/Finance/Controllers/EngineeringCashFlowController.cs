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

namespace EPC.Areas.Finance.Controllers
{
    public class EngineeringCashFlowController : EPCController
    {
        public JsonResult GetList(string EngineeringInfoID, string Year, string analysisType)
        {
            string sql = @"select Sum(PlanValue) as Value,S_M_ContractInfo.ProjectInfo,BelongYear,
BelongMonth from dbo.S_M_ContractInfo_ReceiptObj_PlanReceipt left join S_M_ContractInfo 
on S_M_ContractInfo_ReceiptObj_PlanReceipt.ContractInfoID=S_M_ContractInfo.ID where ProjectInfo='{0}' and BelongYear={1}
group by S_M_ContractInfo.ProjectInfo,BelongYear,BelongMonth";

            var planReceiptDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID, Year));
            sql = @"select Sum(PlanValue) as Value,BelongYear,BelongMonth from S_P_ContractInfo_PaymentObj_PaymentPlan
where EngineeringInfo='{0}' and BelongYear={1} group by EngineeringInfo,BelongYear,BelongMonth";
            var planPaymentDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID, Year));

            sql = @"select Sum(PaymentValue) as Value,Year(PaymentDate) as BelongYear,Month(PaymentDate) as BelongMonth
 from S_P_Payment where EngineeringInfoID='{0}' and  Year(PaymentDate)={1}
 group by EngineeringInfoID,Year(PaymentDate),Month(PaymentDate)";
            var paymentDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID, Year));

            sql = @"select Sum(ReceiptValue) as Value,Year(ReceiptDate) as BelongYear,
Month(ReceiptDate) as BelongMonth from S_M_Receipt  where ProjectInfo='{0}' and Year(ReceiptDate)={1}
 group by Year(ReceiptDate),Month(ReceiptDate)";
            var receiptDt = this.SqlHelper.ExecuteDataTable(String.Format(sql, EngineeringInfoID, Year));

            var resultDt = this.createDt();
            var planReceptRow = FillDataRow("计划收款", resultDt, planReceiptDt, AnalysisType.Month.ToString());
            var receptRow = FillDataRow("实际收款", resultDt, receiptDt, analysisType,true);

            var receptDeviationRow = resultDt.NewRow();
            receptDeviationRow["Name"] = "收款偏差";
            var totalValue = 0m;
            for (int i = 1; i <= 12; i++)
            {
                var planValue = Convert.ToDecimal(planReceptRow[i + "_Month"]);
                var receptValue = Convert.ToDecimal(receptRow[i + "_Month"]);
                var deviationValue = planValue - receptValue;
                receptDeviationRow[i + "_Month"] = deviationValue;
                totalValue += deviationValue;
            }
            receptDeviationRow["Total"] = totalValue;
            resultDt.Rows.Add(receptDeviationRow);

            var planPaymentRow = FillDataRow("计划付款", resultDt, planPaymentDt, AnalysisType.Month.ToString());
            var paymentRow = FillDataRow("实际付款", resultDt, paymentDt, analysisType,true);
            var payDeviationRow = resultDt.NewRow();
            payDeviationRow["Name"] = "付款偏差";
            totalValue = 0m;
            for (int i = 1; i <= 12; i++)
            {
                var planValue = Convert.ToDecimal(planPaymentRow[i + "_Month"]);
                var paymentValue = Convert.ToDecimal(paymentRow[i + "_Month"]);
                var deviationValue = planValue - paymentValue;
                payDeviationRow[i + "_Month"] = deviationValue;
                totalValue += deviationValue;
            }
            payDeviationRow["Total"] = totalValue;
            resultDt.Rows.Add(payDeviationRow);

            var cashFlowRow = resultDt.NewRow();
            cashFlowRow["Name"] = "现金流";
            totalValue = 0m;
            for (int i = 1; i <= 12; i++)
            {
                var paymentValue = Convert.ToDecimal(paymentRow[i + "_Month"]);
                var receptValue = Convert.ToDecimal(receptRow[i + "_Month"]);
                var cashFlow = receptValue - paymentValue;
                cashFlowRow[i + "_Month"] = cashFlow;
                totalValue += cashFlow;
            }
            cashFlowRow["Total"] = totalValue;
            resultDt.Rows.Add(cashFlowRow);

            var result = new Dictionary<string, object>();

            var chartData = GetChartData(Year, planReceiptDt, receiptDt, planPaymentDt, paymentDt);
            var credits = new Dictionary<string, object>();
            credits.SetValue("enabled", false);
            chartData.SetValue("credits", credits);

            var lineData = GetLineChartData(Year, planReceiptDt, receiptDt, planPaymentDt, paymentDt);
            var lineCredits = new Dictionary<string, object>();
            lineCredits.SetValue("enabled", false);
            lineData.SetValue("credits", lineCredits);

            result.SetValue("lineChartData", lineData);
            result.SetValue("chartData", chartData);
            result.SetValue("data", resultDt);
            return Json(result);
        }

        DataTable createDt()
        {
            var dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Year", typeof(int));
            for (int i = 1; i <= 12; i++)
            {
                var field = i + "_Month";
                dt.Columns.Add(field, typeof(decimal));
            }
            dt.Columns.Add("Total", typeof(decimal));
            return dt;
        }

        DataRow FillDataRow(string Name, DataTable resultDt, DataTable sourceDt, string analysisType, bool stopOnCurrent = false)
        {
            var dataRow = resultDt.NewRow();
            dataRow["Name"] = Name;
            var totalValue = 0m;
            if (analysisType == AnalysisType.Month.ToString())
            {
                for (int i = 1; i <= 12; i++)
                {
                    var field = i.ToString() + "_Month";
                    if (stopOnCurrent && i > DateTime.Now.Month)
                    {
                        dataRow[field] = 0; continue;
                    }

                    var rows = sourceDt.Select("BelongMonth = '" + i + "'");
                    if (rows.Length > 0)
                    {
                        dataRow[field] = rows[0]["Value"];
                        totalValue += Convert.ToDecimal(rows[0]["Value"]);
                    }
                    else
                    {
                        dataRow[field] = 0;
                    }
                }
                dataRow["Total"] = totalValue;
            }
            else
            {
                for (int i = 1; i <= 12; i++)
                {
                    var field = i.ToString() + "_Month";
                    if (stopOnCurrent && i > DateTime.Now.Month)
                    {
                        dataRow[field] = 0; continue;
                    }
                    var rows = sourceDt.Select("BelongMonth = '" + i + "'");
                    if (rows.Length > 0)
                    {
                        totalValue += Convert.ToDecimal(rows[0]["Value"]);
                        dataRow[field] = totalValue;
                    }
                    else
                    {
                        dataRow[field] = 0;
                    }
                }
            }


            resultDt.Rows.Add(dataRow);
            return dataRow;
        }

        public Dictionary<string, object> GetChartData(string Year, DataTable planReceiptDt, DataTable receiptDt, DataTable planPaymentDt, DataTable paymentDt)
        {
            var dt = new DataTable();
            var series = "PlanReceipt,Receipt,ReceiptDeviation,PlanPayment,Payment,PayDeviation";
            var serieNames = "计划收款,实际收款,收款偏差,计划付款,实际付款,付款偏差";
            dt.Columns.Add("Month");
            dt.Columns.Add("PlanReceipt", typeof(decimal));
            dt.Columns.Add("Receipt", typeof(decimal));
            dt.Columns.Add("PlanPayment", typeof(decimal));
            dt.Columns.Add("Payment", typeof(decimal));
            dt.Columns.Add("PayDeviation", typeof(decimal));
            dt.Columns.Add("ReceiptDeviation", typeof(decimal));

            var miniYValue = 0;
            for (int i = 1; i <= 12; i++)
            {
                var dataRow = dt.NewRow();
                dataRow["Month"] = i + "月";
                var planReceiptRows = planReceiptDt.Select("BelongMonth=" + i.ToString());
                dataRow["PlanReceipt"] = planReceiptRows.Length == 0 ? 0m : Convert.ToDecimal(planReceiptRows[0]["Value"]);

                var receiptRows = receiptDt.Select("BelongMonth=" + i.ToString());
                dataRow["Receipt"] = receiptRows.Length == 0 ? 0m : Convert.ToDecimal(receiptRows[0]["Value"]);

                var planPaymentRows = planPaymentDt.Select("BelongMonth=" + i.ToString());
                dataRow["PlanPayment"] = planPaymentRows.Length == 0 ? 0m : Convert.ToDecimal(planPaymentRows[0]["Value"]);

                var paymentRows = paymentDt.Select("BelongMonth=" + i.ToString());
                dataRow["Payment"] = paymentRows.Length == 0 ? 0m : Convert.ToDecimal(paymentRows[0]["Value"]);

                dataRow["PayDeviation"] = Convert.ToDecimal(dataRow["PlanPayment"]) - Convert.ToDecimal(dataRow["Payment"]);
                dataRow["ReceiptDeviation"] = Convert.ToDecimal(dataRow["PlanReceipt"]) - Convert.ToDecimal(dataRow["Receipt"]);

                dt.Rows.Add(dataRow);

                if (miniYValue > Convert.ToInt32(dataRow["PayDeviation"]))
                    miniYValue = Convert.ToInt32(dataRow["PayDeviation"]);
                if (miniYValue > Convert.ToInt32(dataRow["ReceiptDeviation"]))
                    miniYValue = Convert.ToInt32(dataRow["ReceiptDeviation"]);
            }
            var columChart = HighChartHelper.CreateColumnChart(Year + "年计划收支分析", dt, "Month", serieNames.Split(','), series.Split(','));
            columChart.yAxisInfo.MiniValue = miniYValue;
            return columChart.Render();
        }

        public Dictionary<string, object> GetLineChartData(string Year, DataTable planReceiptDt, DataTable receiptDt, DataTable planPaymentDt, DataTable paymentDt)
        {
            var series = "PlanCashFlow,CashFlow";
            var serieNames = "计划现金流,实际现金流";
            var dt = new DataTable();
            dt.Columns.Add("Month");
            dt.Columns.Add("PlanCashFlow", typeof(decimal));
            dt.Columns.Add("CashFlow", typeof(decimal));
            var miniYValue = 0;
            for (int i = 1; i <= 12; i++)
            {
                var dataRow = dt.NewRow();
                dataRow["Month"] = i + "月";
                var planReceiptRows = planReceiptDt.Select("BelongMonth=" + i.ToString());
                var planReceipt = planReceiptRows.Length == 0 ? 0m : Convert.ToDecimal(planReceiptRows[0]["Value"]);
                var planPaymentRows = planPaymentDt.Select("BelongMonth=" + i.ToString());
                var planPayment = planPaymentRows.Length == 0 ? 0m : Convert.ToDecimal(planPaymentRows[0]["Value"]);
                dataRow["PlanCashFlow"] = planReceipt - planPayment;

                var receiptRows = receiptDt.Select("BelongMonth=" + i.ToString());
                var receiptValue = receiptRows.Length == 0 ? 0m : Convert.ToDecimal(receiptRows[0]["Value"]);
                var paymentRows = paymentDt.Select("BelongMonth=" + i.ToString());
                var paymentvalue = paymentRows.Length == 0 ? 0m : Convert.ToDecimal(paymentRows[0]["Value"]);
                dataRow["CashFlow"] = receiptValue - paymentvalue;
                dt.Rows.Add(dataRow);

                if (miniYValue > Convert.ToInt32(dataRow["CashFlow"]))
                    miniYValue = Convert.ToInt32(dataRow["CashFlow"]);
                if (miniYValue > Convert.ToInt32(dataRow["PlanCashFlow"]))
                    miniYValue = Convert.ToInt32(dataRow["PlanCashFlow"]);
            }

            var columChart = HighChartHelper.CreateColumnChart(Year + "年现金流分析", dt, "Month", serieNames.Split(','), series.Split(','));
            columChart.Chart.Type = "line";
            columChart.yAxisInfo.MiniValue = miniYValue;
            return columChart.Render();
        }
    }

    public enum AnalysisType
    {
        [System.ComponentModel.Description("单月统计")]
        Month,
        [System.ComponentModel.Description("累计统计")]
        Total
    }
}
