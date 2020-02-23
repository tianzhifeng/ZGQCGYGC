using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula;
using Base.Logic.Domain;
using Formula.Helper;
using MvcAdapter;

namespace Base.Areas.UI.Controllers
{
    public class FreePivotController : BaseController
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
            var list = entities.Set<S_UI_FreePivot>().Where(qb).Select(c => new { ID = c.ID, Code = c.Code, Name = c.Name, ConnName = c.ConnName, ModifyTime = c.ModifyTime, Description = c.Description });
            GridData data = new GridData(list);
            data.total = qb.TotolCount;

            return Json(data);
        }

        #endregion

        #region 基本信息

        public ActionResult Edit()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        public JsonResult GetModel(string id)
        {
            return JsonGetModel<S_UI_FreePivot>(id);
        }

        [ValidateInput(false)]
        public JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_FreePivot>();
            if (entities.Set<S_UI_FreePivot>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception("编号不能重复！");
            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == entity.CategoryID);
            entity.ConnName = category.Code;
            return JsonSave<S_UI_FreePivot>(entity);
        }

        public JsonResult Delete()
        {
            return JsonDelete<S_UI_FreePivot>(Request["ListIDs"]);
        }

        #endregion

        #region 克隆

        public JsonResult Clone(string cloneID)
        {
            var info = entities.Set<S_UI_FreePivot>().SingleOrDefault(c => c.ID == cloneID);
            var newInfo = new S_UI_FreePivot();
            FormulaHelper.UpdateModel(newInfo, info);

            newInfo.ID = FormulaHelper.CreateGuid();
            newInfo.Code += "copy";
            newInfo.Name += "(副本)";
            newInfo.ModifyTime = null;
            newInfo.ModifyUserID = "";
            newInfo.ModifyUserName = "";
            entities.Set<S_UI_FreePivot>().Add(newInfo);
            entities.SaveChanges();
            return Json("");
        }


        #endregion
    }
}
