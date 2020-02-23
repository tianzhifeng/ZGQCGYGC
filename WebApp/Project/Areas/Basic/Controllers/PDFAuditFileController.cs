using Formula.Helper;
using Project.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using iTextSharp.text.pdf;

namespace Project.Areas.Basic.Controllers
{
    public class PDFAuditFileController : ProjectController<S_E_Product>
    {
        public ActionResult PDFAuditFileList()
        {
            return View();
        }

        public ActionResult PDFAuditFileUpload()
        {
            return View();
        }

        public JsonResult GetPDFAuditFileList()
        {
            var productID = GetQueryString("ID");
            var PDFAuditFiles = new List<Dictionary<string, object>>();
            var product = this.GetEntityByID<S_E_Product>(productID);
            if (product != null)
                PDFAuditFiles = JsonHelper.ToList(product.PDFAuditFiles);
            else
            {
                var version = this.GetEntityByID<S_E_ProductVersion>(productID);
                if (version != null)
                    PDFAuditFiles = JsonHelper.ToList(version.PDFAuditFiles);
                else
                    throw new Formula.Exceptions.BusinessException("未能找到指定的成果。");
            }
            return Json(PDFAuditFiles);
        }

        public JsonResult SaveAuditFile(string ProductID, string AuditStep, string FileID, bool IsOverride)
        {
            var product = this.GetEntityByID<S_E_Product>(ProductID);
            if (product == null) throw new Formula.Exceptions.BusinessException("未能找到指定的成果。");
            var PDFAuditFiles = JsonHelper.ToList(product.PDFAuditFiles);
            if (IsOverride)
                PDFAuditFiles.RemoveWhere(a => a.GetValue("AuditStep") == AuditStep && a.GetValue("SubmitUser") == CurrentUserInfo.UserID);

            var dic = new Dictionary<string, object>();
            dic.SetValue("AuditStep", AuditStep);
            dic.SetValue("SubmitUser", CurrentUserInfo.UserID);
            dic.SetValue("SubmitUserName", CurrentUserInfo.UserName);
            dic.SetValue("SubmitDate", System.DateTime.Now.ToString("s"));
            dic.SetValue("Attachment", FileID);
            PDFAuditFiles.Add(dic);

            product.PDFAuditFiles = JsonHelper.ToJson(PDFAuditFiles);
            product.UpdateVersison();
            this.entities.SaveChanges();

            #region 获取PDF中的校审信息
            byte[] bytes = FileStoreHelper.GetFile(FileID);
            var list = GetAnnots(bytes);
            #endregion
            var reslut = new Dictionary<string, object>();
            reslut.Add("list", list);
            reslut.Add("code", product.Code);
            return Json(reslut);
        }

