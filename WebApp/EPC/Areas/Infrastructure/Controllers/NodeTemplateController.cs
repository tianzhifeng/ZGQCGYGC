using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic.Domain;
using Formula;
using EPC.Logic;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class NodeTemplateController : InfrastructureController<S_T_NodeTemplate>
    {

        public override ActionResult Edit()
        {
            return View();
        }

        public override ActionResult List()
        {
            var tab = new Tab();
            var planStateCategory = CategoryFactory.GetCategory(typeof(NodeClass), "节点类型", "NodeType");
            planStateCategory.Multi = false;
            tab.Categories.Add(planStateCategory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        protected override void BeforeSave(S_T_NodeTemplate entity, bool isNew)
        {
            if (isNew)
            {
                var root = new S_T_NodeTemplate_Detail();
                root.ID = Formula.FormulaHelper.CreateGuid();
                root.FullID = root.ID;
                root.Name = entity.Name;
                root.Code = entity.Code;
                root.NodeType = "Root";
                root.ParentID = "";
                root.NodeTemplateID = entity.ID;
                root.SortIndex = 0;
                this.entities.Set<S_T_NodeTemplate_Detail>().Add(root);
            }
        }

        public JsonResult AddChild(string parentID)
        {
            var parent = this.GetEntityByID<S_T_NodeTemplate_Detail>(parentID);
            var child = new S_T_NodeTemplate_Detail();
            child.ID = Formula.FormulaHelper.CreateGuid();
            child.FullID = parent.FullID + "." + child.ID;
            child.Name = "新建节点";
            child.Code = "";
            child.NodeType = "";
            child.ParentID = parent.ID;
            child.NodeTemplateID = parent.NodeTemplateID;
            child.SortIndex = this.entities.Set<S_T_NodeTemplate_Detail>().Count(c => c.ParentID == parentID) + 1;
            this.entities.Set<S_T_NodeTemplate_Detail>().Add(child);
            this.entities.SaveChanges();
            return Json(child);
        }

        public JsonResult SaveDetail(string DetailInfo)
        {
            var list = JsonHelper.ToList(DetailInfo);
            foreach (var item in list)
            {
                var node = this.GetEntityByID<S_T_NodeTemplate_Detail>(item.GetValue("ID"));
                if (node == null) continue;
                this.UpdateEntity<S_T_NodeTemplate_Detail>(node, item);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteDetail(string DetailInfo)
        {
            var list = JsonHelper.ToList(DetailInfo);
            foreach (var item in list)
            {
                var id = item.GetValue("ID");
                if (String.IsNullOrEmpty(id)) continue;
                var node = this.GetEntityByID<S_T_NodeTemplate_Detail>(id);
                if (node == null) continue;
                if (String.IsNullOrEmpty(node.ParentID)) throw new Formula.Exceptions.BusinessValidationException("根节点不允许删除");
                this.entities.Set<S_T_NodeTemplate_Detail>().Delete(d => d.FullID.StartsWith(node.FullID));
                this.entities.SaveChanges();
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetDetailTree(string NodeTemplateID)
        {
            var data = this.entities.Set<S_T_NodeTemplate_Detail>().Where(c => c.NodeTemplateID ==
                NodeTemplateID).OrderBy(c => c.SortIndex).ToList();
            return Json(data);
        }
    }
}
