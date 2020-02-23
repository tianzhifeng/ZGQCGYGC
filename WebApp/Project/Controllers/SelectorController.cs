using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Config;
using Project.Logic.Domain;
using Project.Logic;
namespace Project.Controllers
{
    public class SelectorController : ProjectController
    {

        public ActionResult SelectProject()
        {
            return View();
        }

        public JsonResult GetProjectInfo(QueryBuilder qb)
        {
            var grid = entities.Set<S_I_ProjectInfo>().WhereToGridData(qb);

            return Json(grid);
        }

    }
}
