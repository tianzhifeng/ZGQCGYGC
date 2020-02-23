using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Monitor.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configuration.MessageHandlers.Add(new CorsMessageHandler()); //处理跨域
        }
    }

    /// <summary>
    /// 跨域请求
    /// </summary>
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