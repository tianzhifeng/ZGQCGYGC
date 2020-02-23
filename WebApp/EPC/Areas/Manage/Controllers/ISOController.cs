using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;

namespace EPC.Areas.Manage.Controllers
{
    public class ISOController : EPCController
    {
        public JsonResult Delete(string ListIDs, string FolderID)
        {
            var configEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var define = configEntities.Set<S_T_ISODefine>().Find(FolderID);
            if (define == null) { throw new Formula.Exceptions.BusinessValidationException("没有找ISO表单定义对象，无法删除"); }
            string sql = "select * from {0} where ID='{1}'";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, define.TableName, ListIDs));
            if (dt.Rows.Count > 0)
            {
                if (dt.Columns.Contains("FlowPhase") && dt.Rows[0]["FlowPhase"].ToString() != "Start")
                {
                    throw new Formula.Exceptions.BusinessValidationException("已经启动流程审批的记录不能删除");
                }
                this.SqlHelper.ExecuteNonQuery(String.Format("delete from {0} where ID='{1}'", define.TableName, ListIDs));
            }
            return Json("");
        }

        public JsonResult GetTreeList(string EngineeringInfoID, string ViewType,string FilterInfo)
        {
            var configEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息");
            }
            var defineList = configEntities.S_T_ISODefine.OrderBy(c => c.SortIndex).ToList();
            var catagories = defineList.Select(d => d.Catagory).Distinct().ToList();
            if (!String.IsNullOrEmpty(ViewType) && ViewType.ToLower() == "phase")
                catagories = defineList.Select(d => d.PhaseInfo).Distinct().ToList();
            if (!String.IsNullOrEmpty(FilterInfo))
            {
                catagories = catagories.Where(c => c == FilterInfo).ToList();
            }
            var result = new List<Dictionary<string, object>>();
            foreach (var catagory in catagories)
            {
                var item = new Dictionary<string, object>();
                item.SetValue("ID", catagory);
                item.SetValue("Name", catagory);
                item.SetValue("Type", "Group");
                result.Add(item);
                var formDefines = defineList.Where(d => d.Catagory == catagory).ToList();
                if (!String.IsNullOrEmpty(ViewType) && ViewType.ToLower() == "phase")
                {
                    formDefines = defineList.Where(d => d.PhaseInfo == catagory).ToList();
                }
                foreach (var formDefine in formDefines)
                {
                    var formDefDic = new Dictionary<string, object>();
                    formDefDic.SetValue("ID", formDefine.ID);
                    formDefDic.SetValue("ParentID", catagory);
                    formDefDic.SetValue("DefineID", formDefine.ID);
                    formDefDic.SetValue("Name", formDefine.Name);
                    formDefDic.SetValue("Type", "FormDefine");
                    formDefDic.SetValue("CanAddNewForm", formDefine.CanAddNewForm);
                    formDefDic.SetValue("AddUrl", formDefine.LinkFormUrl);
                    result.Add(formDefDic);
                    _createItem(formDefine, formDefDic, result, EngineeringInfoID);
                }
            }
            return Json(result);
        }

        void _createItem(S_T_ISODefine isoDefine, Dictionary<string, object> parent, List<Dictionary<string, object>> result, string EngineeringInfoID)
        {
            var db = SQLHelper.CreateSqlHelper(isoDefine.ConnName);
            if (String.IsNullOrWhiteSpace(isoDefine.TableName))
            {
                return;
            }
            var sql = "select * from " + isoDefine.TableName + " where EngineeringInfoID='" + EngineeringInfoID + "'";
            var dt = db.ExecuteDataTable(sql);
            if (String.IsNullOrEmpty(isoDefine.NameFieldInfo) && !dt.Columns.Contains("Name"))
                throw new Formula.Exceptions.BusinessValidationException("【" + isoDefine.Name + "】定义配置中，数据源SQL必须定义NAME列");
            parent.SetValue("FileCount", dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                var formItem = FormulaHelper.DataRowToDic(row);
                formItem.SetValue("ParentID", isoDefine.ID);
                formItem.SetValue("ID", row["ID"].ToString());
                formItem.SetValue("Type", "ISOForm");
                formItem.SetValue("FormTmpCode", isoDefine.FormCode);
                formItem.SetValue("CanDelete", isoDefine.CanAddNewForm);
                var enumDefList = new List<Dictionary<string, object>>();
                if (!String.IsNullOrEmpty(isoDefine.EnumFieldInfo))
                    enumDefList = JsonHelper.ToList(isoDefine.EnumFieldInfo);
                string name = StringFunction.ReplaceRegString(isoDefine.NameFieldInfo, FormulaHelper.DataRowToDic(row), enumDefList);
                formItem.SetValue("Name", name);

                if (!String.IsNullOrWhiteSpace(isoDefine.LinkFormUrl))
                {
                    var url = "";
                    if (isoDefine.LinkFormUrl.IndexOf("?") >= 0)
                    {
                        url = isoDefine.LinkFormUrl + "&ID=" + row["ID"].ToString() + "";
                    }
                    else
                    {
                        url = isoDefine.LinkFormUrl + "?ID=" + row["ID"].ToString() + "";
                    }
                    url += "&FuncType=View";
                    var urlParams = url.Split('?')[1].Split('&');
                    foreach (var paramsString in urlParams)
                    {
                        if (paramsString.Split('=')[0].ToLowerInvariant() == "flowcode")
                        {
                            url = url.Replace(paramsString, "");
                        }
                    }

                    formItem.SetValue("LinkUrl", url);
                }
                result.Add(formItem);
            }
        }
    }
}
