using Config;
using Config.Logic;
using EPC.Logic.Domain;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class ParamSettingsController : EPCController
    {
        //获取配置
        public JsonResult GetModel(string id)
        {
            string sql = "select * from S_T_DefineParams";
            var result = new Dictionary<string, object>();
            var infraSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = infraSqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                result[row["Code"].ToString()] = row["Value"].ToString();
            }
            return Json(result);
        }
        //保存配置
        public JsonResult Save()
        {
            var dic = new Dictionary<string, object>();
            var formData = this.Request["FormData"];
            dic = JsonHelper.ToObject(formData);
            var infraSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            foreach (var key in dic.Keys)
            {
                if (!String.IsNullOrEmpty(dic.GetValue(key)))
                {
                    var sql = String.Format("select count(ID) from S_T_DefineParams where Code='{0}'", key);
                    var obj = infraSqlHelper.ExecuteScalar(sql);
                    if (Convert.ToInt32(obj) > 0)
                    {
                        sql = String.Format("update S_T_DefineParams set Value='{1}' where Code='{0}'", key, dic.GetValue(key));
                    }
                    else
                    {
                        sql = String.Format("insert into S_T_DefineParams (Code,Value) values ('{0}','{1}')", key, dic.GetValue(key));
                    }
                    infraSqlHelper.ExecuteNonQuery(sql);
                }
            }
            return Json("");
        }
        //应用
        public JsonResult ResetSysParam()
        {
            S_T_DefineParams.Reset();
            return Json("");
        }
    }
}
