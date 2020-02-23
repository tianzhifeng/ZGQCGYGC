using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using Workflow.Logic.Domain;
using Workflow.Logic;
using Formula;
using System.Reflection;
using OfficeAuto.Logic;
using WebOffice.Logic;
using System.Text.RegularExpressions;
using Config;

namespace OfficeAuto.Areas.OfficialDoc.Controllers
{
    public class MemoController : BaseFlowController<T_B_MemoManagement>
    {
        //
        // GET: /OfficialDoc/Memo/
        public override JsonResult GetModel(string id)
        {
            T_B_MemoManagement model = GetEntity<T_B_MemoManagement>(id);
            if (string.IsNullOrEmpty(id))
            {
                model.Status = PostingStatus.Draft.ToString();
            }
            return Json(model);
        }

        public override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            WorkflowEntities entitiesFlow = FormulaHelper.GetEntities<WorkflowEntities>();
            bool flowComplete = base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
            if (taskExec.S_WF_InsTask.Status == FlowTaskStatus.Complete.ToString())
            {
                T_B_MemoManagement model = entities.Set<T_B_MemoManagement>().Find(taskExec.S_WF_InsFlow.FormInstanceID);
                if (model != null)
                {
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

        public JsonResult TaoHong(string id, string hongtou, string mergeDocID, string docID, string zihao, string title)
        {
            T_B_MemoManagement model = entities.Set<T_B_MemoManagement>().Find(id);
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
            T_B_MemoManagement model = entities.Set<T_B_MemoManagement>().Find(id);
            mergeDocID = OfficeAuto.Logic.PostingFO.Sign(id, mergeDocID, docID);
            model.MergeDocID = mergeDocID;
            entities.SaveChanges();
            return Json(model);
        }

    }
}
