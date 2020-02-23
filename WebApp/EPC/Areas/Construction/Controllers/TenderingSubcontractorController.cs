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
using Workflow.Logic.Domain;

namespace EPC.Areas.Construction.Controllers
{
    public class TenderingSubcontractorController : EPCFormContorllor<T_C_TenderingSubcontractor>
    {
        protected override void OnFlowEnd(T_C_TenderingSubcontractor entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            var tenderingID = entity.T_C_TenderingInfoID;
            if (!string.IsNullOrEmpty(tenderingID))
            {
                var tenderingInfo = this.EPCEntites.Set<T_C_TenderingInfo>().Where(x => x.ID == tenderingID).FirstOrDefault();
                if (tenderingInfo != null)
                {
                    tenderingInfo.BidAmount = entity.BidAmount;
                    tenderingInfo.BidSubcontractor = entity.BidSubcontractor;
                    tenderingInfo.BidSubcontractorName = entity.BidSubcontractorName;

                    this.EPCEntites.Set<S_P_Invitation_Winner>().Delete(x => x.InvitationID == tenderingID);
                    S_P_Invitation_Winner sw = new S_P_Invitation_Winner();
                    sw.ID = FormulaHelper.CreateGuid();
                    sw.Winner = entity.BidSubcontractor;
                    sw.WinnerName = entity.BidSubcontractorName;
                    sw.WinPrice = Convert.ToDecimal(entity.BidAmount);
                    sw.InvitationID = tenderingID;
                    this.EPCEntites.Set<S_P_Invitation_Winner>().Add(sw);

                    this.EPCEntites.SaveChanges();
                }
            }
        }

    }
}
