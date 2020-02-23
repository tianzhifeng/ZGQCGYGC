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

namespace Comprehensive.Areas.Employee.Controllers
{
    public class EmployeeController : BaseReportController
    {
        protected override string title
        {
            get { return "雇员报表"; }
        }

        private SQLHelper _sqlHelper = null;
        protected override SQLHelper sqlHelper
        {
            get
            {
                if (_sqlHelper == null)
                {
                    _sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Comprehensive);
                }
                return _sqlHelper;
            }
        }

        protected override string mainTable
        {
            get { return "S_HR_Employee"; }
        }

        protected override string fk
        {
            get { return "EmployeeID"; }
        }
    }
}
