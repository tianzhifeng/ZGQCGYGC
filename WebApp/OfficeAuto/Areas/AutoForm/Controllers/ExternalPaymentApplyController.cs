using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Config.Logic;
using Formula;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class ExternalPaymentApplyController : OfficeAutoFormContorllor<T_TM_ExternalPaymentApply>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            var strContractAmount = dic.GetValue("ContractAmount");
            var strContractID = dic.GetValue("ContractID");
            var strID = dic.GetValue("ID");
            var strThisTimePaymentAmount = dic.GetValue("ThisTimePaymentAmount");
            var thisTimePaymentAmount = 0.0M;
            if (!string.IsNullOrWhiteSpace(strThisTimePaymentAmount))
                thisTimePaymentAmount = Convert.ToDecimal(strThisTimePaymentAmount);
            var alreadyPaymentAmount = this.BusinessEntities.Set<T_TM_ExternalPaymentApply>().Where(c => c.ContractID == strContractID && c.ID != strID).Sum(c => c.ThisTimePaymentAmount);
            var contractAmount = 0.0M;
            if (!string.IsNullOrWhiteSpace(strContractAmount))
                contractAmount = Convert.ToDecimal(strContractAmount);
            if (alreadyPaymentAmount + thisTimePaymentAmount > contractAmount)
                throw new Formula.Exceptions.BusinessException("本次付款金额+已付款金额超过合同额，请修改！");
            base.BeforeSave(dic, formInfo, isNew);
        }

    }
}
