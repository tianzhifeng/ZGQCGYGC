using EPC.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;

namespace EPC.Controllers
{
    public class WBSStructSelectorController : InfrastructureController
    {
        public JsonResult GetStructTree(string ModeID)
        {
            var data = this.entities.Set<S_C_WBSStruct>().Where(c => c.ModeID == ModeID).OrderBy(c => c.SortIndex).ToList();
            return Json(data);
        }

        public JsonResult GetAllStructTree()
        {
            var data = this.entities.Set<S_C_WBSStruct>().Where(c=>c.NodeType!="Task").OrderBy(c => c.SortIndex).ToList();
            var modes = this.entities.Set<S_C_Mode>().ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in modes)
            {
                var rootNode = new Dictionary<string, object>();
                rootNode.SetValue("ID", item.ID);
                rootNode.SetValue("ParentID", "");
                rootNode.SetValue("NodeType", "Mode");
                rootNode.SetValue("Name", item.Name);
                result.Add(rootNode);
                var list = data.Where(c => c.ModeID == item.ID).OrderBy(c => c.SortIndex).ToList();
                foreach (var stnode in list)
                {
                    var node = new Dictionary<string, object>();
                    node.SetValue("ID", stnode.ID);
                    if (String.IsNullOrEmpty(stnode.ParentID))
                    {
                        node.SetValue("ParentID", item.ID);
                    }
                    else
                    {
                        node.SetValue("ParentID", stnode.ParentID);
                    }               
                    node.SetValue("NodeType", stnode.NodeType);
                    node.SetValue("Name", stnode.Name);
                    result.Add(node);
                }
            }
            return Json(result);
        }
    }
}
