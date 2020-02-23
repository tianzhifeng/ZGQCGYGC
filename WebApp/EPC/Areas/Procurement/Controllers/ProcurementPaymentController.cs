using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using Formula.Helper;

namespace EPC.Areas.Procurement.Controllers
{
    public class ProcurementPaymentController : EPCFormContorllor<S_P_Payment>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            var paymentD = dic.GetValue("ID");
            if (!isNew)
            {
                var contract = this.GetEntityByID<S_P_ContractInfo>(dic.GetValue("ContractInfo"));
                if (contract == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的合同信息");
                dic.SetValue("ContractValue", contract.ContractAmount);
                var contractPaymentValue = Convert.ToDecimal(contract.S_P_Payment.Where(d => d.ID != paymentD).Sum(d => d.PaymentValue));
                dic.SetValue("ContractPaymentValue", contractPaymentValue);
                var remainPaymentValue = Convert.ToDecimal(contract.ContractAmount) - contractPaymentValue;
                if (remainPaymentValue < 0) remainPaymentValue = 0;
                dic.SetValue("RemainPaymentValue", remainPaymentValue);

                string sql = @"select S_P_Payment_InvoiceRelation.ID,S_P_Invoice.InvoiceCode,
S_P_Invoice.InvoiceValue,S_P_Invoice.InvoiceDate, SumRelationInfo.SumRelateValue,
S_P_Payment_InvoiceRelation.SortIndex
,S_P_Payment_InvoiceRelation.InvoiceID,
S_P_Payment_InvoiceRelation.RelationValue
 from S_P_Payment_InvoiceRelation
left join S_P_Invoice on S_P_Invoice.ID=S_P_Payment_InvoiceRelation.InvoiceID
left join (select Sum(RelationValue) as SumRelateValue,InvoiceID
from S_P_Payment_InvoiceRelation where S_P_PaymentID !='{0}'
group by InvoiceID) SumRelationInfo on  SumRelationInfo.InvoiceID = S_P_Invoice.ID
where S_P_PaymentID='{0}'";
                var invoiceRelateionDt = this.EPCSQLDB.ExecuteDataTable(String.Format(sql, dic.GetValue("ID")));
                dic.SetValue("InvoiceRelation", JsonHelper.ToJson(invoiceRelateionDt));
            }

        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var contract = this.GetEntityByID<S_P_ContractInfo>(dic.GetValue("ContractInfo"));
            if (contract == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的合同信息");
            //付款不能超过合同金额
            var paymentD = dic.GetValue("ID");
            var paymentValue = String.IsNullOrEmpty(dic.GetValue("PaymentValue")) ? 0m : Convert.ToDecimal(dic.GetValue("PaymentValue"));
            var remainValue = Convert.ToDecimal(contract.S_P_Payment.Where(d => d.ID != paymentD).Sum(d => d.PaymentValue));
            var contractValue = Convert.ToDecimal(contract.ContractAmount);
            if (paymentValue + remainValue > contractValue)
                throw new Formula.Exceptions.BusinessValidationException("累计付款金额不能超过合同总金额");
        }
        
        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "InvoiceRelation")
            {
                var invoiceValue = String.IsNullOrEmpty(detail.GetValue("InvoiceValue")) ? 0m : Convert.ToDecimal(detail.GetValue("InvoiceValue"));
                var relationValue = String.IsNullOrEmpty(detail.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(detail.GetValue("RelationValue"));
                var invoice = this.GetEntityByID<S_P_Invoice>(detail.GetValue("InvoiceID"));
                if (invoice == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的发票");
                var sumRelationValue = invoice.S_P_Payment_InvoiceRelation.Where(d => d.ID != detail.GetValue("ID")).Sum(d => d.RelationValue);
                var canRelationValue = invoiceValue - sumRelationValue;
                if ((sumRelationValue + relationValue) > invoiceValue)
                    throw new Formula.Exceptions.BusinessValidationException("【" + invoice.InvoiceCode + "】发票冲销金额不能大于可冲销金额【" + canRelationValue + "】");
            }
            else if (subTableName == "PaymentObjRelation")
            {
                var planValue = String.IsNullOrEmpty(detail.GetValue("PlanValue")) ? 0m : Convert.ToDecimal(detail.GetValue("PlanValue"));
                var relationValue = String.IsNullOrEmpty(detail.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(detail.GetValue("RelationValue"));
                if (relationValue > planValue)
                    throw new Formula.Exceptions.BusinessValidationException("【" + detail.GetValue("PlanName") + "】计划付款关联金额不能大于计划金额【" + planValue + "】");
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "PaymentObjRelation")
            {
                var sumRelationValue = detailList.Select(a => String.IsNullOrEmpty(a.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(a.GetValue("RelationValue"))).Sum();
                var receiptValue = String.IsNullOrEmpty(dic.GetValue("PaymentValue")) ? 0m : Convert.ToDecimal(dic.GetValue("PaymentValue"));
                if (sumRelationValue > receiptValue)
                    throw new Formula.Exceptions.BusinessValidationException("付款项或计划关联总金额不能大于付款金额【" + receiptValue + "】");
                var ids = detailList.Where(c => c.ContainsKey("ID") && !string.IsNullOrEmpty(c["ID"])).Select(c => c["ID"]).ToList();
                var paymentD = dic.GetValue("ID");
                var removeRelations = this.EPCEntites.Set<S_P_Payment_PaymentObjRelation>().Where(d => d.S_P_PaymentID == paymentD
                    && !ids.Contains(d.ID)).ToList();
                foreach (var item in removeRelations)
                {
                    var plan = item.S_P_ContractInfo_PaymentObj.S_P_ContractInfo_PaymentObj_PaymentPlan.FirstOrDefault(a => a.ID == item.PlanID);
                    this.EPCEntites.Set<S_P_Payment_PaymentObjRelation>().Remove(item);
                    if (plan != null)
                        plan.Reset();
                }
            }
            else if (subTableName == "PaymentDetail")
            {
                if (detailList.Count > 0)
                {
                    var sumRelationValue = detailList.Select(a => String.IsNullOrEmpty(a.GetValue("PaymentValue")) ? 0m : Convert.ToDecimal(a.GetValue("PaymentValue"))).Sum();
                    var receiptValue = String.IsNullOrEmpty(dic.GetValue("PaymentValue")) ? 0m : Convert.ToDecimal(dic.GetValue("PaymentValue"));
                    if (sumRelationValue != receiptValue)
                        throw new Formula.Exceptions.BusinessValidationException("合同明细关联金额总计必须等于付款金额【" + receiptValue + "】");
                }              
            }
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic.GetValue("ID"));
            if (entity == null) throw new Formula.Exceptions.BusinessValidationException("保存失败，未能找到指定的付款记录");
            //设置付款与发票和计划的关联关系
            entity.SetRelateInfo();

            //重新汇总合同上的实际付款数据
            var contract = this.GetEntityByID<S_P_ContractInfo>(dic.GetValue("ContractInfo"));
            contract.SummaryPaymentData();

            //重新汇总合同下各付款项的实际付款数据
            foreach (var item in contract.S_P_ContractInfo_PaymentObj.ToList())
            {
                item.SummaryPaymentValue();
            }
            entity.ToCost(S_T_DefineParams.Params.GetValue("PaymentAutoSettle").ToLower() == "true");
            this.EPCEntites.SaveChanges();
        }

        public override JsonResult Delete()
        {
            if (!String.IsNullOrEmpty(Request["ListIDs"]))
            {
                var ids = Request["ListIDs"].Split(',');
                foreach (var Id in ids)
                {
                    var entity = this.GetEntityByID(Id);
                    if (entity == null) continue;
                    entity.Delete();
                    this.EPCEntites.SaveChanges();
                }
                this.EPCEntites.SaveChanges();
            }
            return Json("");
        }
    }
}
