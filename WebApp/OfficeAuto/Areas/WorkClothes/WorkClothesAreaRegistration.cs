using System.Web.Mvc;

namespace OfficeAuto.Areas.WorkClothes
{
    public class WorkClothesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WorkClothes";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WorkClothes_default",
                "WorkClothes/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
