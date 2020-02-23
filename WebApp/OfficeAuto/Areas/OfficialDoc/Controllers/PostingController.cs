using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Controllers;
using OfficeAuto.Logic.Domain;
using OfficeAuto.Logic;
using Config;
using Workflow.Logic.Domain;
using Formula;
using Workflow.Logic;
using System.IO;
using WebOffice.Logic;
using Formula.Helper;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OfficeAuto.Areas.OfficialDoc.Controllers
{
    public class PostingController : BaseFlowController<S_D_Posting>
    {
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /OfficialDoc/Posting/
        public override ActionResult List()
        {
            return base.List();
        }

        public ActionResult ListView()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult MainBody()
        {
            return View();
        }

        public ActionResult SelectHongTou()
        {
            return View();
        }

        public JsonResult GetHongTou()
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            string tmplPath = System.Configuration.ConfigurationManager.AppSettings["HongTouTemplate"];
            string directoryPath = System.Web.HttpContext.Current.Server.MapPath(tmplPath);
            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath);
                foreach (string file in files)
                {
                    Dictionary<string, string> item = new Dictionary<string, string>();
                    item["value"] = file;
                    item["text"] = file.Split('\\')[file.Split('\\').Length - 1];
                    list.Add(item);
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TaoHong(string id, string hongtou, string mergeDocID, string docID, string zihao, string title)
        {
            S_D_Posting model = entities.Set<S_D_Posting>().Find(id);
            Regex reg = new Regex(@"^(?<fpath>([a-zA-Z]:\\)([\s\.\-\w]+\\)*)(?<fname>[\w]+.[\w]+)");
            if (reg.IsMatch(hongtou))
            {
                mergeDocID = OfficeAuto.Logic.PostingFO.TaoHong(hongtou, mergeDocID, docID, zihao, title);
            }
            else
            {
                byte[] buffer = WebOfficeSettings.Instance.Storage.GetFile(hongtou);
                mergeDocID = OfficeAuto.Logic.PostingFO.TaoHong(buffer, mergeDocID, docID, zihao, title);
            }
            model.MergeDocID = mergeDocID;
            entities.SaveChanges();
            return Json(model);
        }

        public JsonResult Sign(string id, string mergeDocID, string docID)
        {
            S_D_Posting model = entities.Set<S_D_Posting>().Find(id);
            mergeDocID = OfficeAuto.Logic.PostingFO.Sign(id, mergeDocID, docID);
            model.MergeDocID = mergeDocID;
            entities.SaveChanges();
            return Json(model);
        }

        public override JsonResult GetModel(string id)
        {
            S_D_Posting model = GetEntity<S_D_Posting>(id);
            if (string.IsNullOrEmpty(id))
            {
                UserInfo user = Formula.FormulaHelper.GetUserInfo();
                model.Drafter = user.UserName;
                model.DrafterID = user.UserID;
                model.DraftDate = DateTime.Now.Date;
                model.DraftDept = user.UserOrgName;
                model.DraftDeptID = user.UserOrgID;
                model.Status = PostingStatus.Draft.ToString();
                model.DocID = PostingFO.CreateEmptyWord();
            }
            return Json(model);
        }

        public JsonResult GetSWFFile(string docID)
        {
            var rtn = new
                {
                    File = PostingFO.GetSWFFile(docID)
                };
            return Json(rtn);
        }

        public override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            WorkflowEntities entitiesFlow = FormulaHelper.GetEntities<WorkflowEntities>();
            bool flowComplete = base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
            S_D_Posting model = entities.Set<S_D_Posting>().Find(taskExec.S_WF_InsFlow.FormInstanceID);
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
                    if (model.Status == PostingStatus.Draft.ToString())
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
                    //转PDF
                    if (steps.Find(c => c.ID == stepID).Code == PostingStatus.TaoHong.ToString())
                    {
                        if (string.IsNullOrEmpty(model.MergeDocID))
                            model.MergeDocID = model.DocID;
                        PostingFO.AddPDFTask(model.MergeDocID);
                    }
                    entities.SaveChanges();
                }
            }

            return flowComplete;
        }

        public JsonResult GetListView(MvcAdapter.QueryBuilder qb)
        {
            string status = PostingStatus.Complete.ToString();
            IQueryable<S_D_Posting> query = entities.Set<S_D_Posting>().Where(c => c.Status == status);
            string companyDeptID = GetQueryString("CompanyDeptID");
            UserInfo user = FormulaHelper.GetUserInfo();
            string userID = user.UserID;
            if (!string.IsNullOrEmpty(companyDeptID))
                query = query.Where(c => c.CompanyDeptID == companyDeptID);
            MvcAdapter.GridData gridData = query.WhereToGridData(qb);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }

        #region 撤销流程
        public override JsonResult DeleteFlow()
        {
            string id = GetQueryString("ID");
            PostingFO.ResetStatus(id);
            return base.DeleteFlow();
        }

        public override void UnExecTaskExec(string taskExecID)
        {
            string id = GetQueryString("ID");
            WorkflowEntities flowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
            S_WF_InsTaskExec taskExec = flowEntities.Set<S_WF_InsTaskExec>().SingleOrDefault(c => c.ID == taskExecID);
            string stepCode = taskExec.S_WF_InsTask.S_WF_InsDefStep.Code;
            PostingFO.ChangeStatus(id, stepCode);
            base.UnExecTaskExec(taskExecID);
        }
        #endregion

        #region 流程监控
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
            MvcAdapter.GridData gridData = entities.Set<S_D_Posting>().Where(c => !string.IsNullOrEmpty(c.InsFlowID)).WhereToGridData(qb);
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
            List<S_D_Posting> list = entities.Set<S_D_Posting>().Where(c => c.InsFlowID == flowID).ToList();
            foreach (S_D_Posting item in list)
            {
                item.InsFlowID = null;
                item.ExecutedSteps = string.Empty;
                item.Status = PostingStatus.Draft.ToString();
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
            string type = CommentType.Posting.ToString();
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
            List<S_D_Comment> list = CommentFO.GetComments(CommentType.Posting, userID);
            return Json(list);
        }

        public JsonResult AddComment(string comment, string userID, string userName)
        {
            bool isAdd = CommentFO.Add(CommentType.Posting, comment, userID, userName);
            return Json(isAdd);
        }
        
        #endregion
    }
}
