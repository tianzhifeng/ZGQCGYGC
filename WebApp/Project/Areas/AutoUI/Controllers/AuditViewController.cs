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

namespace Project.Areas.AutoUI.Controllers
{
    public class AuditViewController : ProjectFormContorllor<T_EXE_Audit>
    {
        //public override bool ValiateToken
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}

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
            var ArchiveType = System.Configuration.ConfigurationManager.AppSettings["ArchiveType"];
            ViewBag.ArchiveType = string.IsNullOrEmpty(ArchiveType) ? "PdfFile" : ArchiveType;
            return base.PageView();
        }

        public override JsonResult Delete()
        {
            if (!String.IsNullOrEmpty(Request["ListIDs"]))
                this.BeforeDelete(Request["ListIDs"].Split(','));
            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);
            this.BusinessEntities.SaveChanges();
            var ids = Request["ID"];
            var products = this.BusinessEntities.Set<S_E_Product>().Where(a => ids.Contains(a.AuditID)).ToList();
            foreach (var item in products)
            {
                item.AuditState = Project.Logic.AuditState.Create.ToString();
                item.AuditID = "";
                item.UpdateVersison();
            }
            this.BusinessEntities.SaveChanges();
            return Json("");
        }

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            var row = dt.Rows[0];
            var wbsID = GetQueryString("WBSID");
            if (!isNew)
            {
                wbsID = row["WBSID"].ToString();
                var id = row["ID"].ToString();
                ViewBag.AuditID = id;
                ViewBag.IsNew = false.ToString();
                ViewBag.FlowPhase = row["FlowPhase"].ToString();
            }
            else
            {
                ViewBag.AuditID = "";
                ViewBag.BatchID = this.GetQueryString("BatchID");
                ViewBag.IsNew = true.ToString();
                ViewBag.FlowPhase = row["FlowPhase"].ToString();
            }
            var wbs = this.GetEntityByID<S_W_WBS>(wbsID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + wbsID + "】的WBS对象");
            var phaseCode = wbs.PhaseCode;
            if (phaseCode.Split('&').Length == 3)
                phaseCode = phaseCode.Split('&')[2];
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
        }

        /// <summary>
        /// 重写撤销方法
        /// </summary>
        /// <returns></returns>
        public override JsonResult DeleteFlow()
        {
            var id = GetQueryString("ID");
            var entity = this.BusinessEntities.Set<T_EXE_Audit>().FirstOrDefault(a => a.ID == id);
            var products = this.BusinessEntities.Set<S_E_Product>().Where(a => id.Contains(a.AuditID)).ToList();
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
            var entity = this.BusinessEntities.Set<T_EXE_Audit>().FirstOrDefault(a => a.ID == id);
            var products = this.BusinessEntities.Set<S_E_Product>().Where(a => id.Contains(a.AuditID)).ToList();
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
            var entity = this.BusinessEntities.Set<T_EXE_Audit>().FirstOrDefault(a => a.ID == id);
            var products = this.BusinessEntities.Set<S_E_Product>().Where(a => id.Contains(a.AuditID)).ToList();
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
            var entity = this.BusinessEntities.Set<T_EXE_Audit>().FirstOrDefault(a => a.ID == id);
            var products = this.BusinessEntities.Set<S_E_Product>().Where(a => id.Contains(a.AuditID)).ToList();
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

        public JsonResult GetProducts(string AuditID)
        {
            var data = this.BusinessEntities.Set<S_E_Product>().Where(d => d.AuditID == AuditID).ToList();
            return Json(data);
        }

        public JsonResult GetVersionProducts(string AuditID)
        {
            string sql = @"select m.* from S_E_ProductVersion m
cross apply (select top 1 * from S_E_ProductVersion d 
where AuditID='{0}' and AuditState='Pass' 
and m.ProductID=d.ProductID order by d.Version desc) _d
where m.AuditID='{0}' and _d.ID=m.ID";
            sql = string.Format(sql, AuditID);
            var result = this.ProjectSQLDB.ExecuteDataTable(sql);
            return Json(result);
        }

        public JsonResult GetBatchProducts(string BatchID)
        {
            var data = this.BusinessEntities.Set<S_E_Product>().Where(d => d.BatchID == BatchID).ToList();
            return Json(data);
        }

        public JsonResult AddProducts(string Products, string ID)
        {
            var audit = this.GetEntityByID<T_EXE_Audit>(ID);
            if (audit != null)
            {
                var list = JsonHelper.ToList(Products);
                foreach (var item in list)
                {
                    var product = this.GetEntityByID<S_E_Product>(item.GetValue("ID"));
                    product.AuditState = AuditState.Design.ToString();
                    product.AuditID = audit.ID;
                    product.UpdateVersison();
                }
            }
            this.BusinessEntities.SaveChanges();
            return Json("");
        }

        public JsonResult RemoveProducts(string Products)
        {
            var list = JsonHelper.ToList(Products);
            foreach (var item in list)
            {
                var product = this.GetEntityByID<S_E_Product>(item.GetValue("ID"));
                if (product == null)
                {
                    continue;
                }
                product.BatchID = string.Empty;
                product.AuditID = string.Empty;
                product.AuditState = "Create";
            }
            this.BusinessEntities.SaveChanges();
            return Json("");
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
                if (product.S_E_ProductVersion.Count > 1)
                {
                    throw new Formula.Exceptions.BusinessException("进行过升版的成果不能进行删除");
                }
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
                    product.AuditID = dic.GetValue("ID");

                    product.PlotSealGroup = item.GetValue("PlotSealGroup");
                    product.PlotSealGroupName = item.GetValue("PlotSealGroupName");
                    product.PlotSealGroupKey = item.GetValue("PlotSealGroupKey");

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


            var entity = this.BusinessEntities.Set<T_EXE_Audit>().SingleOrDefault(c => c.ID == flow.FormInstanceID);
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
                    entity.SynchProductAuditState(string.IsNullOrEmpty(nextStep.Code) ? "Flow" : nextStep.Code);

                    //CAD消息随大事记发送 2019年4月4日
                    #region 发项目消息，用于发送cad通知

                    //var receiverIds = nextExecUserIDs;
                    //var receiverNames = nextExecUserNames;
                    //var title = /*currentStep.Name + routing.Name + */"校审通知";
                    //var content = "【" + entity.ProjectInfoName + "】";
                    //if (!string.IsNullOrEmpty(entity.SubProjectName))
                    //    content += "【" + entity.SubProjectName + "】";
                    //if (!string.IsNullOrEmpty(entity.PhaseCode))
                    //    content += "【" + FormulaHelper.GetService<IEnumService>().GetEnumText("Project.Phase", entity.PhaseCode) + "】";
                    //if (!string.IsNullOrEmpty(entity.MajorCode))
                    //    content += "【" + FormulaHelper.GetService<IEnumService>().GetEnumText("Project.Major", entity.MajorCode) + "】";
                    //if (routing.Code == "Pass")
                    //{
                    //    title = "校审流程通过通知";
                    //    content += "有成果通过校审流程";
                    //    receiverIds = entity.Designer;
                    //    receiverNames = entity.DesignerName;
                    //}
                    //else
                    //    content += "我的" + nextStep.Name + "：有成果需要" + nextStep.Name;
                    //FormulaHelper.CreateFO<ProjectInfoFO>().SendNotice(entity.ProjectInfoID, entity.WBSID, entity.MajorCode, "",
                    //    receiverIds, receiverNames, title, content, "", "", entity.ID, "T_EXE_Audit", ProjectNoticeType.Major.ToString(), "", "");
                    #endregion

                    #region 校审意见发消息
                    var sql = @"select * from T_EXE_Audit_AdviceDetail with(nolock) where T_EXE_AuditID='{0}' 
and ID not in (select RelateID from S_N_Notice where RelateType='T_EXE_Audit_AdviceDetail' and ProjectInfoID='{1}')";
                    sql = string.Format(sql, entity.ID, entity.ProjectInfoID);
                    var dt = this.ProjectSQLDB.ExecuteDataTable(sql);
                    foreach (DataRow row in dt.Rows)
                    {
                        var dic = FormulaHelper.DataRowToDic(row);
                        var _title = "批注通知";
                        var _content = "【" + entity.ProjectInfoName + "】";
                        if (!string.IsNullOrEmpty(entity.SubProjectName))
                            _content += "【" + entity.SubProjectName + "】";
                        if (!string.IsNullOrEmpty(entity.PhaseCode))
                            _content += "【" + FormulaHelper.GetService<IEnumService>().GetEnumText("Project.Phase", entity.PhaseCode) + "】";
                        if (!string.IsNullOrEmpty(entity.MajorCode))
                            _content += "【" + FormulaHelper.GetService<IEnumService>().GetEnumText("Project.Major", entity.MajorCode) + "】";
                        _content += "的成果【" + dic.GetValue("ProductCode") + "】由 " + dic.GetValue("CreateUserName") + " 增加了一条批注意见：" + dic.GetValue("MsitakeContent");
                        FormulaHelper.CreateFO<ProjectInfoFO>().SendNotice(entity.ProjectInfoID, entity.WBSID, entity.MajorCode, "",
                            entity.Designer, entity.DesignerName, _title, _content, "", "", entity.ID, "T_EXE_Audit_AdviceDetail", ProjectNoticeType.User.ToString(),
                            dic.GetValue("SubmitUser"), dic.GetValue("SubmitUserName"));
                    }
                    #endregion
                }
            }
            this.BusinessEntities.SaveChanges();
            #endregion
        }

        protected override void OnFlowEnd(T_EXE_Audit entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                var products = this.BusinessEntities.Set<S_E_Product>().Where(d => d.AuditID == entity.ID).ToList();
                foreach (var product in products)
                {
                    //product.Collactor = entity.Collactor;
                    //product.CollactorName = entity.CollactorName;
                    //product.Auditor = entity.Auditor;
                    //product.AuditorName = entity.AuditorName;
                    //product.Approver = entity.Approver;
                    //product.ApproverName = entity.ApproverName;
                    //更新成果的SignState
                    product.SignState = product.AuditState;
                    product.Save();
                }
                entity.Publish();
            }
            this.BusinessEntities.SaveChanges();
        }

        public JsonResult GetProductsForYDT(string FormInstanceId)
        {
            var audit = this.BusinessEntities.Set<T_EXE_Audit>().FirstOrDefault(a => a.ID == FormInstanceId);
            if (audit != null && audit.FlowPhase == "End")
            {
                string sql = @"select m.* from S_E_ProductVersion m
cross apply (select top 1 * from S_E_ProductVersion d 
where AuditID='{0}'  and AuditState='Pass' 
and m.ProductID=d.ProductID order by d.Version desc) _d
where m.AuditID='{0}' and _d.ID=m.ID";
                sql = string.Format(sql, FormInstanceId);
                var result = this.ProjectSQLDB.ExecuteDataTable(sql);
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var ssql = string.Format(@"select *,ID ProductID from S_E_Product where AuditID = '{0}'", FormInstanceId);
            var data = this.ProjectSQLDB.ExecuteDataTable(ssql);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SyncMyAdvice(string AuditStep, string Type, string Advice, string ProjectInfoID)
        {
            var project = this.BusinessEntities.Set<S_I_ProjectInfo>().FirstOrDefault(a => a.ID == ProjectInfoID);
            if (project == null) throw new Formula.Exceptions.BusinessException("找不到指定的项目");
            var user = FormulaHelper.GetUserInfo();
            var advice = this.BusinessEntities.Set<S_AE_AuditAdvice>().FirstOrDefault(a => a.Advice == Advice);
            if (advice == null)
            {
                advice = new S_AE_AuditAdvice();
                advice.ID = FormulaHelper.CreateGuid();
                advice.ProjectInfo = ProjectInfoID;
                advice.ProjectInfoName = project.Name;
                advice.BelongUser = user.UserID;
                advice.BelongUserName = user.UserName;
                advice.AuditStep = AuditStep;
                advice.Type = Type;
                advice.Advice = Advice;

                EntityCreateLogic<S_AE_AuditAdvice>(advice);
                this.BusinessEntities.Set<S_AE_AuditAdvice>().Add(advice);
                this.BusinessEntities.SaveChanges();
            }
            return Json("");
        }
    }
}