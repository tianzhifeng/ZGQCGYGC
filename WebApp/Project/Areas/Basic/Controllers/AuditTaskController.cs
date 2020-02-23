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
using Config.Logic;

namespace Project.Areas.Basic.Controllers
{
    public class AuditTaskController : ProjectController
    {
        public JsonResult GetList(QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(GetQueryString("ProjectInfoID")))
                qb.Add("ProjectInfoID", QueryMethod.Equal, GetQueryString("ProjectInfoID"));
            string sql = @"select DisplayName,CreateDate,LinkUrl,ID,WBSID,BusniessID,ProjectInfoID from S_W_Activity Where ActivityKey in ('Collact','Audit','Approve') and (OwnerUserID  like '%{0}%') And (State='Create') ORDER BY ID DESC";
            sql = string.Format(sql, FormulaHelper.UserID);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Project");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

    }
}
