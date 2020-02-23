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


namespace EPC.Areas.Design.Controllers
{
    public class BomFetchDataController : EPCFormContorllor<T_E_BomFetchData>
    {
        protected override void OnFlowEnd(T_E_BomFetchData entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                foreach (var item in entity.T_E_BomFetchData_BomDetail.ToList())
                {
                    var pbom = this.EPCEntites.Set<S_P_Bom>().FirstOrDefault(c => c.ID == item.BomID);
                    if (pbom == null) continue;
                    pbom.Attachment = item.FetchData;
                    pbom.AttachmentName = item.FetchDataName; 
                    pbom.FetchPacthID = entity.ID;
                    pbom.FactFetchDate = entity.FetchFinishDate;
                    pbom.FetchState = ProjectState.Finish.ToString();
                }
                this.EPCEntites.SaveChanges();
            }
        }
    }
}
