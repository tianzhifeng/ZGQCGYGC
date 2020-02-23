using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileStore.SWFUpload
{
    /// <summary>
    /// FileCheckHandler 的摘要说明
    /// </summary>
    public class FileCheckHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");

            string fileName = context.Request["filename"];

            if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["FS_FileExtFilter"]))
            {
                context.Response.Write("0");
            }
            else
            {
                var arr = System.Configuration.ConfigurationManager.AppSettings["FS_FileExtFilter"].Split('|');
                foreach (var item in arr)
                {
                    if (fileName.EndsWith("." + item))
                    {
                        context.Response.Write("1");
                        return;
                    }
                }
                context.Response.Write("0");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}