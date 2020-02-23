using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Text;
using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using OfficeAuto.Logic;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class ReimbursementApplyController : OfficeAutoFormContorllor<T_F_ReimbursementApply>
    {
        protected override void OnFlowEnd(T_F_ReimbursementApply entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null) {
                entity.Push();
            }
            this.BusinessEntities.SaveChanges();
        }
    }
}
