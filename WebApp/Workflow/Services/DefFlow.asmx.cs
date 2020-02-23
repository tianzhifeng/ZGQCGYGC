using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Formula;
using Workflow.Logic.Domain;
using Config;
using System.Data;
using Formula.Helper;
using Workflow.Logic;

namespace Base.Services
{
    /// <summary>
    /// DefFlowService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class DefFlowService : System.Web.Services.WebService
    {

        WorkflowEntities entities = FormulaHelper.GetEntities<WorkflowEntities>();

        #region 流程跟踪
        [WebMethod]
        public string GetFlowTraceLayoutByFormInstanceID(string formInstanceID)
        {
            return entities.S_WF_InsFlow.Where(c => c.FormInstanceID == formInstanceID).SingleOrDefault().S_WF_InsDefFlow.ViewConfig;
        }

        [WebMethod]
        public string GetFlowTrace(string formInstanceID)
        {
            string sql = @"
select S_WF_InsTaskExec.ID as ID
,S_WF_InsTaskExec.CreateTime as CreateTime
,TaskUserName
,ExecUserName
,ExecTime
,ExecComment
,S_WF_InsTaskExec.Type as Type
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserNames
,FlowName
,FlowCategory
,FlowSubCategory
,S_WF_InsDefStep.Name as StepName
,S_WF_InsDefStep.ID as StepID
,ExecRoutingIDs
from S_WF_InsTaskExec
right join S_WF_InsTask on InsTaskID=S_WF_InsTask.ID
join S_WF_InsFlow on S_WF_InsTask.InsFlowId=S_WF_InsFlow.ID
join S_WF_InsDefStep on InsDefStepID=S_WF_InsDefStep.ID
where FormInstanceID='{0}' and (S_WF_InsTask.WaitingSteps is null or S_WF_InsTask.WaitingSteps='') and  (S_WF_InsTask.WaitingRoutings is null or S_WF_InsTask.WaitingRoutings='')
order by ExecTime desc
";

            sql = string.Format(sql, formInstanceID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");

            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            return JsonHelper.ToJson(dt);
            //return entities.S_WF_InsFlow.Where(c => c.FormInstanceID == formInstanceID).SingleOrDefault().S_WF_InsTask
            //      .Where(c => c.Status == "Processing" && string.IsNullOrEmpty(c.WaitingRoutings) && string.IsNullOrEmpty(c.WaitingSteps))
            //      .Select(c => c.InsDefStepID).ToArray();
        }

        #endregion

        [WebMethod]
        public string GetFlowLayOut(string defineID)
        {
            string result = entities.S_WF_DefFlow.Where(c => c.ID == defineID).SingleOrDefault().ViewConfig;
            return result;
        }

        [WebMethod]
        public string SaveFlowLayOut(string defineID, string xml)
        {
            string result = string.Empty;
            try
            {
                var flowDef = entities.S_WF_DefFlow.Where(c => c.ID == defineID).SingleOrDefault();
                flowDef.ViewConfig = xml;
                entities.SaveChanges();

                result = "T:" + defineID;
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);

                if (exp.InnerException != null)
                    result = "F:" + defineID + ":" + exp.InnerException.Message;
                else
                    result = "F:" + defineID + ":" + exp.Message;
            }
            return result;
        }

        [WebMethod]
        public string NewActivity(string defineID, string guid, string name, string type)
        {
            string result = "T";
            try
            {

                var flowDefine = entities.S_WF_DefFlow.Where(c => c.ID == defineID).SingleOrDefault();

                if (flowDefine == null)
                    throw new Exception("指定的工作流定义未找到");

                S_WF_DefStep step = new S_WF_DefStep();
                step.ID = guid.ToString();
                step.Type = type;
                step.Name = name;
                step.SortIndex = flowDefine.S_WF_DefStep.Count;
                step.DefFlowID = defineID;
                if (type == "Completion")
                    step.Phase = "End";
                else
                    step.Phase = "Processing";
                step.CooperationMode = TaskCooperationMode.Single.ToString();
                step.AllowDoBackFirst = "1";
                step.AllowDoBackFirstReturn = "0";
                if (type == "Inital")
                {
                    step.HideAdvice = "1";
                }
                entities.S_WF_DefStep.Add(step);
                entities.SaveChanges();


                result = "T:" + guid;
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);

                if (exp.InnerException != null)
                    result = "F:" + guid + ":" + exp.InnerException.Message;
                else
                    result = "F:" + guid + ":" + exp.Message;
            }
            return result;
        }

