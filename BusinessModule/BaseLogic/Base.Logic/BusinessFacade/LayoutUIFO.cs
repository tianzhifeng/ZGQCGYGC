using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula.Helper;
using Formula;
using Base.Logic.Domain;
using Base.Logic.Model.UI.Form;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;
using System.Data;
using Config;
using Config.Logic;
using Formula.Exceptions;

namespace Base.Logic.BusinessFacade
{
    public class LayoutUIFO
    {
        public static string uiRegStr = "\\{[()（），。、；,.;0-9a-zA-Z_\u4e00-\u9faf]*\\}";//，。、；,.;

        #region 表单Html及脚本

        #region CreateFormHiddenHtml

        public string CreateFormHiddenHtml(string code)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var form = entities.Set<S_UI_Form>().Where(c => c.Code == code).OrderBy(c => c.ID).FirstOrDefault();
            if (form == null)
                throw new Exception(string.Format("编号为：“{0}”的表单不存在", code));
            StringBuilder sb = new StringBuilder();
            var items = JsonHelper.ToObject<List<FormItem>>(form.Items).Where(c => String.IsNullOrWhiteSpace(c.ItemType));

            foreach (var item in items)
            {
                sb.AppendFormat("\n<input name=\"{0}\" class=\"mini-hidden\" />", item.Code);
            }

            items = JsonHelper.ToObject<List<FormItem>>(form.Items).Where(c => !String.IsNullOrWhiteSpace(c.ItemType) && c.Visible == "false");
            foreach (var item in items)
            {
                if (item.ItemType == "SubTable")
                    continue;
                sb.AppendFormat(GetFormItemHtml(form, item));
            }

            return sb.ToString();
        }

        #endregion

        #region CreateFormHtml

        public string CreateFormHtml(string code, DataRow formDataRow)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();

            var formInfo = entities.Set<S_UI_Form>().Where(c => c.Code == code).OrderByDescending(c => c.CreateTime).FirstOrDefault();

            if (formDataRow == null) //解决导出HTMLformDataRow为null问题
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
                var dt = sqlHelper.ExecuteDataTable(string.Format("select top 1 * from {0}", formInfo.TableName));
                if (dt.Rows.Count == 0)
                {
                    formDataRow = dt.NewRow();
                }
                else
                {
                    formDataRow = dt.Rows[0];
                }
            }
            else
            {
                DateTime dateFormCreate = DateTime.Now; //表单创建时间
                if (formDataRow.Table.Columns.Contains("CreateTime") && (formDataRow["CreateTime"] is DBNull) == false)
                    dateFormCreate = DateTime.Parse(formDataRow["CreateTime"].ToString());

                formInfo = entities.Set<S_UI_Form>().Where(c => c.Code == code && c.CreateTime < dateFormCreate).OrderBy(c => c.CreateTime).FirstOrDefault();//找到相应的版本
                if (formInfo == null)
                    throw new Exception(string.Format("编号为：“{0}”的表单不存在", code));
            }

            var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);
            Regex reg = new Regex(uiRegStr);

            string layout = "";
            if (FormulaHelper.GetCurrentLGID() == "EN" && !string.IsNullOrEmpty(formInfo.LayoutEN))
                layout = formInfo.LayoutEN;
            else
                layout = formInfo.Layout;

            string result = reg.Replace(layout ?? "", (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                FormItem item = items.SingleOrDefault(c => c.Name == value);

                #region 没有控件时

                if (item == null)
                {
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request[value]))
                        return HttpContext.Current.Request[value];
                    else
                        return m.Value;
                }

                #endregion

                #region 控件类型为空时

                if (string.IsNullOrEmpty(item.ItemType))
                {
                    if (formDataRow != null && formDataRow.Table.Columns.Contains(item.Code))
                        return string.Format("<span id='{0}'>{1}</span>", item.Code, formDataRow[item.Code].ToString());
                    else
                        return string.Format("<span id='{0}'></span>", item.Code);
                }

                #endregion

                return GetFormItemHtml(formInfo, item);

            });

            return result;
        }

        private string GetFormItemHtml(S_UI_Form formInfo, FormItem item)
        {
            #region 控件类型为子表时

            if (item.ItemType == "SubTable")
            {
                return CreateSubTableHtml(formInfo, item);
            }

            #endregion

            string miniuiSettings = GetMiniuiSettings(item.Settings);
            if (miniuiSettings == "")
                miniuiSettings = "style='width:100%'";

            string dataPty = ""; //控件的data属性

            if (item.ItemType == "TextBox" | item.ItemType == "TextArea")
            {
                if (!miniuiSettings.Contains("maxLength"))
                {
                    if (item.FieldType == "nvarchar(50)")
                        miniuiSettings += " maxLength='50'";
                    if (item.FieldType == "nvarchar(200)")
                        miniuiSettings += " maxLength='200'";
                    if (item.FieldType == "nvarchar(500)")
                        miniuiSettings += " maxLength='500'";
                    if (item.FieldType == "nvarchar(2000)")
                        miniuiSettings += " maxLength='2000'";
                }
            }
            else if (item.ItemType == "CheckBoxList" || item.ItemType == "RadioButtonList" || item.ItemType == "ComboBox")
            {
                dataPty = GetFormItemDataPty(formInfo.TableName, item.Code, item.Settings);
            }


            return string.Format("<input name='{0}' {5} class='mini-{1}' {2} {3} {4} {6}/>"
                , item.Code, item.ItemType.ToLower(), miniuiSettings
                , item.Enabled == "true" ? "" : "enabled='false'"
                , item.Visible == "true" ? "" : "visible='false'"
                , item.ItemType == "ButtonEdit" ? (Config.Constant.IsOracleDb ? string.Format("textName='{0}NAME'", item.Code) : string.Format("textName='{0}Name'", item.Code)) : ""
                , dataPty
                );
        }


        #endregion

        #region CreateFormScript

        public string CreateFormScript(string code, bool isOutput = false)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var form = entities.Set<S_UI_Form>().Where(c => c.Code == code).OrderByDescending(c => c.ID).FirstOrDefault();
            if (form == null)
                throw new Exception(string.Format("编号为：“{0}”的表单不存在", code));
            StringBuilder sb = new StringBuilder("\n");
            var list = JsonHelper.ToObject<List<FormItem>>(form.Items);

            #region 添加选择器脚本

            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.Settings))
                    continue;

                if (item.ItemType == "ButtonEdit" || item.ItemType == "LinkEdit")
                {
                    var dic = JsonHelper.ToObject<Dictionary<string, string>>(item.Settings);

                    string returnParams = "value:ID,text:Name";
                    if (dic.ContainsKey("ReturnParams"))
                        returnParams = dic["ReturnParams"];

                    if (dic["onbuttonclick"].ToString().Trim() != "" || dic["SelectorKey"].ToString().Trim() == "")
                    {
                        //自定义了buttonclick则不在添加选择器,或没有选择key
                    }
                    else if (dic["SelectorKey"].ToString() == "SystemUser")
                    {
                        if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                            sb.AppendFormat("addMultiUserSelector('{0}',{{returnParams:'{1}'}});\n", item.Code, returnParams);
                        else
                            sb.AppendFormat("addSingleUserSelector('{0}',{{returnParams:'{1}'}});\n", item.Code, returnParams);
                    }
                    else if (dic["SelectorKey"].ToString() == "SystemOrg")
                    {
                        StringBuilder paramStr = new StringBuilder();
                        if (dic.ContainsKey("UrlParams") && dic["UrlParams"] != "")
                        {
                            var paramSettings = dic["UrlParams"].Split('&');
                            foreach (var param in paramSettings)
                            {
                                var keyValue = param.Split('=');
                                paramStr.Append(keyValue[0]);
                                paramStr.Append(":'");
                                paramStr.Append(keyValue.Length > 1 ? keyValue[1] : "");
                                paramStr.Append("',");
                            }
                        }

                        if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                            sb.AppendFormat("addMultiOrgSelector('{0}',{{{2} returnParams:'{1}'}});\n", item.Code, returnParams, paramStr.ToString());
                        else
                            sb.AppendFormat("addSingleOrgSelector('{0}',{{{2} returnParams:'{1}'}});\n", item.Code, returnParams, paramStr.ToString());
                    }
                    else if (dic["SelectorKey"].ToString() != "")
                    {
                        string selectorKey = dic["SelectorKey"];
                        var selector = entities.S_UI_Selector.SingleOrDefault(c => c.Code == selectorKey);
                        if (selector != null)
                        {
                            var url = selector.URLSingle;
                            if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                                url = selector.URLMulti;

                            if (dic.ContainsKey("UrlParams") && dic["UrlParams"] != "")
                            {
                                if (url.Contains("?"))
                                    url += "&" + dic["UrlParams"];
                                else
                                    url += "?" + dic["UrlParams"];
                            }

                            sb.AppendFormat("addSelector('{0}', '{1}', {{ allowResize: true,title:'{2}',width:'{3}',height:'{4}',returnParams:'{5}' }});\n"
                                , item.Code, url, selector.Title, selector.Width, selector.Height, returnParams);
                        }
                    }
                }
                else if (item.ItemType == "SubTable")
                {
                    var _dic = JsonHelper.ToObject(item.Settings);
                    var subList = JsonHelper.ToObject<List<FormItem>>(_dic["listData"].ToString());
                    foreach (var subItem in subList)
                    {
                        if (string.IsNullOrEmpty(subItem.Settings))
                            continue;

                        if (subItem.ItemType == "ButtonEdit")
                        {
                            string selectorName = item.Code + "_" + subItem.Code;

                            var dic = JsonHelper.ToObject<Dictionary<string, string>>(subItem.Settings);

                            string returnParams = "value:ID,text:Name";
                            if (dic.ContainsKey("ReturnParams"))
                                returnParams = dic["ReturnParams"];

                            if (dic["SelectorKey"].ToString() == "SystemUser")
                            {
                                if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                                    sb.AppendFormat("addMultiUserSelector('{0}',{{returnParams:'{1}'}});\n", selectorName, returnParams);
                                else
                                    sb.AppendFormat("addSingleUserSelector('{0}',{{returnParams:'{1}'}});\n", selectorName, returnParams);
                            }
                            else if (dic["SelectorKey"].ToString() == "SystemOrg")
                            {
                                StringBuilder paramStr = new StringBuilder();
                                if (dic.ContainsKey("UrlParams") && dic["UrlParams"] != "")
                                {
                                    var paramSettings = dic["UrlParams"].Split('&');
                                    foreach (var param in paramSettings)
                                    {
                                        var keyValue = param.Split('=');
                                        paramStr.Append(keyValue[0]);
                                        paramStr.Append(":'");
                                        paramStr.Append(keyValue.Length > 1 ? keyValue[1] : "");
                                        paramStr.Append("',");
                                    }
                                }

                                if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                                    sb.AppendFormat("addMultiOrgSelector('{0}',{{{2} returnParams:'{1}'}});\n", selectorName, returnParams, paramStr.ToString());
                                else
                                    sb.AppendFormat("addSingleOrgSelector('{0}',{{{2} returnParams:'{1}'}});\n", selectorName, returnParams, paramStr.ToString());
                            }
                            else if (dic["SelectorKey"].ToString() != "")
                            {
                                string selectorKey = dic["SelectorKey"];
                                var selector = entities.S_UI_Selector.SingleOrDefault(c => c.Code == selectorKey);
                                if (selector != null)
                                {
                                    var url = selector.URLSingle;
                                    if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                                        url = selector.URLMulti;

                                    if (dic.ContainsKey("UrlParams") && dic["UrlParams"] != "")
                                    {
                                        if (url.Contains("?"))
                                            url += "&" + dic["UrlParams"];
                                        else
                                            url += "?" + dic["UrlParams"];
                                    }

                                    sb.AppendFormat("addSelector('{0}', '{1}', {{ allowResize: true,title:'{2}',width:'{3}',height:'{4}',returnParams:'{5}' }});\n"
                                        , selectorName, url, selector.Title, selector.Width, selector.Height, returnParams);
                                }
                            }
                        }
                    }
                }

            }

            #endregion

            #region 添加系统枚举

            var enumNameList = new List<string>();

            IEnumService enumService = FormulaHelper.GetService<IEnumService>();

            //增加流程阶段枚举
            sb.AppendFormat("\n var FlowPhase = {0}; ", JsonHelper.ToJson(enumService.GetEnumTable("FlowPhase")));

            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.Settings))
                    continue;
                if (item.ItemType == "CheckBoxList" || item.ItemType == "RadioButtonList" || item.ItemType == "ComboBox")
                {
                    var dic = JsonHelper.ToObject(item.Settings);
                    var data = dic["data"].ToString().Trim();
                    if (data == "")
                        continue;

                    var key = GetEnumKey(form.ConnName, form.TableName, item.Code, data);
                    var enumStr = GetEnumString(form.ConnName, form.TableName, item.Code, data);

                    if (!string.IsNullOrEmpty(enumStr))
                    {
                        if (isOutput)
                            sb.Append(GetOutputEnumString(form.ConnName, form.TableName, item.Code, data));
                        else
                            sb.AppendFormat("\n var {0} = {1}; ", key, enumStr);
                    }
                }

                if (item.ItemType == "SubTable")
                {
                    var _dic = JsonHelper.ToObject(item.Settings);
                    var subTableItems = JsonHelper.ToObject<List<FormItem>>(_dic["listData"].ToString());
                    foreach (var subItem in subTableItems)
                    {
                        if (string.IsNullOrEmpty(subItem.Settings))
                            continue;
                        if (subItem.ItemType == "ComboBox")
                        {
                            var dic = JsonHelper.ToObject(subItem.Settings);
                            var data = dic["data"].ToString().Trim();
                            if (data == "")
                                continue;

                            string tableName = form.TableName + "_" + item.Code;
                            string key = GetEnumKey(form.ConnName, tableName, subItem.Code, data);
                            string enumStr = GetEnumString(form.ConnName, tableName, subItem.Code, data);
                            if (!string.IsNullOrEmpty(enumStr))
                            {
                                if (isOutput)
                                    sb.AppendFormat(GetOutputEnumString(form.ConnName, tableName, subItem.Code, data));
                                else
                                    sb.AppendFormat("\n var {0} = {1}; ", key, enumStr);
                            }
                        }
                    }
                }
            }

            #endregion

            sb.Append(@"
function commitGridEdit(gridId) { 
    var grid = mini.get(gridId);        
    grid.commitEdit();    
}
function moveUp(gridId) {
    var dataGrid = mini.get(gridId);
    var rows = dataGrid.getSelecteds();
    dataGrid.moveUp(rows);
}
function moveDown(gridId) {
    var dataGrid = mini.get(gridId);
    var rows = dataGrid.getSelecteds();
    dataGrid.moveDown(rows);
}    
");
            sb.AppendLine();
            sb.Append(HttpContext.Current.Server.HtmlDecode(form.ScriptText));

            return sb.ToString();
        }

        #endregion

        #region GetDefaultValue

        public string GetDefaultValue(string code, string defaultValueTmpl, Dictionary<string, DataTable> dtDic)
        {
            if (string.IsNullOrEmpty(defaultValueTmpl))
                return "";

            string result = "";
            if (dtDic.Count == 0)
            {
                result = ReplaceString(defaultValueTmpl, null);
            }
            else
            {
                result = ReplaceString(defaultValueTmpl, null, null, dtDic);  //新的配置方式为默认数据表名.字段名

                if (result.Contains('{') == true)//替换完成后，发现仍然有大括号，说明使用的配置方式为：字段名，需要遍历默认值表
                {
                    foreach (var dt in dtDic.Values)
                    {
                        if (dt.Rows.Count > 0)
                            result = ReplaceString(result, dt.Rows[0]);
                        if (result.Contains('{') == false)  //没有大括号，说明已经替换完毕
                            break;
                    }
                }
            }


            return result;
        }

        #endregion

        #region GetDefaultValueRows

        public Dictionary<string, DataTable> GetDefaultValueDic(string DefaultValueSettings, DataRow row = null)
        {
            try
            {
                if (string.IsNullOrEmpty(DefaultValueSettings))
                    return new Dictionary<string, DataTable>();

                Dictionary<string, DataTable> defaultValueDic = new Dictionary<string, DataTable>();
                foreach (var item in JsonHelper.ToList(DefaultValueSettings))
                {
                    SQLHelper tmpSQLHelper = SQLHelper.CreateSqlHelper(item["ConnName"].ToString());
                    string defaultSQL = ReplaceString(item["SQL"].ToString(), row);
                    DataTable tmpDT = tmpSQLHelper.ExecuteDataTable(defaultSQL);

                    string code = Guid.NewGuid().ToString();
                    if (item.ContainsKey("Code") && item["Code"].ToString().Trim() != "")
                        code = item["Code"].ToString().Trim();
                    defaultValueDic.Add(code, tmpDT);
                }

                return defaultValueDic;
            }
            catch (AuthorizationException ex)
            {
                throw ex;
            }
            catch (BusinessException ex)
            {
                throw ex;
            }
            catch
            {
                throw new BusinessException("默认值Sql执行失败！");
            }
        }

        #endregion

        #region 私有方法

        #region CreateSubTableHtml

        private string CreateSubTableHtml(S_UI_Form formInfo, FormItem formItem)
        {
            if (string.IsNullOrEmpty(formItem.Settings))
                return "";
            var dic = JsonHelper.ToObject(formItem.Settings);
            var list = JsonHelper.ToObject<List<FormItem>>(dic["listData"].ToString());
            if (list.Count == 0)
                return "";

            //默认值Dic
            var defaultDic = new Dictionary<string, string>();
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.DefaultValue))
                    continue;
                if (item.DefaultValue.Contains(',') && item.ItemType == "ButtonEdit")
                {
                    defaultDic.Add(item.Code, GetDefaultValue(item.Code, item.DefaultValue.Split(',').First(), GetDefaultValueDic(formInfo.DefaultValueSettings)));
                    defaultDic.Add(item.Code + "Name", GetDefaultValue(item.Code, item.DefaultValue.Split(',').Last(), GetDefaultValueDic(formInfo.DefaultValueSettings)));
                }
                else
                {
                    defaultDic.Add(item.Code, GetDefaultValue(item.Code, item.DefaultValue, GetDefaultValueDic(formInfo.DefaultValueSettings)));
                }
            }


            string miniuiSettings = GetMiniuiSettings(dic["formData"].ToString());
            if (miniuiSettings == "")
                miniuiSettings = "style='width:100%;height:100px;'";

            var dicSubTableSettings = JsonHelper.ToObject(dic["formData"].ToString());

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
<div id='toolbar{3}' class='mini-toolbar'  style='height:25px;border-bottom: 0px;{4}'>
    <table>
        <tr>
            <td style='text-align:left;'>
                <a class='mini-button' id='btn{3}Add' iconcls='icon-add' onclick='addRow({0});' visible='{5}'>添加</a>
                <a class='mini-button' id='btn{3}Delete' iconcls='icon-remove' onclick='delRow({1});' visible='{6}'>移除</a>
                <a class='mini-button' iconcls='icon-up' onclick='moveUp({2});' visible='{7}'>上移</a>
                <a class='mini-button' iconcls='icon-down' onclick='moveDown({2});' visible='{8}'>下移</a>
            </td>
            <td>
            </td>
        </tr>
    </table>
