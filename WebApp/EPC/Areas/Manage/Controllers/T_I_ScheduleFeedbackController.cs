using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;

namespace EPC.Areas.Manage.Controllers
{
    public class TScheduleFeedbackController : EPCFormContorllor<T_I_ScheduleFeedback>
    {
        protected override void OnFlowEnd(T_I_ScheduleFeedback entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
                this.EPCEntites.SaveChanges();
            }
        }
    }
}
