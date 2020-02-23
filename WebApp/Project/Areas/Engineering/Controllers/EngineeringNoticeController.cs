using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Logic.Domain;

namespace Project.Areas.Engineering.Controllers
{
    public class EngineeringNoticeController : ProjectController<S_N_Notice>
    {
        [ValidateInput(false)]
        public override JsonResult Save()
        {
            this.ValidateRequest = false;
            var entity = UpdateEntity<S_N_Notice>();
            entity.CreateDate = DateTime.Now;
            entity.CreateUserID = Formula.FormulaHelper.GetUserInfo().UserID;
            entity.CreateUserName = Formula.FormulaHelper.GetUserInfo().UserName;
            entity.Title = entity.Title.Replace("\"", " ").Replace("'", " ");
            entity.IsFromSys = "False";
            entities.SaveChanges();
            return Json("");
        }
    }
}
