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
namespace Project.Areas.Basic.Controllers
{
    public class WBSMajorController : ProjectController<S_W_TaskWork>
    {
        public override JsonResult GetModel(string id)
        {
            S_W_TaskWork taskWork = entities.Set<S_W_TaskWork>().Find(id);
            if (taskWork == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + id + "】的套图信息，加载工作包信息面板失败");
            var workWBS = taskWork.S_W_WBS;
            if (workWBS == null) throw new Formula.Exceptions.BusinessException("未找到当前套图对应的工作包节点信息");
            var majorWBS = workWBS.Parent;
            var returnDic = taskWork.ToDic();
            returnDic.SetValue("MajorWBSID", majorWBS.ID);
            returnDic.SetValue("MajorValue", majorWBS.WBSValue);
            returnDic.SetValue("MajorName", majorWBS.Name);
            returnDic.SetValue("MajorChargeUserID", majorWBS.ChargeUserID);
            returnDic.SetValue("MajorChargeUserName", majorWBS.ChargeUserName);
            returnDic.SetValue("MajorWBSChargeDeptID", majorWBS.WBSDeptID);
            returnDic.SetValue("MajorWBSChargeDeptName", majorWBS.WBSDeptName);
            //获取卷册工日VolumeWorkDay
            return Json(returnDic);
        }

        public JsonResult SaveSingleTaskWork(string TaskWork, string ParentWBSID)
        {
            Dictionary<string, object> dic = JsonHelper.ToObject<Dictionary<string, object>>(TaskWork);
            var parent = this.GetEntityByID<S_W_WBS>(ParentWBSID);
            if (parent == null) throw new Formula.Exceptions.BusinessException("未获取到当前工作包的父节点，保存套图失败。");
            SaveTaskWorkByDic(dic, parent);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetVolumeUserList(string taskWorkID)
        {
            var taskWork = entities.Set<S_W_TaskWork>().Find(taskWorkID);
            if (taskWork == null) throw new Formula.Exceptions.BusinessException("未找到ID为【" + taskWorkID + "】的套图信息，获取设计人员列表失败。");
            var designerRoleCode = AuditRoles.Designer.ToString();
            var data = entities.Set<S_W_RBS>().Where(c => c.WBSID == taskWork.WBSID && c.RoleCode == designerRoleCode).ToList();
            return Json(data);
        }

        public ActionResult WBS()
        {
            string projectInfoID = GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息");
            var majorStruct = projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Major.ToString());
            if (majorStruct == null) throw new Formula.Exceptions.BusinessException("项目不存在专业节点定义，无法进行专业策划");
            var structRoles = majorStruct.S_T_WBSStructRole.Where(d => d.SychWBS != true.ToString()).OrderBy(d => d.SortIndex).ToList();
            ViewBag.StructRoles = structRoles;

            string sychField = "";
            foreach (var item in structRoles)
            {
                sychField += item.RoleCode + "UserID,";
                sychField += item.RoleCode + "UserName,";
            }
            ViewBag.SychField = sychField.TrimEnd(',');
            ViewBag.ProjectInfoID = GetQueryString("ProjectInfoID");
            ViewBag.SpaceCode = GetQueryString("SpaceCode");
            ViewBag.ProjectInfo = JsonHelper.ToJson(projectInfo);
            ViewBag.ShowOwnNode = GetQueryString("ShowOwnNode");
            return View();
        }

