using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPC.Logic.Domain;
using Config.Logic;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
using Formula.Helper;
using System.Data.Objects.SqlClient;

namespace EPC.Areas.Construction.Controllers
{
    public class ContractInfoSettlePeriodController : EPCController
    {
        public ActionResult List()
        {
            return View();
        }

        public JsonResult GetUnFinishedPeriods()
        {
            string contractInfoID = GetQueryString("ContractInfoID");
            var periods = entities.Set<S_P_ContractInfo_Fb_SettlePeriod>()
                .Where(a => a.ContractInfoID == contractInfoID && !a.Finished).OrderBy(a => a.PeriodNum);
            return Json(periods);
        }

        public JsonResult DeletePeriod(string dataID)
        {
            S_P_ContractInfo_Fb_SettlePeriod period = entities.Set<S_P_ContractInfo_Fb_SettlePeriod>().Find(dataID);
            if (period == null)
            {
                //throw new Formula.Exceptions.BusinessValidationException("ID为【" + id + "】的计量期不存在");
            }
            else if (period.Finished)
            {
                throw new Formula.Exceptions.BusinessValidationException("第" + period.PeriodNum + "计量期已结束,不能删除。");
            }
            else if (entities.Set<S_P_ContractInfo_Fb_BOQCheck>().Any(a => a.SettlePeriodID == dataID))
            {
                throw new Formula.Exceptions.BusinessValidationException("第" + period.PeriodNum + "计量期已有计量记录，因此计量期不能删除");
            }
            else
            {                
                //后续的期号重排
                var periods = entities.Set<S_P_ContractInfo_Fb_SettlePeriod>().Where(a => a.PeriodNum > period.PeriodNum && a.ContractInfoID == period.ContractInfoID);
                foreach (var p in periods)
                {
                    p.PeriodNum -= 1;
                }
                entities.Set<S_P_ContractInfo_Fb_SettlePeriod>().Delete(a => a.ID == dataID);
            }
            entities.SaveChanges();
            return Json("");
        }
        //protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        //{
        //    var contractInfoID = GetQueryString("ContractInfoID");
        //    S_P_ContractInfo_Fb_SettlePeriod tmp = EPCEntites.Set<S_P_ContractInfo_Fb_SettlePeriod>().Where(a => a.ContractInfoID == contractInfoID).OrderByDescending(a => a.PeriodNum).FirstOrDefault();
        //    int currentNum = 1;
        //    if (tmp != null)
        //    {
        //        currentNum = tmp.PeriodNum + 1;
        //    }
        //    dic.SetValue("PeriodNum", currentNum.ToString());
        //    base.BeforeSave(dic, formInfo, isNew);
        //}

        public JsonResult GetPeriodList(QueryBuilder qb)
        {           
            string contractInfoID = GetQueryString("ContractInfoID");
            qb.SortField = "PeriodNum";
            qb.SortOrder = "asc";
            var results = (from a in entities.Set<S_P_ContractInfo_Fb_SettlePeriod>().Where(a => a.ContractInfoID == contractInfoID)
                          join b in entities.Set<S_P_ContractInfo_Fb_BOQCheck>()
                          on a.ID equals b.SettlePeriodID into abR
                          from c in abR.DefaultIfEmpty()
                          group new { a, c } by new { a.ID, a.PeriodNum, a.PeriodName, a.Remark, a.BeginDate, a.EndDate,a.Finished } into d
                          select new
                          {
                              ID = d.Key.ID,
                              PeriodNum = d.Key.PeriodNum,
                              PeriodName = d.Key.PeriodName,
                              BeginDate = d.Key.BeginDate,
                              EndDate = d.Key.EndDate,
                              Remark = d.Key.Remark,
                              Finished = d.Key.Finished,
                              BOQCheckCount = d.Count(a => a.c != null) == 0 ? "" : SqlFunctions.StringConvert((double)d.Count(a => a.c != null))
                          }).Where(qb);

            return Json(results);
        }

