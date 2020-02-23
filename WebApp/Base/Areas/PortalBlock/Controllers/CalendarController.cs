using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula;
using Config;
using System.Data;
using MvcAdapter;

namespace Base.Areas.PortalBlock.Controllers
{
    public class CalendarController : BaseController
    {

        public JsonResult GetModel()
        {
            return Json("");
        }

        //
        // GET: /PortalBlock/Calendar/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Edit(string start, string end, string allDay)
        {
            return View();
        }

        public JsonResult Save()
        {
            S_H_Calendar model = UpdateEntity<S_H_Calendar>();
            entities.SaveChanges();
            return Json(model);
        }

        public JsonResult Delete(string id)
        {
            return base.JsonDelete<S_H_Calendar>(id);
        }

        public JsonResult GetPersonalSchedules(string start, string end)
        {
            string userID = FormulaHelper.GetUserInfo().UserID;
            IQueryable<S_H_Calendar> query = entities.Set<S_H_Calendar>().Where(c => c.CreateUserID == userID);
            if (!string.IsNullOrEmpty(start))
            {
                DateTime dateS = Convert.ToDateTime(start);
                query = query.Where(c => c.EndTime == null || c.EndTime >= dateS);
            }
            if (!string.IsNullOrEmpty(end))
            {
                DateTime dateE = Convert.ToDateTime(end);
                query = query.Where(c => c.StartTime < dateE);
            }
            List<S_H_Calendar> listSchedule = query.ToList();

            var listEvent = SchedulesToEvents(listSchedule);

            var tasks = GetTasks(userID, start, end);
            foreach (DataRow row in tasks.Rows)
            {
                Event ev = new Event();
                ev.id = row["ID"].ToString();
                ev.title = row["Name"].ToString();
                if (row["CreateTime"] != null && row["CreateTime"] != DBNull.Value)
                    ev.start = Convert.ToDateTime(row["CreateTime"]).Date;
                if (row["DeadLine"] != null && row["DeadLine"] != DBNull.Value)
                    ev.end = Convert.ToDateTime(row["DeadLine"]).Date;
                ev.allDay = true;
                ev.editable = false;
                ev.startEditable = false;
                ev.endEditable = false;
                ev.type = EventType.Schedule;
                ev.description = "task";//用于区别schedule
                listEvent.Add(ev);
            }

            return Json(listEvent, JsonRequestBehavior.AllowGet);
        }

        private List<Event> GetPersonalSchedules(MvcAdapter.QueryBuilder qb)
        {
            string userID = FormulaHelper.GetUserInfo().UserID;
            IQueryable<S_H_Calendar> query = entities.Set<S_H_Calendar>().Where(c => c.CreateUserID == userID);
            List<S_H_Calendar> list = query.Where(qb).ToList();
            return SchedulesToEvents(list);
        }

        private List<Event> SchedulesToEvents(List<S_H_Calendar> list)
        {
            List<Event> listEvent = new List<Event>();
            foreach (S_H_Calendar item in list)
            {
                Event ev = new Event();
                ev.id = item.ID;
                ev.title = item.Title;
                ev.description = item.Description;
                ev.start = Convert.ToDateTime(item.StartTime);
                ev.end = Convert.ToDateTime(item.EndTime);
                ev.allDay = Convert.ToDateTime(ev.start).ToString("T") == "0:00:00" && Convert.ToDateTime(ev.end).ToString("T") == "0:00:00" ? true : false;
                ev.editable = true;
                ev.startEditable = true;
                ev.endEditable = true;
                ev.type = EventType.Schedule;
                listEvent.Add(ev);
            }
            return listEvent;
        }

        private DataTable GetTasks(string userId, string start, string end)
        {
            string sql = @"
select S_T_Task.ID,Name,S_T_Task.DeadLine,S_T_TaskExec.CreateTime from S_T_Task join S_T_TaskExec on TaskID=S_T_Task.ID where ExecUserID='{0}' and S_T_TaskExec.ExecTime is null
";
            sql = string.Format(sql, userId);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            return dt;
        }

        public JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            string sqlBase = "select id,starttime, endtime , title,description , '' as url,'' as width,'' as height,'Schedule' as \"type\",'' as FormInstanceID from S_H_Calendar where CreateUserID = '{0}' ";
            string userID = FormulaHelper.GetUserInfo().UserID;
            sqlBase = string.Format(sqlBase, userID);
            SQLHelper baseSqlHelper = SQLHelper.CreateSqlHelper("Base");
            var dt = baseSqlHelper.ExecuteDataTable(sqlBase, qb);
            var gridData = new GridData(dt);
            gridData.total = qb.TotolCount;
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
    }

    public class Event
    {
        public string id;
        public DateTime start;
        public DateTime end;
        public string title;
        public string description;
        public bool allDay;
        public string url;
        public string width;
        public string height;
        public bool editable;
        public bool startEditable;
        public bool endEditable;
        public EventType type;
    }

    public enum EventType
    {
        Schedule,
        Task,
        News,
    }
}
