using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using Formula;
using HR.Logic;
using HR.Logic.Domain;
using System.Data;
using MvcAdapter;

namespace HR.Areas.ResourcePriceInfo.Controllers
{
    public class ResInfoController : HRController<S_W_ResourcePrice>
    {
        public override ActionResult List()
        {
            var tab = new Tab();
            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 5, 2, false, "所属年份");
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            yearCategory.Multi = false;
            tab.Categories.Add(yearCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            string sql = @"select * from (
select S_M_EnumItem.Code as ResourceCode,S_M_EnumItem.Name as ResourceName,S_M_EnumItem.SortIndex
from dbo.S_M_EnumItem
left join dbo.S_M_EnumDef on S_M_EnumItem.EnumDefID=S_M_EnumDef.ID
where S_M_EnumDef.Code='HR.ResourceLevel') tableInfo";

            var priceSQL = "select * from S_W_ResourcePrice";
            var item = qb.Items.FirstOrDefault(c => c.Field == "BelongYear");
            var belongYear = DateTime.Now.Year;
            if (item != null)
            {
                belongYear = Convert.ToInt32(item.Value);
                qb.Items.Remove(item);
            }
            priceSQL += " order by StartDate desc";

            var resourceDt = this.SqlHelper.ExecuteDataTable(priceSQL);
            var list = resourceDt.AsEnumerable();
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var result = FormulaHelper.DataTableToListDic(db.ExecuteDataTable(sql, qb));
            foreach (var dic in result)
            {
                for (int i = 1; i <= 12; i++)
                {
                    var priceRow = list.FirstOrDefault(c => c["ResourceCode"].ToString() == dic.GetValue("ResourceCode")
                        && Convert.ToInt32(c["BelongMonth"]) == i && Convert.ToInt32(c["BelongYear"]) == belongYear);
                    if (priceRow == null)
                    {
                        var date = new DateTime(belongYear, i, 1);
                        priceRow = list.FirstOrDefault(c => Convert.ToDateTime(c["StartDate"]) <= date
                            && c["ResourceCode"].ToString() == dic.GetValue("ResourceCode"));
                        if (priceRow == null)
                        {
                            dic.SetValue("UnitPrice_" + i.ToString(), 0);
                        }
                        else
                        {
                            dic.SetValue("UnitPrice_" + i.ToString(), priceRow["UnitPrice"]);
                        }
                    }
                    else
                    {
                        dic.SetValue("UnitPrice_" + i.ToString(), priceRow["UnitPrice"]);
                    }
                }
                var maxVersionRow = list.FirstOrDefault(c => c["ResourceCode"].ToString() == dic.GetValue("ResourceCode"));
                if (maxVersionRow != null)
                {
                    dic.SetValue("ModifyUser", maxVersionRow["ModifyUser"]);
                    dic.SetValue("ModifyDate", maxVersionRow["ModifyDate"]);
                    dic.SetValue("Postion", maxVersionRow["Postion"]);
                }
                dic.SetValue("BelongYear", belongYear);
            }
            var gridData = new GridData(result);
            gridData.total = qb.TotolCount;
            return Json(gridData);
        }

        public override JsonResult GetModel(string id)
        {
            var resourceCode = this.GetQueryString("ResourceCode");
            var resourceName = this.GetQueryString("ResourceName");

            if (String.IsNullOrEmpty(this.GetQueryString("BelongYear")))
            {
                throw new Formula.Exceptions.BusinessValidationException("必须指定年份");
            }
            if (String.IsNullOrEmpty(this.GetQueryString("BelongMonth")))
            {
                throw new Formula.Exceptions.BusinessValidationException("必须指定月份");
            }
            if (String.IsNullOrEmpty(resourceCode))
            {
                throw new Formula.Exceptions.BusinessValidationException("必须指定资源等级");
            }
            var belongYear = Convert.ToInt32(this.GetQueryString("BelongYear"));
            var belongMonth = Convert.ToInt32(this.GetQueryString("BelongMonth"));
            var entity = this.entities.Set<S_W_ResourcePrice>().FirstOrDefault(c => c.ResourceCode == resourceCode && c.BelongYear ==
               belongYear && c.BelongMonth == belongMonth);
            if (entity == null)
            {
                var date = new DateTime(Convert.ToInt32(belongYear), Convert.ToInt32(belongMonth), 1);
                var dic = new Dictionary<string, object>();
                dic.SetValue("ResourceCode", resourceCode);
                dic.SetValue("ResourceName", resourceName);
                dic.SetValue("BelongYear", belongYear);
                dic.SetValue("BelongMonth", belongMonth);
                dic.SetValue("StartDate", date);
                var lastEty = this.entities.Set<S_W_ResourcePrice>().Where(c => c.ResourceCode == resourceCode
                    && c.StartDate <= date).OrderByDescending(c => c.StartDate).FirstOrDefault();
                if (lastEty != null)
                {
                    dic.SetValue("UnitPrice", lastEty.UnitPrice);
                    dic.SetValue("Postion", lastEty.Postion);
                    dic.SetValue("Level", lastEty.Level);
                }
                return Json(dic);
            }
            else
            {
                return Json(entity);
            }
        }

