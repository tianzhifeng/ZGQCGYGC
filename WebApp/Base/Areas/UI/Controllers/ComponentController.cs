using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula.Helper;

namespace Base.Areas.UI.Controllers
{
    public class ComponentController : BaseController<S_UI_Component>
    {
        #region 基本信息

        public override ActionResult Edit()
        {
            var list = entities.Set<S_M_Category>().OrderBy(c => c.FullID).ToList();
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i];
                if (item.FullID == item.ID)
                {
                    list.Remove(item);
                    continue;
                }
                item.FullID = item.FullID.Substring(item.FullID.IndexOf('.') + 1);
                if (item.FullID.Contains('.'))
                {
                    string sId = item.FullID.Split('.').First();
                    item.Name = list.SingleOrDefault(c => c.ID == sId).Name + "." + item.Name;
                }

            }

            ViewBag.EnumCategory = JsonHelper.ToJson(list.Select(c => new { value = c.ID, text = c.Name }));


            return View();
        }

        #endregion

    }
}
