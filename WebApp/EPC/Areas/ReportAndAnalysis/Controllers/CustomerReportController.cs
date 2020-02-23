using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Collections;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using MvcAdapter;
using EPC.Logic;
using EPC.Logic.Domain;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class CustomerReportController : EPCController
    {
        public ActionResult CustomerInfoReport()
        {
            var tab = new Tab();
            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 7);
            yearCategory.SetDefaultItem();
            tab.Categories.Add(yearCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public ActionResult CustomerNewContract()
        {
            var tab = new Tab();
            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 7, 1, false);
            yearCategory.Multi = false;
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            tab.Categories.Add(yearCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetNewContractInfoList(QueryBuilder qb, string Year)
        {
            #region
            string sql = @"select * from (
 select	S_M_CustomerInfo.*,
		isnull(ContractAmount,0) as ContractValue,
		isnull(InvoiceValue,0) as InvoiceValue,
		isnull(PlanReceiptValue,0) as PlanReceiptValue,
		isnull(ReceiptValue,0) as ReceiptValue
from dbo.S_M_CustomerInfo
left join 
	(select Sum(ContractRMBValue) as ContractAmount,PartyA
	 from dbo.S_M_ContractInfo where SignDate is not null and Year(SignDate)='{0}'
	 group by PartyA) ContractTable 
on S_M_CustomerInfo.ID = ContractTable.PartyA
left join 
	(select Sum(InvoiceValue) as InvoiceValue,CustomerID 
	 from (
			select InvoiceInfo.* from (
										select Sum(InvoiceValue) as InvoiceValue,ContractInfo,CustomerInfo as CustomerID 
										from S_M_Invoice where State='Normal'
										group by ContractInfo,CustomerInfo ) InvoiceInfo
			left join S_M_ContractInfo on InvoiceInfo.ContractInfo=S_M_ContractInfo.ID
			where Year(S_M_ContractInfo.SignDate)='{0}') InvoiceTable
	group by InvoiceTable.CustomerID) InvoiceInfoTable
on  S_M_CustomerInfo.ID = InvoiceInfoTable.CustomerID 
left join 
	(select Sum(PlanReceiptValue) as PlanReceiptValue,CustomerID 
	 from (
			select ReceiptObject.* from (
										 select isnull(Sum(ReceiptValue),0) as PlanReceiptValue,
												isnull(Sum(FactBadValue),0) as BadDebtValue,PartyA as CustomerID,
												S_M_ContractInfoID as ContractInfoID
										 from S_M_ContractInfo_ReceiptObj
										 left join S_M_ContractInfo 
										 on S_M_ContractInfo_ReceiptObj.S_M_ContractInfoID=S_M_ContractInfo.ID
										 group by PartyA,S_M_ContractInfoID) ReceiptObject
			left join S_M_ContractInfo on ReceiptObject.ContractInfoID=S_M_ContractInfo.ID
			where Year(S_M_ContractInfo.SignDate)='{0}' ) PlanTable 
	group by PlanTable.CustomerID ) PlanInfoTable
on  S_M_CustomerInfo.ID = PlanInfoTable.CustomerID 
left join 
	(select Sum(ReceiptValue) as ReceiptValue,CustomerInfo 
	 from (
			select ReceiptInfo.* from (
										select Sum(ReceiptValue) as ReceiptValue,ContractInfo,CustomerInfo 
										from S_M_Receipt
										group by ContractInfo,CustomerInfo) ReceiptInfo
			left join S_M_ContractInfo on ReceiptInfo.ContractInfo=S_M_ContractInfo.ID
			where Year(S_M_ContractInfo.SignDate)='{0}') ReceiptTable
	group by ReceiptTable.CustomerInfo )ReceiptInfoTable
on  S_M_CustomerInfo.ID = ReceiptInfoTable.CustomerInfo)ResultTable ";
            #endregion
            sql = String.Format(sql, Year);
            this.FillQueryBuilderFilter<S_M_CustomerInfo>(qb);
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            var sumSql = @"select isnull(Sum(ContractValue),0) as SumContractValue, isnull(Sum(InvoiceValue),0) as SumInvoiceValue,
 isnull(Sum(ReceiptValue),0) as SumReceiptValue, isnull(Sum(PlanReceiptValue),0) as SumPlanReceiptValue
            from (" + sql + "" + qb.GetWhereString() + ") SummaryData";
            var sumDt = this.SqlHelper.ExecuteDataTable(sumSql);
            if (sumDt.Rows.Count > 0)
            {
                data.sumData.SetValue("ContractValue", Convert.ToDecimal(sumDt.Rows[0]["SumContractValue"]).ToString("c"));
                data.sumData.SetValue("InvoiceValue", Convert.ToDecimal(sumDt.Rows[0]["SumInvoiceValue"]).ToString("c"));
                data.sumData.SetValue("ReceiptValue", Convert.ToDecimal(sumDt.Rows[0]["SumReceiptValue"]).ToString("c"));
                data.sumData.SetValue("PlanReceiptValue", Convert.ToDecimal(sumDt.Rows[0]["SumPlanReceiptValue"]).ToString("c"));
            }
            return Json(data);
        }

        public JsonResult GetList(QueryBuilder qb, string Year)
        {
            string sql = this.CreateSql(Year);
            this.FillQueryBuilderFilter<S_M_CustomerInfo>(qb);

            var data = this.SqlHelper.ExecuteGridData(sql, qb);

            var sumSql = @"select isnull(Sum(ContractValue),0) as SumContractValue, isnull(Sum(InvoiceValue),0) as SumInvoiceValue,
 isnull(Sum(ReceiptValue),0) as SumReceiptValue, isnull(Sum(PlanReceiptValue),0) as SumPlanReceiptValue,
 isnull(Sum(BadDebtValue),0) as SumBadDebtValue, isnull(Sum(PlanReceivableValue),0) as SumPlanReceivableValue,
 isnull(Sum(RemainContractValue),0) as SumRemainContractValue, isnull(Sum(ReceivableValue),0) as SumReceivableValue
            from (" + sql + "" + qb.GetWhereString() + ") SummaryData";
            var sumDt = this.SqlHelper.ExecuteDataTable(sumSql);
            if (sumDt.Rows.Count > 0)
            {
                data.sumData.SetValue("ContractValue", Convert.ToDecimal(sumDt.Rows[0]["SumContractValue"]).ToString("c"));
                data.sumData.SetValue("InvoiceValue", Convert.ToDecimal(sumDt.Rows[0]["SumInvoiceValue"]).ToString("c"));
                data.sumData.SetValue("ReceiptValue", Convert.ToDecimal(sumDt.Rows[0]["SumReceiptValue"]).ToString("c"));
                data.sumData.SetValue("PlanReceiptValue", Convert.ToDecimal(sumDt.Rows[0]["SumPlanReceiptValue"]).ToString("c"));
                data.sumData.SetValue("BadDebtValue", Convert.ToDecimal(sumDt.Rows[0]["SumBadDebtValue"]).ToString("c"));
                data.sumData.SetValue("PlanReceivableValue", Convert.ToDecimal(sumDt.Rows[0]["SumPlanReceivableValue"]).ToString("c"));
                data.sumData.SetValue("RemainContractValue", Convert.ToDecimal(sumDt.Rows[0]["SumRemainContractValue"]).ToString("c"));
                data.sumData.SetValue("ReceivableValue", Convert.ToDecimal(sumDt.Rows[0]["SumReceivableValue"]).ToString("c"));
            }
            return Json(data);
        }

        private string CreateSql(string BelongYear)
        {
            string sql = @"select * from 
(
	select S_M_CustomerInfo.*,isnull(ContractValue,0) as ContractValue,
	isnull(InvoiceValue,0) as InvoiceValue,
	isnull(ReceiptValue,0) as ReceiptValue,
	case when isnull(ContractValue,0)=0 then 0 else Round(isnull(ReceiptValue,0)/isnull(ContractValue,0)*100,2)
	end as ReceiptRate,
	isnull(PlanReceiptValue,0) as PlanReceiptValue,
	isnull(BadDebtValue,0) as BadDebtValue,
	isnull(PlanReceiptValue,0) - isnull(ReceiptValue,0) - isnull(BadDebtValue,0) as PlanReceivableValue,
	isnull(ContractValue,0) - isnull(ReceiptValue,0) - isnull(BadDebtValue,0) as RemainContractValue,
	isnull(InvoiceValue,0) - isnull(ReceiptValue,0) - isnull(BadDebtValue,0) as ReceivableValue
	from S_M_CustomerInfo
	left join (select Sum(ContractRMBValue) as ContractValue,PartyA from S_M_ContractInfo
			   where SignDate is not null {0}
			   group by PartyA) ContractInfo 
    on S_M_CustomerInfo.ID = PartyA
	left join (select Sum(InvoiceValue) as InvoiceValue,CustomerInfo from S_M_Invoice 
			   where 1=1  and State='Normal' {1}
			   group by CustomerInfo) InvoiceInfo 
	on S_M_CustomerInfo.ID = CustomerInfo
	left join (select Sum(ReceiptValue) as ReceiptValue,CustomerInfo from S_M_Receipt
			   where 1=1  {2}
			   group by CustomerInfo) ReceiptInfo 
	on S_M_CustomerInfo.ID = ReceiptInfo.CustomerInfo
	left join (select isnull(Sum(ReceiptValue),0) as PlanReceiptValue,
					  isnull(Sum(FactBadValue),0) as BadDebtValue,PartyA 
			   from S_M_ContractInfo_ReceiptObj
			   left join S_M_ContractInfo 
			   on S_M_ContractInfo_ReceiptObj.S_M_ContractInfoID=S_M_ContractInfo.ID
			   where 1=1 {3}
			   group by PartyA) ReceiptObjectInfo 
	on S_M_CustomerInfo.ID = ReceiptObjectInfo.PartyA

) ResultTable";
            string whereStr0 = String.Empty;
            string whereStr1 = String.Empty;
            string whereStr2 = String.Empty;
            string PlanWhereStr = String.Empty;
            if (!String.IsNullOrEmpty(BelongYear) && BelongYear != Formula.Constant.CategoryAllKey)
            {
                whereStr0 = " and Year(SignDate) in ('" + BelongYear.Replace(",", "','") + "') ";
                whereStr1 = " and Year(InvoiceDate) in ('" + BelongYear.Replace(",", "','") + "') ";
                whereStr2 = " and Year(ReceiptDate) in ('" + BelongYear.Replace(",", "','") + "') ";
                PlanWhereStr = " and Year(PlanFinishDate) in ('" + BelongYear.Replace(",", "','") + "') ";
            }
            sql = String.Format(sql, whereStr0, whereStr1, whereStr2, PlanWhereStr);
            return sql;
        }

        public JsonResult GetProjectList(QueryBuilder qb)
        {
            this.FillQueryBuilderFilter<S_I_Engineering>(qb);
            var data = this.entities.Set<S_I_Engineering>().WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetReceiptObjectList(QueryBuilder qb)
        {
            string sql = @" select * from (select S_M_ContractInfo_ReceiptObj.*,S_M_ContractInfo.Name as ContractName,
                            S_M_ContractInfo.PartyA,
                            S_M_ContractInfo.SerialNumber as ContractCode from S_M_ContractInfo_ReceiptObj left join dbo.S_M_ContractInfo
                            on S_M_ContractInfo_ReceiptObj.S_M_ContractInfoID= S_M_ContractInfo.ID ) TableInfo ";
            this.FillQueryBuilderFilter<S_M_ContractInfo>(qb);
            qb.PageSize = 0;
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetContractSituationList(QueryBuilder qb)
        {
            string sql = @" SELECT * FROM dbo.S_M_ContractInfo
                            LEFT JOIN (SELECT Province,ID AS customerid FROM dbo.S_M_CustomerInfo)c
                            ON S_M_ContractInfo.PartyA = c.customerid ";
            this.FillQueryBuilderFilter<S_M_ContractInfo>(qb);
            qb.PageSize = 0;
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

    }
}
