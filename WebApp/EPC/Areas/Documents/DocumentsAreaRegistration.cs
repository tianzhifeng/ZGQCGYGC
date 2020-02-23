using System.Web.Mvc;

namespace EPC.Areas.Documents
{
    public class DocumentsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Documents";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Documents_default",
                "Documents/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
