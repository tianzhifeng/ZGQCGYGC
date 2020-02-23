using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config.Logic;
using EPC;
using EPC.Logic;
using Formula;
using Formula.Helper;
using Config;

namespace EPC.Areas.Cooperation.Models
{
    public class LoggerFilter : FilterAttribute, IActionFilter
    {
        public string Table { get; set; }
        public EnumOperaType OperaType { get; set; }
        public string NameField { get; set; }
        public string IDField { get; set; }
        public string EngineeringInfoField { get; set; }
        public string Remark { get; set; }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var userInfo = FormulaHelper.GetUserInfo();
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);

            Dictionary<string, object> operaLog = new Dictionary<string, object>();
            operaLog.SetValue("ID", FormulaHelper.CreateGuid());

            string tableIDs = GetTableFieldValue(filterContext, IDField);
            string name = GetTableFieldValue(filterContext, NameField, tableIDs, Table);
            if (string.IsNullOrEmpty(EngineeringInfoField))
                EngineeringInfoField = "EngineeringInfoID";

            string engineeringInfoID = GetTableFieldValue(filterContext, EngineeringInfoField, tableIDs, Table);

            if (OperaType == EnumOperaType.None)
            {
                OperaType = GetAddorUpdateOpera(Table, tableIDs);
            }

            operaLog.SetValue("EngineeringInfoID", engineeringInfoID);
            operaLog.SetValue("OperateType", OperaType.ToString());
            operaLog.SetValue("TableIDs", tableIDs);
            operaLog.SetValue("Name", name);
            operaLog.SetValue("DBTable", Table);
            operaLog.SetValue("DBTableName", Table);
            operaLog.SetValue("Remark", Remark);

            operaLog.SetValue("CreateUser", userInfo.UserName);
            operaLog.SetValue("CreateUserID", userInfo.UserID);
            operaLog.SetValue("CompanyID", userInfo.UserOrgID);

            operaLog.InsertDB(sqlHelper, "S_A_ParticipantOperaLog");

        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            return;//throw new NotImplementedException();
        }

        private string GetQueryString(ActionExecutedContext filterContext, string key)
        {
            var Request = filterContext.HttpContext.Request;
            var formDic = GetformDic(filterContext);
            string value = Request.QueryString[key];
            if (string.IsNullOrEmpty(value))
                value = Request.Form[key];
            if (string.IsNullOrEmpty(value))
            {
                if (formDic.ContainsKey(key))
                    value = formDic[key].ToString();
            }

            if (value != null)
                return value;
            else
                return "";
        }

        private Dictionary<string, object> GetformDic(ActionExecutedContext filterContext)
        {
            var Request = filterContext.HttpContext.Request;
            Dictionary<string, object> formDic = new Dictionary<string, object>();
            //if (formDic == null)
            {
                if (!string.IsNullOrEmpty(Request["FormData"]))
                    formDic = JsonHelper.ToObject<Dictionary<string, object>>(Request["FormData"]);
                else
                    formDic = new Dictionary<string, object>();
            }
            return formDic;
        }

        private EnumOperaType GetAddorUpdateOpera(string table, string ids)
        {
            string sql = string.Format("select * from {0} where '{1}' like '%' + id + '%'", table, ids);
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            var resObj = sqlHelper.ExecuteScalar(sql);
            return resObj == null ? EnumOperaType.Add : EnumOperaType.Update;
        }

        private string GetEngineeringInfoID(string table, string ids)
        {
            string engineeringInfoID = "";
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            string colSearch = "select COLUMN_NAME from information_schema.COLUMNS where table_name = '" + table + "'";
            var colDT = sqlHelper.ExecuteDataTable(colSearch);
            var dic = FormulaHelper.DataTableToListDic(colDT);
            if (dic.Any(a => a.GetValue("COLUMN_NAME").ToLower() == "engineeringInfoid"))
            {
                string sql = string.Format("select top 1 {0} from {2} where '{1}' like '%' + id + '%'", "engineeringInfoid", ids, table);
                var resObj = sqlHelper.ExecuteScalar(sql);
                if (resObj != null) engineeringInfoID = resObj.ToString();
            }
            else if (dic.Any(a => a.GetValue("COLUMN_NAME").ToLower() == "engineeringInfo"))
            {
                string sql = string.Format("select top 1 {0} from {2} where '{1}' like '%' + id + '%'", "engineeringInfo", ids, table);
                var resObj = sqlHelper.ExecuteScalar(sql);
                if (resObj != null) engineeringInfoID = resObj.ToString();
            }

            return engineeringInfoID;
        }

        private string GetTableFieldValue(ActionExecutedContext filterContext, string field, string ids = "", string table = "")
        {
            var idFieldArr = field.Split('.');
            string result = "";

            string tmpField = field;
            //xxx.xxx 格式
            if (idFieldArr.Length == 2)
            {
                var queryKey = idFieldArr[0];
                var fieldKey = idFieldArr[1];
                tmpField = fieldKey;
                if (!string.IsNullOrEmpty(queryKey))
                {
                    var keyValue = GetQueryString(filterContext, queryKey);

                    //数组
                    if (keyValue.Contains("["))
                    {
                        var dicList = JsonHelper.ToList(keyValue);
                        result = string.Join(",", dicList.Select(a => a.GetValue(fieldKey)));
                    }
                    else
                    {
                        var dic = JsonHelper.ToObject(keyValue);
                        result = dic.GetValue(fieldKey);
                    }
                }
            }
            else
            {
                result = GetQueryString(filterContext, field);
            }

            //数据库查
            if (string.IsNullOrEmpty(result))
            {
                var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
                string sql = string.Format("select top 1 {0} from {2} where '{1}' like '%' + id + '%'", tmpField, ids, table);
                var resObj = sqlHelper.ExecuteScalar(sql);
                if (resObj != null) result = resObj.ToString();
            }

            return result;
        }
    }
    public class LoggerHelper
    {
        public static void InserLogger(string Table, EnumOperaType operaType, string ids, string engineeringInfoID, string name, string remark = "")
        {
            var userInfo = FormulaHelper.GetUserInfo();
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);

            Dictionary<string, object> operaLog = new Dictionary<string, object>();
            operaLog.SetValue("ID", FormulaHelper.CreateGuid());
            operaLog.SetValue("OperateType", operaType.ToString());
            operaLog.SetValue("TableIDs", ids);
            operaLog.SetValue("Name", name);
            operaLog.SetValue("EngineeringInfoID", engineeringInfoID);
            operaLog.SetValue("DBTable", Table);
            operaLog.SetValue("DBTableName", Table);
            operaLog.SetValue("Remark", remark);

            operaLog.SetValue("CreateUser", userInfo.UserName);
            operaLog.SetValue("CreateUserID", userInfo.UserID);
            operaLog.SetValue("CompanyID", userInfo.UserOrgID);//TODO

            operaLog.InsertDB(sqlHelper, "S_A_ParticipantOperaLog");
        }
    }

}