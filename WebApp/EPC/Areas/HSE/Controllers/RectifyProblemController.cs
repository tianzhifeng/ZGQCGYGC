using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic;
using EPC.Logic.Domain;
using Workflow.Logic.Domain;

namespace EPC.Areas.HSE.Controllers
{
    public class RectifyProblemController : EPCFormContorllor<T_HSE_RectifySheet_RectifyProblems>
    {
        //
        // GET: /HSE/Rectify/
        private EPCEntities dbContext = FormulaHelper.GetEntities<EPCEntities>();

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);

            //修改
            if (!isNew)
            {
                string id = dic.GetValue("ID");
                var problem = dbContext.T_HSE_RectifySheet_RectifyProblems.Find(id);
                if (problem != null)
                {
                    if (dbContext.T_HSE_RectifySheet.Any(a => a.ID == problem.T_HSE_RectifySheetID))
                    {
                        throw new Formula.Exceptions.BusinessValidationException("问题【" + problem.Problems + "】有整改记录,因此不能修改");
                    }
                }
            }
        }

        protected override void BeforeDelete(string[] Ids)
        {
            base.BeforeDelete(Ids);
            //删除的问题已经处于整改的流程中或者已结束，则不能够删除问题
            foreach (string id in Ids)
            {
                var problem = dbContext.T_HSE_RectifySheet_RectifyProblems.Find(id);
                if (problem != null)
                {
                    if (dbContext.T_HSE_RectifySheet.Any(a => a.ID == problem.T_HSE_RectifySheetID))
                    {
                        throw new Formula.Exceptions.BusinessValidationException("问题【" + problem.Problems + "】有整改记录,因此不能删除");
                    }
                    //同时删除T_HSE_SafeCheck_CheckContentRecord
                    else
                    {
                        dbContext.T_HSE_SafeCheck_CheckContentRecord.Delete(a => a.ID == problem.T_HSE_SafeCheck_CheckContentRecordID);
                    }
                }
            }
        }
    }
}
