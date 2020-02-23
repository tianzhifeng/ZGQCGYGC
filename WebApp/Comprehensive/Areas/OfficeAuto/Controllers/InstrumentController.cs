using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Comprehensive.Logic.Domain;
using Config.Logic;
using Comprehensive.Logic;

namespace Comprehensive.Areas.OfficeAuto.Controllers
{
    public class InstrumentController : ComprehensiveFormController<S_I_InstrumentInfo>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                var id = dic.GetValue("ID");
                dic.SetValue("PhysicalState", PhysicalState.在库.ToString());
            }
            base.BeforeSave(dic, formInfo, isNew);
        }

        public void ValidateBorrowStatus(string instrumentID)
        {
            var instrumentInfo = this.GetEntityByID<S_I_InstrumentInfo>(instrumentID);
            if (instrumentInfo.PhysicalState == PhysicalState.已报废.ToString())
            {
                throw new Formula.Exceptions.BusinessValidationException("所选物品已报废，不能发起借用流程！");
            }
            else if (instrumentInfo.PhysicalState != PhysicalState.在库.ToString())
            {
                throw new Formula.Exceptions.BusinessValidationException("所选物品不在库，不能发起借用流程！");
            }
        }

        public void ValidateDiscardStatus(string instrumentID)
        {
            var instrumentInfo = this.GetEntityByID<S_I_InstrumentInfo>(instrumentID);
            if (instrumentInfo.PhysicalState == PhysicalState.已报废.ToString())
            {
                throw new Formula.Exceptions.BusinessValidationException("所选物品已报废，无需再次发起报废流程！");
            }
        }
    }
}
