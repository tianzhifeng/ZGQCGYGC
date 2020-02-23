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
    public class PBSStructDefineController : InfrastructureController<S_C_PBSStruct>
    {
        public override JsonResult GetTree()
        {
            string modeID = this.Request["ModeID"];
            var data = this.entities.Set<S_C_PBSStruct>().Where(d => d.ModeID == modeID).OrderBy(d => d.SortIndex).ToList();
            return Json(data);
        }

        protected override void BeforeSave(S_C_PBSStruct entity, bool isNew)
        {
            if (isNew)
            {
                var parentNode = this.GetEntityByID(entity.ParentID);
                if (parentNode == null) throw new Formula.Exceptions.BusinessValidationException("找不到父节点");
                parentNode.AddChild(entity);
            }
        }

        protected override void AfterGetMode(S_C_PBSStruct entity, bool isNew)
        {
            if (isNew)
            {
                entity.IsEnum = false;
                entity.CanSynToWBS = false;
            }
        }

        public override JsonResult DeleteNode()
        {
            string ID = this.Request["ID"];
            var node = this.GetEntityByID<S_C_PBSStruct>(ID);
            if (node != null)
            {
                node.Delete();
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
