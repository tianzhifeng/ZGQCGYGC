using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using System.Data;
using MvcAdapter;
using Formula;
using Formula.Helper;
using Newtonsoft.Json;
using System.Configuration;
using Workflow.Logic;

namespace MvcConfig.Areas.Auth.Controllers
{
    public class UserController : BaseController
    {        
        public JsonResult GetDefaultAdvice()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            var dt = sqlHelper.ExecuteDataTable(string.Format("select ID,Advice from S_A_DefaultAdivce where UserID='{0}'", FormulaHelper.UserID));
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddDefaultAdvice(string advice)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string id = FormulaHelper.CreateGuid();
            string sql = string.Format(@"
if(not exists( select * from S_A_DefaultAdivce where UserID='{0}' and Advice='{1}'))
    INSERT INTO S_A_DefaultAdivce(ID,UserID,Advice) VALUES('{2}','{0}','{1}')
", FormulaHelper.UserID, advice, id);
            sqlHelper.ExecuteNonQuery(sql);
            return Json(new { ID=id});
        }

        public JsonResult DelDefaultAdvice(string id)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string sql = string.Format(@"delete from S_A_DefaultAdivce where id='{0}'", id);
            sqlHelper.ExecuteNonQuery(sql);
            return Json(new { result = 1 });
        }

        protected string OwnerDeptOrg = "System_OwnerDept";

        public ActionResult Test()
        {
            return View();
        }

        private void ViewBagTitle()
        {
            var LGID = FormulaHelper.GetCurrentLGID() == "EN";
            ViewBag.EmployeeID = LGID ? "ID" : "工号";
            ViewBag.Name = LGID ? "Name" : "姓名";
            ViewBag.Collect = LGID ? "Collect" : "收藏";
            ViewBag.Department = LGID ? "Department" : "部门";
            ViewBag.OriginalDepartment = LGID ? "Original Department" : "原部门";
            ViewBag.OriginalID = LGID ? "Original ID" : "原工号";
            ViewBag.Choice = LGID ? "Choice" : "选择";
            ViewBag.Cancel = LGID ? "Cancel" : "取消";
            ViewBag.LeaveTime = LGID ? "Leave Time" : "离退日期";
            ViewBag.QueryKey = LGID ? "ID Or Name" : "请输入工号或姓名";
            ViewBag.Personnel = LGID ? "Selected Personnel" : "已选择的人员";
            ViewBag.Dept = LGID ? "D<br />e<br />p<br />t<br />." : "按<br />组<br />织<br />部<br />门";
            ViewBag.Role = LGID ? "R<br />o<br />l<br />e" : "按<br />系<br />统<br />角<br />色";
            ViewBag.Contact = LGID ? "C<br />o<br />n<br />t<br />a<br />c<br />t" : "我<br />的<br />联<br />系<br />人";
            ViewBag.Leave = LGID ? "L<br />e<br />a<br />v<br />e" : "已<br />离<br />退<br />用<br />户";
        }

        public ActionResult SingleSelector()
        {
            ViewBagTitle();
            return View();
        }

        public ActionResult MultiSelector()
        {
            ViewBagTitle();
            return View();
        }

        [ValidateInput(false)]
        public JsonResult DropNode()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;

            Dictionary<string, object> dragNode = JsonHelper.ToObject<Dictionary<string, object>>(Request["dragNode"]);
            Dictionary<string, object> dropNode = JsonHelper.ToObject<Dictionary<string, object>>(Request["dropNode"]);
            string dragAction = Request["dragAction"];

