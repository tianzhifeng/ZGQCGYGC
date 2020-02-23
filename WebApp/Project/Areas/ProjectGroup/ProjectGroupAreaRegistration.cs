using System.Web.Mvc;

namespace Project.Areas.ProjectGroup
{
    public class ProjectGroupAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ProjectGroup";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProjectGroup_default",
                "ProjectGroup/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
