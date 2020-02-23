using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;

namespace MvcConfig.Areas.Auth.Controllers
{
    public class ResController : BaseController
    {
        public ActionResult Selector()
        {
            return View();
        }

        public JsonResult GetTree()
        {
            string fullID = Request["RootFullID"];
            if (string.IsNullOrEmpty(fullID))
                return Json("", JsonRequestBehavior.AllowGet);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            if (System.Configuration.ConfigurationManager.AppSettings["Auth_PowerDiscrete"].ToLower() == "true")
                return Json(sqlHelper.ExecuteDataTable(string.Format("select * from S_A_Res where FullID like '{0}%' and FullID not like '{1}%' order by ParentID,SortIndex", fullID, Config.Constant.SystemMenuFullID), new SearchCondition()), JsonRequestBehavior.AllowGet);
            else
                return Json(sqlHelper.ExecuteDataTable(string.Format("select * from S_A_Res where FullID like '{0}%' order by ParentID,SortIndex", fullID), new SearchCondition()), JsonRequestBehavior.AllowGet);

        }


    }
}