</div>
", JsonHelper.ToJson(defaultDic) + ",{gridId:\"" + formItem.Code + "\",isLast:true}"
 , "{gridId:\"" + formItem.Code + "\"}"
 , "\"" + formItem.Code + "\""
 , formItem.Code
 , dicSubTableSettings.ContainsKey("ShowToolbar") && dicSubTableSettings["ShowToolbar"].ToString() == "false" ? "display:none;" : ""
 , dicSubTableSettings.ContainsKey("ShowBtnAdd") && dicSubTableSettings["ShowBtnAdd"].ToString() == "false" ? "false" : "true"
 , dicSubTableSettings.ContainsKey("ShowBtnRemove") && dicSubTableSettings["ShowBtnRemove"].ToString() == "false" ? "false" : "true"
 , dicSubTableSettings.ContainsKey("ShowBtnUp") && dicSubTableSettings["ShowBtnUp"].ToString() == "false" ? "false" : "true"
 , dicSubTableSettings.ContainsKey("ShowBtnDown") && dicSubTableSettings["ShowBtnDown"].ToString() == "false" ? "false" : "true"
 );



            sb.AppendFormat(@" 
        <div id='{0}' {1} {2} {3} class='mini-datagrid' allowcellvalid='true' multiselect='true' allowcelledit='true' allowcellselect='true' showpager='false' allowUnselect='false'>
 ", formItem.Code, miniuiSettings
  , formItem.Enabled == "true" ? "" : "enabled='false'"
  , formItem.Visible == "true" ? "" : "visible='false'");


            sb.Append(@"
 <div property='columns'>
");
            if (dicSubTableSettings.ContainsKey("showIndexColumn") && dicSubTableSettings["showIndexColumn"].ToString() == "true")
            {
                sb.Append(@"
            <div type='indexcolumn'></div>
");
            }

            if (dicSubTableSettings.ContainsKey("showCheckColumn") && dicSubTableSettings["showCheckColumn"].ToString() == "true")
            {
                sb.AppendFormat(@"       
            <div type='checkcolumn'></div>
");
            }

            foreach (var item in list)
            {
                #region 特殊控件处理

                if (item.ItemType == "CheckBox")
                {
                    sb.AppendFormat("\n<div type='checkboxcolumn' field='{0}' header='{1}' {2} {3}></div>"
                        , item.Code
                        , item.Name
                        , GetMiniuiSettings(JsonHelper.ToJson(item))
                        , GetMiniuiSettings(item.Settings)
                        );
                    continue;
                }
                else if (item.ItemType == "SingleFile")
                {

                    sb.AppendFormat(@"
<div field='{0}' name='{0}' displayfield='{0}Name' header='{1}' {2} renderer='onFileRender'>
    <input property='editor' class='mini-buttonedit' style='width: 100%;' onclick='btnUploadifiveClick' label='{3}'/>
</div>"

                       , item.Code
                       , item.Name
                       , GetMiniuiSettings(JsonHelper.ToJson(item))
                       , GetMiniuiSettings(item.Settings)
                       );



                    continue;
                }

                #endregion

                miniuiSettings = GetMiniuiSettings(item.Settings ?? "");
                if (miniuiSettings == "")
                    miniuiSettings = "style='width:100%'";

                string ColumnSettings = GetMiniuiSettings(item.ColumnSettings ?? "");//列格式

                string dataPty = "";
                if (item.ItemType == "ComboBox")
                {
                    string tableName = formInfo.TableName + "_" + formItem.Code;
                    dataPty = GetFormItemDataPty(tableName, item.Code, item.Settings);
                }

                #region 获取vtype
                string vtype = "";
                if (!string.IsNullOrEmpty(item.Settings))
                {
                    var _dic = JsonHelper.ToObject<Dictionary<string, string>>(item.Settings);
                    if (_dic.ContainsKey("required") && _dic["required"] == "true")
                        vtype += "required;";
                    if (_dic.ContainsKey("vtype"))
                        vtype += _dic["vtype"];
                }
                #endregion

                string comboBoxPty = "type='comboboxcolumn'";
                if (item.ItemType == "ComboBox")
                {
                    var columSettingsDic = JsonHelper.ToObject(item.Settings);
                    if (columSettingsDic.ContainsKey("textName") && columSettingsDic["textName"].ToString() != "")
                        comboBoxPty = string.Format("displayField='{0}'", columSettingsDic["textName"]);
                }


                sb.AppendFormat(@"
        <div name='{3}' field='{3}' {8} header='{4}' {5} {6} {7} {0} autoShowPopup='true' {12} {13}>
                <input {9} property='editor' class='mini-{1}' {2} {10} {11} {14} />
        </div>
"
                    , GetMiniuiSettings(JsonHelper.ToJson(item))
                    , item.ItemType.ToLower()
                    , miniuiSettings
                    , item.Code
                    , item.Name
                    , item.ItemType == "DatePicker" && ColumnSettings.IndexOf("dateformat") >= 0 ? "dateFormat='yyyy-MM-dd'" : ""
                    , item.ItemType == "ComboBox" ? comboBoxPty : ""
                    , vtype == "" ? "" : string.Format("vtype='{0}'", vtype)
                    , item.ItemType == "ButtonEdit" ? "displayfield='" + item.Code + (Config.Constant.IsOracleDb ? "NAME'" : "Name'") : ""
                    , item.ItemType == "ButtonEdit" ? " name='" + formItem.Code + "_" + item.Code + "'" : ""  //列表上选择暂时不支持智能感知，因此先加allowInput
                    , item.ItemType == "ComboBox" && miniuiSettings.Contains("multiselect='true'") == false && miniuiSettings.Contains("onitemclick=") == false ? "onitemclick=\"commitGridEdit('" + formItem.Code + "');\"" : ""
                    , item.ItemType == "DatePicker" ? "onhidepopup=\"commitGridEdit('" + formItem.Code + "');\"" : ""
                    , string.IsNullOrEmpty(item.SummaryType) ? "" : string.Format("summaryType='{0}' summaryRenderer='onSummaryRenderer'", item.SummaryType)
                    , ColumnSettings
                    , dataPty
                    );
            }

            sb.AppendFormat(@"
        
    </div>
</div>
");
            return sb.ToString();
        }

        #endregion


        #endregion

        #endregion

        #region 列表Html及脚本

        //导航HTML
        public string LayoutHtml(string code)
        {
            StringBuilder sb = new StringBuilder();
            var entities = FormulaHelper.GetEntities<BaseEntities>();

            string strFun = @"
    function onLoadLayout(e, key, type, isShow) {
        var selectID;
        if (type == 'list') {
            var navGrid = mini.get('navDataGrid');
            var select = navGrid.getSelected();
            if (select)
                selectID = select.ID;
        } else {
            var node = tree.getSelectedNode();
            if (node)
                selectID = node.ID;
        } 
        var curTab = listTabs.getActiveTab();
        listTabs.removeAll();
        for (var i = 0; i < layout.length; i++) {
            var tmplCode = layout[i].name;
            var url = layout[i].url + '&Sub_Key=' + key;
            if (selectID && layout[i].IsFK == 'T')
                url += '&Sub_ID=' + selectID;
            var tab = { title: layout[i].title, name: tmplCode, url: isShow ? url : '/MvcConfig/Workflow/Flow/FlowEmptyText', showCloseButton: false };
            tab.ondestroy = function (e) {
                var tabs = e.sender;
                var iframe = tabs.getTabIFrameEl(e.tab);

                //获取子页面返回数据
                var pageReturnData = iframe.contentWindow.getData ? iframe.contentWindow.getData() : '';
            }
            listTabs.addTab(tab);
        }
        if (curTab && curTab.name != '')
            listTabs.activeTab(listTabs.getTab(curTab.name));
        else
            listTabs.activeTab(listTabs.getTab(0));
    }
";

            string strLists = @"
<div style='width: calc(100% - 10px); height: calc(100% - 10px);border: 5px solid #cccccc;' >
<div class='mini-splitter' vertical='{0}' style='width: 100%; height: 100%;' borderstyle='border-top:0px;border-bottom:0px;background-color: #ccc;'>
     <div showcollapsebutton='false' style='width: 100%; height: 100%; border: 1px solid #999;'  size='{4}'>
        {1}
        {2}
     </div>
    <div showcollapsebutton='false' style='width: 100%; height: 100%; border: 1px solid #999;'>
        {3}
    </div>
</div></div>
";

            string strListTabs = @"
<div style='width: calc(100% - 10px); height: calc(100% - 10px);border: 5px solid #cccccc;' >
<div class='mini-splitter' vertical='{0}' style='width: 100%; height: 100%;' borderstyle='border-top:0px;border-bottom:0px;background-color: #ccc;'>
     <div showcollapsebutton='false' style='border: 1px solid #999;' size='{4}'>
        {1}
        {2}
     </div>
 <div showcollapsebutton='false' style='width: 100%; height: 100%; '>
        <div id='tabs' class='mini-tabs' 
            style='width: 100%; height: 100%;' 
            plain='false'>
        </div>
 </div>
</div>
</div>
 <script type='text/javascript'>
     mini.parse();
     var listTabs = mini.get('tabs');
     var layout = mini.decode({3});
     {5}
    onLoadLayout(this,'{6}ID','list', false);
 </script>
";

            string strTabs = @"
<div class='mini-fit'>
<div id='tabs' class='mini-tabs' 
     style='width: 100%; height: 100%;' bodyStyle='padding:0;border:0;'
     plain='false'>
</div></div>
 <script type='text/javascript'>
     mini.parse();
     var listTabs = mini.get('tabs');
     var layout = mini.decode({0});
     {1}
     onLoadTabLayout();
 </script>
";

            string strTreeCustom = @"
<div style='width: calc(100% - 10px); height: calc(100% - 10px);border: 5px solid #cccccc;' >
<div class='mini-splitter' style='width:100%;height:100%;' vertical='{7}' borderstyle='border:0px;padding-top:0px;padding-right:0px;padding-bottom:0px;background-color: #ccc;'>
    <div size='{0}' minsize='240' showCollapseButton='false' style='border: 1px solid #999;'>
        <div class='mini-toolbar gw-grid-toolbar' style='padding: 0px 0px 0px 0px;'>
            <table>
                <tr>
                    <td>              
                    </td>
                    <td class='gw-toolbar-right'>
                        <input id='treeKey' class='mini-buttonedit gw-searchbox' emptytext='请输入查询关键字' onenter='quickSearchTree' onbuttonclick='quickSearchTree' />
                    </td>
                </tr>
            </table>
        </div>
        <div class='mini-fit' style='background-color: #fff;border-top: 1px solid #ccc'>
            <ul id='tree' class='mini-tree' url='GetTree?TmplCode={1}' style='width:100%;'
                showTreeIcon='true' idField='{2}' onnodeclick=""onLoadLayout(this,'{9}ID','tree', true)"" textField='{3}' parentField='{4}' expandOnLoad='{5}' resultAsTree='false'>        
            </ul>
        </div>
    </div>
    <div showCollapseButton='false'>
        <div id='tabs' class='mini-tabs' 
         style='width: 100%; height: 100%;' 
         plain='false'>
        </div>
    </div>        
</div>
</div>
 <script type='text/javascript'>
     mini.parse();
     var listTabs = mini.get('tabs');
     var tree = mini.get('tree');
     var layout = mini.decode({6});
     {8}
     {10}
    onLoadLayout(this,'{9}ID','tree', false);
 </script>
";

            string strTreeGridCustom = @"
<div style='width: calc(100% - 10px); height: calc(100% - 10px);border: 5px solid #cccccc;' >
<div class='mini-splitter' style='width: 100%; height: 100%;'  vertical='{10}' borderstyle='border:0px;padding-top:0px;padding-right:0px;padding-bottom:0px;background-color: #ccc;'>
    <div size='{0}' minsize='240' showcollapsebutton='false' style='border: 1px solid #999;'>
        {9}
        <div id='tree' class='mini-treegrid' style='width: 100%; height: calc(100% - 37px);background-color: #fff;' url='GetTree?TmplCode={1}' showtreeicon='true'
            idfield='{2}' treecolumn='treename' parentfield='{4}' onnodeclick=""onLoadLayout(this,'{11}ID','tree', true)"" resultastree='false' expandonload='{5}'>
            <div property='columns'>     
               <div type='checkcolumn' width='30'></div>
               <div name='treename' field='{3}' width='200'></div>    
                {8}  
            </div>
        </div>
    </div>
    <div showcollapsebutton='false'>
        <div id='tabs' class='mini-tabs'
            style='width: 100%; height: 100%;' 
            plain='false'>
        </div>
    </div>
</div>
</div>
<script type='text/javascript'>
    mini.parse();
    var listTabs = mini.get('tabs');
    var layout = mini.decode({6});
    var tree = mini.get('tree');
    {7}
    onLoadLayout(this,'{11}ID','tree', false);
</script>
";

            string strGrid = @"
<div class='mini-fit' id='divNavGrid'>
    <div id='navDataGrid' class='mini-datagrid' style='width: 100%; height: 100%;' url='GetList?TmplCode={1}' {2}>
        <div property='columns'>     
           <div type='checkcolumn' width='30'></div>    
            {0}  
        </div>
    </div>
</div>
";

            string strBar = @"
<div class='mini-toolbar gw-grid-toolbar'  style='padding: 0px 0px 0px 0px;'>
    <table>
        <tr>
            <td>
                {2}
            </td>
            <td class='gw-toolbar-right' style='float: right;'>
                <input id='navKey' style='display: {3};' class='mini-buttonedit gw-searchbox' emptytext='请输入查询关键字' {0} {1} />
            </td>
        </tr>
    </table>
</div>";


            var uiLayout = entities.Set<S_UI_Layout>().Where(c => c.Code.Contains(code));
            EnumUseType curUseType = EnumUseType.Lists;
            S_UI_Layout layout = new S_UI_Layout();
            List<Dictionary<string, object>> fields = null;
            if (uiLayout.FirstOrDefault().UseType == EnumUseType.Lists.ToString())
                layout = uiLayout.FirstOrDefault(c => c.Code.Contains("NavTable_"));
            else
            {
                curUseType = (EnumUseType)Enum.Parse(typeof(EnumUseType), uiLayout.FirstOrDefault().UseType);
                layout = uiLayout.FirstOrDefault();
            }
            fields = JsonHelper.ToList(layout.LayoutField);
            List<string> searchKeys = new List<string>();


            string strField = "";
            StringBuilder sbField = new StringBuilder();
            bool multi = false;

            foreach (var field in fields)
            {
                if (field.GetValue("AllowSearch") == "true")
                {
                    searchKeys.Add(field.GetValue("field"));
                }

                string header = field["header"].ToString();
                var LGID = FormulaHelper.GetCurrentLGID();
                if (field.ContainsKey("header" + LGID))
                    header = field["header" + LGID].ToString();
                if (header.Contains("."))
                {
                    multi = true;
                    break;
                }
                sbField.AppendFormat("<div name='{3}' {0} {1} header='{2}'></div>", GetMiniuiSettings(field)
                    , field.ContainsKey("Settings") ? GetMiniuiSettings(field["Settings"].ToString()) : ""
                    , header, field["field"].ToString());
            }
            if (multi)
            {
                fields.RemoveWhere(c => c["Visible"].ToString() == "false");
                strField = CreateListColumn("", fields);
            }
            else
            {
                strField = sbField.ToString();
            }
            if (searchKeys.Count > 0)
            {
                string quick = "quickSearch('" + string.Join(",", searchKeys.ToArray()) + "',{gridId:'navDataGrid',queryBoxId:'navKey'})";
                if (curUseType == EnumUseType.TreeGridCustom)
                    quick = "treeGridSearch('" + string.Join(",", searchKeys.ToArray()) + "',{queryBoxId:'navKey', queryKey:'" + layout.TextField + "'})";
                strBar = string.Format(strBar, "onenter=\"" + quick + "\"", "onbuttonclick=\"" + quick + "\"",
                    curUseType == EnumUseType.Lists || curUseType == EnumUseType.ListTabs || curUseType == EnumUseType.TreeGridCustom ? string.Format("<span class='span_title'>{0}</span>", layout.Name) : "", "block");
            }
            else
            {
                if (curUseType == EnumUseType.Lists)
                    strBar = "";
                else
                    strBar = string.Format(strBar, "", "",
                        curUseType == EnumUseType.ListTabs || curUseType == EnumUseType.TreeGridCustom ? string.Format("<span class='span_title'>{0}</span>", layout.Name) : "", "none");
            }
            string tableName = layout.TableNames;
            if (tableName.IndexOf('_') >= 0)
                tableName = tableName.Substring(tableName.LastIndexOf('_') + 1, tableName.Length - tableName.LastIndexOf('_') - 1);
            string mode = layout.Mode == EnumUseMode.HighLow.ToString() ? "true" : "false";
            switch (curUseType)
            {
                case EnumUseType.Lists:
                    strGrid = string.Format(strGrid, strField, layout.Code, "onrowclick=\"onRowClick(this,'" + tableName + "ID')\"");
                    sb.AppendFormat(strLists, "false", strBar, strGrid, CreateHtml(code), layout.Height);
                    break;
                case EnumUseType.ListTabs:
                    strGrid = string.Format(strGrid, strField, layout.Code, "onrowclick=\"onLoadLayout(this,'" + tableName + "ID', 'list', true)\"");
                    sb.AppendFormat(strListTabs, mode, strBar, strGrid, layout.LayoutTab, layout.Height, strFun, tableName);
                    break;
                case EnumUseType.Tabs:
                    sb.AppendFormat(strTabs, layout.LayoutTab, @"                  
                        function onLoadTabLayout() {
                            for (var i = 0; i < layout.length; i++) {
                                var tmplCode = layout[i].name;
                                var url = layout[i].url;
                                var tab = { title: layout[i].title, name: tmplCode, url: url, showCloseButton: false };
                                tab.ondestroy = function (e) {
                                    var tabs = e.sender;
                                    var iframe = tabs.getTabIFrameEl(e.tab);

                                    //获取子页面返回数据
                                    var pageReturnData = iframe.contentWindow.getData ? iframe.contentWindow.getData() : '';
                                }
                                listTabs.addTab(tab);
                                listTabs.activeTab(listTabs.getTab(0));
                            }
                        }
                    ");
                    break;
                case EnumUseType.TreeCustom:
                    sb.AppendFormat(strTreeCustom, layout.Height, layout.Code, layout.IDField, layout.TextField, layout.ParentField, layout.IsExpand == "F" ? "true" : " false", layout.LayoutTab,
                     "false", @"function quickSearchTree(e) {
                            var sender = e.sender;
                            var tree = mini.get('tree');
                            if (sender) {
                                var key = mini.get('treeKey').getValue();
                                if (key == '') {
                                    tree.clearFilter();
                                } else {
                                    key = key.toLowerCase();
                                    tree.filter(function (node) {
                                        var nodeText = node." + layout.TextField + @";
                                        var text = nodeText ? nodeText.toLowerCase() : '';
                                        if (text.indexOf(key) != -1) {
                                            return true;
                                        }
                                    });
                                }
                            }
                        }", tableName, strFun);
                    break;
                case EnumUseType.TreeGridCustom:
                    sb.AppendFormat(strTreeGridCustom, layout.Height, layout.Code, layout.IDField, layout.TextField,
                        layout.ParentField, layout.IsExpand == "F" ? "true" : " false", layout.LayoutTab, strFun, strField, strBar, mode, tableName);
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }

        public string CreateHtml(string code)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var listDef = entities.Set<S_UI_Layout>().SingleOrDefault(c => c.Code == code);
            if (listDef == null)
                throw new Exception(string.Format("编号为：“{0}”的列表不存在", code));
            var fields = JsonHelper.ToList(listDef.LayoutField);
            var buttons = JsonHelper.ToList(listDef.LayoutButton);
            var showQueryForm = false; var rowCount = 2;
            var layout = entities.Set<S_UI_Layout>().SingleOrDefault(c => c.ID == listDef.ParentID);
            string tableName = layout.TableNames;
            if (layout != null && tableName.IndexOf('_') >= 0)
                tableName = tableName.Substring(tableName.LastIndexOf('_') + 1, tableName.Length - tableName.LastIndexOf('_') - 1);
            if (!String.IsNullOrEmpty(listDef.Settings))
            {
                var settings = JsonHelper.ToObject(listDef.Settings);
                if (settings.GetValue("ShowQueryForm") == "T")
                {
                    showQueryForm = true;
                    if (!String.IsNullOrEmpty(settings.GetValue("QueryFormColmuns")))
                    {
                        rowCount = Convert.ToInt32(settings.GetValue("QueryFormColmuns"));
                    }
                }
            }
            //详细查询字段
            var queryFields = fields.Where(c => c.ContainsKey("ItemType") && c["ItemType"].ToString() != "");
            //快速查询字段
            var quickQueryFields = fields.Where(c => c.ContainsKey("AllowSearch") && c["AllowSearch"].ToString() == "true");

            #region QueryForm

            StringBuilder sbQuery = new StringBuilder();

            int i = 0;
            foreach (var item in queryFields)
            {
                if (i % rowCount == 0)
                    sbQuery.Append("<tr>");

                if (i + 1 % rowCount == 0)
                    sbQuery.Append("<td width=\"5%\" /></tr>");
                else
                {
                    sbQuery.Append(GetQueryItemHtml(listDef, item, showQueryForm));
                }

                i++;
            }
            var queryHtml = string.Empty; string strQueryForm = string.Empty;
            if (showQueryForm)
            {
                queryHtml = @"
    <div class='queryDiv'>
        <form id='queryForm' method='post'>
        <table>
           {0}
        </table>
        </form>
    </div>";
                strQueryForm = string.Format(queryHtml, sbQuery);
            }
            else
            {
                queryHtml = @"
<div id='queryWindow' class='mini-window' title='详细查询' style='width: 690px; height: @{1}px;'>
    <div class='queryDiv'>
        <form id='queryForm' method='post'>
        <table>
           {0}
        </table>
        </form>
        <div>
            <a class='mini-button' onclick='search()' iconcls='icon-find' style='margin-right: 20px;'>查询</a>
            <a class='mini-button' onclick='clearQueryForm()' iconcls='icon-undo'>清空</a>
        </div>
    </div>
</div>";
                strQueryForm = string.Format(queryHtml, sbQuery, 100 + 22 * (queryFields.Count() / 2));
            }
            #endregion

            #region Bar条

            StringBuilder sbButtons = new StringBuilder();
            foreach (var item in buttons)
            {
                string onclick = "";
                if (item.ContainsKey("URL") && !string.IsNullOrEmpty(item["URL"].ToString()))
                {
                    onclick = "onclick='openWindow(\"" + item["URL"] + "&Sub_Key=" + tableName + "ID&Sub_ID={Sub_ID}\"";
                    if (item.ContainsKey("Settings"))
                    {
                        var sets = JsonHelper.ToObject(item["Settings"].ToString());
                        if (sets.ContainsKey("Field") && !string.IsNullOrEmpty(sets["Field"].ToString()))
                            continue;

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        if (sets.ContainsKey("PopupWidth") && sets["PopupWidth"].ToString() != "")
                            dic.Add("width", sets["PopupWidth"].ToString());
                        else
                            dic.Add("width", "1000");
                        if (sets.ContainsKey("PopupHeight") && sets["PopupHeight"].ToString() != "")
                            dic.Add("height", sets["PopupHeight"].ToString());
                        if (sets.ContainsKey("PopupTitle") && sets["PopupTitle"].ToString() != "")
                            dic.Add("title", sets["PopupTitle"].ToString());
                        if (sets.ContainsKey("Confirm") && sets["Confirm"].ToString() == "true")
                            dic.Add("mustConfirm", "true");
                        if (sets.ContainsKey("SelectMode") && sets["SelectMode"].ToString() != "")
                            dic.Add(sets["SelectMode"].ToString(), "true");
                        dic.Add("NavGrid", "navDataGrid");
                        dic.Add("Field", "ID");
                        onclick += "," + JsonHelper.ToJson(dic);

                        if (sets.ContainsKey("onclick") && !string.IsNullOrWhiteSpace(sets["onclick"].ToString())) //如果有自定义的按钮onclick
                            onclick = "";
                    }
                    else
                    {
                        onclick += ",{width:1000}";
                    }

                    if (onclick != "")
                        onclick += ");'";
                }

                sbButtons.AppendFormat("\n<a class='mini-button' {0} {1} {2}></a>"
                    , GetMiniuiSettings(item)
                    , onclick
                    , item.ContainsKey("Settings") ? GetMiniuiSettings(item["Settings"].ToString()) : "");
            }

            StringBuilder sb = new StringBuilder();
            string strBar = "";
            string strEmptyText = @"<div id='divDes' style='font-weight: bold; color:#000; text-align: center; background-color:#eee; position:absolute;  height:100%; width: 100%;'>
                <div style='position:absolute; top:48%;text-align:center;width:100%; font-size:14px;'>
                    请选择一条记录!
                </div>
                </div>
            ";
            if (showQueryForm)
            {
                strBar = @"
<div class='mini-toolbar gw-grid-toolbar' id='toolbar' style='padding: 0px 0px 0px 0px;display: none;height:60px!important'>
     <table style='border-bottom: 1px solid #ccc;'>
        <tr>
            <td style='font-weight: bold;padding-left: 10px;height: 20px;line-height: 20px;font-size: 12px;color: #333;'>
                {1}
            </td>
            <td class='gw-toolbar-right'>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                {1}
            </td>
            <td>
                {0}
            </td>
            <td class='gw-toolbar-right'>
            <a class='mini-button' onclick='search()' iconcls='icon-find'>查询</a>
            <a class='mini-button' onclick='clearQueryForm()' iconcls='icon-undo'>清空</a>
            </td>
        </tr>
    </table>
</div>";
                strBar = string.Format(strBar
                       , sbButtons, layout.Name
                       );
            }
            else
            {
                strBar = @"
<div class='mini-toolbar gw-grid-toolbar' id='toolbar' style='padding: 0px 0px 0px 0px;display: none;height:60px!important'>
     <table style='border-bottom: 1px solid #ccc;'>
        <tr>
            <td style='font-weight: bold;padding-left: 10px;height: 20px;line-height: 20px;font-size: 12px;color: #333;'>
                {3}
            </td>
            <td class='gw-toolbar-right'>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                {0}
            </td>
            <td class='gw-toolbar-right'>
            {1}
            {2}
            </td>
        </tr>
    </table>
</div>";
                string strQuickQueryBox = string.Format("<input id='key' class='mini-buttonedit gw-searchbox' emptytext='请输入{0}' onenter=\"quickSearch('{1}');\" onbuttonclick=\"quickSearch('{1}');\" />"
                    , string.Join("或", quickQueryFields.Select(c => c["header"].ToString()).ToArray())
                    , string.Join(",", quickQueryFields.Select(c => c["field"].ToString()).ToArray())
                    );
                string strSearchButton = "<a class='mini-button' onclick=\"showWindow('queryWindow');\" iconcls='icon-find'>详细查询</a>";

                strBar = string.Format(strBar
                    , sbButtons
                    , quickQueryFields.Count() > 0 ? strQuickQueryBox : ""
                    , queryFields.Count() > 0 ? strSearchButton : "", layout.Name
                    );
            }

            #endregion

            #region Grid

            string strField = "";
            StringBuilder sbField = new StringBuilder();
            bool multi = false;
            foreach (var field in fields)
            {
                string header = field["header"].ToString();
                var LGID = FormulaHelper.GetCurrentLGID();
                if (field.ContainsKey("header" + LGID))
                    header = field["header" + LGID].ToString();
                if (header.Contains("."))
                {
                    multi = true;
                    break;
                }
                sbField.AppendFormat("<div name='{3}' {0} {1} header='{2}'></div>", GetMiniuiSettings(field)
                    , field.ContainsKey("Settings") ? GetMiniuiSettings(field["Settings"].ToString()) : ""
                    , header, field["field"].ToString());
            }
            if (multi)
            {
                fields.RemoveWhere(c => c["Visible"].ToString() == "false");
                strField = CreateListColumn("", fields);
            }
            else
            {
                strField = sbField.ToString();
            }




            string strGrid = @"
<div class='mini-fit' id='divGrid' style='display: none;height:calc(100% - 60px);'>
    <div id='dataGrid' class='mini-datagrid' style='width: 100%; height: 100%;' url='GetList' {0} onDrawSummaryCell='onDrawSummaryCell'>
        <div property='columns'>         
            {2}  
            {3}
            {1}
        </div>
    </div>
</div>";
            strGrid = string.Format(strGrid
                , GetMiniuiSettings(listDef.LayoutGrid)
                , strField //sbField
                , listDef.HasRowNumber == "1" ? "<div type='indexcolumn' headerAlign='center'>序号</div>" : ""
                , listDef.HasCheckboxColumn != "0" ? "<div type='checkcolumn'></div>" : ""
                );

            #endregion

            if (showQueryForm)
            {
                return strEmptyText + "\n" + strBar + "\n" + strQueryForm + "\n" + strGrid + "\n" + createExportExcelbtn(code);
            }
            else
            {
                return strEmptyText + "\n" + strBar + "\n" + strGrid + "\n" + strQueryForm + "\n" + createExportExcelbtn(code);
            }
        }


        private string CreateListColumn(string fieldName, List<Dictionary<string, object>> fields)
        {
            if (fields.Count == 0)
                return "";

            if (fields.Count == 1)
            {
                var field = fields[0];
                return string.Format("<div name='{3}' field='{2}' {0} {1}></div>", GetMiniuiSettings(field)
                     , field.ContainsKey("Settings") ? GetMiniuiSettings(field["Settings"].ToString()) : ""
                     , field["field"], field["field"]);
            }

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(fieldName))
                sb.AppendFormat("<div header='{0}' headerAlign='center'><div property='columns'>", fieldName);
            sb.AppendLine();
            while (fields.Count > 0)
            {
                var field = fields[0];
                string name = field["header"].ToString().Split('.').First();
                var newFields = fields.Where(c => c["header"].ToString().StartsWith(name + ".") || c["header"].ToString() == name).ToList();
                fields.RemoveWhere(c => c["header"].ToString().StartsWith(name + ".") || c["header"].ToString() == name);

                foreach (var item in newFields)
                {
                    item["header"] = item["header"].ToString().Replace(name + ".", "");
                }
                sb.AppendLine();
                string str = CreateListColumn(name, newFields);
                sb.AppendFormat(str);
            }
            sb.AppendLine();
            if (!string.IsNullOrEmpty(fieldName))
                sb.AppendFormat("</div></div>");
            return sb.ToString();
        }

        public string CreateScript(string code, bool isOutput = false)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var listDef = entities.Set<S_UI_Layout>().SingleOrDefault(c => c.Code == code);
            if (listDef == null)
                throw new Exception(string.Format("编号为：“{0}”的列表不存在", code));

            StringBuilder sb = new StringBuilder();

            var fields = JsonHelper.ToList(listDef.LayoutField);

            #region 字段详细
            foreach (var field in fields)
            {
                if (!field.ContainsKey("Settings"))
                    continue;
                var settings = JsonHelper.ToObject(field["Settings"].ToString());

                if (!settings.ContainsKey("EnumKey") || settings["EnumKey"].ToString() == "")
                    continue;

                string enumKey = settings["EnumKey"].ToString();

                string tableName = listDef.TableNames.Split(',')[0];
                var key = GetEnumKey(listDef.ConnName, tableName, field["field"].ToString(), enumKey);
                string enumStr = GetEnumString(listDef.ConnName, tableName, field["field"].ToString(), enumKey);

                if (isOutput)
                    sb.AppendFormat(GetOutputEnumString(listDef.ConnName, tableName, field["field"].ToString(), enumKey));
                else
                {
                    if (string.IsNullOrEmpty(enumStr))
                        enumStr = "[]";
                    sb.AppendFormat("\n var {0} = {1}", key, enumStr);
                }

                sb.AppendFormat("\n addGridEnum('dataGrid', '{0}', '{1}');", field["field"], key);
            }

            #endregion

            #region 按钮
            sb.AppendLine();
            var buttons = JsonHelper.ToList(listDef.LayoutButton);
            foreach (var item in buttons)
            {
                if (item.ContainsKey("Settings"))
                {
                    var sets = JsonHelper.ToObject(item["Settings"].ToString());
                    if (!sets.ContainsKey("Field") || string.IsNullOrEmpty(sets["Field"].ToString()))
                        continue;

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    var displayContent = "";
                    if (sets.ContainsKey("DisplayContent") && sets["DisplayContent"].ToString() != "")
                        displayContent = sets["DisplayContent"].ToString();
                    if (displayContent == "buttonIcon")
                        dic.Add("buttonClass", item["iconcls"]);
                    else if (displayContent == "buttonName")
                        dic.Add("linkText", item["text"]);

                    string addStr = "";
                    if (sets.ContainsKey("onclick") && !string.IsNullOrEmpty(sets["onclick"].ToString()))
                    {
                        dic.Add("onButtonClick", sets["onclick"].ToString());
                        addStr = string.Format("\n addGridButton('dataGrid','{0}',{1});", sets["Field"], JsonHelper.ToJson(dic));
                    }
                    else
                    {
                        if (sets.ContainsKey("PopupWidth") && sets["PopupWidth"].ToString() != "")
                            dic.Add("width", sets["PopupWidth"].ToString());
                        else
                            dic.Add("width", "1000");
                        if (sets.ContainsKey("PopupHeight") && sets["PopupHeight"].ToString() != "")
                            dic.Add("height", sets["PopupHeight"].ToString());
                        if (sets.ContainsKey("PopupTitle") && sets["PopupTitle"].ToString() != "")
                            dic.Add("title", sets["PopupTitle"].ToString());
                        if (sets.ContainsKey("Confirm") && sets["Confirm"].ToString() == "true")
                            dic.Add("mustConfirm", "true");
                        if (sets.ContainsKey("SelectMode") && sets["SelectMode"].ToString() != "")
                            dic.Add(sets["SelectMode"].ToString(), "true");

                        addStr = string.Format("\n addGridLink('dataGrid','{0}','{1}',{2});", sets["Field"], item["URL"], JsonHelper.ToJson(dic));
                    }

                    sb.Append(addStr);
                }

            }

            #endregion

            #region 详细查询
            sb.AppendLine();
            foreach (var field in fields)
            {
                if (!field.ContainsKey("ItemType") || field["ItemType"].ToString() != "ButtonEdit")
                    continue;

                if (!field.ContainsKey("QuerySettings"))
                    continue;
                string mode = field.ContainsKey("QueryMode") ? field["QueryMode"].ToString() : "";
                string queryMode = getQueryMode(mode);
                string name = string.Format("${0}${1}", queryMode, field["field"]);

                var dic = JsonHelper.ToObject<Dictionary<string, string>>(field["QuerySettings"].ToString());

                if (dic == null)
                    throw new BusinessException(string.Format("详细查询配置错误：{0}", field["field"]));

                if (dic["SelectorKey"].ToString() == "SystemUser")
                {
                    string rtnParam = "";
                    if (dic.ContainsKey("ReturnParams") && dic["ReturnParams"].ToString() != "")
                        rtnParam = ",{ returnParams:'" + dic["ReturnParams"].ToString() + "'}";

                    if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                        sb.AppendFormat("addMultiUserSelector('{0}'{1});\n", name, rtnParam);
                    else
                        sb.AppendFormat("addSingleUserSelector('{0}'{1});\n", name, rtnParam);
                }
                else if (dic["SelectorKey"].ToString() == "SystemOrg")
                {
                    string rtnParam = "";
                    if (dic.ContainsKey("ReturnParams") && dic["ReturnParams"].ToString() != "")
                        rtnParam = ",{ returnParams:'" + dic["ReturnParams"].ToString() + "'}";

                    if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                        sb.AppendFormat("addMultiOrgSelector('{0}'{1});\n", name, rtnParam);
                    else
                        sb.AppendFormat("addSingleOrgSelector('{0}'{1});\n", name, rtnParam);
                }
                else
                {
                    //sb.AppendFormat("addSelector('{0}', '/MvcConfig/UI/Selector/PageView?TmplCode={1}', {2});\n"
                    //    , name, dic["SelectorKey"], "{ allowResize: true,title:'请选择' }");

                    string selectorKey = dic["SelectorKey"];
                    var selector = entities.S_UI_Selector.SingleOrDefault(c => c.Code == selectorKey);
                    if (selector != null)
                    {
                        var url = selector.URLSingle;
                        if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                            url = selector.URLMulti;
                        if (dic.ContainsKey("ReturnParams") && dic["ReturnParams"].ToString() != "")
                            sb.AppendFormat("addSelector('{0}', '{1}', {{ allowResize: true,title:'{2}',width:'{3}',height:'{4}',returnParams:'{5}' }});\n"
                                , name, url, selector.Title, selector.Width, selector.Height, dic["ReturnParams"]);
                        else
                            sb.AppendFormat("addSelector('{0}', '{1}', {{ allowResize: true,title:'{2}',width:'{3}',height:'{4}' }});\n"
                                , name, url, selector.Title, selector.Width, selector.Height);
                    }

                }


            }

            #endregion

            return sb.ToString() + "\n" + HttpContext.Current.Server.HtmlDecode(listDef.ScriptText);

        }

        private string GetQueryItemHtml(S_UI_Layout listDef, Dictionary<string, object> field, bool showQueryForm = false)
        {
            string mode = field.ContainsKey("QueryMode") ? field["QueryMode"].ToString() : "";
            string queryMode = getQueryMode(mode);

            string code = field["field"].ToString();
            string name = field["header"].ToString();
            var LGID = FormulaHelper.GetCurrentLGID();
            if (field.ContainsKey("header" + LGID)) //双语言支持
                name = field["header" + LGID].ToString();

            string miniCls = field["ItemType"].ToString().ToLower();
            string dataPty = "";
            if (field.ContainsKey("Settings"))
            {
                var settings = JsonHelper.ToObject(field["Settings"].ToString());
                if (settings.ContainsKey("EnumKey"))
                {
                    dataPty = string.Format(" data='{0}' ", GetEnumKey(listDef.ConnName, listDef.TableNames.Split(',')[0], code, settings["EnumKey"].ToString()));
                }
            }

            string html = "";

            string miniuiSettings = GetMiniuiSettings(field.ContainsKey("QuerySettings") ? field["QuerySettings"].ToString() : "{}");

            html = string.Format("<input name=\"${0}${1}\" class=\"mini-{2}\" {3} {4} style='width:100%' />"
   , queryMode, code, miniCls, miniuiSettings, dataPty);


            if (field["QueryMode"].ToString() == "Between")
            {
                html = string.Format("<input name=\"${0}${1}\" class=\"mini-{2}\" {4} style='width:45%'/>&nbsp;-&nbsp;<input name=\"${3}${1}\" class=\"mini-{2}\" {4} style='width:45%'/>"
                  , "FR", code, miniCls, "TO", miniuiSettings);
            }

            if (showQueryForm)
            {
                return string.Format("<td >{0}</td><td  nowrap=\"nowrap\" style=\"padding-right:40px;\">{1}</td>", name, html);
            }
            else
            {
                return string.Format("<td width=\"15%\">{0}</td><td width=\"35%\" nowrap=\"nowrap\">{1}</td>", name, html);
            }
        }

        private string getQueryMode(string mode)
        {
            string queryMode = "";
            switch (mode)
            {
                case "Equal":
                    queryMode = "EQ";
                    break;
                case "NotEqual":
                    queryMode = "UQ";
                    break;
                case "GreaterThan":
                    queryMode = "GT";
                    break;
                case "LessThan":
                    queryMode = "LT";
                    break;
                case "In":
                    queryMode = "IN";
                    break;
                case "GreaterThanOrEqual":
                    queryMode = "FR";
                    break;
                case "LessThanOrEqual":
                    queryMode = "TO";
                    break;
                case "Like":
                    queryMode = "LK";
                    break;
                case "InLike":
                    queryMode = "IL";
                    break;
                case "StartsWith":
                    queryMode = "SW";
                    break;
                case "EndsWith":
                    queryMode = "EW";
                    break;
                case "IGNORE":
                    queryMode = "";
                    break;
                default:
                    queryMode = "LK";
                    break;
            }
            return queryMode;
        }

        private string createExportExcelbtn(string code)
        {
            StringBuilder sb = new StringBuilder();

            string includeColumns = "";
            string excelKey = "";
            string gridId = "dataGrid";
            //string text = "导出";

            includeColumns = !string.IsNullOrWhiteSpace(includeColumns) && !includeColumns.EndsWith(",") ? includeColumns + "," : includeColumns;
            excelKey = code;
            //var btnExportHTML = "<a id='btnExport' class='mini-button' iconcls='icon-excel-export' plain='true' onclick=\"ExportExcel('{0}', '{1}', '{2}')\">{3}</a>";

            var strFormHTML = @"    
    <!--导出Excel——模拟异步ajax提交表单 -->
    <form id='excelForm{0}' style='display:none;' action='/MvcConfig/Aspose/ExportExcel' method='post' target='excelIFrame{0}'>
        <input type='hidden' name='jsonColumns' />
        <input type='hidden' name='title' />
        <input type='hidden' name='excelKey' />
        <input type='hidden' name='queryFormData' />
        <input type='hidden' name='quickQueryFormData' />
        <input type='hidden' name='queryTabData' />
        <input type='hidden' name='sortOrder' />
        <input type='hidden' name='sortField' />
        <input type='hidden' name='pageSize' />
        <input type='hidden' name='pageIndex' />
        <input type='hidden' name='exportCurrentPage' />
        <input type='hidden' name='dataUrl' />
        <input type='hidden' name='referUrl' />
    </form>
    <iframe id='excelIFrame{0}' name='excelIFrame{0}' style='display:none;'></iframe>";

            sb.AppendLine(string.Format(strFormHTML, excelKey));

            var strExcelWindowHTML = @"
<!--导出Excel——自定义删选字段-->
<div id='excelWindow{2}' class='mini-window' title='导出数据' style='width: 262px; height: 280px; display:none;'
    showmodal='true' allowresize='false' allowdrag='true'>
    <div id='gridColumns{2}' class='mini-listbox' style='width: 250px; height: 200px;' showcheckbox='true'
        multiselect='true' textfield='ChineseName' valuefield='FieldName'>
    </div>
    <div style='float: right; padding-top: 6px;'>
        <a class='mini-button' iconcls='icon-excel' plain='false' onclick='{0}'>
            导出</a>
        <a class='mini-button' iconcls='icon-cancel' plain='false' onclick='{1}'>
            取消</a>
    </div>
</div>";

            sb.AppendLine(string.Format(strExcelWindowHTML,
                string.Format("downloadExcelData(\"{0}\",\"{1}\");", excelKey, gridId), string.Format("closeExcelWindow(\"{0}\")", excelKey), excelKey));

            //sb.AppendLine(string.Format(btnExportHTML, excelKey, gridId, includeColumns.ToLower(), text));

            return sb.ToString();
        }

        #endregion

        #region 获取枚举字符串

        private string GetOutputEnumString(string connName, string tableName, string fieldCode, string data)
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
            if (data.StartsWith("[") == true)
            {
                result = string.Format("\n var enum_{0}_{1} = {2}; ", tableName, fieldCode, data);
            }
            else if (data == "" || data == "FromMeta" || fromMeta == true)
            {
                result = string.Format("\n @Html.GetMetaEnum('{0}','{1}','{2}')", connName, tableName, fieldCode);
            }
            else
            {
                result = string.Format("\n @Html.GetEnum('{0}')", data);
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

        private string GetFormItemDataPty(string tableName, string fieldCode, string settings)
        {
            if (string.IsNullOrEmpty(settings))
                return "";

            var dic = JsonHelper.ToObject<Dictionary<string, string>>(settings);
            string data = "";
            if (dic.ContainsKey("data"))
                data = dic["data"];
            return string.Format(" data='{0}'", GetEnumKey("", tableName, fieldCode, data));
        }

        #endregion

        #region Settings处理

        public string GetMiniuiSettings(string settings)
        {
            if (string.IsNullOrEmpty(settings))
                return "";
            var dic = JsonHelper.ToObject(settings);
            return GetMiniuiSettings(dic);
        }

        public string GetMiniuiSettings(Dictionary<string, object> dic)
        {
            if (dic.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            string style = "";
            foreach (string key in dic.Keys)
            {
                if (key != "Enabled" && key != "Visible" && key.StartsWith(key.Substring(0, 1).ToUpper())) //以大写字符开头的不处理
                    continue;
                var LGID = FormulaHelper.GetCurrentLGID();
                if (key == "header" && !string.IsNullOrEmpty(LGID) && dic.Keys.Contains("Header" + LGID)) //列表定义多语言列表header
                {
                    sb.AppendFormat(" header='{0}'", dic["Header" + LGID]);
                }
                else if (key == "text" && !string.IsNullOrEmpty(LGID) && dic.Keys.Contains("Text" + LGID)) //列表定义多语言列表header
                {
                    sb.AppendFormat(" text='{0}'", dic["Text" + LGID]);
                }
                else if (dic[key] != null && dic[key].ToString() != "")
                {
                    if (key == "data")
                    {
                        //if (dic[key].ToString().StartsWith("["))
                        //    sb.AppendFormat(" {0}='{1}'", key, dic[key]);
                        //else
                        //    sb.AppendFormat(" {0}='{1}'", key, dic[key].ToString().Split('.').Last()); //以逗号隔开的枚举名
                    }
                    else if (key == "readonly")
                    {
                        if (dic[key].ToString() == "true")
                            sb.AppendFormat(" {0}='{1}'", key, dic[key]);
                    }
                    else
                    {
                        if (key.StartsWith("style_"))
                            style += string.Format("{0}:{1};", key.Split('_')[1], dic[key]);
                        else
                            sb.AppendFormat(" {0}='{1}'", key.ToLower(), dic[key]);
                    }
                }
            }

            string result = sb.ToString();
            if (!string.IsNullOrEmpty(style))
                result += " style='" + style + "'";
            return result;
        }


        #endregion

        #region 获取拼音首字符

        public string GetHanZiPinYinString(string name)
        {
            List<string> list = new List<string>();
            var arr = name.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                var c = arr[i];
                if (c >= '0' && c <= '9')
                {
                    list.Add(c.ToString());
                }
                else if (c >= 'a' && c <= 'z')
                {
                    list.Add(c.ToString());
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    list.Add(c.ToString());
                }
                else if (c == '_')
                {
                    list.Add(c.ToString());
                }
                else
                {
                    list.Add(getHanZiPin(c.ToString()));
                }
            }

            string result = string.Join("", list);
            result = result.Replace(" ", "");
            return result;
        }

        private string getHanZiPin(string c)
        {
            if ("()（），。、；,.;".Contains(c))
                return "";

            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            if (array.Length == 1)
                return "";
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));

            if (i < 0xB0A1) return "$";
            if (i < 0xB0C5) return "A";
            if (i < 0xB2C1) return "B";
            if (i < 0xB4EE) return "C";
            if (i < 0xB6EA) return "D";
            if (i < 0xB7A2) return "E";
            if (i < 0xB8C1) return "F";
            if (i < 0xB9FE) return "G";
            if (i < 0xBBF7) return "H";
            if (i < 0xBFA6) return "J";
            if (i < 0xC0AC) return "K";
            if (i < 0xC2E8) return "L";
            if (i < 0xC4C3) return "M";
            if (i < 0xC5B6) return "N";
            if (i < 0xC5BE) return "O";
            if (i < 0xC6DA) return "P";
            if (i < 0xC8BB) return "Q";
            if (i < 0xC8F6) return "R";
            if (i < 0xCBFA) return "S";
            if (i < 0xCDDA) return "T";
            if (i < 0xCEF4) return "W";
            if (i < 0xD1B9) return "X";
            if (i < 0xD4D1) return "Y";
            if (i < 0xD7FA) return "Z";
            return "$";
        }


        //private string getHanZiPin(string str)
        //{
        //    if (str.CompareTo("吖") < 0) return "";
        //    if (str.CompareTo("八") < 0) return "A";
        //    if (str.CompareTo("嚓") < 0) return "B";
        //    if (str.CompareTo("咑") < 0) return "C";
        //    if (str.CompareTo("妸") < 0) return "D";
        //    if (str.CompareTo("发") < 0) return "E";
        //    if (str.CompareTo("旮") < 0) return "F";
        //    if (str.CompareTo("铪") < 0) return "G";
        //    if (str.CompareTo("讥") < 0) return "H";
        //    if (str.CompareTo("咔") < 0) return "J";
        //    if (str.CompareTo("垃") < 0) return "K";
        //    if (str.CompareTo("嘸") < 0) return "L";
        //    if (str.CompareTo("拏") < 0) return "M";
        //    if (str.CompareTo("噢") < 0) return "N";
        //    if (str.CompareTo("妑") < 0) return "O";
        //    if (str.CompareTo("七") < 0) return "P";
        //    if (str.CompareTo("亽") < 0) return "Q";
        //    if (str.CompareTo("仨") < 0) return "R";
        //    if (str.CompareTo("他") < 0) return "S";
        //    if (str.CompareTo("哇") < 0) return "T";
        //    if (str.CompareTo("夕") < 0) return "W";
        //    if (str.CompareTo("丫") < 0) return "X";
        //    if (str.CompareTo("帀") < 0) return "Y";
        //    if (str.CompareTo("咗") < 0) return "Z";
        //    return "";

        //    //byte[] arrCN = Encoding.Default.GetBytes(cnChar);
        //    //if (arrCN.Length > 1)
        //    //{
        //    //    int area = (short)arrCN[0];
        //    //    int pos = (short)arrCN[1];
        //    //    int code = (area << 8) + pos;
        //    //    int[] areacode = new int[] { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };

        //    //    for (int i = 0; i < 26; i++)
        //    //    {
        //    //        int max = 55290;
        //    //        if (i != 25) max = areacode[i + 1];
        //    //        if (areacode[i] <= code && code < max)
        //    //        {
        //    //            return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
        //    //        }
        //    //    }
        //    //    return "";
        //    //}

        //    //else return cnChar;

        //}

        #endregion

        #region String替换

        /// <summary>
        /// 替换{}内容为当前地址栏参数或当前人信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public string ReplaceString(string sql, DataRow row = null, Dictionary<string, string> dic = null, Dictionary<string, DataTable> dtDic = null)
        {
            if (string.IsNullOrEmpty(sql))
                return sql;

            var user = FormulaHelper.GetUserInfo();
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            string result = reg.Replace(sql, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');

                if (!string.IsNullOrEmpty(HttpContext.Current.Request[value]))
                    return HttpContext.Current.Request[value];

                if (dtDic != null && dtDic.Count > 0)
                {
                    var arr = value.Split('.');
                    if (arr.Length == 1)
                    {
                        if (dtDic.ContainsKey(value)) //默认值为整个表
                            return JsonHelper.ToJson(dtDic[value]);
                    }
                    else if (arr.Length == 2) //默认子编号名.字段名
                    {
                        if (dtDic.ContainsKey(arr[0]))
                        {
                            var dt = dtDic[arr[0]];
                            if (dt.Rows.Count > 0 && dt.Columns.Contains(arr[1]))
                            {
                                return dt.Rows[0][arr[1]].ToString();
                            }
                        }
                    }

                }
                if (row != null && row.Table.Columns.Contains(value))
                    return row[value].ToString();
                if (dic != null && dic.ContainsKey(value))
                    return dic[value];

                switch (value)
                {
                    case Formula.Constant.CurrentUserID:
                        return user.UserID;
                    case Formula.Constant.CurrentUserName:
                        return user.UserName;
                    case Formula.Constant.CurrentUserOrgID:
                        return user.UserOrgID;
                    case Formula.Constant.CurrentUserOrgCode:
                        return user.UserOrgCode;
                    case Formula.Constant.CurrentUserOrgName:
                        return user.UserOrgName;
                    case Formula.Constant.CurrentUserPrjID:
                        return user.UserPrjID;
                    case Formula.Constant.CurrentUserPrjName:
                        return user.UserPrjName;
                    case "CurrentTime":
                        return DateTime.Now.ToString();
                    case "CurrentDate":
                        return DateTime.Now.Date.ToString("yyyy-MM-dd");
                    case "CurrentYear":
                        return DateTime.Now.Year.ToString();
                    case "CurrentMonth":
                        return DateTime.Now.Month.ToString();
                    case "CurrentQuarter":
                        return ((DateTime.Now.Month + 2) / 3).ToString();
                    default:
                        return "";
                }
            });

            return result;
        }


        #endregion

        #region Word导出

        public DataSet GetWordDataSource(string code, string id)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var word = entities.Set<S_UI_Word>().SingleOrDefault(c => c.Code == code);
            if (word == null)
                throw new Exception(string.Format("编号为：“{0}”的Word导出不存在", code));

            if (word.Description == "FormWord")
                return GetFormWordDataSource(code, id); //表单定义的Word导出数据

            var enumService = FormulaHelper.GetService<IEnumService>();

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(word.ConnName);
            string sql = string.Format("select * from ({0}) a where ID='{1}'", word.SQL, id);
            DataTable dtForm = sqlHelper.ExecuteDataTable(sql);
            dtForm.TableName = "dtForm";
            if (dtForm.Rows.Count == 0)
                throw new Exception("不存在ID为：" + id + "的记录！");

            DataSet ds = new DataSet("dataSet1");
            ds.Tables.Add(dtForm);


            var items = JsonHelper.ToObject<List<Dictionary<string, string>>>(word.Items);
            foreach (var item in items)
            {
                string fieldCode = item["Code"];


                if (item["ItemType"] == "Datetime")
                {
                    object fieldValue = dtForm.Rows[0][fieldCode];
                    dtForm.Columns.Remove(fieldCode);
                    dtForm.Columns.Add(fieldCode);
                    if (fieldValue is DBNull)
                    {
                        dtForm.Rows[0][fieldCode] = "";
                    }
                    else
                    {
                        string format = "yyyy-MM-dd";
                        var settings = new Dictionary<string, string>();
                        if (item.ContainsKey("Settings"))
                            settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);
                        if (settings.ContainsKey("format"))
                            format = settings["format"];

                        dtForm.Rows[0][fieldCode] = Convert.ToDateTime(fieldValue).ToString(format);
                    }

                }
                else if (item["ItemType"] == "Float")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);

                    object fieldValue = dtForm.Rows[0][fieldCode];
                    dtForm.Columns.Remove(fieldCode);
                    dtForm.Columns.Add(fieldCode);
                    if (fieldValue is DBNull)
                    {
                        dtForm.Rows[0][fieldCode] = "";
                    }
                    else
                    {
                        dtForm.Rows[0][fieldCode] = Convert.ToDouble(fieldValue).ToString(settings["format"]);
                    }
                }
                else if (item["ItemType"] == "Enum")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);
                    #region 处理枚举

                    IList<DicItem> enumData = GetEnum(word.ConnName, word.TableNames.Split(',')[0], item["Code"], settings["data"]);

                    if (settings["isCheckBox"] == "true")
                    {
                        StringBuilder sb = new StringBuilder();
                        var value = dtForm.Rows[0][fieldCode].ToString();

                        int repeatItems = 0;
                        if (settings.ContainsKey("repeatItems"))
                            repeatItems = int.Parse(settings["repeatItems"]);
                        for (int i = 0; i < enumData.Count; i++)
                        {
                            var d = enumData[i];
                            if (value.Split(',').Contains(d.Value))
                                sb.AppendFormat(" √{0}", d.Text);
                            else
                                sb.AppendFormat(" □{0}", d.Text);

                            if (repeatItems > 0 && (i + 1) % repeatItems == 0 && i + 1 < enumData.Count)
                                sb.AppendLine();

                        }
                        dtForm.Rows[0][fieldCode] = sb.ToString();
                    }
                    else
                    {
                        var value = dtForm.Rows[0][fieldCode].ToString().Split(',');
                        dtForm.Rows[0][fieldCode] = "";
                        foreach (var d in enumData)
                        {
                            if (value.Contains(d.Value))
                                dtForm.Rows[0][fieldCode] += "," + d.Text;
                        }
                        dtForm.Rows[0][fieldCode] = dtForm.Rows[0][fieldCode].ToString().Trim(',', ' ');
                    }
                    #endregion
                }
                else if (item["ItemType"] == "SubTable" || item["ItemType"] == "FieldTable")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, object>>(item["Settings"]);
                    var formData = JsonHelper.ToObject<Dictionary<string, string>>(settings["formData"].ToString());
                    var listData = JsonHelper.ToObject<List<Dictionary<string, string>>>(settings["listData"].ToString());

                    sql = ReplaceString(formData["SQL"], dtForm.Rows[0]);
                    DataTable subDT = sqlHelper.ExecuteDataTable(sql);

                    //最低子表行数
                    if (formData.ContainsKey("MinRowCount") && formData["MinRowCount"].ToString().Trim() != "")
                    {
                        var minRowCount = int.Parse(formData["MinRowCount"].ToString());
                        for (var i = subDT.Rows.Count; i < minRowCount; i++)
                        {
                            subDT.Rows.Add(subDT.NewRow());
                        }
                    }

                    #region 大字段表数据

                    if (item["ItemType"] == "FieldTable")
                    {
                        if (subDT.Rows.Count == 1)
                        {
                            string json = subDT.Rows[0][0].ToString();
                            if (json == "")
                                json = "[]";
                            var list = JsonHelper.ToObject<List<Dictionary<string, object>>>(json);
                            subDT.Columns.Clear();
                            subDT.Rows.Clear();
                            if (list.Count > 0)
                            {
                                foreach (var c in list[0].Keys)
                                {
                                    subDT.Columns.Add(c);
                                }
                                foreach (var r in list)
                                {
                                    var row = subDT.NewRow();
                                    foreach (var c in r.Keys)
                                    {
                                        if (row.Table.Columns.Contains(c))
                                        {
                                            row[c] = r[c];
                                        }
                                    }
                                    subDT.Rows.Add(row);
                                }

                            }
                        }
                    }

                    #endregion

                    if (!subDT.Columns.Contains("RowNumber"))
                    {
                        subDT.Columns.Add("RowNumber");
                        for (int i = 0; i < subDT.Rows.Count; i++)
                            subDT.Rows[i]["RowNumber"] = i + 1;
                    }

                    subDT.TableName = fieldCode;
                    ds.Tables.Add(subDT);


                    foreach (var subItem in listData)
                    {
                        string subFieldCode = subItem["Code"];
                        if (subDT.Columns.Contains(subFieldCode) == false)
                            continue;

                        var subSettings = JsonHelper.ToObject<Dictionary<string, string>>(subItem["Settings"]);

                        if (subItem["ItemType"] == "Enum")
                        {
                            #region 处理枚举

                            if (subSettings["isCheckBox"] == "true")
                            {
                                foreach (DataRow subRow in subDT.Rows)
                                {
                                    string subValue = subRow[subFieldCode].ToString();
                                    if (subValue == "1" || subValue == "T" || subValue.ToLower() == "true")
                                        subRow[subFieldCode] = "√";
                                }
                            }
                            else
                            {
                                string tableName = word.TableNames.Split(',')[0] + "_" + item["Code"];
                                IList<DicItem> SubEnumData = GetEnum(word.ConnName, tableName, subFieldCode, subSettings["data"]);

                                foreach (DataRow subRow in subDT.Rows)
                                {
                                    string[] subValues = subRow[subFieldCode].ToString().Split(',');
                                    var v = string.Join(",", SubEnumData.Where(c => subValues.Contains(c.Value)).Select(c => c.Text).ToArray());
                                    if (v != "")
                                        subRow[subFieldCode] = v;
                                }
                            }
                            #endregion
                        }
                        else if (subItem["ItemType"] == "Datetime")
                        {
                            var values = subDT.AsEnumerable().Select(c => c[subFieldCode]).ToArray();
                            subDT.Columns.Remove(subFieldCode);
                            subDT.Columns.Add(subFieldCode);
                            for (int i = 0; i < subDT.Rows.Count; i++)
                            {
                                subDT.Rows[i][subFieldCode] = values[i] is DBNull ? "" : Convert.ToDateTime(values[i]).ToString(subSettings["format"]);
                            }
                        }
                        else if (subItem["ItemType"] == "Float")
                        {
                            var values = subDT.AsEnumerable().Select(c => c[subFieldCode]).ToArray();
                            subDT.Columns.Remove(subFieldCode);
                            subDT.Columns.Add(subFieldCode);
                            for (int i = 0; i < subDT.Rows.Count; i++)
                            {
                                subDT.Rows[i][subFieldCode] = values[i] is DBNull ? "" : Convert.ToDouble(values[i]).ToString(subSettings["format"]);
                            }
                        }
                    }

                }
            }

            #region word导出流程意见

            if (System.Configuration.ConfigurationManager.AppSettings["Flow_WordExportComment"] == "True")
            {
                var dtFlowComments = GetFlowComment(id);
                ds.Tables.Add(dtFlowComments);
            }

            #endregion

            return ds;
        }

        private DataTable GetFlowComment(string id)
        {
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.WorkFlow);
            string sqlFlowExecList = @"
