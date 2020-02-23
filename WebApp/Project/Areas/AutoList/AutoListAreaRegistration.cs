using System.Web.Mvc;

namespace Project.Areas.AutoList
{
    public class AutoListAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AutoList";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AutoList_default",
                "AutoList/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
