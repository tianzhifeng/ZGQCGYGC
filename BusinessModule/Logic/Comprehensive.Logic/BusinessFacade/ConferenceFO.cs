using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Config;
using Formula;
using Formula.Helper;
using MvcAdapter;
using System.Collections;
using Comprehensive.Logic.Domain;

namespace Comprehensive.Logic
{
    public class ConferenceFO
    {
        #region 转换时间

        public static DateTime ConvertTime(string sTime)
        {
            string sReturn = "";
            string[] sTimes = sTime.Split('-');
            foreach (string s in sTimes)
            {
                if (s.Length == 1)
                {
                    sReturn += "0" + s + '-';
                }
                else
                {
                    sReturn += s + '-';
                }
            }
            sReturn = sReturn.TrimEnd('-');
            return Convert.ToDateTime(sReturn);
        }

        #endregion

        #region 获取相关配置

        /// <summary>
        /// 未暂用
        /// </summary>
        public static string ConferenceUnOccupy
        {
            get
            {
                return @"<img src=""/Comprehensive/CusStyle/images_conference/add.png"" width=""24"" height=""24"" align=""absmiddle"" onclick=""LinkToThis('{0}','{1}','{2}');""/>";
            }
        }

        /// <summary>
        /// 暂用
        /// </summary>
        public static string ConferenceOccupy
        {
            get
            {
                return @"<img src=""/Comprehensive/CusStyle/images_conference/date.png"" title='已被""{0}""会议占用' width=""24"" height=""24"" align=""absmiddle"" onclick=""LinkToView('{1}');""/>";
            }
        }

        /// <summary>
        /// 会议室使用情况描述主体
        /// </summary>
        public static string MeetingBody
        {
            get
            {
                string temp = @"<tr><td align=""center"" class=""bm-grid-celll"">{0}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{1}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{2}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{3}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{4}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{5}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{6}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{7}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{8}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{9}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{10}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{11}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{12}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{13}</td><td align=""center"" valign=""middle"" class=""bm-grid-cellf "">{14}</td></tr>";
                return temp;
            }
        }
        /// <summary>
        /// 上午会议场数
        /// </summary>
        public static int AmConferenceNum
        {
            get
            {
                int num = 20;
                string strNum = System.Configuration.ConfigurationManager.AppSettings["AmConferenceNum"];
                if (!string.IsNullOrWhiteSpace(strNum))
                    num = Convert.ToInt32(strNum);
                return num;
            }
        }
        /// <summary>
        /// 上午时段开始时间（整数）
        /// </summary>
        public static int AmStart
        {
            get
            {
                int num = 7;
                string strNum = System.Configuration.ConfigurationManager.AppSettings["AmStart"];
                if (!string.IsNullOrWhiteSpace(strNum))
                    num = Convert.ToInt32(strNum);
                return num;
            }
        }
        /// <summary>
        /// 上午时段结束时间（整数）
        /// </summary>
        public static int AmEnd
        {
            get
            {
                int num = 12;
                string strNum = System.Configuration.ConfigurationManager.AppSettings["AmEnd"];
                if (!string.IsNullOrWhiteSpace(strNum))
                    num = Convert.ToInt32(strNum);
                return num;
            }
        }

        /// <summary>
        /// 时段内可申请会议条件限制
        /// </summary>
        public static int Limit
        {
            get
            {
                int num = 1;
                string strNum = System.Configuration.ConfigurationManager.AppSettings["Limit"];
                if (!string.IsNullOrWhiteSpace(strNum))
                    num = Convert.ToInt32(strNum);
                return num;
            }
        }

        /// <summary>
        /// 下午会议场数
        /// </summary>
        public static int PmConferenceNum
        {
            get
            {
                int num = 36;
                string strNum = System.Configuration.ConfigurationManager.AppSettings["PmConferenceNum"];
                if (!string.IsNullOrWhiteSpace(strNum))
                    num = Convert.ToInt32(strNum);
                return num;
            }
        }

