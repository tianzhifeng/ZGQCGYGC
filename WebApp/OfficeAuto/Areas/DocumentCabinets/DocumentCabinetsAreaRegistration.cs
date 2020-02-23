using System.Web.Mvc;

namespace OfficeAuto.Areas.DocumentCabinets
{
    public class DocumentCabinetsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DocumentCabinets";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DocumentCabinets_default",
                "DocumentCabinets/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
