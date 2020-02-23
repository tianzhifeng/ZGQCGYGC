using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Base.Logic.Domain;
using Formula.Helper;
using Formula;
using Config;

namespace Base.Areas.Calendar.Controllers
{
    public class HolidayController : BaseController
    {
        #region 展示

        public ViewResult Index()
        {
            if (string.IsNullOrEmpty(Request["Year"]))
            {
                Response.Redirect("/Base/Calendar/Holiday/Index?Year=" + DateTime.Now.Year);
            }
            int year = int.Parse(Request["Year"]);

            ViewBag.DayList = GetYeayDay(year);
            var workdayList = GetYearWorkday(year);
            var holidayList = GetYearHoliday(year);
            var festivalList = GetYearFestival(year);
            ViewBag.WorkdayList = workdayList;
            ViewBag.HolidayList = holidayList;
            ViewBag.FestivalList = festivalList;

            DateTime d = new DateTime(year, 1, 1);

            int workDay = 52 * 5;//52周
            int holiday = 52 * 2;//52周

            d = d.AddDays(52 * 7);
            while (d.Year == year)
            {
                d = d.AddDays(1);
                if (d.DayOfWeek == DayOfWeek.Sunday || d.DayOfWeek == DayOfWeek.Saturday)
                    holiday++;
                else
                    workDay++;
            }

            int workDayCount = workdayList.Sum(c => c.Count());
            int holidayCount = holidayList.Sum(c => c.Count());
            int festivalCount = festivalList.Sum(c => c.Count());

            workDay = workDay + workDayCount - holidayCount - festivalCount;
            holiday = holiday + holidayCount + festivalCount - workDayCount;

            ViewBag.WorkdayCount = workDay;
            ViewBag.HolidayCount = holiday;

            return View();
        }

        private List<int[]> GetYearWorkday(int year)
        {
            List<int[]> list = new List<int[]>();
            for (int i = 1; i <= 12; i++)
            {
                var arr = entities.Set<S_C_Holiday>().Where(c => c.Year == year && c.Month == i && c.IsHoliday == "0").Select(c => c.Day ?? -1).ToArray();
                list.Add(arr);
            }
            return list;
        }

        private List<int[]> GetYearHoliday(int year)
        {
            List<int[]> list = new List<int[]>();
            for (int i = 1; i <= 12; i++)
            {
                var arr = entities.Set<S_C_Holiday>().Where(c => c.Year == year && c.Month == i && c.IsHoliday == "1").Select(c => c.Day ?? -1).ToArray();
                list.Add(arr);
            }
            return list;
        }
        private List<int[]> GetYearFestival(int year)
        {
            List<int[]> list = new List<int[]>();
            for (int i = 1; i <= 12; i++)
            {
                var arr = entities.Set<S_C_Holiday>().Where(c => c.Year == year && c.Month == i && c.IsHoliday == "2").Select(c => c.Day ?? -1).ToArray();
                list.Add(arr);
            }
            return list;
        }

        private List<int[,]> GetYeayDay(int year)
        {
            List<int[,]> list = new List<int[,]>();

            for (int i = 1; i <= 12; i++)
            {
                list.Add(GetMonthDay(year, i));
            }
            return list;
        }

        private int[,] GetMonthDay(int year, int monty)
        {
            DateTime day = DateTime.Parse(string.Format("{0}-{1}-1", year, monty));
            int week1 = (int)day.DayOfWeek;//获取当年当月1号的星期            
            int lastday = day.AddMonths(1).AddDays(-1).Day; //获取当月的最后一天

            var arr = new int[6, 7];
            int row = 0;
            int col = 0;
            for (int i = 1; i <= lastday; i++)
            {
                row = (i + week1 - 1) / 7;
                col = (i + week1 - 1) % 7;
                arr[row, col] = i;
            }
            return arr;
        }

        #endregion

        #region 保存

        public JsonResult DeleteHoliday(string data, int year)
        {
            Dictionary<string, int> dic = JsonHelper.ToObject<Dictionary<string, int>>(data);
            int month = dic["month"];
            int day = dic["day"];
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sqlHelper.ExecuteNonQuery(string.Format("delete from S_C_Holiday where Year='{0}' and Month='{1}' and Day='{2}'", year, month, day));
            return Json("{}");
        }

        public JsonResult AddHoliday(string data, int year)
        {
            Dictionary<string, int> dic = JsonHelper.ToObject<Dictionary<string, int>>(data);
            int month = dic["month"];
            int day = dic["day"];

            //entities.Set<S_C_Holiday>().Delete(c => c.Year == year && c.Month == month && c.Day == day);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sqlHelper.ExecuteNonQuery(string.Format("delete from S_C_Holiday where Year='{0}' and Month='{1}' and Day='{2}'", year, month, day));

            S_C_Holiday entity = new S_C_Holiday
            {
                ID = FormulaHelper.CreateGuid(),
                Year = year,
                Month = month,
                Day = day,
                Date = new DateTime(year, month, day),
                IsHoliday = "1"
            };
            entity.DayOfWeek = ((DateTime)entity.Date).DayOfWeek.ToString();
            entities.Set<S_C_Holiday>().Add(entity);
            entities.SaveChanges();
            return Json("{}");
        }

