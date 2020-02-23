using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Formula;
using Base.Logic.Domain;
using Config;
using System.Text.RegularExpressions;


namespace MvcConfig.Areas.UI.Controllers
{
    public class DataSourceController : BaseController
    {
        public JsonResult GetPageData(QueryBuilder qb, string tmplCode)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var info = entities.Set<S_UI_DataSource>().SingleOrDefault(c => c.Code == tmplCode);
            if (info == null)
                return Json("数据源不存在，TmplCode：" + tmplCode);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(info.ConnName);
            string sql = replaceSql(info.SQL);
            var data = sqlHelper.ExecuteGridData(sql, qb);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetData(string tmplCode)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var info = entities.Set<S_UI_DataSource>().SingleOrDefault(c => c.Code == tmplCode);
            if (info == null)
                return Json("数据源不存在，TmplCode：" + tmplCode);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(info.ConnName);
            string sql = replaceSql(info.SQL);
            var data = sqlHelper.ExecuteDataTable(sql);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private string replaceSql(string sql)
        {
            var user = FormulaHelper.GetUserInfo();
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            string result = reg.Replace(sql, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');

                if (!string.IsNullOrEmpty(Request[value]))
                    return Request[value];

                switch (value)
                {
                    case Formula.Constant.CurrentUserID:
                        return user.UserID;
                    case Formula.Constant.CurrentUserName:
                        return user.UserName;
                    case Formula.Constant.CurrentUserOrgID:
                        return user.UserOrgID;
                    case Formula.Constant.CurrentUserOrgCode:
                        return user.UserOrgCode;
                    case Formula.Constant.CurrentUserOrgName:
                        return user.UserOrgName;
                    case Formula.Constant.CurrentUserPrjID:
                        return user.UserPrjID;
                    case Formula.Constant.CurrentUserPrjName:
                        return user.UserPrjName;
                    case "CurrentTime":
                        return DateTime.Now.ToString();
                    case "CurrentDate":
                        return DateTime.Now.Date.ToString("yyyy-MM-dd");
                    default:
                        return m.Value;
                }
            });

            return result;
        }

    }
}
