using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Logic.Domain;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class AuditModeController : BaseConfigController<S_T_AuditMode>
    {
        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var modeID = this.GetQueryString("ModeID");
            var list = this.entities.Set<S_T_AuditMode>().Where(d => d.ProjectModeID == modeID).ToList();
            if (list.Count == 0)
            {
                var phaseList = this.entities.Set<S_D_WBSAttrDefine>().Where(d => d.Type == "Phase")
                    .Select(d => new { AttrID = d.ID, ProjectModeID = modeID, PhaseValue = d.Code, PhaseName = d.Name, AuditMode = "OneByTwo", d.SortIndex })
                                .OrderBy(d => d.SortIndex).ToList();
                return Json(phaseList);
            }
            else
            {
                return Json(list);
            }
        }

    }
}
