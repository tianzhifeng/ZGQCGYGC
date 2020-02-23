

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
using System.Web;


#endregion

namespace MvcAdapter
{
    public class FlowService
    {
        private FlowFO flowFO = null;
        private IFlowController baseController = null;
        private WorkflowEntities flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
        private Dictionary<string, object> formDic = null;

        public FlowService(IFlowController baseController, string formData, string formInstanceID, string taskExecID)
        {
            flowFO = FormulaHelper.CreateFO<FlowFO>();
            this.baseController = baseController;
            this.formDic = flowFO.GetFormDic(formData, taskExecID, formInstanceID);
            FormulaHelper.ContextSet("FormData", formData);
        }

        public string GetRoutingIconCls(string routingCode)
        {
            return "icon-auditing_pass";
        }

        public void SetTaskFirstViewTime(string taskExecID)
        {
            if (!string.IsNullOrEmpty(taskExecID))
            {
                var taskExec = flowEntities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
                if (taskExec != null)
                {
                    if (taskExec.FirstViewTime == null)
                        taskExec.FirstViewTime = DateTime.Now;
                    if (taskExec.S_WF_InsTask.FirstViewTime == null)
                        taskExec.S_WF_InsTask.FirstViewTime = DateTime.Now;
                    flowEntities.SaveChanges();
                }
            }
        }

        #region 重载Delete方法

