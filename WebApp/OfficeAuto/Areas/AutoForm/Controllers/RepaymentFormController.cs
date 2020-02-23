using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Formula;
using Workflow.Logic.Domain;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class RepaymentFormController : OfficeAutoFormContorllor<T_BM_RepaymentForm>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            base.AfterGetData(dt, isNew, upperVersionID);
            if (isNew == true)
            {
                //当前人的未还款金额汇总
                var loanInfos = this.BusinessEntities.Set<S_BM_LoanApply>().Where(c => c.ActualBorrower == this.CurrentUserInfo.UserID).ToList();
                var unReturnAmount = loanInfos.Sum(c => c.LoanAmount) - loanInfos.Sum(c => c.AlreadyReturnAmount);

                dt.Rows[0]["UnReturnAmount"] = unReturnAmount;
                dt.AcceptChanges();

            }
        }

        protected override void OnFlowEnd(T_BM_RepaymentForm entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            entity.Push();
            this.BusinessEntities.SaveChanges();
        }

    }
}
