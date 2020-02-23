using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Base.Logic.Domain;
using Config;
using System.Data;
using Formula.Helper;
using Base.Logic.BusinessFacade;
using Formula;
using System.Web.Security;
using System.Text;
using Newtonsoft.Json;
using Aspose.Words.Reporting;
using Formula.Exceptions;
using Base.Logic;

namespace Base.Areas.Auth.Controllers
{
    public class UserController : BaseController<S_A_User>
    {
        #region Excel 批量导入
        //public JsonResult VaildExcelDataUser()
        //{
        //    var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
        //    string data = reader.ReadToEnd();
        //    var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
        //    var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

        //    var orgs = entities.Set<S_A_Org>().ToList();
        //    var errors = excelData.Vaildate(e =>
        //    {
        //        if (e.FieldName == "DeptName" && !string.IsNullOrWhiteSpace(e.Value))
        //        {
        //            var dept = orgs.FirstOrDefault(o => o.Name == e.Value);
        //            if (dept == null)
        //            {
        //                e.IsValid = false;
        //                e.ErrorText = string.Format("部门（{0}）不存在！", e.Value);
        //            }
        //        }
        //    });

        //    return Json(errors);
        //}

        //public JsonResult BatchSaveUser()
        //{
        //    var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
        //    string data = reader.ReadToEnd();
        //    var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
        //    var list = JsonConvert.DeserializeObject<List<S_A_User>>(tempdata["data"]);

        //    entities.Configuration.AutoDetectChangesEnabled = false;
        //    var oldUsers = entities.Set<S_A_User>().ToList();
        //    var orgs = entities.Set<S_A_Org>().ToList();
        //    foreach (var user in list)
        //    {
        //        var newUser = oldUsers.FirstOrDefault(u => u.Code == user.Code);
        //        if (newUser != null)
        //        {
        //            newUser.Name = user.Name;
        //            newUser.Sex = user.Sex;
        //            newUser.WorkNo = user.WorkNo;
        //            newUser.Phone = user.Phone;
        //            newUser.MobilePhone = user.MobilePhone;
        //            newUser.Email = user.Email;
        //        }
        //        else
        //        {
        //            newUser = new S_A_User();
        //            newUser.ID = FormulaHelper.CreateGuid();
        //            newUser.Code = user.Code;
        //            newUser.Name = user.Name;
        //            newUser.Sex = user.Sex;
        //            newUser.WorkNo = user.WorkNo;
        //            newUser.Phone = user.Phone;
        //            newUser.MobilePhone = user.MobilePhone;
        //            newUser.Email = user.Email;
        //            newUser.DeptID = "";
        //            newUser.GroupID = "a1b10168-61a9-44b5-92ca-c5659456deb5";
        //            entities.Set<S_A_User>().Add(newUser);
        //        }

        //        if (!string.IsNullOrWhiteSpace(user.DeptName))
        //        {
        //            var dept = orgs.FirstOrDefault(o => o.Name == user.DeptName);
        //            newUser.DeptID = dept.ID;
        //            newUser.DeptFullID = dept.FullID;
        //            newUser.DeptName = dept.Name;
        //        }

        //        if (string.IsNullOrEmpty(newUser.Password))
        //            newUser.Password = newUser.Code.GetHashCode().ToString();
        //        if (newUser.S_A__OrgUser.Count == 0)
        //            newUser.S_A__OrgUser.Add(new S_A__OrgUser() { UserID = newUser.ID, OrgID = Config.Constant.OrgRootID });

        //    }
        //    try
        //    {
        //        entities.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWriter.Error(ex);
        //        throw ex;
        //    }
        //    entities.Configuration.AutoDetectChangesEnabled = true;
        //    return Json("Success");
        //}

        //public ActionResult ExportWord(string id)
        //{
        //    var key = "user";
        //    var wordExporter = new AsposeWordExporter();

        //    var ds = new DataSet();
        //    var user = entities.Set<S_A_User>().Find(id);
        //    ds.AddList(user, "User");

        //    var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
        //    var tmplPath = path.EndsWith("\\") ? string.Format("{0}{1}.xls", path, key) : string.Format("{0}\\{1}.doc", path, key);

        //    var buffer = wordExporter.ExportWord(ds, tmplPath);

