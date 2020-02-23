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

namespace EPC.Areas.HSE.Controllers
{
    public class HSESafeTroublesManageController : EPCController
    {
        //
        // GET: /HSE/HSESafeTroublesManage/

        public ActionResult Index()
        {
            return View();
        }

        public void Delete()
        {
            if (!String.IsNullOrEmpty(Request["ListIDs"]))
            {
                var dbContext = FormulaHelper.GetEntities<EPCEntities>();
                foreach (var item in Request["ListIDs"].Split(','))
                {
                    dbContext.T_HSE_Troubles.Delete(x => x.ID == item);
                }
                dbContext.SaveChanges();
            }
        }

    }
}
