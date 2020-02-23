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

namespace EPC.Areas.Infrastructure.Controllers
{
    public class WBSStructDefineController : InfrastructureController<S_C_WBSStruct>
    {
        public override JsonResult GetTree()
        {
            string modeID = this.Request["ModeID"];
            var data = this.entities.Set<S_C_WBSStruct>().Where(d => d.ModeID == modeID).OrderBy(d => d.SortIndex).ToList();
            return Json(data);
        }

        protected override void BeforeSave(S_C_WBSStruct entity, bool isNew)
        {
            if (isNew)
            {
                var parentNode = this.GetEntityByID(entity.ParentID);
                if (parentNode == null) throw new Formula.Exceptions.BusinessValidationException("找不到父节点");
                parentNode.AddChild(entity);
            }
            else
            {
                var defineNodes = this.entities.Set<S_C_ScheduleDefine_Nodes>().Where(c => c.StructInfoID == entity.ID).ToList();
                foreach (var item in defineNodes)
                {
                    item.NodeType = entity.NodeType;
                    item.EnumKey = entity.EnumKey;
                    item.TaskType = entity.TaskType;
                    item.IsDynmanic = entity.IsDynmanic;
                    item.IsEnum = entity.IsEnum;
                }
            }
        }

        protected override void AfterGetMode(S_C_WBSStruct entity, bool isNew)
        {
            if (isNew)
            {
                entity.IsDynmanic = "0";
                entity.IsEnum = "0";
            }
        }

        public override JsonResult DeleteNode()
        {
            string json = this.Request["ListData"];
            var list = JsonHelper.ToList(json);
            foreach (var item in list)
            {
                var node = this.GetEntityByID<S_C_WBSStruct>(item.GetValue("ID"));
                if (node != null)
                {
                    node.Delete();
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var sourceNode = this.GetEntityByID<S_C_WBSStruct>(sourceID);
            if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的节点，无法移动");
            if (dragAction.ToLower() == "add")
            {
                var target = this.GetEntityByID<S_C_WBSStruct>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                sourceNode.ParentID = target.ID;
                foreach (var item in sourceNode.AllChildren)
                {
                    item.FullID = item.FullID.Replace(sourceNode.FullID, target.FullID + "." + sourceNode.ID);
                }
                sourceNode.FullID = target.FullID + "." + sourceNode.ID;
                var maxSortIndex = target.Children.Max(c => c.SortIndex);
                sourceNode.SortIndex = maxSortIndex + 1;
            }
            else if (dragAction.ToLower() == "before")
            {
                var target = this.GetEntityByID<S_C_WBSStruct>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");

                this.entities.Set<S_C_WBSStruct>().Where(c => c.ParentID == target.ParentID
                   && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 1);
                sourceNode.ParentID = target.ParentID;
                foreach (var item in sourceNode.AllChildren)
                {
                    item.FullID = item.FullID.Replace(sourceNode.FullID, target.Parent.FullID + "." + sourceNode.ID);
                }
                sourceNode.FullID = target.Parent.FullID + "." + sourceNode.ID;
                sourceNode.SortIndex = target.SortIndex - 1;

            }
            else if (dragAction.ToLower() == "after")
            {
                var target = this.GetEntityByID<S_C_WBSStruct>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");

                this.entities.Set<S_C_WBSStruct>().Where(c => c.ParentID == target.ParentID
                   && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 1);
                sourceNode.ParentID = target.ParentID;
                foreach (var item in sourceNode.AllChildren)
                {
                    item.FullID = item.FullID.Replace(sourceNode.FullID, target.Parent.FullID + "." + sourceNode.ID);
                }
                sourceNode.FullID = target.Parent.FullID + "." + sourceNode.ID;
                sourceNode.SortIndex = target.SortIndex + 1;
            }
            this.entities.SaveChanges();
            return Json(sourceNode);
        }
    }
}
