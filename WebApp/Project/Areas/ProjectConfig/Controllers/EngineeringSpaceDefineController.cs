using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class EngineeringSpaceDefineController : BaseConfigController<S_T_EngineeringSpace>
    {
        protected override void BeforeSave(S_T_EngineeringSpace entity, bool isNew)
        {
            entity.Save();
        }

        public JsonResult Publish(string code)
        {
            CacheHelper.Remove(CommonConst.enginnerSpaceCacheKey + code);
            return Json("");
        }

    }
}