            string sql = "";
            var dropSortIndex = Convert.ToDouble(dropNode["SortIndex"]);
            double newSortIndex;
            if (dragAction == "after")
            {
                sql = string.Format("select min(SortIndex) from S_A_UserLinkMan where UserID='{0}' and SortIndex>'{1}'", userID, dropSortIndex);
                object obj = sqlHelper.ExecuteScalar(sql);
                if (obj == null)
                    newSortIndex = Math.Ceiling(dropSortIndex) + 1;
                else
                    newSortIndex = (Convert.ToDouble(obj) + dropSortIndex) / 2;
            }
            else
            {
                sql = string.Format("select max(SortIndex) from S_A_UserLinkMan where UserID='{0}' and SortIndex<'{1}'", userID, dropSortIndex);
                object obj = sqlHelper.ExecuteScalar(sql);
                if (obj == null || obj.ToString() == "")
                    newSortIndex = Math.Floor(dropSortIndex) - 1;
                else
                    newSortIndex = (Convert.ToDouble(obj) + dropSortIndex) / 2;
            }

            sql = string.Format("Update S_A_UserLinkMan Set SortIndex='{0}' Where UserID='{1}' and LinkManID='{2}'", newSortIndex, userID, dragNode["ID"]);

            sqlHelper.ExecuteNonQuery(sql);

