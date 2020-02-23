using System.Web.Mvc;

namespace Project.Areas.CadConfig
{
    public class CadConfigAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CadConfig";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CadConfig_default",
                "CadConfig/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
