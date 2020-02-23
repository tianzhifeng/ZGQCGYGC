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
using EPC.Logic;
using Formula;
using System.Text;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class DesignInputDefController : InfrastructureController<S_T_DesignInputDefine>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            return base.GetList(qb);
        }

        public override JsonResult SaveList()
        {
            var list = this.UpdateList<S_T_DesignInputDefine>();
            var enumService = FormulaHelper.GetService<IEnumService>();
            var classEnum = EnumBaseHelper.GetEnumDef("Base.DesignInpuClass");
            if (classEnum == null) { throw new Formula.Exceptions.BusinessValidationException("枚举【Base.DesignInpuClass】已经被删除，无法确认资料分类"); }
            foreach (var define in list)
            {
                var enumItem = classEnum.EnumItem.FirstOrDefault(d => d.Code == define.Class);
                if (enumItem != null)
                    define.InputTypeIndex = Convert.ToInt32(enumItem.SortIndex);
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
