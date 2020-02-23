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
using Base.Logic.Domain;

namespace EPC.Areas.Manage.Controllers
{
    public class SupplierController : EPCFormContorllor<S_I_SupplierInfo>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
            var id = dic.GetValue("ID");
            var isDomestic = dic.GetValue("IsDomestic");
            var code = dic.GetValue("SupplierCode");
            //验证编号重复
            var exist = this.EPCEntites.Set<S_I_SupplierInfo>().FirstOrDefault(a => a.ID != id && a.SupplierCode == code);
            //最新编号
            if (exist != null)
            {
                int AutoNum;
                var Code = createCode(isDomestic, out AutoNum);
                dic.SetValue("SupplierCode", Code);
                dic.SetValue("AutoNum", AutoNum.ToString());
            }
        }
        
        public JsonResult ValidateQualification(string ID, string yearCount)
        {
            var entity = this.GetEntityByID(ID);
            validateQualification(entity, yearCount);
            return Json("");
        }

        private void validateQualification(S_I_SupplierInfo entity, string yearCount)
        {
            if (entity == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的供应商信息，操作失败");
            if (entity.FlowPhase == "Processing")
            {
                throw new Formula.Exceptions.BusinessValidationException("供应商【" + entity.SupplierName + "】已经在进行审批，请等待审批完成后再申请");
            }
            if (entity.State != SupplierState.Create.ToString() && entity.State != SupplierState.DisQualification.ToString())
            {
                throw new Formula.Exceptions.BusinessValidationException("供应商【" + entity.SupplierName + "】状态不符，只有待准入和不合格供应商才能提交申请");
            }
            if (entity.State == SupplierState.DisQualification.ToString())
            {
                if (entity.LastChangeStateDate.HasValue)
                {
                    var year = String.IsNullOrEmpty(yearCount) ? 0 : Convert.ToInt32(yearCount);
                    var date = entity.LastChangeStateDate.Value.AddYears(year);
                    if (date > DateTime.Now)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("供应商【" + entity.SupplierName + "】要在【" + date.ToShortDateString() + "】后才能进行申请");
                    }
                }
            }
        }

        public JsonResult ValidateDisQualification(string ListIDs)
        {
            foreach (var ID in ListIDs.Split(','))
            {
                var entity = this.GetEntityByID(ID);
                validateDisQualification(entity);
            }
            return Json("");
        }

        private void validateDisQualification(S_I_SupplierInfo entity)
        {
            if (entity == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的供应商信息，操作失败");
            if (entity.FlowPhase == "Processing")
            {
                throw new Formula.Exceptions.BusinessValidationException("供应商【" + entity.SupplierName + "】已经在进行审批，请等待审批完成后再申请");
            }
            if (entity.State != SupplierState.Create.ToString() && entity.State != SupplierState.Qualification.ToString())
            {
                throw new Formula.Exceptions.BusinessValidationException("供应商【" + entity.SupplierName + "】状态不符，只有待准入和合格供应商才能提交申请");
            }
        }

        public JsonResult ValidateOutBlackList(string ListIDs, string yearCount)
        {
            foreach (var ID in ListIDs.Split(','))
            {
                var entity = this.GetEntityByID(ID);
                validateOutBlackList(entity, yearCount);
            }
            return Json("");
        }

        private void validateOutBlackList(S_I_SupplierInfo entity, string yearCount)
        {
            if (entity == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的供应商信息，操作失败");
            if (entity.FlowPhase == "Processing")
            {
                throw new Formula.Exceptions.BusinessValidationException("供应商已经在进行审批，请等待审批完成后再申请");
            }
            if (entity.State != SupplierState.BlackList.ToString())
            {
                throw new Formula.Exceptions.BusinessValidationException("供应商【" + entity.SupplierName + "】状态不符，只有黑名单的供应商才能提交申请");
            }
            if (entity.LastChangeStateDate.HasValue)
            {
                var year = String.IsNullOrEmpty(yearCount) ? 0 : Convert.ToInt32(yearCount);
                var date = entity.LastChangeStateDate.Value.AddYears(year);
                if (date > DateTime.Now)
                {
                    throw new Formula.Exceptions.BusinessValidationException("供应商【" + entity.SupplierName + "】要在【" + date.ToShortDateString() + "】后才能进行申请");
                }
            }
        }

        public JsonResult ValidateBlackList(string ListIDs)
        {
            foreach (var ID in ListIDs.Split(','))
            {
                var entity = this.GetEntityByID(ID);
                validateBlackList(entity);
            }
            return Json("");
        }

        private void validateBlackList(S_I_SupplierInfo entity)
        {
            if (entity == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的供应商信息，操作失败");
            if (entity.FlowPhase == "Processing")
            {
                throw new Formula.Exceptions.BusinessValidationException("供应商【" + entity.SupplierName + "】已经在进行审批，请等待审批完成后再申请");
            }

            if (entity.State == SupplierState.BlackList.ToString())
            {
                throw new Formula.Exceptions.BusinessValidationException("供应商【" + entity.SupplierName + "】状态不符，已经是黑名单的供应商不能重复申请");
            }
        }

        
        public JsonResult CreateSerialNumber(string IsDomestic)
        {
            int AutoNum;
            var Code = createCode(IsDomestic, out AutoNum);
            //var formInfo = baseEntities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == "SupplierInfo");
            //string SerialNumber =  GetSerialNumber(formInfo.Code, formInfo.SerialNumberSettings, false, null, null);
            return Json(new { Code, AutoNum });
        }

        private string createCode(string IsDomestic, out int AutoNum)
        {
            var preCode = (IsDomestic == "1" ? "01" : "02");
            var maxAutoNum = this.EPCEntites.Set<S_I_SupplierInfo>().Where(a => a.IsDomestic == IsDomestic).Select(a => a.AutoNum).Max();
            if (!maxAutoNum.HasValue)
                maxAutoNum = 0;
            AutoNum = maxAutoNum.Value + 1;
            var Code = preCode + AutoNum.ToString().PadLeft(4, '0');
            return Code;
        }
    }
}
