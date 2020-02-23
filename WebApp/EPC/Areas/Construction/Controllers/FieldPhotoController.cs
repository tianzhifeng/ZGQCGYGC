using Config.Logic;
using EPC.Logic.Domain;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Construction.Controllers
{
    public class FieldPhotoController : EPCController
    {
        //
        // GET: /Construction/FieldPhoto/

        public ActionResult PageView()
        {
            string engineeringID = GetQueryString("EngineeringInfoID");

            var videoList = entities.Set<S_C_ProjectVideo>().Where(a => a.ProjectID == engineeringID)
                .OrderByDescending(a => a.CreateDate).ToList();

            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
            int groupIndex = 1;
            foreach (var video in videoList)
            {
                string firstLine = video.Remark;

                int picIndex = 1;
                if (string.IsNullOrEmpty(video.FileIDs))
                    continue;

                var fileIDArr = video.FileIDs.Split(',');
                foreach (var fileID in fileIDArr)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.SetValue("FirstLine", firstLine);
                    dic.SetValue("PicNum", "图" + groupIndex + "-" + picIndex);
                    dic.SetValue("Publisher", video.CreateUserName);
                    dic.SetValue("PublishDate", video.CreateDate.Value.ToString("yyyy-MM-dd"));
                    dic.SetValue("ImgID", fileID);
                    picIndex++;
                    results.Add(dic);
                }
                groupIndex++;
            }

            ViewBag.ImgList = results;
            return View();
        }

        public ActionResult ViewBigImg()
        {
            return View();
        }

        public ActionResult GetPic(string fieldId, int? width, int? height)
        {
            byte[] fileBytes = FileStoreHelper.GetFile(fieldId);
            Image img = ImageHelper.GetImageFromBytes(fileBytes);

            if(width!=null && height != null)
            {
                
                int fWidth = (int)((double)height.Value * (double)img.Width / (double)img.Height);

                //纵向布满后后横向超出了给定宽度
                //则横向布满
                if (fWidth > width.Value)
                {
                    int fHeight = (int)((double)img.Height * (double)width.Value / (double)img.Width);
                    img = img.GetThumbnailImage(width.Value, fHeight, null, IntPtr.Zero);
                }
                else
                {
                    img = img.GetThumbnailImage(fWidth, height.Value, null, IntPtr.Zero);
                }
            }            

            return new ImageActionResult(img);
        }
    }
}
