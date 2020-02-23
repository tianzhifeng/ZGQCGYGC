using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Formula;
using Config;
using Workflow.Logic.Domain;
using Workflow.Logic;
using Base.Logic.Domain;
using HR.Logic.Domain;

namespace HR
{
    public class HRFormContorllor<T> : BaseAutoFormController where T : class, new()
    {
        SQLHelper sqlDB;
        public virtual SQLHelper HRSQLDB
        {
            get
            {
                if (sqlDB == null)
                    sqlDB = SQLHelper.CreateSqlHelper(ConnEnum.HR);
                return sqlDB;
            }
        }

        public override DbContext BusinessEntities
        {
            get
            {
                return FormulaHelper.GetEntities<HREntities>();
            }
        }

        UserInfo _userInfo;
        protected UserInfo CurrentUserInfo
        {
            get
            {
                if (_userInfo == null)
                    _userInfo = FormulaHelper.GetUserInfo();
                return _userInfo;
            }
        }


        #region 扩展虚方法

        protected virtual void OnFlowEnd(T entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        { }

        protected virtual void AfterWithDrawingTask(T entity, S_WF_InsTaskExec taskExec)
        { }

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
            SetFormFlowInfo(taskExec.S_WF_InsFlow);
            if (flag)
            {
                if (entity == null) throw new Formula.Exceptions.BusinessException("实体ID为空，未能找到相关实体对象，无法执行逻辑");
                OnFlowEnd(entity, taskExec, routing);
            }
            this.BusinessEntities.SaveChanges();
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

    }
}