        public JsonResult PushToUserPrice(string ResourceList, string formData)
        {
            var list = JsonHelper.ToList(ResourceList);
            var data = JsonHelper.ToObject(formData);
            if (String.IsNullOrEmpty(data.GetValue("StartDate")))
            {
                throw new Formula.Exceptions.BusinessValidationException("没有设置生效日期，无法批量设置单价");
            }
            if (String.IsNullOrEmpty(data.GetValue("BelongYear")))
            {
                throw new Formula.Exceptions.BusinessValidationException("没有设置生效年份，无法批量设置单价");
            }
            if (String.IsNullOrEmpty(data.GetValue("BelongMonth")))
            {
                throw new Formula.Exceptions.BusinessValidationException("没有设置生效月份，无法批量设置单价");
            }
            var startDate = Convert.ToDateTime(data.GetValue("StartDate"));
            var belongYear = Convert.ToInt32(data.GetValue("BelongYear"));
            var belongMonth = Convert.ToInt32(data.GetValue("BelongMonth"));
            var isRemoveLater = data.GetValue("RemoveLater") == true.ToString().ToLower() ? true : false;

            foreach (var item in list)
            {
                var resCode = item.GetValue("ResourceCode");
                if (String.IsNullOrEmpty(resCode)) continue;
                var resourceList = this.entities.Set<S_W_ResourcePrice>().Where(c => c.ResourceCode == resCode
               && c.StartDate >= startDate).ToList();
                //此处需要判定起始日期是否有单价记录，如果没有单价记录，则需要去之前一个有单价记录的数据来作为起始日期的单价
                var startResInfo = resourceList.FirstOrDefault(c => c.StartDate == startDate);
                if (startResInfo == null)
                {
                    startResInfo = this.entities.Set<S_W_ResourcePrice>().Where(c => c.ResourceCode == resCode && c.StartDate <= startDate).OrderByDescending(c => c.StartDate).FirstOrDefault();
                    if (startResInfo != null)
                    {
                        var resInfo = startResInfo.Clone<S_W_ResourcePrice>();
                        resInfo.StartDate = new DateTime(belongYear, belongMonth, 1);
                        resInfo.BelongYear = belongYear;
                        resInfo.BelongMonth = belongMonth;
                        resourceList.Add(resInfo);
                    }
                }
                var userPriceList = this.entities.Set<S_W_UserUnitPrice>().Where(c => c.ResourceCode == resCode).ToList();
                var userInfoList = userPriceList.Select(c => new { UserName = c.UserName, UserID = c.UserID, HRUserID = c.HRUserID }).Distinct().ToList();

                foreach (var userInfo in userInfoList)
                {
                    foreach (var resPriceInfo in resourceList.OrderBy(c => c.StartDate).ToList())
                    {
                        var priceInfo = userPriceList.FirstOrDefault(c => c.UserID == userInfo.UserID && c.StartDate == resPriceInfo.StartDate);
                        if (priceInfo == null)
                        {
                            priceInfo = new S_W_UserUnitPrice();
                            priceInfo.ID = FormulaHelper.CreateGuid();
                            priceInfo.UserName = userInfo.UserName;
                            priceInfo.UserID = userInfo.UserID;
                            priceInfo.HRUserID = userInfo.HRUserID;
                            priceInfo.ResourceCode = resPriceInfo.ResourceCode;
                            priceInfo.UnitPrice = resPriceInfo.UnitPrice;
                            priceInfo.StartDate = resPriceInfo.StartDate;
                            priceInfo.ModifyDate = DateTime.Now;
                            priceInfo.ModifyUser = this.CurrentUserInfo.UserName;
                            priceInfo.ModifyUserID = this.CurrentUserInfo.UserID;
                            priceInfo.CreateDate = DateTime.Now;
                            priceInfo.BelongYear = resPriceInfo.BelongYear;
                            priceInfo.BelongMonth = resPriceInfo.BelongMonth;
                            this.entities.Set<S_W_UserUnitPrice>().Add(priceInfo);

                        }
                        else if (priceInfo.ResourceCode == resPriceInfo.ResourceCode)
                        {
                            priceInfo.UnitPrice = resPriceInfo.UnitPrice;
                            priceInfo.ModifyDate = DateTime.Now;
                            priceInfo.ModifyUser = this.CurrentUserInfo.UserName;
                            priceInfo.ModifyUserID = this.CurrentUserInfo.UserID;
                        }
                    }
                }
                this.entities.SaveChanges();
            }          
            return Json("");
        }

    }
}
