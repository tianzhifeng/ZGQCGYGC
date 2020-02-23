using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using MvcAdapter.ImportExport;
using System.Data;
using System.Web.Security;
using Aspose.Cells;
using System.IO;
using Newtonsoft.Json.Converters;
using Formula;
using Formula.ImportExport;
using Formula.Helper;
using System.Text.RegularExpressions;
using Base.Logic.Domain;
using Base.Logic;
using System.Text;
using Config;
using Config.Logic;
using Base.Logic.BusinessFacade;

namespace MvcConfig.Controllers
{
    public class AsposeController : Controller
    {
        // Post  ExportExcel 导出Excel
        [ValidateInput(false)]
        public ActionResult ExportExcel(string dataUrl, string queryFormData, string quickQueryFormData, string queryTabData, string sortField, string sortOrder, string jsonColumns, string excelKey, string title, string referUrl, int pageSize, int pageIndex, bool exportCurrentPage)
        {
            dataUrl = Server.UrlDecode(dataUrl);

            Formula.LogWriter.Info(string.Format("ExportExcel - Excel导出的Key：{0} - 开始", excelKey));

            #region 收集自动生成模板的列信息
            var columns = JsonConvert.DeserializeObject<List<ColumnInfo>>(jsonColumns);
            // 清空中文名称为空的列
            if (columns != null)
            {
                for (var i = columns.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrWhiteSpace(columns[i].ChineseName))
                    {
                        columns.RemoveAt(i);
                    }
                    if (!string.IsNullOrWhiteSpace(columns[i].DataType))
                    {
                        columns[i].DataType = "";
                    }
                }
                HttpContext.Items["__ColumnInfo"] = columns;
            }
            #endregion

