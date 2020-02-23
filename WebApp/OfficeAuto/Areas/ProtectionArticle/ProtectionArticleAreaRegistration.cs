using System.Web.Mvc;

namespace OfficeAuto.Areas.ProtectionArticle
{
    public class ProtectionArticleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ProtectionArticle";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProtectionArticle_default",
                "ProtectionArticle/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
