using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using EPC.Areas.Cooperation.Models;
using Base.Logic.Domain;
using MvcAdapter;
using System.Data;
using Formula.Exceptions;
using System.Drawing;

namespace EPC.Areas.Cooperation.Controllers
{
    public class VideoController : EPCFormContorllor<S_C_ProjectVideo>
    {
        public override ActionResult PageView()
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            string sql = "select Name as text,ID as value from S_I_Section where EngineeringInfoID = '" + engineeringInfoID + "'";
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            string json = JsonHelper.ToJson(dt);
            @ViewBag.SectionData = json;
            return View();
        }

        public JsonResult GetModel(string id)
        {
            var dic = new Dictionary<string, object>();
            bool isNew = true;
            if (!String.IsNullOrEmpty(id))
            {
                var sql = String.Format("select * from {0} where ID='{1}'", "S_C_ProjectVideo", id);
                var dt = this.EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    dic = FormulaHelper.DataRowToDic(dt.Rows[0]);
                    isNew = false;
                }
                else
                {
                    throw new Formula.Exceptions.BusinessValidationException(String.Format("数据表【{0}】中没有找到ID为【{1}】的记录，无法读取数据", "S_C_ProjectVideo", id));
                }
            }
            AfterGetData(dic, isNew, "");
            return Json(dic);
        }

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {

                string sectionStr = dic.GetValue("Section");
                string sql = "select Name from S_I_Section where ID = '" + sectionStr + "'";
                var name = this.EPCSQLDB.ExecuteScalar(sql);
                dic.SetValue("SectionName", name.ToString());

        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                dic.SetValue("ProjectID", GetQueryString("EngineeringInfoID"));
                dic.SetValue("EngineeringInfoID", GetQueryString("EngineeringInfoID"));
                dic.SetValue("CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                dic.SetValue("CreateUser", CurrentUserInfo.UserName);
                dic.SetValue("CreateUserID", CurrentUserInfo.UserID);
                dic.SetValue("CreateUserName", CurrentUserInfo.UserName);
                dic.SetValue("ID", FormulaHelper.CreateGuid());

            }
        }

        protected override void AfterSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            LoggerHelper.InserLogger("S_C_ProjectVideo", isNew ? EnumOperaType.Add : EnumOperaType.Update, dic.GetValue("ID"), dic.GetValue("EngineeringInfoID"), dic.GetValue("Title"));
        }

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
            if (GetQueryString("MonthVideo") == "true")
            {
                var monthItem = dateCat.Items.FirstOrDefault(a => a.Value == "Month");
                if (monthItem != null)
                    monthItem.IsDefault = true;
                var allItem = dateCat.Items.FirstOrDefault(a => a.Value == "All");
                if (allItem != null)
                    allItem.IsDefault = false;
            }
            tab.Categories.Add(dateCat);

            //类别
            var catagory = CategoryFactory.GetCategory("EPC.VideoType", "类别", "ProjectVideoType");
            catagory.Multi = false;
            tab.Categories.Add(catagory);
            //标段
            string sql = "select Name as text,ID as value from S_I_Section where EngineeringInfoID = '" + engineeringInfoID + "'";
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            string json = JsonHelper.ToJson(dt);
            var sectionCat = CategoryFactory.GetCategoryByString(json, "标段", "Section");
            sectionCat.Multi = false;
            tab.Categories.Add(sectionCat);
            //是否发布
            var catagoryPublish = CategoryFactory.GetCategory("System.TrueOrFalse", "是否已发布", "IsPublish");
            catagoryPublish.Multi = false;
            tab.Categories.Add(catagoryPublish);
            if (GetQueryString("IsPublish") == "0")
            {
                var item = catagoryPublish.Items.FirstOrDefault(a => a.Value == "0");
                if (item != null)
                    item.IsDefault = true;
                var allItem = catagoryPublish.Items.FirstOrDefault(a => a.Value == "All");
                if (allItem != null)
                    allItem.IsDefault = false;
            }

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetVideoList(QueryBuilder qb)
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            string isMine = GetQueryString("IsMine");

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

            string isMineFilterStr = "";
            if (isMine == "true")
            {
                isMineFilterStr = " and CreateUserOrg = '" + CurrentUserInfo.UserCompanyID + "'";
            }

            string sql = "select * from S_C_ProjectVideo where EngineeringInfoID = '" + engineeringInfoID + "' {0} {1}";
            var dt = this.EPCSQLDB.ExecuteDataTable(string.Format(sql, dateFilterStr, isMineFilterStr), qb);
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

        [LoggerFilter(OperaType = EnumOperaType.Update, Table = "S_C_ProjectVideo", IDField = "ID", NameField = "Title", Remark = "发布")]
        public JsonResult PublishVideo()
        {
            string id = GetQueryString("ID");
            string sql = "update S_C_ProjectVideo set IsPublish = '1' where ID = '" + id + "'";
            this.EPCSQLDB.ExecuteNonQuery(sql);
            return Json("");
        }

        [LoggerFilter(OperaType = EnumOperaType.Update, Table = "S_C_ProjectVideo", IDField = "ID", NameField = "Title", Remark = "取消发布")]
        public JsonResult DisPublishVideo()
        {
            string id = GetQueryString("ID");
            string sql = "update S_C_ProjectVideo set IsPublish = '0' where ID = '" + id + "'";
            this.EPCSQLDB.ExecuteNonQuery(sql);
            return Json("");
        }

        [LoggerFilter(OperaType = EnumOperaType.Delete, Table = "S_C_ProjectVideo", IDField = "ListData.ID", NameField = "ListData.Title", Remark = "批量删除")]
        public JsonResult DeleteVideo()
        {
            string listDataStr = GetQueryString("ListData");
            var dicList = JsonHelper.ToList(listDataStr);
            var idList = dicList.Select(a => a.GetValue("ID"));
            string sql = string.Format("delete from S_C_ProjectVideo where ID in ('{0}')", string.Join("','", idList));
            this.EPCSQLDB.ExecuteNonQuery(sql);
            return Json("");
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
