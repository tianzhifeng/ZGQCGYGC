using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Config;
using Formula.Exceptions;
using Formula.Helper;
using System.Web;
using System.Text.RegularExpressions;

namespace Workflow.Logic.Domain
{
    public partial class S_WF_InsDefStep
    {
        #region 创建任务
        public S_WF_InsTask CreateTask(S_WF_InsFlow flow, S_WF_InsTask fromTask, S_WF_InsDefRouting routing, string taskUserIDs, string taskUserNames, string taskUserIDsGroup, string taskRoleIDs, string taskOrgIDs)
        {
            UserInfo user = FormulaHelper.GetUserInfo();
            return this.CreateTask(flow, fromTask, routing, taskUserIDs, taskUserNames, taskUserIDsGroup, taskRoleIDs, taskOrgIDs, user);
        }
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="fromTask"></param>
        /// <param name="routing"></param>
        /// <param name="taskUserIDs"></param>
        /// <param name="taskUserNames"></param>
        /// <param name="taskUserIDsGroup"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public S_WF_InsTask CreateTask(S_WF_InsFlow flow, S_WF_InsTask fromTask, S_WF_InsDefRouting routing, string taskUserIDs, string taskUserNames, string taskUserIDsGroup, string taskRoleIDs, string taskOrgIDs, UserInfo user)
        {           
            try
            {
                var task = new S_WF_InsTask();
                string senderTaskUserIDs = user.UserID;
                string senderTaskUserNames = user.UserName;
                if (fromTask != null && fromTask.S_WF_InsDefStep.CooperationMode == TaskCooperationMode.All.ToString() && routing != null && routing.Type != RoutingType.Distribute.ToString())
                {
                    senderTaskUserIDs = string.Join(",", fromTask.S_WF_InsTaskExec.Select(c => c.ExecUserID).ToArray());
                    senderTaskUserNames = string.Join(",", fromTask.S_WF_InsTaskExec.Select(c => c.ExecUserName).ToArray());
                }

                string complete = FlowTaskStatus.Complete.ToString();

                //所有分支路由使用同一个任务
                if (routing != null && routing.Type == RoutingType.Branch.ToString())
                {
                    var preTask = flow.S_WF_InsTask.Where(c => c.InsDefStepID == this.ID && c.Status != complete).LastOrDefault();
                    if (preTask != null && preTask.TaskUserIDs == taskUserIDs)
                    {
                        if (preTask.WaitingRoutings.Contains(routing.ID))//等待汇聚的任务
                        {
                            preTask.WaitingRoutings = StringHelper.Exclude(preTask.WaitingRoutings, routing.ID);
                            preTask.SendTaskIDs = StringHelper.Include(preTask.SendTaskIDs, fromTask.ID);
                            preTask.SendTaskUserIDs = StringHelper.Include(preTask.SendTaskUserIDs, senderTaskUserIDs);
                            preTask.SendTaskUserNames = StringHelper.Include(preTask.SendTaskUserNames, senderTaskUserNames);
                        }
                        else //非等待汇聚任务（一般由分支分出的任务）
                        {
                            preTask.SendTaskIDs = StringHelper.Include(preTask.SendTaskIDs, fromTask.ID);
                            preTask.SendTaskUserIDs = StringHelper.Include(preTask.SendTaskUserIDs, senderTaskUserIDs);
                            preTask.SendTaskUserNames = StringHelper.Include(preTask.SendTaskUserNames, senderTaskUserNames);
                            preTask.CreateTime = DateTime.Now; //重现修改创建时间
                        }
                        //设置任务的紧急度
                        if (FormulaHelper.ContextGetValueString("Urgency") == "true")
                            preTask.Urgency = "1";
                        else
                            preTask.Urgency = "0";
                        return preTask;
                    }
                    else
                    {
                        string branch = RoutingType.Branch.ToString();
                        //增加汇聚路由的过滤                    
                        var routings = this.S_WF_InsDefFlow.S_WF_InsDefRouting.Where(c => c.Type == branch && c.EndID == this.ID && c.Name == routing.Name && c.ID != routing.ID).ToList();
                        routings = FilterRouting(routings, flow);
                        task.WaitingRoutings = string.Join(",", routings.Select(c => c.ID).ToArray());
                    }
                }
                else if (routing != null && routing.Type != RoutingType.Distribute.ToString()) //非分支路由，任务执行人不变，不创建新任务
                {
                    var preTask = flow.S_WF_InsTask.Where(c => c.InsDefStepID == this.ID && c.Status != complete).LastOrDefault();
                    if (preTask != null && preTask.TaskUserIDs == taskUserIDs)
                    {
                        preTask.SendTaskIDs = StringHelper.Include(preTask.SendTaskIDs, fromTask.ID);
                        preTask.SendTaskUserIDs = StringHelper.Include(preTask.SendTaskUserIDs, senderTaskUserIDs);
                        preTask.SendTaskUserNames = StringHelper.Include(preTask.SendTaskUserNames, senderTaskUserNames);
                        //设置任务的紧急度
                        if (FormulaHelper.ContextGetValueString("Urgency") == "true")
                            preTask.Urgency = "1";
                        else
                            preTask.Urgency = "0";

                        //重新打开弱控按钮（弱控路由）
                        string strWeakType = RoutingType.Weak.ToString();
                        var weakRouting = this.S_WF_InsDefRouting.Where(c => c.EndID == routing.InsDefStepID && c.Type == strWeakType).SingleOrDefault();
                        if (weakRouting != null)
                        {
                            foreach (var item in preTask.S_WF_InsTaskExec)
                            {
                                item.WeakedRoutingIDs = StringHelper.Exclude(item.WeakedRoutingIDs, weakRouting.ID);
                            }
                        }

                        return preTask;
                    }
                }

                //设置任务的紧急度
                if (FormulaHelper.ContextGetValueString("Urgency") == "true")
                    task.Urgency = "1";
                else
                    task.Urgency = "0";

                if (this.Type == StepTaskType.Normal.ToString() && string.IsNullOrEmpty(taskUserIDs))
                    throw new FlowException("路由的执行人配置不正确");

                task.ID = FormulaHelper.CreateGuid();
                task.CreateTime = DateTime.Now;
                task.SendTaskIDs = fromTask == null ? "" : fromTask.ID;
                task.SendTaskUserIDs = senderTaskUserIDs;
                task.SendTaskUserNames = senderTaskUserNames;
                task.TaskUserIDs = taskUserIDs;
                task.TaskUserNames = taskUserNames;
                task.TaskUserIDsGroup = taskUserIDsGroup;
                task.TaskRoleIDs = taskRoleIDs;
                task.TaskOrgIDs = taskOrgIDs;
                task.Status = FlowTaskStatus.Processing.ToString();

                task.InsDefStepID = this.ID;
                task.InsFlowID = flow.ID;
                task.Type = this.Type;
                task.TaskName = flow.GetInsName(this.S_WF_InsDefFlow.TaskNameTemplete).Replace("{StepName}", this.Name).Replace("{FlowName}", this.S_WF_InsDefFlow.Name);
                task.TaskCategory = flow.GetInsName(this.S_WF_InsDefFlow.TaskCategoryTemplete).Replace("{StepName}", this.Name).Replace("{FlowName}", this.S_WF_InsDefFlow.Name);
                task.TaskSubCategory = flow.GetInsName(this.S_WF_InsDefFlow.TaskSubCategoryTemplete).Replace("{StepName}", this.Name).Replace("{FlowName}", this.S_WF_InsDefFlow.Name);

                flow.S_WF_InsTask.Add(task);
                task.S_WF_InsFlow = flow;
                task.S_WF_InsDefStep = this;

                //结束环节不要任务执行人
                if (task.Type == StepTaskType.Completion.ToString())
                {
                    task.TaskUserIDs = "";
                    task.TaskUserNames = "";
                    task.TaskUserIDsGroup = "";
                    task.TaskRoleIDs = "";
                    task.TaskOrgIDs = "";
                }

                //子流程任务执行人分配给系统
                if (task.Type == StepTaskType.SubFlow.ToString())
                {
                    task.TaskUserIDs = "system";
                    task.TaskUserNames = "系统";
                    task.TaskUserIDsGroup = "";
                    task.TaskRoleIDs = "";
                    task.TaskOrgIDs = "";
                }

                //任务的等待环节           
                foreach (var t in flow.S_WF_InsTask.Where(c => c.CompleteTime == null))
                {
                    if (this.WaitingStepIDs.Split(',').Contains(t.InsDefStepID))
                        task.WaitingSteps += "," + t.InsDefStepID;
                }
                task.WaitingSteps = task.WaitingSteps.Trim(',');
                //更新其它等待本环节的任务
                foreach (var item in flow.S_WF_InsTask.Where(c => c.CompleteTime == null && c.ID != task.ID && c.S_WF_InsDefStep.WaitingStepIDs.Contains(this.ID)))
                {
                    if (item.WaitingSteps.Contains(this.ID) == false)
                    {
                        item.WaitingSteps += "," + this.ID;
                        item.WaitingSteps.Trim(',');
                    }
                }

                //当环节为组协作完成且用户没有分组时,按组织角色分组 2013-8
                if (this.CooperationMode == TaskCooperationMode.GroupSingle.ToString() && string.IsNullOrEmpty(task.TaskUserIDsGroup))
                {
                    throw new FlowException("环节设置为组单人完成，但执行用户无法分组");

                    //if (string.IsNullOrEmpty(task.TaskRoleIDs) || string.IsNullOrEmpty(task.TaskOrgIDs))
                    //    throw new FlowException("环节设置为组单人完成，但执行用户无法分组");

                    //List<Object> list = new List<object>();

                    //IUserService userService = FormulaHelper.GetService<IUserService>();

                    //foreach (string userID in task.TaskUserIDs.Split(','))
                    //{
                    //    foreach (string roleID in task.TaskRoleIDs.Split(','))
                    //    {
                    //        foreach (string orgID in task.TaskOrgIDs.Split(','))
                    //        {
                    //            if (userService.InRole(userID, roleID, orgID))
                    //            {
                    //                string groupName = string.Format("{0}.{1}", roleID, orgID).GetHashCode().ToString();
                    //                list.Add(new { GroupName = groupName, UserID = userID });
                    //            }
                    //        }
                    //    }
                    //}
                    //task.TaskUserIDsGroup = JsonHelper.ToJson(list);
                }

                //记录打回直送
                if (routing != null) //首环节时，routing为null
                {
                    if (routing.AllowDoBack == "1" || routing.OnlyDoBack == "1")
                        task.DoBackRoutingID = routing.ID;
                    if (routing.OnlyDoBack == "1")
                        task.OnlyDoBack = "1";
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        #endregion

        #region 私有方法

        #region 汇聚路由的过滤

        private List<S_WF_InsDefRouting> FilterRouting(List<S_WF_InsDefRouting> routings, S_WF_InsFlow flow)
        {
            for (int i = routings.Count - 1; i >= 0; i--)
            {
                var routing = routings[i];
                if (!string.IsNullOrEmpty(routing.AuthFormData))
                {
                    if (HasFormDataAuth(flow, routing.AuthFormData) == false)
                    {
                        routings.Remove(routing);
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(routing.AuthVariable))
                {
                    if (HasVariableAuth(flow.ID, routing.AuthVariable) == false)
                    {
                        routings.Remove(routing);
                        continue;
                    }
                }
            }

            return routings;
        }

        private bool HasFormDataAuth(S_WF_InsFlow flow, string authFormData)
        {
            if (string.IsNullOrEmpty(authFormData))
                return true;
            var formDic = flow.FormDic;

            var user = FormulaHelper.GetUserInfo();

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
                    default:
                        if (!formDic.ContainsKey(value))
                            throw new Exception(string.Format("权限过滤出错，字段:“{0}”不存在", value));
                        return formDic[value] == null ? "" : formDic[value].ToString();
                }
            });

            //string sql = string.Format("select count(1) where {0}", authFormData);
            //为了兼容Oracle修改
            string sql = string.Format("select count(1) from S_WF_DefFlow where {0}", authFormData);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            object obj = sqlHelper.ExecuteScalar(sql);
            return Convert.ToInt32(obj) > 0;
        }

        public bool HasVariableAuth(string flowID, string variableAuth)
        {
            if (string.IsNullOrEmpty(variableAuth))
                return true;

            string fieldNames = "";
            foreach (string item in variableAuth.Split(new string[] { "and", "or" }, StringSplitOptions.RemoveEmptyEntries))
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

        #endregion
    }
}
