using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic.Domain;
using Config;
using Config.Logic;
using System.Data;
using Project.Logic;

namespace Project.Areas.Monitor.Controllers
{
    public class ReceiptAnalyzeController : ProjectController
    {
        public JsonResult GetList()
        {
//            string projectInfoID = this.Request["ProjectInfoID"];
//            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
//            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("没有找到指定的项目对象");
//            var marketDBContext = Formula.FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
//            var project = marketDBContext.S_I_Project.FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
//            if (project == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目对象");
//            string sql = @"select S_C_Receipt.CustomerName,S_C_Receipt.ContractCode,S_C_Receipt.ContractName,
//S_C_ManageContract_ReceiptObj.Name,S_C_Receipt.ContractInfoID,S_C_ReceiptPlanRelation.RelationValue,
//S_C_InvoiceReceiptRelation.RelationValue as InvoiceValue,
//S_C_ManageContract_ReceiptObj.ProjectInfo as ProjectInfoID,S_C_ManageContract_ReceiptObj.ProjectInfoName,
//S_C_Receipt.BelongYear,
//S_C_Receipt.BelongMonth,S_C_Receipt.BelongQuarter ,ArrivedDate, 0 as ReceiptScale
//from S_C_ReceiptPlanRelation
//left join S_C_Receipt on S_C_ReceiptPlanRelation.ReceiptID=S_C_Receipt.ID
//left join S_C_ManageContract_ReceiptObj 
//on S_C_ReceiptPlanRelation.ReceiptObjectID=S_C_ManageContract_ReceiptObj.ID
//left join S_C_InvoiceReceiptRelation on S_C_InvoiceReceiptRelation.ReceiptID=S_C_Receipt.ID
//where ProjectInfo='{0}' order by ArrivedDate";
//            var marketDb = SQLHelper.CreateSqlHelper(ConnEnum.Market);
//            var dt = marketDb.ExecuteDataTable(String.Format(sql, project.ID));
//            var contractValueObj = marketDb.ExecuteScalar(String.Format("select Sum(ProjectValue) as ContractValue from S_C_ManageContract_ProjectRelation where ProjectID='{0}'", project.ID));
//            var sumReceiptValue = 0m;
//            var contractValue = contractValueObj == null || contractValueObj == DBNull.Value ? 0m : Convert.ToDecimal(contractValueObj);
//            foreach (DataRow item in dt.Rows)
//            {
//                sumReceiptValue += item["RelationValue"] == null || item["RelationValue"] == DBNull.Value ? 0 : Convert.ToDecimal(item["RelationValue"]);
//                if (contractValue > 0)
//                {
//                    item["ReceiptScale"] = Math.Round(sumReceiptValue / contractValue * 100, 2);
//                }
//            }
            var result = new Dictionary<string, object>();
            //result.SetValue("data", dt);
            //result.SetValue("pieChartData", this.GetPieChart(project.ID));
            //result.SetValue("chartData", this.GetYearChartData(project.ID));
            return Json(result);
        }

        public Dictionary<string, object> GetPieChart(string ProjectID)
        {
            var sql = @"select Sum(FactReceiptValue) as SumRecepitValue, Sum(FactInvoiceValue) as SumInvoiceValue,ProjectInfo as ProjectInfoID,
ProjectInfoName from S_C_ManageContract_ReceiptObj
where ProjectInfo='{0}'
group by ProjectInfo,ProjectInfoName";

            var dataSource = new DataTable();
            dataSource.Columns.Add("nameField");
            dataSource.Columns.Add("valueField",typeof(decimal));

            var receiptValue=0m;
            var invoiceValue=0m;
            var remainInvoiceValue=0m;
            var remaintContractValue = 0m;
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var dt = db.ExecuteDataTable(String.Format(sql, ProjectID));
            if (dt.Rows.Count > 0)
            {
                receiptValue = dt.Rows[0]["SumRecepitValue"] == null || dt.Rows[0]["SumRecepitValue"] == DBNull.Value ? 0m : Convert.ToDecimal(dt.Rows[0]["SumRecepitValue"]);
                invoiceValue = dt.Rows[0]["SumInvoiceValue"] == null || dt.Rows[0]["SumInvoiceValue"] == DBNull.Value ? 0m : Convert.ToDecimal(dt.Rows[0]["SumInvoiceValue"]);
                remainInvoiceValue = (invoiceValue - receiptValue) < 0 ? 0m : invoiceValue - receiptValue;
            }
            sql = @"select Sum(ProjectValue) as ContractValue,ProjectID  from S_C_ManageContract_ProjectRelation
where ProjectID='{0}' group by ProjectID";
            dt = db.ExecuteDataTable(String.Format(sql, ProjectID));
            if (dt.Rows.Count > 0)
            {
                var contractValue = dt.Rows[0]["ContractValue"] == null || dt.Rows[0]["ContractValue"] == DBNull.Value ? 0m : Convert.ToDecimal(dt.Rows[0]["ContractValue"]);
                remaintContractValue = (contractValue - receiptValue - remainInvoiceValue) < 0 ? receiptValue : contractValue - receiptValue - remainInvoiceValue;
            }

            var contractRemainRow =  dataSource.NewRow();
            contractRemainRow["nameField"] = "剩余合同额";
            contractRemainRow["valueField"] = remaintContractValue;
            dataSource.Rows.Add(contractRemainRow);

            var invoiceRemainRow = dataSource.NewRow();
            invoiceRemainRow["nameField"] = "应收款";
            invoiceRemainRow["valueField"] = remainInvoiceValue;
            dataSource.Rows.Add(invoiceRemainRow);

            var receiptRow = dataSource.NewRow();
            receiptRow["nameField"] = "已收款";
            receiptRow["valueField"] = receiptValue;
            dataSource.Rows.Add(receiptRow);

            var chart = HighChartHelper.CreatePieChart("项目总体情况分析", "金额", dataSource);
            var result = chart.Render();
            var credits = new Dictionary<string, object>();
            credits.SetValue("enabled", false);
            result.SetValue("credits", credits);
            return result;
        }

