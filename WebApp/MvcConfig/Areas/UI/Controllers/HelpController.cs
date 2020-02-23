using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;

namespace MvcConfig.Areas.UI.Controllers
{
    public class HelpController : BaseController
    {
        public ActionResult PageView()
        {
            var url = Request["Url"];
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select * from S_UI_Help where '{0}' like Url+'%'", url);
            var dt = sqlHelper.ExecuteDataTable(sql);
            string html = "";
            if (dt.Rows.Count == 1)
            {
                html = dt.Rows[0]["Layout"].ToString();
                if (dt.Rows[0]["HelpPageType"].ToString() == "url")
                {
                    Response.Redirect(dt.Rows[0]["HelpUrl"].ToString());
                    Response.End();
                }
                else if (dt.Rows[0]["HelpPageType"].ToString() == "file")
                {
                    var fileUrl = System.Configuration.ConfigurationManager.AppSettings["FS_MasterServerUrl"];
                    fileUrl = fileUrl.Split(new string[] { "Services" }, StringSplitOptions.None).FirstOrDefault();
                    fileUrl += "Download.aspx?FileID=" + dt.Rows[0]["HelpFile"].ToString();
                    Response.Redirect(fileUrl);
                    Response.End();
                }
            }
            ViewBag.HelpHtml = html;

            return View();
        }

        public JsonResult HasHelp(string url)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select * from S_UI_Help where '{0}' like Url+'%'", url);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false" });
            }
        }
    }
}