        /// <summary>
        /// 下午时段开始时间（整数）
        /// </summary>
        public static int PmStart
        {
            get
            {
                int num = 12;
                string strNum = System.Configuration.ConfigurationManager.AppSettings["PmStart"];
                if (!string.IsNullOrWhiteSpace(strNum))
                    num = Convert.ToInt32(strNum);
                return num;
            }
        }

        /// <summary>
        /// 下午时段结束时间（整数）
        /// </summary>
        public static int PmEnd
        {
            get
            {
                int num = 21;
                string strNum = System.Configuration.ConfigurationManager.AppSettings["PmEnd"];
                if (!string.IsNullOrWhiteSpace(strNum))
                    num = Convert.ToInt32(strNum);
                return num;
            }
        }

        #endregion

        /// <summary>
        /// 给参与人员发消息
        /// </summary>
        /// <param name="oldMeeting"></param>
        /// <param name="meeting"></param>
        /// <param name="funcType"></param>
        public static void SendMsgToJoiner(T_M_ConferenceApply oldMeeting, T_M_ConferenceApply meeting, string funcType)
        {
            string joinUserId = meeting.JoinUser;
            if (string.IsNullOrWhiteSpace(joinUserId))
                return;
            string joinUserName = meeting.JoinUserName;
            string title = "会议通知";
            string content = string.Empty;
            string url = "/OfficeAuto/AutoForm/MConferenceApply/Edit?FuncType=view&ID=" + meeting.ID;
            if (funcType == "insert")
            {
                content = "您好，诚邀您于【" + meeting.MeetingStart + "】在【" + meeting.MeetingRoomName + "】参加主题为【" + meeting.Title +
                          "】的会议,主持人是【" + meeting.HostName + "】,会议联系人【" + meeting.ApplyUserName + "】。请您准时参加。";
            }
            else if (funcType == "update")
            {
                title = "会议变更通知";
                content = "您好，您于【" + oldMeeting.MeetingStart + "】在【" + oldMeeting.MeetingRoomName + "】主题为【" +
                          oldMeeting.Title + "】的会议已修改，修改后的情况如下：\r\n";
                content += "时间：" + meeting.MeetingStart + " " + meeting.MeetingStartHour + "时" + meeting.MeetingStartMin +
                           "分";
                content += "\r\n\t地点:" + meeting.MeetingRoomName;
                content += "\r\n\t主题：" + meeting.Title;
                content += "\r\n\t参会人员：" + meeting.JoinUserName;
                content += "\r\n\t会议联系人：" + meeting.ApplyUserName;
            }
            else if (funcType == "delete")
            {
                title = "会议取消通知";
                content = "您好，您于【" + meeting.MeetingStart + "】在【" + meeting.MeetingRoomName + "】主题为【" + meeting.Title +
                          "】的会议已被取消，,会议联系人【" + meeting.ApplyUserName + "】,在此通知。";
                url = "";
            }

            FormulaHelper.GetService<IMessageService>().SendMsg(title, content, url, "", joinUserId, joinUserName);
        }

