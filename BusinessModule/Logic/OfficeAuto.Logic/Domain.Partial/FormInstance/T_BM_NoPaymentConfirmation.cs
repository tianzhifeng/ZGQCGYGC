using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeAuto.Logic.Domain
{
    public partial class T_BM_NoPaymentConfirmation
    {

        public void Push()
        {
            var loanInfoOfNoPaymentConfirmation = this.T_BM_NoPaymentConfirmation_LoanInfo.ToList();
            var loanInfoIDs = loanInfoOfNoPaymentConfirmation.Select(c => c.LoanApplyID).ToArray();
            var oaEntities = FormulaHelper.GetEntities<OfficeAutoEntities>();
            var loanInfoList = oaEntities.Set<S_BM_LoanApply>().Where(c => loanInfoIDs.Contains(c.ID)).ToList();
            loanInfoList.Update(c => c.UnReturnConfirmDate = DateTime.Now);
        }
    }
}