        public JsonResult MultiSaveAuditFile(string AuditID, string ChangeAuditID,string AuditStep, string FileIDs, bool IsOverride)
        {
            var productList = new List<S_E_Product>();
            if (!string.IsNullOrEmpty(AuditID))
                productList = this.entities.Set<S_E_Product>().Where(a => a.AuditID == AuditID).ToList();
            else if (!string.IsNullOrEmpty(ChangeAuditID))
                productList = this.entities.Set<S_E_Product>().Where(a => a.ChangeAuditID == ChangeAuditID).ToList();
            List<Dictionary<string, object>> errorFiles = new List<Dictionary<string, object>>();//上传的文件解析错误，找到多个成果，未找到对应的成果
            List<Dictionary<string, object>> successFiles = new List<Dictionary<string, object>>();
            foreach (var FileID in FileIDs.Split(','))
            {
                if (FileID.IndexOf("_") < 0 || FileID.ToLower().IndexOf(".pdf") < 0)
                {
                    errorFiles.Add(new Dictionary<string, object> { { "name", FileID }, { "message", "文件名解析错误" } });
                    continue;
                }
                var validateProducts = productList.Where(a => !string.IsNullOrEmpty(a.MainFile) && a.MainFile.Split('_')[1] == FileID.Split('_')[1]).ToList();
                if (validateProducts.Count > 1)
                {
                    errorFiles.Add(new Dictionary<string, object> { { "name", FileID.Split('_')[1] }, { "message", "根据成果附件匹配到多个成果" } });
                    continue;
                }
                if (validateProducts.Count == 0)
                {
                    errorFiles.Add(new Dictionary<string, object> { { "name", FileID.Split('_')[1] }, { "message", "根据成果附件未找到对应的成果" } });
                    continue;
                }
                var product = validateProducts.FirstOrDefault();
                var PDFAuditFiles = JsonHelper.ToList(product.PDFAuditFiles);
                if (IsOverride)
                    PDFAuditFiles.RemoveWhere(a => a.GetValue("AuditStep") == AuditStep && a.GetValue("SubmitUser") == CurrentUserInfo.UserID);

                var dic = new Dictionary<string, object>();
                dic.SetValue("AuditStep", AuditStep);
                dic.SetValue("SubmitUser", CurrentUserInfo.UserID);
                dic.SetValue("SubmitUserName", CurrentUserInfo.UserName);
                dic.SetValue("SubmitDate", System.DateTime.Now.ToString("s"));
                dic.SetValue("Attachment", FileID);
                PDFAuditFiles.Add(dic);

                product.PDFAuditFiles = JsonHelper.ToJson(PDFAuditFiles);
                product.UpdateVersison();

                //提取pdf批注信息
                byte[] bytes = FileStoreHelper.GetFile(FileID);
                var list = GetAnnots(bytes);
                successFiles.Add(new Dictionary<string, object> { { "list", list }, { "code", product.Code } });
            }


            this.entities.SaveChanges();

            return Json(new { successFiles, errorFiles });
        }

        public JsonResult SaveAuditFiles(string ProductID, string Files)
        {
            var product = this.GetEntityByID<S_E_Product>(ProductID);
            if (product == null) throw new Formula.Exceptions.BusinessException("未能找到指定的成果。");
            product.PDFAuditFiles = Files;
            product.UpdateVersison();

            this.entities.SaveChanges();
            return Json("");
        }

        public List<PDFAnnot> GetAnnots(byte[] pdfIn)
        {
            var list = new List<PDFAnnot>();
            PdfReader reader = new PdfReader(pdfIn);
            for (int n = 1; n <= reader.NumberOfPages; n++)
            {
                PdfDictionary page = reader.GetPageN(n);
                PdfArray annotsArray = page.GetAsArray(PdfName.ANNOTS);
                if (annotsArray == null)
                    continue;
                for (int k = 0; k < annotsArray.Size; k++)
                {
                    PdfDictionary annot = (PdfDictionary)PdfReader.GetPdfObject(annotsArray.GetPdfObject(k));
                    //内容
                    PdfString content = (PdfString)PdfReader.GetPdfObject(annot.Get(PdfName.CONTENTS));
                    //创建日期
                    PdfString creationDate = (PdfString)PdfReader.GetPdfObject(annot.Get(PdfName.CREATIONDATE));
                    //作者
                    PdfString author = (PdfString)PdfReader.GetPdfObject(annot.Get(PdfName.T));

                    //去掉无用状态
                    if (content != null && creationDate != null && author != null)
                    {
                        list.Add(new PDFAnnot
                        {
                            Content = Tostr(content),
                            UserId = Tostr(author),
                            Date = ToDate(creationDate)
                        });
                    }
                }
            }
            return list;
        }

        private static string Tostr(PdfString pdfstr)
        {
            string strRet = null;
            if (pdfstr != null)
            {
                strRet = pdfstr.ToUnicodeString();
            }
            return strRet;
        }

        private static DateTime? ToDate(PdfString pdfstr)
        {
            string strRet = null;
            if (pdfstr != null)
            {
                strRet = pdfstr.ToUnicodeString();

                return PdfDate.Decode(strRet);
            }
            return null;
        }

        public class PDFAnnot
        {
            public string Content;
            public string UserId;
            public DateTime? Date;
        }
    }
}
