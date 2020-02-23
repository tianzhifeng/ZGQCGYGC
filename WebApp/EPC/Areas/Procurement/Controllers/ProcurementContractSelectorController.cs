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

namespace EPC.Areas.Procurement.Controllers
{
    public class ProcurementContractSelectorController : EPCController
    {
        //获取合同列表
        public JsonResult GetContractList(QueryBuilder qb, string PartyB, string EngineeringInfoID)
        {
            string sql = @"select *,isnull(ContractAmount,0)-isnull(SumInvoiceValue,0) as NoInvoiceAmount
            ,isnull(ContractAmount,0)-isnull(SumPaymentValue,0) as NoPaymentAmount from S_P_ContractInfo";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            qb.Add("ContractState", QueryMethod.Equal, "Sign");
            if (!String.IsNullOrEmpty(EngineeringInfoID))
                qb.Add("EngineeringInfoID", QueryMethod.Equal, EngineeringInfoID);
            if (!String.IsNullOrEmpty(PartyB))
                qb.Add("PartyB", QueryMethod.Equal, PartyB);


            //增加合同ID过滤 songliangliang 2018.6.8
            if (!String.IsNullOrEmpty(GetQueryString("ContractInfoID")))
            {
                qb.Add("ID", QueryMethod.Equal, GetQueryString("ContractInfoID"));
            }
            //增加合同类别(采购/分包)过滤 songliangliang 2018.6.8
            else if (!String.IsNullOrEmpty(GetQueryString("ContractProperty")))
            {
                qb.Add("ContractProperty", QueryMethod.Equal, GetQueryString("ContractProperty"));
            }            

            var data = db.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetPaymentObjectList(QueryBuilder qb, string ContractInfoID)
        {
            string sql = @"select *,(isnull(PlanPaymentValue,0)-isnull(SumInvoiceValue,0)) as RemainInvoiceValue 
from dbo.S_P_ContractInfo_PaymentObj where S_P_ContractInfoID ='{0}' {1} order by SortIndex ";
            var whereStr = qb.GetWhereString(false);
            sql = String.Format(sql, ContractInfoID, whereStr);
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult GetContractPlanPaymentList(QueryBuilder qb, string ContractInfoID)
        {
            //有计划优先计划，没计划就用付款项
            string sql = @"select * from (
select pobj.ID,pobj.ID PaymentObj,ppplan.ID PlanID,pobj.S_P_ContractInfoID ContractInfo,
ISNULL(ppplan.Condition,pobj.Condition) Condition,
ISNULL(ppplan.PaymentObjName,pobj.Name) PaymentObjName,
ISNULL(ppplan.PlanValue,isnull(pobj.PlanPaymentValue,0)-isnull(pobj.SumPaymentValue,0)) PlanValue,
CONVERT(varchar(100),  ISNULL(ppplan.PlanDate,pobj.PlanPaymentDate), 23) PlanDate,
con.Name ContractInfoName,con.SerialNumber ContractInfoCode,con.PartyB SupplierInfo,
con.PartyBName SupplierInfoName,pobj.SortIndex
from S_P_ContractInfo_PaymentObj pobj
outer apply ( select top 1 * from S_P_ContractInfo_PaymentObj_PaymentPlan pplan 
where pobj.id=pplan.PaymentObj  and State in ('UnPayment')
order by pplan.ID) ppplan
left join S_P_ContractInfo con on con.ID=pobj.S_P_ContractInfoID
) tmp";
            if (!String.IsNullOrEmpty(ContractInfoID))
                qb.Add("ContractInfo", QueryMethod.Equal, ContractInfoID);
            if (qb.DefaultSort)
                qb.SetSort("SortIndex", "asc");
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            var data = db.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetContractInvoiceList(QueryBuilder qb, string ContractInfoID)
        {
            string sql = @" select S_P_Invoice.*,isnull(SumRelationValue,0) as SumRelationValue,
isnull(InvoiceValue,0)-isnull(SumRelationValue,0) as RemainRelationValue from S_P_Invoice
left join (select Sum(RelationValue) as SumRelationValue,InvoiceID from dbo.S_P_Payment_InvoiceRelation
group by InvoiceID) RelationInfo
on RelationInfo.InvoiceID = S_P_Invoice.ID where S_P_Invoice.ContractInfo='{0}' and State='Normal'";
            string whereStr = qb.GetWhereString(false);
            if (!String.IsNullOrEmpty(whereStr)) sql += whereStr;
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, ContractInfoID));
            return Json(dt);
        }
    }
}
