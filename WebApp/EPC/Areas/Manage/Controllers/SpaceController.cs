
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
using System.Text.RegularExpressions;
using System.Text;

namespace EPC.Areas.Manage.Controllers
{
    public class SpaceController : EPCController
    {
        private const int MaxEngineeringNameLength = 50;
        public ActionResult Index()
        {
            string EngineeringInfoID = this.Request["EngineeringInfoID"];
            string WBSID = this.Request["WBSID"];
            var IsOpenForm = this.GetQueryString("IsOpenForm");
            if (String.IsNullOrEmpty(EngineeringInfoID))
            {
                var defaultEnter = this.GetDefaultEnter();
                if (defaultEnter != null)
                {
                    WBSID = defaultEnter.WBSID;
                    EngineeringInfoID = defaultEnter.EngineeringInfoID;
                }
            }
            var wbs = this.GetEntityByID<S_I_WBS>(WBSID);
            if (wbs == null)
            {
                ViewBag.DefaultEnterID = "";
                ViewBag.DefaultEngineeringInfoD = "";
                ViewBag.DefaultTitle = "";
            }
            else
            {
                ViewBag.DefaultEnterID = wbs.ID;
                ViewBag.DefaultEngineeringInfoD = wbs.EngineeringInfoID;
                ViewBag.DefaultTitle = wbs.Name;
            }
            var infraDb = SQLHelper.CreateSqlHelper(ConnEnum.Infrastructure);
            var dt = infraDb.ExecuteDataTable(@"select distinct NavWBSType from dbo.S_C_Meun where NavWBSType is not null and NavWBSType<>''");
            var type = String.Join(",", dt.AsEnumerable().Select(c => c["NavWBSType"].ToString()).ToList());
            if (String.IsNullOrEmpty(type))
                type = "Root";
            else if (!type.Contains("Root"))
            {
                type += ",Root";
            }
            ViewBag.NavNodeType = type;
            return View();
        }

        public JsonResult GetTreeList(QueryBuilder qb)
        {
            var showRoot = this.GetQueryString("ShowRoot");
            var sql = @"select S_I_WBS.ID,S_I_WBS.Name,S_I_WBS.Code,S_I_WBS.EngineeringInfoID,WBSAuthUser,SortIndex,ParentID,
S_I_Engineering.ChargerUserName,S_I_Engineering.ChargerDeptName,BuildDate as CreateDate from S_I_WBS 
left join S_I_Engineering on S_I_WBS.EngineeringInfoID=S_I_Engineering.ID
where S_I_WBS.ID=S_I_WBS.FullID ";
            var dt = this.SqlHelper.ExecuteDataTable(sql, qb);
            var resultDt = dt.Clone();

            var totalCount = qb.TotolCount;
            if (S_T_DefineParams.Params.GetValue("ShowWBSNav") == true.ToString().ToLower())
            {
                //启用WBS空间 则需要获取PWBS 节点数据
                var engineeringInfoIDs = String.Join(",", dt.AsEnumerable().Where(c => c["EngineeringInfoID"] != null && c["EngineeringInfoID"] != DBNull.Value
                    && !String.IsNullOrEmpty(c["EngineeringInfoID"].ToString())).Select(c => c["EngineeringInfoID"].ToString()).ToList());
                sql = @"select  S_I_WBS.ID,S_I_WBS.Name,S_I_WBS.Code,S_I_WBS.EngineeringInfoID,WBSAuthUser,SortIndex,ParentID,
ChargerUserName,ChargerDeptName,CreateDate from S_I_WBS
where ParentID<>'' and ParentID is not null  and EngineeringInfoID in ('" + engineeringInfoIDs.Replace(",", "','")
                    + "') and  StructInfoID in ('" + S_T_DefineParams.Params.GetValue("NavWBSType").Replace(",", "','") + "')";
                qb.PageSize = 0;
                qb.SortField = "SortIndex";
                qb.SortOrder = SortMode.Asc.ToString();
                qb.Items.Clear();
                var nodeDt = this.SqlHelper.ExecuteDataTable(sql, qb);
                var list = nodeDt.AsEnumerable();
                foreach (DataRow dr in dt.Rows)
                {
                    if (String.IsNullOrEmpty(showRoot) || showRoot.ToLower() != false.ToString().ToLower())
                    {
                        resultDt.ImportRow(dr);
                    }
                    var rows = list.Where(c => c["EngineeringInfoID"].ToString() == dr["EngineeringInfoID"].ToString()).ToList();
                    foreach (DataRow subNode in rows)
                    {
                        if (list.Count(c => c["ID"].ToString() == subNode["ParentID"].ToString()) == 0)
                        {
                            subNode["ParentID"] = dr["ID"].ToString();
                        }
                        subNode["Code"] = "";
                        resultDt.ImportRow(subNode);
                    }
                }
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    resultDt.ImportRow(dr);
                }
            }
            var data = new GridData(resultDt);
            data.total = totalCount;
            return Json(data);
        }

