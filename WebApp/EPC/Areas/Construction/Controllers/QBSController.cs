using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;
using Newtonsoft.Json;

namespace EPC.Areas.Construction.Controllers
{
    public class QBSController : EPCController<S_Q_QBS>
    {
        //
        // GET: /Construction/QBS/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetTreeList(string engineeringInfoID)
        {
            var result = new List<Dictionary<string, object>>();
            var qbsData = this.entities.Set<S_Q_QBS>().Where(x => x.EngineeringInfoID == engineeringInfoID&&(x.NodeType== "UnitConstruction"|| x.NodeType == "SubWork")).OrderBy(y => y.SortIndex).ToList();

            foreach (var item in qbsData)
            {
                var itemDic = FormulaHelper.ModelToDic<S_Q_QBS>(item);
                result.Add(itemDic);
            }
            return Json(result);
        }

    }
}
