using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using Formula;
using System.Data;
using Formula.Helper;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using Workflow.Logic;
using System.Web.Script.Serialization;

namespace MvcConfig.Areas.Workflow.Controllers
{
    public class TaskController : BaseAutoFormController
    {
        public static Dictionary<string, RoutingParams> routingList = new Dictionary<string, RoutingParams>();
        public JsonResult GetMyTaskTree()
        {
            #region 待办
            Dictionary<string, object> dicUndo = new Dictionary<string, object>();
            dicUndo["value"] = "Undo";
            dicUndo["text"] = "待办任务";
            dicUndo["name"] = "待办任务";
            dicUndo["iconCls"] = "Undo";
            dicUndo["type"] = "Status";
            List<Dictionary<string, object>> undoCategory = GetCategorys();
            dicUndo["children"] = undoCategory;
            #endregion

            #region 已办
            Dictionary<string, object> dicDone = new Dictionary<string, object>();
            dicDone["value"] = "Done";
            dicDone["text"] = "已办任务";
            dicDone["name"] = "已办任务";
            dicDone["iconCls"] = "Done";
            dicDone["type"] = "Status";
            List<Dictionary<string, object>> doneCategory = GetCategorys();
            dicDone["children"] = doneCategory;
            #endregion

            #region 已委托
            Dictionary<string, object> dicDelegate = new Dictionary<string, object>();
            dicDelegate["value"] = "Delegate";
            dicDelegate["text"] = "已委托任务";
            dicDelegate["name"] = "已委托任务";
            dicDelegate["iconCls"] = "Delegate";
            dicDelegate["type"] = "Status";
            #endregion

            #region 设计
            Dictionary<string, object> dicDesign = new Dictionary<string, object>();
            dicDesign["value"] = "Design";
            dicDesign["text"] = "设计任务";
            dicDesign["name"] = "设计任务";
            dicDesign["iconCls"] = "Design";
            dicDesign["type"] = "Status";
            #endregion

            List<Dictionary<string, object>> treeData = new List<Dictionary<string, object>>();
            treeData.Add(dicUndo);
            treeData.Add(dicDone);
            treeData.Add(dicDelegate);
            treeData.Add(dicDesign);

            return Json(treeData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTimeIntervals()
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Type source = typeof(TimeInterval);
            foreach (string name in Enum.GetNames(source))
            {
                //object enumValue = Enum.Parse(source, name);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string enumDesc = GetEnumDesc(source, name);
                dic["title"] = enumDesc;
                dic["name"] = name;
                list.Add(dic);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUndoCategoryCount()
        {
            string sql = @"select count(S_WF_InsTaskExec.ID) as eCount,S_WF_InsDefFlow.CategoryID
                            from S_WF_InsTaskExec with(nolock) 
                            join S_WF_InsTask with(nolock) on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='') and S_WF_InsTask.ID=InsTaskID
                            join S_WF_InsFlow with(nolock) on S_WF_InsFlow.Status in('Processing','Complete') and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
                            join S_WF_InsDefFlow with(nolock) on InsDefFlowID=S_WF_InsDefFlow.ID
                            join S_WF_InsDefStep with(nolock) on InsDefStepID = S_WF_InsDefStep.ID
                            group by S_WF_InsDefFlow.CategoryID
                            ";
            sql = string.Format(sql, FormulaHelper.UserID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            dt.Columns["eCount"].ColumnName = "Count";
            int total = 0;
            foreach (DataRow row in dt.Rows)
            {
                total += int.Parse(row["Count"].ToString());
            }
            dt.Rows.Add(total, "");
            return Json(dt, JsonRequestBehavior.AllowGet);

        }

        #region 改版任务中心
        public ActionResult MyTaskCenter()
        {
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            ViewBag.NomalTaskCount = "";
            ViewBag.DesignTaskCount = "";
            ViewBag.Title = LGID ? "My Task" : "我的任务";
            ViewBag.Task = LGID ? "T<br />a<br />s<br />k<br />" : "待<br />办<br />任<br />务<br />";
            ViewBag.Completed = LGID ? "F<br />i<br />n<br />i<br />s<br />h<br />" : "已<br />办<br />任<br />务<br />";
            ViewBag.Replace = LGID ? "R<br />e<br />p<br />l<br />a<br />c<br />e<br />" : "替<br />代<br />任<br />务<br />";
            ViewBag.Daily = LGID ? "D<br />a<br />i<br />l<br />y<br />" : "日<br />常<br />任<br />务<br />";
            return View();
        }

        public void ViewBagTitle()
        {
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            ViewBag.urgent = LGID ? "Urgent" : "紧急度";
            ViewBag.taskName = LGID ? "Task Name" : "任务名称";
            ViewBag.taskType = LGID ? "Task Type" : "任务类型";
            ViewBag.flowName = LGID ? "Flow Name" : "流程名称";
            ViewBag.flowClass = LGID ? "Category" : "流程分类";
            ViewBag.sendTime = LGID ? "Send Time" : "发送时间";
            ViewBag.sender = LGID ? "Sender" : "发送人";
            ViewBag.recipient = LGID ? "Recipient" : "接收人";
            ViewBag.flow = LGID ? "Flow" : "流程";
            ViewBag.detail = LGID ? "Query" : "详细查询";
            ViewBag.ok = LGID ? "Submit" : "查询";
            ViewBag.reset = LGID ? "Reset" : "清空";
            ViewBag.exeTime = LGID ? "Execute Time" : "执行时间";
            ViewBag.curExecutor = LGID ? "Current Executor" : "当前执行人";
            ViewBag.linkState = LGID ? "Node State" : "环节状态";
            ViewBag.entrustment = LGID ? "Entrustment" : "被委托人";
            ViewBag.PCD = LGID ? "Plan Finish Date" : "计划完成日期";
            ViewBag.CCD = LGID ? "Confirm Finish Date" : "确认完成日期";
            ViewBag.AFD = LGID ? "Actual Finish Date" : "实际完成日期";
            ViewBag.completeState = LGID ? "Finish State" : "完成状态";
            ViewBag.GoToUp = LGID ? "Up" : "置顶";
            //快速查询提示
            ViewBag.KeyEmptyText = LGID ? "Keywords" : "请输入关键字";
        }

        public ActionResult MyDelegateList()
        {
            ViewBagTitle();
            return View();
        }

        public ActionResult MyUndoList()
        {
            ViewBagTitle();
            return View();
        }

        public ActionResult MyDoneList()
        {
            ViewBagTitle();
            return View();
        }

        public ActionResult MyNormalTaskList()
        {
            ViewBagTitle();
            return View();
        }

        public JsonResult GetMyUndoList(QueryBuilder qb, string queryTabData)
        {
            List<Dictionary<string, string>> ht = JsonHelper.ToObject<List<Dictionary<string, string>>>(queryTabData);
            var flowCategory = getQueryValue(ht, "FlowCategory");

            string sql = @"select S_WF_InsTaskExec.ID as ID
,InsDefStepID
,S_WF_InsDefFlow.Name as DefFlowName,
S_WF_InsDefStep.Name as DefStepName
,S_WF_InsTaskExec.ID as TaskExecID
,S_WF_InsTaskExec.Type as TaskExecType
,S_WF_InsTask.ID as TaskID
,S_WF_InsTask.InsDefStepID as StepID
,S_WF_InsTask.InsFlowID as FlowID
,S_WF_InsTask.Type as TaskType
,S_WF_InsTask.Urgency
,S_WF_InsTask.GoToUp
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserIDs
,SendTaskUserNames
,S_WF_InsTaskExec.TaskUserID
,S_WF_InsTaskExec.TaskUserName
,S_WF_InsTask.Status as Status
,S_WF_InsTask.CreateTime as CreateTime
,FormInstanceID
,FlowName
,FlowCategory
,FlowSubCategory
,S_WF_DefFlow.Code
,S_WF_InsFlow.CurrentStep
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
from S_WF_InsTaskExec with(nolock) 
join S_WF_InsTask with(nolock) on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='') and S_WF_InsTask.ID=InsTaskID
join S_WF_InsFlow with(nolock) on S_WF_InsFlow.Status in('Processing','Complete') and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
join S_WF_InsDefFlow with(nolock) on InsDefFlowID=S_WF_InsDefFlow.ID
join S_WF_InsDefStep with(nolock) on InsDefStepID = S_WF_InsDefStep.ID
left join S_WF_DefSubForm with(nolock) on SubFormID=S_WF_DefSubForm.ID
left join S_WF_DefFlow with(nolock) on S_WF_DefFlow.ID = S_WF_InsDefFlow.DefFlowID
";
            sql = string.Format(sql, FormulaHelper.UserID);
            if (!string.IsNullOrEmpty(flowCategory))
                sql += string.Format(" where S_WF_InsFlow.FlowCategory IN ('{0}') ", flowCategory.TrimEnd(',').Replace(",", "','"));

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        /// <summary>
        /// 获取第一个按钮的参数
        /// </summary>
        /// <param name="taskExecID"></param>
        /// <param name="formInstanceID"></param>
        /// <returns></returns>
        private RoutingParams GetFirstButtonRoutingParams(string id, string formInstanceID, string taskExecID)
        {
            if (routingList.ContainsKey(id))
            {
                return routingList[id];
            }
            else
            {
                List<ButtonInfo> lists = flowService.JsonGetFlowButtons(formInstanceID, taskExecID);
                if (lists.Count > 0)
                {
                    var click = lists.First().onclick;
                    System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"(?<={).*?(?=})");
                    System.Text.RegularExpressions.MatchCollection mc = reg.Matches(click);
                    foreach (System.Text.RegularExpressions.Match m in mc) { click = "{" + m.Value + "}"; }
                    JavaScriptSerializer json = new JavaScriptSerializer();
                    RoutingParams routingParam = json.Deserialize<RoutingParams>(click);
                    routingList.Add(id, routingParam);
                    return routingParam;
                }
                else { return null; }
            }
        }

        /// <summary>
        /// 批量审批详细
        /// </summary>
        /// <param name="taskExecID"></param>
        /// <param name="formInstanceID"></param>
        /// <returns></returns>
        public string DetailRouting()
        {
            List<string> texts = new List<string>();
            string str = "";
            string selectJson = Request["data"];
            JavaScriptSerializer json = new JavaScriptSerializer();
            List<FlowTask> list = json.Deserialize<List<FlowTask>>(selectJson);
            foreach (var item in list)
            {
                RoutingParams routing = GetFirstButtonRoutingParams(item.ID, item.FormInstanceID, item.TaskExecID);
                texts.Add(item.TaskName + "-路由(" + routing.routingName + ")");
            }
            if (texts.Count > 0)
            {
                for (var i = 0; i < texts.Count; i++)
                {
                    str += "<div style='text-align:left'>" + texts[i] + "</div>";
                }
                str = "<div class='scrollbar' style='max-height:200px;  overflow-y:auto;'>" + str + "</div>";
            }
            return str;
        }
        /// <summary>
        /// 批量审批
        /// </summary>
        /// <param name="taskExecID"></param>
        /// <param name="formInstanceID"></param>
        /// <returns></returns>
        public int BatchApproval()
        {
            int success = 0;
            Dictionary<string, string> dic = null;
            string selectJson = Request["data"];
            JavaScriptSerializer json = new JavaScriptSerializer();
            List<FlowTask> list = json.Deserialize<List<FlowTask>>(selectJson);
            foreach (var item in list)
            {
                try
                {
                    RoutingParams routing = GetFirstButtonRoutingParams(item.ID, item.FormInstanceID, item.TaskExecID);
                    DataTable table = GetModelDT(item.FormInstanceID, false, item.TaskExecID, item.Code);
                    if (!(string.IsNullOrEmpty(routing.defaultComment) && routing.mustInputComment == true) && CheckBatchApproval(item.InsDefStepID, table))
                    {
                        if (!string.IsNullOrEmpty(routing.orgIDFromField))
                        {
                            List<string> userIDs = routing.userIDs.Split(',').ToList();
                            List<string> fieldIds = routing.userIDsFromField.Split(',').ToList();
                            for (var i = 0; i < fieldIds.Count; i++)
                            {
                                var ids = table.Rows[0][fieldIds[i]].ToString().Split(',');
                                for (var j = 0; j < ids.Length; j++)
                                {
                                    if (!userIDs.Contains(ids[j]))
                                    {
                                        userIDs.Add(ids[j]);
                                    }
                                }
                            }
                            routing.userIDs = string.Join(",", userIDs.ToArray());
                            if (routing.userIDs.IndexOf(',') == 0)
                                routing.userIDs = routing.userIDs.Substring(1);

                        }
                        if (!string.IsNullOrEmpty(routing.orgIDFromField) || !string.IsNullOrEmpty(routing.roleIDsFromField))
                        {
                            if (routing.selectMode != "SelectOneOrg" && routing.selectMode != "SelectMultiOrg")
                            {
                                routing.userIDs = GetUserIDs(routing.roleIDs, routing.orgIDs, "");
                            }
                        }

                        var nextExecUserIDs = routing.userIDs.Trim().TrimEnd(',').TrimStart(',');
                        dic = SubmitBatchApproval(item.FormInstanceID, routing.routingID, item.TaskExecID, nextExecUserIDs, "", routing.roleIDs, routing.orgIDs, routing.defaultComment, item.Code);
                        if (dic.Keys.Count > 0)
                        {
                            dic.Add("DefaultComment", routing.defaultComment);

                            var ID = dic["ID"];
                            var NextTaskExecID = dic["NextTaskExecID"];
                            var NextRoutingID = dic["NextRoutingID"];
                            var NextRoutingName = dic["NextRoutingName"];
                            var NextExecUserIDs = dic["NextExecUserIDs"];
                            var DefaultComment = dic["DefaultComment"];
                            if (!string.IsNullOrEmpty(NextRoutingID))
                            {
                                SubmitBatchApproval(ID, NextRoutingID, NextTaskExecID, NextExecUserIDs, "", "", "", DefaultComment, item.Code);
                            }
                        }
                        success += 1;
                    }
                }
                catch (Exception ex)
                {

                }

            }

            return success;
        }


        /// <summary>
        /// 已办任务
        /// </summary>
        /// <param name="qb"></param>
        /// <param name="queryData"></param>
        /// <returns></returns>
        public JsonResult GetMyDoneList(QueryBuilder qb, string queryTabData)
        {
            List<Dictionary<string, string>> ht = JsonHelper.ToObject<List<Dictionary<string, string>>>(queryTabData);
            var timeInterval = getQueryValue(ht, "CreateTime");
            var flowCategory = getQueryValue(ht, "FlowCategory");

            string sql = @"
            select S_WF_InsTaskExec.ID as ID
            ,S_WF_InsTaskExec.ID as TaskExecID
            ,S_WF_InsTask.ID as TaskID
            ,S_WF_InsTaskExec.Type as TaskExecType
            ,S_WF_InsTask.InsDefStepID as StepID
            ,S_WF_InsTask.InsFlowID as FlowID
            ,S_WF_InsTask.Type as TaskType
            ,S_WF_InsTask.Urgency
            ,S_WF_InsTask.GoToUp
            ,TaskName
            ,TaskCategory
            ,TaskSubCategory
            ,SendTaskUserIDs
            ,SendTaskUserNames
            ,S_WF_InsTaskExec.TaskUserID
            ,S_WF_InsTaskExec.TaskUserName
            ,S_WF_InsTask.Status as Status
            ,S_WF_InsTask.CreateTime as CreateTime
            ,S_WF_InsTaskExec.ExecTime
            ,FormInstanceID
            ,FlowName
            ,FlowCategory
            ,FlowSubCategory
            ,S_WF_InsFlow.CurrentStep as CurrentStepX
            ,case when isnull(S_WF_InsFlow.CurrentUserNames,'')!='' then S_WF_InsFlow.CurrentUserNames+'('+S_WF_InsFlow.CurrentStep+')' else '' end as CurrentStep
            ,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
            ,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
            ,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
            from S_WF_InsTaskExec with(nolock) 
            join S_WF_InsTask with(nolock) on ExecTime is not null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (S_WF_InsTaskExec.CreateTime >= {2}'{1}') and S_WF_InsTask.ID=InsTaskID
            join S_WF_InsFlow with(nolock) on S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
            join S_WF_InsDefFlow with(nolock) on InsDefFlowID=S_WF_InsDefFlow.ID
            join S_WF_InsDefStep with(nolock) on InsDefStepID = S_WF_InsDefStep.ID
            left join S_WF_DefSubForm with(nolock) on SubFormID=S_WF_DefSubForm.ID
            ";

            DateTime? start = null;
            DateTime? end = null;

            bool isHistory = false;//更早（一年前）
            if (!string.IsNullOrEmpty(timeInterval))
            {
                DateTime?[] startEnd = GetStartEnd((TimeInterval)Enum.Parse(typeof(TimeInterval), timeInterval));
                start = startEnd[0];
                end = startEnd[1];

                if (timeInterval == TimeInterval.MoreEarlier.ToString())
                    isHistory = true;
            }
            if (!string.IsNullOrEmpty(flowCategory) || start != null || end != null)
            {
                string where = " where 1=1 ";
                if (!string.IsNullOrEmpty(flowCategory))
                    where += string.Format(" AND (S_WF_InsFlow.FlowCategory IN ('{0}')) ", flowCategory.TrimEnd(',').Replace(",", "','"));

                if (start != null)
                    where += string.Format(" AND (S_WF_InsTaskExec.CreateTime >= '{0}')", Convert.ToDateTime(start).ToString("yyyy-MM-dd"));

                if (end != null)
                {
                    where += string.Format(" AND (S_WF_InsTaskExec.CreateTime <  '{0}')", Convert.ToDateTime(end).ToString("yyyy-MM-dd"));
                }

                sql += where;
            }
            if (isHistory == true)//历史只查五年以内,避免效率问题
                sql = string.Format(sql, FormulaHelper.UserID, DateTime.Now.AddYears(-5).ToString("yyyy-MM-dd"), Config.Constant.IsOracleDb ? "date" : "");
            else
                sql = string.Format(sql, FormulaHelper.UserID, DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd"), Config.Constant.IsOracleDb ? "date" : "");

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }
        public JsonResult GetMyDelegateList(QueryBuilder qb)
        {
            string sql = @"select S_WF_InsTaskExec.ID as ID
,InsDefStepID
,S_WF_InsTaskExec.ID as TaskExecID
,S_WF_InsTaskExec.Type as TaskExecType
,S_WF_InsTask.ID as TaskID
,S_WF_InsTask.InsDefStepID as StepID
,S_WF_InsTask.InsFlowID as FlowID
,S_WF_InsTask.Type as TaskType
,S_WF_InsTask.Urgency
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserIDs
,SendTaskUserNames
,S_WF_InsTaskExec.TaskUserName
,S_WF_InsTask.Status as Status
,S_WF_InsTask.CreateTime as CreateTime
,S_WF_InsTaskExec.ExecUserID
,S_WF_InsTaskExec.ExecUserName
,S_WF_InsTaskExec.ExecTime
,FormInstanceID
,FlowName
,FlowCategory
,FlowSubCategory
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
from S_WF_InsTaskExec with(nolock) 
join S_WF_InsTask with(nolock)  on TaskUserID = '{0}' and ExecUserID <> '{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='') and S_WF_InsTask.ID=InsTaskID
join S_WF_InsFlow with(nolock)  on S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
join S_WF_InsDefFlow with(nolock)  on InsDefFlowID=S_WF_InsDefFlow.ID
join S_WF_InsDefStep with(nolock)  on InsDefStepID = S_WF_InsDefStep.ID
left join S_WF_DefSubForm with(nolock)  on SubFormID=S_WF_DefSubForm.ID
";
            sql = string.Format(sql, FormulaHelper.UserID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }
        public string getQueryValue(List<Dictionary<string, string>> ht, string queryField)
        {
            if (ht == null) return "";
            foreach (var item in ht)
            {
                if (item["queryfield"] == queryField)
                    return item["value"];
            }
            return "";
        }
        public static string GetDoneFlowCatalogyEnumData()
        {
            string sql = string.Format(@" select FlowCategory,Count(*) Quantity
 FROM S_WF_InsTaskExec with(nolock)  inner join 
 S_WF_InsFlow with(nolock)  ON S_WF_InsTaskExec.InsFlowID = S_WF_InsFlow.ID
 where ExecTime is not null  AND ExecUserID='{0}'
 GROUP BY FlowCategory", FormulaHelper.UserID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            Hashtable ht = new Hashtable();
            ht["enumkey"] = "FlowCategory";
            ht["title"] = LGID ? "Category" : "流程分类";
            ht["queryfield"] = "FlowCategory";
            ArrayList alItems = new ArrayList();

            foreach (DataRow row in dt.Rows)
            {
                if (string.IsNullOrEmpty(row["FlowCategory"].ToString())) continue;
                var quantity = row["Quantity"].ToString();
                var text = row["FlowCategory"].ToString() + string.Format("({0})", quantity);
                var itemHs = new Hashtable();
                itemHs["value"] = row["FlowCategory"].ToString();
                itemHs["text"] = text;
                itemHs["radio"] = "F";
                alItems.Add(itemHs);
            }
            ht["menus"] = alItems;
            return JsonHelper.ToJson(ht);
        }
        public static string GetUndoFlowCatalogyEnumData()
        {
            string sql = string.Format(@" select FlowCategory,Count(*) Quantity
from S_WF_InsTaskExec with(nolock)
join S_WF_InsTask with(nolock) on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='') and S_WF_InsTask.ID=InsTaskID
join S_WF_InsFlow with(nolock) on S_WF_InsFlow.Status in('Processing','Complete') and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  
join S_WF_InsDefFlow with(nolock) on InsDefFlowID=S_WF_InsDefFlow.ID 
join S_WF_InsDefStep with(nolock) on InsDefStepID = S_WF_InsDefStep.ID 
 GROUP BY FlowCategory", FormulaHelper.UserID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            Hashtable ht = new Hashtable();
            ht["enumkey"] = "FlowCategory";
            ht["title"] = LGID ? "Category" : "流程分类";
            ht["queryfield"] = "FlowCategory";
            ArrayList alItems = new ArrayList();

            foreach (DataRow row in dt.Rows)
            {
                if (string.IsNullOrEmpty(row["FlowCategory"].ToString())) continue;
                var quantity = row["Quantity"].ToString();
                var text = row["FlowCategory"].ToString() + string.Format("({0})", quantity);
                var itemHs = new Hashtable();
                itemHs["value"] = row["FlowCategory"].ToString();
                itemHs["text"] = text;
                itemHs["radio"] = "F";
                alItems.Add(itemHs);
            }
            ht["menus"] = alItems;
            return JsonHelper.ToJson(ht);
        }
        public static string GetTimeIntervalEnumData()
        {
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            Hashtable ht = new Hashtable();
            ht["enumkey"] = "CreateTime";
            ht["title"] = LGID ? "Time" : "任务时间";
            ht["queryfield"] = "CreateTime";
            ArrayList alItems = new ArrayList();

            NameValueCollection nvc = GetNVCFromEnumValue(typeof(TimeInterval));
            if (LGID)
                nvc = GetNVCFromEnumValue(typeof(TimeIntervalEN));
            foreach (string key in nvc)
            {
                Hashtable htItem = new Hashtable();
                htItem["value"] = nvc[key];
                htItem["text"] = key;
                htItem["radio"] = "T";
                alItems.Add(htItem);
            }
            ht["menus"] = alItems;
            return JsonHelper.ToJson(ht);
        }

        public JsonResult GoToUp()
        {
            string TaskID = Request["TaskID"];
            string NumKey = Request["NumKey"];
            if (!string.IsNullOrWhiteSpace(TaskID))
            {
                string GoTopSql = string.Empty;
                SQLHelper WfSqlHelper = SQLHelper.CreateSqlHelper("Workflow");
                if (NumKey.ToLower() == "null")
                {
                    GoTopSql = string.Format(@"UPDATE dbo.S_WF_InsTask SET GoToUp=(SELECT CASE WHEN MAX(GoToUp) IS NULL THEN 0 ELSE MAX(GoToUp)+1 END AS Num FROM dbo.S_WF_InsTask)
WHERE ID='{0}'", TaskID);
                    WfSqlHelper.ExecuteNonQuery(GoTopSql);
                }
                else
                {
                    GoTopSql = string.Format(@"UPDATE dbo.S_WF_InsTask SET GoToUp=NULL WHERE ID='{0}'", TaskID);
                    WfSqlHelper.ExecuteNonQuery(GoTopSql);
                }
            }
            return Json("");
        }
        #endregion

        #region 私有方法

        private List<Dictionary<string, object>> GetCategorys()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            string sql = "select * from S_M_Category with(nolock) ";
            DataTable dt = SQLHelper.CreateSqlHelper("Base").ExecuteDataTable(sql);
            if (dt != null)
            {
                DataRow[] drs = dt.Select("ParentID='0'");
                foreach (DataRow dr in drs)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic["value"] = Convert.ToString(dr["Code"]);
                    dic["text"] = Convert.ToString(dr["Name"]);
                    dic["name"] = Convert.ToString(dr["Name"]);
                    dic["iconCls"] = "Module";
                    dic["category"] = Convert.ToString(dr["ID"]);
                    dic["type"] = "Category";
                    list.Add(dic);
                }
            }
            return list;
        }

        /// <summary>
        /// 读取枚举的Description
        /// </summary>
        /// <param name="source">枚举Type</param>
        /// <param name="enumName">需要读取</param>
        /// <returns></returns>
        private static string GetEnumDesc(Type source, string enumName)
        {
            FieldInfo[] fieldinfo = source.GetFields();
            foreach (FieldInfo item in fieldinfo)
            {
                if (item.Name != enumName) continue;
                Object[] obj = item.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (obj.Length == 0) continue;
                DescriptionAttribute desc = (DescriptionAttribute)obj[0];
                return desc.Description;
            }
            return enumName;
        }

        private DateTime?[] GetStartEnd(TimeInterval ti)
        {
            DateTime? start = null, end = DateTime.Today.AddDays(1);
            switch (ti)
            {
                case TimeInterval.LastThreeDays:
                    start = DateTime.Today.AddDays(-2);
                    break;
                case TimeInterval.LastWeek:
                    start = DateTime.Today.AddDays(-6);
                    break;
                case TimeInterval.LastTwoWeeks:
                    start = DateTime.Today.AddDays(-13);
                    break;
                case TimeInterval.LastMonth:
                    start = DateTime.Today.AddMonths(-1);
                    break;
                case TimeInterval.LastYear:
                    start = DateTime.Today.AddYears(-1);
                    break;
                case TimeInterval.MoreEarlier:
                    start = null;
                    end = DateTime.Today.AddYears(-1);
                    break;
            }
            DateTime?[] startEnd = new DateTime?[2];
            startEnd[0] = start;
            startEnd[1] = end;
            return startEnd;
        }

        /// <summary>
        /// 从枚举类型和它的特性读出并返回一个键值对
        /// </summary>
        /// <param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        /// <returns>键值对</returns>
        private static NameValueCollection GetNVCFromEnumValue(Type enumType)
        {
            NameValueCollection nvc = new NameValueCollection();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    nvc.Add(strText, field.Name);
                }
            }
            return nvc;
        }

        #endregion
    }

    public enum TimeInterval
    {
        [Description("最近三天")]
        LastThreeDays,
        [Description("最近一周")]
        LastWeek,
        [Description("最近两周")]
        LastTwoWeeks,
        [Description("一个月内")]
        LastMonth,
        [Description("一年内")]
        LastYear,
        [Description("更早(一年前)")]
        MoreEarlier,
    }

    public enum TimeIntervalEN
    {
        [Description("Within Three Days")]
        LastThreeDays,
        [Description("Within A Week")]
        LastWeek,
        [Description("Within Two Weeks")]
        LastTwoWeeks,
        [Description("Within A Month")]
        LastMonth,
        [Description("Within A Year")]
        LastYear,
        [Description("A Year Ago")]
        MoreEarlier,
    }

    public class FlowTask
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string InsDefStepID { get; set; }
        public string InsDefFlowID { get; set; }
        public string DefFlowName { get; set; }
        public string DefStepName { get; set; }
        public string TaskExecID { get; set; }
        public string TaskName { get; set; }
        public string TaskUserID { get; set; }
        public string TaskUserName { get; set; }
        public string FormInstanceID { get; set; }
    }

}
