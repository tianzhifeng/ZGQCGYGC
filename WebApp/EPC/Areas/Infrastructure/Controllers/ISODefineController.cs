using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Formula;
using Config;
using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic;
using EPC.Logic.Domain;


namespace EPC.Areas.Infrastructure.Controllers
{
    public class ISODefineController : InfrastructureController<S_T_ISODefine>
    {
        protected override void BeforeSave(S_T_ISODefine entity, bool isNew)
        {
            if (isNew)
            {
                if (String.IsNullOrEmpty(entity.CanAddNewForm))
                    entity.CanAddNewForm = false.ToString();
            }
        }
    }
}
