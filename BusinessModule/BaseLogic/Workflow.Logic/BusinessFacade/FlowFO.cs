using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Formula.Helper;
using System.Text.RegularExpressions;
using Config;
using Config.Logic;
using System.Data;
using Formula.Exceptions;
using Workflow.Logic.Domain;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Collections;
using Newtonsoft.Json;
using System.Reflection;
using System.ComponentModel;

namespace Workflow.Logic.BusinessFacade
{
    public class FlowFO
    {
        WorkflowEntities entities = FormulaHelper.GetEntities<WorkflowEntities>();

        #region 删除流程

        public void DeleteFlowByFormInstanceID(string formInstanceID)
        {
            try
            {
                if (string.IsNullOrEmpty(formInstanceID))
                    throw new FlowException("formInstanceID参数不能为空!");
                SQLHelper sqlHelper = null;
                var flow = entities.Set<S_WF_InsFlow>().SingleOrDefault(c => c.FormInstanceID == formInstanceID);
                if (flow == null) return;
                foreach (var step in flow.S_WF_InsDefFlow.S_WF_InsDefStep.Where(c => !string.IsNullOrEmpty(c.SubFormID)).ToArray())
                {
                    var subForm = entities.Set<S_WF_DefSubForm>().SingleOrDefault(c => c.ID == step.SubFormID);
                    sqlHelper = SQLHelper.CreateSqlHelper(subForm.ConnName);
                    string sql = string.Format("delete from {0} where ID='{1}'", subForm.TableName, formInstanceID);
                    sqlHelper.ExecuteNonQuery(sql);
                }
                sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
                var taskexecs = entities.Set<S_WF_InsTaskExec>().Where(x => x.InsFlowID == flow.ID && x.ExecTime == null).ToList();
                foreach (var item in taskexecs)
                {
                    item.Delete(item.S_WF_InsTask, item.S_WF_InsFlow, item.S_WF_InsTask.S_WF_InsDefStep.DefStepID, item.S_WF_InsFlow.S_WF_InsDefFlow.Code);

                }
                //sqlHelper.ExecuteDataTable(string.Format("select ID from S_WF_INSTASKEXEC WHERE flowid='{0}' and exectime<>null",flow.ID));
                sqlHelper.ExecuteNonQuery(string.Format("delete from S_WF_InsFlow where FormInstanceID='{0}'", formInstanceID));

            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        #endregion

        #region 启动流程

        public S_WF_InsTaskExec StartFlow(string flowCode, string formInstanceID, string userIds, string userNames)
        {
            try
            {
                if (string.IsNullOrEmpty(userIds))
                    throw new FlowException("启动流程参数UserID不能为空！");

                int maxReceiverCount = 50;
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Flow_ReceiverCount"]))
                    maxReceiverCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Flow_ReceiverCount"]);

                if (userIds.Split(',').Length > maxReceiverCount)
                    throw new FlowException("任务执行人不能超过" + maxReceiverCount + "个");

                flowCode = flowCode.Trim();

                S_WF_InsDefFlow insDefFlow = null;

                //获取流程定义
                var defFlow = entities.Set<S_WF_DefFlow>().Where(c => c.Code == flowCode).SingleOrDefault();
                if (defFlow == null)
                {
                    insDefFlow = entities.S_WF_InsDefFlow.SingleOrDefault(c => c.Code == flowCode && c.IsFreeFlow == "1");
                    if (insDefFlow == null)
                        throw new FlowException(string.Format("编号为：{0}的流程不存在", flowCode));
                }
                else
                {
                    //获取流程定义实例
                    insDefFlow = defFlow.GetInsDefFlow();
                }

                //创建流程实例
                var user = FormulaHelper.GetUserInfo();//启动流程时
                var insFlow = insDefFlow.CreateFlow(formInstanceID, user.UserID, user.UserName);

                //获取流程定义实例首环节
                string Inital = StepTaskType.Inital.ToString();
                var insStep = insDefFlow.S_WF_InsDefStep.Where(c => c.Type == Inital).SingleOrDefault();

                //创建首环节任务
                var insTask = insStep.CreateTask(insFlow, null, null, userIds, userNames, "", "", "");
                //创建任务执行明细
                var taskExecs = insTask.CreateTaskExec();

                entities.SaveChanges();

                return taskExecs.FirstOrDefault();
            }
            catch (Exception ex)
            {
                string info = string.Format(@"启动流程方法StartFlow,参数(flowCode:{0},formInstanceID:{1}, userIds:{2}, userNames:{3})<br>
                    出错原因分析:流程定义首环节不存在<br>
                        ", flowCode, formInstanceID, userIds, userNames);
                throw new FlowException(info, ex);
            }

        }

        #endregion

        #region 执行任务

        public bool ExecTask(string taskExecID, string routingCode, string nextExecUserIDs, string nextExecUserNames, string execComment = "")
        {
            nextExecUserIDs = nextExecUserIDs.Trim(' ', ',');
            nextExecUserNames = nextExecUserNames.Trim(' ', ',');

            var taskExec = entities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
            var routing = taskExec.S_WF_InsTask.S_WF_InsDefStep.S_WF_InsDefRouting.SingleOrDefault(c => c.Code == routingCode);
            return ExecTask(taskExecID, routing.ID, nextExecUserIDs, nextExecUserNames, "", "", "", execComment, "");
        }

        /// <summary>
        /// 执行任务，如果流程结束则返回true
        /// </summary>
        /// <param name="taskExecID"></param>
        /// <param name="routingID"></param>
        /// <param name="nextExecUserIDs"></param>
        /// <param name="nextExecUserNames"></param>
        /// <param name="nextExecUserIDsGroup"></param>
        /// <param name="execComment"></param>
        /// <returns></returns>
        public bool ExecTask(string taskExecID, string routingID, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment, string allBranchRoutingIDs = "")
        {
            try
            {
                nextExecUserIDs = nextExecUserIDs.Trim(' ', ',');
                nextExecUserNames = nextExecUserNames.Trim(' ', ',');

                int maxReceiverCount = 50;
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Flow_ReceiverCount"]))
                    maxReceiverCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Flow_ReceiverCount"]);
                if (nextExecUserIDs.Split(',').Length > maxReceiverCount)
                    throw new FlowException("任务执行人不能超过" + maxReceiverCount + "个");

                if (allBranchRoutingIDs == "") //所有分支路由的ID，用于判断当前任务是否要完成。（例如5个分支路由，根据条件出现3个，那么3个）
                    allBranchRoutingIDs = routingID;

                bool boolFlowComplete = false;


                List<S_WF_InsTask> listNextTask = new List<S_WF_InsTask>();

                var taskExec = entities.Set<S_WF_InsTaskExec>().Where(c => c.ID == taskExecID).SingleOrDefault();
                if (taskExec == null)
                    throw new FlowException("任务已经不存在！");
                if (taskExec.ExecTime != null)
                    throw new FlowException("任务已经执行过！");

                ////增加是否移动端执行任务标记
                if (HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request["IsMobileRequest"]))
                    taskExec.ApprovalInMobile = HttpContext.Current.Request["IsMobileRequest"];
                else
                    taskExec.ApprovalInMobile = "0";

                UserInfo user = FormulaHelper.GetUserInfoByID(taskExec.ExecUserID);

                var routing = entities.S_WF_InsDefRouting.Where(c => c.ID == routingID).SingleOrDefault();

                if (routing.InsDefFlowID != taskExec.S_WF_InsFlow.InsDefFlowID)
                    throw new FlowException("流程定义版本已经发生变化！");

                var task = taskExec.S_WF_InsTask;
                var step = routing.S_WF_InsDefStep;
                var flow = task.S_WF_InsFlow;


                //签字
                routing.Signature(taskExec, execComment);
                //设置表单数据
                routing.SetFormData(flow.FormInstanceID, taskExec, execComment, nextExecUserIDs, nextExecUserNames);
                //执行taskExec
                taskExec.Execute(routing, execComment, allBranchRoutingIDs);

                bool distributeTask = false;
                bool completeTask = false;
                string sendTaskUserID = user.UserID; //下一步任务发送人

                #region 判断是否完成任务和是否分发下一步任务

                RoutingType routingType = (RoutingType)Enum.Parse(typeof(RoutingType), routing.Type);
                TaskCooperationMode cooperationType = (TaskCooperationMode)Enum.Parse(typeof(TaskCooperationMode), step.CooperationMode);

                switch (routingType)
                {
                    case RoutingType.Back:  //打回操作，无论是否协作完成，直接通过
                        distributeTask = true;
                        completeTask = true;
                        sendTaskUserID = user.UserID;
                        break;
                    case RoutingType.Normal:
                        switch (cooperationType)
                        {
                            case TaskCooperationMode.Single:
                                distributeTask = true;
                                completeTask = true;
                                sendTaskUserID = user.UserID;
                                break;
                            case TaskCooperationMode.All:
                                bool flag = task.S_WF_InsTaskExec.Where(c => c.ExecTime == null && c.ID != taskExecID && (c.Type == TaskExecType.Normal.ToString() || c.Type == TaskExecType.Delegate.ToString())).Count() == 0;
                                distributeTask = flag;
                                completeTask = flag;
                                sendTaskUserID = "";
                                break;
                            case TaskCooperationMode.GroupSingle:
                                List<Dictionary<string, string>> listDic = JsonHelper.ToObject<List<Dictionary<string, string>>>(task.TaskUserIDsGroup);
                                foreach (var item in task.S_WF_InsTaskExec.Where(c => c.ExecTime != null))
                                {
                                    var groupNames = listDic.Where(c => c["UserID"] == item.TaskUserID).Select(c => c["GroupName"]).ToArray();
                                    listDic.RemoveWhere(c => groupNames.Contains(c["GroupName"]));
                                }
                                distributeTask = listDic.Count == 0;
                                completeTask = listDic.Count == 0;
                                sendTaskUserID = user.UserID;
                                break;
                        }
                        break;
                    case RoutingType.Branch:
                    case RoutingType.SingleBranch:
                        switch (cooperationType)
                        {
                            case TaskCooperationMode.Single:
                                distributeTask = true;
                                completeTask = taskExec.ExecTime != null; // 有执行时间，说明taskExec已经完成
                                sendTaskUserID = user.UserID;
                                break;
                            case TaskCooperationMode.All:
                                //throw new FlowException("分支路由只支持单人通过");
                                bool flag = task.S_WF_InsTaskExec.Where(c => c.ExecTime == null && c.ID != taskExecID && (c.Type == TaskExecType.Normal.ToString() || c.Type == TaskExecType.Delegate.ToString())).Count() == 0;
                                distributeTask = flag;
                                completeTask = (taskExec.ExecTime != null) && flag;
                                sendTaskUserID = "";
                                break;
                            case TaskCooperationMode.GroupSingle:
                                throw new FlowException("组单人通过只支持普通路由类型");
                        }
                        break;
                    case RoutingType.Weak:
                        switch (cooperationType)
                        {
                            case TaskCooperationMode.Single:
                                distributeTask = true;
                                completeTask = false;
                                sendTaskUserID = user.UserID;
                                break;
                            case TaskCooperationMode.All:
                                distributeTask = true;
                                completeTask = false;
                                sendTaskUserID = user.UserID;
                                break;
                            case TaskCooperationMode.GroupSingle:
                                throw new FlowException("组单人通过只支持普通路由类型");
                        }
                        break;
                    case RoutingType.AntiWeak:
                        switch (cooperationType)
                        {
                            case TaskCooperationMode.Single:
                                distributeTask = false;
                                completeTask = true;
                                sendTaskUserID = user.UserID;
                                break;
                            case TaskCooperationMode.All:
                                //throw new FlowException("弱控完成路由只支持单人通过");
                                bool flag = task.S_WF_InsTaskExec.Where(c => c.ExecTime == null && c.ID != taskExecID && (c.Type == TaskExecType.Normal.ToString() || c.Type == TaskExecType.Delegate.ToString())).Count() == 0;
                                distributeTask = false;
                                completeTask = flag;
                                sendTaskUserID = "";
                                break;
                            case TaskCooperationMode.GroupSingle:
                                throw new FlowException("组单人通过只支持普通路由类型");
                        }
                        break;
                    case RoutingType.Distribute:
                        switch (cooperationType)
                        {
                            case TaskCooperationMode.Single:
                                throw new FlowException("单人通过环节不需要分发");
                            case TaskCooperationMode.All:
                                distributeTask = true;
                                bool flag = task.S_WF_InsTaskExec.Where(c => c.ExecTime == null && c.ID != taskExecID).Count() == 0;
                                completeTask = flag;
                                sendTaskUserID = user.UserID;
                                break;
                            case TaskCooperationMode.GroupSingle:
                                throw new FlowException("组单人通过只支持普通路由类型");
                        }
                        break;
                }

                #endregion

                //单步回收，撤销其它任务,完成本任务，分发下一步任务
                if (routingType == RoutingType.WithdrawOther)
                {
                    distributeTask = true;
                    completeTask = true;
                    foreach (var t in flow.S_WF_InsTask.ToList())
                    {
                        if (t.ID == task.ID)
                            continue;
                        if (t.Status == FlowTaskStatus.Processing.ToString()) //移除已他进行中任务
                        {
                            t.Status = FlowTaskStatus.Complete.ToString();
                            t.CompleteTime = DateTime.Now;
                            foreach (var exec in t.S_WF_InsTaskExec)
                            {
                                if (exec.ExecTime == null)
                                {
                                    exec.ExecTime = DateTime.Now;
                                    exec.ExecUserID = "system";
                                    exec.ExecUserName = "系统";
                                    exec.ExecComment = "系统回收";
                                }
                            }
                        }
                    }
                }
                S_WF_InsDefStep nextStep = null;
                S_WF_InsTask nextTask = null;
                if (distributeTask == true)
                {
                    //保存表单数据版本
                    if (routing.SaveFormVersion == "1")
                        routing.SaveFormDataVersion(task);
                    //保存变量版本
                    routing.SaveFlowVariableVersion(task);
                    //设置流程变量
                    routing.SetFlowVariable(task.S_WF_InsFlow);

                    //查询下一环节
                    nextStep = entities.S_WF_InsDefStep.Where(c => c.ID == routing.EndID).SingleOrDefault();

                    #region 执行人为空跳转到下一环节
                    if (nextStep == null)
                        throw new FlowException("流程定义找不到环节,请检查流程定义！");

                    while (string.IsNullOrEmpty(nextExecUserIDs) && !string.IsNullOrEmpty(nextStep.EmptyToStep)) //if改为while实现连续跳转环节
                    {
                        var nextNextStep = entities.S_WF_InsDefStep.SingleOrDefault(c => c.ID == nextStep.EmptyToStep);
                        var nextRouting = entities.S_WF_InsDefRouting.FirstOrDefault(c => c.EndID == nextNextStep.ID);
                        var nextNextRoutingParams = GetRoutingParams(nextRouting, taskExec, flow.FormInstanceID);

                        //如果选人，则不能跳转
                        if (string.IsNullOrEmpty(nextNextRoutingParams.userIDs) && !string.IsNullOrEmpty(nextRouting.SelectMode))
                            break;

                        //替换执行的路由、环节、执行人等
                        var userService = FormulaHelper.GetService<IUserService>();
                        routing = nextRouting;
                        nextStep = nextNextStep;
                        nextExecUserIDs = nextNextRoutingParams.userIDs;
                        nextExecUserNames = userService.GetUserNames(nextExecUserIDs);
                        nextExecUserIDsGroup = nextNextRoutingParams.userIDsGroup == null || nextNextRoutingParams.userIDsGroup.Count == 0
                        ? "" : JsonHelper.ToJson(nextNextRoutingParams.userIDsGroup);
                        nextExecRoleIDs = nextNextRoutingParams.roleIDs;
                        nextExecOrgIDs = nextNextRoutingParams.orgIDs;
                    }

                    #endregion
                    //创建下一环节任务
                    nextTask = nextStep.CreateTask(task.S_WF_InsFlow, task, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs);

                    if (nextTask == null)
                        throw new FlowException("流程定义找不到下一环节任务,请检查流程定义！");

                    //创建任务执行明细
                    nextTask.CreateTaskExec();

                    //2019.12.6 增加等待环节设置在结束环节上的特殊情况
                    if ((nextTask.WaitingSteps == "" || nextTask.WaitingSteps == step.ID) && nextTask.WaitingRoutings == "")
                    {
                        if (nextStep.Type == StepTaskType.Completion.ToString() && routing.Type != RoutingType.AntiWeak.ToString())
                        {
                            if (flow.S_WF_InsTask.Count(c => c.InsDefStepID == task.InsDefStepID && c.ID != task.ID && c.Status == "Processing") == 0) //按人分发的情况会大于零
                            {
                                //流程结束
                                flow.Status = FlowTaskStatus.Complete.ToString();
                                flow.CompleteTime = DateTime.Now;
                                boolFlowComplete = true;

                                //计算耗时
                                var canlendarService = FormulaHelper.GetService<ICalendarService>();
                                flow.TimeConsuming = canlendarService.GetWorkHourCount((DateTime)flow.CreateTime, (DateTime)flow.CompleteTime);

                                //完成全部任务
                                foreach (var item in flow.S_WF_InsTask)
                                {
                                    item.Complete();
                                }
                            }
                        }
                    }
                }

                //完成任务
                if (completeTask == true)
                {
                    task.Complete();
                    //发送消息
                    routing.SendMsg(taskExec, execComment, nextExecUserIDs, nextExecUserNames);
                    //给发起人发消息

                    //流程结束后给申请人发消息
                    if (System.Configuration.ConfigurationManager.AppSettings["Flow_SendMsgToInitiator"] == "True")
                    {
                        var initiator = flow.S_WF_InsTask.FirstOrDefault(c => c.Type == "Inital").TaskUserIDs;//发起人ID
                        var initiatorName = flow.S_WF_InsTask.FirstOrDefault(c => c.Type == "Inital").TaskUserNames;

                        string content = string.Format("您的审批单已进入下一环节：{0}", task.TaskName);
                        string link = "";//不带连接了......
                        FormulaHelper.GetService<IMessageService>().SendMsg(content, content, link, "", initiator, initiatorName, null, MsgReceiverType.UserType, MsgType.Normal);
                    }


                }

                //设置表单流程阶段
                string completeStepName = task.S_WF_InsDefStep.Name; //当前完成任务的环节
                if (completeTask == false)
                    completeStepName = "";
                if (completeTask && nextTask != null && nextTask.WaitingSteps == "" && nextTask.WaitingRoutings == "")
                {
                    routing.SetFormPhase(flow, nextStep, completeStepName);
                }
                else
                {
                    routing.SetFormPhase(flow, null, completeStepName);
                }

                entities.SaveChanges();

                return boolFlowComplete;

            }
            catch (Exception ex)
            {
                string info = string.Format(@"执行任务ExecTask,参数(
                        taskExecID:{0}<br> 
                        routingID:{1}<br>
                        nextExecUserIDs:{2}<br> 
                        nextExecUserNames:{3}<br>
                        nextExecUserIDsGroup:{4}<br>
                        nextExecRoleIDs:{5}<br> 
                        nextExecOrgIDs:{6}<br> 
                        execComment:{7}<br>
                        )<br>
                        ", taskExecID, routingID, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
                throw new FlowException(info, ex);
            }

        }

        #endregion

        #region 撤销执行
        //判断是否可撤销
        public bool AllowWithDraw(S_WF_InsTaskExec taskExec)
        {
            var flow = taskExec.S_WF_InsFlow;
            var task = taskExec.S_WF_InsTask;

            if (flow.Status == FlowTaskStatus.Complete.ToString())
                return false;

            if (task.Status != FlowTaskStatus.Complete.ToString())
                return false;

            if (taskExec == null)
                throw new FlowException("当前流程没有找到对应执行的任务，请检查流程是否已修改过!");

            #region 调整为可逐级打回 20140929；

            if (string.IsNullOrEmpty(taskExec.ExecRoutingIDs))
                return false;

            #endregion

            string[] insRoutings = taskExec.ExecRoutingIDs.Split(',');
            var routins = entities.Set<S_WF_InsDefRouting>().Where(c => insRoutings.Contains(c.ID)).ToArray();

            foreach (var routing in routins)
            {
                if (routing.AllowWithdraw != "1")
                    return false;
            }

            foreach (var nextTask in flow.S_WF_InsTask.Where(c => c.SendTaskIDs.Contains(task.ID)).ToArray())
            {
                if (nextTask.Status != FlowTaskStatus.Processing.ToString())
                    return false;

                //if (nextTask.SendTaskIDs == task.ID)
                //{

                //}
                //else
                //{
                //    return false;
                //}
            }
            return true;
        }

        public void UnExecTask(string taskExecID)
        {
            try
            {
                var taskExec = entities.Set<S_WF_InsTaskExec>().Where(c => c.ID == taskExecID).SingleOrDefault();

                var flow = taskExec.S_WF_InsFlow;
                var task = taskExec.S_WF_InsTask;

                if (flow.Status == FlowTaskStatus.Complete.ToString())
                    throw new FlowException("流程结束，不能撤销！");

                if (task.Status != FlowTaskStatus.Complete.ToString())
                    throw new FlowException("只有已完成的任务能撤销！");

                if (string.IsNullOrEmpty(taskExec.ExecRoutingIDs))
                    throw new FlowException("该任务执行时，没有路由ExecRoutingIDs,不能撤销！");

                string[] insRoutings = taskExec.ExecRoutingIDs.Split(',');
                var routins = entities.Set<S_WF_InsDefRouting>().Where(c => insRoutings.Contains(c.ID)).ToArray();

                foreach (var routing in routins)
                {
                    if (routing.AllowWithdraw != "1")
                        throw new FlowException(string.Format("{0}操作不能撤销！", routing.Name));
                }

                foreach (var nextTask in flow.S_WF_InsTask.Where(c => c.SendTaskIDs.Contains(task.ID)).ToArray())
                {
                    if (nextTask.Status != FlowTaskStatus.Processing.ToString())
                        throw new FlowException("下一步任务不在过程中,任务不能撤销");

                    var r = routins.SingleOrDefault(c => c.EndID == nextTask.InsDefStepID);
                    if (r == null)
                        continue;
                    if (r.Type == RoutingType.Branch.ToString())
                    {
                        nextTask.WaitingRoutings = StringHelper.Include(nextTask.WaitingRoutings, r.ID);
                    }
                    else
                    {
                        //移除任务前先移除等待
                        foreach (var item in flow.S_WF_InsTask.Where(c => c.CompleteTime == null && c.ID != nextTask.ID))
                        {
                            if (item.WaitingSteps.Contains(nextTask.InsDefStepID))
                            {
                                item.WaitingSteps = StringHelper.Exclude(item.WaitingSteps, nextTask.InsDefStepID);
                                //同步接口
                                if (string.IsNullOrEmpty(item.WaitingRoutings) && string.IsNullOrEmpty(item.WaitingSteps))
                                    foreach (var obj in item.S_WF_InsTaskExec)
                                        obj.Create(item, item.S_WF_InsFlow, item.S_WF_InsDefStep.DefStepID, item.S_WF_InsFlow.S_WF_InsDefFlow.Code);

                            }
                        }
                        //调用任务明细的删除方法
                        var execList = nextTask.S_WF_InsTaskExec.ToList();
                        foreach (var item in execList)
                        {
                            entities.Set<S_WF_InsTaskExec>().Remove(item);
                            //entities.Entry(item).State = EntityState.Deleted;
                            item.Delete(task, flow, task.S_WF_InsDefStep.DefStepID, flow.S_WF_InsDefFlow.Code);//同步接口用
                        }
                        entities.Set<S_WF_InsTask>().Remove(nextTask);  //删除下一环节任务
                    }
                }
                //流程中的最新环节修改为撤回的环节
                flow.CurrentStep = task.S_WF_InsDefStep.Name;
                //本环节任务改为执行中
                taskExec.ExecTime = null;
                taskExec.ExecComment = "";
                taskExec.ExecRoutingIDs = "";
                taskExec.Create(task, task.S_WF_InsFlow, task.S_WF_InsDefStep.DefStepID, task.S_WF_InsFlow.S_WF_InsDefFlow.Code);//当作新任务，同步接口

                task.Status = FlowTaskStatus.Processing.ToString();
                task.CompleteTime = null;

                //恢复任务后恢复等待
                foreach (var item in flow.S_WF_InsTask.Where(c => c.CompleteTime == null && c.ID != task.ID))
                {
                    if (item.S_WF_InsDefStep.WaitingStepIDs.Contains(task.InsDefStepID))
                    {
                        item.WaitingSteps = StringHelper.Include(item.WaitingSteps, task.InsDefStepID);
                    }
                }


                //恢复流程变量
                if (!string.IsNullOrEmpty(task.VariableVersion))
                {
                    List<Dictionary<string, object>> listDic = JsonHelper.ToObject<List<Dictionary<string, object>>>(task.VariableVersion);

                    foreach (var v in flow.S_WF_InsVariable)
                    {
                        foreach (var routing in routins)
                        {
                            if (routing.VariableSet.Split(',', '=').Contains(v.VariableName))
                            {
                                var varItem = listDic.SingleOrDefault(c => c["VariableName"].ToString() == v.VariableName);
                                if (varItem != null)
                                    v.VariableValue = varItem["VariableValue"].ToString();
                                else
                                    v.VariableValue = "";
                                continue;
                            }
                        }
                    }
                }

                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }


        #endregion

        #region 打回任务

        //参数routingId放弃使用
        public void FlowDoBack(string taskExecID, string routingId, string execComment)
        {
            try
            {
                var taskExec = entities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
                var task = taskExec.S_WF_InsTask;
                var flow = taskExec.S_WF_InsFlow;
                var sendTask = entities.Set<S_WF_InsTask>().SingleOrDefault(c => c.ID == taskExec.S_WF_InsTask.SendTaskIDs);
                var sendStep = sendTask.S_WF_InsDefStep;


                //完成任务
                var user = FormulaHelper.GetUserInfoByID(taskExec.ExecUserID);
                taskExec.ExecTime = DateTime.Now;
                taskExec.ExecRoutingIDs = "";
                taskExec.ExecUserID = user.UserID;
                taskExec.ExecUserName = user.UserName;
                taskExec.ExecComment = execComment;
                taskExec.Complete(taskExec.S_WF_InsTask, taskExec.S_WF_InsFlow, taskExec.S_WF_InsTask.S_WF_InsDefStep.DefStepID, taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code);
                ////增加是否移动端执行任务标记
                if (HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request["IsMobileRequest"]))
                    taskExec.ApprovalInMobile = HttpContext.Current.Request["IsMobileRequest"];
                else
                    taskExec.ApprovalInMobile = "0";
                task.Complete();

                //产生新任务
                var resultTask = sendStep.CreateTask(flow, task, null, task.SendTaskUserIDs, task.SendTaskUserNames, null, null, null);
                #region 调整为可逐级打回 20140929；   【VariableVersion】如何处理，未知
                //  撤销按钮判断：FormController.cs文件的JsonGetFlowButtons方法【如何处理，未知】
                //       
                //  撤销按钮判断：FlowFO.AllowWithDraw(taskExec)
                //       if (string.IsNullOrEmpty(taskExec.ExecRoutingIDs))     return false;
                //  撤销动作：    FlowFO.UnExecTask(taskExecID)
                //       if (string.IsNullOrEmpty(taskExec.ExecRoutingIDs))   throw new FlowException("该任务执行时，没有路由ExecRoutingIDs,不能撤销！");
                //       
                //       foreach (var nextTask in flow.S_WF_InsTask.Where(c => c.SendTaskIDs.Contains(task.ID)).ToArray())
                //          1、因为逐级打回，本任务没有下一任务，应抛出异常；【优化：若taskExec.ExecRoutingIDs为空，抛出异常】
                //          2、若本任务产生多个下一级任务，部分任务已执行、部分未执行，将有部分子任务被撤销、而本级任务未撤销，任务挂起；

                resultTask.SendTaskIDs = task.ID;
                resultTask.SendTaskUserIDs = task.TaskUserIDs;
                resultTask.SendTaskUserNames = task.TaskUserNames;
                //resultTask.DoBackRoutingID = task.DoBackRoutingID;
                //resultTask.OnlyDoBack = task.OnlyDoBack;

                #endregion
                resultTask.CreateTaskExec();
                //打回任务名称增加标识
                resultTask.TaskName = resultTask.TaskName;

                //打回时重写当前环节
                flow.CurrentStep = sendStep.Name;
                flow.CurrentUserNames = resultTask.TaskUserNames;

                entities.SaveChanges();

                if (string.IsNullOrEmpty(task.DoBackRoutingID))
                {
                    var routing = entities.Set<S_WF_InsDefRouting>().SingleOrDefault(c => c.InsDefStepID == sendStep.ID && c.EndID == task.S_WF_InsDefStep.ID);
                    routing.SetFormPhase(flow, sendStep, task.S_WF_InsDefStep.Name);
                }
                //打回签字
                DoBackSign(taskExec, execComment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion

        #region 驳回到回首环节

        public void FlowDoBackFirst(string taskExecID, string execComment)
        {
            try
            {
                var taskExec = entities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
                var task = taskExec.S_WF_InsTask;
                var flow = taskExec.S_WF_InsFlow;
                string initStepType = StepTaskType.Inital.ToString();

                var sendStep = flow.S_WF_InsDefFlow.S_WF_InsDefStep.SingleOrDefault(c => c.Type == initStepType);
                var sendTask = flow.S_WF_InsTask.LastOrDefault(c => c.InsDefStepID == sendStep.ID);


                //完成任务
                var user = FormulaHelper.GetUserInfoByID(taskExec.ExecUserID);
                taskExec.ExecTime = DateTime.Now;
                taskExec.ExecRoutingIDs = "";
                taskExec.ExecRoutingName = "驳回首环节";
                taskExec.ExecUserID = user.UserID;
                taskExec.ExecUserName = user.UserName;
                taskExec.ExecComment = execComment;
                ////增加是否移动端执行任务标记
                if (HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request["IsMobileRequest"]))
                    taskExec.ApprovalInMobile = HttpContext.Current.Request["IsMobileRequest"];
                else
                    taskExec.ApprovalInMobile = "0";
                taskExec.Complete(taskExec.S_WF_InsTask, taskExec.S_WF_InsFlow, taskExec.S_WF_InsTask.S_WF_InsDefStep.DefStepID, taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code);

                task.Complete();

                if (task.S_WF_InsDefStep.AllowDoBackFirst != task.S_WF_InsDefStep.AllowDoBackFirstReturn)//打回直送时不要撤销其他任务 20190911
                {
                    //撤销进行中任务
                    foreach (var t in flow.S_WF_InsTask.ToList())
                    {
                        if (t.ID == task.ID)
                            continue;
                        if (t.Status == FlowTaskStatus.Processing.ToString()) //移除已他进行中任务
                        {
                            t.Status = FlowTaskStatus.Complete.ToString();
                            t.CompleteTime = DateTime.Now;
                            foreach (var exec in t.S_WF_InsTaskExec)
                            {
                                if (exec.ExecTime == null)
                                {
                                    exec.ExecTime = DateTime.Now;
                                    exec.ExecUserID = "system";
                                    exec.ExecUserName = "系统";
                                    exec.ExecComment = "系统回收";
                                }
                                exec.Delete(task, task.S_WF_InsFlow, task.S_WF_InsDefStep.DefStepID, task.S_WF_InsFlow.S_WF_InsDefFlow.Code);//同步接口用
                            }
                        }
                    }
                }

                //产生新任务
                var resultTask = sendStep.CreateTask(flow, task, null, sendTask.TaskUserIDs, sendTask.TaskUserNames, sendTask.TaskUserIDsGroup, sendTask.TaskRoleIDs, sendTask.TaskOrgIDs);
                resultTask.CreateTaskExec();

                //打回任务名字加个标识
                resultTask.TaskName = "驳回-" + resultTask.TaskName;

                //打回时重写当前环节
                flow.CurrentStep = sendStep.Name;
                flow.CurrentUserNames = resultTask.TaskUserNames;

                entities.SaveChanges();

                //打回签字
                DoBackSign(taskExec, execComment);

                #region 控制表单的FlowPhase和StepName

                string connName = flow.S_WF_InsDefFlow.ConnName;
                string tableName = flow.S_WF_InsDefFlow.TableName;
                string formInstanceID = flow.FormInstanceID;

                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);


                string sql = string.Format("select * from {0} where ID='{1}'", tableName, formInstanceID);
                DataTable dt = sqlHelper.ExecuteDataTable(sql);
                sql = "";

                if (dt.Columns.Contains("FlowPhase") || dt.Columns.Contains("FLOWPHASE")) //FLOWPHASE为了兼容Oracle
                {
                    var sqlUpdate = string.Format(" update {0} set FlowPhase='{1}' where ID='{2}'", tableName, sendStep.Phase, formInstanceID);
                    sqlHelper.ExecuteNonQuery(sqlUpdate);
                }
                if (dt.Columns.Contains("StepName") || dt.Columns.Contains("STEPNAME"))   //STEPNAME为了兼容Oracle
                {
                    var sqlUpdate = string.Format(" update {0} set StepName='{1}' where ID='{2}'", tableName, sendStep.Name, formInstanceID);
                    sqlHelper.ExecuteNonQuery(sqlUpdate);
                }

                #endregion
            }
            catch (Exception ex)
            {
                string info = string.Format(@"驳回到回首环节方法FlowDoBackFirst,参数(taskExecID:{0},execComment:{1})<br>
                    出错原因分析:<br>
                        1.taskExecID不能为空<br>
                        2.当前任务不存在或已执行过<br>
                        ", taskExecID, execComment);
                throw new FlowException(info, ex);
            }
        }

        private void DoBackSign(S_WF_InsTaskExec taskExec, string execComment)
        {
            try
            {
                string signField = taskExec.S_WF_InsTask.S_WF_InsDefStep.DoBackSignField;

                if (string.IsNullOrEmpty(signField))
                    return;

                SignatureObj signatureObj = new SignatureObj();
                UserInfo user = FormulaHelper.GetUserInfoByID(taskExec.ExecUserID);

                string formData = HttpContext.Current.Request["FormData"];
                string connName = taskExec.S_WF_InsFlow.S_WF_InsDefFlow.ConnName;
                string tableName = taskExec.S_WF_InsFlow.S_WF_InsDefFlow.TableName;
                string formInstanceID = taskExec.S_WF_InsTask.S_WF_InsFlow.FormInstanceID;

                //表单字典
                var formDic = S_WF_InsFlow.GetFormDic(formData, connName, tableName, formInstanceID);
                if (!formDic.ContainsKey(signField))
                    throw new FlowException(string.Format("签字字段不存在:{0}", signField));

                List<SignatureObj> list = null;
                string alreadySign = formDic[signField].ToString();
                if (alreadySign != "" && alreadySign.StartsWith("[") == false)
                    throw new FlowException(string.Format("{0}字段的内容不能解析为签字格式！", signField));
                if (!string.IsNullOrEmpty(alreadySign))
                {
                    list = JsonHelper.ToObject<List<SignatureObj>>(alreadySign);
                    //容错错误数据
                    if (list == null)
                        list = new List<SignatureObj>();

                    if (System.Configuration.ConfigurationManager.AppSettings["Flow_SignDistinct"] != "False")
                    {
                        //消除重复签字 
                        list = list.Where(c => c.ExecUserID != user.UserID).ToList();
                    }

                }
                else
                    list = new List<SignatureObj>();


                var userList = new List<Dictionary<string, string>>();
                Dictionary<string, string> userDic = new Dictionary<string, string>();
                userDic.Add("UserID", taskExec.TaskUserID);
                userDic.Add("UserName", taskExec.TaskUserName);
                userDic.Add("GroupName", "");
                userList.Add(userDic);


                foreach (var dic in userList)
                {
                    SignatureObj obj = new SignatureObj();
                    obj.TaskUserGroupName = dic["GroupName"];
                    obj.TaskUserID = dic["UserID"];
                    obj.TaskUserName = dic["UserName"];
                    obj.ExecUserID = user.UserID;
                    obj.ExecUserName = user.UserName;
                    obj.RoutingCode = "";
                    obj.RoutingName = "打回";
                    obj.RoutingValue = "";
                    obj.SignTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    obj.SignComment = execComment;
                    obj.SignatureProtectValue = JsonHelper.ToJson(obj.SignatureProtectField).GetHashCode().ToString();
                    obj.StepCode = taskExec.S_WF_InsTask.S_WF_InsDefStep.Code;
                    obj.StepName = taskExec.S_WF_InsTask.S_WF_InsDefStep.Name;

                    list.Add(obj);

                }

                string strSign = JsonHelper.ToJson<List<SignatureObj>>(list);

                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
                string sql = string.Format("update {0} set {1}='{2}' where ID='{3}'", tableName, signField, strSign, taskExec.S_WF_InsTask.S_WF_InsFlow.FormInstanceID);
                sqlHelper.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        #endregion

        #region 首环节送驳回人

        public void FlowDoBackFirstReturn(string taskExecID, string execComment)
        {
            try
            {
                var taskExec = entities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
                var task = taskExec.S_WF_InsTask;
                var flow = taskExec.S_WF_InsFlow;
                var sendTask = entities.Set<S_WF_InsTask>().SingleOrDefault(c => c.ID == taskExec.S_WF_InsTask.SendTaskIDs);
                var sendStep = sendTask.S_WF_InsDefStep;

                if (sendTask == null)
                    throw new FlowException(string.Format("任务ID为{0}找不到,任务已结束或已执行!", taskExec.S_WF_InsTask.SendTaskIDs));

                var resultTask = sendStep.CreateTask(flow, task, null, sendTask.TaskUserIDs, sendTask.TaskUserNames, sendTask.TaskUserIDsGroup, sendTask.TaskRoleIDs, sendTask.TaskOrgIDs);
                resultTask.CreateTaskExec();

                //完成任务
                var user = FormulaHelper.GetUserInfoByID(taskExec.ExecUserID);
                taskExec.ExecTime = DateTime.Now;
                taskExec.ExecRoutingIDs = "";
                taskExec.ExecRoutingName = "送驳回人";
                taskExec.ExecUserID = user.UserID;
                taskExec.ExecUserName = user.UserName;
                taskExec.ExecComment = execComment;
                ////增加是否移动端执行任务标记
                if (HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request["IsMobileRequest"]))
                    taskExec.ApprovalInMobile = HttpContext.Current.Request["IsMobileRequest"];
                else
                    taskExec.ApprovalInMobile = "0";
                taskExec.Complete(taskExec.S_WF_InsTask, taskExec.S_WF_InsFlow, taskExec.S_WF_InsTask.S_WF_InsDefStep.DefStepID, taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code);

                task.Complete();

                entities.SaveChanges();
                //随便找个routing
                var routing = sendStep.S_WF_InsDefRouting.FirstOrDefault();
                routing.SetFormPhase(flow, sendStep, task.S_WF_InsDefStep.Name);
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        #endregion

        #region 查询任务


        public DataTable GetUserTask(string userID, SearchCondition scnd)
        {
            string sql = @"
select TaskExecID= S_WF_InsTaskExec.ID
,TaskID=S_WF_InsTask.ID
,StepID=S_WF_InsTask.InsDefStepID
,FlowID=S_WF_InsTask.InsFlowID
,TaskType=S_WF_InsTask.Type
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserNames
,[Status]=S_WF_InsTask.[Status]
,CreateTime=S_WF_InsTask.CreateTime
,FormInstanceID
,FlowName
,FlowCategory
,FlowSubCategory
,FormUrl
,FormWidth
,FormHeight
from S_WF_InsFlow 
join S_WF_InsTask on S_WF_InsFlow.Status='Processing' and S_WF_InsFlow.ID=InsFlowID 
join S_WF_InsTaskExec on S_WF_InsTask.Status in ('Processing') and S_WF_InsTask.Type in('Normal','Inital') and S_WF_InsTask.ID=InsTaskID
join S_WF_InsDefFlow on InsDefFlowID=S_WF_InsDefFlow.ID
where (S_WF_InsFlow.CreateTime between '{0}' and '{1}') and  S_WF_InsTaskExec.ExecUserID='{2}'  and S_WF_InsTaskExec.ExecTime is null
and WaitingRoutings='' and WaitingSteps=''
";

            DateTime startTime = DateTime.Parse(string.Format("{0}-{1}-{2}", DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day));
            sql = string.Format(sql, startTime, DateTime.Now, userID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");

            DataTable dt = sqlHelper.ExecuteDataTable(sql, scnd);

            return dt;
        }

        #endregion

        #region 获取首环节

        public S_WF_InsDefStep GetStartStep(string flowCode)
        {
            try
            {
                //获取流程定义
                var defFlow = entities.Set<S_WF_DefFlow>().Where(c => c.Code == flowCode).FirstOrDefault();

                if (defFlow == null)
                    throw new FlowException(string.Format("编号为：{0}的流程不存在", flowCode));

                //获取流程定义实例
                var insDefFlow = defFlow.GetInsDefFlow();

                string Inital = StepTaskType.Inital.ToString();
                return insDefFlow.S_WF_InsDefStep.Where(c => c.Type == Inital).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }

        #endregion

        #region 获取当前流程所有路由
        public List<S_WF_InsDefRouting> GetRoutingList(string id)
        {
            List<S_WF_InsDefRouting> list = new List<S_WF_InsDefRouting>();
            return entities.S_WF_InsDefRouting.Where(c => c.InsDefFlowID == id).ToList();
        }
        #endregion

        #region 获取路由

        public List<S_WF_InsDefRouting> GetStartRoutingList(string flowCode, Dictionary<string, object> formDic)
        {
            try
            {
                List<S_WF_InsDefRouting> list = new List<S_WF_InsDefRouting>();

                var def = entities.S_WF_DefFlow.Where(c => c.Code == flowCode).OrderByDescending(c => c.ID).FirstOrDefault();

                if (def == null)
                    throw new FlowException(string.Format("编号为：{0}的流程不存在", flowCode));

                var insDef = def.GetInsDefFlow();
                string Inital = StepTaskType.Inital.ToString();
                var step = insDef.S_WF_InsDefStep.Where(c => c.Type == Inital).SingleOrDefault();

                var userService = FormulaHelper.GetService<IUserService>();

                foreach (var routing in step.S_WF_InsDefRouting.OrderBy(c => c.SortIndex))
                {
                    //如果有变量权限限制，则为不符合条件
                    if (!HasStartVariableAuth(insDef, routing.AuthVariable))
                        continue;


                    //根据表单数据过滤                
                    if (!HasFormDataAuth(formDic, routing.AuthFormData, null))
                        continue;

                    //根据Sql权限过滤
                    if (!HasSqlAuth(formDic, routing.AuthFromSql, null))
                        continue;

                    #region 用户过滤

                    UserInfo user = null;
                    if (!string.IsNullOrEmpty(routing.AuthTargetUser)) //指定目标用户为表单字段
                    {
                        string authTargetUser = routing.AuthTargetUser.Trim('{', '}');
                        if (formDic.Count() > 0 && formDic.ContainsKey(authTargetUser))
                            user = FormulaHelper.GetUserInfoByID(formDic[authTargetUser].ToString());
                        if (user == null)
                            continue;
                    }
                    else
                    {
                        user = FormulaHelper.GetUserInfo();//启动流程前
                    }

                    //指定用户过滤
                    if (!string.IsNullOrEmpty(routing.AuthUserIDs))
                    {
                        if (!routing.AuthUserIDs.Split(',').Contains(user.UserID))
                            continue;
                    }

                    //根据组织、角色过滤
                    if (!string.IsNullOrEmpty(routing.AuthRoleIDs))
                    {
                        string prjRoles = PrjRoleExt.GetPrjRoles(routing.AuthRoleIDs);
                        string engRoles = PrjRoleExt.GetEngRoles(routing.AuthRoleIDs);
                        if (!string.IsNullOrEmpty(prjRoles))
                        {
                            if (!PrjRoleExt.InPrjRole(user.UserID, prjRoles))
                                continue;
                        }
                        else if (!string.IsNullOrEmpty(engRoles))
                        {
                            if (!PrjRoleExt.InEngRole(user.UserID, engRoles))
                                continue;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(routing.AuthOrgIDs))
                            {
                                string orgIDs = routing.AuthOrgIDs;
                                if (formDic.Count() > 0 && formDic.ContainsKey(routing.AuthOrgIDs)) //权限组织取自表单
                                    orgIDs = formDic[routing.AuthOrgIDs] == null ? "" : formDic[routing.AuthOrgIDs].ToString();
                                if (!userService.InRole(user.UserID, routing.AuthRoleIDs, orgIDs))
                                    continue;
                            }
                            else
                            {
                                if (!userService.InRole(user.UserID, routing.AuthRoleIDs, user.UserOrgID))
                                    continue;
                            }
                        }

                    }
                    else if (!string.IsNullOrEmpty(routing.AuthOrgIDs))
                    {
                        if (!userService.InOrg(user.UserID, routing.AuthOrgIDs))
                            continue;
                    }

                    #endregion

                    //加入
                    if (routing.Type == RoutingType.Branch.ToString())
                    {
                        string Branch = RoutingType.Branch.ToString();
                        var existRouting = list.Where(c => c.Type == Branch && c.Code == routing.Code && c.Name == routing.Name).SingleOrDefault();
                        if (existRouting != null)
                        {
                            existRouting.ID += "," + routing.ID;
                        }
                        else
                        {
                            list.Add(routing);
                        }
                    }
                    else if (routing.Type == RoutingType.SingleBranch.ToString())
                    {
                        string singleBranch = RoutingType.SingleBranch.ToString();
                        var existRouting = list.Where(c => c.Type == singleBranch).SingleOrDefault();
                        if (existRouting == null)
                        {
                            list.Add(routing);
                        }
                    }
                    else
                    {
                        list.Add(routing);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                string info = string.Format(@"获取路由方法GetStartRoutingList,参数(flowCode:{0})<br>
                    出错原因分析:<br>
                        1.流程不存在路由或没有绘制流程图<br>
                        2.流程下一个环节或权限过滤配置有误(如字段格式必须是{})<br>
                        ", flowCode);
                throw new FlowException(info, ex);
            }
        }

        public List<S_WF_InsDefRouting> GetRoutingList(string taskExecID, Dictionary<string, object> formDic)
        {
            List<S_WF_InsDefRouting> list = new List<S_WF_InsDefRouting>();
            try
            {
                var taskExec = entities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault();
                if (taskExec.ExecTime != null) //已经执行过的任务，不能再获取路由
                    return list;

                var task = taskExec.S_WF_InsTask;
                var step = task.S_WF_InsDefStep;

                var userService = FormulaHelper.GetService<IUserService>();
                foreach (var routing in step.S_WF_InsDefRouting.OrderBy(c => c.SortIndex))
                {
                    //根据流程变量过滤
                    if (!HasVariableAuth(task.InsFlowID, routing.AuthVariable))
                        continue;
                    //根据表单数据过滤
                    if (!HasFormDataAuth(formDic, routing.AuthFormData, taskExec))
                        continue;
                    //根据Sql权限
                    if (!HasSqlAuth(formDic, routing.AuthFromSql, taskExec))
                        continue;

                    #region 用户过滤

                    UserInfo user = null;
                    if (!string.IsNullOrEmpty(routing.AuthTargetUser)) //指定目标用户为表单字段
                    {
                        string authTargetUser = routing.AuthTargetUser.Trim('{', '}');
                        if (formDic.ContainsKey(authTargetUser))
                            user = FormulaHelper.GetUserInfoByID(formDic[authTargetUser].ToString());
                        if (user == null)
                            continue;
                    }
                    else
                    {
                        user = FormulaHelper.GetUserInfoByID(taskExec.TaskUserID);
                    }


                    //指定用户过滤
                    if (!string.IsNullOrEmpty(routing.AuthUserIDs))
                    {
                        if (!routing.AuthUserIDs.Split(',').Contains(user.UserID))
                            continue;
                    }

                    //根据组织、角色过滤
                    if (!string.IsNullOrEmpty(routing.AuthRoleIDs))
                    {
                        string prjRoles = PrjRoleExt.GetPrjRoles(routing.AuthRoleIDs);
                        string engRoles = PrjRoleExt.GetEngRoles(routing.AuthRoleIDs);
                        if (!string.IsNullOrEmpty(prjRoles))
                        {
                            if (!PrjRoleExt.InPrjRole(user.UserID, prjRoles))
                                continue;
                        }
                        else if (!string.IsNullOrEmpty(engRoles))
                        {
                            if (!PrjRoleExt.InEngRole(user.UserID, engRoles))
                                continue;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(routing.AuthOrgIDs))
                            {
                                string orgIDs = routing.AuthOrgIDs;
                                if (formDic.ContainsKey(routing.AuthOrgIDs)) //权限组织取自表单
                                    orgIDs = formDic[routing.AuthOrgIDs] == null ? "" : formDic[routing.AuthOrgIDs].ToString();
                                if (!userService.InRole(user.UserID, routing.AuthRoleIDs, orgIDs))
                                    continue;
                            }
                            else
                            {
                                if (!userService.InRole(user.UserID, routing.AuthRoleIDs, user.UserOrgID))
                                    continue;
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(routing.AuthOrgIDs))
                    {
                        if (!userService.InOrg(user.UserID, routing.AuthOrgIDs))
                            continue;
                    }

                    #endregion

                    //分支路由过滤
                    if (routing.Type == RoutingType.Branch.ToString())
                    {
                        if (taskExec.ExecRoutingIDs.Split(',').Contains(routing.ID))
                            continue;
                    }
                    //弱控路由过滤
                    if (routing.Type == RoutingType.Weak.ToString())
                    {
                        if (taskExec.WeakedRoutingIDs.Split(',').Contains(routing.ID))
                            continue;
                    }


                    //加入
                    if (routing.Type == RoutingType.Branch.ToString())
                    {
                        string Branch = RoutingType.Branch.ToString();
                        var existRouting = list.Where(c => c.Type == Branch && c.Code == routing.Code && c.Name == routing.Name).SingleOrDefault();
                        if (existRouting != null)
                        {
                            existRouting.ID += "," + routing.ID;
                        }
                        else
                        {
                            list.Add(routing);
                        }
                    }
                    else if (routing.Type == RoutingType.SingleBranch.ToString())
                    {
                        string singleBranch = RoutingType.SingleBranch.ToString();
                        var existRouting = list.Where(c => c.Type == singleBranch).SingleOrDefault();
                        if (existRouting == null)
                        {
                            list.Add(routing);
                        }
                    }
                    else
                    {
                        list.Add(routing);
                    }
                }
            }
            catch (Exception ex)
            {
                string info = string.Format(@"根据任务获取路由方法GetRoutingList,参数(taskExecID:{0})<br>
                    出错原因分析:<br>
                        1.流程不存在路由或没有绘制流程图<br>
                        2.流程下一个环节或权限过滤配置有误(如字段格式必须是{})<br>
                        ", taskExecID);
                throw new FlowException(info, ex);
            }
            return list;
        }

        #region 判断表单数据是否满足

        private bool HasFormDataAuth(Dictionary<string, object> formDic, string authFormData, S_WF_InsTaskExec taskExec)
        {
            if (string.IsNullOrWhiteSpace(authFormData))
                return true;
            try
            {
                UserInfo user = null;
                if (taskExec == null)
                    user = FormulaHelper.GetUserInfo(); //没有任务时
                else
                    user = FormulaHelper.GetUserInfoByID(taskExec.TaskUserID);

                Regex reg = new Regex(@"\{[0-9a-zA-Z_\u4e00-\u9faf]*\}", RegexOptions.IgnoreCase);
                authFormData = reg.Replace(authFormData, (Match m) =>
                {
                    string value = m.Value.Trim('{', '}');

                    switch (value)
                    {
                        case "CurrentUserID":
                            return user.UserID;
                        case "CurrentUserOrgID":
                            return user.UserOrgID;
                        case "TaskUserID":
                            return taskExec.TaskUserID;
                        default:
                            if (formDic.Count() > 0 && !formDic.ContainsKey(value))
                                throw new Exception(string.Format("权限过滤出错，字段:“{0}”不存在", value));
                            return (formDic.Count() == 0 || formDic[value] == null) ? "" : formDic[value].ToString();
                    }
                });


                //string sql = string.Format("select count(1) where {0}", authFormData);
                //为了兼容Oracle修改
                string sql = string.Format("select count(1) from S_WF_DefFlow where {0}", authFormData);

                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
                object obj = sqlHelper.ExecuteScalar(sql);
                return Convert.ToInt32(obj) > 0;
            }
            catch (Exception ex)
            {
                string msg = "路由的表达式权限错误：" + authFormData;
                string info = string.Format(@"判断表单数据是否满足方法HasFormDataAuth,参数(authFormData:{0})<br>
                    出错原因分析:<br>
                        1.权限过滤出错，字段不存在或格式不正确<br>
                        2.{1}<br>
                        ", authFormData, msg);
                throw new FlowException(info, ex);
            }
        }

        #endregion

        #region 判断变量权限是否满足

        public bool HasStartVariableAuth(S_WF_InsDefFlow insDef, string variableAuth)
        {
            try
            {
                if (string.IsNullOrEmpty(variableAuth))
                    return true;

                if (string.IsNullOrEmpty(insDef.InitVariable))
                    throw new Exception("首环节路由所使用的变量必须初始化！");
                string where = variableAuth;
                foreach (var item in insDef.InitVariable.Split(','))
                {
                    string key = item.Split('=')[0];
                    string value = item.Split('=')[1];
                    where = where.Replace(key, value);
                }
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
                string sql = string.Format("select count(1) from S_WF_DefFlow where {0}", where);

                object obj = sqlHelper.ExecuteScalar(sql);
                return Convert.ToInt32(obj) > 0;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        /// <summary>
        /// 判断某流程的变量权限是否满足
        /// </summary>
        /// <param name="flowID"></param>
        /// <param name="variableAuth"></param>
        /// <returns></returns>
        public bool HasVariableAuth(string flowID, string variableAuth)
        {
            if (string.IsNullOrEmpty(variableAuth))
                return true;

            string fieldNames = "";
            foreach (string item in variableAuth.Split(new string[] { " and ", " or " }, StringSplitOptions.RemoveEmptyEntries))
            {
                fieldNames += item.Trim().Split(new string[] { " ", "=", ">", "<" }, StringSplitOptions.RemoveEmptyEntries).First() + ",";
            }
            fieldNames = fieldNames.Trim(',');

            string sql = @"select count(1) from (select InsFlowID,VariableName,VariableValue from S_WF_InsVariable) a pivot (max(VariableValue) for VariableName in ({1})) b where InsFlowID='{0}' and {2} ";
            sql = string.Format(sql
                , flowID
                , string.Join(",", fieldNames.Split(',').Distinct())
                , variableAuth);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            object obj = sqlHelper.ExecuteScalar(sql);

            if (Convert.ToInt32(obj) == 0)
                return false;
            return true;
        }

        #endregion

        #region 判断满足sql权限

        private bool HasSqlAuth(Dictionary<string, object> formDic, string sql, S_WF_InsTaskExec taskExec)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return true;

            try
            {
                UserInfo user = null;
                if (taskExec == null)
                    user = FormulaHelper.GetUserInfo(); //没有任务时
                else
                    user = FormulaHelper.GetUserInfoByID(taskExec.TaskUserID);

                Regex reg = new Regex(@"\{[0-9a-zA-Z_\u4e00-\u9faf]*\}", RegexOptions.IgnoreCase);
                sql = reg.Replace(sql, (Match m) =>
                {
                    string value = m.Value.Trim('{', '}');

                    switch (value)
                    {
                        case "CurrentUserID":
                            return user.UserID;
                        case "CurrentUserOrgID":
                            return user.UserOrgID;
                        case "TaskUserID":
                            return taskExec.TaskUserID;
                        default:
                            if (System.Configuration.ConfigurationManager.ConnectionStrings[value] != null)
                            {
                                return SQLHelper.CreateSqlHelper(value).DbName;
                            }
                            else
                            {
                                if (formDic.Count() > 0 && !formDic.ContainsKey(value))
                                    throw new Exception(string.Format("权限过滤出错，字段:“{0}”不存在", value));
                                return (formDic.Count() == 0 || formDic[value] == null) ? "" : formDic[value].ToString();
                            }
                    }
                });

                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
                object obj = sqlHelper.ExecuteScalar(sql);
                return Convert.ToInt32(obj) > 0;
            }
            catch (Exception ex)
            {
                string msg = "sql权限错误：" + sql;
                throw new FlowException(msg, ex);
            }
        }



        #endregion

        #endregion

        #region 事后委托

        public void DelegateTaskExec(string taskExecID, string beDelegateUserID, string beDelegateUserName)
        {
            var taskExec = entities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault();
            taskExec.Complete(taskExec.S_WF_InsTask, taskExec.S_WF_InsFlow, taskExec.S_WF_InsTask.S_WF_InsDefStep.DefStepID, taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code);

            taskExec.ExecUserID = beDelegateUserID;
            taskExec.ExecUserName = beDelegateUserName;
            taskExec.Create(taskExec.S_WF_InsTask, taskExec.S_WF_InsFlow, taskExec.S_WF_InsTask.S_WF_InsDefStep.DefStepID, taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code);

            taskExec.Type = TaskExecType.Delegate.ToString();
            entities.SaveChanges();
        }


        #endregion

        #region 传阅

        public void CirculateTaskExec(string taskExecID, string beCirculateUserIDs, string beCirculateUserNames)
        {
            try
            {
                if (string.IsNullOrEmpty(taskExecID))
                    throw new FlowException("taskExecID不能为空!");
                var taskExec = entities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault();
                if (taskExec == null)
                    throw new FlowException("当前任务不存在或已执行!");
                var user = FormulaHelper.GetUserInfoByID(taskExec.ExecUserID);


                var beUserIDs = beCirculateUserIDs.Split(',');
                var beUserNames = beCirculateUserNames.Split(',');

                for (int i = 0; i < beUserIDs.Length; i++)
                {
                    var viewTaskExec = new S_WF_InsTaskExec();
                    viewTaskExec.ID = FormulaHelper.CreateGuid();
                    viewTaskExec.InsTaskID = taskExec.InsTaskID;
                    viewTaskExec.InsFlowID = taskExec.InsFlowID;
                    viewTaskExec.ExecRoutingIDs = "";
                    viewTaskExec.CreateTime = DateTime.Now;
                    viewTaskExec.TaskUserID = user.UserID;
                    viewTaskExec.TaskUserName = user.UserName;
                    viewTaskExec.Type = TaskExecType.Circulate.ToString();
                    viewTaskExec.ExecUserID = beUserIDs[i];
                    viewTaskExec.ExecUserName = beUserNames[i];

                    viewTaskExec.Create(taskExec.S_WF_InsTask, taskExec.S_WF_InsFlow, taskExec.S_WF_InsTask.S_WF_InsDefStep.DefStepID, taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code);
                    entities.S_WF_InsTaskExec.Add(viewTaskExec);

                }

                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        #endregion

        #region 阅毕和回复

        public void ViewTaskExec(string taskExecID, string comment)
        {
            try
            {
                if (string.IsNullOrEmpty(taskExecID))
                    throw new FlowException("taskExecID不能为空!");
                var taskExec = entities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault();
                if (taskExec == null)
                    throw new FlowException("当前任务不存在或已执行!");
                taskExec.ExecTime = DateTime.Now;

                taskExec.Complete(taskExec.S_WF_InsTask, taskExec.S_WF_InsFlow, taskExec.S_WF_InsTask.S_WF_InsDefStep.DefStepID, taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code);
                if (taskExec.Type == TaskExecType.Ask.ToString())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(taskExec.ExecComment);
                    if (!string.IsNullOrEmpty(comment))
                    {
                        sb.Append("|" + comment);
                    }
                    comment = sb.ToString();
                }
                ////增加是否移动端执行任务标记
                if (HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request["IsMobileRequest"]))
                    taskExec.ApprovalInMobile = HttpContext.Current.Request["IsMobileRequest"];
                else
                    taskExec.ApprovalInMobile = "0";
                taskExec.ExecComment = comment;
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }

        #endregion

        #region 加签

        public void AskTaskExec(string taskExecID, string beAskUserIDs, string beAskUserNames, string execComment)
        {
            try
            {
                var taskExec = entities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault();
                var user = FormulaHelper.GetUserInfoByID(taskExec.ExecUserID);
                taskExec.ExecComment = execComment;
                var beUserIDs = beAskUserIDs.Split(',');
                var beUserNames = beAskUserNames.Split(',');

                for (int i = 0; i < beUserIDs.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(beUserIDs[i].ToString()))
                    {
                        var viewTaskExec = new S_WF_InsTaskExec();
                        viewTaskExec.ID = FormulaHelper.CreateGuid();
                        viewTaskExec.InsTaskID = taskExec.InsTaskID;
                        viewTaskExec.InsFlowID = taskExec.InsFlowID;
                        viewTaskExec.ExecRoutingIDs = "";
                        viewTaskExec.CreateTime = DateTime.Now;
                        viewTaskExec.TaskUserID = user.UserID;
                        viewTaskExec.TaskUserName = user.UserName;
                        viewTaskExec.Type = TaskExecType.Ask.ToString();
                        viewTaskExec.ExecUserID = beUserIDs[i].ToString();
                        viewTaskExec.ExecUserName = beUserNames[i].ToString();
                        //viewTaskExec.ExecComment = execComment;

                        viewTaskExec.Create(taskExec.S_WF_InsTask, taskExec.S_WF_InsFlow, taskExec.S_WF_InsTask.S_WF_InsDefStep.DefStepID, taskExec.S_WF_InsFlow.S_WF_InsDefFlow.Code);

                        //viewTaskExec.Create(taskExec.S_WF_InsTask);
                        entities.S_WF_InsTaskExec.Add(viewTaskExec);
                    }
                }

                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                string info = string.Format(@"加签AskTaskExec,参数(
                    taskExecID:{0}, beAskUserIDs:{1}, beAskUserNames:{2}, execComment:{3})<br>
                    出错原因分析:<br>
                        1.taskExecID不能为空<br>
                        2.当前任务不存在或已执行<br>
                        ", taskExecID, beAskUserIDs, beAskUserNames, execComment);
                throw new FlowException(info, ex);
            }
        }

        #endregion

        #region 撤销加签

        public void WidthdrawAskTaskExec(string taskExecID)
        {
            try
            {
                if (string.IsNullOrEmpty(taskExecID))
                    throw new FlowException("taskExecID不能为空!");
                string askType = TaskExecType.Ask.ToString();
                var askTaskExecs = entities.S_WF_InsTaskExec.Where(c => c.ID == taskExecID).SingleOrDefault()
                    .S_WF_InsTask.S_WF_InsTaskExec.Where(c => c.Type == askType).ToArray();
                foreach (var item in askTaskExecs)
                {
                    item.Delete(item.S_WF_InsTask, item.S_WF_InsFlow, item.S_WF_InsTask.S_WF_InsDefStep.DefStepID, item.S_WF_InsFlow.S_WF_InsDefFlow.Code);

                    entities.Set<S_WF_InsTaskExec>().Remove(item);
                }

                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new FlowException("操作移除当前任务时出错!", ex);
            }
        }

        #endregion

        #region 流程维护方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="defFlowID"></param>
        public string CloneDefFlow(string defFlowID)
        {
            try
            {
                var def = entities.S_WF_DefFlow.Where(c => c.ID == defFlowID).SingleOrDefault();
                var newDef = def.Clone();
                newDef.Code = newDef.ID;
                newDef.Name += "-复制";
                newDef.AlreadyReleased = "0";//修订中
                newDef.ModifyTime = null;
                entities.S_WF_DefFlow.Add(newDef);
                entities.SaveChanges();
                return newDef.ID;
            }
            catch (Exception ex)
            {
                throw new FlowException("向S_WF_DefFlow表添加数据出错!", ex);
            }

        }


        #endregion

        #region 获取表单数据字典

        public Dictionary<string, object> GetFormDic(string formData, string taskExecID, string formInstanceID)
        {
            var taskExec = entities.S_WF_InsTaskExec.SingleOrDefault(c => c.ID == taskExecID);
            return GetFormDic(formData, taskExec, formInstanceID);
        }

        private Dictionary<string, object> _formDic = null;
        public Dictionary<string, object> GetFormDic(string formData, S_WF_InsTaskExec taskExec, string formInstanceID)
        {
            try
            {
                if (_formDic == null)
                {
                    #region 表单数据
                    if (string.IsNullOrEmpty(formData))
                        formData = "{}";
                    _formDic = JsonHelper.ToObject<Dictionary<string, object>>(formData);
                    #endregion

                    if (!string.IsNullOrEmpty(formInstanceID))
                    {
                        #region 子表数据库数据

                        if (taskExec != null)
                        {
                            string subFormID = taskExec.S_WF_InsTask.S_WF_InsDefStep.SubFormID;
                            if (!string.IsNullOrEmpty(subFormID))
                            {
                                var subForm = entities.S_WF_DefSubForm.SingleOrDefault(c => c.ID == subFormID);
                                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(subForm.ConnName);
                                DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from {0} where ID='{1}'", subForm.TableName, formInstanceID));
                                if (dt.Rows.Count > 0)
                                {
                                    foreach (DataColumn col in dt.Columns)
                                    {
                                        if (!_formDic.ContainsKey(col.ColumnName))
                                            _formDic.Add(col.ColumnName, dt.Rows[0][col].ToString());
                                    }
                                }
                            }
                        }

                        #endregion

                        #region 主表数据库数据
                        var flow = FormulaHelper.GetEntities<WorkflowEntities>().Set<S_WF_InsFlow>().SingleOrDefault(c => c.FormInstanceID == formInstanceID);
                        if (flow != null)
                        {
                            var defFlow = flow.S_WF_InsDefFlow;
                            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(defFlow.ConnName);
                            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from {0} where ID='{1}'", defFlow.TableName, formInstanceID));
                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataColumn col in dt.Columns)
                                {
                                    if (!_formDic.ContainsKey(col.ColumnName))
                                        _formDic.Add(col.ColumnName, dt.Rows[0][col].ToString());
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FlowException("获取表单字段出错,请检查表单字段的设置!", ex);
            }
            return _formDic;
        }

        #endregion

        #region BaseFlowController方法

        public RoutingParams GetRoutingParams(S_WF_InsDefRouting routing, S_WF_InsTaskExec taskExec, string formInstanceID, bool isMobileClient = false)
        {
            S_WF_InsTask task = null;
            if (taskExec != null)
                task = taskExec.S_WF_InsTask;

            string formData = FormulaHelper.ContextGetValueString("FormData");

            Dictionary<string, object> formDic = GetFormDic(formData.ToString(), taskExec, formInstanceID);

            RoutingParams param = new RoutingParams
            {
                routingID = routing.ID,
                routingCode = routing.Code,
                routingName = routing.Name,
                selectMode = routing.SelectMode,
                selectAgain = false,
                closeForm = routing.Type != RoutingType.Weak.ToString(),
                notNullFields = routing.NotNullFields ?? "",
                defaultComment = routing.DefaultComment,
                mustInputComment = routing.MustInputComment == "1",
                signatureDivID = routing.SignatureDivID,
                signatureProtectFields = routing.SignatureProtectFields,
                signatureField = routing.SignatureField,
                orgIDFromField = routing.OrgIDFromField,
                roleIDsFromField = routing.RoleIDsFromField,
                userIDsFromField = routing.UserIDsFromField,
                inputSignPwd = routing.InputSignPwd,
                userIDsGroupFromField = routing.UserIDsGroupFromField,
                flowComplete = entities.Set<S_WF_InsDefStep>().Where(c => c.ID == routing.EndID).SingleOrDefault().Type == StepTaskType.Completion.ToString(),
                signatureType = routing.SignatureType
            };

            param.orgIDs = GetRoutingOrgIDs(routing, formDic, taskExec);
            param.roleIDs = GetRoutingRoleIDs(routing, formDic, taskExec);
            param.userIDs = GetRoutingUserIDs(routing, formDic, taskExec, param.orgIDs, param.roleIDs, isMobileClient);
            param.excudeUserIDs = GetRoutingExcudeUserIDs(routing, taskExec, formDic);
            var userGroupListString = GetRoutingUserIDsGroup(routing, formDic);
            param.userIDsGroup = String.IsNullOrEmpty(userGroupListString) ? new List<Dictionary<string, object>>() : JsonHelper.ToList(userGroupListString);

            if (param.userIDs == "" && task != null)
            {
                S_WF_InsTask preTask = null;
                if (routing.Type == RoutingType.Distribute.ToString())
                    preTask = task.S_WF_InsFlow.S_WF_InsTask.Where(c => c.InsDefStepID == routing.EndID).Where(c => c.SendTaskUserIDs == taskExec.ExecUserID).OrderBy(c => c.ID).LastOrDefault();
                else
                    preTask = task.S_WF_InsFlow.S_WF_InsTask.Where(c => c.InsDefStepID == routing.EndID).OrderBy(c => c.ID).LastOrDefault();

                if (preTask != null)
                    param.userIDs = preTask.TaskUserIDs;
            }

            if (routing.SelectAgain == "1" && string.IsNullOrEmpty(routing.UserIDsFromField) && task != null)
            {
                #region 已打回的路由设置
                param.selectAgain = true;
                if (isMobileClient || string.IsNullOrEmpty(routing.SelectMode) || routing.SelectMode == "SelectDoback")
                {

                }
                else
                {
                    param.userIDs = "";
                }
                #endregion
            }

            return param;
        }

        public string GetFormValue(Dictionary<string, object> formDic, string key)
        {
            if (formDic.Keys.Contains(key) && formDic[key] != null)
                return formDic[key].ToString();
            return "";
        }

        public string GetRoutingOrgIDs(S_WF_InsDefRouting routing, Dictionary<string, object> formDic, S_WF_InsTaskExec taskExec)
        {
            if (routing.ID.Split(',').Length > 1)
                return "";
            try
            {
                S_WF_InsTask task = null;
                if (taskExec != null)
                    task = taskExec.S_WF_InsTask;

                if (!string.IsNullOrEmpty(routing.OrgIDs))
                    return routing.OrgIDs;
                else if (!string.IsNullOrEmpty(routing.OrgIDFromField))
                {
                    if (routing.OrgIDFromField.Contains('{')) //在配置项目ID和专业时  2015-1-13项目角色的支持功能
                    {
                        Regex reg = new Regex("\\{[0-9a-zA-Z_\u4e00-\u9faf]*\\}");
                        string result = reg.Replace(routing.OrgIDFromField, (Match m) =>
                        {
                            string value = m.Value.Trim('{', '}');
                            if (string.IsNullOrEmpty(HttpContext.Current.Request[value]) == false)
                                return HttpContext.Current.Request[value];
                            else if (formDic.ContainsKey(value))
                                return formDic.GetValue(value);
                            else
                                throw new Exception(string.Format("字段不存在：{0}", value));
                        });
                        return result;
                    }
                    else //直接配置组织取自字段时
                    {
                        return GetFormValue(formDic, routing.OrgIDFromField);
                    }
                }
                else if (routing.OrgIDFromUser == "CurrentUser")
                {
                    if (taskExec != null)
                        return FormulaHelper.GetUserInfoByID(taskExec.TaskUserID).UserOrgID;
                    else
                        return FormulaHelper.GetUserInfo().UserOrgID; //首环节时还没任务
                }
                else if (routing.OrgIDFromUser == "SendTaskUser")
                {
                    if (task != null && !string.IsNullOrEmpty(task.SendTaskUserIDs))
                    {
                        string sql = string.Format("select distinct DeptID from S_A_User where DeptID is not null and DeptID<>'' and ID in('{0}')", task.SendTaskUserIDs.Replace(",", "','"));
                        var dt = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(sql);
                        string deptIds = string.Join(",", dt.AsEnumerable().Select(c => c["DeptId"].ToString()).ToArray());
                        return deptIds;
                    }
                    else
                        throw new FlowException("流程配置错误：没有任务发送人ID");
                }
                else if (routing.OrgIDFromUser != "")
                {
                    string field = routing.OrgIDFromUser.Trim('{', '}');
                    if (formDic.ContainsKey(field))
                    {
                        string userID = formDic[field].ToString();
                        var user = FormulaHelper.GetService<IUserService>().GetUserInfoByID(userID);
                        if (user != null)
                            return user.UserOrgID;
                        else
                            throw new FlowException("无方法找到用户，UserID：" + userID);
                    }
                    else
                        throw new FlowException("表单上没有字段：" + field);
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        public string GetRoutingRoleIDs(S_WF_InsDefRouting routing, Dictionary<string, object> formDic, S_WF_InsTaskExec taskExec)
        {
            if (routing.ID.Split(',').Length > 1)
                return "";

            S_WF_InsTask task = null;
            if (taskExec != null)
                task = taskExec.S_WF_InsTask;

            if (!string.IsNullOrEmpty(routing.RoleIDs))
                return routing.RoleIDs;
            if (!string.IsNullOrEmpty(routing.RoleIDsFromField))
                return GetFormValue(formDic, routing.RoleIDsFromField);
            return "";
        }

        public string GetRoutingUserIDs(S_WF_InsDefRouting routing, Dictionary<string, object> formDic, S_WF_InsTaskExec taskExec, string orgIDs, string roleIDs, bool isMobileClient = false) //isMobileClient解决移动通取表单字段问题
        {
            if (routing.ID.Split(',').Length > 1)
                return "";
            try
            {
                S_WF_InsTask task = null;
                if (taskExec != null)
                    task = taskExec.S_WF_InsTask;

                string result = "";

                #region 排除指定人
                string excludeUserIds = GetRoutingExcudeUserIDs(routing, taskExec, formDic);
                #endregion

                if (!string.IsNullOrEmpty(routing.UserIDs)) //指定用户
                    result = routing.UserIDs;
                else if (isMobileClient == true && !string.IsNullOrEmpty(routing.UserIDsFromField))//移动通表单去字段问题
                {
                    foreach (var item in routing.UserIDsFromField.Split(','))
                    {
                        result += GetFormValue(formDic, item) + ",";
                    }
                    if (result.Length > 0)
                        result = result.Substring(0, result.Length - 1);
                }
                //以前台取的表单字段为主，下面注释掉  2015-8月24,继续修改为分支路由可以后台取表单字段，解决分支路由不能配置表单字段问题
                else if (routing.Type == RoutingType.Branch.ToString() && !string.IsNullOrEmpty(routing.UserIDsFromField)) //取自表单字段
                    result = GetFormValue(formDic, routing.UserIDsFromField);
                //增加一般任务也可从表单上获取下一步执行人，如果这里不获取，则自动通过会因为取不到表单上的人而失效 2018-8-13 by Eric.Yang
                else if (routing.Type == RoutingType.Normal.ToString() && !string.IsNullOrEmpty(routing.UserIDsFromField))
                    result = GetFormValue(formDic, routing.UserIDsFromField);
                else if (!string.IsNullOrEmpty(routing.UserIDsFromSql)) //执行人取自sql
                {
                    UserInfo user = null;
                    if (taskExec == null)
                        user = FormulaHelper.GetUserInfo(); //没有任务时
                    else
                        user = FormulaHelper.GetUserInfoByID(taskExec.TaskUserID);

                    Regex reg = new Regex(@"\{[0-9a-zA-Z_\u4e00-\u9faf]*\}", RegexOptions.IgnoreCase);
                    string sql = reg.Replace(routing.UserIDsFromSql, (Match m) =>
                    {
                        string value = m.Value.Trim('{', '}');

                        switch (value)
                        {
                            case "CurrentUserID":
                                return user.UserID;
                            case "CurrentUserOrgID":
                                return user.UserOrgID;
                            case "TaskUserID":
                                return taskExec.TaskUserID;
                            default:
                                if (System.Configuration.ConfigurationManager.ConnectionStrings[value] != null)
                                {
                                    return SQLHelper.CreateSqlHelper(value).DbName;
                                }
                                else
                                {
                                    if (!formDic.ContainsKey(value))
                                        throw new Exception(string.Format("权限过滤出错，字段:“{0}”不存在", value));
                                    return formDic[value] == null ? "" : formDic[value].ToString();
                                }
                        }
                    });

                    try
                    {
                        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
                        var dt = sqlHelper.ExecuteDataTable(sql);
                        result = string.Join(",", dt.AsEnumerable().Select(c => c[0].ToString()));
                    }
                    catch
                    {
                        string msg = "路由的执行人取自Sql出错：" + sql;
                        throw new FlowException(msg);
                    }
                }
                else if (task != null)
                {
                    #region 下一步执行人取自环节
                    if (!string.IsNullOrEmpty(routing.UserIDsFromStepExec)) //来自环节任务执行人
                    {
                        var stepTask = task.S_WF_InsFlow.S_WF_InsTask.Where(c => c.InsDefStepID == routing.UserIDsFromStepExec).LastOrDefault();
                        if (stepTask == null)
                            throw new FlowException("流程配置错误");
                        result = string.Join(",", stepTask.S_WF_InsTaskExec.Select(c => c.ExecUserID).ToArray());
                    }
                    else if (!string.IsNullOrEmpty(routing.UserIDsFromStep)) //环节任务接受人
                    {
                        var stepTask = task.S_WF_InsFlow.S_WF_InsTask.Where(c => c.InsDefStepID == routing.UserIDsFromStep).LastOrDefault();
                        if (stepTask == null)
                            throw new FlowException("流程配置错误");
                        result = stepTask.TaskUserIDs;
                    }
                    else if (!string.IsNullOrEmpty(routing.UserIDsFromStepSender)) //环节任务发送人
                    {
                        //如果取自本任务环节的发送人
                        if (routing.UserIDsFromStepSender == task.S_WF_InsDefStep.ID)
                            result = task.SendTaskUserIDs;
                        else
                        {
                            var stepTask = task.S_WF_InsFlow.S_WF_InsTask.Where(c => c.InsDefStepID == routing.UserIDsFromStepSender).LastOrDefault();
                            if (stepTask == null)
                                throw new FlowException("流程配置错误");
                            result = stepTask.SendTaskUserIDs;
                        }
                    }
                    #endregion
                }

                string result1 = "";
                if (!string.IsNullOrEmpty(roleIDs))//根步执据角色取下一行人
                {
                    result1 = FormulaHelper.GetService<IRoleService>().GetUserIDsInRoles(roleIDs, orgIDs, excludeUserIds);
                    //2018-1-30 剥离项目角色选人功能
                    var prjRoleUser = PrjRoleExt.GetRoleUserIDs(roleIDs, orgIDs);
                    if (!string.IsNullOrEmpty(prjRoleUser))
                        result1 = (prjRoleUser + "," + result1).Trim(',');
                }
                else if (!string.IsNullOrEmpty(orgIDs))//根据组织取下一步执行人
                    result1 = FormulaHelper.GetService<IOrgService>().GetUserIDsInOrgs(orgIDs);


                result = (result + "," + result1).Trim(',');

                //排除指定人
                result = StringHelper.Exclude(result, excludeUserIds);

                result = StringHelper.Distinct(result);
                return result;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        public string GetRoutingExcudeUserIDs(S_WF_InsDefRouting routing, S_WF_InsTaskExec taskExec, Dictionary<string, object> formDic)
        {
            string excludeUserIds = "";
            if (!string.IsNullOrEmpty(routing.ExcludeUser))
            {
                if (routing.ExcludeUser == "CurrentUser")
                {
                    if (taskExec != null)
                        excludeUserIds = taskExec.TaskUserID;
                    else
                        excludeUserIds = FormulaHelper.GetUserInfo().UserID;
                }
                else if (routing.ExcludeUser == "SendTaskUser")
                {
                    if (taskExec == null)
                    {
                        throw new FlowException(string.Format("{0}:排除执行人配置错误", routing.Name));
                    }
                    else
                    {
                        excludeUserIds = taskExec.S_WF_InsTask.SendTaskUserIDs;
                    }
                }
                else
                {
                    if (formDic.ContainsKey(routing.ExcludeUser))
                    {
                        excludeUserIds = formDic[routing.ExcludeUser].ToString();
                    }
                    else
                    {
                        throw new FlowException(string.Format("表单上没有字段：{0}", routing.ExcludeUser));
                    }
                }
            }

            return excludeUserIds;
        }

        public string GetRoutingUserIDsGroup(S_WF_InsDefRouting routing, Dictionary<string, object> formDic)
        {
            if (routing.ID.Split(',').Length > 1)
                return "";

            if (!string.IsNullOrEmpty(routing.UserIDsGroupFromField))
                return GetFormValue(formDic, routing.UserIDsGroupFromField);
            return "";
        }

        #endregion

        #region 自动执行任务接口

        public void AutoExecTask(string taskExecID, string execComment)
        {
            try
            {
                var routingList = AutoExecGetRoutingList(taskExecID);
                if (routingList.Count == 0)
                    return;
                var routing = routingList.First();
                routingList.RemoveWhere(c => c.ID != routing.ID && c.Type != RoutingType.Branch.ToString() && c.Name != routing.Name);


                foreach (var item in routingList)
                {
                    AutoExecTask(taskExecID, execComment, item.ID, string.Join(",", routingList.Select(c => c.ID).ToArray()));
                }
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        public void AutoExecTask(string taskExecID, string execComment, string routingID, string allBranchRoutingIDs = "")
        {

            var taskExec = entities.S_WF_InsTaskExec.SingleOrDefault(c => c.ID == taskExecID);
            if (taskExec == null)
                throw new FlowException("任务不存在或已执行!");
            //模拟当前用户登录
            var agentUser = FormulaHelper.GetUserInfoByID(taskExec.ExecUserID);
            FormulaHelper.ContextSet("AgentUserLoginName", agentUser.Code);


            var task = taskExec.S_WF_InsTask;
            var flow = taskExec.S_WF_InsFlow;
            var step = task.S_WF_InsDefStep;
            var formDic = GetFormDic(null, taskExec, flow.FormInstanceID);

            var routing = entities.S_WF_InsDefRouting.SingleOrDefault(c => c.ID == routingID);
            if (routing.DenyAutoPass == "1")
            {
                taskExec.TimeoutAutoPassResult = "The task can not be done automatically.";//记录自动执行状态
                entities.SaveChanges();
                return;
            }
            var param = GetRoutingParams(routing, taskExec, flow.FormInstanceID);

            string nextUserIDs = param.userIDs;
            string nextUserNames = FormulaHelper.GetService<IUserService>().GetUserNames(nextUserIDs);


            try
            {
                ExecTask(taskExecID, routingID, nextUserIDs, nextUserNames,
                    param.userIDsGroup == null || param.userIDsGroup.Count == 0 ? "" : JsonHelper.ToJson(param.userIDsGroup),
                    param.roleIDs, param.orgIDs, execComment, allBranchRoutingIDs);
                taskExec.TimeoutAutoPassResult = "Success";//记录自动执行状态
                entities.SaveChanges();
            }
            catch (Exception e)
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
                string sql = "update S_WF_InsTaskExec set TimeoutAutoPassResult='{0}' where ID='{1}'";
                sql = string.Format(sql, e.Message.Substring(0, 500), taskExecID);
                sqlHelper.ExecuteNonQuery(sql); //记录错误
            }

            //取消模拟当前用户登录
            FormulaHelper.ContextRemoveByKey("AgentUserLoginName");

        }

        public List<S_WF_InsDefRouting> AutoExecGetRoutingList(string taskExecID)
        {
            var taskExec = entities.S_WF_InsTaskExec.SingleOrDefault(c => c.ID == taskExecID);
            //模拟当前用户登录
            var agentUser = FormulaHelper.GetUserInfoByID(taskExec.ExecUserID);
            FormulaHelper.ContextSet("AgentUserLoginName", agentUser.Code);

            var formDic = GetFormDic("", taskExec, taskExec.S_WF_InsFlow.FormInstanceID);
            var result = GetRoutingList(taskExecID, formDic);

            //取消模拟当前用户登录
            FormulaHelper.ContextRemoveByKey("AgentUserLoginName");

            return result;
        }

        #endregion

        #region 导出

        public string exportSql(string defID)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);

                string sql = string.Format("select * from S_WF_DefFlow where ID='{0}'", defID);

                DataTable dtDefFlow = sqlHelper.ExecuteDataTable(sql);
                sql = string.Format("select * from S_WF_DefDelegate where DefFlowID='{0}'", defID);
                DataTable dtDefDelegate = sqlHelper.ExecuteDataTable(sql);
                sql = string.Format("select * from S_WF_DefStep where DefFlowID='{0}'", defID);
                DataTable dtDefStep = sqlHelper.ExecuteDataTable(sql);
                sql = string.Format("select * from S_WF_DefRouting where DefFlowID='{0}'", defID);
                DataTable dtDefRouting = sqlHelper.ExecuteDataTable(sql);
                sql = string.Format("select * from S_WF_DefSubForm where DefFlowID='{0}'", defID);
                DataTable dtDefSubForm = sqlHelper.ExecuteDataTable(sql);

                if (Config.Constant.IsOracleDb)
                {
                    #region oracle 版本
                    //声明变量
                    sb.Append("DECLARE");
                    sb.AppendLine();
                    List<string> l = new List<string>();
                    foreach (DataColumn col in dtDefFlow.Columns)
                    {
                        if (col.DataType == typeof(string))
                        {
                            if (l.Contains(col.ColumnName) == false)
                            {
                                sb.AppendFormat("par_{0} nclob;", col.ColumnName);
                                sb.AppendLine();
                                l.Add(col.ColumnName);
                            }
                        }
                    }
                    foreach (DataColumn col in dtDefDelegate.Columns)
                    {
                        if (l.Contains(col.ColumnName) == false)
                        {
                            sb.AppendFormat("par_{0} nclob;", col.ColumnName);
                            sb.AppendLine();
                            l.Add(col.ColumnName);
                        }
                    }
                    foreach (DataColumn col in dtDefStep.Columns)
                    {
                        if (l.Contains(col.ColumnName) == false)
                        {
                            sb.AppendFormat("par_{0} nclob;", col.ColumnName);
                            sb.AppendLine();
                            l.Add(col.ColumnName);
                        }
                    }
                    foreach (DataColumn col in dtDefRouting.Columns)
                    {
                        if (l.Contains(col.ColumnName) == false)
                        {
                            sb.AppendFormat("par_{0} nclob;", col.ColumnName);
                            sb.AppendLine();
                            l.Add(col.ColumnName);
                        }
                    }
                    foreach (DataColumn col in dtDefSubForm.Columns)
                    {
                        if (l.Contains(col.ColumnName) == false)
                        {
                            sb.AppendFormat("par_{0} nclob;", col.ColumnName);
                            sb.AppendLine();
                            l.Add(col.ColumnName);
                        }
                    }

                    sb.AppendLine("begin"); //begin

                    sb.Append(SQLHelper.CreateUpdateSql("S_WF_DefFlow", dtDefFlow));
                    sb.AppendLine(SQLHelper.CreateInsertSql("S_WF_DefDelegate", dtDefDelegate));
                    sb.Append(SQLHelper.CreateUpdateSql("S_WF_DefStep", dtDefStep));
                    sb.Append(SQLHelper.CreateUpdateSql("S_WF_DefRouting", dtDefRouting));
                    sb.Append(SQLHelper.CreateUpdateSql("S_WF_DefSubForm", dtDefSubForm));

                    sb.AppendLine("end;");

                    #endregion
                }
                else
                {
                    sb.AppendFormat("USE {0}", sqlHelper.DbName);
                    sb.AppendLine();

                    sb.AppendLine(string.Format("delete from S_WF_DefFlow where ID='{0}'", defID));
                    sb.AppendLine(SQLHelper.CreateInsertSql("S_WF_DefFlow", dtDefFlow));
                    sb.AppendLine(SQLHelper.CreateInsertSql("S_WF_DefDelegate", dtDefDelegate));
                    sb.AppendLine(SQLHelper.CreateInsertSql("S_WF_DefStep", dtDefStep));
                    sb.AppendLine(SQLHelper.CreateInsertSql("S_WF_DefRouting", dtDefRouting));
                    sb.AppendLine(SQLHelper.CreateInsertSql("S_WF_DefSubForm", dtDefSubForm));
                }
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
            return sb.ToString();
        }

        #endregion

        #region 自由流程

        /// <summary>
        /// 创建自由流程，如果流程编号已经存在，则不创建
        /// </summary>
        /// <param name="flowCode"></param>
        /// <param name="flowCategory"></param>
        /// <param name="flowNameTmpl"></param>
        /// <param name="taskNameTmpl"></param>
        /// <param name="connName"></param>
        /// <param name="tableName"></param>
        /// <param name="formUrl"></param>
        /// <param name="formWidth"></param>
        /// <param name="formHeight"></param>
        /// <returns></returns>
        public S_WF_InsDefFlow CreateFreeFlow(string flowCode, string flowNameTmpl, string taskNameTmpl, string connName, string tableName, string formUrl, string forwardRoutingName = "转发", string endRoutingName = "结束", string flowCategory = "")
        {
            var def = entities.S_WF_InsDefFlow.SingleOrDefault(c => c.Code == flowCode && c.IsFreeFlow == "1");
            if (def != null)
                return def;
            try
            {
                def = new S_WF_InsDefFlow() { IsFreeFlow = "1", ID = FormulaHelper.CreateGuid(), Code = flowCode, FlowNameTemplete = flowNameTmpl, TaskNameTemplete = taskNameTmpl, FlowCategorytemplete = flowCategory, ConnName = connName, TableName = tableName, FormUrl = formUrl };
                S_WF_InsDefStep step1 = new S_WF_InsDefStep() { ID = FormulaHelper.CreateGuid(), Name = "开始", Type = "Inital", Phase = "Processing", CooperationMode = "Single" };
                S_WF_InsDefStep step2 = new S_WF_InsDefStep() { ID = FormulaHelper.CreateGuid(), Name = "审定", Type = "Normal", Phase = "Processing", CooperationMode = "All" };
                S_WF_InsDefStep step3 = new S_WF_InsDefStep() { ID = FormulaHelper.CreateGuid(), Name = "结束", Type = "Completion", Phase = "End", CooperationMode = "Single" };
                S_WF_InsDefRouting routing1 = new S_WF_InsDefRouting() { ID = FormulaHelper.CreateGuid(), Code = "Start", Name = forwardRoutingName, Type = "Normal", SelectMode = "SelectMultiUser", SaveForm = "1", MustInputComment = "1" };
                S_WF_InsDefRouting routing2 = new S_WF_InsDefRouting() { ID = FormulaHelper.CreateGuid(), Code = "Forwarding", Name = forwardRoutingName, Type = "Distribute", SelectMode = "SelectMultiUser", SaveForm = "1", MustInputComment = "1" };
                S_WF_InsDefRouting routing3 = new S_WF_InsDefRouting() { ID = FormulaHelper.CreateGuid(), Code = "End", Name = endRoutingName, Type = "Normal", SelectMode = "", SaveForm = "0", MustInputComment = "1" };


                entities.S_WF_InsDefFlow.Add(def);
                def.S_WF_InsDefStep.Add(step1);
                def.S_WF_InsDefStep.Add(step2);
                def.S_WF_InsDefStep.Add(step3);
                def.S_WF_InsDefRouting.Add(routing1);
                def.S_WF_InsDefRouting.Add(routing2);
                def.S_WF_InsDefRouting.Add(routing3);
                step1.S_WF_InsDefRouting.Add(routing1);
                step2.S_WF_InsDefRouting.Add(routing2);
                step2.S_WF_InsDefRouting.Add(routing3);
                routing1.EndID = step2.ID;
                routing2.EndID = step2.ID;
                routing3.EndID = step3.ID;

                def.ViewConfig = @"
<WorkFlow ID='{FlowDefID}' Name='' Description=''>
    <Activitys>
        <Activity  ID='{Step1ID}' Name='开始' Type='Inital' PositionX='121' PositionY='101' RepeatDirection='Vertical' ZIndex='0'></Activity>
        <Activity  ID='{Step2ID}' Name='' Type='Normal' PositionX='300' PositionY='100' RepeatDirection='Vertical' ZIndex='0'></Activity>
        <Activity  ID='{Step3ID}' Name='结束' Type='Completion' PositionX='481' PositionY='101' RepeatDirection='Vertical' ZIndex='0'></Activity>
    </Activitys>
    <Rules>
        <Rule  ID='{Routing1ID}' Name='' LineType='Line' BeginActivityID='{Step1ID}' EndActivityID='{Step2ID}' BeginPointX='141.999603271484' BeginPointY='96.8603363037109' EndPointX='251' EndPointY='95.5032196044922' TurnPoint1X='0' TurnPoint1Y='0' TurnPoint2X='0' TurnPoint2Y='0' ZIndex='9'></Rule>
        <Rule  ID='{Routing2ID}' Name='' LineType='Polyline' BeginActivityID='{Step2ID}' EndActivityID='Step2ID' BeginPointX='274.378387451172' BeginPointY='71' EndPointX='274.378387451172' EndPointY='71' TurnPoint1X='366' TurnPoint1Y='26' TurnPoint2X='236' TurnPoint2Y='26' ZIndex='16'></Rule>
        <Rule  ID='{Routing3ID}' Name='' LineType='Line' BeginActivityID='{Step2ID}' EndActivityID='{Step3ID}' BeginPointX='341' BeginPointY='96.1381225585938' EndPointX='452.015075683594' EndPointY='96.1323318481445' TurnPoint1X='0' TurnPoint1Y='0' TurnPoint2X='0' TurnPoint2Y='0' ZIndex='17'></Rule>
    </Rules>
</WorkFlow>
";
                def.ViewConfig = def.ViewConfig
                    .Replace("{FlowDefID}", def.ID)
                    .Replace("{Step1ID}", step1.ID)
                    .Replace("{Step2ID}", step2.ID)
                    .Replace("{Step3ID}", step3.ID)
                    .Replace("{Routing1ID}", routing1.ID)
                    .Replace("{Routing2ID}", routing2.ID)
                    .Replace("{Routing3ID}", routing3.ID);

                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
            return def;

        }

        /// <summary>
        /// 启动自由流程并执行到第二环节
        /// </summary>
        /// <param name="formInstanceID"></param>
        /// <param name="nextUserIDs"></param>
        /// <param name="nextUserNames"></param>
        /// <returns></returns>
        public void StartFreeFlow(string flowCode, string formInstanceID, string nextUserIDs, string nextUserNames, string exeComment)
        {
            var user = FormulaHelper.GetUserInfo();
            var taskExec = StartFlow(flowCode, formInstanceID, user.UserID, user.UserName);
            ExecTask(taskExec.ID, "Start", nextUserIDs, nextUserNames, exeComment);
        }

        #endregion

        #region 返写流程当前环节与用户

        public void BackWriteInsFlowPhase(string routingID, string nextExecUserNames)
        {
            try
            {
                if (string.IsNullOrEmpty(routingID))
                    throw new FlowException("routingID不能为空!");
                //查询下一环节
                var nextStep = entities.S_WF_InsDefStep.Where(c => c.ID == routingID).SingleOrDefault();
                if (nextStep != null)
                {
                    var insFlows = entities.S_WF_InsFlow.Where(c => c.InsDefFlowID == nextStep.InsDefFlowID).OrderByDescending(o => o.CreateTime);
                    if (insFlows != null && insFlows.Count() > 0)
                    {
                        var insFlow = insFlows.First();
                        //insFlow.CurrentStep = nextStep.Name;
                        insFlow.CurrentUserNames = nextExecUserNames;
                    }
                }
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new FlowException("返写S_WF_InsFlow流转阶段时出错!", ex);
            }
        }

        #endregion

        #region 流程定义同步到实例

        public void SyncDefToInsDef(string defID)
        {
            try
            {
                if (Config.Constant.IsOracleDb)
                {
                }
                else
                {
                    //删除没有流程的定义实例
                    string sql = @"
            delete from S_WF_InsDefFlow where ID in (select ID from (
select S_WF_InsDefFlow.ID,S_WF_InsDefFlow.DefFlowID, flowID=S_WF_InsFlow.ID from S_WF_InsDefFlow left join S_WF_InsFlow on S_WF_InsDefFlow.ID=InsDefFlowID 
) a where flowID is null and DefFlowID='{0}')
            ";
                    SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
                    sqlHelper.ExecuteNonQuery(string.Format(sql, defID));
                }

                var def = entities.S_WF_DefFlow.SingleOrDefault(c => c.ID == defID);

                //对有过程中的流程定义实例修改
                foreach (var insDef in entities.S_WF_InsDefFlow.Where(c => c.DefFlowID == defID && c.S_WF_InsFlow.Count(d => d.Status == "Processing") > 0).ToArray())
                {
                    #region 流程定义赋值
                    insDef.AllowDeleteForm = def.AllowDeleteForm;
                    insDef.CategoryID = def.CategoryID;
                    insDef.Code = def.Code;
                    insDef.ConnName = def.ConnName;
                    insDef.Description = def.Description;
                    insDef.FlowCategorytemplete = def.FlowCategorytemplete;
                    insDef.FlowNameTemplete = def.FlowNameTemplete;
                    insDef.FlowSubCategoryTemplete = def.FlowSubCategoryTemplete;
                    insDef.FormHeight = def.FormHeight;
                    insDef.FormNumberPartA = def.FormNumberPartA;
                    insDef.FormNumberPartB = def.FormNumberPartB;
                    insDef.FormNumberPartC = def.FormNumberPartC;
                    insDef.FormUrl = def.FormUrl;
                    insDef.FormWidth = def.FormWidth;
                    insDef.InitVariable = def.InitVariable;
                    //不能修改同步时间，此为流程的实例版本
                    //insDef.ModifyTime = def.ModifyTime;
                    insDef.MsgSendToAll = def.MsgSendToAll;
                    insDef.Name = def.Name;
                    insDef.TableName = def.TableName;
                    insDef.TaskCategoryTemplete = def.TaskCategoryTemplete;
                    insDef.TaskNameTemplete = def.TaskNameTemplete;
                    insDef.TaskSubCategoryTemplete = def.TaskSubCategoryTemplete;
                    #endregion

                    var insSteps = insDef.S_WF_InsDefStep.Where(c => !string.IsNullOrEmpty(c.DefStepID)).ToArray();
                    foreach (var insStep in insSteps)
                    {
                        var step = def.S_WF_DefStep.SingleOrDefault(c => c.ID == insStep.DefStepID);
                        if (step == null) continue; //环节已经被删除

                        #region 赋值环节属性

                        insStep.AllowAsk = step.AllowAsk;
                        insStep.AllowCirculate = step.AllowCirculate;
                        insStep.AllowDelegate = step.AllowDelegate;
                        insStep.AllowDoBackFirst = step.AllowDoBackFirst;
                        insStep.AllowDoBackFirstReturn = step.AllowDoBackFirstReturn;
                        insStep.AllowSave = step.AllowSave;
                        insStep.Code = step.Code;
                        insStep.CooperationMode = step.CooperationMode;
                        insStep.DisableElements = step.DisableElements;
                        insStep.EditableElements = step.EditableElements;
                        insStep.HiddenElements = step.HiddenElements;
                        insStep.VisibleElements = step.VisibleElements;

                        insStep.KeepWhenEnd = step.KeepWhenEnd;
                        //insStep.Name = step.Name; //环节名无法修改（在图上）
                        insStep.Phase = step.Phase;
                        insStep.SaveVariableAuth = step.SaveVariableAuth;
                        insStep.SortIndex = step.SortIndex;
                        insStep.SubFlowCode = step.SubFlowCode;
                        insStep.TimeoutAlarm = step.TimeoutAlarm;
                        insStep.TimeoutAutoPass = step.TimeoutAutoPass;
                        insStep.TimeoutDeadline = step.TimeoutDeadline;
                        insStep.TimeoutNotice = step.TimeoutNotice;
                        insStep.Type = step.Type;
                        insStep.Urgency = step.Urgency;
                        insStep.VisibleElements = step.VisibleElements;
                        insStep.HideAdvice = step.HideAdvice;
                        insStep.WaitingStepIDs = "";
                        insStep.DoBackSave = step.DoBackSave;

                        //关联ID的处理
                        var waitStepIDs = step.WaitingStepIDs.Split(',');
                        insStep.WaitingStepIDs = string.Join(",", insSteps.Where(c => waitStepIDs.Contains(c.DefStepID)).Select(c => c.ID).ToArray());
                        if (!string.IsNullOrEmpty(step.EmptyToStep))
                            insStep.EmptyToStep = insSteps.SingleOrDefault(c => c.DefStepID == step.EmptyToStep).ID;//为空时跳转到其它环节

                        #endregion
                    }

                    var insRoutings = insDef.S_WF_InsDefRouting.Where(c => !string.IsNullOrEmpty(c.DefRoutingID)).ToArray();

                    foreach (var insRouting in insRoutings)
                    {
                        var routing = def.S_WF_DefRouting.SingleOrDefault(c => c.ID == insRouting.DefRoutingID);
                        if (routing == null) continue;//路由已经被删除

                        #region 赋值路由属性

                        insRouting.AllowDoBack = routing.AllowDoBack;
                        insRouting.AllowWithdraw = routing.AllowWithdraw;
                        insRouting.AuthFormData = routing.AuthFormData;
                        insRouting.AuthOrgIDs = routing.AuthOrgIDs;
                        insRouting.AuthOrgNames = routing.AuthOrgNames;
                        insRouting.AuthRoleIDs = routing.AuthRoleIDs;
                        insRouting.AuthRoleNames = routing.AuthRoleNames;
                        insRouting.AuthTargetUser = routing.AuthTargetUser;
                        insRouting.AuthUserIDs = routing.AuthUserIDs;
                        insRouting.AuthUserNames = routing.AuthUserNames;
                        insRouting.AuthVariable = routing.AuthVariable;
                        insRouting.AuthFromSql = routing.AuthFromSql;
                        insRouting.Code = routing.Code;
                        insRouting.DefaultComment = routing.DefaultComment;
                        insRouting.DenyAutoPass = routing.DenyAutoPass;
                        insRouting.ExcludeUser = routing.ExcludeUser;
                        insRouting.FormDataSet = routing.FormDataSet;
                        insRouting.MsgOrgIDFromUser = routing.MsgOrgIDFromUser;
                        insRouting.MsgOrgIDs = routing.MsgOrgIDs;
                        insRouting.MsgOrgNames = routing.MsgOrgNames;
                        insRouting.MsgOrgIDsFromField = routing.MsgOrgIDsFromField;
                        insRouting.MsgRoleIDs = routing.MsgRoleIDs;
                        insRouting.MsgRoleNames = routing.MsgRoleNames;
                        insRouting.MsgRoleIDsFromField = routing.MsgRoleIDsFromField;
                        insRouting.MsgSendToTaskUser = routing.MsgSendToTaskUser;
                        insRouting.MsgTmpl = routing.MsgTmpl;
                        insRouting.MsgType = routing.MsgType;
                        insRouting.MsgUserIDs = routing.MsgUserIDs;
                        insRouting.MsgUserIDsFromField = routing.MsgUserIDsFromField;
                        //insRouting.MsgUserIDsFromStep = routing.MsgUserIDsFromStep;
                        //insRouting.MsgUserIDsFromStepExec = routing.MsgUserIDsFromStepExec;
                        //insRouting.MsgUserIDsFromStepSender = routing.MsgUserIDsFromStepSender;
                        insRouting.MsgUserNames = routing.MsgUserNames;
                        insRouting.MustInputComment = routing.MustInputComment;
                        insRouting.Name = routing.Name;
                        insRouting.NotNullFields = routing.NotNullFields;
                        insRouting.OnlyDoBack = routing.OnlyDoBack;
                        insRouting.OrgIDFromField = routing.OrgIDFromField;
                        insRouting.OrgIDFromUser = routing.OrgIDFromUser;
                        insRouting.OrgIDs = routing.OrgIDs;
                        insRouting.OrgNames = routing.OrgNames;
                        insRouting.RoleIDs = routing.RoleIDs;
                        insRouting.RoleIDsFromField = routing.RoleIDsFromField;
                        insRouting.RoleNames = routing.RoleNames;
                        insRouting.SaveForm = routing.SaveForm;
                        insRouting.SaveFormVersion = routing.SaveFormVersion;
                        insRouting.SelectAgain = routing.SelectAgain;
                        insRouting.SelectMode = routing.SelectMode;
                        insRouting.SignatureCancelDivIDs = routing.SignatureCancelDivIDs;
                        insRouting.SignatureDivID = routing.SignatureDivID;
                        insRouting.SignatureField = routing.SignatureField;
                        insRouting.SignatureProtectFields = routing.SignatureProtectFields;
                        insRouting.SortIndex = routing.SortIndex;
                        //insRouting.Title = routing.Title; //在流程图上，无法修改
                        insRouting.Type = routing.Type;
                        insRouting.UserIDs = routing.UserIDs;
                        insRouting.UserIDsFromField = routing.UserIDsFromField;
                        //insRouting.UserIDsFromStep = routing.UserIDsFromStep;
                        //insRouting.UserIDsFromStepExec = routing.UserIDsFromStepExec;
                        //insRouting.UserIDsFromStepSender = routing.UserIDsFromStepSender;
                        insRouting.UserIDsGroupFromField = routing.UserIDsGroupFromField;
                        insRouting.UserNames = routing.UserNames;
                        insRouting.UserIDsFromSql = routing.UserIDsFromSql;
                        insRouting.Value = routing.Value;
                        insRouting.VariableSet = routing.VariableSet;
                        insRouting.SignatureType = routing.SignatureType;
                        insRouting.ExcludeUser = routing.ExcludeUser;

                        //管理ID的处理-消息接收人取自环节
                        insRouting.MsgUserIDsFromStep = string.Join(",", insSteps.Where(c => c.DefStepID == routing.MsgUserIDsFromStep).Select(c => c.ID));
                        insRouting.MsgUserIDsFromStepExec = string.Join(",", insSteps.Where(c => c.DefStepID == routing.MsgUserIDsFromStepExec).Select(c => c.ID));
                        insRouting.MsgUserIDsFromStepSender = string.Join(",", insSteps.Where(c => c.DefStepID == routing.MsgUserIDsFromStepSender).Select(c => c.ID));
                        //管理ID的处理-任务接收人取自环节
                        insRouting.UserIDsFromStep = string.Join(",", insSteps.Where(c => c.DefStepID == routing.UserIDsFromStep).Select(c => c.ID));
                        insRouting.UserIDsFromStepExec = string.Join(",", insSteps.Where(c => c.DefStepID == routing.UserIDsFromStepExec).Select(c => c.ID));
                        insRouting.UserIDsFromStepSender = string.Join(",", insSteps.Where(c => c.DefStepID == routing.UserIDsFromStepSender).Select(c => c.ID));

                        #endregion
                    }
                }

                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        #endregion

        #region 克隆流程定义实例

        public string CloneInsDefFlow(string insDefFlowID, string insFlowIds)
        {
            if (insDefFlowID.EndsWith("-"))
                return insDefFlowID;
            var newInsDefFlow = new S_WF_InsDefFlow();
            try
            {
                var suffer = "_" + insFlowIds.GetHashCode().ToString() + "-"; //新流程定义实例后缀

                var insDefFlow = entities.S_WF_InsDefFlow.SingleOrDefault(c => c.ID == insDefFlowID);
                FormulaHelper.UpdateModel(newInsDefFlow, insDefFlow);
                newInsDefFlow.ID = newInsDefFlow.ID + suffer;
                newInsDefFlow.ModifyTime = DateTime.Now;//新克隆的流程，修改时间为现在
                entities.S_WF_InsDefFlow.Add(newInsDefFlow);

                foreach (var insStep in insDefFlow.S_WF_InsDefStep.ToArray())
                {
                    var newStep = new S_WF_InsDefStep();
                    FormulaHelper.UpdateModel(newStep, insStep);
                    newInsDefFlow.ViewConfig = newInsDefFlow.ViewConfig.Replace(insStep.ID, insStep.ID + suffer);

                    newStep.ID = newStep.ID + suffer; //环节ID增加短横线
                    newStep.InsDefFlowID = newStep.InsDefFlowID + suffer; //流程定义实例ID增加短横线
                    newStep.WaitingStepIDs = append(newStep.WaitingStepIDs, suffer); //等待环节增加短横线
                    entities.S_WF_InsDefStep.Add(newStep);

                    foreach (var insRouting in insStep.S_WF_InsDefRouting.ToArray())
                    {
                        var newRouting = new S_WF_InsDefRouting();
                        FormulaHelper.UpdateModel(newRouting, insRouting);
                        newInsDefFlow.ViewConfig = newInsDefFlow.ViewConfig.Replace(insRouting.ID, insRouting.ID + suffer);

                        newRouting.ID = newRouting.ID + suffer;//路由ID增加短横线
                        newRouting.EndID = newRouting.EndID + suffer;//
                        newRouting.InsDefFlowID = newRouting.InsDefFlowID + suffer;//
                        newRouting.InsDefStepID = newRouting.InsDefStepID + suffer;//                  

                        if (!String.IsNullOrEmpty(insRouting.MsgUserIDsFromStep) && !insRouting.MsgUserIDsFromStep.Contains(suffer))
                            newRouting.MsgUserIDsFromStep = append(newRouting.MsgUserIDsFromStep.Substring(0, 36), suffer);

                        if (!String.IsNullOrEmpty(insRouting.MsgUserIDsFromStepSender) && !insRouting.MsgUserIDsFromStepSender.Contains(suffer))
                            newRouting.MsgUserIDsFromStepSender = append(newRouting.MsgUserIDsFromStepSender.Substring(0, 36), suffer);

                        if (!String.IsNullOrEmpty(insRouting.MsgUserIDsFromStepExec) && !insRouting.MsgUserIDsFromStepExec.Contains(suffer))
                            newRouting.MsgUserIDsFromStepExec = append(newRouting.MsgUserIDsFromStepExec.Substring(0, 36), suffer);

                        //任务接收人
                        if (!String.IsNullOrEmpty(insRouting.UserIDsFromStep) && !insRouting.UserIDsFromStep.Contains(suffer))
                            newRouting.UserIDsFromStep = append(newRouting.UserIDsFromStep.Substring(0, 36), suffer);

                        //任务执行人
                        if (!String.IsNullOrEmpty(insRouting.UserIDsFromStepExec) && !insRouting.UserIDsFromStepExec.Contains(suffer))
                            newRouting.UserIDsFromStepExec = append(newRouting.UserIDsFromStepExec.Substring(0, 36), suffer);

                        //任务发起人
                        if (!String.IsNullOrEmpty(insRouting.UserIDsFromStepSender) && !insRouting.UserIDsFromStepSender.Contains(suffer))
                            newRouting.UserIDsFromStepSender = append(newRouting.UserIDsFromStepSender.Substring(0, 36), suffer);
                        entities.S_WF_InsDefRouting.Add(newRouting);
                    }
                }

                var flows = entities.S_WF_InsFlow.Where(c => insFlowIds.Contains(c.ID)).ToArray();
                foreach (var flow in flows)
                {
                    flow.InsDefFlowID = flow.InsDefFlowID + suffer;//替换ID

                    foreach (var task in flow.S_WF_InsTask)
                    {
                        task.InsDefStepID = task.InsDefStepID + suffer; //替换ID
                        task.DoBackRoutingID = task.DoBackRoutingID + suffer; //替换ID
                        task.WaitingRoutings = append(task.WaitingRoutings, suffer);//替换ID
                        task.WaitingSteps = append(task.WaitingSteps, suffer);//替换ID                        
                    }
                    foreach (var taskExec in flow.S_WF_InsTaskExec)
                    {
                        taskExec.ExecRoutingIDs = append(taskExec.ExecRoutingIDs, suffer);//替换ID
                        taskExec.WeakedRoutingIDs = append(taskExec.WeakedRoutingIDs, suffer);//替换ID                  
                    }
                }
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
            return newInsDefFlow.ID;
        }

        public void CopyInsDefFlowTo(string insDefFlowID)
        {
            var defID = insDefFlowID.Substring(0, 36);
            var suffer = insDefFlowID.Substring(36);

            var flows = entities.S_WF_InsFlow.Where(c => c.InsDefFlowID == defID).ToArray();
            foreach (var flow in flows)
            {
                flow.InsDefFlowID = flow.InsDefFlowID + suffer;//替换ID

                foreach (var task in flow.S_WF_InsTask)
                {
                    task.InsDefStepID = task.InsDefStepID + suffer; //替换ID
                    task.DoBackRoutingID = task.DoBackRoutingID + suffer; //替换ID
                    task.WaitingRoutings = append(task.WaitingRoutings, suffer);//替换ID
                    task.WaitingSteps = append(task.WaitingSteps, suffer);//替换ID

                }
                foreach (var taskExec in flow.S_WF_InsTaskExec)
                {
                    taskExec.ExecRoutingIDs = append(taskExec.ExecRoutingIDs, suffer);//替换ID
                    taskExec.WeakedRoutingIDs = append(taskExec.WeakedRoutingIDs, suffer);//替换ID                  
                }

                var routings = new List<S_WF_InsDefRouting>();
                if (flow.S_WF_InsDefFlow == null)
                {
                    var flowInsDefine = this.entities.Set<S_WF_InsDefFlow>().Find(flow.InsDefFlowID);
                    if (flowInsDefine != null)
                    {
                        routings = flowInsDefine.S_WF_InsDefRouting.ToList();
                    }
                }
                else
                {
                    routings = flow.S_WF_InsDefFlow.S_WF_InsDefRouting.ToList();
                }
                foreach (var rounting in routings)
                {
                    if (!String.IsNullOrEmpty(rounting.MsgUserIDsFromStep) && !rounting.MsgUserIDsFromStep.Contains(suffer))
                        rounting.MsgUserIDsFromStep = append(rounting.MsgUserIDsFromStep.Substring(0, 36), suffer);

                    if (!String.IsNullOrEmpty(rounting.MsgUserIDsFromStepSender) && !rounting.MsgUserIDsFromStepSender.Contains(suffer))
                        rounting.MsgUserIDsFromStepSender = append(rounting.MsgUserIDsFromStepSender.Substring(0, 36), suffer);

                    if (!String.IsNullOrEmpty(rounting.MsgUserIDsFromStepExec) && !rounting.MsgUserIDsFromStepExec.Contains(suffer))
                        rounting.MsgUserIDsFromStepExec = append(rounting.MsgUserIDsFromStepExec.Substring(0, 36), suffer);


                    //任务接收人
                    if (!String.IsNullOrEmpty(rounting.UserIDsFromStep) && !rounting.UserIDsFromStep.Contains(suffer))
                        rounting.UserIDsFromStep = append(rounting.UserIDsFromStep.Substring(0, 36), suffer);

                    //任务执行人
                    if (!String.IsNullOrEmpty(rounting.UserIDsFromStepExec) && !rounting.UserIDsFromStepExec.Contains(suffer))
                        rounting.UserIDsFromStepExec = append(rounting.UserIDsFromStepExec.Substring(0, 36), suffer);

                    //任务发起人
                    if (!String.IsNullOrEmpty(rounting.UserIDsFromStepSender) && !rounting.UserIDsFromStepSender.Contains(suffer))
                        rounting.UserIDsFromStepSender = append(rounting.UserIDsFromStepSender.Substring(0, 36), suffer);

                }

            }

            entities.SaveChanges();

        }

        /// <summary>
        /// 字符串数组追加短横线
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string append(string str, string suffer)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            var arr = str.Split(',');
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = arr[i] + suffer;
            }
            return string.Join(",", arr);
        }

        #endregion

        #region 流程图说明

        public string GetJsonValue(JEnumerable<JToken> jToken, string key)
        {
            IEnumerator enumerator = jToken.GetEnumerator();
            while (enumerator.MoveNext())
            {
                JToken jc = (JToken)enumerator.Current;
                if (jc is JObject || ((JProperty)jc).Value is JObject)
                {
                    return GetJsonValue(jc.Children(), key);
                }
                else
                {
                    if (((JProperty)jc).Name == key)
                    {
                        return ((JProperty)jc).Value.ToString();
                    }
                }
            }
            return null;
        }
        public string GetDescription(Enum en)
        {
            Type type = en.GetType();   //获取类型
            MemberInfo[] memberInfos = type.GetMember(en.ToString());   //获取成员
            if (memberInfos != null && memberInfos.Length > 0)
            {
                DescriptionAttribute[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];   //获取描述特性

                if (attrs != null && attrs.Length > 0)
                {
                    return attrs[0].Description;    //返回当前描述
                }
            }
            return en.ToString();
        }

        public string FlowChart(string id, string tmplCode)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                var insDefFlow = entities.S_WF_InsDefFlow.Where(c => c.ID == id).SingleOrDefault();
                JObject res = JsonConvert.DeserializeObject<JObject>(insDefFlow.ViewConfig);
                List<S_WF_InsDefRouting> routingList = new List<S_WF_InsDefRouting>();

                routingList = GetRoutingList(id);
                foreach (var item in res)
                {
                    if (item.Key != "paths")
                    {
                        str.Append("{");
                        str.Append(item.Key + ":" + item.Value + ",");
                    }
                    else
                    {
                        str.Append("paths:{");
                        int count = 1;
                        foreach (var items in item.Value)
                        {
                            string text = "流程未启用无法获取执行人", selectModeName = "", imageText = "\uf071";
                            string lineID = GetJsonValue(items.Children(), "lineID");
                            StringBuilder textLink = new StringBuilder();
                            textLink.Append("\"textLink\":[");
                            if (!string.IsNullOrEmpty(lineID))
                            {
                                var routing = routingList.Where(c => c.ID == lineID).SingleOrDefault();
                                if (routing != null)
                                {
                                    List<string> list = new List<string>();
                                    RoutingParams param = new RoutingParams();
                                    try
                                    {
                                        param = GetRoutingParams(routing, null, "", false);
                                    }
                                    catch (Exception)
                                    {

                                    }
                                    if (!string.IsNullOrEmpty(param.userIDs))
                                    {
                                        foreach (string userID in param.userIDs.Split(','))
                                        {
                                            var userInfo = FormulaHelper.GetUserInfoByID(userID);
                                            if (userInfo != null)
                                            {
                                                list.Add(userInfo.UserName);
                                            }
                                        }
                                        if (list.Count > 0)
                                        {
                                            text = string.Join(",", list.ToArray());
                                            imageText = string.Format("\uf007({0})", list.Count.ToString());
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(param.selectMode))
                                    {
                                        foreach (var en in param.selectMode.Split(','))
                                        {
                                            FlowSelectUserMode flowSelectUserMode = (FlowSelectUserMode)Enum.Parse(typeof(FlowSelectUserMode), en);
                                            switch (flowSelectUserMode)
                                            {
                                                case FlowSelectUserMode.SelectOneUser:
                                                    selectModeName = GetDescription(FlowSelectUserMode.SelectOneUser);
                                                    break;
                                                case FlowSelectUserMode.SelectMultiUser:
                                                    selectModeName = GetDescription(FlowSelectUserMode.SelectMultiUser);
                                                    break;
                                                case FlowSelectUserMode.SelectOneUserInScope:
                                                    selectModeName = GetDescription(FlowSelectUserMode.SelectOneUserInScope);
                                                    break;
                                                case FlowSelectUserMode.SelectMultiUserInScope:
                                                    selectModeName = GetDescription(FlowSelectUserMode.SelectMultiUserInScope);
                                                    break;
                                                case FlowSelectUserMode.SelectOneOrg:
                                                    selectModeName = GetDescription(FlowSelectUserMode.SelectOneOrg);
                                                    break;
                                                case FlowSelectUserMode.SelectMultiOrg:
                                                    selectModeName = GetDescription(FlowSelectUserMode.SelectMultiOrg);
                                                    break;
                                                //case FlowSelectUserMode.SelectDoback:
                                                //    selectModeName = GetDescription(FlowSelectUserMode.SelectDoback);
                                                //    break;
                                                case FlowSelectUserMode.SelectMultiPrjUser:
                                                    selectModeName = GetDescription(FlowSelectUserMode.SelectMultiPrjUser);
                                                    break;
                                                case FlowSelectUserMode.SelectOnePrjUser:
                                                    selectModeName = GetDescription(FlowSelectUserMode.SelectOnePrjUser);
                                                    break;
                                            }
                                            list.Add(selectModeName);
                                            textLink.Append("{");
                                            textLink.Append(string.Format("\"onclick\":{0}", JsonHelper.ToJson(param).ToString()));
                                            textLink.Append("}");
                                        }
                                        if (list.Count > 0)
                                        {
                                            text = string.Join(",", list.ToArray());
                                            imageText = string.Format("\uf03a");
                                        }
                                    }
                                    else if (!string.IsNullOrEmpty(param.userIDsFromField) || !string.IsNullOrEmpty(param.userIDsGroupFromField))
                                    {
                                        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                                        DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_UI_Form where Code='{0}'", tmplCode));
                                        if (dt.Rows.Count > 0)
                                        {
                                            var formItems = JsonHelper.ToObject<List<Base.Logic.Model.UI.Form.FormItem>>(dt.Rows[0]["Items"].ToString());
                                            string[] fieldStr;
                                            if (!string.IsNullOrEmpty(param.userIDsFromField))
                                            {
                                                fieldStr = param.userIDsFromField.Split(',');
                                                text = "执行人取自表单({0})";
                                            }
                                            else
                                            {
                                                fieldStr = param.userIDsGroupFromField.Split(',');
                                                text = "执行人分组取自表单({0})";
                                            }
                                            foreach (string field in fieldStr)
                                            {
                                                foreach (var formItem in formItems)
                                                {
                                                    if (formItem.Code == field)
                                                    {
                                                        list.Add(formItem.Name);
                                                    }
                                                }
                                            }
                                        }

                                        if (list.Count > 0)
                                        {
                                            text = string.Format(text, string.Join(",", list.ToArray()));
                                            imageText = string.Format("\uf0f6");
                                        }
                                    }
                                    else if (param.flowComplete)
                                    {
                                        imageText = "";
                                    }

                                }
                            }

                            textLink.Append("],");
                            textLink.Append("\"fullName\": \"" + text + "\"");
                            str.Append(items.ToString().Replace("\"text\": \"\"", "\"text\": \"" + imageText + "\"," + textLink.ToString()));
                            if (count != item.Value.Count())
                            {
                                str.Append(",");
                            }
                            count++;
                        }
                        str.Append("}}");
                    }
                }
            }
            catch (Exception ex)
            {
                string info = string.Format(@"流程图说明FlowChart,参数(id:{0},tmplCode:{1})<br>
                    出错原因分析:<br>
                        1.参数id或tmplCode为空<br>
                        2.找不到流程定义S_WF_InsDefFlow.ViewConfig或者流程还没有升级，ViewConfig必须是json格式的<br>
                    ", id, tmplCode);
                throw new FlowException(info, ex);
            }
            return str.ToString();
        }

        #endregion

    }


}
