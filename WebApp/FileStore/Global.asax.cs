using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using FileStore.Logic.BusinessFacade;

namespace FileStore
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            if (FileStoreConfig.IsMaster)
                new MasterServiceFO().CheckLocalServerRootFolders();
            else
                new OuterServiceFO().CheckLocalServerRootFolders();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //过滤IP白名单，localhost 和允许IP 才能够访问文件服务，如允许列表为空，则允许所有IP访问
            //等保要求 文件服务不允许随意访问，以提高系统的安全性   by Eric.Yang 2019-5-23
            var allowList = System.Configuration.ConfigurationManager.AppSettings["FileStoreIPAllowList"];
            if (String.IsNullOrEmpty(allowList))
            {
                return;
            }
            string clientHost = this.Context.Request.UserHostName;
            if (!String.IsNullOrEmpty(clientHost) && (clientHost.ToLower().IndexOf("localhost") >= 0
                || clientHost.ToLower().IndexOf("127.0.0.1") >= 0 || clientHost.ToLower().IndexOf("::1") >= 0))
            {
                return;
            }
            foreach (var item in allowList.Split(','))
            {
                if (!String.IsNullOrEmpty(clientHost) && clientHost.ToLower().IndexOf(item.ToLower()) >= 0)
                {
                    return;
                }
                //新增网段匹配
                if (item.Split('.').Contains("*"))
                {
                    var matchHost = item.Split('.');
                    var hosts = clientHost.Split('.');
                    var length = matchHost.Length < hosts.Length ? matchHost.Length : hosts.Length;
                    var result = true;
                    for (int i = 0; i < length; i++)
                    {
                        if (matchHost[i] == "*")
                            continue;
                        if (hosts[i] != matchHost[i])
                        {
                            result = false;
                        }
                    }
                    if (result)
                    {
                        return;
                    }
                }
            }

            Response.Write(String.Format("Access Forbidden, HostName :{0}  is not in allow list ", clientHost));
            Response.End();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}