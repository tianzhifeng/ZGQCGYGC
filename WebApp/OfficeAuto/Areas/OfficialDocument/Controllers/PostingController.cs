using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Controllers;
using OfficeAuto.Logic.Domain;
using OfficeAuto.Logic;
using Config;
using Workflow.Logic.Domain;
using Formula;
using Workflow.Logic;
using System.IO;
using Config.Logic;

namespace OfficeAuto.Areas.OfficialDocument.Controllers
{
    public class PostingController : OfficeAutoFormContorllor<S_D_Posting>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            string hasRedFile = "false", hasDoc = "false";
            if (isNew)
            {
                ViewBag.RecordID = "";
                ViewBag.WebDocPath = "";
            }
            else
            {
                var id = dt.Rows[0]["ID"].ToString();
                ViewBag.RecordID = id;
                if (dt.Rows[0]["RedFile"] != DBNull.Value)
                    hasRedFile = "true";
                if (dt.Rows[0]["MainFile"] != DBNull.Value || dt.Rows[0]["RedFile"] != DBNull.Value)
                    hasDoc = "true";
            }
            ViewBag.HasRedFile = hasRedFile;
            ViewBag.HasDoc = hasDoc;
        }

        public JsonResult SaveFile(string DataID, string HasRedFile)
        {
            var post = this.BusinessEntities.Set<S_D_Posting>().FirstOrDefault(a => a.ID == DataID);
            if (post == null)
            {
                throw new Formula.Exceptions.BusinessException("未找到关联的发文记录，请重试或联系管理员！");
            }
            else
            {
                if (Request.Files.Count > 0)
                {
                    var t = Request.Files[0].InputStream;
                    byte[] bt = new byte[t.Length];
                    t.Read(bt, 0, int.Parse(t.Length.ToString()));
                    if (HasRedFile.ToLower() == "false")
                        post.MainFile = bt;
                    else
                        post.RedFile = bt;
                }
            }
            this.BusinessEntities.SaveChanges();
            return Json("");
        }

        public FileResult GetFile(string DataID, string HasRedFile)
        {
            var posting = this.BusinessEntities.Set<S_D_Posting>().FirstOrDefault(a => a.ID == DataID);
            byte[] file = posting.MainFile;
            if (HasRedFile.ToLower() == "true")
                file = posting.RedFile;
            return File(file, "application/msword");
        }

        public FileResult GetTemplate()
        {
            var path = Server.MapPath("/OfficialDocTemplate/EmptyTemplate.doc");
            if (!System.IO.File.Exists(path))
                throw new Formula.Exceptions.BusinessException("未找到发文模版，请重试或联系管理员！");
            FileInfo fi = new FileInfo(path);
            byte[] buff = new byte[fi.Length];
            FileStream fs = fi.OpenRead();
            fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return File(buff, "application/msword");
        }

        public FileResult GetRedFile(string fileID)
        {
            byte[] file = FileStoreHelper.GetFile(fileID);
            return File(file, "application/msword");
        }

        public FileResult GetStamp()
        {
            var path = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["SignatureImage"]);
            if (!System.IO.File.Exists(path))
                throw new Formula.Exceptions.BusinessException("未找到公章图片，请重试或联系管理员！");
            FileInfo fi = new FileInfo(path);
            byte[] buff = new byte[fi.Length];
            FileStream fs = fi.OpenRead();
            fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return File(buff, "image/png");
        }
    }
}
