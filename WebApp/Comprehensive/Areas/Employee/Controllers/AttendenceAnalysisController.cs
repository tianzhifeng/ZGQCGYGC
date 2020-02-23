using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using Formula.Helper;
using MvcAdapter;
using Formula;
using System.Drawing.Imaging;
using System.Drawing;
using Base.Logic.Domain;
using System.Web.Security;
using Comprehensive.Logic.BusinessFacade;
using Newtonsoft.Json;
using Formula.ImportExport;
using System.Data;

namespace Comprehensive.Areas.Employee.Controllers
{
    public class AttendenceAnalysisController : ComprehensiveFormController<S_A_AttendanceInfo>
    {
        public ActionResult List()
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

        public JsonResult GetList(QueryBuilder qb)
        {
            var qbItems = qb.Items;
            var year = System.DateTime.Now.Year;
            var month = System.DateTime.Now.Month;
            for (int i = qbItems.Count - 1; i >= 0; i--)
            {
                var qbItem = qbItems[i];
                if (qbItem.Field == "Year")
                {
                    year = Convert.ToInt32(qbItem.Value.ToString());
                    qb.Items.Remove(qbItem);
                }
                if (qbItem.Field == "Month")
                {
                    month = Convert.ToInt32(qbItem.Value.ToString());
                    qb.Items.Remove(qbItem);
                }
            }
            var daysInMonth = DateTime.DaysInMonth(year, month);

            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("UserID");
            table.Columns.Add("Code");
            table.Columns.Add("Name");
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day).ToString("yyyy-MM-dd");
                table.Columns.Add(date + "_Morning");
                table.Columns.Add(date + "_Afternoon");
            }
            table.Columns.Add("SumNormal");
            table.Columns.Add("SumLate");
            table.Columns.Add("SumEarly");
            table.Columns.Add("SumAbsence");
            table.Columns.Add("SumEvection");
            table.Columns.Add("SumLeave");

            var thisMonthList = this.ComprehensiveDbContext.Set<S_A_AttendanceInfo>().Where(a => a.Year == year && a.Month == month).ToList();
            var leave = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "请假").Name;
            var evection = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "出差").Name;
            var normal = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "正常").Name;
            var late = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "迟到").Name;
            var absence = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "缺勤").Name;
            var early = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "早退").Name;
            var emplist = this.ComprehensiveDbContext.Set<S_HR_Employee>().Where(a => a.IsDeleted == "0").Where(qb).ToList();
            foreach (var employee in emplist)
            {
                var row = table.NewRow();
                row["ID"] = employee.ID;
                row["UserID"] = employee.UserID;
                row["Code"] = employee.Code;
                row["Name"] = employee.Name;
                if (thisMonthList.Count > 0)
                {
                    var thisEmployeeList = thisMonthList.Where(a => a.Person == employee.UserID);
                    foreach (var attendence in thisEmployeeList)
                    {
                        var date = DateTime.Parse(attendence.Date.ToString()).ToString("yyyy-MM-dd");
                        row[date + "_Morning"] = attendence.Morning;
                        row[date + "_Afternoon"] = attendence.Afternoon;
                    }
                    row["SumNormal"] = (thisEmployeeList.Count(a => a.Morning == normal) + thisEmployeeList.Count(a => a.Afternoon == normal)) / 2.0;
                    row["SumLate"] = (thisEmployeeList.Count(a => a.Morning == late) + thisEmployeeList.Count(a => a.Afternoon == late)) / 2.0;
                    row["SumEarly"] = (thisEmployeeList.Count(a => a.Morning == early) + thisEmployeeList.Count(a => a.Afternoon == early)) / 2.0;
                    row["SumAbsence"] = (thisEmployeeList.Count(a => a.Morning == absence) + thisEmployeeList.Count(a => a.Afternoon == absence)) / 2.0;
                    row["SumEvection"] = (thisEmployeeList.Count(a => a.Morning == evection) + thisEmployeeList.Count(a => a.Afternoon == evection)) / 2.0;
                    row["SumLeave"] = (thisEmployeeList.Count(a => a.Morning == leave) + thisEmployeeList.Count(a => a.Afternoon == leave)) / 2.0;
                }
                table.Rows.Add(row);
            }

            var rtn = new Dictionary<string, object>();
            rtn["data"] = table;
            rtn["total"] = qb.TotolCount;
            rtn["year"] = year;
            rtn["month"] = month;
            rtn["daysInMonth"] = daysInMonth;
            return Json(rtn);
        }
    }
}