        //    if (buffer != null)
        //    {
        //        return File(buffer, "application/vnd.ms-word", Url.Encode("用户信息") + ".doc");
        //    }

        //    return Content("导出数据失败，请检查相关配置！");
        //}
        #endregion

        public override ActionResult Edit()
        {
            ViewBag.enumCompony = "null";
            return View();
        }

        public JsonResult GetUserInfo()
        {
            string ID = FormulaHelper.GetUserInfo().UserID;
            var user = base.entities.Set<S_A_User>().Where(c => c.ID == ID).SingleOrDefault();
            return Json(user);
        }

        public void SaveUserInfo(string data)
        {
            var json = JsonHelper.ToObject<List<Dictionary<string, string>>>(data);
            if (json.Count > 0)
            {
                foreach (var item in json)
                {
                    string ID = item["ID"].ToString();
                    var user = base.entities.Set<S_A_User>().Where(c => c.ID == ID).SingleOrDefault();
                    user.Email = item["Email"];
                    user.Phone = item["Phone"];
                    user.MobilePhone = item["MobilePhone"];
                    user.Address = item["Address"];
                }
                entities.SaveChanges();
            }
        }

        public override JsonResult GetModel(string id)
        {
            AuthFO authFO = FormulaHelper.CreateFO<AuthFO>();
            var entity = GetEntity<S_A_User>(id);
            string deptNames = authFO.GetUserDeptNames(id);
            var dic = FormulaHelper.ModelToDic(entity);
            dic.Add("DeptNames", deptNames);
            return Json(dic);
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }

            if (!string.IsNullOrEmpty(Request["CorpID"]))
                qb.Add("CorpID", QueryMethod.In, Request["CorpID"]);


            qb.Fields = "distinct ID,Code,Name,WorkNo,Sex,InDate,OutDate,Phone,MobilePhone,Email,Address,SortIndex,IsDeleted,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX,ModifyTime,ErrorCount,CorpID";

            string sql = @"
SELECT S_A_User.ID,S_A_User.Code,S_A_User.Name,WorkNo,Sex,InDate,OutDate,Phone,MobilePhone,Email,Address,S_A_User.SortIndex,S_A_User.IsDeleted,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX,ModifyTime,FullID,S_A_User.ErrorCount,CorpID
 from S_A_User
join S_A__OrgUser on UserID=S_A_User.ID
join S_A_Org on S_A_Org.ID=OrgID
where S_A_User.IsDeleted='0' or S_A_User.IsDeleted is null";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);
            dt.Columns.Add("DeptNames");
            AuthFO authFO = FormulaHelper.CreateFO<AuthFO>();
            foreach (DataRow row in dt.Rows)
            {
                row["DeptNames"] = authFO.GetUserDeptNames(row["ID"].ToString());
            }

            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public JsonResult GetRetiredList(QueryBuilder qb)
        {
            if (qb.DefaultSort)
            {
                qb.SortField = "SortIndex,WorkNo";
                qb.SortOrder = "asc,asc";
            }

            string sql = @"select ID,GroupID,Code,Name,WorkNo,Sex,Description,InDate,OutDate,Phone,MobilePhone,Email,Address,SortIndex,LastLoginTime,LastLoginIP,LastSessionID,ErrorCount,ErrorTime,IsDeleted,DeleteTime,PrjID,PrjName,DeptID,DeptFullID,DeptName,RTX,ModifyTime
            from S_A_User where IsDeleted='1'";
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);
            dt.Columns.Add("DeptNames");
            AuthFO authFO = FormulaHelper.CreateFO<AuthFO>();
            foreach (DataRow row in dt.Rows)
            {
                row["DeptNames"] = authFO.GetUserDeptNames(row["ID"].ToString());
            }

            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public override JsonResult Save()
        {
            var user = base.UpdateEntity<S_A_User>();

            if (entities.Set<S_A_User>().Count(c => c.Code == user.Code && c.ID != user.ID) > 0)
                throw new Exception("用户账号不能重复");

            if (!string.IsNullOrEmpty(user.RTX))
            {
                if (entities.Set<S_A_User>().Count(c => c.RTX == user.RTX && c.ID != user.ID) > 0)
                    throw new Exception("RTX帐号不能重复");
            }
            user.Code = user.Code.Trim();
            if (string.IsNullOrEmpty(user.Password))
                user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Code.ToLower(), "SHA1");
            if (user.S_A__OrgUser.Count == 0)
                user.S_A__OrgUser.Add(new S_A__OrgUser() { UserID = user.ID, OrgID = Config.Constant.OrgRootID });

