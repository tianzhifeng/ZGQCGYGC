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
using Formula.Helper;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class DesignInputDefController : BaseConfigController<S_D_DesignInputDefine>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            return base.GetList(qb);
        }

        public override JsonResult SaveList()
        {
            var list = this.UpdateList<S_D_DesignInputDefine>();
            var enumService = FormulaHelper.GetService<IEnumService>();
            var classEnum = EnumBaseHelper.GetEnumDef("Project.DataSource");
            if (classEnum == null) { throw new Formula.Exceptions.BusinessException("枚举【Project.DataSource】已经被删除，无法确认资料分类"); }
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
