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

namespace Comprehensive.Areas.Employee.Controllers
{
    public class AttendanceInfoController : ComprehensiveFormController<S_A_AttendanceInfo>
    {
        #region Excel导入

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var orgService = FormulaHelper.GetService<IOrgService>();
            var orgs = orgService.GetOrgs();
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "PersonName")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("姓名不能为空", e.Value);
                    }
                    var employee = this.ComprehensiveDbContext.Set<S_HR_Employee>().FirstOrDefault(a => a.Name == e.Value && a.IsDeleted == "0");
                    if (employee == null)
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("找不到姓名为【{0}】的员工", e.Value);
                    }
                    else
                    {
                        var user = baseEntities.Set<S_A_User>().FirstOrDefault(a => a.ID == employee.UserID);
                        if (user == null)
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("姓名为【{0}】的员工尚未生成用户信息！】", e.Value);
                        }
                    }
                }
                if (e.FieldName == "Date" && string.IsNullOrWhiteSpace(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("日期不能为空", e.Value);
                }
                if (e.FieldName == "Time" && string.IsNullOrWhiteSpace(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("时间不能为空", e.Value);
                }
            });

            return Json(errors);
        }

        public JsonResult SaveExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<S_A_AttendanceInfo>>(tempdata["data"]);
            var currentUser = FormulaHelper.GetUserInfo();
            var orgService = FormulaHelper.GetService<IOrgService>();
            var orgs = orgService.GetOrgs();

            var sdate = GetQueryString("AStartDate");
            if (sdate == "" || sdate == null) throw new Formula.Exceptions.BusinessValidationException("未选择开始时间，请重新操作！");
            var edate = GetQueryString("AEndDate");
            if (edate == "" || edate == null) throw new Formula.Exceptions.BusinessValidationException("未选择结束时间，请重新操作！");
            var startDate = DateTime.Parse(sdate);
            var endDate = DateTime.Parse(edate);

            var leave = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "请假").Name;
            var evection = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "出差").Name;
            var normal = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "正常").Name;
            var late = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "迟到").Name;
            var absence = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "缺勤").Name;
            var early = EnumBaseHelper.GetEnumDef("HR.AttendenceState").EnumItem.FirstOrDefault(a => a.Code == "早退").Name;

            #region 初始化所有人的考勤数据
            //根据日历和传入的起止日期添加所有人的考勤记录  都为缺勤
            //考勤列表
            var alist = this.ComprehensiveDbContext.Set<S_A_AttendanceInfo>().Where(a => a.Date.HasValue && a.Date.Value >= startDate && a.Date.Value <= endDate).ToList();
            foreach (var employee in this.ComprehensiveDbContext.Set<S_HR_Employee>().Where(a => a.IsDeleted == "0").ToList())
            {
                var thisDate = startDate;
                var user = baseEntities.Set<S_A_User>().FirstOrDefault(a => a.ID == employee.UserID);
                if (user == null) continue;
                while (thisDate <= endDate)
                {
                    var holiday = baseEntities.Set<S_C_Holiday>().FirstOrDefault(a => a.Date == thisDate);
                    //周末作为工作日或者正常工作日
                    //S_C_Holiday IsHoliday状态：0，周末设置为工作日；1：工作日设置为假日；2：工作日或周末设置为节日；
                    if ((holiday != null && holiday.IsHoliday == "0") ||
                        (holiday == null && thisDate.DayOfWeek != DayOfWeek.Saturday && thisDate.DayOfWeek != DayOfWeek.Sunday))
                    {
                        var attendanceInfo = alist.FirstOrDefault(a => a.Person == user.ID && a.Date == thisDate);
                        if (attendanceInfo == null)
                        {
                            attendanceInfo = new S_A_AttendanceInfo();
                            attendanceInfo.ID = FormulaHelper.CreateGuid();
                            EntityCreateLogic<S_A_AttendanceInfo>(attendanceInfo);
                            attendanceInfo.Person = user.ID;
                            attendanceInfo.PersonName = user.Name;
                            attendanceInfo.Dept = user.DeptID;
                            attendanceInfo.DeptName = user.DeptName;

                            attendanceInfo.Year = thisDate.Year;
                            attendanceInfo.Month = thisDate.Month;
                            attendanceInfo.Date = thisDate.Date;

                            attendanceInfo.Morning = absence;
                            attendanceInfo.Afternoon = absence;
                            this.ComprehensiveDbContext.Set<S_A_AttendanceInfo>().Add(attendanceInfo);
                            alist.Add(attendanceInfo);
                        }
                    }
                    thisDate = thisDate.AddDays(1);
                }
            }
            #endregion

            #region 录入考勤信息
            foreach (var attendance in list)
            {
                var employee = this.ComprehensiveDbContext.Set<S_HR_Employee>().FirstOrDefault(a => a.Name == attendance.PersonName && a.IsDeleted == "0");
                if (employee == null) continue;
                var attendanceInfo = alist.FirstOrDefault(a => a.Person == employee.UserID && a.Date == attendance.Date);
                if (attendanceInfo != null)
                {
                    var attendenceTime = DateTime.Parse(DateTime.Parse(attendance.Date.ToString()).ToShortDateString() + " "
                        + DateTime.Parse(attendance.Time.ToString()).ToLongTimeString());

                    #region 判断考勤状态的时间
                    var noonTime = DateTime.Parse(DateTime.Parse(attendance.Date.ToString()).ToShortDateString() + " 12:00:00");
                    var nineTime = DateTime.Parse(DateTime.Parse(attendance.Date.ToString()).ToShortDateString() + " 9:05:00");
                    var tenTime = DateTime.Parse(DateTime.Parse(attendance.Date.ToString()).ToShortDateString() + " 10:30:00");
                    var sixteenTime = DateTime.Parse(DateTime.Parse(attendance.Date.ToString()).ToShortDateString() + " 16:00:00");
                    var eighteenTime = DateTime.Parse(DateTime.Parse(attendance.Date.ToString()).ToShortDateString() + " 18:00:00");
                    #endregion

                    //9:05后算迟到，10：30后算缺勤半天，18:00前算早退，16:00前算缺勤半天
                    if (attendenceTime < noonTime)
                    {
                        //上午
                        if (attendanceInfo.PostTime == null)
                        {
                            attendanceInfo.PostTime = attendenceTime;
                        }
                        if (attendanceInfo.Morning != leave && attendanceInfo.Morning != evection)
                        {
                            if (attendenceTime <= DateTime.Parse(attendanceInfo.PostTime.ToString()))
                            {
                                attendanceInfo.PostTime = attendenceTime;
                                attendanceInfo.Morning = normal;
                                if (attendenceTime > nineTime)
                                {
                                    attendanceInfo.Morning = late;
                                    if (attendenceTime > tenTime)
                                    {
                                        attendanceInfo.Morning = absence;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //下午
                        if (attendanceInfo.LeaveTime == null)
                        {
                            attendanceInfo.LeaveTime = attendenceTime;
                        }
                        if (attendanceInfo.Afternoon != leave && attendanceInfo.Afternoon != evection)
                        {
                            if (attendenceTime >= DateTime.Parse(attendanceInfo.LeaveTime.ToString()))
                            {
                                attendanceInfo.LeaveTime = attendenceTime;
                                attendanceInfo.Afternoon = normal;
                                if (attendenceTime < eighteenTime)
                                {
                                    attendanceInfo.Afternoon = early;
                                    if (attendenceTime < sixteenTime)
                                    {
                                        attendanceInfo.Afternoon = absence;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.ComprehensiveDbContext.SaveChanges();
            #endregion

            return Json("Success");
        }
        #endregion
    }
}
