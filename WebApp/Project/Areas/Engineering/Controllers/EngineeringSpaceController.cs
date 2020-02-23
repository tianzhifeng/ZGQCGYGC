using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;
using Newtonsoft.Json;
using System.Configuration;
using Base.Logic.BusinessFacade.Portal;

namespace Project.Areas.Engineering.Controllers
{
    public class EngineeringSpaceController : ProjectController
    {
        #region 工程空间

        public ActionResult MainIndex()
        {
            var groupInfoID = this.GetQueryString("GroupInfoID");
            var IsOpenForm = this.GetQueryString("IsOpenForm");
            var projectInfoID = this.GetQueryString("ProjectInfoID");
            if (String.IsNullOrEmpty(IsOpenForm)) IsOpenForm = "False";
            var groupInfo = this.GetEntityByID<S_I_ProjectGroup>(groupInfoID);
            if (groupInfo == null)
            {
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
                if (projectInfo == null)
                {
                    #region 默认项目或工程
                    //判定如果默认项目只能是该人员所参与的项目，否则项目换人后，会造成员工进去后没有权限
                    //var projectIDs = this.entities.Set<S_W_OBSUser>().Where(d => d.UserID == this.CurrentUserInfo.UserID).Select(d => d.ProjectInfoID).Distinct().ToList();
                    //var defaultEnter = this.entities.Set<S_I_UserDefaultProjectInfo>().Where(d => d.UserID == this.CurrentUserInfo.UserID
                    //    && projectIDs.Contains(d.ProjectInfoID)).OrderByDescending(d => d.ID).FirstOrDefault();
                    var sql = "select top 1 * from S_I_UserDefaultProjectInfo with(nolock) where UserID='" + this.CurrentUserInfo.UserID + @"' and ProjectInfoID in (
                        select distinct ProjectInfoID from S_W_OBSUser where UserID='" + this.CurrentUserInfo.UserID + "') order by ID desc";
                    var defaultEnter = this.SqlHelper.ExecuteObject<S_I_UserDefaultProjectInfo>(sql);
                    ViewBag.DefaultEnterID = "";
                    ViewBag.DefaultEnterType = "";
                    ViewBag.DefaultTitle = "";
                    if (defaultEnter != null)
                    {
                        if (!String.IsNullOrEmpty(defaultEnter.ProjectInfoID))
                        {
                            projectInfo = this.GetEntityByID<S_I_ProjectInfo>(defaultEnter.ProjectInfoID);
                            if (projectInfo != null)
                            {
                                ViewBag.DefaultTitle = projectInfo.Name.Replace("\"", "\\\"");
                                ViewBag.DefaultEnterType = "Project";
                                ViewBag.DefaultEnterID = projectInfo.ID;
                                ViewBag.DefaultEngieeringID = projectInfo.GroupRootID;
                                ViewBag.DefaultEngieeringName = projectInfo.GroupRootInfo.Name.Replace("\"", "\\\"");
                                ViewBag.DefaultMarketEngineeringID = projectInfo.GroupInfo.RelateID;
                            }
                        }
                        else
                        {
                            var group = this.GetEntityByID<S_I_ProjectGroup>(defaultEnter.EngineeringInfoID);
                            if (group != null)
                            {
                                ViewBag.DefaultTitle = group.Name.Replace("\"", "\\\"");
                                ViewBag.DefaultEnterType = "Engineering";
                                ViewBag.DefaultEnterID = defaultEnter.EngineeringInfoID;
                                ViewBag.DefaultEngieeringID = group.ID;
                                ViewBag.DefaultEngieeringName = group.Name.Replace("\"", "\\\"");
                                ViewBag.DefaultMarketEngineeringID = group.RelateID;
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    ViewBag.DefaultTitle = projectInfo.Name.Replace("\"", "\\\"");
                    ViewBag.DefaultEnterType = "Project";
                    ViewBag.DefaultEnterID = projectInfo.ID;
                    ViewBag.DefaultEngieeringID = projectInfo.GroupRootID;
                    ViewBag.DefaultEngieeringName = projectInfo.GroupRootInfo.Name.Replace("\"", "\\\"");
                    ViewBag.DefaultMarketEngineeringID = projectInfo.GroupInfo.RelateID;
                }
            }
            else
            {
                if (groupInfo.Type == EngineeringSpaceType.Project.ToString())
                {
                    var projectInfo = this.entities.Set<S_I_ProjectInfo>().FirstOrDefault(d => d.GroupID == groupInfo.ID);
                    if (projectInfo != null)
                    {
                        ViewBag.DefaultTitle = projectInfo.Name.Replace("\"", "\\\"");
                        ViewBag.DefaultEnterType = "Project";
                        ViewBag.DefaultEnterID = projectInfo.ID;
                        ViewBag.DefaultEngieeringID = projectInfo.GroupRootID;
                        ViewBag.DefaultEngieeringName = projectInfo.GroupRootInfo.Name.Replace("\"", "\\\"");
                        ViewBag.DefaultMarketEngineeringID = projectInfo.GroupInfo.RelateID;
                    }
                }
                else
                {
                    ViewBag.DefaultTitle = groupInfo.Name.Replace("\"", "\\\"");
                    ViewBag.DefaultEnterType = "Engineering";
                    ViewBag.DefaultEnterID = groupInfo.ID;
                    ViewBag.DefaultEngieeringID = groupInfo.ID;
                    ViewBag.DefaultEngieeringName = groupInfo.Name.Replace("\"", "\\\"");
                    ViewBag.DefaultMarketEngineeringID = groupInfo.RelateID;
                }
            }
            ViewBag.IsOpenForm = IsOpenForm;
            ViewBag.IsUseNewPortal = PortalMain.IsUseNewPortal;
            ViewBag.IsEngineeringMode = ConfigurationManager.AppSettings["EngineeringMode"].ToLower();
            return View();
        }

        public ActionResult MainContent()
        {
            //var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            string GroupInfoID = this.Request["GroupInfoID"];
            var group = this.GetEntityByID<S_I_ProjectGroup>(GroupInfoID);
            if (group == null)
            {
                throw new Formula.Exceptions.BusinessException("未能获得指定工程信息");
            }
            //var engineeringInfo = marketEntities.S_I_Engineering.Find(group.RelateID);
            //if (engineeringInfo == null)
            //{
            //    throw new Formula.Exceptions.BusinessException("未能获得指定工程信息");
            //}
            //ViewBag.EngineeringInfo = engineeringInfo;
            ViewBag.ProjectGroupInfo = group;

            var firstTaskNotice = this.entities.Set<T_CP_TaskNotice>().FirstOrDefault(d => d.GroupID == GroupInfoID);
            DateTime? startWorkingDate = null;
            if (firstTaskNotice != null)
                startWorkingDate = firstTaskNotice.CreateDate;
            ViewBag.StartWorkingDate = startWorkingDate;

            //项目列表信息
            var projectList = group.ProjectInfoList;
            ViewBag.ProjectList = projectList;
            var projectPrintCount = new Dictionary<string, int>();
            foreach (var project in projectList)
            {
                var printCount = this.entities.Set<S_E_Product>().Where(d => d.ProjectInfoID == project.ID).Select(d => d.PrintCount).Sum();
                projectPrintCount.Add(project.ID, printCount.HasValue ? printCount.Value : 0);
            }
            ViewBag.ProjectPrintCount = projectPrintCount;

            //主要人员信息
            ViewBag.UserInfo = group.GetMainUserWithRolesTable();

            //工程公告数量
            var sql = string.Format(
                "select * from S_N_Notice where GroupInfoID='{0}'", GroupInfoID);
            var noticeInfo = SqlHelper.ExecuteList<S_N_Notice>(sql).ToList();
            //var noticeInfo = this.entities.Set<S_N_Notice>().Where(d => d.GroupInfoID == GroupInfoID).ToList();
            var lastestNotice = noticeInfo.Where(a => a.NoticeType == "Project" || a.IsFromSys == "False").OrderByDescending(d => d.CreateDate).Take(5).ToList();
            ViewBag.LastestNotice = lastestNotice;
            ViewBag.NoticeNum = noticeInfo.Where(d => d.IsFromSys == "False").Count();
            ViewBag.SysNoticeNum = noticeInfo.Where(d => d.IsFromSys == "True").Count();

            //最新工程资料
            var dataList = this.entities.Set<S_D_DataCollection>().Where(d => d.GroupInfoID == GroupInfoID).OrderByDescending(d => d.ID).Take(5).ToList();
            ViewBag.DataList = dataList;

            //var prjList = this.entities.Set<S_I_ProjectInfo>().Where(d => d.GroupRootID == GroupInfoID).ToList();
            //最新校审完成成果信息
            List<string> prjIDs = new List<string>();
            var productList = new List<S_E_Product>();
            if (projectList.Count != 0)
            {
                prjIDs = projectList.Select(d => d.ID).ToList();
                string auditPass = AuditState.Pass.ToString();
                productList = this.entities.Set<S_E_Product>().Where(d => d.AuditState == auditPass && prjIDs.Contains(d.ProjectInfoID)).OrderByDescending(d => d.ID).Take(5).ToList();
            }
            ViewBag.ProductList = productList;

            return View();
        }

        public JsonResult GetSliderResTree()
        {
            string groupID = this.Request["GroupInfoID"];
            ProjectGroupFO fo = new ProjectGroupFO();
            var resList = fo.GetSpaceDefine(this.CurrentUserInfo.UserID, groupID);
            resList.RemoveWhere(d => d.Type == "Root");
            return Json(resList);
        }

        public ActionResult Slider()
        {
            string GroupInfoID = this.GetQueryString("GroupInfoID");
            var group = this.GetEntityByID<S_I_ProjectGroup>(GroupInfoID);
            if (group == null) throw new Formula.Exceptions.BusinessException("未能找到指定的工程信息");

            Formula.FormulaHelper.CreateFO<Project.Logic.ProjectGroupFO>().SetUserProjecGrouptList(this.CurrentUserInfo.UserID, GroupInfoID);
            return View();
        }

        public ActionResult SliderProject()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectFO = Formula.FormulaHelper.CreateFO<Project.Logic.ProjectInfoFO>();
            if (String.IsNullOrEmpty(projectInfoID))
                projectInfoID = projectFO.GetDefaultProjectID(this.CurrentUserInfo.UserID);
            else
                projectFO.SetUserProjectList(this.CurrentUserInfo.UserID, projectInfoID);
            this.ViewBag.CurrentUserID = this.CurrentUserInfo.UserID;
            this.ViewBag.DefaultCode = "";
            this.ViewBag.DefaultSpaceDefineID = "";
            this.ViewBag.ShowSpace = "False";
            this.ViewBag.DefaultUrl = "";
            if (String.IsNullOrEmpty(projectInfoID))
            {
                var entraceList = new List<Entrance>();
                this.ViewBag.Entrace = entraceList;
                this.ViewBag.DisplayCount = 0;
                this.ViewBag.ProjectInfoID = projectInfoID;
                this.ViewBag.ProjectJson = "";
                this.ViewBag.NeedMenu = false;
                this.ViewBag.ShowSpace = false.ToString();
                this.ViewBag.ProjectJson = "{}";
            }
            else
            {
                string userID = this.ViewBag.CurrentUserID;
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
                var entraceList = projectFO.GetEntrance(userID, projectInfoID);
                //entraceList.Sort((a, b) =>
                //{
                //    if (a.SpaceDefineCode == "Project")
                //        return -1;
                //    else if (b.SpaceDefineCode == "Project")
                //        return 1;
                //    else
                //        return a.SortIndex.CompareTo(b.SortIndex);
                //});
                foreach (var item in entraceList)
                {
                    if (item.IsDefault)
                    {
                        this.ViewBag.DefaultCode = item.Code;
                        this.ViewBag.DefaultSpaceDefineID = item.SpaceDefineID;
                        this.ViewBag.DefaultUrl = item.LinkUrl;
                        this.ViewBag.DefaultItem = item;
                        this.ViewBag.DefaultName = item.Name;
                    }
                }
                this.ViewBag.ProjectInfoID = projectInfo.ID;
                this.ViewBag.Entrace = entraceList;
                this.ViewBag.ProjectJson = JsonHelper.ToJson(projectInfo);
            }
            var userProjectTable = projectFO.GetUserProjectList(this.CurrentUserInfo.UserID);
            this.ViewBag.RelationProject = JsonHelper.ToJson(userProjectTable);
            return View();
        }

        public JsonResult GetSpaceDefine(string DefineID, string SpaceCode, string ProjectInfoID)
        {
            var projectFO = FormulaHelper.CreateFO<ProjectInfoFO>();
            var result = projectFO.GetSpaceDefine(this.CurrentUserInfo.UserID, ProjectInfoID, DefineID, SpaceCode);
            return Json(result);
        }

        public ActionResult MainPage()
        {
            //var marketEntities = FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            string groupID = this.Request["GroupInfoID"];
            var group = this.GetEntityByID<S_I_ProjectGroup>(groupID);
            if (group == null)
            {
                throw new Formula.Exceptions.BusinessException("未能获得指定工程信息");
            }
            //var engineeringInfo = marketEntities.S_I_Engineering.Find(group.RelateID);
            //if (engineeringInfo == null)
            //{
            //    throw new Formula.Exceptions.BusinessException("未能获得指定工程信息");
            //}
            //ViewBag.EngineeringInfo = engineeringInfo;
            ViewBag.ProjectGroupInfo = group;
            ViewBag.ProjectList = group.ProjectInfoList;
            ViewBag.UserInfo = group.GetUserTable();
            ViewBag.Information = new List<Dictionary<string, object>>();
            return View();
        }

        public JsonResult GetTreeList(QueryBuilder qb)
        {
            string sql = @"SELECT * FROM (SELECT S_I_ProjectGroup.*,'' as RootName,EngineeringInfoUserInfo.UserIDs as EngineeringUserIDs FROM S_I_ProjectGroup 
left join (SELECT EngineeringInfoID,
[UserIDs]=STUFF((SELECT ','+UserID FROM V_I_EngineeringUserInfo A 
WHERE A.EngineeringInfoID=V_I_EngineeringUserInfo.EngineeringInfoID FOR XML PATH('')), 1, 1, '')
FROM V_I_EngineeringUserInfo
GROUP BY EngineeringInfoID) EngineeringInfoUserInfo
on S_I_ProjectGroup.RelateID = EngineeringInfoUserInfo.EngineeringInfoID
WHERE TYPE='Engineering' ) EngineeringInfoTable ";
            var dt = this.SqlHelper.ExecuteDataTable(sql, qb);
            string fullIDs = "";
            foreach (DataRow row in dt.Rows)
            {
                fullIDs += " FULLID LIKE '" + row["FullID"].ToString() + "%' or";
            }
            string childrenSql = @"SELECT * FROM (select RootName, S_I_ProjectInfo.ID as ProjectInfoID,S_I_ProjectGroup.* from S_I_ProjectGroup
left join S_I_ProjectInfo on S_I_ProjectInfo.GroupID=S_I_ProjectGroup.ID
left join (select ID,Name as RootName from S_I_ProjectGroup where ParentID is null or ParentID='') RootTable on S_I_ProjectGroup.RootID=RootTable.ID
WHERE TYPE in ('UnderEngineering','Project')  ) CHILDTALBEINFO ";
            if (!String.IsNullOrEmpty(fullIDs)) childrenSql += " WHERE " + fullIDs.TrimEnd(new char[] { 'r', 'o' });
            else childrenSql += " WHERE 1!=1";
            var children = this.SqlHelper.ExecuteDataTable(childrenSql);
            var list = FormulaHelper.DataTableToListDic(dt);
            list.AddRange(FormulaHelper.DataTableToListDic(children));
            var data = new GridData(list);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public JsonResult GetMyList(QueryBuilder qb)
        {
            var engineeringMode = System.Configuration.ConfigurationManager.AppSettings["EngineeringMode"].ToLower();
            string prefix = "";
            string sql = " select top 10 * from ( ";
            string projectSql = @" 
select S_I_UserDefaultProjectInfo.ID as DateID,UserID,RootName, S_I_ProjectInfo.ID as ProjectInfoID, 
S_I_ProjectGroup.ID,'{1}'+S_I_ProjectGroup.Name as Name,S_I_ProjectGroup.Code,S_I_ProjectGroup.ProjectClass,
S_I_ProjectGroup.RelateID,S_I_ProjectGroup.[Type],S_I_ProjectGroup.ParentID,S_I_ProjectGroup.PhaseContent,
S_I_ProjectGroup.RootID,S_I_ProjectGroup.ChargeUserName,S_I_ProjectGroup.DeptName,S_I_ProjectGroup.CreateDate
from S_I_UserDefaultProjectInfo with(nolock) 
left join S_I_ProjectInfo on S_I_UserDefaultProjectInfo.ProjectInfoID=S_I_ProjectInfo.ID
left join S_I_ProjectGroup on S_I_ProjectGroup.ID=S_I_ProjectInfo.GroupID
left join (select ID,Name as RootName from S_I_ProjectGroup where ParentID is null or ParentID='') RootTable on S_I_ProjectGroup.RootID=RootTable.ID
where ProjectInfoID!='' and ProjectInfoID is not null) TableInfo
where UserID = '{0}'";

