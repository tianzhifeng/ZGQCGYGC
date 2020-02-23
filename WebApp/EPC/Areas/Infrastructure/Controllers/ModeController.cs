using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic.Domain;

namespace EPC.Areas.Infrastructure.Controllers
{
    public class ModeController : InfrastructureController<S_C_Mode>
    {
        public ActionResult Config()
        {
            string ModeID = this.GetQueryString("ID");
            ViewBag.ModeID = ModeID;
            return View();
        }

        public JsonResult GetMeunRoot()
        {
            string modeID = this.GetQueryString("ModeID");
            var data = this.entities.Set<S_C_Meun>().Where(d => d.ModeID == modeID && d.MeunType == "Root").OrderBy(d => d.SortIndex).ToList();
            return Json(data);
        }

        protected override void BeforeSave(S_C_Mode entity, bool isNew)
        {
            entity.Save();
        }

        public JsonResult Copy(string ID)
        {
            var mode = this.GetEntityByID(ID);
            if (mode == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的项目模式，无法复制");
            }
            mode.Copy();
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
