using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Formula;
using Formula.Helper;
using System.Text;
using Config.Logic;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class ReimbursementFormGWFYMXController : OfficeAutoFormContorllor<S_EP_ReimbursementApply>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            base.AfterGetData(dt, isNew, upperVersionID);
            if (dt != null && dt.Rows.Count > 0)
            {
                var reimbursementApplyID = GetQueryString("ReimbursementApplyID");
                var reimbursementApplyInfo = this.BusinessEntities.Set<S_EP_ReimbursementApply>().Find(reimbursementApplyID);
                if (reimbursementApplyInfo == null)
                    throw new Formula.Exceptions.BusinessException("获取报销单信息失败，请联系管理员!");
                var reimbursementApplySXRYBID = GetQueryString("ReimbursementApplySXRYBID");
                var reimbursementApplySXRYBInfo = this.BusinessEntities.Set<S_EP_ReimbursementApply_SXRYB>().Find(reimbursementApplySXRYBID);
                if (reimbursementApplySXRYBInfo == null)
                    throw new Formula.Exceptions.BusinessException("获取报销单信息失败，请联系管理员!");
                var singleRow = dt.Rows[0];

                if (isNew == true)
                {
                    singleRow["TravelUser"] = reimbursementApplySXRYBInfo.ReimburseUser;
                    singleRow["TravelUserName"] = reimbursementApplySXRYBInfo.ReimburseUserName;
                    singleRow["TravelType"] = reimbursementApplySXRYBInfo.TravelRecordType;
                }
                if (reimbursementApplyInfo.CGQSSJ != null)
                    singleRow["CGQSSJ"] = reimbursementApplyInfo.CGQSSJ;
                if (reimbursementApplyInfo.CGJSSJ != null)
                    singleRow["CGJSSJ"] = reimbursementApplyInfo.CGJSSJ;

                //获取报销人已报销差旅费日期
                var sql = @"select rf.ReimbursementFormCode,rf.Advertiser,rf.AdvertiserName,GNFYJTF.StartingDate,GNFYJTF.ArrivedDate 
from S_EP_ReimbursementApply_GNFYJTF GNFYJTF
left join S_EP_ReimbursementApply rf on rf.ID = GNFYJTF.S_EP_ReimbursementApplyID
where rf.Advertiser in ('{0}') and GNFYJTF.StartingDate>='{1}' and GNFYJTF.ArrivedDate<='{2}' and rf.ID <> '{3}'
union
select rf.ReimbursementFormCode,GWFYMX.TravelUser,GWFYMX.TravelUserName,GWFYMX.DepartureDate,GWFYMX.LeftDate 
from S_EP_ReimbursementApply_GWFYMX GWFYMX
left join S_EP_ReimbursementApply rf on rf.ID = GWFYMX.S_EP_ReimbursementApplyID
where rf.Advertiser in ('{0}') and GWFYMX.DepartureDate>='{1}' and GWFYMX.LeftDate<='{2}' and rf.ID <> '{3}'";
                sql = string.Format(sql, reimbursementApplySXRYBInfo.ReimburseUser, reimbursementApplyInfo.ReimbursementEffectiveDateStart, reimbursementApplyInfo.ReimbursementEffectiveDateEnd, singleRow["ID"]);
                var BXRYBXCLFRQdt = this.SQLDB.ExecuteDataTable(sql);
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("\n var BXRYBXCLFRQ={0}", JsonHelper.ToJson(BXRYBXCLFRQdt));
                sb.AppendFormat("\n var reimbursementEffectiveDateStart='{0}'", reimbursementApplyInfo.ReimbursementEffectiveDateStart);
                sb.AppendFormat("\n var reimbursementEffectiveDateEnd='{0}'", reimbursementApplyInfo.ReimbursementEffectiveDateEnd);
                ViewBag.DataSource += sb.ToString();
            }

        }
        public JsonResult GetRelatedCurrencyRate(string relatedCurrencys, string leftDate, string departureDate)
        {
            if (!string.IsNullOrWhiteSpace(relatedCurrencys))
                relatedCurrencys = relatedCurrencys.Replace(",", "','");

            if (!string.IsNullOrWhiteSpace(leftDate))
                leftDate = leftDate.Replace("\"", "");

            if (!string.IsNullOrWhiteSpace(departureDate))
                departureDate = departureDate.Replace("\"", "");
            var tanslatedLeftDate = DateTime.Parse(leftDate);
            var tanslatedDepartureDate = DateTime.Parse(departureDate);
            var days = (tanslatedLeftDate - tanslatedDepartureDate).Days;
            var sql = @"select ForeignCurrency,SUM(ISNULL(MiddlePriceRate,0))/{2} as MiddlePriceRate  from E_BDS_BankForeignExchangeRate
where DataUpdatetTime >= '{0}' and DataUpdatetTime <='{1}' and ForeignCurrency in ('{3}')
group by ForeignCurrency";
            sql = string.Format(sql, tanslatedDepartureDate, tanslatedLeftDate, days, relatedCurrencys);
            var dt = this.SQLDB.ExecuteDataTable(sql);

            return Json(dt);
        }

        public JsonResult GetRelatedStandardInfo(string travelType, string country, string city, string startDate)
        {
            var returnList = new List<Dictionary<string, object>>();
            string GYYPZFBZID = string.Empty;
            string GYYPJYJLBZCurrency = string.Empty;
            string strCityWhere = string.Empty;
            if (!string.IsNullOrWhiteSpace(city))
                strCityWhere = "and City = '" + city + "'";

            if (!string.IsNullOrWhiteSpace(startDate))
                startDate = startDate.Replace("\"", "");

            string sql = @"select ID,  'ZCBTMYBT' as StandardType, Currency,Subsidyamount as StandardAmount,Businesstype,StartDate from E_BDS_OnsiteSubsidyStandard
where Businesstype like'%{0}%' and Country = '{1}' and StartDate <='{2}' {3}
union 
select ID, 'GYYPZFBZ' as StandardType,Currency,Accommodation as StandardAmount,Businesstype,StartDate from E_BDS_ForeignAccommodationStandard
where Businesstype ='{0}'  and Country = '{1}' and StartDate <='{2}'  {3}
union
select ID, 'GYWPZFBZ' as StandardType, Currency,Reward as StandardAmount,'' as Businesstype,StartDate from E_BDS_AccommodationSavingEaward
where  Country = '{1}' and StartDate <='{2}'  {3}
union
select ID, 'GYHSBTBZ' as StandardType,Currency,Mealallowance as StandardAmount, Businesstype,StartDate from E_BDS_ForeignFoodSubsidyStandard
where Businesstype like '%{0}%'   and Country = '{1}' and StartDate <='{2}' {3} 
union
select ID, 'GYGZFBTBZ' as StandardType,Currency,Subsidies as StandardAmount, Businesstype,StartDate from E_BDS_ForeignFoodSubsidyStandard
where Businesstype like '%{0}%'  and Country = '{1}' and StartDate <='{2}' {3}
";
            sql = string.Format(sql, travelType, country, startDate, strCityWhere);
            var dt = this.SQLDB.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                //驻场补贴
                var ZCBTMYBTRows = dt.Select("StandardType = 'ZCBTMYBT'").OrderByDescending(c => c.Field<DateTime>("StartDate")).ToArray();
                if (ZCBTMYBTRows != null && ZCBTMYBTRows.Count() > 0)
                {
                    var singleDic = new Dictionary<string, object>();
                    singleDic.SetValue("StandardType", "ZCBTMYBT");
                    singleDic.SetValue("StandardAmount", ZCBTMYBTRows[0]["StandardAmount"]);
                    singleDic.SetValue("StandardCurrency", ZCBTMYBTRows[0]["Currency"]);
                    returnList.Add(singleDic);
                }

                //伙食补贴
                var GYHSBTBZRows = dt.Select("StandardType = 'GYHSBTBZ'").OrderByDescending(c => c.Field<DateTime>("StartDate")).ToArray();
                if (GYHSBTBZRows != null && GYHSBTBZRows.Count() > 0)
                {
                    var singleDic = new Dictionary<string, object>();
                    singleDic.SetValue("StandardType", GYHSBTBZRows[0]["StandardType"]);
                    singleDic.SetValue("StandardAmount", GYHSBTBZRows[0]["StandardAmount"]);
                    singleDic.SetValue("StandardCurrency", GYHSBTBZRows[0]["Currency"]);
                    returnList.Add(singleDic);
                }

                //公杂费
                var GYGZFBTBZRows = dt.Select("StandardType = 'GYGZFBTBZ'").OrderByDescending(c => c.Field<DateTime>("StartDate")).ToArray();
                if (GYGZFBTBZRows != null && GYGZFBTBZRows.Count() > 0)
                {
                    var singleDic = new Dictionary<string, object>();
                    singleDic.SetValue("StandardType", GYGZFBTBZRows[0]["StandardType"]);
                    singleDic.SetValue("StandardAmount", GYGZFBTBZRows[0]["StandardAmount"]);
                    singleDic.SetValue("StandardCurrency", GYGZFBTBZRows[0]["Currency"]);
                    returnList.Add(singleDic);
                }

                //国外有票住宿费标准
                var GYYPZFBZRows = dt.Select("StandardType = 'GYYPZFBZ'").OrderByDescending(c => c.Field<DateTime>("StartDate")).ToArray();
                if (GYYPZFBZRows != null && GYYPZFBZRows.Count() > 0)
                {
                    GYYPZFBZID = GYYPZFBZRows[0]["ID"].ToString();
                    GYYPJYJLBZCurrency = GYYPZFBZRows[0]["Currency"].ToString();
                    var singleDic = new Dictionary<string, object>();
                    singleDic.SetValue("StandardType", GYYPZFBZRows[0]["StandardType"]);
                    singleDic.SetValue("StandardAmount", GYYPZFBZRows[0]["StandardAmount"]);
                    singleDic.SetValue("StandardCurrency", GYYPZFBZRows[0]["Currency"]);
                    returnList.Add(singleDic);
                }

                //国外无票住宿费标准
                var GYWPZFBZRows = dt.Select("StandardType = 'GYWPZFBZ'").OrderByDescending(c => c.Field<DateTime>("StartDate")).ToArray();
                if (GYWPZFBZRows != null && GYWPZFBZRows.Count() > 0)
                {
                    var singleDic = new Dictionary<string, object>();
                    singleDic.SetValue("StandardType", GYWPZFBZRows[0]["StandardType"]);
                    singleDic.SetValue("StandardAmount", GYWPZFBZRows[0]["StandardAmount"]);
                    singleDic.SetValue("StandardCurrency", GYWPZFBZRows[0]["Currency"]);
                    returnList.Add(singleDic);
                }
            }


            //有票住宿费节约奖励
            sql = @"select fsrrr.ID,PJRJYJLEQ,PJRJYJLEZ,fsrrr.Reward,fsrr.Country,fsrr.Currency   from E_BDS_ForeignSavingReward_Reward fsrrr
left join E_BDS_ForeignSavingReward fsrr on fsrrr.E_BDS_ForeignSavingRewardID = fsrr.ID  
where Country='{0}' and StartDate <='{1}' order by fsrrr.PJRJYJLEZ desc";
            sql = string.Format(sql, country,startDate);
            dt = this.SQLDB.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                var singleDic = new Dictionary<string, object>();
                var rows = dt.Rows;
                singleDic.SetValue("StandardType", "GYYPJYJLBZ");
                singleDic.SetValue("StandardAmount", Json(dt));
                singleDic.SetValue("StandardCurrency", rows[0]["Currency"]);
                returnList.Add(singleDic);
            }

            return Json(returnList);
        }

    }
}
