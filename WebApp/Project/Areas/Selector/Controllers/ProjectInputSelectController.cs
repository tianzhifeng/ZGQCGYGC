using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;

namespace Project.Areas.Selector.Controllers
{
    public class ProjectInputSelectController : ProjectController
    {
        public ActionResult ProductSelector()
        {
            var tab = new Tab();
            var majorCatagory = CategoryFactory.GetCategory("Project.Major", "所属专业", "MajorValue");
            majorCatagory.Multi = false;
            tab.Categories.Add(majorCatagory);

            var phaseCatagory = CategoryFactory.GetCategory("Project.Phase", "所属阶段", "PhaseValue");
            phaseCatagory.Multi = false;
            tab.Categories.Add(phaseCatagory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public ActionResult BusinessDocSelector()
        {
            var ProjectInfoID = this.GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            ViewBag.EngineeringInfo = projectInfo.EngineeringInfoID;
            return View();
        }

        public JsonResult GetTree()
        {
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("");
            string sql = @"select S_I_ProjectGroup.*,S_I_ProjectInfo.ID as ProjectInfoID from S_I_ProjectGroup
left join S_I_ProjectInfo on S_I_ProjectGroup.ID = S_I_ProjectInfo.GroupID where FullID like '{0}%' and S_I_ProjectGroup.ID !='{1}' ";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format( sql,projectInfo.GroupRootID,projectInfo.GroupID));
            return Json(dt);
        }

        public JsonResult GetProductList(QueryBuilder qb)
        {
            var data = this.entities.Set<S_E_Product>().Where(d => !String.IsNullOrEmpty(d.MainFile)).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetInputList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            qb.SortField = "InputType,ID"; qb.SortOrder = "asc,asc";
            string sql = @"select S_D_Input.ID,S_D_Input.ProjectInfoID,S_D_Input.EngineeringInfoID,
InputType,DocType,Catagory,InfoName+'('+Convert(varchar,isnull(FileCount,0))+')' as InfoName,
CreateUser,LastUploadDate from S_D_Input Left join (select Count(0) as FileCount,Max(CreateDate) as LastUploadDate,InputID 
from S_D_InputDocument group by InputID) FileInfo 
on S_D_Input.ID=FileInfo.InputID";
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }
    }
}