        public JsonResult GetMyList(QueryBuilder qb)
        {
            var engineeringMode = System.Configuration.ConfigurationManager.AppSettings["EngineeringMode"].ToLower();
            string prefix = "";
            string sql = @" 
select S_I_WBS.ID,isnull(S_I_WBS.FullName,S_I_WBS.Name) as Name,S_I_WBS.Code,S_I_UserDefaultEnter.ID as DateID,
S_I_WBS.EngineeringInfoID,
S_I_WBS.NodeType,
S_I_WBS.ChargerDeptName,S_I_WBS.ChargerUserName,S_I_WBS.CreateDate
from S_I_UserDefaultEnter left join S_I_WBS on S_I_UserDefaultEnter.WBSID=S_I_WBS.ID
where UserID='{0}'";
            qb.PageSize = 0;
            qb.SetSort("DateID", "desc");
            var data = this.SqlHelper.ExecuteGridData(String.Format(sql, this.CurrentUserInfo.UserID, prefix), qb);
            return Json(data);
        }

        public JsonResult GetListGroupbyState()
        {
            string sql = @"SELECT COUNT(*) AS num,State FROM (SELECT S_I_Engineering.*,'' as RootName,EngineeringInfoUserInfo.UserIDs 
as EngineeringUserIDs FROM S_I_Engineering 
left join (SELECT EngineeringInfoID,
[UserIDs]=STUFF((SELECT ','+UserID FROM S_I_OBS_User A 
WHERE A.EngineeringInfoID=S_I_OBS_User.EngineeringInfoID FOR XML PATH('')), 1, 1, '')
FROM S_I_OBS_User GROUP BY EngineeringInfoID) EngineeringInfoUserInfo
on S_I_Engineering.ID = EngineeringInfoUserInfo.EngineeringInfoID) EngineeringInfoTable where ModeCode !='' and ModeCode is not NULL
GROUP BY State";
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            var data = new GridData(dt);
            return Json(data);
        }

