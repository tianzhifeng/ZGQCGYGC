using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    /// <summary>
    /// 防护用户入库
    /// </summary>
    public class ProtectionArticleEntryController : OfficeAutoFormContorllor<T_ProtectionArticle_Register>
    {
        //
        // GET: /AutoForm/ProtectionArticleEntry/

        protected override void OnFlowEnd(T_ProtectionArticle_Register entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                //流程结束之后更新库存数量
                var ledgerModel = BusinessEntities.Set<T_ProtectionArticle_Ledger>().FirstOrDefault();
                if (ledgerModel != null)
                {
                    ledgerModel.SafetyHatQuantity = (Convert.ToInt32(ledgerModel.SafetyHatQuantity) + Convert.ToInt32(entity.SafetyHatQuantity)).ToString();
                    ledgerModel.SafetyShoesQuantity = (Convert.ToInt32(ledgerModel.SafetyShoesQuantity) + Convert.ToInt32(entity.SafetyShoesQuantity)).ToString();

                    var lstShoes = BusinessEntities.Set<T_ProtectionArticle_Register_SafetyShoeDetail>().Where(x => x.T_ProtectionArticle_RegisterID == entity.ID).ToList();
                    ledgerModel.Size35Quantity = (Convert.ToInt32(ledgerModel.Size35Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size35))).ToString();
                    ledgerModel.Size36Quantity = (Convert.ToInt32(ledgerModel.Size36Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size36))).ToString();
                    ledgerModel.Size37Quantity = (Convert.ToInt32(ledgerModel.Size37Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size37))).ToString();
                    ledgerModel.Size38Quantity = (Convert.ToInt32(ledgerModel.Size38Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size38))).ToString();
                    ledgerModel.Size39Quantity = (Convert.ToInt32(ledgerModel.Size39Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size39))).ToString();
                    ledgerModel.Size40Quantity = (Convert.ToInt32(ledgerModel.Size40Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size40))).ToString();
                    ledgerModel.Size41Quantity = (Convert.ToInt32(ledgerModel.Size41Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size41))).ToString();
                    ledgerModel.Size42Quantity = (Convert.ToInt32(ledgerModel.Size42Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size42))).ToString();
                    ledgerModel.Size43Quantity = (Convert.ToInt32(ledgerModel.Size43Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size43))).ToString();
                    ledgerModel.Size44Quantity = (Convert.ToInt32(ledgerModel.Size44Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size44))).ToString();
                    ledgerModel.Size45Quantity = (Convert.ToInt32(ledgerModel.Size45Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size45))).ToString();
                    ledgerModel.Size46Quantity = (Convert.ToInt32(ledgerModel.Size46Quantity) + lstShoes.Sum(x => Convert.ToInt32(x.Size46))).ToString();
                }
                

                BusinessEntities.SaveChanges();

            }

            base.OnFlowEnd(entity, taskExec, routing);

        }

    }
}
