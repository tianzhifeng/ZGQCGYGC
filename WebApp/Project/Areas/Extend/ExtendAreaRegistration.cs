using System.Web.Mvc;

namespace Project.Areas.Extend
{
    public class ExtendAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Extend";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Extend_default",
                "Extend/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
