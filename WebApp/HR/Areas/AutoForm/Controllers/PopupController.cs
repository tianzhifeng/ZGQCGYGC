using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using MvcAdapter;
using HR.Logic.BusinessFacade;
namespace HR.Areas.AutoForm.Controllers
{
    public class PopupController : BaseController
    {
        public ActionResult EmployeeSelect()
        {
            string MultiSelect = Request["MultiSelect"];
            MultiSelect = (string.IsNullOrWhiteSpace(MultiSelect)) ? "false" : MultiSelect;
            ViewBag.MultiSelect = MultiSelect;
            return View();
        }
        public JsonResult GetEmployeeList(QueryBuilder qb, string employeeState)
        {
            GridData data = null;
            if (employeeState == "0")
            {
                data = EmployeeServiceAuto.GetIncumbencyEmployeeData(qb);
            }
            else if (employeeState == "1")
            {
                data = EmployeeServiceAuto.GetRetiredEmployeeData(qb);
            }
            else
            {
                data = EmployeeServiceAuto.GetEmployeeData(qb);
            }

            return Json(data);
        }

    }
}
