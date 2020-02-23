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
using Base.Logic.Domain;

namespace EPC.Areas.Construction.Controllers
{
    public class ContractReviewController : EPCFormContorllor<T_P_FbContractReview>
    {
        protected override void OnFlowEnd(T_P_FbContractReview entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            var contract = EPCEntites.Set<S_P_ContractInfo>().Find(entity.ContractID);
            if(contract == null)
                throw new Formula.Exceptions.BusinessValidationException("找不到id为【" + entity.ContractID + "】的分包合同");

            contract.ContractState = "Review";
            EPCEntites.SaveChanges();
        }

        public JsonResult CheckReviewFlowStart(string ContractID)
        {
            //优先将暂存的流程启动
            var tmp = this.EPCEntites.Set<T_P_FbContractReview>().FirstOrDefault(a => a.FlowPhase == "Start" && a.ContractID == ContractID);
            if (tmp != null)
                return Json(tmp.ToDic());
            //如果已在流程中，则不允许修改
            var exsit = this.EPCEntites.Set<T_P_FbContractReview>().FirstOrDefault(a => a.ContractID == ContractID
                && a.FlowPhase != "End");
            if (exsit != null)
                throw new Formula.Exceptions.BusinessValidationException("【" + exsit.CreateUser + "】已经发起了合同评审流程，无法重复发起");
            return Json("");
        }
    }
}
