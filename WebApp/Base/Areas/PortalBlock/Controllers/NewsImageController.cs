using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using MvcAdapter;
using Config;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Formula.Helper;

namespace Base.Areas.PortalBlock.Controllers
{
    public class NewsImageController : BaseController
    {
        private const string NewsImagePrefix = "NewsImage_";
        //
        // GET: /PortalBlock/NewsImage/

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Gallery()
        {
            return View();
        }

        public ActionResult Gallerys()
        {
            return View();
        }

        public ActionResult GetPic(string groupID, string id, int? width, int? height)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json("", JsonRequestBehavior.AllowGet);

            //防止sql注入
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sqlStr = "select PictureEntire from S_I_NewsImage where ID =@id ";
            Dictionary<string, object> infoParams = new Dictionary<string, object>();
            infoParams.Add("@id", id);
            var obj = sqlHelper.ExecuteScalar(sqlStr, infoParams, CommandType.Text);
            if(obj!=null)
                return new ImageActionResult((byte[])obj, width, height);
            else
                return Content(string.Empty);
        }

        public ActionResult GetPicThumb(string groupID, string id, int? width, int? height)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json("", JsonRequestBehavior.AllowGet);

            //防止sql注入
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sqlStr = "select PictureThumb from S_I_NewsImage where ID =@id ";
            Dictionary<string, object> infoParams = new Dictionary<string, object>();
            infoParams.Add("@id", id);
            var obj = sqlHelper.ExecuteScalar(sqlStr, infoParams, CommandType.Text);
            if (obj != null)
                return new ImageActionResult((byte[])obj, width, height);
            else
                return Content(string.Empty);
        }

        public JsonResult Delete(string id)
        {
            entities.Set<S_I_NewsImage>().Delete(c => c.GroupID == id);
            entities.Set<S_I_NewsImageGroup>().Delete(c => c.ID == id);
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult GetModel(string id, string title)
        {
            S_I_NewsImageGroup model = GetEntity<S_I_NewsImageGroup>(id);
            if (!string.IsNullOrEmpty(title))
                model.Title = title;
            return Json(model);
        }

        public JsonResult GetModelByDeptHome(string id)
        {
            S_I_NewsImageGroup model = GetEntity<S_I_NewsImageGroup>(id);
            if (string.IsNullOrEmpty(id))
            {
                UserInfo user = Formula.FormulaHelper.GetUserInfo();
                model.DeptDoorId = user.UserOrgID;
                model.DeptDoorName = user.UserOrgName;
                model.CreateTime = DateTime.Now;
                model.CreateUserID = user.UserID;
                model.CreateUserName = user.UserName;
            }
            return Json(model);
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            IQueryable<S_I_NewsImageGroup> list = entities.Set<S_I_NewsImageGroup>().Where(c => string.IsNullOrEmpty(c.DeptDoorId)).AsQueryable();
            GridData gridData = list.WhereToGridData(qb);
            return Json(gridData);
        }

        public JsonResult GetPicturesInfo(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json("", JsonRequestBehavior.AllowGet);

            //防止sql注入
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = "select ID,GroupID,PictureName,Description,SortIndex from S_I_NewsImage WITH(NOLOCK) where GroupID=@groupid order by SortIndex";
            Dictionary<string, object> infoParams = new Dictionary<string, object>();
            infoParams.Add("@groupid", id);
            var dt = sqlHelper.ExecuteDataTable(sql, infoParams, CommandType.Text);

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewsImageGroupExtend(string deptHomeID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"select c.ID,c.Title,b.ID as NewsImageID from
                            (
                            select GroupID,min(SortIndex) as SortIndex from S_I_NewsImage with(nolock) group by GroupID
                            ) a join S_I_NewsImage b with(nolock) on a.GroupID=b.GroupID and a.SortIndex=b.SortIndex 
                            right join S_I_NewsImageGroup c with(nolock) on c.ID=b.GroupID
                            {0}
                            order by c.CreateTime desc";
            string where = !string.IsNullOrEmpty(deptHomeID) ? "where DeptDoorId = '" + deptHomeID + "'" : "where DeptDoorId is null or DeptDoorId='' ";
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format(sql, where));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewsImageGroupByDeptHome(string deptHomeID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"select c.ID,c.Title,c.Remark,b.ID as NewsImageID,b.CreateTime from
                            (
                            select GroupID,min(SortIndex) as SortIndex from S_I_NewsImage with(nolock) group by GroupID
                            ) a join S_I_NewsImage b with(nolock) on a.GroupID=b.GroupID and a.SortIndex=b.SortIndex 
                            right join S_I_NewsImageGroup c with(nolock) on c.ID=b.GroupID
                            where DeptDoorId = '{0}'
                            order by c.CreateTime desc";
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format(sql, deptHomeID));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadPictures()
        {
            if (Request.Files["FileData"] != null && !string.IsNullOrEmpty(Request["GroupID"]))
            {
                var t = Request.Files["FileData"].InputStream;
                string fileName = Request.Files["FileData"].FileName;
                string extName = Path.GetExtension(fileName);
                Image img = Image.FromStream(t);
                ImageFormat imgFormat = ImageHelper.GetImageFormat(extName);
                byte[] bt = ImageHelper.ImageToBytes(img, imgFormat);
                int height = img.Height;
                int width = img.Width;
                int limitedHeight = !string.IsNullOrEmpty(Request["ThumbHeight"]) ? Convert.ToInt32(Request["ThumbHeight"]) : 60;
                int thumbHeight, thumbWidth;
                byte[] btThumb = null;
                if (height > limitedHeight)
                {
                    thumbHeight = limitedHeight;
                    thumbWidth = thumbHeight * width / height;
                    Image imgThumb = img.GetThumbnailImage(thumbWidth, thumbHeight, null, IntPtr.Zero);
                    btThumb = ImageHelper.ImageToBytes(imgThumb, imgFormat);
                }
                else
                {
                    btThumb = bt;
                }
                if (height > 538)
                {
                    int imgWidth = 538 * width / height;
                    Image imgThumb = ZoomImage(img, 538, imgWidth);
                    bt = ImageHelper.ImageToBytes(imgThumb, imgFormat);
                }
                S_I_NewsImage newsImage = new S_I_NewsImage();
                string groupID = Request["GroupID"];
                newsImage.ID = Formula.FormulaHelper.CreateGuid();
                newsImage.GroupID = Request["GroupID"];
                newsImage.PictureName = fileName;
                newsImage.PictureEntire = bt;
                newsImage.PictureThumb = btThumb;
                newsImage.SortIndex = entities.Set<S_I_NewsImage>().Where(c => c.GroupID == groupID).Count();
                newsImage.CreateTime = DateTime.Now;
                newsImage.CreateUserID = Formula.FormulaHelper.GetUserInfo().UserID;
                newsImage.CreateUserName = Formula.FormulaHelper.GetUserInfo().UserName;

                entities.Set<S_I_NewsImage>().Add(newsImage);
                entities.SaveChanges();

                return Json(new { ID = newsImage.ID, GroupID = newsImage.GroupID, PictureName = newsImage.PictureName,SortIndex = newsImage.SortIndex });
            }
            else
            {
                return Json(string.Empty);
            }
        }

        public JsonResult ZoomPic(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new Exception("没有图片新闻头！");

            var picList = entities.Set<S_I_NewsImage>().Where(c => c.GroupID == id).ToList();
            picList.ForEach(c =>
            {
                try
                {
                    if (c.PictureEntire != null)
                    {
                        using (MemoryStream _ms = new MemoryStream(c.PictureEntire))
                        {
                            Image _img = Bitmap.FromStream(_ms, true);
                            if (_img.Height > 538)
                            {
                                int imgWidth = 538 * _img.Width / _img.Height;
                                Image imgThumb = ZoomImage(_img, 538, imgWidth);
                                c.PictureEntire = ImageHelper.ImageToBytes(imgThumb, ImageFormat.Png);
                            }
                        }
                    }
                }
                catch { }
            });
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult DeleteNewsImage(string groupID, string id)
        {
            base.JsonDelete<S_I_NewsImage>(id);
            List<S_I_NewsImage> list = entities.Set<S_I_NewsImage>().Where(c => c.GroupID == groupID).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SortIndex = i;
            }
            entities.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult Save()
        {
            var obj = UpdateEntity<S_I_NewsImageGroup>();
            var List = UpdateList<S_I_NewsImage>();
            entities.SaveChanges();

            return Json("");
        }

        //按指定大小缩放图片
        private Image ZoomImage(Image bitmap, int destHeight, int destWidth)
        {
            try
            {
                System.Drawing.Image sourImage = bitmap;
                int width = 0, height = 0;
                //按比例缩放             
                int sourWidth = sourImage.Width;
                int sourHeight = sourImage.Height;
                if (sourHeight > destHeight || sourWidth > destWidth)
                {
                    if ((sourWidth * destHeight) > (sourHeight * destWidth))
                    {
                        width = destWidth;
                        height = (destWidth * sourHeight) / sourWidth;
                    }
                    else
                    {
                        height = destHeight;
                        width = (sourWidth * destHeight) / sourHeight;
                    }
                }
                else
                {
                    width = sourWidth;
                    height = sourHeight;
                }
                Bitmap destBitmap = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage(destBitmap);
                g.Clear(Color.Transparent);
                //设置画布的描绘质量           
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(sourImage, new Rectangle((destWidth - width) / 2, (destHeight - height) / 2, width, height), 0, 0, sourImage.Width, sourImage.Height, GraphicsUnit.Pixel);
                g.Dispose();
                //设置压缩质量       
                System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters();
                long[] quality = new long[1];
                quality[0] = 100;
                System.Drawing.Imaging.EncoderParameter encoderParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;
                sourImage.Dispose();
                return destBitmap;
            }
            catch
            {
                return bitmap;
            }
        }

    }
}
