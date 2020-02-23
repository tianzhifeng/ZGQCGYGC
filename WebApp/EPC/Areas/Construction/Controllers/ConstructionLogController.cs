using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic;
using EPC.Logic.Domain;
using Workflow.Logic.Domain;
using Base.Logic.Domain;

namespace EPC.Areas.Construction.Controllers
{
    public class ConstructionLogController : EPCController
    {
        //
        // GET: /ConstructionManagement/ConstructionLog/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Log()
        {
            string sql = "select  Date,IsHoliday from S_C_Holiday where Year='" + DateTime.Now.Year + "' ";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var engineeringInfoID = Request["EngineeringInfoID"];
            var dt = db.ExecuteDataTable(sql);
            ViewBag.Holiday = JsonHelper.ToJson(dt);
            var entities = new EPCEntities();
            var dateList = entities.T_C_ConstructionLog.Where(d => d.EngineeringInfo == engineeringInfoID).Select(d => d.DateD).Distinct().ToList();
            ViewBag.FilledDate = String.Join(",", dateList);
            return View();
        }

        public JsonResult GetConstructionLog(string EngineeringInfoID,DateTime? DateD)
        {            
            T_C_ConstructionLog log = entities.Set<T_C_ConstructionLog>().Where(x => x.EngineeringInfo == EngineeringInfoID && x.DateD == DateD)
                .OrderByDescending(a=>a.CreateDate).FirstOrDefault();

            return Json(log);
        }
    }
}
