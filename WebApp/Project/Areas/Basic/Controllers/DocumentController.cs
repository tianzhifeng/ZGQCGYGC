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

namespace Project.Areas.Basic.Controllers
{
    public class DocumentController : ProjectController<S_D_Document>
    {
        public ActionResult DocumentEdit()
        {
            var ArchiveType= System.Configuration.ConfigurationManager.AppSettings["ArchiveType"];
            ViewBag.ArchiveType = string.IsNullOrEmpty(ArchiveType) ? "PdfFile" : ArchiveType;
            return View();
        }

        protected override void BeforeSave(S_D_Document entity, bool isNew)
        {
            if (isNew)
            {
                entity.State = "Normal";
                var dbsInfo = this.GetEntityByID<S_D_DBS>(entity.DBSID);
                if (dbsInfo == null) throw new Formula.Exceptions.BusinessException("DBS节点不存在，请联系管理员！");
                entity.DBSFullID = dbsInfo.FullID;
            }
        }

        protected override void AfterSave(S_D_Document entity, bool isNew)
        {
            //entity.AddDocumentVersion();
            this.entities.SaveChanges();
            base.AfterSave(entity, isNew);
        }
    }
}
