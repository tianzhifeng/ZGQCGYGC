using System.Web.Mvc;

namespace HR.Areas.AttendanceInfo
{
    public class AttendanceInfoAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AttendanceInfo";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AttendanceInfo_default",
                "AttendanceInfo/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
