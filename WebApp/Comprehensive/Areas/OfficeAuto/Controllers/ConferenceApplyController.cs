using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Config;
using MvcAdapter;
using Formula;
using System.Data;

namespace Comprehensive.Areas.OfficeAuto.Controllers
{
    public class ConferenceApplyController : ComprehensiveFormController<T_M_ConferenceApply>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                string dayType = this.GetQueryString("DayType"); //Request.QueryString["DayType"] == "Up"
                if (dayType.ToLower() == "up")
                {
                    dt.Rows[0]["MeetingStartHour"] = ConferenceFO.AmStart;
                    dt.Rows[0]["MeetingEndHour"] = ConferenceFO.AmEnd;
                }
                else
                {
                    dt.Rows[0]["MeetingStartHour"] = ConferenceFO.PmStart;
                    dt.Rows[0]["MeetingEndHour"] = ConferenceFO.PmEnd;
                }
                dt.Rows[0]["MeetingStartMin"] = "0";
                dt.Rows[0]["MeetingEndMin"] = "0";
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var id = dic.GetValue("ID");

            var startDate = ConferenceFO.JoinDateTime(Convert.ToDateTime(Convert.ToDateTime(dic.GetValue("MeetingStart")).ToShortDateString()), dic.GetValue("MeetingStartHour"), dic.GetValue("MeetingStartMin"));
            dic.SetValue("MeetingStart", startDate.ToString());
            var endDate = ConferenceFO.JoinDateTime(Convert.ToDateTime(Convert.ToDateTime(dic.GetValue("MeetingEnd")).ToShortDateString()), dic.GetValue("MeetingEndHour"), dic.GetValue("MeetingEndMin"));
            dic.SetValue("MeetingEnd", endDate.ToString());

            string sql = "select count(0) from T_M_ConferenceApply where ID!='{0}' and MeetingRoom='{1}' and MeetingEnd>'{2}' and MeetingStart<'{3}'";
            var obj = this.ComprehensiveSQLDB.ExecuteScalar(String.Format(sql, dic.GetValue("ID"), dic.GetValue("MeetingRoom"),
                 dic.GetValue("MeetingStart"), dic.GetValue("MeetingEnd")));
            if (Convert.ToInt32(obj) > 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("会议预定申请的时间与已预定申请的会议时间有冲突，请确认！");
            }
        }

        #region 会议室申请

        public JsonResult DoDelete(string id)
        {
            var mo = this.ComprehensiveDbContext.Set<T_M_ConferenceApply>().FirstOrDefault(p => p.ID == id);
            string fid = "";
            if (mo != null)
            {
                ConferenceFO.SendMsgToJoiner(mo, mo, "delete"); //给参与人发送消息
                fid = mo.ID;
            }
            this.ComprehensiveDbContext.Set<T_M_ConferenceApply>().Remove(mo);
            this.ComprehensiveDbContext.SaveChanges();

            #region 删除相应流程记录信息

            SQLHelper shWf = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            string strSql = string.Format("Delete From S_WF_InsFlow Where FormInstanceID='{0}'", fid);
            shWf.ExecuteNonQuery(strSql);
            #endregion

            return Json("");
        }

        public JsonResult GetConferenceApplyList(QueryBuilder qb)
        {
            var user = FormulaHelper.GetUserInfo();
            var data = entities.Set<T_M_ConferenceApply>().Where(c => c.ApplyUser == user.UserID).WhereToGridData(qb);
            return Json(data);
        }


        public JsonResult GetApplyList(QueryBuilder qb)
        {
            var user = FormulaHelper.GetUserInfo();
            var ids = Request.QueryString.Get("IDs");
            if (!String.IsNullOrWhiteSpace(ids))
            {

                string sql = string.Format("Select * From T_M_ConferenceApply Where ID in ('{0}')", ids.Replace(",", "','"));
                var dt1 = this.ComprehensiveSQLDB.ExecuteList<T_M_ConferenceApply>(sql);
                return Json(dt1);
            }
            string strSql = string.Format("Select * From T_M_ConferenceApply Where State='" + "未召开" + "' ", user.UserID);
            GridData dt = this.ComprehensiveSQLDB.ExecuteGridData(strSql, qb);
            return Json(dt);
        }
        #endregion

        #region 会议室可申请拼接HTML

        public JsonResult Weeks()
        {
            DateTime dtMonday;
            if (string.IsNullOrEmpty(Request["Monday"]))
            {
                dtMonday =
                    DateTime.Today.AddDays(Convert.ToDouble((0 - Convert.ToInt16(DateTime.Now.AddDays(-1).DayOfWeek))));
            }
            else
            {
                DateTime dTemp = DateTime.Parse(Request["Monday"]);
                dtMonday = dTemp;
            }
            DateTime dtMondayUpStart = dtMonday.AddHours(ConferenceFO.AmStart);
            DateTime dtMondayUpEnd = dtMonday.AddHours(ConferenceFO.AmEnd);
            DateTime dtMondayDownStart = dtMonday.AddHours(ConferenceFO.PmStart);
            DateTime dtMondayDownEnd = dtMonday.AddHours(ConferenceFO.PmEnd);

            string MeetingTable = ConferenceFO.GetTables(dtMonday, dtMondayUpStart, dtMondayUpEnd, dtMondayDownStart, dtMondayDownEnd, true);
            ViewBag.MeetingBody = MeetingTable;
            return Json(ViewBag);
        }
        #endregion

    }
}
