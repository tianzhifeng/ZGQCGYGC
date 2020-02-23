using System.Web.Mvc;

namespace EPC.Areas.Bidding
{
    public class BiddingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Bidding";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Bidding_default",
                "Bidding/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
