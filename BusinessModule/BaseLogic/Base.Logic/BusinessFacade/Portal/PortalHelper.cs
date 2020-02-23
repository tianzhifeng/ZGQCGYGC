using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Base.Logic.BusinessFacade.Portal
{
    public static class PortalHelper
    {
        public static string GetPageHtml(string pageFullPath)
        {
            string _pageHtml = "";
            string filePath = HttpContext.Current.Server.MapPath(pageFullPath);
            if (File.Exists(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fs);
                _pageHtml = reader.ReadToEnd();
                reader.Close();
                fs.Close();
                return _pageHtml;
            }
            else { return ""; }
        }

        public static string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;

        }

        public static string SplitCondition(string str)
        {
            List<string> IDs = new List<string>();
            foreach (var item in str.Split(','))
            {
                IDs.Add("'" + item + "'");
            }
            return string.Join(",", IDs.ToArray());
        }

        public static string GetHTMLCode(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            //使用Cookie设置AllowAutoRedirect属性为false,是解决“尝试自动重定向的次数太多。”的核心
            request.CookieContainer = new CookieContainer();
            request.AllowAutoRedirect = false;
            WebResponse response = (WebResponse)request.GetResponse();
            Stream sm = response.GetResponseStream();
            System.IO.StreamReader streamReader = new System.IO.StreamReader(sm);
            //将流转换为字符串
            string html = streamReader.ReadToEnd();
            streamReader.Close();
            return html;
        }

        public static string SplitOnClick(string html, string templateID, string id)
        {
            if (!string.IsNullOrEmpty(html))
            {
                string firstHtml = html.Substring(0, html.IndexOf('>')) + string.Format(" TID=\"{1}\" {0}", !string.IsNullOrEmpty(id) ? string.Format("onclick=\"ClickPortal(\'{0}\')\"", id) : "", templateID);
                string lastHtml = html.Substring(html.IndexOf('>'), html.Length - html.IndexOf('>'));
                return firstHtml + lastHtml;
            }
            else { return ""; }
        }
    }
}
