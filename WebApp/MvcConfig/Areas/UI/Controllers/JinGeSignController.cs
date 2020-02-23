using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcConfig.Areas.UI.Controllers
{
    public class JinGeSignController : BaseController
    {
        public JsonResult GetSignList(string formId)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(string.Format("select * from S_UI_JinGeSign where FormId='{0}'", formId));
            return Json(dt);
        }
    }
}
