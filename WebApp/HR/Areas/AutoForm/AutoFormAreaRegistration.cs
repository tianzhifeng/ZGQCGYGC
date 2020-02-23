using System.Web.Mvc;

namespace HR.Areas.AutoForm
{
    public class AutoFormAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AutoForm";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AutoForm_default",
                "AutoForm/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
