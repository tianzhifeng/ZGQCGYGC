using EPC.Logic.Domain;
using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Manage.Controllers
{
    public class DemandTaskNoticeViewController : EPCFormContorllor<T_I_DemandTaskNotice>
    {
        //
        // GET: /Manage/DemandTaskNoticeView/
        public JsonResult SetSubsidy(string ids, string val)
        {
            if (!string.IsNullOrEmpty(ids) && !string.IsNullOrEmpty(val))
            {
                ids = ids.Trim(',');
                string[] strid = ids.Split(',');
                EPCEntities epcent = FormulaHelper.GetEntities<EPCEntities>();
                var lists = epcent.Set<T_I_DemandTaskNotice>().Where(c => ids.Contains(c.ID));
                foreach (T_I_DemandTaskNotice item in lists)
                {
                    item.IsSubsidy = val;
                }
                epcent.SaveChanges();
            }
            return Json("");
        }
    }
}
