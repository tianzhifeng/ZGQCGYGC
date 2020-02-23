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


namespace EPC.Areas.Procurement.Controllers
{
    public class ArrivalController : EPCFormContorllor<S_P_Arrival>
    {
        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic.GetValue("ID"));
            entity.SentToPBom();
            this.EPCEntites.SaveChanges();

            var pbomids = entity.S_P_Arrival_DetailInfo.ToList().Select(a => a.PBomID);
            var boms = this.EPCEntites.Set<S_P_Bom>().Where(a => pbomids.Contains(a.ID)).ToList();
            foreach (var bom in boms)
                if (bom.FactArrivedDate == null)
                    bom.FactArrivedDate = entity.ArrivalDate;
            this.EPCEntites.SaveChanges();
        }

        protected override void BeforeDelete(string[] Ids)
        {
            foreach (var id in Ids)
            {
                var item = this.GetEntityByID(id);
                if (item == null)
                    continue;
                item.ClearPBom();
            }
        }
    }
}