            if (engineeringMode == "true")
            {
                string engineeringSql = @" select S_I_UserDefaultProjectInfo.ID as DateID,UserID,'' as RootName,'' as ProjectInfoID, 
S_I_ProjectGroup.ID,'[工程]'+S_I_ProjectGroup.Name as Name,S_I_ProjectGroup.Code,S_I_ProjectGroup.ProjectClass,
S_I_ProjectGroup.RelateID,S_I_ProjectGroup.[Type],S_I_ProjectGroup.ParentID,S_I_ProjectGroup.PhaseContent,
S_I_ProjectGroup.RootID,S_I_ProjectGroup.ChargeUserName,S_I_ProjectGroup.DeptName,S_I_ProjectGroup.CreateDate
 from S_I_UserDefaultProjectInfo with(nolock) 
left join S_I_ProjectGroup on S_I_UserDefaultProjectInfo.EngineeringInfoID=S_I_ProjectGroup.ID
where ProjectInfoID='' or ProjectInfoID is null
union 
";
                sql += engineeringSql + projectSql;
                prefix = "[项目]";
            }
            else
                sql += projectSql;

            qb.PageSize = 0;
            qb.SetSort("DateID", "desc");
            var data = this.SqlHelper.ExecuteGridData(String.Format(sql, this.CurrentUserInfo.UserID, prefix), qb);
            return Json(data);
        }

