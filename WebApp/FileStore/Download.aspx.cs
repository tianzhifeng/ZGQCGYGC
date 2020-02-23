using System;
using System.Linq;
using System.Web;
using Config;
using System.Configuration;

namespace FileStore
{
    public partial class Download : System.Web.UI.Page
    {
        DownloadServer downloadServer = new DownloadServer();

        protected void Page_Load(object sender, EventArgs e)
        {
            string fileIds = GetFileIds();

            fileIds = Server.UrlDecode(fileIds);

            if (string.IsNullOrEmpty(fileIds))
            {
                Response.Write("需要参数FileId");
                return;
            }

            string _flag = Request["jump"] ?? "0";
            if (_flag.Equals("0"))
            {
                #region 判断是否跳转,防止三次跳转
                string url = "";
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("FileStore");
                System.Data.DataTable dt = null;
                //1.取对应服务器
                if (ConfigurationManager.AppSettings["FS_Distributed"] != "True")
                {
                    //1.1 非分布式文件系统，需要跳转到文件所在的服务器下载文件
                    int fID = int.Parse(fileIds.Split(',')[0].Split('_')[0]);
                    string sql = "select a.HttpUrl,a.HttpUrlInner from FsFile with(nolock) left join FsServer a with(nolock) on UploadServerName=a.ServerName where FsFile.ID=" + fID.ToString();
                    dt = sqlHelper.ExecuteDataTable(sql);
                }
                else
                {
                    //1.2 分布式的文件系统，需要判断跳转到个人使用的文件服务器
                    if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
                    {
                        string sql = string.Format("select HttpUrl,HttpUrlInner from FsServer join UserFileServer on UserName='{0}' and FsServer.ServerName= UserFileServer.ServerName", HttpContext.Current.User.Identity.Name);
                        dt = sqlHelper.ExecuteDataTable(sql);
                        if (dt == null || dt.Rows.Count == 0)
                        {
                            sql = string.Format("select HttpUrl,HttpUrlInner from FsServer where ServerName='{0}'", System.Configuration.ConfigurationManager.AppSettings["FS_ServerName"]);
                            dt = sqlHelper.ExecuteDataTable(sql);
                        }
                    }
                }
                //2.判断来自内外网
                if (dt != null && dt.Rows.Count > 0)
                {
                    //默认值
                    url = dt.Rows[0]["HttpUrl"].ToString().Trim();

                    //2018-4 解决内网中无法使用外网地址的问题，方法是使用配置文件中OutWebHostIP的值进行指定
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OutWebHostIP"]))
                    {
                        var OutWebHostIP = ConfigurationManager.AppSettings["OutWebHostIP"].ToString().Trim();
                        if (Request.Url.Host.Equals(OutWebHostIP, StringComparison.CurrentCultureIgnoreCase))
                            url = dt.Rows[0]["HttpUrlInner"].ToString().Trim();
                    }
                }
                //3.异常处理
                if (string.IsNullOrEmpty(url))
                {
                    Response.StatusCode = 500;
                    Response.Write("没有身份凭证或配置有误！");
                    Response.End();
                    return;
                }
                //4.判断不一样就跳转
                Uri u = new Uri(url);
                if (!Request.Url.Host.Equals(u.Host, StringComparison.CurrentCultureIgnoreCase) || !Request.Url.Port.Equals(u.Port))
                {
                    url = url.ToLower();
                    url = url.Remove(url.IndexOf("/services"));
                    url += "/download.aspx";
                    url += Request.Url.Query;
                    url += "&jump=1";//防止分布式部署情况下的跳转异常

                    Response.Redirect(url);
                    return;
                }
                #endregion
            }

            if (!string.IsNullOrEmpty(Request["format"]))
            {
                downloadServer.ExportViewFile(fileIds, Request["format"].ToLower());
            }
            else
            {
                downloadServer.ValidateFileSize(fileIds);

                string realFileName = downloadServer.GetResultFileName(fileIds);
                realFileName.Replace(" ", "%20");

                //long fileSize = bytes.Length;
                Response.ContentType = "application/octet-stream ; Charset=UTF8";
                Response.CacheControl = "public";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"" + realFileName + "\"");
                //Response.AddHeader("Content-Length", fileSize.ToString());
                Response.AddHeader("Content-Transfer-Encoding", "binary");

                downloadServer.ExportFile(fileIds);

                Response.End();
            }

            HttpContext.Current.ApplicationInstance.CompleteRequest();

        }


        private string GetFileIds()
        {
            if (!string.IsNullOrEmpty(Request["FileID"]))
            {
                if (System.Configuration.ConfigurationManager.AppSettings["FS_FreeDownload"] == "True")
                {
                    return Request["FileID"];
                }
                else
                {
                    Response.Write("不能直接通过输入地址下载文件！");
                    Response.End();
                    return null;
                }

            }
            else if (!string.IsNullOrEmpty(Request["auth"]))
            {
                string fileId = Request["auth"];
                fileId = System.Text.Encoding.Default.GetString(Convert.FromBase64String(fileId.Replace("%2B", "+")));

                DateTime t = DateTime.Parse(fileId.Split('_').LastOrDefault());
                if (System.Configuration.ConfigurationManager.AppSettings["FS_FreeDownload"] != "True" && DateTime.Now > t)
                {
                    Response.Write("下载权限已超时，请重新获取下载权限！");
                    Response.End();
                    return null;
                }
                //return fileId.Split('_').FirstOrDefault();
                return fileId.Remove(fileId.LastIndexOf('_'));
            }
            else
            {
                string id = Request["id"];
                //id = id.Split('_').FirstOrDefault();
                id += DateTime.Now.AddMinutes(1).ToString("_yyyy-MM-dd HH:mm:ss");
                string fileID = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(id)).Replace("+", "%2B");
                Response.Write(fileID);
                Response.End();
                return null;
            }
        }
    }
}