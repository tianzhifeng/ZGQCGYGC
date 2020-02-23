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
using System.Data;

namespace EPC.Areas.Manage.Controllers
{
    public class CustomerInfoController : EPCFormContorllor<S_M_CustomerInfo>
    {
        protected override void BeforeDelete(string[] Ids)
        {
            foreach (var Id in Ids)
            {
                var customer = this.GetEntityByID(Id);
                customer.ValidateDelete();
            }
        }

        public JsonResult GetLinksInfoByCustomer(string customer)
        {
            var linkManList= this.EPCEntites.Set<S_M_CustomerInfo_LinkMan>().Where(d => d.S_M_CustomerInfoID == customer).ToList();
            return Json(linkManList);
        }
        //员工登录智能提示
        public JsonResult SmartReminder(string key)
        {
            SQLHelper sqlHelp = SQLHelper.CreateSqlHelper("Engineering");
            string sqlCommand = "select ID,Name,CreateUser,CreateDate from S_M_CustomerInfo where Name like '%" + key + "%'";
            DataTable data = sqlHelp.ExecuteDataTable(sqlCommand);
            return Json(data);
        }
    }
}
