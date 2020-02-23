using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class CommonController : BaseController
    {
        /// <summary>
        /// 根据省份获取城市枚举
        /// </summary>
        /// <param name="province"></param>
        /// <returns></returns>
        public JsonResult GetCityEnum(string province)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("SELECT * FROM S_M_EnumItem WHERE EnumDefID IN (SELECT ID FROM S_M_EnumDef Where Code='System.City') AND SubEnumDefCode='{0}'  Order by SortIndex", province);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            List<object> list = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new { text = dt.Rows[i]["Name"], value = dt.Rows[i]["Code"] });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取枚举
        /// </summary>
        /// <param name="EnumCode"></param>
        /// <returns></returns>
        public JsonResult GetEnum(string EnumCode)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("SELECT * FROM S_M_EnumItem WHERE EnumDefID IN (SELECT ID FROM S_M_EnumDef Where Code='{0}')  Order by SortIndex", EnumCode);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            List<object> list = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new { text = dt.Rows[i]["Name"], value = dt.Rows[i]["Code"], id = dt.Rows[i]["Code"] });
            }
            return Json(list);
        }
    }
}
