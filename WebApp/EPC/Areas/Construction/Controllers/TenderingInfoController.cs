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

namespace EPC.Areas.Construction.Controllers
{
    public class TenderingInfoController:EPCFormContorllor<T_C_TenderingInfo>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            var engineeringInfoID = dic.GetValue("EngineeringInfoID");
            var section = dic.GetValue("AssociationSegment");
            var tenderingInfo = this.EPCEntites.Set<T_C_TenderingInfo>().Where(m => m.EngineeringInfoID == engineeringInfoID && m.AssociationSegment == section).FirstOrDefault();
            if (tenderingInfo != null)
            {
                if(tenderingInfo.SerialNumber!= dic.GetValue("SerialNumber"))
                { 
                    throw new Formula.Exceptions.BusinessValidationException("标段已有招标信息，请重新确认！");
                }
            }
        }
        protected override void AfterSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            string tenderingID = dic.GetValue("ID");
            var spInvitation = this.EPCEntites.Set<S_P_Invitation>().Where(x => x.ID == tenderingID).FirstOrDefault();
            var userInfo = FormulaHelper.GetUserInfo();
            if (!string.IsNullOrEmpty(tenderingID) && spInvitation == null)
            {
                S_P_Invitation sp = new S_P_Invitation();
                sp.ID = tenderingID;
                sp.CreateDate = DateTime.Now;
                sp.CreateUser = userInfo.UserName;
                sp.CreateUserID = userInfo.UserID;
                sp.ModifyDate = DateTime.Now;
                sp.ModifyUser = userInfo.UserName;
                sp.ModifyUserID = userInfo.UserID;
                sp.Name = dic.GetValue("Name");
                sp.SerialNumber = dic.GetValue("SerialNumber");
                sp.InvitationType = dic.GetValue("BidType");
                sp.InvitationState = dic.GetValue("EngineeringState");
                sp.SubCategory = "Construction";
                this.EPCEntites.Set<S_P_Invitation>().Add(sp);

                S_P_Invitation_EngineeringRelaltion spe = new S_P_Invitation_EngineeringRelaltion();
                spe.ID = FormulaHelper.CreateGuid();
                spe.S_P_InvitationID = tenderingID;
                spe.EngineeringInfo = dic.GetValue("EngineeringInfoID");
                spe.EngineeringInfoName = dic.GetValue("EngineeringInfoName");
                this.EPCEntites.Set<S_P_Invitation_EngineeringRelaltion>().Add(spe);
            }
            else
            {
                spInvitation.ModifyDate = DateTime.Now;
                spInvitation.ModifyUser = userInfo.UserName;
                spInvitation.ModifyUserID = userInfo.UserID;
                spInvitation.Name = dic.GetValue("Name");
                spInvitation.SerialNumber = dic.GetValue("SerialNumber");
                spInvitation.InvitationType = dic.GetValue("BidType");
            }
            string subcontractor = dic.GetValue("InviteSubcontractor");
            if (!string.IsNullOrEmpty(subcontractor))
            {
                this.EPCEntites.Set<S_P_Invitation_Supplier>().Delete(x => x.S_P_InvitationID == tenderingID);
                var subSupplier = JsonHelper.ToObject<List<Dictionary<string, string>>>(dic["InviteSubcontractor"]);
                foreach (var item in subSupplier)
                {
                    S_P_Invitation_Supplier supplier = new S_P_Invitation_Supplier();
                    supplier.ID = FormulaHelper.CreateGuid();
                    supplier.S_P_InvitationID = tenderingID;
                    supplier.SupplierID = item["SupplierID"];
                    supplier.SupplierName = item["SupplierName"];
                    supplier.SupplierContact = item["SupplierContact"];
                    supplier.SupplierEmail = item["SupplierEmail"];
                    supplier.SupplierContactPhone = item["SupplierContactPhone"];
                    supplier.Remark = item["Remark"];
                    this.EPCEntites.Set<S_P_Invitation_Supplier>().Add(supplier);
                }
            }
            else
            {
                this.EPCEntites.Set<S_P_Invitation_Supplier>().Delete(x => x.S_P_InvitationID == tenderingID);
            }
            this.EPCEntites.SaveChanges();
        }

        public override JsonResult Delete()
        {
            if (!String.IsNullOrEmpty(Request["ListIDs"]))
            {
                foreach (var item in Request["ListIDs"].Split(','))
                {
                    this.EPCEntites.Set<S_P_Invitation_Supplier>().Delete(x => x.S_P_InvitationID == item);
                    this.EPCEntites.Set<S_P_Invitation_EngineeringRelaltion>().Delete(x => x.S_P_InvitationID == item);
                    this.EPCEntites.Set<S_P_Invitation>().Delete(x => x.ID == item);
                    this.EPCEntites.Set<T_C_TenderingInfo>().Delete(x => x.ID == item);
                }
                this.EPCEntites.SaveChanges();
            }
            return Json("");
        }

    }
}
