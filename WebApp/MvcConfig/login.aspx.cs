using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Config;

namespace MvcConfig
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string userCode = Request["UserCode"];
            string pwd = Request["pwd"];
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(userCode.Trim().ToLower() + pwd.Trim(), "SHA1");
            string sql = string.Format("select count(1) from S_A_User where Code='{0}' and Password='{1}'", userCode, pwd);
            var obj = sqlHelper.ExecuteScalar(sql);
            if (obj.ToString() == "0")
            {                
                Response.Write("登录失败！");
                Response.End();
            }
            else
            {
                FormsAuthentication.SetAuthCookie(userCode, false);
                Response.Write("登录成功！");
                Response.End();
            }
        }
    }
}