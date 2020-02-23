using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using MvcAdapter;

namespace EPC.Controllers
{
    public class PBSSelectorController : EPCController
    {
        public JsonResult GetPBSTree(string EngineeringInfoID)
        {
            var data = this.entities.Set<S_I_PBS>().Where(c => c.EngineeringInfoID == EngineeringInfoID).ToList();
            var tmpList = data.Select(a => new { ID = a.ID, ParentID = a.ParentID, Name = a.Name, CanSelect = a.StructNodeInfo.Children.Count == 0 });
            return Json(tmpList);
        }
    }
}