        public JsonResult GetProjectList(QueryBuilder qb)
        {
            string sql = @"select RootName,ProjectInfo.ID as ProjectInfoID, S_I_ProjectGroup.*,[UserIDs] from (
SELECT ID,GroupID,
[UserIDs]=STUFF((SELECT ','+UserID FROM V_I_ProjectUserInfo A WHERE A.ID=V_I_ProjectUserInfo.ID FOR XML PATH('')), 1, 1, '')
FROM V_I_ProjectUserInfo 
GROUP BY ID,GroupID) ProjectInfo
left join S_I_ProjectGroup
on S_I_ProjectGroup.ID= ProjectInfo.GroupID
left join (select ID,Name as RootName from S_I_ProjectGroup where ParentID is null or ParentID='') RootTable 
on S_I_ProjectGroup.RootID=RootTable.ID";
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetProjectIndex(string ProjectInfoID)
        {
            var data = "MainContentProject";
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("找不到指定的项目。");
            var spaceDefine = projectInfo.ProjectMode.S_T_SpaceDefine.FirstOrDefault(a => a.Code == "Project" && String.IsNullOrEmpty(a.ParentID));
            if (spaceDefine != null)
                data = String.IsNullOrEmpty(spaceDefine.LinkUrl) ? "MainContentProject" : spaceDefine.LinkUrl;
            return Json(new { index = data });
        }

