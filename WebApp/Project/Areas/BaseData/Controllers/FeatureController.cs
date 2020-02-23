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
using Config;

namespace Project.Areas.BaseData.Controllers
{
    public class FeatureController : BaseConfigController<S_D_Feature>
    {
        protected override void BeforeSave(S_D_Feature entity, bool isNew)
        {
            if(!isNew)
                entity.Save();
        }

        protected override void BeforeDelete(List<S_D_Feature> entityList)
        {
            foreach (var item in entityList)
                item.Delete(true);
        }
    }
}