        public override JsonResult GetTree()
        {
            var fo = FormulaHelper.CreateFO<WBSFO>();
            string projectInfoID = GetQueryString("ProjectInfoID");
            string code = this.Request["SpaceCode"];
            if (String.IsNullOrEmpty(code))
            {
                var relateMajorCode = this.entities.Set<S_W_RBS>().Where(d => d.UserID == this.CurrentUserInfo.UserID
                     && d.ProjectInfoID == projectInfoID && !String.IsNullOrEmpty(d.WBSCode) && d.WBSType == WBSNodeType.Major.ToString()).Select(c => c.WBSCode).Distinct().ToList();
                foreach (var item in relateMajorCode)
                    code += item + ",";
            }
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (String.IsNullOrEmpty(projectInfoID)) throw new Formula.Exceptions.BusinessException("参数ProjectInfoID不能为空");
            var result = fo.CreateWBSTree(projectInfoID, WBSNodeType.Major.ToString(), false, code, "", false, false);
            foreach (var item in result)
            {
                var wbsID = item.GetValue("VirtualID");
                if (String.IsNullOrEmpty(wbsID)) continue;
                if (item.GetValue("WBSType") == WBSNodeType.Project.ToString()) continue;
                var wbs = entities.Set<S_W_WBS>().Include("S_W_RBS").SingleOrDefault(d => d.ID == wbsID);
                if (wbs == null) continue;
                if (wbs.S_W_RBS.Count(d => d.UserID == this.CurrentUserInfo.UserID && d.MajorValue == wbs.WBSValue) > 0)
                    item.SetValue("HasAuth", TrueOrFalse.True.ToString());
                else
                    item.SetValue("HasAuth", TrueOrFalse.False.ToString());
                foreach (var stRole in wbs.StructNodeInfo.S_T_WBSStructRole.ToList())
                {
                    string userID = string.Empty; string userName = string.Empty;
                    foreach (var rbs in wbs.S_W_RBS.Where(d => d.RoleCode == stRole.RoleCode).ToList())
                    {
                        userID += rbs.UserID + ",";
                        userName += rbs.UserName + ",";
                    }
                    item.SetValue(stRole.RoleCode + "UserID", userID.TrimEnd(','));
                    item.SetValue(stRole.RoleCode + "UserName", userName.TrimEnd(','));
                }
            }
            string showOwn = GetQueryString("ShowOwnNode");
            if (showOwn == TrueOrFalse.True.ToString())
            {
                #region 隐藏和自己无关的专业
                var deleteIds = new List<string>();
                var maxLevel = result.Max(a => Convert.ToInt32(a.GetValue("VirtualLevel")));
                for (int i = maxLevel; i > 1; i--)
                {
                    foreach (var item in result.Where(a => a.GetValue("VirtualLevel") == i.ToString()))
                    {
                        var itemID = item.GetValue("VirtualID");
                        if (i == maxLevel)
                        {
                            if (item.GetValue("HasAuth") == TrueOrFalse.False.ToString())//末层节点没有权限的 需要删除
                                deleteIds.Add(itemID);
                        }
                        else
                        {
                            if (!result.Where(a => !deleteIds.Contains(a.GetValue("VirtualID"))).Any(a => a.GetValue("ParentID") == itemID))//不存在子节点的 需要删除
                                deleteIds.Add(itemID);
                        }
                    }
                }
                result = result.Where(a => !deleteIds.Contains(a.GetValue("VirtualID"))).ToList();
                #endregion
            }

            return Json(result);
        }

