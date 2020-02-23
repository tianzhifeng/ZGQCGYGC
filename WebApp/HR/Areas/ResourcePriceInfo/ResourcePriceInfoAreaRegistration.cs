using System.Web.Mvc;

namespace HR.Areas.ResourcePriceInfo
{
    public class ResourcePriceInfoAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ResourcePriceInfo";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ResourcePriceInfo_default",
                "ResourcePriceInfo/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
