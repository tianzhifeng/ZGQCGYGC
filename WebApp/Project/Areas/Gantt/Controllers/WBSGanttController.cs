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

namespace Project.Areas.Gantt.Controllers
{
    public class WBSGanttController : ProjectController<S_W_WBS>
    {
        public ActionResult WBSGantt()
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
            return this.View();
        }

        public  override JsonResult GetTree()
        {
            var fo = FormulaHelper.CreateFO<WBSFO>();
            string projectInfoID = this.Request["ProjectInfoID"];
            string viewType = String.IsNullOrEmpty(this.Request["ViewType"]) ? WBSNodeType.Project.ToString() : this.Request["ViewType"];
            if (String.IsNullOrEmpty(projectInfoID)) throw new Formula.Exceptions.BusinessException("参数ProjectInfoID不能为空");
           // var result = fo.GetGranttWBSTree(projectInfoID, viewType, true);
            return Json("");
        }

        public JsonResult AddChild(string parentIDs, string Children)
        {
            foreach (var parentID in parentIDs.TrimEnd(',').Split(','))
            {
                var parent = this.GetEntityByID<S_W_WBS>(parentID);
                if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + parentID + "】的WBS节点，无法增加子节点");
                var list = JsonHelper.ToList(Children);
                foreach (var item in list)
                {
                    var wbs = new S_W_WBS();
                    this.UpdateEntity<S_W_WBS>(wbs, item);
                    parent.AddChild(wbs);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddChildWithDefAttr(string Children)
        {
            var dic = JsonHelper.ToObject(Children);
            string parentIDs = dic["ParentIDs"].ToString();
            string children = dic["childNodes"].ToString();
            string type = dic["Type"].ToString();
            var list = BaseConfigFO.GetWBSAttrList(type);
            foreach (var parentID in parentIDs.TrimEnd(',').Split(','))
            {
                var parent = this.GetEntityByID<S_W_WBS>(parentID);
                if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + parentID + "】的WBS节点，无法增加子节点");
                foreach (var item in children.Split(','))
                {
                    var attrDefine = list.FirstOrDefault(d => d.Code == item);
                    if (attrDefine == null) continue;
                    var wbs = new S_W_WBS();
                    wbs.WBSType = dic["Type"].ToString();
                    wbs.Name = attrDefine.Name;
                    wbs.SortIndex = attrDefine.SortIndex;
                    wbs.WBSValue = item;
                    parent.AddChild(wbs);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult WBSCopy(string TargetInfo, string DataSource)
        {
            var targetNode = JsonHelper.ToObject(TargetInfo);
            var dataSource = JsonHelper.ToList(DataSource);
            var targetWBS = this.GetEntityByID<S_W_WBS>(targetNode.GetValue("WBSID"));
            if (targetWBS == null) throw new Formula.Exceptions.BusinessException("目标节点未找到，无法导入WBS");
            var top = dataSource.Min(d => Convert.ToInt32(d["Level"]));
            if (top == 1) top++; //如果全部选中项目节点也会在选中行里中，此处就需要获取所有的项目下层节点，level 统一降一级
            var topNodes = dataSource.Where(d => Convert.ToInt32(d["Level"]) == top).ToList();
            foreach (var item in topNodes)
            {
                var topWBS = this.GetEntityByID<S_W_WBS>(item["ID"].ToString());
                if (topWBS == null) throw new Formula.Exceptions.BusinessException("所选中的WBS不存在，无法完成导入操作");
                topWBS.CopyTo(targetWBS);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteWBS(string WBSInfo)
        {
            var list = JsonHelper.ToList(WBSInfo);
            foreach (var item in list)
            {
                var wbsId = item.GetValue("WBSID");
                if (String.IsNullOrEmpty(wbsId)) continue;
                var wbs = this.GetEntityByID<S_W_WBS>(wbsId);
                if (wbs.WBSType == WBSNodeType.Project.ToString()) throw new Formula.Exceptions.BusinessException("WBS根节点不允许删除");
                if (wbs == null) continue;
                wbs.Delete();
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult PublishWBS(string WBSInfo, string ViewType)
        {
            var list = JsonHelper.ToList(WBSInfo);
            foreach (var item in list)
            {
                var wbs = this.GetEntityByID<S_W_WBS>(item.GetValue("WBSID"));
                if (wbs == null) continue;
                if (ViewType == wbs.ProjectMode.RootStructInfoNode.Code || wbs.ProjectMode.RootStructInfoNode.Code.IndexOf(ViewType) >= 0)
                {
                    this.UpdateEntity<S_W_WBS>(wbs, item);
                    wbs.SetProperty("PlanStartDate", item["Start"]);
                    wbs.SetProperty("PlanEndDate", item["Finish"]);
                }
                else
                {
                    wbs.SetProperty("PlanStartDate", item["Start"]);
                    wbs.SetProperty("PlanEndDate", item["Finish"]);
                }
                wbs.Save();
            }
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
    }
}
