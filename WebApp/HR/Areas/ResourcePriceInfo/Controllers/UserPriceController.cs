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
    public class UserPriceController : HRController<S_W_UserUnitPrice>
    {
        public override ActionResult List()
        {
            var tab = new Tab();
            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 5, 2, false, "所属年份");
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            yearCategory.Multi = false;
            tab.Categories.Add(yearCategory);

            var delCategory = CategoryFactory.GetCategoryByString("[{value:'0',text:'否'},{value:'1',text:'是'}]", "是否离职", "IsDeleted");
            delCategory.SetDefaultItem("0");
            delCategory.Multi = false;
            tab.Categories.Add(delCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            string sql = @"select ID as HRUserID,Name as UserName,Code as UserCode,Sex,DeptID,DeptName,JoinCompanyDate,UserID,IsDeleted
from T_Employee where UserID is not null and UserID <> ''";

            var item = qb.Items.FirstOrDefault(c => c.Field == "BelongYear");
            var belongYear = DateTime.Now.Year;
            if (item != null)
            {
                belongYear = Convert.ToInt32(item.Value);
                qb.Items.Remove(item);
            }

            var resourceDt = this.SqlHelper.ExecuteDataTable("select * from S_W_UserUnitPrice order by StartDate desc");
            var list = resourceDt.AsEnumerable();
            var dt = this.SqlHelper.ExecuteDataTable(sql, qb);
            var result = FormulaHelper.DataTableToListDic(this.SqlHelper.ExecuteDataTable(sql, qb));
            foreach (var dic in result)
            {
                for (int i = 1; i <= 12; i++)
                {
                    var priceRow = list.FirstOrDefault(c => c["HRUserID"].ToString() == dic.GetValue("HRUserID") && Convert.ToInt32(c["BelongMonth"]) == i
                        && Convert.ToInt32(c["BelongYear"]) == belongYear);
                    if (priceRow == null)
                    {
                        var date = new DateTime(belongYear, i, 1);
                        priceRow = list.FirstOrDefault(c => Convert.ToDateTime(c["StartDate"]) <= date
                            && c["HRUserID"].ToString() == dic.GetValue("HRUserID"));
                        if (priceRow == null)
                        {
                            dic.SetValue("UnitPrice_" + i.ToString(), 0);
                        }
                        else
                        {
                            dic.SetValue("UnitPrice_" + i.ToString(), priceRow["UnitPrice"]);
                            dic.SetValue("ResourceCode_" + i.ToString(), priceRow["ResourceCode"]);
                        }
                    }
                    else
                    {
                        dic.SetValue("UnitPrice_" + i.ToString(), priceRow["UnitPrice"]);
                        dic.SetValue("ResourceCode_" + i.ToString(), priceRow["ResourceCode"]);
                    }
                }
                var maxVersionRow = list.FirstOrDefault(c => c["HRUserID"].ToString() == dic.GetValue("HRUserID"));
                if (maxVersionRow != null)
                {
                    dic.SetValue("ModifyUser", maxVersionRow["ModifyUser"]);
                    dic.SetValue("ModifyDate", maxVersionRow["ModifyDate"]);
                    dic.SetValue("ResourceCode", maxVersionRow["ResourceCode"]);
                }
                dic.SetValue("BelongYear", belongYear);
            }
            var gridData = new GridData(result);
            gridData.total = qb.TotolCount;
            return Json(gridData);
        }

        public override JsonResult GetModel(string id)
        {
            var hrUserID = this.GetQueryString("HRUserID");
            if (String.IsNullOrEmpty(this.GetQueryString("BelongYear")))
            {
                throw new Formula.Exceptions.BusinessValidationException("必须指定年份");
            }
            if (String.IsNullOrEmpty(this.GetQueryString("BelongMonth")))
            {
                throw new Formula.Exceptions.BusinessValidationException("必须指定月份");
            }
            if (String.IsNullOrEmpty(hrUserID))
            {
                throw new Formula.Exceptions.BusinessValidationException("必须指定人员");
            }
            var hrUser = this.GetEntityByID<T_Employee>(hrUserID);
            if (hrUser == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("指定的人员不存在，请检查选中的员工是否在人员基本信息中存在");
            }
            var belongYear = Convert.ToInt32(this.GetQueryString("BelongYear"));
            var belongMonth = Convert.ToInt32(this.GetQueryString("BelongMonth"));
            var entity = this.entities.Set<S_W_UserUnitPrice>().FirstOrDefault(c => c.HRUserID == hrUserID && c.BelongYear ==
             belongYear && c.BelongMonth == belongMonth);
            if (entity == null)
            {
                var date = new DateTime(Convert.ToInt32(belongYear), Convert.ToInt32(belongMonth), 1);
                var dic = new Dictionary<string, object>();
                dic.SetValue("HRUserID", hrUserID);
                dic.SetValue("UserID", hrUser.UserID);
                dic.SetValue("UserName", hrUser.Name);
                dic.SetValue("BelongYear", belongYear);
                dic.SetValue("BelongMonth", belongMonth);
                dic.SetValue("StartDate", date);
                var lastEty = this.entities.Set<S_W_UserUnitPrice>().Where(c => c.HRUserID == hrUserID
                    && c.StartDate <= date).OrderByDescending(c => c.StartDate).FirstOrDefault();
                if (lastEty != null)
                {
                    dic.SetValue("UnitPrice", lastEty.UnitPrice);
                    dic.SetValue("ResourceCode", lastEty.ResourceCode);
                }
                return Json(dic);
            }
            else
            {
                return Json(entity);
            }
        }

        protected override void AfterGetMode(Dictionary<string, object> entityDic, S_W_UserUnitPrice entity, bool isNew)
        {
            if (isNew)
            {
                var hrUserID = this.GetQueryString("HRUserID");
                var htUser = this.entities.Set<T_Employee>().Find(hrUserID);
                if (htUser != null)
                {
                    entityDic.SetValue("HRUserID", htUser.ID);
                    entityDic.SetValue("UserID", htUser.UserID);
                    entityDic.SetValue("UserName", htUser.Name);
                    entityDic.SetValue("StartDate", DateTime.Now);
                }
            }
        }

        public JsonResult SetUserUnitPrice(string UserInfo, string formData)
        {
            var list = JsonHelper.ToList(UserInfo);
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
                var hrUserID = item.GetValue("HRUserID");
                if (String.IsNullOrEmpty(hrUserID)) continue;
                var hrUser = this.entities.Set<T_Employee>().Find(hrUserID);
                if (hrUser == null) continue;
                if (isRemoveLater)
                {
                    this.entities.Set<S_W_UserUnitPrice>().Delete(c => c.HRUserID == hrUser.ID && c.StartDate >= startDate);
                    this.entities.SaveChanges();
                }

                var entity = this.entities.Set<S_W_UserUnitPrice>().FirstOrDefault(c => c.HRUserID == hrUser.ID && c.BelongYear == belongYear && c.BelongMonth == belongMonth);
                if (entity != null)
                {
                    entity.UserName = hrUser.Name;
                    entity.UserID = hrUser.UserID;
                    entity.UnitPrice = String.IsNullOrEmpty(data.GetValue("UnitPrice")) ? 0m : Convert.ToDecimal(data.GetValue("UnitPrice"));
                    entity.StartDate = startDate;
                    entity.BelongYear = startDate.Year;
                    entity.BelongMonth = startDate.Month;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUser = this.CurrentUserInfo.UserName;
                    entity.ModifyUserID = this.CurrentUserInfo.UserID;
                }
                else
                {
                    entity = new S_W_UserUnitPrice();
                    entity.ID = FormulaHelper.CreateGuid();
                    entity.UserName = hrUser.Name;
                    entity.UserID = hrUser.UserID;
                    entity.HRUserID = hrUser.ID;
                    entity.UnitPrice = String.IsNullOrEmpty(data.GetValue("UnitPrice")) ? 0m : Convert.ToDecimal(data.GetValue("UnitPrice"));
                    entity.StartDate = startDate;
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUser = this.CurrentUserInfo.UserName;
                    entity.ModifyUserID = this.CurrentUserInfo.UserID;
                    entity.CreateDate = DateTime.Now;
                    entity.BelongYear = startDate.Year;
                    entity.BelongMonth = startDate.Month;
                    this.entities.Set<S_W_UserUnitPrice>().Add(entity);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SetUserResourceCode(string UserInfo, string formData)
        {
            var list = JsonHelper.ToList(UserInfo);
            var data = JsonHelper.ToObject(formData);
            if (String.IsNullOrEmpty(data.GetValue("StartDate")))
            {
                throw new Formula.Exceptions.BusinessValidationException("没有设置生效日期，无法批量设置");
            }
            if (String.IsNullOrEmpty(data.GetValue("BelongYear")))
            {
                throw new Formula.Exceptions.BusinessValidationException("没有设置生效年份，无法批量设置");
            }
            if (String.IsNullOrEmpty(data.GetValue("BelongMonth")))
            {
                throw new Formula.Exceptions.BusinessValidationException("没有设置生效月份，无法批量设置");
            }
            if (String.IsNullOrEmpty(data.GetValue("ResourceCode")))
            {
                throw new Formula.Exceptions.BusinessValidationException("没有资源等级，无法批量设置");
            }
            var resourceCode = data.GetValue("ResourceCode");
            var isRemoveLater = data.GetValue("RemoveLater") == true.ToString().ToLower() ? true : false;
            var startDate = Convert.ToDateTime(data.GetValue("StartDate"));
            var belongYear = Convert.ToInt32(data.GetValue("BelongYear"));
            var belongMonth = Convert.ToInt32(data.GetValue("BelongMonth"));

            var resourceList = this.entities.Set<S_W_ResourcePrice>().Where(c => c.ResourceCode == resourceCode
                && c.StartDate >= startDate).ToList();

            //此处需要判定起始日期是否有单价记录，如果没有单价记录，则需要去之前一个有单价记录的数据来作为起始日期的单价
            var startResInfo = resourceList.FirstOrDefault(c => c.StartDate == startDate);
            if (startResInfo == null)
            {
                startResInfo = this.entities.Set<S_W_ResourcePrice>().Where(c => c.ResourceCode == resourceCode && c.StartDate <= startDate).OrderByDescending(c => c.StartDate).FirstOrDefault();
                if (startResInfo != null)
                {
                    var resInfo = startResInfo.Clone<S_W_ResourcePrice>();
                    resInfo.StartDate = new DateTime(belongYear, belongMonth, 1);
                    resInfo.BelongYear = belongYear;
                    resInfo.BelongMonth = belongMonth;
                    resourceList.Add(resInfo);
                }
            }

            foreach (var item in list)
            {
                var hrUserID = item.GetValue("HRUserID");
                if (String.IsNullOrEmpty(hrUserID)) continue;
                var hrUser = this.entities.Set<T_Employee>().Find(hrUserID);
                if (hrUser == null) continue;
                if (isRemoveLater)
                {
                    this.entities.Set<S_W_UserUnitPrice>().Delete(c => c.HRUserID == hrUser.ID && c.StartDate >= startDate);
                    this.entities.SaveChanges();
                }

                foreach (var resource in resourceList)
                {
                    var entity = this.entities.Set<S_W_UserUnitPrice>().FirstOrDefault(c => c.HRUserID == hrUser.ID
                        && c.BelongYear == resource.BelongYear && c.BelongMonth == resource.BelongMonth);
                    if (entity != null)
                    {
                        entity.UserName = hrUser.Name;
                        entity.UserID = hrUser.UserID;
                        entity.ResourceCode = resource.ResourceCode;
                        entity.UnitPrice = resource.UnitPrice;
                        entity.StartDate = resource.StartDate;
                        entity.BelongYear = resource.BelongYear;
                        entity.BelongMonth = resource.BelongMonth;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyUser = this.CurrentUserInfo.UserName;
                        entity.ModifyUserID = this.CurrentUserInfo.UserID;
                    }
                    else
                    {
                        entity = new S_W_UserUnitPrice();
                        entity.ID = FormulaHelper.CreateGuid();
                        entity.UserName = hrUser.Name;
                        entity.UserID = hrUser.UserID;
                        entity.HRUserID = hrUser.ID;
                        entity.ResourceCode = resource.ResourceCode;
                        entity.UnitPrice = resource.UnitPrice;
                        entity.StartDate = resource.StartDate;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyUser = this.CurrentUserInfo.UserName;
                        entity.ModifyUserID = this.CurrentUserInfo.UserID;
                        entity.CreateDate = DateTime.Now;
                        entity.BelongYear = resource.BelongYear;
                        entity.BelongMonth = resource.BelongMonth;
                        this.entities.Set<S_W_UserUnitPrice>().Add(entity);
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

    }
}
