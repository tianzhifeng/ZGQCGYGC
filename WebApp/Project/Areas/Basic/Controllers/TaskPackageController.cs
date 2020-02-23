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

namespace Project.Areas.Basic.Controllers
{
    public class TaskPackageController : ProjectController<S_W_TaskWork>
    {
        public ActionResult TaskPackageImport()
        {
            ViewBag.Phase = JsonHelper.ToJson(Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Phase));
            ViewBag.Major = JsonHelper.ToJson(Project.Logic.BaseConfigFO.GetWBSEnum(WBSNodeType.Major));
            return View();
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            var baseConfigEntities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var packageDic = baseConfigEntities.Set<S_D_PackageDic>().ToList();
            var MajorCode = this.Request["MajorCode"];
            var PhaseCode = this.Request["PhaseCode"];
            if (!String.IsNullOrEmpty(MajorCode))
                packageDic = packageDic.Where(a => a.MajorCode == MajorCode).ToList();
            if (!String.IsNullOrEmpty(PhaseCode))
                packageDic = packageDic.Where(a => PhaseCode.Contains(a.PhaseCode)).ToList();

            //string sql = "select * from S_D_PackageDic ";
            //if (!String.IsNullOrEmpty(this.Request["MajorCode"]))
            //    qb.Add("MajorCode", QueryMethod.Equal, this.Request["MajorCode"]);
            //if (!String.IsNullOrEmpty(this.Request["PhaseCode"]))
            //    qb.Add("PhaseCode", QueryMethod.Like, this.Request["PhaseCode"]);
            //var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.ProjectBaseConfig);
            //var data = sqlHelper.ExecuteGridData(sql, qb);
            return Json(packageDic);
        }

        public JsonResult GetProjectList(QueryBuilder qb)
        {
            var data = this.entities.Set<S_I_ProjectInfo>().WhereToGridData(qb);
            return Json(data);
        }

        public override JsonResult GetTree()
        {
            string ProjectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            string majorCode = this.Request["MajorValue"];
            var list = projectInfo.S_W_WBS.ToList();
            return Json(list);
        }
    }
}
