using Config;
using Formula.Helper;
using HR.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config.Logic;
using Formula;
using System.ComponentModel;
using Base.Logic.Domain;

namespace HR.Logic
{
    [Description("工时填报与结算类型")]
    public enum WorkHourSaveType
    {
        [Description("按小时填报按小时结算")]
        Hour,
        [Description("按小时填报按天结算")]
        HD,
        [Description("按天填报按天结算")]
        Day,
    }

    public class WorkHourFO
    {
        //工时填报与结算类型；Hour：按小时填报按小时结算（默认），HD：按小时填报按天结算，Day：按天填报按天结算
        string workHourType = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkHourType"]) ? WorkHourSaveType.Hour.ToString() :
            System.Configuration.ConfigurationManager.AppSettings["WorkHourType"];
        decimal NormalHoursMax = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]) ? 8 :
            Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]);

        public void ValidateData(List<Dictionary<string, object>> list, DateTime date, decimal maxNormaValue = 8, decimal maxEtraValue = 0)
        {
            var startDate = DateTimeHelper.GetWeekFirstDayMon(date);
            var endDate = DateTimeHelper.GetWeekLastDaySun(date);
            for (DateTime i = startDate; i < endDate; i = i.AddDays(1))
            {
                var normalValue = 0m;
                var extraValue = 0m;
                var normalValueField = i.ToString("yyyy-MM-dd") + "_NormalValue";
                var addtionalValueField = i.ToString("yyyy-MM-dd") + "_AdditionalValue";
                foreach (var item in list)
                {
                    normalValue += String.IsNullOrEmpty(item.GetValue(normalValueField)) ? 0 : Convert.ToDecimal(item.GetValue(normalValueField));
                    extraValue += String.IsNullOrEmpty(item.GetValue(addtionalValueField)) ? 0 : Convert.ToDecimal(item.GetValue(addtionalValueField));
                }
                if (normalValue > maxNormaValue)
                    throw new Formula.Exceptions.BusinessException("【" + i.ToShortDateString() + "】的正常工时不能大于" + maxNormaValue.ToString());
                if (extraValue > maxEtraValue)
                    throw new Formula.Exceptions.BusinessException("【" + i.ToShortDateString() + "】的加班工时不能大于" + maxEtraValue.ToString());
            }
        }

        public void SaveWorkHour(List<Dictionary<string, object>> list, DateTime date, UserInfo user, string workHourSaveType, decimal maxNormaValue = 8)
        {
            var startDate = DateTimeHelper.GetWeekFirstDayMon(date);
            var endDate = DateTimeHelper.GetWeekLastDaySun(date);
            var entities = Formula.FormulaHelper.GetEntities<HREntities>();
            var employee = entities.Set<T_Employee>().FirstOrDefault(d => d.UserID == user.UserID);
            var hrdb = SQLHelper.CreateSqlHelper(ConnEnum.HR);

            var delStr = "";
            for (DateTime i = startDate; i <= endDate; i = i.AddDays(1))
            {
                delStr += string.Format(@"
delete S_W_UserWorkHour where WorkHourDate = '{0}' and State = 'Create' and UserID = '{1}'", i, user.UserID);
            }
            hrdb.ExecuteNonQuery(delStr);

            #region 保存工时数据
            foreach (var item in list)
            {
                var projectID = item.GetValue("ProjectID");
                var SubProjectCode = item.GetValue("SubProjectCode");
                var MajorCode = item.GetValue("MajorCode");
                var WorkContent = item.GetValue("WorkContent");
                for (DateTime i = startDate; i <= endDate; i = i.AddDays(1))
                {
                    var normalValueField = i.ToString("yyyy-MM-dd") + "_NormalValue";
                    var normalValue = String.IsNullOrEmpty(item.GetValue(normalValueField)) ? 0M : Convert.ToDecimal(item.GetValue(normalValueField));
                    var addtionalValueField = i.ToString("yyyy-MM-dd") + "_AdditionalValue";
                    var addtionalValue = String.IsNullOrEmpty(item.GetValue(addtionalValueField)) ? 0M : Convert.ToDecimal(item.GetValue(addtionalValueField));
                   
                    if ((normalValue == 0 && addtionalValue == 0))
                    {
                        continue;
                    }
                    var sql = string.Format(
                        "select top 1 ID from S_W_UserWorkHour where ProjectID='{0}' and SubProjectCode='{1}' and MajorCode='{2}' and WorkContent='{3}' and UserID='{4}' and WorkHourDate='{5}'",
                        projectID, SubProjectCode, MajorCode, WorkContent, user.UserID, i);
                    var dataTable = hrdb.ExecuteDataTable(sql);
                    if (dataTable.Rows.Count == 0)
                    {
                        var userWorkHour = new S_W_UserWorkHour();
                        userWorkHour.ID = FormulaHelper.CreateGuid();
                        userWorkHour.UserID = user.UserID;
                        userWorkHour.UserName = user.UserName;
                        userWorkHour.UserDeptID = user.UserOrgID;
                        userWorkHour.UserDeptName = user.UserOrgName;
                        userWorkHour.WorkHourDate = i;
                        userWorkHour.UserCode = user.Code;
                        if (employee != null)
                        {
                            userWorkHour.EmployeeID = employee.ID;
                        }
                        userWorkHour.State = "Create";
                        userWorkHour.IsConfirm = "0";
                        userWorkHour.IsStep1 = "0";
                        userWorkHour.IsStep2 = "0";
                        userWorkHour.CreateUser = user.UserName;
                        userWorkHour.CreateUserID = user.UserID;
                        userWorkHour.SupplementID = "";
                        userWorkHour.BelongMonth = i.Month;
                        userWorkHour.BelongYear = i.Year;
                        userWorkHour.BelongQuarter = ((i.Month - 1) / 3) + 1;
                        userWorkHour.ProjectChargerUser = item.GetValue("ProjectChargerUser");
                        userWorkHour.ProjectChargerUserName = item.GetValue("ProjectChargerUserName");
                        userWorkHour.ProjectCode = item.GetValue("ProjectCode");
                        userWorkHour.ProjectDept = item.GetValue("ProjectDept");
                        userWorkHour.ProjectDeptName = item.GetValue("ProjectDeptName");
                        userWorkHour.ProjectID = projectID;
                        userWorkHour.ProjectName = item.GetValue("ProjectName");
                        userWorkHour.SubProjectCode = item.GetValue("SubProjectCode");
                        userWorkHour.SubProjectName = item.GetValue("SubProjectName");
                        userWorkHour.TaskWorkCode = item.GetValue("TaskWorkCode");
                        userWorkHour.TaskWorkName = item.GetValue("TaskWorkName");
                        userWorkHour.WorkContent = item.GetValue("WorkContent");
                        userWorkHour.MajorCode = item.GetValue("MajorCode");
                        userWorkHour.MajorName = item.GetValue("MajorName");
                        userWorkHour.WorkHourType = item.GetValue("WorkHourType");

                        userWorkHour.NormalValue = normalValue;
                        userWorkHour.AdditionalValue = addtionalValue;
                        userWorkHour.WorkHourValue = normalValue + addtionalValue;
                        userWorkHour.WorkHourDay = ConvertHourToDay(userWorkHour.WorkHourValue, workHourSaveType, maxNormaValue);

                        userWorkHour.ToDic().InsertDB(hrdb, "S_W_UserWorkHour", userWorkHour.ID);
                    }
                    else
                    {
                        var id = dataTable.Rows[0]["ID"].ToString();
                        var dic = new Dictionary<string, object>();
                        dic.SetValue("NormalValue", normalValue);
                        dic.SetValue("AdditionalValue", addtionalValue);
                        dic.SetValue("WorkHourValue", normalValue + addtionalValue);
                        dic.SetValue("WorkHourDay", ConvertHourToDay(normalValue + addtionalValue, workHourSaveType, maxNormaValue));
                        dic.UpdateDB(hrdb, "S_W_UserWorkHour", id);
                    }
                }
            }
            #endregion

        }

        public decimal? ConvertHourToDay(decimal? hourValue, string workHourSaveType, decimal maxNormaValue = 8)
        {
            decimal? rtn = 0m;
            if (workHourSaveType == WorkHourSaveType.HD.ToString())
            {
                //只有当按小时填报，按天结算的时候，才需要将填报值转换为天数
                if (maxNormaValue != 0)
                {
                    var hour = hourValue.HasValue ? hourValue.Value : 0M;
                    rtn = Math.Round(hour / maxNormaValue, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
                rtn = hourValue;
            return rtn;
        }

        public decimal GetWorkValue(string queryStartDate, string queryEndDate)
        {
            var startDate = Convert.ToDateTime(queryStartDate);
            var endDate = Convert.ToDateTime(queryEndDate);
            return GetWorkValue(startDate, endDate);
        }
        public decimal GetWorkValue(DateTime startDate, DateTime endDate)
        {
            TimeSpan sp = endDate.Subtract(startDate);
            var needApply = Convert.ToDecimal(sp.Days) + 1;
            var years = new List<int>() { startDate.Year };
            if (endDate.Year != startDate.Year)
                years.Add(endDate.Year);
            var baseEntities = FormulaHelper.GetEntities<Base.Logic.Domain.BaseEntities>();
            var holidayConfig = baseEntities.Set<S_C_Holiday>().Where(d => d.Year.HasValue && years.Contains(d.Year.Value)).ToList();
            var holiday = 0;
            for (DateTime i = startDate; i <= endDate; i = i.AddDays(1))
            {
                bool isholiday = false;
                if (i.DayOfWeek == DayOfWeek.Saturday || i.DayOfWeek == DayOfWeek.Sunday)
                    isholiday = true;
                var config = holidayConfig.FirstOrDefault(d => d.Date == i);
                if (config != null)
                {
                    if (config.IsHoliday == "0")
                        isholiday = false;
                    else
                        isholiday = true;
                }
                if (isholiday)
                    holiday++;
            }
            if (workHourType == WorkHourSaveType.Day.ToString())
            {
                needApply = (needApply - holiday);
            }
            else
            {
                needApply = (needApply - holiday) * NormalHoursMax;
            }
            return needApply;
        }
    }
}
