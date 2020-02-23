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
namespace Project.Areas.AutoList.Controllers
{
    public class WBSTaskWorkEditListController : ProjectEditListContorllor
    { 
        //public JsonResult GetDefaultTaskWorkDefaultRoleSet(string wbsNodeID)
        //{
        //    Dictionary<string, object> item = new Dictionary<string, object>();
        //    item.SetValue("ID", wbsNodeID);
            
        //    var wbs = BusinessEntities.Set<S_W_WBS>().Include("S_W_RBS").SingleOrDefault(d => d.ID == wbsNodeID);
        //    var CurrentUserInfo =  FormulaHelper.GetUserInfo();
 
        //    if (wbs != null)
        //    {
        //        item.SetValue("ProjectInfoID", wbs.ProjectInfoID);
        //        if (wbs.S_I_ProjectInfo.S_W_OBSUser.Count(d => d.UserID == CurrentUserInfo.UserID && d.MajorValue == wbs.WBSValue) > 0)
        //            item.SetValue("HasAuth", TrueOrFalse.True.ToString());
        //        else
        //            item.SetValue("HasAuth", TrueOrFalse.False.ToString());
        //        foreach (var stRole in wbs.StructNodeInfo.S_T_WBSStructRole.ToList())
        //        {
        //            string userID = string.Empty; string userName = string.Empty;
        //            foreach (var rbs in wbs.S_W_RBS.Where(d => d.RoleCode == stRole.RoleCode).ToList())
        //            {
        //                userID += rbs.UserID + ",";
        //                userName += rbs.UserName + ",";
        //            }
        //            item.SetValue(stRole.RoleCode + "UserID", userID.TrimEnd(','));
        //            item.SetValue(stRole.RoleCode + "UserName", userName.TrimEnd(','));
        //        }
        //    }

        //    string projectInfoID = wbs.ProjectInfoID;
        //    var projectInfo = wbs.S_I_ProjectInfo;
        //    if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息");
        //    var majorStruct = projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Major.ToString());
        //    if (majorStruct == null) throw new Formula.Exceptions.BusinessException("项目不存在专业节点定义，无法进行专业策划");
        //    var structRoles = majorStruct.S_T_WBSStructRole.Where(d => d.SychWBS != true.ToString()).OrderBy(d => d.SortIndex).ToList();
        //    ViewBag.StructRoles = structRoles;

        //    string sychField = "";
        //    foreach (var item1 in structRoles)
        //    {
        //        sychField += item1.RoleCode + "UserID,";
        //        sychField += item1.RoleCode + "UserName,";
        //    }
        //    ViewBag.SychField = sychField.TrimEnd(',');
        //    return Json(new { sychField = sychField, item = item });
        //}

        public JsonResult SaveWork(string TaskInfo, string ParentWBSID)
        {
            var list = JsonHelper.ToList(TaskInfo);
            var parent = BusinessEntities.Set<S_W_WBS>().Find(ParentWBSID);
            if (parent == null) throw new Formula.Exceptions.BusinessException("未获取到当前工作包的父节点，保存失败。");
            foreach (var item in list)
            {
                S_W_TaskWork taskWork;
                if (item.GetValue("_state") == "removed")
                {
                    taskWork = BusinessEntities.Set<S_W_TaskWork>().Find(item.GetValue("ID"));
                    if (taskWork != null)
                    {
                        taskWork.Delete(false);
                    }
                }
                else
                {                    
                    if (String.IsNullOrEmpty(item.GetValue("ID")))
                    {
                        taskWork = new S_W_TaskWork();
                        this.UpdateEntity<S_W_TaskWork>(taskWork, item);
                        parent.AddTaskWork(taskWork);
                    }
                    else
                    {
                        taskWork = BusinessEntities.Set<S_W_TaskWork>().Find(item.GetValue("ID"));
                        this.UpdateEntity<S_W_TaskWork>(taskWork, item);
                        taskWork.Save();
                    }
                }
            }
            BusinessEntities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportPackage(string DataSource, string TargetID, string WithUser)
        {
            var list = JsonHelper.ToList(DataSource);
            var wbs = BusinessEntities.Set<S_W_WBS>().Find(TargetID);
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

        public string CheckDelete(string IDs)
        {
            var error = string.Empty;
            var idAry = IDs.Split(',').ToArray();
            var taskWorkList = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => idAry.Contains(a.ID)).ToList();
            foreach (var taskWork in taskWorkList)
            {
                if (taskWork.S_W_WBS.S_E_Product.Count > 0)
                    error += string.Format("【{0}】下已经有文件</br>", taskWork.Name);
            }
            return error;
        }
    }
}
