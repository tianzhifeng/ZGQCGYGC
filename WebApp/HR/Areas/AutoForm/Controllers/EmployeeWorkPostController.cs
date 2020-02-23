using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using HR.Logic.BusinessFacade;

namespace HR.Areas.AutoForm.Controllers
{
    public class EmployeeWorkPostController : HRFormContorllor<T_EmployeeWorkPost>
    {

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic["ID"]);
            if (entity == null) entity = new T_EmployeeWorkPost();
            this.UpdateEntity(entity, dic);
            if (isNew)
            {
                entity.SynchPostInfo();
            }
            else
            {
                if (entity.IsTheNewest == TrueOrFalse.True.ToString())
                {
                    entity.SynchPostInfo();
                }
            }
        }
    }
}
