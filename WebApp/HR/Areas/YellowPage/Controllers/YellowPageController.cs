using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using System.Data.Entity;
using Formula;
using Base.Logic.Domain;
using System.Reflection;
using Formula.DynConditionObject;
using Config;
using System.Data;
using MvcAdapter;
using System.Drawing.Imaging;
using Formula.Helper;
using System.Drawing;


namespace HR.Areas.YellowPage.Controllers
{
    public class YellowPageController : BaseController<T_Employee>
    {
        //
        // GET: /YellowPage/YellowPage/

        public JsonResult GetOrgTree()
        {
            DbContext _entities = FormulaHelper.GetEntities<BaseEntities>();
            var rootFullId = Request["RootFullID"] ?? "";

            PropertyInfo ptySortIndex = typeof(S_A_Org).GetProperty("SortIndex");

            var res = new Specifications();
            IQueryable<S_A_Org> query = _entities.Set<S_A_Org>().AsQueryable();

            res.Clear();
            res.AndAlso("FullID", rootFullId, QueryMethod.StartsWith);
            query = query.Where(res.GetExpression<S_A_Org>());

            query = DataBaseFilter(query); //数据过滤

            if (ptySortIndex != null)
                query = query.OrderBy("SortIndex", true);

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            const string sql = @" SELECT a.ID,a.UserID,a.Name,a.Sex,a.Portrait,a.DeptID,a.DeptName,a.MobilePhone,a.OfficePhone,a.HomePhone,a.Birthday,a.PositionalTitles FROM T_Employee a  where a.IsDeleted='0'";
            SQLHelper helper = SQLHelper.CreateSqlHelper(ConnEnum.HR);
            DataTable dt = helper.ExecuteDataTable(sql);
            var userList = new List<T_Employee>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var user = new T_Employee();
                user.ID = objToString(dt.Rows[i]["ID"]);
                user.UserID = objToString(dt.Rows[i]["UserID"]);
                user.Name = objToString(dt.Rows[i]["Name"]);
                user.Sex = objToString(dt.Rows[i]["Sex"]);
                user.OfficePhone = objToString(dt.Rows[i]["OfficePhone"]);
                user.MobilePhone = objToString(dt.Rows[i]["MobilePhone"]);
                user.HomePhone = objToString(dt.Rows[i]["HomePhone"]);
                if (objToString(dt.Rows[i]["Birthday"]) == "")
                {
                    user.Birthday = null;
                }
                else
                {
                    user.Birthday = Convert.ToDateTime(objToString(dt.Rows[i]["Birthday"]));
                }
                user.DeptID = objToString(dt.Rows[i]["DeptID"]);
                user.DeptIDName = objToString(dt.Rows[i]["DeptName"]);
                user.DeptName = objToString(dt.Rows[i]["DeptName"]);
                //user.Portrait = objToString(dt.Rows[i]["Portrait"]);
                user.PositionalTitles = objToString(dt.Rows[i]["PositionalTitles"]);
                userList.Add(user);
            }
            return View(userList);
        }

