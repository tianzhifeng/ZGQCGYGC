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
using EPC.Areas.Cooperation.Models;

namespace EPC.Areas.Construction.Controllers
{
    public class WorkingContactController : EPCFormContorllor<T_C_WorkingContact>
    {

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
            else
            {
                string sql = string.Format("select * from T_C_WorkingContact where ID='{0}' ", dic.GetValue("ID"));
                var dt = EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Formula.Exceptions.BusinessValidationException("联系单不存在，请确认！");

                string actionType = Request["actionType"];
                if (!string.IsNullOrEmpty(actionType) && actionType == "Resolve")
                {
                    var status = dt.Rows[0]["Status"].ToString();
                    if (!string.IsNullOrEmpty(status) && status == "Receive")
                    {
                        dic.SetValue("Status", "Resolve");
                        dic.SetValue("ResolveDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                    }
                    else
                    {
                        throw new Formula.Exceptions.BusinessValidationException("只有已接收的联系单才能处理，请确认！");
                    }
                }


            }

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


    }
}
