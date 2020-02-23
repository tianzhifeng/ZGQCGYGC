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
    public class CBSSelectorController : EPCController
    {
        public JsonResult GetCBSTree(string EngineeringInfoID)
        {
            var sql = "select * from S_I_CBS with(nolock)  where EngineeringInfoID='" + EngineeringInfoID + "'";
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            return Json(dt);
        }
    }
}
