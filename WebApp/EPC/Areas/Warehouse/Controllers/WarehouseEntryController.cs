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
    public class WarehouseEntryController : EPCFormContorllor<T_W_WarehouseEntry>
    {
        protected override void OnFlowEnd(T_W_WarehouseEntry entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            var equipmentDetail = entity.T_W_WarehouseEntry_EquipmentDetail.ToList();
            var storageAccounts = this.EPCEntites.Set<S_W_EquipmentAccount>().Where(a => a.StorageInfoID == entity.StorageInfo).ToList();
            var latestEquipments = this.EPCEntites.Set<S_W_LatestEquipment>().Where(a => a.StorageInfoID == entity.StorageInfo).ToList();

            if (entity.Type != EntryType.Allot.ToString())
            {
                #region 采购
                var detailSql = string.Format("select * from T_W_WarehouseEntry_EquipmentDetail where T_W_WarehouseEntryID = '{0}'", entity.ID);
                //直接把查询的数据转成 List<S_W_EquipmentAccount>
                var details = this.EPCSQLDB.ExecuteList<S_W_EquipmentAccount>(detailSql);
                foreach (var detail in details)
                {
                    detail.FillDate = entity.FillDate;
                    detail.FillUser = entity.FillUser;
                    detail.FillUserName = entity.FillUserName;
                    detail.StorageInfoID = entity.StorageInfo;
                    detail.Type = EquipmentAccountType.Entry.ToString();
                    detail.FormID = entity.ID;
                    detail.RootID = detail.ID;
                    detail.ExchangeRate = entity.ExchangeRate;
                    detail.PBomID = string.IsNullOrEmpty(detail.PBomID) ? "" : detail.PBomID;
                    var equipment = equipmentDetail.FirstOrDefault(a => a.ID == detail.ID);
                    if (equipment != null)
                    {
                        detail.Account = equipment.Count;
                        detail.LeftCount = equipment.Count;
                        //修改合同设备清单的到货数
                        var content = this.EPCEntites.Set<S_P_ContractInfo_Content>().FirstOrDefault(a => a.ID == equipment.ContentID);
                        if (content != null)
                        {
                            content.YetCount = this.EPCEntites.Set<T_W_WarehouseEntry_EquipmentDetail>().
                                Where(a => a.ContentID == content.ID).ToList().Sum(a => a.Count);
                        }

                        var pbom = this.EPCEntites.Set<S_P_Bom>().FirstOrDefault(a => a.ID == equipment.PBomID);
                        if (pbom != null)
                        {
                            if (string.IsNullOrEmpty(detail.Specification)) detail.Specification = pbom.Specification;
                            if (string.IsNullOrEmpty(detail.Branding)) detail.Branding = pbom.Branding;
                            if (string.IsNullOrEmpty(detail.Material)) detail.Material = pbom.Material;
                            if (string.IsNullOrEmpty(detail.MaterialSub)) detail.MaterialSub = pbom.MaterialSub;
                        }
                    }
                    this.EPCEntites.Set<S_W_EquipmentAccount>().Add(detail);

                    storageAccounts.Add(detail);
                    var latest = latestEquipments.FirstOrDefault(a => a.Supplier == detail.Supplier
                        && a.Code == detail.Code && a.Name == detail.Name && a.ExpirationDate == detail.ExpirationDate && a.PBomID == detail.PBomID);
                    if (latest == null)
                    {
                        latest = new S_W_LatestEquipment
                        {
                            ID = FormulaHelper.CreateGuid(),
                            StorageInfoID = detail.StorageInfoID,
                            Code = detail.Code,
                            Name = detail.Name,
                            Model = detail.Model,
                            Specification = detail.Specification,
                            Branding = detail.Branding,
                            Material = detail.Material,
                            MaterialSub = detail.MaterialSub,
                            Supplier = detail.Supplier,
                            SupplierName = detail.SupplierName,
                            Unit = detail.Unit,
                            ExpirationDate = detail.ExpirationDate,
                            PBomID = detail.PBomID
                        };
                        this.EPCEntites.Set<S_W_LatestEquipment>().Add(latest);
                    }
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
                #endregion
            }
            else
            {
                #region 调拨
                var allotID = entity.RelateForm;
                var equipmentAccounts = this.EPCEntites.Set<S_W_EquipmentAccount>().Where(a => a.FormID == allotID).ToList();
                foreach (var equipmentAccount in equipmentAccounts)
                {
                    var newAccount = equipmentAccount.Clone<S_W_EquipmentAccount>();
                    newAccount.ID = FormulaHelper.CreateGuid();
                    newAccount.FillDate = entity.FillDate;
                    newAccount.FillUser = entity.FillUser;
                    newAccount.FillUserName = entity.FillUserName;
                    newAccount.RootID = equipmentAccount.RootID;
                    newAccount.StorageInfoID = entity.StorageInfo;
                    newAccount.Account = -newAccount.Account;
                    newAccount.LeftCount = newAccount.Account;
                    newAccount.Type = EquipmentAccountType.Entry.ToString();
                    newAccount.FormID = entity.ID;

                    this.EPCEntites.Set<S_W_EquipmentAccount>().Add(newAccount);

                    storageAccounts.Add(newAccount);
                    var latest = latestEquipments.FirstOrDefault(a => a.Supplier == newAccount.Supplier
                        && a.Code == newAccount.Code && a.Name == newAccount.Name && a.ExpirationDate == newAccount.ExpirationDate && a.PBomID == newAccount.PBomID);
                    if (latest == null)
                    {
                        latest = new S_W_LatestEquipment
                        {
                            ID = FormulaHelper.CreateGuid(),
                            StorageInfoID = newAccount.StorageInfoID,
                            Code = newAccount.Code,
                            Name = newAccount.Name,
                            Model = newAccount.Model,
                            Specification = newAccount.Specification,
                            Branding = newAccount.Branding,
                            Material = newAccount.Material,
                            MaterialSub = newAccount.MaterialSub,
                            Supplier = newAccount.Supplier,
                            SupplierName = newAccount.SupplierName,
                            Unit = newAccount.Unit,
                            ExpirationDate = newAccount.ExpirationDate,
                            PBomID = newAccount.PBomID
                        };
                        this.EPCEntites.Set<S_W_LatestEquipment>().Add(latest);
                    }
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
                #endregion
            }

            //回写Bom的物资编码
            foreach (var content in entity.T_W_WarehouseEntry_EquipmentDetail)
            {
                var bom = this.GetEntityByID<S_P_Bom>(content.PBomID);
                if (bom != null)
                {
                    if (!string.IsNullOrEmpty(content.Code) && content.Code != bom.Number)
                        bom.Number = content.Code;
                }
            }

            this.EPCEntites.SaveChanges();
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (decimal.Parse(detail.GetValue("Count")) <= 0)
                throw new Formula.Exceptions.BusinessException("入库数量必须大于零，请重新输入");
            if (dic.GetValue("Type").ToString() == EntryType.Allot.ToString())
            {
                var allotDetailID = detail.GetValue("ContentID").ToString();
                var id = detail.GetValue("ID").ToString();
                var count = detail.GetValue("Count").ToString();
                var allotDetail = this.EPCEntites.Set<T_W_WarehouseAllot_EquipmentDetail>().FirstOrDefault(a => a.ID == allotDetailID);
                if (allotDetail == null)
                    throw new Formula.Exceptions.BusinessException("未能找到此调拨单的设备明细");
                var list = this.EPCEntites.Set<T_W_WarehouseEntry_EquipmentDetail>().Where(a => a.ContentID == allotDetailID && a.ID != id).ToList();
                var sum = list.Sum(a => a.Count) ?? 0;
                if (sum + decimal.Parse(count) > allotDetail.Count)
                    throw new Formula.Exceptions.BusinessException("[" + detail.GetValue("Name").ToString() + "]设备的累计入库数量(" + sum + decimal.Parse(count) + ")不能大于调拨数量(" + allotDetail.Count + ")");
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var engineering = dic.GetValue("Engineering");
            var engineeringInfoID = dic.GetValue("EngineeringInfoID");
            if (!string.IsNullOrEmpty(engineering) && string.IsNullOrEmpty(engineeringInfoID))
                dic.SetValue("EngineeringInfoID", engineering);
            decimal sum = 0;
            var detailList = JsonHelper.ToList(dic.GetValue("EquipmentDetail"));
            foreach (var detail in detailList)
                sum += decimal.Parse(detail.GetValue("TotalPrice"));
            dic.SetValue("TotalPrice", sum.ToString());
        }

        public JsonResult GetArrivalDetail(string ArrivalID)
        {
            var sql = string.Format(@"
select S_P_Arrival_DetailInfo.*,UnitPrice,Content.Supplier,Content.SupplierName,
(UnitPrice*Quantity) TotalPrice,(UnitPrice*Quantity*ExchangeRate) RMBTotalPrice,RMBTaxRate,
ExchangeRate from S_P_Arrival_DetailInfo
left join (select S_P_ContractInfo_Content.*,PartyB Supplier,PartyBName SupplierName,ExchangeRate from 
S_P_ContractInfo_Content left join S_P_ContractInfo 
on S_P_ContractInfo_Content.S_P_ContractInfoID = S_P_ContractInfo.ID
)Content on S_P_Arrival_DetailInfo.BomInfo = Content.ID
where S_P_ArrivalID = '{0}'", ArrivalID);
            var list = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(list);
        }

        public JsonResult GetAllotDetail(string allotID)
        {
            var sql = string.Format("select * from T_W_WarehouseAllot_EquipmentDetail where T_W_WarehouseAllotID = '{0}'", allotID);
            var list = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(list);
        }
    }
}