        public override ActionResult List()
        {
            string name = Request.QueryString.Get("EmployeeName");
            string deptId = Request.QueryString.Get("DeptID");
            string sortIndex = Request.QueryString.Get("SortIndex");
            string sortChar = Request.QueryString.Get("SortChar");
            SQLHelper helperHr = SQLHelper.CreateSqlHelper(ConnEnum.HR);

            string sql = @"SELECT a.ID,a.UserID,a.Name,a.Sex,a.Portrait,a.DeptID,a.DeptName,a.MobilePhone,a.OfficePhone,a.HomePhone,a.Birthday,a.PositionalTitles,a.Email FROM " + helperHr.DbName + "..T_Employee a  where a.IsDeleted='0'";
            if (!String.IsNullOrWhiteSpace(name))
            {
                string[,] hz = getHanziScope(name);
                for (int i = 0; i < hz.GetLength(0); i++)
                {
                    sql += " AND SUBSTRING(a.Name, " + (i + 1) + ", 1) >= '" + hz[i, 0] + "' AND SUBSTRING(a.Name, " + (i + 1) + ", 1) < '" + hz[i, 1] + "'";
                }
                sql += @" OR a.Name LIKE '%" + name + @"%'";
            }
            if (!String.IsNullOrWhiteSpace(sortChar))
            {
                string[,] hz = getHanziScope(sortChar);
                for (int i = 0; i < hz.GetLength(0); i++)
                {
                    sql += " AND ((SUBSTRING(a.Name, " + (i + 1) + ", 1) >= '" + hz[i, 0] + "' AND SUBSTRING(a.Name, " + (i + 1) + ", 1) < '" + hz[i, 1] + "') OR a.Name LIKE '"+ sortChar + "%')";
                }
            }
            if (!String.IsNullOrWhiteSpace(deptId))
            {
                sql += @" AND a.UserID IN (SELECT UserID FROM S_A__OrgUser WHERE OrgID = '" + deptId + @"')";
            }
            if (!String.IsNullOrWhiteSpace(sortIndex))
            {
                sql += @" ORDER BY a.PositionalTitles " + sortIndex;
            }
            SQLHelper helper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
           
            DataTable dt = helper.ExecuteDataTable(sql);
            var userList = new List<T_Employee>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var user = new T_Employee();
                user.ID = objToString(dt.Rows[i]["ID"]);
                user.UserID = objToString(dt.Rows[i]["UserID"]);
                user.Name = objToString(dt.Rows[i]["Name"]);
                user.Sex = objToString(dt.Rows[i]["Sex"]);
                user.OfficePhone = objToString(dt.Rows[i]["OfficePhone"]);
                user.MobilePhone = objToString(dt.Rows[i]["MobilePhone"]);
                user.HomePhone = objToString(dt.Rows[i]["HomePhone"]);
                user.Email = objToString(dt.Rows[i]["Email"]);
                if (objToString(dt.Rows[i]["Birthday"]) == "")
                {
                    user.Birthday = null;
                }
                else
                {
                    user.Birthday = Convert.ToDateTime(objToString(dt.Rows[i]["Birthday"]));
                }
                user.DeptID = objToString(dt.Rows[i]["DeptID"]);
                user.DeptIDName = objToString(dt.Rows[i]["DeptName"]);
                user.DeptName = objToString(dt.Rows[i]["DeptName"]);
                //user.Portrait = objToString(dt.Rows[i]["Portrait"]);
                user.PositionalTitles = objToString(dt.Rows[i]["PositionalTitles"]);
                userList.Add(user);
            }
            return View(userList);
        }

        private string objToString(object obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
                return string.Empty;
            return obj.ToString();
        }

        private string[,] getHanziScope(string pinyinIndex)
        {
            pinyinIndex = pinyinIndex.ToLower();
            string[,] hz = new string[pinyinIndex.Length, 2];
            for (int i = 0; i < pinyinIndex.Length; i++)
            {
                string index = pinyinIndex.Substring(i, 1);
                if (index == "a") { hz[i, 0] = "吖"; hz[i, 1] = "驁"; }
                else if (index == "b") { hz[i, 0] = "八"; hz[i, 1] = "簿"; }
                else if (index == "c") { hz[i, 0] = "嚓"; hz[i, 1] = "錯"; }
                else if (index == "d") { hz[i, 0] = "咑"; hz[i, 1] = "鵽"; }
                else if (index == "e") { hz[i, 0] = "妸"; hz[i, 1] = "樲"; }
                else if (index == "f") { hz[i, 0] = "发"; hz[i, 1] = "猤"; }
                else if (index == "g") { hz[i, 0] = "旮"; hz[i, 1] = "腂"; }
                else if (index == "h") { hz[i, 0] = "妎"; hz[i, 1] = "夻"; }
                else if (index == "j") { hz[i, 0] = "丌"; hz[i, 1] = "攈"; }
                else if (index == "k") { hz[i, 0] = "咔"; hz[i, 1] = "穒"; }
                else if (index == "l") { hz[i, 0] = "垃"; hz[i, 1] = "鱳"; }
                else if (index == "m") { hz[i, 0] = "嘸"; hz[i, 1] = "椧"; }
                else if (index == "n") { hz[i, 0] = "拏"; hz[i, 1] = "桛"; }
                else if (index == "o") { hz[i, 0] = "噢"; hz[i, 1] = "漚"; }
                else if (index == "p") { hz[i, 0] = "妑"; hz[i, 1] = "曝"; }
                else if (index == "q") { hz[i, 0] = "七"; hz[i, 1] = "裠"; }
                else if (index == "r") { hz[i, 0] = "亽"; hz[i, 1] = "鶸"; }
                else if (index == "s") { hz[i, 0] = "仨"; hz[i, 1] = "蜶"; }
                else if (index == "t") { hz[i, 0] = "他"; hz[i, 1] = "籜"; }
                else if (index == "w") { hz[i, 0] = "屲"; hz[i, 1] = "鶩"; }
                else if (index == "x") { hz[i, 0] = "夕"; hz[i, 1] = "鑂"; }
                else if (index == "y") { hz[i, 0] = "丫"; hz[i, 1] = "韻"; }
                else if (index == "z") { hz[i, 0] = "帀"; hz[i, 1] = "咗"; }
                else { hz[i, 0] = index; hz[i, 1] = index; }
            }
            return hz;
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


       
    }
}
