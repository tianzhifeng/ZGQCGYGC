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
using System.Reflection;
using EPC.Logic.Domain;

namespace Project.Areas.Basic.Controllers
{
    public class WBSController : ProjectController<S_W_WBS>
    {
        public ActionResult WBS()
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
            //是否必须先进行策划流程
            if (projectInfo.Extention2 == "NeedSchemeForm")
            {
                if (this.entities.Set<T_SC_PetrifactionProjectScheme>().Any(a => a.ProjectInfoID == projectInfo.ID && a.FlowPhase == "End"))
                    ViewBag.NeedSchemeForm = false;
                else
                    ViewBag.NeedSchemeForm = true;
            }
            else
                ViewBag.NeedSchemeForm = false;
            ViewBag.IsSimple = false;
            var isSimple = GetQueryString("IsSimple");
            if (isSimple.ToLower() == "true")
                ViewBag.IsSimple = false;
            return this.View();
        }

        public ActionResult WBSAddWithAttrDefine()
        {
            string childType = this.Request["Type"];
            ViewBag.NameTitle = EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), childType);
            if (String.IsNullOrEmpty(childType)) throw new Formula.Exceptions.BusinessException("未指定需要增加的WBS节点类型，无法增加节点");

            //项目策划-专业、阶段根据策划及业务类型过滤
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            var project = this.entities.Set<S_I_ProjectInfo>().Find(projectInfoID);
            var prjClass = string.Empty;
            if (project != null)
                prjClass = project.ProjectClass;
            var list = BaseConfigFO.GetWBSEnum(childType, prjClass);
            ViewBag.DefineAttr = JsonHelper.ToJson(list);
            var allDefineAttr = BaseConfigFO.GetWBSEnum(childType, "");
            ViewBag.AllDefineAttr = JsonHelper.ToJson(allDefineAttr);
            var prjClassDt = EnumBaseHelper.GetEnumTable("Base.ProjectClass");
            var projectClassRows = prjClassDt.Select("value='" + prjClass + "'");
            if (projectClassRows != null && projectClassRows.Count() > 0)
                ViewBag.ProjectClassName = projectClassRows[0]["text"].ToString();
            else
                ViewBag.ProjectClassName = "";
            ViewBag.WBSType = childType;


            return this.View();
        }

        public ActionResult WBSAdd()
        {
            string childType = this.Request["Type"];
            if (String.IsNullOrEmpty(childType)) throw new Formula.Exceptions.BusinessException("未指定需要增加的WBS节点类型，无法增加节点");
            var list = BaseConfigFO.GetWBSAttrList(childType);
            if (list.Count > 0)
                this.ViewBag.IsAttrDefine = true.ToString();
            else
                this.ViewBag.IsAttrDefine = false.ToString();
            return this.View();
        }

        public JsonResult GetWBSTreeList(string ProjectInfoID, string IncludeWorkTask)
        {
            var includeTask = true;
            if (!String.IsNullOrEmpty(IncludeWorkTask) && IncludeWorkTask.ToLower() == false.ToString().ToLower())
                includeTask = false;
            if (String.IsNullOrEmpty(ProjectInfoID)) throw new Formula.Exceptions.BusinessException("参数ProjectInfoID不能为空");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息，请确认ID为【" + ProjectInfoID + "】的项目信息存在");
            var sql = String.Format(@"select Code,ModeCode,isnull(WBSTypeRoleCode,'') WBSTypeRoleCode,RoleCodes=
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
            var structDefineDt = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql).AsEnumerable();
            sql = "select * from S_W_WBS WHERE ProjectInfoID='" + ProjectInfoID + "'";
            if (!includeTask)
            {
                sql += " and WBSType  not in ('" + WBSNodeType.Work.ToString() + "','" + WBSNodeType.CooperationPackage.ToString() + "')";
            }
            sql += " order by sortindex ";
            var rbsDt = this.SqlHelper.ExecuteDataTable(@"select WBSID,RoleCode,UserID,UserName from S_W_RBS with(nolock)
where ProjectInfoID='" + ProjectInfoID + "'").AsEnumerable();
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            var result = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dic = FormulaHelper.DataRowToDic(row);
                var stRow = structDefineDt.Where(c => c["Code"].ToString() == row["WBSStructCode"].ToString()).FirstOrDefault();
                if (stRow != null && stRow["RoleCodes"] != null && stRow["RoleCodes"] != DBNull.Value)
                {
                    var roleCodes = stRow["RoleCodes"] == null || stRow["RoleCodes"] == DBNull.Value ? "" : stRow["RoleCodes"].ToString();
                    dic.SetValue("RoleCodes", roleCodes);
                    dic.SetValue("WBSTypeRoleCode", stRow["WBSTypeRoleCode"].ToString());
                    foreach (var roleCode in roleCodes.Split(','))
                    {
                        var rbsList = rbsDt.Where(c => c["WBSID"].ToString() == row["ID"].ToString() && c["RoleCode"].ToString() == roleCode).ToList();
                        var userIDs = String.Join(",", rbsList.Select(c => c["UserID"].ToString()).ToList());
                        var userNames = String.Join(",", rbsList.Select(c => c["UserName"].ToString()).ToList());
                        dic.SetValue(roleCode + "UserID", userIDs);
                        dic.SetValue(roleCode + "UserName", userNames);
                    }
                }
                result.Add(dic);
            }
            return Json(result);
        }

        public JsonResult GetMenu(string ID, string ShowDeleteBtn)
        {
            var wbs = this.GetEntityByID<S_W_WBS>(ID);

            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ID + "】的WBS对象");
            if (wbs.StructNodeInfo == null) throw new Formula.Exceptions.BusinessException("WBS节点未定义类别，无法显示菜单");
            var structList = wbs.StructNodeInfo.S_T_ProjectMode.S_T_WBSStructInfo.ToList();
            var childNodeCodes = wbs.StructNodeInfo.ChildCode.Split(',');
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            foreach (var item in childNodeCodes)
            {
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
            if (wbs.WBSType == WBSNodeType.CooperationPackage.ToString())
            {
                var relateItem = new Dictionary<string, object>();
                relateItem["name"] = "relateMileStone";
                relateItem["text"] = "关联计划";
                relateItem["iconCls"] = "icon-edit";
                relateItem["onClick"] = "relateMileStone";
                result.Add(relateItem);
            }
            if (wbs.WBSType != WBSNodeType.Project.ToString() && ShowDeleteBtn.ToLower() != false.ToString().ToLower())
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

        public JsonResult AddChild(string parentIDs, string Children, string WBSType)
        {
            var result = new List<S_W_WBS>();
            var list = JsonHelper.ToList(Children);
            var majorType = WBSNodeType.Major.ToString();
            var majorWBSAttrList = BaseConfigFO.GetWBSAttrList(majorType);
            //EPCWBS信息
            var epcEntities = FormulaHelper.GetEntities<EPCEntities>();

            foreach (var parentID in parentIDs.TrimEnd(',').Split(','))
            {
                var parent = this.GetEntityByID<S_W_WBS>(parentID);
                if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + parentID + "】的WBS节点，无法增加子节点");

                var epcParentWBSNode = epcEntities.Set<S_I_WBS>().Find(parentID);
                if (epcParentWBSNode == null)
                    throw new Formula.Exceptions.BusinessException("未能找到ID为【" + parentID + "】的WBS节点，无法增加子节点");

                var rbsList = parent.S_W_RBS.ToList();
                foreach (var item in list)
                {
                    var wbs = new S_W_WBS();
                    this.UpdateEntity<S_W_WBS>(wbs, item);
                    var subEntityNode = parent.AddChild(wbs, true, true);
                    result.Add(subEntityNode);

                    var epcWBS = new S_I_WBS();
                    this.UpdateEntity<S_I_WBS>(epcWBS, item);
                    epcWBS.NodeType = WBSNodeType.SubProject.ToString();
                    epcWBS.ID = wbs.ID;
                    var epcSubProjectNode = epcParentWBSNode.AddChild(epcWBS, true, false);

                    var majorCodes = item.GetValue("MajorCode");
                    if (!string.IsNullOrWhiteSpace(majorCodes))
                    {
                        foreach (var majorCode in majorCodes.Split(','))
                        {
                            var majorAttrDefine = majorWBSAttrList.FirstOrDefault(d => d.Code == majorCode);
                            if (majorAttrDefine == null) continue;

                            var majorWBSNode = new S_W_WBS();
                            majorWBSNode.WBSType = majorType;
                            majorWBSNode.Name = majorAttrDefine.Name;
                            majorWBSNode.SortIndex = majorAttrDefine.SortIndex;
                            majorWBSNode.WBSValue = majorCode;
                            result.Add(subEntityNode.AddChild(majorWBSNode));

                            var epcMajorWBSNode = new S_I_WBS();
                            epcMajorWBSNode.NodeType = majorType;
                            epcMajorWBSNode.Name = majorAttrDefine.Name;
                            epcMajorWBSNode.SortIndex = majorAttrDefine.SortIndex;
                            epcMajorWBSNode.Value = majorCode;
                            epcMajorWBSNode.ID = majorWBSNode.ID;
                            epcSubProjectNode.AddChild(epcMajorWBSNode,true,true);

                        }

                    }

                    if (!string.IsNullOrEmpty(parent.MajorCode))
                    {
                        if (wbs.StructNodeInfo.S_T_WBSStructRole.Count > 0)
                        {
                            var roleDefines = wbs.StructNodeInfo.S_T_WBSStructRole.Where(c => c.SychWBS != true.ToString()).ToList();
                            foreach (var roleDefine in roleDefines)
                            {
                                var users = rbsList.Where(a => a.RoleCode == roleDefine.RoleCode).Select(a => a.UserID).ToArray();
                                if (users.Length > 0)
                                    wbs.SetUsers(roleDefine.RoleCode, users, false, true, false, true);
                            }
                        }
                    }
                }
            }
            this.entities.SaveChanges();
            epcEntities.SaveChanges();
            return Json(result);
        }

        public JsonResult AddChildWithDefAttr(string Children)
        {
            var dic = JsonHelper.ToObject(Children);
            string parentIDs = dic["ParentIDs"].ToString();
            string children = string.Empty;
            if (dic.ContainsKey("childNodes"))
                children = dic["childNodes"].ToString();
            if (dic.ContainsKey("allChildNodes"))
                children = dic["allChildNodes"].ToString();
            string type = dic["Type"].ToString();
            var list = BaseConfigFO.GetWBSAttrList(type);
            var result = new List<S_W_WBS>();
            //EPCWBS信息
            var epcEntities = FormulaHelper.GetEntities<EPCEntities>();

            foreach (var parentID in parentIDs.TrimEnd(',').Split(','))
            {
                var parent = this.GetEntityByID<S_W_WBS>(parentID);
                if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + parentID + "】的WBS节点，无法增加子节点");
                var epcParentWBSNode = epcEntities.Set<S_I_WBS>().Find(parentID);
                if (epcParentWBSNode == null)
                    throw new Formula.Exceptions.BusinessException("未能找到ID为【" + parentID + "】的WBS节点，无法增加子节点");
                foreach (var item in children.Split(','))
                {
                    var attrDefine = list.FirstOrDefault(d => d.Code == item);
                    if (attrDefine == null) continue;
                    var wbs = new S_W_WBS();
                    wbs.WBSType = dic["Type"].ToString();
                    wbs.Name = attrDefine.Name;
                    wbs.SortIndex = attrDefine.SortIndex;
                    wbs.WBSValue = item;
                    result.Add(parent.AddChild(wbs));
                    var epcWBS = new S_I_WBS();
                    epcWBS.NodeType = dic["Type"].ToString();
                    epcWBS.Name = attrDefine.Name;
                    epcWBS.SortIndex = attrDefine.SortIndex;
                    epcWBS.Value = item;
                    epcWBS.ID = wbs.ID;
                    epcParentWBSNode.AddChild(epcWBS,true,true);
                }
            }
            this.entities.SaveChanges();
            epcEntities.SaveChanges();
            return Json(result);
        }

        public JsonResult SaveData(string WBSInfo, string ProjectInfoID)
        {
            var list = JsonHelper.ToList(WBSInfo);
            var mileStoneList = this.entities.Set<S_P_MileStone>().Where(a => a.ProjectInfoID == ProjectInfoID).ToList();
            var coopPlanList = this.entities.Set<S_P_CooperationPlan>().Where(a => a.ProjectInfoID == ProjectInfoID).ToList();
            foreach (var item in list)
            {
                var wbs = this.GetEntityByID<S_W_WBS>(item.GetValue("ID"));
                if (wbs == null) continue;
                S_P_CooperationPlan oldPlan = null;
                var oldMileStone = JsonHelper.ToList(wbs.RelateMileStone).FirstOrDefault();
                if (oldMileStone != null)
                    oldPlan = coopPlanList.FirstOrDefault(a => a.MileStoneID == oldMileStone.GetValue("ID"));
                this.UpdateEntity<S_W_WBS>(wbs, item);
                var mileStoneIDs = JsonHelper.ToList(wbs.RelateMileStone).Select(a => a.GetValue("ID"));
                var mileStones = mileStoneList.Where(a => mileStoneIDs.Contains(a.ID)).ToList();
                DateTime? bStart = DateTime.MinValue, bFinish = DateTime.MaxValue;
                if (oldPlan != null)
                {
                    oldPlan.WBSID = wbs.RootNode.ID;
                    oldPlan.WBSFullID = wbs.RootNode.FullID;
                }
                if (mileStones.Count > 0)
                {
                    foreach (var mileStone in mileStones)
                    {
                        if (mileStone.PlanStartDate != null && (mileStone.PlanStartDate > bStart))
                            bStart = mileStone.PlanStartDate;
                        if (mileStone.PlanFinishDate != null && (mileStone.PlanFinishDate < bFinish))
                            bFinish = mileStone.PlanFinishDate;
                        var newPlan = coopPlanList.FirstOrDefault(a => a.MileStoneID == mileStone.ID);
                        if (newPlan != null)
                        {
                            newPlan.WBSID = wbs.ID;
                            newPlan.WBSFullID = wbs.FullID;
                        }
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
            var majors = project.S_W_WBS.Where(a => a.WBSType == WBSNodeType.Major.ToString()).Select(a => a.WBSValue).Distinct();
            project.SynchMajorData(string.Join(",", majors));
            project.ResetOBSUserFromRBS();
            if (project.State == ProjectCommoneState.Plan.ToString())
                project.State = ProjectCommoneState.Execute.ToString();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var result = new Dictionary<string, object>();
            var sourceNode = this.GetEntityByID<S_W_WBS>(sourceID);
            if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的节点，无法移动");
            if (dragAction.ToLower() == "before")
            {
                var target = this.GetEntityByID<S_W_WBS>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                this.entities.Set<S_W_WBS>().Where(c => c.ParentID == target.ParentID && c.ProjectInfoID == sourceNode.ProjectInfoID
                   && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 0.001);
                sourceNode.SortIndex = target.SortIndex - 0.001;
            }
            else if (dragAction.ToLower() == "after")
            {
                var target = this.GetEntityByID<S_W_WBS>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");
                this.entities.Set<S_W_WBS>().Where(c => c.ParentID == target.ParentID && c.ProjectInfoID == sourceNode.ProjectInfoID
                  && c.SortIndex > target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 0.001);
                sourceNode.SortIndex = target.SortIndex + 0.001;
            }
            result = FormulaHelper.ModelToDic<S_W_WBS>(sourceNode);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportPackage(string DataSource, string TargetID, string ProjectInfoID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            var list = JsonHelper.ToList(DataSource);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ProjectInfoID + "】的项目对象，导入工作包失败");
            var majorList = projectInfo.GetMajorList();
            if (majorList.Count == 0) throw new Formula.Exceptions.BusinessException("没有策划专业节点，无法进行工作包导入操作");
            if (String.IsNullOrEmpty(TargetID))
            {
                foreach (var item in list)
                {
                    string majorValue = item.GetValue("MajorCode");
                    var wbslist = majorList.Where(d => d.WBSValue == majorValue).ToList();
                    foreach (var wbs in wbslist)
                    {
                        var task = new S_W_TaskWork();
                        task.MajorValue = majorValue;
                        task.Name = item.GetValue("Name");
                        task.Code = item.GetValue("Code");
                        if (!String.IsNullOrEmpty(item.GetValue("WorkLoad")))
                            task.Workload = Convert.ToDecimal(item.GetValue("WorkLoad"));
                        task.PhaseValue = item.GetValue("PhaseCode");
                        wbs.AddTaskWork(task);
                    }
                }
            }
            else
            {
                var wbs = this.GetEntityByID<S_W_WBS>(TargetID);
                if (wbs == null)
                    throw new Formula.Exceptions.BusinessException("未能找到ID为【" + TargetID + "】的WBS节点，导入工作包失败");
                foreach (var item in list)
                {
                    string majorValue = item.GetValue("MajorCode");
                    var task = new S_W_TaskWork();
                    task.MajorValue = majorValue;
                    task.Name = item.GetValue("Name");
                    task.Code = item.GetValue("Code");
                    if (!String.IsNullOrEmpty(item.GetValue("WorkLoad")))
                        task.Workload = Convert.ToDecimal(item.GetValue("WorkLoad"));
                    task.PhaseValue = item.GetValue("PhaseCode");
                    wbs.AddTaskWork(task);
                }
            }
            this.entities.SaveChanges();
            return Json("");
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

        public JsonResult PublishWBSBulk(string ProjectInfoID)
        {
            var projectDt = this.SqlHelper.ExecuteDataTable(String.Format("select * from S_I_ProjectInfo where ID='{0}'", ProjectInfoID));
            if (projectDt.Rows.Count == 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定ID为【" + ProjectInfoID + "】的项目信息，请联系管理员");
            }
            var versionDic = new Dictionary<string, object>();
            Action action = () =>
            {
                var versionDt = this.SqlHelper.ExecuteDataTable("select * from S_W_WBS_Version where ProjectInfoID='" + ProjectInfoID + "' and FlowPhase!='End'");
                if (versionDt.Rows.Count > 0)
                {
                    //如果之前存在一个临时版本（流程未完成的版本），则删除该版本下所有节点
                    this.SqlHelper.ExecuteNonQuery("delete from S_W_WBS_Version_Node where VersionID='" + versionDt.Rows[0]["ID"] + "'");
                    versionDic = FormulaHelper.DataRowToDic(versionDt.Rows[0]);
                }
                else
                {
                    versionDic.SetValue("ID", FormulaHelper.CreateGuid());
                    versionDic.SetValue("FlowPhase", "Start");
                    versionDic.SetValue("CreateDate", DateTime.Now);
                    versionDic.SetValue("CreateUser", this.CurrentUserInfo.UserName);
                    versionDic.SetValue("CreateUserID", this.CurrentUserInfo.UserID);
                    versionDic.SetValue("ProjectInfoID", ProjectInfoID);
                    versionDic.SetValue("ProjectInfoName", projectDt.Rows[0]["Name"]);
                    versionDic.SetValue("ProjectInfo", ProjectInfoID);
                    versionDic.SetValue("ProjectInfoCode", projectDt.Rows[0]["Code"]);
                    var maxVersion = Convert.ToInt32(
                    this.SqlHelper.ExecuteScalar("select isnull(Max(VersionNumber),0)+1 from S_W_WBS_Version where ProjectInfoID='" + ProjectInfoID + "'"));
                    versionDic.SetValue("VersionNumber", maxVersion);
                    versionDic.SetValue("VersionName", projectDt.Rows[0]["Name"] + "第【" + maxVersion + "】版WBS计划");
                    versionDic.InsertDB(this.SqlHelper, "S_W_WBS_Version", versionDic.GetValue("ID"));
                }
                S_W_WBS_Version.UpgradeVersion(versionDic);
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
            return Json(new { FormID = versionDic.GetValue("ID") });
        }

        public JsonResult VersionRollBack(string VersionID)
        {

            var versionNodeDt = this.SqlHelper.ExecuteDataTable(String.Format("select * from S_W_WBS_Version_Node where ID='{0}'", VersionID));
            if (versionNodeDt.Rows.Count == 0)
            {
                throw new Exception("没有找到对应的版本WBS数据，无法进行回滚");
            }
            var versionDic = new Dictionary<string, object>();
            Action action = () =>
            {


                //if (versionNodeDt.Rows.Count > 0)
                //{
                //    //如果之前存在一个临时版本（流程未完成的版本），则删除该版本下所有节点
                //    this.SqlHelper.ExecuteNonQuery("delete from S_W_WBS_Version_Node where VersionID='" + versionNodeDt.Rows[0]["ID"] + "'");
                //    versionDic = FormulaHelper.DataRowToDic(versionNodeDt.Rows[0]);
                //}
                //else
                //{
                //    versionDic.SetValue("ID", FormulaHelper.CreateGuid());
                //    versionDic.SetValue("FlowPhase", "Start");
                //    versionDic.SetValue("CreateDate", DateTime.Now);
                //    versionDic.SetValue("CreateUser", this.CurrentUserInfo.UserName);
                //    versionDic.SetValue("CreateUserID", this.CurrentUserInfo.UserID);
                //    versionDic.SetValue("ProjectInfoID", ProjectInfoID);
                //    versionDic.SetValue("ProjectInfoName", projectDt.Rows[0]["Name"]);
                //    versionDic.SetValue("ProjectInfo", ProjectInfoID);
                //    versionDic.SetValue("ProjectInfoCode", projectDt.Rows[0]["Code"]);
                //    var maxVersion = Convert.ToInt32(
                //    this.SqlHelper.ExecuteScalar("select isnull(Max(VersionNumber),0)+1 from S_W_WBS_Version where ProjectInfoID='" + ProjectInfoID + "'"));
                //    versionDic.SetValue("VersionNumber", maxVersion);
                //    versionDic.SetValue("VersionName", projectDt.Rows[0]["Name"] + "第【" + maxVersion + "】版WBS计划");
                //    versionDic.InsertDB(this.SqlHelper, "S_W_WBS_Version", versionDic.GetValue("ID"));
                //}
                //S_W_WBS_Version.UpgradeVersion(versionDic);
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
            return Json(new { FormID = versionDic.GetValue("ID") });
        }

        public JsonResult WBSCopy(string ProjectInfoID, string TargetInfo, string DataSource)
        {
            var targetNode = JsonHelper.ToObject(TargetInfo);
            var sourceNodes = JsonHelper.ToList(DataSource);
            Action action = () =>
            {
                var fo = FormulaHelper.CreateFO<WBSFO>();
                fo.ImportWBSNodes(targetNode, sourceNodes);
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

        public JsonResult GetProjectList(QueryBuilder qb)
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null)
            {
                var gridData = this.entities.Set<S_I_ProjectInfo>().WhereToGridData(qb);
                return Json(gridData);
            }
            else
            {
                var gridData = this.entities.Set<S_I_ProjectInfo>().Where(d => d.ModeCode == projectInfo.ModeCode && d.ID != projectInfoID).WhereToGridData(qb);
                return Json(gridData);
            }
        }

        public JsonResult GetTemplateList(QueryBuilder qb)
        {
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("");
            var entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var data = entities.S_D_WBSTemplate.Where(d => d.ModeCodes.Contains(projectInfo.ModeCode)).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetTemplateTree(string TemplateID)
        {
            //var entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            //var data = entities.S_D_WBSTemplateNode.Where(d => d.TemplateID == TemplateID).OrderBy(d => d.SortIndex).ToList();
            //return Json(data);
            SQLHelper configDB = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            var sql = @"select WBSType WBSStructCode,* from S_D_WBSTemplateNode where TemplateID='" + TemplateID + "' order by SortIndex";
            var dt = configDB.ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult ImportStandardWBS(string TargetInfo, string DataSource)
        {
            var entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var targetNode = JsonHelper.ToObject(TargetInfo);
            var dataSource = JsonHelper.ToList(DataSource);
            var targetWBS = this.GetEntityByID<S_W_WBS>(targetNode["ID"].ToString());
            if (targetWBS == null) throw new Formula.Exceptions.BusinessException("目标节点未找到，无法导入WBS");
            var top = dataSource.Min(d => Convert.ToInt32(d["Level"]));
            if (top == 0) top++; //如果全部选中项目节点也会在选中行里中，此处就需要获取所有的项目下层节点，level 统一降一级
            var topNodes = dataSource.Where(d => Convert.ToInt32(d["Level"]) == top).ToList();
            foreach (var item in topNodes)
            {
                var templateID = item.GetValue("ID");
                var topWBS = entities.S_D_WBSTemplateNode.SingleOrDefault(d => d.ID == templateID);
                if (topWBS == null) continue;
                targetWBS.ImportTemplateNode(topWBS);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        #region User

        public JsonResult ImportUserFormOBS(string ProjectInfoID, string UserList, string WBSList)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ProjectInfoID + "】的项目信息对象，无法导入OBS人员");
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
                        wbs.SetUser(roleCode, user.GetValue("UserID"));
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 参考中冶长天策划WBS

        public JsonResult GetWBSTree(string ProjectInfoID)
        {

            if (String.IsNullOrEmpty(ProjectInfoID)) throw new Formula.Exceptions.BusinessException("参数ProjectInfoID不能为空");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息，请确认ID为【" + ProjectInfoID + "】的项目信息存在");

            var sql = "select * from S_W_WBS WHERE ProjectInfoID='" + ProjectInfoID + "'  order by sortindex ";
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult GetSubProjectList(string projectInfoID, string parentID)
        {

            string sql = @"select * from S_W_WBS where ProjectInfoID ='{0}' and ParentID='{1}'";
            sql = string.Format(sql, projectInfoID, parentID);
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            return Json(dt);
        }



        #endregion

    }
}
