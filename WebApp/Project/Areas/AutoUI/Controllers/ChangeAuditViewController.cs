using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using Base.Logic.BusinessFacade;
using System.Data;
using Config.Logic;
using Workflow.Logic.Domain;
using Workflow.Logic;
using System.Text;
using DocSystem.Logic.Domain;

namespace Project.Areas.AutoUI.Controllers
{
    public class ChangeAuditViewController : ProjectFormContorllor<T_EXE_ChangeAudit>
    {
        public override ActionResult PageView()
        {
            string userName = this.GetQueryString("SystemName");
            if (!string.IsNullOrEmpty(userName))
            {
                userName = HttpUtility.UrlDecode(userName);
                string pwd = this.GetQueryString("PWD");
                string sql = "select count(0) from S_A_User where Code ='" + userName + "'";
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                if (Convert.ToInt32(db.ExecuteScalar(sql)) > 0 && !String.IsNullOrEmpty(userName))
                {
                    FormulaHelper.ContextSet("AgentUserLoginName", userName);
                    FormulaHelper.SetAuthCookie(userName);
                }
            }
            return base.PageView();
        }
        
        /// <summary>
        /// 重写撤销方法
        /// </summary>
        /// <returns></returns>
        public override JsonResult DeleteFlow()
        {
            var id = GetQueryString("ID");
            var entity = this.BusinessEntities.Set<T_EXE_ChangeAudit>().FirstOrDefault(a => a.ID == id);
            var products = this.BusinessEntities.Set<S_E_Product>().Where(a => id.Contains(a.ChangeAuditID)).ToList();
            var taskExecID = Request["TaskExecID"];
            if (string.IsNullOrEmpty(taskExecID))
            {
                var insFlow = entities.Set<S_WF_InsFlow>().FirstOrDefault(a => a.FormInstanceID == id);
                var firstStep = insFlow.S_WF_InsDefFlow.S_WF_InsDefStep.FirstOrDefault(a => a.Type == StepTaskType.Inital.ToString());
                if (firstStep != null)
                {
                    entity.StepName = firstStep.Name;
                    foreach (var item in products)
                    {
                        item.AuditState = firstStep.Code;
                        item.UpdateVersison();
                    }
                }
            }
            else
            {
                var taskExec = entities.Set<S_WF_InsTaskExec>().Where(c => c.ID == taskExecID).SingleOrDefault();
                var task = taskExec.S_WF_InsTask;
                var insDefStep = task.S_WF_InsDefStep;
                foreach (var item in products)
                {
                    item.AuditState = insDefStep.Code;
                    item.UpdateVersison();
                }
            }
            var result = base.DeleteFlow();
            this.BusinessEntities.SaveChanges();
            return result;
        }

        public override JsonResult DoBack(string taskExecID, string routingID, string execComment)
        {
            var result = base.DoBack(taskExecID, routingID, execComment);
            var insDefRouting = entities.Set<S_WF_InsDefRouting>().FirstOrDefault(a => a.ID == routingID);
            var insDefStep = insDefRouting.S_WF_InsDefStep;
            var id = GetQueryString("ID");
            var entity = this.BusinessEntities.Set<T_EXE_ChangeAudit>().FirstOrDefault(a => a.ID == id);
            var products = this.BusinessEntities.Set<S_E_Product>().Where(a => id.Contains(a.ChangeAuditID)).ToList();
            foreach (var item in products)
            {
                item.AuditState = insDefStep.Code;
                item.UpdateVersison();
            }
            entity.StepName = insDefStep.Name;
            this.BusinessEntities.SaveChanges();
            return result;
        }

        public override JsonResult DoBackFirst(string taskExecId, string execComment)
        {
            var result = base.DoBackFirst(taskExecId, execComment);
            var id = GetQueryString("ID");
            var entity = this.BusinessEntities.Set<T_EXE_ChangeAudit>().FirstOrDefault(a => a.ID == id);
            var products = this.BusinessEntities.Set<S_E_Product>().Where(a => id.Contains(a.ChangeAuditID)).ToList();
            var insFlow = entities.Set<S_WF_InsFlow>().FirstOrDefault(a => a.FormInstanceID == id);
            var firstStep = insFlow.S_WF_InsDefFlow.S_WF_InsDefStep.FirstOrDefault(a => a.Type == StepTaskType.Inital.ToString());
            if (firstStep != null)
            {
                entity.StepName = firstStep.Name;
                foreach (var item in products)
                {
                    item.AuditState = firstStep.Code;
                    item.UpdateVersison();
                }
            }
            this.BusinessEntities.SaveChanges();
            return result;
        }

