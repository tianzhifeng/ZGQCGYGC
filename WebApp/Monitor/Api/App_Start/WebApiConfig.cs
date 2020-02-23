using System.Web.Http;

namespace Monitor.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //移除默认xml显示
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}/{action}",
                defaults: new { id = RouteParameter.Optional, action=RouteParameter.Optional }
            );
        }
    }
}
