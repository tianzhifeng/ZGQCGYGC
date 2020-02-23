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
using Newtonsoft.Json;
using Formula.ImportExport;

namespace EPC.Areas.Finance.Controllers
{
    public class ProjectReimbursementController : EPCFormContorllor<S_F_ProjectReimbursement>
    {
        protected override void OnFlowEnd(S_F_ProjectReimbursement entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();                              
                this.EPCEntites.SaveChanges();
                if (S_T_DefineParams.Params.GetValue("ProjReimbursementAutoSettle").ToLower() == "true")
                {
                    S_I_Engineering engineering = this.EPCEntites.Set<S_I_Engineering>().Find(entity.Project);
                    engineering.SumCBSCost();
                }  
            }
        }
    }
}
