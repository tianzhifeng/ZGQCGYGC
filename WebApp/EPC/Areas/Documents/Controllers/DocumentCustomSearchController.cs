using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;
using Newtonsoft.Json;
using Formula.ImportExport;
using Base.Logic.Domain;
using System.ComponentModel;

namespace EPC.Areas.Documents.Controllers
{
    public class DocumentCustomSearchController : EPCFormContorllor<S_D_Document>
    {
        #region Project
        public ActionResult Project()
        {
            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");

            var infrastructureDbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
            var folderTemplate = infrastructureDbContext.Set<S_T_FolderTemplate>().FirstOrDefault(a => a.ModeKey.Contains(engineeringInfo.Mode.ID));
            if (folderTemplate == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的文档目录结构模板");

            ViewBag.Property = "[]";
            ViewBag.Attribute = "[]";
            ViewBag.QuickSearchName = "";
            ViewBag.QuickSearchCode = "";
            if (!string.IsNullOrEmpty(folderTemplate.DisplayColJson))
            {
                var dicList = JsonHelper.ToList(folderTemplate.DisplayColJson);
                ViewBag.ColDefine = dicList;//.Where(a => a.GetValue("IsDisplay") == "true");
                ViewBag.Attribute = JsonHelper.ToJson(dicList.Where(a => a.GetValue("IsAttr") == "true" && a.GetValue("IsCustomSearch") == "true")
                                           .Select(a => new { text = a.GetValue("Name"), value = a.GetValue("Code") }));
                ViewBag.Property = JsonHelper.ToJson(dicList.Where(a => a.GetValue("IsAttr") == "false" && a.GetValue("IsCustomSearch") == "true")
                                             .Select(a => new { text = a.GetValue("Name"), value = a.GetValue("Code") }));

                ViewBag.QuickSearchName = string.Join("或", dicList.Where(a => a.GetValue("IsQuickSearch") == "true")
                                             .Select(a => a.GetValue("Name")));
                ViewBag.QuickSearchCode = string.Join(",", dicList.Where(a => a.GetValue("IsQuickSearch") == "true")
                                             .Select(a => a.GetValue("Code")));
            }

            return View();
        } 
        public JsonResult QuickSearchDocuments(QueryBuilder qb, string EngineeringInfoID, bool ShowMine, string FolderID)
        {
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            string selectSql = @"select S_D_Document.*,isnull(authTable.WriteAuth,0) as WriteAuth,isnull(authTable.DownLoadAuth,0) as DownLoadAuth,isnull(authTable.BrowseAuth,0) as BrowseAuth 
                               from S_D_Document left join (" + GetAuthSql(EngineeringInfoID) + ") authTable on authTable.FolderID = S_D_Document.FolderID";

            string conditionSql = " where EngineeringInfoID = '{0}' ";
            if (ShowMine)
            {
                conditionSql += " and S_D_Document.CreateUserID = '" + CurrentUserInfo.UserID + "'";
            }

            if (!string.IsNullOrEmpty(FolderID))
            {
                conditionSql += " and S_D_Document.FolderID = '" + FolderID + "'";
            }

            string sql = string.Format(selectSql + conditionSql + " order by CreateDate desc", EngineeringInfoID);
            var dt = db.ExecuteDataTable(sql, qb);
            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }
        public JsonResult TimeSpanSearchDocuments(string EngineeringInfoID, bool ShowMine, string SpanDescrib, string FolderID)
        {
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            string selectSql = @"select S_D_Document.*,isnull(authTable.WriteAuth,0) as WriteAuth,isnull(authTable.DownLoadAuth,0) as DownLoadAuth,isnull(authTable.BrowseAuth,0) as BrowseAuth 
                               from S_D_Document left join (" + GetAuthSql(EngineeringInfoID) + ") authTable on authTable.FolderID = S_D_Document.FolderID";

            string conditionSql = " where EngineeringInfoID = '{0}' ";
            DateTime now = DateTime.Now;
            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;
            if (SpanDescrib == "day")
            {
                dtStart = new DateTime(now.Year, now.Month, now.Day);
                dtEnd = dtStart.AddDays(1);                
            }
            else if (SpanDescrib == "week")
            {
                DateTime weekStart = now.AddDays(-(int)now.DayOfWeek);
                DateTime weekEnd = now.AddDays(7 - (int)now.DayOfWeek);
                dtStart = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day);
                dtEnd = new DateTime(weekEnd.Year, weekEnd.Month, weekEnd.Day);
            }
            else if (SpanDescrib == "month")
            {
                dtStart = new DateTime(now.Year, now.Month, 1);
                dtEnd = dtStart.AddMonths(1);
            }

            conditionSql += " and " + string.Format("S_D_Document.CreateDate >= '{0}' and S_D_Document.CreateDate < '{1}'", dtStart, dtEnd);
            if (ShowMine)
            {
                conditionSql += " and S_D_Document.CreateUserID = '" + CurrentUserInfo.UserID + "'";
            }

            if (!string.IsNullOrEmpty(FolderID))
            {
                conditionSql += " and S_D_Document.FolderID = '" + FolderID + "'";
            }

            string sql = string.Format(selectSql + conditionSql + " order by S_D_Document.CreateDate desc", EngineeringInfoID);
            var dt = db.ExecuteDataTable(sql);
            return Json(dt);
        }
        public JsonResult CustomSearchDocuments(QueryBuilder qb, string EngineeringInfoID, bool ShowMine, string PropertyList, string FolderID)
        {
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            string selectSql = @"select S_D_Document.*,authTable.FolderName, isnull(authTable.WriteAuth,0) as WriteAuth,isnull(authTable.DownLoadAuth,0) as DownLoadAuth,isnull(authTable.BrowseAuth,0) as BrowseAuth 
                               from S_D_Document left join (" + GetAuthSql(EngineeringInfoID) + ") authTable on authTable.FolderID = S_D_Document.FolderID";
           
            string conditionSql = " where EngineeringInfoID = '{0}' ";
            if (!string.IsNullOrEmpty(PropertyList))
            {
                var tmpList = JsonHelper.ToList(PropertyList);
                if (tmpList.Any(a => a.GetValue("cType") == "Attr"))
                {
                    selectSql += " left join S_D_Document_Attr on S_D_Document.ID = S_D_Document_Attr.DocumentID ";
                }

                foreach (var d in tmpList)
                {
                    if (string.IsNullOrEmpty(d.GetValue("cType")) ||
                        string.IsNullOrEmpty(d.GetValue("cValue")) ||
                        string.IsNullOrEmpty(d.GetValue("formula")) ||
                        string.IsNullOrEmpty(d.GetValue("condition")))
                    {
                        continue;
                    }

                    if (d.GetValue("cType") == "Base")
                    {
                        conditionSql += " and " + GetSqlFormula("S_D_Document." + d.GetValue("cValue"), d.GetValue("formula"), d.GetValue("condition")) + " ";
                    }
                    else if (d.GetValue("cType") == "Attr")
                    {
                        conditionSql += " and " + GetSqlFormula("S_D_Document_Attr.AttrName", "equal", d.GetValue("cValue")) + " ";
                        conditionSql += " and " + GetSqlFormula("S_D_Document_Attr.AttrValue", d.GetValue("formula"), d.GetValue("condition")) + " ";
                    }
                    else if (d.GetValue("cType") == "Folder")
                    {
                        conditionSql += " and " + GetSqlFormula("FolderName", d.GetValue("formula"), d.GetValue("condition")) + " ";
                    }
                }
            }

            if (ShowMine)
            {
                conditionSql += " and S_D_Document.CreateUserID = '" + CurrentUserInfo.UserID + "'";
            }

            if (!string.IsNullOrEmpty(FolderID))
            {
                conditionSql += " and S_D_Document.FolderID = '" + FolderID + "'";
            }

            string sql = string.Format(selectSql + conditionSql + " order by CreateDate desc", EngineeringInfoID);

            var dt = db.ExecuteDataTable(sql, qb);
            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }
        private string GetSqlFormula(string left, string formula, string right)
        {
            if (formula == "equal")
                return left + "='" + right + "'";
            else if (formula == "contain")
                return left + " like " + "'%" + right + "%'";
            else
                return "";
        }        
        private string GetAuthSql(string EngineeringInfoID)
        {
            #region 权限表查询 authSql
            var userService = FormulaHelper.GetService<IUserService>();
            var sysRoles = userService.GetRoleCodesForUser(this.CurrentUserInfo.UserID, string.Empty);
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            var projectRole = engineeringInfo.GetUserOBSCode(this.CurrentUserInfo.UserID);
            var roleCodes = (sysRoles.Trim().TrimEnd(',') + "," + projectRole.Trim()).Replace(",", "','");
            string authSql = @"
            --多角色权限求和
            select FolderID,FolderName, 
            sum(convert(int,(case WriteAuth when 'False' then 0 when 'True' then 1 else WriteAuth end))) as WriteAuth,
            sum(convert(int,(case DownLoadAuth when 'False' then 0 when 'True' then 1 else DownLoadAuth end))) as DownLoadAuth,
            sum(convert(int,(case BrowseAuth when 'False' then 0 when 'True' then 1 else BrowseAuth end))) as BrowseAuth
            from
            --查询继承父项权限的数据
            (SELECT 
            S_D_Folder.ID as FolderID,
            S_D_Folder.Name as FolderName,
            isnull(parentsAuth.WriteAuth,'0') as WriteAuth,
            isnull(parentsAuth.DownLoadAuth,'0') as DownLoadAuth, 
            isnull(parentsAuth.BrowseAuth,'0') as BrowseAuth,
            parentsAuth.RelateID as RelateID 
            FROM S_D_Folder 
            --连接查询父项的权限
            left join 
            (select S_D_Folder_Auth.*,S_D_Folder.InhertAuth,S_D_Folder.FullID from S_D_Folder inner join S_D_Folder_Auth on S_D_Folder.ID = S_D_Folder_Auth.FolderID) parentsAuth
            on S_D_Folder.FullID like '%'+ parentsAuth.FolderID + '%'
            --过滤父项权限,只取最高级父项的权限
            inner join
            (SELECT 
            S_D_Folder.ID as FolderID,
            min(len(parentsAuth.FullID)) as minParentFullIDLength, 
            parentsAuth.RelateID as ParentRelateID FROM S_D_Folder
            left join (select S_D_Folder_Auth.*,S_D_Folder.InhertAuth,S_D_Folder.FullID, S_D_Folder.Name from S_D_Folder inner join S_D_Folder_Auth on S_D_Folder.ID = S_D_Folder_Auth.FolderID) parentsAuth
            on S_D_Folder.FullID like '%'+ parentsAuth.FolderID + '%'
            where S_D_Folder.InhertAuth = 'True' and S_D_Folder.EngineeringInfoID = '{0}'
            group by S_D_Folder.ID,  parentsAuth.RelateID) lengthTb
            on lengthTb.minParentFullIDLength = len(parentsAuth.FullID) and lengthTb.FolderID = S_D_Folder.ID and lengthTb.ParentRelateID = parentsAuth.RelateID
            --组合未继承父项权限的数据
            union all
            select 
            S_D_Folder.ID as FolderID, 
            S_D_Folder.Name as FolderName,
            isnull(WriteAuth,'0') as WriteAuth,
            isnull(DownLoadAuth,'0') as DownLoadAuth, 
            isnull(BrowseAuth,'0') as BrowseAuth,
            S_D_Folder_Auth.RelateID
            from S_D_Folder
            left join S_D_Folder_Auth on S_D_Folder.ID = S_D_Folder_Auth.FolderID
            where S_D_Folder.InhertAuth = 'False' and S_D_Folder.EngineeringInfoID = '{0}') cTB where RelateID in ('" + roleCodes + "') group by FolderID,FolderName";
            #endregion
            authSql = string.Format(authSql, EngineeringInfoID);
            return authSql;
        }
        #endregion

