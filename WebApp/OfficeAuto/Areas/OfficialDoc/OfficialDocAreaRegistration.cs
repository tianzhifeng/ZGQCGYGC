using System.Web.Mvc;

namespace OfficeAuto.Areas.OfficialDoc
{
    public class OfficialDocAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OfficialDoc";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OfficialDoc_default",
                "OfficialDoc/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
