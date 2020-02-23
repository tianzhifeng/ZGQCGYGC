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
using Newtonsoft.Json;
using Formula.ImportExport;
using EPC.Areas.Cooperation.Models;
using MvcAdapter;
using Base.Logic.Domain;

namespace EPC.Areas.Cooperation.Controllers
{
    public class WorkingContactController : EPCFormContorllor<T_C_WorkingContact>
    {
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult List()
        {
            var tab = new Tab();
            var statusEnum = CategoryFactory.GetCategory("EPC.ContactStatus", "状态", "Status");
            var status = GetQueryString("Status").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (status.Length > 0)
                statusEnum.SetMultiDefaultItem(status.ToList());
            statusEnum.Multi = false;
            tab.Categories.Add(statusEnum);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            return View();
        }
        public ActionResult AllList()
        {
            var tab = new Tab();
            var statusEnum = CategoryFactory.GetCategory("EPC.ContactStatus", "状态", "Status");
            var status = GetQueryString("Status").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (status.Length > 0)
                statusEnum.SetMultiDefaultItem(status.ToList());
            statusEnum.Multi = false;
            tab.Categories.Add(statusEnum);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            return View();
        }
        public JsonResult GetList(QueryBuilder qb)
        {
            string engineeringInfo = GetQueryString("EngineeringInfo");
            string sql =string.Format(@"select * from T_C_WorkingContact where EngineeringInfo='{0}' and MessageCompany='' ", engineeringInfo);
            var data = this.EPCSQLDB.ExecuteGridData(sql, qb);
            return Json(data);
        }
        public JsonResult GetAllList(QueryBuilder qb)
        {
            string engineeringInfo = GetQueryString("EngineeringInfo");
            string sql = string.Format(@"select * from T_C_WorkingContact where EngineeringInfo='{0}'  ", engineeringInfo);
            var data = this.EPCSQLDB.ExecuteGridData(sql, qb);
            return Json(data);
        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                string engineeringInfo = GetQueryString("EngineeringInfo");
                if (string.IsNullOrEmpty(engineeringInfo))
                    throw new Formula.Exceptions.BusinessValidationException("请选择项目！");

                string sql = string.Format("select * from S_I_Engineering where ID='{0}' ", engineeringInfo);
                var dt = EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Formula.Exceptions.BusinessValidationException("所选项目不存在！");
                var engineering = FormulaHelper.DataRowToDic(dt.Rows[0]);

                dic.SetValue("EngineeringInfo", engineering.GetValue("ID"));
                dic.SetValue("EngineeringInfoName", engineering.GetValue("Name"));
                dic.SetValue("Code", engineering.GetValue("SerialNumber"));

                SerialNumberParam param = new SerialNumberParam()
                {
                    Code = "TCMWorkingContact",
                    PrjCode = "",
                    OrgCode = "",
                    UserCode = "",
                    CategoryCode = "",
                    SubCategoryCode = "",
                    OrderNumCode = ""
                };
                var serialNumber = SerialNumberHelper.GetSerialNumberString("{YY}{MM}{DD}-{NNNN}", param, "YearCode,MonthCode",true);

                dic.SetValue("SerialNumber", serialNumber);
                dic.SetValue("MainCompany", engineering.GetValue("CustomerInfo"));
                dic.SetValue("MainCompanyName", engineering.GetValue("CustomerInfoName"));
                dic.SetValue("SendCompany", CurrentUserInfo.UserOrgID);
                dic.SetValue("SendCompanyName", CurrentUserInfo.UserOrgName);
                dic.SetValue("ID", FormulaHelper.CreateGuid());
            }

        }

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                dic.SetValue("FlowPhase", "End");
                dic.SetValue("EngineeringInfoID", dic.GetValue("EngineeringInfo"));
                dic.SetValue("MessageCompany", string.Empty);
                dic.SetValue("MessageCompanyName", string.Empty);
                dic.SetValue("Status", "Add");
            }
        }

        protected override void AfterSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            LoggerHelper.InserLogger("T_C_WorkingContact", isNew ? EnumOperaType.Add : EnumOperaType.Update, dic.GetValue("ID"), dic.GetValue("EngineeringInfo"), dic.GetValue("Name"));
        }


        //接收联系单
        [LoggerFilter(OperaType = EnumOperaType.Update, Table = "T_C_WorkingContact", IDField = "ListData.ID", NameField = "ListData.Name", Remark = "接收-工作联系单")]
        public JsonResult Receive()
        {
            string listDataStr = GetQueryString("ListData");
            var dicList = JsonHelper.ToList(listDataStr);
            if (!dicList.Exists(d => d.GetValue("Status") == "Add"))
                throw new Formula.Exceptions.BusinessValidationException("只有新增的联系单才能接收，请确认！");

            var idList = dicList.Select(a => a.GetValue("ID"));
            string sql = string.Format("update T_C_WorkingContact set Status='Received',ReceiveDate=GETDATE(),ReceiveUserID='{1}',ReceiveUserName='{2}' where ID in('{0}') and Status='Add' ",
    string.Join("','", idList), CurrentUserInfo.UserID, CurrentUserInfo.UserName);
            this.EPCSQLDB.ExecuteNonQuery(sql);
            return Json("");
        }

        //处理工作单
        [LoggerFilter(OperaType = EnumOperaType.Update, Table = "T_C_WorkingContact", IDField = "ListData.ID", NameField = "ListData.Name", Remark = "处理-工作联系单")]
        public JsonResult Resolve()
        {
            string listDataStr = GetQueryString("ListData");
            var dicList = JsonHelper.ToList(listDataStr);
            if (!dicList.Exists(d => d.GetValue("Status") == "Received"))
                throw new Formula.Exceptions.BusinessValidationException("只有已接收的联系单才能处理，请确认！");

            var idList = dicList.Select(a => a.GetValue("ID"));
            string sql = string.Format("update T_C_WorkingContact set Status='Resolved',ResolveDate=GETDATE(),ResolveUserID='{1}',ResolveUserName='{2}' where ID in('{0}') and Status='Received' ",
    string.Join("','", idList), CurrentUserInfo.UserID, CurrentUserInfo.UserName);
            this.EPCSQLDB.ExecuteNonQuery(sql);
            return Json("");
        }

        public JsonResult GetModel(string id)
        {
            var dic = new Dictionary<string, object>();
            bool isNew = true;
            if (!String.IsNullOrEmpty(id))
            {
                var sql = String.Format("select * from {0} where ID='{1}'", "T_C_WorkingContact", id);
                var dt = this.EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    dic = FormulaHelper.DataRowToDic(dt.Rows[0]);
                    isNew = false;
                }
                else
                {
                    throw new Formula.Exceptions.BusinessValidationException(String.Format("数据表【{0}】中没有找到ID为【{1}】的记录，无法读取数据", "T_C_WorkingContact", id));
                }
            }
            AfterGetData(dic, isNew,"");
            return Json(dic);
        }

    }
}
