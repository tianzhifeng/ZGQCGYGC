using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;
using Base.Logic.Domain;

namespace Base.Areas.ShortMsg.Controllers
{
    public class MsgController : BaseController
    {
        //
        // GET: /ShortMsg/Msg/

        public ActionResult Index()
        {
            ViewBag.Title = "我的消息";
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            ViewBag.MessageArea = LGID ? "Area" : "消息区";
            return View();
        }


        public ActionResult List()
        {
            return View();
        }

        public JsonResult GetAllList(MvcAdapter.QueryBuilder qb)
        {
            string userID = FormulaHelper.UserID;
            return Json(entities.Set<S_S_MsgBody>().WhereToGridData(qb));
        }

        public JsonResult Delete()
        {
            return base.JsonDelete<S_S_MsgBody>(Request["ListIDs"]);
        }
    }
}
