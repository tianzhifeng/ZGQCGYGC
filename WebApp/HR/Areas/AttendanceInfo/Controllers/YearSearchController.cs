using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HR.Logic;
using HR.Logic.Domain;
using MvcAdapter;
using Config;
using Formula;
using System.Data;
using Base.Logic.Domain;
using Formula.Helper;

namespace HR.Areas.AttendanceInfo.Controllers
{
    public class YearSearchController : BaseController<R_W_AttendanceInfo>
    {
        public override ActionResult List()
        {
            var tab = new Tab();

            var yearCategory = CategoryFactory.GetYearCategory("Year", 5, 1, false);
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            yearCategory.Multi = false;
            tab.Categories.Add(yearCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            var year = System.DateTime.Now.Year;

            var yearqb = qb.Items.FirstOrDefault(a => a.Field == "Year");
            if (yearqb != null)
            {
                year = Convert.ToInt32(yearqb.Value);
                qb.Items.Remove(yearqb);
            }

            var enumServcie = FormulaHelper.GetService<IEnumService>();
            var columns = enumServcie.GetEnumDataSource("HR.AttendanceLegends");

            #region 初始化DataTable
            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("DeptName");
            table.Columns.Add("Workdays");
            table.Columns.Add("AnnualLeavedays");
            foreach (var column in columns)
                table.Columns.Add(column.Value);
            #endregion

            var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            var userList = baseEntities.Set<S_A_User>().Where(a => a.IsDeleted == "0").Where(qb).ToList();
            var dataList = entities.Set<R_W_AttendanceInfo>().Where(a => a.Year == year ).ToList();
            var annualLeaveList = entities.Set<S_W_AttendanceAnnualLeave>().Where(a => a.Year == year).ToList();

            AttendanceFO fo = FormulaHelper.CreateFO<AttendanceFO>();
            //List<DateTime?> shouldAttendanceDays = fo.GetShouldAttendanceDays(year);
            //var workdays = shouldAttendanceDays.Count;

            foreach (var user in userList)
            {
                var row = table.NewRow();
                row["ID"] = user.ID;
                row["Name"] = user.Name;
                row["DeptName"] = user.DeptName;
                row["Workdays"] = 0;// workdays;
                row["AnnualLeavedays"] = annualLeaveList.Where(a => a.UserID == user.ID).Sum(a => a.Days);
                table.Rows.Add(row);
                foreach (var column in columns)
                {
                    //Morning 存的考勤状态，MorningType存的当Morning为请假时的请假类别
                    var _am = dataList.Where(a => a.UserID == user.ID && (a.Morning == column.Value || a.MorningType == column.Value)).Count();
                    var _pm = dataList.Where(a => a.UserID == user.ID && (a.Afternoon == column.Value || a.AfternoonType == column.Value)).Count();
                    var _count = Convert.ToDouble(_am + _pm) * 0.5;
                    row[column.Value] = _count;
                }
            }

            var rtn = new Dictionary<string, object>();
            rtn["data"] = table;
            rtn["total"] = qb.TotolCount;
            rtn["year"] = year;
            return Json(rtn);
        }

    }
}
