using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MonthWorkController : BaseController<R_W_AttendanceInfo>
    {
        public ActionResult PersonMonthTab()
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

        #region 个人月份明细数据

        public JsonResult GetPersonMonthDetail(QueryBuilder qb, string Year, string Month)
        {
            var year = System.DateTime.Now.Year;
            if (!string.IsNullOrEmpty(Year))
                year = Convert.ToInt32(Year);
            var month = System.DateTime.Now.Month;
            if (!string.IsNullOrEmpty(Month))
                month = Convert.ToInt32(Month);

            #region 初始化DataTable
            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("DeptName");
            table.Columns.Add("DateType");
            //自然日
            var daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day).ToString("yyyy-MM-dd");
                table.Columns.Add(date);
                table.Columns.Add(date + "_ID");
            }
            #endregion

            var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            var userList = baseEntities.Set<S_A_User>().Where(a => a.IsDeleted == "0").Where(qb).ToList();
            var dataList = entities.Set<R_W_AttendanceInfo>().Where(a => a.Year == year && a.Month == month).ToList();
            foreach (var user in userList)
            {
                var row_am = table.NewRow();
                row_am["ID"] = user.ID;
                row_am["Name"] = user.Name;
                row_am["DeptName"] = user.DeptName;
                row_am["DateType"] = "上午";
                table.Rows.Add(row_am);
                var row_pm = table.NewRow();
                row_pm["ID"] = user.ID;
                row_pm["Name"] = user.Name;
                row_pm["DeptName"] = user.DeptName;
                row_pm["DateType"] = "下午";
                table.Rows.Add(row_pm);
                var userDataList = dataList.Where(a => a.UserID == user.ID);
                foreach (var data in userDataList)
                {
                    var date = DateTime.Parse(data.Date.ToString()).ToString("yyyy-MM-dd");
                    row_am[date] = data.Morning;
                    row_am[date + "_ID"] = data.ID;
                    if (data.Morning == AttendenceState.Leave.ToString())
                        row_am[date] = data.MorningType;
                    row_pm[date] = data.Afternoon;
                    row_pm[date + "_ID"] = data.ID;
                    if (data.Afternoon == AttendenceState.Leave.ToString())
                        row_pm[date] = data.AfternoonType;
                }
            }

            var rtn = new Dictionary<string, object>();
            rtn["data"] = table;
            rtn["total"] = qb.TotolCount;
            rtn["year"] = year;
            rtn["month"] = month;
            rtn["daysInMonth"] = daysInMonth;
            return Json(rtn);
        }

        #endregion

        #region 个人月份汇总数据
        public JsonResult GetPersonMonthList(QueryBuilder qb, string Year, string Month)
        {
            var year = System.DateTime.Now.Year;
            if (!string.IsNullOrEmpty(Year))
                year = Convert.ToInt32(Year);
            var month = System.DateTime.Now.Month;
            if (!string.IsNullOrEmpty(Month))
                month = Convert.ToInt32(Month);
            var enumServcie = FormulaHelper.GetService<IEnumService>();
            var columns = enumServcie.GetEnumDataSource("HR.AttendanceLegends");

            #region 初始化DataTable
            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("DeptName");
            table.Columns.Add("Workdays");
            foreach (var column in columns)
                table.Columns.Add(column.Value);
            #endregion

            var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            var userList = baseEntities.Set<S_A_User>().Where(a => a.IsDeleted == "0").Where(qb).ToList();
            var dataList = entities.Set<R_W_AttendanceInfo>().Where(a => a.Year == year && a.Month == month).ToList();
            //需要只显示入职以后的考勤数据
            var employeeList = this.entities.Set<T_Employee>().Where(a => a.IsDeleted == "0").ToList();

            AttendanceFO fo = FormulaHelper.CreateFO<AttendanceFO>();
            List<DateTime?> shouldAttendanceDays = fo.GetShouldAttendanceDays(year, month);
            var workdays = shouldAttendanceDays.Count;

            foreach (var user in userList)
            {
                //需要只显示入职以后的考勤数据
                var employee = employeeList.FirstOrDefault(a => a.UserID == user.ID);
                DateTime? joinDate = null;//入职时间
                if (employee != null && employee.JoinCompanyDate.HasValue)
                    joinDate = employee.JoinCompanyDate.Value;
                if (joinDate.HasValue)
                    workdays = shouldAttendanceDays.Where(a => a.Value >= joinDate.Value).Count();
                var row = table.NewRow();
                row["ID"] = user.ID;
                row["Name"] = user.Name;
                row["DeptName"] = user.DeptName;
                row["Workdays"] = workdays;
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
            rtn["month"] = month;
            return Json(rtn);
        }

        #endregion
    }
}