        public JsonResult DeleteWorkday(string data, int year)
        {
            Dictionary<string, int> dic = JsonHelper.ToObject<Dictionary<string, int>>(data);
            int month = dic["month"];
            int day = dic["day"];
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sqlHelper.ExecuteNonQuery(string.Format("delete from S_C_Holiday where Year='{0}' and Month='{1}' and Day='{2}'", year, month, day));
            return Json("{}");
        }

        public JsonResult AddWorkday(string data, int year)
        {
            Dictionary<string, int> dic = JsonHelper.ToObject<Dictionary<string, int>>(data);
            int month = dic["month"];
            int day = dic["day"];

            //entities.Set<S_C_Holiday>().Delete(c => c.Year == year && c.Month == month && c.Day == day);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sqlHelper.ExecuteNonQuery(string.Format("delete from S_C_Holiday where Year='{0}' and Month='{1}' and Day='{2}'", year, month, day));

            S_C_Holiday entity = new S_C_Holiday
            {
                ID = FormulaHelper.CreateGuid(),
                Year = year,
                Month = month,
                Day = day,
                Date = new DateTime(year, month, day),
                IsHoliday = "0"
            };
            entity.DayOfWeek = ((DateTime)entity.Date).DayOfWeek.ToString();
            entities.Set<S_C_Holiday>().Add(entity);
            entities.SaveChanges();
            return Json("{}");
        }

        public JsonResult DeleteFestival(string data, int year)
        {
            Dictionary<string, int> dic = JsonHelper.ToObject<Dictionary<string, int>>(data);
            int month = dic["month"];
            int day = dic["day"];
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sqlHelper.ExecuteNonQuery(string.Format("delete from S_C_Holiday where Year='{0}' and Month='{1}' and Day='{2}'", year, month, day));
            return Json("{}");
        }

        public JsonResult AddFestival(string data, int year)
        {
            Dictionary<string, int> dic = JsonHelper.ToObject<Dictionary<string, int>>(data);
            int month = dic["month"];
            int day = dic["day"];

            //entities.Set<S_C_Holiday>().Delete(c => c.Year == year && c.Month == month && c.Day == day);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            sqlHelper.ExecuteNonQuery(string.Format("delete from S_C_Holiday where Year='{0}' and Month='{1}' and Day='{2}'", year, month, day));

            S_C_Holiday entity = new S_C_Holiday
            {
                ID = FormulaHelper.CreateGuid(),
                Year = year,
                Month = month,
                Day = day,
                Date = new DateTime(year, month, day),
                IsHoliday = "2"
            };
            entity.DayOfWeek = ((DateTime)entity.Date).DayOfWeek.ToString();
            entities.Set<S_C_Holiday>().Add(entity);
            entities.SaveChanges();
            return Json("{}");
        }

        //设置 全年无休
        public JsonResult SetAllWorkDay(int year)
        {
            //所有双休日 设置为工作日
            string[] weekends = new string[]{DayOfWeek.Saturday.ToString(),DayOfWeek.Sunday.ToString()};
            var existList = this.entities.Set<S_C_Holiday>().Where(a => a.Year == year && weekends.Contains(a.DayOfWeek)).ToList();

            DateTime d = new DateTime(year, 1, 1);
            while (d.Year == year)
            {
                d = d.AddDays(1);
                if (d.DayOfWeek == DayOfWeek.Sunday || d.DayOfWeek == DayOfWeek.Saturday)
                {
                    var entity = existList.FirstOrDefault(a => a.Date == d);
                    if (entity == null)
                    {
                        entity = new S_C_Holiday();
                        entity.ID = FormulaHelper.CreateGuid();
                        entity.Year = d.Year;
                        entity.Month = d.Month;
                        entity.Day = d.Day;
                        entity.Date = d;
                        entity.DayOfWeek = ((DateTime)entity.Date).DayOfWeek.ToString();
                        entities.Set<S_C_Holiday>().Add(entity);
                    }
                    entity.IsHoliday = "0";
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        //取消 全年无休
        public JsonResult RevertAllWorkDay(int year)
        {
            //删除 双休日作为工作日的数据
            string[] weekends = new string[] { DayOfWeek.Saturday.ToString(), DayOfWeek.Sunday.ToString() };
            this.entities.Set<S_C_Holiday>().Delete(a => a.Year == year && weekends.Contains(a.DayOfWeek) && a.IsHoliday == "0");
            entities.SaveChanges();
            return Json("");
        }

        #endregion


    }
}
