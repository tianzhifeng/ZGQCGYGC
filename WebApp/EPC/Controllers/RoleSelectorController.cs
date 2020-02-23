using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcAdapter;
using Config;
using Config.Logic;
using Formula.Helper;
using System.Data;

namespace EPC.Controllers
{
    public class RoleSelectorController : InfrastructureController
    {
        public JsonResult GetSysRoleList(QueryBuilder qb)
        {
            string sql = @"select distinct Code as RoleCode,Name as RoleName,Description from S_A_Role where Type='SysRole'";
            var db = Config.SQLHelper.CreateSqlHelper(Config.ConnEnum.Base);
            var data = db.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetPrjRoleList(QueryBuilder qb)
        {
            string sql = "select distinct RoleName ,RoleCode,SortIndex from S_T_RoleDefine";
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetCooperationRoleList(QueryBuilder qb)
        {
            var dt = EnumBaseHelper.GetEnumTable(typeof(EPC.Logic.CoopertationRoleType));
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow item in dt.Rows)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("RoleCode",item["value"]);
                dic.SetValue("RoleName", item["text"]);
                list.Add(dic);
            }
            return Json(list);
        }
    }
}
