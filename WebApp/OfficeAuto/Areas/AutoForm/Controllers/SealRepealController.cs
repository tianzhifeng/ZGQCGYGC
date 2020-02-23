using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class SealRepealController : OfficeAutoFormContorllor<T_Seal_Repeal>
    {
        //
        // GET: /AutoForm/SealRepeal/
        protected override void OnFlowEnd(T_Seal_Repeal entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            //修改印章刻制的废止状态
            var changeId = entity.SealInfoID;

            var changeEntity = BusinessEntities.Set<T_Seal_Change>().Find(changeId);

            if (changeEntity == null)
            {
                throw new Formula.Exceptions.BusinessException("印章刻制不存在");
            }

            changeEntity.SealStatus = "1";

            BusinessEntities.SaveChanges();
        }
    }
}
