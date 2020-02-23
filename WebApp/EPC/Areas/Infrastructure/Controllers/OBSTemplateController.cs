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
    public class OBSTemplateController : InfrastructureController<S_C_OBSTemplate>
    {
        public override JsonResult GetTree()
        {
            string modeID = this.Request["ModeID"];
            var data = this.entities.Set<S_C_OBSTemplate>().Where(d => d.ModeID == modeID).OrderBy(d => d.SortIndex).ToList();
            return Json(data);
        }

        protected override void BeforeSave(S_C_OBSTemplate entity, bool isNew)
        {
            if (isNew)
            {
                var parentNode = this.GetEntityByID(entity.ParentID);
                if (parentNode == null) throw new Formula.Exceptions.BusinessValidationException("找不到父节点");
                parentNode.AddChild(entity);
            }
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var sourceNode = this.GetEntityByID<S_C_OBSTemplate>(sourceID);
            if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的定义节点，无法移动");
            if (dragAction.ToLower() == "add")
            {
                var target = this.GetEntityByID<S_C_OBSTemplate>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");

                sourceNode.ParentID = target.ID;
                sourceNode.FullID = target.FullID+"."+sourceNode.ID;
                var details = this.entities.Set<S_C_OBSTemplate>().Where(c => c.ParentID == target.ID).ToList();
                if (details.Count == 0)
                    sourceNode.SortIndex = 0;
                else
                {
                    var maxSortIndex = details.Max(c => c.SortIndex);
                    sourceNode.SortIndex = maxSortIndex + 1;
                }
            }
            else if (dragAction.ToLower() == "before")
            {
                var target = this.GetEntityByID<S_C_OBSTemplate>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");
                if (target.Parent == null) throw new Formula.Exceptions.BusinessValidationException("不能移动至根节点");
                this.entities.Set<S_C_OBSTemplate>().Where(c => c.ParentID == target.ParentID
                   && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 1);
                sourceNode.ParentID = target.ParentID;
                sourceNode.FullID = target.Parent.FullID + "." + sourceNode.ID;
                sourceNode.SortIndex = target.SortIndex - 1;
            }
            else if (dragAction.ToLower() == "after")
            {
                var target = this.GetEntityByID<S_C_OBSTemplate>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");
                if (target.Parent == null) throw new Formula.Exceptions.BusinessValidationException("不能移动至根节点");
                this.entities.Set<S_C_OBSTemplate>().Where(c => c.ParentID == target.ParentID 
                  && c.SortIndex > target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 1);
                sourceNode.ParentID = target.ParentID;
                sourceNode.FullID = target.Parent.FullID+"."+sourceNode.ID;
                sourceNode.SortIndex = target.SortIndex + 1;
            }
            this.entities.SaveChanges();
            return Json(sourceNode);
        }
    }
}
