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
    public class DynamicQueryController : EPCController<S_W_LatestEquipment>
    {
        public JsonResult SumEquipment(string EngineeringInfoID)
        {
            var storages = this.entities.Set<S_W_StorageInfo>().Where(a => a.Engineering == EngineeringInfoID).ToList();
            foreach (var storage in storages)
            {
                var equipmentAccounts = this.entities.Set<S_W_EquipmentAccount>().Where(a => a.StorageInfoID == storage.ID).ToList();
                var latestEquipments = this.entities.Set<S_W_LatestEquipment>().Where(a => a.StorageInfoID == storage.ID).ToList();
                var equipments = equipmentAccounts.Select(a =>
                    new { Code = a.Code, Name = a.Name, Supplier = a.Supplier, SupplierName = a.SupplierName, ExpirationDate = a.ExpirationDate, PBomID = a.PBomID }).Distinct().ToList();
                foreach (var equipment in equipments)
                {
                    var latest = latestEquipments.FirstOrDefault(a => a.Supplier == equipment.Supplier
                        && a.Code == equipment.Code && a.Name == equipment.Name && a.ExpirationDate == equipment.ExpirationDate && a.PBomID == equipment.PBomID);
                    if (latest == null)
                    {
                        var account = equipmentAccounts.FirstOrDefault(a => a.Code == equipment.Code && a.Name == equipment.Name
                            && a.Supplier == equipment.Supplier && a.ExpirationDate == equipment.ExpirationDate && a.PBomID == equipment.PBomID);
                        latest = new S_W_LatestEquipment
                        {
                            ID = FormulaHelper.CreateGuid(),
                            StorageInfoID = account.StorageInfoID,
                            Code = account.Code,
                            Name = account.Name,
                            Model = account.Model,
                            Specification = account.Specification,
                            Branding = account.Branding,
                            Material = account.Material,
                            MaterialSub = account.MaterialSub,
                            Supplier = account.Supplier,
                            SupplierName = account.SupplierName,
                            Unit = account.Unit,
                            ExpirationDate = account.ExpirationDate,
                            PBomID = account.PBomID
                        };
                        this.entities.Set<S_W_LatestEquipment>().Add(latest);
                    }
                    var list = equipmentAccounts.Where(a => a.Supplier == latest.Supplier && a.Code == latest.Code
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
                    var type = EquipmentAccountType.Scrap.ToString();
                    var scrapCount = Math.Abs((decimal)list.Where(a => a.Type == type).Sum(a => a.Account));
                    latest.Account = count;
                    latest.TotalPrice = Math.Round(total, 2);
                    latest.RMBTotalPrice = Math.Round(totalRMB, 2);
                    latest.RMBTaxPrice = Math.Round(tax, 2);
                    latest.RMBTaxOutPrice = Math.Round(taxOut, 2);
                    latest.UnitPrice = latest.Account == 0 ? latest.UnitPrice : Math.Round((decimal)(latest.TotalPrice / latest.Account), 2);
                    latest.ScrapCount = scrapCount;
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}