using System.Web.Mvc;

namespace HR.Areas.Qualification
{
    public class QualificationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Qualification";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Qualification_default",
                "Qualification/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
