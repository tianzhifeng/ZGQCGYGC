using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Config;
using System.Data;
/***
 * 总裁桌面
 * **/
namespace HR.Areas.Report.Controllers
{
    public class LeadDesktopController : HRController
    {
    
        public ActionResult Tab()
        {
            return View();
        }


        public ActionResult DeptInfo() {


            ViewBag.DeptID = Formula.FormulaHelper.GetUserInfo().UserOrgID;

            return View();
        }


    }
}
