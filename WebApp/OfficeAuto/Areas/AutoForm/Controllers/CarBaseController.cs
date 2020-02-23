using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using MvcAdapter;
using Formula;
using System.Drawing;
using Formula.Helper;
using System.Drawing.Imaging;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class CarBaseController : OfficeAutoFormContorllor<T_Car_Base>
    {
        //
        // GET: /AutoForm/CarBase/

        public JsonResult UploadImg()
        {
            T_Car_Base entity = UpdateEntity<T_Car_Base>();

            if (Request.Files.Count > 0)
            {
                var t = Request.Files[0].InputStream;
                byte[] bt = new byte[t.Length];
                t.Read(bt, 0, int.Parse(t.Length.ToString()));

                entity.CarPhoto = bt;

                BusinessEntities.SaveChanges();
            }
            return Json(new { ID = entity.ID });
        }

        public JsonResult DeleteImage()
        {
            string id = GetQueryString("ID");
            var entity = BusinessEntities.Set<T_Car_Base>().Find(id);
            if (entity != null)
            {
                entity.CarPhoto = null;
            }
            BusinessEntities.SaveChanges();
            return Json(new { ID = id });
        }

        public ActionResult GetImg(string id)
        {
            ImageActionResult result = null;

            T_Car_Base entity = BusinessEntities.Set<T_Car_Base>().Find(id);
            byte[] img = null;
            if (entity != null)
            {
                img = entity.CarPhoto;

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
    }
}
