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
    public class SupplierQualificationController : EPCFormContorllor<T_I_SupplierQualification>
    {
        public override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec,
            Workflow.Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames,
            string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {

            var result = base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
            var entity = this.GetEntityByID(taskExec.S_WF_InsFlow.FormInstanceID);
            if (entity == null) { throw new Formula.Exceptions.BusinessValidationException("未能找到指定的表单数据，无法执行任务"); }

            var supplier = this.GetEntityByID<S_I_SupplierInfo>(entity.SupplierInfo);
            if (supplier != null)
            {
                supplier.FlowPhase = entity.FlowPhase;
            }
            return result;
        }

        protected override void OnFlowEnd(T_I_SupplierQualification entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                var supplier = this.GetEntityByID<S_I_SupplierInfo>(entity.SupplierInfo);
                if (supplier == null) return;
                if (entity.Result == true.ToString())
                {
                    supplier.State = SupplierState.Qualification.ToString();
                }
                else
                {
                    supplier.State = SupplierState.DisQualification.ToString();
                }
                supplier.LastChangeStateDate = DateTime.Now;
            }
        }
    }
}
