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

namespace EPC.Areas.Contract.Controllers
{
    public class InvoiceController : EPCFormContorllor<S_M_Invoice>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
            //InvoiceDate必须有值
            if (string.IsNullOrEmpty(dic.GetValue("InvoiceDate")))
            {
                if (!string.IsNullOrEmpty(dic.GetValue("CreateDate")))
                {
                    dic.SetValue("InvoiceDate", dic.GetValue("CreateDate"));
                }
                else
                {
                    dic.SetValue("InvoiceDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
        }



        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            if (!isNew)
            {
                var contract = this.GetEntityByID<S_M_ContractInfo>(dic.GetValue("ContractInfo"));
                dic.SetValue("ContractValue", contract.ContractRMBValue);
                var invoiceID = dic.GetValue("ID");
                var contractInvoiceValue = Convert.ToDecimal(contract.S_M_Invoice.Where(d => d.ID != invoiceID).Sum(d => d.InvoiceValue));
                dic.SetValue("ContractInvoiceValue", contractInvoiceValue);
                var remainInvoiceValue = Convert.ToDecimal(contract.ContractRMBValue) - contractInvoiceValue;
                if (remainInvoiceValue < 0) remainInvoiceValue = 0;
                dic.SetValue("RemainInvoiceValue", remainInvoiceValue);
            }
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "ReceiptObjRelation")
            {
                var relationValue = String.IsNullOrEmpty(detail.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(detail.GetValue("RelationValue"));
                var factInvoiceValue = String.IsNullOrEmpty(detail.GetValue("InvoiceValue")) ? 0m : Convert.ToDecimal(detail.GetValue("InvoiceValue"));
                var receiptValue = String.IsNullOrEmpty(detail.GetValue("ReceiptObjectValue")) ? 0m : Convert.ToDecimal(detail.GetValue("ReceiptObjectValue"));
                var remainInvoiceValue = receiptValue - factInvoiceValue;
                if (relationValue > remainInvoiceValue) throw new Formula.Exceptions.BusinessValidationException("【" + detail.GetValue("ReceiptObjectName") + "】的开票总金额不能大于收款项金额");
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList,
            Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "ReceiptObjRelation")
            {
                var sumRelation = 0M;
                var invoiceValue = String.IsNullOrEmpty(dic.GetValue("InvoiceValue")) ? 0m : Convert.ToDecimal(dic.GetValue("InvoiceValue"));
                foreach (var item in detailList)
                {
                    var relationValue = String.IsNullOrEmpty(item.GetValue("RelationValue")) ? 0m : Convert.ToDecimal(item.GetValue("RelationValue"));
                    sumRelation += relationValue;
                }
                if (sumRelation > invoiceValue) throw new Formula.Exceptions.BusinessValidationException("关联的收款项金额不能大于开票金额");
            }
            if (subTableName == "InvoiceDetail")
            {
                var totalValue = detailList.Select(c => String.IsNullOrEmpty(c.GetValue("DetailValue")) ? 0m : Convert.ToDecimal(c.GetValue("DetailValue"))).Sum();
                var invoiceValue = String.IsNullOrEmpty(dic.GetValue("InvoiceValue")) ? 0m : Convert.ToDecimal(dic.GetValue("InvoiceValue"));
                if (detailList.Count > 0)
                {
                    if (totalValue != invoiceValue)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("发票明细的发票金额总和必须等于开票金额【" + invoiceValue + "】");
                    }
                }
            }
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic.GetValue("ID"));
            entity.CauculateTax();
            var contract = this.GetEntityByID<S_M_ContractInfo>(dic.GetValue("ContractInfo"));
            contract.SummaryInvoiceData();
            foreach (var item in contract.S_M_ContractInfo_ReceiptObj.ToList())
            {
                item.SummaryInvoiceValue();
            }
            this.EPCEntites.SaveChanges();
        }

        public JsonResult Invalid(string invoiceList)
        {
            var list = JsonHelper.ToList(invoiceList);
            foreach (var item in list)
            {
                var invoice = this.GetEntityByID(item.GetValue("ID"));
                if (invoice == null)
                    throw new Formula.Exceptions.BusinessException("未能找到ID为【" + item.GetValue("ID") + "】的开票信息，无法进行作废操作");
                invoice.Invalid();
            }
            this.EPCEntites.SaveChanges();
            return Json("");
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
