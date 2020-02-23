using System.Web.Mvc;

namespace Project.Areas.Engineering
{
    public class EngineeringAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Engineering";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Engineering_default",
                "Engineering/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
