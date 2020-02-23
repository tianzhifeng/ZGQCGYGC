using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;
using System.Web.Security;
using Config;

namespace MvcConfig.Areas.Workflow.Controllers
{
    public class PasswordController : Controller
    {
        public ActionResult Validation()
        {
            return View();
        }

        public JsonResult ValidatePwd(string pwd)
        {
            string userID = FormulaHelper.UserID;
            string sql = string.Format("select count(1) from S_A_User where ID='{0}' and SignPwd='{1}'", userID,
                FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", userID, pwd), "SHA1"));
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var obj = sqlHelper.ExecuteScalar(sql);
            return Json(obj.ToString() == "1");           
        }
    }
}
