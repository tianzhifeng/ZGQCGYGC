using System.Web.Mvc;

namespace Project.Areas.AutoUI
{
    public class AutoUIAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AutoUI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AutoUI_default",
                "AutoUI/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
