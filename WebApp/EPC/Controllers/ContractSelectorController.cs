using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using MvcAdapter;


namespace EPC.Controllers
{
    public class ContractSelectorController : EPCController
    {

        //获取合同列表
        public JsonResult GetContractList(QueryBuilder qb, string PartyA)
        {
            string sql = @"select * from (select S_M_ContractInfo.*,isnull(ContractRMBValue,0)-isnull(SumInvoiceValue,0) as NoInvoiceAmount
,isnull(ContractRMBValue,0)-isnull(SumReceiptValue,0) as NoReceiptAmount,BankAccounts,Telephone,Address,
TaxAccounts, BankAddress,BankName
from S_M_ContractInfo left join S_M_CustomerInfo 
on S_M_ContractInfo.PartyA = S_M_CustomerInfo.ID where SignDate is not null) ContractInfo";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            qb.Add("ContractState", QueryMethod.Equal, "Sign");
            if (!String.IsNullOrEmpty(PartyA))
                qb.Add("PartyA", QueryMethod.Equal, PartyA);
            string ProjectInfo = this.GetQueryString("ProjectInfo");
            if (!String.IsNullOrEmpty(ProjectInfo))
            {
                qb.Add("ProjectInfo", QueryMethod.Equal, ProjectInfo);
            }
            var data = db.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetReceiptObjectList(QueryBuilder qb, string ContractInfoID)
        {
            string sql = @"select *,(isnull(ReceiptValue,0)-isnull(FactInvoiceValue,0)-isnull(FactBadValue,0)) as RemainInvoiceValue from dbo.S_M_ContractInfo_ReceiptObj
where S_M_ContractInfoID ='{0}' {1} order by SortIndex ";
            var whereStr = qb.GetWhereString(false);
            sql = String.Format(sql, ContractInfoID, whereStr);
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult GetContractPlanReceiptList(QueryBuilder qb, string ContractInfoID)
        {
            var state = PlanReceiptState.UnReceipt.ToString();
            var data = this.entities.Set<S_M_ContractInfo_ReceiptObj_PlanReceipt>().Where(d => d.ContractInfoID == ContractInfoID
                && d.State == state).Where(qb).OrderBy(d => d.PlanDate).ToList();
            return Json(data);
        }

        public JsonResult GetContractInvoiceList(QueryBuilder qb, string ContractInfoID)
        {
            string sql = @" select S_M_Invoice.*,isnull(SumRelationValue,0) as SumRelationValue,
isnull(InvoiceValue,0)-isnull(SumRelationValue,0) as RemainRelationValue from S_M_Invoice
left join (select Sum(RelationValue) as SumRelationValue,InvoiceID from dbo.S_M_Receipt_InvoiceRelation
group by InvoiceID) RelationInfo
on RelationInfo.InvoiceID = S_M_Invoice.ID where S_M_Invoice.ContractInfo='{0}' and State='Normal'";
            string whereStr = qb.GetWhereString(false);
            if (!String.IsNullOrEmpty(whereStr)) sql += whereStr;
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, ContractInfoID));
            return Json(dt);
        }

        public JsonResult GetCreditNoteReceiptList(QueryBuilder qb, string ContractInfoID)
        {
            string sql = @"select S_M_ContractInfo_ReceiptObj.ID, S_M_ContractInfo.ContractRMBValue,S_M_ContractInfo.SumInvoiceValue, S_M_ContractInfo_ReceiptObj.Name, S_M_ContractInfo_ReceiptObj.PlanFinishDate, isnull(MinusRelationValue,0) as MinusRelationValue, (isnull(FactInvoiceValue,0)- isnull(ApplyCreditValue,0)) as RemainValue from dbo.S_M_ContractInfo_ReceiptObj
inner join S_M_ContractInfo on S_M_ContractInfo_ReceiptObj.S_M_ContractInfoID = S_M_ContractInfo.ID
left join (select abs(sum(isnull(RelationValue,0))) as MinusRelationValue,S_M_ReceiptObjID from S_M_Invoice_ReceiptObjRelation inner join S_M_Invoice on S_M_Invoice.ID = S_M_Invoice_ReceiptObjRelation.S_M_InvoiceID where S_M_Invoice.InvoiceType = 'CreditNote' group by S_M_ReceiptObjID ) InvoiceReceiptObj on InvoiceReceiptObj.S_M_ReceiptObjID = S_M_ContractInfo_ReceiptObj.ID
left join (select abs(sum(isnull(T_C_CreditNoteApply_Detail.CreditValue,0))) as ApplyCreditValue,PlanReceiptID from T_C_CreditNoteApply inner join T_C_CreditNoteApply_Detail on T_C_CreditNoteApply.ID = T_C_CreditNoteApply_Detail.T_C_CreditNoteApplyID where T_C_CreditNoteApply.FlowPhase <> 'End' group by  PlanReceiptID) CreditNoteApplyDetail on CreditNoteApplyDetail.PlanReceiptID  = S_M_ContractInfo_ReceiptObj.ID
where S_M_ContractInfoID = '{0}' order by PlanFinishDate";
            string whereStr = qb.GetWhereString(false);
            sql = string.Format(sql, ContractInfoID, whereStr);
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, ContractInfoID));
            return Json(dt);
        }

        public JsonResult GetCreditNoteInvoiceList(QueryBuilder qb, string ContractInfoID)
        {
            string sql = @"select s_m_invoice.*, (InvoiceValue - isnull(CreditValue,0)) as RemainValue, isnull(CreditValue,0) CreditValue from s_m_invoice left join (select sum(CreditValue) as CreditValue,InvoiceID from T_C_CreditNoteApply_InvoiceDetail group by InvoiceID) apply on apply.InvoiceID = s_m_invoice.ID
where contractinfo = '{0}' and invoicetype <> 'CreditNote'  and State = 'Normal'";
            string whereStr = qb.GetWhereString(false);
            sql = string.Format(sql, ContractInfoID, whereStr);
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, ContractInfoID));
            return Json(dt);
        }
    }
}
