using System;
using System.Data;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using Config;
using Formula;
using Formula.Helper;
using Base.Logic.BusinessFacade.Portal;
using Base.Logic.Domain;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using JWT;
using JWT.Algorithms;
using System.Configuration;
using JWT.Serializers;

namespace PortalLTE.Controllers
{
    public class MainController : Controller
    {
        string Title = !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["alertTitleStr"]) ? System.Configuration.ConfigurationManager.AppSettings["alertTitleStr"] : "上海金慧软件有限公司";

        #region 登录、登出

        public ActionResult Login()
        {
            ViewBag.Title = Title;
            if (PortalMain.IsUseNewPortal)
            {
                Response.Redirect("/PortalLTE/Main/Portal");
                return null;
            }
            else
            {
                return View();
            }
        }

        public ActionResult Portal(bool hasLayout = true)
        {
            ViewBag.Title = Title;
            var tokenKey = System.Configuration.ConfigurationManager.AppSettings["TokenKey"];
           
            string token = Request.QueryString[tokenKey];
            if (!PortalMain.IsUseNewPortal)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    Response.Redirect("/PortalLTE/Main/Index?"+ tokenKey+"="+ token);
                    return null;
                }
                else
                {
                    Response.Redirect("/PortalLTE/Main/Index");
                    return null;
                }
              
            }
            var userService = FormulaHelper.GetService<IUserService>();
            string systemName = userService.GetCurrentUserLoginName();
            if (String.IsNullOrEmpty(systemName))
                systemName = FormulaHelper.GetService<ISSOService>().GetLoginAccount(HttpContext.ApplicationInstance);
            if (!string.IsNullOrEmpty(systemName))
            {
                ViewBag.User = userService.GetUserInfoBySysName(systemName);
                var user = FormulaHelper.GetUserInfo();
                ViewBag.TopMenu = GetTopMenu(Config.Constant.MenuRooID);

            }
            else
            {
                ViewBag.User = new UserInfo();
                ViewBag.TopMenu = new List<DataRow>();
            }
            ViewBag.IsUseLeftShortCut = PortalMain.IsUseLeftShortCut;

            if (!string.IsNullOrEmpty(systemName))
                ViewBag.PortalLTE = PortalMain.GetIndexHTML(Request.Url.AbsoluteUri, Request["ID"]);
            else
            {
                string portalHtml = "";
                List<Dictionary<string, string>> listDic = new List<Dictionary<string, string>>();
                var noLoginPortals = PortalMain.GetPortalIndexHTML("");
                for (int i = 0; i < noLoginPortals.Count; i++)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("ID", noLoginPortals[i]["ID"]);
                    dic.Add("Key", noLoginPortals[i]["Key"]);
                    dic.Add("Title", noLoginPortals[i]["Title"]);
                    if (noLoginPortals[i].ContainsKey("Count"))
                        dic.Add("Count", noLoginPortals[i]["Count"]);
                    listDic.Add(dic);
                    portalHtml += string.Format("<div id='{0}' class='nav_nologin_list {2}'>{1}</div>",
                        noLoginPortals[i]["ID"], noLoginPortals[i]["Html"], i == 0 ? "" : "display");
                }
                ViewBag.NoLoginPortalLTE = portalHtml;
                ViewBag.NoLoginPortalDic = string.Format("var NoLoginPortalDic={0}", JsonHelper.ToJson(listDic));
            }

            ViewBag.HasLayout = hasLayout;
            ViewBag.IsUseNewPortal = PortalMain.IsUseNewPortal;
            ViewBag.PortalKeys = PortalMain.PortalKeys;
            ViewBag.LGID = FormulaHelper.GetCurrentLGID();
            ViewBag.NoPopupLayer = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["noPopupLayer"]).ToLower();
            var portalNewsTime = System.Configuration.ConfigurationManager.AppSettings["PortalNewsTime"];
            if (!string.IsNullOrEmpty(portalNewsTime))
                ViewBag.PortalNewsTime = Convert.ToInt32(portalNewsTime);
            else
                ViewBag.PortalNewsTime = 24;
            return View();
        }

        public JsonResult DoLogin(string loginName, string password, string LGID = "")
        {
            if (string.IsNullOrWhiteSpace(loginName) || loginName.Length >= 20)
            {
                throw new Exception("用户名不合法！");
            }
            password = EncryptHelper.AesDecrpt(password);
            int ErrPwdLockTime = 10;
            int ErrPwdTimes = 3;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ErrPwdLockTime"]))
                ErrPwdLockTime = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ErrPwdLockTime"]);
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ErrPwdTimes"]))
                ErrPwdTimes = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ErrPwdTimes"]);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            //防止sql注入
            string sql = "select * from S_A_User WITH(NOLOCK) where Code=@code AND IsDeleted<>1";
            Dictionary<string, object> infoParams = new Dictionary<string, object>();
            infoParams.Add("@code", loginName);
            var dt = sqlHelper.ExecuteDataTable(sql, infoParams, CommandType.Text);

            //锁定时间
            if (dt.Rows.Count > 0)
            {
                string strErrTime = dt.Rows[0]["ErrorTime"].ToString();
                if (strErrTime != "" && DateTime.Parse(strErrTime).AddMinutes(ErrPwdLockTime) < DateTime.Now)
                {
                    sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set ErrorTime=null,ErrorCount=0 where ID= (SELECT TOP 1 ID FROM S_A_User WITH(NOLOCK)  WHERE CODE='{0}')", loginName));
                    dt.Rows[0]["ErrorCount"] = 0;
                }
            }

            if (dt.Rows.Count == 0)
            {
                throw new Exception("用户不存在！");
            }
            else if (dt.Rows[0]["ErrorCount"].ToString() != "" && Convert.ToInt32(dt.Rows[0]["ErrorCount"]) >= ErrPwdTimes)
            {
                if (dt.Rows[0]["ErrorTime"].ToString() == "")
                {
                    sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set ErrorTime='{1}'  where ID= (SELECT TOP 1 ID FROM S_A_User WITH(NOLOCK)  WHERE CODE='{0}')", loginName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                }

                throw new Exception(string.Format("输入密码错误次数超过{0}次，请联系管理员！", ErrPwdTimes));
            }
            else if ((!password.Contains("~.?/&") && dt.Rows[0]["Password"].ToString() != FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", loginName, password), "SHA1")
                && dt.Rows[0]["Password"].ToString() != FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", loginName.ToLower(), password), "SHA1")) ||
                (password.Contains("~.?/&") && !password.Contains(dt.Rows[0]["Password"].ToString())))
            {
                sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set ErrorCount=ErrorCount+1 where ID= (SELECT TOP 1 ID FROM S_A_User WITH(NOLOCK)  WHERE CODE='{0}')", loginName));
                throw new Exception("密码错误！");
            }
            else
            {
                //认证成功
                HttpApplicationState http = System.Web.HttpContext.Current.Application;

                //验证是否强制同时单账户登录
                var isLoginSingleton = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLoginSingleton"]);
                if (isLoginSingleton)
                {
                    if (CheckLogin(http, loginName) == 1)
                    {
                        http.Add(loginName, GetClientMAC());
                    }
                    else if (CheckLogin(http, loginName) == 3)
                    {
                        http.Set(loginName, GetClientMAC());
                    }
                }
                //FormsAuthentication保存票据
                string bill = loginName;
                if (!string.IsNullOrEmpty(LGID))
                    bill = loginName + "^" + LGID;
                System.Web.Security.FormsAuthentication.SetAuthCookie(bill, false);
                sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set ErrorCount=0,LastLoginTime='{1}'  where ID= (SELECT TOP 1 ID FROM S_A_User WITH(NOLOCK)  WHERE CODE='{0}')", loginName, DateTime.Now));

                var ip = System.Web.HttpContext.Current.Request.UserHostAddress;
                var insertSql = string.Format(@"insert into S_L_LoginLog values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                    dt.Rows[0]["ID"].ToString(), dt.Rows[0]["Name"].ToString(), dt.Rows[0]["Code"].ToString(),
                    System.DateTime.Now.Date.ToString("yyyy-MM-dd"), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ip, "", "MIS");
                sqlHelper.ExecuteNonQuery(insertSql);
            }

            return Json(password.Contains("~.?/&") ? password : "~.?/&" + FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", loginName, password), "SHA1"));
        }

        #region 内部方法.验证跨终端登录

        //判断是否是跨终端登录  1-没有登录过 2-同帐号同机登录 3-异地登录
        private int CheckLogin(HttpApplicationState http, string loginName)
        {
            int returnVal = 1;
            if (http.AllKeys.Where(o => o.ToString().ToLower() == loginName.ToLower()).ToArray().Length > 0)
            {
                string[] allKeys = System.Web.HttpContext.Current.Application.AllKeys;
                for (int i = 0; i < allKeys.Length; i++)
                {
                    //同帐号同机登录
                    if (System.Web.HttpContext.Current.Application.Get(i).ToString() == GetClientMAC())
                    {
                        returnVal = 2;
                    }
                    else
                    {
                        returnVal = 3;
                    }
                }
            }

            return returnVal;
        }

        [System.Runtime.InteropServices.DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [System.Runtime.InteropServices.DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);

        private string GetClientMAC()
        {
            string mac_dest = string.Empty;
            // 在此处放置用户代码以初始化页面
            try
            {
                string userip = Request.UserHostAddress;
                string strClientIP = Request.UserHostAddress.ToString().Trim();
                Int32 ldest = inet_addr(strClientIP); //目的地的ip 
                Int32 lhost = inet_addr("");   //本地服务器的ip 
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                string mac_src = macinfo.ToString("X");
                while (mac_src.Length < 12)
                {
                    mac_src = mac_src.Insert(0, "0");
                }
                for (int i = 0; i < 11; i++)
                {
                    if (0 == (i % 2))
                    {
                        if (i == 10)
                        {
                            mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                        else
                        {
                            mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mac_dest.Replace("-", "");
        }

        #endregion

        public JsonResult DoLogout()
        {
            string key = string.Format("{0}_GetResTree_{1}", FormulaHelper.UserID, Config.Constant.MenuRooID);
            CacheHelper.Remove(key);
            System.Web.Security.FormsAuthentication.SignOut();
            return Json("");
        }

        #endregion

        #region 修改密码

        public ActionResult ForgetPwd()
        {
            ViewBag.Title = Title;
            return View();
        }

        public JsonResult ChangePwd(string loginName, string oldPwd, string newPwd, string newPwdOk)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            //防止sql注入
            string sql = "select * from S_A_User WITH(NOLOCK) where Code=@code AND IsDeleted<>1";
            Dictionary<string, object> infoParams = new Dictionary<string, object>();
            infoParams.Add("@code", loginName);
            var dt = sqlHelper.ExecuteDataTable(sql, infoParams, CommandType.Text);

            if (dt.Rows.Count == 0)
            {
                throw new Exception("用户不存在！");
            }
            else
            {

                oldPwd = EncryptHelper.AesDecrpt(oldPwd);
                newPwd = EncryptHelper.AesDecrpt(newPwd);
                newPwdOk = EncryptHelper.AesDecrpt(newPwdOk);

                if (dt.Rows[0]["Password"].ToString() != FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", loginName, oldPwd), "SHA1")
                && dt.Rows[0]["Password"].ToString() != FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", loginName.ToLower(), oldPwd), "SHA1"))
                {
                    throw new Exception("原密码错误！");
                }
                else if (newPwd != newPwdOk)
                {
                    throw new Exception("两次输入的新密码不一致！");
                }
                else
                {
                    sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set Password='{1}' where ID='{0}'", dt.Rows[0]["ID"].ToString(), FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", loginName.ToLower(), newPwd), "SHA1")));
                    return Json("ok");
                }
            }
        }

        #endregion

        #region Index
               
        public ActionResult Index(bool hasLayout = true)
        {
            var tokenKey = System.Configuration.ConfigurationManager.AppSettings["TokenKey"];

            string token = Request.QueryString[tokenKey];
            if (PortalMain.IsUseNewPortal)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    Response.Redirect("/PortalLTE/Main/Portal?" + tokenKey + "=" + token);
                    return null;
                }
                else
                {
                    Response.Redirect("/PortalLTE/Main/Portal");
                    return null;
                }

            }
      
            ViewBag.IsUseLeftShortCut = PortalMain.IsUseLeftShortCut;
            var user = FormulaHelper.GetUserInfo();
            ViewBag.HasLayout = hasLayout;
            ViewBag.TopMenu = GetTopMenu(Config.Constant.MenuRooID);
            ViewBag.PotalLTE = PortalMain.GetIndexHTML(Request.Url.AbsoluteUri, Request["ID"]);
            ViewBag.UserTemplet = PortalMain.GetUserTemplets(Request["ID"]);
            ViewBag.NoPopupLayer = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["noPopupLayer"]).ToLower();
            var portalNewsTime = System.Configuration.ConfigurationManager.AppSettings["PortalNewsTime"];
            if (!string.IsNullOrEmpty(portalNewsTime))
                ViewBag.PortalNewsTime = Convert.ToInt32(portalNewsTime);
            else
                ViewBag.PortalNewsTime = 24;
            ViewBag.User = user;
            ViewBag.LGID = FormulaHelper.GetCurrentLGID();
            ViewBag.Title = Title;
           
            return View();
        }

        /// <summary>
        /// 获取子菜单
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public string GetMenuHtml(string parentId)
        {
            var dt = FormulaHelper.GetService<IResService>().GetResTree(Config.Constant.MenuRooID, FormulaHelper.UserID);

            string result = CreateMenu(parentId, dt.AsEnumerable());

            return result;
        }

        #endregion

        #region 地图导航
        public ActionResult Navigition()
        {
            ViewBag.Title = Title;
            var userService = FormulaHelper.GetService<IUserService>();
            string systemName = userService.GetCurrentUserLoginName();
            if (String.IsNullOrEmpty(systemName))
                systemName = FormulaHelper.GetService<ISSOService>().GetLoginAccount(HttpContext.ApplicationInstance);
            if (!string.IsNullOrEmpty(systemName))
            {
                ViewBag.User = userService.GetUserInfoBySysName(systemName);
                var user = FormulaHelper.GetUserInfo();
                ViewBag.TopMenu = GetTopMenu(Config.Constant.MenuRooID);

            }
            else
            {
                ViewBag.User = new UserInfo();
                ViewBag.TopMenu = new List<DataRow>();
            }


            EnumerableRowCollection<DataRow> navsResTree = CacheHelper.Get("NavsResTree") as EnumerableRowCollection<DataRow>;
            if (navsResTree == null)
            {
                var dt = FormulaHelper.GetService<IResService>().GetResTree(Config.Constant.MenuRooID, FormulaHelper.UserID).AsEnumerable();
                var newResult = new List<DataRow>();
                navsResTree = dt;
                CacheHelper.Set("NavsResTree", dt,3600);
            }

            ViewBag.NavHtmlRows=GetNavMenu(Config.Constant.MenuRooID, navsResTree);
            ViewBag.IsUseNewPortal = PortalMain.IsUseNewPortal;
            ViewBag.PortalKeys = PortalMain.PortalKeys;
            ViewBag.LGID = FormulaHelper.GetCurrentLGID();
            ViewBag.NoPopupLayer = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["noPopupLayer"]).ToLower();
            var portalNewsTime = System.Configuration.ConfigurationManager.AppSettings["PortalNewsTime"];
            if (!string.IsNullOrEmpty(portalNewsTime))
                ViewBag.PortalNewsTime = Convert.ToInt32(portalNewsTime);
            else
                ViewBag.PortalNewsTime = 24;
            return View();
        }

        private string GetNavMenu(string parentId, EnumerableRowCollection<DataRow> dt)
        {
            StringBuilder sb = new StringBuilder();
            List<DataRow> childMenunull = new List<DataRow>();
            foreach (DataRow row in dt.Where(c => c["ParentID"].ToString() == parentId).OrderBy(c => c["SortIndex"]).ToList())
            {
                string id = row["ID"].ToString();

                string childMenu = GetNavMenu(id, dt);

                if (childMenu == "")
                {
                    childMenunull.Add(row);
                }
                else
                {
                    if (parentId == Config.Constant.MenuRooID)
                        sb.AppendFormat(@"
 <div  class='col-sm-12 ' style='padding-left:15%;width:100%;padding-right:15%;padding-bottom:20px;'>
<h4 class='col-sm-12' style='font-weight: 600;font-size: 17px;'>{0}</h4>
<ul class='list-unstyled col-sm-12 box-shadowul' style='box-shadow: 1px 1px 0px 1px #E2E2E2;border-bottom-right-radius: 5px;padding-left:30px;padding-bottom: 20px;'>
<li>
        {2}
</li>
</ul>
</div>
", row["Name"], row["IconClass"], childMenu);
                    else
                        sb.AppendFormat(@"
<h4 class='col-sm-12' style='color:#3a3838;font-size:15px'>{0}</h4>
<ul class='list-unstyled col-sm-12' style='padding-left:30px;padding-bottom:10px;'>
<li>
        {2}
</li>
</ul>

", row["Name"], row["IconClass"], childMenu);
                }
            }
            if (parentId == Config.Constant.MenuRooID)
            {
                var navhtml = string.Format(@"<div  class='col-sm-12 ' style='padding-left:15%;width:100%;padding-right:15%;padding-bottom:20px;'>");
                foreach (var row in childMenunull)
                {
                    string id = row["ID"].ToString();
                    navhtml = navhtml+string.Format(@"
                    <div class='col-sm-3 navigitiondiv'  style='cursor:pointer;margin-right: 15px;margin-top:2px;line-height: 35px;
                        height: 35px;' onclick='goUrl(""{0}"",""{2}"",this)'>
                    <i class='{1}' style='display: block;
                        width: 20px;top:10px;top:10px !important;'></i><div id='{4}' class='navigitions' style='color: #A4A4A4;display: table-caption;
                        height: 35px;
                        line-height: 35px;
                        width: 87%;
                        overflow: hidden;
                        text-overflow: ellipsis;
                        white-space: nowrap;
                        margin-left: 20px;
                        position: absolute;
                        top: 0px;'> {0} </div></div>"
                        , row["Name"], string.IsNullOrEmpty(row["IconClass"].ToString()) ? "glyphicon glyphicon-th" : row["IconClass"].ToString(), row["Url"], row["IconUrl"], id);
                }
                navhtml = navhtml + "</div>";
                sb.AppendFormat(navhtml);
            }
            else
            {
                foreach (var row in childMenunull)
                {
                    string id = row["ID"].ToString();
                    sb.AppendFormat(@"
                    <div class='col-sm-3 navigitiondiv'  style='cursor:pointer;margin-right: 15px;margin-top:2px;line-height: 35px;
                        height: 35px;' onclick='goUrl(""{0}"",""{2}"",this)'>
                    <i class='{1}' style='display:block;
                        top:10px !important;'></i><div id='{4}' class='navigitions' style='color: #A4A4A4;display: table-caption;
                        height: 35px;
                        line-height: 35px;
                        width: 87%;
                        overflow: hidden;
                        text-overflow: ellipsis;
                        white-space: nowrap;
                        margin-left: 20px;
                        position: absolute;
                        top: 0px;'> {0} </div></div>"
                        , row["Name"], string.IsNullOrEmpty(row["IconClass"].ToString()) ? "glyphicon glyphicon-th" : row["IconClass"].ToString(), row["Url"], row["IconUrl"], id);
                }
            }
                return sb.ToString();
        }
        #endregion

        #region 保存门户块配置

        public JsonResult SaveSettings()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string userId = FormulaHelper.UserID;
            sqlHelper.ExecuteNonQuery(string.Format("update S_A_User set PortalSettings='{0}' where ID='{1}'", Request["settings"], userId));
            return Json("");
        }

        #endregion

        #region 创建菜单

        private List<DataRow> GetTopMenu(string menuRootID)
        {
            var dt = FormulaHelper.GetService<IResService>().GetResTree(Config.Constant.MenuRooID, FormulaHelper.UserID).AsEnumerable();
            var newResult = new List<DataRow>();
            foreach (var row in dt)
            {
                if (row["ParentID"].ToString() == menuRootID)
                    newResult.Add(row);
            }
            return newResult;
        }


        private string CreateMenu(string parentId, EnumerableRowCollection<DataRow> dt)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in dt.Where(c => c["ParentID"].ToString() == parentId).OrderBy(c => c["SortIndex"]).ToList())
            {
                string id = row["ID"].ToString();

                string childMenu = CreateMenu(id, dt);

                if (childMenu == "")
                {
                    sb.AppendFormat(@"
<li style='cursor:pointer'><a ui-href='{2}' style='overflow: hidden;white-space: nowrap;text-overflow: ellipsis;' onclick='goUrl(this)'><i class='{1}'></i><span style='overflow: hidden;white-space: nowrap;text-overflow: ellipsis;width: 160px;display:block;'>{0}</span><i class='fa fa-star-o pull-right' title='添加快捷' onclick='saveShortCut(""{0}"",""{2}"",""{3}""); event.stopPropagation();' style='display:none'></i></a></li>", row["Name"], row["IconClass"], row["Url"], row["IconUrl"]);
                }
                else
                {
                    if (parentId == Config.Constant.MenuRooID)
                        sb.AppendFormat(@"
<li class='treeview'><a href='#' onclick='openSubMenu(this,event)'><i class='{1}'></i><span>{0}</span> <i class='fa fa-angle-left pull-right'></i></a>
    <ul class='treeview-menu'>
        {2}
    </ul>
</li>
", row["Name"], row["IconClass"], childMenu);
                    else
                        sb.AppendFormat(@"
<li><a href='#' onclick='openSubMenu(this,event)'><i class='{1}'></i>{0}<i class='fa fa-angle-left pull-right'></i></a>
    <ul class='treeview-menu'>
        {2}
    </ul>
</li>

", row["Name"], row["IconClass"], childMenu);
                }
            }

            return sb.ToString();
        }
     
        #endregion

        #region 代办已办(board1)

        public JsonResult UndoTask()
        {
            var json = JsonHelper.ToJson(getUndoTask());
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoneTask()
        {
            var json = JsonHelper.ToJson(getDoneTask());
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MyTask()
        {
            DataTable undo = getUndoTask(true);
            DataTable done = getDoneTask(true);
            undo.Columns.Add("DoneCount");
            foreach (DataRow dr in undo.Rows)
                dr["DoneCount"] = done.Rows.Count;
            return Json(JsonHelper.ToJson(undo), JsonRequestBehavior.AllowGet);
        }


        private DataTable getUndoTask(bool isNew = false)
        {
            string preYear = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");

            string sql = string.Empty;
            if (Config.Constant.IsOracleDb)
            {
                sql = @"SELECT * FROM (select rownum,S_WF_InsTaskExec.ID as TaskExecID,S_WF_InsTask.SendTaskUserIDs,S_WF_InsTask.SendTaskUserNames
,S_WF_InsTask.TaskName
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
,FormInstanceID as ID
,S_WF_InsTaskExec.CreateTime
,case when S_WF_InsTask.Urgency='1' then 'list-item-mark' else '' end as Urgency
from S_WF_InsTaskExec join S_WF_InsTask on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings='' or WaitingRoutings is null) and (WaitingSteps='' or WaitingSteps is null) and (S_WF_InsTaskExec.CreateTime >= {1} ) and S_WF_InsTask.ID=InsTaskID 
join S_WF_InsFlow on S_WF_InsFlow.Status='Processing' and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID 
join S_WF_InsDefFlow on S_WF_InsFlow.InsDefFlowID=S_WF_InsDefFlow.ID  
join S_WF_InsDefStep on InsDefStepID = S_WF_InsDefStep.ID
left join S_WF_DefSubForm on SubFormID=S_WF_DefSubForm.ID
order by S_WF_InsTask.Urgency desc,S_WF_InsTaskExec.CreateTime desc) A WHERE rownum<=6";
            }
            else
            {
                sql = @"select S_WF_InsTaskExec.ID as TaskExecID,S_WF_InsTask.SendTaskUserIDs,S_WF_InsTask.SendTaskUserNames
,S_WF_InsTask.TaskName
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
,FormInstanceID as ID
,S_WF_InsTaskExec.CreateTime
,case when S_WF_InsTask.Urgency='1' then 'list-item-mark' else '' end as Urgency
from S_WF_InsTaskExec with(nolock) join S_WF_InsTask on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings='' or WaitingRoutings is null) and (WaitingSteps='' or WaitingSteps is null) and (S_WF_InsTaskExec.CreateTime >= {1} ) and S_WF_InsTask.ID=InsTaskID 
join S_WF_InsFlow with(nolock) on S_WF_InsFlow.Status='Processing' and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID 
join S_WF_InsDefFlow with(nolock) on S_WF_InsFlow.InsDefFlowID=S_WF_InsDefFlow.ID  
join S_WF_InsDefStep with(nolock) on InsDefStepID = S_WF_InsDefStep.ID
left join S_WF_DefSubForm with(nolock) on SubFormID=S_WF_DefSubForm.ID
order by S_WF_InsTask.Urgency desc,S_WF_InsTaskExec.CreateTime desc";
            }
            sql = string.Format(sql, FormulaHelper.UserID,
                Config.Constant.IsOracleDb ? string.Format("to_date('{0}','yyyy-MM-dd')", preYear) : string.Format("'{0}'", preYear)
                );

            SQLHelper helper = SQLHelper.CreateSqlHelper("WorkFlow");
            DataTable dt = null;
            if (isNew)
                dt = helper.ExecuteDataTable(sql);
            else
                dt = helper.ExecuteDataTable(sql, 0, 8, CommandType.Text);
            dt.Columns.Add("Index");
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                dt.Rows[i - 1]["Index"] = i.ToString();
            }
            return dt;
        }

        private DataTable getDoneTask(bool isNew = false)
        {
            string sql = string.Empty;
            if (Config.Constant.IsOracleDb)
            {
                sql = @"SELECT * FROM (select rownum,FormInstanceID  as ID
,S_WF_InsTaskExec.ID as TaskExecID,S_WF_InsTask.TaskUserIDs,S_WF_InsTask.TaskUserNames
,S_WF_InsTask.TaskName
,S_WF_InsTaskExec.ExecTime 
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
from S_WF_InsTaskExec join S_WF_InsTask on ExecTime is not null and ExecUserID='{0}' and S_WF_InsTask.Type in ('Normal','Inital') and S_WF_InsTask.ID=InsTaskID 
join S_WF_InsFlow on S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID 
join S_WF_InsDefFlow on S_WF_InsDefFlow.ID=S_WF_InsFlow.InsDefFlowID 
join S_WF_InsDefStep on InsDefStepID = S_WF_InsDefStep.ID
left join S_WF_DefSubForm on SubFormID=S_WF_DefSubForm.ID
order by S_WF_InsTaskExec.ExecTime desc) A WHERE rownum<=6";
            }
            else
            {
                sql = @"select FormInstanceID as ID
,S_WF_InsTaskExec.ID as TaskExecID,S_WF_InsTask.TaskUserIDs,S_WF_InsTask.TaskUserNames
,S_WF_InsTask.TaskName
,S_WF_InsTaskExec.ExecTime 
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormUrl else S_WF_DefSubForm.FormUrl end as FormUrl
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormWidth else S_WF_DefSubForm.FormWidth end as FormWidth
,case when  S_WF_DefSubForm.ID is null then S_WF_InsDefFlow.FormHeight else S_WF_DefSubForm.FormHeight end as FormHeight
from S_WF_InsTaskExec with(nolock) join S_WF_InsTask with(nolock) on ExecTime is not null and ExecUserID='{0}' and S_WF_InsTask.Type in ('Normal','Inital') and S_WF_InsTask.ID=InsTaskID 
join S_WF_InsFlow with(nolock) on S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID 
join S_WF_InsDefFlow with(nolock) on S_WF_InsDefFlow.ID=S_WF_InsFlow.InsDefFlowID 
join S_WF_InsDefStep with(nolock) on InsDefStepID = S_WF_InsDefStep.ID
left join S_WF_DefSubForm with(nolock) on SubFormID=S_WF_DefSubForm.ID
order by S_WF_InsTaskExec.ExecTime desc";
            }


            sql = string.Format(sql, FormulaHelper.UserID);


            SQLHelper helper = SQLHelper.CreateSqlHelper("WorkFlow");
            DataTable dt = null;
            if (isNew)
                dt = helper.ExecuteDataTable(sql);
            else
                dt = helper.ExecuteDataTable(sql, 0, 8, CommandType.Text);

            dt.Columns.Add("Index");
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                dt.Rows[i - 1]["Index"] = i.ToString();
            }

            return dt;
        }

        #endregion

        #region 图片新闻(board2)
        #endregion

        #region 设计任务(board3)


        public JsonResult GetDesignTask()
        {
            SQLHelper helper = SQLHelper.CreateSqlHelper("Project");

            string sql = string.Empty;
            if (Config.Constant.IsOracleDb)
            {
                sql = @"SELECT * FROM (
SELECT rownum, S_W_Activity.DisplayName,CreateUser,CreateUserID, S_W_Activity.CreateDate, S_W_Activity.LinkUrl,S_W_Activity.ID,S_W_Activity.WBSID,S_W_Activity.BusniessID,S_W_Activity.ProjectInfoID FROM  S_W_Activity
WHERE (S_W_Activity.OwnerUserID= '{0}') and S_W_Activity.ActivityKey='DesignTask' and S_W_Activity.State='Create' ORDER BY S_W_Activity.ID DESC) A WHERE rownum<=6
";
            }
            else
            {
                sql = @"
SELECT S_W_Activity.DisplayName,CreateUser,CreateUserID, S_W_Activity.CreateDate, S_W_Activity.LinkUrl,S_W_Activity.ID,S_W_Activity.WBSID,S_W_Activity.BusniessID,S_W_Activity.ProjectInfoID FROM  S_W_Activity
WHERE (S_W_Activity.OwnerUserID= '{0}') and S_W_Activity.ActivityKey='DesignTask' and S_W_Activity.State='Create' ORDER BY S_W_Activity.ID DESC
";
            }
            sql = string.Format(sql, FormulaHelper.UserID);

            DataTable dt = helper.ExecuteDataTable(sql);

            dt.Columns.Add("Index");
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                dt.Rows[i - 1]["Index"] = i.ToString();
            }

            string json = JsonHelper.ToJson(dt);

            return Json(json, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region 天气预报(board4)

        #endregion

        #region 公司新闻(board6)

        public JsonResult GetCorpInformation()
        {
            var dt = bl_GetServiceData("CNews", 5);
            string json = JsonHelper.ToJson(dt);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 个人信息(board5)

        public FileResult UserImage()
        {
            string sql = string.Format("select Picture from S_A_UserImg where UserID='{0}'", FormulaHelper.UserID);
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(sql);
            if (dt.Rows.Count == 0 || dt.Rows[0]["Picture"] is DBNull)
                return File(GetDefaultPortraitFileBytes(), "image/png");
            else
                return File(dt.Rows[0]["Picture"] as byte[], "image/png");
        }

        private byte[] GetDefaultPortraitFileBytes()
        {
            var portrait = CacheHelper.Get("GW_DefaultPortrait");
            if (portrait == null)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\user.png";
                FileInfo fileInfo = new FileInfo(path);
                FileStream fs = fileInfo.OpenRead();
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                fs.Close();
                portrait = bytes;
                CacheHelper.Set("GW_DefaultPortrait", bytes);
            }
            return portrait as byte[];
        }

        public JsonResult DeleteShortCut()
        {
            string id = Request["ID"];

            if (!string.IsNullOrEmpty(id))
            {
                var baseEntity = FormulaHelper.GetEntities<BaseEntities>();
                baseEntity.Set<S_H_ShortCut>().Delete(s => s.ID == id);
                baseEntity.SaveChanges();
            }
            return Json(string.Empty);
        }
        public JsonResult SortShortCut()
        {
            string shortcutstrs = Request["shortcuts"];
            if (!string.IsNullOrEmpty(shortcutstrs))
            {
                List<ShortCut> shortcuts = JsonHelper.ToObject<List<ShortCut>>(shortcutstrs);
                var baseEntity = FormulaHelper.GetEntities<BaseEntities>();
                foreach (var item in shortcuts)
                {
                    var shortcut = baseEntity.Set<S_H_ShortCut>().Where(s => s.ID == item.ID).FirstOrDefault();
                    if (shortcut != null)
                    {
                        shortcut.SortIndex = item.SortIndex;
                    }
                }
                baseEntity.SaveChanges();
            }
            return Json(string.Empty);
        }
        public JsonResult SetShortCutIcon()
        {
            string id = Request["ID"];
            string icon = Request["Icon"];

            if (!string.IsNullOrEmpty(id))
            {
                var baseEntity = FormulaHelper.GetEntities<BaseEntities>();

                var shortcut = baseEntity.Set<S_H_ShortCut>().Where(s => s.ID == id).FirstOrDefault();
                if (shortcut != null)
                {
                    shortcut.IconImage = icon;
                }
                baseEntity.SaveChanges();
            }
            return Json(string.Empty);
        }
        public JsonResult GetShortCut()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteList<ShortCut>(string.Format("select * from S_H_ShortCut where CreateUserID='{0}' order by SortIndex", FormulaHelper.UserID));
            List<ShortCut> list = new List<ShortCut>();
            foreach (ShortCut row in dt)
            {
                if (string.IsNullOrEmpty(row.IconImage)|| row.IconImage.IndexOf('/')>=0)
                    row.IconImage = "glyphicon glyphicon-link";
                if (!string.IsNullOrEmpty(row.Name))
                {
                    string name = row.Name.Clone().ToString();
                    if (name.Length > 4)
                        row.Name = name.Remove(3) + "...";
                    row.Title = name;
                    list.Add(row);
                }
            }
            string json = JsonHelper.ToJson(list);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPortals()
        {
            string json = JsonHelper.ToJson(PortalMain.GetUserRoleTemplets(FormulaHelper.UserID));
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        class ShortCut
        {
            public string ID { get; set; }
            public string IconImage { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public int? SortIndex { get; set; }
        }


        #endregion

        #region 我的消息(board7)

        public JsonResult GetMessage()
        {
            string sql = string.Format(@"
SELECT case when FirstViewTime is null then '未读' else '已读' end State,S_S_MsgBody.Title,S_S_MsgBody.Content,S_S_MsgBody.ContentText, S_S_MsgBody.SendTime, S_S_MsgBody.SenderName,S_S_MsgBody.ID,S_S_MsgReceiver.ID as ReceiverID,S_S_MsgBody.SenderID,S_S_MsgBody.SenderName
FROM S_S_MsgReceiver WITH(NOLOCK) LEFT OUTER JOIN S_S_MsgBody WITH(NOLOCK) ON S_S_MsgBody.ID = S_S_MsgReceiver.MsgBodyID
WHERE (S_S_MsgReceiver.UserID = '{0}') and S_S_MsgReceiver.IsDeleted<>'1'  and S_S_MsgBody.IsDeleted<>'1' ORDER BY S_S_MsgReceiver.ID DESC", FormulaHelper.UserID);

            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(sql, 0, 6, CommandType.Text);
            dt.Columns.Add("Index");
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                dt.Rows[i - 1]["Index"] = i.ToString();
            }

            var json = JsonHelper.ToJson(dt);

            return Json(json, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region 通知公告(board8)

        public JsonResult GetNotice()
        {
            var dt = bl_GetServiceData("Notices", 5);
            var json = JsonHelper.ToJson(dt);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 项目公告(board9)

        public JsonResult GetPJNews()
        {
            var dt = bl_GetServiceData("PJNews", 5);
            var json = JsonHelper.ToJson(dt);
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 私有方法

        //公司新闻、通知公告等获取数据方法
        private DataTable bl_GetServiceData(string blockKey, int? count)
        {
            string UserID = FormulaHelper.UserID;

            DataTable dt = new DataTable();
            //判断PublicInform下是不是有BlockKey对应的栏目
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dtCatlog = sqlHelper.ExecuteDataTable("select * from S_I_PublicInformCatalog where CatalogKey = '" + blockKey + "'");
            string[] arrOrgFullID = FormulaHelper.GetUserInfo().UserFullOrgIDs.Replace(",", ".").Split('.').Distinct().ToArray();
            DataTable userRole = sqlHelper.ExecuteDataTable("select * from S_A__RoleUser where UserID = '" + UserID + "'");
            if (dtCatlog != null && dtCatlog.Rows.Count > 0)
            {
                int iCount = DBNull.Value.Equals(dtCatlog.Rows[0]["InHomePageNum"]) ? (count == null ? 5 : Convert.ToInt32(count)) : Convert.ToInt32(dtCatlog.Rows[0]["InHomePageNum"]);
                if (Config.Constant.IsOracleDb)
                {
                    string whereReceiveDeptId = string.Empty;
                    for (int i = 0; i < arrOrgFullID.Length; i++)
                    {
                        whereReceiveDeptId += "INSTR(ReceiveDeptId,'" + arrOrgFullID[i] + "',1,1) > 0";
                        if (i < arrOrgFullID.Length - 1)
                            whereReceiveDeptId += " or ";
                    }
                    string whereSql = " and ((nvl(ReceiveUserId,'empty') = 'empty' and nvl(ReceiveDeptId,'empty') = 'empty') or (nvl(ReceiveUserId,'empty') <> 'empty' and INSTR(ReceiveUserId,'" + UserID + "',1,1) > 0) or (nvl(ReceiveDeptId,'empty') <> 'empty' and (" + whereReceiveDeptId + "))) ";
                    dt = sqlHelper.ExecuteDataTable("select ID,Title,CatalogId,Urgency,Important,CreateTime,CreateUserName,ContentText from S_I_PublicInformation where CatalogId = '" + dtCatlog.Rows[0]["ID"].ToString() + "' and (ExpiresTime is null or to_char(ExpiresTime,'yyyy-MM-dd') >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "') " + whereSql + " ORDER BY Important DESC,Urgency DESC, CreateTime DESC", 0, iCount, CommandType.Text);
                }
                else
                {
                    string whereReceiveDeptId = string.Empty;
                    for (int i = 0; i < arrOrgFullID.Length; i++)
                    {
                        whereReceiveDeptId += "charindex('" + arrOrgFullID[i] + "',ReceiveDeptId) > 0";
                        if (i < arrOrgFullID.Length - 1)
                            whereReceiveDeptId += " or ";
                    }

                    string whereReceiveRoleId = string.Empty;
                    for (int i = 0; i < userRole.Rows.Count; i++)
                    {
                        whereReceiveRoleId += " or charindex('" + userRole.Rows[i]["RoleID"].ToString() + "',ReceiveRoleId) > 0";
                    }
                    string whereSql = " and ((isnull(ReceiveUserId,'') = '' and isnull(ReceiveDeptId,'') = '') or (isnull(ReceiveUserId,'') <> '' and charindex('" + UserID + "',ReceiveUserId) > 0) or (isnull(ReceiveDeptId,'') <> '' and (" + whereReceiveDeptId + "))) ";
                    var sql = "select * from S_I_PublicInformation where CatalogId = '" + dtCatlog.Rows[0]["ID"].ToString() 
                        + "' and isnull(DeptDoorId,'') = '' and (ExpiresTime is null or ExpiresTime >= convert(datetime,convert(varchar(10),getdate(),120))) " + whereSql 
                        + " and (isnull(ReceiveRoleId,'')='' " + whereReceiveRoleId + ") ORDER BY Important DESC,Urgency DESC, CreateTime DESC";
                    dt = sqlHelper.ExecuteDataTable(sql, 0, iCount, CommandType.Text);
                }
                if (dt != null)
                {
                    dt.Columns.Add("CreateDate");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!DBNull.Value.Equals(dt.Rows[i]["CreateTime"]))
                        {
                            dt.Rows[i]["CreateDate"] = Convert.ToDateTime(dt.Rows[i]["CreateTime"]).ToShortDateString();
                        }
                    }
                }
            }
            else
            {
                switch (blockKey.ToLower())
                {
                    case "friendlink":
                        {
                            int iCount = count == null ? 5 : Convert.ToInt32(count);
                            if (Config.Constant.IsOracleDb)
                            {
                                string whereReceiveDeptId = string.Empty;
                                for (int i = 0; i < arrOrgFullID.Length; i++)
                                {
                                    whereReceiveDeptId += "INSTR(DeptId,'" + arrOrgFullID[i] + "',1,1) > 0";
                                    if (i < arrOrgFullID.Length - 1)
                                        whereReceiveDeptId += " or ";
                                }
                                string whereSql = " and ((nvl(UserId,'empty') = 'empty' and nvl(DeptId,'empty') = 'empty') or (nvl(UserId,'empty') <> 'empty' and INSTR(DeptId,'" + UserID + "',1,1) > 0) or (nvl(DeptId,'empty') <> 'empty' and (" + whereReceiveDeptId + "))) ";
                                dt = sqlHelper.ExecuteDataTable("select * from S_I_FriendLink where 1=1 " + whereSql + " ORDER BY SortIndex", 0, iCount, CommandType.Text);
                            }
                            else
                            {
                                string whereReceiveDeptId = string.Empty;
                                for (int i = 0; i < arrOrgFullID.Length; i++)
                                {
                                    whereReceiveDeptId += "charindex('" + arrOrgFullID[i] + "',DeptId) > 0";
                                    if (i < arrOrgFullID.Length - 1)
                                        whereReceiveDeptId += " or ";
                                }
                                string whereSql = " where ((isnull(UserId,'') = '' and isnull(DeptId,'') = '') or (isnull(UserId,'') <> '' and charindex('" + UserID + "',UserId) > 0) or (isnull(DeptId,'') <> '' and (" + whereReceiveDeptId + "))) ";
                                dt = sqlHelper.ExecuteDataTable("select * from S_I_FriendLink " + whereSql + " ORDER BY SortIndex", 0, iCount, CommandType.Text);
                            }
                            break;
                        }

                }
            }
            return dt;
        }

        #endregion

        #region 获取个人的轮询数据，如提醒、任务、消息数量、是否退出

        public JsonResult GetUserProps()
        {
            //1.补充是否需要验证单帐号登录模式
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLoginSingleton"]))
            {
                string loginName = FormulaHelper.GetUserInfo().Code;
                if (CheckLogin(System.Web.HttpContext.Current.Application, loginName) == 3)
                {
                    //强制退出，跳过计算其他数据
                    return Json(new { LogOut = true }, JsonRequestBehavior.AllowGet);
                }
            }

            //2.取提醒、任务、消息数量
            string userId = FormulaHelper.UserID;
            var obj = new
            {
                LogOut = false,
                TaskCount = GetTaskCount(userId),
                MsgCount = GetMsgCount(userId),
                AlarmCount = GetAlarmCount(userId)
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        private int GetTaskCount(string userID)
        {
            string sql = @"select count(1) from S_WF_InsTaskExec with(nolock)
join S_WF_InsTask with(nolock)  on ExecTime is null and ExecUserID='{0}' and S_WF_InsTask.Type in('Normal','Inital') and (WaitingRoutings='' or WaitingRoutings is null) and (WaitingSteps='' or WaitingSteps is null) and (S_WF_InsTaskExec.CreateTime >= {1}) and S_WF_InsTask.ID=InsTaskID 
join S_WF_InsFlow with(nolock)  on S_WF_InsFlow.Status='Processing' and S_WF_InsFlow.ID=S_WF_InsTask.InsFlowID  ";

            sql = string.Format(sql, userID,
                Config.Constant.IsOracleDb ? string.Format("to_date('{0}','yyyy-MM-dd')", DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd")) : string.Format("'{0}'", DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss"))
                );

            object scal = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow).ExecuteScalar(sql);

            int designCount = 0;
            //try
            //{
            //    sql = "select count(ID) from S_W_Activity where OwnerUserID='" + userID + "' and State='Create' and ActivityKey in ('Design','Collact','Audit','Approve')";
            //    object designCountObject = SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteScalar(sql);

            //    designCount = Convert.ToInt32(designCountObject);
            //}
            //catch (System.Exception ex)
            //{}

            int normalTaskCount = 0;
            try
            {
                sql = "select count(ID) from S_T_TaskExec with(nolock) where ExecUserID='" + userID + "' and ExecTime is null";
                object normalTaskCountObject = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteScalar(sql);

                normalTaskCount = Convert.ToInt32(normalTaskCountObject);
            }
            catch (System.Exception ex)
            { }


            return Convert.ToInt32(scal) + designCount + normalTaskCount;
        }

        private int GetMsgCount(string userID)
        {
            string sql = "select count(ID) from S_S_MsgReceiver with(nolock) where UserID='{0}' and (IsDeleted='0' or isDeleted is null) and FirstViewTime is null";

            object scal = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteScalar(string.Format(sql, userID));

            return Convert.ToInt32(scal);
        }

        private int GetAlarmCount(string userId)
        {
            string sql = string.Format("SELECT count(ID) FROM S_S_Alarm with(nolock) WHERE (IsDelete IS NULL OR IsDelete='0') AND OwnerID='{0}' AND DeadlineTime>='{1}'", userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (Config.Constant.IsOracleDb)
            {
                sql = string.Format("SELECT count(ID) FROM S_S_Alarm with(nolock) WHERE (IsDelete IS NULL OR IsDelete='0') AND OwnerID='{0}' AND DeadlineTime>= to_date('{1}','yyyy-MM-dd HH24:mi:ss')", userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            object scal = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteScalar(sql);
            return Convert.ToInt32(scal);
        }

        #endregion

        #region 友情链接(board10)

        public JsonResult GetFriendLink()
        {

            var dt = bl_GetServiceData("friendlink", 1000);
            var json = JsonHelper.ToJson(dt);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 部门通讯录
        public JsonResult GetDeptUsers()
        {
            var userInfo = FormulaHelper.GetUserInfo();
            var hrBase = SQLHelper.CreateSqlHelper(ConnEnum.HR).DbName;
            var datatable = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(string.Format(@"select a.ID as UserID,a.Name,
                case when isnull(b.MobilePhone,'')='' then a.MobilePhone else b.MobilePhone end MobilePhone,b.JobName
                from S_A_User a left join {1}.dbo.T_Employee b on a.ID=b.UserID
                where a.DeptID='{0}' and a.IsDeleted<>'1'", userInfo.UserOrgID, hrBase));
            return Json(JsonHelper.ToJson(datatable), JsonRequestBehavior.AllowGet);
        }

        public FileResult DeptUserImage(string userID = "")
        {
            string sql = string.Format("select ID,Name,(select Picture from S_A_UserImg where UserID=S_A_User.ID) Picture from S_A_User where ID='{0}'", string.IsNullOrEmpty(userID) ? FormulaHelper.UserID : userID);
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(sql);
            if (dt.Rows.Count == 0 || dt.Rows[0]["Picture"] is DBNull)
            {
                if (dt.Rows.Count > 0)
                {
                    string name = Convert.ToString(dt.Rows[0]["Name"]);
                    userID = Convert.ToString(dt.Rows[0]["ID"]);
                    return File(DrawDefaultIcon(name, userID), "image/png");
                }
                else
                {
                    return File(GetDefaultPortraitFileBytes(), "image/png");
                }
            }
            else
                return File(dt.Rows[0]["Picture"] as byte[], "image/png");
        }

        private byte[] DrawDefaultIcon(string name, string id, int width = 80, int height = 80)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Regex regEnglish = new Regex("^[a-zA-Z]");
                var text = name;
                if (regEnglish.IsMatch(name))
                {
                    if (name.Length >= 4)
                    {
                        text = name.Substring(name.Length - 4);
                    }
                }
                else
                {
                    if (name.Length >= 2)
                    {
                        text = name.Substring(name.Length - 2);
                    }
                }
                Bitmap bit = new Bitmap(width + 1, height + 1);
                Graphics g = Graphics.FromImage(bit);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.Transparent);
                Rectangle rectangle = new Rectangle(0, 0, width, height);
                // 如要使边缘平滑，请取消下行的注释
                g.SmoothingMode = SmoothingMode.HighQuality;

                //设置文本背景色
                SolidBrush[] sbrush =
                {
                    new SolidBrush(Color.FromArgb(110,184,131)),
                    new SolidBrush(Color.FromArgb(216,174,72)),
                    new SolidBrush(Color.FromArgb(235,167,164)),
                    new SolidBrush(Color.FromArgb(166,116,163)),
                    new SolidBrush(Color.FromArgb(196,217,28)),
                    new SolidBrush(Color.FromArgb(91,186,216)),
                    new SolidBrush(Color.FromArgb(55,163,210)),
                    new SolidBrush(Color.FromArgb(129,172,60)),
                    new SolidBrush(Color.FromArgb(234,171,94))
                };

                Random r = new Random();
                Thread.Sleep(10);
                g.FillEllipse(sbrush[r.Next(sbrush.Length)], rectangle);

                Font font = new Font(new FontFamily("微软雅黑"), 20, FontStyle.Regular);

                g.DrawString(text, font, new SolidBrush(Color.White), new Rectangle(0, 21, width, height), new StringFormat { Alignment = StringAlignment.Center });
                MemoryStream ms = new MemoryStream();
                bit.Save(ms, ImageFormat.Png);
                return ms.GetBuffer();
            }
            return null;
        }

        #endregion

        #region 获取用户信息
        public JsonResult GetUser()
        {
            var dataTable = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(string.Format("select ID as UserID,Name as UserName,WorkNo,DeptName as UserOrgName,LastLoginTime from S_A_User where ID='{0}'", FormulaHelper.UserID));
            return Json(JsonHelper.ToJson(dataTable), JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult SaveUserTemplet(string id, string json)
        {
            return Json(PortalMain.SaveUserTemplet(id, json));
        }


        public JsonResult GetPublicInformCatalog(string catalogKey, int pageIndex = 1, int pageSize = 5)
        {
            try
            {
                var userInfo = FormulaHelper.GetUserInfo();
                string userID = userInfo.UserID;
                string[] arrOrgFullID = userInfo.UserFullOrgIDs.Replace(",", ".").Split('.').Distinct().ToArray();
                string whereReceiveRoleId = "";
                //处理接收角色
                IUserService userService = FormulaHelper.GetService<IUserService>();
                var roleIds = userService.GetRolesForUser(userID, "").Split(',');

                foreach (var roleId in roleIds)
                {
                    whereReceiveRoleId += string.Format(" or ReceiveRoleId like '%{0}%'", roleId);
                }
                string whereReceiveDeptId = string.Empty;
                for (int i = 0; i < arrOrgFullID.Length; i++)
                {
                    whereReceiveDeptId += "charindex('" + arrOrgFullID[i] + "',ReceiveDeptId) > 0";
                    if (i < arrOrgFullID.Length - 1)
                        whereReceiveDeptId += " or ";
                }
                string whereSql = " and ((isnull(ReceiveUserId,'') = '' and isnull(ReceiveDeptId,'') = ''  and isnull(ReceiveRoleId,'') = '') or (isnull(ReceiveUserId,'') <> '' and charindex('" + userID + "',ReceiveUserId) > 0) or (isnull(ReceiveDeptId,'') <> '' and (" + whereReceiveDeptId + "))" + whereReceiveRoleId + ") ";
                var table = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable("select a.* from S_I_PublicInformation a left join S_I_PublicInformCatalog b on a.CatalogId = b.id where CatalogKey = '" + catalogKey + "' and isnull(DeptDoorId,'') = '' and (ExpiresTime is null or ExpiresTime >= convert(datetime,convert(varchar(10),getdate(),120))) " + whereSql + " ORDER BY Important DESC,Urgency DESC, CreateTime DESC");
                return Json(JsonHelper.ToJson(table), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var table = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(string.Format(@"SELECT TOP {1} * FROM (
  select top 1000000 a.*,b.CatalogKey,b.CatalogName,b.IsPublic from S_I_PublicInformation a left join S_I_PublicInformCatalog b 
                on a.CatalogId = b.id where  b.CatalogKey='{0}' and IsPublic='1' ORDER BY  CreateTime DESC
) T WHERE id NOT IN(

                     SELECT TOP {2} id FROM (
      select top 1000000 a.*,b.CatalogKey,b.CatalogName,b.IsPublic from S_I_PublicInformation a left join S_I_PublicInformCatalog b 
                on a.CatalogId = b.id where  b.CatalogKey='{0}' and IsPublic='1' ORDER BY  CreateTime DESC                
                     ) TT  ORDER BY CreateTime DESC) ORDER BY CreateTime DESC", catalogKey, pageSize, pageSize * pageIndex - pageSize));
                table.Columns.Add("TableCount", Type.GetType("System.Int32"));
                int count = Convert.ToInt32(SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteScalar(string.Format(@"select count(1) count from S_I_PublicInformation a left join S_I_PublicInformCatalog b 
                on a.CatalogId = b.id where  b.CatalogKey='{0}' and IsPublic='1'", catalogKey)));
                foreach (DataRow dr in table.Rows)
                {
                    dr["TableCount"] = count;
                }
                return Json(JsonHelper.ToJson(table), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetImageNews()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var table = sqlHelper.ExecuteDataTable(@"select top 6 ID,Title,(SELECT rTRIM(ID)+',' FROM S_I_NewsImage where GroupID=a.ID FOR XML PATH('')) as NewsImageIDs,1 IsMis,CreateTime,CreateUserName  
from S_I_NewsImageGroup a where isnull (a.DeptDoorId,'') ='' order by a.SortIndex,a.CreateTime");
            var json = JsonHelper.ToJson(table);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPortalMenus()
        {
            var dt = FormulaHelper.GetService<IResService>().GetResTree(Config.Constant.MenuRooID, FormulaHelper.UserID);
            var json = JsonHelper.ToJson(dt.Select(string.Format("ParentID='{0}'", Config.Constant.MenuRooID)).CopyToDataTable());
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBanners(string type)
        {
            string path = HttpContext.Server.MapPath(string.Format("/PortalLTE/Images/Portal/{0}/Banner", type));
            var files = Directory.GetFiles(path);
            List<string> list = new List<string>();
            foreach (var item in files)
            {
                list.Add(Path.GetFileName(item));
            }
            return Json(list.OrderBy(c => c), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetToken()
        {
            var user = FormulaHelper.GetUserInfo();
            var token = "";
             var secretKey = String.IsNullOrEmpty(ConfigurationManager.AppSettings["SecretKey"]) ? String.Empty : ConfigurationManager.AppSettings["SecretKey"];
            if (!String.IsNullOrEmpty(secretKey))
            {
                var dic = new Dictionary<string, object>();
                dic["systemName"] = user.Code;
                var expiredTimeSpan = 1;
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ExpiredTimeSpan"]))
                {
                    var timeSpan = ConfigurationManager.AppSettings["ExpiredTimeSpan"];
                    if (System.Text.RegularExpressions.Regex.IsMatch(timeSpan, "^[1-9]\\d*$"))
                    {
                        expiredTimeSpan = Convert.ToInt32(timeSpan);
                    }
                }
                var jwtcreatedOver =
    Math.Round((DateTime.UtcNow.AddMinutes(expiredTimeSpan) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + 5);
                dic["exp"] = jwtcreatedOver;// 指定token的生命周期。unix时间戳格式               
                IJwtAlgorithm algorithm = new HMACSHA512Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
                token = encoder.Encode(dic, secretKey);
            }
            var tokens = new
            {
                Token = token,
                TokenKey = String.IsNullOrEmpty(ConfigurationManager.AppSettings["TokenKey"]) ? String.Empty : ConfigurationManager.AppSettings["TokenKey"],
            };
            return Json(tokens,JsonRequestBehavior.AllowGet);
        }
    }


    #region 对称加解密工具类 
    public static class EncryptHelper
    {
        /// <summary>
        /// AES秘钥
        /// </summary>
        private const string aesKey = "Easyman-easyman3";

        /// <summary>
        /// AES偏移向量
        /// </summary>
        private const string aesIV = "Easyman-easyman3";

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="toEncryptString">待加密的明文</param>
        /// <returns></returns>
        public static string AesEncrypt(string toEncryptString)
        {
            var rijndaelCipher = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };

            var toEncryptBytes = Encoding.UTF8.GetBytes(toEncryptString);
            var keyBytes = Encoding.UTF8.GetBytes(aesKey);
            var ivBytes = Encoding.UTF8.GetBytes(aesIV);

            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] cipherBytes = transform.TransformFinalBlock(toEncryptBytes, 0, toEncryptBytes.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="toDecrpt"></param>
        /// <returns></returns>
        public static string AesDecrpt(string toDecrpt)
        {
            var key = Encoding.UTF8.GetBytes(aesKey);
            var encryptedData = Convert.FromBase64String(toDecrpt);
            var iv = Encoding.UTF8.GetBytes(aesIV);

            var rDel = new RijndaelManaged
            {
                Key = key,
                IV = iv,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rDel.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
    }
    #endregion
}
