using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Cooperation.Controllers
{
    public class ImageController : EPCController
    {
        public ActionResult ImgView()
        {
            ViewBag.FieldIDs = GetQueryString("fieldIDs");
            var defaultIndex = GetQueryString("defaultIndex");
            ViewBag.DefaultIndex = string.IsNullOrEmpty(defaultIndex) ? 0 : Convert.ToInt32(defaultIndex);

            return View();
        }

        public ActionResult GetPic(string fieldId, int? width, int? height)
        {
            byte[] fileBytes = FileStoreHelper.GetFile(fieldId);
            Image img = ImageHelper.GetImageFromBytes(fileBytes);

            if (width != null && height != null)
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
