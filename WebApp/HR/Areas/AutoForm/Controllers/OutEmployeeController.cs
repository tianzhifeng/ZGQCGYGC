using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using HR.Logic.Domain;
using MvcAdapter;
using Formula;
using System.Drawing;
using System.Drawing.Imaging;
using Formula.Helper;
using Formula.Exceptions;
using System.Reflection;
using HR.Logic.BusinessFacade;

namespace HR.Areas.AutoForm.EmployeeMessage.Controllers
{
    public class OutEmployeeController : HRFormContorllor<T_Employee>
    {
        public ActionResult Course()
        {
            return View();
        }
        #region 拼接tab

        public ActionResult Tab()
        {
            string Way = EmploymentWay.外聘员工.ToString();
            string EmployeeID = Request["ID"];
            string FuncType = Request["FuncType"];
            ViewBag.TabHtml = EmployeeServiceAuto.BuildTabs(EmployeeID, FuncType, Way);
            return View();
        }
        #endregion

        #region 外聘人员基本信息


        public JsonResult GetModel(string id)
        {
            T_Employee model = entities.Set<T_Employee>().Find(id);
            if (model == null)
            {
                model = new T_Employee();
                model.EmploymentWay = EmploymentWay.外聘员工.ToString();
                model.EmployeeState = EmployeeState.Incumbency.ToString();
                model.IsHaveAccount = "0";
            }
            else
            {
                //职称
                var atiList = entities.Set<T_EmployeeAcademicTitle>().Where(c => c.EmployeeID == id).ToList();
                foreach (T_EmployeeAcademicTitle item in atiList)
                {
                    if (!model.PositionalTitles.Split(',').Contains(item.Title))
                        model.PositionalTitles += item.Title + ",";
                }
                if (!string.IsNullOrEmpty(model.PositionalTitles))
                    model.PositionalTitles = model.PositionalTitles.TrimEnd(',');

                //岗位/岗级
                var wpList = entities.Set<T_EmployeeWorkPost>().Where(c => c.EmployeeID == id).ToList();
                foreach (T_EmployeeWorkPost item in wpList)
                {
                    if (!model.Post.Split(',').Contains(item.Post))
                        model.Post += item.Post + ",";
                    if (!model.PostLevel.Split(',').Contains(item.PostLevel))
                        model.PostLevel += item.PostLevel + ",";
                }
                if (!string.IsNullOrEmpty(model.Post))
                {
                    model.Post = model.Post.TrimEnd(',');
                    model.PostLevel = model.PostLevel.TrimEnd(',');
                }


                //学历
                var ad = entities.Set<T_EmployeeAcademicDegree>().Where(c => c.EmployeeID == id).OrderBy("GraduationDate", false).FirstOrDefault();
                if (ad != null)
                {
                    model.Educational = ad.Education;
                    model.EducationalMajor = ad.FirstProfession;
                }


                //合同类型/定岗日期
                var contract = entities.Set<T_EmployeeContract>().Where(c => c.EmployeeID == id).OrderBy("ContractStartDate", false).FirstOrDefault();
                if (contract != null)
                {
                    model.ContractType = contract.ContractCategory;
                    model.DeterminePostsDate = contract.PostDate;
                }

            }


            return Json(model);
        }

        /// <summary>
        /// 根据员工大类获取员工小类
        /// </summary>
        /// <param name="bigType"></param>
        /// <returns></returns>
        public JsonResult GetEmployeeSmallTypeEnum(string bigType)
        {
            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("SELECT * FROM S_M_EnumItem WHERE EnumDefID IN (SELECT ID FROM S_M_EnumDef Where Code='HR.EmployeeSmallType') AND Category like '%{0}%' Order by SortIndex", bigType);
            DataTable dt = baseHelper.ExecuteDataTable(sql);
            List<object> list = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new { text = dt.Rows[i]["Name"], value = dt.Rows[i]["Code"] });
            }
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="imageType">图片类型</param>
        /// <returns></returns>
        public JsonResult UploadImg(string imageType)
        {
            T_Employee entity = UpdateEntity<T_Employee>();

            if (Request.Files.Count > 0)
            {
                var t = Request.Files[0].InputStream;
                byte[] bt = new byte[t.Length];
                t.Read(bt, 0, int.Parse(t.Length.ToString()));

                switch (imageType)
                {
                    case "Portrait":
                        entity.Portrait = bt;
                        break;
                    case "IdentityCardFace":
                        entity.IdentityCardFace = bt;
                        break;
                    case "IdentityCardBack":
                        entity.IdentityCardBack = bt;
                        break;
                    case "Sign":
                        entity.SignImage = bt;
                        break;
                }

                entities.SaveChanges();
            }
            return Json(new { ID = entity.ID, ImageType = imageType });
        }

        public ActionResult GetPic(string id, string imageType)
        {
            ImageActionResult result = null;
            IUserService service = FormulaHelper.GetService<IUserService>();

            T_Employee entity = entities.Set<T_Employee>().Find(id);
            byte[] img = null;
            if (entity != null)
            {
                switch (imageType)
                {
                    case "Portrait":
                        img = entity.Portrait;
                        break;
                    case "IdentityCardFace":
                        img = entity.IdentityCardFace;
                        break;
                    case "IdentityCardBack":
                        img = entity.IdentityCardBack;
                        break;
                    case "Sign":
                        img = entity.SignImage;
                        break;
                }

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


            if (result == null)
            {
                string noneImageName = GetNoneImageName(imageType);
                result = new ImageActionResult(noneImageName, ImageFormat.Jpeg);
            }
            return result;
        }

        /// <summary>
        /// 根据类型返回默认图路径
        /// </summary>
        /// <param name="imageType">类型</param>
        /// <returns></returns>
        private string GetNoneImageName(string imageType)
        {
            string path = "";
            switch (imageType)
            {
                case "Portrait":
                    path = Server.MapPath(@"/CommonWebResource/RelateResource/image/photo.jpg");
                    break;
                case "IdentityCardFace":
                    path = Server.MapPath(@"/HR/Script/images/sfz01.jpg");
                    break;
                case "IdentityCardBack":
                    path = Server.MapPath(@"/HR/Script/images/sfz02.jpg");
                    break;
                case "Sign":
                    path = Server.MapPath(@"/CommonWebResource/RelateResource/image/signname.jpg");
                    break;

            }
            return path;
        }


        public JsonResult DeleteImage(string imageType)
        {
            string id = GetQueryString("ID");
            T_Employee entity = entities.Set<T_Employee>().Find(id);
            if (entity != null)
            {

                switch (imageType)
                {
                    case "Portrait":
                        entity.Portrait = null;
                        break;
                    case "IdentityCardFace":
                        entity.IdentityCardFace = null;
                        break;
                    case "IdentityCardBack":
                        entity.IdentityCardBack = null;
                        break;
                    case "Sign":
                        entity.SignImage = null;
                        break;
                }
            }
            entities.SaveChanges();
            return Json(new { ID = id, ImageType = imageType });
        }



        public JsonResult GetList(QueryBuilder qb)
        {

            var data = entities.Set<T_Employee>().Where(c => c.IsDeleted == "0" && c.EmploymentWay == "外聘员工").WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult DeleteEmployee()
        {
            string listIDs = Request["ListIDs"];
            string[] idArray = listIDs.Split(',');

            entities.Set<T_Employee>().Where(c => idArray.Contains(c.ID)).Update(c =>
            {
                c.IsDeleted = "1";
                c.DeleteTime = DateTime.Now;
            });
            entities.SaveChanges();
            return Json(JsonAjaxResult.Successful());
        }

        #endregion

    }
}
