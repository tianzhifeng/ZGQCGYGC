using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using System.Text;
using Formula;
using Workflow.Logic.Domain;
using Workflow.Logic;
using Formula.Helper;
using Base.Logic.Domain;
using System.Configuration;


namespace MvcConfig.Areas.Workflow.ControllersGetFlowExecList
{
    public class TraceController : BaseController
    {

        string sqlFlowExecList = @"
select S_WF_InsTaskExec.ID as ID
,S_WF_InsTaskExec.CreateTime as CreateTime
,TaskUserID
,TaskUserName
,ExecUserID
,ExecUserName
,ExecTime
,ExecComment
,S_WF_InsTaskExec.Type as Type
,S_WF_InsTask.ID as TaskID
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserNames
,FlowName
,FlowCategory
,FlowSubCategory
,case when '{1}'='EN' then isnull(S_WF_InsDefStep.NameEN, S_WF_InsDefStep.Name) else S_WF_InsDefStep.Name end as StepName
,S_WF_InsDefStep.ID as StepID
,ExecRoutingIDs
,ExecRoutingName
,S_WF_InsFlow.InsDefFlowID
,S_WF_InsTask.DoBackRoutingID
,S_WF_InsTask.OnlyDoBack
,S_WF_InsTaskExec.InsTaskID
,S_WF_InsTaskExec.ApprovalInMobile
from S_WF_InsTaskExec
right join S_WF_InsTask on InsTaskID=S_WF_InsTask.ID
join S_WF_InsFlow on S_WF_InsTask.InsFlowId=S_WF_InsFlow.ID
join S_WF_InsDefStep on InsDefStepID=S_WF_InsDefStep.ID
where FormInstanceID='{0}' and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='')
order by isnull(S_WF_InsTaskExec.CreateTime,S_WF_InsTask.CreateTime),S_WF_InsTaskExec.ID
";

        #region GetFlowExecList

