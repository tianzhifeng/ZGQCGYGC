using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
namespace HR.Areas.AttendanceInfo.Controllers
{
    public class AttendanceConfigController : BaseController<S_W_AttendanceConfig>
    {
        public override JsonResult GetModel(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                var entity = entities.Set<S_W_AttendanceConfig>().FirstOrDefault();
                if (entity != null)
                {
                    return Json(entity);
                }
            }
            return base.GetModel(id);
        }
    }
}
