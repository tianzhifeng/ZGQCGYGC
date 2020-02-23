using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using Formula.Helper;
using System.Text;
using System.Data;
using Formula.Exceptions;

namespace EPC.Areas.Procurement.Controllers
{
    public class ProcurementContractController : EPCFormContorllor<S_P_ContractInfo>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            var orgService = Formula.FormulaHelper.GetService<IOrgService>();
            var orgList = orgService.GetOrgs(Config.Constant.OrgRootID);
            var rootOrg = orgList.FirstOrDefault(d => d.ID == Config.Constant.OrgRootID);
            dic.SetValue("PartyA", rootOrg.ID);
            dic.SetValue("PartyAName", rootOrg.Name);
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            dic.SetValue("FullID", dic.GetValue("ID"));
            if (dic.GetValue("IsSubContract") == true.ToString().ToUpper())
            {
                dic.SetValue("FullID", dic.GetValue("ParentContract") + "." + dic.GetValue("ID"));
            }

            if (!string.IsNullOrEmpty(GetQueryString("State")))
            {
                dic.SetValue("ContractState", GetQueryString("State"));
            }
            var ContractAmount = 0.0m;
            decimal.TryParse(dic.GetValue("ContractAmount"), out ContractAmount);
            var contractID = dic.GetValue("ID");
            //验证合同金额不能超过已付款金额
            var sumApplyValue = this.EPCEntites.Set<S_P_Payment>().Where(a => a.ContractInfo == contractID).ToList().Sum(a => a.PaymentValue);//s表
            sumApplyValue += this.EPCEntites.Set<T_P_PaymentApply>().Where(a => a.ContractInfo == contractID && a.FlowPhase != "End").ToList().Sum(a => a.ApplyValue);//流程
            if (ContractAmount < sumApplyValue)
            {
                throw new Formula.Exceptions.BusinessValidationException("原币种合同额不能小于已付款金额【" + sumApplyValue + "】");
            }
            var invoiceApplyValue = this.EPCEntites.Set<S_P_Invoice>().Where(a => a.ContractInfo == contractID).ToList().Sum(a => a.InvoiceValue);
            if (ContractAmount < invoiceApplyValue)
            {
                throw new Formula.Exceptions.BusinessValidationException("原币种合同额不能小于收票金额【" + invoiceApplyValue + "】");
            }
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            //校验设备数量不能超过可采购数量
            if (subTableName == "Content")
            {
                var conid = dic.GetValue("ID");
                var bomid = detail.GetValue("PBomID");
                var number = detail.GetValue("Number");
                var bom = this.GetEntityByID<S_P_Bom>(bomid);
                if (bom == null)
                    return;
                if (dic.GetValue("ValidateBomQuantity") == true.ToString().ToLower())
                {
                    var qty = 0m; var remain = 0m;
                    var sum = this.EPCEntites.Set<S_P_ContractInfo_Content>().Where(a => a.S_P_ContractInfoID != conid && a.PBomID == bomid).Select(a => a.ContractQuantity).Sum();
                    if (!sum.HasValue) sum = 0;
                    if (bom.Quantity.HasValue) qty = bom.Quantity.Value;
                    remain = qty - sum.Value;
                    var value = String.IsNullOrEmpty(detail.GetValue("ContractQuantity")) ? 0m : Convert.ToDecimal(detail.GetValue("ContractQuantity"));
                    if (value > remain)
                        throw new Formula.Exceptions.BusinessValidationException("设备【" + bom.Name + "】的数量不能超过可采购数量【" + remain + "】");
                }

                //来自招标比选则不能超过招标比选的量
                if (dic.GetValue("ContractType") == "Contract")
                {
                    var device = EPCEntites.Set<S_P_Invitation_Device>().Find(detail.GetValue("ApplyDetailID"));
                    if (device == null)
                        throw new Formula.Exceptions.BusinessValidationException("未找到【" + bom.Name + "】的招标比价信息");

                    var thisQuantity = 0.0m;
                    if (!String.IsNullOrEmpty(detail.GetValue("ContractQuantity")))
                        thisQuantity = Convert.ToDecimal(detail.GetValue("ContractQuantity"));
                    var applyDetailID = detail.GetValue("ApplyDetailID");
                    var detailID = detail.GetValue("ID");
                    var contractQuantity = EPCEntites.Set<S_P_ContractInfo_Content>().Where(a => a.ID != detailID && a.ApplyDetailID == applyDetailID).Sum(a => a.ContractQuantity);
                    var restQuantiy = device.InvitationQuantity - contractQuantity;
                    if (restQuantiy < thisQuantity)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("设备【" + bom.Name + "】的数量不能超过招标可签订数量【" + restQuantiy + "】");
                    }
                }

