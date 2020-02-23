using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using MvcAdapter;
using Formula.Helper;
using Formula;
using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using System.Data;
using Base.Logic.Domain;
using EPC.Logic;
using System.Transactions;
using Comprehensive;
using EPC.Logic.Domain;
using System.Text;

namespace Comprehensive.Areas.WorkHour.Controllers
{
    public class WorkHourSubmitController : ComprehensiveController
    {
        string workHourType = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["WorkHourType"]) ? "Hour" :
            System.Configuration.ConfigurationManager.AppSettings["WorkHourType"];

        decimal NormalHoursMax = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]) ? 8 :
            Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["NormalHoursMax"]);

        decimal maxExtraHour = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ExtraHoursMax"]) ? 0 :
               Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["ExtraHoursMax"]);

        public ActionResult List()
        {
            string sql = "select  Date,IsHoliday from S_C_Holiday where Year='" + DateTime.Now.Year + "' ";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = db.ExecuteDataTable(sql);
            ViewBag.Holiday = JsonHelper.ToJson(dt);
            var dateList = this.entities.Set<S_W_UserWorkHour>().Where(d => d.UserID == this.CurrentUserInfo.UserID
                && d.BelongMonth == DateTime.Now.Month).Select(d => d.WorkHourDate).Distinct().ToList();
            ViewBag.FilledDate = String.Join(",", dateList);
            ViewBag.PieChart = JsonHelper.ToJson(GetPieChartOption(DateTime.Now.Year, DateTime.Now.Month));
            ViewBag.MaxWorkHours = NormalHoursMax;
            ViewBag.MaxExtraHour = maxExtraHour;
            ViewBag.NeedAdditional = String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["NeedAdditional"]) ? true :
               Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["NeedAdditional"]);
            var employ = this.entities.Set<S_HR_Employee>().FirstOrDefault(a => a.UserID == this.CurrentUserInfo.UserID);
            if (employ != null && !String.IsNullOrEmpty(employ.EngageMajor))
                ViewBag.EngageMajor = employ.EngageMajor.Split(',')[0];
            return View();
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            qb.Add("UserID", QueryMethod.Equal, this.CurrentUserInfo.UserID);
            string sql = @"SELECT * from S_W_UserWorkHour";
            qb.SortOrder = "Asc";
            qb.PageSize = 0;
            var dt = this.SqlHelper.ExecuteDataTable(sql, qb);
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var date = Convert.ToDateTime(row["WorkHourDate"]);
                var workHourInfo = list.FirstOrDefault(delegate(Dictionary<string, object> dic)
                {
                    if (dic.GetValue("ProjectID") == row["ProjectID"].ToString() && dic.GetValue("WorkContent") == row["WorkContent"].ToString()
                        && dic.GetValue("SubProjectCode") == row["SubProjectCode"].ToString() && dic.GetValue("MajorCode") == row["MajorCode"].ToString()
                        && dic.GetValue("WorkTimeMajor") == row["WorkTimeMajor"].ToString())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                if (workHourInfo == null)
                {
                    workHourInfo = new Dictionary<string, object>();
                    workHourInfo.SetValue("WorkHourType", row["WorkHourType"]);
                    workHourInfo.SetValue("ProjectID", row["ProjectID"]);
                    workHourInfo.SetValue("ProjectName", row["ProjectName"]);
                    workHourInfo.SetValue("ProjectCode", row["ProjectCode"]);
                    workHourInfo.SetValue("SubProjectName", row["SubProjectName"]);
                    workHourInfo.SetValue("SubProjectCode", row["SubProjectCode"]);
                    workHourInfo.SetValue("WorkTimeMajor", row["WorkTimeMajor"]);
                    workHourInfo.SetValue("MajorCode", row["MajorCode"]);
                    workHourInfo.SetValue("MajorName", row["MajorName"]);
                    workHourInfo.SetValue("ProjectChargerUser", row["ProjectChargerUser"]);
                    workHourInfo.SetValue("ProjectChargerUserName", row["ProjectChargerUserName"]);
                    workHourInfo.SetValue("ProjectDeptName", row["ProjectDeptName"]);
                    workHourInfo.SetValue("ProjectDept", row["ProjectDept"]);
                    workHourInfo.SetValue("WorkContent", row["WorkContent"]);
                    var normalField = date.ToString("yyyy-MM-dd") + "_NormalValue";
                    if (row["NormalValue"] != null && row["NormalValue"] != DBNull.Value &&
                     Convert.ToDecimal(row["NormalValue"]) > 0)
                        workHourInfo.SetValue(normalField, row["NormalValue"]);
                    var additionalField = date.ToString("yyyy-MM-dd") + "_AdditionalValue";
                    if (row["AdditionalValue"] != null && row["AdditionalValue"] != DBNull.Value &&
                        Convert.ToDecimal(row["AdditionalValue"]) > 0)
                        workHourInfo.SetValue(additionalField, row["AdditionalValue"]);
                    //if (date.AddDays(13).Date < DateTime.Now.Date || date.Date > DateTime.Now.Date)
                    //    workHourInfo.SetValue("UnDateLock", true.ToString());
                    //if (date.AddDays(13).Date < DateTime.Now.Date || date.Date > DateTime.Now.Date)
                    //    workHourInfo.SetValue("UnDateLock", true.ToString());//**epc 有 epm 没有
                    list.Add(workHourInfo);
                }
                else
                {
                    var normalField = date.ToString("yyyy-MM-dd") + "_NormalValue";
                    var normalValue = String.IsNullOrEmpty(workHourInfo.GetValue(normalField)) ? 0 : Convert.ToDecimal(workHourInfo.GetValue(normalField));
                    if (row["NormalValue"] != null && row["NormalValue"] != DBNull.Value)
                        normalValue += Convert.ToDecimal(row["NormalValue"]);
                    if (normalValue > 0)
                        workHourInfo.SetValue(normalField, normalValue);

                    var additionalField = date.ToString("yyyy-MM-dd") + "_AdditionalValue";
                    var additionalValue = String.IsNullOrEmpty(workHourInfo.GetValue(additionalField)) ? 0 : Convert.ToDecimal(workHourInfo.GetValue(additionalField));
                    if (row["AdditionalValue"] != null && row["AdditionalValue"] != DBNull.Value)
                        additionalValue += Convert.ToDecimal(row["AdditionalValue"]);
                    if (additionalValue > 0)
                        workHourInfo.SetValue(additionalField, additionalValue);
                }
                if (row["State"].ToString() == "Locked")
                {
                    workHourInfo.SetValue("Locked", true.ToString());
                }
            }
            return Json(list);
        }

        public JsonResult GetSummaryList(string Year, string Month)
        {
            string sql = @"select  Sum(" + (workHourType != WorkHourSaveType.HD.ToString() ? "NormalValue" : "Round(isnull(NormalValue,0)/" + NormalHoursMax.ToString() + ",2)") + @") as SumNormalValue,
Sum(" + (workHourType != WorkHourSaveType.HD.ToString() ? "AdditionalValue" : "Round(isnull(AdditionalValue,0)/" + NormalHoursMax.ToString() + ",2)") + @") as SumAdditionalValue,
Sum(" + (workHourType != WorkHourSaveType.HD.ToString() ? "WorkHourValue" : "WorkHourDay") + @") as SumWorkHourValue,
WorkHourType,WorkTimeMajor,ProjectName,ProjectID
from S_W_UserWorkHour where UserID='{0}' and BelongYear='{1}' and BelongMonth='{2}'
group by ProjectName,ProjectID,WorkHourType,WorkTimeMajor
order by WorkHourType";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, this.CurrentUserInfo.UserID, Year, Month));
            return Json(dt);
        }

        public JsonResult SaveWorkHour(string ListData, string date)
        {
            var selectDate = Convert.ToDateTime(date);
            var list = JsonHelper.ToList(ListData);
            var fo = FormulaHelper.CreateFO<WorkHourFO>();
            fo.ValidateData(list, selectDate, NormalHoursMax, maxExtraHour);
            fo.SaveWorkHour(list, selectDate, this.CurrentUserInfo, workHourType, NormalHoursMax);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteWorkHour(string ListData, string date)
        {
            var selectDate = Convert.ToDateTime(date);
            var startDate = DateTimeHelper.GetWeekFirstDayMon(selectDate);
            var endDate = DateTimeHelper.GetWeekLastDaySun(selectDate);
            var list = JsonHelper.ToList(ListData);
            //var employee = this.entities.Set<S_HR_Employee>().FirstOrDefault(d => d.UserID == this.CurrentUserInfo.UserID);
            foreach (var item in list)
            {
                var projectID = item.GetValue("ProjectID");
                var WorkTimeMajor = item.GetValue("WorkTimeMajor");
                var SubProjectCode = item.GetValue("SubProjectCode");
                var MajorCode = item.GetValue("MajorCode");
                var WorkContent = item.GetValue("WorkContent");
                for (DateTime i = startDate; i < endDate; i = i.AddDays(1))
                {
                    var userWorkHour = this.entities.Set<S_W_UserWorkHour>().FirstOrDefault(d => d.ProjectID == projectID && d.SubProjectCode == SubProjectCode
            && d.MajorCode == MajorCode && d.WorkTimeMajor == WorkTimeMajor && d.WorkContent == WorkContent && d.WorkHourDate == i);
                    if (userWorkHour == null)
                        continue;
                    if (userWorkHour != null)
                    {
                        entities.Set<S_W_UserWorkHour>().Remove(userWorkHour);
                    }
                    if (userWorkHour.State == "Locked")
                    {
                        throw new Formula.Exceptions.BusinessValidationException("已经审批完成的工时记录，不能删除");
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetPieChart(string Year, string Month)
        {
            var belongYear = DateTime.Now.Year;
            var belongMonth = DateTime.Now.Month;
            if (!String.IsNullOrEmpty(Year)) belongYear = Convert.ToInt32(Year);
            if (!String.IsNullOrEmpty(Month)) belongMonth = Convert.ToInt32(Month);
            var result = GetPieChartOption(belongYear, belongMonth);
            var credits = new Dictionary<string, object>();
            credits.SetValue("enabled", false);
            result.SetValue("credits", credits);
            return Json(result);
        }

        public JsonResult SetWorkHourState(string SelectedData, string State)
        {
            Action action = () =>
            {
                var list = JsonHelper.ToList(SelectedData);
                foreach (var item in list)
                {
                    var workHourInfo = this.GetEntityByID<S_W_UserWorkHour>(item.GetValue("ID"));
                    if (workHourInfo == null) throw new Formula.Exceptions.BusinessValidationException("当前有不存在的工时记录，请重新确认！");

                    decimal? workHourValue = null;
                    if (State == "Locked")
                    {
                        if (!string.IsNullOrEmpty(item.GetValue("WorkHourValue")))
                            workHourValue = decimal.Parse(item.GetValue("WorkHourValue"));
                        #region 工时审批通过后写入成本数据
                        workHourInfo.ImportToCost();
                        #endregion
                    }
                    else
                    {
                        if (workHourInfo.State == "Locked")
                            throw new Formula.Exceptions.BusinessValidationException("已通过审核的工时不能被驳回！");

                        workHourValue = 0M;
                        workHourInfo.RevertCost();
                    }
                    workHourInfo.State = State;
                    workHourInfo.ConfirmValue = workHourValue;
                    workHourInfo.ConfirmDay = new WorkHourFO().ConvertHourToDay(workHourInfo.ConfirmValue, workHourType, NormalHoursMax);
                }
                this.entities.SaveChanges();
            };

            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    action();
                    ts.Complete();
                }
            }
            else
            {
                action();
            }
            return Json("");
        }

        public JsonResult ApproveWorkHourList(string SelectedData, string State)
        {
            Action action = () =>
            {
                if (string.IsNullOrEmpty(State))
                    throw new Formula.Exceptions.BusinessException("未传入State，无法操作");
                var list = JsonHelper.ToList(SelectedData);
                var fo = new WorkHourFO();
                SQLHelper hrDB = SQLHelper.CreateSqlHelper(ConnEnum.Comprehensive);
                var enumDef = EnumBaseHelper.GetEnumDef("Comprehensive.WorkHourState");
                var enumItems = new List<EnumItemInfo>();
                if (enumDef != null)
                    enumItems = enumDef.EnumItem.ToList();
                var _state = State;
                if (State == "Locked")
                    _state = "Confirm";
                var fieldDt = GetFieldTable(hrDB, "S_W_UserWorkHour");
                foreach (var item in list)
                {
                    var itemStateLevel = 0d;
                    var itemState = item.GetValue("State");
                    if (enumItems.Any(a => a.Code == itemState))
                        itemStateLevel = enumItems.FirstOrDefault(a => a.Code == itemState).SortIndex.Value;
                    var fnStateLevel = 0d;
                    if (enumItems.Any(a => a.Code == State))
                        fnStateLevel = enumItems.FirstOrDefault(a => a.Code == State).SortIndex.Value;
                    if (fnStateLevel < itemStateLevel)
                        throw new Formula.Exceptions.BusinessException("已经【" + enumItems.FirstOrDefault(a => a.Code == itemState).Name
                            + "】的数据不能再【" + enumItems.FirstOrDefault(a => a.Code == State).Name + "】，无法保存");

                    if (!item.ContainsKey(_state + "Value"))
                        throw new Formula.Exceptions.BusinessException("列表中不包含【" + _state + "Value】字段，无法保存");
                    if (!item.ContainsKey(_state + "Day"))
                        throw new Formula.Exceptions.BusinessException("列表中不包含【" + _state + "Day】字段，无法保存");
                    if (!item.ContainsKey(_state + "User"))
                        throw new Formula.Exceptions.BusinessException("列表中不包含【" + _state + "User】字段，无法保存");
                    if (!item.ContainsKey(_state + "Date"))
                        throw new Formula.Exceptions.BusinessException("列表中不包含【" + _state + "Date】字段，无法保存");
                    if (!item.ContainsKey("Is" + _state))
                        throw new Formula.Exceptions.BusinessException("列表中不包含【Is" + _state + "】字段，无法保存");
                    var id = item.GetValue("ID");

                    item.SetValue(_state + "User", this.CurrentUserInfo.UserID);
                    item.SetValue(_state + "UserName", this.CurrentUserInfo.UserName);
                    item.SetValue(_state + "Date", System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    item.SetValue("Is" + _state, "1");
                    item.SetValue("State", State);

                    decimal? workHourValue = null;
                    if (!string.IsNullOrEmpty(item.GetValue("WorkHourValue")))
                        workHourValue = decimal.Parse(item.GetValue("WorkHourValue"));
                    decimal? approveValue = null;
                    if (!string.IsNullOrEmpty(item.GetValue(_state + "Value")))
                        approveValue = decimal.Parse(item.GetValue(_state + "Value"));
                    if (approveValue == null)
                        approveValue = workHourValue;//审批工时为空时，默认审批工时=填报工时；
                    item.SetValue(_state + "Value", approveValue);
                    decimal? approveDay = fo.ConvertHourToDay(approveValue, workHourType, NormalHoursMax);
                    item.SetValue(_state + "Day", approveDay);

                    var sb = new StringBuilder();
                    sb.AppendLine(CreateUpdateSql(item, hrDB, "S_W_UserWorkHour", id, fieldDt));

                    var detailDic = new Dictionary<string, object>();
                    detailDic.SetValue("ID", FormulaHelper.CreateGuid());
                    detailDic.SetValue("S_W_UserWorkHourID", id);
                    detailDic.SetValue("SortIndex", 0);
                    detailDic.SetValue("ApproveValue", approveValue);
                    detailDic.SetValue("ApproveDay", approveDay);
                    detailDic.SetValue("ApproveDate", item.GetValue(_state + "Date"));
                    detailDic.SetValue("ApproveStep", State);
                    detailDic.SetValue("ApproveUser", this.CurrentUserInfo.UserID);
                    detailDic.SetValue("ApproveUserName", this.CurrentUserInfo.UserName);
                    sb.AppendLine(detailDic.CreateInsertSql(hrDB, "S_W_UserWorkHour_ApproveDetail", detailDic.GetValue("ID")));

                    var sql = sb.ToString();
                    hrDB.ExecuteNonQuery(sql);

                    if (State == "Locked")
                    {
                        #region 工时审批通过后写入成本数据
                        S_W_UserWorkHour workHourInfo = new S_W_UserWorkHour();
                        UpdateEntity<S_W_UserWorkHour>(workHourInfo, item);
                        workHourInfo.ImportToCost();
                        #endregion
                    }

                }
            };

            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    action();
                    ts.Complete();
                }
            }
            else
            {
                action();
            }

            return Json("");
        }

        public JsonResult GetWorkHourApproveDetail(string S_W_UserWorkHourID, QueryBuilder qb)
        {
            var data = this.entities.Set<S_W_UserWorkHour_ApproveDetail>().Where(a => a.S_W_UserWorkHourID == S_W_UserWorkHourID).WhereToGridData(qb);
            return Json(data);
        }

        private DataTable GetFieldTable(SQLHelper access, string tableName)
        {
            string sql = string.Format("select FieldCode=a.name from syscolumns a  inner join sysobjects d on a.id=d.id and d.name='{0}'", tableName);
            return access.ExecuteDataTable(sql);
        }
        private string CreateUpdateSql(Dictionary<string, object> entity, SQLHelper access, string tableName, string ID, DataTable FieldTable)
        {
            var fields = FieldTable.AsEnumerable().Select(c => c[0].ToString()).ToArray();
            StringBuilder sb = new StringBuilder();
            foreach (var item in entity)
            {
                string name = item.Key.ToString();
                if (name == "ID")
                    continue;
                if (!fields.Contains(name))
                    continue;
                if (item.Value == null)
                    continue;
                string value = item.Value.ToString();
                //这里不能为NULL，必须给个空，因为SubProjectCode初始ef增加方式为空，审批时sql增加方式如果为null，填报页面保存后会重复增加
                if (String.IsNullOrEmpty(value))
                    sb.AppendFormat(",{0}=''", name);
                else
                    sb.AppendFormat(",{0}='{1}'", name, value.Replace("'", "''"));
            }
            string sql = string.Format(@"UPDATE {0} SET {2} WHERE ID='{1}'", tableName, ID, sb.ToString().Trim(','));
            return sql;
        }


        #region 饼图
        private Dictionary<string, object> GetPieChartOption(int year, int month)
        {
            var baseEntities = FormulaHelper.GetEntities<Base.Logic.Domain.BaseEntities>();
            var holidayConfig = baseEntities.Set<S_C_Holiday>().Where(d => d.Year == year).ToList();
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            TimeSpan sp = endDate.Subtract(startDate);
            var day = Convert.ToDecimal(sp.Days) + 1;
            var holiday = 0;
            var title = year.ToString() + "年" + month + "月 应填：{0}  已填：{1} 未填：{2}";

            //**与epm完全不一样
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
            if (workHourType == "Hour")
            {
                day = (day - holiday) * NormalHoursMax;
            }
            else
            {
                day = (day - holiday);
            }
            //**

            string sql = @"select isnull(Sum(NormalValue),0) as valueField,ProjectName as nameField from dbo.S_W_UserWorkHour
where UserID='{0}' and WorkHourDate>='{1}' and WorkHourDate<='{2}' group by ProjectName";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, this.CurrentUserInfo.UserID, startDate, endDate));
            if (dt.Rows.Count == 0)
            {
                var row = dt.NewRow();
                row["valueField"] = day;
                row["nameField"] = "未填工时";
                dt.Rows.Add(row);
                title = String.Format(title, day, 0, Convert.ToInt16(day));
            }
            else
            {
                var obj = Convert.ToDecimal(dt.Compute("Sum(valueField)", ""));
                if ((day - obj) > 0)
                {
                    var row = dt.NewRow();
                    row["valueField"] = day - obj;
                    row["nameField"] = "未填工时";
                    dt.Rows.Add(row);
                }

                title = String.Format(title, day, Convert.ToInt16(obj), Convert.ToInt16(day - obj));
            }
            var pieChart = Comprehensive.Logic.HighChartHelper.CreatePieChart(title, "工时", dt);
            return pieChart.Render();
        }
        #endregion
    }
}
