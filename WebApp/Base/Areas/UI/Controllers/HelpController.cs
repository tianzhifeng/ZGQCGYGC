using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula;
using MvcAdapter;
using Formula.Helper;

namespace Base.Areas.UI.Controllers
{
    public class HelpController : BaseController
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
            var list = entities.Set<S_UI_Help>().Where(qb).Select(c => new { ID = c.ID, Name = c.Name,  Url = c.Url, ModifyTime = c.ModifyTime });
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
            return JsonGetModel<S_UI_Help>(id);
        }
       
        [ValidateInput(false)]
        public JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_Help>();
            if (entities.Set<S_UI_Help>().Where(c => c.Url == entity.Url && c.ID != entity.ID).Count() > 0)
                throw new Exception("帮助页Url必须唯一！");
            return JsonSave<S_UI_Help>();
        }

        public JsonResult Delete()
        {
            return JsonDelete<S_UI_Help>(Request["ListIDs"]);
        }

        #endregion
    }
}
