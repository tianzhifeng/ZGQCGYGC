using System.Web.Mvc;

namespace HR.Areas.YellowPage
{
    public class YellowPageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "YellowPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "YellowPage_default",
                "YellowPage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