        public JsonResult GetFlowExecList(string id,MvcAdapter.QueryBuilder qb)
        {
            if (string.IsNullOrEmpty(id))
                return Json("[]");
            var LGID = FormulaHelper.GetCurrentLGID();
            string sql = string.Format(sqlFlowExecList, id, LGID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");

            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            if (dt.Rows.Count == 0)
                return Json(dt);

            string insDefFlowID = dt.Rows[0]["InsDefFlowID"].ToString();
            sql = string.Format("select ID,Name from S_WF_InsDefRouting where InsDefFlowID='{0}'", insDefFlowID);
            DataTable dtRouting = sqlHelper.ExecuteDataTable(sql, qb);

            dt.Columns.Add("UseTime");
            dt.Columns.Add("TaskUserDept");

            var userService = FormulaHelper.GetService<IUserService>();

            foreach (DataRow row in dt.Rows)
            {
                string ExecRoutingIDs = row["ExecRoutingIDs"].ToString().Trim(',');
                if (!string.IsNullOrEmpty(ExecRoutingIDs) && row["ExecRoutingName"].ToString() == "")
                {
                    row["ExecRoutingName"] = dtRouting.AsEnumerable().SingleOrDefault(c => c["ID"].ToString() == ExecRoutingIDs.Split(',').LastOrDefault())["Name"];
                }
                //处理打回和直送操作的名称
                if (string.IsNullOrEmpty(ExecRoutingIDs) && row["ExecRoutingName"].ToString() == "" && row["ExecTime"].ToString() != "")
                {
                    if (row["Type"].ToString() == TaskExecType.Normal.ToString() || row["Type"].ToString() == TaskExecType.Delegate.ToString())
                    {
                        if (row["DoBackRoutingID"].ToString() != "")
                            row["ExecRoutingName"] = "驳回";
                        if (row["OnlyDoBack"].ToString() == "1")
                            row["ExecRoutingName"] = "送驳回人";
                    }
                    else if (row["Type"].ToString() == TaskExecType.Circulate.ToString())
                    {
                        row["ExecRoutingName"] = "阅毕";
                    }
                    else if (row["Type"].ToString() == TaskExecType.Ask.ToString())
                    {
                        row["ExecRoutingName"] = "阅毕";
                    }

                }

                string CreateTime = row["CreateTime"].ToString();
                string ExecTime = row["ExecTime"].ToString();
                if (!string.IsNullOrEmpty(ExecTime))
                {
                    if (!string.IsNullOrEmpty(row["ApprovalInMobile"].ToString()))
                    {
                        switch (row["ApprovalInMobile"].ToString())
                        {
                            case "1":
                                row["ApprovalInMobile"] = "";
                                break;
                            case "2":
                                row["ApprovalInMobile"] = "钉钉";
                                break;
                            case "3":
                                row["ApprovalInMobile"] = "移动通";
                                break;
                            case "5":
                                row["ApprovalInMobile"] = "微信";
                                break;
                            case "6":
                                row["ApprovalInMobile"] = "轮巡工具自动通过";
                                break;
                            default:
                                row["ApprovalInMobile"] = "PC";
                                break;
                        }
                    }
                    else
                    {
                        row["ApprovalInMobile"] = "PC";
                    }
                    var span = DateTime.Parse(ExecTime) - DateTime.Parse(CreateTime);
                    row["UseTime"] = string.Format("{0}小时{1}分", span.Days * 24 + span.Hours, span.Minutes == 0 ? 1 : span.Minutes);
                }
                if (row["TaskUserID"].ToString() != "")
                {
                    var ogUser = userService.GetUserInfoByID(row["TaskUserID"].ToString());
                    if (ogUser != null)
                    {
                        row["TaskUserDept"] = ogUser.UserOrgName;
                    }
                }
                else
                {
                    row["TaskUserName"] = "";
                    row["ExecUserName"] = "";
                }
                //操作意见取最新回复
                var entities = FormulaHelper.GetEntities<BaseEntities>();
                string routingID = row["ID"].ToString();
                string execComment = row["ExecComment"].ToString();
                var msgBody = entities.Set<S_S_MsgBody>().Where(c => c.FlowMsgID == routingID).OrderByDescending(c => c.SendTime);
                if (msgBody.Count() > 0)
                {
                    execComment = msgBody.First().Content;
                }
                row["ExecComment"] = execComment;
            }

            return Json(dt);
        }

        #endregion

        #region 流程操作信息

        private DataTable GetFlowExec(string id)
        {
            var LGID = FormulaHelper.GetCurrentLGID();
            string sql = string.Format(sqlFlowExecList, id, LGID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            if (string.IsNullOrEmpty(id) || dt.Rows.Count == 0)
                return null;

            string insDefFlowID = dt.Rows[0]["InsDefFlowID"].ToString();
            sql = string.Format(@"select S_WF_InsDefRouting.ID,case when '{1}'='EN' and isnull(S_WF_DefRouting.NameEN,'')!='' then S_WF_DefRouting.NameEN else S_WF_InsDefRouting.Name end Name
            from S_WF_InsDefRouting left join S_WF_DefRouting
            on S_WF_InsDefRouting.DefRoutingID = S_WF_DefRouting.id where S_WF_InsDefRouting.InsDefFlowID='{0}'", insDefFlowID, LGID);
            DataTable dtRouting = sqlHelper.ExecuteDataTable(sql);

            dt.Columns.Add("UseTime");
            dt.Columns.Add("TaskUserDept");
            dt.Columns.Add("ExecUserDept");

            var userService = FormulaHelper.GetService<IUserService>();

            foreach (DataRow row in dt.Rows)
            {
                string ExecRoutingIDs = row["ExecRoutingIDs"].ToString().Trim(',');
                if (!string.IsNullOrEmpty(ExecRoutingIDs) && row["ExecRoutingName"].ToString() == "")
                {
                    row["ExecRoutingName"] = dtRouting.AsEnumerable().SingleOrDefault(c => c["ID"].ToString() == ExecRoutingIDs.Split(',').LastOrDefault())["Name"];
                }
                //处理打回和直送操作的名称
                if (string.IsNullOrEmpty(ExecRoutingIDs) && row["ExecRoutingName"].ToString() == "" && row["ExecTime"].ToString() != "")
                {
                    if (row["Type"].ToString() == TaskExecType.Normal.ToString() || row["Type"].ToString() == TaskExecType.Delegate.ToString())
                    {
                        if (row["DoBackRoutingID"].ToString() != "")
                            row["ExecRoutingName"] = "驳回";
                        if (row["OnlyDoBack"].ToString() == "1")
                            row["ExecRoutingName"] = "送驳回人";
                    }
                    else if (row["Type"].ToString() == TaskExecType.Circulate.ToString())
                    {
                        row["ExecRoutingName"] = "阅毕";
                    }
                    else if (row["Type"].ToString() == TaskExecType.Ask.ToString())
                    {
                        row["ExecRoutingName"] = "阅毕";
                    }

                }

                string CreateTime = row["CreateTime"].ToString();
                string ExecTime = row["ExecTime"].ToString();
                if (!string.IsNullOrEmpty(ExecTime))
                {
                    if (!string.IsNullOrEmpty(row["ApprovalInMobile"].ToString()))
                    {
                        switch (row["ApprovalInMobile"].ToString())
                        {
                            case "1":
                                row["ApprovalInMobile"] = "";
                                break;
                            case "2":
                                row["ApprovalInMobile"] = "来自钉钉";
                                break;
                            case "3":
                                row["ApprovalInMobile"] = "来自移动通";
                                break;
                            case "5":
                                row["ApprovalInMobile"] = "来自微信";
                                break;
                            case "6":
                                row["ApprovalInMobile"] = "来自轮巡工具自动通过";
                                break;
                            default:
                                row["ApprovalInMobile"] = "";
                                break;
                        }
                    }
                    else
                    {
                        row["ApprovalInMobile"] = "";
                    }
                    var span = DateTime.Parse(ExecTime) - DateTime.Parse(CreateTime);
                    row["UseTime"] = string.Format("{0}小时{1}分", span.Days * 24 + span.Hours, span.Minutes == 0 ? 1 : span.Minutes);
                }
                if (row["TaskUserID"].ToString() != "")
                {
                    var ogUser = userService.GetUserInfoByID(row["TaskUserID"].ToString());
                    if (ogUser != null)
                    {
                        row["TaskUserDept"] = ogUser.UserOrgName;
                    }
                }
                else
                {
                    row["TaskUserName"] = "";
                    row["ExecUserName"] = "";
                }
                if (row["ExecUserID"].ToString() != "")
                {
                    var taskUser = userService.GetUserInfoByID(row["ExecUserID"].ToString());
                    if (taskUser != null)
                        row["ExecUserDept"] = taskUser.UserOrgName;
                }
                else
                {
                    row["ExecUserDept"] = "";
                }
            }

            return dt;
        }

        public string GetFlowReplyComment(string id)
        {
            if (string.IsNullOrEmpty(id))
                return "";
            DataTable dt = GetFlowExec(id);
            StringBuilder sb = new StringBuilder();
            var LGID = FormulaHelper.GetCurrentLGID();
            sb.Append("<div id=\"flowAskMsg\"  style=\"height:230px; margin:0 auto;overflow:hidden;display: none; position: absolute; opacity: 0.9; filter: alpha(opacity=50); background: #f5f5f5;z-index:1;border:1px solid #676767;\"></div>");
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Type"].ToString() != TaskExecType.Ask.ToString())
                    {
                        string taskUserDept = dt.Rows[i]["ExecUserDept"].ToString() != "" ? "（" + dt.Rows[i]["ExecUserDept"] + "）" : dt.Rows[i]["ExecUserDept"].ToString();
                        string date = dt.Rows[i]["ExecTime"].ToString() != "" ? ((DateTime)dt.Rows[i]["ExecTime"]).ToString("yyyy-MM-dd HH:mm") : "";
                        sb.AppendFormat(string.Format("<div style=\"width:100%;\" id=\"'{0}'\">", dt.Rows[i]["ID"]));
                        sb.AppendFormat(string.Format("<ul><li class=\"flow-No\"><span>{0}</span></li>", (i + 1).ToString()));
                        sb.AppendFormat(string.Format("<li class=\"flow-stepName\">{0}</li>", dt.Rows[i]["StepName"]));
                        sb.AppendFormat(string.Format("<li>{0} {1}</li>", dt.Rows[i]["ExecUserName"], taskUserDept));
                        sb.AppendFormat(string.Format("<li class=\"flow-execRoutingName\">{0}</li>", dt.Rows[i]["ExecRoutingName"]));
                        sb.AppendFormat(string.Format("<li>{0}</li>", date));
                        if (dt.Rows[i]["ID"].ToString() != "")
                            sb.AppendFormat(string.Format("<li class=\"flow-comment\" onclick=\"replyComment(\'{0}\',\'{1}\')\"><img style=\"font-size:12px;padding-right:2px;vertical-align:middle;\" src=\"/MvcConfig/Scripts/Images/MyTask/reply.png\" />" + (LGID == "EN" ? "Reply" : "回复") + "</li>", dt.Rows[i]["ID"], dt.Rows[i]["ExecUserName"]));
                        if (dt.Select(string.Format(" TaskUserID='{0}' and InsTaskID='{1}' and Type='Ask'", dt.Rows[i]["TaskUserID"], dt.Rows[i]["InsTaskID"])).Count() > 0)
                            sb.AppendFormat(string.Format("<li style=\"cursor:pointer;float:right;padding-right:18px;color:#3c8dbc;\" onclick=\"flowAskClick('{0}','{1}')\"><img style=\"font-size:12px;padding-right:2px;vertical-align:middle;padding-bottom:2px;\" src=\"/MvcConfig/Scripts/Images/MyTask/ask.png\" />" + (LGID == "EN" ? "Ask" : "加签") + "</li>", dt.Rows[i]["TaskUserID"], dt.Rows[i]["InsTaskID"]));
                        sb.AppendFormat(string.Format("</ul></div><div class=\"flow-execComment\">{0}",  dt.Rows[i]["ExecComment"]));
                       sb.AppendFormat(string.Format("<div style=\"float:right;margin-top: -10px;font-size: 11px;font-style: italic;margin-right: 10px;\">{0}</div></div>", dt.Rows[i]["ApprovalInMobile"]));

                        sb.AppendFormat(GetComment(dt.Rows[i]["ID"].ToString()));
                    }
                }
            }
            return sb.ToString();
        }
        private string getMiniFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "";
            var start = fileName.IndexOf('_');
            var end = fileName.LastIndexOf('_');
            if (end == start)
                end = fileName.Length;
            return fileName.Substring(start + 1, end - start - 1);
        }

