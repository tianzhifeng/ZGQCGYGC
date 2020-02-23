using System.Web.Mvc;

namespace Project.Areas.Gantt
{
    public class GanttAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Gantt";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Gantt_default",
                "Gantt/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
