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
using Newtonsoft.Json;
using Formula.ImportExport;

namespace EPC.Areas.Bidding.Controllers
{
    public class BidSchemaController : EPCFormContorllor<T_M_BidSchema>
    {
        protected override void OnFlowEnd(T_M_BidSchema entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push(true);
                this.EPCEntites.SaveChanges();
            }
        }
        
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var dbContext = FormulaHelper.GetEntities<EPCEntities>();
            string engineeringInfoID = dic.GetValue("EngineeringInfoID");
            if (isNew)
            {
                if (dbContext.Set<T_M_BidSchema>().Count(c => c.FlowPhase != "End" && c.EngineeringInfoID == engineeringInfoID) > 0)
                {
                    throw new Formula.Exceptions.BusinessValidationException("尚有投标策划表未审批完成，无法进行升版操作");
                }
            }
            S_I_Engineering enginnering = dbContext.Set<S_I_Engineering>().Find(engineeringInfoID);
            if (enginnering.State != ProjectState.Bid.ToString())
            { throw new Formula.Exceptions.BusinessValidationException("已经完成立项的项目不能再进行投标策划表调整"); }
        }
    }
}
