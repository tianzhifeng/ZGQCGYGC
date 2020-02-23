using Config;
using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Base.Areas.Log.Controllers
{
    public class MacAddressController : BaseController
    {
        public ActionResult List()
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["Terminal"] == null)
            {
                throw new Exception("没有此功能或者Web.config配置不正确");
            }
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            var sqlHelper = SQLHelper.CreateSqlHelper("Terminal");
            var list = sqlHelper.ExecuteGridData("select * from S_S_UserClientInfo", qb);
            return Json(list);
        }

        public JsonResult SaveMac(string macAddress, string ID)
        {
            var sql = string.Format("update S_S_UserClientInfo set ClientID = '{1}' where ID = '{0}'", ID, macAddress);
            var sqlHelper = SQLHelper.CreateSqlHelper("Terminal");
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult GetModel(string id)
        {
            var sqlHelper = SQLHelper.CreateSqlHelper("Terminal");
            var sql = string.Format("select ClientID OldClientID,* from S_S_UserClientInfo where ID = '{0}'", id);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (dt.Rows.Count > 0)
                dic = Formula.FormulaHelper.DataRowToDic(dt.Rows[0]);
            return Json(dic);
        }

        public JsonResult ReleaseMac(string macInfo)
        {
            var sqlHelper = SQLHelper.CreateSqlHelper("Terminal");
            var list = JsonHelper.ToList(macInfo);
            var ids = new List<string >();
            foreach (var item in list)
                ids.Add(item.GetValue("ID"));
            var sql = string.Format("update S_S_UserClientInfo set ClientID = '' where ID in ('{0}')", string.Join("','", ids));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }
    }
}
