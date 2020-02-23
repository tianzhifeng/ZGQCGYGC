using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Config;
using MvcAdapter;
using Formula;
using System.Data;

namespace Comprehensive.Areas.OfficeAuto.Controllers
{
    public class ConferenceController : ComprehensiveFormController<T_M_ConferenceApply_Budget>
    {

        public JsonResult getLDSJ(QueryBuilder qb)
        {
            var ids = Request ["ID"];

            string strSql = string.Format("Select * From T_M_ConferenceApply_Budget Where T_M_ConferenceApplyID='{0}' ", ids);
            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Comprehensive);
            DataTable dt = baseHelper.ExecuteDataTable(strSql);
            return Json(dt);
        }

    }
}
