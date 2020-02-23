

#region 引入的命名空间

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Formula;
using Formula.Exceptions;
using System.Web.Mvc;
using System.Transactions;
using System.Configuration;
using Formula.Helper;
using Config;
using Workflow.Logic.Domain;
using Workflow.Logic;
using Workflow.Logic.BusinessFacade;
using Formula.DynConditionObject;
using System.Data;


#endregion

namespace MvcAdapter
{
    public abstract class BaseFlowController<T> : BaseController<T>, IFlowController where T : class, new()
    {
        FlowService _flowService = null;
        FlowService flowService
        {
            get
            {
                if (_flowService == null)
                {
                    _flowService = new FlowService(this, Request["FormData"], Request["ID"], Request["TaskExecID"]);
                }
                return _flowService;
            }
        }

        public override ActionResult Edit()
        {
            flowService.TransferToUpperVersionView();//升版逻辑
            return base.Edit();
        }

        public virtual JsonResult GetUpperVersionData()
        {
            var result = GetModel(Request["UpperVersion"]);
            return flowService.TransferToUpperVersionData(result);
        }

        public override JsonResult Save()
        {
            //保存流程意见
            if (!string.IsNullOrEmpty(Request["FlowAdvice"]) && !string.IsNullOrEmpty(Request["TaskExecID"]))
            {
                var flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
                string taskExecID = Request["TaskExecID"];
                var taskExec = flowEntities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
                taskExec.ExecComment = Request["FlowAdvice"];
                flowEntities.SaveChanges();
            }

            return base.Save();
        }

        public JsonResult SaveBA(string tmplCode)
        {
            return base.Save();
        }

        public override JsonResult GetModel(string id)
        {
            flowService.SetTaskFirstViewTime(Request["TaskExecID"]);
            return base.GetModel(id);
        }

        public virtual void UnExecTaskExec(string taskExecID)
        {
            FlowFO flowFO = new FlowFO();
            flowFO.UnExecTask(taskExecID);
        }

        public virtual bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment, string code)
        {
            FlowFO flowFO = new FlowFO();
            return flowFO.ExecTask(taskExec.ID, routing.ID, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment, Request["RoutingID"]);//Request["RoutingID"] 当分支路由时，其值为逗号隔开的全部分支ID
        }
        public virtual bool ExecTaskExec(S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            FlowFO flowFO = new FlowFO();
            return flowFO.ExecTask(taskExec.ID, routing.ID, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment, Request["RoutingID"]);//Request["RoutingID"] 当分支路由时，其值为逗号隔开的全部分支ID
        }
        public virtual string GetRoutingIconCls(string routingCode)
        {
            return flowService.GetRoutingIconCls(routingCode);
        }

