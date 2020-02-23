using Base.Logic.BusinessFacade;
using Base.Logic.Domain;
using Base.Logic.Model.UI.Form;
using Config;
using Formula;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Base.Areas.UI.Controllers
{
    public class ExcelPrintController : BaseController<S_UI_ExcelPrint>
    {

        public override ActionResult List()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            return View();
        }

        public override ActionResult Edit()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        public JsonResult UploadExcel()
        {
            if (Request.Files.Count > 0)
            {
                var dir = HttpContext.Server.MapPath("/") + "ExcelPrint";
                if (System.IO.Directory.Exists(dir) == false)
                    System.IO.Directory.CreateDirectory(dir);

                string code = Request["TmplCode"];
                string fileFullName = dir + "/" + code + System.IO.Path.GetExtension(Request.Files[0].FileName);
                Request.Files[0].SaveAs(fileFullName);

                var define = entities.Set<S_UI_ExcelPrint>().Where(c => c.Code == code).SingleOrDefault();
                var user = FormulaHelper.GetUserInfo();
                define.ModifyUserID = user.UserID;
                define.ModifyUserName = user.UserName;
                define.ModifyTime = DateTime.Now;
                entities.SaveChanges();
            }
            return Json("");
        }

        public FileResult ExcelFile(string tmplCode)
        {
            var define = entities.Set<S_R_Define>().Where(c => c.Code == tmplCode).SingleOrDefault();
            string filePath = HttpContext.Server.MapPath("/") + "ExcelPrint/" + tmplCode + ".xlsx";
            string filePath1 = HttpContext.Server.MapPath("/") + "ExcelPrint/" + tmplCode + ".xls";
            if (System.IO.File.Exists(filePath))
                return File(filePath, "application/octet-stream ; Charset=UTF8", tmplCode + ".xlsx");
            else
                return File(filePath1, "application/octet-stream ; Charset=UTF8", tmplCode + ".xls");
        }


        public JsonResult GetExcelPrintList(QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CategoryID"]))
            {
                string categoryID = Request["CategoryID"];
                var arr = entities.Set<S_M_Category>().Where(c => c.ID == categoryID || c.ParentID == categoryID).Select(c => c.ID).ToArray();
                string ids = string.Join(",", arr);
                qb.Add("CategoryID", QueryMethod.In, ids);
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"select * from S_UI_ExcelPrint";

            if (!string.IsNullOrEmpty(Request["CategoryID"]))
                sql += string.Format(" where CategoryID='{0}'", Request["CategoryID"]);


            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);


            dt.Columns.Add("HasExcel");
            foreach (DataRow row in dt.Rows)
            {
                string path = HttpContext.Server.MapPath("/") + "ExcelPrint/" + row["Code"].ToString() + ".xlsx";
                string path1 = HttpContext.Server.MapPath("/") + "ExcelPrint/" + row["Code"].ToString() + ".xls";
                if (System.IO.File.Exists(path) || System.IO.File.Exists(path1))
                    row["HasExcel"] = "1";
                else
                    row["HasExcel"] = "0";
            }


            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public JsonResult GetItemList(string excelPrintId)
        {
            var entity = entities.Set<S_UI_ExcelPrint>().SingleOrDefault(c => c.ID == excelPrintId);
            if (!string.IsNullOrEmpty(entity.Items) && entity.Items != "[]")
                return Json(entities.Set<S_UI_ExcelPrint>().SingleOrDefault(c => c.ID == excelPrintId).Items);
            else
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(entity.ConnName);
                string sql = string.Format(@"
				SELECT 
                    字段名= convert(varchar(100), a.name),
                    表名= convert(varchar(50), d.name ),
                    类型= CONVERT(varchar(50),b.name),
                    库名= 'ServerModeXpoDemo',
                    字段说明=convert(varchar(50), isnull(g.[value],''))
                    FROM dbo.syscolumns a
                    left join dbo.systypes b on a.xusertype=b.xusertype
                    inner join dbo.sysobjects d on a.id=d.id and d.xtype='U' and d.name<>'dtproperties'
                    left join dbo.syscomments e on a.cdefault=e.id
                    left join sys.extended_properties g on a.id=g.major_id and a.colid=g.minor_id
                    left join sys.extended_properties f on d.id=f.major_id and f.minor_id=0
                    where d.name ='{0}'
", entity.TableName);
                var dt = sqlHelper.ExecuteDataTable(sql);
                List<Object> list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new { ItemType = "Text", Code = row["字段名"].ToString(), Name = row["字段说明"].ToString() });
                }
                return Json(list);
            }
        }

        public JsonResult SaveItemList(string excelPrintId, string itemList)
        {
            var word = entities.Set<S_UI_ExcelPrint>().SingleOrDefault(c => c.ID == excelPrintId);
            word.Items = itemList;
            var user = FormulaHelper.GetUserInfo();
            word.ModifyUserID = user.UserID;
            word.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }


    }
}