        /// <summary>
        /// 拼接table
        /// </summary>
        /// <param name="dtMonday"></param>
        /// <param name="dtMondayUpStart"></param>
        /// <param name="dtMondayUpEnd"></param>
        /// <param name="dtMondayDownStart"></param>
        /// <param name="dtMondayDownEnd"></param>
        /// <param name="show"></param>
        /// <returns></returns>
        public static string GetTables(DateTime dtMonday, DateTime dtMondayUpStart, DateTime dtMondayUpEnd,
            DateTime dtMondayDownStart, DateTime dtMondayDownEnd, bool show = false)
        {
            SQLHelper OfficeAutoDb = SQLHelper.CreateSqlHelper(ConnEnum.Comprehensive);
            #region SQL String

            const string strTemp = @"SELECT '{0}' as Week,S_M_ConferenceRoom.Id AS MetroomId,S_M_ConferenceRoom.ROOMNAME,
COUNT(Temp.MeetingRoomName) AS IsItEmpty,Temp.Title,Temp.Id,Temp.MeetingStartHour,
Temp.MeetingStartMin,Temp.MeetingEndHour,Temp.MeetingEndMin FROM S_M_ConferenceRoom
right join (select * from T_M_ConferenceApply where  (T_M_ConferenceApply.MeetingStart <='{1}' 
and T_M_ConferenceApply.MeetingEnd>'{1}')
or (T_M_ConferenceApply.MeetingStart <'{2}' and T_M_ConferenceApply.MeetingEnd>='{2}')
or (T_M_ConferenceApply.MeetingStart >='{1}' and T_M_ConferenceApply.MeetingEnd<='{2}')) Temp 
on Temp.MeetingRoom=S_M_ConferenceRoom.Id GROUP BY Temp.MeetingEndHour,Temp.MeetingEndMin,
S_M_ConferenceRoom.Id,Temp.MeetingStartHour,Temp.MeetingStartMin,S_M_ConferenceRoom.ROOMNAME,
Temp.Title,Temp.Id
";

            string sSqlAll = String.Format(strTemp, "MondayDown", dtMondayDownStart.AddDays(0), dtMondayDownEnd.AddDays(0)) + " union ";

            sSqlAll += String.Format(strTemp, "TuesdayDown", dtMondayDownStart.AddDays(1), dtMondayDownEnd.AddDays(1)) + " union ";

            sSqlAll += String.Format(strTemp, "WednesdayDown", dtMondayDownStart.AddDays(2), dtMondayDownEnd.AddDays(2)) + " union ";

            sSqlAll += String.Format(strTemp, "ThursdayDown", dtMondayDownStart.AddDays(3), dtMondayDownEnd.AddDays(3)) + " union ";

            sSqlAll += String.Format(strTemp, "FridayDown", dtMondayDownStart.AddDays(4), dtMondayDownEnd.AddDays(4)) + " union ";

            sSqlAll += String.Format(strTemp, "SaturdayDown", dtMondayDownStart.AddDays(5), dtMondayDownEnd.AddDays(5)) + " union ";

            sSqlAll += String.Format(strTemp, "SundayDown", dtMondayDownStart.AddDays(6), dtMondayDownEnd.AddDays(6)) + " union ";

            sSqlAll += String.Format(strTemp, "MondayUp", dtMondayUpStart.AddDays(0), dtMondayUpEnd.AddDays(0)) + " union ";

            sSqlAll += String.Format(strTemp, "TuesdayUp", dtMondayUpStart.AddDays(1), dtMondayUpEnd.AddDays(1)) + " union ";

            sSqlAll += String.Format(strTemp, "WednesdayUp", dtMondayUpStart.AddDays(2), dtMondayUpEnd.AddDays(2)) + " union ";

            sSqlAll += String.Format(strTemp, "ThursdayUp", dtMondayUpStart.AddDays(3), dtMondayUpEnd.AddDays(3)) + " union ";

            sSqlAll += String.Format(strTemp, "FridayUp", dtMondayUpStart.AddDays(4), dtMondayUpEnd.AddDays(4)) + " union ";

            sSqlAll += String.Format(strTemp, "SaturdayUp", dtMondayUpStart.AddDays(5), dtMondayUpEnd.AddDays(5)) + " union ";

            sSqlAll += String.Format(strTemp, "SundayUp", dtMondayUpStart.AddDays(6), dtMondayUpEnd.AddDays(6));

            #endregion

            DataTable tbConference = OfficeAutoDb.ExecuteDataTable(sSqlAll);
            DataTable tbRooms = OfficeAutoDb.ExecuteDataTable("select Id,RoomAddress from S_M_ConferenceRoom");

            string responeHtml = "";

            bool isShow = false;
            string systemWeek = DateTime.Now.DayOfWeek.ToString();
            if (dtMonday > DateTime.Now || show)
            {
                isShow = true;
            }

            foreach (DataRow dr in tbRooms.Rows)
            {
                var roomId = dr[0].ToString();
                //星期一上午会议
                string s1Up = getTdInfo(roomId, tbConference, isShow, "Monday", "Up", dtMondayUpEnd, systemWeek, 0, 0);

                //星期一下午会议
                string s1Down = getTdInfo(roomId, tbConference, isShow, "Monday", "Down", dtMondayDownEnd, systemWeek, 0, 1);

                //星期二上午会议
                string s2Up = getTdInfo(roomId, tbConference, isShow, "Tuesday", "Up", dtMondayUpEnd, systemWeek, 1, 0);

                //星期二下午会议
                string s2Down = getTdInfo(roomId, tbConference, isShow, "Tuesday", "Down", dtMondayDownEnd, systemWeek, 1, 1);

                //星期三上午会议
                string s3Up = getTdInfo(roomId, tbConference, isShow, "Wednesday", "Up", dtMondayUpEnd, systemWeek, 2, 0);

                //星期三下午会议
                string s3Down = getTdInfo(roomId, tbConference, isShow, "Wednesday", "Down", dtMondayDownEnd, systemWeek, 2, 1);

                //星期四上午会议
                string s4Up = getTdInfo(roomId, tbConference, isShow, "Thursday", "Up", dtMondayUpEnd, systemWeek, 3, 0);

                //星期四下午会议
                string s4Down = getTdInfo(roomId, tbConference, isShow, "Thursday", "Down", dtMondayDownEnd, systemWeek, 3, 1);

                //星期五上午会议
                string s5Up = getTdInfo(roomId, tbConference, isShow, "Friday", "Up", dtMondayUpEnd, systemWeek, 4, 0);

                //星期五下午会议
                string s5Down = getTdInfo(roomId, tbConference, isShow, "Friday", "Down", dtMondayDownEnd, systemWeek, 4, 1);

                //星期六上午会议
                string s6Up = getTdInfo(roomId, tbConference, isShow, "Saturday", "Up", dtMondayUpEnd, systemWeek, 5, 0);

                //星期六下午会议
                string s6Down = getTdInfo(roomId, tbConference, isShow, "Saturday", "Down", dtMondayDownEnd, systemWeek, 5, 1);

                //星期天上午会议
                string s7Up = getTdInfo(roomId, tbConference, isShow, "Sunday", "Up", dtMondayUpEnd, systemWeek, 6, 0);

                //星期天下午会议
                string s7Down = getTdInfo(roomId, tbConference, isShow, "Sunday", "Down", dtMondayDownEnd, systemWeek, 6, 1);

                responeHtml += String.Format(ConferenceFO.MeetingBody, dr[1], s1Up, s1Down, s2Up, s2Down, s3Up, s3Down, s4Up, s4Down, s5Up, s5Down, s6Up, s6Down, s7Up, s7Down);
            }

            return responeHtml;
        }

