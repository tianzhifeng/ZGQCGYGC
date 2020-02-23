using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Workflow.Logic;
using Workflow.DTO;
using MvcAdapter;
using Workflow.Logic.BusinessFacade;
using Workflow.Logic.Domain;
using Formula;
using System.Data;
using Config;
using System.Collections.Specialized;
using Formula.Helper;
using System.Web;
using System.Collections;
namespace Workflow.Web.Controllers
{
    public class FlowTaskAPIController : ApiController, IFlowController
    {
        FlowService _flowService = null;
        FlowService flowService
        {
            get
            {
                if (_flowService == null)
                {
                    string formInstanceID = Request.RequestUri.ParseQueryString().Get("FormInstanceID");
                    string taskExecID = Request.RequestUri.ParseQueryString().Get("TaskExecID");
                    _flowService = new FlowService(this, "", formInstanceID, taskExecID);
                }
                return _flowService;
            }
        }

        /// <summary>
        /// 获取任务详.
        /// </summary>
        /// <param name="id">任务ID,即TaskExecID.</param>
        /// <returns></returns>
        [ActionName("Tasks")]
        [HttpGet]
        public TaskDTO _GetTaskDetail(string id)
        {
            var formInstanceID = Request.RequestUri.ParseQueryString().Get("FormInstanceID");
            var userID = Request.RequestUri.ParseQueryString().Get("UserID");
            var userAccount = Request.RequestUri.ParseQueryString().Get("UserAccount");
            FormulaHelper.ContextSet("AgentUserLoginName", userAccount);

            S_WF_InsTaskExec task = GetTaskExec(id);
            TaskDTO dto = new TaskDTO();
            dto.ID = task.ID;
            dto.CreateTime = task.CreateTime;
            dto.FlowName = task.S_WF_InsFlow.FlowName;
            dto.TaskName = task.S_WF_InsTask.TaskName;
            dto.FlowCategory = task.S_WF_InsFlow.FlowCategory;
            dto.FormInstanceID = task.S_WF_InsFlow.FormInstanceID;
            dto.FormInstanceCode = task.S_WF_InsFlow.S_WF_InsDefFlow.TableName;
            dto.FormDic = task.S_WF_InsFlow.FormDic;
            dto.SendTaskUserNames = task.S_WF_InsTask.SendTaskUserNames;
            dto.CreateUserID = task.S_WF_InsFlow.CreateUserID;
            dto.CreateUserName = task.S_WF_InsFlow.CreateUserName;

            //新加字段
            dto.ExecTime = task.ExecTime;
            dto.IsFinished = task.ExecTime.HasValue;
            dto.TaskUserID = task.ExecUserID;
            dto.TaskUserName = task.ExecUserName;
            dto.FlowDefCode = task.S_WF_InsFlow.S_WF_InsDefFlow.Code;

            dto.Routines = GetRountineList(formInstanceID, id, dto.FormDic);


            dto.DefStepID = task.S_WF_InsTask.S_WF_InsDefStep.DefStepID;
            dto.DefStepName = task.S_WF_InsTask.S_WF_InsDefStep.Name;

            dto.HideAdvice = task.S_WF_InsTask.S_WF_InsDefStep.HideAdvice == "1";

            FormulaHelper.ContextRemoveByKey("AgentUserLoginName");
            return dto;
        }
        /// <summary>
        /// 执行任务.
        /// </summary>
        /// <param name="id">路由ID.</param>
        /// <returns></returns>
        [ActionName("Submit")]
        [HttpGet]
        public bool _SubmitTask(string id)
        {
            try
            {

                string routingIDs = id;
                string formInstanceID = Request.RequestUri.ParseQueryString().Get("FormInstanceID");
                string taskExecID = Request.RequestUri.ParseQueryString().Get("TaskExecID");
                string nextExecUserIDs = Request.RequestUri.ParseQueryString().Get("NextExecUserIDs");
                string execComment = Request.RequestUri.ParseQueryString().Get("Comment");
                string execUserID = Request.RequestUri.ParseQueryString().Get("ExecUserID");

                string userAccount = Request.RequestUri.ParseQueryString().Get("UserAccount");
                //FormulaHelper.ContextSet("AgentUserLoginName", userAccount);


                //模拟http请求
                var entities = FormulaHelper.GetEntities<WorkflowEntities>();
                var taskExec = entities.S_WF_InsTaskExec.SingleOrDefault(c => c.ID == taskExecID);
                string url = taskExec.S_WF_InsFlow.S_WF_InsDefFlow.FormUrl;
                string search = "";
                string action = "";
                if (url.Contains('?'))
                {
                    int index = url.IndexOf('?');
                    search = url.Substring(index);
                    url = url.Remove(index);
                }
                url = url.Remove(url.LastIndexOf('/'));

                Hashtable pars = new Hashtable();

                if (routingIDs == "btnDoBack")
                {
                    action = "DoBack";
                    pars.Add("TaskExecID", taskExecID);
                    pars.Add("RoutingID", routingIDs);
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnDoBackFirst")
                {
                    action = "DoBackFirst";
                    pars.Add("TaskExecID", taskExecID);
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnDoBackFirstReturn")
                {
                    action = "DoBackFirstReturn";
                    pars.Add("TaskExecID", taskExecID);
                    pars.Add("ExecComment", execComment);
                }
                else
                {
                    action = "Submit";
                    pars.Add("ID", formInstanceID);
                    pars.Add("RoutingID", routingIDs);
                    pars.Add("TaskExecID", taskExecID);
                    pars.Add("NextExecUserIDs", nextExecUserIDs);
                    pars.Add("NextExecUserIDsGroup", "");
                    pars.Add("nextExecRoleIDs", "");
                    pars.Add("nextExecOrgIDs", "");
                    pars.Add("ExecComment", execComment);
                    pars.Add("AutoPass", "True");

                    //解决移动通组单人通过，没有传递NextExecUserIDsGroup问题（移动通没人改了，后台解决）
                    var routing = entities.Set<S_WF_InsDefRouting>().SingleOrDefault(c => c.ID == routingIDs);
                    if (routing != null && !string.IsNullOrEmpty(routing.UserIDsGroupFromField))
                    {
                        var insDefFlow = taskExec.S_WF_InsFlow.S_WF_InsDefFlow;
                        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(insDefFlow.ConnName);
                        var dt = sqlHelper.ExecuteDataTable(string.Format("select * from {0} where ID='{1}'", insDefFlow.TableName, formInstanceID));
                        pars.Add("NextExecUserIDsGroup", dt.Rows[0][routing.UserIDsGroupFromField].ToString());
                    }

                }



                HttpHelper httpHelper = new HttpHelper();
                string host = GetHostUrl();
                string requestUri = host + "/Workflow/FlowTask/GetAuthCookie?userCode=" + HttpUtility.UrlEncode(userAccount);
                string result = httpHelper.GetHtml(requestUri, pars, true);//模拟登录
                if (result.Trim('\"', ' ') == "ok")
                {
                    url = url + "/" + action + search;
                    result = httpHelper.GetHtml(host + url, pars, true);
                    if (result == "error")
                        return false;
                    else
                    {
                        Dictionary<string, string> dic;
                        try
                        {
                            //移动端的自动通过功能
                            dic = JsonHelper.ToObject<Dictionary<string, string>>(result);
                        }
                        catch
                        {
                            LogWriter.Error("调用api报错，url:" + host + url + ",报错信息为:" + result);
                            return false;
                        }

                        while (dic.ContainsKey("NextTaskExecID"))
                        {
                            pars["TaskExecID"] = dic["NextTaskExecID"];
                            pars["RoutingID"] = dic["NextRoutingID"];
                            pars["NextExecUserIDs"] = dic["NextExecUserIDs"];

                            result = httpHelper.GetHtml(host + url, pars, true);

                            if (result == "error")
                                break;
                            else
                                dic = JsonHelper.ToObject<Dictionary<string, string>>(result);
                        }

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);

                return false;
            }
        }

        /// <summary>
        /// 新版移动通的提交表单方法
        /// </summary>
        /// <param name="id">路由ID.</param>
        /// <returns></returns>
        [ActionName("SubmitForm")]
        [HttpPost]
        public string _SubmitForm(string id, [FromBody]PostSubmitTaskDTO taskDto)
        {
            try
            {
                string routingIDs = id;
                string formInstanceID = Request.RequestUri.ParseQueryString().Get("FormInstanceID");
                string taskExecID = Request.RequestUri.ParseQueryString().Get("TaskExecID");
                string nextExecUserIDs = Request.RequestUri.ParseQueryString().Get("NextExecUserIDs");
                string execComment = Request.RequestUri.ParseQueryString().Get("Comment");
                string execUserID = Request.RequestUri.ParseQueryString().Get("ExecUserID");

                string userAccount = Request.RequestUri.ParseQueryString().Get("UserAccount");
                //FormulaHelper.ContextSet("AgentUserLoginName", userAccount);

                //新版表单定义会传递表单数据 2016-11-16
                string formData = taskDto.FormDic;
                string FlowCode = taskDto.FlowCode;

                var url = "";
                var entities = FormulaHelper.GetEntities<WorkflowEntities>();
                Hashtable pars = new Hashtable();

                if (taskExecID != "0") //提交任务时
                {
                    var taskExec = entities.S_WF_InsTaskExec.SingleOrDefault(c => c.ID == taskExecID);
                    url = taskExec.S_WF_InsFlow.S_WF_InsDefFlow.FormUrl;
                    pars.Add("TaskExecID", taskExecID);
                }
                else //首次提交表单时
                {
                    var flowDef = entities.S_WF_DefFlow.SingleOrDefault(c => c.Code == FlowCode);
                    url = flowDef.FormUrl;
                }

                string search = "";
                string action = "";
                if (url.Contains('?'))
                {
                    int index = url.IndexOf('?');
                    search = url.Substring(index);
                    url = url.Remove(index);
                }
                url = url.Remove(url.LastIndexOf('/'));


                if (string.IsNullOrEmpty(formData))
                    pars.Add("FormData", "{\"ID\":\"" + formInstanceID + "\"}");
                else
                    pars.Add("FormData", formData);
                if (!string.IsNullOrEmpty(FlowCode))
                    pars.Add("FlowCode", FlowCode);

                if (routingIDs == "btnDoBack")
                {
                    action = "DoBack";
                    pars.Add("RoutingID", routingIDs);
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnDoBackFirst")
                {
                    action = "DoBackFirst";
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnDoBackFirstReturn")
                {
                    action = "DoBackFirstReturn";
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnDelegate")
                {
                    action = "DelegateTask";
                    pars.Add("NextExecUserIDs", nextExecUserIDs);
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnAsk")
                {
                    action = "AskTask";
                    pars.Add("NextExecUserIDs", nextExecUserIDs);
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnCirculate")
                {
                    action = "CirculateTask";
                    pars.Add("NextExecUserIDs", nextExecUserIDs);
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnView" || routingIDs == "btnReply")
                {
                    action = "ViewTask";
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnWithdrawAsk")
                {
                    action = "WithdrawAskTask";
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnWithdraw")
                {
                    action = "DeleteFlow";
                    pars.Add("ExecComment", execComment);
                }
                else if (routingIDs == "btnDelete")
                {
                    action = "Delete";
                    pars.Add("ExecComment", execComment);
                }
                else
                {
                    action = "Submit";
                    pars.Add("ID", formInstanceID);
                    pars.Add("RoutingID", routingIDs);
                    pars.Add("NextExecUserIDs", nextExecUserIDs);
                    pars.Add("NextExecUserIDsGroup", "");
                    pars.Add("nextExecRoleIDs", "");
                    pars.Add("nextExecOrgIDs", "");
                    pars.Add("ExecComment", execComment);
                    pars.Add("AutoPass", "False");

                    //解决移动通组单人通过，没有传递NextExecUserIDsGroup问题（移动通没人改了，后台解决）
                    var routing = entities.Set<S_WF_InsDefRouting>().SingleOrDefault(c => c.ID == routingIDs);
                    if (routing != null && !string.IsNullOrEmpty(routing.UserIDsGroupFromField))
                    {
                        var formDic = JsonHelper.ToObject(formData);
                        pars["NextExecUserIDsGroup"] = formDic[routing.UserIDsGroupFromField].ToString();
                    }
                }

                //增加参数IsMobileRequest
                string IsMobileRequest = Request.RequestUri.ParseQueryString().Get("IsMobileRequest");
                if (string.IsNullOrEmpty(IsMobileRequest))
                    pars.Add("IsMobileRequest", "0");
                else
                    pars.Add("IsMobileRequest", IsMobileRequest);


                HttpHelper httpHelper = new HttpHelper();

                string host = GetHostUrl();
                string requestUri = host + "/Workflow/FlowTask/GetAuthCookie?userCode=" + HttpUtility.UrlEncode(userAccount);
                string result = httpHelper.GetHtml(requestUri, pars, true);//模拟登录
                if (result.Trim('\"', ' ') == "ok")
                {
                    url = url + "/" + action + search;
                    result = httpHelper.GetHtml(host + url, pars, true);
                    if (result == "error")
                        return "调用api报错，url: " + host + url;
                    else if (result == "")//委托加签传阅，不需要自动通过
                        return null;
                    else
                    {
                        Dictionary<string, string> dic;
                        try
                        {
                            if (HttpContext.Current.Response.StatusCode == 500)
                            {
                                dic = JsonHelper.ToObject<Dictionary<string, string>>(result);
                                return dic["errmsg"];
                            }
                            //移动端的自动通过功能
                            dic = JsonHelper.ToObject<Dictionary<string, string>>(result);
                        }
                        catch
                        {
                            LogWriter.Error("调用api报错，url:" + host + url + ",报错信息为:" + result);
                            return result;
                        }

                        while (dic.ContainsKey("NextTaskExecID"))
                        {
                            pars["TaskExecID"] = dic["NextTaskExecID"];
                            pars["RoutingID"] = dic["NextRoutingID"];
                            pars["NextExecUserIDs"] = dic["NextExecUserIDs"];

                            result = httpHelper.GetHtml(host + url, pars, true);

                            if (result == "error")
                                break;
                            else
                                dic = JsonHelper.ToObject<Dictionary<string, string>>(result);

                        }

                        return null;
                    }
                }
                else
                {
                    return "调用api报错，url: " + host + url;
                }
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp);
                return exp.Message;
            }
        }
        public void SetFormFlowInfo(string id, string flowInfo)
        {
        }

        /// <summary>
        /// 流程跟踪.
        /// </summary>
        /// <param name="id">表单实体ID(FormInstanceID)</param>
        [ActionName("FlowTrace")]
        [HttpGet]
        public IEnumerable<FlowTraceDTO> _FlowTrace(string id)
        {
            #region sqlFlowExecList
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
,S_WF_InsDefStep.Name as StepName
,S_WF_InsDefStep.ID as StepID
,ExecRoutingIDs
,ExecRoutingName
,S_WF_InsFlow.InsDefFlowID
,S_WF_InsTask.DoBackRoutingID
,S_WF_InsTask.OnlyDoBack
,ApprovalInMobile
from S_WF_InsTaskExec
right join S_WF_InsTask on InsTaskID=S_WF_InsTask.ID
join S_WF_InsFlow on S_WF_InsTask.InsFlowId=S_WF_InsFlow.ID
join S_WF_InsDefStep on InsDefStepID=S_WF_InsDefStep.ID and S_WF_InsDefStep.Type<>'Completion'
where FormInstanceID='{0}' and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='')
order by isnull(S_WF_InsTaskExec.CreateTime,S_WF_InsTask.CreateTime)
";
            #endregion

            string sql = string.Format(sqlFlowExecList, id);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Workflow");
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return new List<FlowTraceDTO>();

            string insDefFlowID = dt.Rows[0]["InsDefFlowID"].ToString();
            sql = string.Format("select ID,Name from S_WF_InsDefRouting where InsDefFlowID='{0}'", insDefFlowID);
            DataTable dtRouting = sqlHelper.ExecuteDataTable(sql);

            List<FlowTraceDTO> traceList = new List<FlowTraceDTO>();
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
                    if (row["DoBackRoutingID"].ToString() != "")
                        row["ExecRoutingName"] = "驳回";
                    if (row["OnlyDoBack"].ToString() == "1")
                        row["ExecRoutingName"] = "送驳回人";
                }

                DateTime? execTime = null;
                if (!string.IsNullOrWhiteSpace(row["ExecTime"].ToString()))
                    execTime = DateTime.Parse(row["ExecTime"].ToString());

                traceList.Add(new FlowTraceDTO
                {
                    ID = row["ID"].ToString(),
                    ExecComment = row["ExecComment"].ToString(),
                    ExecRoutingName = row["ExecRoutingName"].ToString(),
                    ExecTime = execTime,
                    ExecUserID = row["ExecUserID"].ToString(),
                    ExecUserName = row["ExecUserName"].ToString(),
                    StepName = row["StepName"].ToString(),
                    FlowReplys = GetFlowReply(row["ID"].ToString()),
                    ApprovalInMobile = row["ApprovalInMobile"].ToString()
                });
            }
            return traceList;
        }

        private List<FlowReplyDTO> GetFlowReply(string id)
        {
            List<FlowReplyDTO> list = new List<FlowReplyDTO>();
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            var table = sqlHelper.ExecuteDataTable(string.Format("Select ID,SenderID,SenderName,Content,SendTime,AttachFileIDs From S_S_MsgBody Where FlowMsgID='{0}'", id));
            foreach (DataRow row in table.Rows)
            {
                DateTime? sendTime = null;
                if (!string.IsNullOrWhiteSpace(row["SendTime"].ToString()))
                    sendTime = DateTime.Parse(row["SendTime"].ToString());
                list.Add(new FlowReplyDTO
                {
                    ReplyID = row["ID"].ToString(),
                    FlowMsgID = id,
                    SenderUserID = row["SenderID"].ToString(),
                    SenderName = row["SenderName"].ToString(),
                    Content = row["Content"].ToString(),
                    SenderDate = sendTime,
                    AttachFileIDs = row["AttachFileIDs"].ToString()
                });
            }
            return list;
        }

        /// <summary>
        /// 新版流程定义增加的方法，用于启动流程时获取路由
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("FlowStartRoutine")]
        [HttpGet]
        public List<RoutineDTO> _StartRoutine(string id)
        {
            var userAccount = Request.RequestUri.ParseQueryString().Get("UserAccount");
            FormulaHelper.ContextSet("AgentUserLoginName", userAccount);

            List<ButtonInfo> buttonInfoList = flowService.JsonGetFlowButtons("", "");

            FormulaHelper.ContextRemoveByKey("AgentUserLoginName");


            var list = new List<RoutineDTO>();
            foreach (var btn in buttonInfoList)
            {
                if (btn.routingParams != null)
                {
                    var routine = new RoutineDTO();
                    routine.ID = btn.id;

                    if (btn.routingParams.selectMode == "SelectOneUser" || btn.routingParams.selectMode == "SelectOneUserInScope")
                    {
                        routine.OwnerType = OwnerType.Single;
                    }
                    else if (btn.routingParams.selectMode == "SelectMultiUser" ||
                       btn.routingParams.selectMode == "SelectMultiUserInScope" ||
                        btn.routingParams.selectMode == "SelectOneOrg" ||
                        btn.routingParams.selectMode == "SelectMultiOrg")
                    {
                        routine.OwnerType = OwnerType.Multi;
                    }
                    else
                    {
                        routine.OwnerType = OwnerType.None;
                    }
                    routine.OwnerUserIDs = btn.routingParams.userIDs;
                    routine.RoutineName = btn.text;
                    routine.MustInputComment = btn.routingParams.mustInputComment;

                    list.Add(routine);
                }
            }
            return list;
        }

        public void SetFormFlowPhase(string id, string flowPhase, string stepName)
        {
            throw new NotImplementedException();
        }
        public void DeleteForm(string ids)
        {
            throw new NotImplementedException();
        }
        public bool ExistForm(string id)
        {
            return true;
        }
        public System.Web.Mvc.JsonResult Save()
        {
            return null;
        }
        public System.Web.Mvc.JsonResult SaveBA(string tmplCode)
        {
            return null;
        }
        public void UnExecTaskExec(string taskExecID)
        {
            FlowFO flowFO = new FlowFO();
            flowFO.UnExecTask(taskExecID);
        }
        public bool ExecTaskExec(Logic.Domain.S_WF_InsTaskExec taskExec, Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            FlowFO flowFO = new FlowFO();
            return flowFO.ExecTask(taskExec.ID, routing.ID, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment, routing.ID);//原参数为Request["RoutingID"]，前台传入参数， 当分支路由时，其值为逗号隔开的全部分支ID；改为routing.ID潜在问题，不支持分支。
        }
        public bool ExecTaskExec(Logic.Domain.S_WF_InsTaskExec taskExec, Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment, string code)
        {
            FlowFO flowFO = new FlowFO();
            return flowFO.ExecTask(taskExec.ID, routing.ID, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment, routing.ID);//原参数为Request["RoutingID"]，前台传入参数， 当分支路由时，其值为逗号隔开的全部分支ID；改为routing.ID潜在问题，不支持分支。
        }
        /// <summary>
        /// 填完表单获取分支
        /// </summary>
        /// <param name="formInstanceID"></param>
        /// <param name="id"></param>
        /// <param name="formDic"></param>
        /// <returns></returns>

        [ActionName("GetRountineLists")]
        [HttpPost]
        public List<RoutineDTO> GetRountineLists(string formInstanceID, string id, [FromBody]PostSubmitTaskDTO taskDto)
        {
            var userAccount = Request.RequestUri.ParseQueryString().Get("UserAccount");
            FormulaHelper.ContextSet("AgentUserLoginName", userAccount);

            string formdata = JsonHelper.ToJson(taskDto.FormDic);
            FlowService flowservice = new FlowService(this, taskDto.FormDic, "", id);
            List<ButtonInfo> buttonInfoList = new List<ButtonInfo>();
            if (string.IsNullOrEmpty(id))
            {
                buttonInfoList = flowservice.JsonGetFlowButtons("", "");
                var list = new List<RoutineDTO>();
                foreach (var btn in buttonInfoList)
                {
                    if (btn.routingParams != null)
                    {
                        var routine = new RoutineDTO();
                        routine.ID = btn.id;
                        if (btn.routingParams.userIDs == "")
                        {
                            if (btn.routingParams.selectMode == "SelectOneUser")
                            {
                                routine.OwnerType = OwnerType.Single;
                            }
                            else if (btn.routingParams.selectMode == "SelectMultiUser")
                            {
                                routine.OwnerType = OwnerType.Multi;
                            }
                            else if (!string.IsNullOrEmpty(btn.routingParams.userIDsFromField) ||
                                !string.IsNullOrEmpty(btn.routingParams.orgIDFromField) ||
                                !string.IsNullOrEmpty(btn.routingParams.roleIDsFromField))
                            {
                                routine.OwnerType = OwnerType.None;
                            }
                            else
                            {
                                routine.OwnerType = OwnerType.None;
                            }
                        }
                        else
                        {
                            if (btn.routingParams.selectMode == "SelectOneUserInScope" || btn.routingParams.selectMode == "SelectOnePrjUser")
                            {
                                routine.OwnerType = OwnerType.Single;
                            }
                            else if (btn.routingParams.selectMode == "SelectMultiUserInScope" || btn.routingParams.selectMode == "SelectMultiPrjUser")
                            {
                                routine.OwnerType = OwnerType.Multi;
                            }
                            else
                            {
                                routine.OwnerType = OwnerType.Special;
                            }

                            if (btn.routingParams.selectAgain)
                            {
                                if (btn.routingParams.selectMode == "SelectOneUserInScope" || btn.routingParams.selectMode == "SelectOnePrjUser" || btn.routingParams.selectMode == "SelectOneUser")
                                {
                                    routine.OwnerType = OwnerType.Single;
                                    if (btn.routingParams.selectMode == "SelectOneUser")
                                    {
                                        routine.OwnerUserIDs = "";
                                    }
                                }
                                else if (btn.routingParams.selectMode == "SelectMultiUserInScope" || btn.routingParams.selectMode == "SelectMultiPrjUser" || btn.routingParams.selectMode == "SelectMultiUser")
                                {
                                    routine.OwnerType = OwnerType.Multi;
                                    if (btn.routingParams.selectMode == "SelectMultiUser")
                                    {
                                        routine.OwnerUserIDs = "";
                                    }
                                }
                            }
                        }
                        routine.OwnerUserIDs = btn.routingParams.userIDs;
                        routine.RoutineName = btn.text;
                        routine.MustInputComment = btn.routingParams.mustInputComment;

                        list.Add(routine);
                    }
                }
                return list;
            }
            else
            {
                buttonInfoList = flowservice.JsonGetFlowButtons(formInstanceID, id, true);
            }


            FormulaHelper.ContextRemoveByKey("AgentUserLoginName");
            List<RoutineDTO> Routines = new List<RoutineDTO>();
            List<RoutineDTO> staticRoutines = new List<RoutineDTO>();
            for (var i = 0; i < buttonInfoList.Count; i++)
            {
                RoutineDTO routine;
                var item = buttonInfoList[i];

                if (item.id == "btnDoBack" || item.id == "btnDoBackFirst" || item.id == "btnDoBackFirstReturn")
                {
                    routine = new RoutineDTO
                    {
                        ID = item.id,
                        RoutineName = item.text,
                    };
                    routine.OwnerType = OwnerType.None;
                    routine.OwnerUserIDs = "";
                    staticRoutines.Add(routine);
                    continue;
                }
                else if (item.id == "btnDelegate" || item.id == "btnAsk")
                {
                    routine = new RoutineDTO
                    {
                        ID = item.id,
                        RoutineName = item.text,
                    };
                    routine.OwnerType = OwnerType.Single;
                    routine.OwnerUserIDs = "";
                    staticRoutines.Add(routine);
                    continue;

                }
                else if (item.id == "btnCirculate")
                {
                    routine = new RoutineDTO
                    {
                        ID = item.id,
                        RoutineName = item.text,
                    };
                    routine.OwnerType = OwnerType.Multi;
                    routine.OwnerUserIDs = "";
                    staticRoutines.Add(routine);
                    continue;
                }
                else if (item.id == "btnView" || item.id == "btnReply" || item.id == "btnWithdrawAsk" || item.id == "btnWithdraw" || item.id == "btnDelete")
                {
                    routine = new RoutineDTO
                    {
                        ID = item.id,
                        RoutineName = item.text,
                    };
                    routine.OwnerType = OwnerType.None;
                    routine.OwnerUserIDs = "";
                    staticRoutines.Add(routine);
                    continue;
                }
                else if (item.id.StartsWith("btn"))
                {
                    //buttonInfoList.Remove(item);   
                    //这句注释掉，否则循环都乱了
                    continue;
                }


                routine = new RoutineDTO
                {
                    ID = item.id,
                    RoutineName = item.text,
                    MustInputComment = item.routingParams.mustInputComment,
                    SelectAgain = item.routingParams.selectAgain,
                    DefaultComment = item.routingParams.defaultComment
                };
                Routines.Add(routine);
                routine.OwnerType = OwnerType.None;
                routine.OwnerUserIDs = item.routingParams.userIDs;

                if (item.routingParams.flowComplete == true) //移动端在流程结束时不再选人
                {
                    routine.OwnerType = OwnerType.None;
                    routine.OwnerUserIDs = "";
                    continue;
                }


                if (item.routingParams.routingID.Contains(','))//分支路由不用选人
                {
                    routine.OwnerType = OwnerType.None;
                    continue;
                }

                if (item.routingParams.userIDs == "")
                {
                    if (item.routingParams.selectMode == "SelectOneUser")
                    {
                        routine.OwnerType = OwnerType.Single;
                    }
                    else if (item.routingParams.selectMode == "SelectMultiUser")
                    {
                        routine.OwnerType = OwnerType.Multi;
                    }
                    else if (!string.IsNullOrEmpty(item.routingParams.userIDsFromField) ||
                        !string.IsNullOrEmpty(item.routingParams.orgIDFromField) ||
                        !string.IsNullOrEmpty(item.routingParams.roleIDsFromField))
                    {
                        routine.OwnerType = OwnerType.None;
                    }
                    else
                    {
                        routine.OwnerType = OwnerType.None;
                    }
                }
                else
                {
                    if (item.routingParams.selectMode == "SelectOneUserInScope" || item.routingParams.selectMode == "SelectOnePrjUser")
                    {
                        routine.OwnerType = OwnerType.Single;
                    }
                    else if (item.routingParams.selectMode == "SelectMultiUserInScope" || item.routingParams.selectMode == "SelectMultiPrjUser")
                    {
                        routine.OwnerType = OwnerType.Multi;
                    }
                    else
                    {
                        routine.OwnerType = OwnerType.Special;
                    }

                    if (item.routingParams.selectAgain)
                    {
                        if (item.routingParams.selectMode == "SelectOneUserInScope" || item.routingParams.selectMode == "SelectOnePrjUser" || item.routingParams.selectMode == "SelectOneUser")
                        {
                            routine.OwnerType = OwnerType.Single;
                            if (item.routingParams.selectMode == "SelectOneUser")
                            {
                                routine.OwnerUserIDs = "";
                            }
                        }
                        else if (item.routingParams.selectMode == "SelectMultiUserInScope" || item.routingParams.selectMode == "SelectMultiPrjUser" || item.routingParams.selectMode == "SelectMultiUser")
                        {
                            routine.OwnerType = OwnerType.Multi;
                            if (item.routingParams.selectMode == "SelectMultiUser")
                            {
                                routine.OwnerUserIDs = "";
                            }
                        }
                    }
                }
            }

            //排序需要与PC端一致，故反过来
            for (int i = 0; i < staticRoutines.Count; i++)
                Routines.Add(staticRoutines[i]);

            return Routines;
        }
        private List<RoutineDTO> GetRountineList(string formInstanceID, string id, StringDictionary formDic)
        {
            List<ButtonInfo> buttonInfoList = flowService.JsonGetFlowButtons(formInstanceID, id, true);
            List<RoutineDTO> Routines = new List<RoutineDTO>();
            List<RoutineDTO> staticRoutines = new List<RoutineDTO>();
            for (var i = 0; i < buttonInfoList.Count; i++)
            {
                RoutineDTO routine;
                var item = buttonInfoList[i];

                if (item.id == "btnDoBack" || item.id == "btnDoBackFirst" || item.id == "btnDoBackFirstReturn")
                {
                    routine = new RoutineDTO
                    {
                        ID = item.id,
                        RoutineName = item.text,
                    };
                    routine.OwnerType = OwnerType.None;
                    routine.OwnerUserIDs = "";
                    staticRoutines.Add(routine);
                    continue;
                }
                else if (item.id == "btnDelegate" || item.id == "btnAsk")
                {
                    routine = new RoutineDTO
                    {
                        ID = item.id,
                        RoutineName = item.text,
                    };
                    routine.OwnerType = OwnerType.Single;
                    routine.OwnerUserIDs = "";
                    staticRoutines.Add(routine);
                    continue;

                }
                else if (item.id == "btnCirculate")
                {
                    routine = new RoutineDTO
                    {
                        ID = item.id,
                        RoutineName = item.text,
                    };
                    routine.OwnerType = OwnerType.Multi;
                    routine.OwnerUserIDs = "";
                    staticRoutines.Add(routine);
                    continue;
                }
                else if (item.id == "btnView" || item.id == "btnReply" || item.id == "btnWithdrawAsk" || item.id == "btnWithdraw" || item.id == "btnDelete")
                {
                    routine = new RoutineDTO
                    {
                        ID = item.id,
                        RoutineName = item.text,
                    };
                    routine.OwnerType = OwnerType.None;
                    routine.OwnerUserIDs = "";
                    staticRoutines.Add(routine);
                    continue;
                }
                else if (item.id.StartsWith("btn"))
                {
                    //buttonInfoList.Remove(item);   
                    //这句注释掉，否则循环都乱了
                    continue;
                }


                routine = new RoutineDTO
                {
                    ID = item.id,
                    RoutineName = item.text,
                    MustInputComment = item.routingParams.mustInputComment,
                    SelectAgain = item.routingParams.selectAgain,
                    DefaultComment = item.routingParams.defaultComment
                };
                Routines.Add(routine);
                routine.OwnerType = OwnerType.None;
                routine.OwnerUserIDs = item.routingParams.userIDs;

                if (item.routingParams.flowComplete == true) //移动端在流程结束时不再选人
                {
                    routine.OwnerType = OwnerType.None;
                    routine.OwnerUserIDs = "";
                    continue;
                }


                if (item.routingParams.routingID.Contains(','))//分支路由不用选人
                {
                    routine.OwnerType = OwnerType.None;
                    continue;
                }

                if (item.routingParams.userIDs == "")
                {
                    if (item.routingParams.selectMode == "SelectOneUser")
                    {
                        routine.OwnerType = OwnerType.Single;
                    }
                    else if (item.routingParams.selectMode == "SelectMultiUser")
                    {
                        routine.OwnerType = OwnerType.Multi;
                    }
                    else if (!string.IsNullOrEmpty(item.routingParams.userIDsFromField) ||
                        !string.IsNullOrEmpty(item.routingParams.orgIDFromField) ||
                        !string.IsNullOrEmpty(item.routingParams.roleIDsFromField))
                    {
                        routine.OwnerType = OwnerType.None;
                    }
                    else
                    {
                        routine.OwnerType = OwnerType.None;
                    }
                }
                else
                {
                    if (item.routingParams.selectMode == "SelectOneUserInScope" || item.routingParams.selectMode == "SelectOnePrjUser")
                    {
                        routine.OwnerType = OwnerType.Single;
                    }
                    else if (item.routingParams.selectMode == "SelectMultiUserInScope" || item.routingParams.selectMode == "SelectMultiPrjUser")
                    {
                        routine.OwnerType = OwnerType.Multi;
                    }
                    else
                    {
                        routine.OwnerType = OwnerType.Special;
                    }

                    if (item.routingParams.selectAgain)
                    {
                        if (item.routingParams.selectMode == "SelectOneUserInScope" || item.routingParams.selectMode == "SelectOnePrjUser" || item.routingParams.selectMode == "SelectOneUser")
                        {
                            routine.OwnerType = OwnerType.Single;
                            if (item.routingParams.selectMode == "SelectOneUser")
                            {
                                routine.OwnerUserIDs = "";
                            }
                        }
                        else if (item.routingParams.selectMode == "SelectMultiUserInScope" || item.routingParams.selectMode == "SelectMultiPrjUser" || item.routingParams.selectMode == "SelectMultiUser")
                        {
                            routine.OwnerType = OwnerType.Multi;
                            if (item.routingParams.selectMode == "SelectMultiUser")
                            {
                                routine.OwnerUserIDs = "";
                            }
                        }
                    }
                }
            }

            //排序需要与PC端一致，故反过来
            for (int i = 0; i < staticRoutines.Count; i++)
                Routines.Add(staticRoutines[i]);

            return Routines;
        }
        private S_WF_InsTaskExec GetTaskExec(string taskExecID)
        {
            WorkflowEntities flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
            S_WF_InsTaskExec task = flowEntities.S_WF_InsTaskExec.FirstOrDefault(p => p.ID == taskExecID);
            return task;
        }


        System.Web.Mvc.JsonResult IFlowController.GetTaskExec(string taskexecID)
        {
            throw new NotImplementedException();
        }


        public System.Web.Mvc.JsonResult SetSerialNumber(string id)
        {
            //return System.Web.Mvc.JsonResult()
            throw new NotImplementedException();
        }

        private string GetHostUrl()
        {
            var schema = this.Request.RequestUri.Scheme;
            var host = HttpContext.Current.Request.Headers["host"];
            return schema + "://" + host;
        }
    }
}
