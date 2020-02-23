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
    public class TScheduleFeedbackWithWBSController : EPCFormContorllor<T_I_ScheduleFeedback>
    {
        protected override void OnFlowEnd(T_I_ScheduleFeedback entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                foreach (var item in entity.T_I_ScheduleFeedback_TaskDetail.ToList())
                {
                    var task = this.EPCEntites.Set<S_I_WBS_Task>().FirstOrDefault(c => c.ID == item.TaskID);
                    if (task == null) continue;
                    var versionTaskList = this.EPCEntites.Set<S_I_WBS_Version_Task>().Where(c => c.TaskID == task.ID).ToList();
                    foreach (var vTask in versionTaskList)
                    {
                        vTask.FactStartDate = item.FactStartDate;
                        vTask.FactEndDate = item.FactEndDate;
                        vTask.Evidence = item.Attachment;
                        vTask.EvidenceName = item.AttachmentName;
                        vTask.Progress = item.Progress.HasValue ? Convert.ToDecimal(item.Progress.Value) : 0;
                    }
                    task.FactEndDate = item.FactEndDate;
                    task.FactStartDate = item.FactStartDate;
                    task.Progress = item.Progress.HasValue ? Convert.ToDecimal(item.Progress.Value) : 0;
                }
                this.EPCEntites.SaveChanges();
            }
        }
    }
}
