using EPC.Logic.Domain;
using Config.Logic;
using Formula;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Formula.Exceptions;

namespace EPC.Areas.Construction.Controllers
{
    public class FbContractInfoController : EPCFormContorllor<S_P_ContractInfo>
    {
        public ActionResult CList()
        {
            return View();
        }
        public ActionResult BOQSummary()
        {
            return View();
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
            decimal contractAmount = 0;
            decimal.TryParse(dic.GetValue("ContractAmount"), out contractAmount);
            string contractInfoID = dic.GetValue("ID");
            var lastVersion = EPCEntites.Set<S_C_BOQ_Version>().Where(a => a.ContractInfoID == contractInfoID).OrderByDescending(a => a.ID).FirstOrDefault();
            if (lastVersion != null)
            {
                decimal boqTotalPrice = EPCEntites.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == lastVersion.ID).ToList().Sum(a => a.Price ?? 0);

                if (contractAmount < boqTotalPrice)
                {
                    throw new Formula.Exceptions.BusinessValidationException("合同人民币金额不能小于清单总价" + boqTotalPrice + "元");
                }
            }

            if (!string.IsNullOrEmpty(GetQueryString("State")))
            {
                dic.SetValue("ContractState", GetQueryString("State"));
            }
        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            var orgService = Formula.FormulaHelper.GetService<IOrgService>();
            var orgList = orgService.GetOrgs(Config.Constant.OrgRootID);
            var rootOrg = orgList.FirstOrDefault(d => d.ID == Config.Constant.OrgRootID);
            dic.SetValue("PartyA", rootOrg.ID);
            dic.SetValue("PartyAName", rootOrg.Name);
        }

        public JsonResult GetFbContractInfo(string EngineeringInfoID, string NumOrName)
        {
            List<S_P_ContractInfo> result = new List<S_P_ContractInfo>();
            if (string.IsNullOrEmpty(NumOrName))
            {
                result = EPCEntites.Set<S_P_ContractInfo>()
                    .Where(a => a.EngineeringInfoID == EngineeringInfoID && a.ContractProperty == "Construction").OrderByDescending(a => a.CreateDate).ToList();
            }
            else
            {
                result = EPCEntites.Set<S_P_ContractInfo>().Where(
                    a => a.EngineeringInfoID == EngineeringInfoID
                    && a.ContractProperty == "Construction"
                    && (a.Name.Contains(NumOrName) || a.SerialNumber.Contains(NumOrName))).OrderByDescending(a => a.CreateDate).ToList();
            }
            return Json(result);
        }

        public JsonResult GetBOQSummary(string ContractInfoID, QueryBuilder qb)
        {
            S_C_BOQ_Version zeroVersion = EPCEntites.Set<S_C_BOQ_Version>().Where(a => a.ContractInfoID == ContractInfoID).OrderBy(a => a.VersionNumber).FirstOrDefault();
            string zeroVersionID = "";
            if (zeroVersion != null)
            {
                zeroVersionID = zeroVersion.ID;
            }
            var zeroVersionDetail = EPCEntites.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == zeroVersionID);

            var resultQuery = (from a in EPCEntites.Set<S_C_BOQ>().Where(a => a.ContractInfoID == ContractInfoID)
                               join b in zeroVersionDetail
                               on a.ID equals b.BOQID into abR
                               from ab in abR.DefaultIfEmpty()
                               select new
                               {
                                   ID = a.ID,
                                   Name = a.Name,
                                   Code = a.Code,
                                   Property = a.Property,
                                   Unit = a.Unit,
                                   //终版本
                                   UnitPrice = a.UnitPrice,
                                   Quantity = a.Quantity,
                                   Price = a.UnitPrice * a.Quantity,
                                   //0版
                                   ZeroUnitPrice = ab.UnitPrice ?? 0,
                                   ZeroQuantity = ab.Quantity ?? 0,
                                   ZeroPrice = (ab.UnitPrice ?? 0) * (ab.Quantity ?? 0),
                                   //累计变更
                                   UnitPriceDelta = a.UnitPrice - (ab.UnitPrice ?? 0),
                                   QuantityDelta = a.Quantity - (ab.Quantity ?? 0),
                                   PriceDelta = a.UnitPrice * a.Quantity - (ab.UnitPrice ?? 0) * (ab.Quantity ?? 0),
                                   //已计量根据计量单
                                   CheckQuantityTotal = a.CheckQuantityTotal ?? 0,
                                   //已计合价根据计价单
                                   CheckPriceTotal = a.CheckPriceTotal ?? 0,
                                   Percent = Math.Round((decimal)((a.UnitPrice * a.Quantity ?? 0) == 0 ? 0 : (a.CheckPriceTotal ?? 0) / (a.UnitPrice * a.Quantity) * 100), 2)
                               }).Where(qb);

            return Json(resultQuery.ToList());
        }

        protected override void BeforeDelete(string[] Ids)
        {
            foreach (var Id in Ids)
            {
                var contract = this.GetEntityByID(Id);
                if (contract.SignDate != null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("已签约合同不能删除");
                }
                else if (EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck>().Any(a => a.ContractInfoID == contract.ID))
                {
                    throw new Formula.Exceptions.BusinessValidationException("合同【" + contract.Name + "】已经被计量，不能进行删除操作");
                }
                contract.ValidateDelete();

                //开始删除BOQ
                EPCEntites.Set<S_C_BOQ_Version_Detail>().Delete(a => EPCEntites.Set<S_C_BOQ_Version>().Any(b => b.ContractInfoID == Id && b.ID == a.VersionID));
                EPCEntites.Set<S_C_BOQ_Version>().Delete(a => a.ContractInfoID == Id);
                EPCEntites.Set<S_C_BOQ>().Delete(a => a.ContractInfoID == Id);
            }
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
            EPCEntites.SaveChanges();
            return Json("");
        }
    }
}