            return Json("");

        }

        public JsonResult DeleteLinkMan(string id)
        {
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            sqlHelper.ExecuteDataTable(string.Format("delete  S_A_UserLinkMan Where UserID='{0}' and LinkManID in ({1})", userID, id));

            return Json("");
        }

        public JsonResult GetOrgUserList(string OrgID, QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }
            var rootFullId = Request["RootFullID"];
            if (string.IsNullOrEmpty(OrgID) && !string.IsNullOrEmpty(rootFullId))
            {
                OrgID = rootFullId.Trim(',', ' ').Split('.').LastOrDefault();
            }
            string key = "";
            if (qb.Items.Count() > 0)
                key = qb.Items[0].Value.ToString();
            string sql = getUserSql(key, OrgID, "", null);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            qb.Items.Clear();
            qb.Fields = "distinct ID,Code,Name,SortIndex,WorkNo,Sex,Phone,MobilePhone,Email,Address,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX";

            if (!string.IsNullOrEmpty(Request["CorpID"]))
            {
                qb.Add("FullID", QueryMethod.InLike, Request["CorpID"]);
            }

            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }


        public JsonResult SaveUserLinkMan(string linkManID)
        {
            linkManID = linkManID.Substring(1, linkManID.Length - 2);
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt1 = sqlHelper.ExecuteDataTable(string.Format("select ID from S_A_UserLinkMan  where UserID='{0}' and LinkManID='{1}' ", userID, linkManID));
            if (dt1.Rows.Count > 0)
                return Json(JsonAjaxResult.Successful("联系人已存在！"));

            string newID = FormulaHelper.CreateGuid();
            object obj = sqlHelper.ExecuteScalar(string.Format("select Max(SortIndex) as SortIndex from S_A_UserLinkMan where UserID='{0}'", userID));
            double maxSort = 0;
            if (obj is DBNull)
                maxSort = 0;
            else
                maxSort = Convert.ToDouble(obj);

            sqlHelper.ExecuteDataTable(string.Format("Insert Into S_A_UserLinkMan (ID,UserID,LinkManID,SortIndex) Values ('{0}','{1}','{2}',{3})", newID, userID, linkManID, Math.Ceiling(maxSort) + 1));
            return Json(JsonAjaxResult.Successful("添加成功！"));
        }


        public JsonResult GetLinkManTree()
        {
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select S_A_User.ID,case when '{1}'='EN' then isnull(S_A_User.NameEN,S_A_User.Name) else S_A_User.Name end as Name,case when '{1}'='EN' then isnull(S_A_User.NameEN,S_A_User.Name) else Name end as UserName,S_A_User.DeptName,S_A_User.DeptID,S_A_User.WorkNo,'Root' as ParentID,S_A_UserLinkMan.SortIndex from S_A_UserLinkMan Join S_A_User on S_A_UserLinkMan.LinkManID=S_A_User.ID where S_A_UserLinkMan.UserID='{0}' Order By SortIndex ASC", userID, FormulaHelper.GetCurrentLGID()));
            foreach (DataRow dr in dt.Rows)
            {
                dr["Name"] = string.Format("<span style=\"hand:cursor;color:blue;text-decoration:underline;\" id=\"{0}\" onclick='LinkManSelect(\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\")'>{5} ({3})</span>(<span style=\"cursor:hand\" onclick='RemoveLinkMan(\"{0}\")' title='从我的联系人移除'><img width=\"16\" height=\"16\" align=\"absmiddle\"  src=\"/Base/Scripts/ShortMsg/delete.png\" border=\"0\" /></span>)", dr["ID"], dr["UserName"], dr["WorkNo"], dr["DeptName"], dr["DeptID"], dr["Name"]);
            }
            var dtRow = dt.NewRow();
            dtRow["ID"] = "Root";
            dtRow["Name"] = "我的联系人(<span style=\"hand:cursor;color:blue;text-decoration:underline;\" onclick=\"DeleteUser(this)\">移除</span>)";
            dtRow["ParentID"] = "-1";
            dtRow["SortIndex"] = "-1";
            dt.Rows.Add(dtRow);
            return Json(dt);
        }

        public JsonResult GetMyLinkManIDs()
        {
            string userID = Formula.FormulaHelper.GetUserInfo().UserID;
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select S_A_User.ID from S_A_UserLinkMan Join S_A_User on S_A_UserLinkMan.LinkManID=S_A_User.ID where S_A_UserLinkMan.UserID='{0}' ", userID));

            return Json(dt);
        }

        public ActionResult SingleScopeSelector()
        {
            return View();
        }

        public ActionResult MultiScopeSelector()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult GetUsers(string ids)
        {
            string sql = "select ID,GroupID,Code,Name,WorkNo,Sex,Description,InDate,OutDate,Phone,MobilePhone,Email,Address,SortIndex,LastLoginTime,LastLoginIP,LastSessionID,ErrorCount,ErrorTime,IsDeleted,DeleteTime,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX,ModifyTime from S_A_User where ID in ('" + ids.Replace(",", "','") + "')";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            if (Config.Constant.IsOracleDb)
            {
                sql = "SELECT ID, WORKNO as \"WorkNo\", NAME as \"Name\" FROM S_A_USER WHERE ID IN ('" + ids.Replace(",", "','") + "')";
            }
            return Json(sqlHelper.ExecuteDataTable(sql), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDobackUserList(string userIDs, QueryBuilder qb)
        {
            string sql = string.Format("select * from S_A_User where ID in ('{0}')", userIDs.Replace(",", "','"));
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }


        [AllowAnonymous]
        public JsonResult GetScopeUserList(string orgIDs, string roleIDs, string aptitude, QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");

            string userIDs = GetQueryString("UserIDs");//地址传递过来UserIDs，则从UserIDs里面选
            if (userIDs != "")
            {
                string sql = string.Format("select * from S_A_User where ID in('{0}')", userIDs.Replace(",", "','"));

                qb.Fields = "distinct ID,Code,Name,SortIndex,WorkNo,Sex,Phone,MobilePhone,Email,Address,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX";
                return Json(sqlHelper.ExecuteGridData(sql, qb));
            }
            else
            {
                //roleIDs可能为编号,转换为roleIDs
                if (!string.IsNullOrEmpty(roleIDs))
                {
                    DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select ID from S_A_Role where Code in ('{0}')", roleIDs.Replace(",", "','")));
                    if (dt.Rows.Count > 0)
                        roleIDs = string.Join(",", dt.AsEnumerable().Select(c => c["ID"].ToString()).ToArray());
                }


                string key = "";

                if (qb.Items.Count() > 0)
                    key = qb.Items[0].Value.ToString();

                string sql = getUserSql(key, orgIDs, roleIDs, aptitude);

                qb.Items.Clear();
                qb.Fields = "distinct ID,Code,Name,SortIndex,WorkNo,Sex,Phone,MobilePhone,Email,Address,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX";
                return Json(sqlHelper.ExecuteGridData(sql, qb));
            }
        }

        [AllowAnonymous]
        public JsonResult SelectUsers(string key, string value, string orgIDs, string roleIDs, string aptitude)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            string whereStr = string.Empty;
            key = key.Replace("'", "''");//查询条件的单引号
            if (!string.IsNullOrEmpty(key))
            {
                int firstCode = (int)key[0];
                if (48 <= firstCode && firstCode <= 57)	//number
                    if (Config.Constant.IsOracleDb)
                        whereStr = " \"WorkNo\" like '" + key + "%'";
                    else
                        whereStr = " WorkNo like '" + key + "%'";
                else if ((65 <= firstCode && firstCode <= 90) || (97 <= firstCode && firstCode <= 122))	//letter
                {
                    string[,] hz = GetHanziScope(key);
                    for (int i = 0; i < hz.GetLength(0); i++)
                    {
                        if (!string.IsNullOrEmpty(whereStr))
                            whereStr += " and ";
                        if (Config.Constant.IsOracleDb)
                            whereStr += "nlssort(SUBSTR(\"Name\", " + (i + 1) + ", 1),'NLS_SORT=SCHINESE_PINYIN_M') >= nlssort('" + hz[i, 0] + "','NLS_SORT=SCHINESE_PINYIN_M') AND nlssort(SUBSTR(\"Name\", " + (i + 1) + ", 1),'NLS_SORT=SCHINESE_PINYIN_M') <= nlssort('" + hz[i, 1] + "','NLS_SORT=SCHINESE_PINYIN_M')";
                        else
                            whereStr += "SUBSTRING(Name, " + (i + 1) + ", 1) >= '" + hz[i, 0] + "' AND SUBSTRING(Name, " + (i + 1) + ", 1) <= '" + hz[i, 1] + "'";
                    }

                    if (Config.Constant.IsOracleDb)
                        whereStr = string.Format("  ({0}) or regexp_like(Name,'{1}','i')", whereStr, key);
                    else
                        whereStr = string.Format("  ({0}) or Name like '{1}%'", whereStr, key);
                }
                else if (firstCode >= 255)
                {	//chinese
                    if (Config.Constant.IsOracleDb)
                        whereStr = "\"Name\" like '%" + key + "%'";
                    else
                        whereStr = "Name like '%" + key + "%'";
                }
            }
            if (!string.IsNullOrEmpty(value))
            {
                if (!string.IsNullOrEmpty(whereStr))
                    whereStr += " and ";
                whereStr += "ID not in ('" + value.Replace(",", "','") + "')";
            }

            string sql = "";
            if (Config.Constant.IsOracleDb)
                sql = "select * from (select S_A_User.ID,S_A_User.CODE as \"Code\",WORKNO as \"WorkNo\", S_A_User.NAME as \"Name\",SEX as \"Sex\", DeptID,DEPTNAME as \"DeptName\",FullID from S_A_User join S_A__OrgUser on UserID=S_A_User.ID join S_A_Org on OrgID=S_A_Org.ID where nvl(S_A_User.IsDeleted,0) = 0) Users ";
            else
                sql = "select * from (select S_A_User.ID,S_A_User.Code,WorkNo,S_A_User.Name,Sex,DeptID,DeptName,FullID from S_A_User join S_A__OrgUser on UserID=S_A_User.ID join S_A_Org on OrgID=S_A_Org.ID where isnull(S_A_User.IsDeleted,0) = 0) Users ";

            if (!string.IsNullOrEmpty(orgIDs) || !string.IsNullOrEmpty(roleIDs))
            {
                sql = "select * from (" + GetScopeSql(orgIDs, roleIDs) + ") Users ";
            }
            else if (!string.IsNullOrEmpty(aptitude))
            {
                sql = "select * from (" + GetAptitudeSql(aptitude) + ") Users ";
            }

            if (!string.IsNullOrEmpty(whereStr))
                sql += " where " + whereStr;

            if (Config.Constant.IsOracleDb)
            {
                sql = "select * from (" + sql + ") FilterUsers Where rownum <= 10";

            }
            else
            {
                sql = "select top 20 * from (" + sql + ") FilterUsers order by WorkNo";
            }
            SearchCondition cnd = new SearchCondition();
            cnd.Fields = "distinct ID,WorkNo,Name,DeptID,DeptName";
            return Json(sqlHelper.ExecuteDataTable(sql, cnd), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetRetiredUserList(QueryBuilder qb)
        {
            string sql = string.Format("select ID,GroupID,Code,case when '{0}'='EN' then isnull(NameEN,Name) else name end as Name,WorkNo,Sex,Description,InDate,OutDate,Phone,MobilePhone,Email,Address,SortIndex,LastLoginTime,LastLoginIP,LastSessionID,ErrorCount,ErrorTime,IsDeleted,DeleteTime,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX,ModifyTime from S_A_User where IsDeleted='1'", FormulaHelper.GetCurrentLGID());
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper("Base");
            return Json(JsonHelper.ToJson(sqlHelper.ExecuteGridData(sql, qb)));
        }

        [AllowAnonymous]
        public JsonResult GetProjectScopeUserList(string orgIDs, string roleIDs, QueryBuilder qb)
        {
            var dt = PrjRoleExt.GetScopeUserList(orgIDs, roleIDs, qb);
            var data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        #region 私有方法

        private string getUserSql(string key, string orgIDs, string roleIDs, string aptitude)
        {
            //if (string.IsNullOrEmpty(orgIDs))
            //    orgIDs = Config.Constant.OrgRootID;

            if (string.IsNullOrEmpty(key))
            {
                if (string.IsNullOrEmpty(aptitude))
                    return GetScopeSql(orgIDs, roleIDs);
                else
                    return GetAptitudeSql(aptitude);
            }
            else
            {
                string whereStr = "";
                if (!string.IsNullOrEmpty(key))
                {
                    if (key.Contains(',') || key.Contains('，')) //当key中有逗号时
                    {
                        whereStr = " where 1=2";
                        foreach (string str in key.Split(',', '，'))
                        {
                            whereStr += string.Format(" or Name like '{0}%'", str);
                        }
                    }
                    else
                    {
                        int firstCode = (int)key[0];

                        string[,] hz = GetHanziScope(key);
                        for (int i = 0; i < hz.GetLength(0); i++)
                        {
                            if (whereStr != "")
                                whereStr += " and";
                            if (Config.Constant.IsOracleDb)
                                whereStr += " nlssort(SUBSTR(Name, " + (i + 1) + ", 1),'NLS_SORT=SCHINESE_PINYIN_M') >= nlssort('" + hz[i, 0] + "','NLS_SORT=SCHINESE_PINYIN_M') AND nlssort(SUBSTR(Name, " + (i + 1) + ", 1),'NLS_SORT=SCHINESE_PINYIN_M') <= nlssort('" + hz[i, 1] + "','NLS_SORT=SCHINESE_PINYIN_M')";
                            else
                                whereStr += " SUBSTRING(Name, " + (i + 1) + ", 1) >= '" + hz[i, 0] + "' AND SUBSTRING(Name, " + (i + 1) + ", 1) < '" + hz[i, 1] + "'";
                        }
                        if (Config.Constant.IsOracleDb)
                            whereStr = string.Format(" where ({0}) or regexp_like(WorkNo,'{1}','i') or regexp_like(Name,'{1}','i')", whereStr, key);
                        else
                            whereStr = string.Format(" where ({0}) or WorkNo like '{1}%' or Name like '{1}%'", whereStr, key);
                    }

                }
                string sql = string.Format("select * from ( {0} ) Users ", string.IsNullOrEmpty(aptitude) ? GetScopeSql(orgIDs, roleIDs) : GetAptitudeSql(aptitude));
                sql += whereStr;

                return sql;
            }

        }

        private string[,] GetHanziScope(string pinyinIndex)
        {
            pinyinIndex = pinyinIndex.ToLower();
            string[,] hz = new string[pinyinIndex.Length, 2];
            for (int i = 0; i < pinyinIndex.Length; i++)
            {
                string index = pinyinIndex.Substring(i, 1);
                if (index == "a") { hz[i, 0] = "吖"; hz[i, 1] = "驁"; }
                else if (index == "b") { hz[i, 0] = "八"; hz[i, 1] = "簿"; }
                else if (index == "c") { hz[i, 0] = "嚓"; hz[i, 1] = "錯"; }
                else if (index == "d") { hz[i, 0] = "咑"; hz[i, 1] = "鵽"; }
                else if (index == "e") { hz[i, 0] = "妸"; hz[i, 1] = "樲"; }
                else if (index == "f") { hz[i, 0] = "发"; hz[i, 1] = "猤"; }
                else if (index == "g") { hz[i, 0] = "旮"; hz[i, 1] = "腂"; }
                else if (index == "h") { hz[i, 0] = "妎"; hz[i, 1] = "夻"; }
                else if (index == "j") { hz[i, 0] = "丌"; hz[i, 1] = "攈"; }
                else if (index == "k") { hz[i, 0] = "咔"; hz[i, 1] = "穒"; }
                else if (index == "l") { hz[i, 0] = "垃"; hz[i, 1] = "鱳"; }
                else if (index == "m") { hz[i, 0] = "嘸"; hz[i, 1] = "椧"; }
                else if (index == "n") { hz[i, 0] = "拏"; hz[i, 1] = "桛"; }
                else if (index == "o") { hz[i, 0] = "噢"; hz[i, 1] = "漚"; }
                else if (index == "p") { hz[i, 0] = "妑"; hz[i, 1] = "曝"; }
                else if (index == "q") { hz[i, 0] = "七"; hz[i, 1] = "裠"; }
                else if (index == "r") { hz[i, 0] = "亽"; hz[i, 1] = "鶸"; }
                else if (index == "s") { hz[i, 0] = "仨"; hz[i, 1] = "蜶"; }
                else if (index == "t") { hz[i, 0] = "他"; hz[i, 1] = "籜"; }
                else if (index == "w") { hz[i, 0] = "屲"; hz[i, 1] = "鶩"; }
                else if (index == "x") { hz[i, 0] = "夕"; hz[i, 1] = "鑂"; }
                else if (index == "y") { hz[i, 0] = "丫"; hz[i, 1] = "韻"; }
                else if (index == "z") { hz[i, 0] = "帀"; hz[i, 1] = "咗"; }
                else { hz[i, 0] = index; hz[i, 1] = index; }
            }
            return hz;
        }

        private string GetScopeSql(string orgIDs, string roleIDs)
        {
            string field = string.Format("S_A_User.ID,S_A_User.Code,case when '{0}'='EN' then isnull(S_A_User.NameEN,S_A_User.Name) else S_A_User.Name end as Name,S_A_User.SortIndex, WorkNo,Sex,Phone,MobilePhone,Email,Address,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX,FullID", FormulaHelper.GetCurrentLGID());

            if (string.IsNullOrWhiteSpace(orgIDs) && string.IsNullOrWhiteSpace(roleIDs))
                return string.Format("select {0} from S_A_User join S_A__OrgUser on UserID=S_A_User.ID join S_A_Org on S_A_Org.ID=OrgID where S_A_User.IsDeleted<>'1'", field);
            else if (string.IsNullOrWhiteSpace(roleIDs) == true)
            {
                string orgStr = "";
                if (!string.IsNullOrEmpty(orgIDs) && ConfigurationManager.AppSettings.AllKeys.Contains("UserSelectOnlyCurrent") && ConfigurationManager.AppSettings["UserSelectOnlyCurrent"].ToLower().Equals("true"))
                    orgStr = string.Format(" and DeptID in ('{0}') ", orgIDs.Replace(",", "','"));
                else
                    orgStr = string.Format(" and OrgID in('{0}')", orgIDs.Replace(",", "','"));

                return string.Format("select {1} from S_A_User join S_A__OrgUser on UserID=S_A_User.ID join S_A_Org on S_A_Org.ID=OrgID where S_A_User.IsDeleted<>'1' {0} ", orgStr, field);
            }
            else
            {

                string userIDs = FormulaHelper.GetService<IRoleService>().GetUserIDsInRoles(roleIDs, orgIDs);
                //2018-1-30 剥离项目角色选人功能
                var prjRoleUser = PrjRoleExt.GetRoleUserIDs(roleIDs, orgIDs);
                if (!string.IsNullOrEmpty(prjRoleUser))
                    userIDs = (prjRoleUser + "," + userIDs).Trim(',');

                //Oracle的in查询长度不能超过1000
                var arr = userIDs.Split(',');
                string where = "";
                var i = 0;
                while (i * 1000 < arr.Length)
                {
                    where += string.Format(" or S_A_User.ID in('{0}')", string.Join("','", arr.Skip(i * 1000).Take(1000)));
                    i++;
                }
                where = where.Substring(4);
                string sql = string.Format("select {1} from S_A_User join S_A__OrgUser on UserID=S_A_User.ID join S_A_Org on S_A_Org.ID=OrgID where S_A_User.IsDeleted<>'1' and {0}", where, field);
                return sql;
            }

        }

        private string GetAptitudeSql(string aptitude)
        {
            string hrBaseName = SQLHelper.CreateSqlHelper(ConnEnum.HR).DbName;
            string field = string.Format("S_A_User.ID,S_A_User.Code,case when '{0}'='EN' then isnull(S_A_User.NameEN,S_A_User.Name) else S_A_User.Name end as Name,S_A_User.SortIndex, WorkNo,Sex,Phone,MobilePhone,Email,Address,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX", FormulaHelper.GetCurrentLGID());

            var aptiParam = JsonConvert.DeserializeObject<AptitudeParam>(aptitude);
            string whereStr = "";
            if (!string.IsNullOrEmpty(aptiParam.Major))
                whereStr += " and Major='" + aptiParam.Major + "'";
            if (!string.IsNullOrEmpty(aptiParam.ProjectClass))
                whereStr += " and ProjectClass='" + aptiParam.ProjectClass + "'";
            if (!string.IsNullOrEmpty(aptiParam.Position))
                whereStr += " and Position='" + aptiParam.Position + "'";
            if (aptiParam.AptitudeLevel != 0)
                whereStr += " and AptitudeLevel>=" + aptiParam.AptitudeLevel.ToString();
            string whereApplyStr = whereStr;
            if (!string.IsNullOrEmpty(aptiParam.ProjectInfoID))
                whereApplyStr += " and ProjectInfoID='" + aptiParam.ProjectInfoID + "'";

            string sql = string.Format(@"select {0} from S_A_User join (
select distinct UserID from {1}..S_D_UserAptitude where 1=1 {2}
union
select distinct UserID from {1}..S_D_UserAptitudeApply where 1=1 {3})
b on S_A_User.ID=b.UserID", field, hrBaseName, whereStr, whereApplyStr);

            return sql;
        }

        struct AptitudeParam
        {
            public string Major;
            public string ProjectClass;
            public string Position;
            public int AptitudeLevel;
            public string ProjectInfoID;
        }

        #endregion

        public JsonResult TestGetUsers(string roleIDs, string orgIDs)
        {
            IRoleService roleService = FormulaHelper.GetService<IRoleService>();
            IUserService userService = FormulaHelper.GetService<IUserService>();

            string userIDs = roleService.GetUserIDsInRoles(roleIDs, orgIDs);
            //2018-1-30 剥离项目角色选人功能
            var prjRoleUser = PrjRoleExt.GetRoleUserIDs(roleIDs, orgIDs);
            if (!string.IsNullOrEmpty(prjRoleUser))
                userIDs = (prjRoleUser + "," + userIDs).Trim(',');

            string userNames = userService.GetUserNames(userIDs);

            return Json(new { UserIDs = userIDs, UserNames = userNames });

        }
    }
}
