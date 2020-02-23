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
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Formula.Helper;

using System.Reflection;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class ISOController : BaseConfigController<S_T_ISODefine>
    {
        protected override void BeforeSave(S_T_ISODefine entity, bool isNew)
        {
            if (isNew) {
                if (String.IsNullOrEmpty(entity.CanAddNewForm))
                    entity.CanAddNewForm = false.ToString();
            }
        }
    }
}
