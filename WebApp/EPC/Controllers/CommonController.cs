using Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Controllers
{
    public class CommonController : EPCController
    {
        SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
        /// <summary>
        /// 获取省份枚举
        /// </summary>
        /// <param name="country">国家</param>
        /// <returns></returns>
        public JsonResult GetProvinceEnum(string country)
        {
            string sql = string.Format("SELECT * FROM S_M_EnumItem WHERE EnumDefID IN (SELECT ID FROM S_M_EnumDef Where Code='System.Province') AND Category='{0}' Order by SortIndex", country);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            List<object> list = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new { text = dt.Rows[i]["Name"], value = dt.Rows[i]["Code"] });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取城市枚举
        /// </summary>
        /// <param name="province">省份</param>
        /// <returns></returns>
        public JsonResult GetCityEnum(string province)
        {
            string sql = string.Format("SELECT * FROM S_M_EnumItem WHERE EnumDefID IN (SELECT ID FROM S_M_EnumDef Where Code='System.City') AND SubCategory='{0}'  Order by SortIndex", province);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            List<object> list = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new { text = dt.Rows[i]["Name"], value = dt.Rows[i]["Code"] });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