        public Dictionary<string, object> GetYearChartData(string projectID)
        {
            string sql = @"select 
Sum(S_C_ReceiptPlanRelation.RelationValue) as ReceiptValue,
S_C_ManageContract_ReceiptObj.ProjectInfo as ProjectInfoID,
S_C_Receipt.BelongMonth,S_C_Receipt.BelongQuarter ,S_C_Receipt.BelongYear
from S_C_ReceiptPlanRelation
left join S_C_Receipt on S_C_ReceiptPlanRelation.ReceiptID=S_C_Receipt.ID
left join S_C_ManageContract_ReceiptObj on S_C_ReceiptPlanRelation.ReceiptObjectID=S_C_ManageContract_ReceiptObj.ID
where ProjectInfo='{0}' group by S_C_ManageContract_ReceiptObj.ProjectInfo,
S_C_Receipt.BelongMonth,S_C_Receipt.BelongQuarter ,S_C_Receipt.BelongYear
";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var dt = db.ExecuteDataTable(String.Format(sql, projectID));
            var dataSource = new DataTable();
            dataSource.Columns.Add("YearMonth", typeof(string));
            dataSource.Columns.Add("ReceiptValue", typeof(decimal));
            dataSource.Columns.Add("SumReceiptValue", typeof(decimal));
            var sumValue = 0m;
            foreach (DataRow item in dt.Rows)
            {
                var row = dataSource.NewRow();
                var belongYear = Convert.ToInt32(item["BelongYear"]);
                var belongMonth = Convert.ToInt32(item["BelongMonth"]);
                if (belongMonth < 10)
                    row["YearMonth"] = belongYear + "-0" + belongMonth + "月";
                else
                    row["YearMonth"] = belongYear + "-" + belongMonth + "月";
                var value = item["ReceiptValue"] == null || item["ReceiptValue"] == DBNull.Value ? 0m : Convert.ToDecimal(item["ReceiptValue"]);
                sumValue += value;
                row["ReceiptValue"] = value;
                row["SumReceiptValue"] = sumValue;
                dataSource.Rows.Add(row);
            }
            #region 生成图表
            var yAxies = new List<yAxis>();
            var y1 = new yAxis { MiniValue = 0, TitleInfo = new Dictionary<string, object>(), Lable = new Dictionary<string, object>() };
            y1.TitleInfo.SetValue("text", "到款金额");
            y1.Lable.SetValue("format", "{value}元");
            yAxies.Add(y1);

            var serDefines = new List<Series>();


            var costSer = new Series { Name = "收款", Field = "ReceiptValue", Type = "column", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            var sumCostSer = new Series { Name = "累计收款", Field = "SumReceiptValue", Type = "spline", yAxis = 0, Tooltip = new Dictionary<string, object>() };
            serDefines.Add(costSer);
            serDefines.Add(sumCostSer);

            var chart = HighChartHelper.CreateColumnXYChart("收款分析", "", dataSource, "YearMonth", yAxies, serDefines, null);
            return chart;
            #endregion
        }
    }
}
