using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPC.Logic.Domain;
using Config.Logic;
using EPC.Logic;

namespace EPC.Areas.Construction.Controllers
{
    public class ContractInfoBOQPriceCheckController : EPCFormContorllor<S_P_ContractInfo_Fb_BOQPriceCheck>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            base.BeforeSave(dic, formInfo, isNew);
            //给S_P_ContractInfo_Fb_BOQPriceCheck的SettlePeriodID赋值
            string boqCheckIDs = dic.GetValue("BOQCheckIDs");
            if (!string.IsNullOrEmpty(boqCheckIDs))
            {
                string[] arrboqCheckID = boqCheckIDs.Split(',');
                if (arrboqCheckID.Length != 0)
                {
                    string boqCheckID = arrboqCheckID[0];
                    var boqCheck = EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck>().Find(boqCheckID);
                    if (boqCheck != null)
                    {
                        dic.SetValue("SettlePeriodID", boqCheck.SettlePeriodID);
                    }
                }
            }
        }

        protected override void OnFlowEnd(S_P_ContractInfo_Fb_BOQPriceCheck entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            foreach (var tmp in entity.S_P_ContractInfo_Fb_BOQPriceCheck_Detail.ToList())
            {
                string boqID = tmp.S_C_BOQID;
                decimal justCheckPrice = tmp.ConfirmPrice ?? 0;
                S_C_BOQ boq = EPCEntites.Set<S_C_BOQ>().Find(boqID);
                //累计已计价部分
                boq.CheckPriceTotal = (boq.CheckPriceTotal ?? 0) + justCheckPrice;
            }
        }      

        public JsonResult GetBOQPriceCheckInfoAndDetail(string ContractInfoID, string PriceCheckID, string BOQCheckIDs)
        {
            #region 起止时间
            if (string.IsNullOrEmpty(BOQCheckIDs))
            {
                return Json("");
            }
            string[] BOQCheckIDArr = BOQCheckIDs.Split(',');
            var tmp = from a in EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck>().Where(a => BOQCheckIDArr.Contains(a.ID))
                      join b in EPCEntites.Set<S_P_ContractInfo_Fb_SettlePeriod>()
                      on a.SettlePeriodID equals b.ID
                      group new { a, b } by new { } into c
                      select new
                      {
                          BeginDate = c.Min(a => a.b.BeginDate),
                          EndDate = c.Max(a => a.b.EndDate)
                      };
            var dateResult = tmp.SingleOrDefault();
            #endregion

            #region detail
            //本结算期
            var thisPeriod = EPCEntites.Set<S_P_ContractInfo_Fb_SettlePeriod>()
                .Join(EPCEntites.Set<S_P_ContractInfo_Fb_BOQPriceCheck>().Where(a => a.ID == PriceCheckID),
                a => a.ID, b => b.SettlePeriodID, (a, b) => new { PeriodNum = a.PeriodNum, PeriodID = a.ID }).SingleOrDefault();

            if (thisPeriod == null)
            {
                //异常
                throw new Formula.Exceptions.BusinessValidationException("未找到结算期");
            }
            
            //上期末的detail
            var periodBeforeDetail = from a in EPCEntites.Set<S_P_ContractInfo_Fb_BOQPayCheck>().Where(a => a.ContractInfoID == ContractInfoID)
                                     join b in EPCEntites.Set<S_P_ContractInfo_Fb_BOQPayCheck_Detail>().Where(a => a.PayCheckDetailType != (int)PayCheckDetailTemplateType.BOQ && a.PayCheckDetailType != (int)PayCheckDetailTemplateType.Summary)//排除首尾行
                                     on a.ID equals b.S_P_ContractInfo_Fb_BOQPayCheckID
                                     join c in EPCEntites.Set<S_P_ContractInfo_Fb_SettlePeriod>().Where(a => a.PeriodNum <= thisPeriod.PeriodNum)//之前所有期
                                     on a.SettlePeriodID equals c.ID into abc
                                     from abcR in abc.DefaultIfEmpty()
                                     select new
                                     {
                                         TempleID = b.TempleID,
                                         Name = b.Name,
                                         TemplePrice = b.ConfirmPrice,
                                         PeriodID = abcR.ID,
                                         PeriodNum = abcR.PeriodNum
                                     };

            //上期末的项目付款金额累计值
            var periodBeforeDetailGroup = periodBeforeDetail.GroupBy(a => a.TempleID).Select(a => new
            {
                TempleID = a.FirstOrDefault().TempleID,
                Name = a.FirstOrDefault().Name,
                LastPeriodTotalPrice = a.Where(b => b.PeriodNum < thisPeriod.PeriodNum).Sum(b => b.TemplePrice),//上期末
                PeriodTotalPrice = a.Where(b => b.PeriodNum <= thisPeriod.PeriodNum).Sum(b => b.TemplePrice)//本期末
            });

            var details = (from a in EPCEntites.Set<S_P_ContractInfo_Fb_BOQPayCheck_DetailTemplate>()
                          join b in periodBeforeDetailGroup
                          on a.ID equals b.TempleID into ab
                          from frmab in ab.DefaultIfEmpty()
                          select new 
                          {
                              TempleID = a.ID,
                              Name = a.Name,
                              CalcDirect = a.CalcDirect,
                              LastPeriodTotalPrice = (frmab.LastPeriodTotalPrice ?? 0),
                              DeclarePrice = 0.0M,
                              ConfirmPrice = 0.0M,
                              PeriodTotalPrice = (frmab.PeriodTotalPrice ?? 0),
                              PayCheckDetailType = a.PayCheckDetailType
                          }).ToList();

            if (!details.Any(a => a.PayCheckDetailType == (int)PayCheckDetailTemplateType.BOQ))
            {
                throw new Formula.Exceptions.BusinessValidationException("模板中没有设置【" + PayCheckDetailTemplateType.BOQ.ToString() + "】项");
            }
            else if (!details.Any(a => a.PayCheckDetailType == (int)PayCheckDetailTemplateType.Summary))
            {
                throw new Formula.Exceptions.BusinessValidationException("模板中没有设置【" + PayCheckDetailTemplateType.Summary.ToString() + "】项");
            }


            var checkDetails = EPCEntites.Set<S_P_ContractInfo_Fb_BOQPriceCheck_Detail>().Where(a => a.S_P_ContractInfo_Fb_BOQPriceCheckID == PriceCheckID).ToList();
            
            //工程价款项
            var boqDetail = details.FirstOrDefault(a => a.PayCheckDetailType == (int)PayCheckDetailTemplateType.BOQ);
            details.Remove(boqDetail);//移除无值项
            //插入有值的项
            details.Insert(0, new
            {
                TempleID = boqDetail.TempleID,
                Name = boqDetail.Name,
                CalcDirect = boqDetail.CalcDirect,
                LastPeriodTotalPrice = checkDetails.Sum(a => a.LastPeriodTotalPrice ?? 0),
                DeclarePrice = checkDetails.Sum(a => a.DeclarePrice ?? 0),
                ConfirmPrice = checkDetails.Sum(a => a.ConfirmPrice ?? 0),
                PeriodTotalPrice = checkDetails.Sum(a => a.PeriodTotalPrice ?? 0),
                PayCheckDetailType = boqDetail.PayCheckDetailType
            });

            //汇总项
            var sumDetail = details.FirstOrDefault(a => a.PayCheckDetailType == (int)PayCheckDetailTemplateType.Summary);
            details.Remove(sumDetail);//移除无值项
            //插入有值的项
            details.Add(new
            {
                TempleID = sumDetail.TempleID,
                Name = sumDetail.Name,
                CalcDirect = sumDetail.CalcDirect,
                LastPeriodTotalPrice = details.Sum(a => a.LastPeriodTotalPrice),
                DeclarePrice = details.Sum(a => a.DeclarePrice),
                ConfirmPrice = details.Sum(a => a.ConfirmPrice),
                PeriodTotalPrice = details.Sum(a => a.PeriodTotalPrice),
                PayCheckDetailType = sumDetail.PayCheckDetailType
            });

            #endregion

            return Json(new { Detail = details, BeginEndDate = dateResult });
        }
    }
}
