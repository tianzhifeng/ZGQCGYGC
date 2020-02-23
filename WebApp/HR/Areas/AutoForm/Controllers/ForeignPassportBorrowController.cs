using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Config.Logic;
namespace HR.Areas.AutoForm.Controllers
{
    public class ForeignPassportBorrowController : HRFormContorllor<T_Foreign_PassportBorrow>
    {
        //
        // GET: /AutoForm/ForeignPassportBorrow/

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                var PassportID = dic.GetValue("PassportID");
                var Passport = BusinessEntities.Set<T_Foreign_Passport>().Find(PassportID);
                Passport.PassportState = "借出";
                BusinessEntities.SaveChanges();

            }
        }

        public JsonResult DoRetrun()
        {
            var IDs = GetQueryString("IDs");
            BusinessEntities.Set<T_Foreign_Passport>().Where(c => IDs.Contains(c.ID)).Update(c => c.PassportState = "在库");
            BusinessEntities.SaveChanges();
            return Json("");
        }

    }
}
