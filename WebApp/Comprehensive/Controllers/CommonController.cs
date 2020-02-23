using Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Comprehensive.Controllers
{
    public class CommonController : ComprehensiveController
    {
        //根据枚举值获取该枚举对应子枚举
        public JsonResult GetSubEnum(string SubEnumKey, string Category)
        {
            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("SELECT * FROM S_M_EnumItem WHERE EnumDefID IN (SELECT ID FROM S_M_EnumDef Where Code='{0}') AND Category like '%{1}%' Order by SortIndex", SubEnumKey, Category);
            DataTable dt = baseHelper.ExecuteDataTable(sql);
            List<object> list = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new { text = dt.Rows[i]["Name"], value = dt.Rows[i]["Code"] });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