        private string GetComment(string flowMsgId)
        {
            if (String.IsNullOrEmpty(flowMsgId))
            {
                //如果消息表中出现了FlowMsgID是空字符串的现象，则流程所有结束关节都会出现这些消息回复
                //此处做个判定，禁止显示关联流程ID未空的数据 by Eric.Yang 2019-7-17
                return String.Empty;
            }
            StringBuilder sb = new StringBuilder();
            var entitie = FormulaHelper.GetEntities<BaseEntities>();
            var msgBody = entitie.Set<S_S_MsgBody>().Where(c => c.FlowMsgID == flowMsgId).ToArray();
            foreach (var item in msgBody)
            {
                string date = item.SendTime.ToString() != "" ? ((DateTime)item.SendTime).ToString("yyyy-MM-dd HH:mm") : "";
                sb.AppendFormat(string.Format("<div style=\"width:100%;\" id=\"'{0}'\">", item.ID));
                sb.AppendFormat(string.Format("<div class=\"flow-replyComment\">"));
                sb.AppendFormat(string.Format("<div class=\"flow-reply\"><span class=\"flow-replyName\">回复意见</span><span style=\"margin-right: 16px;\">{0}</span><span>{1}</span></div>", item.SenderName, date));
                sb.AppendFormat(string.Format("<div style=\"background-color: #ecf0f5;margin-bottom: 10px;padding: 5px 15px;line-height: 24px;\"><div style=\"\">{0}</div>", item.Content));
                if (!string.IsNullOrEmpty(item.AttachFileIDs))
                {
                    sb.AppendFormat(string.Format("<div><img src=\"/CommonWebResource/RelateResource/image/customctrl/singlefile.png\" style=\"padding-top:6px;\"/><a style=\"cursor:pointer;color:#3c8dbc;\" onclick=\"DownloadFile('{1}')\">{0}</a></div>", getMiniFileName(item.AttachFileIDs), item.AttachFileIDs));
                }
                sb.AppendFormat("</div></div></div>");
            }
            return sb.ToString();
        }


