using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Reflection;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Config;
using Project.Logic.Domain;
using Project.Logic;
using Config.Logic;

namespace Project.Areas.Monitor.Controllers
{
    public class StructViewController : ProjectController
    {

        public ActionResult WBSStructView()
        {
            string wbsType = WBSNodeType.Project.ToString();
            string sql = "select distinct [Type] from S_D_WBSAttrDefine ";
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
                wbsType += row["Type"].ToString() + ",";
            ViewBag.lockedWBSType = wbsType.TrimEnd(',');
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            var transform = projectInfo.ProjectMode.S_T_WBSStructInfo.Where(d => d.CanTransform == true.ToString()
                && d.Code != WBSNodeType.Work.ToString() && d.Code != WBSNodeType.Project.ToString()).
                Select(c => new { value = c.Code, text = "按" + c.Name, index = 1 }).ToList();
            transform.Add(new { value = "Project", text = "默认", index = 0 });
            ViewBag.TransForm = JsonHelper.ToJson(transform.OrderBy(d => d.index).ToList());
            var rootStruct = projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Project.ToString());
            ViewBag.DefaultViewType = rootStruct.Code + "," + rootStruct.ChildCode;

            ViewBag.VirtualScroll = "false";
            var wbsCount = this.SqlHelper.ExecuteScalar("select count(ID) from S_W_WBS WHERE ProjectInfoID='" + projectInfoID + "'");
            if (Convert.ToInt32(wbsCount) > 300)
            {
                //大于300使用虚加载，需要更换TREEGIRD样式
                ViewBag.VirtualScroll = "true";
            }
            return this.View();
        }

        public JsonResult GetTree()
        {
            var projectInfoID = this.Request["ProjectInfoID"];
            string viewType = String.IsNullOrEmpty(this.Request["ViewType"]) ? WBSNodeType.Project.ToString() : this.Request["ViewType"];
            var projectFO = FormulaHelper.CreateFO<ProjectInfoFO>();
            var result = projectFO.GetWBSTree(projectInfoID, viewType, true, "", false);
            var activityList = this.entities.Set<S_W_Activity>().Where(d => d.ProjectInfoID == projectInfoID && d.ActivityKey != "DesignTask").OrderBy(d => d.ID).ToList();
            foreach (var item in activityList)
            {
                var activity = item.ToDic();
                var parentDic = result.FirstOrDefault(delegate(Dictionary<string, object> dic)
                {
                    if (String.IsNullOrEmpty(dic.GetValue("VirtualID"))) return false;
                    if (String.IsNullOrEmpty(dic.GetValue("ID"))) return false;
                    if (dic.GetValue("ID") == item.WBSID) return true;
                    return false;
                });
                if (parentDic == null)
                {
                    activity.SetValue("ParentID", item.WBSID);
                    var parentWBS = result.FirstOrDefault(a => a.GetValue("ID") == item.WBSID);
                    if (parentWBS == null)
                    {
                        parentWBS = result.FirstOrDefault(a => a.GetValue("VirtualID") == item.WBSID);
                        if (parentWBS == null)
                            continue;
                        else
                        {
                            var fullID = parentWBS.GetValue("VirtualFullID") + "." + activity.GetValue("ID");
                            activity.SetValue("VirtualFullID", fullID);
                        }
                    }
                    else
                    {
                        var fullID = parentWBS.GetValue("FullID") + "." + activity.GetValue("ID");
                        activity.SetValue("FullID", fullID);
                    }
                }
                else
                {
                    activity.SetValue("ParentID", parentDic.GetValue("VirtualID"));
                    var fullID = parentDic.GetValue("VirtualFullID") + "." + activity.GetValue("ID");
                    activity.SetValue("VirtualFullID", fullID);
                }
                //activity.SetValue("WBSType", activity.GetValue("ActivityKey"));
                activity.SetValue("VirtualID", activity.GetValue("ID"));
                activity.SetValue("WBSType", "");
                activity.SetValue("Name", activity.GetValue("ActvityName"));
                activity.SetValue("ChargeUserID", activity.GetValue("OwnerUserID"));
                activity.SetValue("ChargeUserName", activity.GetValue("OwnerUserName"));
                if (!String.IsNullOrEmpty(activity.GetValue("CreateDate")))
                    activity.SetValue("FactStartDate", Convert.ToDateTime(activity.GetValue("CreateDate")));
                if (!String.IsNullOrEmpty(activity.GetValue("FinishDate")))
                    activity.SetValue("FactEndDate", Convert.ToDateTime(activity.GetValue("CreateDate")));
                result.Add(activity);
            }
            var orderField = viewType == WBSNodeType.Project.ToString() ? "FullID" : "VirtualFullID";
            result = result.OrderBy(a => a.GetValue(orderField)).ThenBy(a => a.GetValue("ID")).ToList();
            return Json(result);
        }

