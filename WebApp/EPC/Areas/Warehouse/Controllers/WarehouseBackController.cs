﻿using EPC;
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
    public class WarehouseBackController : EPCFormContorllor<T_W_WarehouseBack>
    {
        protected override void OnFlowEnd(T_W_WarehouseBack entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            var equipmentDetail = entity.T_W_WarehouseBack_EquipmentDetail.ToList();
            var outID = entity.OutInfo;
            var equipmentAccount = this.EPCEntites.Set<S_W_EquipmentAccount>().Where(a => a.FormID == outID).ToList();
            var storageAccounts = this.EPCEntites.Set<S_W_EquipmentAccount>().Where(a => a.StorageInfoID == entity.StorageInfo).ToList();
            var latestEquipments = this.EPCEntites.Set<S_W_LatestEquipment>().Where(a => a.StorageInfoID == entity.StorageInfo).ToList();

            foreach (var equipment in equipmentDetail)
            {
                var accounts = new List<S_W_EquipmentAccount>();
                equipmentAccount.Where(a => a.ExpirationDate == null && a.Code == equipment.Code && a.Name == equipment.Name
                    && a.Supplier == equipment.Supplier).ToList().ForEach(a => accounts.Add(a));
                equipmentAccount.Where(a => a.ExpirationDate != null && a.Code == equipment.Code && a.Name == equipment.Name
                    && a.Supplier == equipment.Supplier).OrderByDescending(a => a.ExpirationDate).ToList().ForEach(a => accounts.Add(a));
                if (accounts.Count <= 0) continue;
                var accountSum = equipmentAccount.Sum(a => a.LeftCount);
                if (Math.Abs(accountSum == null ? 0 : (decimal)accountSum) < Math.Abs(equipment.Count == null ? 0 : (decimal)equipment.Count))
                    continue;
                var leftCount = -equipment.Count;
                while (leftCount < 0)
                {
                    var detailSql = string.Format("select Top 1 * from T_W_WarehouseBack_EquipmentDetail where ID = '{0}'", equipment.ID);
                    //直接把查询的数据转成 S_W_EquipmentAccount
                    var detail = this.EPCSQLDB.ExecuteObject<S_W_EquipmentAccount>(detailSql);

                    detail.ID = FormulaHelper.CreateGuid();
                    detail.FillDate = entity.FillDate;
                    detail.FillUser = entity.FillUser;
                    detail.FillUserName = entity.FillUserName;
                    detail.StorageInfoID = entity.StorageInfo;
                    detail.Type = EquipmentAccountType.Back.ToString();
                    detail.FormID = entity.ID;

                    detail.Specification = accounts[0].Specification;
                    detail.Branding = accounts[0].Branding;
                    detail.Material = accounts[0].Material;
                    detail.MaterialSub = accounts[0].MaterialSub;
                    detail.Supplier = accounts[0].Supplier;
                    detail.SupplierName = accounts[0].SupplierName;
                    detail.ExpirationDate = accounts[0].ExpirationDate;
                    detail.RootID = accounts[0].RootID;
                    detail.PBomID = accounts[0].PBomID;

                    detail.ExchangeRate = accounts[0].ExchangeRate;
                    detail.RMBTaxRate = accounts[0].RMBTaxRate;

                    if (leftCount <= accounts[0].LeftCount)
                    {
                        detail.Account = -accounts[0].LeftCount;
                        detail.LeftCount = detail.Account;
                        leftCount -= accounts[0].LeftCount;
                        detail.UnitPrice = accounts[0].UnitPrice;
                        detail.TotalPrice = Math.Abs((decimal)(detail.UnitPrice * detail.Account));
                        detail.RMBTotalPrice = detail.TotalPrice * detail.ExchangeRate;
                        detail.RMBTaxPrice = Math.Abs((decimal)(accounts[0].RMBTaxPrice / accounts[0].Account * detail.Account));
                        detail.RMBTaxOutPrice = detail.RMBTotalPrice - detail.RMBTaxPrice;
                        accounts[0].LeftCount = 0;
                        accounts.RemoveAt(0);
                    }
                    else
                    {
                        accounts[0].LeftCount = accounts[0].LeftCount - leftCount;
                        detail.Account = -leftCount;
                        detail.LeftCount = detail.Account;
                        leftCount = 0;
                        detail.UnitPrice = accounts[0].UnitPrice;
                        detail.TotalPrice = Math.Abs((decimal)(detail.UnitPrice * detail.Account));
                        detail.RMBTotalPrice = detail.TotalPrice * detail.ExchangeRate;
                        detail.RMBTaxPrice = Math.Abs((decimal)(accounts[0].RMBTaxPrice / accounts[0].Account * detail.Account));
                        detail.RMBTaxOutPrice = detail.RMBTotalPrice - detail.RMBTaxPrice;
                    }

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
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            var count = decimal.Parse(detail.GetValue("Count"));
            if (count <= 0)
                throw new Formula.Exceptions.BusinessException("退库数量必须大于零，请重新输入");
            var outCount = decimal.Parse(detail.GetValue("OutCount"));
            if (outCount < count)
                throw new Formula.Exceptions.BusinessException("退库数量不能大于出库数量，请重新输入");

            var OutInfo = dic.GetValue("OutInfo");
            var ID = dic.GetValue("ID");

            var code = detail.GetValue("Code");
            var name = detail.GetValue("Name");
            var supplier = detail.GetValue("Supplier");

            var equipmentAccount = this.EPCEntites.Set<S_W_EquipmentAccount>().Where(a => a.FormID == OutInfo
                && a.Code == code && a.Name == name && a.Supplier == supplier).ToList();
            if (string.IsNullOrEmpty(supplier))
                equipmentAccount = this.EPCEntites.Set<S_W_EquipmentAccount>().Where(a => a.FormID == OutInfo
                && a.Code == code && a.Name == name && string.IsNullOrEmpty(a.Supplier)).ToList();
            var accountCount = equipmentAccount.Sum(a => a.Account);

            var backInFlowIDs = this.EPCEntites.Set<T_W_WarehouseBack>().Where(a => a.OutInfo == OutInfo
                && a.FlowPhase != "End" && a.ID != ID).Select(b => b.ID).ToList();
            var backCount = this.EPCEntites.Set<T_W_WarehouseBack_EquipmentDetail>().Where(a => backInFlowIDs.Contains(a.ID)
                && a.Code == code && a.Name == name && a.Supplier == supplier).Sum(b => b.Count);

            if ((Math.Abs((decimal)accountCount) - (backCount == null ? 0 : backCount)) < count)
                throw new Formula.Exceptions.BusinessException("退库数量大于可退库数量，请重新输入");
        }

        public JsonResult GetOutDetail(string OutID)
        {
            var sql = string.Format("select * from T_W_WarehouseOut_EquipmentDetail where T_W_WarehouseOutID = '{0}'", OutID);
            var list = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(list);
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var engineering = dic.GetValue("Engineering");
            var engineeringInfoID = dic.GetValue("EngineeringInfoID");
            if (!string.IsNullOrEmpty(engineering) && string.IsNullOrEmpty(engineeringInfoID))
                dic.SetValue("EngineeringInfoID", engineering);
        }
    }
}