using System.Web.Mvc;

namespace EPC.Areas.Cooperation
{
    public class CooperationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Cooperation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Cooperation_default",
                "Cooperation/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
