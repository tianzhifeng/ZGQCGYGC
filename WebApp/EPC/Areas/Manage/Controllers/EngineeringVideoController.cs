using Config;
using EPC.Logic;
using Formula;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Formula.Helper;
using System.Text.RegularExpressions;
using System.Data;
using System.Drawing;
using EPC.Logic.Domain;

namespace EPC.Areas.Manage.Controllers
{
    public class EngineeringVideoController : EPCController
    {
        public ActionResult List()
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            var tab = new Tab();
            //时间
            var dateArr = new List<dynamic>()
            { new { text = "今天", value = "Today" },
              new { text = "本周", value = "Week" },
              new { text = "本月", value = "Month" },
              new { text = "本年", value = "Year" }};

            var dateArrJson = JsonHelper.ToJson(dateArr);
            var dateCat = CategoryFactory.GetCategoryByString(dateArrJson, "时间", "DateFilter");
            dateCat.Multi = false;
            tab.Categories.Add(dateCat);

            //类别
            var catagory = CategoryFactory.GetCategory("EPC.VideoType", "类别", "ProjectVideoType");
            catagory.Multi = false;
            tab.Categories.Add(catagory);
            //标段
            var sectionList = entities.Set<S_I_Section>().Where(a => a.EngineeringInfoID == engineeringInfoID)
                .Select(a => new { text = a.Name, value = a.ID }).ToList();
            string json = JsonHelper.ToJson(sectionList);
            var sectionCat = CategoryFactory.GetCategoryByString(json, "标段", "Section");
            sectionCat.Multi = false;
            tab.Categories.Add(sectionCat);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");

            //处理DateFilter
            var item = qb.Items.FirstOrDefault(a => a.Field == "DateFilter");
            string dateFilterStr = "";
            if (item != null)
            {
                if (item.Value.ToString() == "Today")
                {
                    dateFilterStr = " and DateDiff(dd,CreateDate,getdate())=0";
                }
                else if (item.Value.ToString() == "Week")
                {
                    dateFilterStr = " and datediff(week,CreateDate,getdate())=0";
                }
                else if (item.Value.ToString() == "Month")
                {
                    dateFilterStr = " and datediff(month,CreateDate,getdate())=0";
                }
                else if (item.Value.ToString() == "Year")
                {
                    dateFilterStr = " and datediff(year,CreateDate,getdate())=0";
                }
                qb.Items.Remove(item);
            }

            string sql = "select * from S_C_ProjectVideo where EngineeringInfoID = '" + engineeringInfoID + "' {0}";
            var dt = SqlHelper.ExecuteDataTable(string.Format(sql, dateFilterStr), qb);
            dt.Columns.Add(new DataColumn("FileID"));
            dt.Columns.Add(new DataColumn("PicCount"));

            foreach (DataRow dr in dt.Rows)
            {
                dr["PicCount"] = 0;
                dr["FileID"] = "";
                if (dr["FileIDs"] != null)
                {
                    string fileIDs = dr["FileIDs"].ToString();
                    if (!string.IsNullOrEmpty(fileIDs))
                    {
                        var fileIDArr = fileIDs.Split(',');
                        if (fileIDArr.Length > 0)
                        {
                            dr["FileID"] = fileIDArr[0];
                        }                            

                        dr["PicCount"] = fileIDArr.Length;
                    }
                }
            }
            GridData gridData = new GridData(dt);
            gridData.total = qb.TotolCount;

            return Json(gridData);
        }

        public ActionResult GetPic(string fileId, int? width, int? height)
        {
            string fileIdPre = fileId;
            if (!string.IsNullOrEmpty(fileIdPre))
            {
                var arr = fileIdPre.Split('_');
                if (arr.Length > 0) 
                {
                    fileIdPre = arr[0];
                }
            }

            byte[] fileBytes = FileStoreHelper.GetFile(fileIdPre);
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
