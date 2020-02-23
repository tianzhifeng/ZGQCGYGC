using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic;
using EPC.Logic.Domain;

namespace EPC.Areas.Design.Controllers
{
    public class CooperationController : EPCController
    {
        public JsonResult GetCooperationTree(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            var list = this.entities.Set<S_I_WBS>().Where(c => c.EngineeringInfoID == EngineeringInfoID && c.NodeType == "Major").ToList();
            var result = new List<Dictionary<string, object>>();
            var obsUserList = this.entities.Set<S_I_OBS_User>().Where(c => c.EngineeringInfoID == engineeringInfo.ID && !String.IsNullOrEmpty(c.MajorCode)).ToList();
            foreach (var item in list)
            {
                if (result.Exists(c => c.GetValue("ID") == item.ID))
                    continue;
                var dic = this.fillDicWithWBS(item);
                if (obsUserList.Exists(c => c.MajorCode == item.Value && c.UserID == this.CurrentUserInfo.UserID))
                {
                    dic.SetValue("HasAuth", true.ToString());
                }
                else
                {
                    dic.SetValue("HasAuth", false.ToString());
                }
                result.Add(dic);
                foreach (var node in item.Ancestor)
                {
                    if (result.Exists(c => c.GetValue("ID") == node.ID) || node.NodeType == "Root")
                        continue;
                    dic = this.fillDicWithWBS(node);
                    result.Add(dic);
                }
            }
            result = result.OrderBy(c => c["SortIndex"]).ToList();
            return Json(result);
        }

        Dictionary<string, object> fillDicWithWBS(S_I_WBS item)
        {
            var dic = new Dictionary<string, object>();
            dic.SetValue("ID", item.ID);
            dic.SetValue("ParentID", item.ParentID);
            dic.SetValue("EngineeringInfoID", item.EngineeringInfoID);
            dic.SetValue("FullID", item.FullID);
            dic.SetValue("Value", item.Value);
            dic.SetValue("Name", item.Name);
            dic.SetValue("WBSID", item.ID);
            dic.SetValue("NodeType", item.NodeType);
            dic.SetValue("SortIndex", item.SortIndex);
            dic.SetValue("TaskID", "");
            if (item.S_I_Engineering.S_R_Resource.Count(c => c.WBSID == item.ID && c.ResourceID == this.CurrentUserInfo.UserID
                 && !String.IsNullOrEmpty(c.RoleCode) && String.IsNullOrEmpty(c.TaskID)) > 0)
            {
                dic.SetValue("InUser", true.ToString());
                var roleCodes = item.S_I_Engineering.S_R_Resource.Where(c => c.WBSID == item.ID && c.ResourceID == this.CurrentUserInfo.UserID &&
                     !String.IsNullOrEmpty(c.RoleCode) && String.IsNullOrEmpty(c.TaskID)).Select(c => c.RoleCode).ToList();
                dic.SetValue("RoleCode", String.Join(",", roleCodes));
            }
            else
            {
                dic.SetValue("InUser", false.ToString());
                dic.SetValue("RoleCode", "");
            }
            return dic;
        }

        Dictionary<string, object> fillDicWithTask(S_I_WBS_Task item)
        {
            var dic = new Dictionary<string, object>();
            dic.SetValue("ID", item.ID);
            dic.SetValue("ParentID", item.ParentID);
            dic.SetValue("EngineeringInfoID", item.EngineeringInfoID);
            dic.SetValue("FullID", item.WBSFullID);
            dic.SetValue("Value", item.Code);
            dic.SetValue("Name", item.Name);
            dic.SetValue("WBSID", item.ParentID);
            dic.SetValue("NodeType", item.TaskType);
            dic.SetValue("SortIndex", item.SortIndex);
            dic.SetValue("TaskID", item.ID);
            if (item.S_I_Engineering.S_R_Resource.Count(c => c.TaskID == item.ID && c.ResourceID == this.CurrentUserInfo.UserID && !String.IsNullOrEmpty(c.RoleCode)) > 0)
            {
                dic.SetValue("InUser", true.ToString());
                var roleCodes = item.S_I_Engineering.S_R_Resource.Where(c => c.TaskID == item.ID && c.ResourceID == this.CurrentUserInfo.UserID &&
                    !String.IsNullOrEmpty(c.RoleCode)).Select(c => c.RoleCode).ToList();
                dic.SetValue("RoleCode", String.Join(",", roleCodes));
            }
            else
            {
                dic.SetValue("InUser", false.ToString());
                dic.SetValue("RoleCode", "");
            }
            return dic;
        }

        public JsonResult GetPlanList(QueryBuilder qb, string WBSID)
        {
            var taskType = TaskType.CooperationTask.ToString();
            var wbs = this.GetEntityByID<S_I_WBS>(WBSID);
            var data = this.entities.Set<S_I_WBS_Task>().Where(c => c.WBSFullID.StartsWith(wbs.FullID)
                && c.TaskType == taskType).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetMajorInputList(QueryBuilder qb, string WBSID)
        {
            var wbs = this.GetEntityByID<S_I_WBS>(WBSID);
            var data = this.entities.Set<S_E_MajorPutInfo>().Where(d => d.WBSID == WBSID && String.IsNullOrEmpty(d.TaskID)).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetReceiveList(QueryBuilder qb, string WBSID)
        {
            string sql = @"select *,case when TaskID is null or TaskID ='' then '计划外提资'
else '计划内提资' end as CooperationType from S_E_CooperationExe
where InWBSID='" + WBSID + "'";
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetShareInfoList(QueryBuilder qb, string WBSID)
        {
            var wbs = this.GetEntityByID<S_I_WBS>(WBSID);
            var data = this.entities.Set<S_E_DrawingCooperation>().Where(d => d.WBSID == wbs.ID).WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetExecuteList(string TaskID)
        {
            var list = this.entities.Set<S_E_MajorPutInfo>().Where(d => d.TaskID == TaskID).OrderByDescending(c => c.ID).ToList();
            return Json(list);
        }

        public JsonResult DelShareInfo(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var entity = this.GetEntityByID<S_E_DrawingCooperation>(item.GetValue("ID"));
                if (entity == null) continue;
                if (entity.FileSource == "CAD")
                {
                    throw new Formula.Exceptions.BusinessValidationException("不能删除CAD上传的图纸文件");
                }
                this.entities.Set<S_E_DrawingCooperation>().Remove(entity);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RefuseRecevie(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var entity = this.GetEntityByID<S_E_CooperationExe>(item.GetValue("ID"));
                if (entity == null) continue;
                entity.Refuse();
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ConfirmRecevie(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var entity = this.GetEntityByID<S_E_CooperationExe>(item.GetValue("ID"));
                if (entity == null) continue;
                entity.Confirm();
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
