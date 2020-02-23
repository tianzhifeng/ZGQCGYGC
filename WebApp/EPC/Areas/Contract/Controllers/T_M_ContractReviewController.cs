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
using Formula.Exceptions;

namespace EPC.Areas.Contract.Controllers
{
    public class ContractReviewController : EPCFormContorllor<T_M_ContractReview>
    {
        protected override void OnFlowEnd(T_M_ContractReview entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            //entity.ContractState = "Review";
            T_M_ContractReview contract = EPCEntites.Set<T_M_ContractReview>().Find(entity.ID);
            contract.Approver = CurrentUserInfo.UserID;
            contract.ApproverName = CurrentUserInfo.UserName;
            contract.ApproverDate = DateTime.Now;
            //if (contract == null)
            //    throw new BusinessException("已无法找到id为【" + entity.Contract + "】的合同信息");
            //contract.ContractState = "Review";
            //EPCEntites.SaveChanges();
        }
    }
}