        [WebMethod]
        public string DeleteActivity(string acitivtyIDs)
        {
            string result = "T";
            try
            {
                var stepIDs = acitivtyIDs.Split(',');
                //entities.S_WF_DefStep.Delete(c => stepIDs.Contains(c.ID));
                var steps = entities.S_WF_DefStep.Where(c => stepIDs.Contains(c.ID)).ToArray();

                var allSteps = steps[0].S_WF_DefFlow.S_WF_DefStep.ToArray();
                foreach (var step in steps)
                {
                    //移除等待环节
                    foreach (var item in allSteps)
                    {
                        item.WaitingStepIDs = StringHelper.Exclude(item.WaitingStepIDs, step.ID);
                    }
                    //删除环节
                    entities.S_WF_DefStep.Remove(step);
                    //删除路由
                    entities.S_WF_DefRouting.Delete(c => c.EndID == step.ID);
                }
                entities.SaveChanges();
                result = "T:" + acitivtyIDs;
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);
                if (exp.InnerException != null)
                    result = "F:" + acitivtyIDs + ":" + exp.InnerException.Message;
                else
                    result = "F:" + acitivtyIDs + ":" + exp.Message;
            }
            return result;
        }

        [WebMethod]
        public string NewRule(string defineID, string guid, string beginActivityID, string endAcitivityID, string name)
        {
            string result = "T";
            try
            {
                var flowDef = entities.S_WF_DefFlow.Where(c => c.ID == defineID).SingleOrDefault();
                if (flowDef == null)
                    throw new Exception("指定的工作流定义未找到");
                var startStep = entities.S_WF_DefStep.Where(c => c.ID == beginActivityID).SingleOrDefault();
                var endStep = entities.S_WF_DefStep.Where(c => c.ID == endAcitivityID).SingleOrDefault();

                var routing = new S_WF_DefRouting();
                routing.ID = guid;
                routing.DefStepID = startStep.ID;
                routing.EndID = endStep.ID;
                routing.DefFlowID = defineID;
                routing.SortIndex = startStep.S_WF_DefRouting.Count;
                routing.Type = RoutingType.Normal.ToString();
                routing.Name = "送" + endStep.Name;
                routing.AllowDoBack = "1";//默认允许打回  2014-12-1
                routing.SaveForm = "1"; //默认自动保存表单 2014-12-1
                routing.MustInputComment = "1"; //默认弹出意见框 2014-12-1
                routing.AllowWithdraw = "1"; //默认允许撤销 2014-12-1
                entities.S_WF_DefRouting.Add(routing);

                if (routing.Name == "送结束")
                {
                    routing.Name = "结束";
                    routing.SelectMode = "";
                }

                entities.SaveChanges();

                result = "T:" + guid;
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);
                if (exp.InnerException != null)
                    result = "F:" + guid + ":" + exp.InnerException.Message;
                else
                    result = "F:" + guid + ":" + exp.Message;
            }
            return result;
        }

        [WebMethod]
        public string DeleteRule(string ruleIDs)
        {
            string result = "T";
            try
            {
                var routingIDs = ruleIDs.Split(',');
                var routings = entities.S_WF_DefRouting.Where(c => routingIDs.Contains(c.ID)).ToArray();
                foreach (var routing in routings)
                {
                    entities.S_WF_DefRouting.Remove(routing);
                }
                entities.SaveChanges();

                result = "T:" + ruleIDs;
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);
                if (exp.InnerException != null)
                    result = "F:" + ruleIDs + ":" + exp.InnerException.Message;
                else
                    result = "F:" + ruleIDs + ":" + exp.Message;
            }
            return result;
        }
    }
}
