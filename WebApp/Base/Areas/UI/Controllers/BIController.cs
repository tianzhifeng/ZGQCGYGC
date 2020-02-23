using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Base.Logic.Domain;
using Formula;


namespace Base.Areas.UI.Controllers
{
    public class BIController : BaseController<S_UI_BIConfig>
    {
        public override ActionResult List()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            return View();
        }

        public ActionResult ConfigLayout()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            string id = this.GetQueryString("ID");
            var config = this.entities.Set<S_UI_BIConfig>().FirstOrDefault(d => d.ID == id);
            if (config == null) throw new Exception("未能找到编号为【" + id + "】的BI定义对象");
            ViewBag.TmpCode = config.Code;
            return View();
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_BIConfig>();
            if (entities.Set<S_UI_BIConfig>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception(string.Format("编号重复，名称“{0}”，编号：“{1}”", entity.Name, entity.Code));
            entity.ModifyDate = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUser = user.UserID;
            entity.ModifyUserName = user.UserName;

            if (!string.IsNullOrEmpty(Request["Script"]))
                entity.ScriptText = Request["Script"];

            entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult Copy(string ID)
        {
            var config = this.entities.Set<S_UI_BIConfig>().Find(ID);
            if (config == null)
                throw new Formula.Exceptions.BusinessValidationException("没有找到指定的BI定义信息，无法复制");
            var user = Formula.FormulaHelper.GetUserInfo();
            var cloneConfig = new S_UI_BIConfig();
            cloneConfig.Blocks = config.Blocks;
            cloneConfig.Name = config.Name + "（复制）";
            cloneConfig.Code = config.Code + "（Copy）";
            cloneConfig.Layout = config.Layout;
            cloneConfig.ModifyDate = DateTime.Now;
            cloneConfig.CreateDate = DateTime.Now;
            cloneConfig.ModifyUser = user.UserID;
            cloneConfig.ModifyUserName = user.UserName;
            cloneConfig.CreateUserName = user.UserName;
            cloneConfig.CreateUser = user.UserID;
            cloneConfig.ID = FormulaHelper.CreateGuid();
            this.entities.Set<S_UI_BIConfig>().Add(cloneConfig);
            this.entities.SaveChanges();
            return Json(new { ID = cloneConfig.ID });
        }

    }
}
