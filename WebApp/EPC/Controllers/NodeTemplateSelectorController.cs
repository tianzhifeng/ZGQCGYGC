using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using MvcAdapter;

namespace EPC.Controllers
{
    public class NodeTemplateSelectorController : InfrastructureController<S_T_NodeTemplate>
    {
        public JsonResult GetTreeList(string NodeTemplateID)
        {
            var data = this.entities.Set<S_T_NodeTemplate_Detail>().Where(c => c.NodeTemplateID == NodeTemplateID).OrderBy(c => c.SortIndex).ToList();
            return Json(data);
        }
    }
}
