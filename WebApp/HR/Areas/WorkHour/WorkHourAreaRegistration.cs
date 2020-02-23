using System.Web.Mvc;

namespace HR.Areas.WorkHour
{
    public class WorkHourAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WorkHour";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WorkHour_default",
                "WorkHour/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
