using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Config;
using Formula;
using MvcAdapter;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto.Controllers
{
    public class SurveyController : BaseController
    {
        private readonly SQLHelper sh = SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);
        private readonly UserInfo user = FormulaHelper.GetUserInfo();
        private string tittle = "";

        public ActionResult SurveyList()
        {
            string questIDs = "";
            string id = GetQueryString("ID");

            string strSql1 = "Select * From S_Survey_Subject where ID='" + id + "'";
            DataTable dt1 = sh.ExecuteDataTable(strSql1);
            ViewBag.tittle = dt1.Rows[0]["SurveyName"] + "问卷调查";
            tittle = ViewBag.tittle;
            ViewBag.Writer = dt1.Rows[0]["IssueName"].ToString();


            if (dt1.Rows[0]["SurveyAttchment"].ToString() != "")
            {
                string url = ConfigurationManager.AppSettings["FS_MasterServerUrl"];
                url = url.Split(new[] { "Services" }, StringSplitOptions.RemoveEmptyEntries).First();
                ViewBag.Attachment = "<a href='" + url + "Download.aspx?fileid=" + dt1.Rows[0]["SurveyAttchment"] +
                                     "'>点击查看附件</a>"; //附件
            }
            else ViewBag.Attachment = "<a>无附件</a>";


            string htr = "";
            string strSql3 = "";
            string type = ""; //问题类别
            DataTable dt3;
            string strSql2 = "Select * From S_Survey_Question where SurveyID='" + dt1.Rows[0]["ID"] + "'";
            DataTable dt2 = sh.ExecuteDataTable(strSql2);
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                object questionId = dt2.Rows[i]["ID"];
                questIDs += questionId + "|" + dt2.Rows[i]["QuestionType"] + "|" + dt2.Rows[i]["IsMustAnswer"] +
                            ",";
                type = "";
                if (dt2.Rows[i]["IsMustAnswer"].ToString() == "1")
                {
                    type += "*";
                }
                if (dt2.Rows[i]["QuestionType"].ToString() == "文本")
                {
                    type += "【填空题】";
                }
                else type += "【" + dt2.Rows[i]["QuestionType"] + "】";

                if (dt2.Rows[i]["IsComment"].ToString() == "1")
                {
                    htr += string.Format(@"<tr>
                                    <td colspan='3'><div class='surveyResultQu'>
                                                <div class='r-qu-body-title' id='{0}'>{1}{2}{3}
                                                    <img src='/OfficeAuto/CusStyle/images_files/comment_chat.png'
                                                        alt='评论' width='24' height='24' border='0' align='absmiddle' onclick=" +
                                         "\"OpenComment('" + questionId + "');\"" + @"/>
                                                </div>
                                                <div class='r-qu-body-content'>
                                                    <table class='suQuTable' border='0' cellspacing='0' cellpadding='0'>
                                                        <tbody>", questionId + "_1", (i + 1).ToString() + "、",
                                         dt2.Rows[i]["QuestionName"], type);
                }
                else
                {
                    htr += string.Format(@"<tr>
                                    <td colspan='3'><div class='surveyResultQu'>
                                                <div class='r-qu-body-title' id='{0}'>{1}{2}{3}
                                                </div>
                                                <div class='r-qu-body-content'>
                                                    <table class='suQuTable' border='0' cellspacing='0' cellpadding='0'>
                                                        <tbody>", questionId + "_1", (i + 1).ToString() + "、",
                                         dt2.Rows[i]["QuestionName"], type);
                }

                if (dt2.Rows[i]["QuestionType"].ToString() == "文本")
                {
                    //获取问题答案
                    string sqlMsg = "select * from S_Survey_QuestionVote where QuestionID='" + questionId +
                                    "' and UserID='" + user.UserID + "'";
                    DataTable dtTemp = sh.ExecuteDataTable(sqlMsg);
                    string content = "";
                    if (dtTemp.Rows.Count > 0)
                    {
                        content = dtTemp.Rows[0]["QuestionMsg"].ToString();
                    }

                    htr += string.Format(@"<tr>
                                                                <td width='5'>
                                                                </td>
                                                                <td class='bfbTd'>
                                                                    <label for='textarea'>
                                                                    </label>
                                                                    <textarea name='{0}' id='{0}' cols='45' rows='5' style='width: 100%'>{1}</textarea>
                                                                </td>
                                                                <td colspan='4'>
                                                                    &nbsp;
                                                                </td>
                                                            </tr>", questionId, content);
                }
                else if (dt2.Rows[i]["QuestionType"].ToString() == "单选")
                {
                    strSql3 = "select * from S_Survey_Option where QuestionID='" + questionId + "'";
                    dt3 = sh.ExecuteDataTable(strSql3);
                    const char m = 'A';
                    for (int j = 0; j < dt3.Rows.Count; j++)
                    {
                        //获取选中项
                        string sqlVote = "select * from S_Survey_Vote where OptionID='" + dt3.Rows[j]["ID"] +
                                         "' and UserID='" + user.UserID + "'";
                        DataTable dtTemp = sh.ExecuteDataTable(sqlVote);
                        bool isChecked = dtTemp.Rows.Count > 0;

                        htr += string.Format(@"<tr>
                                                                <td width='30'>
                                                                    <input type='radio' name='{0}' id='{1}' value='{1}' {4}/>
                                                                </td>
                                                                <td>
                                                                    {2}、{3}
                                                                </td>
                                                                <td width='20'>
                                                                    &nbsp;
                                                                </td>
                                                            </tr>", questionId, dt3.Rows[j]["ID"],
                                             Convert.ToChar(m + j), dt3.Rows[j]["OptionName"],
                                             isChecked ? "checked" : "");
                    }
                }
                else if (dt2.Rows[i]["QuestionType"].ToString() == "多选")
                {
                    strSql3 = "select * from S_Survey_Option where QuestionID='" + questionId + "'";
                    dt3 = sh.ExecuteDataTable(strSql3);
                    char m = 'A';
                    for (int j = 0; j < dt3.Rows.Count; j++)
                    {
                        //获取选中项
                        object optionId = dt3.Rows[j]["ID"];
                        string sqlVote = "select * from S_Survey_Vote where OptionID='" + optionId + "' and UserID='" +
                                         user.UserID + "'";
                        DataTable dtTemp = sh.ExecuteDataTable(sqlVote);
                        bool isChecked = dtTemp.Rows.Count > 0;

                        htr += string.Format(@"<tr>
                                                                <td width='30'>
                                                                    <input type='checkbox' name='{0}' id='{1}' {4}/>
                                                                </td>
                                                                <td>
                                                                    {2}、{3}
                                                                </td>
                                                                <td width='20'>
                                                                    &nbsp;
                                                                </td>
                                                            </tr>", questionId, dt3.Rows[j]["ID"],
                                             Convert.ToChar(m + j), dt3.Rows[j]["OptionName"],
                                             isChecked ? "checked" : "");
                    }
                }

                htr += @"                                </tbody>
                                                    </table>
                                                    <div style='clear: both;'>
                                                    </div>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>";
            }
            ViewBag.htr = htr;
            ViewBag.QuestIDs = questIDs.Substring(0, questIDs.Length - 1);

            return View(ViewBag);
        }

        public ActionResult Comment()
        {
            string id = GetQueryString("QuesID");
            string strSql1 = "Select * From S_Survey_Question where ID='" + id + "'";
            DataTable dt = sh.ExecuteDataTable(strSql1);
            string htr = "";
            htr += string.Format(@"<tr>
                                        <td colspan='3'>
                                            <div class='surveyResultQu'>
                                                <div class='r-qu-body-title'>
                                                    {0}
                                                </div>
                                            </div>
                                        </td>
                                    </tr>", dt.Rows[0]["QuestionName"]);
            string strSql2 = "select * from S_Survey_QuestionComment where userID='" + user.UserID +
                             "' and QuestionID='" + id + "'";
            DataTable dt2 = sh.ExecuteDataTable(strSql2);
            if (dt2.Rows.Count != 0)
            {
                ViewBag.Comment = dt2.Rows[0]["Comment"].ToString();
            }
            else ViewBag.Comment = "";
            ViewBag.htr = htr;
            ViewBag.tittle = tittle;
            return View(ViewBag);
        }

        public ActionResult Result() //初始化统计页面
        {
            string opParam = "";
            string id = GetQueryString("ID");
            string strSql1 = "Select * From S_Survey_Subject where ID='" + id + "'";
            string str = "select * from S_Survey_Subject where '" + DateTime.Now +
                         "' between SurveyStartDate and SurveyEndDate and ID='" + id + "'";
            if (sh.ExecuteDataTable(str).Rows.Count != 0)
            {
                ViewBag.State = "此调查正在进行！";
            }
            else ViewBag.State = "此调查已经结束！";
            DataTable dt1 = sh.ExecuteDataTable(strSql1);
            ViewBag.tittle = dt1.Rows[0]["SurveyName"] + "问卷调查";
            ViewBag.Writer = dt1.Rows[0]["IssueName"].ToString();

            if (dt1.Rows[0]["SurveyAttchment"].ToString() != "")
            {
                string url = ConfigurationManager.AppSettings["FS_MasterServerUrl"];
                url = url.Split(new[] { "Services" }, StringSplitOptions.RemoveEmptyEntries).First();
                ViewBag.Attachment = "<a href='" + url + "Download.aspx?fileid=" + dt1.Rows[0]["SurveyAttchment"] +
                                     "'>点击查看附件</a>"; //附件
            }
            else ViewBag.Attachment = "<a>无附件</a>";
            string htr = "";
            string strSql5 = string.Format(@"select count(UserID) from S_Survey_Vote 
                                where OptionID in (select ID from S_Survey_Option where QuestionID in(select ID from S_Survey_Question where SurveyID ='{0}'))
                                group by UserID", dt1.Rows[0]["ID"]);
            int totalNumber = sh.ExecuteDataTable(strSql5).Rows.Count;
            string strSql2 = "Select * From S_Survey_Question where SurveyID='" + dt1.Rows[0]["ID"] + "'";
            DataTable dt2 = sh.ExecuteDataTable(strSql2);
            string isMemoryUser = dt1.Rows[0]["IsMemoryUser"].ToString();
            string urlUserTemp1 = "/OfficeAuto/Survey/QuestionVote?QuesID={0}&type=text&isMemoryUser=" + isMemoryUser;
            string urlUserTemp2 = "/OfficeAuto/Survey/SurveyUserList?SurveyID={0}&OptionID={1}&type=radio";
            const string aTemp = "&nbsp;&nbsp;<a href='javascript:void(0)' onclick=\"OpenUserList('{0}','{2}','{3}');\">{1}次</a>";
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                object questionId = dt2.Rows[i]["ID"];
                string type = "";
                if (dt2.Rows[i]["IsMustAnswer"].ToString() == "1")
                {
                    type += "*";
                }
                if (dt2.Rows[i]["QuestionType"].ToString() == "文本")
                {
                    type += "【填空题】";
                }
                else type += "【" + dt2.Rows[i]["QuestionType"] + "】";

                htr += string.Format(@"<tr>
                                    <td colspan='3'><div class='surveyResultQu'>
                                                <div class='r-qu-body-title'>{0}{1}{2}
                                                </div>
                                                <div class='r-qu-body-content'>
                                                    <table class='suQuTable' border='0' cellspacing='0' cellpadding='0'>
                                                        <tbody>", (i + 1).ToString() + "、", dt2.Rows[i]["QuestionName"],
                                     type);

                string urlUser;
                string strSql3 = "";
                int n;
                if (dt2.Rows[i]["QuestionType"].ToString() == "文本")
                {
                    strSql3 = "select count(*) from S_Survey_QuestionVote where QuestionID='" + questionId + "'";
                    object obj = sh.ExecuteScalar(strSql3);
                    if (obj != null)
                    {
                        n = Convert.ToInt32(obj);
                    }
                    else n = 0;
                    urlUser = String.Format(urlUserTemp1, questionId);

                    string aString = String.Format(aTemp, urlUser, n, "text", isMemoryUser);
                    if (isMemoryUser == "0")
                        aString = "&nbsp;&nbsp;" + n + "次";
                    htr += string.Format(@"<tr>
                                                                <td width='15'>
                                                                    &nbsp;
                                                                </td>
                                                                <td class='bfbTd'>
                                                                    回答数：{0}
                                                                </td>
                                                                <td colspan='4'>
                                                                    &nbsp;
                                                                </td>
                                                            </tr>", aString);
                }
                else
                {
                    DataTable dt3;
                    string strSql4 = "";
                    if (dt2.Rows[i]["QuestionType"].ToString() == "单选")
                    {
                        strSql3 = "select * from S_Survey_Option where QuestionID='" + questionId + "'";
                        dt3 = sh.ExecuteDataTable(strSql3);


                        char m = 'A';
                        for (int j = 0; j < dt3.Rows.Count; j++)
                        {
                            strSql4 = "select count(*) from S_Survey_Vote where OptionID='" + dt3.Rows[j]["ID"] + "'";
                            //计算选择该选项的次数
                            object obj = sh.ExecuteScalar(strSql4);
                            if (obj != null)
                            {
                                n = Convert.ToInt32(obj);
                            }
                            else n = 0;
                            urlUser = String.Format(urlUserTemp2, dt1.Rows[0]["ID"], dt3.Rows[j]["ID"]);

                            string aString = String.Format(aTemp, urlUser, n, "radio", isMemoryUser);
                            if (isMemoryUser == "0")
                                aString = "&nbsp;&nbsp;" + n + "次";
                            opParam += dt3.Rows[j]["ID"] + "|" + totalNumber + "|" + n + ",";
                            htr += string.Format(@"<tr>
                                                                <td width='15'>
                                                                    &nbsp;
                                                                </td>
                                                                <td>
                                                                    {0}{1}
                                                                </td>
                                                                <td width='180'>
                                                                    <div class='progressbarDiv progress0' id='{2}'>
                                                                    </div>
                                                                </td>
                                                                <td width='50' align='right' class='bfbTd' id='{3}'>
                                                                    {4}%
                                                                </td>
                                                                <td width='55' align='left' class='tdAnCount'>
                                                                    {5}
                                                                </td>
                                                                <td width='20'>
                                                                    &nbsp;
                                                                </td>
                                                            </tr>", Convert.ToChar(m + j) + "、",
                                                 dt3.Rows[j]["OptionName"], dt3.Rows[j]["ID"], dt3.Rows[j]["ID"] + "_1",
                                                 (n * 1.0 / totalNumber) * 100, aString);
                        }
                    }
                    else if (dt2.Rows[i]["QuestionType"].ToString() == "多选")
                    {
                        strSql3 = "select * from S_Survey_Option where QuestionID='" + questionId + "'";
                        dt3 = sh.ExecuteDataTable(strSql3);

                        char m = 'A';
                        for (int j = 0; j < dt3.Rows.Count; j++)
                        {
                            strSql4 = "select count(*) from S_Survey_Vote where OptionID='" + dt3.Rows[j]["ID"] + "'";
                            //计算选择该选项的次数
                            object obj = sh.ExecuteScalar(strSql4);
                            if (obj != null)
                            {
                                n = Convert.ToInt32(obj);
                            }
                            else n = 0;
                            opParam += dt3.Rows[j]["ID"] + "|" + totalNumber + "|" + n + ",";
                            urlUser = String.Format(urlUserTemp2, dt1.Rows[0]["ID"], dt3.Rows[j]["ID"]);

                            string aString = String.Format(aTemp, urlUser, n, "radio", isMemoryUser);
                            if (isMemoryUser == "0")
                                aString = "&nbsp;&nbsp;" + n + "次";
                            htr += string.Format(@"<tr>
                                                                <td width=‘15'>
                                                                    &nbsp;
                                                                </td>
                                                                <td>
                                                                    {0}{1}
                                                                </td>
                                                                <td width='180'>
                                                                    <div class='progressbarDiv progress0' id='{2}'>
                                                                    </div>
                                                                </td>
                                                                <td width='50' align='right' class='bfbTd' id='{3}'>
                                                                    {4}%
                                                                </td>
                                                                <td width='55' align='left' class='tdAnCount'>
                                                                    {5}
                                                                </td>
                                                                <td width='20'>
                                                                    &nbsp;
                                                                </td>
                                                            </tr>", Convert.ToChar(m + j) + "、",
                                                 dt3.Rows[j]["OptionName"], dt3.Rows[j]["ID"], dt3.Rows[j]["ID"] + "_1",
                                                 (n * 1.0 / totalNumber) * 100, aString);
                        }
                    }
                }

                htr += @"                                </tbody>
                                                    </table>
                                                    <div style='clear: both;'>
                                                    </div>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>";
            }
            ViewBag.htr = htr;
            ViewBag.time = DateTime.Now;
            if (opParam.Length > 0)
                ViewBag.OpParam = opParam.Substring(0, opParam.Length - 1);
            ViewBag.totalNumber = totalNumber;
            return View(ViewBag);
        }

        public ActionResult QuestionVote()
        {
            string htr = "";
            string ismemberuser = GetQueryString("IsMemoryUser");
            string QuesID = GetQueryString("QuesID");
            string sql = "select * from S_Survey_QuestionVote where QuestionID='" + QuesID + "'";
            DataTable dt = sh.ExecuteDataTable(sql);
            if (ismemberuser == "1")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    htr += string.Format(@"<tr>
                                            <td colspan='3'>
                                            <a><font color='#0099FF'>回复内容：</font></a>{2}
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width='25%' align='right'>
                                            </td>
                                            <td width='30%' align='right'>
                                                <a><font color='#0099FF'>回复人：</font></a>{0}
                                            </td>
                                            <td width='45%' align='left'>
                                               <a><font color='#0099FF'>回复时间：</font></a>{1}
                                            </td>
                                        </tr>    
                                        <tr>
                                            <td colspan='3'>
                                                <font color='#000000'>---------------------------------------------------------------------------------------------</font>
                                            </td>
                        </tr>", dt.Rows[i]["UserName"], dt.Rows[i]["VoteDate"], dt.Rows[i]["QuestionMsg"]);
                }
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    htr += string.Format(@"<tr>
                                                <td colspan='3'>
                                                  <a><font color='#0099FF'>回复内容：</font></a>  {1}
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width='25%' align='right'>
                                                </td>
                                                <td width='30%' align='right'>
                                                </td>
                                                <td width='45%' align='left'>
                                                   <a><font color='#0099FF'>回复时间：</font></a>{0}
                                                </td>
                                            </tr>  
                                            <tr>
                                                <td colspan='3'>
                                                <font color='#000000'>---------------------------------------------------------------------------------------------</font>
                                            </td>
                        </tr>", dt.Rows[i]["VoteDate"], dt.Rows[i]["QuestionMsg"]);
                }
            }
            ViewBag.htr = htr;

            return View(ViewBag);
        }

        public JsonResult GetSurveyList(QueryBuilder qb)
        {
            string str = @"
select *,SurveyState = '投票中',(select case (select count(*) as co from S_Survey_Vote where UserID='{0}' and OptionID in (select ID from S_Survey_Option where QuestionID in (select ID from S_Survey_Question where SurveyID=S_Survey_Subject.ID))) when 0 then case (select count(*) as co from S_Survey_QuestionVote where UserID='{0}' and QuestionID in (select ID from S_Survey_Question where SurveyID=S_Survey_Subject.ID)) when 0 then '未投票' else '已投票' end else '已投票' end) as VoteState
from S_Survey_Subject
where (charindex('{0}',QueryUserIDs)>0 {2})
and State='T' and convert(char(10),SurveyEndDate,121)>= '{1}'
union 
select *,SurveyState = '已过期',(select case (select count(*) as co from S_Survey_Vote where UserID='{0}' and OptionID in (select ID from S_Survey_Option where QuestionID in (select ID from S_Survey_Question where SurveyID=S_Survey_Subject.ID))) when 0 then case (select count(*) as co from S_Survey_QuestionVote where UserID='{0}' and QuestionID in (select ID from S_Survey_Question where SurveyID=S_Survey_Subject.ID)) when 0 then '未投票' else '已投票' end else '已投票' end) as VoteState
from S_Survey_Subject
where (charindex('{0}',QueryUserIDs)>0 {2})
and State='T' and convert(char(10),SurveyEndDate,121) < '{1}'";
            var sql = "SELECT OrgID FROM S_A__OrgRoleUser WHERE UserID='{0}' UNION SELECT OrgID FROM S_A__OrgUser WHERE UserID='{0}'";
            var orgDT = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(String.Format(sql, user.UserID));
            var orgIDs = orgDT.AsEnumerable().Select(a => a["OrgID"].ToString()).ToArray();
            var whereStr = "";
            foreach (var orgID in orgIDs)
            {
                whereStr += " or QueryDeptIDs like '%" + orgID + "%'";
            }
            str = string.Format(str, user.UserID, System.DateTime.Now.ToString("yyyy-MM-dd"), whereStr);
            qb.SortField = "SurveyStartDate";
            qb.SortOrder = "desc";
            GridData data = sh.ExecuteGridData(str, qb);
            return Json(data);
        }

        public JsonResult GetSurveyResultList(QueryBuilder qb)
        {
            string str = "select * from S_Survey_Subject where charindex('" + user.UserID +
                         "',StatisticsUserIDs)>0 and State='T'";
            GridData data = sh.ExecuteGridData(str, qb);
            return Json(data);
        }

        public JsonResult GetSurveyUserList(QueryBuilder qb) //获取投票用户
        {
            string OptionID = "";
            string sql = "";
            OptionID = GetQueryString("OptionID");
            sql = "select * from S_Survey_Vote where OptionID='" + OptionID + "'";

            GridData data = sh.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult SaveMsg(string quesID, string Comment) //保存文本
        {
            var mo = this.entities.Set<S_Survey_QuestionVote>().FirstOrDefault(a => a.QuestionID == quesID && a.UserID == user.UserID);
            if (mo == null)
            {
                mo = new S_Survey_QuestionVote();
                mo.ID = FormulaHelper.CreateGuid();
                mo.QuestionID = quesID;
                mo.UserID = user.UserID;
                mo.UserName = user.UserName;
                entities.Set<S_Survey_QuestionVote>().Add(mo);
            }

            mo.QuestionMsg = Comment;
            mo.VoteDate = DateTime.Now;
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveVote(string ID) // 记录投票
        {
            var mo = new S_Survey_Vote();
            mo.ID = FormulaHelper.CreateGuid();
            mo.OptionID = ID;
            mo.UserID = user.UserID;
            mo.UserName = user.UserName;
            mo.VoteDate = DateTime.Now;
            entities.Set<S_Survey_Vote>().Add(mo);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveComment(string QuesID, string comment) // 记录评论
        {
            string sql = "select * from S_Survey_QuestionComment where QuestionID='" + QuesID + "' and UserID='" +
                         user.UserID + "'";
            DataTable dt = sh.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                var mo = new S_Survey_QuestionComment();
                mo.ID = FormulaHelper.CreateGuid();
                mo.QuestionID = QuesID;
                mo.UserID = user.UserID;
                mo.Comment = comment;
                mo.UserName = user.UserName;
                mo.VoteDate = DateTime.Now;
                entities.Set<S_Survey_QuestionComment>().Add(mo);
                entities.SaveChanges();
            }
            else
            {
                sql = "update S_Survey_QuestionComment set Comment='" + comment + "' where QuestionID='" + QuesID +
                      "' and UserID='" + user.UserID + "'";
                sh.ExecuteNonQuery(sql);
            }
            return Json(new { ID = "" });
        }


        public JsonResult Check() //检索是否已投票
        {
            ViewBag.Flag = "true";
            string id = GetQueryString("ID");
            string strSql = "select * from S_Survey_Vote where UserID='" + user.UserID +
                            "' and OptionID in (select ID from S_Survey_Option where QuestionID in (select ID from S_Survey_Question where SurveyID='" +
                            id + "'))";
            object Obj = sh.ExecuteScalar(strSql);
            if (Obj != null && Obj.ToString() != "")
            {
                ViewBag.Flag = "false";
            }
            return Json(ViewBag);
        }
    }
}