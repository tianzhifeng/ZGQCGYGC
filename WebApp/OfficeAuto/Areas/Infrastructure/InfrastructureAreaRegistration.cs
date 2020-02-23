using System.Web.Mvc;

namespace OfficeAuto.Areas.Infrastructure
{
    public class InfrastructureAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Infrastructure";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Infrastructure_default",
                "Infrastructure/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
