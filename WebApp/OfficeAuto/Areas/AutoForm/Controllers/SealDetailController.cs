using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class SealDetailController : OfficeAutoFormContorllor<T_Seal_Apply>
    {
        public JsonResult GetApplyDetail(string id)
        {
            var sql = @"select * from T_Seal_Apply_Detail where T_Seal_ApplyID = '{0}'";
            var data = SQLDB.ExecuteDataTable(string.Format(sql, id));
            return Json(data);
        }

        public JsonResult GetBorrowDetail(string id)
        {
            var sql = @"select * from T_Seal_Borrow_Detail where T_Seal_BorrowID = '{0}'";
            var data = SQLDB.ExecuteDataTable(string.Format(sql, id));
            return Json(data);
        }
    }
}
