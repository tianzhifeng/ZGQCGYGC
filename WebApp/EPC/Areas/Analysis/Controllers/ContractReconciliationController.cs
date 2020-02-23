using Config;
using EPC.Logic.Domain;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Analysis.Controllers
{
    public class ContractReconciliationController : EPCFormContorllor<S_M_ContractInfo>
    {
        public JsonResult GetContractList(QueryBuilder  qb)
        {
            string sql = "select * from S_M_ContractInfo";
            SQLHelper epcSQLHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            DataTable dt = epcSQLHelper.ExecuteDataTable(sql);
           // GridData gridData = epcSQLHelper.ExecuteGridData(sql, qb);
            return Json(dt);

        }

    }
}
