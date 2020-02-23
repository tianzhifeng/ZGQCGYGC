using System.Web.Mvc;

namespace Comprehensive.Areas.OfficeAuto
{
    public class OfficeAutoAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OfficeAuto";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OfficeAuto_default",
                "OfficeAuto/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