        /// <summary>
        /// 获取TD里的信息
        /// </summary>
        /// <param name="roomId">会议室ID</param>
        /// <param name="tbConference">会议信息列表</param>
        /// <param name="isShow">是否显示</param>
        /// <param name="week">星期</param>
        /// <param name="upOrDown">上午"Up"或下午"Down"</param>
        /// <param name="dtMondayUpOrDownEnd">星期一上午或下午的结束时间</param>
        /// <param name="systemWeek">今天星期</param>
        /// <param name="nT">今天星期数字表示</param>
        /// <param name="nUPorDown">上午下午数字表示</param>
        /// <returns></returns>
        private static string getTdInfo(string roomId, DataTable tbConference, bool isShow, string week, string upOrDown, DateTime dtMondayUpOrDownEnd, string systemWeek, int nT, int nUPorDown)
        {
            var str = "";
            if (isShow || bigDateOfWeek(systemWeek, week, dtMondayUpOrDownEnd, nT, nUPorDown))
            {
                var thisDay = week + upOrDown;
                var query = tbConference.Select("Week='" + thisDay + "' and MetroomId='" + roomId + "'").ToList();
                int start;
                int end;
                int nowLimit;
                if (upOrDown == "Up")
                {
                    start = ConferenceFO.AmStart;
                    end = ConferenceFO.AmEnd;
                    nowLimit = ConferenceFO.AmConferenceNum;
                }
                else
                {
                    start = ConferenceFO.PmStart;
                    end = ConferenceFO.PmEnd;
                    nowLimit = ConferenceFO.PmConferenceNum;
                }

                if (query.Count == 0)
                {
                    str = String.Format(ConferenceUnOccupy, roomId, dtMondayUpOrDownEnd.AddDays(nT).ToShortDateString(), upOrDown);
                }
                else if (query.Count == nowLimit)
                {
                    string[] titles = new string[nowLimit];
                    string[] ids = new string[nowLimit];
                    for (int i = 0; i < query.Count; i++)
                    {
                        titles[i] = query[i]["Title"].ToString();
                        ids[i] = query[i]["Id"].ToString();
                    }
                    str = String.Format(ConferenceOccupy, String.Join(",", titles), String.Join(",", ids));
                }
                else
                {
                    var lastEndHour = Convert.ToInt32(query[0]["MeetingEndHour"].ToString());
                    var firtStartHour = Convert.ToInt32(query[0]["MeetingStartHour"].ToString());

                    for (int i = 1; i < query.Count; i++)
                    {
                        var endHour = Convert.ToInt32(query[i]["MeetingEndHour"].ToString());
                        var startHour = Convert.ToInt32(query[i]["MeetingStartHour"].ToString());
                        if (lastEndHour < endHour)
                        {
                            lastEndHour = endHour;
                        }
                        if (firtStartHour > startHour)
                        {
                            firtStartHour = startHour;
                        }
                    }

                    if ((end - lastEndHour >= ConferenceFO.Limit) || (firtStartHour - start >= ConferenceFO.Limit))
                    {
                        //2017-11-20 判断跨天至上午
                        var flag = true;
                        if (upOrDown != "Up")
                        {
                            if (lastEndHour <= 12)
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                        //可以继续申请会议
                        str = String.Format(ConferenceFO.ConferenceUnOccupy, roomId, dtMondayUpOrDownEnd.AddDays(nT).ToShortDateString(), upOrDown);
                        }

                    }

                    string[] titles = new string[query.Count];
                    string[] ids = new string[query.Count];
                    for (int i = 0; i < query.Count; i++)
                    {
                        titles[i] = query[i]["Title"].ToString();
                        ids[i] = query[i]["Id"].ToString();
                    }
                    str += String.Format(ConferenceFO.ConferenceOccupy, String.Join(",", titles), String.Join(",", ids));
                }
            }
            else
            {
                str = "-";
            }

            return @str;
        }


        private static bool bigDateOfWeek(string systemWeek, string valueWeek, DateTime dtMondayDownStart, int nT,
            int nUPorDown)
        {
            DateTime dtTemp = dtMondayDownStart.AddDays(nT);
            const string strWeek = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday";
            if (strWeek.IndexOf(systemWeek, StringComparison.Ordinal) <= strWeek.IndexOf(valueWeek, StringComparison.Ordinal))
            {
                if (nUPorDown == 0 && systemWeek == DateTime.Today.DayOfWeek.ToString())
                {
                    if (DateTime.Now > dtTemp)
                        return false;
                    return true;
                }
                if (nUPorDown == 1 && systemWeek == DateTime.Today.DayOfWeek.ToString())
                {
                    if (DateTime.Now > dtTemp)
                        return false;
                    return true;
                }
                return true;
            }
            return false;
        }

        #region 日期时间拼接

        public static DateTime JoinDateTime(DateTime date,string hour,string min)
        {
            DateTime newDate =new DateTime();
            newDate = date.AddHours(double.Parse(hour)).AddMinutes(double.Parse(min));
            return newDate;
        }

        #endregion
    }
}
