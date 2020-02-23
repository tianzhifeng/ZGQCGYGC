using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using Config.Logic;
using Formula.Helper;
using System.Text;
using System.Data;
using Formula.ImportExport;
using MvcAdapter.ImportExport;
using Newtonsoft.Json;
using System.IO;
using Aspose.Cells;
using Formula.Exceptions;
using Formula;
using HR.Logic.Domain;
using Base.Logic.Domain;
using System.Text.RegularExpressions;
using Base.Logic.Model.UI.Form;

namespace HR.Areas.Search.Controllers
{
    public class EmployeeController : BaseReportController
    {
        protected override string title
        {
            get { return "雇员报表"; }
        }

        private SQLHelper _sqlHelper = null;
        protected override SQLHelper sqlHelper
        {
            get
            {
                if (_sqlHelper == null)
                {
                    _sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.HR);
                }
                return _sqlHelper;
            }
        }

        protected override string mainTable
        {
            get { return "T_Employee"; }
        }

        protected override string fk
        {
            get { return "EmployeeID"; }
        }

        public override ActionResult ReportList()
        {
            var hrEntity = FormulaHelper.GetEntities<HREntities>();
            var baseEntity = FormulaHelper.GetEntities<BaseEntities>();
            List<ReportTreeItem> treeData = new List<ReportTreeItem>();
            StringBuilder enumSb = new StringBuilder("\n");

            var setList = hrEntity.Set<T_EmployeeBaseSet>().Where(a => a.EmploymentWay == "正式员工").OrderBy(a => a.SortIndex).ToList();
            int num = 0;
            foreach (var item in setList)
            {
                var url = item.Url;
                var tmpCode = string.Empty;
                var startIndex = url.IndexOf("TmplCode=");
                url = url.Substring(startIndex).Replace("TmplCode=", "");
                var length = url.IndexOf("&");
                if (length <= 0)
                    continue;
                tmpCode = url.Substring(0, length);
                if (!treeData.Any(a => a.tmpCode == tmpCode))
                    treeData.Add(new ReportTreeItem { tmpCode = tmpCode, fieldName = item.Title, id = num.ToString() });
                num++;
            }
            var tmpCodes = treeData.Select(a => a.tmpCode).ToList();
            var formList = baseEntity.Set<S_UI_Form>().Where(a => tmpCodes.Contains(a.Code)).ToList();
            List<string> enumKeyList = new List<string>();
            foreach (var item in treeData)
            {
                var formDef = formList.FirstOrDefault(a => a.Code == item.tmpCode);
                var table = formDef.TableName;
                item.children = new List<ReportTreeChildItem>();
                var fields = JsonHelper.ToObject<List<FormItem>>(formDef.Items);
                foreach (var field in fields)
                {
                    var fieldType = field.FieldType;
                    var itemType = field.ItemType;
                    if (fieldType.Equals("image"))
                        continue;
                    if (itemType.Equals("SingleFile") || itemType.Equals("MultiFile") || itemType.Equals("AuditSign")
                        || itemType.Equals("CheckBox") || itemType.Equals("LinkEdit") || itemType.Equals("SubTable"))
                        continue;
                    var child = new ReportTreeChildItem();
                    item.children.Add(child);
                    child.pid = item.id;
                    child.table = table;
                    child.fieldCode = field.Code;
                    child.fieldName = field.Name;
                    if (fieldType.Equals("datetime"))
                    {
                        child.ctrlType = "date";
                        child.queryType = ">";
                    }
                    else if (itemType == "ButtonEdit" || itemType == "ComboBox" || itemType == "CheckBoxList" || itemType == "RadioButtonList")
                    {
                        child.ctrlType = "combobox";
                        child.queryType = "in";
                        if (itemType == "ButtonEdit")
                            child.fieldCode += "Name";
                        else
                        {
                            if (string.IsNullOrEmpty(field.Settings))
                                continue;
                            var settings = JsonHelper.ToObject<Dictionary<string, string>>(field.Settings);
                            var dataStr = settings.GetValue("data");
                            if (string.IsNullOrEmpty(dataStr))
                                continue;
                            child.enumKey = GetEnumKey("", table, child.fieldCode, dataStr);
                            if (!enumKeyList.Contains(child.enumKey))
                            {
                                enumKeyList.Add(child.enumKey);
                                var enumStr = GetEnumString(formDef.ConnName, table, child.fieldCode, dataStr);
                                if (!string.IsNullOrEmpty(enumStr))
                                    enumSb.AppendFormat("\n var {0} = {1}; ", child.enumKey, enumStr);
                            }
                        }
                    }
                    else
                    {
                        child.ctrlType = "textbox";
                        child.queryType = "like";
                    }
                }
            }
            ViewBag.TreeData = JsonHelper.ToJson(treeData);
            ViewBag.EnumDataStr = enumSb.ToString();
            return View();
        }