        public JsonResult GetWBSTreeView(string ProjectInfoID)
        {
            var sql = @"select * from (select ID,Name,WBSType,Code,ParentID,WBSValue,SortIndex,ChargeUserID,ChargeUserName,PlanStartDate,PlanEndDate,
FactStartDate,FactEndDate,ProjectInfoID  from dbo.S_W_WBS
union
select ID,ActvityName as Name,'Activity' as WBSType,'' as Code,WBSID as ParentID,
ActivityKey as WBSValue,0 as SortIndex,OwnerUserID as ChargeUserID,OwnerUserName as ChargeUserName,
null as PlanStartDate,null as PlanEndDate,null as FactStartDate, FinishDate as FactEndDate,
ProjectInfoID
 from dbo.S_W_Activity) tableInfo
where ProjectInfoID='{0}'";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, ProjectInfoID));

            return Json(dt);
        }

        public JsonResult LazyLoadTree(string ProjectInfoID, string id)
        {
            var sql = String.Format(@"select * from (select ID,Name,WBSType,Code,ParentID,WBSValue,SortIndex,ChargeUserID,ChargeUserName,PlanStartDate,PlanEndDate,
FactStartDate,FactEndDate,ProjectInfoID  from dbo.S_W_WBS
union
select ID,ActvityName as Name,'Activity' as WBSType,'' as Code,WBSID as ParentID,
ActivityKey as WBSValue,0 as SortIndex,OwnerUserID as ChargeUserID,OwnerUserName as ChargeUserName,
null as PlanStartDate,null as PlanEndDate,null as FactStartDate, FinishDate as FactEndDate,
ProjectInfoID
 from dbo.S_W_Activity) tableInfo  where ProjectInfoID='{0}'", ProjectInfoID);
            var allNodes = this.SqlHelper.ExecuteDataTable(sql);
            if (!String.IsNullOrEmpty(id))
                sql += " and ParentID='" + id + "'";
            else
                sql += " and (ParentID is null or ParentID ='')";
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            var list = FormulaHelper.DataTableToListDic(dt);


            foreach (var item in list)
            {
                if (allNodes.Select("ParentID='" + item.GetValue("ID") + "'").Length > 0)
                {
                    item.SetValue("isLeaf", false);
                    item.SetValue("expanded", false);
                }
            }
            return Json(list);
        }

        public JsonResult GetWBSUserInfo(string WBSID)
        {
            var wbs = this.GetEntityByID<S_W_WBS>(WBSID);
            if (wbs == null || wbs.StructNodeInfo == null) return Json("");
            var result = new List<Dictionary<string, object>>();
            foreach (var item in wbs.StructNodeInfo.S_T_WBSStructRole.OrderBy(d => d.SortIndex).ToList())
            {
                var user = new Dictionary<string, object>();
                user["WBSID"] = WBSID;
                user["RoleCode"] = item.RoleCode;
                user["RoleName"] = item.RoleName;
                string userID = string.Empty, userName = string.Empty;
                string userDeptID = string.Empty, userDeptName = string.Empty;
                foreach (var rbs in wbs.S_W_RBS.Where(d => d.RoleCode == item.RoleCode))
                {
                    userID += rbs.UserID + ",";
                    userName += rbs.UserName + ",";
                    userDeptID += rbs.UserDeptID + ",";
                    userDeptName += rbs.UserDeptName + ",";
                }
                user["UserID"] = userID.TrimEnd(',');
                user["UserName"] = userName.TrimEnd(',');
                user.SetValue("UserDeptID", userDeptID.TrimEnd(','));
                user.SetValue("UserDeptName", userDeptName.TrimEnd(','));
                result.Add(user);
            }
            var gridData = new GridData(result);
            return Json(gridData);
        }
    }
}
