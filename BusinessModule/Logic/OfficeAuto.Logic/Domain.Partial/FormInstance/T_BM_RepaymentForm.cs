using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeAuto.Logic.Domain
{
    public partial class T_BM_RepaymentForm
    {

        public void Push()
        {
            //本次还款单对应的
            var loanInfosOfRF = this.T_BM_RepaymentForm_LoanInfo.ToList();
            var loanInfoIDs = loanInfosOfRF.Select(c => c.LoanApplyID).ToArray();
            var officeAutoEntities = FormulaHelper.GetEntities<OfficeAutoEntities>();
            var loanInfos = officeAutoEntities.Set<S_BM_LoanApply>().Where(c => loanInfoIDs.Contains(c.ID)).ToList();
            foreach (var loanInfoOfRF in loanInfosOfRF)
            {
                var loanInfo = loanInfos.FirstOrDefault(c => c.ID == loanInfoOfRF.LoanApplyID);
                if (loanInfo != null)
                    loanInfo.Repayment(loanInfoOfRF.ThisTimeAmount);
            }
        }
    }
}
