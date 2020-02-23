using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic.Domain;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class RoleDefineController : InfrastructureController<S_T_RoleDefine>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            return base.GetList(qb);
        }
    }
}