        public override JsonResult Delete()
        {
            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);
            return Json("");
        }

        #region 委托、传阅、加签

        #region DelegateTask

        public virtual JsonResult DelegateTask()
        {
            flowService.DelegateTask(Request["TaskExecID"], Request["NextExecUserIDs"]);
            return Json("");
        }

        #endregion

        #region AskTask

        public virtual JsonResult AskTask()
        {
            flowService.AskTask(Request["TaskExecID"], Request["NextExecUserIDs"], Request["ExecComment"]);
            return Json("");
        }

        #endregion

        #region WithdrawAskTask

        public virtual JsonResult WithdrawAskTask()
        {
            flowService.WithdrawAskTask(Request["TaskExecID"]);
            return Json("");
        }

        #endregion

        #region CirculateTask

        public virtual JsonResult CirculateTask()
        {
            flowService.CirculateTask(Request["TaskExecID"], Request["NextExecUserIDs"]);
            return Json("");
        }

        #endregion

        #region ViewTask

        public virtual JsonResult ViewTask()
        {
            flowService.ViewTask(Request["TaskExecID"], Request["ExecComment"]);
            return Json("");
        }


        #endregion

        #endregion

        #region DoBack

        public virtual JsonResult DoBack(string taskExecID, string routingID, string execComment)
        {
            flowService.DoBack(taskExecID, routingID, execComment);
            return Json("");
        }

        #endregion

        #region DoBackFirst

        public virtual JsonResult DoBackFirst(string taskExecId, string execComment)
        {
            flowService.DoBackFirst(taskExecId, execComment);
            return Json("");
        }

        #endregion

        #region DoBackFirstReturn

        public virtual JsonResult DoBackFirstReturn(string taskExecId, string execComment)
        {
            flowService.DoBackFirstReturn(taskExecId, execComment);
            return Json("");
        }

        #endregion

        #region Submit

        public virtual JsonResult Submit()
        {
            string id = flowService.Submit(GetQueryString("ID"), Request["RoutingID"], Request["TaskExecID"], Request["NextExecUserIDs"], Request["NextExecUserIDsGroup"], Request["nextExecRoleIDs"], Request["nextExecOrgIDs"], Request["ExecComment"], Request["AutoPass"] == "True");
            var dic = flowService.GetAutoSubmitParam(id, Request["RoutingID"], Request["TaskExecID"], Request["NextExecUserIDs"]);
            dic.Add("ID", id);
            return Json(dic);
        }

        #endregion

        #region DeleteFlow 实际为流程的撤销方法

        public virtual JsonResult DeleteFlow()
        {
            flowService.DeleteFlow(GetQueryString("ID"), Request["TaskExecID"]);
            return Json("");
        }

        #endregion

        #region 发起人自由撤销流程

        [ValidateInput(false)]
        public virtual JsonResult FreeWidthdraw()
        {
            flowService.FreeWidthdraw(GetQueryString("ID"));
            return Json("");
        }

        #endregion

        #region GetFormControlInfo
        /// <summary>
        /// 获取表单控制信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFormControlInfo()
        {
            var dic = flowService.GetFormControlInfo(Request["TaskExecID"]);
            return Json(dic);
        }

        #endregion

        #region GetFlowButtons

        [ValidateInput(false)]
        public virtual JsonResult GetFlowButtons()
        {
            return JsonGetFlowButtons(null);
        }

        public virtual JsonResult JsonGetFlowButtons(string formData)
        {
            var btnList = flowService.JsonGetFlowButtons(Request["ID"], Request["TaskExecID"]);
            return Json(btnList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GetUser

        public string GetUserIDs(string roleIDs, string orgIDs, string excludeUserIDs)
        {
            return flowService.GetUserIDs(roleIDs, orgIDs, excludeUserIDs);
        }

        public string UserNames(string userIDs)
        {
            return flowService.UserNames(userIDs);
        }

        #endregion

        #region 接口方法

        public void SetFormFlowInfo(string id, string flowInfo)
        {
            Specifications res = new Specifications();
            res.AndAlso("ID", id, QueryMethod.Equal);
            T obj = entities.Set<T>().Where(res.GetExpression<T>()).FirstOrDefault();

            PropertyInfo pi = typeof(T).GetProperty("FlowInfo", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            if (pi != null)
                pi.SetValue(obj, flowInfo, null);
            entities.SaveChanges();
        }

        public void SetFormFlowPhase(string id, string flowPhase, string stepName)
        {
            Specifications res = new Specifications();
            res.AndAlso("ID", id, QueryMethod.Equal);
            T obj = entities.Set<T>().Where(res.GetExpression<T>()).FirstOrDefault();

            PropertyInfo pi = typeof(T).GetProperty("FlowPhase", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            if (pi != null)
                pi.SetValue(obj, flowPhase, null);
            pi = typeof(T).GetProperty("StepName", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
            if (pi != null)
                pi.SetValue(obj, stepName, null);

            entities.SaveChanges();
        }

        public void DeleteForm(string ids)
        {
            base.JsonDelete<T>(ids);
        }

        public bool ExistForm(string id)
        {
            Specifications res = new Specifications();
            res.AndAlso("ID", id, QueryMethod.Equal);
            T obj = entities.Set<T>().Where(res.GetExpression<T>()).FirstOrDefault();
            return obj != null;
        }

        #endregion

        #region 将废弃使用

        public JsonResult GetBusLeftTaskList()
        {
            var obj = flowService.GetBusLeftTaskList(Request["ID"], Request["TaskExecID"]);
            return Json(obj);
        }

        #endregion



        public JsonResult GetTaskExec(string taskexecID)
        {
            var flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
            return Json(flowEntities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskexecID));
        }

        public JsonResult SetSerialNumber(string id)
        {
            return Json("");
            //throw new NotImplementedException();
        }
    }

}
