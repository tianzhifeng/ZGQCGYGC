using Base.Logic.Domain;
using EPC.Logic.Domain;
using Formula;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Manage.Controllers
{
    public class CustomerClassificationController : EPCFormContorllor<T_Market_CustomerClassification>
    {
        //
        // GET: /Manage/CustomerClassification/

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (dic != null && dic.ContainsKey("CustomerIDs") && dic.ContainsKey("CustomerClassification")) {
                string ids = dic["CustomerIDs"];
                string value = dic["CustomerClassification"];
                if (!string.IsNullOrEmpty(ids)) {
                    ids = ids.Trim(',');
                    string[] eids = ids.Split(',');
                    EPCEntities epcentity = FormulaHelper.GetEntities<EPCEntities>();
                    var ctms=epcentity.Set<S_M_CustomerInfo>().Where(c => eids.Contains(c.ID));
                    foreach (S_M_CustomerInfo item in ctms) {
                        item.CustomerClassification = value;
                    }
                    epcentity.SaveChanges();
                }
            }

        }

    }
}
