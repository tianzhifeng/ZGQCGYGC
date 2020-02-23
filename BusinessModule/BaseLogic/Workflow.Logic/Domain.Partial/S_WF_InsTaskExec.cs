using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula.Helper;
using Formula;
using Config;
using Formula.Exceptions;
using System.Configuration;
using System.Threading;

namespace Workflow.Logic.Domain
{
    public partial class S_WF_InsTaskExec
    {
        private bool _created = false;
        public void Complete(S_WF_InsTask ss=null, S_WF_InsFlow sf = null, string defStepID = "", string s_WF_InsDefFlowCode = "")
        {
            //判断是否需要推送到钉钉待办
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutTxtTaskApi"]))
            {
                Thread thread = new Thread(new ParameterizedThreadStart(delegate { ToDD("Audit", ss, sf, defStepID, s_WF_InsDefFlowCode); }));
                thread.Start();
            }
        }
        public void Create(S_WF_InsTask ss, S_WF_InsFlow sf = null, string defStepID = "", string s_WF_InsDefFlowCode="")
        {
            //TODO:先判断 
            if (_created || !string.IsNullOrEmpty(ss.WaitingRoutings) || !string.IsNullOrEmpty(ss.WaitingSteps))
                return;
            _created = true;
            //判断是否需要推送到钉钉待办
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutTxtTaskApi"]))
            {
                Thread thread = new Thread(new ParameterizedThreadStart(delegate { ToDD("Create", ss, sf, defStepID, s_WF_InsDefFlowCode); }));
                thread.Start();
            }
        }
        public void Delete(S_WF_InsTask ss = null, S_WF_InsFlow sf = null, string defStepID = "", string s_WF_InsDefFlowCode = "")
        {
            //判断是否需要推送到钉钉待办
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutTxtTaskApi"]))
            {
                Thread thread = new Thread(new ParameterizedThreadStart(delegate { ToDD("Cannel", ss, sf, defStepID, s_WF_InsDefFlowCode); }));
                thread.Start();
            }
        }

        void ToDD(string Action, S_WF_InsTask ss = null, S_WF_InsFlow sf = null, string defStepID = "",string s_WF_InsDefFlowCode="")
        {
            ITaskOutCenterService iotte = FormulaHelper.GetService<ITaskOutCenterService>(); 
            if (ss == null)
            {
                ss = this.S_WF_InsTask;
            }
            if (sf == null)
            {
                sf = this.S_WF_InsFlow;
            }
            if (string.IsNullOrEmpty(defStepID))
            {
                defStepID = ss.S_WF_InsDefStep.DefStepID;
            }
            if (string.IsNullOrEmpty(s_WF_InsDefFlowCode))
            {
                s_WF_InsDefFlowCode = this.S_WF_InsFlow.S_WF_InsDefFlow.Code;
            }
            iotte.TxtTask(this.ID, s_WF_InsDefFlowCode, sf.FlowName, ss.InsFlowID, sf.CreateTime.GetValueOrDefault(), sf.FormInstanceID, sf.Status, ss.Type, Action, defStepID, sf.CreateUserName, sf.CreateUserID, this.ExecUserID, this.ExecUserName);
            //iotte.TxtTask(this, Action, ss);
        }

