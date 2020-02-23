using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Formula;
using HR.Logic.Domain;
using Base.Logic.Domain;
using System.Globalization;
using System.ComponentModel;

namespace HR.Logic
{
    public class AttendanceFO
    {

        private HREntities entities = FormulaHelper.GetEntities<HREntities>();
        private BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();

        /// <summary>
        /// 从数据库判断是否为节日、假日（1：假日；2：节日）
        /// </summary>
        /// <param name="date">日期</param>
        public bool IsHolidayFromDb(DateTime date)
        {
            return baseEntities.Set<S_C_Holiday>().Any(i => i.Year == date.Year && i.Month == date.Month && i.Day == date.Day && (i.IsHoliday == "1" || i.IsHoliday == "2"));
        }

        /// <summary>
        /// 从数据库判断是否为工作日
        /// </summary>
        /// <param name="date">日期</param>
        public bool IsWorkDayFromDb(DateTime date)
        {
            return baseEntities.Set<S_C_Holiday>().Any(i => i.Year == date.Year && i.Month == date.Month && i.Day == date.Day && i.IsHoliday == "0");
        }

        /// <summary>
        /// 判断是否为工作日
        /// </summary>
        /// <param name="date">要判断的日期</param>
        public bool IsWorkDay(DateTime date)
        {
            if (IsHolidayFromDb(date))
            {
                return false;
            }
            if (IsWorkDayFromDb(date))
            {
                return true;
            }
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据年月获取当月的应出勤天数
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>应出勤日期列表</returns>
        public List<DateTime?> GetShouldAttendanceDays(int year, int month)
        {
            List<DateTime?> shouldAttendanceDays = new List<DateTime?>();
            var date = Convert.ToDateTime(year + "-" + month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') + "-01");
            var monthDays = DateTime.DaysInMonth(year, month);
            for (int i = 0; i < monthDays; i++)
            {
                var dateTemp = date.Date.AddDays(i);
                //if (dateTemp > DateTime.Now)
                //{
                //    break;
                //}
                if (IsWorkDay(dateTemp))
                {
                    shouldAttendanceDays.Add(dateTemp);
                }
            }
            return shouldAttendanceDays;
        }

        /// <summary>
        /// 根据年月获取当年的应出勤天数
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>应出勤日期列表</returns>
        public List<DateTime?> GetShouldAttendanceDays(int year)
        {
            List<DateTime?> shouldAttendanceDays = new List<DateTime?>();
            for (int month = 1; month <= 12; month++)
            {
                var date = Convert.ToDateTime(year + "-" + month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') + "-01");
                var monthDays = DateTime.DaysInMonth(year, month);
                for (int i = 0; i < monthDays; i++)
                {
                    var dateTemp = date.Date.AddDays(i);
                    if (IsWorkDay(dateTemp))
                    {
                        shouldAttendanceDays.Add(dateTemp);
                    }
                }
            }
            return shouldAttendanceDays;
        }

        /// <summary>
        /// 判断员工该日是否迟到
        /// </summary>
        /// <param name="workBase">考勤基础设置信息</param>
        /// <param name="firstSign">第一次打卡时间</param>
        public static bool IsLate(WorkBase workBase, DateTime? firstSign)
        {
            if (firstSign != null)
            {
                var first = ((DateTime)firstSign).ToShortTimeString();
                var f = Convert.ToDateTime(first);
                //var sb = Convert.ToDateTime(workBase.StratWorkAm);
                var zc = Convert.ToDateTime(workBase.StratWorkAm);
                var cd = Convert.ToDateTime(workBase.LateDate);
                //if (DateTime.Compare(f, sb) > 0 && DateTime.Compare(f, cd) <= 0)
                //只判断是否迟到
                if (zc < f && f <= cd)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 判断员工该日是否异常
        /// </summary>
        /// <param name="workBase"></param>
        /// <param name="firstSign"></param>
        /// <returns></returns>
        public static bool IsError(WorkBase workBase, DateTime? sign)
        {
            if (sign != null)
            {
                var time = ((DateTime)sign).ToShortTimeString();
                var t = Convert.ToDateTime(time);
                var cd = Convert.ToDateTime(workBase.LateDate);
                var zt = Convert.ToDateTime(workBase.LeaveEarlyDate);
                if (cd < t && t < zt)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 判断员工该日是否早退
        /// </summary>
        /// <param name="workBase">考勤基础设置信息</param>
        /// <param name="lastSign">第二次打卡时间</param>
        public static bool IsLeaveEarly(WorkBase workBase, DateTime? lastSign)
        {
            if (lastSign != null)
            {
                var last = ((DateTime)lastSign).ToShortTimeString();
                var l = Convert.ToDateTime(last);
                var zc = Convert.ToDateTime(workBase.OffWorkDate);
                var zt = Convert.ToDateTime(workBase.LeaveEarlyDate);
                //var xb = Convert.ToDateTime(workBase.OffWorkDate);
                //if (DateTime.Compare(l, zt) >= 0 && DateTime.Compare(l, xb) < 0)
                //只判断是否早退
                if (zt <= l && l < zc)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// 考勤基础设置信息类
    /// </summary>
    public class WorkBase
    {
        /// <summary>
        /// 上班打卡时间
        /// </summary>
        public string StratWorkAm { get; set; }

        /// <summary>
        /// 迟到时间
        /// </summary>
        public string LateDate { get; set; }


        /// <summary>
        /// 下班打卡时间
        /// </summary>
        public string OffWorkDate { get; set; }

        /// <summary>
        /// 早退时间
        /// </summary>
        public string LeaveEarlyDate { get; set; }

        /// <summary>
        /// 根据打卡数据生成迟到早退出勤缺席状态
        /// </summary>
        public bool UseAttendanceInfo { get; set; }


        public WorkBase()
        {
            DbContext entities = Formula.FormulaHelper.GetEntities<HREntities>();
            var entity = entities.Set<S_W_AttendanceConfig>().FirstOrDefault();
            if (entity != null)
            {
                StratWorkAm = Convert.ToDateTime(entity.AmCheckDate).ToShortTimeString();
                LateDate = Convert.ToDateTime(entity.AmLateCheckDate).ToShortTimeString();
                OffWorkDate = Convert.ToDateTime(entity.PmCheckDate).ToShortTimeString();
                LeaveEarlyDate = Convert.ToDateTime(entity.PmEarlyDate).ToShortTimeString();
                UseAttendanceInfo = entity.UseAttendanceInfo.HasValue ? entity.UseAttendanceInfo.Value : false;
            }
        }
    }

    /// <summary>
    /// 上下午标示
    /// </summary>
    [Description("上下午标示")]
    public enum DayFlag
    {
        /// <summary>
        /// 上午
        /// </summary>
        [Description("上午")]
        AM,
        /// <summary>
        /// 下午
        /// </summary>
        [Description("下午")]
        PM
    }

    /// <summary>
    /// 考勤签到标示
    /// </summary>
    [Description("考勤签到标示")]
    public enum CheckType
    {
        /// <summary>
        /// 签到
        /// </summary>
        [Description("签到")]
        SignIn,

        /// <summary>
        /// 签退
        /// </summary>
        [Description("签退")]
        SignOut
    }

    [Description("生效标示")]
    public enum EffectiveFlag
    {
        /// <summary>
        /// 签到
        /// </summary>
        [Description("是")]
        Y,

        /// <summary>
        /// 签退
        /// </summary>
        [Description("否")]
        N
    }

    [Description("考勤状态")]
    public enum AttendenceState
    {
        /// <summary>
        /// 出勤
        /// </summary>
        [Description("出勤")]
        Normal,
        /// <summary>
        /// 迟到
        /// </summary>
        [Description("迟到")]
        ComeLate,
        /// <summary>
        /// 早退
        /// </summary>
        [Description("早退")]
        LeaveEarly,
        /// <summary>
        /// 缺勤
        /// </summary>
        [Description("缺勤")]
        Absence,
        /// <summary>
        /// 出差
        /// </summary>
        [Description("出差")]
        Business,
        /// <summary>
        /// 请假
        /// </summary>
        [Description("请假")]
        Leave,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Error
    }

}
