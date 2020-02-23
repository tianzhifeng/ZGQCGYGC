using System.Web.Mvc;

namespace OfficeAuto.Areas.Lawsuit
{
    public class LawsuitAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Lawsuit";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Lawsuit_default",
                "Lawsuit/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
