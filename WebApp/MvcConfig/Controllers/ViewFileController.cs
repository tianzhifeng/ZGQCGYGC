using Config.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcConfig.Controllers
{
    public class ViewFileController : BaseController
    {
        private static string CacheViewFilePath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["CacheViewFilePath"];
            }
        }
        private static string ViewMode
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ViewMode"];
            }
        }
        public ActionResult ViewerSWF(string FileID)
        {
            ViewBag.IsExist = false;
            
            if (!System.IO.File.Exists(GetFileFullName(FixFileID(FileID))))
            {
                var sqlDb = Config.SQLHelper.CreateSqlHelper(Config.ConnEnum.FileStore);
                sqlDb.ExecuteNonQuery(string.Format("Update FsFile Set ConvertResult=null Where ID='{0}'", FixFileID(FileID)));
                ViewBag.IsExist = false;

                ImageDirect(FileID);
            }
            else
            {
                var filePath = "/MvcConfig/ViewFile/GetFile?FileID=" + FixFileID(FileID);
                ViewBag.FilePath = filePath;
                ViewBag.IsExist = true;
            }
            return View();
        }

        public ActionResult ViewerPDF(string FileID)
        {
            ViewBag.IsExist = false;
            if (!System.IO.File.Exists(GetFileFullName(FixFileID(FileID))))
            {
                var sqlDb = Config.SQLHelper.CreateSqlHelper(Config.ConnEnum.FileStore);
                sqlDb.ExecuteNonQuery(string.Format("Update FsFile Set ConvertResult=null Where ID='{0}'", FixFileID(FileID)));
                ViewBag.IsExist = false;
                ViewBag.IsImg = false;
                ViewBag.FileID = FileID;
                string fileName = FileStoreHelper.GetFileFullPath(FileID);
                if (fileName.LastIndexOf('.') > -1)
                {
                    var suffix = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                    if (suffix == "jpeg" || suffix == "bmp"  || suffix == "gif" || suffix == "png" || suffix == "jpg")
                    {
                        ViewBag.IsImg = true;
                    }
                    //ImageDirect(FileID);
                }
            }
            else
            {
                var filePath = "/MvcConfig/ViewFile/GetFile?FileID=" + FixFileID(FileID);
                ViewBag.FilePath = filePath;
                ViewBag.IsExist = true;
            }
            return View();
        }

        #region 获取文件
        public FileResult GetSnap(string fileID)
        {
            string fileFullName = GetFileFullName(fileID, true);
            byte[] snapFile = GetViewFile(fileFullName);

            if (snapFile != null)
                return File(snapFile, "image/png");
            else
            {
                var sqlDb = Config.SQLHelper.CreateSqlHelper(Config.ConnEnum.FileStore);
                sqlDb.ExecuteNonQuery(string.Format("Update FsFile Set ConvertResult=null Where ID='{0}'", FixFileID(fileID)));
                throw new Formula.Exceptions.WebException("服务器上找不到缩略图文件！");
            }
        }

        public FileResult GetFile(string fileID)
        {
            string fileFullName = GetFileFullName(fileID);
            byte[] swfFile = GetViewFile(fileFullName);

            if(swfFile != null)
                return File(swfFile, "application/x-shockwave-flash/" + Path.GetExtension(fileFullName));
            else
                throw new Formula.Exceptions.WebException("服务器上找不到浏览文件！");
        }
        [HttpGet]
        public FileResult GetImgFile(string fileID)
        {
            //string fileFullName = GetFileFullName(fileID);
            //byte[] swfFile = GetViewFile(fileFullName);
            fileID = fileID.ToLower();
            //如果附件为图片格式
            if (fileID.Contains(".jpeg") ||fileID.Contains(".bmp") || fileID.Contains(".jpg") || fileID.Contains(".png") || fileID.Contains(".gif"))
            {                
                string fileName = FileStoreHelper.GetFileFullPath(fileID);
                if (fileName.LastIndexOf('.') > -1)
                {
                    var suffix = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();

                    byte[]  fileBytes = FileStoreHelper.GetFile(fileID);
                    if (fileBytes == null)
                    {
                        throw new Formula.Exceptions.WebException("服务器上找不到浏览文件！");
                    }
                    if (fileBytes != null)
                    {

                        switch (suffix)
                        {
                            case "jpg":
                                return File(fileBytes, "application/x-jpg/" + Path.GetExtension(fileName));

                            case "png":
                                return File(fileBytes, "application/x-png/" + Path.GetExtension(fileName));
                            case "gif":
                                return File(fileBytes, "image/gif/" + Path.GetExtension(fileName));
                            case "jpeg":
                                return File(fileBytes, "image/jpeg/" + Path.GetExtension(fileName));
                            case "bmp":
                                return File(fileBytes, "application/x-bmp/" + Path.GetExtension(fileName));
                             default:
                                throw new Formula.Exceptions.WebException("文件格式不支持！");
                        }
                    }
                }
            }
            throw new Formula.Exceptions.WebException("文件格式不正确！");

        }
        #endregion

        #region 助手方法
        private string FixFileID(string fileID)
        {
            if (fileID.IndexOf('_') >= 0)
                return fileID.Split('_')[0];
            else
                return fileID;
        }

        private string GetFileFullName(string fileID, bool isSnap = false)
        {
            int num = Convert.ToInt32(fileID) / 1000 + 1;
            string filePath = Path.Combine(CacheViewFilePath, string.Format("{0}", num.ToString("D8")));

            string suffix = "swf";
            if (isSnap)
            {

                if (ViewMode.ToLower() == EnumViewMode.TilePicViewer.ToString().ToLower())
                {
                    suffix = "jpg";
                    filePath = Path.Combine(CacheViewFilePath,"Dwg/", string.Format("{0}", num.ToString("D8")));
                }
                else
                    suffix = "png";

            }
            else if (ViewMode.ToLower() == EnumViewMode.PDFViewer.ToString().ToLower())
                suffix = "pdf";

            return System.IO.Path.Combine(filePath, fileID + "." + suffix);
        }

        /// <summary>
        /// 根据文件ID获取浏览文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private byte[] GetViewFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return null;
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            long fileSize = fileStream.Length;
            byte[] fileBuffer = new byte[fileSize];
            fileStream.Read(fileBuffer, 0, (int)fileSize);
            fileStream.Close();

            return fileBuffer;
        }

        /// <summary>
        /// 如果是可直接浏览文件格式，直接跳转
        /// </summary>
        /// <param name="fileID"></param>
        private void ImageDirect(string fileID)
        {
            fileID = fileID.ToLower();
            //如果附件为图片格式
            if (fileID.Contains(".jpg") || fileID.Contains(".png") || fileID.Contains(".gif"))
            {
                object fileBytes = null;
                string fileName = FileStoreHelper.GetFileFullPath(fileID);
                if (fileName.LastIndexOf('.') > -1)
                {
                    var suffix = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();

                    fileBytes = FileStoreHelper.GetFile(fileID);
                    if (fileBytes != null && fileBytes != DBNull.Value)
                    {
                        this.Response.Clear();

                        if (suffix == "jpg" || suffix == "png" || suffix == "gif")
                        {
                            this.Response.ContentType = "image/gif";
                            this.Response.OutputStream.Write(fileBytes as byte[], 0, ((byte[])fileBytes).Length);
                            this.Response.End();
                        }
                    }
                }
            }
        }
        #endregion

    }

    public enum ResultStatus
    {
        Process,
        Success,
        Error
    }

    public enum EnumViewMode
    {
        SWFViewer,
        PDFViewer,
        TilePicViewer
    }
}