                //不能小于已到货数量
                var quantity = 0.0m;
                if (!String.IsNullOrEmpty(detail.GetValue("ContractQuantity")))
                    quantity = Convert.ToDecimal(detail.GetValue("ContractQuantity"));

                var arrivalQuantity = 0.0m;
                if (!String.IsNullOrEmpty(detail.GetValue("ArrivalQuantity")))
                    arrivalQuantity = Convert.ToDecimal(detail.GetValue("ArrivalQuantity"));

                if (quantity < arrivalQuantity)
                {
                    throw new Formula.Exceptions.BusinessValidationException("设备【" + detail.GetValue("Name") + "】的数量不能小于到货数量【" + arrivalQuantity + "】");
                }
            }
            else if (subTableName == "PaymentObj")
            {
                var paymentID = detail.GetValue("ID");
                var PlanPaymentValue = 0.0m;
                var SumPaymentValue = 0.0m;// EPCEntites.Set<S_P_Payment_PaymentObjRelation>().Where(a => a.PaymentObjID == paymentID).ToList().Sum(a => a.RelationValue ?? 0);
                var SumInvoiceValue = 0.0m;//EPCEntites.Set<S_P_Invoice_PaymentObjRelation>().Where(a => a.S_P_PaymentObjID == paymentID).ToList().Sum(a => a.RelationValue ?? 0);
                decimal.TryParse(detail.GetValue("PlanPaymentValue"), out PlanPaymentValue);
                decimal.TryParse(detail.GetValue("SumPaymentValue"), out SumPaymentValue);
                decimal.TryParse(detail.GetValue("SumInvoiceValue"), out SumInvoiceValue);
                var tmp = Math.Max(SumPaymentValue, SumInvoiceValue);
                if ((SumPaymentValue > SumInvoiceValue) && PlanPaymentValue < SumPaymentValue)
                {
                    throw new Formula.Exceptions.BusinessValidationException("付款项【" + detail.GetValue("Name") + "】的金额不能小于已付款金额【" + SumPaymentValue + "元】");
                }
                if ((SumPaymentValue < SumInvoiceValue) && PlanPaymentValue < SumInvoiceValue)
                {
                    throw new Formula.Exceptions.BusinessValidationException("付款项【" + detail.GetValue("Name") + "】的金额不能小于已开票金额【" + SumInvoiceValue + "元】");
                }
            }
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            var contractValue = String.IsNullOrEmpty(dic.GetValue("ContractAmount")) ? 0m : Convert.ToDecimal(dic.GetValue("ContractAmount"));
            //合同金额不能小于设备总价
            if (subTableName == "Content")
            {
                var sumValue = 0m;
                foreach (var item in detailList)
                    sumValue += String.IsNullOrEmpty(item.GetValue("TotalPrice")) ? 0m : Convert.ToDecimal(item.GetValue("TotalPrice"));
                if (sumValue > contractValue)
                    throw new Formula.Exceptions.BusinessValidationException("设备材料金额总计不能大于合同金额");

                var applyDetailIDList = detailList.Select(c => c.GetValue("ApplyDetailID")).Distinct().ToList();
                #region 判定采购申请的数量，因为同样的设备会在采购设备明细中出现2次，且价格不同（类似第二件半价），所以采购申请的数量需要统一校验，不能单个校验
                //零星采购申请单独判断
                if (dic.GetValue("OrderType") == "零星采购申请")
                {
                    foreach (var applyDetailID in applyDetailIDList)
                    {
                        var applyDetail = EPCEntites.Set<S_P_ProcurementSmartApply_PBom>().FirstOrDefault(c => c.ID == applyDetailID);
                        if (applyDetail == null)
                            continue;//历史数据不包含采购申请,不报错。

                        var obj = this.EPCSQLDB.ExecuteScalar(String.Format("select Sum(ContractQuantity) from S_P_ContractInfo_Content where ApplyDetailID='{0}' and S_P_ContractInfoID!='{1}'",
                            applyDetailID, dic.GetValue("ID")));
                        var otherContractQuantity = 0.0d;
                        if (obj != null && obj != DBNull.Value)
                        {
                            otherContractQuantity = Convert.ToDouble(obj);
                        }

                        var quantity = 0.0d;
                        var list = detailList.Where(c => c.GetValue("ApplyDetailID") == applyDetailID).ToList();
                        foreach (var applyDetailInfo in list)
                        {
                            var contractQuantity = 0.0d;
                            if (!String.IsNullOrEmpty(applyDetailInfo.GetValue("ContractQuantity")))
                                contractQuantity = Convert.ToDouble(applyDetailInfo.GetValue("ContractQuantity"));
                            quantity += contractQuantity;
                        }
                        if (applyDetail.Quantity < (otherContractQuantity + quantity))
                        {
                            throw new Exception("设备材料【" + applyDetail.Name + "】合计不能大于零星采购申请数量【" + applyDetail.Quantity + "】");
                        }
                    }
                }
                else
                {
                    foreach (var applyDetailID in applyDetailIDList)
                    {
                        var applyDetail = EPCEntites.Set<S_P_ProcurementApply_PBom>().FirstOrDefault(c => c.ID == applyDetailID);
                        if (applyDetail == null)
                            continue;//历史数据不包含采购申请,不报错。

                        var obj = this.EPCSQLDB.ExecuteScalar(String.Format("select Sum(ContractQuantity) from S_P_ContractInfo_Content where ApplyDetailID='{0}' and S_P_ContractInfoID!='{1}'",
                            applyDetailID, dic.GetValue("ID")));
                        var otherContractQuantity = 0m;
                        if (obj != null && obj != DBNull.Value)
                        {
                            otherContractQuantity = Convert.ToDecimal(obj);
                        }

                        var quantity = 0m;
                        var list = detailList.Where(c => c.GetValue("ApplyDetailID") == applyDetailID).ToList();
                        foreach (var applyDetailInfo in list)
                        {
                            var contractQuantity = 0m;
                            if (!String.IsNullOrEmpty(applyDetailInfo.GetValue("ContractQuantity")))
                                contractQuantity = Convert.ToDecimal(applyDetailInfo.GetValue("ContractQuantity"));
                            quantity += contractQuantity;
                        }
                        if (applyDetail.Quantity < (otherContractQuantity + quantity))
                        {
                            throw new Exception("设备材料【" + applyDetail.Name + "】合计不能大于采购申请数量【" + applyDetail.Quantity + "】");
                        }
                    }
                }

                #endregion
            }
            else if (subTableName == "PaymentObj")
            {
                var sumValue = 0m;
                foreach (var item in detailList)
                    sumValue += String.IsNullOrEmpty(item.GetValue("PlanPaymentValue")) ? 0m : Convert.ToDecimal(item.GetValue("PlanPaymentValue"));
                if (sumValue > contractValue)
                    throw new Formula.Exceptions.BusinessValidationException("付款项金额总计不能大于合同金额");
            }
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //合同签订后才执行更新pbom及增加cbscontract
            if (string.IsNullOrEmpty(dic.GetValue("SignDate")))
            {
                return;
            }

            //汇总设备采购数量至PBoom表
            StringBuilder sb = new StringBuilder();
            string sql = @"update S_P_Bom
