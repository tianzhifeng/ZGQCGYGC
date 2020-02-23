using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using MvcAdapter;
using Formula;
using Workflow.Logic.Domain;
using Workflow.Logic;
using System.Text;
using Workflow.Logic.BusinessFacade;

namespace MvcConfig.Areas.Workflow.Controllers
{
    public class MonitorController : BaseController
    {
        public JsonResult GetTree()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            if (Config.Constant.IsOracleDb)
            {
                DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID, NAME as \"Name\", PARENTID as \"ParentID\", FULLID as \"FullID\" FROM S_M_CATEGORY WHERE FULLID like '{0}%'", "0"));
                return Json(dt, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_M_Category where FullID like '{0}%'", "0"));
                return Json(dt, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            string sql = string.Format(@"select S_WF_InsFlow.FormInstanceID as ID, CreateTime as CREATETIME,CompleteTime as COMPLETETIME, Name as NAME,FlowName as INSTANCENAME,Code as CODE,CreateUserName,formUrl,formWidth,formHeight,InsDefFlowID,S_WF_InsFlow.ID as FlowID,S_WF_InsDefFlow.ModifyTime from S_WF_InsFlow
left join S_WF_InsDefFlow on S_WF_InsDefFlow.ID=InsDefFlowID
 where Status='{0}'", Request["Status"]);

            string categoryID = Request["NodeFullID"].Split('.').Last();
            if (categoryID != "0")
            {
                sql += string.Format(" and CategoryID='{0}'", categoryID);
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        public JsonResult GetTaskExecList()
        {
            string taskID = Request["TaskID"];
            string sql = string.Format("select ID,TaskUserID,TaskUserName,ExecUserID,ExecUserName,ExecTime from S_WF_InsTaskExec where InsTaskID='{0}'", taskID);
            SQLHelper sqlHelpe = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            DataTable dt = sqlHelpe.ExecuteDataTable(sql);
            dt.Columns.Add("Status");
            foreach (DataRow row in dt.Rows)
            {
                if (row["ExecTime"].ToString() == "")
                    row["Status"] = "未执行";
                else
                    row["Status"] = "已执行";
            }
            return Json(dt);
        }

        public JsonResult AddTaskExec(string taskID, string UserID, string userName)
        {
            //创建TaskExec
            S_WF_InsTaskExec taskExec = new S_WF_InsTaskExec();
            taskExec.ID = FormulaHelper.CreateGuid();
            taskExec.InsTaskID = taskID;
            taskExec.CreateTime = DateTime.Now;
            taskExec.TaskUserID = UserID;
            taskExec.TaskUserName = userName;

            //执行人
            taskExec.ExecUserID = taskExec.TaskUserID;
            taskExec.ExecUserName = taskExec.TaskUserName;
            taskExec.Type = TaskExecType.Normal.ToString();

            var entities = FormulaHelper.GetEntities<WorkflowEntities>();
            var task = entities.Set<S_WF_InsTask>().Where(c => c.ID == taskID).SingleOrDefault();
            taskExec.InsFlowID = task.InsFlowID;
            entities.Set<S_WF_InsTaskExec>().Add(taskExec);
            entities.SaveChanges();

            return Json("");
        }

        public JsonResult DelTaskExec(string taskID, string listIDs)
        {
            var entities = FormulaHelper.GetEntities<WorkflowEntities>();
            var task = entities.Set<S_WF_InsTask>().SingleOrDefault(c => c.ID == taskID);
            string[] ids = listIDs.Split(',');

            if (task.S_WF_InsTaskExec.Count() == ids.Count())
                throw new Exception("不能全部删除！");
            foreach (var item in task.S_WF_InsTaskExec.ToArray())
            {
                if (item.ExecTime != null)
                    throw new Exception("已完成的任务不能删除");

                if (ids.Contains(item.ID))
                    entities.Set<S_WF_InsTaskExec>().Remove(item);
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult ReplaceExecUser(string taskExecID, string userID, string userName)
        {
            var entities = FormulaHelper.GetEntities<WorkflowEntities>();
            var exec = entities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
            exec.ExecUserID = userID;
            exec.ExecUserName = userName;
            exec.CreateTime = DateTime.Now; // 换人后修改创建时间，一般推送到移动端
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult Delete(string listIDs)
        {
            SQLHelper sqlWorkflowHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            string sql = "select FormInstanceID,ConnName,TableName from S_WF_InsFlow join S_WF_InsDefFlow on InsDefFlowID=S_WF_InsDefFlow.ID where FormInstanceID in('{0}') order by ConnName";
            sql = string.Format(sql, listIDs.Replace(",", "','"));
            DataTable dt = sqlWorkflowHelper.ExecuteDataTable(sql);

            foreach (DataRow row in dt.Rows)
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(row["ConnName"].ToString());
                sql = string.Format("delete from {0} where ID='{1}'", row["TableName"], row["FormInstanceID"]);
                sqlHelper.ExecuteNonQuery(sql);
            }

            sql = "delete from S_WF_InsFlow where FormInstanceID in ('{0}')";
            sql = string.Format(sql, listIDs.Replace(",", "','"));
            sqlWorkflowHelper.ExecuteDataTable(sql);

            return Json("");
        }

        public string GetDefFlow(string formInstanceID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ViewConfig from dbo.S_WF_InsDefFlow where ID in (select InsDefFlowID from  dbo.S_WF_InsFlow where FormInstanceID='{0}')", formInstanceID));

            return dt.Rows[0]["ViewConfig"].ToString();
        }

        public string FlowChart(string formInstanceID, string tmplCode)
        {
            FlowFO flowFO = new FlowFO();
            return flowFO.FlowChart(formInstanceID, tmplCode);
        }

        public string GetFlowTrace(string formInstanceID)
        {
            string sql = @"
            select S_WF_InsTaskExec.ID as ID
            ,ExecTime
            ,S_WF_InsTaskExec.Type as Type
            ,S_WF_InsDefStep.Type as StepType
            ,case when '{1}'='EN' and S_WF_InsDefStep.NameEN is not null then S_WF_InsDefStep.NameEN else S_WF_InsDefStep.Name end as StepName
            ,S_WF_InsDefStep.ID as StepID
            ,S_WF_InsTask.InsDefStepID as InsDefStepID
            from S_WF_InsTaskExec
            right join S_WF_InsTask on InsTaskID=S_WF_InsTask.ID
            join S_WF_InsFlow on S_WF_InsTask.InsFlowId=S_WF_InsFlow.ID
            join S_WF_InsDefStep on InsDefStepID=S_WF_InsDefStep.ID
            where FormInstanceID='{0}' and (S_WF_InsTask.WaitingSteps is null or S_WF_InsTask.WaitingSteps='') and  (S_WF_InsTask.WaitingRoutings is null or S_WF_InsTask.WaitingRoutings='')";

            var LGID = FormulaHelper.GetCurrentLGID();
            sql = string.Format(sql, formInstanceID, LGID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            string result = "";
            StringBuilder activeRect = new StringBuilder();
            StringBuilder historyNode = new StringBuilder();
            StringBuilder historyRect = new StringBuilder();
            activeRect.Append("{\"activeRects\":{\"rects\":[");
            historyNode.Append("\"historyNodes\":{\"rects\":[");
            historyRect.Append("\"historyRects\":{\"rects\":[");
            //{"activeRects":{"rects":[{"paths":[],"name":"任务3"},{"paths":[],"name":"任务4"},{"paths":[],"name":"任务2"}]},
            //"historyRects":{"rects":[{"paths":["TO 任务1"],"name":"开始"},{"paths":["TO 分支"],"name":"任务1"},{"paths":["TO 任务3","TO 任务4","TO 任务2"],"name":"分支"}]}}
            foreach (DataRow dr in dt.Rows)
            {
                if (string.IsNullOrEmpty(Convert.ToString(dr["ExecTime"])))
                {
                    activeRect.Append("{");
                    activeRect.AppendFormat("\"paths\":[],\"ID\":\"{0}\",\"name\":\"{1}\",\"Type\":\"{2}\"", dr["StepID"].ToString(), dr["StepName"].ToString(), dr["StepType"].ToString());
                    activeRect.Append("},");
                }
                else
                {
                    historyNode.Append("{");
                    historyNode.AppendFormat("\"paths\":[],\"ID\":\"{0}\",\"name\":\"{1}\",\"Type\":\"{2}\"", dr["StepID"].ToString(), dr["StepName"].ToString(), dr["StepType"].ToString());
                    historyNode.Append("},");
                }
            }
            DataTable dtRouting = sqlHelper.ExecuteDataTable(string.Format(@"select c.ID,c.Name,c.InsDefFlowID,c.InsDefStepID,c.EndID,b.ExecTime 
                        from S_WF_InsFlow a,S_WF_InsTaskExec b,S_WF_InsDefRouting c
                        where a.ID=b.InsFlowID and a.InsDefFlowID=c.InsDefFlowID 
                        and a.FormInstanceID='{0}' and b.ExecRoutingIDs like '%'+c.ID+'%'", formInstanceID));
            foreach (DataRow dr in dtRouting.Rows)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dr["ExecTime"])))
                {
                    historyRect.Append("{");
                    historyRect.AppendFormat("\"paths\":[],\"ID\":\"{0}\",\"name\":\"{1}\",\"StepID\":\"{2}\",\"EndID\":\"{3}\"", dr["ID"].ToString(), dr["Name"].ToString(), dr["InsDefStepID"].ToString(), dr["EndID"].ToString());
                    historyRect.Append("},");
                }
            }
            if (dt.Rows.Count > 0)
                result =
                    (activeRect.ToString().Length > 25 ? activeRect.ToString().Substring(0, activeRect.Length - 1) + "]}" : "") +
                    (historyNode.ToString().Length > 25 ? "," + historyNode.ToString().Substring(0, historyNode.Length - 1) + "]}" : "") +
                    (historyRect.ToString().Length > 25 ? "," + historyRect.ToString().Substring(0, historyRect.Length - 1) + "]}" : "") + "}";
            return result;
        }

        #region 编辑流程定义实例（版本）

        public JsonResult CloneInsDefFlow(string insDefId, string flowIds)
        {
            var flowFO = new FlowFO();
            var newInsDefFlowID = flowFO.CloneInsDefFlow(insDefId, flowIds);
            return Json(new { NewInsDefFlowId = newInsDefFlowID });
        }

        public JsonResult CopyInsDefFlowTo(string InsDefFlowID)
        {
            var flowFO = new FlowFO();
            flowFO.CopyInsDefFlowTo(InsDefFlowID);
            return Json(new { InsDefFlowID = InsDefFlowID });
        }

        #endregion

        #region 催办

        public JsonResult UrgeUser(string taskID)
        {
            var entities = FormulaHelper.GetEntities<WorkflowEntities>();
            var task = entities.Set<S_WF_InsTask>().SingleOrDefault(c => c.ID == taskID);


            string content = string.Format("任务：“{0}”比较急，请尽快审批！", task.TaskName);
            string link = "";//不带连接了......
            FormulaHelper.GetService<IMessageService>().SendMsg(content, content, link, "", task.TaskUserIDs, task.TaskUserNames, null, MsgReceiverType.UserType, MsgType.Normal);

            return Json("");
        }

        #endregion

        #region 完成任务

        public JsonResult CompleteTask(string taskID)
        {
            var entities = FormulaHelper.GetEntities<WorkflowEntities>();
            var task = entities.Set<S_WF_InsTask>().SingleOrDefault(c => c.ID == taskID);
            var flow = task.S_WF_InsFlow;

            if (flow.S_WF_InsTask.Count(c => c.ID != taskID && c.Status == "Processing") == 0)
            {
                flow.Status = FlowTaskStatus.Complete.ToString();
                string sql = string.Format("update {0} set FlowPhase='End',StepName='结束' where ID='{1}'", flow.S_WF_InsDefFlow.TableName, flow.FormInstanceID);
                SQLHelper sqlhelper = SQLHelper.CreateSqlHelper(flow.S_WF_InsDefFlow.ConnName);
                sqlhelper.ExecuteNonQuery(sql);
            }

            foreach (var exec in task.S_WF_InsTaskExec)
            {
                if (exec.ExecTime == null)
                {
                    exec.ExecTime = DateTime.Now;
                    exec.ExecRoutingName = "强制完成";
                }
            }
            task.Complete();

            //创建结束环节任务，否则新版流程跟踪出不来
            //创建下一环节任务
            var nextStep = flow.S_WF_InsDefFlow.S_WF_InsDefStep.FirstOrDefault(c => c.Type == "Completion");
            var nextTask = nextStep.CreateTask(task.S_WF_InsFlow, task, null, "", "", "", "", "");            
            nextTask.CreateTaskExec();


            entities.SaveChanges();
            return Json("");
        }

        public JsonResult DobackTask(string taskID)
        {
            var entities = FormulaHelper.GetEntities<WorkflowEntities>();
            var task = entities.Set<S_WF_InsTask>().SingleOrDefault(c => c.ID == taskID);

            var taskList = entities.Set<S_WF_InsTask>().Where(c =>c.InsFlowID==task.InsFlowID && c.CreateTime > task.CreateTime && c.ID != taskID).ToList();
            foreach (var item in taskList)
            {
                entities.Set<S_WF_InsTask>().Remove(item);
            }

            task.CompleteTime = null;
            task.FirstViewTime = null;
            task.Status = FlowTaskStatus.Processing.ToString();
            foreach (var item in task.S_WF_InsTaskExec)
            {
                item.ExecTime = null;
                item.ExecComment = "";
                item.ExecRoutingIDs = "";
                item.ExecRoutingName = "";
            }
            entities.SaveChanges();
            return Json("");
        }

        #endregion
    }
}
