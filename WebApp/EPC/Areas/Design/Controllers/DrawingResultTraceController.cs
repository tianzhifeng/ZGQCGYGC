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
    public class DrawingResultTraceController : EPCController<S_E_DrawingResult>
    {
        public JsonResult GetDesignWorkSpaceTree(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息，无法开展设计工作");
            var list = this.entities.Set<S_I_WBS>().Where(c => c.EngineeringInfoID == EngineeringInfoID).ToList();
            var designTaskType = TaskType.DesignTask.ToString();
            var taskList = this.entities.Set<S_I_WBS_Task>().Where(c => c.TaskType == designTaskType && c.EngineeringInfoID == EngineeringInfoID).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in taskList)
            {
                var wbs = list.FirstOrDefault(c => c.ID == item.ParentID);
                if (!result.Exists(c => c.GetValue("ID") == wbs.ID))
                {
                    var wbsDic = this.fillDicWithWBS(wbs);
                    result.Add(wbsDic);
                    foreach (var node in wbs.Ancestor)
                    {
                        if (result.Exists(c => c.GetValue("ID") == node.ID) || node.NodeType == "Root")
                            continue;
                        wbsDic = this.fillDicWithWBS(node);
                        result.Add(wbsDic);
                    }
                }
                var dic = this.fillDicWithTask(item);
                result.Add(dic);
            }
            result = result.OrderBy(c => c["SortIndex"]).ToList();
            return Json(result);
        }

        Dictionary<string, object> fillDicWithWBS(S_I_WBS item)
        {
            var dic = new Dictionary<string, object>();
            dic.SetValue("ID", item.ID);
            dic.SetValue("ParentID", item.ParentID);
            dic.SetValue("FullID", item.FullID);
            dic.SetValue("Value", item.Value);
            dic.SetValue("Name", item.Name);
            dic.SetValue("WBSID", item.ID);
            dic.SetValue("NodeType", item.NodeType);
            dic.SetValue("SortIndex", item.SortIndex);
            dic.SetValue("TaskID", "");
            dic.SetValue("DrawingCount", this.entities.Set<S_E_DrawingResult>().Count(c => c.TaskID == item.ID));
            return dic;
        }

        Dictionary<string, object> fillDicWithTask(S_I_WBS_Task item)
        {
            var dic = new Dictionary<string, object>();
            dic.SetValue("ID", item.ID);
            dic.SetValue("ParentID", item.ParentID);
            dic.SetValue("FullID", item.WBSFullID);
            dic.SetValue("Value", item.Code);
            dic.SetValue("Name", item.Name);
            dic.SetValue("WBSID", item.ParentID);
            dic.SetValue("NodeType", item.TaskType);
            dic.SetValue("SortIndex", item.SortIndex);
            dic.SetValue("TaskID", item.ID);
            dic.SetValue("DrawingCount", this.entities.Set<S_E_DrawingResult>().Count(c=>c.TaskID==item.ID));
            return dic;
        }

        public JsonResult GetDrawingList(QueryBuilder qb, string TaskID, string WBSID)
        {
            var wbs = this.GetEntityByID<S_I_WBS>(WBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的WBS节点，无法展现列表");
            qb.Add("WBSFullID", QueryMethod.StartsWith, wbs.FullID);
            if (!String.IsNullOrEmpty(TaskID))
            {
                qb.Add("TaskID", QueryMethod.Equal, TaskID);
            }
            var data = this.entities.Set<S_E_DrawingResult>().WhereToGridData(qb);
            return Json(data);
        }
    }
}