        public JsonResult AddPeriod(string ContractInfoID)
        {
            if (!entities.Set<S_P_ContractInfo>().Any(a => a.ID == ContractInfoID))
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到ID为【" + ContractInfoID + "】的合同信息");
            }
            S_P_ContractInfo_Fb_SettlePeriod tmp = entities.Set<S_P_ContractInfo_Fb_SettlePeriod>().Where(a => a.ContractInfoID == ContractInfoID).OrderByDescending(a => a.PeriodNum).FirstOrDefault();
            int currentNum = 1;
            if (tmp != null)
            {
                currentNum = tmp.PeriodNum + 1;
            }

            entities.Set<S_P_ContractInfo_Fb_SettlePeriod>().Add(new S_P_ContractInfo_Fb_SettlePeriod()
            {
                ID = FormulaHelper.CreateGuid(),
                ContractInfoID = ContractInfoID,
                PeriodName = "第" + currentNum + "期",
                BeginDate = tmp != null ? tmp.EndDate : DateTime.Now,
                EndDate = DateTime.Now,
                PeriodNum = currentNum,
                CreateDate = DateTime.Now,
                CreateUser = CurrentUserInfo.UserName,
                CreateUserID = CurrentUserInfo.UserID,
                OrgID = CurrentUserInfo.UserOrgID
            });
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult UpdatePeriod(string data)
        {
            var dic = JsonHelper.ToObject(data);
            var d = this.GetEntityByID<S_P_ContractInfo_Fb_SettlePeriod>(dic.GetValue("ID"));
            if (d.Finished)
            {
                throw new Formula.Exceptions.BusinessValidationException("第" + d.PeriodNum + "计量期已结束,不能编辑。");
            }
            if (d != null)
            {
                string id = dic.GetValue("ID");
                this.UpdateEntity<S_P_ContractInfo_Fb_SettlePeriod>(d, dic);
                entities.SaveChanges();
            }
            return Json("");
        }

        public JsonResult SaveList(string listData)
        {
            var list = JsonHelper.ToList(listData);
            foreach (var item in list)
            {
                string dataStateFromUICtrl = item.GetValue("_state").ToLower(); 
                if (dataStateFromUICtrl == "modified")
                {
                    var data = this.GetEntityByID<S_P_ContractInfo_Fb_SettlePeriod>(item.GetValue("ID"));
                    if (data != null)
                    {
                        if (data.Finished)
                        {
                            throw new Formula.Exceptions.BusinessValidationException("第" + data.PeriodNum + "计量期已结束,不能编辑。");
                        }

                        string id = item.GetValue("ID");
                        this.UpdateEntity<S_P_ContractInfo_Fb_SettlePeriod>(data, item);
                    }
                }
                else if (dataStateFromUICtrl == "added")
                {
                    S_P_ContractInfo_Fb_SettlePeriod period = new S_P_ContractInfo_Fb_SettlePeriod();
                    period.ID = FormulaHelper.CreateGuid();
                    period.ContractInfoID = item.GetValue("ContractInfoID");
                    period.PeriodNum = Convert.ToInt32(item.GetValue("PeriodNum"));
                    period.PeriodName = item.GetValue("PeriodName");
                    period.BeginDate = Convert.ToDateTime(item.GetValue("BeginDate"));
                    period.EndDate = Convert.ToDateTime(item.GetValue("EndDate"));
                    period.Remark = item.GetValue("Remark");
                    period.CreateDate = DateTime.Now;
                    period.CreateUser = CurrentUserInfo.UserName;
                    period.CreateUserID = CurrentUserInfo.UserID;
                    period.OrgID = CurrentUserInfo.UserOrgID;
                    entities.Set<S_P_ContractInfo_Fb_SettlePeriod>().Add(period);
                }
            }
            entities.SaveChanges();
            return Json("");
        }
    }
}
