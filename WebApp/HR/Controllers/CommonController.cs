using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HR.Controllers
{
    public class CommonController : BaseController
    {

        public JsonResult GetYearEnum()
        {
            List<object> list = new List<object>();
            for (int year = 2000; year <= DateTime.Now.Year + 5; year++)
            {
                list.Add(new { value = year, text = year });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}
