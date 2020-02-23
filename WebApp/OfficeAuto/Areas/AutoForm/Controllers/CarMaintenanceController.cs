using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.BusinessFacade;
using OfficeAuto.Logic.Domain;


namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class CarMaintenanceController : OfficeAutoFormContorllor<T_Car_Maintenance>
    {
        //
        // GET: /AutoForm/CarMaintenance/

        public JsonResult GetMaintenanceCategorySmallEnum(string bigType)
        {
            List<object> list = CommonMethod.GetSubEnum("OA.MaintenanceCategorySmall", bigType);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