        private string GetEnumKey(string connName, string tableName, string fieldCode, string data)
        {
            bool fromMeta = false;
            if (data.StartsWith("[") == false)
            {
                var arr = data.Split(',');
                if (arr.Length == 3) //如果data为ConnName,tableName,fieldCode时
                {
                    connName = arr[0];
                    tableName = arr[1];
                    fieldCode = arr[2];
                    fromMeta = true;
                }
                else if (arr.Length == 2)//如果data为tableName,fieldCode时
                {
                    tableName = arr[0];
                    fieldCode = arr[1];
                    fromMeta = true;
                }
            }

            string result = "";
            if (data.StartsWith("["))
                result = string.Format("enum_{0}_{1}", tableName, fieldCode);
            else if (data == "" || data == "FromMeta" || fromMeta == true)
            {
                result = string.Format("enum_{0}_{1}", tableName, fieldCode);
            }
            else
            {
                result = data.Split('.').Last();
            }

            return result;
        }

        private string GetEnumString(string connName, string tableName, string fieldCode, string data)
        {
            bool fromMeta = false;
            if (data.StartsWith("[") == false)
            {
                var arr = data.Split(',');
                if (arr.Length == 3) //如果data为ConnName,tableName,fieldCode时
                {
                    connName = arr[0];
                    tableName = arr[1];
                    fieldCode = arr[2];
                    fromMeta = true;
                }
                else if (arr.Length == 2)//如果data为tableName,fieldCode时
                {
                    tableName = arr[0];
                    fieldCode = arr[1];
                    fromMeta = true;
                }
            }

            string result = "";
            if (data.StartsWith("["))
                result = data;
            else if (data == "" || data == "FromMeta" || fromMeta == true)
            {
                var entities = FormulaHelper.GetEntities<BaseEntities>();
                var field = entities.Set<S_M_Field>().FirstOrDefault(c => c.Code == fieldCode && c.S_M_Table.Code == tableName && c.S_M_Table.ConnName == connName);
                if (field == null || string.IsNullOrEmpty(field.EnumKey))
                    result = string.Format("var enum_{0}_{1} = {2};", tableName, fieldCode, "[]");
                else if (field.EnumKey.Trim().StartsWith("["))
                    result = field.EnumKey;
                else
                {
                    IEnumService enumService = FormulaHelper.GetService<IEnumService>();
                    try
                    {
                        result = JsonHelper.ToJson(enumService.GetEnumTable(field.EnumKey));
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                IEnumService enumService = FormulaHelper.GetService<IEnumService>();
                try
                {
                    result = JsonHelper.ToJson(enumService.GetEnumTable(data));
                }
                catch (Exception) { }
            }

            return result;
        }
    }
}

public class ReportTreeItem
{
    public string fieldName { get; set; }
    public string tmpCode { get; set; }
    public string id { get; set; }
    public List<ReportTreeChildItem> children { get; set; }
}
public class ReportTreeChildItem
{
    public string pid { get; set; }
    public string table { get; set; }
    public string fieldCode { get; set; }
    public string fieldName { get; set; }
    public string ctrlType { get; set; }
    public string queryType { get; set; }
    public string enumKey { get; set; }
}
