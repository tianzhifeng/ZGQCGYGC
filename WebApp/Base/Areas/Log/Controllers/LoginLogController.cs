using Config;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Base.Areas.Log.Controllers
{
    public class LoginLogController : BaseController
    {
        public JsonResult GetList(QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var timeWhere = "";
            if (qb.Items.Count(a => a.Field == "LoginDate") == 0 || !string.IsNullOrEmpty(GetQueryString("IsFilter")))
            {
                var timeFilter = GetQueryString("TimeFilter");
                if (string.IsNullOrEmpty(timeFilter)) timeFilter = "0";
                timeWhere = string.Format(" and LoginDate>='{0}'", System.DateTime.Now.Date.AddDays(0 - int.Parse(timeFilter)));
            }
            var sql = string.Format("select * from S_L_LoginLog where 1=1 {0}", timeWhere);
            var list = sqlHelper.ExecuteGridData(sql, qb);
            return Json(list);
        }
    }
}