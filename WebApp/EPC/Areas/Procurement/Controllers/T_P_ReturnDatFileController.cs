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
    public class ReturnDatFileController : EPCFormContorllor<T_P_ReturnDatFile>
    {
        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "BomDetail")
            {
                var bomID = detail.GetValue("BomID");
                var pBom = this.GetEntityByID<S_P_Bom>(bomID);
                pBom.ReturnDatPathID = dic.GetValue("ID");
                if (!String.IsNullOrEmpty(dic.GetValue("ReturnDatFile")))
                {
                    pBom.ReturnDatFile = dic.GetValue("ReturnDatFile");
                    pBom.ReturnDatFileName = dic.GetValue("ReturnDatFileName");
                }
            }
        }

        protected override void OnFlowEnd(T_P_ReturnDatFile entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                var bomDetails = entity.T_P_ReturnDatFile_BomDetail.ToList();
                foreach (var item in bomDetails)
                {
                    var pBom = this.GetEntityByID<S_P_Bom>(item.BomID);
                    if (pBom != null)
                    {                
                        if (!String.IsNullOrEmpty(item.ReturnDatFile))
                        {
                            pBom.ReturnDatFile = item.ReturnDatFile;
                            pBom.ReturnDatFileName = item.ReturnDatFileName;
                        }                      
                        pBom.ReturnDatState = ProjectState.Finish.ToString();
                        if (!pBom.FactReturnDate.HasValue)
                        {
                            pBom.FactReturnDate = entity.ReturnDatDate;
                        }                        
                    }
                }
                this.EPCEntites.SaveChanges();
            }
        }
    }
}