        public JsonResult AskTask(string taskUserID, string insTaskID, MvcAdapter.QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            string sql = string.Format(@"select ID,ExecUserID,ExecUserName,
            case when patindex('%|%',ExecComment) > 0 then substring(ExecComment,1,patindex('%|%',ExecComment) - 1) else ExecComment end as ExecComment,
            case when patindex('%|%',ExecComment) > 0 then substring(ExecComment,patindex('%|%',ExecComment)+1,len(ExecComment))  else ExecComment end as AskComment
            ,convert(varchar(16),ExecTime,20) as ExecTime
            from S_WF_InsTaskExec where TaskUserID='{0}' and InsTaskID='{1}' and Type='Ask'", taskUserID, insTaskID);
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }


        private string getQueryValue(List<Dictionary<string, string>> ht, string queryField)
        {
            if (ht == null) return "";
            foreach (var item in ht)
            {
                return item[queryField];
            }
            return "";
        }
        public void Save(string data)
        {
            var entitie = FormulaHelper.GetEntities<BaseEntities>();
            var list = JsonHelper.ToObject<List<Dictionary<string, string>>>(data);
            string flowID = getQueryValue(list, "flowID");
            string nodeID = getQueryValue(list, "nodeID");
            string ids = getQueryValue(list, "ids");
            string content = getQueryValue(list, "comment");
            DataTable dt = GetFlowExec(flowID);
            if (dt == null)
                return;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ID"].ToString() == nodeID)
                {
                    string receiverIDs = dr["TaskUserID"].ToString();
                    string receiverNames = FormulaHelper.GetUserInfoByID(receiverIDs).UserName;
                    if (ids.Length > 0)
                    {
                        receiverIDs += "," + ids;
                        foreach (string userId in ids.Split(','))
                        {
                            receiverNames += "," + FormulaHelper.GetUserInfoByID(userId).UserName;
                        }
                    }
                    S_S_MsgBody body = new S_S_MsgBody();
                    string msgBodyId = FormulaHelper.CreateGuid();
                    body.ID = msgBodyId;
                    body.Title = "关于《" + dr["FlowName"].ToString() + "》的回复";
                    body.Content = content;
                    body.ContentText = content;
                    #region 去除url中的token
                    var lowerUrl = Request.UrlReferrer.ToString().ToLower();
                    var lowerParam = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["TokenKey"]) ? ConfigurationManager.AppSettings["TokenKey"] : "GWToken";
                    lowerParam = lowerParam.ToLower();
                    if (lowerUrl.IndexOf("&" + lowerParam) > 0)
                    {
                        var beginUrl = Request.UrlReferrer.ToString().Substring(0, lowerUrl.IndexOf("&" + lowerParam));
                        var endUrl = Request.UrlReferrer.ToString().Substring(lowerUrl.IndexOf("&" + lowerParam) + 1, Request.UrlReferrer.ToString().Length - lowerUrl.IndexOf("&" + lowerParam) - 1);
                        if (endUrl.IndexOf("&") > 0)
                            endUrl = endUrl.Substring(endUrl.IndexOf("&"), endUrl.Length - endUrl.IndexOf("&"));
                        else
                            endUrl = "";
                        body.LinkUrl= beginUrl + endUrl;
                    }else if (lowerUrl.IndexOf("?" + lowerParam) > 0)
                    {
                        var beginUrl = Request.UrlReferrer.ToString().Substring(0, lowerUrl.IndexOf("?" + lowerParam));
                        var endUrl = Request.UrlReferrer.ToString().Substring(lowerUrl.IndexOf("?" + lowerParam) + 1, Request.UrlReferrer.ToString().Length - lowerUrl.IndexOf("?" + lowerParam) - 1);
                        if (endUrl.IndexOf("&") > 0)
                            endUrl = "?" + endUrl.Substring(endUrl.IndexOf("&") + 1, endUrl.Length - endUrl.IndexOf("&") - 1);
                        else
                            endUrl = "";
                        body.LinkUrl= beginUrl + endUrl;
                    }else
                    {
                        body.LinkUrl = Request.UrlReferrer.ToString();
                    }
                    #endregion
                    body.IsSystemMsg = "0";
                    body.SendTime = DateTime.Now;
                    body.SenderID = FormulaHelper.UserID;
                    body.SenderName = FormulaHelper.GetUserInfo().UserName;
                    body.ReceiverIDs = receiverIDs;
                    body.ReceiverNames = receiverNames;
                    body.AttachFileIDs = getQueryValue(list, "fileID");
                    body.FlowMsgID = nodeID;
                    entitie.Set<S_S_MsgBody>().Add(body);
                    entitie.SaveChanges();

                    foreach (string receiverID in receiverIDs.Split(','))
                    {
                        SaveReceiver(msgBodyId, receiverID);
                    }
                }
            }

        }

        private void SaveReceiver(string msgBodyId, string userId)
        {
            var entitie = FormulaHelper.GetEntities<BaseEntities>();
            S_S_MsgReceiver receiver = new S_S_MsgReceiver();
            receiver.ID = FormulaHelper.CreateGuid();
            receiver.MsgBodyID = msgBodyId;
            receiver.UserID = userId;
            receiver.UserName = FormulaHelper.GetUserInfoByID(userId).UserName;
            receiver.IsDeleted = "0";
            entitie.Set<S_S_MsgReceiver>().Add(receiver);
            entitie.SaveChanges();
        }

        #endregion

        public ViewResult Diagram()
        {
            string id = Request["ID"];
            var flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();

            var flow = flowEntities.S_WF_InsFlow.SingleOrDefault(c => c.FormInstanceID == id);
            if (flow == null)
            {
                ViewBag.ExistFlow = "false";
                ViewBag.FormUrl = "";
            }
            else
            {
                ViewBag.ExistFlow = "true";
                ViewBag.FormUrl = flow.S_WF_InsDefFlow.FormUrl.Contains("?") ? flow.S_WF_InsDefFlow.FormUrl + "&FuncType=View&ID=" + id : flow.S_WF_InsDefFlow.FormUrl + "?FuncType=View&ID=" + id;
                if (flow.S_WF_InsDefFlow.IsFreeFlow == "1")
                    Response.Redirect("/MvcConfig/Workflow/Trace/FreeFlowExecDetail?ID=" + id);
            }
            return View();
        }

        public ViewResult Sequence()
        {
            ViewBag.TableHtml = getSequenceHtml(Request["ID"]);
            return View();
        }


        private string getSequenceHtml(string formInstanceID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");

            string sqlStep = string.Format(@"select S_WF_InsDefStep.Name from S_WF_InsDefStep
join S_WF_InsFlow on S_WF_InsFlow.FormInstanceID='{0}' and S_WF_InsFlow.InsDefFlowID=S_WF_InsDefStep.InsDefFlowID where S_WF_InsDefStep.Type<>'Completion'
order by S_WF_InsDefStep.SortIndex", formInstanceID);

            DataTable dtStepName = sqlHelper.ExecuteDataTable(sqlStep);

            string[] stepArr = dtStepName.AsEnumerable().Select(c => c["Name"].ToString()).Distinct().ToArray();
            var LGID = FormulaHelper.GetCurrentLGID();
            string sql = string.Format(sqlFlowExecList, formInstanceID, LGID);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table width='{0}px' class='CTable' cellpadding='0' cellspacing='0'>", dtStepName.Rows.Count * 200 + 130);
            sb.Append("<tr style='background:#E9EBE7;height:50px'><td align='center' width='130px'><strong>执行时间\\环节内容</strong></td>");
            foreach (string stepName in stepArr)
            {
                sb.AppendFormat("<td width='200px' align='center'><strong>{0}</strong></td>", stepName);
            }
            sb.Append("</tr>");
            int i = 0;
            string _stepName = "";
            foreach (DataRow row in dt.Rows)
            {
                if (_stepName != row["StepName"].ToString())
                    i++;
                _stepName = row["StepName"].ToString();
                sb.Append(getSequenceRowHtml(row, stepArr, i));


            }

            sb.Append("</table>");
            return sb.ToString();
        }

        private string getSequenceRowHtml(DataRow row, string[] stepArr, int idx)
        {
            string cellHtml = @"
<table class='ITable'  border='0' style='background:{4}' cellpadding='0' cellspacing='0' width='100%' height='100%'>
    <tr><td><img src='/CommonWebResource/Theme/Default/miniui/icons/user.png' /></td><td align='right'>接&nbsp;收&nbsp;人&nbsp;：</td><td align='left'>{0}</td></tr>
    <tr><td><img src='/CommonWebResource/Theme/Default/miniui/icons/node.png' /></td><td>接收时间：</td><td>{1}</td></tr>
    <tr><td><img src='/CommonWebResource/Theme/Default/miniui/icons/user.png' /></td><td align='right'>操&nbsp;作&nbsp;人&nbsp;：</td><td align='left'>{2}</td></tr>    
    <tr><td><img src='/CommonWebResource/Theme/Default/miniui/icons/node.png' /></td><td>操作时间：</td><td>{3}</td></tr>
</table>
";
            cellHtml = string.Format(cellHtml
                , string.Format("<a href='#' onclick='viewDetail(\"{1}\",\"{2}\");'>{0}</a>", row["TaskUserName"], row["ID"], row["TaskUserID"])
                , row["CreateTime"].ToString() == "" ? "" : DateTime.Parse(row["CreateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                , string.Format("<a href='#' onclick='viewDetail(\"{1}\",\"{2}\");'>{0}</a>", row["ExecUserName"], row["ID"], row["ExecUserID"])
                , row["ExecTime"].ToString() == "" ? "" : DateTime.Parse(row["ExecTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                , row["ExecTime"].ToString() == "" ? "#B4D02E" : "#A1D8DD");


            StringBuilder sb = new StringBuilder();
            sb.Append("\n<tr style='height:100px;'>");
            if (row["CreateTime"].ToString() != "")
                sb.AppendFormat("<td align='left' style='padding-left:4px;background:#E9EBE7;'><font size='4' color='#555'><b>第{0}步</b></font><font color='red'><strong>{2}</strong></font><br>{1}</td>", idx.ToString(), row["ExecTime"].ToString() == "" ? "　日期：<br>　时间：" : DateTime.Parse(row["ExecTime"].ToString()).ToString("　日期：yyyy-MM-dd<br>　时间：HH:mm:ss"), row["ExecTime"].ToString() == "" ? "（执行中）" : "");
            else
                sb.AppendFormat("<td align='left' style='padding-left:4px;background:#E9EBE7;'><font size='4' color='#555'><b>第{0}步</b></font><font color='#A1D8DD'><strong>{2}</strong></font><br>{1}</td>", idx.ToString(), row["ExecTime"].ToString() == "" ? "　日期：<br>　时间：" : DateTime.Parse(row["ExecTime"].ToString()).ToString("　日期：yyyy-MM-dd<br>　时间：HH:mm:ss"), row["ExecTime"].ToString() == "" ? "（结束）" : "");


            foreach (string stepName in stepArr)
            {
                if (stepName == row["StepName"].ToString())
                    sb.AppendFormat("<td align='center' valign='center'>{0}</td>", cellHtml);
                else
                    sb.AppendFormat("<td>&nbsp;</td>");
            }

            sb.Append("</tr>");
            return sb.ToString();
        }

        public JsonResult GetUserInfo(string id, string userid)
        {
            string sql = string.Format("SELECT * FROM S_WF_INSTASKEXEC WHERE ID='{0}'", id);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            DataTable dtExec = sqlHelper.ExecuteDataTable(sql);

            sql = string.Format("select ID, NAME as \"Name\",DEPTNAME as \"DeptName\",PHONE as \"Phone\",MOBILEPHONE as \"MobilePhone\" from S_A_User where ID='{0}'", userid);
            sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dtUser = sqlHelper.ExecuteDataTable(sql);

            dtUser.Columns.Add("CreateTime");
            dtUser.Columns.Add("ExecTime");
            dtUser.Columns.Add("ExecComment");

            DataRow row = dtUser.Rows[0];
            row["CreateTime"] = dtExec.Rows[0]["CreateTime"];
            row["ExecTime"] = dtExec.Rows[0]["ExecTime"];
            row["ExecComment"] = dtExec.Rows[0]["ExecComment"];
            return Json(row);
        }
    }
}