        #endregion

        #region 项目空间

        public ActionResult MainContentProject()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var dt = this.SqlHelper.ExecuteDataTable(string.Format("select * from S_I_ProjectInfo where ID='{0}'", projectInfoID));
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null || dt.Rows.Count == 0)
            {
                throw new Formula.Exceptions.BusinessException("未能获得指定项目信息");
            }
            var businessType = EnumBaseHelper.GetEnumDef("Base.ProjectClass").EnumItem.ToList().FirstOrDefault(a => a.Code == projectInfo.ProjectClass);
            if (businessType != null)
                projectInfo.ProjectClass = businessType.Name;
            ViewBag.ProjectInfo = projectInfo;
            var marketProjectInfoID = projectInfo.MarketProjectInfoID;

            #region QBSTree
            var baseConfig = FormulaHelper.GetEntities<BaseConfigEntities>();
            var catagoryType = QBSNodeType.Catagory.ToString();
            var qbsType = QBSNodeType.QBS.ToString();
            var QBSCatagoryList = projectInfo.ProjectMode.S_T_QBSTemplate.Where(d => d.NodeType == catagoryType).OrderBy(d => d.SortIndex).ToList();
            var QBSList = projectInfo.ProjectMode.S_T_QBSTemplate.Where(d => d.NodeType == qbsType).OrderBy(d => d.SortIndex).ToList();

            var uiFO = new Base.Logic.BusinessFacade.UIFO();
            List<QBSTemplateInfo> QBSResList = new List<QBSTemplateInfo>();
            var qualitative = QBSType.Qualitative.ToString();
            var quantify = QBSType.Quantify.ToString();
            foreach (var item in QBSList)
            {
                var QBSInfo = new QBSTemplateInfo();
                this.UpdateEntity(QBSInfo, item.ToDic());
                QBSResList.Add(QBSInfo);

                var sql = item.SQL;
                if (string.IsNullOrEmpty(sql))
                    continue;
                var resSQL = uiFO.ReplaceString(sql, dt.Rows[0]);
                var connDt = SQLHelper.CreateSqlHelper(item.ConnName);
                var resDt = connDt.ExecuteDataTable(resSQL);

                //定性
                if (item.QBSType == qualitative)
                {
                    if (resDt.Columns.Contains(CommonConst.QualitativeState) && resDt.Rows.Count != 0 && !string.IsNullOrEmpty(resDt.Rows[0][CommonConst.QualitativeState].ToString()))
                    {
                        QBSInfo.ResState = DateTime.Parse(resDt.Rows[0][CommonConst.QualitativeState].ToString()).ToString("yyyy-MM-dd");
                        QBSInfo.LinkUrl = uiFO.ReplaceString(item.LinkUrl, resDt.Rows[0]);
                    }
                    else
                    {
                        QBSInfo.ResState = "";
                        QBSInfo.LinkUrl = uiFO.ReplaceString(item.LinkUrl);
                    }
                }
                //定量
                else
                {
                    if (resDt.Columns.Contains(CommonConst.QuantifyNum) && resDt.Rows.Count != 0)
                    {
                        QBSInfo.ResState = resDt.Rows[0][CommonConst.QuantifyNum].ToString();
                        QBSInfo.LinkUrl = uiFO.ReplaceString(item.LinkUrl, resDt.Rows[0]);
                    }
                    else
                    {
                        QBSInfo.ResState = "0";
                        QBSInfo.LinkUrl = uiFO.ReplaceString(item.LinkUrl);
                    }
                }
            }
            ViewBag.QBSCatagoryList = QBSCatagoryList;
            ViewBag.QBSList = QBSResList;
            #endregion

            #region 项目专业信息
            var projectType = Project.Logic.WBSNodeType.Project.ToString();
            var majorType = Project.Logic.WBSNodeType.Major.ToString();
            var wbsInfo = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID).OrderBy(a => a.Level).ThenBy(a => a.SortIndex).ToList();
            var majorWBSList = wbsInfo.Where(d => d.WBSType == projectType || d.WBSType == majorType).Select(d => new
            {
                Name = d.Name,
                ProjectInfoID = d.ProjectInfoID,
                WBSType = d.WBSType,
                WBSValue = d.WBSValue
            }).Distinct().ToList();
            var obsUsers = this.entities.Set<S_W_OBSUser>().Where(d => d.ProjectInfoID == projectInfoID).ToList();
            var OBSUserList = new List<OBSUserInfo>();
            foreach (var obsUser in obsUsers)
            {
                var obsInfo = new OBSUserInfo();
                this.UpdateEntity(obsInfo, obsUser.ToDic());
                var userInfo = FormulaHelper.GetUserInfoByID(obsUser.UserID);
                if (userInfo != null)
                {
                    obsInfo.TEL = userInfo.MobilePhone;

                    OBSUserList.Add(obsInfo);
                }
                #region 角色排序
                switch (obsUser.RoleCode)
                {
                    default: obsInfo.SortIndex = 900; break;
                    case "MajorPrinciple":
                        obsInfo.SortIndex = 100;
                        break;
                    case "Designer":
                        obsInfo.SortIndex = 200;
                        break;
                    case "Collactor":
                        obsInfo.SortIndex = 300;
                        break;
                    case "Auditor":
                        obsInfo.SortIndex = 400;
                        break;
                    case "Approver":
                        obsInfo.SortIndex = 400;
                        break;
                    case "ProjectManager":
                        obsInfo.SortIndex = 10;
                        break;
                    case "DesignManager":
                        obsInfo.SortIndex = 20;
                        break;
                }
                #endregion
            }
            var productList = this.entities.Set<S_E_Product>().Where(d => d.ProjectInfoID == projectInfoID).ToList();

            var workHourList = SQLHelper.CreateSqlHelper(ConnEnum.Comprehensive).ExecuteList<WorkHourViewList>(string.Format("select * from S_W_UserWorkHour where ProjectID='{0}'", projectInfoID));
            ViewBag.WBSInfo = wbsInfo;
            ViewBag.ObsUserInfo = OBSUserList;
            ViewBag.ProductList = productList;
            ViewBag.WorkHourList = workHourList;
            //var costList = FormulaHelper.GetEntities<MarketEntities>().Set<S_FC_CostInfo>().Where(a => a.ProjectID == marketProjectInfoID && a.ProjectType == Market.Logic.Const.defaultDirectCostType).ToList();
            //var directCostValue = costList.Where(a => a.CostType == Market.Logic.Const.defaultDirectCostType).Select(a => a.CostValue).Sum();
            //var paymentCostValue = costList.Where(a => a.CostType != Market.Logic.Const.defaultDirectCostType).Select(a => a.CostValue).Sum();
            var totalCostValue = 0m;// costList.Select(a => a.CostValue).Sum();
            ViewBag.DirectValue = 0m; //string.Format("{0:N2}", directCostValue / 10000);
            ViewBag.PaymentCostValue = 0m;// string.Format("{0:N2}", paymentCostValue / 10000);
            ViewBag.TotalCostValue = 0m;// string.Format("{0:N2}", totalCostValue / 10000);

            var productNumColumnOptionList = new Dictionary<string, object>();
            var printCountColumnOptionList = new Dictionary<string, object>();
            foreach (var wbs in majorWBSList)
            {
                var majorValue = "";
                if (wbs.WBSType != projectType)
                    majorValue = wbs.WBSValue;

                productNumColumnOptionList.Add("productNum_" + wbs.WBSValue, JsonHelper.ToJson(this.GetColumnChartOption(wbs.ProjectInfoID, "提交量", "ProductNum", majorValue)));
                printCountColumnOptionList.Add("printCount_" + wbs.WBSValue, JsonHelper.ToJson(this.GetColumnChartOption(wbs.ProjectInfoID, "出图量", "PrintCount", majorValue)));
            }
            ViewBag.ProductNumColumnOption = JsonHelper.ToJson(productNumColumnOptionList);
            ViewBag.PrintCountColumnOption = JsonHelper.ToJson(printCountColumnOptionList);

            #endregion

            #region 里程碑信息

            var milestoneList = this.entities.Set<S_P_MileStone>().Where(d => d.ProjectInfoID == projectInfoID).ToList();
            var prjBaseHelper = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            var majorDt = prjBaseHelper.ExecuteDataTable("select Name,Code from S_D_WBSAttrDefine where Type='Major'");
            var majorDic = new Dictionary<string, string>();
            foreach (DataRow item in majorDt.Rows)
            {
                majorDic.Add(item["Code"].ToString(), item["Name"].ToString());
            }
            ViewBag.MileStoneList = milestoneList;
            ViewBag.MajorDic = majorDic;
            var projectList = projectInfo.S_W_WBS.Where(a => a.WBSType == WBSNodeType.Project.ToString()).ToList();
            var phaseList = projectInfo.S_W_WBS.Where(a => a.WBSType == WBSNodeType.Phase.ToString()).ToList();
            if (phaseList.Count == 0)
            {
                var subProjectList = projectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.SubProject.ToString()).ToList();
                if (subProjectList.Count == 0)
                {
                    ViewBag.MileStoneWBSList = projectList;
                }
                else
                {
                    ViewBag.MileStoneWBSList = subProjectList;
                }
            }
            else
            {
                phaseList.ForEach(a => projectList.Add(a));
                ViewBag.MileStoneWBSList = projectList;
            }
            //var subProjectList = projectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.SubProject.ToString()).ToList();
            //if ((subProjectList.Count == 0 && !projectInfo.WBSRoot.StructNodeInfo.ChildCode.Contains(WBSNodeType.SubProject.ToString()))
            //    || milestoneList.Count == milestoneList.Count(a=>a.WBSID==projectInfo.RootWBSID))//兼容所有里程碑都挂在root节点上的情况
            //{
            //    var list = new List<S_W_WBS>();
            //    list.Add(projectInfo.WBSRoot);
            //    ViewBag.MileStoneWBSList = list;
            //}
            //else
            //{
            //    ViewBag.MileStoneWBSList = subProjectList;
            //}

            #endregion

            return View();
        }


        public ActionResult MainContentElePower()
        {
            string userName = this.GetQueryString("SystemName");
            if (!string.IsNullOrEmpty(userName))
            {
                userName = HttpUtility.UrlDecode(userName);
                string pwd = this.GetQueryString("PWD");
                string sql = "select count(0) from S_A_User where Code ='" + userName + "'";
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                if (Convert.ToInt32(db.ExecuteScalar(sql)) > 0 && !String.IsNullOrEmpty(userName))
                {
                    FormulaHelper.ContextSet("AgentUserLoginName", userName);
                    FormulaHelper.SetAuthCookie(userName);
                }
            }
            string projectInfoID = this.Request["ProjectInfoID"];
            var dt = this.SqlHelper.ExecuteDataTable(string.Format("select * from S_I_ProjectInfo where ID='{0}'", projectInfoID));
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null || dt.Rows.Count == 0)
            {
                throw new Formula.Exceptions.BusinessException("未能获得指定项目信息");
            }
            ViewBag.ProjectInfo = projectInfo;
            var marketProjectInfoID = projectInfo.MarketProjectInfoID;

            #region QBSTree
            var baseConfig = FormulaHelper.GetEntities<BaseConfigEntities>();
            var catagoryType = QBSNodeType.Catagory.ToString();
            var qbsType = QBSNodeType.QBS.ToString();
            var QBSCatagoryList = projectInfo.ProjectMode.S_T_QBSTemplate.Where(d => d.NodeType == catagoryType).OrderBy(d => d.SortIndex).ToList();
            var QBSList = projectInfo.ProjectMode.S_T_QBSTemplate.Where(d => d.NodeType == qbsType).OrderBy(d => d.SortIndex).ToList();

            var uiFO = new Base.Logic.BusinessFacade.UIFO();
            List<QBSTemplateInfo> QBSResList = new List<QBSTemplateInfo>();
            var qualitative = QBSType.Qualitative.ToString();
            var quantify = QBSType.Quantify.ToString();
            foreach (var item in QBSList)
            {
                var QBSInfo = new QBSTemplateInfo();
                this.UpdateEntity(QBSInfo, item.ToDic());
                QBSResList.Add(QBSInfo);

                var sql = item.SQL;
                if (string.IsNullOrEmpty(sql))
                    continue;
                var resSQL = uiFO.ReplaceString(sql, dt.Rows[0]);
                var connDt = SQLHelper.CreateSqlHelper(item.ConnName);
                var resDt = connDt.ExecuteDataTable(resSQL);

                //定性
                if (item.QBSType == qualitative)
                {
                    if (resDt.Columns.Contains(CommonConst.QualitativeState) && resDt.Rows.Count != 0 && !string.IsNullOrEmpty(resDt.Rows[0][CommonConst.QualitativeState].ToString()))
                    {
                        QBSInfo.ResState = DateTime.Parse(resDt.Rows[0][CommonConst.QualitativeState].ToString()).ToString("yyyy-MM-dd");
                        QBSInfo.LinkUrl = uiFO.ReplaceString(item.LinkUrl, resDt.Rows[0]);
                    }
                    else
                    {
                        QBSInfo.ResState = "";
                        QBSInfo.LinkUrl = uiFO.ReplaceString(item.LinkUrl);
                    }
                }
                //定量
                else
                {
                    if (resDt.Columns.Contains(CommonConst.QuantifyNum) && resDt.Rows.Count != 0)
                    {
                        QBSInfo.ResState = resDt.Rows[0][CommonConst.QuantifyNum].ToString();
                        QBSInfo.LinkUrl = uiFO.ReplaceString(item.LinkUrl, resDt.Rows[0]);
                    }
                    else
                    {
                        QBSInfo.ResState = "0";
                        QBSInfo.LinkUrl = uiFO.ReplaceString(item.LinkUrl);
                    }
                }
            }
            ViewBag.QBSCatagoryList = QBSCatagoryList;
            ViewBag.QBSList = QBSResList;
            #endregion

            #region 项目专业信息
            var projectType = Project.Logic.WBSNodeType.Project.ToString();
            var majorType = Project.Logic.WBSNodeType.Major.ToString();
            var wbsInfo = this.entities.Set<S_W_WBS>().Where(d => d.ProjectInfoID == projectInfoID).OrderBy(a => a.Level).ThenBy(a => a.SortIndex).ToList();
            var majorWBSList = wbsInfo.Where(d => d.WBSType == projectType || d.WBSType == majorType).Select(d => new
            {
                Name = d.Name,
                ProjectInfoID = d.ProjectInfoID,
                WBSType = d.WBSType,
                WBSValue = d.WBSValue
            }).Distinct().ToList();
            var obsUsers = this.entities.Set<S_W_OBSUser>().Where(d => d.ProjectInfoID == projectInfoID).ToList();
            var OBSUserList = new List<OBSUserInfo>();
            foreach (var obsUser in obsUsers)
            {
                var obsInfo = new OBSUserInfo();
                this.UpdateEntity(obsInfo, obsUser.ToDic());
                var userInfo = FormulaHelper.GetUserInfoByID(obsUser.UserID);
                if (userInfo != null)
                {
                    obsInfo.TEL = userInfo.MobilePhone;

                    OBSUserList.Add(obsInfo);
                }
                #region 角色排序
                switch (obsUser.RoleCode)
                {
                    default: obsInfo.SortIndex = 900; break;
                    case "MajorPrinciple":
                        obsInfo.SortIndex = 100;
                        break;
                    case "Designer":
                        obsInfo.SortIndex = 200;
                        break;
                    case "Collactor":
                        obsInfo.SortIndex = 300;
                        break;
                    case "Auditor":
                        obsInfo.SortIndex = 400;
                        break;
                    case "Approver":
                        obsInfo.SortIndex = 400;
                        break;
                    case "ProjectManager":
                        obsInfo.SortIndex = 10;
                        break;
                    case "DesignManager":
                        obsInfo.SortIndex = 20;
                        break;
                }
                #endregion
            }
            var productList = this.entities.Set<S_E_Product>().Where(d => d.ProjectInfoID == projectInfoID).ToList();

            var workHourList = SQLHelper.CreateSqlHelper(ConnEnum.HR).ExecuteList<WorkHourViewList>(string.Format("select * from S_W_UserWorkHour where ProjectID='{0}'", projectInfoID));
            ViewBag.WBSInfo = wbsInfo;
            ViewBag.ObsUserInfo = OBSUserList;
            ViewBag.ProductList = productList;
            ViewBag.WorkHourList = workHourList;

            var taskWorks = this.entities.Set<S_W_TaskWork>().Where(a => a.ProjectInfoID == projectInfoID).ToList();
            ViewBag.taskWorks = taskWorks;

            //var costList = FormulaHelper.GetEntities<MarketEntities>().Set<S_FC_CostInfo>().Where(a => a.ProjectID == marketProjectInfoID && a.ProjectType == Market.Logic.Const.defaultDirectCostType).ToList();
            //var directCostValue = costList.Where(a => a.CostType == Market.Logic.Const.defaultDirectCostType).Select(a => a.CostValue).Sum();
            //var paymentCostValue = costList.Where(a => a.CostType != Market.Logic.Const.defaultDirectCostType).Select(a => a.CostValue).Sum();
            //var totalCostValue = costList.Select(a => a.CostValue).Sum();
            ViewBag.DirectValue = string.Format("{0:N2}", 0 / 10000);
            ViewBag.PaymentCostValue = string.Format("{0:N2}", 0 / 10000);
            ViewBag.TotalCostValue = string.Format("{0:N2}", 0 / 10000);

            var productNumColumnOptionList = new Dictionary<string, object>();
            var printCountColumnOptionList = new Dictionary<string, object>();
            foreach (var wbs in majorWBSList)
            {
                var majorValue = "";
                if (wbs.WBSType != projectType)
                    majorValue = wbs.WBSValue;

                productNumColumnOptionList.Add("productNum_" + wbs.WBSValue, JsonHelper.ToJson(this.GetColumnChartOption(wbs.ProjectInfoID, "提交量", "ProductNum", majorValue)));
                printCountColumnOptionList.Add("printCount_" + wbs.WBSValue, JsonHelper.ToJson(this.GetColumnChartOption(wbs.ProjectInfoID, "出图量", "PrintCount", majorValue)));
            }
            ViewBag.ProductNumColumnOption = JsonHelper.ToJson(productNumColumnOptionList);
            ViewBag.PrintCountColumnOption = JsonHelper.ToJson(printCountColumnOptionList);

            #endregion

            #region 里程碑信息

            var milestoneList = this.entities.Set<S_P_MileStone>().Where(d => d.ProjectInfoID == projectInfoID).ToList();
            var prjBaseHelper = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            var majorDt = prjBaseHelper.ExecuteDataTable("select Name,Code from S_D_WBSAttrDefine where Type='Major'");
            var majorDic = new Dictionary<string, string>();
            foreach (DataRow item in majorDt.Rows)
            {
                majorDic.Add(item["Code"].ToString(), item["Name"].ToString());
            }
            ViewBag.MileStoneList = milestoneList;
            ViewBag.MajorDic = majorDic;
            var subProjectList = projectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.SubProject.ToString()).ToList();
            if ((subProjectList.Count == 0 && !projectInfo.WBSRoot.StructNodeInfo.ChildCode.Contains(WBSNodeType.SubProject.ToString()))
                || milestoneList.Count == milestoneList.Count(a => a.WBSID == projectInfo.RootWBSID))//兼容所有里程碑都挂在root节点上的情况
            {
                var list = new List<S_W_WBS>();
                list.Add(projectInfo.WBSRoot);
                ViewBag.MileStoneWBSList = list;
            }
            else
            {
                ViewBag.MileStoneWBSList = subProjectList;
            }

            #endregion
            var noticeSql = string.Format("select * from S_N_Notice ");
            var noticeInfo = SqlHelper.ExecuteList<S_N_Notice>(noticeSql).ToList();
            //var noticeInfo = this.entities.Set<S_N_Notice>().Where(d => d.GroupInfoID == GroupInfoID).ToList();
            var lastestNotice = noticeInfo.Where(a => a.NoticeType == "Project" || a.IsFromSys == "False").OrderByDescending(d => d.CreateDate).Take(5).ToList();
            ViewBag.LastestNotice = lastestNotice;
            ViewBag.NoticeNum = noticeInfo.Where(d => d.IsFromSys == "False").Count();
            ViewBag.SysNoticeNum = noticeInfo.Where(d => d.IsFromSys == "True").Count();
            return View();
        }

        #region 柱状图
        private Dictionary<string, object> GetColumnChartOption(string projectInfoID, string series, string serieFields, string majorValue = "")
        {
            DataTable resDT = new DataTable();
            //var startDate = DateTime.Now.AddMonths(-5);
            for (var i = 0; i < 6; i++)
            {
                //var curYear = startDate.Year;
                //var curMonth = startDate.Month;
                var sql = @"select ProjectInfoID,(case when DateDiff(ww,CreateDate,getDate())=0 then '本周'
	when DateDiff(ww,CreateDate,getDate())=1 then '上周' 
	else '前'+cast(DateDiff(ww,CreateDate,getDate())+1 as nvarchar(20))+'周' end) as BelongWeek,
	count(0) as ProductNum,sum(isnull(PrintCount,0)) as PrintCount
from S_E_Product
where DateDiff(ww,CreateDate,getDate())={1}
	and ProjectInfoID='{0}' {2}
Group by DateDiff(ww,CreateDate,getDate()),ProjectInfoID";

                var majorValueSQL = "";
                if (!string.IsNullOrEmpty(majorValue))
                    majorValueSQL = " and MajorValue='" + majorValue + "'";
                sql = string.Format(sql, projectInfoID, i, majorValueSQL);
                var dt = this.SqlHelper.ExecuteDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    var curWeek = "";
                    switch (i)
                    {
                        case 0:
                            curWeek = "本周";
                            break;
                        case 1:
                            curWeek = "上周";
                            break;
                        default:
                            curWeek = "前" + (i + 1).ToString() + "周";
                            break;
                    }
                    dt.Rows.Add(projectInfoID, curWeek, 0, 0);
                }
                resDT.Merge(dt);
                //startDate = startDate.AddMonths(1);
            }

            var columChart = HighChartHelper.CreateColumnChart("", resDT, "BelongWeek", series.Split(','), serieFields.Split(','));
            columChart.Is3D = false;
            columChart.TitleInfo.Text = "";
            return columChart.Render();
        }

        #endregion

        #region 自定义类
        public class QBSTemplateInfo : S_T_QBSTemplate
        {
            public string ResState { set; get; }
        }
        public class OBSUserInfo : S_W_OBSUser
        {
            public string TEL { set; get; }
            public int SortIndex { get; set; }
        }
        #endregion


        #endregion
    }

    public class WorkHourViewList
    {
        public string MajorCode { get; set; }
        public decimal? WorkHourValue { get; set; }
    }
}
