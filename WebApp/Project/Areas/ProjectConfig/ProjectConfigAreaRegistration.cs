using System.Web.Mvc;

namespace Project.Areas.ProjectConfig
{
    public class ProjectConfigAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ProjectConfig";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProjectConfig_default",
                "ProjectConfig/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
