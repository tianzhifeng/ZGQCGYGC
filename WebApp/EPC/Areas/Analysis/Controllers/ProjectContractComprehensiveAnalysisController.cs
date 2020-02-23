using Config;
using EPC.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Analysis.Controllers
{
    public class ProjectContractComprehensiveAnalysisController : EPCFormContorllor<E_PC_ComprehensiveAnalysisConfig>
    {

        public JsonResult GetColumnConfigTree()
        {

            SQLHelper epcSQLHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            var dataTable = epcSQLHelper.ExecuteDataTable("select ID,ParentID,Name,Level from E_PC_ComprehensiveAnalysisConfig");
            return Json(dataTable);

        }

    }
}
