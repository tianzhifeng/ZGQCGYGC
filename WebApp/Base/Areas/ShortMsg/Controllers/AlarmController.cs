using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Config;
using Formula;

namespace Base.Areas.ShortMsg.Controllers
{
    public class AlarmController : BaseController<S_S_Alarm>
    {
        public ActionResult AlarmList()
        {
            ViewBagTitle();
            return View();
        }
        public ActionResult AlarmDetail(string id)
        {
            ViewBagTitle();
            var model = entities.Set<S_S_Alarm>().FirstOrDefault(p => p.ID == id);
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qb"></param>
        /// <param name="important">重要度</param>
        /// <param name="urgency">紧急性</param>
        /// <returns></returns>
        public JsonResult GetAlarmList(MvcAdapter.QueryBuilder qb, string important, string urgency, string type)
        {
            /*不支持默认多个排序
            IQueryable<S_S_Alarm> data = entities.Set<S_S_Alarm>().Where(p => p.OwnerID == FormulaHelper.UserID && (string.IsNullOrEmpty(p.IsDelete) == true || p.IsDelete != "1"));
            if (!string.IsNullOrEmpty(important))
                data = data.Where(p => p.Important == important);
            if (!string.IsNullOrEmpty(urgency))
                data = data.Where(p => p.Urgency == urgency);

            if (!string.IsNullOrEmpty(type))
            {
                if (type.ToLower() == "last")//最新
                    data = data.Where(p => p.DeadlineTime >= DateTime.Now);
                else if (type.ToLower() == "history")//历史
                    data = data.Where(p => p.DeadlineTime < DateTime.Now);
            }
            return Json(data.WhereToGridData(qb));
            */

            string sql = string.Format(@"SELECT * FROM S_S_Alarm WHERE OwnerID='{0}' AND (IsDelete is null OR IsDelete='0')", FormulaHelper.UserID);
            if (!string.IsNullOrEmpty(important))
                sql += string.Format(" AND (Important ='{0}') ", important);
            if (!string.IsNullOrEmpty(urgency))
                sql += string.Format(" AND (Urgency ='{0}') ", urgency);

            if (!string.IsNullOrEmpty(type))
            {
                if (Config.Constant.IsOracleDb == false)
                {
                    if (type.ToLower() == "last")//最新
                        sql += string.Format(" AND (DeadlineTime >='{0}') ", DateTime.Now);
                    else if (type.ToLower() == "history")//历史
                        sql += string.Format(" AND (DeadlineTime <'{0}') ", DateTime.Now);
                }
                else
                {
                    if (type.ToLower() == "last")//最新
                        sql += string.Format(" AND (DeadlineTime >sysdate)");
                    else if (type.ToLower() == "history")//历史
                        sql += string.Format(" AND (DeadlineTime < sysdate) ");
                }
            }
            var data = SQLHelper.CreateSqlHelper("Base").ExecuteGridData(sql, qb);
            return Json(data);

        }
        public JsonResult GetLastAlarmList()
        {
            var user = FormulaHelper.GetUserInfo();
            List<S_S_Alarm> data = entities.Set<S_S_Alarm>().Where(p => p.OwnerID == user.UserID && (string.IsNullOrEmpty(p.IsDelete) == true || p.IsDelete != "T") && p.DeadlineTime >= DateTime.Now).OrderBy(p => p.DeadlineTime).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 标记为假删除.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult DeleteAlarm(string ids)
        {
            List<S_S_Alarm> list = entities.Set<S_S_Alarm>().Where(p => ids.IndexOf(p.ID) >= 0).ToList();
            foreach (var item in list)
                item.IsDelete = "1";
            entities.SaveChanges();
            return Json("");
        }

        private void ViewBagTitle()
        {
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            ViewBag.Title = LGID ? "Title" : "标题";
            ViewBag.Urgent = LGID ? "Urgent" : "紧急度";
            ViewBag.Importance = LGID ? "Importance" : "重要度";
            ViewBag.Last = LGID ? "Last" : "最新";
            ViewBag.History = LGID ? "History" : "历史";
            ViewBag.All = LGID ? "All" : "全部";
            ViewBag.LevelFirst = LGID ? "IU." : "重要紧急";
            ViewBag.LevelSecond = LGID ? "Important" : "重要";
            ViewBag.LevelThird = LGID ? "Urgent" : "紧急";
            ViewBag.LevelFourth = LGID ? "Commonly" : "一般";
            ViewBag.Delete= LGID ? "Delete" : "删除提醒";
            ViewBag.Deadline = LGID ? "Deadline" : "截止时间";
            ViewBag.SendTime = LGID ? "Time" : "发送时间";
            ViewBag.ReceiverName = LGID ? "Recipient" : "接收人";
            ViewBag.DetailedInquiry = LGID ? "Query" : "详细查询";
            ViewBag.QueryInput = LGID ? "Please Type And Title" : "请输入类型或标题";
            ViewBag.Filter = LGID ? "Filter" : "过滤";
            ViewBag.Type = LGID ? "Type" : "类型";
            ViewBag.Handle = LGID ? "Handle" : "快速办理";
            ViewBag.Content = LGID ? "Content" : "内容";
            ViewBag.TO = LGID ? "TO" : "至";
            ViewBag.Query = LGID ? "Query" : "查询";
            ViewBag.Clear = LGID ? "Clear" : "清空";
        }
        public override ActionResult List()
        {
            ViewBagTitle();
            return View();
        }
    }
}
