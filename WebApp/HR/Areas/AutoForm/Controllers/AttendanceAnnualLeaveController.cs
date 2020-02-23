using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HR.Logic.Domain;
using Config.Logic;
using Base.Logic.Domain;
using Formula;
using Newtonsoft.Json;
using Formula.ImportExport;

namespace HR.Areas.AutoForm.Controllers
{
    public class AttendanceAnnualLeaveController : HRFormContorllor<S_W_AttendanceAnnualLeave>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var user = dic.GetValue("UserID");
            var year = Convert.ToInt32(dic.GetValue("Year"));
            var id = dic.GetValue("ID");
            var exsit = this.BusinessEntities.Set<S_W_AttendanceAnnualLeave>().FirstOrDefault(a => a.UserID == user && a.Year == year && a.ID != id);
            if (exsit != null)
                throw new Formula.Exceptions.BusinessException("【" + exsit.UserIDName + "】已经存在【" + year + "】年的年假数据");
            base.BeforeSave(dic, formInfo, isNew);
        }

        public JsonResult CreateYearData(int Year)
        {
            var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            var userlist = baseEntities.Set<S_A_User>().Where(a => a.IsDeleted == "0").ToList();
            var empList = this.BusinessEntities.Set<T_Employee>().Where(a => a.IsDeleted == "0").ToList();
            var dataList = this.BusinessEntities.Set<S_W_AttendanceAnnualLeave>().Where(a => a.Year == Year).ToList();
            var YearDate = new DateTime(Year, 12, 31);//以当年的最后一天减去参加工作年份作为计算依据
            foreach (var user in userlist)
            {
                var emp = empList.FirstOrDefault(a => a.UserID == user.ID);
                if (emp == null)
                    continue;
                var data = dataList.FirstOrDefault(a => a.UserID == user.ID);
                if (data == null)
                {
                    data = new S_W_AttendanceAnnualLeave();
                    data.CreateDate = DateTime.Now;
                    data.CreateUser = this.CurrentUserInfo.UserName;
                    data.CreateUserID = this.CurrentUserInfo.UserID;
                    data.OrgID = this.CurrentUserInfo.UserOrgID;
                    data.CompanyID = this.CurrentUserInfo.UserCompanyID;
                    data.ID = FormulaHelper.CreateGuid();
                    data.Year = Year;
                    data.UserID = user.ID;
                    data.UserIDName = user.Name;
                    data.Dept = user.DeptID;
                    data.DeptName = user.DeptName;
                    this.BusinessEntities.Set<S_W_AttendanceAnnualLeave>().Add(data);
                }
                else
                {
                    data.ModifyDate = DateTime.Now;
                    data.ModifyUser = this.CurrentUserInfo.UserName;
                    data.ModifyUserID = this.CurrentUserInfo.UserID;
                }
                //1年-10年，年假5天；10年及-20年，年假10天；20年及以上15天
                data.Days = 5;
                if (emp.JoinWorkDate.HasValue)
                {
                    TimeSpan ts = YearDate.Subtract(emp.JoinWorkDate.Value);
                    var workYear = ts.Days / 365;
                    if (workYear < 10)
                        data.Days = 5;
                    else if (workYear < 20)
                        data.Days = 10;
                    else
                        data.Days = 15;
                }
                this.BusinessEntities.SaveChanges();
            }
            return Json("");
        }

        public JsonResult CheckData(int Year)
        {
            var HasData = false;
            var exsit = this.BusinessEntities.Set<S_W_AttendanceAnnualLeave>().Any(a => a.Year == Year);
            if (exsit)
                HasData = true;
            return Json(new { HasData });
        }

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var userService = FormulaHelper.GetService<IUserService>();
            var users = userService.GetAllUsers();
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "UserIDName")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("员工不能为空", e.Value);
                    }
                    else
                    {
                        if (!users.Any(a => a.UserName == e.Value))
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("员工不存在", e.Value);
                        }
                    }
                }
                if (e.FieldName == "Year")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("年份不能为空", e.Value);
                    }
                    else
                    {
                        int _year = 0;
                        if (!int.TryParse(e.Value, out _year))
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("年份必须为整数", e.Value);
                        }
                    }
                }
                if (e.FieldName == "Days" )
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("年假天数不能为空", e.Value);
                    }
                    else
                    {
                        int _days = 0;
                        if (!int.TryParse(e.Value, out _days))
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("年假天数必须为整数", e.Value);
                        }
                    }
                }
            });

            return Json(errors);
        }

        public JsonResult SaveExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tempdata["data"]);
            var userService = FormulaHelper.GetService<IUserService>();
            var users = userService.GetAllUsers();

            var allList = this.BusinessEntities.Set<S_W_AttendanceAnnualLeave>().ToList();

            foreach (var item in list)
            {
                var user = users.FirstOrDefault(a => a.UserName == item.GetValue("UserIDName"));
                var entity = allList.FirstOrDefault(a => a.UserID == user.UserID);
                if (entity == null)
                {
                    entity = new S_W_AttendanceAnnualLeave();
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUser = CurrentUserInfo.UserName;
                    entity.CreateUserID = CurrentUserInfo.UserID;
                    entity.OrgID = CurrentUserInfo.UserOrgID;
                    entity.CompanyID = CurrentUserInfo.UserCompanyID;
                    entity.ID = FormulaHelper.CreateGuid();
                    this.BusinessEntities.Set<S_W_AttendanceAnnualLeave>().Add(entity);
                }
                else
                {
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUser = CurrentUserInfo.UserName;
                    entity.ModifyUserID = CurrentUserInfo.UserID;
                }
                this.UpdateEntity<S_W_AttendanceAnnualLeave>(entity, item);
                entity.UserID = user.UserID;
                entity.Dept = user.UserOrgID;
                entity.DeptName = user.UserOrgName;
            }

            this.BusinessEntities.SaveChanges();

            return Json("Success");
        }
    }
}
