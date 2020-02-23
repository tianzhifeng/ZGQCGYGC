using System.Web.Mvc;

namespace OfficeAuto.Areas.OfficialDocument
{
    public class OfficialDocumentAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OfficialDocument";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OfficialDocument_default",
                "OfficialDocument/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
