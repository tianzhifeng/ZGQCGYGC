using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using Formula.Helper;
using System.Text;
using System.Data;
using Formula.ImportExport;
using MvcAdapter.ImportExport;
using Newtonsoft.Json;
using System.IO;
using Aspose.Cells;
using Formula.Exceptions;

namespace HR.Areas.Search.Controllers
{
    public class DeptController : BaseReportController
    {
        protected override string title
        {
            get { return "部门报表"; }
        }

        SQLHelper _sqlHelper = null;
        protected override SQLHelper sqlHelper
        {
            get
            {
                if (_sqlHelper == null)
                {
                    _sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.HR);
                }

                return _sqlHelper;
            }
        }

        protected override string mainTable
        {
            get { return "(select distinct DeptID as ID,DeptName from T_Employee) T_Dept"; }
        }

        protected override string fk
        {
            get { return "fkDeptID"; }
        }
    }
}
