using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Collections;
using Formula;
using Formula.Helper;
using Config;
using Project.Logic;
using Project.Logic.Domain;



namespace Project.Areas.Extend.Controllers
{
    public class WorkSpaceController : ProjectController
    {
        public ActionResult CooperationPlanSpace()
        {
            ViewBag.ProjectInfoID = GetQueryString("ProjectInfoID");
            ViewBag.SpaceCode = GetQueryString("SpaceCode");
            return View();
        }
        public ActionResult WorkSpaceMain()
        {
            string projectInfoID = GetQueryString("ProjectInfoID");
            string spaceCode = GetQueryString("SpaceCode");

            var projectInfo = entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.ID == projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("找不到ID为【" + projectInfoID + "】的项目！");
            if (projectInfo.WBSRoot == null) throw new Formula.Exceptions.BusinessException("找不到项目对应的WBS根节点！");
            ViewBag.ProjectInfo = projectInfo;
            ViewBag.SpaceCode = spaceCode;
            ViewBag.WBSRoot = projectInfo.WBSRoot;
            var transform = projectInfo.ProjectMode.S_T_WBSStructInfo.Where(d => d.CanTransform == true.ToString()
               && d.Code != WBSNodeType.Work.ToString() && d.Code != WBSNodeType.Project.ToString()).
               Select(c => new { value = c.Code, text = "按" + c.Name, index = 1 }).ToList();
            transform.Add(new { value = "Project", text = "默认", index = 0 });
            transform.Add(new { value = "Progress", text = "按时间", index = 2 });
            ViewBag.TransForm = JsonHelper.ToJson(transform.OrderBy(d => d.index).ToList());
            var rootStruct = projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Project.ToString());
            ViewBag.DefaultViewType = rootStruct.Code + "," + rootStruct.ChildCode;
            return View();
        }

        public ActionResult DesignWorkSpace()
        {
            string projectInfoID = GetQueryString("ProjectInfoID");
            string spaceCode = GetQueryString("SpaceCode");

            var projectInfo = entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.ID == projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("找不到ID为【" + projectInfoID + "】的项目！");
            if (projectInfo.WBSRoot == null) throw new Formula.Exceptions.BusinessException("找不到项目对应的WBS根节点！");
            ViewBag.ProjectInfo = projectInfo;
            ViewBag.SpaceCode = spaceCode;
            
            return View();
        }

        public JsonResult GetTree()
        {
            string projectInfoID = GetQueryString("ProjectInfoID");
            string spaceCode = GetQueryString("SpaceCode");
            string viewType = GetQueryString("ViewType");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象，无法展现工作区");
            var wbsFO = FormulaHelper.CreateFO<WBSFO>();
            var result = new List<Dictionary<string, object>>();
            var projectFO = FormulaHelper.CreateFO<ProjectInfoFO>();
            result = projectFO.GetWorkSpaceWBS(projectInfoID, this.CurrentUserInfo.UserID, viewType, spaceCode);
            return Json(result);
        }

    }
}
