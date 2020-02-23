using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    /// <summary>
    /// 管理工装申领
    /// </summary>
    public class WorkClothesManageReceiveController : OfficeAutoFormContorllor<T_WorkClothes_ManageReceive>
    {

        protected override void OnFlowEnd(T_WorkClothes_ManageReceive entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                //流程结束之后更新库存数量
                var lstDetail = BusinessEntities.Set<T_WorkClothes_ManageReceive_ReceiveDetail>().Where(x => x.T_WorkClothes_ManageReceiveID == entity.ID).ToList();

                var stock = BusinessEntities.Set<T_WorkClothes_Stock>().FirstOrDefault();
                var lstStockDetail = BusinessEntities.Set<T_WorkClothes_Stock_ClothesDetail>().Where(x => x.T_WorkClothes_StockID == stock.ID).ToList();

                if (entity.Location == "天津")
                    lstDetail.ForEach(x =>
                    {
                        var detailModel = lstStockDetail.FirstOrDefault(s => s.Location == "天津" && s.Category == x.Category);

                        if (detailModel == null)
                        {
                            //工装申领理论上库存表一定存在  
                            BusinessEntities.Set<T_WorkClothes_Stock_ClothesDetail>().Add(new T_WorkClothes_Stock_ClothesDetail
                            {
                                Category = x.Category,
                                Location = "天津",
                                Size155Quantity = Convert.ToInt32(x.Size155Quantity).ToString(),
                                Size160Quantity = Convert.ToInt32(x.Size160Quantity).ToString(),
                                Size165Quantity = Convert.ToInt32(x.Size165Quantity).ToString(),
                                Size170Quantity = Convert.ToInt32(x.Size170Quantity).ToString(),
                                Size175Quantity = Convert.ToInt32(x.Size175Quantity).ToString(),
                                Size180Quantity = Convert.ToInt32(x.Size180Quantity).ToString(),
                                Size185Quantity = Convert.ToInt32(x.Size185Quantity).ToString(),
                                TotalQuantity = Convert.ToInt32(x.TotalQuantity).ToString(),
                                T_WorkClothes_StockID = stock.ID,
                                ID = Formula.FormulaHelper.CreateGuid()

                            });
                        }
                        else
                        {
                            //减少库存
                            detailModel.Size155Quantity = (Convert.ToInt32(detailModel.Size155Quantity) - Convert.ToInt32(x.Size155Quantity)).ToString();
                            detailModel.Size160Quantity = (Convert.ToInt32(detailModel.Size160Quantity) - Convert.ToInt32(x.Size160Quantity)).ToString();
                            detailModel.Size165Quantity = (Convert.ToInt32(detailModel.Size165Quantity) - Convert.ToInt32(x.Size165Quantity)).ToString();
                            detailModel.Size170Quantity = (Convert.ToInt32(detailModel.Size170Quantity) - Convert.ToInt32(x.Size170Quantity)).ToString();
                            detailModel.Size175Quantity = (Convert.ToInt32(detailModel.Size175Quantity) - Convert.ToInt32(x.Size175Quantity)).ToString();
                            detailModel.Size180Quantity = (Convert.ToInt32(detailModel.Size180Quantity) - Convert.ToInt32(x.Size180Quantity)).ToString();
                            detailModel.Size185Quantity = (Convert.ToInt32(detailModel.Size185Quantity) - Convert.ToInt32(x.Size185Quantity)).ToString();
                            detailModel.TotalQuantity = (Convert.ToInt32(detailModel.TotalQuantity) - Convert.ToInt32(x.TotalQuantity)).ToString();
                        }


                    });

                if (entity.Location == "洛阳")
                    lstDetail.ForEach(x =>
                {
                    var detailModel = lstStockDetail.FirstOrDefault(s => s.Location == "洛阳" && s.Category == x.Category);

                    if (detailModel == null)
                    {
                        //工装申领理论上库存表一定存在  
                        BusinessEntities.Set<T_WorkClothes_Stock_ClothesDetail>().Add(new T_WorkClothes_Stock_ClothesDetail
                        {
                            Category = x.Category,
                            Location = "洛阳",
                            Size155Quantity = Convert.ToInt32(x.Size155Quantity).ToString(),
                            Size160Quantity = Convert.ToInt32(x.Size160Quantity).ToString(),
                            Size165Quantity = Convert.ToInt32(x.Size165Quantity).ToString(),
                            Size170Quantity = Convert.ToInt32(x.Size170Quantity).ToString(),
                            Size175Quantity = Convert.ToInt32(x.Size175Quantity).ToString(),
                            Size180Quantity = Convert.ToInt32(x.Size180Quantity).ToString(),
                            Size185Quantity = Convert.ToInt32(x.Size185Quantity).ToString(),
                            TotalQuantity = Convert.ToInt32(x.TotalQuantity).ToString(),
                            T_WorkClothes_StockID = stock.ID,
                            ID = Formula.FormulaHelper.CreateGuid()

                        });
                    }
                    else
                    {
                        //减少库存
                        detailModel.Size155Quantity = (Convert.ToInt32(detailModel.Size155Quantity) - Convert.ToInt32(x.Size155Quantity)).ToString();
                        detailModel.Size160Quantity = (Convert.ToInt32(detailModel.Size160Quantity) - Convert.ToInt32(x.Size160Quantity)).ToString();
                        detailModel.Size165Quantity = (Convert.ToInt32(detailModel.Size165Quantity) - Convert.ToInt32(x.Size165Quantity)).ToString();
                        detailModel.Size170Quantity = (Convert.ToInt32(detailModel.Size170Quantity) - Convert.ToInt32(x.Size170Quantity)).ToString();
                        detailModel.Size175Quantity = (Convert.ToInt32(detailModel.Size175Quantity) - Convert.ToInt32(x.Size175Quantity)).ToString();
                        detailModel.Size180Quantity = (Convert.ToInt32(detailModel.Size180Quantity) - Convert.ToInt32(x.Size180Quantity)).ToString();
                        detailModel.Size185Quantity = (Convert.ToInt32(detailModel.Size185Quantity) - Convert.ToInt32(x.Size185Quantity)).ToString();
                        detailModel.TotalQuantity = (Convert.ToInt32(detailModel.TotalQuantity) - Convert.ToInt32(x.TotalQuantity)).ToString();
                    }

                });

                BusinessEntities.SaveChanges();

            }

            base.OnFlowEnd(entity, taskExec, routing);

        }


    }
}
