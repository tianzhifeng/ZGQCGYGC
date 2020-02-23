using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Formula.Helper;
using Workflow.Logic.Domain;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class NoPaymentConfirmationController : OfficeAutoFormContorllor<T_BM_NoPaymentConfirmation>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            base.AfterGetData(dt, isNew, upperVersionID);
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                var createUserWorkNo = row["CreateUserWorkNo"] == null ? "" : row["CreateUserWorkNo"].ToString();
                var currentUserInfo = this.CurrentUserInfo;
                var Id = row["ID"].ToString();
                if (createUserWorkNo == currentUserInfo.WorkNo)
                {
                    //首环节，重新获取借款明细表
                    string loanInfoSql = @"select ID as LoanApplyID, SerialNumber as LoanSerialNumber, ApplyDate as LoanDate, LoanAmount
,AlreadyReturnAmount,UnReturnAmount,LoanReason,TaskNature,UnReturnReason,PlanRepayDate from 
(
	select la.*,(ISNULL(la.LoanAmount,0)-ISNULL(la.AlreadyReturnAmount,0)) as UnReturnAmount from S_BM_LoanApply la  
	where FormCode = 'BM_Loanapply' and IsBranchLoan = 'False' and FlowPhase = 'End' and ActualBorrower='{0}'
) aa 
left join 
(
	select LoanApplyID,UnReturnReason,PlanRepayDate from T_BM_NoPaymentConfirmation_LoanInfo 
	where T_BM_NoPaymentConfirmationID='{1}'
)bb on aa.ID = bb.LoanApplyID
where UnReturnAmount>0  ";
                    loanInfoSql = string.Format(loanInfoSql, currentUserInfo.UserID, Id);
                    var loanInfoDt = this.SQLDB.ExecuteDataTable(loanInfoSql);
                    row["LoanInfo"] = JsonHelper.ToJson(loanInfoDt);
                }
                if (isNew == true)
                    row["CreateUserPhone"] = currentUserInfo.UserPhone;
                dt.AcceptChanges();
            }
        }

        protected override void OnFlowEnd(T_BM_NoPaymentConfirmation entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            if (entity == null)
                throw new Formula.Exceptions.BusinessException("获取当前未还款确认失败，请联系管理员！");
            entity.Push();
            this.BusinessEntities.SaveChanges();
        }
    }
}
