using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using Config;
using OfficeAuto.Controllers;
using OfficeAuto.Logic;
using Workflow.Logic.Domain;
using Formula;
using Workflow.Logic;
using System.Reflection;

namespace OfficeAuto.Areas.OfficialDoc.Controllers
{
    public class IncomingController : BaseFlowController<S_D_Incoming>
    {
        //
        // GET: /OfficialDoc/Incoming/
        WorkflowEntities entitiesFlow = FormulaHelper.GetEntities<WorkflowEntities>();

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult ListView()
        {
            return View();
        }

        public override JsonResult GetModel(string id)
        {
            S_D_Incoming model = GetEntity<S_D_Incoming>(id);
            if (string.IsNullOrEmpty(id))
            {
                UserInfo user = Formula.FormulaHelper.GetUserInfo();
                model.Applicant = user.UserName;
                model.ApplicantID = user.UserID;
                model.Status = IncomingStatus.Register.ToString();
            }
            return Json(model);
        }

        public JsonResult GetYears()
        {
            List<Dictionary<string, object>> years = new List<Dictionary<string, object>>();
            for (int i = 2014; i < 2050; i++)
            {
                Dictionary<string, object> year = new Dictionary<string, object>();
                year["value"] = i;
                year["text"] = i.ToString();
                years.Add(year);
            }
            return Json(years, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCode(string id, string code)
        {
            if (!IncomingFO.ValidateUniqueCode(id, code))
            {
                throw new Exception("收文编号不唯一!");
            }
            return Json(string.Empty);
        }

        public override JsonResult Save()
        {
            S_D_Incoming model = UpdateEntity<S_D_Incoming>();
            
            return base.Save();
        }

        public override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            bool flowComplete = base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
            S_D_Incoming model = entities.Set<S_D_Incoming>().Find(taskExec.S_WF_InsFlow.FormInstanceID);
            if (taskExec.S_WF_InsTask.Status == FlowTaskStatus.Complete.ToString())
            {
                if (model != null)
                {
                    if (string.IsNullOrEmpty(model.InsFlowID))
                    {
                        model.InsFlowID = taskExec.S_WF_InsTask.InsFlowID;
                    }

                    string[] arr = new string[2] { routing.InsDefStepID, routing.EndID };
                    List<S_WF_InsDefStep> steps = entitiesFlow.Set<S_WF_InsDefStep>().Where(c => arr.Contains(c.ID)).ToList();
                    string endID = routing.EndID;
                    if (steps.Find(c => c.ID == endID) != null)
                    {
                        model.Status = steps.Find(c => c.ID == endID).Code;
                    }
                    string exStatus = string.Empty;
                    string stepID = routing.InsDefStepID;
                    if (steps.Find(c => c.ID == stepID) != null)
                    {
                        exStatus = steps.Find(c => c.ID == stepID).Code;
                    }
                    if (model.Status == IncomingStatus.Register.ToString())
                    {
                        string[] arrExecSteps = string.IsNullOrEmpty(model.ExecutedSteps) ? new string[1] { exStatus } : (model.ExecutedSteps + "," + exStatus).Split(',');
                        foreach (string str in arrExecSteps)
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                string[] arrValTxt = new string[2] { str, str + "ID" };
                                foreach (string s in arrValTxt)
                                {
                                    PropertyInfo pi = model.GetType().GetProperty(s, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                    if (pi != null && pi.CanWrite)
                                        pi.SetValue(model, string.Empty, null);
                                }
                            }
                        }
                        model.ExecutedSteps = string.Empty;
                    }
                    else
                    {
                        if (exStatus != string.Empty)
                        {
                            if (string.IsNullOrEmpty(model.ExecutedSteps))
                                model.ExecutedSteps = exStatus;
                            else
                                model.ExecutedSteps += "," + exStatus;
                        }
                    }




                    entities.SaveChanges();
                }
            }

            return flowComplete;
        }

        public JsonResult GetListView(MvcAdapter.QueryBuilder qb)
        {
            string status = PostingStatus.Complete.ToString();
            IQueryable<S_D_Incoming> query = entities.Set<S_D_Incoming>().Where(c => c.Status == status);
            MvcAdapter.GridData gridData = query.WhereToGridData(qb);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        #region 撤销流程
        public override JsonResult DeleteFlow()
        {
            string id = GetQueryString("ID");
            IncomingFO.ResetStatus(id);
            return base.DeleteFlow();
        }

        public override void UnExecTaskExec(string taskExecID)
        {
            string id = GetQueryString("ID");
            WorkflowEntities flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
            S_WF_InsTaskExec taskExec = flowEntities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
            string stepCode = taskExec.S_WF_InsTask.S_WF_InsDefStep.Code;
            IncomingFO.ChangeStatus(id, stepCode);
            base.UnExecTaskExec(taskExecID);
        }
        #endregion

        #region 收文监控
        public ActionResult MonitorTabs()
        {
            return View();
        }

        public ActionResult TaskChange()
        {
            ViewBag.TaskProcessingStatus = FlowTaskStatus.Processing.ToString();
            return View();
        }

        public ActionResult FlowDelete()
        {
            return View();
        }

        public JsonResult GetListFlow(MvcAdapter.QueryBuilder qb)
        {
            MvcAdapter.GridData gridData = entities.Set<S_D_Incoming>().Where(c => !string.IsNullOrEmpty(c.InsFlowID)).WhereToGridData(qb);
            return Json(gridData);
        }

        public JsonResult DeleteInsFlow(string flowID)
        {
            if (string.IsNullOrEmpty(flowID))
            {
                throw new Exception("不存在流程ID");
            }
            string sqlFlow = string.Format("delete from S_WF_InsFlow where ID = '{0}' ", flowID);
            SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow).ExecuteNonQuery(sqlFlow);
            List<S_D_Incoming> list = entities.Set<S_D_Incoming>().Where(c => c.InsFlowID == flowID).ToList();
            foreach (S_D_Incoming item in list)
            {
                item.InsFlowID = null;
                item.ExecutedSteps = string.Empty;
                item.Status = IncomingStatus.Register.ToString();
            }
            entities.SaveChanges();
            return Json(string.Empty);
        }

        #endregion

        #region 批语
        public ActionResult CommentList()
        {
            return View();
        }

        public JsonResult GetCommentList(MvcAdapter.QueryBuilder qb)
        {
            string type = CommentType.Incoming.ToString();
            MvcAdapter.GridData gridData = entities.Set<S_D_Comment>().Where(c => c.Type == type && c.IsTemplate == "1").WhereToGridData(qb);
            return Json(gridData);
        }

        public JsonResult SaveCommentList()
        {
            UpdateList<S_D_Comment>();
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult GetUserComments(string userID)
        {
            List<S_D_Comment> list = CommentFO.GetComments(CommentType.Incoming, userID);
            return Json(list);
        }

        public JsonResult AddComment(string comment, string userID, string userName)
        {
            bool isAdd = CommentFO.Add(CommentType.Incoming, comment, userID, userName);
            return Json(isAdd);
        }

        #endregion
    }
}
