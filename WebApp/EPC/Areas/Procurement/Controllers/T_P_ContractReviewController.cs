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

namespace EPC.Areas.Procurement.Controllers
{
    public class ContractReviewController : EPCFormContorllor<T_P_ContractReview>
    {
        protected override void OnFlowEnd(T_P_ContractReview entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            S_P_ContractInfo contract = EPCEntites.Set<S_P_ContractInfo>().Find(entity.ContractInfo);
            if (contract == null)
                throw new BusinessException("已无法找到id为【" + entity.ContractInfo + "】的采购合同信息");
            contract.ContractState = "Review";

            //foreach (var content in contract.S_P_ContractInfo_Content)
            //{
            //    var tContent = entity.T_P_ContractReview_Content.FirstOrDefault(a => a.PBomID == content.PBomID);
            //    if(tContent != null)
            //    {
            //        content.SFZJ = tContent.SFZJ;
            //    }
            //}

            EPCEntites.SaveChanges();
        }
    }
}
