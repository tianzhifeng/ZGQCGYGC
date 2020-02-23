using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula.Helper;

namespace Base.Areas.UI.Controllers
{
    public class DataSourceController : BaseController<S_UI_DataSource>
    {
        #region 基本信息

        public override ActionResult Edit()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        #endregion

    }
}
