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

namespace EPC.Areas.Design.Controllers
{
    public class AuditController : EPCFormContorllor<S_E_AuditForm>
    {
        public override ActionResult PageView()
        {
            string userName = this.GetQueryString("SystemName");
            if (!string.IsNullOrEmpty(userName))
            {
                string pwd = this.GetQueryString("PWD");
                string sql = "select count(0) from S_A_User where Code ='" + userName + "'";
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                if (Convert.ToInt32(db.ExecuteScalar(sql)) > 0 && !String.IsNullOrEmpty(userName))
                {
                    FormulaHelper.ContextSet("AgentUserLoginName", userName);
                    System.Web.Security.FormsAuthentication.SetAuthCookie(userName, false);
                }
            }
            return base.PageView();
        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                ViewBag.AuditID = "";
                ViewBag.BatchID = this.GetQueryString("BatchID");
                ViewBag.IsNew = true.ToString();
            }
            else
            {
                var id = dic.GetValue("ID");
                ViewBag.AuditID = id;
                ViewBag.IsNew = false.ToString();
            }
        }

        public JsonResult GetProducts(string AuditID)
        {
            var data = this.EPCEntites.Set<S_E_DrawingResult>().Where(d => d.AuditID == AuditID).ToList();
            return Json(data);
        }

        public JsonResult GetBatchProducts(string BatchID)
        {
            var data = this.EPCEntites.Set<S_E_DrawingResult>().Where(d => d.BatchID == BatchID).ToList();
            return Json(data);
        }

        public override JsonResult Delete()
        {
            if (!String.IsNullOrEmpty(Request["ListIDs"]))
                this.BeforeDelete(Request["ListIDs"].Split(','));
            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);
            this.EPCEntites.SaveChanges();
            var ids = Request["ID"];
            this.EPCEntites.Set<S_E_DrawingResult>().Where(c => ids.Contains(c.AuditID)).Update(
                c => { c.AuditID = ""; c.AuditState = "Create"; });
            this.EPCEntites.SaveChanges();
            return Json("");
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var products = JsonHelper.ToList(this.Request["Products"]);
            foreach (var item in products)
            {
                var product = this.GetEntityByID<S_E_DrawingResult>(item.GetValue("ID"));
                if (product != null)
                    product.AuditID = dic.GetValue("ID");
            }
        }

        public override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing,
             string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            var workFlowEntities = FormulaHelper.GetEntities<WorkflowEntities>();

            #region 同步工序步骤
            var currentTask = taskExec.S_WF_InsTask;
            var flow = taskExec.S_WF_InsFlow;
            if (flow == null) throw new Formula.Exceptions.BusinessValidationException("");
            var currentStep = currentTask.S_WF_InsDefStep;
            var nextStep = workFlowEntities.Set<S_WF_InsDefStep>().SingleOrDefault(c => c.ID == routing.EndID);
            var nextTask = nextStep.S_WF_InsTask.OrderByDescending(c => c.CreateTime).FirstOrDefault();  //取最新的任务

            var entity = this.EPCEntites.Set<S_E_AuditForm>().SingleOrDefault(c => c.ID == flow.FormInstanceID);
            if (entity != null)
            {
                var key = currentStep.Code;
                var name = "(" + entity.SerialNumber + ")" + currentStep.Name;
                var displayName = entity.PhaseCode + "-" + entity.MajorCode + "-" + currentStep.Name;

                var taskProc = entity.CreateTaskProc(key, name, displayName);
                if (taskProc != null && taskExec.CreateTime.HasValue)
                    taskProc.FactEndDate = taskExec.CreateTime.Value;
                entity.SynchProductAuditState(nextStep.Code);
            }
            this.EPCEntites.SaveChanges();
            bool isFlowComplete = base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
            #endregion
            return isFlowComplete;
        }

        protected override void OnFlowEnd(S_E_AuditForm entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                entity.Push();
            }
            this.EPCEntites.SaveChanges();
        }

    }
}
