using EPC;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;
using Config.Logic;
using Formula.Helper;

namespace EPC.Areas.Warehouse.Controllers
{
    public class WarehouseInventoryController : EPCFormContorllor<T_W_WarehouseInventory>
    {
        protected override void OnFlowEnd(T_W_WarehouseInventory entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            var equipmentDetail = entity.T_W_WarehouseInventory_EquipmentDetail.ToList();
            var equipmentAccount = this.EPCEntites.Set<S_W_EquipmentAccount>().Where(a => a.StorageInfoID == entity.StorageInfo).ToList();
            var storageAccounts = this.EPCEntites.Set<S_W_EquipmentAccount>().Where(a => a.StorageInfoID == entity.StorageInfo).ToList();
            var latestEquipments = this.EPCEntites.Set<S_W_LatestEquipment>().Where(a => a.StorageInfoID == entity.StorageInfo).ToList();

            foreach (var equipment in equipmentDetail)
            {
                if (equipment.InventoryCount != 0)
                {
                    var account = equipmentAccount.FirstOrDefault(a => a.Code == equipment.Code && a.Name == equipment.Name && a.Supplier == equipment.Supplier);

                    var detailSql = string.Format("select Top 1 * from T_W_WarehouseInventory_EquipmentDetail where ID = '{0}'", equipment.ID);
                    //直接把查询的数据转成 S_W_EquipmentAccount
                    var detail = this.EPCSQLDB.ExecuteObject<S_W_EquipmentAccount>(detailSql);
                    detail.ID = FormulaHelper.CreateGuid();
                    detail.FillDate = entity.FillDate;
                    detail.FillUser = entity.FillUser;
                    detail.FillUserName = entity.FillUserName;
                    detail.StorageInfoID = entity.StorageInfo;
                    detail.FormID = entity.ID;

                    detail.Specification = account.Specification;
                    detail.Branding = account.Branding;
                    detail.Material = account.Material;
                    detail.MaterialSub = account.MaterialSub;
                    detail.Supplier = account.Supplier;
                    detail.SupplierName = account.SupplierName;
                    detail.ExpirationDate = null;
                    detail.RootID = account.RootID;
                    detail.PBomID = account.PBomID;

                    detail.ExchangeRate = 1;
                    detail.RMBTaxRate = 0;

                    detail.Account = equipment.InventoryCount;
                    detail.LeftCount = detail.Account;
                    detail.UnitPrice = equipment.InventoryPrice / equipment.InventoryCount;
                    detail.TotalPrice = equipment.InventoryPrice;
                    detail.RMBTotalPrice = detail.TotalPrice * detail.ExchangeRate;
                    detail.RMBTaxPrice = 0;
                    detail.RMBTaxOutPrice = detail.RMBTotalPrice - detail.RMBTaxPrice;
                    if (equipment.InventoryCount > 0)
                        detail.Type = EquipmentAccountType.Profit.ToString();
                    else
                        detail.Type = EquipmentAccountType.Deficit.ToString();

                    this.EPCEntites.Set<S_W_EquipmentAccount>().Add(detail);

                    storageAccounts.Add(detail);
                    var latest = latestEquipments.FirstOrDefault(a => a.Supplier == detail.Supplier
                        && a.Code == detail.Code && a.Name == detail.Name && a.ExpirationDate == detail.ExpirationDate && a.PBomID == detail.PBomID);
                    if (latest != null)
                    {
                        var list = storageAccounts.Where(a => a.Supplier == latest.Supplier && a.Code == latest.Code
                            && a.Name == latest.Name && a.ExpirationDate == latest.ExpirationDate && a.PBomID == latest.PBomID).ToList();
                        decimal count = 0, total = 0, totalRMB = 0, tax = 0, taxOut = 0;
                        foreach (var item in list)
                        {
                            count += (decimal)item.Account;
                            total += (decimal)item.Account * (decimal)item.UnitPrice;
                            totalRMB += (decimal)item.Account * (decimal)item.UnitPrice * (decimal)item.ExchangeRate;
                            tax += (decimal)(item.Account > 0 ? item.RMBTaxPrice : (0 - item.RMBTaxPrice));
                            taxOut += (decimal)(item.Account > 0 ? item.RMBTaxOutPrice : (0 - item.RMBTaxOutPrice));
                        }
                        latest.Account = count;
                        latest.TotalPrice = Math.Round(total, 2);
                        latest.RMBTotalPrice = Math.Round(totalRMB, 2);
                        latest.RMBTaxPrice = Math.Round(tax, 2);
                        latest.RMBTaxOutPrice = Math.Round(taxOut, 2);
                        latest.UnitPrice = latest.Account == 0 ? latest.UnitPrice : Math.Round((decimal)(latest.TotalPrice / latest.Account), 2);
                    }
                }
            }
            var inventoryInfo = this.GetEntityByID<S_W_InventoryInfo>(entity.InventoryInfoID);
            if (inventoryInfo != null) inventoryInfo.State = InventoryState.Handled.ToString();
            this.EPCEntites.SaveChanges();
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                var inventoryInfo = this.GetEntityByID<S_W_InventoryInfo>(dic.GetValue("InventoryInfoID"));
                if (inventoryInfo != null) inventoryInfo.State = InventoryState.Processing.ToString();
                this.EPCEntites.SaveChanges();
            }
        }

        public JsonResult GetEquipmentAccount(string StorageInfo)
        {
            var sql = string.Format(@"select a.*,ISNULL(b.[Count], 0) LastCount,ISNULL(b.Price, 0) LastPrice
from (select Code,Name,Model,Specification,Branding,Material,MaterialSub,
Supplier,SupplierName,Unit,SUM(Account)SumAccount,SUM(Account*UnitPrice) SumTotalPrice,SUM(RMBTotalPrice) SumRMBTotalPrice
from S_W_LatestEquipment where StorageInfoID ='{0}' and Account > 0
GROUP BY StorageInfoID,Code,Name,Model,Specification,Branding,Material,MaterialSub,Supplier,SupplierName,Unit
HAVING SUM(Account)>0) a
left join (select * from S_W_InventoryInfo_EquipmentDetail where S_W_InventoryInfoID
in (select top 1 ID from S_W_InventoryInfo where StorageInfo='{0}' ORDER BY CreateDate DESC)
) b on b.Code = a.Code and b.Name = a.Name and b.Supplier = a.Supplier", StorageInfo);
            var list = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(list);
        }

        public JsonResult SetState(string InventoryID)
        {
            var inventory = this.GetEntityByID<S_W_InventoryInfo>(InventoryID);
            if (inventory == null) throw new Formula.Exceptions.BusinessException("找不到指定的库存盘点");
            inventory.State = InventoryState.Publish.ToString();
            this.EPCEntites.SaveChanges();
            return Json("");
        }
    }
}