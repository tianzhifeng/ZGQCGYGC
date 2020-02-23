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
    public class InvoiceApplyController : EPCFormContorllor<T_M_InvoiceApply>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic["ID"]);
            if (entity == null) entity = new T_M_InvoiceApply();
            this.UpdateEntity(entity, dic);
            entity.Validate();
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "Detail")
            {
                var remainValue = String.IsNullOrEmpty(detail["RemainInvoiceAmount"]) ? 0 : Convert.ToDecimal(detail["RemainInvoiceAmount"]);
                var amount = String.IsNullOrEmpty(detail["ApplyInvoiceAmount"]) ? 0 : Convert.ToDecimal(detail["ApplyInvoiceAmount"]);
                if (remainValue < amount)
                    throw new Formula.Exceptions.BusinessValidationException("【" + detail["PlanReceiptName"] + "】不能超过剩余开票金额");
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "InvoiceDetail")
            {
                var totalValue = detailList.Select(c => String.IsNullOrEmpty(c.GetValue("DetailValue")) ? 0m : Convert.ToDecimal(c.GetValue("DetailValue"))).Sum();
                var invoiceValue = String.IsNullOrEmpty(dic.GetValue("Amount")) ? 0m : Convert.ToDecimal(dic.GetValue("Amount"));
                if (detailList.Count > 0)
                {
                    if (totalValue != invoiceValue)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("发票明细的发票金额总和必须等于开票申请的开票金额【" + invoiceValue + "】");
                    }
                }
            }
        }

        protected override void OnFlowEnd(T_M_InvoiceApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.State = "Wait";
                entity.Submit();
            }
            this.EPCEntites.SaveChanges();
        }
    }
}
