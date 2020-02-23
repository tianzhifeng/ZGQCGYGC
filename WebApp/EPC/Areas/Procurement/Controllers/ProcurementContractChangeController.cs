using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;
using Config.Logic;
using Config;
using Formula.Helper;
using Base.Logic.Model.UI.Form;
using Base.Logic.Domain;
using System.Data;
using Formula;
using EPC.Logic.Domain;
using System.Text;

namespace EPC.Areas.Procurement.Controllers
{
    public class ContractChangeController : EPCFormContorllor<S_P_ProcurementContractChange>
    {
        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            string tmplCode = Request["TmplCode"];
            var formInfo = baseEntities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == tmplCode);
            if (formInfo == null) throw new Exception("没有找到编号为【" + tmplCode + "】的表单定义");
            var items = JsonHelper.ToList(formInfo.Items).Where(c => c.GetValue("ItemType") == "SubTable").ToList();
            string ContractID = dic.GetValue("ContractID");
            if (isNew)
            {
                ContractID = GetQueryString("ContractID");
                foreach (var item in items)
                {
                    var tableName = formInfo.TableName + "_" + item.GetValue("Code");
                    var contractTableName = "S_P_ContractInfo" + "_" + item.GetValue("Code");
                    string sql = "SELECT count(0) as TableCount FROM sysobjects WHERE name='{0}'";
                    var obj = Convert.ToInt32(this.EPCSQLDB.ExecuteScalar(String.Format(sql, contractTableName)));
                    if (obj > 0)
                    {
                        sql = "Select * from {0} where S_P_ContractInfoID='{1}'";
                        var subTable = this.EPCSQLDB.ExecuteDataTable(String.Format(sql, contractTableName, ContractID));
                        var subTableData = new List<Dictionary<string, object>>();
                        foreach (DataRow subItem in subTable.Rows)
                        {
                            var TmpDic = Formula.FormulaHelper.DataRowToDic(subItem);
                            //if (item.GetValue("Code") == "PaymentObj")
                            //{
                            //    string paymentID = subItem["ID"].ToString();
                            //    bool CantChange = false;
                            //    if (EPCEntites.Set<S_P_Payment_PaymentObjRelation>().Any(a => a.PaymentObjID == paymentID)
                            //        || EPCEntites.Set<S_P_Invoice_PaymentObjRelation>().Any(a => a.S_P_PaymentObjID == paymentID))
                            //    {
                            //        CantChange = true;
                            //    }
                            //    TmpDic.SetValue("CantChange", CantChange);
                            //} 
                            TmpDic.SetValue("ID", "");
                            TmpDic.SetValue("OrlID", subItem["ID"]);
                            subTableData.Add(TmpDic);
                        }
                        var json = JsonHelper.ToJson(subTableData);
                        dic.SetValue(item.GetValue("Code"), json);//子表json赋值                         
                    }
                }
            }

