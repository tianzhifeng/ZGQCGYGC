using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;

namespace EPC.Areas.Contract.Controllers
{
    public class ReceiptRegisterController : EPCFormContorllor<S_M_ReceiptRegister>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (!isNew)
            {
                var entity = this.GetEntityByID(dic.GetValue("ID"));
                if (entity == null) entity = new S_M_ReceiptRegister();
                this.UpdateEntity(entity, dic);
                if (this.EPCEntites.Set<S_M_Receipt>().Count(d => d.RegisterID == entity.ID) > 0)
                {
                    var sumValue = this.EPCEntites.Set<S_M_Receipt>().Where(d => d.RegisterID == entity.ID).Sum(d => d.ReceiptValue);
                    //if (entity.ReceiptValue < sumValue)
                    //    throw new Formula.Exceptions.BusinessValidationException("收款金额不能小于已认领金额【" + sumValue + "】");
                }
            }
        }

        public override JsonResult Delete()
        {
            if (!String.IsNullOrEmpty(Request["ListIDs"]))
            {
                var ids = Request["ListIDs"].Split(',');
                foreach (var Id in ids)
                {
                    var entity = this.GetEntityByID(Id);
                    if (entity == null) continue;
                    entity.Delete();
                }
            }
            this.EPCEntites.SaveChanges();
            return Json("");
        }

        public JsonResult GetReceiptInfo(string RegisterID)
        {
            var list =  this.EPCEntites.Set<S_M_Receipt>().Where(d => d.RegisterID == RegisterID).OrderBy(d=>d.ID).ToList();
            return Json(list);
        }

        public JsonResult Confirm(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var register = this.GetEntityByID(item.GetValue("ID"));
                if (register == null) continue;
                var receiptList = this.EPCEntites.Set<S_M_Receipt>().Where(d => d.RegisterID == register.ID).ToList();
                foreach (var receipt in receiptList)
                {
                    receipt.State = "Normal";
                }
            }
            this.EPCEntites.SaveChanges();
            return Json("");
        }
    }
}
