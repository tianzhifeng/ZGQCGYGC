using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using System.Text.RegularExpressions;

using Formula.Helper;
using Formula;
using Config;
using Workflow.Logic.Domain;
using Workflow.Logic;
using Base.Logic.Domain;
using Config.Logic;
using Project.Logic.Domain;


namespace Project
{
    public class ProjectFormContorllor<T> : BaseAutoFormController where T : class, new()
    {
        SQLHelper sqlDB;
        public virtual SQLHelper ProjectSQLDB
        {
            get
            {
                if (sqlDB == null)
                    sqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Project);
                return sqlDB;
            }
        }

        public override DbContext BusinessEntities
        {
            get
            {
                return FormulaHelper.GetEntities<ProjectEntities>();
            }
        }

        #region 扩展虚方法

        protected virtual void OnFlowEnd(T entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        { }

        protected virtual void AfterWithDrawingTask(T entity, S_WF_InsTaskExec taskExec)
        { }

        protected virtual void AfterExeTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing,
            string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup,
            string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
        }

        protected virtual void SendStartNotice(S_UI_Form formInfo, T entity, string allTaskUserIDs, string allTaskUserNames, string stepName)
        {
            var dic = FormulaHelper.ModelToDic<T>(entity);
            var projectInfoID = dic.GetValue("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null)
                return;
            var isoDefine = projectInfo.ProjectMode.S_T_ISODefine.FirstOrDefault(d => d.FormCode == formInfo.Code);
            if (isoDefine == null) return;
            SendNotice(isoDefine, dic, projectInfo, "Start", allTaskUserIDs, allTaskUserNames, stepName);

        }

        protected virtual void SendEndNotice(S_UI_Form formInfo, T entity, string allTaskUserIDs, string allTaskUserNames, string stepName)
        {
            var dic = FormulaHelper.ModelToDic<T>(entity);
            var projectInfoID = dic.GetValue("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null)
                return;
            var isoDefine = projectInfo.ProjectMode.S_T_ISODefine.FirstOrDefault(d => d.FormCode == formInfo.Code);
            if (isoDefine == null) return;
            SendNotice(isoDefine, dic, projectInfo, "End", allTaskUserIDs, allTaskUserNames, stepName);
        }

        protected virtual void SendStepNotice(S_UI_Form formInfo, T entity, string allTaskUserIDs, string allTaskUserNames, string stepName)
        {
            var dic = FormulaHelper.ModelToDic<T>(entity);
            var projectInfoID = dic.GetValue("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null)
                return;
            var isoDefine = projectInfo.ProjectMode.S_T_ISODefine.FirstOrDefault(d => d.FormCode == formInfo.Code);
            if (isoDefine == null) return;
            SendNotice(isoDefine, dic, projectInfo, "Step", allTaskUserIDs, allTaskUserNames, stepName, true);
        }

        #endregion

        public override ActionResult PageView()
        {
            return base.PageView();
        }

        public override bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing,
            string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup,
            string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            string tmplCode = Request["TmplCode"];
            var formInfo = baseEntities.Set<S_UI_Form>().Where(c => c.Code == tmplCode).OrderByDescending(c => c.ID).FirstOrDefault(); //获取最新一个版本即可
            var entity = this.GetEntityByID<T>(taskExec.S_WF_InsFlow.FormInstanceID);
            bool flag = false;
            Workflow.Logic.BusinessFacade.FlowFO flowFO = new Workflow.Logic.BusinessFacade.FlowFO();
            flag = flowFO.ExecTask(taskExec.ID, routing.ID, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment, Request["RoutingID"]);
            //流程表单定义的流程逻辑
            ExecFlowLogic(routing.Code, taskExec.S_WF_InsFlow.FormInstanceID, "");
            this.AfterExeTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
            SetFormFlowInfo(taskExec.S_WF_InsFlow);

            var nextStep = this.entities.Set<S_WF_InsDefStep>().SingleOrDefault(c => c.ID == routing.EndID);
            var allTaskUserIDs = taskExec.S_WF_InsFlow.S_WF_InsTaskExec.Select(a => a.TaskUserID).Distinct().ToList();
            var allTaskUserNames = taskExec.S_WF_InsFlow.S_WF_InsTaskExec.Select(a => a.TaskUserName).Distinct().ToList();
            allTaskUserIDs.AddRange(nextExecUserIDs.Split(','));
            allTaskUserNames.AddRange(nextExecUserNames.Split(','));
            if (flag)
            {
                if (entity == null) throw new Formula.Exceptions.BusinessException("实体ID为空，未能找到相关实体对象，无法执行逻辑");
                OnFlowEnd(entity, taskExec, routing);
                this.SendEndNotice(formInfo, entity, string.Join(",", allTaskUserIDs.Distinct()), string.Join(",", allTaskUserNames.Distinct()), nextStep.Name);
            }
            else if (taskExec.S_WF_InsTask.Type == StepTaskType.Inital.ToString())
            {
                if (formInfo == null) throw new Formula.Exceptions.BusinessException("编号为【" + tmplCode + "】的表单定义不存在");
                if (entity == null) throw new Formula.Exceptions.BusinessException("实体ID为空，未能找到相关实体对象，无法执行逻辑");
                this.SendStartNotice(formInfo, entity, string.Join(",", allTaskUserIDs.Distinct()), string.Join(",", allTaskUserNames.Distinct()), nextStep.Name);
            }
            else if (taskExec.S_WF_InsTask.Type == StepTaskType.Normal.ToString())
            {
                if (formInfo == null) throw new Formula.Exceptions.BusinessException("编号为【" + tmplCode + "】的表单定义不存在");
                if (entity == null) throw new Formula.Exceptions.BusinessException("实体ID为空，未能找到相关实体对象，无法执行逻辑");
                this.SendStepNotice(formInfo, entity, string.Join(",", allTaskUserIDs.Distinct()), string.Join(",", allTaskUserNames.Distinct()), nextStep.Name);
            }
            this.BusinessEntities.SaveChanges();
            //执行路由的流程逻辑（部分逻辑执行时需要前后上下文关系数据，故需要放在基类的流程逻辑中执行）
            if (routing != null)
            {
                routing.ExeLogic();
                this.entities.SaveChanges();
            }
            return flag;
        }

        public virtual T GetEntityByID(string ID, bool fromDB = false)
        {
            return GetEntityByID<T>(ID, fromDB);
        }

        public override JsonResult Save()
        {
            return this.SaveBA(Request["TmplCode"]);
        }

        public override JsonResult SaveBA(string tmplCode)
        {
            return base.SaveBA(tmplCode);
        }

        public override JsonResult Delete()
        {
            return base.Delete();
        }

        public override JsonResult WithdrawAskTask()
        {
            flowService.WithdrawAskTask(Request["TaskExecID"]);
            var taskExec = this.entities.Set<S_WF_InsTaskExec>().Find(Request["TaskExecID"]);
            if (taskExec != null)
            {
                var entity = this.GetEntityByID<T>(taskExec.S_WF_InsFlow.FormInstanceID);
                if (entity == null) throw new Formula.Exceptions.BusinessException("实体ID为空，未能找到相关实体对象，无法执行逻辑");
                this.AfterWithDrawingTask(entity, taskExec);
            }
            return Json("");
        }

        void SendNotice(S_T_ISODefine isoDefine, Dictionary<string, object> dic, S_I_ProjectInfo projectInfo, string sendType, string allTaskUserIDs, string allTaskUserNames, string stepName, bool isStep = false)
        {
            var regText = sendType == "Start" ? isoDefine.StartNoticeContent : isoDefine.EndNoticeContent;
            if (isStep)
                regText = isoDefine.StepNoticeContent;
            if (isoDefine.SendNotice != "True" || String.IsNullOrEmpty(regText))
            {
                return;
            }
            var majorCode = string.Empty;
            if (!string.IsNullOrEmpty(isoDefine.MajorField))
            {
                majorCode = dic.GetValue(isoDefine.MajorField);
            }
            var formID = dic.GetValue("ID");
            var notice = this.BusinessEntities.Set<S_N_Notice>().FirstOrDefault(d => d.RelateType == sendType && d.RelateID == formID);
            if (notice == null || isStep)
            {
                notice = this.BusinessEntities.Set<S_N_Notice>().Create();
                notice.ID = FormulaHelper.CreateGuid();
                var enumDefine = new List<Dictionary<string, object>>();
                if (!String.IsNullOrEmpty(isoDefine.EnumFieldInfo))
                    enumDefine = JsonHelper.ToList(isoDefine.EnumFieldInfo);
                dic.SetValue("StepName", stepName);
                var content = replaceNameString(regText, dic, enumDefine);
                notice.Title = content;
                notice.Content = content;
                if (!String.IsNullOrEmpty(isoDefine.LinkViewUrl))
                {
                    notice.LinkUrl = isoDefine.LinkViewUrl.IndexOf("?") >= 0 ?
                     isoDefine.LinkViewUrl + "&ID=" + formID :
                          isoDefine.LinkViewUrl + "?ID=" + formID;
                }
                notice.CreateDate = DateTime.Now;
                notice.RelateID = formID;
                notice.RelateType = sendType;
                notice.CreateUserID = dic.GetValue("CreateUserID");
                notice.CreateUserName = dic.GetValue("CreateUser");
                notice.GroupInfoID = projectInfo.GroupRootID;
                notice.IsFromSys = "True";
                notice.ProjectInfoID = projectInfo.ID;
                notice.MajorValue = majorCode;
                notice.NoticeType = isoDefine.Level ?? "Project";
                if (isoDefine.Level == "Flow")
                {
                    notice.ReceiverIDs = allTaskUserIDs;
                    notice.ReceiverNames = allTaskUserNames;
                }
                else if (isoDefine.Level == "Major")
                {
                    notice.WBSID = dic.GetValue("WBSID");
                }
                if (isoDefine.ExpiresDate != null)
                {
                    notice.ExpiresTime = DateTime.Now.AddDays((double)isoDefine.ExpiresDate);
                }

                this.BusinessEntities.Set<S_N_Notice>().Add(notice);
            }
        }

        string replaceNameString(string regString, Dictionary<string, object> data, List<Dictionary<string, object>> EnumDefine)
        {
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            string result = reg.Replace(regString, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                var sValue = data.GetValue(value);
                var define = EnumDefine.FirstOrDefault(d => d["FieldName"].ToString() == value);
                if (define != null)
                {
                    var enumValue = "";
                    var EnumService = FormulaHelper.GetService<IEnumService>();
                    foreach (var item in sValue.Split(','))
                    {
                        enumValue += EnumService.GetEnumText(define.GetValue("EnumKey"), item) + ",";
                    }
                    return enumValue.TrimEnd(',');
                }
                return sValue;
            });
            return result;
        }

    }
}