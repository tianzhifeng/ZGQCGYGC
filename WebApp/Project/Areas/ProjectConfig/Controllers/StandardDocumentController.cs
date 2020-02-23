using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Logic.Domain;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class StandardDocumentController : BaseConfigController<S_D_StandardDocument>
    {
        protected override void AfterGetMode(S_D_StandardDocument entity, bool isNew)
        {
            if (isNew)
            {
                entity.State = "Normal";
                base.AfterGetMode(entity, isNew);
            }
        }

        protected override void BeforeSave(S_D_StandardDocument entity, bool isNew)
        {
            var standardID = this.GetQueryString("StandardID");
            var standardInfo = this.entities.Set<S_D_StandardDocument>().FirstOrDefault(d => d.Code == entity.Code && d.StandardID == standardID && d.ID != entity.ID);
            if (standardInfo != null) throw new Formula.Exceptions.BusinessException("该目录下已存在编号为【" + entity.Code + "】的标准规范,请尝试其它编号！");

            base.BeforeSave(entity, isNew);
        }
    }
}
