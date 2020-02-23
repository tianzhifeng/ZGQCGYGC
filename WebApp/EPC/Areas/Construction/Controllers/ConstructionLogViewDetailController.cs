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
using Base.Logic.Domain;

namespace EPC.Areas.Construction.Controllers
{
    public class ConstructionLogViewDetailController : EPCController
    {
        private EPCEntities dbContext = FormulaHelper.GetEntities<EPCEntities>();
        //
        // GET: /ConstructionManagement/ConstructionLogViewDetail/

        public ActionResult Index()
        {
            return View();
        }

        public void AddOrUpdate(string id,string userID,string userName)
        {
            T_C_ConstructionReports_ViewDetails vd = dbContext.T_C_ConstructionReports_ViewDetails.Where(m => m.T_C_ConstructionReportsID == id&&m.ViewUserID== userID).FirstOrDefault();
            if (vd != null)
            {
                vd.ViewTime = DateTime.Now;
            }
            else
            {
                T_C_ConstructionReports_ViewDetails newVD = new T_C_ConstructionReports_ViewDetails();
                newVD.ID = FormulaHelper.CreateGuid();
                newVD.T_C_ConstructionReportsID = id;
                newVD.ViewTime = DateTime.Now;
                newVD.ViewUserID = userID;
                newVD.ViewUserName = userName;
                dbContext.T_C_ConstructionReports_ViewDetails.Add(newVD);
            }
            dbContext.SaveChanges();
        }

    }
}