select S_WF_InsTaskExec.ID as ID
,S_WF_InsTaskExec.CreateTime as CreateTime
,TaskUserID
,TaskUserName
,ExecUserID
,ExecUserName
,ExecTime
,ExecComment
,S_WF_InsTaskExec.Type as Type
,S_WF_InsTask.ID as TaskID
,TaskName
,TaskCategory
,TaskSubCategory
,SendTaskUserNames
,FlowName
,FlowCategory
,FlowSubCategory
,S_WF_InsDefStep.Name as StepName
,S_WF_InsDefStep.ID as StepID
,ExecRoutingIDs
,ExecRoutingName
,S_WF_InsFlow.InsDefFlowID
,S_WF_InsTask.DoBackRoutingID
,S_WF_InsTask.OnlyDoBack
,S_WF_InsTaskExec.Type
from S_WF_InsTaskExec
right join S_WF_InsTask on InsTaskID=S_WF_InsTask.ID
join S_WF_InsFlow on S_WF_InsTask.InsFlowId=S_WF_InsFlow.ID
join S_WF_InsDefStep on InsDefStepID=S_WF_InsDefStep.ID
where FormInstanceID='{0}' and (WaitingRoutings is null or WaitingRoutings='') and (WaitingSteps is null or WaitingSteps='')
order by isnull(S_WF_InsTaskExec.CreateTime,S_WF_InsTask.CreateTime),S_WF_InsTaskExec.ID
";
            sqlFlowExecList = string.Format(sqlFlowExecList, id);

            var dt = sqlHelper.ExecuteDataTable(sqlFlowExecList);
            dt.TableName = "FlowComments";


            string insDefFlowID = dt.Rows[0]["InsDefFlowID"].ToString();
            string sql = string.Format("select ID,Name from S_WF_InsDefRouting where InsDefFlowID='{0}'", insDefFlowID);
            DataTable dtRouting = sqlHelper.ExecuteDataTable(sql);

            dt.Columns.Add("UseTime");
            dt.Columns.Add("TaskUserDept");

            var userService = FormulaHelper.GetService<IUserService>();

            foreach (DataRow row in dt.Rows)
            {
                string ExecRoutingIDs = row["ExecRoutingIDs"].ToString().Trim(',');
                if (!string.IsNullOrEmpty(ExecRoutingIDs) && row["ExecRoutingName"].ToString() == "")
                {
                    row["ExecRoutingName"] = dtRouting.AsEnumerable().SingleOrDefault(c => c["ID"].ToString() == ExecRoutingIDs.Split(',').LastOrDefault())["Name"];
                }
                //处理打回和直送操作的名称
                if (string.IsNullOrEmpty(ExecRoutingIDs) && row["ExecRoutingName"].ToString() == "" && row["ExecTime"].ToString() != "")
                {
                    if (row["Type"].ToString() == "Normal" || row["Type"].ToString() == "Delegate")
                    {
                        if (row["DoBackRoutingID"].ToString() != "")
                            row["ExecRoutingName"] = "驳回";
                        if (row["OnlyDoBack"].ToString() == "1")
                            row["ExecRoutingName"] = "送驳回人";
                    }
                    else if (row["Type"].ToString() == "Circulate")
                    {
                        row["ExecRoutingName"] = "阅毕";
                    }
                    else if (row["Type"].ToString() == "Ask")
                    {
                        row["ExecRoutingName"] = "阅毕";
                    }

                }

                string CreateTime = row["CreateTime"].ToString();
                string ExecTime = row["ExecTime"].ToString();
                if (!string.IsNullOrEmpty(ExecTime))
                {
                    var span = DateTime.Parse(ExecTime) - DateTime.Parse(CreateTime);
                    row["UseTime"] = string.Format("{0}小时{1}分", span.Days * 24 + span.Hours, span.Minutes == 0 ? 1 : span.Minutes);
                }
                if (row["TaskUserID"].ToString() != "")
                {
                    row["TaskUserDept"] = userService.GetUserInfoByID(row["TaskUserID"].ToString()).UserOrgName;
                }
                else
                {
                    row["TaskUserName"] = "";
                    row["ExecUserName"] = "";
                }
                if (row["TaskUserID"].ToString() != "")
                {
                    row["TaskUserDept"] = userService.GetUserInfoByID(row["TaskUserID"].ToString()).UserOrgName;
                }
                else
                {
                    row["TaskUserName"] = "";
                    row["ExecUserName"] = "";
                }
                //操作意见取最新回复
                var entities = FormulaHelper.GetEntities<BaseEntities>();
                string routingID = row["ID"].ToString();
                string execComment = row["ExecComment"].ToString();
                var msgBody = entities.Set<S_S_MsgBody>().Where(c => c.FlowMsgID == routingID).OrderByDescending(c => c.SendTime);
                if (msgBody.Count() > 0)
                {
                    execComment = msgBody.First().Content;
                }
                row["ExecComment"] = execComment;
            }



            return dt;
        }

        /// <summary>
        /// 根据配置数据获取枚举
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IList<DicItem> GetEnum(string connName, string tableName, string fieldCode, string data)
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
                return JsonHelper.ToObject<List<DicItem>>(data);
            else if (data == "" || data == "FromMeta" || fromMeta == true)
            {
                var entities = FormulaHelper.GetEntities<BaseEntities>();
                var field = entities.Set<S_M_Field>().FirstOrDefault(c => c.Code == fieldCode && c.S_M_Table.Code == tableName && c.S_M_Table.ConnName == connName);
                if (field == null || string.IsNullOrEmpty(field.EnumKey))
                    return new List<DicItem>();
                else if (field.EnumKey.Trim().StartsWith("["))
                    return JsonHelper.ToObject<List<DicItem>>(field.EnumKey);
                else
                {
                    IEnumService enumService = FormulaHelper.GetService<IEnumService>();
                    result = JsonHelper.ToJson(enumService.GetEnumTable(field.EnumKey));
                    return enumService.GetEnumDataSource(field.EnumKey);
                }
            }
            else
            {
                IEnumService enumService = FormulaHelper.GetService<IEnumService>();
                return enumService.GetEnumDataSource(data);

            }
        }


        /// <summary>
        /// 表单定义的Word导出配置
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataSet GetFormWordDataSource(string code, string id)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var form = entities.Set<S_UI_Form>().SingleOrDefault(c => c.Code == code);
            if (form == null)
                throw new Exception(string.Format("编号为：“{0}”的表单不存在", code));

            var enumService = FormulaHelper.GetService<IEnumService>();

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(form.ConnName);
            string sql = string.Format("select * from {0} a where ID='{1}'", form.TableName, id);
            DataTable dtForm = sqlHelper.ExecuteDataTable(sql);
            dtForm.TableName = "dtForm";
            if (dtForm.Rows.Count == 0)
                throw new Exception("不存在ID为：" + id + "的记录！");

            DataSet ds = new DataSet("dataSet1");
            ds.Tables.Add(dtForm);


            var items = JsonHelper.ToObject<List<Dictionary<string, string>>>(form.Items);
            foreach (var item in items)
            {
                string fieldCode = item["Code"];

                if (item["ItemType"] == "ButtonEdit")
                {
                    dtForm.Rows[0][fieldCode] = dtForm.Rows[0][fieldCode + "Name"];
                }
                else if (item["ItemType"] == "AuditSign")
                {
                    string json = dtForm.Rows[0][fieldCode].ToString();
                    if (json == "")
                        json = "[]";
                    var list = JsonHelper.ToObject<List<Dictionary<string, object>>>(json);
                    DataTable subDT = new DataTable(fieldCode);
                    ds.Tables.Add(subDT);
                    if (list.Count > 0)
                    {
                        foreach (var c in list[0].Keys)
                        {
                            subDT.Columns.Add(c);
                        }
                        foreach (var r in list)
                        {
                            var row = subDT.NewRow();
                            foreach (var c in r.Keys)
                            {
                                row[c] = r[c];
                            }
                            subDT.Rows.Add(row);
                        }
                    }
                }
                else if (item["ItemType"] == "SingleFile" || item["ItemType"] == "MultiFile")
                {
                    string fileName = dtForm.Rows[0][fieldCode].ToString();
                    string result = "";
                    foreach (var fName in fileName.Split(','))
                    {
                        result += "\n" + fName.Substring(fName.IndexOf('_') + 1);
                    }
                    dtForm.Rows[0][fieldCode] = result.Trim('\n', ' ');
                }
                else if (item["ItemType"] == "DatePicker") //日期框
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);

                    object fieldValue = dtForm.Rows[0][fieldCode];
                    dtForm.Columns.Remove(fieldCode);
                    dtForm.Columns.Add(fieldCode);
                    if (fieldValue is DBNull)
                    {
                        dtForm.Rows[0][fieldCode] = "";
                    }
                    else
                    {
                        if (settings != null)
                            dtForm.Rows[0][fieldCode] = Convert.ToDateTime(fieldValue).ToString(settings["format"]);
                        else
                            dtForm.Rows[0][fieldCode] = Convert.ToDateTime(fieldValue).ToString("yyyy-MM-dd");
                    }
                }
                else if (item["ItemType"] == "CheckBox") //复选框
                {
                    var value = dtForm.Rows[0][fieldCode].ToString();
                    if (value == "1")
                        dtForm.Rows[0][fieldCode] = string.Format(" √{0}", item["Name"]);
                    else
                        dtForm.Rows[0][fieldCode] = string.Format(" □{0}", item["Name"]);
                }
                else if (item["ItemType"] == "CheckBoxList" || item["ItemType"] == "RadioButtonList")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);
                    #region 处理枚举

                    IList<DicItem> enumData = new List<DicItem>();
                    if (settings != null)
                        enumData = GetEnum(form.ConnName, form.TableName, fieldCode, settings["data"]);

                    StringBuilder sb = new StringBuilder();
                    var value = dtForm.Rows[0][fieldCode].ToString();
                    int repeatItems = 0;
                    if (settings.ContainsKey("repeatItems") && settings["repeatItems"] != "")
                        repeatItems = int.Parse(settings["repeatItems"]);
                    for (int i = 0; i < enumData.Count; i++)
                    {
                        var d = enumData[i];
                        if (value.Split(',').Contains(d.Value))
                            sb.AppendFormat(" √{0}", d.Text);
                        else
                            sb.AppendFormat(" □{0}", d.Text);

                        if (repeatItems > 0 && (i + 1) % repeatItems == 0 && i + 1 < enumData.Count)
                            sb.AppendLine();

                    }
                    dtForm.Rows[0][fieldCode] = sb.ToString();

                    #endregion
                }
                else if (item["ItemType"] == "ComboBox")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);

                    #region 处理枚举

                    IList<DicItem> enumData = GetEnum(form.ConnName, form.TableName, fieldCode, settings["data"]);

                    var value = dtForm.Rows[0][fieldCode].ToString();
                    foreach (var d in enumData)
                    {
                        if (d.Value == value)
                            dtForm.Rows[0][fieldCode] = d.Text;
                    }

                    #endregion
                }
                else if (item["ItemType"] == "SubTable")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, object>>(item["Settings"]);
                    var formData = JsonHelper.ToObject<Dictionary<string, string>>(settings["formData"].ToString());
                    var listData = JsonHelper.ToObject<List<Dictionary<string, string>>>(settings["listData"].ToString());

                    sql = string.Format("select * from {0}_{1} where {0}ID = '{2}'", form.TableName, fieldCode, id);
                    DataTable subDT = sqlHelper.ExecuteDataTable(sql);

                    if (!subDT.Columns.Contains("RowNumber"))
                    {
                        subDT.Columns.Add("RowNumber");
                        for (int i = 0; i < subDT.Rows.Count; i++)
                            subDT.Rows[i]["RowNumber"] = i + 1;
                    }

                    subDT.TableName = fieldCode;
                    ds.Tables.Add(subDT);


                    foreach (var subItem in listData)
                    {
                        string subFieldCode = subItem["Code"];
                        if (subDT.Columns.Contains(subFieldCode) == false)
                            continue;

                        var subSettings = JsonHelper.ToObject<Dictionary<string, string>>(subItem["Settings"] == "" ? "{}" : subItem["Settings"]);

                        if (subItem["ItemType"] == "ComboBox")
                        {
                            #region 处理枚举

                            IList<DicItem> SubEnumData = GetEnum(form.ConnName, form.TableName, fieldCode, subSettings["data"]);

                            foreach (DataRow subRow in subDT.Rows)
                            {
                                string[] subValues = subRow[subFieldCode].ToString().Split(',');
                                var v = string.Join(",", SubEnumData.Where(c => subValues.Contains(c.Value)).Select(c => c.Text).ToArray());
                                if (v != "")
                                    subRow[subFieldCode] = v;
                            }

                            #endregion
                        }
                        else if (subItem["ItemType"] == "DatePicker")
                        {
                            var values = subDT.AsEnumerable().Select(c => c[subFieldCode]).ToArray();
                            subDT.Columns.Remove(subFieldCode);
                            subDT.Columns.Add(subFieldCode);
                            for (int i = 0; i < subDT.Rows.Count; i++)
                            {
                                subDT.Rows[i][subFieldCode] = values[i] is DBNull ? "" : Convert.ToDateTime(values[i]).ToString(subSettings.ContainsKey("format") ? subSettings["format"] : "yyyy-MM-dd");
                            }
                        }
                        else if (subItem["ItemType"] == "CheckBox")
                        {
                            var values = subDT.AsEnumerable().Select(c => c[subFieldCode].ToString()).ToArray();
                            subDT.Columns.Remove(subFieldCode);
                            subDT.Columns.Add(subFieldCode);
                            for (int i = 0; i < subDT.Rows.Count; i++)
                            {
                                subDT.Rows[i][subFieldCode] = values[i] == "1" ? "√" : "";
                            }
                        }
                        else if (subItem["ItemType"] == "ButtonEdit")
                        {
                            for (int i = 0; i < subDT.Rows.Count; i++)
                            {
                                subDT.Rows[i][subFieldCode] = subDT.Rows[i][subFieldCode + "Name"];
                            }
                        }
                    }

                }
            }

            #region word导出流程意见

            if (System.Configuration.ConfigurationManager.AppSettings["Flow_WordExportComment"] == "True")
            {
                var dtFlowComments = GetFlowComment(id);
                ds.Tables.Add(dtFlowComments);
            }

            #endregion

            return ds;
        }

        #endregion

        public void TransferEnum(DataTable dt, string connName, string tableName, string fieldCode, string enumKey)
        {
            var enumDic = GetEnum(connName, tableName, fieldCode, enumKey);
            foreach (DataRow row in dt.Rows)
            {
                string enumValue = row[fieldCode].ToString();

                string text = "";
                foreach (var item in enumValue.Split(','))
                {
                    var obj = enumDic.FirstOrDefault(c => c.Value == item);
                    if (obj != null)
                        text += "," + obj.Text;
                    else
                        text += "," + enumValue;
                }
                row[fieldCode] = text.Trim(',', ' ');

            }
        }

        #region 获取固定宽度字段集合
        public List<string> GetFixedWidthFields(string code)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var listDef = entities.Set<S_UI_Layout>().SingleOrDefault(c => c.Code == code);
            if (listDef == null)
                throw new Exception(string.Format("编号为：“{0}”的列表不存在", code));
            var fields = JsonHelper.ToList(listDef.LayoutField);
            Regex reg = new Regex(@"^[1-9]\d*$");
            List<string> list = new List<string>();
            foreach (var field in fields)
            {
                if (field.Keys.ToList().Exists(o => o.ToLower() == "width") && reg.IsMatch(field["width"].ToString()))
                {
                    list.Add(field["field"].ToString().ToUpper());
                }
            }
            return list;
        }
        #endregion

        #region 发布

        public void ReleaseForm(string id)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == id);
            var items = JsonHelper.ToObject<List<FormItem>>(entity.Items ?? "[]");
            string sql = CreateReleaseFormSql(id);

            if (Config.Constant.IsOracleDb)
            {
                sql = string.Format(@"
declare  flagExist number;
begin
{0}
end;
", sql);
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(entity.ConnName);
            sqlHelper.ExecuteNonQuery(sql);

            #region //更新数据字典

            MetaFO metaFO = FormulaHelper.CreateFO<MetaFO>();
            metaFO.ImportTable(entity.ConnName, entity.TableName);
            foreach (var item in items)
            {
                if (item.ItemType == "SubTable")
                {
                    metaFO.ImportTable(entity.ConnName, entity.TableName + "_" + item.Code);
                }
            }

            #endregion
        }

        public List<FormItem> GetUseFields(string categoryID)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            List<FormItem> lists = new List<FormItem>();
            var forms = entities.Set<S_UI_Form>().Where(c => c.CategoryID == categoryID);
            if (forms.Count() > 0)
            {
                foreach (var form in forms)
                {
                    string items = form.Items;
                    List<FormItem> formItems = JsonHelper.ToObject<List<FormItem>>(items);
                    foreach (var formItem in formItems)
                    {
                        if (lists.Where(c => c.Code == formItem.Code).Count() == 0)
                        {
                            formItem.Code = FilterSqlKeys(formItem.Code);
                            lists.Add(formItem);
                        }
                    }
                }
            }

            return lists;
        }

        public string FilterSqlKeys(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            value = Regex.Replace(value, @";", string.Empty);
            value = Regex.Replace(value, @"'", string.Empty);
            value = Regex.Replace(value, @"&", string.Empty);
            value = Regex.Replace(value, @"%20", string.Empty);
            value = Regex.Replace(value, @"--", string.Empty);
            value = Regex.Replace(value, @"==", string.Empty);
            value = Regex.Replace(value, @" <", string.Empty);
            value = Regex.Replace(value, @">", string.Empty);
            value = Regex.Replace(value, @"%", string.Empty);

            return value.Replace("(", "").Replace(")", "");
        }



        public string DeleteField(string connName, string tableName, List<FormItem> items)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
            string sql = string.Format("select * from syscolumns where id=object_id('{0}')", tableName);
            DataTable dtForm = sqlHelper.ExecuteDataTable(sql);
            StringBuilder sb = new StringBuilder();

            string[] fields = { "ID", "CreateDate", "ModifyDate", "CreateUserID", "CreateUser", "ModifyUserID", "ModifyUser", "OrgID", "CompanyID", "FlowPhase", "FlowInfo", "StepName" };
            DataTable fkTable = sqlHelper.ExecuteDataTable(string.Format(@"select
                object_name(b.parent_object_id) as tableName
                from sys.foreign_keys A
                inner join sys.foreign_key_columns B on A.object_id=b.constraint_object_id
                inner join sys.columns C on B.parent_object_id=C.object_id and B.parent_column_id=C.column_id 
                inner join sys.columns D on B.referenced_object_id=d.object_id and B.referenced_column_id=D.column_id 
                where object_name(B.referenced_object_id)='{0}'", tableName));

            foreach (DataRow dr in dtForm.Rows)
            {
                string fieldName = dr["name"].ToString();
                string field = fields.FirstOrDefault(c => c == fieldName);
                if (items.Where(c => c.Code == fieldName).Count() == 0 && string.IsNullOrEmpty(field))
                {
                    var table = fkTable.Select(string.Format(" tableName like '%{0}'", fieldName));
                    if (table.Count() > 0)
                    {
                        sb.AppendFormat(@"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))
                            BEGIN
                                {0}
                            END
                        ", table[0]["tableName"]);
                    }
                    if (!fieldName.Contains("ID"))
                    {
                        sb.AppendFormat(@"if exists(select * from syscolumns where id=object_id('{0}') and name='{1}')
                            ALTER TABLE {0} DROP COLUMN {1}
                    ", tableName, fieldName);
                    }
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
                sb.AppendFormat(@"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{1}]') AND type in (N'U'))
                    BEGIN
                        {0}
                    END
                ", sb.ToString(), tableName);
            return sb.ToString();
        }

        public string CreateReleaseFormSql(string id)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == id);
            var items = JsonHelper.ToObject<List<FormItem>>(entity.Items);
            StringBuilder sb = new StringBuilder();
            sb.Append(DeleteField(entity.ConnName, entity.TableName, items));
            sb.Append(CreateTable(entity.TableName, entity.Name));

            foreach (var item in items)
            {
                sb.Append(CreateField(entity.TableName, item.Code, item.FieldType, item.Name));

                #region 同时处理ComboBox、ButtonEdit和SubTable
                if (item.ItemType == "ButtonEdit")
                    sb.Append(CreateField(entity.TableName, item.Code + "Name", item.FieldType, item.Name + "名称"));
                else if (item.ItemType == "ComboBox")
                {
                    var settings = JsonHelper.ToObject(item.Settings ?? "{}");
                    if (settings.ContainsKey("textName") && settings["textName"].ToString() != "")
                        sb.Append(CreateField(entity.TableName, settings["textName"].ToString(), item.FieldType, item.Name + "名称"));
                }
                else if (item.ItemType == "SubTable")
                {
                    var subTableItems = getSubTableItem(item.Settings);
                    sb.Append(CreateSubTable(entity.TableName, entity.TableName + "_" + item.Code, item.Name, subTableItems));
                }
                #endregion
            }

            return sb.ToString();
        }

        #region 子表创建

        public string CreateSubTable(string mainTableName, string tableName, string tableDesc, List<FormItem> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CreateSubTable(mainTableName, tableName, tableDesc));

            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.FieldType))
                    continue;
                sb.Append(CreateField(tableName, item.Code, item.FieldType, item.Name));
                if (item.ItemType == "ButtonEdit")
                    sb.Append(CreateField(tableName, item.Code + "Name", item.FieldType, item.Name + "名称"));
                else if (item.ItemType == "SingleFile")
                    sb.Append(CreateField(tableName, item.Code + "Name", item.FieldType, item.Name + "名称"));
            }

            return sb.ToString();
        }

        #endregion

        #region 数据库操作方法

        #region 创建表

        public string CreateTable(string tableName, string tableDesc)
        {
            if (Config.Constant.IsOracleDb)
            {
                string sql = @"
execute immediate
'CREATE TABLE {0} (
  ID NVARCHAR2(50) NOT NULL,
  CreateDate TIMESTAMP(3) NULL,
  ModifyDate TIMESTAMP(3) NULL,
  CreateUserID NVARCHAR2(50) NULL,
  CreateUser NVARCHAR2(50) NULL,
  ModifyUserID NVARCHAR2(50) NULL,
  ModifyUser NVARCHAR2(50) NULL,
  OrgID NVARCHAR2(50) NULL,
  CompanyID NVARCHAR2(50) NULL,
  FlowPhase NVARCHAR2(50) NULL,
  FlowInfo NTEXT NULL,
  StepName NVARCHAR2(500) NULL)
  STORAGE ( 
    NEXT 1048576 )';
commit;
execute immediate
'CREATE UNIQUE INDEX SYS_{0}
  ON {0}(ID)';
commit;
execute immediate
'ALTER TABLE {0} ADD ( 
  CONSTRAINT SYS_{0}
    PRIMARY KEY ( ID) 
    USING INDEX SYS_{0} 
    ENABLE 
    VALIDATE )';
