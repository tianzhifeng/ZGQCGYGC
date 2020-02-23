using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPC.Logic.Domain;
using Config.Logic;
using Newtonsoft.Json;
using Base.Logic.Domain;

namespace EPC.Areas.Construction.Controllers
{
    public class ContractInfoBOQCheckController : EPCFormContorllor<S_P_ContractInfo_Fb_BOQCheck>
    {
        protected override void OnFlowEnd(S_P_ContractInfo_Fb_BOQCheck entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            base.OnFlowEnd(entity, taskExec, routing);
            //将之前的所有计量期全部设置为完成
            S_P_ContractInfo_Fb_SettlePeriod period = EPCEntites.Set<S_P_ContractInfo_Fb_SettlePeriod>().Find(entity.SettlePeriodID);
            EPCEntites.Set<S_P_ContractInfo_Fb_SettlePeriod>().Where(a => a.PeriodNum < period.PeriodNum && a.ContractInfoID == period.ContractInfoID).ToList().Update(a => a.Finished = true);
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, S_UI_Form formInfo)
        {
            base.BeforeSaveSubTable(dic, subTableName, detailList, formInfo);
            if (detailList.Count == 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("计量的清单不能为空");
            }

            foreach (var tmp in detailList)
            {
                string boqID = tmp.GetValue("S_C_BOQID");
                string ID = tmp.GetValue("ID");
                string confirmQuantity = tmp.GetValue("ConfirmQuantity");
                string declareQuantity = tmp.GetValue("DeclareQuantity");
                decimal justCheckQuantity = Convert.ToDecimal(confirmQuantity);
                decimal justDelcareQuantity = Convert.ToDecimal(declareQuantity);
                //首次提交才进入
                if (EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck_Detail>().Find(ID) == null)
                {                    
                    S_C_BOQ boq = EPCEntites.Set<S_C_BOQ>().Find(boqID);

                    //后台判断新加入数量与已有数量和不能超过工程量清单
                    decimal confirmBoqInBoqCheck = EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck_Detail>().Where(a => a.S_C_BOQID == boqID).Select(a => a.ConfirmQuantity??0).ToList().Sum();
                    if (boq.Quantity < confirmBoqInBoqCheck + justCheckQuantity)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("清单【" + tmp.GetValue("Name") + "】本期确认得到的结果已超出清单工程量,请重新添加清单输入");
                    }

                    if (boq.Quantity < confirmBoqInBoqCheck + justDelcareQuantity)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("清单【" + tmp.GetValue("Name") + "】本期申报得到的结果已超出清单工程量,请重新添清单加输入");
                    }

                    //锁定boq
                    boq.Locked = true;
                    //累计已计量部分
                    boq.CheckQuantityTotal = (boq.CheckQuantityTotal ?? 0) + justCheckQuantity;
                }                
            }
        }

        //
        protected override void BeforeDelete(string[] Ids)
        {
            base.BeforeDelete(Ids);

            foreach (var Id in Ids)
            {
                //如果进度款审核表选择的是必须已通过的计量单则已通过计量单是不会删除的，在这种情况下这判断就显得多余 
                bool bExist = EPCEntites.Set<S_P_ContractInfo_Fb_BOQPriceCheck>().Any(a => a.BOQCheckIDs.Contains(Id));
                if (bExist)
                {
                    throw new Formula.Exceptions.BusinessValidationException("已经关联了分包合同的进度款审核表，无法删除");
                }
                //流程撤销或者删除的情况下需要处理S_C_BOQ的CheckQuantity
                else
                {
                    var boqCheck = EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck>().Find(Id);
                    var detailList = EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck_Detail>().Where(a => a.S_P_ContractInfo_Fb_BOQCheckID == Id).ToList();
                    foreach (S_P_ContractInfo_Fb_BOQCheck_Detail detail in detailList)
                    {
                        //累计部分删除已经确认的数量
                        S_C_BOQ boq = EPCEntites.Set<S_C_BOQ>().Find(detail.S_C_BOQID);
                        boq.CheckQuantityTotal = (boq.CheckQuantityTotal ?? 0) - detail.ConfirmQuantity;
                        //如果boq没有被任何计量单应用则lock改为false
                        //其中a.ID != detail.ID，排除自己(将要被删除所以不算在内)
                        if (!EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck_Detail>().Any(a => a.S_C_BOQID == detail.S_C_BOQID && a.ID != detail.ID))
                        {
                            boq.Locked = false;
                        }
                    }
                }
            }           
        }

        /// <summary>
        /// 根据boq的id及peroid的id,返回该period中boq的累计完成指标数据
        /// </summary>
        /// <param name="strBoqList"></param>
        /// <param name="periodID"></param>
        /// <returns></returns>
        public JsonResult GetPeriodBOQ(string strBoqList, string periodID)
        {
            List<Dictionary<string, object>> boqList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(strBoqList);
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
            foreach (var boq in boqList)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                results.Add(dic);
                dic.SetValue("Code", boq.GetValue("Code"));//清单编号
                dic.SetValue("S_C_BOQID", boq.GetValue("ID"));//清单编号
                dic.SetValue("Name", boq.GetValue("Name"));//清单名称
                dic.SetValue("Unit", boq.GetValue("Unit"));//单位
                dic.SetValue("ContractInfoQuantity", boq.GetValue("Quantity"));//合同工程量

                string boqID = boq.GetValue("ID");
                double quantity = 0;
                double.TryParse(boq.GetValue("Quantity"), out quantity);
                decimal lastPeroidTotal = 0.0M;
                #region 上期末累计
                {
                    var period = EPCEntites.Set<S_P_ContractInfo_Fb_SettlePeriod>().Find(periodID);
                    //之前的期数
                    var periods = EPCEntites.Set<S_P_ContractInfo_Fb_SettlePeriod>().Where(a => a.PeriodNum < period.PeriodNum && a.ContractInfoID == period.ContractInfoID);
                    //之前期数中所有计量单(跟通过没通过没关系,一旦记录进来就统计)
                    var peroidPassBOQCheck = from a in periods
                                             join b in EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck>()//.Where(a => a.FlowPhase == "End")跟通过没通过没关系,一旦记录进来就统计
                                             on a.ID equals b.SettlePeriodID
                                             select new
                                             {
                                                 BOQCheckID = b.ID
                                             };

                    //计量单下的所有boq
                    var periodBOQs = EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck_Detail>().Where(a => peroidPassBOQCheck.Any(b => b.BOQCheckID == a.S_P_ContractInfo_Fb_BOQCheckID));
                    lastPeroidTotal = periodBOQs.Where(a => a.S_C_BOQID == boqID).Select(a => a.ConfirmQuantity ?? 0).ToList().Sum();
                    //boq中已经完成的部分做求和
                    dic.SetValue("LastPeriodTotal", lastPeroidTotal);
                }
                #endregion

                //#region 本期末累计完成、本期完成、本期完成比例、累计完成比例
                //{
                //    //本期所有通过的计量单
                //    var thisperoidPassBOQCheck = EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck>().Where(a => /*跟通过没通过没关系,一旦记录进来就统计a.FlowPhase == "End" &&*/ a.SettlePeriodID == periodID);
                //    //本期所有通过的计量单的工程量清单
                //    var thisperiodBOQs = EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck_BOQ>().Where(a => thisperoidPassBOQCheck.Any(b => b.ID == a.S_P_ContractInfo_Fb_BOQCheckID));
                //    //本期对应boq的确认量
                //    double thisPeriodTotal = thisperiodBOQs.Where(a => a.S_C_BOQID == boqID).Select(a => a.ConfirmQuantity).ToList().Sum();

                //    //本期完成比例
                //    dic.SetValue("PeroidProp", quantity == 0 ? "" : (thisPeriodTotal / quantity).ToString("p"));
                //    double allTotal = lastPeroidTotal + thisPeriodTotal;
                //    //本期末累计
                //    dic.SetValue("PeriodTotal", allTotal);
                //    //累计完成比例
                //    dic.SetValue("Prop", quantity == 0 ? "" : (allTotal / quantity).ToString("p"));
                //}
                //#endregion
            }

            return Json(results);
        }

        public JsonResult GetBOQCheckDetail(string boqCheckIDs)
        {
            if (string.IsNullOrEmpty(boqCheckIDs))
                return Json("");

            string[] idArr = boqCheckIDs.Split(',');
            var details = (from a in EPCEntites.Set<S_P_ContractInfo_Fb_BOQCheck_Detail>()
                           where idArr.Contains(a.S_P_ContractInfo_Fb_BOQCheckID)
                           join b in EPCEntites.Set<S_C_BOQ>()
                           on a.S_C_BOQID equals b.ID
                           group new { a, b } by new { a.S_C_BOQID, a.Code, a.Name, a.Unit, b.UnitPrice } into c
                           select new
                           {
                               S_C_BOQID = c.Key.S_C_BOQID,
                               //不取a.xx 采用的是当时清单信息
                               Code = c.Key.Code,
                               Name = c.Key.Name,
                               Unit = c.Key.Unit,
                               ContractInfoQuantity = c.Sum(a => a.a.ContractInfoQuantity),
                               LastPeriodTotal = c.Sum(a => a.a.LastPeriodTotal),
                               DeclareQuantity = c.Sum(a => a.a.DeclareQuantity),
                               ConfirmQuantity = c.Sum(a => a.a.ConfirmQuantity),
                               PeriodTotal = c.Sum(a => a.a.PeriodTotal),
                               //取b.xx 采用的是最新的信息
                               UnitPrice = c.Key.UnitPrice,
                               //LastPeriodTotalPrice = Convert.ToDecimal(a.LastPeriodTotal) * b.UnitPrice ?? 0,
                               //DeclarePrice = Convert.ToDecimal(a.DeclareQuantity) * b.UnitPrice ?? 0,
                               //ConfirmPrice = Convert.ToDecimal(a.ConfirmQuantity) * b.UnitPrice ?? 0,
                               //PeriodTotalPrice = Convert.ToDecimal(a.PeriodTotal) * b.UnitPrice ?? 0
                               //处理linq中不识别Convert.ToDecimal 转化为内存数据再进行Convert.ToDecimal转化
                           }).ToList().Select(a => new
                           {
                               S_C_BOQID = a.S_C_BOQID,
                               //不取a.xx 采用的是当时清单信息
                               Code = a.Code,
                               Name = a.Name,
                               Unit = a.Unit,
                               Quantity = a.ContractInfoQuantity,
                               LastPeriodTotal = a.LastPeriodTotal,
                               DeclareQuantity = a.DeclareQuantity,
                               ConfirmQuantity = a.ConfirmQuantity,
                               PeriodTotal = a.PeriodTotal,
                               //取b.xx 采用的是最新的信息
                               UnitPrice = a.UnitPrice,
                               LastPeriodTotalPrice = Convert.ToDecimal(a.LastPeriodTotal) * (a.UnitPrice ?? 0),
                               DeclarePrice = Convert.ToDecimal(a.DeclareQuantity) * (a.UnitPrice ?? 0),
                               ConfirmPrice = Convert.ToDecimal(a.ConfirmQuantity) * (a.UnitPrice ?? 0),
                               PeriodTotalPrice = Convert.ToDecimal(a.PeriodTotal) * (a.UnitPrice ?? 0)
                           });

            return Json(details);
        }
    }
}