            // 导出到Excel的数据源，
            DataTable dt = null;
            if (dataUrl.IndexOf("[]") >= 0 || dataUrl.IndexOf("{") >= 0) // 前台传入Data数据
            {
                //c_hua 2018-09-26注释，当数据中含null值，替换为"null"后，反序列化报错
                //dataUrl = Regex.Replace(dataUrl, @"(?<=\:).*?(?=\,)", new MatchEvaluator(CorrectString), RegexOptions.Compiled | RegexOptions.IgnoreCase);
                dt = JsonConvert.DeserializeObject<DataTable>(dataUrl);
            }
            else // 前台传入请求数据的地址
            {
                var dic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(queryFormData))
                    dic.Add("queryFormData", queryFormData);
                if (!string.IsNullOrWhiteSpace(quickQueryFormData))
                    dic.Add("quickQueryFormData", quickQueryFormData);
                if (!string.IsNullOrWhiteSpace(queryTabData))
                    dic.Add("queryTabData", queryTabData);
                if (!string.IsNullOrWhiteSpace(sortField))
                    dic.Add("sortField", sortField);
                if (!string.IsNullOrWhiteSpace(sortOrder))
                    dic.Add("sortOrder", sortOrder);
                if (exportCurrentPage)
                {
                    dic.Add("pageSize", pageSize);
                    dic.Add("pageIndex", pageIndex);
                }
                else
                {
                    dic.Add("pageSize", 0);
                }
                var serverUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Headers["Host"]);
                dt = Get<DataTable>(serverUrl, dataUrl, referUrl, dic) ?? new DataTable();
            }
            dt.TableName = excelKey;

            var exporter = new AsposeExcelExporter();
            byte[] templateBuffer = null;

            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            var templatePath = path.EndsWith("\\") ? string.Format("{0}{1}_New.xls", path, excelKey) : string.Format("{0}\\{1}_New.xls", path, excelKey);
            templatePath = Server.MapPath("/") + templatePath;
            if (System.IO.File.Exists(templatePath))
            {
                Formula.LogWriter.Info(string.Format("ExportExcel - 采用自定义模板，模板路径为：{0}", templatePath));
                templateBuffer = FileHelper.GetFileBuffer(templatePath);
            }
            else
            {
                templateBuffer = exporter.ParseTemplate(columns, excelKey, title);
            }

            var buffer = exporter.Export(dt, templateBuffer);

            Formula.LogWriter.Info(string.Format("ExportExcel - Excel导出的Key：{0} - 结束", excelKey));
            if (buffer != null)
            {
                return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
            }

            Formula.LogWriter.Info(string.Format("ExportExcel - 导出数据失败，参数: dataUrl={0}<br>queryFormData={1}<br>jsonColumns={2}<br>excelKey={3}<br>title={4}<br> ", dataUrl, queryFormData, jsonColumns, excelKey, title));
            return Content("导出数据失败，请检查相关配置！");
        }

        #region Excel导入

        public ActionResult ExportExcelTemplate(string tmplCode)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var listDef = entities.S_UI_ExcelImport.FirstOrDefault(c => c.Code == tmplCode);
            if (listDef == null)
                throw new Exception(string.Format("找不到TmplCode为：{0}的Excel导入模板,请检查是否已配置!", tmplCode));
            var fields = JsonHelper.ToList(listDef.LayoutField);
            List<ColumnInfo> columns = new List<ColumnInfo>();
            char[] splitStr = { ',', '，' };
            var PKTableName = listDef.TableNames.Split(splitStr)[0];

            //主键表与外键表ID不存在时自动添加
            foreach (var tableName in listDef.TableNames.Split(splitStr))
            {
                columns.Add(new ColumnInfo
                {
                    ChineseName = "ID",
                    FieldName = "ID",
                    Width = "0",
                    Align = "",
                    ExcelFormat = "text",
                    TableName = tableName
                });
            }

            foreach (var field in fields)
            {
                var columnName = field.GetValue("Code");
                if (!string.IsNullOrWhiteSpace(columnName))
                {
                    var spCol = columnName.Split('.');
                    if (columnName.ToUpper() != "ID" || (columnName.IndexOf('.') >= 0 && string.Format("{0}.ID", spCol[0]) != columnName))
                    {
                        var settingStr = field.GetValue("Settings");
                        var setting = new Dictionary<string, object>();
                        if (!string.IsNullOrEmpty(settingStr))
                            setting = JsonHelper.ToObject(settingStr);
                        columns.Add(new ColumnInfo
                        {
                            ChineseName = field.GetValue("Name"),
                            FieldName = columnName.IndexOf('.') >= 0 ? columnName.Split('.')[1] : columnName,
                            Width = !string.IsNullOrEmpty(field.GetValue("Width")) ? field.GetValue("Width") : field.GetValue("Hidden").ToUpper() == "TRUE" ? "0" : "",
                            Align =  !string.IsNullOrEmpty(field.GetValue("Align")) ? field.GetValue("Align") : "",
                            ExcelFormat =setting.GetValue("Format"),
                            TableName = columnName.IndexOf('.') >= 0 ? spCol[0] : PKTableName
                        });
                    }
                }
            }
            var exporter = new AsposeExcelExporter();
            byte[] templateBuffer = null;

            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            var templatePath = path.EndsWith("\\") ? string.Format("{0}{1}_Template.xls", path, tmplCode) : string.Format("{0}\\{1}_New.xls", path, tmplCode);
            templatePath = Server.MapPath("/") + templatePath;
            if (System.IO.File.Exists(templatePath))
            {
                Formula.LogWriter.Info(string.Format("ExportExcelTemplate - 采用自定义模板，模板路径为：{0}", templatePath));
                templateBuffer = FileHelper.GetFileBuffer(templatePath);
            }
            else
            {
                templateBuffer = exporter.ParseTemplate(columns, tmplCode, listDef.Name);
            }

            var buffer = exporter.Export(new DataTable(), templateBuffer);

            Formula.LogWriter.Info(string.Format("ExportExcelTemplate - Excel导出的Key：{0} - 结束", tmplCode));
            if (buffer != null)
            {
                return File(buffer, "application/vnd.ms-excel", Url.Encode(listDef.Name) + ".xls");
            }
            return Content("导出数据失败，请检查相关配置！");
        }

        private Dictionary<string, object> GetSettings(Dictionary<string, object> field)
        {
            if (field.Keys.Contains("Settings"))
            {
                return JsonHelper.ToObject(field.GetValue("Settings"));
            }
            return null;
        }
        
        #region 导入时缓存数据，默认10分钟
        private bool IsUseExcelCache()
        {
            var useExcelCache = System.Configuration.ConfigurationManager.AppSettings["UseExcelCache"];
            if (!string.IsNullOrEmpty(useExcelCache))
                return Convert.ToBoolean(useExcelCache);
            return true;
        }

        private void SetCache(CellValidationArgs cva, string tmplCode, object obj)
        {
            if (IsUseExcelCache())
            {
                //缓存关键字{导入模板编号}-{行号}-{字段名}
                string cacheKey = string.Format("{0}-{1}-{2}", tmplCode, cva.RowIndex.ToString(), cva.FieldName);
                int timeOut = 600; //默认10分钟
                var excelImportTimeOut = System.Configuration.ConfigurationManager.AppSettings["ExcelImportTimeOut"];
                if (!string.IsNullOrEmpty(excelImportTimeOut))
                    timeOut = Convert.ToInt32(excelImportTimeOut);
                CacheHelper.Set(cacheKey, obj, timeOut);
            }
        }

        private object GetCache(CellValidationArgs cva, string tmplCode)
        {
            if (IsUseExcelCache())
            {
                //缓存关键字{导入模板编号}-{行号}-{字段名}
                string cacheKey = string.Format("{0}-{1}-{2}", tmplCode, cva.RowIndex.ToString(), cva.FieldName);
                return HttpRuntime.Cache.Get(cacheKey);
            }
            return null;
        }
        #endregion

        private string FilterNull(string value, string type)
        {
            if (type == "middle")
                value = System.Text.RegularExpressions.Regex.Replace(value, @"\s", "");
            return value;
        }


        private bool IsConvertDBType(DataRow dr, int type)
        {
            var dbType = dr["Type"].ToString();
            //时间
            if (type == 1)
                return dbType.ToLower().IndexOf("time") >= 0 || dbType.ToLower().IndexOf("date") >= 0;
            else if (type == 2) //文本
                return dbType.ToLower().IndexOf("var") >= 0 || dbType.ToLower().IndexOf("xml") >= 0 || dbType.ToLower().IndexOf("text") >= 0 || dbType.ToLower().IndexOf("char") >= 0 || dbType.ToLower().IndexOf("binary") >= 0;
            else if (type == 3) //数据
                return dbType == "bit" || dbType.ToLower().IndexOf("decimal") >= 0 || dbType == "float" || dbType == "int" || dbType == "money" || dbType == "numeric" || dbType == "smallint";
            return false;
        }

        //保存当前导入的数据，进行转换
        private static List<ExcelTableInfo> copyTables = new List<ExcelTableInfo>();

        #region 类型为转换时获取值方法
        private Dictionary<string, object> GetConvertValues(List<Dictionary<string, object>> list, DataRow dr)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (var item in list)
            {
                string column = item.GetValue("value");
                string outputField = item.GetValue("outputField");
                values.Add(outputField, dr[column]);
            }
            return values;
        }
        private Dictionary<string, object> GetConvert(List<Dictionary<string, object>> fields, Dictionary<string, object> settings, Formula.ImportExport.CellValidationArgs cv, string PKTableName, string text, DataTable table = null)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            List<string> inputFields = new List<string>();
            var type = settings.GetValue("Type");
            if (table != null && !string.IsNullOrEmpty(text))
            {
                foreach (DataColumn dc in table.Columns)
                {
                    if (dc.DataType.ToString() == "System.String")
                        inputFields.Add(string.Format(" {0}='{1}'", dc, text));
                }
            }
            var selectListBox = settings.GetValue("SelectListBox");
            var list = JsonHelper.ToObject<List<Dictionary<string, object>>>(selectListBox);

            if (table != null && !string.IsNullOrEmpty(text))
            {
                var dataRow = table.Select(string.Join(" OR", inputFields.ToArray()));
                if (dataRow.Count() > 0)
                    values = GetConvertValues(list, dataRow[0]);
            }
            if (type == "custom" && table == null)
            {
                UIFO fo = new UIFO();
                var connName = settings.GetValue("ConnName");
                var sql = settings.GetValue("SQL");
                DataTable excelTable = new DataTable();
                DataRow excelRow = excelTable.NewRow();

                if (copyTables.Count > 0)
                {
                    foreach (var tb in copyTables)
                    {
                        foreach (var row in tb.Rows)
                        {
                            if (row.RowIndex == cv.RowIndex)
                            {
                                foreach (var cell in row.Cells)
                                {
                                    string fieldName = tb.TableName == PKTableName ? cell.FieldName : string.Format("{0}.{1}", tb.TableName, cell.FieldName);
                                    if (!excelTable.Columns.Contains(fieldName))
                                        excelTable.Columns.Add(fieldName);
                                    var _value  =cell.Value;
                                    var field = fields.FirstOrDefault(a => a.GetValue("Code") == fieldName);
                                    if (field != null)
                                    {
                                        var settingStr = field.GetValue("Settings");
                                        var _settings = new Dictionary<string, object>();
                                        if (!string.IsNullOrEmpty(settingStr))
                                            _settings = JsonHelper.ToObject(settingStr);
                                        _value = FilterNull(_value, _settings.GetValue("Filter"));
                                    }
                                    excelRow[fieldName] =_value;
                                }
                                break;
                            }
                        }
                    }
                }

                var dataTable = SQLHelper.CreateSqlHelper(connName).ExecuteDataTable(fo.ReplaceString(sql, excelRow));
                if (dataTable.Rows.Count > 0)
                    values = GetConvertValues(list, dataTable.Rows[0]);
            }

            return values;
        }
        #endregion

        #region 设置各表中的字段值
        private void SetTableCols(Formula.ImportExport.CellValidationArgs cv, string value)
        {
            string tableName = cv.TableName;
            int rowIndex = cv.RowIndex, colIndex = cv.ColIndex;
            if (copyTables.Count > 0)
            {
                var table = copyTables.FirstOrDefault(c => c.TableName == tableName);
                if (table != null)
                {
                    foreach (var row in table.Rows)
                    {
                        if (row.RowIndex == rowIndex)
                        {
                            if (row.Cells.Count > 0)
                            {
                                foreach (var cell in row.Cells)
                                {
                                    if (cell.Structure.ColIndex == colIndex)
                                    {
                                        cell.Value = value;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        private List<CellErrorInfo> ValidateData(ExcelData excelData, S_UI_ExcelImport import)
        {
            var fields = JsonHelper.ToList(import.LayoutField);
            List<CellErrorInfo> errorInfo = new List<CellErrorInfo>();
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(import.ConnName);
            copyTables = new List<ExcelTableInfo>();
            copyTables = excelData.Tables.ToList();

            #region 取表,取枚举与转换中不带上下文的内容
            Dictionary<string, bool> tableNames = new Dictionary<string, bool>();
            Dictionary<string, DataTable> enumAndConvertDics = new Dictionary<string, DataTable>();
            var enumServcie = FormulaHelper.GetService<IEnumService>();
            string pkTableName = import.TableNames.Split(',')[0];
            tableNames.Add(pkTableName, true);
            foreach (var field in fields)
            {
                var settingStr = field.GetValue("Settings");
                var setting = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(settingStr))
                    setting = JsonHelper.ToObject(settingStr);
                var column = field.GetValue("Code");
                if (column.IndexOf(".") > 0)
                {
                    string tableName = column.Split('.')[0];
                    var tb = tableNames.Where(c => c.Key.Contains(tableName));
                    if (tb.Count() == 0)
                    {
                        tableNames.Add(tableName, false);
                    }
                }
                var itemType = field.GetValue("ItemType");
                var enumType = (ImportItemType)Enum.Parse(typeof(ImportItemType), itemType);
                switch (enumType)
                {
                    case ImportItemType.Enum:
                        var enumKey = setting.GetValue("EnumKey");
                        if (enumKey.StartsWith("[") == false)
                        {
                            var enumDataTable = enumServcie.GetEnumTable(enumKey);
                            if (enumAndConvertDics.Where(c => c.Key.Contains(column)).Count() == 0)
                                enumAndConvertDics.Add(column, enumDataTable);
                        }
                        break;
                    case ImportItemType.Convert:
                        var type = setting.GetValue("Type");
                        if (type != "custom")
                        {
                            DataTable table = new DataTable();
                            var selectorKey = setting.GetValue("SelectorKey");
                            switch (selectorKey)
                            {
                                case "SystemUser":
                                    table = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable("select * from S_A_User");
                                    break;
                                case "SystemOrg":
                                    table = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable("select * from S_A_Org");
                                    break;
                                case "EmployeeSelect":
                                case "Employee":
                                    table = SQLHelper.CreateSqlHelper(ConnEnum.HR).ExecuteDataTable("select * from T_Employee");
                                    break;
                                default:
                                    UIFO fo = new UIFO();
                                    var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
                                    var selector = baseEntities.S_UI_Selector.SingleOrDefault(c => c.Code == selectorKey);
                                    Regex regex = new Regex(@"TmplCode=([^\\&]*)");
                                    var url = string.IsNullOrEmpty(selector.URLSingle) ? selector.URLMulti : selector.URLMulti;
                                    var tmplCode = regex.Match(url).Value.Replace("TmplCode=", "");
                                    var uiList = baseEntities.Set<S_UI_List>().FirstOrDefault(c => c.Code == tmplCode);
                                    table = SQLHelper.CreateSqlHelper(uiList.ConnName).ExecuteDataTable(fo.ReplaceString(uiList.SQL));
                                    break;
                            }
                            if (enumAndConvertDics.Where(c => c.Key.Contains(column)).Count() == 0)
                                enumAndConvertDics.Add(column, table);
                        }
                        else
                        {
                            var sql = setting.GetValue("SQL");
                            if (!string.IsNullOrEmpty(sql) && sql.IndexOf('{') < 0)
                            {
                                var connName = setting.GetValue("ConnName");
                                if (enumAndConvertDics.Where(c => c.Key.Contains(column)).Count() == 0)
                                    enumAndConvertDics.Add(column, SQLHelper.CreateSqlHelper(connName).ExecuteDataTable(sql));
                            }
                        }
                        break;
                }
            }
            #endregion

            //优先处理主键
            foreach (var table in copyTables)
            {
                foreach (var row in table.Rows)
                {
                    foreach (var cell in row.Cells)
                    {
                        string colName = string.Format("{0}.{1}", table.TableName, cell.FieldName);
                        if (cell.FieldName.ToUpper() == "ID" || cell.FieldName.ToUpper() == colName.ToUpper())
                        {
                            cell.Value = FormulaHelper.CreateGuid();
                        }
                    }
                }
            }

            #region 循环验证所有的表内容
            foreach (var tb in tableNames)
            {
                string tableName = tb.Key;
                var fieldTable = sqlHelper.ExecuteDataTable(string.Format(@"select syscolumns.name as Name ,systypes.name Type from syscolumns,systypes where syscolumns.xusertype=systypes.xusertype
                and syscolumns.id=object_id('{0}')", tableName));

                //获取当前表字段
                List<Dictionary<string, object>> tableFields = new List<Dictionary<string, object>>();
                foreach (var field in fields)
                {
                    var column = field.GetValue("Code");
                    if (!tb.Value)
                    {
                        if (column.IndexOf(tableName) >= 0)
                            tableFields.Add(field);
                    }
                    else
                    {
                        if (column.IndexOf(tableName) < 0 && column.IndexOf(".") < 0)
                            tableFields.Add(field);
                    }
                }

                var hiddenTableFields = tableFields.Where(c => c.GetValue("Hidden").ToUpper() == "TRUE").ToList();

                var errors = excelData.Vaildate(e =>
                {
                    if (e.TableName == tableName)
                    {
                        char[] splitStr = { ',', '，' };

                        foreach (var field in tableFields)
                        {
                            var settingStr = field.GetValue("Settings");
                            var settings = new Dictionary<string, object>();
                            if (!string.IsNullOrEmpty(settingStr))
                                settings = JsonHelper.ToObject(settingStr);
                            var column = field.GetValue("Code");
                            if (column.IndexOf(tableName) >= 0)
                                column = column.Replace(tableName, "").Replace(".", "");
                            DataRow fieldDataRow = fieldTable.AsEnumerable().FirstOrDefault(c => c["Name"].ToString() == column);
                            if (column == e.FieldName && fieldDataRow != null && column.ToUpper() != "ID")
                            {
                                var required = Convert.ToBoolean(!string.IsNullOrEmpty(field.GetValue("Required")) ? field.GetValue("Required") : "false");
                                var columnName = field.GetValue("Name");
                                if (required && string.IsNullOrEmpty(e.Value) && hiddenTableFields.Where(c => c.GetValue("Code") == column).Count() <= 0)
                                {
                                    e.IsValid = false;
                                    e.ErrorText = string.Format("列【{0}】不能为空!", columnName);
                                }
                                else
                                {
                                    var itemType = field.GetValue("ItemType");
                                    var enumType = (ImportItemType)Enum.Parse(typeof(ImportItemType), itemType);
                                    var filter = settings.GetValue("Filter");
                                    switch (enumType)
                                    {
                                        case ImportItemType.Date:
                                            #region Date
                                            DateTime outTimeValue;
                                            string dateFormat = settings.GetValue("Format");
                                            if (!DateTime.TryParse(FilterNull(e.Value, filter), out outTimeValue))
                                            {
                                                if (!string.IsNullOrEmpty(FilterNull(e.Value, filter)))
                                                {
                                                    e.IsValid = false;
                                                    e.ErrorText = string.Format("【{0}】无法转成日期数据", FilterNull(e.Value, filter));
                                                }
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(settings.GetValue("Format")))
                                                {
                                                    if (!string.IsNullOrEmpty(dateFormat))
                                                    {
                                                        try
                                                        {
                                                            outTimeValue.ToString(dateFormat);
                                                            if (!IsConvertDBType(fieldDataRow, 1))
                                                            {
                                                                e.IsValid = false;
                                                                e.ErrorText = string.Format("【{0}】无法转成日期类型", FilterNull(e.Value, filter));
                                                            }
                                                        }
                                                        catch (Exception)
                                                        {
                                                            e.IsValid = false;
                                                            e.ErrorText = string.Format("【{0}】无法转成日期格式【{1}】", FilterNull(e.Value, filter), dateFormat);
                                                        }
                                                    }
                                                }
                                            }
                                            if (e.IsValid)
                                            {
                                                var val = outTimeValue.ToString("yyyy-MM") == "0001-01" ? "" : outTimeValue.ToString("yyyy-MM-dd HH:mm:ss");
                                                SetTableCols(e, val);
                                            }
                                            #endregion
                                            break;
                                        case ImportItemType.Enum:
                                            #region Enum
                                            var enumKey = settings.GetValue("EnumKey");
                                            var isMultiple_Enum = settings.GetValue("IsMultiple");
                                            var excelEnumTexts = FilterNull(e.Value, filter).Split(splitStr);
                                            var enumList = new List<string>();
                                            if (!string.IsNullOrEmpty(isMultiple_Enum) && isMultiple_Enum.ToUpper() != "TRUE" && excelEnumTexts.Length > 1)
                                            {
                                                e.IsValid = false;
                                                e.ErrorText = string.Format("单选枚举中存在多个枚举的情况【{0}】", FilterNull(e.Value, filter));
                                            }
                                            else
                                            {
                                                foreach (var text in excelEnumTexts)
                                                {
                                                    var enumValue = "";
                                                    string filterValue = FilterNull(text, filter);
                                                    if (enumKey.StartsWith("[") == false)
                                                    {
                                                        var enumTable = enumAndConvertDics.FirstOrDefault(c => c.Key.Contains(field.GetValue("Code")));
                                                        if (enumTable.Value.Rows.Count > 0)
                                                        {
                                                            var enumDataRow = enumTable.Value.Select(string.Format(" text='{0}'", filterValue));
                                                            if (enumDataRow.Count() > 0)
                                                                enumValue = Convert.ToString(enumDataRow[0]["value"]);
                                                        }
                                                        else
                                                            enumValue = enumServcie.GetEnumValue(enumKey, filterValue);
                                                    }
                                                    else
                                                    {
                                                        var enumData = JsonHelper.ToList(enumKey);
                                                        if (enumData.Count > 0)
                                                        {
                                                            var enumText = enumData.FirstOrDefault(c => c.GetValue("text")== filterValue);
                                                            if (enumText != null)
                                                                enumValue = enumText.GetValue("value");
                                                        }
                                                    }
                                                    if (string.IsNullOrEmpty(enumValue))
                                                    {
                                                        if (!string.IsNullOrEmpty(FilterNull(text, filter)))
                                                        {
                                                            e.IsValid = false;
                                                            e.ErrorText = string.Format("【{0}】不是枚举中的值,请重新核对", FilterNull(text, filter));
                                                        }
                                                        break;
                                                    }
                                                    else
                                                        enumList.Add(enumValue);
                                                }
                                            }
                                            if (e.IsValid && enumList.Count > 0)
                                            {
                                                SetTableCols(e, string.Join(",", enumList.ToArray()));
                                            }
                                            #endregion
                                            break;
                                        case ImportItemType.Convert:
                                            #region Convert
                                            var isMultiple_Convert = settings.GetValue("IsMultiple");
                                            var convertText = FilterNull(e.Value, filter);
                                            var excelSelectorTexts = convertText.Split(splitStr);
                                            var convertList = new List<object>();
                                            var outList = new Dictionary<string, string>();
                                            if (!string.IsNullOrEmpty(isMultiple_Convert) && isMultiple_Convert.ToUpper() != "TRUE" && excelSelectorTexts.Length > 1)
                                            {
                                                e.IsValid = false;
                                                e.ErrorText = string.Format("单选选择器中存在多个数据的情况【{0}】", FilterNull(e.Value, filter));
                                            }
                                            else
                                            {
                                                var code = field.GetValue("Code");
                                                var type = settings.GetValue("Type");
                                                var columns = column.Split(splitStr);
                                                var enumTable = enumAndConvertDics.FirstOrDefault(c => c.Key.Contains(code));
                                                foreach (var text in excelSelectorTexts)
                                                {
                                                    var excelText = FilterNull(text, filter);
                                                    var dic = GetConvert(tableFields, settings, e, pkTableName, excelText, enumTable.Value != null && enumTable.Value.Rows.Count > 0 ? enumTable.Value : null);
                                                    if (dic.Count <= 0 && required)
                                                    {
                                                        e.IsValid = false;
                                                        e.ErrorText = string.Format("【{0}】不是属于选择器中,请重新核对", excelText);
                                                        break;
                                                    }
                                                    else if (dic.Count > 0)
                                                    {
                                                        foreach (var key in dic.Keys)
                                                        {

                                                            if (key == code)
                                                            {
                                                                convertList.Add(dic[key]);
                                                            }
                                                            else
                                                            {
                                                                if (outList.ContainsKey(key) && !string.IsNullOrEmpty(Convert.ToString(dic[key])))
                                                                    outList[key] = outList[key] + "," + dic[key];
                                                                else
                                                                    outList[key] = Convert.ToString(dic[key]);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (e.IsValid && convertList.Count > 0)
                                            {
                                                SetTableCols(e, string.Join(",", convertList.ToArray()));
                                            }
                                            if (outList.Count > 0)
                                            {
                                                foreach (var key in outList.Keys)
                                                {
                                                    excelData.Vaildate(ed =>
                                                    {
                                                        string colName = string.Format("{0}.{1}", ed.TableName, ed.FieldName);
                                                        if (colName == key || (ed.TableName == pkTableName && ed.FieldName == key))
                                                        {
                                                            if (e.RowIndex == ed.RowIndex)
                                                                SetTableCols(ed, outList[key]);
                                                        }
                                                    });
                                                }

                                            }
                                            #endregion
                                            break;
                                        default:
                                            if (e.IsValid)
                                            {
                                                SetTableCols(e, FilterNull(e.Value, filter));
                                            }
                                            break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                });

                foreach (var item in errors)
                {
                    errorInfo.Add(new CellErrorInfo()
                    {
                        RowIndex = item.RowIndex,
                        ColIndex = item.ColIndex,
                        ErrorText = item.ErrorText
                    });
                }
            }
            #endregion

            return errorInfo;
        }

        private DataTable GetCurrentRowToDataTable(ExcelRowInfo row)
        {
            DataTable excelTable = new DataTable();
            DataRow excelRow = excelTable.NewRow();
            foreach (var cell in row.Cells)
            {
                if (!excelTable.Columns.Contains(cell.FieldName))
                    excelTable.Columns.Add(cell.FieldName);
                excelRow[cell.FieldName] = cell.Value;
            }
            excelTable.Rows.Add(excelRow);
            return excelTable;
        }

        #region 为空时插入
        private string GetNullDataRuleSQL(string tableName)
        {
            StringBuilder sb = new StringBuilder();
            string PKColName = string.Format("{0}ID", tableName);
            var ftb = copyTables.FirstOrDefault(c => c.TableName == tableName);
            foreach (var table in copyTables.OrderBy(o => o.Sort))
            {
                if (table.TableName != tableName)
                {
                    sb.AppendFormat(@"
 if not exists(select * from syscolumns where id=object_id('{0}') and name='{1}') 
 begin
    alter table {0} add {1} nvarchar(50)
 end      

", table.TableName, PKColName);
                }

                foreach (var row in table.Rows)
                {
                    var trow = ftb.Rows.FirstOrDefault(c => c.RowIndex == row.RowIndex);
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    foreach (var cell in row.Cells)
                    {
                        string colName = cell.FieldName;
                        if(!string.IsNullOrEmpty(cell.Value))
                            dic.Add(colName, string.Format("'{0}'", cell.Value));

                        if (table.TableName != tableName)
                        {
                            if (!dic.ContainsKey(PKColName))
                                dic.Add(PKColName, string.Format("'{0}'", trow.Cells.FirstOrDefault(c => c.FieldName.ToUpper() == "ID").Value));
                        }
                    }

                    string fields = string.Join(",", dic.Select(c => c.Key).ToArray());
                    string values = string.Join(",", dic.Select(c => c.Value).ToArray());
                    sb.AppendFormat(@"insert into {0}({1})
                                    values({2})

                                    ", table.TableName, fields, values);
                }
            }

            return sb.ToString();
        }
        #endregion

        #region 存在时更新,不存在时插入
        private string GetUpdateDataRuleSQL(S_UI_ExcelImport import, string tableName)
        {
            UIFO fo = new UIFO();
            StringBuilder sb = new StringBuilder();
            Dictionary<int, string> masterDic = new Dictionary<int, string>();
            string PKColName = string.Format("{0}ID", tableName);
            var ftb = copyTables.FirstOrDefault(c => c.TableName == tableName);
            foreach (var row in ftb.Rows)
            {
                var dataTable = GetCurrentRowToDataTable(row);
                var uc = fo.ReplaceString(import.Condition, dataTable.Rows[0]);
                masterDic.Add(row.RowIndex, string.Format(@"

select @PKID=ID from {0} where {1}
if @PKID != '' @3
    begin
          @1
    end
    else
    begin
         @2
    end
set @PKID=''
", tableName, uc));
            }

            foreach (var table in copyTables.OrderBy(o => o.Sort))
            {
                if (table.TableName != tableName)
                {
                    sb.AppendFormat(@"
 if not exists(select * from syscolumns where id=object_id('{0}') and name='{1}') 
 begin
    alter table {0} add {1} nvarchar(50)
 end      

", table.TableName, PKColName);
                }
                foreach (var row in table.Rows)
                {
                    var master = masterDic.FirstOrDefault(c => c.Key.Equals(row.RowIndex));
                    if (master.Key > 0)
                    {
                        var trow = ftb.Rows.FirstOrDefault(c => c.RowIndex == row.RowIndex);
                        List<string> updateDic = new List<string>();
                        Dictionary<string, object> addDic = new Dictionary<string, object>();
                        foreach (var cell in row.Cells)
                        {
                            string colName = cell.FieldName;
                            //update语句
                            if (colName.ToUpper() != "ID")
                            {
                                if(!string.IsNullOrEmpty(cell.Value))
                                    updateDic.Add(string.Format("{0}='{1}'", colName, cell.Value));
                                else
                                    updateDic.Add(string.Format("{0}=null", colName));
                            }
                            //insert语句
                            if (!string.IsNullOrEmpty(cell.Value))
                                addDic.Add(colName, string.Format("'{0}'", cell.Value));
                            if (table.TableName != tableName)
                            {
                                if (!addDic.ContainsKey(PKColName))
                                    addDic.Add(PKColName, "@PKID");
                            }
                        }
                        string fields = string.Join(",", addDic.Select(c => c.Key).ToArray());
                        string values = string.Join(",", addDic.Select(c => c.Value).ToArray());
                        string updateValues = string.Join(",", updateDic.ToArray());

                        string updateSQL = string.Format(@"
    update {0} set {1} where {2}
", table.TableName, updateValues, table.TableName != tableName ? string.Format(" {0}=@PKID", PKColName) : " ID=@PKID");
                        string insertSQL = string.Format(@"
    insert into {0}({1})
        values({2})
", table.TableName, fields, values);

                        string exixtsSQL = "";
                        if (table.TableName != tableName)
                        {
                            exixtsSQL = string.Format(@" and exists(select 1 from {0} where {1}=@PKID)
", table.TableName, PKColName);
                        }
                        sb.Append(master.Value.Replace("@1", updateSQL).Replace("@2", insertSQL).Replace("@3", exixtsSQL));
                    }
                }
            }
            return string.Format(@"declare @PKID varchar(200)=''
                {0}
            ", sb.ToString());
        }
        #endregion

        #region 存在时插入, 不满足条件不插入
        private string GetAddDataRuleSQL(S_UI_ExcelImport import, string tableName)
        {
            UIFO fo = new UIFO();
            StringBuilder sb = new StringBuilder();
            Dictionary<int, string> masterDic = new Dictionary<int, string>();
            string PKColName = string.Format("{0}ID", tableName);
            var ftb = copyTables.FirstOrDefault(c => c.TableName == tableName);
            foreach (var row in ftb.Rows)
            {
                var dataTable = GetCurrentRowToDataTable(row);
                var uc = fo.ReplaceString(import.Condition, dataTable.Rows[0]);
                masterDic.Add(row.RowIndex, string.Format(@"

select @PKID=ID from {0} where {1}
if @PKID != ''
    begin
          @1
    end
set @PKID=''
", tableName, uc));
            }

            foreach (var table in copyTables.OrderBy(o => o.Sort))
            {
                if (table.TableName != tableName)
                {
                    sb.AppendFormat(@"
 if not exists(select * from syscolumns where id=object_id('{0}') and name='{1}') 
 begin
    alter table {0} add {1} nvarchar(50)
 end      

", table.TableName, PKColName);
                }
                foreach (var row in table.Rows)
                {
                    var master = masterDic.FirstOrDefault(c => c.Key.Equals(row.RowIndex));
                    if (master.Key > 0)
                    {
                        var trow = ftb.Rows.FirstOrDefault(c => c.RowIndex == row.RowIndex);
                        Dictionary<string, object> addDic = new Dictionary<string, object>();
                        foreach (var cell in row.Cells)
                        {
                            string colName = cell.FieldName;
                            if(!string.IsNullOrEmpty(cell.Value))
                                addDic.Add(colName, string.Format("'{0}'", cell.Value));

                            if (table.TableName != tableName)
                            {
                                if (!addDic.ContainsKey(PKColName))
                                    addDic.Add(PKColName, "@PKID");
                            }
                        }
                        string fields = string.Join(",", addDic.Select(c => c.Key).ToArray());
                        string values = string.Join(",", addDic.Select(c => c.Value).ToArray());

                        string insertSQL = string.Format(@"
    insert into {0}({1})
        values({2})
", table.TableName, fields, values);

                        sb.Append(master.Value.Replace("@1", insertSQL));
                    }
                }
            }
            return string.Format(@"declare @PKID varchar(200)=''
                {0}
            ", sb.ToString());
        }
        #endregion

        #region 存在时删除, 不存在插入
        private string GetDeleteDataRuleSQL(S_UI_ExcelImport import, string tableName)
        {
            UIFO fo = new UIFO();
            StringBuilder sb = new StringBuilder();
            Dictionary<int, string> masterDic = new Dictionary<int, string>();
            string PKColName = string.Format("{0}ID", tableName);
            var ftb = copyTables.FirstOrDefault(c => c.TableName == tableName);
            foreach (var row in ftb.Rows)
            {
                var dataTable = GetCurrentRowToDataTable(row);
                var uc = fo.ReplaceString(import.Condition, dataTable.Rows[0]);
                masterDic.Add(row.RowIndex, string.Format(@"

select @PKID=ID from {0} where {1}
if @PKID != '' @3
    begin
          @1
    end
    else
    begin
         @2
    end
set @PKID=''
", tableName, uc));
            }

            foreach (var table in copyTables.OrderBy(o => o.Sort))
            {
                if (table.TableName != tableName)
                {
                    sb.AppendFormat(@"
 if not exists(select * from syscolumns where id=object_id('{0}') and name='{1}') 
 begin
    alter table {0} add {1} nvarchar(50)
 end      

", table.TableName, PKColName);
                }
                foreach (var row in table.Rows)
                {
                    var master = masterDic.FirstOrDefault(c => c.Key.Equals(row.RowIndex));
                    if (master.Key > 0)
                    {
                        var trow = ftb.Rows.FirstOrDefault(c => c.RowIndex == row.RowIndex);
                        Dictionary<string, object> addDic = new Dictionary<string, object>();
                        foreach (var cell in row.Cells)
                        {
                            string colName = cell.FieldName;
                            if(!string.IsNullOrEmpty(cell.Value))
                                addDic.Add(colName, string.Format("'{0}'", cell.Value));

                            if (table.TableName != tableName)
                            {
                                if (!addDic.ContainsKey(PKColName))
                                    addDic.Add(PKColName, string.Format("'{0}'", trow.Cells.FirstOrDefault(c => c.FieldName.ToUpper() == "ID").Value));
                            }
                        }
                        string fields = string.Join(",", addDic.Select(c => c.Key).ToArray());
                        string values = string.Join(",", addDic.Select(c => c.Value).ToArray());

                        string deleteSQL = string.Format(@"
    delete {0} where {1}
", table.TableName, table.TableName != tableName ? string.Format(" {0}=@PKID", PKColName) : " ID=@PKID");
                        string insertSQL = string.Format(@"
if(1=1 {3})
begin
    insert into {0}({1})
        values({2})
end
", table.TableName, fields, values, table.TableName == tableName ? "" :
 string.Format("and exists(select 1 from {0} where ID='{1}')", tableName, trow.Cells.FirstOrDefault(c => c.FieldName.ToUpper() == "ID").Value));
                        string exixtsSQL = "";
                        if (table.TableName != tableName)
                        {
                            exixtsSQL = string.Format(@" and exists(select 1 from {0} where {1}=@PKID)
", table.TableName, PKColName);
                        }
                        sb.Append(master.Value.Replace("@1", deleteSQL).Replace("@2", insertSQL).Replace("@3", exixtsSQL));
                    }
                }
            }
            return string.Format(@"declare @PKID varchar(200)=''
                {0}
            ", sb.ToString());
        }
        #endregion

        public string SaveExcelImport(S_UI_ExcelImport import)
        {
            StringBuilder sb = new StringBuilder();
            var layoutField = JsonHelper.ToList(import.LayoutField);
            char[] splitStr = { ',', '，' };
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(import.ConnName);
            var tableName = import.TableNames.Split(splitStr)[0];

            try
            {
                if (copyTables.Count > 0)
                {
                    for (int i = 0; i < import.TableNames.Split(splitStr).Count(); i++)
                    {
                        var tb = copyTables.FirstOrDefault(c => c.TableName == import.TableNames.Split(splitStr)[i]);
                        if (tb != null)
                            tb.Sort = i;
                    }
                    switch (import.DataRule)
                    {
                        case "update":
                            sb.Append(GetUpdateDataRuleSQL(import, tableName));
                            break;
                        case "add":
                            sb.Append(GetAddDataRuleSQL(import, tableName));
                            break;
                        case "delete":
                            sb.Append(GetDeleteDataRuleSQL(import, tableName));
                            break;
                        default:
                            sb.Append(GetNullDataRuleSQL(tableName));
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(sb.ToString()))
                    sqlHelper.ExecuteNonQuery(
                        string.Format(@"
begin tran
{0}
commit tran
", sb.ToString())
                        );
                else
                    return "数据无法保存，请联系管理员查看错误日志！";
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public ActionResult Upload(HttpPostedFileBase Fdata, string tmplCode)
        {
            Formula.LogWriter.Info(string.Format("UploadExcel - Excel导入的Key：{0} - 开始", tmplCode));

            if (Fdata == null || Fdata.InputStream == null)
                return RedirectToAction("Import", new { TmplCode = tmplCode, ErrorMsg = "数据文件没有上传，请上传要导入数据文件！" });

            var fileSize = Fdata.InputStream.Length;
            byte[] fileBuffer = new byte[fileSize];
            Fdata.InputStream.Read(fileBuffer, 0, (int)fileSize);
            Fdata.InputStream.Close();

            ExcelData data = null;
            try
            {
                IImporter importer = new AsposeExcelImporter();
                data = importer.Import(fileBuffer, tmplCode);
            }
            catch (Exception ex)
            {
                Formula.LogWriter.Error(ex);
                return RedirectToAction("Import", new { TmplCode = tmplCode, ErrorMsg = ex.Message });
            }

            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var listDef = entities.S_UI_ExcelImport.FirstOrDefault(c => c.Code == tmplCode);
            if (listDef == null)
                throw new Exception(string.Format("找不到TmplCode为：{0}的Excel导入模板,请检查是否已配置!", tmplCode));

            // 验证数据
            var errors = ValidateData(data, listDef);
            if (errors.Count() > 0)
            {
                var errorFilePath = string.Format(@"~/ErrorExcels/{0}_有错误_{1}.xls", tmplCode, Formula.FormulaHelper.GetUserInfo().UserName);
                WriteErrorInfoToExcel(errors, fileBuffer, errorFilePath);
                return RedirectToAction("Import", new { TmplCode = tmplCode, ErrorCount = errors.Count, ErrorFilePath = Url.Content(errorFilePath) });
            }
            else
            {
                //保存数据
                string strResult = SaveExcelImport(listDef);
                if (!string.IsNullOrEmpty(strResult))
                    return RedirectToAction("Import", new { TmplCode = tmplCode, ErrorCount = errors.Count, ErrorMsg = strResult });
                else
                    return RedirectToAction("Import", new { TmplCode = tmplCode, result = data.Tables[0].Rows.Count });
            }
        }

        #endregion

        private static string CorrectString(Match match)
        {
            string matchValue = match.Value;
            if (matchValue.IndexOf("\"") == -1 && matchValue.IndexOf("{") == -1 && matchValue.IndexOf("}") == -1)
                matchValue = "\"" + matchValue + "\"";
            return matchValue;
        }
        [ValidateInput(false)]
        public ActionResult ExportInlineExcel(string jsonColumns, string excelKey, string title, string masterDataUrl, string masterColumn, string queryFormData, string sortField, string sortOrder, string detailDataUrl, string relateColumn, string referUrl)
        {
            Formula.LogWriter.Info(string.Format("ExportExcel - Excel导出的Key：{0} - 开始", excelKey));
            #region 收集自动生成模板的列信息
            var columns = JsonConvert.DeserializeObject<List<ColumnInfo>>(jsonColumns);
            // 清空中文名称为空的列
            if (columns != null)
            {
                for (var i = columns.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrWhiteSpace(columns[i].ChineseName))
                    {
                        columns.RemoveAt(i);
                    }
                }
                HttpContext.Items["__ColumnInfo"] = columns;
            }
            #endregion

            // 导出到Excel的数据源，
            DataTable dtMaster = null;
            if (masterDataUrl.IndexOf("[]") >= 0 || masterDataUrl.IndexOf("{") >= 0) // 前台传入Data数据
            {
                masterDataUrl = Regex.Replace(masterDataUrl, @"(?<=\:).*?(?=\,)", new MatchEvaluator(CorrectString), RegexOptions.Compiled | RegexOptions.IgnoreCase);
                dtMaster = JsonConvert.DeserializeObject<DataTable>(masterDataUrl);
            }
            else // 前台传入请求数据的地址
            {
                var dic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(queryFormData))
                    dic.Add("queryFormData", queryFormData);
                if (!string.IsNullOrWhiteSpace(sortField))
                    dic.Add("sortField", sortField);
                if (!string.IsNullOrWhiteSpace(sortOrder))
                    dic.Add("sortOrder", sortOrder);
                var serverUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
                dtMaster = Get<DataTable>(serverUrl, masterDataUrl, referUrl, dic) ?? new DataTable();
            }

            DataTable dtDetail = null;
            if (detailDataUrl.IndexOf("[]") >= 0 || detailDataUrl.IndexOf("{") >= 0) // 前台传入Data数据
            {
                dtDetail = JsonConvert.DeserializeObject<DataTable>(detailDataUrl);
            }
            else // 前台传入请求数据的地址
            {
                var dic = new Dictionary<string, object>();
                string[] arrID = new string[dtMaster.Rows.Count];
                for (int i = 0; i < dtMaster.Rows.Count; i++)
                {
                    arrID[i] = Convert.ToString(dtMaster.Rows[i][masterColumn]);
                }
                string ids = string.Join(",", arrID);
                dic.Add("queryFormData", "{\"$IN$" + relateColumn + "\": \"" + ids + "\"}");
                var serverUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
                dtDetail = Get<DataTable>(serverUrl, detailDataUrl, referUrl, dic) ?? new DataTable();
            }
            DataTable dt = new DataTable();
            //增加从表列到主表中
            foreach (DataColumn dc in dtMaster.Columns)
            {
                if (!dt.Columns.Contains(dc.ColumnName))
                {
                    dt.Columns.Add(dc.ColumnName, dc.DataType);
                }
            }
            foreach (DataColumn dc in dtDetail.Columns)
            {
                if (!dt.Columns.Contains(dc.ColumnName))
                {
                    dt.Columns.Add(dc.ColumnName, dc.DataType);
                }
            }
            //合并数据
            foreach (DataRow dr in dtMaster.Rows)
            {
                DataRow[] details = dtDetail.Select(relateColumn + " = '" + Convert.ToString(dr[masterColumn]) + "'");
                if (details.Count() > 0)
                {
                    foreach (DataRow detail in details)
                    {
                        DataRow drNew = dt.NewRow();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (dr.Table.Columns.Contains(dc.ColumnName))
                                drNew[dc.ColumnName] = dr[dc.ColumnName];
                            else if (detail.Table.Columns.Contains(dc.ColumnName))
                                drNew[dc.ColumnName] = detail[dc.ColumnName];
                        }
                        dt.Rows.Add(drNew);
                    }
                }
                else
                {
                    DataRow drNew = dt.NewRow();
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        drNew[dc.ColumnName] = dr[dc.ColumnName];
                    }
                    dt.Rows.Add(drNew);
                }
            }


            dt.TableName = excelKey;
            var exporter = new AsposeExcelExporter();
            byte[] templateBuffer = null;

            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            var templatePath = path.EndsWith("\\") ? string.Format("{0}{1}_New.xls", path, excelKey) : string.Format("{0}\\{1}_New.xls", path, excelKey);
            if (System.IO.File.Exists(templatePath))
            {
                Formula.LogWriter.Info(string.Format("ExportExcel - 采用自定义模板，模板路径为：{0}", templatePath));
                templateBuffer = FileHelper.GetFileBuffer(templatePath);
            }
            else
            {
                templateBuffer = exporter.ParseTemplate(columns, excelKey, title);
            }

            var buffer = exporter.Export(dt, templateBuffer);

            Formula.LogWriter.Info(string.Format("ExportExcel - Excel导出的Key：{0} - 结束", excelKey));
            if (buffer != null)
            {
                return File(buffer, "application/vnd.ms-excel", Url.Encode(title) + ".xls");
            }

            Formula.LogWriter.Info(string.Format("ExportExcel - 导出数据失败，参数: masterDataUrl={0}<br>queryFormData={1}<br>jsonColumns={2}<br>excelKey={3}<br>title={4}<br> ", masterDataUrl, queryFormData, jsonColumns, excelKey, title));
            return Content("导出数据失败，请检查相关配置！");
        }

        /// <summary>
        /// 获取指定key的Excel模板的文件流
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual byte[] GetExcelTemplateFile(string key)
        {
            var path = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"];
            var tmplPath = path.EndsWith("\\") ? string.Format("{0}{1}.xls", path, key) : string.Format("{0}\\{1}.xls", path, key);
            if (!System.IO.File.Exists(tmplPath))
            {
                throw new Exception("找不到指点key的模板文件，key为：" + key);
            }
            var fileBuffer = FileHelper.GetFileBuffer(tmplPath);
            return fileBuffer;
        }

        // Get ImportExcel 导入Excel的界面
        [ValidateInput(false)]
        public ActionResult ImportExcel(string excelkey, string vaildURL, string saveURL, string ErrorFilePath, string ErrorCount, string result, string errorMsg)
        {
            ViewBag.ExcelKey = excelkey;
            ViewBag.VaildURL = vaildURL;
            ViewBag.SaveURL = saveURL;
            ViewBag.ErrorFilePath = ErrorFilePath;
            ViewBag.ErrorCount = ErrorCount;
            ViewBag.ErrorMsg = errorMsg;
            ViewBag.IsSuccess = !string.IsNullOrWhiteSpace(result) ? bool.TrueString : bool.FalseString;
            ViewBag.SuccessCount = result;
            return View();
        }

        [ValidateInput(false)]
        public ActionResult Import(string tmplCode, string ErrorCount, string ErrorFilePath, string result, string errorMsg)
        {
            ViewBag.TmplCode = tmplCode;
            ViewBag.ErrorCount = ErrorCount;
            ViewBag.ErrorFilePath = ErrorFilePath;
            ViewBag.ErrorMsg = errorMsg;
            ViewBag.IsSuccess = !string.IsNullOrWhiteSpace(result) ? bool.TrueString : bool.FalseString;
            ViewBag.SuccessCount = result;
            return View();
        }

        // Get DownloadExcelTemplate 下载Excel模板
        public ActionResult DownloadExcelTemplate(string excelkey)
        {
            var importer = new AsposeExcelImporter();
            var buffer = importer.GetExcelTemplate(excelkey);
            return File(buffer, "application/vnd.ms-excel", excelkey + ".xls");
        }

        //文件上传
        public ActionResult UploadExcel(HttpPostedFileBase Fdata, string excelKey, string vaildURL, string saveURL)
        {
            Formula.LogWriter.Info(string.Format("UploadExcel - Excel导入的Key：{0} - 开始", excelKey));

            if (Fdata == null || Fdata.InputStream == null)
                return RedirectToAction("ImportExcel", new { excelkey = excelKey, vaildURL = vaildURL, saveURL = saveURL, ErrorMsg = "数据文件没有上传，请上传要导入数据文件！" });

            var fileSize = Fdata.InputStream.Length;
            byte[] fileBuffer = new byte[fileSize];
            Fdata.InputStream.Read(fileBuffer, 0, (int)fileSize);
            Fdata.InputStream.Close();

            ExcelData data = null;
            try
            {
                IImporter importer = new AsposeExcelImporter();
                data = importer.Import(fileBuffer, excelKey);
            }
            catch (Exception ex)
            {
                Formula.LogWriter.Error(ex);
                return RedirectToAction("ImportExcel", new { excelkey = excelKey, vaildURL = vaildURL, saveURL = saveURL, ErrorMsg = ex.Message });
            }
            var serverUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Headers["Host"]);

            // 验证数据
            Formula.LogWriter.Info(string.Format("UploadExcel - 进入校验数据逻辑，校验地址：{0} - 开始", vaildURL));
            var errors = Post<List<CellErrorInfo>>(serverUrl, vaildURL, new { data = JsonConvert.SerializeObject(data) });
            Formula.LogWriter.Info(string.Format("UploadExcel - 进入校验数据逻辑，校验地址：{0} - 结束", vaildURL));
            if (errors.Count > 0)
            {
                var errorFilePath = string.Format(@"~/ErrorExcels/{0}_有错误_{1}.xls", excelKey, Formula.FormulaHelper.GetUserInfo().UserName);
                WriteErrorInfoToExcel(errors, fileBuffer, errorFilePath);
                return RedirectToAction("ImportExcel", new { excelkey = excelKey, vaildURL = vaildURL, saveURL = saveURL, ErrorCount = errors.Count, ErrorFilePath = Url.Content(errorFilePath) });
            }

            // 保存数据
            var dataTable = data.GetDataTable();
            Formula.LogWriter.Info(string.Format("UploadExcel - 进入数据保存逻辑，保存地址：{0} - 开始", saveURL));
            var strResult = Post(serverUrl, saveURL, new { data = JsonConvert.SerializeObject(dataTable, new DataTableConverter()) });
            Formula.LogWriter.Info(string.Format("UploadExcel - 进入数据保存逻辑，保存地址：{0} - 结束", saveURL));
            if (strResult != "Success")
            {
                Formula.LogWriter.Error(strResult);
                return RedirectToAction("ImportExcel", new { excelkey = excelKey, vaildURL = vaildURL, saveURL = saveURL, ErrorMsg = "数据无法保存，请联系管理员查看错误日志！" });
            }
            // 清空数据，释放内存
            data = null;

            Formula.LogWriter.Info(string.Format("UploadExcel - Excel导入的Key：{0} - 结束", excelKey));
            return RedirectToAction("ImportExcel", new { excelkey = excelKey, vaildURL = vaildURL, saveURL = saveURL, result = dataTable.Rows.Count.ToString() });
            //return Content(strResult);
        }

        private void WriteErrorInfoToExcel(IList<CellErrorInfo> errors, byte[] fileBuffer, string errorFilePath)
        {
            var workbook = new Workbook(new MemoryStream(fileBuffer));
            foreach (var info in errors)
            {
                var worksheet = workbook.Worksheets[0];

                //Add comment to cell 
                int commentIndex = worksheet.Comments.Add(info.RowIndex, info.ColIndex);

                //Access the newly added comment
                Comment comment = worksheet.Comments[commentIndex];

                //Set the comment note
                comment.Note = info.ErrorText;

                //Set the font of a comment
                comment.Font.Size = 12;
                comment.Font.IsBold = true;
                comment.HeightCM = 5;
                comment.WidthCM = 5;

                //为单元格添加样式    
                var cell = worksheet.Cells[info.RowIndex, info.ColIndex];
                var style = cell.GetStyle();
                //设置背景颜色
                style.ForegroundColor = System.Drawing.Color.Red;
                style.Pattern = BackgroundType.Solid;
                style.Font.IsBold = true;
                style.Font.Color = System.Drawing.Color.White;

                cell.SetStyle(style);
            }

            // 保存错误文件到临时目录
            workbook.Save(Server.MapPath(errorFilePath));
        }

        private void CellValidation(CellValidationArgs e)
        {
            if (e.FieldName == "Code" && e.Value.EndsWith("1"))
            {
                e.IsValid = false;
                e.ErrorText = "内容重复！";
            }
        }

        public class CellValidationArgs
        {
            public CellValidationArgs()
            {
                this.IsValid = true;
            }

            /// <summary>
            /// 行索引值
            /// </summary>
            public int RowIndex { get; set; }

            /// <summary>
            /// 列索引值
            /// </summary>
            public int ColIndex { get; set; }

            /// <summary>
            /// 对应的字段名称
            /// </summary>
            public string FieldName { get; set; }

            /// <summary>
            /// 字段值
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// 枚举Key
            /// </summary>
            public string EnumKey { get; set; }

            /// <summary>
            ///  是否验证通过
            /// </summary>
            public bool IsValid { get; set; }

            /// <summary>
            /// 错误提示文本
            /// </summary>
            public string ErrorText { get; set; }

            /// <summary>
            /// 所在数据行
            /// </summary>
            public DataRow Record { get; set; }

        }

        public class CellErrorInfo
        {
            /// <summary>
            /// 行索引值
            /// </summary>
            public int RowIndex { get; set; }

            /// <summary>
            /// 列索引值
            /// </summary>
            public int ColIndex { get; set; }

            /// <summary>
            /// 错误提示文本
            /// </summary>
            public string ErrorText { get; set; }
        }

        private T Get<T>(string serverUrl, string requestUrl, string referUrl, IDictionary<string, object> data = null)
        {
            T result = default(T);

            var restClient = new RestSharp.RestClient(serverUrl);
            var request = new RestSharp.RestRequest(requestUrl, RestSharp.Method.POST);
            if (data != null)
            {
                foreach (var key in data.Keys)
                {
                    request.AddParameter(key, data[key]);
                }
            }
            request.AddHeader("Referer", referUrl);//指定UrlRefer

            // 追加登录Cookie
            var cookie = FormsAuthentication.GetAuthCookie(User.Identity.Name, true);
            request.AddCookie(FormsAuthentication.FormsCookieName, cookie.Value);
            var response = restClient.Execute(request);
            if (response.ResponseStatus != RestSharp.ResponseStatus.Completed)
            {
                Formula.LogWriter.Info(string.Format("StatusCode:{0},ResponseStatus:{1},ErrorMessage:{2},serverUrl:{3},requestUrl:{4}",
                    response.StatusCode, response.ResponseStatus, response.ErrorMessage, serverUrl, requestUrl));
            }

            var json = response.Content;
            if (json.StartsWith("["))
            {
                result = JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                var jsonData = JsonHelper.ToObject(json);
                if (jsonData["data"] != null)
                {
                    var exportdata = jsonData["data"];
                    result = JsonConvert.DeserializeObject<T>(JsonHelper.ToJson(exportdata));
                }
                else if (json.IndexOf("\"total\":0") < 0)
                {
                    var dtStartIndex = json.IndexOf("[{");
                    var dtEndIndex = json.LastIndexOf("}]");
                    var dtJson = json.Substring(dtStartIndex, dtEndIndex - dtStartIndex + 2);
                    result = JsonConvert.DeserializeObject<T>(dtJson);
                }
            }
            return result;
        }

        private string Post(string apiBaseUri, string requestUrl, object data = null)
        {
            var restClient = new RestSharp.RestClient(apiBaseUri);
            var request = new RestSharp.RestRequest(requestUrl, RestSharp.Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            //if (data != null)
            //{
            //    foreach (var key in data.Keys)
            //    {
            //        request.AddParameter(key, data[key]);
            //    }
            //}
            request.AddBody(data);

            // 追加登录Cookie
            var cookie = FormsAuthentication.GetAuthCookie(User.Identity.Name, true);
            request.AddCookie(FormsAuthentication.FormsCookieName, cookie.Value);
            request.Timeout = 36000;
            var response = restClient.Execute(request);
            if (response.ResponseStatus != RestSharp.ResponseStatus.Completed)
            {
                Formula.LogWriter.Info(string.Format("StatusCode:{0},ResponseStatus:{1},ErrorMessage:{2},apiBaseUri:{3},requestUrl:{4}",
                    response.StatusCode, response.ResponseStatus, response.ErrorMessage, apiBaseUri, requestUrl));
            }
            return response.Content;
        }

        private T Post<T>(string serverUrl, string requestUrl, object data = null) where T : class
        {
            var content = Post(serverUrl, requestUrl, data);
            var result = JsonConvert.DeserializeObject<T>(content);
            return result;
        }
    }
}