commit;";
                sql = string.Format(sql, tableName);


                sql = string.Format(@"
select count(1) into flagExist from user_tables where table_name=upper('{0}') ;
if flagExist = 0  then
{1}
end if;
", tableName, sql);

                return sql;
            }
            else
            {
                string sql = @" 
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))
BEGIN
CREATE TABLE [{0}](
 [ID] [nvarchar](50) NOT NULL,
 [CreateDate] [datetime] NULL,
 [ModifyDate] [datetime] NULL,
 [CreateUserID] [nvarchar](50) NULL, 
 [CreateUser] [nvarchar](50) NULL, 
 [ModifyUserID] [nvarchar](50) NULL, 
 [ModifyUser] [nvarchar](50) NULL, 
 [OrgID] [nvarchar](50) NULL, 
 [CompanyID] [nvarchar](50) NULL, 
 [FlowPhase] [nvarchar](50) NULL, 
 [FlowInfo] [ntext] NULL,
 [StepName] [nvarchar](500) NULL,
 CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END";

                sql = string.Format(sql, tableName, tableDesc);

                sql += string.Format(@"
if EXISTS (SELECT * FROM  sys.extended_properties g WHERE  major_id =(SELECT object_id FROM sys.tables WHERE name = '{0}') and minor_id=0)
	EXECUTE sp_updateextendedproperty 'MS_Description','{1}','user','dbo','table','{0}',NULL,NULL
else
	EXECUTE sp_addextendedproperty 'MS_Description','{1}','user','dbo','table','{0}',NULL,NULL", tableName, tableDesc);

                return sql;
            }

            //代码备注-以供参考：列出表TestTable中列TestCol的描述属性
            //SELECT * FROM ::fn_ListExtendedProperty ( 'MS_Description' , 'User' , 'dbo' , 'Table' , 'TestTable' , 'Column' , 'TestCol' )
        }

        public string CreateSubTable(string mainTableName, string tableName, string tableDesc)
        {
            if (Config.Constant.IsOracleDb)
            {
                string sql = @"
execute immediate
'CREATE TABLE {0}(
  ID NVARCHAR2(50) NOT NULL,
  {1}ID NVARCHAR2(50) NULL,
  SORTINDEX BINARY_DOUBLE NULL,
  ISRELEASED CHAR(1 CHAR) NULL,
  CONSTRAINT C_{0} 
    FOREIGN KEY ( {1}ID)
       REFERENCES {1} ( ID) 
      ON DELETE CASCADE 
    ENABLE 
    VALIDATE)
  STORAGE ( 
    NEXT 1048576 )';
commit;
execute immediate
'CREATE UNIQUE INDEX SYS_{0}
  ON {0}(ID)';
commit;
execute immediate
'ALTER TABLE {0} ADD ( 
  CONSTRAINT SYS_{0} 
    PRIMARY KEY (ID) 
    USING INDEX SYS_{0}
    ENABLE 
    VALIDATE )';
