using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Filters
{
    public class SystemAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                Formula.AuthCodeHelper.CheckTokenRole();
            }
            catch (Exception)
            {
                HttpContext.Current.Response.Redirect("/MvcConfig/Error.html");
            }
            return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var account = filterContext.HttpContext.Items["AgentUserLoginName"];
            if (account != null)
            {
                filterContext.HttpContext.Response.Redirect(HttpContext.Current.Request.Url.ToString());
            }
            base.HandleUnauthorizedRequest(filterContext);
        }

    }
}