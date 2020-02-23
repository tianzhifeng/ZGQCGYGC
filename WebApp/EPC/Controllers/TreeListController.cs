using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Base.Logic.BusinessFacade;
using Formula;
using MvcAdapter;
using Base.Logic.Domain;
using Config;
using Config.Logic;
using System.Text;
using System.IO;
using System.Data;
using Workflow.Logic.BusinessFacade;
using Formula.Helper;
using Formula.Exceptions;
using EPC.Logic.Domain;

namespace EPC.Controllers
{
    public class TreeListController : BaseController
    {
        public ActionResult PageView(string tmplCode)
        {
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            ViewBag.ListHtml = uiFO.CreateListHtml(tmplCode);


            ViewBag.Script = uiFO.CreateListScript(tmplCode);
            ViewBag.FixedFields = string.Format("var FixedFields={0};", Newtonsoft.Json.JsonConvert.SerializeObject(uiFO.GetFixedWidthFields(tmplCode)));
            ViewBag.Title = listDef.Name;

            #region TAB查询
            var fields = JsonHelper.ToList(listDef.LayoutField);
            var tab = new Tab();
            foreach (var field in fields)
            {
                if (!field.ContainsKey("Settings"))
                    continue;
                var settings = JsonHelper.ToObject(field["Settings"].ToString());

                if (!settings.ContainsKey("EnumKey") || settings["EnumKey"].ToString() == "")
                    continue;

                if (!settings.ContainsKey("TabSearchName") || settings["TabSearchName"].ToString() == "")
                    continue;


                Category category = null;
                var hasAllAttr = true;
                string enumKey = settings["EnumKey"].ToString();

                if (settings.ContainsKey("ShowAll") && settings["ShowAll"].ToString() == "false"
                    && settings.ContainsKey("TabSearchDefault") && !String.IsNullOrEmpty(settings["TabSearchDefault"].ToString()))
                {
                    hasAllAttr = false;
                }

                if (enumKey.StartsWith("["))
                    category = CategoryFactory.GetCategoryByString(enumKey, settings["TabSearchName"].ToString(), field["field"].ToString(), hasAllAttr);
                else
                    category = CategoryFactory.GetCategory(enumKey, settings["TabSearchName"].ToString(), field["field"].ToString(), hasAllAttr);

                //默认值
                if (settings.ContainsKey("TabSearchDefault") && settings["TabSearchDefault"].ToString() != "")
                    category.SetDefaultItem(settings["TabSearchDefault"].ToString());
                else
                    category.SetDefaultItem();

                //多选或单选
                if (settings.ContainsKey("TabSearchMulti") && settings["TabSearchMulti"].ToString() == "false")
                    category.Multi = false;
                else
                    category.Multi = true;
                tab.Categories.Add(category);
            }
            if (tab.Categories.Count > 0)
            {
                tab.IsDisplay = true;
                ViewBag.Tab = tab;
                ViewBag.Layout = "~/Views/Shared/_AutoListLayoutTab.cshtml";
            }
            else
            {
                ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            }
            #endregion
            return View();
        }

        protected override System.Data.Entity.DbContext entities
        {
            get {
                return FormulaHelper.GetEntities<EPCEntities>();
            }
        }

    }
}
