﻿using System.Web.Mvc;

namespace Base.Areas.Calendar
{
    public class CalendarAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Calendar";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Calendar_default",
                "Calendar/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
