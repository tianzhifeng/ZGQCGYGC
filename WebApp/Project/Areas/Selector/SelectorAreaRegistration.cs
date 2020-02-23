using System.Web.Mvc;

namespace Project.Areas.Selector
{
    public class SelectorAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Selector";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Selector_default",
                "Selector/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
