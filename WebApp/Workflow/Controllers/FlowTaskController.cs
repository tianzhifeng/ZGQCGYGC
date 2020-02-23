using Formula;
using System.Web;
using System.Web.Mvc;

namespace Workflow.Web.Controllers
{
    public class FlowTaskController : Controller
    {

        public JsonResult GetAuthCookie(string userCode)
        {
            if (string.IsNullOrEmpty(Request["userCode"]))
                return Json("no", JsonRequestBehavior.AllowGet);

            string userAccount = HttpUtility.UrlDecode(Request["userCode"]);

            FormulaHelper.SetAuthCookie(userAccount);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

    }
}