        public JsonResult GetListbyState(string state)
        {
            string sql = @"SELECT * FROM (SELECT S_I_Engineering.*,'' as RootName,EngineeringInfoUserInfo.UserIDs 
as EngineeringUserIDs FROM S_I_Engineering 
left join (SELECT EngineeringInfoID,
[UserIDs]=STUFF((SELECT ','+UserID FROM S_I_OBS_User A 
WHERE A.EngineeringInfoID=S_I_OBS_User.EngineeringInfoID FOR XML PATH('')), 1, 1, '')
FROM S_I_OBS_User GROUP BY EngineeringInfoID) EngineeringInfoUserInfo
on S_I_Engineering.ID = EngineeringInfoUserInfo.EngineeringInfoID) EngineeringInfoTable where ModeCode !='' and ModeCode is not NULL
and State='{0}'
";
            sql = string.Format(sql, state);
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            var data = new GridData(dt);
            return Json(data);
        }

        public JsonResult GetListGroupbyArea()
        {
            string sql = @"SELECT COUNT(*) AS num,Area FROM (SELECT S_I_Engineering.*,'' as RootName,EngineeringInfoUserInfo.UserIDs 
as EngineeringUserIDs FROM S_I_Engineering 
left join (SELECT EngineeringInfoID,
[UserIDs]=STUFF((SELECT ','+UserID FROM S_I_OBS_User A 
WHERE A.EngineeringInfoID=S_I_OBS_User.EngineeringInfoID FOR XML PATH('')), 1, 1, '')
FROM S_I_OBS_User GROUP BY EngineeringInfoID) EngineeringInfoUserInfo
on S_I_Engineering.ID = EngineeringInfoUserInfo.EngineeringInfoID) EngineeringInfoTable where ModeCode !='' and ModeCode is not NULL
GROUP BY Area";
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            var data = new GridData(dt);
            return Json(data);
        }

        public JsonResult GetListbyArea(string area)
        {
            string sql = @"SELECT * FROM (SELECT S_I_Engineering.*,'' as RootName,EngineeringInfoUserInfo.UserIDs 
as EngineeringUserIDs FROM S_I_Engineering 
left join (SELECT EngineeringInfoID,
[UserIDs]=STUFF((SELECT ','+UserID FROM S_I_OBS_User A 
WHERE A.EngineeringInfoID=S_I_OBS_User.EngineeringInfoID FOR XML PATH('')), 1, 1, '')
FROM S_I_OBS_User GROUP BY EngineeringInfoID) EngineeringInfoUserInfo
on S_I_Engineering.ID = EngineeringInfoUserInfo.EngineeringInfoID) EngineeringInfoTable where ModeCode !='' and ModeCode is not NULL
and Area='{0}'
";
            sql = string.Format(sql, area);
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            var data = new GridData(dt);
            return Json(data);
        }

        public ActionResult SubMenu()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            string wbsID = this.GetQueryString("WBSID");
            var wbs = this.GetEntityByID<S_I_WBS>(wbsID);
            if (wbs == null)
            {
            }
            else
            {
                if (wbs.NodeType == "DesignProject")
                    ViewBag.ProjectInfoID = wbs.ID;
                else
                {
                    var designNode = wbs.Ancestor.FirstOrDefault(c => c.NodeType == "DesignProject");
                    if (designNode != null)
                    {
                        ViewBag.ProjectInfoID = designNode.ID;
                    }
                    else
                    {
                        ViewBag.ProjectInfoID = "";
                    }
                }

                this.SetUserProjectList(this.CurrentUserInfo.UserID, wbs.ID, wbs.EngineeringInfoID);
                var spaceList = wbs.GetUserSpace(this.CurrentUserInfo.UserID);
                ViewBag.RootSpace = spaceList;
                var defaultRoot = spaceList.Where(d => d.IsDefault).OrderBy(c => c.RoleSortIndex).FirstOrDefault();
                if (defaultRoot == null) defaultRoot = spaceList.FirstOrDefault();
                if (defaultRoot == null)
                {
                    ViewBag.DefaultCode = "";
                    ViewBag.DefaultUrl = string.Empty;
                    ViewBag.DefaultSpaceDefineID = "";
                    ViewBag.DefaultName = "";
                }
                else
                {
                    ViewBag.DefaultCode = defaultRoot.Code;
                    ViewBag.DefaultUrl = defaultRoot.LinkUrl.Trim();
                    ViewBag.DefaultSpaceDefineID = defaultRoot.ID;
                    ViewBag.DefaultName = defaultRoot.Name;
                }
            }
            ViewBag.EngineeringJson = JsonHelper.ToJson(wbs.S_I_Engineering);
            ViewBag.WBSJson = JsonHelper.ToJson(wbs);
            return View();
        }

        public JsonResult GetSpaceMenu(string WBSID, string DefineID, string RelateID)
        {
            var wbs = this.GetEntityByID<S_I_WBS>(WBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息");
            var infrasContext = FormulaHelper.GetEntities<InfrastructureEntities>();
            var define = infrasContext.S_C_Meun.FirstOrDefault(c => c.ID == DefineID);
            if (define == null) throw new Formula.Exceptions.BusinessValidationException("您没有权限进入该空间");
            var list = wbs.GetUserSpaceMenu(this.CurrentUserInfo.UserID, DefineID, RelateID);
            var result = new Dictionary<string, object>();
            result.SetValue("data", list);
            result.SetValue("Expanded", define.Expanded);
            return Json(result);
        }

        public ActionResult MainContent()
        {
            var dbContext = FormulaHelper.GetEntities<EPC.Logic.Domain.EPCEntities>();
            string EngineeringInfoID = this.Request["EngineeringInfoID"];
            var engineering = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineering == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能获得指定工程信息");
            }
            ViewBag.EngineeringInfo = engineering;
            ViewBag.StartWorkingDate = engineering.CreateDate;

            //主要人员信息
            ViewBag.UserInfo = engineering.GetMainUserWithRolesTable();

            //工程公告数量
            var noticeInfo = this.entities.Set<S_I_Notice>().Where(d => d.EngineeringInfoID == EngineeringInfoID).ToList();
            var lastestNotice = noticeInfo.OrderByDescending(d => d.CreateDate).Take(5).ToList();
            ViewBag.LastestNotice = lastestNotice;
            ViewBag.NoticeNum = noticeInfo.Where(d => d.IsFromSys == "False").Count();
            ViewBag.SysNoticeNum = noticeInfo.Where(d => d.IsFromSys == "True").Count();
            ViewBag.EngineeringInfoDic = Formula.FormulaHelper.ModelToDic<S_I_Engineering>(engineering);
            return View();
        }

        public JsonResult GetWBSPath(string WBSID, QueryBuilder qb)
        {
            var navWBSType = EPC.Logic.Domain.S_T_DefineParams.Params.GetValue("NavWBSType");
            var wbs = this.GetEntityByID<S_I_WBS>(WBSID);
            if (wbs == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的WBS节点，无法进入空间");
            }
            var result = new Dictionary<string, object>();
            qb.PageSize = 0;
            qb.SortField = "SortIndex";
            qb.SortOrder = "Asc";
            string sql = @"select ID,ParentID,NodeType,Name,Code,WBSAuthUser,SortIndex from S_I_WBS where EngineeringInfoID='{0}' 
and len(FullID)<={2} and StructInfoID in ('{1}')";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, wbs.EngineeringInfoID, navWBSType.Replace(",", "','"), wbs.FullID.Length), qb);

            var ancestors = wbs.S_I_Engineering.S_I_WBS.Where(d => wbs.FullID.StartsWith(d.FullID)).
                OrderBy(c => c.FullID).Select(c => new
                {
                    ID = c.ID,
                    Name = c.Name,
                    ParentID = c.ParentID,
                    NodeType = c.NodeType,
                    Value = c.Value,
                    StructInfoID = c.StructInfoID,
                    AuthUser = c.WBSAuthUser
                }).ToList();
            var list = new List<Dictionary<string, object>>();
            foreach (var item in ancestors)
            {
                if (!navWBSType.Contains(item.StructInfoID)) continue;
                var dic = new Dictionary<string, object>();
                dic.SetValue("ID", item.ID);
                dic.SetValue("Name", item.Name);
                dic.SetValue("ParentID", item.ParentID);
                dic.SetValue("NodeType", item.NodeType);
                dic.SetValue("Value", item.Value);
                dic.SetValue("AuthUser", item.AuthUser);
                dic.SetValue("HasAuth", "false");
                if (!String.IsNullOrEmpty(item.AuthUser) && item.AuthUser.Contains(this.CurrentUserInfo.UserID))
                {
                    dic.SetValue("HasAuth", "true");
                }
                list.Add(dic);
            }
            result.SetValue("pathData", list);
            result.SetValue("wbsData", dt);
            return Json(result);
        }

        public S_I_UserDefaultEnter GetDefaultEnter()
        {
            var defaultEnter = this.entities.Set<S_I_UserDefaultEnter>().Where(d => d.UserID == this.CurrentUserInfo.UserID).
                OrderByDescending(d => d.ID).FirstOrDefault();
            return defaultEnter;
        }

        public void SetUserProjectList(string userID, string wbsID, string engineeringInfoID)
        {
            //var wbs = this.GetEntityByID<S_I_WBS>(wbsID);
            //if (wbs == null)
            //    return;
            //var list = this.entities.Set<S_I_UserDefaultEnter>().Where(d => d.UserID == userID).OrderBy(d => d.ID).ToList();
            //if (list != null && list.Count > 0)
            //{
            //    var defaultP = list.Find(c => c.WBSID == wbsID);
            //    if (defaultP != null)
            //        this.entities.Set<S_I_UserDefaultEnter>().Remove(defaultP);
            //    else if (list.Count >= 10)
            //    {
            //        this.entities.Set<S_I_UserDefaultEnter>().Remove(list[0]);
            //    }
            //}

            //var defaultProject = new S_I_UserDefaultEnter();
            //defaultProject.ID = FormulaHelper.CreateGuid();
            //defaultProject.UserID = userID;
            //defaultProject.EngineeringInfoID = engineeringInfoID;
            //defaultProject.WBSID = wbsID;
            //this.entities.Set<S_I_UserDefaultEnter>().Add(defaultProject);
            //this.entities.SaveChanges();
        }

        private string GetFirstString(string stringToSub, int length)
        {
            Regex regex = new Regex(@"[\u4e00-\u9fa5]+", RegexOptions.Compiled);
            char[] stringChar = stringToSub.ToCharArray();
            StringBuilder sb = new StringBuilder();
            int nLength = 0;
            for (int i = 0; i < stringChar.Length; i++)
            {
                if (regex.IsMatch((stringChar[i]).ToString()))
                {
                    nLength += 2;
                }
                else
                {
                    nLength = nLength + 1;
                }

                if (nLength <= length)
                {
                    sb.Append(stringChar[i]);
                }
                else
                {
                    break;
                }
            }
            if (sb.ToString() != stringToSub)
            {
                sb.Append("...");
            }
            return sb.ToString();
        }
    }
}
