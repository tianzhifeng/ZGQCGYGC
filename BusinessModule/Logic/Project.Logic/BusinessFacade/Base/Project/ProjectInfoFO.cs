using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Entity;
using System.Collections;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Config;
using Config.Logic;
using Project.Logic.Domain;

namespace Project.Logic
{
    /// <summary>
    /// 项目对象FO服务类
    /// </summary>
    public class ProjectInfoFO
    {
        ProjectEntities instanceEnitites = FormulaHelper.GetEntities<ProjectEntities>();
        BaseConfigEntities configEntities = FormulaHelper.GetEntities<BaseConfigEntities>();

        public void SendNotice(string ProjectInfoID, string WBSID, string MajorValue, string RoleCodes, string ReceiverIDs, string ReceiverNames,
            string Title, string Content, string Attachments, string LinkUrl, string RelateID, string RelateType, string NoticeType,
            string CreateUserID, string CreateUserName)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(CreateUserID))
            {
                var user = FormulaHelper.GetUserInfo();
                CreateUserID = user.UserID;
                CreateUserName = user.UserName;
            }
            if (string.IsNullOrEmpty(NoticeType))
                NoticeType = ProjectNoticeType.User.ToString();
            foreach (var RoleCode in RoleCodes.Split(','))
            {
                var sql = @"insert into S_N_Notice(ID,ProjectInfoID,WBSID,MajorValue,RoleCode,ReceiverIDs,ReceiverNames,Title,Content,Attachments,
IsFromSys,ExpiresTime,LinkUrl,RelateID,RelateType,NoticeType,CreateDate,CreateUserID,CreateUserName)
                            values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}') ";
                sql = string.Format(sql, FormulaHelper.CreateGuid(), ProjectInfoID, WBSID, MajorValue, RoleCode, ReceiverIDs, ReceiverNames, Title, Content, Attachments,
                    "True", DateTime.Now.AddDays(14).ToString("yyyy-MM-dd HH:mm:ss"), LinkUrl, RelateID, RelateType, NoticeType,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CreateUserID, CreateUserName);
                sb.AppendLine(sql);
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
                SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteNonQuery(sb.ToString());
        }

        /// <summary>
        /// 获取用户最近进入过的项目信息（默认只记录最近进入的5个项目信息）
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>返回用户最近进入过的5个项目信息</returns>
        public DataTable GetUserProjectList(string userID)
        {
            string sql = @"select S_I_ProjectInfo.ID,Name,Code,ChargeUserName from S_I_UserDefaultProjectInfo with(nolock) 
inner join S_I_ProjectInfo with(nolock) on S_I_UserDefaultProjectInfo.ProjectInfoID=S_I_ProjectInfo.ID
where S_I_UserDefaultProjectInfo.UserID='" + userID + "' order by S_I_UserDefaultProjectInfo.ID desc";
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 设置用户默认项目（最多设置10个）
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="projectInfoID">项目ID</param>
        public void SetUserProjectList(string userID, string projectInfoID)
        {
            var projectInfo = instanceEnitites.S_I_ProjectInfo.Find(projectInfoID);
            if (projectInfo == null) return;

            var sb = new StringBuilder();
            var deleteSql = @"
delete from S_I_UserDefaultProjectInfo where UserID='" + userID + "' and ProjectInfoID='" + projectInfoID + @"' 
delete from S_I_UserDefaultProjectInfo
where UserID='" + userID + @"' 
and ID not in (
select top 9 ID from S_I_UserDefaultProjectInfo
where UserID='" + userID + "' order by ID desc)";
            sb.AppendLine(deleteSql);
            var insertSql = string.Format(@"insert into S_I_UserDefaultProjectInfo (ID,UserID,ProjectInfoID,EngineeringInfoID) 
                Values('{0}','{1}','{2}','{3}')", FormulaHelper.CreateGuid(), userID, projectInfoID, projectInfo.GroupRootID);
            sb.AppendLine(insertSql);
            SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteNonQuery(sb.ToString());
        }

        /// <summary>
        /// 获取人员登录的默认项目，默认为上次进入的项目空间，如果用户首次登录，则默认选择第一个用户参与的项目
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>返回默认的项目ID</returns>
        public string GetDefaultProjectID(string userID)
        {
            string result = string.Empty;
            //var defaultProject = this.instanceEnitites.S_I_UserDefaultProjectInfo.Where(d => d.UserID == userID
            //        && !String.IsNullOrEmpty(d.ProjectInfoID)).OrderByDescending(d => d.ID).FirstOrDefault();
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var sql = "select top 1 * from S_I_UserDefaultProjectInfo with(nolock) where UserID='" + userID + @"' 
                            and ProjectInfoID is not null and ProjectInfoID !='' order by ID desc";
            var defaultProject = sqlHelper.ExecuteObject<S_I_UserDefaultProjectInfo>(sql);
            if (defaultProject != null)
            {
                var projectInfo = this.instanceEnitites.S_I_ProjectInfo.FirstOrDefault(d => d.ID == defaultProject.ProjectInfoID);
                if (projectInfo == null)
                {
                    //this.instanceEnitites.S_I_UserDefaultProjectInfo.Remove(defaultProject);
                    //this.instanceEnitites.SaveChanges();
                    sqlHelper.ExecuteNonQuery("delete from S_I_UserDefaultProjectInfo where id='" + defaultProject.ID + "'");
                    result = string.Empty;
                }
                else
                    result = defaultProject.ProjectInfoID;
            }

            if (String.IsNullOrEmpty(result))
            {
                var rbs = this.instanceEnitites.S_W_RBS.FirstOrDefault(d => d.UserID == userID);
                if (rbs != null)
                {
                    result = rbs.ProjectInfoID;
                    this.SetUserProjectList(userID, rbs.ProjectInfoID);
                }
            }
            return result;
        }

        public List<Dictionary<string, object>> GetDBSTree(string projectInfoID, string userID, bool isViewAuth = false, bool isProduct = false, bool showMaxVersion = false)
        {
            var result = new List<Dictionary<string, object>>();
            var projectInfo = this.instanceEnitites.S_I_ProjectInfo.SingleOrDefault(d => d.ID == projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象，无法展现DBS结构");
            var dbsList = this.instanceEnitites.S_D_DBS.Include("S_D_DBSSecurity").Include("S_D_Document").Where(d => d.ProjectInfoID == projectInfoID).OrderBy(a => a.SortIndex).ThenBy(a => a.ID).ToList();

            foreach (var dbs in dbsList)
            {
                if (isProduct)
                {
                    if (dbs.DBSType != DBSType.Mapping.ToString() || dbs.MappingType != DBSMappingType.Product.ToString())
                        continue;
                }
                var allChildren = dbsList.Where(a => a.FullID.StartsWith(dbs.FullID)).ToList();
                var dbsDic = dbs.ToDic();
                var fileCount = allChildren.Sum(d => d.S_D_Document.Count);
                dbsDic.SetValue("AllFileCount", fileCount);
                dbsDic.SetValue("AllArchiveFileCount", allChildren.Sum(d => d.S_D_Document.Count(p => p.ArchiveDate.HasValue)));
                dbsDic.SetValue("FileCount", dbs.S_D_Document.Count);
                if (showMaxVersion)
                {
                    var maxVersionFileCount = 0;
                    var _doclist = new List<S_D_Document>();
                    allChildren.ForEach(a => _doclist.AddRange(a.S_D_Document.ToList()));
                    var _group = _doclist.GroupBy(a => a.RelateID).Select(a =>
                        new { a.Key, MaxVersion = a.Max(b => (string.IsNullOrEmpty(b.Version) ? 0d : Convert.ToDouble(b.Version))) }
                        ).ToList();
                    _doclist = _doclist.Where(a => _group.Any(g => g.Key == a.RelateID && g.MaxVersion == (string.IsNullOrEmpty(a.Version) ? 0d : Convert.ToDouble(a.Version)))).ToList();
                    maxVersionFileCount = _doclist.Count;
                    dbsDic.SetValue("MaxVersionFileCount", maxVersionFileCount);
                }
                if (dbs.DBSType == DBSType.Mapping.ToString() || dbs.DBSType == DBSType.OEMMapping.ToString())
                {
                    dbsDic.SetValue("DBSAuth", FolderAuthType.ReadOnly.ToString());
                }
                else
                {
                    var auth = dbs.GetUserAuth(userID);
                    if (!isViewAuth && auth == FolderAuthType.None) continue;
                    dbsDic.SetValue("DBSAuth", auth.ToString());
                    if (isViewAuth && auth == FolderAuthType.None)
                        dbsDic.SetValue("DBSAuth", FolderAuthType.ReadOnly.ToString());
                }
                result.Add(dbsDic);
            }
            return result;
        }

        /// <summary>
        /// 根据人员和项目ID获取该人员所拥有的所有权限入口
        /// </summary>
        /// <param name="userID">人员ID</param>
        /// <param name="projectInfoID">项目ID</param>
        /// <returns>入口集合</returns>
        public List<Entrance> GetEntrance(string userID, string projectInfoID)
        {
            if (String.IsNullOrEmpty(projectInfoID))
            {
                //var userDefault = instanceEnitites.S_I_UserDefaultProjectInfo.FirstOrDefault(d => d.UserID == userID);
                var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
                var sql = "select top 1 * from S_I_UserDefaultProjectInfo with(nolock) where UserID='" + userID + @"' 
                            and ProjectInfoID is not null and ProjectInfoID !='' order by ID desc";
                var userDefault = sqlHelper.ExecuteObject<S_I_UserDefaultProjectInfo>(sql);
                if (userDefault != null)
                    projectInfoID = userDefault.ProjectInfoID;
            }
            if (String.IsNullOrEmpty(projectInfoID)) throw new Formula.Exceptions.BusinessException("空的项目ID无法加载项目管理空间");
            var userService = FormulaHelper.GetService<IUserService>();
            var sysRoles = userService.GetRoleCodesForUser(userID, Config.Constant.OrgRootID);
            var project = instanceEnitites.S_I_ProjectInfo.FirstOrDefault(d => d.ID == projectInfoID);
            if (project == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象，无法获取空间入口信息");
            if (project.ProjectMode == null) throw new Formula.Exceptions.BusinessException("项目【" + project.Name + "】的管理模式对象为空，请未项目指定管理模式");
            var result = project.GetEntraceBySysRoles(sysRoles);
            var projectList = project.GetEntraceByProjectRoles(project.GetUserOBSRoles(userID));
            foreach (var item in projectList.ToList())
            {
                if (result.Exists(d => d.Code == item.Code)) continue;
                result.Add(item);
            }

            var roles = project.GetUserOBSRoles(userID);
            if (!roles.Exists(d => String.IsNullOrEmpty(d.MajorValue)))
            {
                foreach (var item in roles)
                {
                    if (!result.Exists(d => d.Code == item.MajorValue))
                    {
                        result.RemoveWhere(d => d.Code == item.MajorValue);
                    }
                    else
                    {
                        var defaultEntrace = result.FirstOrDefault(d => d.Code == item.MajorValue);
                        foreach (var entrace in result)
                            entrace.IsDefault = false;
                        defaultEntrace.IsDefault = true;
                        break;
                    }
                }
            }
            if (!result.Exists(d => d.IsDefault) && result.Count > 0)
                result.FirstOrDefault().IsDefault = true;
            return result.OrderBy(d => d.SortIndex).ToList();
        }

        /// <summary>
        /// 获得指定根节点下的菜单入口
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="projectInfoID">项目ID</param>
        /// <param name="defineRootID">空间根ID</param>
        /// <param name="relateCode">空间编号</param>
        /// <returns>入口集合</returns>
        public List<S_T_SpaceDefine> GetSpaceDefine(string userID, string projectInfoID, string defineRootID, string relateCode)
        {
            var userService = FormulaHelper.GetService<IUserService>();
            var project = instanceEnitites.S_I_ProjectInfo.FirstOrDefault(d => d.ID == projectInfoID);
            if (project == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象，无法获取空间入口信息");
            if (project.ProjectMode == null) throw new Formula.Exceptions.BusinessException("项目【" + project.Name + "】的管理模式对象为空，请未项目指定管理模式");

            var sysRoles = userService.GetRoleCodesForUser(userID, Config.Constant.OrgRootID);
            var roleList = project.GetUserOBSRoles(userID);

            var result = project.GetSpaceDefineBySysRoles(defineRootID, sysRoles);
            var projectRoleResult = project.GetSpaceDefineByProjectRoles(defineRootID, roleList, relateCode);

            foreach (var item in projectRoleResult)
            {
                var siItem = result.FirstOrDefault(d => d.ID == item.ID);
                if (siItem == null) { result.Add(item); }
                else
                {
                    if (item.AuthType == SpaceAuthType.FullControl.ToString())
                        siItem.AuthType = SpaceAuthType.FullControl.ToString();
                }
            }

            var root = result.FirstOrDefault(d => d.ID == defineRootID);
            if (root == null)
            {

            }
            if (root.DefineType == SpaceDefineType.Dynamic.ToString())
            {
                var json = FormulaHelper.GetProperty(project, root.DynamicDataFiled);
                if (json != null && !String.IsNullOrEmpty(json.ToString()))
                {
                    var list = JsonHelper.ToList(json.ToString());
                    var rootDic = list.FirstOrDefault(d => d["Value"].ToString() == relateCode);
                    if (rootDic != null)
                        root.Name = rootDic["Name"].ToString();
                }
            }

            return result.OrderBy(d => d.SortIndex).ToList();
        }

        /// <summary>
        /// 获得甘特图里程碑
        /// </summary>
        /// <param name="projectInfoID">项目信息ID</param>
        /// <param name="includeWBS">是否包含WBS信息</param>
        /// <returns>甘特图集合</returns>
        public List<Dictionary<string, object>> GetMileStoneGantt(string projectInfoID, bool includeWBS = false)
        {
            var result = new List<Dictionary<string, object>>();
            var projectInfo = this.instanceEnitites.S_I_ProjectInfo.FirstOrDefault(d => d.ID == projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【】的项目对象，无法获取里程碑信息");
            var wbsRoot = projectInfo.WBSRoot;
            var mileStoneList = projectInfo.S_P_MileStone.Where(d => d.MileStoneType == MileStoneType.Normal.ToString()).OrderBy(d => d.SortIndex).ToList();
            var root = wbsRoot.ToDic();
            root.SetValue("UID", wbsRoot.ID);
            root.SetValue("WBSID", wbsRoot.ID);
            result.Add(root);
            this.FillMileStoneGantList(root, wbsRoot.ID, result, mileStoneList);
            if (includeWBS)
            {
                var wbsList = projectInfo.S_W_WBS.Where(d => d.WBSType != WBSNodeType.PackageType.ToString()
                    && d.WBSType != WBSNodeType.Work.ToString() && d.WBSType != WBSNodeType.Project.ToString()).OrderBy(d => d.SortIndex).ToList();

                foreach (var wbs in wbsList)
                {
                    var dic = FormulaHelper.ModelToDic<S_W_WBS>(wbs);
                    dic.SetValue("UID", wbs.ID);
                    dic.SetValue("ParentTaskUID", wbs.ParentID);
                    dic.SetValue("WBSID", wbs.ID);
                    dic.SetValue("Start", wbs.PlanStartDate);
                    dic.SetValue("Finish", wbs.PlanEndDate);
                    result.Add(dic);
                    FillMileStoneGantList(dic, wbs.ID, result, mileStoneList);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取项目工作区的WBS节点
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetWorkSpaceWBS(string projectInfoID, string userID, string viewType, string majorCode)
        {
            var result = new List<Dictionary<string, object>>();
            var projectInfo = this.instanceEnitites.S_I_ProjectInfo.FirstOrDefault(d => d.ID == projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象，无法获得WBS结构");
            var taskWorkType = WBSNodeType.Work.ToString();
            var rootStruct = projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Project.ToString());
            string defaultViewType = rootStruct.Code + "," + rootStruct.ChildCode;
            var wbsFo = FormulaHelper.CreateFO<WBSFO>();
            if (!String.IsNullOrEmpty(majorCode))
            {
                result = wbsFo.CreateWBSTree(projectInfoID, viewType, true, majorCode);
                foreach (var item in result)
                {
                    item.SetValue("HasAuth", TrueOrFalse.False.ToString());
                    var wbsID = item.GetValue("ID");
                    if (String.IsNullOrEmpty(wbsID))
                        continue;
                    if (item["RBSLIst"] != null && item["RBSLIst"] is List<S_W_RBS>)
                    {
                        var list = item["RBSLIst"] as List<S_W_RBS>;
                        if (list.Exists(c => c.UserID == userID))
                        {
                            item.SetValue("HasAuth", TrueOrFalse.True.ToString());
                        }
                    }
                    else
                    {
                        var wbs = instanceEnitites.S_W_WBS.Include("S_W_RBS").SingleOrDefault(d => d.ID == wbsID);
                        if (wbs == null) continue;
                        if (wbs.S_W_RBS.Count(d => d.UserID == userID) > 0)
                            item.SetValue("HasAuth", TrueOrFalse.True.ToString());
                    }
                }
            }
            else
            {
                var wbsList = instanceEnitites.S_W_WBS.Include("S_W_RBS").Where(d => d.ProjectInfoID == projectInfoID).ToList();
                foreach (var item in wbsList)
                {
                    var dic = item.ToDic();
                    dic.SetValue("VirtualID", item.ID);
                    if (item.S_W_RBS.Count(d => d.UserID == userID) > 0)
                        dic.SetValue("HasAuth", TrueOrFalse.True.ToString());
                    else
                        dic.SetValue("HasAuth", TrueOrFalse.False.ToString());
                    result.Add(dic);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取专业提资区的WBS树节点
        /// </summary>
        /// <param name="projectInfoID"></param>
        /// <param name="userID"></param>
        /// <param name="majorCode"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetCooperationWBS(string projectInfoID, string userID, string majorCode)
        {
            var result = new List<Dictionary<string, object>>();
            var projectInfo = this.instanceEnitites.S_I_ProjectInfo.FirstOrDefault(d => d.ID == projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象，无法获得WBS结构");
            var taskWorkType = WBSNodeType.Work.ToString();
            result.Add(projectInfo.WBSRoot.ToDic());
            var coperationWBSStructCodeList = projectInfo.ProjectMode.S_T_WBSStructInfo.Where(d => !String.IsNullOrEmpty(d.ChildCode) && d.ChildCode.Contains(WBSNodeType.Major.ToString())).Select(d => d.Code).ToList();
            var pwbsList = projectInfo.WBSRoot.AllChildren.Where(d => coperationWBSStructCodeList.Contains(d.WBSType)).ToList();
            foreach (var pwbs in pwbsList)
            {
                var dic = pwbs.ToDic();
                result.Add(dic);
            }
            return result;
        }

        /// <summary>
        /// 获取WBS树形结构，可根据不同视角进行分组切换
        /// </summary>
        /// <param name="projectInfoID">项目ID</param>
        /// <param name="viewType">视角</param>
        /// <param name="majorCode">专业编码</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetWBSTree(string projectInfoID, string viewType = "Project", bool includeWork = false, string majorCode = "", bool includeCoop = true)
        {
            var wbsFO = FormulaHelper.CreateFO<WBSFO>();
            return wbsFO.CreateWBSTree(projectInfoID, viewType, includeWork, majorCode, "", false, includeCoop);
        }

        /// <summary>
        /// 根据项目ID或人员ID来获取人员参与的RBS信息
        /// </summary>
        /// <param name="userID">人员信息，如果人员信息为空，则默认获取当前登录人</param>
        /// <returns>人员在该项目上参与的RBS对象集合</returns>
        public List<Dictionary<string, object>> GetUserInfo(string projectInfoID, string userID = "")
        {
            var result = new List<Dictionary<string, object>>();
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            if (String.IsNullOrEmpty(userID))
                userID = FormulaHelper.GetUserInfo().UserID;
            var rbsList = entities.S_W_RBS.Where(d => d.ProjectInfoID == projectInfoID && d.UserID == userID)
                .Select(d => d.S_W_WBS).Distinct().ToList();
            return result;
        }

        public void DeleteWBSNodeWithADO(DataRow wbsNode, DataTable wbsDt = null, string ProjectInfoID = "", StringBuilder sqlCommand = null)
        {
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            if (wbsDt == null && String.IsNullOrEmpty(ProjectInfoID))
            {
                throw new Formula.Exceptions.BusinessValidationException("删除WBS必须指定项目ID，或者WBS集合");
            }
            if (wbsDt == null)
            {
                wbsDt = db.ExecuteDataTable(@"SELECT ID,Name,WBSValue,FullID,ParentID,Code,WBSType from 
S_W_WBS with(nolock) where ProjectInfoID='" + ProjectInfoID + "' and FullID like '" + wbsNode["FullID"] + "%'");
            }
            var count = Convert.ToInt32(db.ExecuteScalar("select count(ID) from S_E_Product with(nolock) where  WBSFullID like '" + wbsNode["FullID"] + "%'"));
            if (count > 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("已经有成果的节点不能删除");
            }

             if (sqlCommand != null)
            {
                sqlCommand.AppendLine("delete from S_W_WBS WHERE ID='" + wbsNode["ID"] + "'");
            }
            else
            {
                db.ExecuteNonQuery("delete from S_W_WBS WHERE ID='" + wbsNode["ID"] + "'");
            }
             var allChildren = wbsDt.AsEnumerable().Where(c => c["FullID"].ToString().StartsWith(wbsNode["FullID"].ToString()));
             if (sqlCommand != null)
             {
                 sqlCommand.AppendLine("delete from S_W_WBS WHERE ID in ('" + String.Join(",", allChildren.Select(c => c["ID"].ToString())).Replace(",", "','") + "')");
             }
             else
             {
                 db.ExecuteNonQuery("delete from S_W_WBS WHERE ID in ('" + String.Join(",", allChildren.Select(c => c["ID"].ToString())).Replace(",", "','") + "')");
             }
        }

        public void AddWBSChildWithAdo(DataRow parentWBSNode, DataRow childWBSNode, DataTable wbsDt, List<S_T_WBSStructInfo> structList)
        {
            if (!childWBSNode.Table.Columns.Contains("AddState"))
            {
                childWBSNode.Table.Columns.Add("AddState");
            }
            if (parentWBSNode["WBSType"] == null || parentWBSNode["WBSType"] == DBNull.Value || String.IsNullOrEmpty(parentWBSNode["WBSType"].ToString()))
            {
                throw new Formula.Exceptions.BusinessValidationException("必须指定父WBS节点的类型");
            }
            if (childWBSNode["WBSType"] == null || childWBSNode["WBSType"] == DBNull.Value || String.IsNullOrEmpty(childWBSNode["WBSType"].ToString()))
            {
                throw new Formula.Exceptions.BusinessValidationException("必须指定WBS节点的类型");
            }

            var stNode = structList.FirstOrDefault(c => c.Code == parentWBSNode["WBSType"].ToString());
            if (stNode == null)
            {
                throw new Formula.Exceptions.BusinessException("【" + parentWBSNode["Name"].ToString() + "】未获取WBS结构定义对象，无法新增子节点");
            }
            if (!stNode.ValidateChildren(childWBSNode["WBSType"].ToString()))
            {
                throw new Formula.Exceptions.BusinessException("【" + EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), parentWBSNode["WBSType"].ToString())
                 + "】节点下不包含【" + EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), childWBSNode["WBSType"].ToString()) + "】的子节点定义，无法新增子节点");
            }

            if (childWBSNode["ID"] == null || childWBSNode["ID"] == DBNull.Value || String.IsNullOrEmpty(childWBSNode["ID"].ToString()))
                childWBSNode["ID"] = FormulaHelper.CreateGuid();
            childWBSNode["ProjectInfoID"] = parentWBSNode["ProjectInfoID"];
            childWBSNode["ParentID"] = parentWBSNode["ID"];
            childWBSNode["PhaseCode"] = parentWBSNode["PhaseCode"];
            childWBSNode["FullID"] = parentWBSNode["FullID"].ToString() + "." + childWBSNode["ID"].ToString();
            childWBSNode["Level"] = childWBSNode["FullID"].ToString().Split('.').Length;
            childWBSNode["WBSStructCode"] = childWBSNode["WBSType"];
            childWBSNode["AddState"] = true.ToString().ToLower();
            var brothers = wbsDt.AsEnumerable().Where(c => c["ParentID"] != DBNull.Value 
                && c["ParentID"].ToString() == parentWBSNode["ID"].ToString()).ToList();
            if (brothers.Count == 0)
            {
                childWBSNode["CodeIndex"] = 1;
            }
            else
            {
                var maxCodeIndex = brothers.Where(c => c["CodeIndex"] != DBNull.Value && c["CodeIndex"] != null).Max(c => c["CodeIndex"]);
                if (maxCodeIndex != null)
                    childWBSNode["CodeIndex"] = Convert.ToInt32(maxCodeIndex) + 1;
                else
                {
                    childWBSNode["CodeIndex"] = 1;
                }
            }
            childWBSNode["State"] = ProjectCommoneState.Plan.ToString();
            childWBSNode["SortIndex"] = childWBSNode["FullID"].ToString().Split('.').Length * 1000 + 0;
            if ((childWBSNode["WBSValue"] == null || childWBSNode["WBSValue"] == DBNull.Value || String.IsNullOrEmpty(childWBSNode["WBSValue"].ToString()))
                && stNode.IsDefineNode)
                throw new Formula.Exceptions.BusinessException("枚举类WBS节点，必须指定WBSValue");

            //获取所有的上级节点，填充WBS类别属性字段
            var ancestors = wbsDt.AsEnumerable().Where(c => childWBSNode["FullID"].ToString().StartsWith(c["FullID"].ToString()));
            foreach (var ancestor in ancestors)
            {
                var fieldName = ancestor["WBSType"].ToString() + "Code";
                if (childWBSNode.Table.Columns.Contains(fieldName))
                {
                    childWBSNode[fieldName] = ancestor["WBSValue"];
                }
            }
            if (childWBSNode.Table.Columns.Contains(childWBSNode["WBSType"].ToString() + "Code"))
            {
                childWBSNode[childWBSNode["WBSType"].ToString() + "Code"] = childWBSNode["WBSValue"];
            }
            wbsDt.Rows.Add(childWBSNode);
        }

        public void ResetOBSUserFromRBSWithAdo(string ProjectInfoID)
        {
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            db.ExecuteNonQuery(String.Format("delete from S_W_OBSUser where ProjectInfoID='{0}'", ProjectInfoID));
            var enumService = FormulaHelper.GetService<IEnumService>();
            var majorEnums = enumService.GetEnumDataSource("Project.Major").ToList();
            var rbsDt = db.ExecuteDataTable(String.Format("SELECT * FROM S_W_RBS WITH(NOLOCK) WHERE ProjectInfoID='{0}'", ProjectInfoID));
            var obsList = rbsDt.AsEnumerable().Select(c => new
            {
                UserID = c["UserID"].ToString(),
                RoleCode = c["RoleCode"].ToString(),
                RoleName = c["RoleName"].ToString(),
                MajorValue = c["MajorValue"].ToString(),
            }).Distinct().ToList();
            var obsDt = db.ExecuteDataTable("SELECT * FROM S_W_OBSUser WHIT(NOLOCK) WHERE 1<>1");
            foreach (var item in obsList)
            {
                var userInfo = FormulaHelper.GetUserInfoByID(item.UserID);
                var majorName = string.Empty;
                if (majorEnums.Any(a => a.Value == item.MajorValue))
                    majorName = majorEnums.FirstOrDefault(a => a.Value == item.MajorValue).Text;
                var obsRow = obsDt.NewRow();
                obsRow["ID"] = FormulaHelper.CreateGuid();
                obsRow["ProjectInfoID"] = ProjectInfoID;
                obsRow["RoleCode"] = item.RoleCode;
                obsRow["RoleName"] = item.RoleName;
                obsRow["MajorValue"] = item.MajorValue;
                obsRow["MajorName"] = majorName;
                obsRow["UserID"] = userInfo.UserID;
                obsRow["UserName"] = userInfo.UserName;
                obsRow["DeptID"] = userInfo.UserOrgID;
                obsRow["DeptName"] = userInfo.UserOrgName;
                obsRow["IsCloud"] = "F";
                obsDt.Rows.Add(obsRow);
            }
            db.BulkInsertToDB(obsDt, "S_W_OBSUser");
        }


        #region 私有方法

        private void FillMileStoneGantList(Dictionary<string, object> parentNode, string parentID,
           List<Dictionary<string, object>> result, List<S_P_MileStone> mileStoneList)
        {
            var children = mileStoneList.Where(d => d.WBSID == parentID).OrderBy(d => d.SortIndex).ToList();
            foreach (var child in children)
            {
                var dic = child.ToDic();
                dic.SetValue("UID", child.ID);
                dic.SetValue("MileStoneID", child.ID);
                dic.SetValue("ParentTaskUID", parentID);
                if (child.PlanFinishDate == null)
                {
                    dic.SetValue("Start", DateTime.Now.ToShortDateString() + " 00:00:00");
                    dic.SetValue("Finish", DateTime.Now.ToShortDateString() + " 23:59:59");
                }
                else
                {
                    dic.SetValue("Start", child.PlanFinishDate.Value.ToShortDateString() + " 00:00:00");
                    dic.SetValue("Finish", child.PlanFinishDate.Value.ToShortDateString() + " 23:59:59");
                }
                dic.SetValue("Milestone", "1");
                result.Add(dic);
            }
        }

        /// <summary>
        /// 递归创建工作包节点
        /// </summary>
        /// <param name="parentWorkNode"></param>
        /// <param name="parentVirtualNodeID"></param>
        /// <param name="cwbsList"></param>
        /// <param name="result"></param>
        private void createWorkNodes(S_W_WBS parentWorkNode, string parentVirtualNodeID
            , List<S_W_WBS> cwbsList, List<Dictionary<string, object>> result)
        {
            var works = cwbsList.Where(d => d.ParentID == parentWorkNode.ID).ToList();
            foreach (var work in works)
            {
                var dic = work.ToDic();
                var virtualID = FormulaHelper.CreateGuid();
                dic.SetValue("VirtualID", virtualID);
                dic.SetValue("ParentID", parentVirtualNodeID);
                result.Add(dic);
                createWorkNodes(work, virtualID, cwbsList, result);
            }
        }

        #endregion
    }

    public class ProjectViewModel
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ProjectPhase { get; set; }
        public string ProjectType { get; set; }
        public string ProjectClass { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string DeptName { get; set; }
        public string Status { get; set; }
        /// <summary>
        /// 参与专业，从S_E_Entity表中获取
        /// </summary>
        public string JoinMajor { get; set; }

        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }

        public string FactStartDate { get; set; }
        public string FactEndDate { get; set; }

        public IList<MileStoneViewModel> MileStones { get; set; }

        public IList<OBSUserViewModel> OBSUsers { get; set; }
    }

    public class MileStoneViewModel
    {
        public string Name { get; set; }
        public string PlanEndDate { get; set; }
        public string FactEndDate { get; set; }
        public string Status { get; set; }
    }

    public class OBSUserViewModel
    {
        public string OBSName { get; set; }
        public string DeptName { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }

    }
}
