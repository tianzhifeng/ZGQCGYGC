using System.Web.Mvc;

namespace EPC.Areas.Construction
{
    public class ConstructionAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Construction";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Construction_default",
                "Construction/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
