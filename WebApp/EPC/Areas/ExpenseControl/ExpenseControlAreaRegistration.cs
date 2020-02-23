using System.Web.Mvc;

namespace EPC.Areas.ExpenseControl
{
    public class ExpenseControlAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ExpenseControl";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ExpenseControl_default",
                "ExpenseControl/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
