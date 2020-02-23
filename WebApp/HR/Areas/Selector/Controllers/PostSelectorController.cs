using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using MvcAdapter;

namespace HR.Areas.Selector.Controllers
{
    public class PostSelectorController : HRController
    {

        public JsonResult GetPostList(QueryBuilder qb)
        {
            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @" select S_M_EnumItem.* from S_M_EnumItem
left join S_M_EnumDef on S_M_EnumItem.EnumDefID = S_M_EnumDef.ID
where S_M_EnumDef.Code='HR.PersonPost'
 order by SortIndex";
            GridData grid = baseSqlHelper.ExecuteGridData(sql, qb);
            return Json(grid);
        }

    }
}
