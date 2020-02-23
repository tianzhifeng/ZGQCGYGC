﻿using System.Web.Mvc;

namespace EPC.Areas.Design
{
    public class DesignAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Design";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Design_default",
                "Design/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
