using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Collections;
using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using MvcAdapter;
using Project.Logic.Domain;
using Project.Logic;
using Workflow.Logic.BusinessFacade;
using Workflow.Logic.Domain;

namespace Project.Areas.Basic.Controllers
{
    public class DesignController : ProjectController<S_E_Product>
    {
        public ActionResult SubmitForm()
        {
            string wbsID = this.GetQueryString("WBSID");
            var wbs = this.GetEntityByID<S_W_WBS>(wbsID);
            string taskID = this.GetQueryString("TaskWorkID");
            var task = this.GetEntityByID<S_W_TaskWork>(taskID);
            var major = wbs.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Major.ToString());
            ViewBag.ModeCode = wbs.S_I_ProjectInfo.ModeCode;
            ViewBag.MajorCode = major.WBSValue;
            return View();
        }

        public JsonResult GetMainInfo()
        {
            string wbsID = this.GetQueryString("WBSID");
            string taskID = this.GetQueryString("TaskWorkID");
            var dic = new Dictionary<string, object>();
            var wbs = this.GetEntityByID<S_W_WBS>(wbsID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + wbsID + "】的WBS节点");
            var task = this.GetEntityByID<S_W_TaskWork>(taskID);
            if (task == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + taskID + "】的Task节点");

            var subProject = wbs.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.SubProject.ToString());
            var major = wbs.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Major.ToString());
            dic.SetValue("ProjectInfoName", wbs.S_I_ProjectInfo.Name);
            dic.SetValue("ProjectInfoCode", wbs.S_I_ProjectInfo.Code);
            dic.SetValue("TaskWorkName", task.Name);
            dic.SetValue("TaskWorkCode", task.Code);
            if (major != null)
                dic.SetValue("Major", major.WBSValue);
            if (subProject != null)
            {
                dic.SetValue("SubProjectName", subProject.Name);
                dic.SetValue("SubProjectCode", subProject.Code);
            }
            return Json(dic);
        }

        protected override void BeforeDelete(List<S_E_Product> entityList)
        {
            foreach (var item in entityList)
                item.Delete();
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            qb.Add("CreateUserID", QueryMethod.Equal, CurrentUserInfo.UserID);
            return base.GetList(qb);
        }

        public JsonResult ValidationData(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var product = this.GetEntityByID(item.GetValue("ID"));
                if (product.AuditState != AuditState.Create.ToString())
                    throw new Formula.Exceptions.BusinessException("成果【" + product.Name + "】已经提交校审，无法重复提交");
            }
            return Json("");
        }

        public JsonResult FinishTask(string ActivityID)
        {
            var act = this.GetEntityByID<S_W_Activity>(ActivityID);
            if (act == null) throw new Formula.Exceptions.BusinessException("未能找到指定的设计任务，无法进行完成操作");
            act.State = ProjectCommoneState.Finish.ToString();
            var taskWork = this.GetEntityByID<S_W_TaskWork>(act.BusniessID);
            if (taskWork != null)
            {
                taskWork.Finish();
            }
            this.entities.SaveChanges();
            return Json("");
        }

    }
}
