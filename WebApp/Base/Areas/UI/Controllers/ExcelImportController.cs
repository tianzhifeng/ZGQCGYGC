using Base.Logic;
using Base.Logic.BusinessFacade;
using Base.Logic.Domain;
using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Base.Areas.UI.Controllers
{
    public class ExcelImportController : BaseController<S_UI_ExcelImport>
    {
        public override ActionResult List()
        {
            return View();
        }

        public override ActionResult Edit()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        public ActionResult SettingsConvert()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        [ValidateInput(false)]
        public override JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_ExcelImport>();

            entity.ModifyTime = DateTime.Now;
            var user = FormulaHelper.GetUserInfo();
            entity.ModifyUserID = user.UserID;
            entity.ModifyUserName = user.UserName;
            entity.Condition = Request["Condition"];

            return JsonSave<S_UI_ExcelImport>(entity);
        }

        public JsonResult GetSelectorList(bool isSystem = true)
        {
            var list = entities.Set<S_UI_Selector>().Select(c => new { value = c.Code, text = c.Name }).ToList();
            list.Insert(1, new { value = "SystemUser", text = "系统用户" });
            list.Insert(2, new { value = "SystemOrg", text = "组织部门" });
            list.Insert(3, new { value = "Employee", text = "人员" });
            if (isSystem)
                list = list.Where(c => c.value == "SystemOrg" || c.value == "SystemUser" || c.value == "Employee").ToList();
            else
                list = list.Where(c => c.value != "SystemOrg" && c.value != "SystemUser" && c.value != "Employee").ToList();
            return Json(list);
        }

        private DataTable GetTableField(string connName, string tableName)
        {
            var sqlHelper = SQLHelper.CreateSqlHelper(connName);
            string sql = "select a.name as value from syscolumns a inner join sysobjects d on a.id=d.id and d.name='{0}'";
            return sqlHelper.ExecuteDataTable(string.Format(sql, tableName));
        }

        private string GetHostUrl()
        {
            var schema = Request.Url.Scheme;
            var host = Request.Url.Authority;
            return schema + "://" + host;
        }

        public JsonResult GetSelectorField(string selectorKey)
        {
            DataTable table = new DataTable();

            switch (selectorKey)
            {
                case "SystemOrg":
                    table = GetTableField("Base", "S_A_Org");
                    break;
                case "SystemUser":
                    table = GetTableField("Base", "S_A_User");
                    break;
                case "Employee":
                case "EmployeeSelect":
                    table = GetTableField("HR", "T_Employee");
                    break;
                default:
                    var entities = FormulaHelper.GetEntities<BaseEntities>();
                    var selector = entities.S_UI_Selector.SingleOrDefault(c => c.Code == selectorKey);
                    Regex regex = new Regex(@"TmplCode=([^\\&]*)");
                    var url = string.IsNullOrEmpty(selector.URLSingle) ? selector.URLMulti : selector.URLMulti;
                    var tmplCode = regex.Match(url).Value.Replace("TmplCode=", "");
                    var list = entities.S_UI_List.FirstOrDefault(c => c.Code == tmplCode);
                    if (list != null)
                    {
                        var fileds = JsonHelper.ToList(list.LayoutField);
                        table.Columns.Add("value");
                        foreach (var filed in fileds)
                        {
                            if (filed.ContainsKey("field"))
                            {
                                table.Rows.Add(filed["field"]);
                            }
                        }
                    }
                    break;
            }
            return Json(table);
        }

        public JsonResult GetCustomField()
        {
            UIFO fo = new UIFO();
            DataTable table = new DataTable();
            table.Columns.Add("value");
            string sql = fo.ReplaceString(Request["SQL"]);
            string connName = Convert.ToString(Request["ConnName"]);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
            var tb = sqlHelper.ExecuteDataTable(sql);
            if (tb.Columns.Contains("Column1"))
                throw new Exception("请为查询语名的列命名!如:select '{CurrentUserID}' as CurrentUserID");
 
            foreach (var col in tb.Columns)
            {
                table.Rows.Add(col.ToString());
            }
            return Json(table);
        }

        public JsonResult GetExcelImportList(QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CategoryID"]))
            {
                string categoryID = Request["CategoryID"];
                var arr = entities.Set<S_M_Category>().Where(c => c.ID == categoryID || c.ParentID == categoryID).Select(c => c.ID).ToArray();
                string ids = string.Join(",", arr);
                qb.Add("CategoryID", QueryMethod.In, ids);
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"select * from S_UI_ExcelImport";

            if (!string.IsNullOrEmpty(Request["CategoryID"]))
                sql += string.Format(" where CategoryID='{0}'", Request["CategoryID"]);


            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);
            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        #region 从列表定义中导入

        private string GetListType(string itemType)
        {
            string retType = ImportItemType.Text.ToString();
            var enumType = (ListItemType)Enum.Parse(typeof(ListItemType), itemType);
            switch (enumType)
            {
                case ListItemType.DatePicker:
                    retType = ImportItemType.Date.ToString();
                    break;
                case ListItemType.TextBox:
                    retType = ImportItemType.Text.ToString();
                    break;
                default:
                    retType = ImportItemType.Enum.ToString();
                    break;
            }
            return retType;
        }

        private ExcelImportDetailDTO GetListSettings(Dictionary<string, object> field, Dictionary<string, object> settings, string itemType)
        {
            var querySettingsStr = field.GetValue("QuerySettings");
            var querySettings = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(querySettingsStr))
                querySettings = JsonHelper.ToObject(querySettingsStr);
            var enumType = (ListItemType)Enum.Parse(typeof(ListItemType), itemType);
            ExcelImportDetailDTO dto = new ExcelImportDetailDTO();
            switch (enumType)
            {
                case ListItemType.DatePicker:
                    dto.Format = settings.GetValue("dateFormat");
                    break;
                case ListItemType.CheckBoxList:
                case ListItemType.RadioButtonList:
                case ListItemType.ComboBox:
                case ListItemType.CheckBox:
                    dto.EnumKey = settings.GetValue("EnumKey");
                    dto.IsMultiple = querySettings.GetValue("multiSelect");
                    break;
                case ListItemType.ButtonEdit:
                    dto.SelectorKey = querySettings.GetValue("SelectorKey");
                    break;
            }
            return dto;
        }

        public JsonResult AddListTemplet()
        {
            string listID = Request["ID"];
            var list = entities.Set<S_UI_List>().FirstOrDefault(c => c.ID == listID);
            if (entities.Set<S_UI_ExcelImport>().Where(c => c.Code == list.Code).Count() > 0)
                throw new Exception(string.Format("编号为{0}已存在!", list.Code));
            var templet = entities.Set<S_UI_ExcelImport>().Create();
            var userInfo = FormulaHelper.GetUserInfo();
            templet.ID = FormulaHelper.CreateGuid();
            templet.Code = list.Code;
            templet.Name = list.Name;
            templet.ConnName = list.ConnName;
            templet.TableNames = list.TableNames;
            templet.CategoryID = list.CategoryID;
            templet.CreateUserID = userInfo.UserID;
            templet.CreateUserName = userInfo.UserName;
            templet.CreateTime = DateTime.Now;
            templet.ModifyUserID = userInfo.UserID;
            templet.ModifyUserName = userInfo.UserName;
            templet.ModifyTime = DateTime.Now;
            var fields = JsonHelper.ToList(list.LayoutField);
            List<ExcelImportDTO> dto = new List<ExcelImportDTO>();
            foreach (var field in fields)
            {
                var itemType =field.GetValue("ItemType");
                if (string.IsNullOrEmpty(itemType)) itemType = "TextBox";
                var settingStr = field.GetValue("Settings");
                var setting = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(settingStr))
                    setting = JsonHelper.ToObject(settingStr);
                dto.Add(new ExcelImportDTO
                {
                    Code = field.GetValue("field"),
                    Name = field.GetValue("header"),
                    Width = field.GetValue("width"),
                    Align = field.GetValue("align"),
                    ItemType = GetListType(itemType),
                    Settings = GetListSettings(field, setting, itemType)
                });
            }
            templet.LayoutField = JsonHelper.ToJson(dto);
            entities.Set<S_UI_ExcelImport>().Add(templet);
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 从表单定义导入

        private string GetFormType(string itemType)
        {
            string retType = ImportItemType.Text.ToString();
            var enumType = (FormItemType)Enum.Parse(typeof(FormItemType), itemType);
            switch (enumType)
            {
                case FormItemType.DatePicker:
                    retType = ImportItemType.Date.ToString();
                    break;
                case FormItemType.TextBox:
                case FormItemType.TextArea:
                case FormItemType.UEditor:
                case FormItemType.SingleFile:
                case FormItemType.MultiFile:
                case FormItemType.AuditSign:
                case FormItemType.Spinner:
                case FormItemType.LinkEdit:
                    retType = ImportItemType.Text.ToString();
                    break;
                case FormItemType.ButtonEdit:
                    retType = ImportItemType.Convert.ToString();
                    break;
                case FormItemType.CheckBoxList:
                case FormItemType.RadioButtonList:
                case FormItemType.ComboBox:
                case FormItemType.CheckBox:
                    retType = ImportItemType.Enum.ToString();
                    break;
            }
            return retType;
        }

        private ExcelImportDetailDTO GetFormSettings(Dictionary<string, object> field, Dictionary<string, object> settings, string itemType)
        {
            var enumType = (FormItemType)Enum.Parse(typeof(FormItemType), itemType);
            ExcelImportDetailDTO dto = new ExcelImportDetailDTO();
            switch (enumType)
            {
                case FormItemType.DatePicker:
                    dto.Format = settings.GetValue("format");
                    break;
                case FormItemType.CheckBoxList:
                case FormItemType.RadioButtonList:
                case FormItemType.ComboBox:
                case FormItemType.CheckBox:
                    dto.EnumKey = settings.GetValue("data");
                    dto.IsMultiple = settings.GetValue("multiSelect");
                    break;
                case FormItemType.ButtonEdit:
                    dto.SelectorKey = settings.GetValue("SelectorKey");
                    dto.IsMultiple = settings.GetValue("AllowMultiSelect");
                    break;
                case FormItemType.TextBox:
                case FormItemType.TextArea:
                case FormItemType.UEditor:
                case FormItemType.SingleFile:
                case FormItemType.MultiFile:
                case FormItemType.AuditSign:
                case FormItemType.Spinner:
                case FormItemType.LinkEdit:
                    break;
            }
            return dto;
        }


        public JsonResult AddFormTemplet()
        {
            string listID = Request["ID"];
            var form = entities.Set<S_UI_Form>().FirstOrDefault(c => c.ID == listID);
            if (entities.Set<S_UI_ExcelImport>().Where(c => c.Code == form.Code).Count() > 0)
                throw new Exception(string.Format("编号为{0}已存在!", form.Code));
            var templet = entities.Set<S_UI_ExcelImport>().Create();
            var userInfo = FormulaHelper.GetUserInfo();
            templet.ID = FormulaHelper.CreateGuid();
            templet.Code = form.Code;
            templet.Name = form.Name;
            templet.ConnName = form.ConnName;
            templet.TableNames = form.TableName;
            templet.CategoryID = form.CategoryID;
            templet.CreateUserID = userInfo.UserID;
            templet.CreateUserName = userInfo.UserName;
            templet.CreateTime = DateTime.Now;
            templet.ModifyUserID = userInfo.UserID;
            templet.ModifyUserName = userInfo.UserName;
            templet.ModifyTime = DateTime.Now;
            var fields = JsonHelper.ToList(form.Items);
            List<ExcelImportDTO> dto = new List<ExcelImportDTO>();
            foreach (var field in fields)
            {
                var itemType = field.GetValue("itemType");
                if (string.IsNullOrEmpty(itemType)) itemType = "TextBox";
                if (itemType != FormItemType.SubTable.ToString())
                {
                    var settingStr = field.GetValue("Settings");
                    var setting = new Dictionary<string,object>();
                    if (!string.IsNullOrEmpty(settingStr))
                        setting = JsonHelper.ToObject(settingStr);
                    var required = setting.GetValue("required");
                    if (string.IsNullOrEmpty(required)) required = false.ToString();
                    dto.Add(new ExcelImportDTO
                    {
                        Code = field.GetValue("Code"),
                        Name = field.GetValue("Name"),
                        ItemType = GetFormType(itemType),
                        Required = required,
                        Settings = GetFormSettings(field,setting, itemType)
                    });
                }
            }
            templet.LayoutField = JsonHelper.ToJson(dto);
            entities.Set<S_UI_ExcelImport>().Add(templet);
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        public JsonResult GetItemList(string id)
        {
            return Json(entities.Set<S_UI_ExcelImport>().SingleOrDefault(c => c.ID == id).LayoutField);
        }


        public JsonResult SaveItemList(string id, string itemList)
        {
            var excel = entities.Set<S_UI_ExcelImport>().SingleOrDefault(c => c.ID == id);
            excel.LayoutField = itemList;
            var user = FormulaHelper.GetUserInfo();
            var fields = JsonHelper.ToList(itemList);
            List<string> tableNames = excel.TableNames.Split(',').ToList();
            foreach (var field in fields)
            {
                var columnName = field.GetValue("Code");
                if (columnName.IndexOf('.') >= 0)
                {
                    var tableName = columnName.Split('.')[0];
                    if (tableNames.Where(c => c.Equals(tableName)).Count() == 0)
                    {
                        tableNames.Add(tableName);
                    }
                }
            }
            excel.TableNames = string.Join(",", tableNames.ToArray());
            excel.ModifyUserID = user.UserID;
            excel.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }
    }

    public class ExcelImportDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ItemType { get; set; }
        public string Width { get; set; }
        public string Align { get; set; }
        public string Required { get; set; }
        public object Settings { get; set; }
    }

    public class ExcelImportDetailDTO
    {
        public string Type { get; set; }
        public string SelectorKey { get; set; }
        public string FiledListBox { get; set; }
        public string SelectListBox { get; set; }
        public string Filter { get; set; }
        public string Format { get; set; }
        public string IsMultiple { get; set; }
        public string EnumKey { get; set; }
    }
}
