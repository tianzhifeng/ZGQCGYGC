using Project.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Formula;
using Formula.Helper;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class ToDoListDefineController : BaseConfigController<S_T_ToDoListDefine>
    {
        public override JsonResult Save()
        {
            var entity = this.UpdateEntity<S_T_ToDoListDefine>();
            var root = entity.S_T_ToDoListDefineNode.Where(c => c.Type == "Root").FirstOrDefault();
            if (root == null)
            {
                root = new S_T_ToDoListDefineNode();
                root.ID = FormulaHelper.CreateGuid();
                root.FullID = root.ID;
                root.ParentID = "";
                root.Name = entity.Name;
                root.Type = "Root";
                root.SortIndex = 0;
                root.DefineID = entity.ID;
                root.ModeID = entity.ModeID;
                entity.S_T_ToDoListDefineNode.Add(root);
                root.S_T_ToDoListDefine = entity;
            }
            else
            {
                root.Name = entity.Name;
            }
            this.entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult GetDefineTree(string ModeID)
        {
            var list = this.entities.Set<S_T_ToDoListDefineNode>().Where(c => c.ModeID == ModeID).OrderBy(c => c
                .SortIndex).ToList();
            return Json(list);
        }

        public JsonResult AddNode(string ParentID)
        {
            var parent = this.GetEntity<S_T_ToDoListDefineNode>(ParentID);
            if (parent == null) { throw new Formula.Exceptions.BusinessException("没有找到ID为【" + ParentID + "】的节点，无法新增"); }
            var node = new S_T_ToDoListDefineNode();
            node.ID = FormulaHelper.CreateGuid();
            node.ModeID = parent.ModeID;
            if (parent.Type == "Root")
            {
                node.Type = "Category";
                node.Name = "新分类";
            }
            else if (parent.Type == "Category")
            {
                node.Type = "Task";
                node.Name = "新工作";
            }
            else
            {
                node.Type = "Process";
                node.Name = "新工序";
            }
            node.ParentID = parent.ID;
            node.FullID = parent.FullID + "." + node.ID;
            node.SortIndex = parent.S_T_ToDoListDefine.S_T_ToDoListDefineNode.Where(c => c.ParentID == parent.ID).Count();
            node.DefineID = parent.DefineID;
            node.S_T_ToDoListDefine = parent.S_T_ToDoListDefine;
            parent.S_T_ToDoListDefine.S_T_ToDoListDefineNode.Add(node);
            this.entities.SaveChanges();
            return Json(node);
        }

        public JsonResult DeleteDefineNode(string ID)
        {
            var node = this.GetEntity<S_T_ToDoListDefineNode>(ID);
            if (node == null) { throw new Formula.Exceptions.BusinessException("没有找到ID为【" + ID + "】的节点，无法删除"); }
            this.entities.Set<S_T_ToDoListDefineNode>().Delete(c => c.FullID.StartsWith(node.FullID));
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveDefineNode(string FormData, string ID, string ExeAction)
        {
            var dic = JsonHelper.ToObject(FormData);
            var node = this.entities.Set<S_T_ToDoListDefineNode>().Find(ID);
            if (node == null) { throw new Formula.Exceptions.BusinessException("没有找到ID为【" + ID + "】的节点，保存失败"); }
            this.UpdateEntity<S_T_ToDoListDefineNode>(node, dic);
            node.ExeAction = ExeAction;
            this.entities.SaveChanges();
            return Json(node);
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var sourceNode = this.entities.Set<S_T_ToDoListDefineNode>().Find(sourceID);   //this.GetEntityByID<S_T_ToDoListDefineNode>(sourceID);
            if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的设备，无法移动");
            if (dragAction.ToLower() == "before")
            {
                var target = this.entities.Set<S_T_ToDoListDefineNode>().Find(targetID);   //this.GetEntityByID<S_T_ToDoListDefineNode>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                this.entities.Set<S_T_ToDoListDefineNode>().Where(c => c.ParentID == target.ParentID
                   && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 1);
                var parent = this.entities.Set<S_T_ToDoListDefineNode>().Find(target.ParentID);
                if (parent == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标的上级节点，移动失败");
                sourceNode.ParentID = target.ParentID;
                sourceNode.FullID = parent.FullID + "." + sourceNode.ID;
                sourceNode.SortIndex = target.SortIndex - 1;
            }
            else if (dragAction.ToLower() == "after")
            {
                var target = this.entities.Set<S_T_ToDoListDefineNode>().Find(targetID);   //this.GetEntityByID<S_T_ToDoListDefineNode>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                this.entities.Set<S_T_ToDoListDefineNode>().Where(c => c.ParentID == target.ParentID
                   && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 1);
                var parent = this.entities.Set<S_T_ToDoListDefineNode>().Find(target.ParentID);
                if (parent == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标的上级节点，移动失败");
                sourceNode.ParentID = target.ParentID;
                sourceNode.FullID = parent.FullID + "." + sourceNode.ID;
                sourceNode.SortIndex = target.SortIndex + 1;
            }
            this.entities.SaveChanges();
            return Json(sourceNode);
        }

    }
}
