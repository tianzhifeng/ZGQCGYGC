using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeAuto.Logic.Domain
{
    public partial class S_BM_LoanApply
    {

        public void Repayment(decimal? amount)
        {

            var entities = Formula.FormulaHelper.GetEntities<OfficeAutoEntities>();
            if (this.AlreadyReturnAmount == null)
                this.AlreadyReturnAmount = 0.0M;
            this.AlreadyReturnAmount = this.AlreadyReturnAmount + amount;

            var loanValue = 0m;
            var userLoanAccount = entities.S_FC_UserLoanAccount.Where(d => d.UserID == this.ActualBorrower).ToList();
            if (userLoanAccount.Count() > 0)
                loanValue = userLoanAccount.Sum(d => d.AccountValue);
            if (loanValue <= 0)
            {

            }
            else
            {
                var loanInfo = entities.S_FC_UserLoanAccount.Create();
                loanInfo.ID = FormulaHelper.CreateGuid();
                loanInfo.AccountType = LoanAccountType.还款.ToString();
                loanInfo.RelateFormID = this.ID;
                loanInfo.UserDeptID = this.ApplyDept;
                loanInfo.UserDeptName = this.ApplyDeptName;
                loanInfo.UserID = this.ActualBorrower;
                loanInfo.UserName = this.ActualBorrowerName;
                loanInfo.CreateDate = DateTime.Now;
                loanInfo.AccountValue = Convert.ToDecimal(0 - amount);
                entities.S_FC_UserLoanAccount.Add(loanInfo);
            }
        }

        public void Push()
        {
            var entities = Formula.FormulaHelper.GetEntities<OfficeAutoEntities>();
            var loan = entities.S_FC_UserLoanAccount.Create();
            if (!this.LoanAmount.HasValue) { throw new Formula.Exceptions.BusinessException("借款金额不能为空"); }
            loan.ID = Formula.FormulaHelper.CreateGuid();
            loan.UserID = this.ActualBorrower;
            loan.UserName = this.ActualBorrowerName;
            loan.AccountType = LoanAccountType.借款.ToString();
            loan.AccountValue = this.LoanAmount.Value;
            loan.RelateFormID = this.ID;
            var actualBorrowerInfo = FormulaHelper.GetUserInfoByID(this.ActualBorrower);
            loan.UserDeptID = actualBorrowerInfo.UserOrgID;
            loan.UserDeptName = actualBorrowerInfo.UserOrgName;
            loan.CreateDate = DateTime.Now;
            entities.S_FC_UserLoanAccount.Add(loan);
        }

    }
}
