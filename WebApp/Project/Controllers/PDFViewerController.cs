using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.IO;
using Config.Logic;
using Project.Logic.Domain;
using Formula.Helper;

namespace Project.Controllers
{
    public class PDFViewerController : ProjectController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Viewer()
        {
            return View();
        }

        public JsonResult GetRelateInfo(string ProductID, string Version)
        {
            var result = new Dictionary<string, object>();
            var sql = "select top 1 * from S_E_ProductVersion with(nolock) where ProductID='" + ProductID + "' and Version='" + Version + "'";
            var productVersions = SqlHelper.ExecuteList<S_E_ProductVersion>(sql);
            if (productVersions.Count() == 0)
                throw new Formula.Exceptions.BusinessException("找不到成果ID为【" + ProductID + "】版本为【" + Version + "】的成果！");
            var pdfPositon = JsonHelper.ToObject(productVersions[0].PDFSignPositionInfo);
            result.SetValue("Position", pdfPositon);
            result.SetValue("TF", productVersions[0].FileSize);

            var groupInfo = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(productVersions[0].PlotSealGroup))
            {
                foreach (var plotSealGroup in productVersions[0].PlotSealGroup.Split(','))
                {
                    var group = _GetPlotSealGroupInfo(plotSealGroup);
                    groupInfo.SetValue(group.GetValue("BlockKey"), group);
                }
            }
            result.SetValue("GroupInfo", groupInfo);
            return Json(result);
        }

        public JsonResult GetTemplate(string TemplateID)
        {
            var sql = "select * from S_EP_PlotSealTemplate_GroupList where S_EP_PlotSealTemplateID='" + TemplateID + "'";
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            sql = "select * from S_EP_PlotSealTemplate_BorderConfig where S_EP_PlotSealTemplateID='" + TemplateID + "'";
            var bcDT = this.SqlHelper.ExecuteDataTable(sql);
            var result = new Dictionary<string, object>();
            var list = new List<Dictionary<string, object>>();
            var bc = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                if (!string.IsNullOrEmpty(row["GroupID"].ToString()))
                {
                    var group = _GetPlotSealGroupInfo(row["GroupID"].ToString());
                    group.SetValue("PositionXs", row["PositionXs"]);
                    group.SetValue("PositionYs", row["PositionYs"]);
                    group.SetValue("Category", "图章");
                    list.Add(group);
                }
                else
                    list.Add(Formula.FormulaHelper.DataRowToDic(row));
            }
            foreach (DataRow row in bcDT.Rows)
            {
                bc.Add(Formula.FormulaHelper.DataRowToDic(row));
            }
            result.SetValue("BorderConfig", bc);
            result.SetValue("GroupList", list);
            return Json(result);
        }

        public JsonResult GetPlotSealGroupInfo(string GroupID)
        {
            return Json(_GetPlotSealGroupInfo(GroupID));
        }

        public JsonResult GetPlotSealGroupInfos(string GroupIDs)
        {
            var result = new Dictionary<string, object>();
            var groupInfo = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(GroupIDs))
                foreach (var plotSealGroup in GroupIDs.Split(','))
                {
                    var group = _GetPlotSealGroupInfo(plotSealGroup);
                    groupInfo.SetValue(group.GetValue("BlockKey"), group);
                }
            result.SetValue("GroupInfo", groupInfo);
            return Json(result);
        }

        private Dictionary<string, object> _GetPlotSealGroupInfo(string groupID)
        {
            var sealGroup = this.entities.Set<S_EP_PlotSealGroup>().FirstOrDefault(a => a.ID == groupID);
            if (sealGroup == null)
                throw new Formula.Exceptions.BusinessException("ID为【" + groupID + "】的出图章组合不存在！");
            var seals = this.entities.Set<S_EP_PlotSealGroup_GroupInfo>().Where(a => a.S_EP_PlotSealGroupID == groupID).ToList();
            var mainSeal = seals.FirstOrDefault(a => a.IsMain == "true");
            if (mainSeal == null)
                throw new Formula.Exceptions.BusinessException("ID为【" + groupID + "】的出图章组合没有主章！");
            var followSeals = seals.Where(a => a.IsMain != "true");
            var group = new Dictionary<string, object>();
            group.SetValue("BlockKey", mainSeal.BlockKey);
            group.SetValue("GroupID", groupID);
            group.SetValue("GroupName", sealGroup.Name);
            group.SetValue("MainID", mainSeal.PlotSeal);
            group.SetValue("Name", mainSeal.Name);
            group.SetValue("Width", mainSeal.Width);
            group.SetValue("Height", mainSeal.Height);
            var follows = new List<Dictionary<string, object>>();
            foreach (var fSeal in followSeals)
            {
                var fS = new Dictionary<string, object>();
                fS.SetValue("FollowID", fSeal.PlotSeal);
                fS.SetValue("Name", fSeal.Name);
                fS.SetValue("Width", fSeal.Width);
                fS.SetValue("Height", fSeal.Height);
                fS.SetValue("CorrectPosX", fSeal.CorrectPosX);
                fS.SetValue("CorrectPosY", fSeal.CorrectPosY);
                follows.Add(fS);
            }
            group.SetValue("Follows", follows);

            return group;
        }

        public FileResult GetEmptyPDF()
        {
            var path = Server.MapPath("/Project/Views/PDFViewer/empty.pdf");
            if (!System.IO.File.Exists(path))
                throw new Formula.Exceptions.BusinessException("服务器上找不到浏览文件！");
            FileInfo fi = new FileInfo(path);
            byte[] buff = new byte[fi.Length];
            FileStream fs = fi.OpenRead();
            fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            return File(buff, "application/pdf");
        }

        public FileResult GetFile(string fileID)
        {
            byte[] file = FileStoreHelper.GetFile(fileID);

            if (file != null)
                return File(file, "application/pdf");
            else
                throw new Formula.Exceptions.WebException("服务器上找不到浏览文件！");
        }

        public JsonResult SavePosition(string ProductID, string Version,
            string PdfPositionInfo, string GroupIDs, string GroupNames, string GroupKeys)
        {
            var ver = Convert.ToDecimal(Version);
            var product = this.entities.Set<S_E_Product>().FirstOrDefault(a => a.ID == ProductID && a.Version == ver);
            if (product != null)
            {
                product.PDFSignPositionInfo = PdfPositionInfo;
                product.PlotSealGroup = GroupIDs;
                product.PlotSealGroupName = GroupNames;
                product.PlotSealGroupKey = GroupKeys;
            }
            var version = this.entities.Set<S_E_ProductVersion>().FirstOrDefault(a => a.ProductID == ProductID && a.Version == ver);
            if (version != null)
            {
                version.PDFSignPositionInfo = PdfPositionInfo;
                version.PlotSealGroup = GroupIDs;
                version.PlotSealGroupName = GroupNames;
                version.PlotSealGroupKey = GroupKeys;
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ReSign(string Products)
        {
            var list = JsonHelper.ToList(Products);
            foreach (var item in list)
            {
                var ver = Convert.ToDecimal(item.GetValue("Version"));
                var productID = item.GetValue("ProductID");
                var product = this.entities.Set<S_E_Product>().FirstOrDefault(a => a.ID == productID && a.Version == ver);
                if (product != null)
                {
                    product.SignState = "ReSign";
                    product.SignPdfFile = "";
                }
                var version = this.entities.Set<S_E_ProductVersion>().FirstOrDefault(a => a.ProductID == productID && a.Version == ver);
                if (version != null)
                {
                    version.SignState = "ReSign";
                    version.SignPdfFile = "";
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
