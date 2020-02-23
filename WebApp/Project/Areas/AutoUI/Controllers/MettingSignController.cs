using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Workflow.Logic;
using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using Base.Logic.BusinessFacade;
using System.Data;
using Config.Logic;
using Workflow.Logic.Domain;

namespace Project.Areas.AutoUI.Controllers
{
    public class MettingSignController : ProjectFormContorllor<T_EXE_MettingSign>
    {
        public override ActionResult PageView()
        {
            string userName = this.GetQueryString("SystemName");
            if (!string.IsNullOrEmpty(userName))
            {
                userName = HttpUtility.UrlDecode(userName);
                string pwd = this.GetQueryString("PWD");
                string sql = "select count(0) from S_A_User where Code ='" + userName + "'";
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                if (Convert.ToInt32(db.ExecuteScalar(sql)) > 0 && !String.IsNullOrEmpty(userName))
                {
                    FormulaHelper.ContextSet("AgentUserLoginName", userName);
                    FormulaHelper.SetAuthCookie(userName);
                }
            }
            return base.PageView();
        }

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            var batchID = this.GetQueryString("BatchID");
            if (!string.IsNullOrEmpty(batchID) && dt.Rows.Count > 0)
            {
                if (!dt.Columns.Contains("ResultList"))
                    dt.Columns.Add("ResultList");
                if (dt.Rows[0]["ResultList"] != DBNull.Value && dt.Rows[0]["ResultList"] != null
                && !string.IsNullOrEmpty(dt.Rows[0]["ResultList"].ToString()))
                {
                    var detailList = JsonHelper.ToObject<List<Dictionary<string, object>>>(dt.Rows[0]["ResultList"].ToString());
                    if (detailList.Count > 0)
                        return;
                }
                var pdDt = this.ProjectSQLDB.ExecuteDataTable("select * from S_E_Product where BatchID='" + batchID + "'");
                var list = FormulaHelper.DataTableToListDic(pdDt);
                foreach (var item in list)
                {
                    item.SetValue("ProductID", item.GetValue("ID"));
                    item.SetValue("ProductVersion", item.GetValue("Version"));
                    item.SetValue("ID", "");
                    item.SetValue("Major", item.GetValue("MajorName"));
                }
                dt.Rows[0]["ResultList"] = JsonHelper.ToJson(list);
            }
            //base.AfterGetData(dt, isNew, upperVersionID);
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "ResultList")
            {

                var ids = detailList.Where(c => c.ContainsKey("ID") && !string.IsNullOrEmpty(c["ID"])).Select(c => c["ID"]).ToList();
                //获取全部子表项ID
                var oldIds = this.ProjectSQLDB.ExecuteDataTable(string.Format("select ID from T_EXE_MettingSign_ResultList where T_EXE_MettingSignID='{0}'", dic.GetValue("ID")))
                   .AsEnumerable().Select(c => c.Field<string>("ID")).ToList();
                string notExistIDs = string.Join("','", oldIds.Where(c => !ids.Contains(c)).ToArray());
                //将删除成果的会签状态设置为空
                var products = this.BusinessEntities.Set<T_EXE_MettingSign_ResultList>().Where(a => notExistIDs.Contains(a.ID)).ToList();
                foreach (var item in products)
                {
                    var product = this.BusinessEntities.Set<S_E_Product>().Find(item.ProductID);
                    if (product != null)
                    {
                        product.CoSignState = Project.Logic.CoSignState.NoSign.ToString();
                        product.CounterSignAuditID = "";
                        product.UpdateVersison();
                    }
                }
            }
            base.BeforeSaveSubTable(dic, subTableName, detailList, formInfo);
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var products = JsonHelper.ToList(dic.GetValue("ResultList"));
            foreach (var item in products)
            {
                var product = this.GetEntityByID<S_E_Product>(item.GetValue("ProductID"));
                if (product != null)
                {
                    product.CoSignState = Project.Logic.CoSignState.Sign.ToString();
                    product.CounterSignAuditID = dic.GetValue("ID");
                    product.UpdateVersison();
                }
            }
        }

        public override JsonResult Delete()
        {
            var ids = Request["ID"];
            var products = this.BusinessEntities.Set<T_EXE_MettingSign_ResultList>().Where(a => ids.Contains(a.T_EXE_MettingSignID)).ToList();
            foreach (var item in products)
            {
                var product = this.BusinessEntities.Set<S_E_Product>().Find(item.ProductID);
                if (product != null)
                {
                    product.CoSignState = Project.Logic.CoSignState.NoSign.ToString();
                    product.CounterSignAuditID = "";
                    product.UpdateVersison();
                }
            }
            flowService.Delete(Request["ID"], Request["TaskExecID"], Request["ListIDs"]);
            this.BusinessEntities.SaveChanges();
            return Json("");
        }

        public override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            var user = FormulaHelper.GetUserInfo();
            Dictionary<string, object> formDic = Formula.Helper.JsonHelper.ToObject(Request["formData"]);
            string formID = formDic.GetValue("ID");
            var MettingSign = this.BusinessEntities.Set<T_EXE_MettingSign>().Find(formID);
            if (MettingSign == null)
                throw new Formula.Exceptions.BusinessException("未找到会签流程表单信息");

            #region 将流程意见写入详细列表的意见栏中
            var ProjectGroupMembersList = this.BusinessEntities.Set<T_EXE_MettingSign_ProjectGroupMembers>().Where(c => c.T_EXE_MettingSignID == formID).ToList();
            foreach (var item in ProjectGroupMembersList)
            {
                if (item.MettingUser.Contains(user.UserID))
                {
                    item.SignDate = DateTime.Now;
                    if (string.IsNullOrEmpty(item.SignComment))
                        item.SignComment = execComment;
                    else
                        item.SignComment += ";" + execComment;
                }
            }
            #endregion
            this.BusinessEntities.SaveChanges();

            if (MettingSign != null)
            {
                var workFlowEntities = FormulaHelper.GetEntities<WorkflowEntities>();
                var nextStep = workFlowEntities.Set<S_WF_InsDefStep>().SingleOrDefault(c => c.ID == routing.EndID);
                var receiverIds = nextExecUserIDs;
                var receiverNames = nextExecUserNames;
                var title =/* currentStep.Name + routing.Name +*/ "会签通知";
                var content = "【" + MettingSign.ProjectInfoName + "】";
                if (!string.IsNullOrEmpty(MettingSign.SubProjectName))
                    content += "【" + MettingSign.SubProjectName + "】";
                if (!string.IsNullOrEmpty(MettingSign.Phase))
                    content += "【" + FormulaHelper.GetService<IEnumService>().GetEnumText("Project.Phase", MettingSign.Phase) + "】";
                if (routing.Code == "End")
                {
                    title = "会签流程通过通知";
                    content += "有成果通过会签流程";
                    receiverIds = MettingSign.CreateUserID;
                    receiverNames = MettingSign.CreateUser;
                }
                else
                    content += "需要" + nextStep.Name;
                FormulaHelper.CreateFO<ProjectInfoFO>().SendNotice(MettingSign.ProjectInfoID, MettingSign.WBSID, MettingSign.Major, "",
                    receiverIds, receiverNames, title, content, "", "", MettingSign.ID, "T_EXE_MettingSign",ProjectNoticeType.Major.ToString(), "", "");
            }

            return base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
        }

        protected override void OnFlowEnd(T_EXE_MettingSign entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
                entity.Push();
            this.BusinessEntities.SaveChanges();
        }

        public string[] GetMajorCodeByUserID(string userid, string projectInfoID)
        {
            string[] majorinfo = { "", "" };
            var obsuser = this.BusinessEntities.Set<S_W_OBSUser>().FirstOrDefault(c => c.UserID == userid && c.ProjectInfoID == projectInfoID && !string.IsNullOrEmpty(c.MajorName));
            if (obsuser != null)
            {
                majorinfo[0] = obsuser.MajorValue;
                majorinfo[1] = obsuser.MajorName;
            }
            return majorinfo;
        }

    }
}