set ContractQuantity = (
	select isnull(SUM(ContractQuantity),0) from S_P_ContractInfo_Content where PBomID=S_P_Bom.ID
) where EngineeringInfoID='{0}'";
            sql = string.Format(sql, dic.GetValue("EngineeringInfoID"));
            sb.AppendLine(sql);
            //更新PBoom的Islock属性
            sql = @"update S_P_Bom
set Locked=1
where ID in (select PBomID from S_P_ContractInfo_Content where S_P_ContractInfoID='{0}')";
            sql = string.Format(sql, dic.GetValue("ID"));
            sb.AppendLine(sql);

            if (!String.IsNullOrEmpty(dic.GetValue("SignDate")))
            {
                sql = @"update S_P_Bom
set FactContractDate='{1}'
where ID in (select PBomID from S_P_ContractInfo_Content where S_P_ContractInfoID='{0}') and FactContractDate is null";
                var contractDate = String.IsNullOrEmpty(dic.GetValue("SignDate")) ? DateTime.Now : Convert.ToDateTime(dic.GetValue("SignDate"));
                sql = string.Format(sql, dic.GetValue("ID"), contractDate.ToString());
                sb.AppendLine(sql);
            }

            this.EPCSQLDB.ExecuteNonQuery(sb.ToString());
            var contract = this.GetEntityByID(dic.GetValue("ID"));
            if (contract == null) contract = this.entities.Set<S_P_ContractInfo>().Create();
            this.UpdateEntity(contract, dic);
            contract.SynchContractProperties();
            contract.SummaryInvoiceData();
            contract.SummaryPaymentData();
            foreach (var paymentObj in contract.S_P_ContractInfo_PaymentObj.ToList())
            {
                paymentObj.SummaryPaymentValue();
                paymentObj.ResetPlan();
            }

            foreach (var content in contract.S_P_ContractInfo_Content.ToList())
            {
                var bom = this.GetEntityByID<S_P_Bom>(content.PBomID);
                if (bom != null)
                {
                    if (!string.IsNullOrEmpty(content.Number) && content.Number != bom.Number)
                        bom.Number = content.Number;
                    if (string.IsNullOrEmpty(bom.Code) && !string.IsNullOrEmpty(content.Code))
                        bom.Code = content.Code;
                }
            }

            this.EPCEntites.Set<S_I_CBS_Contract>().Delete(c => c.RelateID == contract.ID);
            if (contract.S_P_ContractInfo_CBSInfo.Count > 0)
            {
                foreach (var item in contract.S_P_ContractInfo_CBSInfo.ToList())
                {
                    if (String.IsNullOrEmpty(item.CBSID)) continue;
                    var contractItem = new S_I_CBS_Contract();
                    contractItem.ID = FormulaHelper.CreateGuid();
                    contractItem.Name = contract.Name;
                    contractItem.Code = contract.SerialNumber;
                    contractItem.EngineeringInfoID = contract.S_I_Engineering.ID;
                    contractItem.FullID = contractItem.ID;
                    contractItem.CBSID = item.CBSID;
                    contractItem.CBSFullID = item.CBSFullID;
                    contractItem.Quantity = 1;
                    contractItem.RelateID = contract.ID;
                    contractItem.TotalValue = item.ContractValue;
                    this.EPCEntites.Set<S_I_CBS_Contract>().Add(contractItem);
                }
            }
            else
            {
                foreach (var item in contract.S_P_ContractInfo_Content.ToList())
                {
                    S_I_CBS cbsNode = null;
                    if (String.IsNullOrEmpty(item.CBSInfo))
                    {
                        var cbsBudget = this.EPCEntites.Set<S_I_CBS_Budget>().FirstOrDefault(c => c.BomID == item.PBomID);
                        if (cbsBudget != null)
                        {
                            item.CBSInfo = cbsBudget.CBSID;
                            item.CBSFullID = cbsBudget.CBSFullID;
                            item.CBSInfoName = cbsBudget.S_I_CBS.Name;
                        }
                    }
                    if (String.IsNullOrEmpty(item.CBSInfo))
                    {
                        continue;
                    }
                    else
                    {
                        cbsNode = this.EPCEntites.Set<S_I_CBS>().Find(item.CBSInfo);
                        if (cbsNode != null)
                            item.CBSFullID = cbsNode.FullID;
                    }
                    var contractItem = new S_I_CBS_Contract();
                    FormulaHelper.UpdateModel(contractItem, item);
                    contractItem.ID = FormulaHelper.CreateGuid();
                    contractItem.EngineeringInfoID = contract.S_I_Engineering.ID;
                    contractItem.FullID = contractItem.ID;
                    contractItem.Name = item.Name;
                    contractItem.Code = item.Code;
                    contractItem.CBSID = item.CBSInfo;
                    if (cbsNode != null)
                        contractItem.CBSFullID = cbsNode.FullID;
                    contractItem.Quantity = item.ContractQuantity;
                    contractItem.UnitPrice = item.UnitPrice;
                    contractItem.RelateID = contract.ID;
                    contractItem.TotalValue = item.TotalPrice;
                    contractItem.BomID = item.PBomID;
                    this.EPCEntites.Set<S_I_CBS_Contract>().Add(contractItem);
                }
            }
            this.EPCEntites.SaveChanges();
            if (contract.S_I_Engineering != null)
                contract.S_I_Engineering.SumCBSContractValue();
        }

        public JsonResult GetDeviceDetail(string InvitationID, string SupplierID)
        {
            string sql = @"select S_P_Bom.*,0 as OfferUnitPrice,
S_P_Invitation_Device.InvitationQuantity as OfferQuantity, 
S_I_CBS_Budget.CBSID as CBSInfo,S_I_CBS_Budget.CBSFullID as CBSFullID
from S_P_Invitation_Device
left join S_P_Bom on S_P_Invitation_Device.PBomID=S_P_Bom.ID
 left join  S_I_CBS_Budget on S_I_CBS_Budget.BomID=S_P_Bom.ID
where S_P_InvitationID = '{0}' ";
            var dt = this.EPCSQLDB.ExecuteDataTable(sql = string.Format(sql, InvitationID));

            sql = "select * from S_P_Invitation_Offer where InvitationID = '{0}' and SupplierID='{1}'";
            var offerDt = this.EPCSQLDB.ExecuteDataTable(sql = string.Format(sql, InvitationID, SupplierID));
            foreach (DataRow item in dt.Rows)
            {
                var rows = offerDt.Select("PBomID='" + item["ID"].ToString() + "'");
                if (rows.Length > 0)
                {
                    item["OfferUnitPrice"] = rows[0]["UnitPrice"] == null || rows[0]["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(rows[0]["UnitPrice"]);
                }
            }

            return Json(dt);
        }


        public JsonResult GetUnifiedDeviceDetail(string InvitationID, string SupplierID)
        {
            string sql = @"select *,0 as OfferUnitPrice,InvitationQuantity as OfferQuantity 
from S_P_Invitation_Device where S_P_InvitationID = '{0}'";
            var dt = this.EPCSQLDB.ExecuteDataTable(sql = string.Format(sql, InvitationID));

            sql = "select * from S_P_Invitation_Offer where InvitationID = '{0}' and SupplierID='{1}'";
            var offerDt = this.EPCSQLDB.ExecuteDataTable(sql = string.Format(sql, InvitationID, SupplierID));
            foreach (DataRow item in dt.Rows)
            {
                var rows = offerDt.Select("PBomName='" + item["PBomName"].ToString() + "'");
                if (rows.Length > 0)
                {
                    item["OfferUnitPrice"] = rows[0]["UnitPrice"] == null || rows[0]["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(rows[0]["UnitPrice"]);
                }
            }

            return Json(dt);
        }

        public JsonResult RemovePaymentObj(string PaymentData)
        {
            //付款项 付过钱 收过票不能删除
            var list = JsonHelper.ToList(PaymentData);
            foreach (var item in list)
            {
                var paymentObject = this.GetEntityByID<S_P_ContractInfo_PaymentObj>(item.GetValue("ID"));
                if (paymentObject != null)
                {
                    if (paymentObject.SumInvoiceValue > 0) throw new Formula.Exceptions.BusinessValidationException("付款项【" + paymentObject.Name + "】已经有收票信息，无法进行删除");
                    if (paymentObject.SumPaymentValue > 0) throw new Formula.Exceptions.BusinessValidationException("付款项【" + paymentObject.Name + "】已经有付款信息，无法进行删除");

                    EPCEntites.Set<S_P_ContractInfo_PaymentObj>().Remove(paymentObject);
                }
            }
            EPCEntites.SaveChanges();
            return Json("");
        }

        public JsonResult ValidateRemove(string ContentData)
        {
            string EngineeringInfoID = string.Empty;
            string ContractID = string.Empty;
            //设备到货有记录 不能删除
            var list = JsonHelper.ToList(ContentData);
            foreach (var item in list)
            {
                var contentObject = this.GetEntityByID<S_P_ContractInfo_Content>(item.GetValue("ID"));
                if (contentObject != null)
                {
                    //设备到货有记录 不能删除
                    if (contentObject.ArrivalQuantity > 0)
                        throw new Formula.Exceptions.BusinessValidationException("已经有到货记录的设备，不能移除");
                }
            }
            return Json("");
        }

        public override JsonResult Delete()
        {
            this.BeforeDelete(Request["ListIDs"].Split(','));
            var contractIDs = Request["ListIDs"].Split(',');
            S_I_Engineering engineering = null;
            foreach (var contractID in contractIDs)
            {
                S_P_ContractInfo contract = this.GetEntityByID < S_P_ContractInfo >(contractID);
                engineering = contract.S_I_Engineering;
                var contents = contract.S_P_ContractInfo_Content.ToList();
                foreach (var content in contents)
                {
                    var pBom = EPCEntites.Set<S_P_Bom>().Find(content.PBomID);
                    if (pBom == null)
                        continue;
                    //throw new BusinessException("未找到id为【" + content.PBomID + "】的PBom");
                    //pbom更新以SignDate是否有值为标准，有值说明之前pbom是统计过这个合同额的,因此要扣除
                    if (contract.SignDate != null)
                        pBom.ContractQuantity -= content.ContractQuantity;
                    if (EPCEntites.Set<S_P_ContractInfo_Content>().Count(a => a.PBomID == pBom.ID && a.ID != content.ID) == 0)
                    {
                        pBom.Locked = false;
                        pBom.FactContractDate = null;
                    }
                }
                foreach (var content in contract.S_P_ContractInfo_Content.ToList())
                    this.EPCEntites.Set<S_P_ContractInfo_Content>().Remove(content);
                foreach (var content in contract.S_P_ContractInfo_PaymentObj.ToList())
                    this.EPCEntites.Set<S_P_ContractInfo_PaymentObj>().Remove(content);
                foreach (var content in contract.S_P_ContractInfo_CBSInfo.ToList())
                    this.EPCEntites.Set<S_P_ContractInfo_CBSInfo>().Remove(content);
                this.EPCEntites.Set<S_P_ContractInfo>().Remove(contract);               
                this.EPCEntites.Set<S_I_CBS_Contract>().Delete(c => c.RelateID == contract.ID);
            }

            this.EPCEntites.SaveChanges();
            if (engineering != null)
                engineering.SumCBSContractValue();
            return Json("");
        }

        protected override void BeforeDelete(string[] Ids)
        {
            foreach (var Id in Ids)
            {
                var contract = this.GetEntityByID(Id);
                //评审中 不能删除
                var exsit = this.EPCEntites.Set<T_P_ContractReview>().FirstOrDefault(a => a.ContractInfo == Id
                    && a.FlowPhase == "Processing");
                if (exsit != null)
                    throw new Formula.Exceptions.BusinessValidationException("合同【" + contract.Name + "】已经发起了合同评审流程，不能进行删除操作");

                contract.ValidateDelete();
            }
        }

        public JsonResult CheckReviewFlowStart(string ContractInfoID)
        {
            //优先将暂存的流程启动
            var tmp = this.EPCEntites.Set<T_P_ContractReview>().FirstOrDefault(a => a.FlowPhase == "Start" && a.ContractInfo == ContractInfoID);
            if (tmp != null)
                return Json(tmp.ToDic());
            //如果已在流程中，则不允许修改
            var exsit = this.EPCEntites.Set<T_P_ContractReview>().FirstOrDefault(a => a.ContractInfo == ContractInfoID
                && a.FlowPhase != "End");
            if (exsit != null)
                throw new Formula.Exceptions.BusinessValidationException("【" + exsit.CreateUser + "】已经发起了合同评审流程，无法重复发起");
            return Json("");
        }

        public JsonResult CheckSealFlowStart(string ContractInfoID)
        {
            //优先将暂存的流程启动
            var tmp = this.EPCEntites.Set<T_P_ContractSeal>().FirstOrDefault(a => a.FlowPhase == "Start" && a.ContractInfo == ContractInfoID);
            if (tmp != null)
                return Json(tmp.ToDic());
            //如果已在流程中，则不允许修改
            var exsit = this.EPCEntites.Set<T_P_ContractSeal>().FirstOrDefault(a => a.ContractInfo == ContractInfoID
                && a.FlowPhase != "End");
            if (exsit != null)
                throw new Formula.Exceptions.BusinessValidationException("【" + exsit.CreateUser + "】已经发起了合同用印流程，无法重复发起");
            return Json("");
        }

        public JsonResult SetContractState(string State)
        {
            string id = GetQueryString("ID");
            if (string.IsNullOrEmpty(id))
            {
                throw new BusinessException("需要选择一条合同数据,请重新确认！");
            }
            S_P_ContractInfo contract = EPCEntites.Set<S_P_ContractInfo>().Find(id);
            if (contract == null)
                throw new BusinessException("未找到合同数据");

            contract.ContractState = State;
            //终止后释放数量
            if (State == "Terminate")
            {
                if (contract.S_P_ContractInfo_Content.Any(a => a.ArrivalQuantity > 0))
                    throw new Exception("已经有到货记录的设备，不能终止");
                if (contract.S_P_Payment.Count > 0)
                    throw new Exception("已有付款记录,不能终止");
                if (contract.S_P_Invoice.Count > 0)
                    throw new Exception("已经有开票记录，不能终止");

                contract.SelectOrder = "";
                contract.SelectOrderName = "";
                foreach (var content in contract.S_P_ContractInfo_Content)
                {
                    content.ApplyBomID = "";
                    content.ApplyDetailID = "";
                    content.ApplyFormCode = "";
                    content.ApplyFormID = "";
                }


                var contentBom = contract.S_P_ContractInfo_Content.GroupBy(a => a.PBomID).Select(b => new { BomID = b.Key, Quantity = b.Sum(t => t.ContractQuantity), ContentIDs = b.Select(t => t.ID) }).ToList();
                foreach (var tmp in contentBom)
                {
                    //同一工程下，已签订的不包括本合同的content数量
                    var newContractQuantity = EPCEntites.Set<S_P_ContractInfo_Content>().Where(a => a.S_P_ContractInfo.EngineeringInfoID == contract.EngineeringInfoID
                        && a.PBomID == tmp.BomID
                        && a.S_P_ContractInfo.SignDate != null && !tmp.ContentIDs.Contains(a.ID)).ToList().Sum(a => a.ContractQuantity);

                    EPCEntites.Set<S_P_Bom>().Where(a => a.ID == tmp.BomID).Update(a => a.ContractQuantity = newContractQuantity);
                }
            }

            EPCEntites.SaveChanges();
            return Json("");
        }

        public JsonResult GetContractContents(string ContractInfoID)
        {
            var enty = FormulaHelper.GetEntities<EPCEntities>();
            var contract = enty.Set<S_P_ContractInfo>().FirstOrDefault(c => c.ID == ContractInfoID);
            List<S_P_ContractInfo_Content> content = new List<S_P_ContractInfo_Content>();
            if (contract != null)
            {
                content = enty.Set<S_P_ContractInfo_Content>().Where(c => c.S_P_ContractInfoID == ContractInfoID).ToList();
            }
            return Json(content);
        }

        public JsonResult GetPBomFromInvitationInfo(string applyID)
        {
            string sql = @"select * from (
                           select *,CanQuantity = case when (CanInvitationQuantity-CanContractQuantity) >0 then CanContractQuantity else CanInvitationQuantity end
                            
                            from(select InvitationType,S_P_Invitation.Name as InvitationName,SerialNumber,ApplyDeptName,Winner,WinnerName,ApplyUserName,
                           S_P_Invitation_Device.ID as ApplyDetailID,S_P_Invitation.ID as ApplyFormID,S_P_Invitation_Device.ApplyDetailID as ApplyBomID,
                           procurementApply.ProcurementApplyID,procurementApply.ProcurementApplyCode,
                           S_P_Bom.*,S_P_Invitation_Offer.Quantity as OfferQuantity,S_P_Invitation_Offer.UnitPrice as OfferUnitPrice,S_P_Invitation_Offer.TotalPrice as OfferTotalPrice,
                           isnull(S_P_Invitation_Device.InvitationQuantity,0)-isnull(ContractInfo.ContractQuantity,0) as CanInvitationQuantity,--比价剩余量
                           isnull(S_P_ProcurementApply_PBom.Quantity,0) - isnull(AllContractInfo.AllApplyContractQuantity,0) as CanContractQuantity --采购申请剩余量
                           from S_P_Invitation_Winner
                           left join S_P_Invitation_Offer
                           on S_P_Invitation_Offer.SupplierID=S_P_Invitation_Winner.Winner
                           and S_P_Invitation_Winner.InvitationID=S_P_Invitation_Offer.InvitationID
                           left join dbo.S_P_Invitation on S_P_Invitation_Winner.InvitationID=S_P_Invitation.ID
                           left join dbo.S_P_Invitation_EngineeringRelaltion 
                           on S_P_Invitation_EngineeringRelaltion.S_P_InvitationID=S_P_Invitation_Winner.InvitationID
                           
                           
                           left join (select pBomID,Sum(ContractQuantity) as AllApplyContractQuantity from dbo.S_P_ContractInfo_Content
                           where S_P_ContractInfoID!=''group by pBomID) AllContractInfo 
                           on AllContractInfo.pBomID=S_P_Invitation_Offer.pBomID
                           
                           left join S_P_ProcurementApply_PBom on S_P_ProcurementApply_PBom.BomID = S_P_Invitation_Offer.pBomID
                           left join (select S_P_ProcurementApply.ID as ProcurementApplyID, S_P_ProcurementApply.SerialNumber as ProcurementApplyCode from S_P_ProcurementApply ) procurementApply on S_P_ProcurementApply_PBom.S_P_ProcurementApplyID = procurementApply.ProcurementApplyID
                           left join S_P_Invitation_Device on S_P_Invitation_Device.pbomid = S_P_Invitation_Offer.pbomid and S_P_Invitation_Device.S_P_InvitationID = S_P_Invitation_Offer.invitationid
                           left join (select Sum(ContractQuantity) as ContractQuantity,ApplyDetailID from dbo.S_P_ContractInfo_Content
                           where S_P_ContractInfoID!=''group by ApplyDetailID) ContractInfo 
                           on ContractInfo.ApplyDetailID=S_P_Invitation_Device.ID
                           left join S_P_Bom on S_P_Invitation_Offer.PBomID=S_P_Bom.ID
                           where S_P_Invitation.ID in ('" + applyID.Replace(",", "','") + @"')
                            )tmp1)
                           tmp2
                           where CanQuantity > 0";
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult GetPBomFromProcurementApply(string applyID, string contractInfoID)
        {
            string sql = @"select * from (select S_P_ProcurementApply_PBom.ID as ApplyDetailID,S_P_ProcurementApplyID,
                         S_P_ProcurementApply_PBom.CBSInfo,S_P_ProcurementApply_PBom.CBSInfoName,
                         S_P_ProcurementApply.SerialNumber,S_P_ProcurementApply.ApplyUser,S_P_ProcurementApply.ProcurementType,ApplyUserName,ApplyDate,
                         --S_P_ProcurementApply.Type,
                         S_P_Bom.*,S_P_ProcurementApply_PBom.Quantity as ProcurementApplyQuantity,
                         isnull(S_P_ProcurementApply_PBom.Quantity,0)-isnull(ContentInfo.ContractQuantity,0)-isnull(Result.ResultQuantity,0)-isnull(Apply.ApplyQuantity,0) as CanContractQuantity
                          from S_P_ProcurementApply_PBom
                         left join S_P_ProcurementApply on S_P_ProcurementApply.ID=S_P_ProcurementApplyID
                         left join S_P_Bom on S_P_ProcurementApply_PBom.BomID=S_P_Bom.ID
                         left join (select ApplyDetailID,isnull(Sum(ContractQuantity),0) as ContractQuantity
                         from dbo.S_P_ContractInfo_Content where S_P_ContractInfoID!='" + contractInfoID+@"'
                         group by ApplyDetailID) ContentInfo
                         on ContentInfo.ApplyDetailID=S_P_ProcurementApply_PBom.ID
                         
                         left join ( 
                         select sum(isnull(ContractQuantity,0)) ResultQuantity,ProcurementDetailBomID from (select T_P_ComparisonResult_DeviceInfo.ApplyDetailID as ProcurementDetailBomID,S_P_ContractInfo_Content.* from T_P_ComparisonResult_DeviceInfo
                         left join S_P_ContractInfo_Content on S_P_ContractInfo_Content.ApplyDetailID = T_P_ComparisonResult_DeviceInfo.id) tmp group by ProcurementDetailBomID) Result
                         on Result.ProcurementDetailBomID = S_P_ProcurementApply_PBom.ID
                         
                         left join ( 
                         select sum(isnull(ContractQuantity,0)) ApplyQuantity,ProcurementDetailBomID from (select T_P_InvitationApply_DeviceList.ApplyDetailID as ProcurementDetailBomID,S_P_ContractInfo_Content.* from T_P_InvitationApply_DeviceList
                         left join S_P_ContractInfo_Content on S_P_ContractInfo_Content.ApplyDetailID = T_P_InvitationApply_DeviceList.id) tmp group by ProcurementDetailBomID) Apply
                         on Apply.ProcurementDetailBomID = S_P_ProcurementApply_PBom.ID
                         ) tmp
                         where tmp.CanContractQuantity > 0 and S_P_ProcurementApplyID in ('" + applyID.Replace(",", "','") + @"')";

            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult GetPBomFromSmartProcurementApply(string applyID, string contractInfoID)
        {
            string sql = @"select * from (select S_P_ProcurementSmartApply_PBom.ID as ApplyDetailID,S_P_ProcurementSmartApplyID,
                         S_P_ProcurementSmartApply.SerialNumber,S_P_ProcurementSmartApply.ApplyUser,ApplyUserName,ApplyDate,
                         
                         S_P_Bom.*,S_P_ProcurementSmartApply_PBom.Quantity as ProcurementApplyQuantity,
                         isnull(S_P_ProcurementSmartApply_PBom.Quantity,0)-isnull(ContentInfo.ContractQuantity,0) as CanContractQuantity
                          from S_P_ProcurementSmartApply_PBom
                         left join S_P_ProcurementSmartApply on S_P_ProcurementSmartApply.ID=S_P_ProcurementSmartApplyID
                         left join S_P_Bom on S_P_ProcurementSmartApply_PBom.PBomID=S_P_Bom.ID
                         left join (select ApplyDetailID,isnull(Sum(ContractQuantity),0) as ContractQuantity
                         from dbo.S_P_ContractInfo_Content where S_P_ContractInfoID!='" + contractInfoID + @"'
                         group by ApplyDetailID) ContentInfo
                         on ContentInfo.ApplyDetailID=S_P_ProcurementSmartApply_PBom.ID) tmp
                         where tmp.CanContractQuantity > 0 and S_P_ProcurementSmartApplyID in ('" + applyID.Replace(",", "','") + @"')";

            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(dt);
        }
    }
}
