﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula;

namespace Base.Areas.PortalBlock.Controllers
{
    public class ShortCutController : BaseController<S_H_ShortCut>
    {
        //
        // GET: /PortalBlock/ShortCut/

        public void Add()
        {
            var user = FormulaHelper.GetUserInfo();

            S_H_ShortCut model = UpdateEntity<S_H_ShortCut>(string.Empty);

            if (entities.Set<S_H_ShortCut>().Count(c => c.Url == model.Url && c.CreateUserID == user.UserID) == 0)
            {
                entities.Set<S_H_ShortCut>().Add(model);
                entities.SaveChanges();
            }
        }

        public JsonResult Select(string count)
        {
            int iCount = 5;
            if (!string.IsNullOrEmpty(count))
                iCount = Convert.ToInt32(count);
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;
            List<S_H_ShortCut> list = entities.Set<S_H_ShortCut>().Where(s => s.CreateUserID == userID && s.IsUse == "T").OrderByDescending(t => t.CreateTime).Take(iCount).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public void DeleteShortCut()
        {
            string id = Request["ID"];

            if (!string.IsNullOrEmpty(id))
            {
                entities.Set<S_H_ShortCut>().Delete(s => s.ID == id);
                entities.SaveChanges();
            }
        }

        public JsonResult SortShortCut(string pageIndex, string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                string[] arr = ids.Split(',');
                List<S_H_ShortCut> list = entities.Set<S_H_ShortCut>().Where(c => arr.Contains(c.ID)).ToList();
                for (int i = 0; i < arr.Length; i++)
                {
                    string id = arr[i];
                    S_H_ShortCut model = list.Find(c => c.ID == id);
                    if (model != null)
                    {
                        model.PageIndex = Convert.ToInt32(pageIndex);
                        model.SortIndex = i;
                    }
                }
                entities.SaveChanges();
            }
            return Json(string.Empty);
        }
    }
}