        public void Delete(string id, string taskExecID, string listIDs)
        {
            Func<string> deleteAction = () =>
            {
                if (!string.IsNullOrEmpty(id)) //流程启动时，从表单打开有地址栏参数ID
                {
                    flowFO.DeleteFlowByFormInstanceID(id);
                    baseController.DeleteForm(id);
                }
                else if (!string.IsNullOrEmpty(taskExecID)) //从任务列表打开删除
                {
                    S_WF_InsFlow flow = flowEntities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault().S_WF_InsTask.S_WF_InsFlow;
                    id = flow.FormInstanceID;
                    flowFO.DeleteFlowByFormInstanceID(id);
                    baseController.DeleteForm(id);
                }
                else if (!string.IsNullOrEmpty(listIDs)) //列表上的删除按钮
                {
                    var ids = listIDs.Split(',');
                    foreach (var formInstanceID in ids)
                    {
                        flowFO.DeleteFlowByFormInstanceID(formInstanceID);
                    }
                    baseController.DeleteForm(listIDs);
                }
                return "";
            };

            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var result = deleteAction();
                    ts.Complete();
                }
            }
            else
            {
                deleteAction();
            }
        }

        #endregion

        #region 委托、传阅、加签

        #region DelegateTask

        public virtual void DelegateTask(string taskExecID, string nextExecUserIDs)
        {
            string nextExecUserNames = FormulaHelper.GetService<IUserService>().GetUserNames(nextExecUserIDs);//FormulaHelper.GetUserNames(nextExecUserIDs);

            if (nextExecUserIDs.Contains(','))
                throw new FlowException("只能委托给一个人！");
            flowFO.DelegateTaskExec(taskExecID, nextExecUserIDs, nextExecUserNames);

        }

        #endregion

        #region AskTask

        public void AskTask(string taskExecID, string nextExecUserIDs, string execComment)
        {
            string nextExecUserNames = FormulaHelper.GetService<IUserService>().GetUserNames(nextExecUserIDs); //FormulaHelper.GetUserNames(nextExecUserIDs);
            flowFO.AskTaskExec(taskExecID, nextExecUserIDs, nextExecUserNames, execComment);
        }

        #endregion

        #region WithdrawAskTask

        public void WithdrawAskTask(string taskExecID)
        {
            flowFO.WidthdrawAskTaskExec(taskExecID);
        }

        #endregion

        #region CirculateTask

        public void CirculateTask(string taskExecID, string nextExecUserIDs)
        {
            string nextExecUserNames = FormulaHelper.GetService<IUserService>().GetUserNames(nextExecUserIDs);
            flowFO.CirculateTaskExec(taskExecID, nextExecUserIDs, nextExecUserNames);
        }

        #endregion

        #region ViewTask

        public void ViewTask(string taskExecID, string execComment)
        {
            flowFO.ViewTaskExec(taskExecID, execComment);
        }


        #endregion

        #endregion

        #region DoBack

        public void DoBack(string taskExecID, string routingID, string execComment)
        {
            Action submitAction = () =>
            {
                var taskExec = flowEntities.S_WF_InsTaskExec.SingleOrDefault(c => c.ID == taskExecID);
                string doBackSave = taskExec.S_WF_InsTask.S_WF_InsDefStep.DoBackSave;
                if (string.IsNullOrEmpty(doBackSave) || doBackSave == "0")
                    baseController.Save();
                flowFO.FlowDoBack(taskExecID, routingID, execComment);
            };
            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                TransactionOptions tranOp = new TransactionOptions();
                tranOp.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tranOp))
                {
                    submitAction();
                    ts.Complete();
                }
            }
            else
            {
                submitAction();
            }
        }

        #endregion

        #region DoBackFirst

        public void DoBackFirst(string taskExecId, string execComment)
        {
            Action submitAction = () =>
            {
                var taskExec = flowEntities.S_WF_InsTaskExec.SingleOrDefault(c => c.ID == taskExecId);
                string doBackSave = taskExec.S_WF_InsTask.S_WF_InsDefStep.DoBackSave;
                if (string.IsNullOrEmpty(doBackSave) || doBackSave == "0")
                    baseController.Save();
                flowFO.FlowDoBackFirst(taskExecId, execComment);
            };
            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                TransactionOptions tranOp = new TransactionOptions();
                tranOp.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tranOp))
                {
                    submitAction();
                    ts.Complete();
                }
            }
            else
            {
                submitAction();
            }
        }


        #endregion

        #region DoBackFirstReturn

        public void DoBackFirstReturn(string taskExecId, string execComment)
        {
            Action submitAction = () =>
            {
                baseController.Save();
                flowFO.FlowDoBackFirstReturn(taskExecId, execComment);
            };
            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                TransactionOptions tranOp = new TransactionOptions();
                tranOp.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tranOp))
                {
                    submitAction();
                    ts.Complete();
                }
            }
            else
            {
                submitAction();
            }
        }


        #endregion

        #region Submit

        private string GetFlowCode() //移动通只审批，因此用不到启动流程获取流程编号的此方法
        {
            string flowCode = HttpContext.Current.Request.Form["FlowCode"];
            if (string.IsNullOrEmpty(flowCode))
                flowCode = HttpContext.Current.Request.QueryString["FlowCode"];
            return flowCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">表单ID</param>
        /// <param name="routingIDs">路由IDs</param>
        /// <param name="taskExecID">任务明细ID</param>
        /// <param name="nextExecUserIDs">下一步执行人ID</param>
        /// <param name="nextExecUserIDsGroup">下一步执行人分组</param>
        /// <param name="nextExecRoleIDs">下一步执行人角色</param>
        /// <param name="nextExecOrgIDs">下一步执行人组织</param>
        /// <param name="execComment">执行意见</param>
        /// <param name="autoPass">是否自动通过（下一步为当前人时，自动通过）</param>
        /// <returns></returns>
        public string Submit(string id, string routingIDs, string taskExecID, string nextExecUserIDs, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment, bool autoPass)
        {
            Action submitAction = () =>
            {
                bool aleadySaved = false;

                aleadySaved = autoPass; //自动通过的，肯定已经保存过表单了

                if (baseController.ExistForm(id) == false)
                {
                    JsonResult result = baseController.Save();
                    Dictionary<string, object> dic = JsonHelper.ToObject<Dictionary<string, object>>(JsonHelper.ToJson(result.Data));
                    id = dic["ID"].ToString();//防止html页没有ID隐藏控件
                    aleadySaved = true;
                }

                S_WF_InsTaskExec taskExec = null;
                S_WF_InsTask task = null;
                if (!string.IsNullOrEmpty(taskExecID))
                {
                    taskExec = flowEntities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault();
                    task = taskExec.S_WF_InsTask;
                }

                foreach (string routingID in routingIDs.Split(','))
                {
                    S_WF_InsDefRouting routing = flowEntities.S_WF_InsDefRouting.Where(c => c.ID == routingID).SingleOrDefault();

                    string nextExecUserNames = FormulaHelper.GetService<IUserService>().GetUserNames(nextExecUserIDs); //FormulaHelper.GetUserNames(nextExecUserIDs);

                    if (routing.SaveForm == "1" && aleadySaved == false)
                    {
                        baseController.Save();//调用Save方法
                    }

                    #region 分支路由取人

                    if (routingIDs.Split(',').Length > 1 || (string.IsNullOrEmpty(nextExecUserIDs) && HttpContext.Current.Request["IsMobileRequest"] == "True"))//分支路由情况或移动端提交情况
                    {
                        nextExecOrgIDs = flowFO.GetRoutingOrgIDs(routing, formDic, taskExec);
                        nextExecRoleIDs = flowFO.GetRoutingRoleIDs(routing, formDic, taskExec);
                        nextExecUserIDsGroup = flowFO.GetRoutingUserIDsGroup(routing, formDic);
                        nextExecUserIDs = flowFO.GetRoutingUserIDs(routing, formDic, taskExec, nextExecOrgIDs, nextExecRoleIDs);
                        nextExecUserNames = FormulaHelper.GetService<IUserService>().GetUserNames(nextExecUserIDs);
                    }

                    #endregion

                    if (taskExec == null)
                    {
                        var flow = flowEntities.S_WF_InsFlow.Where(c => c.FormInstanceID == id).SingleOrDefault();
                        if (flow == null)
                        {
                            UserInfo user = FormulaHelper.GetUserInfo(); //启动流程前
                            //启动流程
                            taskExec = flowFO.StartFlow(GetFlowCode(), id, user.UserID, user.UserName);
                            if (!taskExec.S_WF_InsFlow.S_WF_InsDefFlow.S_WF_InsDefRouting.Contains(routing))
                                throw new Exception("流程定义已经修改！");
                        }
                        else
                        {
                            string strInital = StepTaskType.Inital.ToString();
                            taskExec = flow.S_WF_InsTask.Where(c => c.Type == strInital).SingleOrDefault().S_WF_InsTaskExec.SingleOrDefault();
                        }
                    }

                    bool bCompleteFlow = baseController.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
                    if (bCompleteFlow)
                    {
                        //流程结束时候设置流水号
                        baseController.SetSerialNumber(id);
                    }
                    //流程结束后给申请人发消息
                    if (bCompleteFlow && routing.S_WF_InsDefFlow.SendMsgToApplicant == "1")
                    {
                        //流程结束后给申请人发消息
                        string content = string.Format("{0}.{1}已完成", taskExec.S_WF_InsTask.S_WF_InsFlow.FlowName, routing.S_WF_InsDefFlow.Name);
                        string link = "";//不带连接了......
                        FormulaHelper.GetService<IMessageService>().SendMsg(content, content, link, "", taskExec.S_WF_InsFlow.CreateUserID, taskExec.S_WF_InsFlow.CreateUserName, null, MsgReceiverType.UserType, MsgType.Normal);
                    }
                    //流程结束后给全部人发消息
                    if (bCompleteFlow && routing.S_WF_InsDefFlow.MsgSendToAll == "1")
                    {
                        var flow = flowEntities.S_WF_InsFlow.Where(c => c.FormInstanceID == id).SingleOrDefault();
                        var userIDs = string.Join(",", flow.S_WF_InsTaskExec.Select(c => c.ExecUserID).ToArray());
                        var userNames = string.Join(",", flow.S_WF_InsTaskExec.Select(c => c.ExecUserName).ToArray());

                        //流程结束后给申请人发消息
                        string content = string.Format("{0}.{1}已完成", taskExec.S_WF_InsTask.S_WF_InsFlow.FlowName, routing.S_WF_InsDefFlow.Name);
                        string link = "";//不带连接了......
                        FormulaHelper.GetService<IMessageService>().SendMsg(content, content, link, "", userIDs, userNames, null, MsgReceiverType.UserType, MsgType.Normal);
                    }

                    //返写流程当前环节与用户
                    flowFO.BackWriteInsFlowPhase(routing.EndID, nextExecUserNames);
                }
            };

            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                TransactionOptions tranOp = new TransactionOptions();
                tranOp.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tranOp))
                {
                    submitAction();
                    ts.Complete();
                }
            }
            else
            {
                submitAction();
            }

            return id;
        }

        //重写
        public string Submit(string id, string routingIDs, string taskExecID, string nextExecUserIDs, string nextExecUserIDsGroup, string nextExecRoleIDs,
            string nextExecOrgIDs, string execComment, string tmplCode, bool autoPass)
        {
            Action submitAction = () =>
            {
                bool aleadySaved = false;

                aleadySaved = autoPass; //自动通过的，肯定已经保存过表单了


                S_WF_InsTaskExec taskExec = null;
                S_WF_InsTask task = null;
                if (!string.IsNullOrEmpty(taskExecID))
                {
                    taskExec = flowEntities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault();
                    task = taskExec.S_WF_InsTask;
                }

                foreach (string routingID in routingIDs.Split(','))
                {
                    S_WF_InsDefRouting routing = flowEntities.S_WF_InsDefRouting.Where(c => c.ID == routingID).SingleOrDefault();

                    string nextExecUserNames = FormulaHelper.GetService<IUserService>().GetUserNames(nextExecUserIDs); //FormulaHelper.GetUserNames(nextExecUserIDs);

                    #region 分支路由取人

                    if (routingIDs.Split(',').Length > 1) //分支路由情况
                    {
                        nextExecOrgIDs = flowFO.GetRoutingOrgIDs(routing, formDic, taskExec);
                        nextExecRoleIDs = flowFO.GetRoutingRoleIDs(routing, formDic, taskExec);
                        nextExecUserIDsGroup = flowFO.GetRoutingUserIDsGroup(routing, formDic);
                        nextExecUserIDs = flowFO.GetRoutingUserIDs(routing, formDic, taskExec, nextExecOrgIDs, nextExecRoleIDs);
                        nextExecUserNames = FormulaHelper.GetService<IUserService>().GetUserNames(nextExecUserIDs);
                    }

                    #endregion


                    if (routing.SaveForm == "1" && aleadySaved == false)
                    {
                        baseController.SaveBA(tmplCode);//调用Save方法
                    }

                    if (taskExec == null)
                    {
                        var flow = flowEntities.S_WF_InsFlow.Where(c => c.FormInstanceID == id).SingleOrDefault();
                        if (flow == null)
                        {
                            UserInfo user = FormulaHelper.GetUserInfo(); //启动流程前
                            //启动流程
                            taskExec = flowFO.StartFlow(GetFlowCode(), id, user.UserID, user.UserName);
                            if (!taskExec.S_WF_InsFlow.S_WF_InsDefFlow.S_WF_InsDefRouting.Contains(routing))
                                throw new Exception("流程定义已经修改！");
                        }
                        else
                        {
                            string strInital = StepTaskType.Inital.ToString();
                            taskExec = flow.S_WF_InsTask.Where(c => c.Type == strInital).SingleOrDefault().S_WF_InsTaskExec.SingleOrDefault();
                        }
                    }

                    bool bCompleteFlow = baseController.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment, tmplCode);

                    if (bCompleteFlow)
                    {
                        //流程结束时候设置流水号
                        baseController.SetSerialNumber(id);
                    }

                    //流程结束后给申请人发消息
                    if (bCompleteFlow && routing.S_WF_InsDefFlow.SendMsgToApplicant == "1")
                    {
                        //流程结束后给申请人发消息
                        string content = string.Format("{0}.{1}已完成", taskExec.S_WF_InsTask.S_WF_InsFlow.FlowName, routing.S_WF_InsDefFlow.Name);
                        string link = "";//不带连接了......
                        FormulaHelper.GetService<IMessageService>().SendMsg(content, content, link, "", taskExec.S_WF_InsFlow.CreateUserID, taskExec.S_WF_InsFlow.CreateUserName, null, MsgReceiverType.UserType, MsgType.Normal);
                    }
                    //流程结束后给全部人发消息
                    if (bCompleteFlow && routing.S_WF_InsDefFlow.MsgSendToAll == "1")
                    {
                        var flow = flowEntities.S_WF_InsFlow.Where(c => c.FormInstanceID == id).SingleOrDefault();
                        var userIDs = string.Join(",", flow.S_WF_InsTaskExec.Select(c => c.ExecUserID).ToArray());
                        var userNames = string.Join(",", flow.S_WF_InsTaskExec.Select(c => c.ExecUserName).ToArray());

                        //流程结束后给申请人发消息
                        string content = string.Format("{0}.{1}已完成", taskExec.S_WF_InsTask.S_WF_InsFlow.FlowName, routing.S_WF_InsDefFlow.Name);
                        string link = "";//不带连接了......
                        FormulaHelper.GetService<IMessageService>().SendMsg(content, content, link, "", userIDs, userNames, null, MsgReceiverType.UserType, MsgType.Normal);
                    }
                }
            };

            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                TransactionOptions tranOp = new TransactionOptions();
                tranOp.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, tranOp))
                {
                    submitAction();
                    ts.Complete();
                }
            }
            else
            {
                submitAction();
            }
            return id;
        }


        #endregion


        #region GetAutoSubmitParam

        public Dictionary<string, string> GetAutoSubmitParam(string formInstanceID, string routingIDs, string taskExecID, string nextExecUserIDs)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var user = FormulaHelper.GetUserInfo();

            if (nextExecUserIDs != user.UserID) //下一步执行人非当前人
                return dic;
            else if (routingIDs.Contains(',') == true) //分支不能自动通过
                return dic;

            S_WF_InsTaskExec nextTaskExec = null;
            if (string.IsNullOrEmpty(taskExecID) == false) //从任务列表上执行
            {
                S_WF_InsTaskExec taskExec = flowEntities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
                var arrs = taskExec.S_WF_InsFlow.S_WF_InsTaskExec.Where(c => c.S_WF_InsTask.SendTaskIDs == taskExec.S_WF_InsTask.ID).ToArray();
                if (arrs.Length == 1)
                    nextTaskExec = arrs[0];
                //nextTaskExec = taskExec.S_WF_InsFlow.S_WF_InsTaskExec.SingleOrDefault(c => c.S_WF_InsTask.SendTaskIDs == taskExec.S_WF_InsTask.ID);
                if (nextTaskExec == null)
                    return dic;
            }
            else//启动流程时
            {
                nextTaskExec = flowEntities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.S_WF_InsFlow.FormInstanceID == formInstanceID && c.TaskUserID == user.UserID && c.ExecTime == null);
            }

            //没有下一环节任务了，不自动通过
            if (nextTaskExec == null)
                return dic;

            //任务需要等待，不能自动通过
            if (!string.IsNullOrEmpty(nextTaskExec.S_WF_InsTask.WaitingSteps) || !string.IsNullOrEmpty(nextTaskExec.S_WF_InsTask.WaitingRoutings))
                return dic;

            var nextRoutingList = flowFO.GetRoutingList(nextTaskExec.ID, formDic);

            /* //去除画的打回首环节的路由。
             for (int i = nextRoutingList.Count - 1; i >= 0; i--)
             {
                 var endID = nextRoutingList[i].EndID;
                 var endStep = flowEntities.S_WF_InsDefStep.SingleOrDefault(c => c.ID == endID);
                 if (endStep.Type == StepTaskType.Inital.ToString())
                     nextRoutingList.RemoveAt(i);
             }*/
            if (nextRoutingList.Count == 0) //下一步未执行的路由不存在，不能自动通过
                return dic;

            if (nextRoutingList.Count > 1) //下一步路由多于一条不能自动通过
                return dic;


            //范围选人和打回重新选人不能自动通过
            if (nextRoutingList[0].SelectMode == "SelectOneUserInScope" || nextRoutingList[0].SelectMode == "SelectMultiUserInScope" || nextRoutingList[0].SelectMode == "SelectDoback")
                return dic;

            //有必填不能自动通过
            if (!string.IsNullOrWhiteSpace(nextRoutingList[0].NotNullFields))
                return dic;

            //自动通过后依然是本环节，则不能自动通过
            var routing = flowEntities.Set<S_WF_InsDefRouting>().SingleOrDefault(c => c.ID == routingIDs);//当前点击的路由
            if (routing.InsDefStepID == nextRoutingList[0].EndID)
                return dic;


            var routingParam = flowFO.GetRoutingParams(nextRoutingList[0], nextTaskExec, formInstanceID);

            //如果自动通过环节的执行人取自表单，则取人逻辑放在后台执行。（注释放开，后台取人）
            if (!string.IsNullOrEmpty(nextRoutingList[0].UserIDsFromField))
            {
                string userIds = "";
                foreach (string field in nextRoutingList[0].UserIDsFromField.Split(','))
                {
                    if (formDic.Keys.Contains(field) && formDic[field] != null)
                    {
                        userIds = StringHelper.Include(userIds, formDic[field].ToString());
                    }
                }
                routingParam.userIDs = StringHelper.Include(routingParam.userIDs, userIds);
            }

            bool flag = false;

            if (nextRoutingList[0].DenyAutoPass == "1" || nextRoutingList[0].DenyAutoPass.ToLower() == "true")
            {
                flag = false;
            }
            else if (nextRoutingList[0].ID.Contains(','))//分支路由自动执行
            {
                flag = true;
            }
            else if (string.IsNullOrEmpty(routingParam.userIDs) == false) //下一步有执行人自动执行
            {
                flag = true;
            }
            else //结束环节自动执行
            {
                var endID = nextRoutingList[0].EndID;
                var nextStep = flowEntities.Set<S_WF_InsDefStep>().SingleOrDefault(c => c.ID == endID);
                if (nextStep.Type == StepTaskType.Completion.ToString())
                    flag = true;
            }

            if (flag == true)
            {
                dic.Add("NextTaskExecID", nextTaskExec.ID);
                dic.Add("NextRoutingID", nextRoutingList[0].ID);
                dic.Add("NextRoutingName", nextRoutingList[0].Name);
                dic.Add("NextExecUserIDs", routingParam.userIDs);
            }

            return dic;
        }

        #endregion

        #region DeleteFlow 实际为流程的撤销方法

        public void DeleteFlow(string id, string taskExecID)
        {
            string flowPhase = "";
            string stepName = "";
            WorkflowEntities flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
            if (string.IsNullOrEmpty(taskExecID))
            {
                flowPhase = "Start";
                stepName = "";

                var flow = flowEntities.S_WF_InsFlow.SingleOrDefault(c => c.FormInstanceID == id);

                //判断是否能撤销
                if (flow.S_WF_InsTaskExec.Count(c => c.ExecTime != null) > 1)
                    throw new FlowException("流程已经无法撤销");

                flowFO.DeleteFlowByFormInstanceID(id);

            }
            else
            {
                var taskExec = flowEntities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
                flowPhase = taskExec.S_WF_InsTask.S_WF_InsDefStep.Phase;
                stepName = taskExec.S_WF_InsTask.S_WF_InsDefStep.Name;
                baseController.UnExecTaskExec(taskExecID);
            }

            #region 控制表单的FlowPhase和StepName

            baseController.SetFormFlowPhase(id, flowPhase, stepName);

            #endregion

        }

        #endregion

        #region 发起人自由撤销

        /// <summary>
        /// 发起人自由撤销流程
        /// </summary>
        /// <param name="id"></param>
        public void FreeWidthdraw(string id)
        {
            #region 发消息通知

            var flow = flowEntities.Set<S_WF_InsFlow>().SingleOrDefault(c => c.FormInstanceID == id);
            string currentUserID = FormulaHelper.UserID;
            string userIds = string.Join(",", flow.S_WF_InsTaskExec.Where(c => c.ExecUserID != currentUserID && c.ExecTime != null).Select(c => c.ExecUserID).ToArray());
            string userNames = string.Join(",", flow.S_WF_InsTaskExec.Where(c => c.ExecUserID != currentUserID && c.ExecTime != null).Select(c => c.ExecUserName).ToArray());
            //流程结束后给申请人发消息
            string content = string.Format("流程已撤销：{0}", flow.FlowName);
            string link = "";//不带连接了......
            if (userIds.Trim(',').Replace(" ", "") != "")
                FormulaHelper.GetService<IMessageService>().SendMsg(content, content, link, "", userIds, userNames, null, MsgReceiverType.UserType, MsgType.Normal);

            #endregion

            flowFO.DeleteFlowByFormInstanceID(id);
            baseController.SetFormFlowPhase(id, "Start", "");
        }

        #endregion

        #region GetFormControlInfo
        /// <summary>
        /// 获取表单控制信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetFormControlInfo(string taskExecID)
        {
            S_WF_InsDefStep step = null;
            string flowCode = GetFlowCode();
            var flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();

            if (!string.IsNullOrEmpty(taskExecID))
            {
                step = flowEntities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault().S_WF_InsTask.S_WF_InsDefStep;
            }
            else
            {
                FlowFO flowFO = FormulaHelper.CreateFO<FlowFO>();
                step = flowFO.GetStartStep(flowCode);
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("HiddenElements", step.HiddenElements ?? "");
            dic.Add("VisibleElements", step.VisibleElements ?? "");
            dic.Add("EditableElements", step.EditableElements ?? "");
            dic.Add("DisableElements", step.DisableElements ?? "");

            return dic;
        }

        #endregion

        #region GetUser

        public string GetUserIDs(string roleIDs, string orgIDs, string excludeUserIDs)
        {
            string userIDs = "";
            if (!string.IsNullOrEmpty(roleIDs))
            {
                userIDs = FormulaHelper.GetService<IRoleService>().GetUserIDsInRoles(roleIDs, orgIDs, excludeUserIDs);
                //2018-1-30 剥离项目角色选人功能
                var prjRoleUser = PrjRoleExt.GetRoleUserIDs(roleIDs, orgIDs);
                if (!string.IsNullOrEmpty(prjRoleUser))
                    userIDs = (prjRoleUser + "," + userIDs).Trim(',');
            }
            else
                userIDs = FormulaHelper.GetService<IOrgService>().GetUserIDsInOrgs(orgIDs);
            return userIDs;
        }

        public string UserNames(string userIDs)
        {
            return FormulaHelper.GetService<IUserService>().GetUserNames(userIDs);
        }

        #endregion

        #region 获取按钮

        public List<ButtonInfo> JsonGetFlowButtons(string id, string taskExecID, bool isMobileClient = false)
        {
            string flowCode = GetFlowCode();
            List<ButtonInfo> btnList = new List<ButtonInfo>();
            //流程丢失的情况
            if (!string.IsNullOrEmpty(flowCode) && !string.IsNullOrEmpty(id))
            {
                var def = flowEntities.S_WF_DefFlow.Where(c => c.Code == flowCode).OrderByDescending(c => c.ID).FirstOrDefault();

                var flow = flowEntities.S_WF_InsFlow.SingleOrDefault(c => c.FormInstanceID == id);
                if (flow == null)//流程不存在
                {
                    //表单FlowPhase为end
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(def.ConnName);
                    var dt = sqlHelper.ExecuteDataTable(string.Format("select * from {0} where ID='{1}'", def.TableName, id));
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Columns.Contains("FlowPhase") && dt.Rows[0]["FlowPhase"].ToString() == "End")
                            return new List<ButtonInfo>();
                    }
                }
            }
            Boolean commentPos = Convert.ToBoolean(ConfigurationManager.AppSettings["showCommentPosition"]);

            S_WF_InsTaskExec taskExec = null;
            S_WF_InsTask task = null;
            UserInfo userInfo = null;
            if (!string.IsNullOrEmpty(taskExecID))
            {
                taskExec = flowEntities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault();
                task = taskExec.S_WF_InsTask;
                if (task.S_WF_InsDefStep.HideAdvice == "1")
                {
                    commentPos = false;
                }
                userInfo = FormulaHelper.GetUserInfoByID(taskExec.TaskUserID);
            }
            else
            {
                S_WF_InsDefStep defStep = flowFO.GetStartStep(flowCode);
                if (defStep.HideAdvice == "1")
                    commentPos = false;
                userInfo = FormulaHelper.GetUserInfo();//启动流程前
            }

            var isEnglish = FormulaHelper.GetCurrentLGID() != "EN";
            ButtonInfo btnWithdraw = new ButtonInfo { sortIndex = -100, id = "btnWithdraw", text = isEnglish ? "撤销" : "Withdraw", iconCls = "icon-undo", onclick = "flowWithdraw();" };
            ButtonInfo btnFreeWithdraw = new ButtonInfo { sortIndex = -100, id = "btnFreeWithdraw", text = isEnglish ? "撤销" : "FreeWithdraw", iconCls = "icon-undo", onclick = "flowFreeWithdraw();" };
            ButtonInfo btnDelegate = new ButtonInfo { sortIndex = 100, id = "btnDelegate", text = isEnglish ? "委托" : "Delegate", iconCls = "icon-delegate", onclick = (commentPos ? "flowApprove(\"flowDelegating()\")" : "flowDelegating();") };
            ButtonInfo btnCirculate = new ButtonInfo { sortIndex = 101, id = "btnCirculate", text = isEnglish ? "传阅" : "Circulate", iconCls = "icon-circulate", onclick = (commentPos ? "flowApprove(\"flowCirculating()\")" : "flowCirculating();") };
            ButtonInfo btnAsk = new ButtonInfo { sortIndex = 102, id = "btnAsk", text = isEnglish ? "加签" : "Ask", iconCls = "icon-ask", onclick = (commentPos ? "flowApprove(\"flowAsking()\")" : "flowAsking();") };
            ButtonInfo btnWithdrawAsk = new ButtonInfo { sortIndex = 103, id = "btnWithdrawAsk", text = isEnglish ? "撤销加签" : "WithdrawAsk", iconCls = "icon-undo", onclick = "flowWithdrawAsk();" };
            ButtonInfo btnView = new ButtonInfo { sortIndex = 104, id = "btnView", text = isEnglish ? "阅毕" : "View", iconCls = "icon-auditing", onclick = (commentPos ? "flowApprove(\"flowViewing()\")" : "flowViewing();") };
            ButtonInfo btnReply = new ButtonInfo { sortIndex = 105, id = "btnReply", text = isEnglish ? "回复加签" : "ReplyAsk", iconCls = "icon-auditing", onclick = (commentPos ? "flowApprove(\"flowReplying()\")" : "flowReplying();") };
            ButtonInfo btnSave = new ButtonInfo { sortIndex = 90, id = "btnSave", text = isEnglish ? "暂存" : "Save", iconCls = "icon-save", onclick = (commentPos ? "flowApprove(\"flowSave();\")" : "flowSave();") };
            ButtonInfo btnDelete = new ButtonInfo { sortIndex = 91, id = "btnDelete", text = isEnglish ? "删除" : "Delete", iconCls = "icon-remove", onclick = "flowDelete();" };
            ButtonInfo btnTrace = new ButtonInfo { sortIndex = 290, id = "btnTrace", text = isEnglish ? "跟踪" : "Trace", iconCls = "icon-search", onclick = "flowTrace();" };
            ButtonInfo btnExport = new ButtonInfo { sortIndex = 289, id = "btnExport", text = isEnglish ? "导出Word" : "ExportWord", iconCls = "icon-word", onclick = "flowExport('{0}','{1}');" };
            ButtonInfo btnPdf = new ButtonInfo { sortIndex = 288, id = "btnPdf", text = isEnglish ? "导出PDF" : "ExportPDF", iconCls = "icon-pdf", onclick = "flowPdf('{0}','{1}');" };
            ButtonInfo btnPrint = new ButtonInfo { sortIndex = 287, id = "btnPrint", text = isEnglish ? "打印" : "Print", iconCls = "icon-print", onclick = "flowPrint();" };

            ButtonInfo btnDoBack = new ButtonInfo { sortIndex = 80, id = "btnDoBack", text = isEnglish ? "驳回上一步" : "DoBack", iconCls = "icon-undo", onclick = (commentPos ? "flowApprove(\"flowDoBack('{0}','{1}','{2}')\")" : "flowDoBack('{0}','{1}','{2}');") };
            ButtonInfo btnDoBackFirst = new ButtonInfo { sortIndex = 81, id = "btnDoBackFirst", text = isEnglish ? "驳回发起人" : "DoBackFirst", iconCls = "icon-undo", onclick = (commentPos ? "flowApprove(\"flowDoBackFirst('{0}')\")" : "flowDoBackFirst('{0}');") };
            ButtonInfo btnDoBackFirstReturn = new ButtonInfo { sortIndex = 82, id = "btnDoBackFirstReturn", text = isEnglish ? "送驳回人" : "DoBackFirstReturn", iconCls = "icon-redo", onclick = (commentPos ? "flowApprove(\"flowDoBackFirstReturn('{0}')\")" : "flowDoBackFirstReturn('{0}');") };
            ButtonInfo btnFlowChart = new ButtonInfo { sortIndex = 300, id = "btnFlowChart", text = isEnglish ? "流程说明" : "FlowChart", iconCls = "icon-search", onclick = "flowChart('{0}');" };

            List<S_WF_InsDefRouting> routingList = new List<S_WF_InsDefRouting>();
            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(flowCode)) //从表单列表添加
            {
                routingList = flowFO.GetStartRoutingList(flowCode, formDic);

                #region 流程启动时是否显示暂存按钮

                var entities = FormulaHelper.GetEntities<WorkflowEntities>();
                var def = entities.S_WF_DefFlow.Where(c => c.Code == flowCode).OrderByDescending(c => c.ID).FirstOrDefault();

                if (def == null)
                    throw new FlowException(string.Format("编号为：{0}的流程不存在", flowCode));

                var insDef = def.GetInsDefFlow();
                string Inital = StepTaskType.Inital.ToString();
                var step = insDef.S_WF_InsDefStep.Where(c => c.Type == Inital).SingleOrDefault();
                if (step.AllowSave == "1")
                    btnList.Add(btnSave);

                #endregion
            }
            else if (!string.IsNullOrEmpty(taskExecID)) //从任务列表打开
            {
                if (taskExec.ExecTime == null) //未完成任务
                {
                    var taskExecType = (TaskExecType)Enum.Parse(typeof(TaskExecType), taskExec.Type);
                    switch (taskExecType)
                    {
                        case TaskExecType.Ask:
                            btnList.Add(btnReply);
                            break;
                        case TaskExecType.Circulate:
                            btnList.Add(btnView);
                            btnList.Add(btnCirculate);
                            break;
                        case TaskExecType.Delegate:
                            //流程按钮
                            routingList = flowFO.GetRoutingList(taskExecID, formDic);
                            //保存按钮
                            if (taskExec.S_WF_InsTask.S_WF_InsDefStep.AllowSave == "1" && flowFO.HasVariableAuth(taskExec.S_WF_InsTask.InsFlowID, taskExec.S_WF_InsTask.S_WF_InsDefStep.SaveVariableAuth)) //环节允许保存，且满足保存的变量条件
                                btnList.Add(btnSave);
                            break;
                        case TaskExecType.Normal:
                            //流程按钮
                            routingList = flowFO.GetRoutingList(taskExecID, formDic);

                            //委托按钮
                            if (taskExec.S_WF_InsTask.S_WF_InsDefStep.AllowDelegate == "1")
                                btnList.Add(btnDelegate);
                            //加签按钮
                            if (taskExec.S_WF_InsTask.S_WF_InsDefStep.AllowAsk == "1")
                            {
                                string execType = TaskExecType.Ask.ToString();
                                if (taskExec.S_WF_InsTask.S_WF_InsTaskExec.Where(c => c.Type == execType && c.TaskUserID == taskExec.TaskUserID && c.ExecTime == null).Count() > 0)
                                {
                                    btnList.Add(btnWithdrawAsk);
                                    routingList = new List<S_WF_InsDefRouting>();//不显示执行按钮
                                }
                                else
                                {
                                    btnList.Add(btnAsk);
                                }
                            }
                            //传阅按钮
                            if (taskExec.S_WF_InsTask.S_WF_InsDefStep.AllowCirculate == "1")
                                btnList.Add(btnCirculate);
                            //保存按钮
                            if (taskExec.S_WF_InsTask.S_WF_InsDefStep.AllowSave == "1" && flowFO.HasVariableAuth(taskExec.S_WF_InsTask.InsFlowID, taskExec.S_WF_InsTask.S_WF_InsDefStep.SaveVariableAuth)) //环节允许保存，且满足保存的变量条件
                                btnList.Add(btnSave);

                            #region 删除按钮
                            //查询起始环节任务
                            var flow = taskExec.S_WF_InsFlow;
                            var strInital = StepTaskType.Inital.ToString();
                            var startTasks = flowEntities.S_WF_InsTask.Where(c => c.InsFlowID == flow.ID && c.Type == strInital);

                            //发起人可删除自己启动的表单                            
                            if (flow.S_WF_InsDefFlow.AllowDeleteForm == "1")
                            {
                                var lastInitTask = startTasks.OrderBy(c => c.CreateTime).ToList().LastOrDefault();
                                if (lastInitTask.TaskUserIDs == userInfo.UserID && lastInitTask.CompleteTime == null)
                                    btnList.Add(btnDelete);
                            }
                            #endregion

                            break;
                    }

                    #region 驳回和送驳回人
                    if (taskExecType == TaskExecType.Normal || taskExecType == TaskExecType.Delegate)
                    {
                        if (taskExec.S_WF_InsTask.OnlyDoBack == "1")
                        {
                            routingList = new List<S_WF_InsDefRouting>();//清空流程执行按钮
                        }
                        if (taskExec.S_WF_InsTask.DoBackRoutingID != "")
                        {
                            if (taskExec.S_WF_InsTask.OnlyDoBack == "1")
                            {
                                btnDoBack.onclick = string.Format(btnDoBack.onclick, taskExec.ID, taskExec.S_WF_InsTask.DoBackRoutingID, "送驳回人");
                                btnDoBack.text = "送驳回人";
                                btnDoBack.iconCls = "icon-redo";
                            }
                            else
                            {
                                btnDoBack.onclick = string.Format(btnDoBack.onclick, taskExec.ID, taskExec.S_WF_InsTask.DoBackRoutingID, "驳回上一步");
                            }

                            //驳回按钮的名字
                            if (!string.IsNullOrWhiteSpace(taskExec.S_WF_InsTask.S_WF_InsDefStep.DoBackButtonText))
                            {
                                var senderTask = flowEntities.S_WF_InsTask.SingleOrDefault(c => c.ID == taskExec.S_WF_InsTask.SendTaskIDs);
                                if (senderTask != null)
                                {
                                    btnDoBack.text = taskExec.S_WF_InsTask.S_WF_InsDefStep.DoBackButtonText.Replace("{StepName}", senderTask.S_WF_InsDefStep.Name);
                                }
                            }

                            btnList.Add(btnDoBack);
                        }
                    }

                    #endregion

                    #region 驳回首环节和送驳回人

                    if (taskExecType == TaskExecType.Normal || taskExecType == TaskExecType.Delegate)
                    {
                        if (task.SendTaskIDs.Contains(',') == false) //非路由汇聚的普通环节
                        {
                            var sendTask = flowEntities.S_WF_InsTask.SingleOrDefault(c => c.ID == task.SendTaskIDs);
                            if (sendTask != null)
                            {
                                //当前是首环节，且发送环节允许直送
                                if (task.S_WF_InsDefStep.Type == StepTaskType.Inital.ToString() && sendTask.S_WF_InsDefStep.AllowDoBackFirstReturn == "1") //送驳回人
                                {
                                    routingList = new List<S_WF_InsDefRouting>();//不显示执行按钮
                                    btnDoBackFirstReturn.onclick = string.Format(btnDoBackFirstReturn.onclick, taskExec.ID);

                                    //送驳回人路由带上环节名字
                                    btnDoBackFirstReturn.text = string.Format("送驳回人({0})", sendTask.S_WF_InsDefStep.Name);

                                    //直送按钮的名字
                                    if (!string.IsNullOrWhiteSpace(taskExec.S_WF_InsTask.S_WF_InsDefStep.DoBackButtonText))
                                    {
                                        var senderTask = flowEntities.S_WF_InsTask.SingleOrDefault(c => c.ID == taskExec.S_WF_InsTask.SendTaskIDs);
                                        if (senderTask != null)
                                        {
                                            btnDoBackFirstReturn.text = taskExec.S_WF_InsTask.S_WF_InsDefStep.DoBackButtonText.Replace("{StepName}", senderTask.S_WF_InsDefStep.Name);
                                        }
                                    }


                                    btnList.Add(btnDoBackFirstReturn);
                                }
                                if (task.S_WF_InsDefStep.AllowDoBackFirst == "1" && task.S_WF_InsDefStep.Type == StepTaskType.Normal.ToString())//驳回首环节
                                {
                                    btnDoBackFirst.onclick = string.Format(btnDoBackFirst.onclick, taskExec.ID);
                                    btnList.Add(btnDoBackFirst);
                                }
                            }
                        }
                        else //路由汇聚环节
                        {
                            if (task.S_WF_InsDefStep.AllowDoBackFirst == "1" && task.S_WF_InsDefStep.Type == StepTaskType.Normal.ToString())//驳回首环节
                            {
                                btnDoBackFirst.onclick = string.Format(btnDoBackFirst.onclick, taskExec.ID);
                                btnList.Add(btnDoBackFirst);
                            }
                        }
                    }
                    #endregion

                }

                if (flowFO.AllowWithDraw(taskExec))
                    btnList.Add(btnWithdraw);
                btnList.Add(btnTrace);

                #region 发起人可以传阅
                //if (task.S_WF_InsDefStep.Type == StepTaskType.Inital.ToString() && taskExec.Type != TaskExecType.Circulate.ToString())
                if (System.Configuration.ConfigurationManager.AppSettings["Flow_Circulate"] == "True" && taskExec.Type != TaskExecType.Circulate.ToString())
                    btnList.Add(btnCirculate);

                #endregion

                //导出按钮
                object obj = baseSqlHelper.ExecuteScalar(string.Format("select count(1) from S_UI_Word where Code='{0}'", taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code));
                if (Convert.ToInt32(obj) > 0)
                {
                    btnExport.onclick = string.Format(btnExport.onclick, taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code, taskExec.S_WF_InsFlow.FormInstanceID);
                    btnList.Add(btnExport);

                    btnPdf.onclick = string.Format(btnPdf.onclick, taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code, taskExec.S_WF_InsFlow.FormInstanceID);
                    btnList.Add(btnPdf);
                }
            }
            else if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(flowCode)) //从表单列表编辑
            {
                S_WF_InsFlow flow = flowEntities.S_WF_InsFlow.Where(c => c.FormInstanceID == id).SingleOrDefault();
                if (flow == null) //流程尚未启动
                {
                    routingList = flowFO.GetStartRoutingList(flowCode, formDic);
                    btnList.Add(btnSave);
                    btnList.Add(btnDelete);
                }
                else
                {
                    //查询起始环节任务
                    var strInital = StepTaskType.Inital.ToString();
                    var startTasks = flowEntities.S_WF_InsTask.Where(c => c.InsFlowID == flow.ID && c.Type == strInital);

                    //发起人可删除自己启动的表单
                    if (flow.S_WF_InsDefFlow.AllowDeleteForm == "1")
                    {
                        var lastInitTask = startTasks.OrderBy(c => c.CreateTime).ToList().LastOrDefault();
                        if (lastInitTask.TaskUserIDs == userInfo.UserID && lastInitTask.CompleteTime == null)
                            btnList.Add(btnDelete);
                    }

                    //如果配置了可自由撤销
                    if (System.Configuration.ConfigurationManager.AppSettings["Flow_FreeWidthdraw"] == "True")
                    {
                        if (flow.Status != FlowTaskStatus.Complete.ToString())
                            btnList.Add(btnFreeWithdraw);
                    }
                    else if (startTasks.Count() == 1)//表单首次提交可以撤销
                    {
                        var startTask = startTasks.SingleOrDefault();

                        taskExec = startTask.S_WF_InsTaskExec.Where(c => c.ExecUserID == userInfo.UserID).SingleOrDefault();
                        if (taskExec != null) //任务已经执行过
                        {
                            if (taskExec.ExecTime == null)
                                routingList = flowFO.GetStartRoutingList(flowCode, formDic);
                            else
                            {
                                if (flow.Status != FlowTaskStatus.Complete.ToString() && startTask.SendTaskUserIDs == userInfo.UserID && startTask.S_WF_InsFlow.S_WF_InsTaskExec.Where(c => c.ExecTime != null).Count() == 1) //只有当前人执行过任务
                                    btnList.Add(btnWithdraw);
                            }
                        }
                    }

                    btnList.Add(btnTrace);

                    //导出按钮
                    object obj = baseSqlHelper.ExecuteScalar(string.Format("select count(1) from S_UI_Word where Code='{0}'", flowCode));
                    if (Convert.ToInt32(obj) > 0)
                    {
                        btnExport.onclick = string.Format(btnExport.onclick, flowCode, id);
                        btnList.Add(btnExport);

                        btnPdf.onclick = string.Format(btnPdf.onclick, flowCode, id);
                        btnList.Add(btnPdf);
                    }
                }
            }

            foreach (var routing in routingList.OrderByDescending(c => c.SortIndex))
            {
                RoutingParams param = flowFO.GetRoutingParams(routing, taskExec, id, isMobileClient);
                string iconCls = GetRoutingIconCls(routing.Code);
                btnList.Insert(0, new ButtonInfo { routingParams = param, id = routing.ID, iconCls = iconCls, text = !isEnglish && !string.IsNullOrEmpty(routing.NameEN) ? routing.NameEN : routing.Name, onclick = string.Format((commentPos ? "flowApprove(\'flowSubmitting({0})\', 1)" : "flowSubmitting({0});"), JsonHelper.ToJson(param)) });
            }
            object isPrint = baseSqlHelper.ExecuteScalar(string.Format("Select IsPrint From S_UI_Form Where Code='{0}'", HttpContext.Current.Request.QueryString["TmplCode"]));
            if (isPrint != null && isPrint != DBNull.Value && isPrint.ToString() != "" && Convert.ToInt32(isPrint) > 0)
            {
                btnList.Add(btnPrint);
            }
            if (string.IsNullOrEmpty(id))
            {
                var defFlow = flowEntities.S_WF_DefFlow.Where(c => c.Code == flowCode).First();
                if (defFlow != null && defFlow.isFlowChart == "1")
                {
                    var insDefFlow = flowEntities.S_WF_InsDefFlow.Where(c => c.Code == flowCode).OrderByDescending(o => o.ModifyTime).First();
                    if (insDefFlow != null)
                    {
                        btnFlowChart.onclick = string.Format(btnFlowChart.onclick, insDefFlow.ID);
                    }
                    btnList.Add(btnFlowChart);
                }
            }

            if (System.Configuration.ConfigurationManager.AppSettings["Flow_ExportWord"] == "False")
            {
                if (btnList.Contains(btnExport))
                    btnList.Remove(btnExport);
            }
            if (System.Configuration.ConfigurationManager.AppSettings["Flow_ExportPdf"] == "False")
            {
                if (btnList.Contains(btnPdf))
                    btnList.Remove(btnPdf);
            }
            //按钮排序
            btnList = btnList.OrderBy(c => c.sortIndex).ToList();
            return btnList;
        }

        #endregion

        #region 升版

        public string TransferToUpperVersionView()
        {
            string url = "";
            var relateField = HttpContext.Current.Request["RelateField"];
            if (!string.IsNullOrEmpty(relateField) && string.IsNullOrEmpty(HttpContext.Current.Request["ID"]) && string.IsNullOrEmpty(HttpContext.Current.Request["UpperVersion"]))
            {
                var flowCode = HttpContext.Current.Request["FlowCode"];
                SQLHelper flowSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
                DataTable dt = flowSqlHelper.ExecuteDataTable(string.Format("select * from S_WF_DefFlow where Code='{0}'", flowCode));
                if (dt.Rows.Count == 0)
                    throw new FlowException("流程定义不存在：" + flowCode);
                string connName = dt.Rows[0]["ConnName"].ToString();
                string tableName = dt.Rows[0]["TableName"].ToString();

                //流程版本号起始字符
                string FlowVersionNumberStart = "1";
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"]))
                    FlowVersionNumberStart = System.Configuration.ConfigurationManager.AppSettings["FlowVersionNumberStart"];

                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
                string sql = string.Format("select ID,VersionNumber,FlowPhase from {0} with(nolock) where 1=1", tableName);
                foreach (var field in relateField.Split(','))
                {
                    sql += string.Format(" and {0}='{1}'", field, HttpContext.Current.Request[field]);
                }
                sql += " order by ID desc";


                dt = sqlHelper.ExecuteDataTable(sql);

                url = HttpContext.Current.Request.RawUrl;
                if (url.Contains("&_t"))
                    url = url.Substring(0, HttpContext.Current.Request.RawUrl.IndexOf("&_t"));
                if (dt.Rows.Count > 0)
                {
                    var row = dt.AsEnumerable().FirstOrDefault();
                    var versionNumber = HttpContext.Current.Request["VersionNumber"];
                    if (!string.IsNullOrEmpty(versionNumber))
                        row = dt.AsEnumerable().SingleOrDefault(c => c["VersionNumber"].ToString() == versionNumber);
                    if (row == null)
                        throw new FlowException("版本不存在！");

                    url += "&ID=" + row["ID"].ToString();
                    if (url.Contains("&VersionNumber="))
                        url = url.Replace("&VersionNumber=" + HttpContext.Current.Request["VersionNumber"], "&VersionNumber=" + row["VersionNumber"].ToString());
                    else
                        url += "&VersionNumber=" + row["VersionNumber"].ToString();

                    if (url.Contains("&Versions="))
                        url = url.Replace("&Versions=" + HttpContext.Current.Request["Versions"], "&Versions=" + string.Join(",", dt.AsEnumerable().Select(c => c["VersionNumber"].ToString())));
                    else
                        url += "&Versions=" + string.Join(",", dt.AsEnumerable().Select(c => c["VersionNumber"].ToString()));

                    //是否出现升版按钮
                    if (!url.Contains("&AllowUpperVersion") && dt.AsEnumerable().Count(c => c["FlowPhase"].ToString() != "End") == 0)
                        url += "&AllowUpperVersion=true";
                }
                else
                {
                    url += "&ID=" + FormulaHelper.CreateGuid();
                    if (url.Contains("&VersionNumber="))
                        url = url.Replace("&VersionNumber=" + HttpContext.Current.Request["VersionNumber"], "&VersionNumber=" + FlowVersionNumberStart);
                    else
                        url += "&VersionNumber=" + FlowVersionNumberStart;

                    if (url.Contains("&Versions="))
                        url = url.Replace("&Versions=" + HttpContext.Current.Request["Versions"], "&Versions=" + FlowVersionNumberStart);
                    else
                        url += "&Versions=" + FlowVersionNumberStart;
                }
            }
            else
                url = string.Empty;

            return url;
        }

        public JsonResult TransferToUpperVersionData(JsonResult result)
        {
            if (result.Data is Dictionary<string, object>)
            {
                var dic = result.Data as Dictionary<string, object>;
                dic["ID"] = FormulaHelper.CreateGuid();
                dic["FlowPhase"] = "";
                dic["VersionNumber"] = "";
            }
            else if (result.Data is BaseModel)
            {
                Type t = result.Data.GetType();
                PropertyInfo pi = t.GetProperty("ID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (pi != null)
                    pi.SetValue(result.Data, FormulaHelper.CreateGuid(), null);
                pi = t.GetProperty("FlowPhase", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (pi != null)
                    pi.SetValue(result.Data, "", null);
                pi = t.GetProperty("VersionNumber", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (pi != null)
                    pi.SetValue(result.Data, "", null);
            }
            else if (result.Data is DataRow)
            {
                DataRow row = result.Data as DataRow;
                row["ID"] = FormulaHelper.CreateGuid();
                row["FlowPhase"] = "";
                row["VersionNumber"] = "";
            }
            else if (result.Data is string)
            {
                string str = result.Data as string;
                if (str.StartsWith("{"))
                {
                    var dic = JsonHelper.ToObject(str);
                    dic["ID"] = FormulaHelper.CreateGuid();
                    dic["FlowPhase"] = "";
                    dic["VersionNumber"] = "";
                    result.Data = JsonHelper.ToJson(dic);
                }
            }
            return result;
        }

        #endregion

        public object GetBusLeftTaskList(string id, string taskExecID)
        {
            string flowCode = GetFlowCode();
            if (!string.IsNullOrEmpty(taskExecID))
            {

                var taskExec = flowEntities.Set<S_WF_InsTaskExec>().Where(c => c.ID == taskExecID).SingleOrDefault();

                var obj = taskExec.S_WF_InsFlow.S_WF_InsTask.Where(c => c.S_WF_InsTaskExec.Count > 0).OrderBy(c => c.CreateTime)
                      .Select(c => new
                      {
                          Name = c.S_WF_InsDefStep.Name
                          ,
                          exeName = c.TaskUserNames
                          ,
                          ownerName = c.TaskUserNames
                          ,
                          BeginDate = c.CreateTime == null ? "" : ((DateTime)c.CreateTime).ToString("yyyy-MM-dd HH:mm")
                          ,
                          EndDate = c.CompleteTime == null ? "" : ((DateTime)c.CompleteTime).ToString("yyyy-MM-dd HH:mm")
                          ,
                          advice = c.S_WF_InsTaskExec.First().ExecComment
                      });
                return obj;
            }
            return "";

        }

    }

}