        #region Execute
        public void Execute(S_WF_InsDefRouting routing, string execComment, string allBranchRoutings)
        {
            var user = FormulaHelper.GetUserInfo();
            this.Execute(routing, execComment, allBranchRoutings, user);
        }
        public void Execute(S_WF_InsDefRouting routing, string execComment, string allBranchRoutings, UserInfo user)
        {
            try
            {
                //弱控路由处理方式
                if (routing.Type == RoutingType.Weak.ToString())
                {
                    //弱控任务留痕 2014-11-20
                    this.WeakedRoutingIDs = StringHelper.Include(this.WeakedRoutingIDs, routing.ID);

                    var newTaskExec = new S_WF_InsTaskExec();
                    FormulaHelper.UpdateModel(newTaskExec, this);
                    this.S_WF_InsTask.S_WF_InsTaskExec.Add(newTaskExec);
                    newTaskExec.ID = FormulaHelper.CreateGuid();
                    newTaskExec.ExecTime = DateTime.Now;
                    newTaskExec.ExecComment = execComment;
                    newTaskExec.TimeConsuming = 0;
                    newTaskExec.ExecRoutingIDs = routing.ID;

                    return;//停止处理
                }
                this.ExecComment = execComment;

                if (string.IsNullOrEmpty(this.ExecRoutingIDs))
                    this.ExecRoutingIDs = routing.ID;
                else
                {
                    this.ExecRoutingIDs = string.Join(",", (this.ExecRoutingIDs + "," + routing.ID).Split(',').Distinct());
                    this.ExecRoutingName = routing.Name;
                }

                RoutingType routingType = (RoutingType)Enum.Parse(typeof(RoutingType), routing.Type);


                switch (routingType)
                {
                    case RoutingType.Normal:
                        this.ExecTime = DateTime.Now;
                        break;
                    case RoutingType.Branch:
                        if (routing.ID == allBranchRoutings)
                        {
                            this.ExecTime = DateTime.Now;
                        }
                        else
                        {
                            if (StringHelper.HasAll(this.ExecRoutingIDs.Split(','), allBranchRoutings.Split(',')))
                                this.ExecTime = DateTime.Now;
                        }
                        break;
                    case RoutingType.SingleBranch:
                        this.ExecTime = DateTime.Now;
                        break;
                    case RoutingType.Weak:
                        break;
                    case RoutingType.AntiWeak:
                        this.ExecTime = DateTime.Now;
                        break;
                    case RoutingType.Distribute:
                        this.ExecTime = DateTime.Now;
                        break;
                    case RoutingType.Back:
                        this.ExecTime = DateTime.Now;
                        break;
                    case RoutingType.WithdrawOther:
                        this.ExecTime = DateTime.Now;
                        break;
                }

                if (this.ExecTime != null)
                {
                    this.Complete(this.S_WF_InsTask, this.S_WF_InsFlow, this.S_WF_InsTask.S_WF_InsDefStep.DefStepID, this.S_WF_InsFlow.S_WF_InsDefFlow.Code);
             
                    this.ExecUserID = user.UserID;
                    this.ExecUserName = user.UserName;

                    var task = this.S_WF_InsTask;
                    if (task == null)
                        throw new FlowException("当前任务不存在或已执行!");
                    if (!string.IsNullOrEmpty(task.TaskUserIDsGroup)) //如果是组单人通过，则
                    {
                        List<Dictionary<string, string>> listDic = JsonHelper.ToObject<List<Dictionary<string, string>>>(this.S_WF_InsTask.TaskUserIDsGroup);
                        var arr = listDic.Where(c => c["UserID"] == this.TaskUserID).Select(c => c["GroupName"]).ToArray();

                        var entities = FormulaHelper.GetEntities<WorkflowEntities>();

                        foreach (string userid in listDic.Where(c => arr.Contains(c["GroupName"])).Select(c => c["UserID"]))
                        {
                            if (listDic.Count(c => c["UserID"] == userid && arr.Contains(c["GroupName"]) == false) == 0)//不在其它组中
                            {
                                var exec = task.S_WF_InsTaskExec.Where(c => c.TaskUserID == userid && c.TaskUserID != this.TaskUserID).SingleOrDefault();
                                if (exec != null)
                                    entities.S_WF_InsTaskExec.Remove(exec);
                            }
                        }
                    }

                    //计算耗时
                    var canlendarService = FormulaHelper.GetService<ICalendarService>();
                    this.TimeConsuming = canlendarService.GetWorkHourCount((DateTime)this.CreateTime, (DateTime)this.ExecTime);

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