commit;";

                sql = string.Format(sql, tableName, mainTableName);


                sql = string.Format(@"
select count(1) into flagExist from user_tables where table_name=upper('{0}') ;
if flagExist = 0  then
{1}
end if;
", tableName, sql);

                return sql;
            }
            else
            {
                string sql = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))
begin  
    CREATE TABLE [{0}](
        [ID] [nvarchar](50) NOT NULL,
        {1}ID [nvarchar](50) NULL,     
        SortIndex [float] NULL,
        [IsReleased] [char](1) NULL,
        CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
    (
	    [ID] ASC
    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    ALTER TABLE [{0}]  WITH CHECK ADD  CONSTRAINT [FK_{0}_{1}] FOREIGN KEY([{1}ID])
    REFERENCES [{1}] ([ID])
    ON DELETE CASCADE
    ALTER TABLE [{0}] CHECK CONSTRAINT [FK_{0}_{1}]
end
";
                sql = string.Format(sql, tableName, mainTableName, tableDesc);

                //增加注释
                sql += string.Format(@"
if EXISTS (SELECT * FROM  sys.extended_properties g WHERE  major_id =(SELECT object_id FROM sys.tables WHERE name = '{0}') and minor_id=0)
	EXECUTE sp_updateextendedproperty 'MS_Description','{1}','user','dbo','table','{0}',NULL,NULL
else
	EXECUTE sp_addextendedproperty 'MS_Description','{1}','user','dbo','table','{0}',NULL,NULL", tableName, tableDesc);

                return sql;
            }
        }

        #endregion

        #region 创建字段

        public string CreateField(string tableName, string fieldName, string fieldType, string fieldDesc)
        {
            if (string.IsNullOrEmpty(fieldType) || fieldName.ToUpper() == "ID")
                return "";
            if (Config.Constant.IsOracleDb)
            {
                if (fieldType == "nvarchar(max)")
                    fieldType = "NVarChar2(2000)";
                else if (fieldType.Contains("nvarchar"))
                    fieldType = fieldType.Replace("nvarchar", "NVarChar2");
                else if (fieldType == "ntext")
                    fieldType = "NClob";
                else if (fieldType == "datetime")
                    fieldType = "TIMESTAMP(3)";
                else if (fieldType == "decimal(18 2)")
                    fieldType = "Number(18,2)";
                else if (fieldType == "decimal(18 4)")
                    fieldType = "Number(18,4)";

                string sql = @"
SELECT COUNT(1) into flagExist FROM USER_TAB_COLUMNS WHERE TABLE_NAME = upper('{0}') and column_name = upper('{1}');
if flagExist = 0  then
    execute immediate
    'alter table {0} add ({1} {2})';
    commit;
    execute immediate
    'comment on column {0}.{1} is ''{3}''';
    commit;
else
    execute immediate
    'alter table {0} modify ({1} {2})';
    commit;
    execute immediate
    'comment on column {0}.{1} is ''{3}''';
    commit;
end if;";
                sql = string.Format(sql, tableName, fieldName.ToUpper(), fieldType, fieldDesc);
                return sql;
            }
            else
            {
                fieldType = fieldType.Replace(' ', ',');//decimal(18,2)

                string sql = string.Format(@"
if not exists(select * from syscolumns where id=object_id('{0}') and name='{1}')  
    alter table {0} add {1} {2}     
else 
    ALTER TABLE {0} ALTER COLUMN {1} {2}
", tableName, fieldName, fieldType);

                sql += string.Format(@"
if not exists(SELECT  fieldCode= a.name, description= g.[value] FROM  sys.columns a join sys.extended_properties g on (a.object_id = g.major_id AND g.minor_id = a.column_id) WHERE  object_id =(SELECT object_id FROM sys.tables WHERE name = '{0}') and a.name='{1}')
	EXECUTE sp_addextendedproperty 'MS_Description', '{2}', 'user', dbo, 'table', '{0}', 'column', '{1}'
else
	EXECUTE sp_updateextendedproperty N'MS_Description', '{2}', N'user', N'dbo', N'Table', {0}, N'column' , {1}
", tableName, fieldName, fieldDesc);

                return sql;
            }
        }

        #endregion

        #endregion

        #region 私有方法

        public List<FormItem> getSubTableItem(string settings)
        {
            if (string.IsNullOrEmpty(settings) || !settings.Contains("listData"))
                return new List<FormItem>();
            var dic = JsonHelper.ToObject(settings);
            return JsonHelper.ToObject<List<FormItem>>(dic["listData"].ToString());
        }

        #endregion

        #endregion

        #region 获取字段信息
        public DataTable GetFieldInfo(string code)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var itemList = entities.Set<S_UI_Form>().Where(c => c.Code == code).OrderByDescending(c => c.ID).FirstOrDefault().Items;
            var list = JsonHelper.ToObject<List<FormItem>>(itemList);
            DataTable table = new DataTable();
            table.Rows.Add(table.NewRow());
            foreach (var item in list)
            {
                DataColumn dc = new DataColumn(item.Code, typeof(string));
                table.Columns.Add(dc);
                table.Rows[0][item.Code] = item.Name;
            }
            return table;
        }
        #endregion
    }
    public enum EnumUseType
    {
        List,//列表
        Lists, //导航列表
        ListTabs, //导航Tab
        Tabs, //Tab
        TreeCustom, //树导航自定义
        TreeGridCustom //树列表导航自定义
    }
    public enum EnumUseMode
    {
        HighLow, //上下模式
        LeftRight  //左右模式
    }
}