            var currentUser = FormulaHelper.GetUserInfo();

            if (user._state == EntityStatus.added.ToString())
            {
                if (string.IsNullOrEmpty(user.CorpID))
                {
                    var rootOrg = entities.Set<S_A_Org>().FirstOrDefault(c => string.IsNullOrEmpty(c.ParentID));
                    user.CorpID = rootOrg.ID;
                    user.CorpName = rootOrg.Name;
                }
                if (string.IsNullOrEmpty(user.CorpName))
                {
                    var org = entities.Set<S_A_Org>().SingleOrDefault(c => c.ID == user.CorpID);
                    user.CorpName = org.Name;
                }


                if (!string.IsNullOrEmpty(user.CorpID))
                {
                    S_A__OrgUser orgUser = user.S_A__OrgUser.FirstOrDefault(c => c.OrgID == user.CorpID);
                    if (orgUser == null)
                    {
                        orgUser = new S_A__OrgUser();
                        orgUser.OrgID = user.CorpID;
                        orgUser.UserID = user.ID;
                        entities.Set<S_A__OrgUser>().Add(orgUser);
                    }
                }
            }

            return base.JsonSave<S_A_User>(user);
        }

        public JsonResult Reset(string UserIDs)
        {
            var arr = UserIDs.Split(',');
            var users = entities.Set<S_A_User>().Where(c => arr.Contains(c.ID)).ToList();
            foreach (var user in users)
            {
                user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", user.Code.Trim().ToLower(), Request["pwd"]), "SHA1");
            }
            //记录安全审计日志
            AuthFO.Log("修改密码", string.Join(",", users.Select(c => c.Name)), "********");
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult ResetAllUserPwd(string pwd)
        {

            var userList = entities.Set<S_A_User>().OrderBy(c => c.ID).Take(1000);

            int index = 1;
            while (userList.Count() > 0)
            {
                foreach (var user in userList)
                {
                    user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", user.Code.Trim().ToLower(), pwd), "SHA1");
                }
                entities.SaveChanges();
                userList = entities.Set<S_A_User>().OrderBy(c => c.ID).Skip(index * 1000).Take(1000);
                index++;
            }
            //记录安全审计日志
            AuthFO.Log("修改密码", "全部用户", "********");
            return Json("");
        }

        public JsonResult Unlock(string UserIDs)
        {
            var ids = UserIDs.Split(',');
            var users = entities.Set<S_A_User>().Where(c => ids.Contains(c.ID)).ToList();
            foreach (var user in users)
            {
                user.ErrorCount = 0;
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult RetireUser(string UserIDs)
        {
            AuthFO authBF = FormulaHelper.CreateFO<AuthFO>();
            foreach (var id in UserIDs.Split(','))
            {
                authBF.RetireUser(id);
            }
            return Json("");
        }

        public ActionResult RetireList()
        {
            return View();
        }

        #region 图片

        public ActionResult MyPic()
        {
            ViewBag.IsHasDwg = true;
            var UserID = FormulaHelper.UserID;
            S_A_UserImg sa = entities.Set<S_A_UserImg>().FirstOrDefault(c => c.UserID == UserID);
            if (sa == null || sa.DwgFile == null)
                ViewBag.IsHasDwg = false;

            return View();
        }

        public ActionResult UploadPic()
        {
            ViewBag.IsHasDwg = true;
            var UserID = GetQueryString("UserID");
            S_A_UserImg sa = entities.Set<S_A_UserImg>().FirstOrDefault(c => c.UserID == UserID);
            if (sa == null || sa.DwgFile == null)
                ViewBag.IsHasDwg = false;

            return View();
        }

        public JsonResult UploadImg(string UserID, bool isPortrait = false)
        {
            if (string.IsNullOrEmpty(UserID))
                UserID = FormulaHelper.UserID;

            if (Request.Files.Count > 0)
            {
                var t = Request.Files[0].InputStream;
                byte[] bt = new byte[t.Length];
                t.Read(bt, 0, int.Parse(t.Length.ToString()));

                S_A_UserImg sa = entities.Set<S_A_UserImg>().Where(c => c.UserID == UserID).SingleOrDefault();
                if (sa == null)
                {
                    sa = new S_A_UserImg() { ID = FormulaHelper.CreateGuid(), UserID = UserID };
                    entities.Set<S_A_UserImg>().Add(sa);
                }
                if (isPortrait)
                    sa.Picture = bt;
                else
                    sa.SignImg = bt;
                entities.SaveChanges();
            }
            return Json("");
        }

        public JsonResult UploadDwg(string UserID)
        {
            if (string.IsNullOrEmpty(UserID))
                UserID = FormulaHelper.UserID;

            if (Request.Files.Count > 0)
            {
                var t = Request.Files[0].InputStream;
                byte[] bt = new byte[t.Length];
                t.Read(bt, 0, int.Parse(t.Length.ToString()));

                S_A_UserImg sa = entities.Set<S_A_UserImg>().Where(c => c.UserID == UserID).SingleOrDefault();
                if (sa == null)
                {
                    sa = new S_A_UserImg() { ID = FormulaHelper.CreateGuid(), UserID = UserID };
                    entities.Set<S_A_UserImg>().Add(sa);
                }
                sa.DwgFile = bt;
                var user = entities.Set<S_A_User>().FirstOrDefault(a => a.ID == UserID);
                if (user != null) user.ModifyTime = DateTime.Now;
                entities.SaveChanges();
            }
            return Json("");
        }

        public FileResult DownloadDwg(string UserID)
        {
            if (string.IsNullOrEmpty(UserID))
                UserID = FormulaHelper.UserID;

            S_A_User user = entities.Set<S_A_User>().FirstOrDefault(a => a.ID == UserID);
            if (user == null) throw new Exception("用户不存在");
            S_A_UserImg sa = entities.Set<S_A_UserImg>().FirstOrDefault(c => c.UserID == UserID);
            if (sa == null) throw new Exception("尚未上传Dwg签名文件");
            byte[] dwgFile = sa.DwgFile;
            if (dwgFile == null || dwgFile.Length == 0) throw new Exception("尚未上传Dwg签名文件");
            string contentType = "application/x-dwg";
            return File(dwgFile, contentType, user.Name + ".dwg");
        }

        public JsonResult FreeSign(string UserID, string imgType = "Sign")
        {
            if (string.IsNullOrEmpty(UserID))
                UserID = FormulaHelper.UserID;

            S_A_UserImg sa = entities.Set<S_A_UserImg>().Where(c => c.UserID == UserID).SingleOrDefault();
            if (sa != null)
            {
                if (imgType == "Sign")
                    sa.SignImg = null;
                else if (imgType == "Portrait")
                    sa.Picture = null;
                else if (imgType == "Dwg")
                {
                    sa.DwgFile = null;
                    var user = entities.Set<S_A_User>().FirstOrDefault(a => a.ID == UserID);
                    if (user != null) user.ModifyTime = DateTime.Now;
                }
                entities.SaveChanges();
            }
            return Json(new { ImgType = imgType });
        }

        public JsonResult SetSignPwd(string pwd, string UserID)
        {
            if (string.IsNullOrEmpty(UserID))
                UserID = FormulaHelper.UserID;

            var user = entities.Set<S_A_User>().SingleOrDefault(c => c.ID == UserID);
            if (string.IsNullOrEmpty(pwd))
                user.SignPwd = null;
            else
                user.SignPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", user.ID, pwd), "SHA1");
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 权限一览表

        public JsonResult GetResList(QueryBuilder qb)
        {
            string rootID = Config.Constant.MenuRooID;
            if (!string.IsNullOrEmpty(Request["IsRuleView"]))
                rootID = Config.Constant.RuleRootID;


            var users = entities.Set<S_A_User>().Where(qb);

            var resList = entities.Set<S_A_Res>().Where(c => c.FullID.StartsWith(rootID)).ToList();
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            string sql = "select * from S_A_User where IsDeleted='0'";
            DataTable dtUser = sqlHelper.ExecuteDataTable(sql, qb);
            string userIDs = string.Join("','", dtUser.AsEnumerable().Select(c => c.Field<string>("ID")));

            //Oracle数据库字段名长度最大为30，Oracle的substr函数比C#的多1

            sql = @"
select * from
(
select S_A_User.DeptName, S_A_User.Name,S_A_User.Code,S_A_User.WorkNo,S_A_User.ID as UserID,'a'+ {2} as ID from S_A_User  join 
(
--组织权限
select UserID, S_A_Res.* from S_A__OrgUser 
join S_A__OrgRes on S_A__OrgRes.OrgID=S_A__OrgUser.OrgID 
join S_A_Res on S_A_Res.ID=ResID
union
--系统角色权限
select UserID,S_A_Res.* from S_A__RoleUser 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__RoleUser.RoleID 
join S_A_Res on S_A_Res.ID=ResID 
union
--组织角色权限
select UserID,S_A_Res.* from S_A__OrgUser 
join S_A__OrgRole on S_A__OrgUser.OrgID=S_A__OrgRole.OrgID 
join S_A__RoleRes on S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A_Res on S_A_Res.ID=ResID
) tb1 on S_A_User.ID in('{1}') and tb1.UserID = S_A_User.ID 
) a pivot (count(UserID) for ID in({0}))b
";

            if (Config.Constant.IsOracleDb)
            {
                sql = string.Format(sql, "'" + string.Join("','"
                    , resList.Select(c => "a" + c.ID.Replace('-', '_').Substring(10)).Distinct().ToArray()) + "'"
                    , userIDs
                    , "SUBSTR(replace(tb1.ID,'-','_'),11)"
                   );

                DataTable dt = sqlHelper.ExecuteDataTable(sql);
                foreach (DataColumn col in dt.Columns)
                {
                    col.ColumnName = col.ColumnName.Trim('\'');
                }
                GridData data = new GridData(dt);
                data.total = qb.TotolCount;

                return Json(data);
            }
            else
            {
                sql = string.Format(sql
                    , string.Join(",", resList.Select(c => "a" + c.ID.Replace('-', '_').Substring(10)).Distinct().ToArray())
                    , userIDs
                    , "SUBSTRING(replace(tb1.ID,'-','_'),11,26)"
                   );

                DataTable dt = sqlHelper.ExecuteDataTable(sql);
                GridData data = new GridData(dt);
                data.total = qb.TotolCount;

                return Json(data);
            }



        }

        private string getColHtml()
        {
            string rootID = Config.Constant.MenuRooID;
            if (!string.IsNullOrEmpty(Request["IsRuleView"]))
                rootID = Config.Constant.RuleRootID;

            var resList = entities.Set<S_A_Res>().Where(c => c.FullID.StartsWith(rootID)).ToList();
            StringBuilder sb = new StringBuilder();

            var rootRes = resList.Where(c => string.IsNullOrEmpty(c.ParentID)).FirstOrDefault();

            foreach (var res in resList.Where(c => c.ParentID == rootRes.ID))
            {
                sb.AppendFormat("<div field='{0}'>{1} {2}</div>", "a" + res.ID.Replace('-', '_').Substring(10), res.Name, GetColHtml(res.ID, resList));
            }
            return sb.ToString();
        }

        private string GetColHtml(string parentResID, List<S_A_Res> resList)
        {

            var childRes = resList.Where(c => c.ParentID == parentResID);
            if (childRes.Count() == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("<div property='columns'>");
            foreach (var item in resList.Where(c => c.ParentID == parentResID))
            {
                string childCol = GetColHtml(item.ID, resList);
                string colName = item.Name;
                string width = "";
                if (childCol == "")
                {
                    colName = string.Join("<br/>", colName.ToArray());
                    width = "width='20'";
                }
                sb.AppendFormat("<div field='{0}' {3}>{1} {2}</div>", "a" + item.ID.Replace('-', '_').Substring(10), colName, childCol, width);
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        public ViewResult ResView()
        {
            ViewBag.Fields = getColHtml();
            return View();
        }

        public JsonResult GetResFrom(string UserCode, string resID)
        {
            resID = resID.Substring(1).Replace('_', '-');

            resID = entities.Set<S_A_Res>().SingleOrDefault(c => c.ID.EndsWith(resID)).ID;

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("select ID from S_A_User where Code='{0}'", UserCode);
            string userID = sqlHelper.ExecuteScalar(sql).ToString();


            sql = @"

select ID,Name from S_A__OrgRes
join S_A__OrgUser on ResID='{1}' and UserID='{0}' and S_A__OrgRes.OrgID=S_A__OrgUser.OrgID
join S_A_Org on S_A_Org.ID=S_A__OrgRes.OrgID
union
select ID,Name from S_A__RoleRes
join S_A__RoleUser on ResID='{1}' and UserID='{0}' and S_A__RoleRes.RoleID=S_A__RoleUser.RoleID
join S_A_Role on S_A_Role.ID=S_A__RoleRes.RoleID
union
select ID,Name from S_A__RoleRes
join S_A__OrgRole on ResID='{1}' and S_A__RoleRes.RoleID=S_A__OrgRole.RoleID
join S_A__OrgUser on UserID='{0}' and S_A__OrgRole.OrgID=S_A__OrgUser.OrgID
join S_A_Role on S_A_Role.ID=S_A__RoleRes.RoleID

";

            sql = string.Format(sql, userID, resID);



            DataTable dt = sqlHelper.ExecuteDataTable(sql);

            string result = string.Join("\n", dt.AsEnumerable().Select(c => c.Field<string>("Name")).ToArray());

            return Json(new { userName = result });
        }

        #endregion

        #region 用户授权

        public JsonResult GetUserRes(string nodeID)
        {
            return base.JsonGetRelationAll<S_A_User, S_A__UserRes, S_A_Res>(nodeID);
        }

        public JsonResult SetUserRes(string nodeFullID, string relationData, string fullRelation)
        {
            var originalList = entities.Set<S_A__UserRes>().Where(c => c.UserID == nodeFullID && c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID));
            return base.JsonSetRelation<S_A_User, S_A__UserRes, S_A_Res>(nodeFullID, relationData, originalList);
        }

        public JsonResult SetUserRule(string nodeFullID, string relationData, string fullRelation)
        {
            var originalList = entities.Set<S_A__UserRes>().Where(c => c.UserID == nodeFullID && c.S_A_Res.FullID.StartsWith(Config.Constant.RuleRootID));
            return base.JsonSetRelation<S_A_User, S_A__UserRes, S_A_Res>(nodeFullID, relationData, originalList);
        }

        #endregion

        public JsonResult DoReset(string NewCode)
        {
            string ListIDs = Request["ListIDs"];
            if (string.IsNullOrWhiteSpace(ListIDs))
                return Json("");
            var list = entities.Set<S_A_User>().ToList();
            var List = list.Where(t => ListIDs.Contains(t.ID)).ToList();
            foreach (var item in List)
            {
                item.IsDeleted = "0";
                item.DeleteTime = null;
                if (list.Any(a => a.ID != item.ID && a.Code == NewCode && a.IsDeleted == "0"))
                    throw new BusinessValidationException("账号【" + NewCode + "】已经存在");
                item.Code = NewCode;
                #region 部门
                if (!string.IsNullOrWhiteSpace(item.DeptFullID))
                {
                    string[] OrgIDs = item.DeptFullID.Split('.');
                    foreach (var OrgID in OrgIDs)
                    {
                        var relation = item.S_A__OrgUser.Where(t => t.OrgID == OrgID).FirstOrDefault();
                        if (relation == null)
                        {
                            relation = new S_A__OrgUser();
                            relation.UserID = item.ID;
                            relation.OrgID = OrgID;
                            item.S_A__OrgUser.Add(relation);
                        }
                    }
                }
                else
                {
                    var relation = item.S_A__OrgUser.Where(t => t.OrgID == Config.Constant.OrgRootID).FirstOrDefault();
                    if (relation == null)
                    {
                        relation = new S_A__OrgUser();
                        relation.UserID = item.ID;
                        relation.OrgID = Config.Constant.OrgRootID;
                        item.S_A__OrgUser.Add(relation);
                    }
                }
                #endregion
            }
            entities.SaveChanges();
            return Json("");
        }
    }
}
