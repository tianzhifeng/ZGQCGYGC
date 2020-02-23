using EPC.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Formula;

namespace EPC.Areas.Manage.Controllers
{
    public class BidresultsanalysistableController : EPCFormContorllor<BM_Bidresultsanalysistable>
    {
        //
        // GET: /Manage/Bidresultsanalysistable/

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (dic["Bidresult"] == "中标")
            {

            }
        }
        #region 如果竞争对手时新增数据 反写竞争对手列表
        //如果竞争对手时新增数据 反写竞争对手列表
        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, S_UI_Form formInfo)
        {
            if (subTableName == "Competitors")
            {

                string EnterprisenameID = "";
                if (detail.ContainsKey("EnterprisenameID"))
                {
                    EnterprisenameID = detail["EnterprisenameID"];
                }
                EPCEntities epcentity = FormulaHelper.GetEntities<EPCEntities>();
                var Peerinfomanagement = epcentity.Set<S_M_Peerinfomanagement>();
                S_M_Peerinfomanagement model = new S_M_Peerinfomanagement();
                if (string.IsNullOrEmpty(EnterprisenameID))
                {
                    model.ID = FormulaHelper.CreateGuid();
                    model.Name = detail["Enterprisename"];
                    model.CreateUser = CurrentUserInfo.UserName;
                    model.CreateUserID = CurrentUserInfo.UserID;
                    model.CreateDate = DateTime.Now;
                    model.OrgID = CurrentUserInfo.UserOrgID;
                    model.CompanyID = CurrentUserInfo.UserCompanyID;
                    model.FlowPhase = "Start";
                    model.ModifyDate =DateTime.Now;
                    model.ModifyUserID = CurrentUserInfo.UserID;
                    model.ModifyUser = CurrentUserInfo.UserName;
                    model.Companyprofile = detail["Companyprofile"];
                    model.Operatingconditions = detail["Operatingconditions"];
                    model.Businessqualification = detail["Businessqualification"];
                    model.Technicalability = detail["Technicalability"];
                    model.Projectperformance = detail["Projectperformance"];
                    model.Certificationsystem = detail["Certificationsystem"];
                    Peerinfomanagement.Add(model);
                    epcentity.SaveChanges();
                }
            }
        }
        #endregion

    }
}
