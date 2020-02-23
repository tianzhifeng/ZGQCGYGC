using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Controllers
{
    public class SWFUploadController : EPCController
    {
        public ActionResult Upload()
        {
            return View();
        }
    }
}
