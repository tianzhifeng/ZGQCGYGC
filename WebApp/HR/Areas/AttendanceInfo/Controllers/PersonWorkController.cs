using Formula;
using HR.Logic;
using HR.Logic.Domain;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HR.Areas.AttendanceInfo.Controllers
{
    public class PersonWorkController : BaseController<V_S_W_AttendanceInfo>
    {
        public ActionResult AttendanceList()
        {
            var tab = new Tab();

            var yearCategory = CategoryFactory.GetYearCategory("Year", 5, 1, false);
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            yearCategory.Multi = false;
            tab.Categories.Add(yearCategory);

            var monthCategory = CategoryFactory.GetMonthCategory("Month", false);
            monthCategory.SetDefaultItem(DateTime.Now.Month.ToString());
            monthCategory.Multi = false;
            tab.Categories.Add(monthCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public ActionResult MonthList()
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
            var user = FormulaHelper.GetUserInfo();
            var year = int.Parse(qb.Items.FirstOrDefault(a => a.Field == "Year").Value.ToString());
            var month = int.Parse(qb.Items.FirstOrDefault(a => a.Field == "Month").Value.ToString());
            DateTime? monthStartDay = Convert.ToDateTime(year + "-" + month.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(2, '0') + "-01");
            DateTime? monthEndDay = Convert.ToDateTime(monthStartDay.Value.AddMonths(1).AddDays(-1).ToShortDateString() + " 23:59:59");
            var list = entities.Set<V_S_W_AttendanceInfo>().Where(a => a.UserID == user.UserID && a.WorkDay >= monthStartDay && a.WorkDay <= monthEndDay).OrderBy(a => a.WorkDay).ToList();
            var days = list.Select(a => a.WorkDay).Distinct().ToList();

            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("Year");
            table.Columns.Add("Month");
            table.Columns.Add("Date");
            table.Columns.Add("SignIn");
            table.Columns.Add("SignOut");

            foreach (var item in days)
            {
                var date = (DateTime)item;
                var row = table.NewRow();
                row["ID"] = date.ToString();
                row["Year"] = date.Year;
                row["Month"] = date.Month;
                row["Date"] = date.ToShortDateString();
                var signIn = list.Where(a => a.WorkDay == date && a.CheckType == "SignIn").OrderBy(a => a.CheckDate).FirstOrDefault();
                if (signIn != null)
                    row["SignIn"] = ((DateTime)signIn.CheckDate).ToShortTimeString();
                var signOut = list.Where(a => a.WorkDay == date && a.CheckType == "SignOut").OrderByDescending(a => a.CheckDate).FirstOrDefault();
                if (signOut != null)
                {
                    row["SignOut"] = ((DateTime)signOut.CheckDate).ToShortTimeString();
                    if (((DateTime)signOut.CheckDate).Date != signOut.WorkDay)
                        row["SignOut"] += "（" + ((DateTime)signOut.CheckDate).ToString("MM-dd") + "）";
                }
                table.Rows.Add(row);
            }
            return Json(table);
        }

        public JsonResult GetMonthList(QueryBuilder qb)
        {
            var user = FormulaHelper.GetUserInfo();
            var year = int.Parse(qb.Items.FirstOrDefault(a => a.Field == "Year").Value.ToString());
            var enumServcie = FormulaHelper.GetService<IEnumService>();
            var columns = enumServcie.GetEnumDataSource("HR.AttendanceLegends");

            #region 初始化DataTable
            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("Year");
            table.Columns.Add("Month");
            table.Columns.Add("Workdays");
            foreach (var column in columns)
                table.Columns.Add(column.Value);
            #endregion

            AttendanceFO fo = FormulaHelper.CreateFO<AttendanceFO>();
            var dataList = entities.Set<R_W_AttendanceInfo>().Where(a => a.Year == year && a.UserID == user.UserID).ToList();

            for (int i = 1; i <= 12; i++)
            {
                List<DateTime?> shouldAttendanceDays = fo.GetShouldAttendanceDays(year, i);
                var workdays = shouldAttendanceDays.Count;
                var row = table.NewRow();
                row["ID"] = i;
                row["Year"] = year;
                row["Month"] = i;
                row["Workdays"] = workdays;
                table.Rows.Add(row);
                foreach (var column in columns)
                {
                    //Morning 存的考勤状态，MorningType存的当Morning为请假时的请假类别
                    var _am = dataList.Where(a => a.Month == i && (a.Morning == column.Value || a.MorningType == column.Value)).Count();
                    var _pm = dataList.Where(a => a.Month == i && (a.Afternoon == column.Value || a.AfternoonType == column.Value)).Count();
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
