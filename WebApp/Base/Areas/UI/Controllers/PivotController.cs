using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula.Helper;
using Base.Logic.Domain;
using Formula;
using MvcAdapter;
using Config;
using Formula.Exceptions;
using System.Data;

namespace Base.Areas.UI.Controllers
{
    public class PivotController : BaseController
    {
        #region 树和列表数据获取

        public JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CategoryID"]))
            {
                string categoryID = Request["CategoryID"];
                var arr = entities.Set<S_M_Category>().Where(c => c.ID == categoryID || c.ParentID == categoryID).Select(c => c.ID).ToArray();
                string ids = string.Join(",", arr);
                qb.Add("CategoryID", QueryMethod.In, ids);
            }
            var list = entities.Set<S_UI_Pivot>().Where(qb).Select(c => new { ID = c.ID, Code = c.Code, Name = c.Name, ConnName = c.ConnName, SQL = c.SQL, ModifyTime = c.ModifyTime });
            GridData data = new GridData(list);
            data.total = qb.TotolCount;

            return Json(data);
        }

        #endregion

        #region 基本信息

        public ActionResult List()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            return View();
        }

        public ActionResult Edit()
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

        public JsonResult GetModel(string id)
        {
            return JsonGetModel<S_UI_Pivot>(id);
        }

        [ValidateInput(false)]
        public JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_Pivot>();
            if (entities.Set<S_UI_Pivot>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception("编号不能重复！");
            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == entity.CategoryID);
            entity.ConnName = category.Code;   
            return JsonSave<S_UI_Pivot>(entity);
        }

        public JsonResult Delete()
        {
            return JsonDelete<S_UI_Pivot>(Request["ListIDs"]);
        }

        #endregion

        #region 克隆

        public JsonResult Clone(string cloneID)
        {
            var info = entities.Set<S_UI_Pivot>().SingleOrDefault(c => c.ID == cloneID);
            var newInfo = new S_UI_Pivot();
            FormulaHelper.UpdateModel(newInfo, info);

            newInfo.ID = FormulaHelper.CreateGuid();
            newInfo.Code += "copy";          
            newInfo.Name += "(副本)";
            newInfo.ModifyTime = null;
            newInfo.ModifyUserID = "";
            newInfo.ModifyUserName = "";
            entities.Set<S_UI_Pivot>().Add(newInfo);
            entities.SaveChanges();
            return Json("");
        }


        #endregion

        public JsonResult ImportField(string id)
        {
            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select SQL,ConnName from S_UI_Pivot where ID='{0}'", id);
            var dt = baseSqlHelper.ExecuteDataTable(sql);
            if (dt.Rows[0]["SQL"].ToString().Trim() == "")
                throw new BusinessException("透视表SQL不能为空！");           

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(dt.Rows[0]["ConnName"].ToString());
            sql = string.Format("select * from ({0}) dt where 1=2", dt.Rows[0]["SQL"]);
            dt = sqlHelper.ExecuteDataTable(sql);

            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            foreach (DataColumn col in dt.Columns)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["Visible"] = "0";
                dic["FieldName"] = col.ColumnName;
                dic["Caption"] = col.ColumnName;               

                list.Add(dic);
            }
            return Json(list);
        }


    }
}
