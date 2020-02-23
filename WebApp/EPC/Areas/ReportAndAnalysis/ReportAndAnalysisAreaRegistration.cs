using System.Web.Mvc;

namespace EPC.Areas.ReportAndAnalysis
{
    public class ReportAndAnalysisAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ReportAndAnalysis";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ReportAndAnalysis_default",
                "ReportAndAnalysis/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
