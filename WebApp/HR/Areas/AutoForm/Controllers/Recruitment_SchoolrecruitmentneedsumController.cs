using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using HR.Logic.Domain;
using Base.Logic.Domain;
using Formula.Helper;
using Config;
using Config.Logic;
using MvcAdapter;

namespace HR.Areas.AutoForm.Controllers
{
    public class Recruitment_SchoolrecruitmentneedsumController : HRFormContorllor<T_Recruitment_Schoolrecruitmentneedsum>
    {
        // GET: /AutoForm/Recruitment_Schoolrecruitmentneedsum/

        public ActionResult Recruitment_SchoolrecruitmentneedSummaryList()
        {
            var year = DateTime.Now.Year;

            var sql = @"SELECT Dept, DeptName FROM S_E_Recruitment_DeptRequirementReport where [Year] = " + year + @" GROUP BY Dept, DeptName";

            var dt = HRSQLDB.ExecuteDataTable(sql);

            ViewBag.deptDataTable = dt;

            return View();
        }

        public JsonResult GetSchoolrecruitmentneedsumList(QueryBuilder qb)
        {
            var year = DateTime.Now.Year;
            //专业
            var sql = "SELECT Major FROM S_E_Recruitment_DeptRequirementReport where Year = " + year + @" group by major";
            var dt = HRSQLDB.ExecuteDataTable(sql);
            //部门
            var deptsql = "select Dept from S_E_Recruitment_DeptRequirementReport where Year =" + year + " group by Dept";
            var deptdt = HRSQLDB.ExecuteDataTable(deptsql);

            var sumsql = "select Dept,Major,sum(Number) as Number from S_E_Recruitment_DeptRequirementReport where Year =" + year + "  group by Dept,Major";
            var sumdt = HRSQLDB.ExecuteDataTable(sumsql);

            foreach (DataRow dr in deptdt.Rows)
            {
                if (!dt.Columns.Contains(dr["Dept"].ToString()))
                {
                    dt.Columns.Add(dr["Dept"].ToString());
                }
            }

            var result = new List<Dictionary<string, object>>();
            var json = JsonHelper.ToJson(dt);
            result = JsonHelper.ToList(json);
            foreach (var item in result)
            {
                foreach (DataRow dr in deptdt.Rows)
                {
                    var dept = dr["Dept"].ToString();
                    var Number = sumdt.Compute("sum(Number)", " Major='" + item.GetValue("Major") + "' and Dept='" + dept + "' ");
                    item.SetValue(dept, Number == DBNull.Value ? 0 : Number);
                }

            }
            var dataGrid = new GridData(result);
            dataGrid.total = qb.TotolCount;
            return Json(dataGrid);
        }

    }
}
