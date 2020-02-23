using System.Web.Mvc;

namespace EPC.Areas.Analysis
{
    public class AnalysisAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Analysis";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Analysis_default",
                "Analysis/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
