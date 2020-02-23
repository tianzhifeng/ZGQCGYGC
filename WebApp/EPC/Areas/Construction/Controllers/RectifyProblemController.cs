using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic;
using EPC.Logic.Domain;
using Workflow.Logic.Domain;
using Base.Logic.Domain;

namespace EPC.Areas.Construction.Controllers
{
    public class RectifyProblemController : EPCFormContorllor<S_C_RectifySheet>
    {
        public JsonResult DeleteProblems()
        {
            if (!String.IsNullOrEmpty(Request["ListIDs"]))
            {
                foreach (var item in Request["ListIDs"].Split(','))
                {
                    var ccr = this.EPCEntites.Set<S_C_RectifySheet_RectifyProblems>().FirstOrDefault(m => m.ID == item);
                    if (ccr != null)
                    {
                        if (!string.IsNullOrEmpty(ccr.S_C_RectifySheetID))
                            throw new Formula.Exceptions.BusinessValidationException("[" + ccr.Problems + "]已发起问题整改单，不能删除！");
                        this.EPCEntites.Set<S_C_RectifySheet_RectifyProblems>().Remove(ccr);
                    }
                }
                this.EPCEntites.SaveChanges();
            }
            return Json("");
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, S_UI_Form formInfo)
        {
            if (subTableName == "RectifyProblems")
            {
                if (detail.GetValue("OpenDate") == null || string.IsNullOrEmpty(detail.GetValue("OpenDate").ToString()))
                {
                    detail.SetValue("OpenDate", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, S_UI_Form formInfo)
        {
            if (subTableName == "RectifyProblems")
            {
                if (detailList.Count == 0)
                    throw new Formula.Exceptions.BusinessValidationException("请填写整改问题！");
            }
        }

        protected override void OnFlowEnd(S_C_RectifySheet entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            foreach (var item in entity.S_C_RectifySheet_RectifyProblems.ToList())
            {
                if (item.RectifyState == "Closed")
                {
                    item.CloseDate = System.DateTime.Now.Date;
                }
            }
            this.EPCEntites.SaveChanges();
        }
    }
}
