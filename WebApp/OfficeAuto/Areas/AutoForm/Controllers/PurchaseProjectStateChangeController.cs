using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    /// <summary>
    /// 采购项目管理-项目状态变更
    /// </summary>
    public class PurchaseProjectStateChangeController : OfficeAutoFormContorllor<T_PurchaseProject_StateChange>
    {
        protected override void OnFlowEnd(T_PurchaseProject_StateChange entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                //流程结束把变更状态回写到项目立项表中
                var projectModel = BusinessEntities.Set<T_PurchaseProject_ApprovalApply_ProjectList>().FirstOrDefault(x => x.T_PurchaseProject_ApprovalApplyID == entity.Project && x.ProjectName == entity.ProjectName);
                projectModel.ProjectStatus = entity.ChangedState;

                BusinessEntities.SaveChanges();
            }

            base.OnFlowEnd(entity, taskExec, routing);

        }



    }
}
