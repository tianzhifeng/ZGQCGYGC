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


namespace EPC.Areas.Procurement.Controllers
{
    public class ArrivalDetailInfoController : EPCFormContorllor<S_P_Arrival_DetailInfo>
    {
        //单个物资到货,自动补到货单,一货一单
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);

            if (!isNew)
                return;
  
            S_P_Arrival arrival = new S_P_Arrival();
            arrival.ID = FormulaHelper.CreateGuid();

            S_P_ContractInfo_Content content = EPCEntites.Set<S_P_ContractInfo_Content>().Find(dic.GetValue("BomInfo"));
            if (content == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到物资信息");
            }

            decimal quantity = 0;
            decimal.TryParse(dic.GetValue("Quantity"), out quantity);
            if (quantity < 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("到货数量不能为" + quantity);
            }

            //可能多发不判断
            //decimal quantity = 0;
            //decimal.TryParse(dic.GetValue("Quantity"), out quantity);
            //decimal hasArrivalQuantity = EPCEntites.Set<S_P_Arrival_DetailInfo>().Where(a => a.BomInfo == content.ID).ToList().Sum(a => a.Quantity ?? 0);
            //decimal sumRest = content.ContractQuantity ?? 0 - hasArrivalQuantity;
            //if (quantity > sumRest)
            //{
            //    throw new Formula.Exceptions.BusinessValidationException("到货数量超出了剩余数量" + sumRest);
            //}

            arrival.CreateDate = DateTime.Now;
            arrival.ModifyDate = DateTime.Now;
            arrival.CreateUserID = CurrentUserInfo.UserID;
            arrival.CreateUser = CurrentUserInfo.UserName;
            arrival.ModifyUserID = CurrentUserInfo.UserID;
            arrival.ModifyUser = CurrentUserInfo.UserName;
            arrival.OrgID = CurrentUserInfo.UserOrgID;
            arrival.CompanyID = CurrentUserInfo.UserCompanyID;
            arrival.FlowPhase = "Start";
            arrival.StepName = "";
            arrival.EngineeringInfoName = content.S_P_ContractInfo.EngineeringInfoName; //dic.GetValue("EngineeringInfoName");
            arrival.EngineeringInfoCode = content.S_P_ContractInfo.EngineeringInfoCode;// dic.GetValue("EngineeringInfoCode");
            arrival.ContractInfo = content.S_P_ContractInfo.ID;// dic.GetValue("ContractInfo");
            arrival.ContractInfoName = content.S_P_ContractInfo.Name;// dic.GetValue("ContractInfoName");
            arrival.ContractCode = content.S_P_ContractInfo.SerialNumber;// dic.GetValue("ContractInfoNum");
            arrival.ArrivalDate = Convert.ToDateTime(dic.GetValue("ArrivalDate"));
            arrival.CheckUser = CurrentUserInfo.UserID;
            arrival.CheckUserName = CurrentUserInfo.UserName;

            var form = baseEntities.Set<S_UI_Form>().Where(c => c.Code == "Arrival").OrderByDescending(c => c.ID).FirstOrDefault(); //获取最新一个版本即可
            if (form == null)
                throw new Formula.Exceptions.BusinessValidationException("表单编号为Arrival不存在!");
            string SerialNumberSettings = form.SerialNumberSettings;
            var serialNumberDic = JsonHelper.ToObject(SerialNumberSettings);
            SerialNumberParam param = new SerialNumberParam();            
            string tmpl = serialNumberDic["Tmpl"].ToString();
            string resetRule = serialNumberDic["ResetRule"].ToString();
            arrival.SerialNumber = SerialNumberHelper.GetSerialNumberString(tmpl, param, resetRule, true);

            arrival.SendFormID = "";//没有发货单可关联
            arrival.SendFormIDName = "";//同上
            arrival.CheckDate = DateTime.Now;
            arrival.CheckResult = "";//无
            arrival.Register = CurrentUserInfo.UserID;
            arrival.RegisterName = CurrentUserInfo.UserName;
            arrival.RegisterDate = DateTime.Now;
            arrival.Remark = "来自手机端扫描入库";
            arrival.EngineeringInfoID = content.S_P_ContractInfo.EngineeringInfoID;// dic.GetValue("EngineeringInfoID");
            EPCEntites.Set<S_P_Arrival>().Add(arrival);
            EPCEntites.SaveChanges();
  
            dic.SetValue("PBomID", content.PBomID);
            dic.SetValue("Code", content.Code);
            dic.SetValue("Sepcification", content.Model);
            dic.SetValue("Unit", content.Unit);
            //dic.SetValue("BomInfo", content.ID);
            dic.SetValue("BomInfoName", content.Name);
            dic.SetValue("Name", content.Name);
            dic.SetValue("S_P_ArrivalID", arrival.ID);
        }
    }
}
