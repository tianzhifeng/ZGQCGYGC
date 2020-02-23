using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Formula.Helper;
using Formula.Exceptions;
using System.Configuration;
using System.Threading;

namespace Workflow.Logic.Domain
{
    public partial class S_WF_InsTask
    {
        #region CreateTaskExec

        public ICollection<S_WF_InsTaskExec> CreateTaskExec()
        {
            try
            {
                var entities = FormulaHelper.GetEntities<WorkflowEntities>();
                string defFlowID = this.S_WF_InsDefStep.S_WF_InsDefFlow.DefFlowID;

                if (string.IsNullOrEmpty(defFlowID))
                    throw new FlowException("流程不存在!");

                var strTaskUserIDs = this.TaskUserIDs.Split(',');
                var strTaskUserNames = this.TaskUserNames.Split(',');
                for (int i = 0; i < strTaskUserIDs.Length; i++)
                {
                    if (string.IsNullOrEmpty(strTaskUserIDs[i]))
                        continue;

                    if (this.S_WF_InsTaskExec.Where(c => c.TaskUserID == strTaskUserIDs[i] && c.ExecTime == null).Count() > 0)
                        continue;

                    //创建TaskExec
                    S_WF_InsTaskExec taskExec = new S_WF_InsTaskExec();
                    taskExec.ID = FormulaHelper.CreateGuid();
                    taskExec.InsTaskID = this.ID;
                    taskExec.CreateTime = DateTime.Now;
                    taskExec.TaskUserID = strTaskUserIDs[i];
                    taskExec.TaskUserName = strTaskUserNames[i];
                    //执行人
                    taskExec.ExecUserID = taskExec.TaskUserID;
                    taskExec.ExecUserName = taskExec.TaskUserName;
                    taskExec.Type = TaskExecType.Normal.ToString();

                    //处理事前委托
                    var flowDelegates = entities.S_WF_DefDelegate.Where(c => c.DefFlowID == defFlowID && c.DelegateUserID == taskExec.TaskUserID && DateTime.Now > c.BeginTime && DateTime.Now < c.EndTime);
                    foreach (var item in flowDelegates)
                    {
                        bool flag = false;
                        if (string.IsNullOrEmpty(item.DelegateRoleID) && string.IsNullOrEmpty(item.DelegateOrgID))
                        {
                            flag = true;
                        }
                        else if (string.IsNullOrEmpty(item.DelegateRoleID))
                        {
                            //if (FormulaHelper.InOrg(taskExec.TaskUserID, item.DelegateOrgID))
                            //    flag = true;
                            if (FormulaHelper.GetService<IUserService>().InOrg(taskExec.TaskUserID, item.DelegateOrgID))
                                flag = true;
                        }
                        else
                        {
                            //if (FormulaHelper.InRole(taskExec.TaskUserID, item.DelegateRoleID, item.DelegateOrgID))
                            //    flag = true;
                            if (FormulaHelper.GetService<IUserService>().InRole(taskExec.TaskUserID, item.DelegateRoleID, item.DelegateOrgID))
                                flag = true;
                        }
                        if (flag)
                        {
                            taskExec.ExecUserID = item.BeDelegateUserID;
                            taskExec.ExecUserName = item.BeDelegateUserName;
                            taskExec.Type = TaskExecType.Delegate.ToString();
                            break;
                        }
                    }

                    taskExec.InsTaskID = this.ID;
                    taskExec.S_WF_InsFlow = this.S_WF_InsFlow;
                    taskExec.InsFlowID = this.S_WF_InsFlow.ID;
                    this.S_WF_InsTaskExec.Add(taskExec);
                    taskExec.Create(this, this.S_WF_InsFlow, this.S_WF_InsDefStep.DefStepID, this.S_WF_InsFlow.S_WF_InsDefFlow.Code);//当作新任务，同步接口
               
                }

                return this.S_WF_InsTaskExec;
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }

        }

        #endregion

        #region Complete

        public void Complete()
        {
            try
            {
                if (this.Status == FlowTaskStatus.Complete.ToString())
                    return;

                this.Status = FlowTaskStatus.Complete.ToString();
                this.CompleteTime = DateTime.Now;
                //计算耗时
                var canlendarService = FormulaHelper.GetService<ICalendarService>();
                this.TimeConsuming = canlendarService.GetWorkHourCount((DateTime)this.CreateTime, (DateTime)this.CompleteTime);

                var entities = FormulaHelper.GetEntities<WorkflowEntities>();

                //删除所有未完成的非传阅任务
                foreach (var item in this.S_WF_InsTaskExec.ToArray())
                {
                    if (this.S_WF_InsDefStep.KeepWhenEnd != "1" && item.ExecTime == null && item.Type != TaskExecType.Circulate.ToString())
                    {
                      
                        item.Delete(item.S_WF_InsTask, item.S_WF_InsFlow, item.S_WF_InsTask.S_WF_InsDefStep.DefStepID, item.S_WF_InsFlow.S_WF_InsDefFlow.Code);//同步接口用
                        entities.S_WF_InsTaskExec.Remove(item);
                    }
                }


                string complete = FlowTaskStatus.Complete.ToString();
                //如果本任务环节的全部任务已经完成,则
                if (this.S_WF_InsFlow.S_WF_InsTask.Where(c => c.InsDefStepID == this.InsDefStepID && c.ID != this.ID && c.Status != complete).Count() == 0)
                {
                    //更新其它任务的等待状态
                    var otherStepTask = this.S_WF_InsFlow.S_WF_InsTask.Where(c => c.InsDefStepID != this.InsDefStepID && c.Status != complete);
                    foreach (var item in otherStepTask)
                    {
                        item.WaitingSteps = StringHelper.Exclude(item.WaitingSteps, this.InsDefStepID);
                        //item.CreateTime = DateTime.Now;
                        //foreach (var taskExec in item.S_WF_InsTaskExec)
                        //    if (taskExec.ExecTime == null)
                        //        taskExec.CreateTime = DateTime.Now;

                        //同步接口
                        if (string.IsNullOrEmpty(item.WaitingRoutings) && string.IsNullOrEmpty(item.WaitingSteps))
                            foreach (var obj in item.S_WF_InsTaskExec)
                                obj.Create(item, item.S_WF_InsFlow, item.S_WF_InsDefStep.DefStepID, item.S_WF_InsFlow.S_WF_InsDefFlow.Code);
                                
                    }
                }

            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message);
            }
        }

        #endregion

    }
}
