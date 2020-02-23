using System.Web.Mvc;

namespace HR.Areas.UserAptitude
{
    public class UserAptitudeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "UserAptitude";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "UserAptitude_default",
                "UserAptitude/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