        #region ProjectGroup
        public ActionResult ProjectGroup()
        {
            var keyValueList = GetS_D_DocumentPropertyNameList();
            keyValueList.Insert(0, new KeyValuePair<string, string>("项目名称", "S_I_Engineering.Name"));
            ViewBag.Property = JsonHelper.ToJson(keyValueList.Select(a => new { text = a.Key, value = a.Value }).ToList());
            ViewBag.Attribute = GetAttrList();
            return View();
        }
        public JsonResult GroupQuickSearchDocuments(QueryBuilder qb, bool ShowMine)
        {
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            string selectSql = @"select S_I_Engineering.Name as EngineeringName,S_D_Document.*,isnull(authTable.WriteAuth,0) as WriteAuth,isnull(authTable.DownLoadAuth,0) as DownLoadAuth,isnull(authTable.BrowseAuth,0) as BrowseAuth 
                               from S_D_Document left join (" + GetAuthSql() + @") authTable on authTable.FolderID = S_D_Document.FolderID 
                               inner join S_I_Engineering on S_D_Document.EngineeringInfoID = S_I_Engineering.ID";

            string conditionSql = " where 1 = 1 ";
            if (ShowMine)
            {
                conditionSql += " and S_D_Document.CreateUserID = '" + CurrentUserInfo.UserID + "'";
            }

            string sql = string.Format(selectSql + conditionSql + " order by CreateDate desc");
            var dt = db.ExecuteDataTable(sql, qb);
            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public JsonResult GroupTimeSpanSearchDocuments(bool ShowMine, string SpanDescrib)
        {
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            string selectSql = @"select S_I_Engineering.Name as EngineeringName,S_D_Document.*,isnull(authTable.WriteAuth,0) as WriteAuth,isnull(authTable.DownLoadAuth,0) as DownLoadAuth,isnull(authTable.BrowseAuth,0) as BrowseAuth 
                               from S_D_Document left join (" + GetAuthSql() + @") authTable on authTable.FolderID = S_D_Document.FolderID
                               inner join S_I_Engineering on S_D_Document.EngineeringInfoID = S_I_Engineering.ID";

            string conditionSql = " where 1=1 ";
            DateTime now = DateTime.Now;
            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;
            if (SpanDescrib == "day")
            {
                dtStart = new DateTime(now.Year, now.Month, now.Day);
                dtEnd = dtStart.AddDays(1);
            }
            else if (SpanDescrib == "week")
            {
                DateTime weekStart = now.AddDays(-(int)now.DayOfWeek);
                DateTime weekEnd = now.AddDays(7 - (int)now.DayOfWeek);
                dtStart = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day);
                dtEnd = new DateTime(weekEnd.Year, weekEnd.Month, weekEnd.Day);
            }
            else if (SpanDescrib == "month")
            {
                dtStart = new DateTime(now.Year, now.Month, 1);
                dtEnd = dtStart.AddMonths(1);
            }

            conditionSql += " and " + string.Format("S_D_Document.CreateDate >= '{0}' and S_D_Document.CreateDate < '{1}'", dtStart, dtEnd);
            if (ShowMine)
            {
                conditionSql += " and S_D_Document.CreateUserID = '" + CurrentUserInfo.UserID + "'";
            }

            string sql = string.Format(selectSql + conditionSql + " order by S_D_Document.CreateDate desc");
            var dt = db.ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult GroupCustomSearchDocuments(QueryBuilder qb, bool ShowMine, string PropertyList)
        {
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            string selectSql = @"select S_I_Engineering.Name as EngineeringName,S_D_Document.*,authTable.FolderName, isnull(authTable.WriteAuth,0) as WriteAuth,isnull(authTable.DownLoadAuth,0) as DownLoadAuth,isnull(authTable.BrowseAuth,0) as BrowseAuth 
                               from S_D_Document left join (" + GetAuthSql() + @") authTable on authTable.FolderID = S_D_Document.FolderID
                               inner join S_I_Engineering on S_D_Document.EngineeringInfoID = S_I_Engineering.ID";

            string conditionSql = " where 1=1 ";
            if (!string.IsNullOrEmpty(PropertyList))
            {
                var tmpList = JsonHelper.ToList(PropertyList);
                if (tmpList.Any(a => a.GetValue("cType") == "Attr"))
                {
                    selectSql += " left join S_D_Document_Attr on S_D_Document.ID = S_D_Document_Attr.DocumentID ";
                }

                foreach (var d in tmpList)
                {
                    if (string.IsNullOrEmpty(d.GetValue("cType")) ||
                        string.IsNullOrEmpty(d.GetValue("cValue")) ||
                        string.IsNullOrEmpty(d.GetValue("formula")) ||
                        string.IsNullOrEmpty(d.GetValue("condition")))
                    {
                        continue;
                    }

                    if (d.GetValue("cType") == "Base")
                    {
                        conditionSql += " and " + GetSqlFormula(d.GetValue("cValue"), d.GetValue("formula"), d.GetValue("condition")) + " ";
                    }
                    else if (d.GetValue("cType") == "Attr")
                    {
                        conditionSql += " and " + GetSqlFormula("S_D_Document_Attr.AttrName", "equal", d.GetValue("cValue")) + " ";
                        conditionSql += " and " + GetSqlFormula("S_D_Document_Attr.AttrValue", d.GetValue("formula"), d.GetValue("condition")) + " ";
                    }
                }
            }

            if (ShowMine)
            {
                conditionSql += " and S_D_Document.CreateUserID = '" + CurrentUserInfo.UserID + "'";
            }

            string sql = string.Format(selectSql + conditionSql + " order by CreateDate desc");

            var dt = db.ExecuteDataTable(sql, qb);
            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        private List<KeyValuePair<string, string>> GetS_D_DocumentPropertyNameList()
        {
            List<KeyValuePair<string, string>> res = new List<KeyValuePair<string, string>>();
            var properties = typeof(S_D_Document).GetProperties();
            foreach (var property in properties)
            {
                object[] arr = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                string key = property.Name;

                if (arr.Length > 0 && !string.IsNullOrEmpty(((DescriptionAttribute)arr[0]).Description))
                {
                    key = ((DescriptionAttribute)arr[0]).Description;
                }
                else
                {
                    //有Description注释才作为查询字段
                    continue;
                }

                string val = property.Name;
                res.Add(new KeyValuePair<string, string>(key, "S_D_Document." + val));
            }
            return res;
        }

        private string GetAttrList()
        {
            List<KeyValuePair<string, string>> dyList = new List<KeyValuePair<string, string>>();
            List<string> attrDefine = EPCEntites.Set<S_D_Folder>().Select(a => a.AttrDefine).ToList();
            foreach (var define in attrDefine)
            {
                if (string.IsNullOrEmpty(define))
                    continue;

                var dicList = JsonHelper.ToList(define);
                foreach (var dic in dicList)
                {
                    if (dyList.Any(a => a.Value == dic.GetValue("Code")))
                    {
                        continue;
                    }
                    dyList.Add(new KeyValuePair<string, string>(dic.GetValue("Name"), dic.GetValue("Code")));
                }
            }
            return JsonHelper.ToJson(dyList.Select(a => new { text = a.Key, value = a.Value }));
        }

        private string GetAuthSql()
        {
            #region 权限表查询 authSql
            var userService = FormulaHelper.GetService<IUserService>();
            var sysRoles = userService.GetRoleCodesForUser(this.CurrentUserInfo.UserID, string.Empty);
            var roleCodes = (sysRoles.Trim().TrimEnd(',')).Replace(",", "','");
            string authSql = @"
            --多角色权限求和
            select S_I_OBS_User.UserID, auth.RelateID, FolderID,FolderName,auth.EngineeringInfoID,
            sum(WriteAuth) as WriteAuth,
            sum(DownLoadAuth) as DownLoadAuth,
            sum(BrowseAuth) as BrowseAuth
            from S_I_OBS_User
            inner join
            (select FolderID,FolderName,RelateID,EngineeringInfoID,
            convert(int,(case WriteAuth when 'False' then 0 when 'True' then 1 else WriteAuth end)) as WriteAuth,
            convert(int,(case DownLoadAuth when 'False' then 0 when 'True' then 1 else DownLoadAuth end)) as DownLoadAuth,
            convert(int,(case BrowseAuth when 'False' then 0 when 'True' then 1 else BrowseAuth end)) as BrowseAuth
            from
            --查询继承父项权限的数据
            (SELECT 
            S_D_Folder.ID as FolderID,
            S_D_Folder.Name as FolderName,
            S_D_Folder.EngineeringInfoID as EngineeringInfoID,
            isnull(parentsAuth.WriteAuth,'0') as WriteAuth,
            isnull(parentsAuth.DownLoadAuth,'0') as DownLoadAuth, 
            isnull(parentsAuth.BrowseAuth,'0') as BrowseAuth,
            parentsAuth.RelateID as RelateID 
            FROM S_D_Folder 
            --连接查询父项的权限
            left join 
            (select S_D_Folder_Auth.*,S_D_Folder.InhertAuth,S_D_Folder.FullID from S_D_Folder inner join S_D_Folder_Auth on S_D_Folder.ID = S_D_Folder_Auth.FolderID) parentsAuth
            on S_D_Folder.FullID like '%'+ parentsAuth.FolderID + '%'
            --过滤父项权限,只取最高级父项的权限
            inner join
            (SELECT 
            S_D_Folder.ID as FolderID,
            min(len(parentsAuth.FullID)) as minParentFullIDLength, 
            parentsAuth.RelateID as ParentRelateID FROM S_D_Folder
            left join (select S_D_Folder_Auth.*,S_D_Folder.InhertAuth,S_D_Folder.FullID, S_D_Folder.Name from S_D_Folder inner join S_D_Folder_Auth on S_D_Folder.ID = S_D_Folder_Auth.FolderID) parentsAuth
            on S_D_Folder.FullID like '%'+ parentsAuth.FolderID + '%'
            where S_D_Folder.InhertAuth = 'True'
            group by S_D_Folder.ID,  parentsAuth.RelateID) lengthTb
            on lengthTb.minParentFullIDLength = len(parentsAuth.FullID) and lengthTb.FolderID = S_D_Folder.ID and lengthTb.ParentRelateID = parentsAuth.RelateID
            --组合未继承父项权限的数据
            union all
            select 
            S_D_Folder.ID as FolderID, 
            S_D_Folder.Name as FolderName,
            S_D_Folder.EngineeringInfoID as EngineeringInfoID,
            isnull(WriteAuth,'0') as WriteAuth,
            isnull(DownLoadAuth,'0') as DownLoadAuth, 
            isnull(BrowseAuth,'0') as BrowseAuth,
            S_D_Folder_Auth.RelateID
            from S_D_Folder
            left join S_D_Folder_Auth on S_D_Folder.ID = S_D_Folder_Auth.FolderID
            where S_D_Folder.InhertAuth = 'False') cTB) auth on (S_I_OBS_User.EngineeringInfoID = auth.EngineeringInfoID and auth.RelateID = S_I_OBS_User.RoleCode) or auth.RelateID In ('" + roleCodes + @"')
            where S_I_OBS_User.UserID = '{0}'
            group by S_I_OBS_User.UserID,auth.RelateID, FolderID,FolderName,auth.EngineeringInfoID";
            #endregion
            authSql = string.Format(authSql, CurrentUserInfo.UserID);
            return authSql;
        }

        #endregion
    }
}