        public override JsonResult DoBackFirstReturn(string taskExecId, string execComment)
        {
            var result = base.DoBackFirstReturn(taskExecId, execComment);
            var id = GetQueryString("ID");
            var entity = this.BusinessEntities.Set<T_EXE_ChangeAudit>().FirstOrDefault(a => a.ID == id);
            var products = this.BusinessEntities.Set<S_E_Product>().Where(a => id.Contains(a.ChangeAuditID)).ToList();
            var taskExec = entities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecId);
            var task = taskExec.S_WF_InsTask;
            var flow = taskExec.S_WF_InsFlow;
            var sendTask = entities.Set<S_WF_InsTask>().SingleOrDefault(c => c.ID == taskExec.S_WF_InsTask.SendTaskIDs);
            var sendStep = sendTask.S_WF_InsDefStep;
            entity.StepName = sendStep.Name;
            foreach (var item in products)
            {
                item.AuditState = sendStep.Code;
                item.UpdateVersison();
            }
            this.BusinessEntities.SaveChanges();
            return result;
        }

        public override JsonResult Delete()
        {
            List<T_EXE_ChangeAudit> list = new List<T_EXE_ChangeAudit>();
            if (!string.IsNullOrEmpty(Request["ListIDs"]))
            {
                var Ids = Request["ListIDs"].Split(',');
                list.AddRange(this.BusinessEntities.Set<T_EXE_ChangeAudit>().Where(a => Ids.Contains(a.ID)).ToList());
            }
            if (!string.IsNullOrEmpty(Request["ID"]))
            {
                var id = Request["ID"];
                list.Add(this.BusinessEntities.Set<T_EXE_ChangeAudit>().FirstOrDefault(a => a.ID == id));
            }
            if (list.Count > 0)
            {
                var wbsid = list.FirstOrDefault().WBSID;
                var task = this.BusinessEntities.Set<S_W_TaskWork>().FirstOrDefault(a => a.WBSID == wbsid);
                if (task != null)
                {
                    task.ChangeState = TaskWorkChangeState.ApplyFinish.ToString();
                }
            }

            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);
            this.BusinessEntities.SaveChanges();
            var ids = list.Select(a => a.ID).ToList();
            var products = this.BusinessEntities.Set<S_E_Product>().Where(a => ids.Contains(a.ChangeAuditID)).ToList();
            foreach (var item in products)
            {
                item.AuditState = Project.Logic.AuditState.Create.ToString();
                item.ChangeAuditID = "";
                item.UpdateVersison();
            }
            this.BusinessEntities.SaveChanges();
            return Json("");
        }

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            var row = dt.Rows[0];
            var wbsID = GetQueryString("WBSID");
            ViewBag.IsFinish = false.ToString();
            if (!isNew)
            {
                wbsID = row["WBSID"].ToString();
                var id = row["ID"].ToString();
                ViewBag.ChangeAuditID = id;
                ViewBag.IsNew = false.ToString();
                if (row["FlowPhase"].ToString() == "End")
                    ViewBag.IsFinish = true.ToString();
            }
            else
            {
                ViewBag.ChangeAuditID = "";
                ViewBag.IsNew = true.ToString();
            }
            var wbs = this.GetEntityByID<S_W_WBS>(wbsID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + wbsID + "】的WBS对象");
            var phaseCode = wbs.PhaseCode;
            if (String.IsNullOrEmpty(phaseCode))
            {
                phaseCode = wbs.S_I_ProjectInfo.PhaseValue;
            }
            var PhaseRows = EnumBaseHelper.GetEnumTable("Project.Phase").Select(" value in ('" + phaseCode.Replace(",", "','") + "')");
            var phaseEnum = new List<Dictionary<string, object>>();
            foreach (DataRow item in PhaseRows)
            {
                phaseEnum.Add(FormulaHelper.DataRowToDic(item));
            }
            ViewBag.Phase = JsonHelper.ToJson(phaseEnum);
            ViewBag.WBSID = wbsID;
        }

        public JsonResult GetProducts(string ChangeAuditID)
        {
            var data = this.BusinessEntities.Set<S_E_Product>().Where(d => d.ChangeAuditID == ChangeAuditID).ToList();
            return Json(data);
        }
        public JsonResult GetVersionProducts(string ChangeAuditID)
        {
            string sql = @"select m.* from S_E_ProductVersion m
cross apply (select top 1 * from S_E_ProductVersion d 
where ChangeAuditID='{0}' and AuditState='Pass' 
and m.ProductID=d.ProductID order by d.Version desc) _d
where m.ChangeAuditID='{0}' and _d.ID=m.ID";
            sql = string.Format(sql, ChangeAuditID);
            var result = this.ProjectSQLDB.ExecuteDataTable(sql);
            return Json(result);
        }

        public JsonResult GetBatchProducts(string WBSID)
        {
            var passState = AuditState.Pass.ToString();
            var invalidState = ProductState.InInvalid.ToString();
            var data = this.BusinessEntities.Set<S_E_Product>().Where(a => a.WBSID == WBSID &&
                (a.AuditState != passState || a.State == invalidState)).ToList();
            var removeList = new List<S_E_Product>();
            var result = new List<S_E_Product>();
            foreach (var item in data)
            {
                if (string.IsNullOrEmpty(item.ParentID))
                {
                    result.Add(item);
                    continue;
                }
                //变更单 需要显示主图当前版本号的子图
                //去除与主图版本号不匹配的子图
                var parent = data.FirstOrDefault(a => a.ID == item.ParentID);
                if (parent != null && parent.Version == item.ParentVersion)
                    result.Add(item);
                else
                    removeList.Add(item);
            }
            if (removeList.Count > 0)
            {
                //流程结束后，会重置成果变更状态，所以此处删除变更单ID
                removeList.Update(a => a.ChangeAuditID = null);
                this.BusinessEntities.SaveChanges();
            }
            return Json(result);
        }
        
        public JsonResult DeleteProducts(string Products)
        {
            var list = JsonHelper.ToList(Products);
            foreach (var item in list)
            {
                var product = this.GetEntityByID<S_E_Product>(item.GetValue("ID"));
                if (product == null)
                {
                    continue;
                }
                if (product.AuditState != AuditState.Create.ToString()
                    && product.AuditState != AuditState.Design.ToString()
                    && product.AuditState != AuditState.Designer.ToString())
                {
                    throw new Formula.Exceptions.BusinessException("已经发起校审的成果不能删除。");
                }
                else if (product.State == ProductState.Change.ToString())
                    throw new Formula.Exceptions.BusinessException("已经变更的成果不能删除。");
                else if (product.State == ProductState.Invalid.ToString() || product.State == ProductState.InInvalid.ToString())
                    throw new Formula.Exceptions.BusinessException("已经作废的成果不能删除。");
                this.BusinessEntities.Set<S_E_Product>().Remove(product);
            }
            this.BusinessEntities.SaveChanges();
            return Json("");
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                var flowCode = this.Request["FlowCode"];
                var sql = string.Format(@"select Code from S_WF_InsDefStep where InsDefFlowID = (
select top 1 ID from S_WF_InsDefFlow where Code = '{0}' order by ModifyTime desc
)", flowCode);
                var workFlowSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
                var stepList = workFlowSqlHelper.ExecuteList<S_WF_InsDefStep>(sql);
                var auditStateArray = Enum.GetNames(typeof(Project.Logic.AuditState));
                var str = "";
                for (int i = 0; i < auditStateArray.Length; i++)
                {
                    if ((i + 1) % 5 != 0)
                        str += auditStateArray[i] + "、";
                    else
                        str += "<br/>" + auditStateArray[i] + "、";
                }
                foreach (var step in stepList)
                    if (!string.IsNullOrEmpty(step.Code) && !auditStateArray.Contains(step.Code))
                        throw new Formula.Exceptions.BusinessException("校审环节【" + step.Code + "】不符合要求，流程未能启动<br/>"
                            + "环节编号只能是：" + str.TrimEnd('、'));
            }
            var products = JsonHelper.ToList(this.Request["Products"]);
            foreach (var item in products)
            {
                var product = this.GetEntityByID<S_E_Product>(item.GetValue("ID"));
                if (product != null)
                {
                    product.ChangeAuditID = dic.GetValue("ID");
                    product.UpdateVersison();
                }
            }
        }

        public override bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            var auditStateArray = Enum.GetNames(typeof(Project.Logic.AuditState));
            var str = "";
            for (int i = 0; i < auditStateArray.Length; i++)
            {
                if ((i + 1) % 5 != 0)
                    str += auditStateArray[i] + "、";
                else
                    str += "<br/>" + auditStateArray[i] + "、";
            }
            var workFlowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
            var nextStep = workFlowEntities.Set<S_WF_InsDefStep>().SingleOrDefault(c => c.ID == routing.EndID);
            if (!string.IsNullOrEmpty(nextStep.Code) && !auditStateArray.Contains(nextStep.Code))
                throw new Formula.Exceptions.BusinessException("校审环节【" + nextStep.Code + "】不符合要求，流程未能启动<br/>"
                    + "环节编号只能是：" + str.TrimEnd('、'));
            return base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
        }

        protected override void AfterExeTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing, 
            string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
              var workFlowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
            #region 同步Activity

            var currentTask = taskExec.S_WF_InsTask;
            var flow = taskExec.S_WF_InsFlow;
            if (flow == null) throw new Formula.Exceptions.BusinessException("");
            var currentStep = currentTask.S_WF_InsDefStep;
            var nextStep = workFlowEntities.Set<S_WF_InsDefStep>().SingleOrDefault(c => c.ID == routing.EndID);
            var nextTask = nextStep.S_WF_InsTask.OrderByDescending(c => c.CreateTime).FirstOrDefault();  //取最新的任务

            var entity = this.BusinessEntities.Set<T_EXE_ChangeAudit>().SingleOrDefault(c => c.ID == flow.FormInstanceID);
            if (entity != null)
            {
                var key = currentStep.Code;
                var name = "(" + entity.SerialNumber + ")" + currentStep.Name;
                var displayName = entity.PhaseCode + "-" + entity.MajorCode + "-" + currentStep.Name;
                var activity = entity.CreateAcitivity(key, name, displayName);
                if (taskExec.CreateTime.HasValue)
                    activity.CreateDate = taskExec.CreateTime.Value;
                if (currentTask.Status == FlowTaskStatus.Complete.ToString())//解决协作完成时，只要有一个用户通过就更新成果状态的bug
                {
                    //校审环节编号为空时 默认给Flow
                    entity.SynchProductChangeAuditState(string.IsNullOrEmpty(nextStep.Code) ? "Flow" : nextStep.Code);
                    if (!string.IsNullOrEmpty(entity.WBSID))
                    {
                        var task = this.BusinessEntities.Set<S_W_TaskWork>().FirstOrDefault(a => a.WBSID == entity.WBSID);
                        if (task != null)
                        {
                            if (nextStep.Code == AuditState.Design.ToString()
                                || nextStep.Code == AuditState.Designer.ToString())
                                task.ChangeState = TaskWorkChangeState.AuditStart.ToString();
                            else
                                task.ChangeState = TaskWorkChangeState.AuditApprove.ToString();
                        }
                    }
                }
            }
            this.BusinessEntities.SaveChanges();
            #endregion
        }

        protected override void OnFlowEnd(T_EXE_ChangeAudit entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                var products = this.BusinessEntities.Set<S_E_Product>().Where(d => d.ChangeAuditID == entity.ID).ToList();
                foreach (var product in products)
                {
                    //product.Collactor = entity.Collactor;
                    //product.CollactorName = entity.CollactorName;
                    //product.Auditor = entity.Auditor;
                    //product.AuditorName = entity.AuditorName;
                    //product.Approver = entity.Approver;
                    //product.ApproverName = entity.ApproverName;
                    //变更完更新状态
                    if (product.State == ProductState.Change.ToString())
                        product.State = ProductState.Create.ToString();
                    product.Save();
                    if (product.State == ProductState.InInvalid.ToString())
                    {
                        product.State = ProductState.Invalid.ToString();
                        //删除Document
                        var document = BusinessEntities.Set<S_D_Document>().FirstOrDefault(a => a.RelateID == product.ID && a.RelateTable == "S_E_Product");
                        if (document != null && document.State != "Archive")
                        {
                            //BusinessEntities.Set<S_D_Document>().Remove(document);
                            document.State = ProductState.Invalid.ToString();
                        }
                        //BusinessEntities.Set<S_E_Product>().Remove(product);
                    }
                }
                entity.Publish();
            }
            this.BusinessEntities.SaveChanges();
        }
    }
}