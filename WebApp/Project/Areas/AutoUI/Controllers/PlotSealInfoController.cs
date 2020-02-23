using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using System.IO;

namespace Project.Areas.AutoUI.Controllers
{
    public class PlotSealInfoController : ProjectFormContorllor<S_EP_PlotSealInfo>
    {
        public JsonResult UploadImg(string imageType)
        {
            S_EP_PlotSealInfo entity = UpdateEntity<S_EP_PlotSealInfo>();
            if (Request.Files.Count > 0)
            {
                var t = Request.Files[0].InputStream;
                byte[] bt = new byte[t.Length];
                t.Read(bt, 0, int.Parse(t.Length.ToString()));
                entity.SealInfo = bt;
                Image image = ImageHelper.BytesToImage(bt);
                using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    entity.Width = (decimal)Math.Round(image.Width / graphics.DpiX * 25.4);
                    entity.Height = (decimal)Math.Round(image.Height / graphics.DpiY * 25.4);
                }

                BusinessEntities.SaveChanges();
            }
            return Json(new { ID = entity.ID, Width = entity.Width, Height = entity.Height });
        }

        public JsonResult DeleteImage()
        {
            string id = GetQueryString("ID");
            S_EP_PlotSealInfo entity = BusinessEntities.Set<S_EP_PlotSealInfo>().Find(id);
            if (entity != null)
            {
                entity.SealInfo = null;
            }
            BusinessEntities.SaveChanges();
            return Json(new { ID = id });
        }

        public ActionResult GetPic(string id)
        {
            ImageActionResult result = null;
            var entity = this.GetEntityByID(id);
            if (entity != null)
            {
                byte[] img = entity.SealInfo;
                if (img != null)
                {
                    Image image = ImageHelper.BytesToImage(img);
                    if (image != null)
                    {
                        ImageFormat imageFormat = ImageHelper.GetImageFormat(image);
                        result = new ImageActionResult(image, imageFormat);
                    }
                }
            }
            return result;
        }
        public ActionResult GetSealPic(string id)
        {
            ImageActionResult result = null;
            S_EP_PlotSealInfo entity = this.BusinessEntities.Set<S_EP_PlotSealInfo>().Find(id);
            byte[] img = entity.SealInfo;
            if (img != null)
            {
                Image image = ImageHelper.BytesToImage(img);
                if (image != null)
                {
                    ImageFormat imageFormat = ImageHelper.GetImageFormat(image);
                    result = new ImageActionResult(image, imageFormat);
                }
            }
            return result;
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (!isNew)
            {
                var name = dic.GetValue("Name");
                var code = dic.GetValue("Code");
                var type = dic.GetValue("Type");
                var belongUser = dic.GetValue("BelongUser");
                var belongUserName = dic.GetValue("BelongUserName");
                var authInfo = dic.GetValue("AuthInfo");
                var width = dic.GetValue("Width");
                var height = dic.GetValue("Height");
                var blockKey = dic.GetValue("BlockKey");

                var updateSql = string.Format(@"update S_EP_PlotSealGroup_GroupInfo
set Name='{1}',
Code='{2}',
Type='{3}',
BelongUser='{4}',
BelongUserName='{5}',
AuthInfo='{6}',
Width='{7}',
Height='{8}',
BlockKey = '{9}'
where PlotSeal='{0}'", dic.GetValue("ID"), name, code, type, belongUser, belongUserName, authInfo, width, height, blockKey);
                this.ProjectSQLDB.ExecuteNonQuery(updateSql);
            }
        }

        protected override void BeforeDelete(string[] Ids)
        {
            var groupInfos = this.BusinessEntities.Set<S_EP_PlotSealGroup_GroupInfo>().Where(a => Ids.Contains(a.PlotSeal)).ToList();
            if (groupInfos.Count > 0)
            {
                throw new Formula.Exceptions.BusinessException("选中的图章被使用在图章组合中，请重新选择");
            }
        }


        public JsonResult CreateSingleGroup(string seals)
        {
            var list = JsonHelper.ToList(seals);
            int success = 0, noNeed = 0;
            foreach (var item in list)
            {
                var id = item.GetValue("ID");
                var singleGroup = this.BusinessEntities.Set<S_EP_PlotSealGroup>().FirstOrDefault(a => a.MainPlotSealID == id);
                if (singleGroup == null)
                {
                    singleGroup = new S_EP_PlotSealGroup();
                    singleGroup.ID = FormulaHelper.CreateGuid();
                    singleGroup.Name = item.GetValue("Name");
                    singleGroup.Code = item.GetValue("Code");
                    singleGroup.MainPlotSealID = id;
                    EntityCreateLogic<S_EP_PlotSealGroup>(singleGroup);
                    this.BusinessEntities.Set<S_EP_PlotSealGroup>().Add(singleGroup);

                    var groupInfo = new S_EP_PlotSealGroup_GroupInfo();
                    this.UpdateEntity<S_EP_PlotSealGroup_GroupInfo>(groupInfo, item);
                    groupInfo.ID = FormulaHelper.CreateGuid();
                    groupInfo.S_EP_PlotSealGroupID = singleGroup.ID;
                    groupInfo.SortIndex = 0;
                    groupInfo.IsMain = "true";
                    groupInfo.PlotSeal = id;
                    groupInfo.CorrectPosX = 0;
                    groupInfo.CorrectPosY = 0;
                    this.BusinessEntities.Set<S_EP_PlotSealGroup_GroupInfo>().Add(groupInfo);
                    success++;
                }
                else
                    noNeed++;
            }
            this.BusinessEntities.SaveChanges();
            return Json(new { Success = success, NoNeed = noNeed });
        }
    }
}
