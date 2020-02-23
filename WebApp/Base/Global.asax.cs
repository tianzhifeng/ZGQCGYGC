using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace Base
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Formula.FormulaHelper.RegistEntities<Base.Logic.Domain.BaseEntities>(Config.ConnEnum.Base);

            GlobalConfiguration.Configuration.MessageHandlers.Add(new CorsMessageHandler()); //解决http请求跨域问题
        }
    }

    public class CorsMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //得到描述目标Action的HttpActionDescriptor
            if (request.Method == HttpMethod.Options)
            {
                //string method = request.Headers.GetValues("Access-Control-Request-Method").First();
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    //Content = new StringContent("ok")
                };

                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);   // Also sets the task state to "RanToCompletion" 
                // 也将此任务设置成“RanToCompletion（已完成）”
                return tsc.Task;

            }
            else
                return base.SendAsync(request, cancellationToken);

        }
    }
}