        public JsonResult SaveWBSUserInfo(string WBSInfo)
        {
            var list = JsonHelper.ToList(WBSInfo);
            S_I_ProjectInfo prj = null;
            foreach (var item in list)
            {
                if (String.IsNullOrEmpty(item.GetValue("VirtualID")))
                    continue;
                var wbs = this.GetEntityByID<S_W_WBS>(item.GetValue("VirtualID"));

                this.entities.Set<S_W_RBS>().Delete(c => c.WBSID == wbs.ID);
                if (prj == null)
                {
                    prj = wbs.S_I_ProjectInfo;
                    prj.ModifyDate = DateTime.Now;//c_hua 修改时间以便同步程序同步人员给四方系统
                }
                var structInfoRoles = wbs.StructNodeInfo.S_T_WBSStructRole.ToList();
                foreach (var stRole in structInfoRoles)
                {
                    var roleField = stRole.RoleCode + "UserID";
                    var userID = item.GetValue(roleField);
                    if (String.IsNullOrEmpty(userID))
                    {
                        wbs.RemoveUser(stRole.RoleCode);
                        continue;
                    }
                    wbs.SetUsers(stRole.RoleCode, userID.Split(','), false);
                }
            }
            this.entities.SaveChanges();

            prj.ResetOBSUserFromRBS();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveWork(string TaskInfo, string ParentWBSID)
        {
            var list = JsonHelper.ToList(TaskInfo);
            var parent = this.GetEntityByID<S_W_WBS>(ParentWBSID);
            if (parent == null) throw new Formula.Exceptions.BusinessException("未获取到当前工作包的父节点，保存套图失败。");
            foreach (var item in list)
            {
                S_W_TaskWork taskWork;
                if (String.IsNullOrEmpty(item.GetValue("ID")))
                {
                    taskWork = new S_W_TaskWork();
                    this.UpdateEntity<S_W_TaskWork>(taskWork, item);
                    parent.AddTaskWork(taskWork);
                }
                else
                {
                    taskWork = this.GetEntityByID<S_W_TaskWork>(item.GetValue("ID"));
                    this.UpdateEntity<S_W_TaskWork>(taskWork, item);
                    taskWork.Save();
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            string parentID = GetQueryString("ParentID");
            var wbs = this.GetEntityByID<S_W_WBS>(parentID);
            if (wbs == null) return null;
            var dataGrid = this.entities.Set<S_W_TaskWork>().Where(d => d.WBSFullID.StartsWith(wbs.FullID)).WhereToGridData(qb);
            return Json(dataGrid);
        }

        public JsonResult DeleteWBS(string WBSInfo, string ProjectInfoID)
        {
            var list = JsonHelper.ToList(WBSInfo);
            Action action = () =>
            {
                var productDt = this.SqlHelper.ExecuteDataTable(String.Format("SELECT ID,WBSFullID FROM S_E_Product WITH(NOLOCK) WHERE ProjectInfoID='{0}'",
                    ProjectInfoID));
                var deleteWBSList = new List<Dictionary<string, object>>();
                foreach (var item in list)
                {
                    if (!list.Exists(c => c.GetValue("ID") == item.GetValue("ParentID")))
                        deleteWBSList.Add(item);
                }

                foreach (var item in deleteWBSList)
                {
                    var wbsId = item.GetValue("ID");
                    var fullID = item.GetValue("FullID");
                    if (item.GetValue("WBSType") == WBSNodeType.Project.ToString())
                    {
                        throw new Formula.Exceptions.BusinessValidationException("WBS【" + item.GetValue("Name") + "】是根节点，不能删除");
                    }
                    else if (string.IsNullOrEmpty(item.GetValue("MajorCode")) || item.GetValue("VirtualWBSType") == WBSNodeType.Major.ToString())
                    {
                        throw new Formula.Exceptions.BusinessValidationException("只能删除专业下层节点");
                    }
                    if (Convert.ToInt32(productDt.Compute("Count(ID)", "WBSFullID like '" + fullID + "%'")) > 0)
                    {
                        //判定有成果的节点不允许删除
                        throw new Formula.Exceptions.BusinessValidationException("WBS【" + item.GetValue("Name") + "】已经有成果或它的下级节点有成果，无法进行删除");
                    }
                    this.SqlHelper.ExecuteNonQuery(String.Format("delete from S_W_WBS where FullID like '{0}%'", fullID));
                    this.SqlHelper.ExecuteNonQuery(String.Format(@"delete from S_P_CooperationPlan where 
WBSID in (select ID from S_W_WBS where FullID like  '{0}%')", fullID));
                    this.SqlHelper.ExecuteNonQuery(String.Format(@"delete from S_W_Activity where 
WBSID in (select ID from S_W_WBS where FullID like  '{0}%')", fullID));
                    this.SqlHelper.ExecuteNonQuery(String.Format(@"delete from S_P_MileStone where 
WBSID in (select ID from S_W_WBS where FullID like  '{0}%')", fullID));
                }
            };

            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                System.Transactions.TransactionOptions tranOp = new System.Transactions.TransactionOptions();
                tranOp.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (System.Transactions.TransactionScope ts =
                    new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, tranOp))
                {
                    action();
                    ts.Complete();
                }
            }
            else
            {
                action();
            }
            return Json("");
        }

        protected override void BeforeDelete(List<S_W_TaskWork> entityList)
        {
            foreach (var item in entityList)
                item.Delete();
        }

        public JsonResult Publish(string TaskInfo, string ParentWBSID)
        {
            var list = JsonHelper.ToList(TaskInfo);
            var parent = this.GetEntityByID<S_W_WBS>(ParentWBSID);
            if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ParentWBSID + "】的WBS对象，无法发布工作包");
            foreach (var item in list)
            {
                S_W_TaskWork taskWork;
                if (String.IsNullOrEmpty(item.GetValue("ID")))
                {
                    taskWork = new S_W_TaskWork();
                    this.UpdateEntity<S_W_TaskWork>(taskWork, item);
                    parent.AddTaskWork(taskWork);
                }
                else
                {
                    taskWork = this.GetEntityByID<S_W_TaskWork>(item.GetValue("ID"));
                    this.UpdateEntity<S_W_TaskWork>(taskWork, item);
                    taskWork.Save();
                }
                taskWork.Publish();
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportPackage(string DataSource, string TargetID, string WithUser)
        {
            var list = JsonHelper.ToList(DataSource);
            var wbs = this.GetEntityByID<S_W_WBS>(TargetID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + TargetID + "】的WBS节点，无法导入工作包");
            foreach (var item in list)
            {
                var task = new S_W_TaskWork();
                task.Name = item.GetValue("Name");
                task.Code = item.GetValue("Code");
                if (!String.IsNullOrEmpty(item.GetValue("Workload")))
                    task.Workload = Convert.ToDecimal(item.GetValue("Workload"));
                task.PhaseValue = item.GetValue("PhaseCode");
                task.MajorValue = item.GetValue("MajorCode");
                if (WithUser == true.ToString())
                    task.FillWBSUser(wbs);
                wbs.AddTaskWork(task);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportUserFormOBS(string ProjectInfoID, string UserList, string WBSList)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【】的项目信息对象，无法导入OBS人员");
            var userList = JsonHelper.ToList(UserList);
            if (String.IsNullOrEmpty(WBSList))
            {
                foreach (var user in userList)
                {
                    string roleCode = user.GetValue("RoleCode");
                    string majorValue = user.GetValue("MajorValue");
                    var wbsList = projectInfo.S_W_WBS.Where(d => d.WBSType == WBSNodeType.Major.ToString() && d.WBSValue == majorValue).ToList();
                    foreach (var major in wbsList)
                        major.SetUser(roleCode, user.GetValue("UserID"));
                }
            }
            else
            {
                var selectedWBSNodes = JsonHelper.ToList(WBSList);
                foreach (var user in userList)
                {
                    string roleCode = user.GetValue("RoleCode");
                    string majorValue = user.GetValue("MajorValue");
                    foreach (var node in selectedWBSNodes)
                    {
                        if (String.IsNullOrEmpty(node.GetValue("ID"))) continue;
                        var wbs = this.GetEntityByID<S_W_WBS>(node.GetValue("ID"));
                        if (wbs == null) continue;
                        if (wbs.WBSType != WBSNodeType.Major.ToString()) continue;
                        wbs.SetUser(roleCode, user.GetValue("UserID"));
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        /// <summary>
        /// 根据数据字典保存工作包
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="parentWBS"></param>
        internal void SaveTaskWorkByDic(Dictionary<string, object> dic, S_W_WBS parentWBS)
        {
            S_W_TaskWork taskWork;
            if (String.IsNullOrEmpty(dic.GetValue("ID")))
            {
                taskWork = new S_W_TaskWork();
                this.UpdateEntity<S_W_TaskWork>(taskWork, dic);
                parentWBS.AddTaskWork(taskWork);
            }
            else
            {
                taskWork = this.GetEntityByID<S_W_TaskWork>(dic.GetValue("ID"));
                this.UpdateEntity<S_W_TaskWork>(taskWork, dic);
                taskWork.Save();
            }

        }

        #region 石化版专业WBS策划
        public ActionResult WBSSchema()
        {
            string sql = "select distinct [Type] from S_D_WBSAttrDefine ";
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql);
            var lockedWBSType = "";
            foreach (DataRow row in dt.Rows)
                lockedWBSType += row["Type"].ToString() + ",";
            ViewBag.lockedWBSType = lockedWBSType.TrimEnd(',');

            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到ID为【" + projectInfoID + "】的项目信息，请联系管理员");
            sql = String.Format(@"select * from S_D_RoleDefine
where RoleCode in (select distinct RoleCode from dbo.S_T_WBSStructRole with(nolock)
where WBSStructID in (select ID from dbo.S_T_WBSStructInfo with(nolock)
where ModeID=(select top 1 ID from S_T_ProjectMode with(nolock)
where ModeCode='{0}') and SychWBS!='True'))
order by SortIndex", projectInfo.ModeCode);
            var roleColumns = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql);
            ViewBag.RoleColumns = roleColumns;
            var sb = new StringBuilder();
            string returnParams = "value:ID,text:Name";
            foreach (DataRow item in roleColumns.Rows)
            {
                var field = item["RoleCode"] + "UserID";
                sb.AppendFormat("addMultiUserSelector('{0}',{{returnParams:'{1}'}});\n", field, returnParams);
            }
            ViewBag.SelectorScript = sb.ToString();

            ViewBag.VirtualScroll = "false";
            var wbsCount = this.SqlHelper.ExecuteScalar("select count(ID) from S_W_WBS WHERE ProjectInfoID='" + projectInfoID + "'");
            if (Convert.ToInt32(wbsCount) > 300)
            {
                //大于300使用虚加载，需要更换TREEGIRD样式
                ViewBag.VirtualScroll = "true";
            }

            var enumNodeType = new List<Dictionary<string, object>>();
            sql = @"select distinct [Level] from S_W_WBS where ProjectInfoID='" + projectInfoID + "' order by Level";
            var list = this.SqlHelper.ExecuteDataTable(sql);
            for (int i = 0; i < list.Rows.Count; i++)
            {
                var item = list.Rows[i];
                if (i == list.Rows.Count - 1)
                {
                    ViewBag.ExpandLevel = i;
                    continue;
                }
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", item["Level"]);
                dic.SetValue("text", "第" + item["Level"] + "层");
                dic.SetValue("sortindex", item["Level"]);
                enumNodeType.Add(dic);
            }
            ViewBag.NodeTypeEnum = JsonHelper.ToJson(enumNodeType);
            var verion = projectInfo.S_W_WBS_Version.Where(c => c.FlowPhase == "End").OrderByDescending(c => c.ID).FirstOrDefault();
            if (verion == null)
            {
                ViewBag.VersionID = "";
                ViewBag.VersionNo = "0";
            }
            else
            {
                ViewBag.VersionID = verion.ID;
                ViewBag.VersionNo = verion.VersionNumber;
            }
            ViewBag.ProjectClass = projectInfo.ProjectClass;
            ViewBag.ProjectLevel = projectInfo.ProjectLevel;
            sql = String.Format(@"select Code,ModeCode,isnull(WBSTypeRoleCode,'') WBSTypeRoleCode,RoleCodes=
STUFF((SELECT ','+RoleCode FROM (select Code,ModeCode,RoleCode from S_T_WBSStructInfo
inner join S_T_ProjectMode on ModeID=S_T_ProjectMode.ID
inner join S_T_WBSStructRole on WBSStructID=S_T_WBSStructInfo.ID
where ModeCode='{0}'
and SychWBS='False') A 
WHERE A.Code=TableInfo.Code FOR XML PATH('')), 1, 1, '')
 from (select Code,ModeCode,
WBSTypeRoleCode=(select top 1 RoleCode from S_T_WBSStructRole where WBSStructID=S_T_WBSStructInfo.ID and SychWBS='True'
and (SychWBSField='' or SychWBSField is null or SychWBSField='ChargeUserID'))
from S_T_WBSStructInfo
inner join S_T_ProjectMode on ModeID=S_T_ProjectMode.ID
where ModeCode='{0}') TableInfo", projectInfo.ModeCode);
            var structDefineDt = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql);
            ViewBag.StructDefineList = JsonHelper.ToJson(structDefineDt);
            return this.View();
        }

        public JsonResult GetWBSTreeList(string ProjectInfoID, string SpaceCode)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息，请确认ID为【" + ProjectInfoID + "】的项目信息存在");
            var sql = String.Format(@"select Code,ModeCode,RoleCodes=
STUFF((SELECT ','+RoleCode FROM (select Code,ModeCode,RoleCode from S_T_WBSStructInfo
inner join S_T_ProjectMode on ModeID=S_T_ProjectMode.ID
inner join S_T_WBSStructRole on WBSStructID=S_T_WBSStructInfo.ID
where ModeCode='{0}'
and SychWBS='{1}') A 
WHERE A.Code=TableInfo.Code FOR XML PATH('')), 1, 1, '')
 from (select Code,ModeCode from S_T_WBSStructInfo
inner join S_T_ProjectMode on ModeID=S_T_ProjectMode.ID
where ModeCode='{0}') TableInfo", projectInfo.ModeCode, false.ToString().ToLower());
            var structDefineDt = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql).AsEnumerable();
            var rbsDt = this.SqlHelper.ExecuteDataTable(@"select WBSID,RoleCode,UserID,UserName from S_W_RBS with(nolock)
where ProjectInfoID='" + ProjectInfoID + "'").AsEnumerable();

            var fo = FormulaHelper.CreateFO<WBSFO>();
            var result = fo.CreateWBSTree(ProjectInfoID, WBSNodeType.Major.ToString(), true, SpaceCode);
            foreach (var item in result)
            {
                if (String.IsNullOrEmpty(item.GetValue("WBSStructCode"))) continue;
                var stRow = structDefineDt.Where(c => c["Code"].ToString() == item.GetValue("VirtualWBSType")).FirstOrDefault();
                if (stRow != null && stRow["RoleCodes"] != null && stRow["RoleCodes"] != DBNull.Value)
                {
                    var roleCodes = stRow["RoleCodes"] == null || stRow["RoleCodes"] == DBNull.Value ? "" : stRow["RoleCodes"].ToString();
                    item.SetValue("RoleCodes", roleCodes);
                    foreach (var roleCode in roleCodes.Split(','))
                    {
                        var rbsList = rbsDt.Where(c => c["WBSID"].ToString() == item.GetValue("VirtualID") && c["RoleCode"].ToString() == roleCode).ToList();
                        var userIDs = String.Join(",", rbsList.Select(c => c["UserID"].ToString()).ToList());
                        var userNames = String.Join(",", rbsList.Select(c => c["UserName"].ToString()).ToList());
                        item.SetValue(roleCode + "UserID", userIDs);
                        item.SetValue(roleCode + "UserName", userNames);
                    }
                }
            }
            return Json(result);
        }

        void setChildNodeCodes(List<S_T_WBSStructInfo> structList, S_T_WBSStructInfo node, ref string canAddChildNodeCodes)
        {
            if (String.IsNullOrEmpty(node.ChildCode))
                return;
            foreach (var item in node.ChildCode.Split(','))
            {
                var structNode = structList.FirstOrDefault(c => c.Code == item);
                if (structNode == null) continue;
                canAddChildNodeCodes += structNode.Code + ",";
                setChildNodeCodes(structList, structNode, ref canAddChildNodeCodes);
            }
        }

        public JsonResult GetMenu(string ID, string ShowDeleteBtn)
        {
            var wbs = this.GetEntityByID<S_W_WBS>(ID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ID + "】的WBS对象");
            if (wbs.StructNodeInfo == null) throw new Formula.Exceptions.BusinessException("WBS节点未定义类别，无法显示菜单");
            var structList = wbs.StructNodeInfo.S_T_ProjectMode.S_T_WBSStructInfo.ToList();
            var childNodeCodes = wbs.StructNodeInfo.ChildCode.Split(',');
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var majorStruct = structList.FirstOrDefault(c => c.Code == WBSNodeType.Major.ToString());
            var canAddChildNodeCodes = "";
            setChildNodeCodes(structList, majorStruct, ref canAddChildNodeCodes);

            foreach (var item in childNodeCodes)
            {
                if (!canAddChildNodeCodes.Split(',').Contains(item))
                {
                    continue;
                }
                var menuItem = new Dictionary<string, object>();
                if (String.IsNullOrEmpty(item)) continue;
                var childStructNode = structList.FirstOrDefault(c => c.Code == item);
                if (childStructNode == null) continue;
                var name = childStructNode.Name;
                menuItem["name"] = item;
                menuItem["text"] = "增加" + name;
                menuItem["iconCls"] = "icon-add";
                menuItem["onClick"] = "addNode";
                var attrDefineList = BaseConfigFO.GetWBSAttrList(item);
                if (attrDefineList.Count > 0)
                    menuItem["attrDefine"] = "true";
                else
                    menuItem["attrDefine"] = "false";
                result.Add(menuItem);
            }
            if (canAddChildNodeCodes.Split(',').Contains(WBSNodeType.Work.ToString()) ||
                canAddChildNodeCodes.Split(',').Contains(WBSNodeType.CooperationPackage.ToString()))
            {
                var menuItem = new Dictionary<string, object>();
                menuItem["name"] = "importPackage";
                menuItem["text"] = "从工作包词典导入";
                menuItem["iconCls"] = "icon-add";
                menuItem["onClick"] = "importPackage";
            }
            if (wbs.WBSType == WBSNodeType.CooperationPackage.ToString())
            {
                var relateItem = new Dictionary<string, object>();
                relateItem["name"] = "relateMileStone";
                relateItem["text"] = "关联计划";
                relateItem["iconCls"] = "icon-edit";
                relateItem["onClick"] = "relateMileStone";
                result.Add(relateItem);
            }
            if (wbs.WBSType != WBSNodeType.Project.ToString() && canAddChildNodeCodes.Split(',').Contains(wbs.StructNodeInfo.Code)
                && ShowDeleteBtn.ToLower() != false.ToString().ToLower())
            {
                var delItem = new Dictionary<string, object>();
                delItem["name"] = "delete";
                delItem["text"] = "删除";
                delItem["iconCls"] = "icon-remove";
                delItem["onClick"] = "delWBS";
                result.Add(delItem);
            }

            string json = JsonHelper.ToJson(result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool ValidateImportPackageButton(string SelectNodes,string ProjectInfoID)
        {
            var rtn = false;
            var project = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if(project==null) throw new Formula.Exceptions.BusinessException("未找到项目对象");
            if (project.ProjectMode == null) throw new Formula.Exceptions.BusinessException("未找到项目的模式信息");
            var structList =  project.ProjectMode.S_T_WBSStructInfo.Where(a=>!string.IsNullOrEmpty(a.ChildCode)
                &&a.ChildCode.Split(',').Contains(WBSNodeType.Work.ToString())).ToList();
            var authWBSTypes = structList.Select(a => a.Code).Distinct().ToList();
            var list = JsonHelper.ToList(SelectNodes);
            if (list.Count == 0)
                return rtn;
            foreach (var item in list)
            {
                var vType = item.GetValue("VirtualWBSType");
                if (!authWBSTypes.Contains(vType))
                    return rtn;
            }
            rtn = true;
            return rtn;
        }

        public JsonResult PackageWBSCopy(string ProjectInfoID, string TargetNodes, string DataSource)
        {
            var targetNodes = JsonHelper.ToList(TargetNodes);
            var sourceNodes = JsonHelper.ToList(DataSource);
            Action action = () =>
            {
                var fo = FormulaHelper.CreateFO<WBSFO>();
                fo.ImportPackageNodes(targetNodes, sourceNodes);
            };
            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                System.Transactions.TransactionOptions tranOp = new System.Transactions.TransactionOptions();
                tranOp.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (System.Transactions.TransactionScope ts =
                    new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, tranOp))
                {
                    action();
                    ts.Complete();
                }
            }
            else
            {
                action();
            }
            return Json("");
        }

        public JsonResult SaveData(string WBSInfo, string ProjectInfoID)
        {
            var list = JsonHelper.ToList(WBSInfo);
            var mileStoneList = this.entities.Set<S_P_MileStone>().Where(a => a.ProjectInfoID == ProjectInfoID).ToList();
            foreach (var item in list)
            {
                var id = item.GetValue("VirtualID");
                if (string.IsNullOrEmpty(id))
                    id = item.GetValue("ID");
                var wbs = this.GetEntityByID<S_W_WBS>(id);
                if (wbs == null) continue;
                if (wbs.WBSType == WBSNodeType.Major.ToString())
                {
                    wbs.ChargeUserID = item.GetValue("ChargeUserID");
                    wbs.ChargeUserName = item.GetValue("ChargeUserName");
                    if (!string.IsNullOrEmpty(item.GetValue("PlanStartDate")))
                        wbs.PlanStartDate = DateTime.Parse(item.GetValue("PlanStartDate"));
                    else
                        wbs.PlanStartDate = null;
                    if (!string.IsNullOrEmpty(item.GetValue("PlanEndDate")))
                        wbs.PlanEndDate = DateTime.Parse(item.GetValue("PlanEndDate"));
                    else
                        wbs.PlanEndDate = null;
                }
                else
                    this.UpdateEntity<S_W_WBS>(wbs, item);
                var mileStoneIDs = JsonHelper.ToList(wbs.RelateMileStone).Select(a => a.GetValue("ID"));
                var mileStones = mileStoneList.Where(a => mileStoneIDs.Contains(a.ID)).ToList();
                DateTime? bStart = DateTime.MinValue, bFinish = DateTime.MaxValue;
                if (mileStones.Count > 0)
                {
                    foreach (var mileStone in mileStones)
                    {
                        if (mileStone.PlanStartDate != null && (mileStone.PlanStartDate > bStart))
                            bStart = mileStone.PlanStartDate;
                        if (mileStone.PlanFinishDate != null && (mileStone.PlanFinishDate < bFinish))
                            bFinish = mileStone.PlanFinishDate;
                    }
                    if (bStart != DateTime.MinValue)
                    {
                        wbs.BasePlanStartDate = bStart;
                        if (wbs.PlanStartDate > wbs.BasePlanStartDate)
                            throw new Formula.Exceptions.BusinessValidationException("【" + wbs.Name + "】节点的计划开始时间不能小于关联里程碑的最大计划开始时间【" + ((DateTime)wbs.BasePlanStartDate).ToShortDateString() + "】。");
                    }
                    if (bFinish != DateTime.MaxValue)
                    {
                        wbs.BasePlanEndDate = bFinish;
                        if (wbs.PlanEndDate > wbs.BasePlanEndDate)
                            throw new Formula.Exceptions.BusinessValidationException("【" + wbs.Name + "】节点的计划完成时间不能大于关联里程碑的最小计划完成时间【" + ((DateTime)wbs.BasePlanEndDate).ToShortDateString() + "】。");
                    }
                }
                else
                {
                    wbs.BasePlanStartDate = null;
                    wbs.BasePlanEndDate = null;
                }
                if (wbs.StructNodeInfo.S_T_WBSStructRole.Count > 0)
                {
                    var roleDefineChange = wbs.StructNodeInfo.S_T_WBSStructRole.FirstOrDefault(c => c.SychWBS == true.ToString()
                        && (string.IsNullOrEmpty(c.SychWBSField) || c.SychWBSField == "ChargeUserID"));
                    if (roleDefineChange != null)
                    {
                        if (!String.IsNullOrEmpty(wbs.ChargeUserID))
                            wbs.SetUser(roleDefineChange.RoleCode, wbs.ChargeUserID);
                        else
                            wbs.RemoveUser(roleDefineChange.RoleCode);
                    }
                    var otherRoleDefines = wbs.StructNodeInfo.S_T_WBSStructRole.Where(c => c.SychWBS != true.ToString()).ToList();
                    foreach (var roleDefine in otherRoleDefines)
                    {
                        var userID = item.GetValue(roleDefine.RoleCode + "UserID");
                        if (!String.IsNullOrEmpty(userID))
                        {
                            wbs.SetUsers(roleDefine.RoleCode, userID.Split(','), false, true, false, true);
                        }
                        else
                            wbs.RemoveUser(roleDefine.RoleCode);
                    }
                }
                wbs.Save(false);
            }
            var project = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            project.ResetOBSUserFromRBS();
            this.entities.SaveChanges();
            return Json("");
        }
        #endregion
    }
}