            var lastVersionID = dic.GetValue("LastVersionID");
            if (string.IsNullOrEmpty(lastVersionID))
            {
                var contract = EPCEntites.Set<S_P_ContractInfo>().Find(ContractID);
                dic.SetValue("LastVersionData", JsonHelper.ToJson(contract));

                var cbsInfo = contract.S_P_ContractInfo_CBSInfo;
                var cbsDics = FormulaHelper.CollectionToListDic(cbsInfo);
                foreach (var cbsDic in cbsDics)
                    cbsDic.SetValue("OrlID", cbsDic.GetValue("ID"));

                var content = contract.S_P_ContractInfo_Content;
                var contentDics = FormulaHelper.CollectionToListDic(content);
                foreach (var contentDic in contentDics)
                    contentDic.SetValue("OrlID", contentDic.GetValue("ID"));

                var payment = contract.S_P_ContractInfo_PaymentObj;
                var paymentDics = FormulaHelper.CollectionToListDic(payment);
                foreach (var paymentDic in paymentDics)
                    paymentDic.SetValue("OrlID", paymentDic.GetValue("ID"));

                dic.SetValue("LastCBSInfo", JsonHelper.ToJson(cbsDics));
                dic.SetValue("LastContent", JsonHelper.ToJson(contentDics));
                dic.SetValue("LastPaymentObj", JsonHelper.ToJson(paymentDics));
            }
            else
            {
                S_P_ProcurementContractChange change = EPCEntites.Set<S_P_ProcurementContractChange>().Find(lastVersionID);
                if (change != null)
                {
                    S_P_ContractInfo contract = new S_P_ContractInfo();
                    FormulaHelper.UpdateEntity(contract, change.ToDic());//只带合同信息,去掉审批字段数据
                    dic.SetValue("LastVersionData", JsonHelper.ToJson(contract));
                    dic.SetValue("LastCBSInfo", JsonHelper.ToJson(change.S_P_ProcurementContractChange_CBSInfo));
                    dic.SetValue("LastContent", JsonHelper.ToJson(change.S_P_ProcurementContractChange_Content));
                    dic.SetValue("LastPaymentObj", JsonHelper.ToJson(change.S_P_ProcurementContractChange_PaymentObj));
                }
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            dic.SetValue("FullID", dic.GetValue("ContractID"));
            if (dic.GetValue("IsSubContract") == true.ToString().ToUpper())
            {
                dic.SetValue("FullID", dic.GetValue("ParentContract") + "." + dic.GetValue("ContractID"));
            }

            if (!string.IsNullOrEmpty(GetQueryString("State")))
            {
                dic.SetValue("ContractState", GetQueryString("State"));
            }
            var ContractAmount = 0.0m;
            decimal.TryParse(dic.GetValue("ContractAmount"), out ContractAmount);
            var contractID = dic.GetValue("ContractID");
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
                var conid = dic.GetValue("ContractID");
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
                    var detailID = detail.GetValue("OrlID");
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
                            applyDetailID, dic.GetValue("ContractID")));
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
                            applyDetailID, dic.GetValue("ContractID")));
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

        protected override void OnFlowEnd(S_P_ProcurementContractChange entity, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            var contract = entity.Push();
            this.EPCEntites.SaveChanges();
            AfterSave(contract.ToDic());
        }

        //同ProcumentContractController 在SaveChanges之后的处理
        private void AfterSave(Dictionary<string, object> dic)
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
            var contract = EPCEntites.Set<S_P_ContractInfo>().Find(dic.GetValue("ID"));
            if (contract == null) contract = EPCEntites.Set<S_P_ContractInfo>().Create();
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
                var itemList = contract.S_P_ContractInfo_Content.Where(c => !String.IsNullOrEmpty(c.CBSInfo)).ToList();
                foreach (var item in itemList)
                {
                    var contractItem = new S_I_CBS_Contract();
                    FormulaHelper.UpdateModel(contractItem, item);
                    contractItem.ID = FormulaHelper.CreateGuid();
                    contractItem.EngineeringInfoID = contract.S_I_Engineering.ID;
                    contractItem.FullID = contractItem.ID;
                    contractItem.Name = item.Name;
                    contractItem.Code = item.Code;
                    contractItem.CBSID = item.CBSInfo;
                    contractItem.CBSFullID = item.CBSFullID;
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
                var paymentObject = this.GetEntityByID<S_P_ContractInfo_PaymentObj>(item.GetValue("OrlID"));
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
                var contentObject = this.GetEntityByID<S_P_ContractInfo_Content>(item.GetValue("OrlID"));
                if (contentObject != null)
                {
                    //设备到货有记录 不能删除
                    if (contentObject.ArrivalQuantity > 0)
                        throw new Formula.Exceptions.BusinessValidationException("已经有到货记录的设备，不能移除");
                }
            }
            return Json("");
        }
    }
}
