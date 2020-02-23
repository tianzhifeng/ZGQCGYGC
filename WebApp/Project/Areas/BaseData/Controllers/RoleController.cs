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

namespace Project.Areas.BaseData.Controllers
{
    public class RoleController : BaseConfigController<S_D_RoleDefine>
    {
        public override ActionResult Edit()
        {
            var auditStates = Enum.GetNames(typeof(Project.Logic.AuditState)).ToList().Select(a => new { text = a, value = a });
            ViewBag.RoleEnum = JsonHelper.ToJson(auditStates);
            return base.Edit();
        }
        protected override void AfterGetMode(S_D_RoleDefine entity, bool isNew)
        {
            if (isNew)
            {
                entity.RoleType = Project.Logic.RoleType.ProjectRoleType.ToString();
                entity.State = "Normal";
            }
        }

        protected override void BeforeDelete(List<S_D_RoleDefine> entityList)
        {
            foreach (var item in entityList)
                item.Delete();
        }
    }
}
