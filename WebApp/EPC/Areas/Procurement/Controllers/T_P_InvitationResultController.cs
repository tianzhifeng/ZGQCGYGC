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

namespace EPC.Areas.Procurement.Controllers
{
    public class InvitationResultController : EPCFormContorllor<T_P_InvitationResult>
    {
        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "DeviceInfo")
            {
                var detailJson = JsonHelper.ToJson(detail);
                detail.SetValue("Data", detailJson);
            }
        }

        public JsonResult ValidateInvitation(string InvitationID)
        {
           var obj =  this.EPCSQLDB.ExecuteScalar("select count(0) from T_P_InvitationResult where InvitationID='" + InvitationID + "' and FlowPhase='End'");
           if (Convert.ToInt32(obj) > 0)
           {
               throw new Formula.Exceptions.BusinessValidationException("已经启动了供应商选择审批，无法再次发起");
           }
            return Json("");
        }

        protected override void OnFlowEnd(T_P_InvitationResult entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();

            }
        }

    }
}
