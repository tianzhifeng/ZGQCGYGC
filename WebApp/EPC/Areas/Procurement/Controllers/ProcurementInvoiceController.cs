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
    public class ProcurementInvoiceController : EPCFormContorllor<S_P_Invoice>
    {

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            if (!isNew)
            {
                var contract = this.GetEntityByID<S_P_ContractInfo>(dic.GetValue("ContractInfo"));
                dic.SetValue("ContractValue", contract.ContractAmount);
                var invoiceID = dic.GetValue("ID");
                var contractInvoiceValue = Convert.ToDecimal(contract.S_P_Invoice.Sum(d => d.InvoiceValue));
                var alreadyInvoiceValue = Convert.ToDecimal(contract.S_P_Invoice.Where(d => d.ID != invoiceID).Sum(d => d.InvoiceValue));
                dic.SetValue("ContractInvoiceValue", contractInvoiceValue);
                var remainInvoiceValue = Convert.ToDecimal(contract.ContractAmount) - contractInvoiceValue;
                if (remainInvoiceValue < 0) remainInvoiceValue = 0;
                dic.SetValue("AlreadyInvoiceValue", alreadyInvoiceValue);


                var entity = this.GetEntityByID(dic.GetValue("ID"));
                dic.SetValue("InvoiceValue", Math.Abs(entity.InvoiceValue ?? 0));//兼容红冲
                var detailList = entity.S_P_Invoice_PaymentObjRelation.ToList();
                foreach (var detail in detailList)
                {
                    detail.RelationValue = Math.Abs(detail.RelationValue ?? 0);//兼容红冲
                }
                dic.SetValue("PaymentObjRelation", JsonHelper.ToJson(detailList));
            }
        }  

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var contract = this.GetEntityByID<S_P_ContractInfo>(dic.GetValue("ContractInfo"));
            if (contract == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的合同信息");
            var invoiceID = dic.GetValue("ID");
            var invoiceValue = String.IsNullOrEmpty(dic.GetValue("InvoiceValue")) ? 0m : Convert.ToDecimal(dic.GetValue("InvoiceValue"));
            var remainValue = Convert.ToDecimal(contract.S_P_Invoice.Where(d => d.ID != invoiceID).Sum(d => d.InvoiceValue));
            var contractValue = Convert.ToDecimal(contract.ContractAmount);           

            if(dic.GetValue("InvoiceType") == "CreditNote")
            {
                if (Math.Abs(invoiceValue) > Math.Abs(remainValue))
                    throw new Formula.Exceptions.BusinessValidationException("红冲发票金额不能超过已开票金额");

                var tmp = dic.GetValue("InvoiceValue");
                decimal val = 0;
                decimal.TryParse(tmp, out val);
                dic.SetValue("InvoiceValue", (-val).ToString());//红冲取反
            }
            else
            {
                if (invoiceValue + remainValue > contractValue)
                    throw new Formula.Exceptions.BusinessValidationException("累计开票金额不能超过合同总金额");
            }
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if(subTableName == "PaymentObjRelation")
            {
                var relationValue = String.IsNullOrEmpty(detail.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(detail.GetValue("RelationValue"));
                var factInvoiceValue = String.IsNullOrEmpty(detail.GetValue("InvoiceValue")) ? 0m : Convert.ToDecimal(detail.GetValue("InvoiceValue"));
                var paymentValue = String.IsNullOrEmpty(detail.GetValue("PaymentObjectValue")) ? 0m : Convert.ToDecimal(detail.GetValue("PaymentObjectValue"));
                var remainInvoiceValue = paymentValue - factInvoiceValue;

                if (dic.GetValue("InvoiceType") == "CreditNote")
                {
                    if (Math.Abs(relationValue) > Math.Abs(factInvoiceValue)) throw new Formula.Exceptions.BusinessValidationException("【" + detail.GetValue("PaymentObjectName") + "】的本次收票金额不能大于已收票金额");

                    var tmp = detail.GetValue("RelationValue");
                    decimal val = 0;
                    decimal.TryParse(tmp, out val);
                    detail.SetValue("RelationValue", (-val).ToString());//红冲取反                
                }
                else
                {
                    if (relationValue > remainInvoiceValue) throw new Formula.Exceptions.BusinessValidationException("【" + detail.GetValue("PaymentObjectName") + "】的收票总金额不能大于付款项金额");
                }
            }            
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList,
            Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "PaymentObjRelation")
            {
                var sumRelation = 0M;
                var invoiceValue = String.IsNullOrEmpty(dic.GetValue("InvoiceValue")) ? 0m : Convert.ToDecimal(dic.GetValue("InvoiceValue"));
                foreach (var item in detailList)
                {
                    var relationValue = String.IsNullOrEmpty(item.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(item.GetValue("RelationValue"));
                    sumRelation += relationValue;
                }
                if (Math.Abs(sumRelation) > Math.Abs(invoiceValue)) throw new Formula.Exceptions.BusinessValidationException("关联的付款项金额不能大于收票金额");
            }            
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic.GetValue("ID"));
            entity.CauculateTax();
            var contract = this.GetEntityByID<S_P_ContractInfo>(dic.GetValue("ContractInfo"));
            contract.SummaryInvoiceData();
            foreach (var item in contract.S_P_ContractInfo_PaymentObj.ToList())
            {
                item.SummaryInvoiceValue();
            }
            this.EPCEntites.SaveChanges();
        }

        public override JsonResult Delete()
        {
            if (!String.IsNullOrEmpty(Request["ListIDs"]))
            {
                foreach (var item in Request["ListIDs"].Split(','))
                {
                    var invoice = this.GetEntityByID(item);
                    invoice.Delete();
                }
            }
            this.EPCEntites.SaveChanges();
            return Json("");
        }
    }
}
