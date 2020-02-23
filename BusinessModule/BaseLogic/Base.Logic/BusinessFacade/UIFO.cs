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
using System.Collections;

namespace Base.Logic.BusinessFacade
{
    public class UIFO
    {
        public static string uiRegStr = "\\{[()（），。、；,.;0-9a-zA-Z_\u4e00-\u9faf]*\\}";//，。、；,.;

        /// <summary>
        /// 根据表单Id获取对应的版本，如果formInstanceId为空，则获取最新版本
        /// </summary>
        /// <param name="code"></param>
        /// <param name="formInstanceId"></param>
        /// <returns></returns>
        public static S_UI_Form GetFormDef(string code, string formInstanceId)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var formDefs = entities.Database.SqlQuery<S_UI_Form>("select * from S_UI_Form with(nolock) where Code='" + code + "' order by CreateTime desc").ToList();
            if (formDefs == null || formDefs.Count() == 0)
                return null;

            if (formDefs.Count() == 1 || string.IsNullOrEmpty(formInstanceId))
            {
                //不用判断表单版本
                return formDefs.First();
            }
            else
            {
                //有多个表单版本
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formDefs.First().ConnName);
                var dt = sqlHelper.ExecuteDataTable(string.Format("select top 1 * from {0} where ID='{1}'", formDefs.First().TableName, formInstanceId));

                if (dt.Rows.Count == 1)
                {
                    DateTime dateFormCreate = DateTime.Now; //表单创建时间
                    DataRow formDataRow = dt.Rows[0];
                    if (formDataRow.Table.Columns.Contains("CreateTime") && (formDataRow["CreateTime"] is DBNull) == false)
                        dateFormCreate = DateTime.Parse(formDataRow["CreateTime"].ToString());
                    else if (formDataRow.Table.Columns.Contains("CreateDate") && (formDataRow["CreateDate"] is DBNull) == false)
                        dateFormCreate = DateTime.Parse(formDataRow["CreateDate"].ToString());

                    var _forms = formDefs.FindAll(c => c.VersionStartDate < dateFormCreate && (c.VersionEndDate == null || c.VersionEndDate > dateFormCreate));//找到相应的版本
                    if (_forms == null || _forms.Count() == 0)
                        throw new Exception(string.Format("找不到表单[{0}]对应的表单编号[{1}]版本定义", formInstanceId, code));
                    else
                        return _forms[0];
                }
                else
                    return formDefs.First();
            }

        }

        public static S_UI_Word GetWordDef(string code, string formInstanceId)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var wordInfos = entities.Database.SqlQuery<S_UI_Word>("select * from S_UI_Word with(nolock) where Code='" + code + "' order by CreateTime desc").ToList();
            if (wordInfos == null || wordInfos.Count() == 0)
                return null;

            if (wordInfos.Count() == 1 || string.IsNullOrEmpty(formInstanceId))
            {
                //不用判断表单版本
                return wordInfos.First();
            }
            else
            {
                //有多个表单版本
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(wordInfos.First().ConnName);
                var dt = sqlHelper.ExecuteDataTable(string.Format("select * from ({0}) a where ID='{1}'", wordInfos.First().SQL, formInstanceId));
                if (dt.Rows.Count == 1)
                {
                    DateTime dateFormCreate = DateTime.Now; //表单创建时间
                    DataRow formDataRow = dt.Rows[0];
                    if (formDataRow.Table.Columns.Contains("CreateTime") && (formDataRow["CreateTime"] is DBNull) == false)
                        dateFormCreate = DateTime.Parse(formDataRow["CreateTime"].ToString());
                    else if (formDataRow.Table.Columns.Contains("CreateDate") && (formDataRow["CreateDate"] is DBNull) == false)
                        dateFormCreate = DateTime.Parse(formDataRow["CreateDate"].ToString());

                    var _word = wordInfos.FindAll(c => c.VersionStartDate < dateFormCreate && (c.VersionEndDate == null || c.VersionEndDate > dateFormCreate));//找到相应的版本
                    if (_word == null || _word.Count() == 0)
                        throw new Exception(string.Format("找不到表单[{0}]对应的word编号[{1}]版本定义", formInstanceId, code));
                    else
                        return _word[0];
                }
                else
                    return wordInfos.First();
            }

        }


        #region 表单Html及脚本

        #region CreateFormHiddenHtml

        public string CreateFormHiddenHtml(S_UI_Form form)
        {
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

        public string CreateFormHtml(S_UI_Form formInfo)
        {
            var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);
            Regex reg = new Regex(uiRegStr);

            string layout = "";
            string IsEn = HttpContext.Current.Request["IsEn"];
            if ((FormulaHelper.GetCurrentLGID() == "EN" && !string.IsNullOrEmpty(formInfo.LayoutEN)) || IsEn == "T")
                layout = formInfo.LayoutEN;
            else
                layout = formInfo.Layout;

            string script = "";
            //去除所有的换行符号，以免样式出现多余的空格
            layout = Regex.Replace(layout, "<br/>|<BR/>", (Match m) => { return ""; });
            Regex regHelper = new Regex(@"(?<=>)([\n\s\S]*?)(?=<)");
            layout = regHelper.Replace(layout ?? "", (Match m) =>
            {
                string value = m.Value.Trim(' ', '\n', '\t');
                if (value.StartsWith("{"))
                    return m.Value;
                var item = items.SingleOrDefault(c => c.Name == value);
                if (item == null || string.IsNullOrEmpty(item.help) || item.help.Trim(' ', '"') == "")
                    return m.Value;



                //return "<a href='#' onclick='showItemHelper(" + value + ");'>" + value + "</a>";

                if (item.help.StartsWith("{") == false)//以前帮助数据直接保存的html
                {
                    var html = item.help.Trim('"').Replace(@"\n", "").Replace(@"\t", "");
                    script += "\n var " + value + "='" + html + "';";
                    return string.Format("<span data-tooltip='{0}' data-placement='right'>{1}<img src='/CommonWebResource/Theme/Default/MiniUI/icons/help.png'/></span>", html, value);
                }
                else
                {
                    var data = JsonHelper.ToObject<Dictionary<string, string>>(item.help);
                    var html = data["html"].Trim('"').Replace(@"\n", "").Replace(@"\t", "");
                    script += "\n var " + value + "='" + html + "';";

                    //点击帮助图标可以下载附件
                    return string.Format("<span data-tooltip='{0}' data-placement='right'>{1}<img src='/CommonWebResource/Theme/Default/MiniUI/icons/help.png'  onclick='DownloadFile(\"" + data["file"] + "\");'/></span>", html, value);

                }
            });

            var str = "<script type='text/javascript'>";
            str += script;
            str += "\n</script>";
            layout = str + layout;

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

            var triggerField = formInfo.CalTriggerFields.FirstOrDefault(c => c.FieldCode == item.Code);
            if (triggerField != null)
            {
                var settingDic = JsonHelper.ToObject(item.Settings);
                if (String.IsNullOrEmpty(settingDic.GetValue("onvaluechanged")))
                {
                    //如果定义的变更事件是空的，表示用户没有自定义逻辑事件处理，此处就自动默认增加绑定事件用于自动计算
                    settingDic.SetValue("onvaluechanged", "formItemCal(this,\"" + triggerField.CalItemCodes.TrimEnd(',') + "\",\"" + item.Code + "\")");
                    item.Settings = JsonHelper.ToJson(settingDic);
                }
            }
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
            else if (item.ItemType == "UEditor")
            {
                string height = "250px";
                if (!string.IsNullOrEmpty(item.Settings))
                {
                    var dic = JsonHelper.ToObject(item.Settings);
                    height = dic["style_height"].ToString();
                }
                //return string.Format("<textarea name='{0}' class='KindEditor'style='width:100%;height:{1};'></textarea>", item.Code, height);
                return string.Format("<script id='{0}' name='{0}' class='UEditor' type='text/plain' style='width:100%;height:{1}'></script>", item.Code, height);

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

        public string CreateFormScript(S_UI_Form form, bool isOutput = false)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();

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
                        string urlParams = "";
                        if (dic.ContainsKey("UrlParams"))
                            urlParams = dic["UrlParams"];

                        if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                            sb.AppendFormat("addMultiUserSelector('{0}',{{returnParams:'{1}',UrlParams:'{2}'}});\n", item.Code, returnParams, urlParams);
                        else
                            sb.AppendFormat("addSingleUserSelector('{0}',{{returnParams:'{1}',UrlParams:'{2}'}});\n", item.Code, returnParams, urlParams);
                    }
                    else if (dic["SelectorKey"].ToString() == "SystemOrg")
                    {
                        string urlParams = "";
                        if (dic.ContainsKey("UrlParams"))
                            urlParams = dic["UrlParams"];

                        if (dic.ContainsKey("AllowMultiSelect") && dic["AllowMultiSelect"] == "true")
                            sb.AppendFormat("addMultiOrgSelector('{0}',{{ returnParams:'{1}',UrlParams:'{2}'}});\n", item.Code, returnParams, urlParams);
                        else
                            sb.AppendFormat("addSingleOrgSelector('{0}',{{ returnParams:'{1}',UrlParams:'{2}'}});\n", item.Code, returnParams, urlParams);
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

            //获取数据源配置信息，枚举如果是设置的数据源，后续则不再进行枚举获取
            var defaultValueSettings = new List<Dictionary<string, object>>();
            if (!String.IsNullOrEmpty(form.DefaultValueSettings))
            {
                defaultValueSettings = JsonHelper.ToList(form.DefaultValueSettings);
            }

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

                    //获取数据源配置信息，枚举如果是设置的数据源，后续则不再进行枚举获取
                    if (!data.StartsWith("[") && data.Split('.').Length == 1)
                    {
                        if (defaultValueSettings.Exists(c => c.ContainsKey("Code") && c["Code"].ToString() == data))
                            continue;
                    }

                    var key = GetEnumKey(form.TableName, item.Code, data);
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
                            string key = GetEnumKey(tableName, subItem.Code, data);
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

        public Dictionary<string, DataTable> GetDefaultValueDic(string DefaultValueSettings, DataRow row = null, bool validateCurrentUser = true)
        {
            try
            {
                if (string.IsNullOrEmpty(DefaultValueSettings))
                    return new Dictionary<string, DataTable>();

                Dictionary<string, DataTable> defaultValueDic = new Dictionary<string, DataTable>();
                foreach (var item in JsonHelper.ToList(DefaultValueSettings))
                {
                    if (item.GetValue("treeGridSource") == true.ToString().ToLower())
                        continue;
                    SQLHelper tmpSQLHelper = SQLHelper.CreateSqlHelper(item["ConnName"].ToString());
                    string defaultSQL = ReplaceString(item["SQL"].ToString(), row, null, null, validateCurrentUser);
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
            if (string.IsNullOrEmpty(dic.GetValue("listData")))
                return "";
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
            var LGID = FormulaHelper.GetCurrentLGID();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
<div id='toolbar{3}' class='mini-toolbar'  style='height:25px;border-bottom: 0px;{4}'>
    <table>
        <tr>
            <td style='text-align:left;'>
                <a class='mini-button' id='btn{3}Add' iconcls='icon-add' onclick='addRow({0});' visible='{5}'>" + (LGID == "EN" ? "Add" : "添加") + @"</a>
                <a class='mini-button' id='btn{3}Delete' iconcls='icon-remove' onclick='delRow({1});' visible='{6}'>" + (LGID == "EN" ? "Remove" : "移除") + @"</a>
                <a class='mini-button' iconcls='icon-up' onclick='moveUp({2});' visible='{7}'>" + (LGID == "EN" ? "Move Up" : "上移") + @"</a>
                <a class='mini-button' iconcls='icon-down' onclick='moveDown({2});' visible='{8}'>" + (LGID == "EN" ? "Move Down" : "下移") + @"</a>
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
        <div id='{0}' {1} {2} {3} class='mini-datagrid' {4} {5} allowcellvalid='true' multiselect='true' allowcelledit='true' allowcellselect='true' showpager='false' allowUnselect='false' sortMode='client'>
 ", formItem.Code, miniuiSettings
  , formItem.Enabled == "true" ? "" : "enabled='false'"
  , formItem.Visible == "true" ? "" : "visible='false'"
  , dicSubTableSettings.GetValue("IsVirtualScroll") == "true" ? "virtualScroll='true'" : ""
  , dicSubTableSettings.GetValue("IsVirtualColumns") == "true" ? "virtualColumns='true'" : ""
  );
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


            string currentTopHeader = "";
            foreach (var item in list)
            {
                if (item.Visible == "false")
                    continue;
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
<div field='{0}' displayfield='{0}Name' header='{1}' {2}  renderer='onFileRender' {4}>
    <input property='editor' class='mini-fileupload' onclick='btnUploadifiveClick' style='width: 100%;' label='{3}' />  
</div>"
                       , item.Code
                       , item.Name
                       , GetMiniuiSettings(JsonHelper.ToJson(item))
                       , item.Settings
                       , vtype == "" ? "" : string.Format("vtype='{0}'", vtype)
                       );
                    continue;
                }
                #endregion

                var subTableItemCode = formItem.Code + "." + item.Code;
                var triggerField = formInfo.CalTriggerFields.FirstOrDefault(c => c.FieldCode == subTableItemCode);
                if (triggerField != null)
                {
                    var settingDic = JsonHelper.ToObject(item.Settings);
                    if (String.IsNullOrEmpty(settingDic.GetValue("onvaluechanged")))
                    {
                        //如果定义的变更事件是空的，表示用户没有自定义逻辑事件处理，此处就自动默认增加绑定事件
                        settingDic.SetValue("onvaluechanged", "formItemCal(this,\"" + triggerField.CalItemCodes.TrimEnd(',') + "\",\"" + subTableItemCode + "\")");
                        item.Settings = JsonHelper.ToJson(settingDic);
                    }
                }

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

                string comboBoxPty = "type='comboboxcolumn'";
                if (item.ItemType == "ComboBox")
                {
                    var columSettingsDic = JsonHelper.ToObject(item.Settings);
                    if (columSettingsDic.ContainsKey("textName") && columSettingsDic["textName"].ToString() != "")
                        comboBoxPty = string.Format("displayField='{0}'", columSettingsDic["textName"]);
                }

                //多表头处理
                string header = item.Name;
                if (header == null) header = "";
                //判断闭合
                if (currentTopHeader != "" && header.StartsWith(currentTopHeader + ".") == false)
                {
                    sb.AppendLine("</div></div>");
                    currentTopHeader = "";
                }
                if (header.Contains('.'))
                {
                    string topHeader = header.Split('.').First();

                    if (topHeader != currentTopHeader)
                    {
                        currentTopHeader = topHeader;
                        //新的多表头                       
                        sb.AppendFormat("<div header='{0}' headerAlign='center'><div property='columns'>", topHeader);
                    }
                }

                string itemHtml = CreateSubTableItem(item, ColumnSettings, miniuiSettings, vtype, comboBoxPty, dataPty, formItem.Code, LGID);
                sb.Append(itemHtml);
            }
            //循环结束，闭合多表头
            if (currentTopHeader != "")
            {
                currentTopHeader = "";
                sb.AppendLine("</div></div>");
            }

            sb.AppendFormat(@"
                </div>
            </div>");
            return sb.ToString();
        }

        private string CreateSubTableItem(FormItem item, string ColumnSettings, string miniuiSettings, string vtype, string comboBoxPty, string dataPty, string formItemCode, string LGID = "")
        {
            var name = item.Name.Split('.').LastOrDefault();
            if (LGID == "EN" && !string.IsNullOrEmpty(item.NameEN))
                name = item.NameEN;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(item.ColumnSettings))
                dic = JsonHelper.ToObject(item.ColumnSettings);
            string html = string.Format(@"
        <div name='{3}' field='{3}' {8} header='{4}' {5} {6} {7} {0} autoShowPopup='true' {12} {13} allowSort='true'>
                <input {9} property='editor' class='mini-{1}' {2} {10} {11} {14} />
        </div>"
            , GetMiniuiSettings(JsonHelper.ToJson(item))
            , item.ItemType.ToLower()
            , miniuiSettings
            , item.Code
            , name
            , item.ItemType == "DatePicker" && ColumnSettings.IndexOf("dateFormat") >= 0 ? "dateFormat='" + dic.GetValue("dateFormat") + "'" : ""
            , item.ItemType == "ComboBox" ? comboBoxPty : ""
            , vtype == "" ? "" : string.Format("vtype='{0}'", vtype)
            , item.ItemType == "ButtonEdit" ? "displayfield='" + item.Code + (Config.Constant.IsOracleDb ? "NAME'" : "Name'") : ""
            , item.ItemType == "ButtonEdit" ? " name='" + formItemCode + "_" + item.Code + "'" : ""  //列表上选择暂时不支持智能感知，因此先加allowInput
            , item.ItemType == "ComboBox" && miniuiSettings.Contains("multiselect='true'") == false && miniuiSettings.Contains("onitemclick=") == false ? "onitemclick=\"commitGridEdit('" + formItemCode + "');\"" : ""
            , item.ItemType == "DatePicker" ? "onhidepopup=\"commitGridEdit('" + formItemCode + "');\"" : ""
            , string.IsNullOrEmpty(item.SummaryType) ? "" : string.Format("summaryType='{0}' summaryRenderer='onSummaryRenderer'", item.SummaryType)
            , ColumnSettings
            , dataPty
            );
            return html;
        }

        #endregion


        #endregion

        #endregion

        #region 列表Html及脚本


        public string CreateListHtml(string code)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == code);
            if (listDef == null)
                throw new Exception(string.Format("编号为：“{0}”的列表不存在", code));
            var fields = JsonHelper.ToList(listDef.LayoutField);
            var buttons = JsonHelper.ToList(listDef.LayoutButton);
            var showQueryForm = false; var rowCount = 2;
            var isColumnEdit = false;
            var showPager = true;
            var LGID = FormulaHelper.GetCurrentLGID();
            bool showFilterRow = false;
            if (!string.IsNullOrEmpty(listDef.LayoutGrid))
            {
                var settings = JsonHelper.ToObject(listDef.LayoutGrid);
                if (settings.GetValue("allowCellEdit") == "true")
                    isColumnEdit = true;
                if (settings.GetValue("showPager") == "false")
                    showPager = false;
                if (!isColumnEdit && settings.GetValue("showFilterRow") == "true")
                    showFilterRow = true;
            }
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

            if (isColumnEdit && !showPager)
            {
                //不能详查
                queryFields = new List<Dictionary<string, object>>();
                showQueryForm = false;
            }
            #region QueryForm

            StringBuilder sbQuery = new StringBuilder();

            int i = 0; var queryFieldsCount = queryFields.Count();
            foreach (var item in queryFields)
            {
                if (i % rowCount == 0)
                    sbQuery.Append("<tr>");

                if (i + 1 % rowCount == 0)
                    sbQuery.Append("<td width=\"5%\" /></tr>");
                else
                {
                    sbQuery.Append(GetQueryItemHtml(listDef, item, showQueryForm));
                    if ((i + 1) % rowCount == 0)
                        sbQuery.Append("</tr>");
                    else if (i == queryFieldsCount - 1)
                        sbQuery.Append("</tr>");
                }
                //if (i + 1 % rowCount == 0)
                //    sbQuery.Append("<td width=\"5%\" /></tr>");
                //else
                //{

                //}

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
<div id='queryWindow' class='mini-window' title='" + (LGID == "EN" ? "Detailed Inquiry" : "详细查询") + @"' style='width: 690px; height: @{1}px;'>
    <div class='queryDiv'>
        <form id='queryForm' method='post'>
        <table>
           {0}
        </table>
        </form>
        <div>
            <a class='mini-button' onclick='search()' iconcls='icon-find' style='margin-right: 20px;'>" + (LGID == "EN" ? "Query" : "查询") + @"</a>
            <a class='mini-button' onclick='clearQueryForm()' iconcls='icon-undo'>" + (LGID == "EN" ? "Clear" : "清空") + @"</a>
        </div>
    </div>
</div>";
                strQueryForm = string.Format(queryHtml, sbQuery, 100 + 22 * (queryFields.Count() / 2));
            }
            #endregion

            #region Bar条

            StringBuilder sbButtons = new StringBuilder();
            bool isRightKey = false;
            foreach (var item in buttons)
            {
                string onclick = "";
                if (item.ContainsKey("URL") && !string.IsNullOrEmpty(item["URL"].ToString()))
                {
                    onclick = "onclick='openWindow(\"" + item["URL"] + "\"";
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
                if (item.ContainsKey("Settings"))
                {
                    var sets = JsonHelper.ToObject(item["Settings"].ToString());
                    var buttonType = sets.GetValue("ButtonType");
                    if (!string.IsNullOrEmpty(buttonType) && buttonType == "rightKey" && item.GetValue("Enabled") == "true")
                    {
                        isRightKey = true;
                    }
                }
                sbButtons.AppendFormat("\n<a class='mini-button' {0} {1} {2}></a>"
                    , GetMiniuiSettings(item)
                    , onclick
                    , item.ContainsKey("Settings") ? GetMiniuiSettings(item["Settings"].ToString()) : "");
            }

            StringBuilder sb = new StringBuilder();
            string strBar = "";
            if (showQueryForm)
            {
                strBar = @"
<div class='mini-toolbar gw-grid-toolbar' style='padding: 0px 0px 0px 0px;'>
    <table>
        <tr>
            <td>
                {0}
            </td>
            <td class='gw-toolbar-right'>
            <a class='mini-button' onclick='search()' iconcls='icon-find'>" + (LGID == "EN" ? "Query" : "查询") + @"</a>
            <a class='mini-button' onclick='clearQueryForm()' iconcls='icon-undo'>" + (LGID == "EN" ? "Clear" : "清空") + @"</a>
            </td>
        </tr>
    </table>
</div>";
                strBar = string.Format(strBar
                       , sbButtons
                       );
            }
            else
            {
                strBar = @"
<div class='mini-toolbar gw-grid-toolbar' style='padding: 0px 0px 0px 0px;'>
    <table>
        <tr>
            <td>
                {0}
            </td>
            <td class='gw-toolbar-right'>
            {1}
            {3}
            {2}
            <a class='mini-button' onclick='showHelp()' iconcls='icon-help'>" + (LGID == "EN" ? "Help" : "帮助") + @"</a>
            </td>
        </tr>
    </table>
</div>";
                if (LGID == "EN")
                {
                    foreach (var field in quickQueryFields)
                    {
                        if (field.ContainsKey("HeaderEN"))
                            field.SetValue("header", field.GetValue("HeaderEN"));
                        else if (field.ContainsKey("headerEN"))
                            field.SetValue("header", field.GetValue("headerEN"));
                    }
                }
                string emptyText = string.Join(string.Format("{0}", LGID == "EN" ? " Or " : "或"), quickQueryFields.Select(c => c["header"].ToString()).ToArray());
                string strQuickQueryBox = string.Format("<input id='key' class='mini-buttonedit gw-searchbox' style='width:{3}em;' emptytext='" + (LGID == "EN" ? "Input " : "请输入") + "{0}' onenter=\"{2}('{1}');\" onbuttonclick=\"{2}('{1}');\" />"
                    , emptyText
                    , string.Join(",", quickQueryFields.Select(c => c["field"].ToString()).ToArray())
                    , isColumnEdit && !showPager ? "clientSearch" : "quickSearch"
                    , emptyText.Length + 5 >= 15 ? emptyText.Length + 5 : 15
                    );

                string strFilterButton = "<a id='list-filter-btn' class='mini-button' onclick=\"filterSwitch();\" iconcls='icon-filter'>" + (LGID == "EN" ? "Filter" : "筛选") + @"</a>";

                string strSearchButton = "<a class='mini-button' onclick=\"showWindow('queryWindow');\" iconcls='icon-find'>" + (LGID == "EN" ? "Query" : "详细") + @"</a>";

                strBar = string.Format(strBar
                    , sbButtons
                    , quickQueryFields.Count() > 0 ? strQuickQueryBox : ""
                    , queryFields.Count() > 0 ? strSearchButton : ""
                    , showFilterRow ? strFilterButton : ""
                    );
            }

            #endregion

            #region 默认值
            //默认值Dic
            var defaultDic = new Dictionary<string, string>();
            if (isColumnEdit)
            {
                foreach (var item in fields)
                {
                    var _DefaultValue = item.GetValue("Edit_DefaultValue");
                    var _Code = item.GetValue("field");
                    var _ItemType = item.GetValue("Edit_ItemType");
                    if (string.IsNullOrEmpty(_DefaultValue))
                        continue;
                    if (_DefaultValue.Contains(',') && _ItemType == "ButtonEdit")
                    {
                        defaultDic.Add(_Code, GetDefaultValue(_Code, _DefaultValue.Split(',').First(), GetDefaultValueDic(listDef.DefaultValueSettings)));
                        defaultDic.Add(_Code + "Name", GetDefaultValue(_Code, _DefaultValue.Split(',').Last(), GetDefaultValueDic(listDef.DefaultValueSettings)));
                    }
                    else
                    {
                        defaultDic.Add(_Code, GetDefaultValue(_Code, _DefaultValue, GetDefaultValueDic(listDef.DefaultValueSettings)));
                    }
                }
                sb.AppendLine();
                sb.AppendFormat("\n var defaultValueDic = {0}", JsonHelper.ToJson(defaultDic));
            }
            #endregion

            #region Grid

            string strField = "";
            StringBuilder sbField = new StringBuilder();
            bool multi = false;
            foreach (var field in fields)
            {
                string header = field["header"].ToString();
                if (field.ContainsKey("header" + LGID))
                    header = field["header" + LGID].ToString();
                if (header.Contains("."))
                {
                    multi = true;
                    break;
                }
                if (isColumnEdit)
                    sbField.Append(CreateListEditColumnItem(field, listDef));
                else
                    sbField.AppendFormat("<div name='{3}' {0} {1} header='{2}'>{4}</div>", GetMiniuiSettings(field)
                        , field.ContainsKey("Settings") ? GetMiniuiSettings(field["Settings"].ToString()) : ""
                        , header, field["field"].ToString()
                        , showFilterRow ? GetFilterInputHtml(field) : "");
            }
            if (multi)
            {
                fields.RemoveWhere(c => c["Visible"].ToString() == "false");
                if (isColumnEdit)
                    strField = CreateListEditColumn("", fields, listDef);
                else
                    strField = CreateListColumn("", fields, showFilterRow);
            }
            else
            {
                strField = sbField.ToString();
            }


            string strTreeGridPager = @"
<div class='mini-pager' 
    id='treeGridPager' onpagechanged='onPageChanged' sizeList='[10,20,50,100,200,300,500]' pagesize='50' showPageSize='true' showPageIndex='true' showPageInfo='true' >        
</div> 
<script>

    function onTreeGridLoad(e) {
        if (e.text) {
            var resultData = mini.decode(e.text);
            e.result = resultData;
            if (e.result && e.result.data) {
                e.sender.loadList(e.result.data);
                var pager = mini.get('#treeGridPager');
                if (pager) {
                    pager.setTotalCount(e.result.total);
                }
            }  
        }
    }
    $('#dataGrid').attr('onbeforeload', 'onBeforeTreeLoad');
    function onBeforeTreeLoad(e) {
        var tree = e.sender;    //树控件
        var node = e.node;      //当前节点
        var params = e.params;  //参数对象

        //可以传递自定义的属性
        if (node._id != '-1') {
            params.NodeInfo = mini.encode(node);
            params.IsLoadChildren = 'true';
        }
        else {
            params.IsLoadChildren = 'false';
            params.NodeInfo = mini.encode({});
        }
    }

    function onPageChanged(e) {
        var pageIndex = e.pageIndex;
        var pageSize = e.pageSize;
        $('.mini-treegrid').each(function () {
            var grid = mini.get('#' + $(this).attr('id'));
            if (grid.url) {
                grid.setPageIndex(pageIndex);
                grid.setPageSize(pageSize);
                //grid.setUrl(decodeURI(addUrlSearch(grid.url))); //url增加当前地址栏参数           
                grid.reload();
            }
        });
    }
</script>";

            string strTreeGridScript = @"
<script>
    function onTreeGridLoad(e) {
        if (e.text) {
            var resultData = mini.decode(e.text);
            e.result = resultData;
            if (e.result && e.result.data) {
                e.sender.loadList(e.result.data);
                var pager = mini.get('#treeGridPager');
                if (pager) {
                    pager.setTotalCount(e.result.total);
                }
            }              
        }
    }
</script>";

            string strGrid = @"
<div class='mini-fit' id='divGrid'>
    <div id='dataGrid' class='{4}' style='width: 100%; height: 100%;' url='GetList' {0} {5} {7}  onDrawSummaryCell='onDrawSummaryCell'>
        <div property='columns'>         
            {2}  
            {3}
            {1}
        </div>
    </div>
</div>
{6}
";
            bool isTreeGrid = IsTreeGrid(listDef.LayoutGrid);

            strGrid = string.Format(strGrid
                , GetMiniuiSettings(listDef.LayoutGrid, isTreeGrid) + (isColumnEdit && showPager ? " onbeforeload='onDataGridBeforeload' " : isColumnEdit ? " sortMode='client' " : "") + GetTreeGridSettings(listDef.LayoutGrid, fields, isTreeGrid)
                , strField //sbField
                , listDef.HasRowNumber == "1" ? "<div type='indexcolumn' headerAlign='center'>序号</div>" : ""
                , listDef.HasCheckboxColumn != "0" ? "<div type='checkcolumn'></div>" : ""
                , isTreeGrid ? "mini-treegrid" : "mini-datagrid"
                , isRightKey ? "contextmenu='#treeMenu'" : ""
                , isTreeGrid && showPager ? strTreeGridPager : strTreeGridScript
                 , "ajaxOptions={async:true}"
                );

            #endregion

            if (showQueryForm)
            {
                return strBar + "\n" + strQueryForm + "\n" + strGrid + "\n" + createExportExcelbtn(code);
            }
            else
            {
                return strBar + "\n" + strGrid + "\n" + strQueryForm + "\n" + createExportExcelbtn(code);
            }
        }


        private string CreateListColumn(string fieldName, List<Dictionary<string, object>> fields, bool showFilterRow = false)
        {
            if (fields.Count == 0)
                return "";

            if (fields.Count == 1)
            {
                var field = fields[0];
                return string.Format("<div name='{3}' field='{2}' {0} {1}>{4}</div>"
                     , GetMiniuiSettings(field)
                     , field.ContainsKey("Settings") ? GetMiniuiSettings(field["Settings"].ToString()) : ""
                     , field["field"]
                     , field["field"]
                     , showFilterRow ? GetFilterInputHtml(field) : "" //过滤行
                     );
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
                string str = CreateListColumn(name, newFields, showFilterRow);
                sb.AppendFormat(str);
            }
            sb.AppendLine();
            if (!string.IsNullOrEmpty(fieldName))
                sb.AppendFormat("</div></div>");
            return sb.ToString();
        }

        private string CreateListEditColumn(string fieldName, List<Dictionary<string, object>> fields, S_UI_List listDef)
        {
            if (fields.Count == 0)
                return "";

            if (fields.Count == 1)
            {
                var field = fields[0];
                return CreateListEditColumnItem(field, listDef);
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
                string str = CreateListEditColumn(name, newFields, listDef);
                sb.AppendFormat(str);
            }
            sb.AppendLine();
            if (!string.IsNullOrEmpty(fieldName))
                sb.AppendFormat("</div></div>");
            return sb.ToString();
        }

        private string CreateListEditColumnItem(Dictionary<string, object> field, S_UI_List listDef)
        {
            StringBuilder sb = new StringBuilder();
            if (field.GetValue("Visible") == "false")
                return "";
            var _ItemType = field.GetValue("Edit_ItemType");
            var _Code = field.GetValue("field");
            var _Name = field.GetValue("header");
            var _Edit_Settings = field.GetValue("Edit_Settings");
            var miniuiSettings = GetMiniuiSettings(_Edit_Settings);
            var _GridId = "dataGrid";

            #region 特殊控件处理

            if (_ItemType == "CheckBox")
            {
                sb.AppendFormat("\n<div type='checkboxcolumn' name='{0}' {1} {2} {3}></div>"
                    , _Code
                    , ""
                    , GetMiniuiSettings(field)
                    , miniuiSettings
                    );
                return sb.ToString();
            }
            else if (_ItemType == "SingleFile")
            {

                sb.AppendFormat(@"
<div field='{0}' name='{0}' displayfield='{0}Name' {1} {2} renderer='onFileRender'>
    <input property='editor' class='mini-fileupload' onclick='btnUploadifiveClick' style='width: 100%;' label='{3}' />  
</div>"

                   , _Code
                   , ""
                   , GetMiniuiSettings(field)
                   , _Edit_Settings
                   );

                return sb.ToString();
            }

            #endregion

            if (miniuiSettings == "")
                miniuiSettings = "style='width:100%'";

            string ColumnSettings = GetMiniuiSettings(field.GetValue("Edit_ColumnSettings"));//列格式

            string dataPty = "";
            if (_ItemType == "ComboBox")
            {
                string tableName = listDef.TableNames.Split(',')[0] + "_" + _GridId;
                dataPty = GetFormItemDataPty(tableName, _Code, _Edit_Settings);
            }

            #region 获取vtype
            string vtype = "";
            if (!string.IsNullOrEmpty(_Edit_Settings))
            {
                var _dic = JsonHelper.ToObject<Dictionary<string, string>>(_Edit_Settings);
                if (_dic.ContainsKey("required") && _dic["required"] == "true")
                    vtype += "required;";
                if (_dic.ContainsKey("vtype"))
                    vtype += _dic["vtype"];
            }
            #endregion

            string comboBoxPty = "type='comboboxcolumn'";
            if (_ItemType == "ComboBox")
            {
                var columSettingsDic = JsonHelper.ToObject(_Edit_Settings);
                if (columSettingsDic.ContainsKey("textName") && columSettingsDic["textName"].ToString() != "")
                    comboBoxPty = string.Format("displayField='{0}'", columSettingsDic["textName"]);
            }

            string itemHtml = string.Format(@"
                <div name='{3}'  {8} {4} {5} {6} {7} {0} autoShowPopup='true' {12} {13}>
                        <input {9} property='editor' class='mini-{1}' {2} {10} {11} {14} />
                </div>
                ", GetMiniuiSettings(field)
                , _ItemType.ToLower()
                , miniuiSettings
                , _Code
                , ""
                , _ItemType == "DatePicker" && ColumnSettings.IndexOf("dateformat") >= 0 ? "dateFormat='yyyy-MM-dd'" : ""
                , _ItemType == "ComboBox" ? comboBoxPty : ""
                , vtype == "" ? "" : string.Format("vtype='{0}'", vtype)
                , _ItemType == "ButtonEdit" ? "displayfield='" + _Code + (Config.Constant.IsOracleDb ? "NAME'" : "Name'") : ""
                , _ItemType == "ButtonEdit" ? " name='" + _GridId + "_" + _Code + "'" : ""  //列表上选择暂时不支持智能感知，因此先加allowInput
                , _ItemType == "ComboBox" && miniuiSettings.Contains("multiselect='true'") == false && miniuiSettings.Contains("onitemclick=") == false ? "onitemclick=\"commitGridEdit('" + _GridId + "');\"" : ""
                , _ItemType == "DatePicker" ? "onhidepopup=\"commitGridEdit('" + _GridId + "');\"" : ""
                , string.IsNullOrEmpty(field.GetValue("SummaryType")) ? "" : string.Format("summaryType='{0}' summaryRenderer='onSummaryRenderer'", field.GetValue("SummaryType"))
                , ColumnSettings
                , dataPty
                );
            sb.Append(itemHtml);



            return sb.ToString();

        }

        public string CreateEditListScript(BaseEntities entities, S_UI_List listDef, Dictionary<string, object> field, bool isOutput = false)
        {
            StringBuilder sb = new StringBuilder();
            var _ItemType = field.GetValue("Edit_ItemType");
            var _Code = field.GetValue("field");
            var _Name = field.GetValue("header");
            var _Edit_Settings = field.GetValue("Edit_Settings");
            var miniuiSettings = GetMiniuiSettings(_Edit_Settings);
            var _GridId = "dataGrid";

            #region 选择器脚本

            if (_ItemType == "ButtonEdit")
            {
                string selectorName = _GridId + "_" + _Code;

                var dic = JsonHelper.ToObject<Dictionary<string, string>>(_Edit_Settings);

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
            #endregion

            #region 下拉选择枚举
            if (_ItemType == "ComboBox")
            {
                var dic = JsonHelper.ToObject(_Edit_Settings);
                var data = dic.GetValue("data");
                if (!string.IsNullOrEmpty(data))
                {
                    string tableName = listDef.TableNames.Split(',')[0] + "_" + _GridId;
                    string key = GetEnumKey(tableName, _Code, data);
                    string enumStr = GetEnumString(listDef.ConnName, tableName, _Code, data);
                    if (isOutput)
                        sb.AppendFormat(GetOutputEnumString(listDef.ConnName, tableName, _Code, data));
                    else
                    {
                        if (string.IsNullOrEmpty(enumStr))
                            enumStr = "[]";
                        sb.AppendFormat("\n var {0} = {1}; ", key, enumStr);
                    }
                    sb.AppendFormat("\n addGridEnum('{0}', '{1}', '{2}');", _GridId, _Code, key);
                }
            }

            #endregion

            return sb.ToString();
        }

        public string CreateListScript(string code, bool isOutput = false)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == code);
            if (listDef == null)
                throw new Exception(string.Format("编号为：“{0}”的列表不存在", code));

            StringBuilder sb = new StringBuilder();

            var isColumnEdit = false; var showPager = false;
            if (!string.IsNullOrEmpty(listDef.LayoutGrid))
            {
                var settings = JsonHelper.ToObject(listDef.LayoutGrid);
                if (settings.GetValue("allowCellEdit") == "true")
                    isColumnEdit = true;
                if (settings.GetValue("showPager") == "true")
                    showPager = true;
            }
            var fields = JsonHelper.ToList(listDef.LayoutField);

            //确保快速查询与详细查询的查询结果是AND关系，前端已经修订，自定义列表需要将快速查询字段作为固定页面参数存在
            var quickQueryFields = fields.Where(c => c.ContainsKey("AllowSearch") && c["AllowSearch"].ToString() == "true");
            var serchFields = String.Join(",", quickQueryFields.Select(c => c["field"]).ToList());
            sb.AppendFormat("\n normalParamSettings.searchFields ='{0}'", serchFields);

            #region 字段详细
            foreach (var field in fields)
            {
                if (isColumnEdit)
                {
                    sb.AppendLine();
                    sb.Append(CreateEditListScript(entities, listDef, field, isOutput));
                    continue;
                }
                if (!field.ContainsKey("Settings"))
                    continue;
                var settings = JsonHelper.ToObject(field["Settings"].ToString());

                if (!settings.ContainsKey("EnumKey") || settings["EnumKey"].ToString() == "")
                    continue;

                string enumKey = settings["EnumKey"].ToString();

                string tableName = listDef.TableNames.Split(',')[0];
                var key = GetEnumKey(tableName, field["field"].ToString(), enumKey);
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

            #region 编辑列表按钮事件
            if (isColumnEdit)
            {
                if (showPager)
                {
                    var str = @"
                    var showConfirm = true;var loadData = {};
                    function onDataGridBeforeload(e) {
                        var grid = e.sender;
                        loadData = $.extend({}, e.data);
                        if (grid.getChanges().length > 0 && showConfirm) {
                            e.cancel = true;
                            msgUI('你有未保存的数据，是否继续操作？', 2, function (action) {
                                if (action == 'ok') {
                                    showConfirm = false;
                                    grid.load(loadData, function () { showConfirm = true; loadData = {} }, function () { showConfirm = true; loadData = {}});
                                }
                            });
                        }
                    }
";
                    sb.AppendLine(str);
                }
                var isTreeGrid = IsTreeGrid(listDef.LayoutGrid) ? "isTreeGrid:true" : "isTreeGrid:false";
                sb.Append(@"
                    function commitGridEdit(gridId) {
                        if(!gridId||typeof(gridId)=='object') gridId=normalParamSettings.gridId; 
                        var grid = mini.get(gridId);        
                        grid.commitEdit();grid.validate();
                    }
                    function moveUp(gridId) {
                        if(!gridId||typeof(gridId)=='object') gridId=normalParamSettings.gridId; 
                        var dataGrid = mini.get(gridId);
                        var rows = dataGrid.getSelecteds();
                        dataGrid.moveUp(rows);
                    }
                    function moveDown(gridId) {
                        if(!gridId||typeof(gridId)=='object') gridId=normalParamSettings.gridId; 
                        var dataGrid = mini.get(gridId);
                        var rows = dataGrid.getSelecteds();
                        dataGrid.moveDown(rows);
                    }    
                    function addEditRow() {
                        var defaultData = $.extend(true, {}, DefaultValueRowDic); 
                        addRow(defaultData, { isLast: " + (showPager ? "false" : "true") + ", " + isTreeGrid + @" });
                    }
                    function deleteEditRow(normalSettings) {
                        normalSettings = $.extend(true, {" + isTreeGrid + @"}, normalSettings); 
                        delRow(normalSettings);
                    }
                    function saveEditList(normalSettings) {
                        var settings = $.extend(true, {}, executeParamSettings, normalSettings);
                        normalSettings = $.extend(true, {" + isTreeGrid + @"}, normalSettings); 
                        var grid = mini.get(settings.gridId);
                        var deleteList = grid.getChanges(""removed"");
                        if(deleteList.length>0)
                        {
                            msgUI(""存在删除的数据，是否继续保存？"", 2, function (result) {
                                if (result != ""ok"") { return; }
                                " + (showPager ? "saveList" : "saveSortedList") + @"(normalSettings);
                            });
                        }
                        else
                            " + (showPager ? "saveList" : "saveSortedList") + @"(normalSettings);
                    }
                    function clientSearch(searchFields, normalSettings) {
                        var settings = $.extend(true, {}, normalParamSettings, normalSettings);

                        var grid = mini.get(settings.gridId);

                        var keyCo = mini.get(settings.queryBoxId);
                        if (keyCo == undefined) {
                            msgUI(""当前快速查询文本框"" + settings.queryBoxId + ""不存在，请重新检查！"", 1);
                            return;
                        }
                        var searchValues = keyCo.getValue().toLowerCase().replace(/，/g, ',');
                        if (grid != undefined)
                            grid.filter(function (row) {
                                //逗号 或关系
                                var result = false;
                                var keys = searchFields.split(',');
                                var values = searchValues.split(',');
                                for (i = 0; i < keys.length; i++) {
                                    for (var j = 0; j < values.length; j++) {
                                        result = result || String(row[keys[i]]).toLowerCase().indexOf(values[j]) != -1;
                                    }
                                }
                                return result;
                            });
                    }
                    ");
            }
            #endregion

            #region 数据源
            var defaultValueRows = GetDefaultValueDic(listDef.DefaultValueSettings);
            foreach (var key in defaultValueRows.Keys)
            {
                var guid = new Guid();
                if (Guid.TryParse(key, out guid) == false)
                    sb.AppendFormat("\n var {0}={1}", key, JsonHelper.ToJson(defaultValueRows[key]));
            }
            #endregion

            #region 默认值列表
            if (isColumnEdit)
            {
                //默认值Dic
                var defaultDic = new Dictionary<string, string>();
                foreach (var field in fields)
                {
                    var _ItemType = field.GetValue("Edit_ItemType");
                    var _DefaultValue = field.GetValue("Edit_DefaultValue");
                    var _Code = field.GetValue("field");
                    if (string.IsNullOrEmpty(_DefaultValue))
                        continue;
                    if (_DefaultValue.Contains(',') && _ItemType == "ButtonEdit")
                    {
                        defaultDic.Add(_Code, GetDefaultValue(_Code, _DefaultValue.Split(',').First(), defaultValueRows));
                        defaultDic.Add(_Code + "Name", GetDefaultValue(_Code, _DefaultValue.Split(',').Last(), defaultValueRows));
                    }
                    else
                    {
                        defaultDic.Add(_Code, GetDefaultValue(_Code, _DefaultValue, defaultValueRows));
                    }
                }
                sb.AppendFormat("\n var {0}={1};", "DefaultValueRowDic", JsonHelper.ToJson(defaultDic));
            }
            #endregion

            return sb.ToString() + "\n" + HttpContext.Current.Server.HtmlDecode(listDef.ScriptText);

        }

        private string GetQueryItemHtml(S_UI_List listDef, Dictionary<string, object> field, bool showQueryForm = false)
        {
            string mode = field.ContainsKey("QueryMode") ? field["QueryMode"].ToString() : "";
            string queryMode = getQueryMode(mode);

            string code = field["field"].ToString();
            string name = field["header"].ToString();
            var LGID = FormulaHelper.GetCurrentLGID();
            if (field.ContainsKey("Header" + LGID)) //双语言支持
                name = field["Header" + LGID].ToString();

            string miniCls = field["ItemType"].ToString().ToLower();
            string dataPty = "";
            var isColumnEdit = false; var showPager = true;
            if (!string.IsNullOrEmpty(listDef.LayoutGrid))
            {
                var settings = JsonHelper.ToObject(listDef.LayoutGrid);
                if (settings.GetValue("allowCellEdit") == "true")
                    isColumnEdit = true;
                if (settings.GetValue("showPager") == "false")
                    showPager = false;
            }
            if (!isColumnEdit)
            {
                if (field.ContainsKey("Settings"))
                {
                    var settings = JsonHelper.ToObject(field["Settings"].ToString());
                    if (settings.ContainsKey("EnumKey"))
                    {
                        dataPty = string.Format(" data='{0}' ", GetEnumKey(listDef.TableNames.Split(',')[0], code, settings["EnumKey"].ToString()));
                    }
                }
            }
            else
            {
                if (field.ContainsKey("Edit_Settings"))
                {
                    var settings = JsonHelper.ToObject(field["Edit_Settings"].ToString());
                    if (settings.ContainsKey("data"))
                    {
                        dataPty = string.Format(" data='{0}' ", GetEnumKey(listDef.TableNames.Split(',')[0], code, settings["data"].ToString()));
                    }
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

        private string GetFilterInputHtml(Dictionary<string, object> field)
        {
            string _fieldName = field.ContainsKey("field") ? field["field"].ToString() : "";
            string _queryType = field.ContainsKey("QueryMode") ? field["QueryMode"].ToString() : "";
            string _dataType = "S";
            if (field.ContainsKey("Settings"))
            {
                var _settings = JsonHelper.ToObject(field["Settings"].ToString());
                //枚举
                if (_settings.ContainsKey("EnumKey") && !string.IsNullOrWhiteSpace(_settings["EnumKey"].ToString()))
                {
                    string _key = _settings["EnumKey"].ToString();
                    _key = GetEnumKey("", field["field"].ToString(), _key);
                    if (!string.IsNullOrWhiteSpace(_key))
                        return string.Format("<input id='{0}' property='filter' class='mini-filteredit' filterData='{2}.concat(headerDefault)' style='width:100%;' showClose='true' onvaluechanged='gridFilter(this,\"equal\",\"{1}\",{2});' />", _fieldName,_dataType, _key);
                }
                //数值/日期/枚举
                if (_settings.ContainsKey("dataType") && !string.IsNullOrWhiteSpace(_settings["dataType"].ToString()))
                {
                    string _type = _settings["dataType"].ToString().ToLower();
                    if (_type == "int" || _type == "float" || _type == "currency")
                        _dataType = "N";
                    else if (_type == "date")
                        _dataType = "D";
                    else if (_type == "boolean")
                        _dataType = "B";
                }
            }
            switch (_dataType)
            {
                case "N":
                case "D":
                    return string.Format("<input id='{0}' property='filter' class='mini-filteredit' filterData='headerFilters' style='width:100%;' showClose='true' onvaluechanged='gridFilter(this,\"between\",\"{1}\");' />", _fieldName,_dataType);
                case "B":
                    return string.Format("<input id='{0}' property='filter' class='mini-filteredit' filterData='headerDefault' style='width:100%;' showClose='true' onvaluechanged='gridFilter(this,\"equal\",\"{1}\");' />", _fieldName,_dataType);
                default:
                    return string.Format("<input id='{0}' property='filter' class='mini-filteredit' filterData='headerDefault' style='width:100%;' showClose='true' onvaluechanged='gridFilter(this,\"like\",\"{1}\");' />", _fieldName,_dataType);
            }
        }

        #region 右键按钮的HTML
        public string GetGridRightKeyHtml(string tmplCode)
        {
            StringBuilder sb = new StringBuilder();
            var menuUl = "<ul id='treeMenu' class='mini-contextmenu'>{0}</ul>";
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == tmplCode);
            if (listDef == null)
                throw new Exception(string.Format("编号为：“{0}”的列表不存在", tmplCode));
            var buttons = JsonHelper.ToList(listDef.LayoutButton);
            foreach (var item in buttons)
            {
                if (item.ContainsKey("Settings"))
                {
                    var sets = JsonHelper.ToObject(item["Settings"].ToString());
                    var buttonType = sets.GetValue("ButtonType");
                    if (!string.IsNullOrEmpty(buttonType) && buttonType == "rightKey" && item.GetValue("Enabled") == "true")
                    {
                        string onclick = "";
                        if (item.ContainsKey("URL") && !string.IsNullOrEmpty(item["URL"].ToString()))
                        {
                            onclick = "onclick='openWindow(\"" + item["URL"] + "\"";
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
                            onclick += "," + JsonHelper.ToJson(dic);

                            if (sets.ContainsKey("onclick") && !string.IsNullOrWhiteSpace(sets["onclick"].ToString())) //如果有自定义的按钮onclick
                                onclick = "";

                            if (onclick != "")
                                onclick += ");'";
                        }
                        else
                        {
                            var oc = sets.GetValue("onclick");
                            if (!string.IsNullOrEmpty(oc))
                                onclick = string.Format("onclick='{0}'", oc);
                        }

                        sb.AppendFormat("<li name='{0}' iconcls='{1}' {2}>{3}</li>"
                            , item.GetValue("id")
                            , item.GetValue("iconcls")
                            , onclick
                            , item.GetValue("text"));
                    }
                }
            }

            return string.Format(menuUl, sb.ToString());
        }
        #endregion

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
            {
                var LGID = FormulaHelper.GetCurrentLGID();
                if (!string.IsNullOrEmpty(LGID))
                {
                    var enums = JsonHelper.ToObject<List<Dictionary<string, object>>>(data);
                    if (enums.Count > 0 && enums.Where(c => c.Keys.Contains("textEN")).Count() > 0)
                    {
                        foreach (var item in enums)
                        {
                            var text = item.GetValue("textEN");
                            item.SetValue("text", text);
                            item.Remove("textEN");
                        }
                        result = JsonHelper.ToJson(enums);
                    }
                    else
                        result = data;
                }
                else
                    result = data;
            }
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

        private string GetEnumKey(string tableName, string fieldCode, string data)
        {
            bool fromMeta = false;
            if (data.StartsWith("[") == false)
            {
                var arr = data.Split(',');
                if (arr.Length == 3) //如果data为ConnName,tableName,fieldCode时
                {
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
            return string.Format(" data='{0}'", GetEnumKey(tableName, fieldCode, data));
        }

        #endregion

        #region Settings处理

        public string GetMiniuiSettings(string settings, bool isTreeGrid = false)
        {
            if (string.IsNullOrEmpty(settings))
                return "";
            var dic = JsonHelper.ToObject(settings);
            return GetMiniuiSettings(dic, isTreeGrid);
        }

        public string GetMiniuiSettings(Dictionary<string, object> dic, bool isTreeGrid = false)
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
                else if (key == "Settings" && dic["Settings"] != null && !string.IsNullOrEmpty(dic["Settings"].ToString())) //如果checkbox，存在setting，则需要根据setting设置相关属性
                {
                    var setting = JsonHelper.ToObject(dic["Settings"].ToString());
                    if (setting.Keys.Contains("trueValue") && setting["trueValue"] != null && !string.IsNullOrEmpty(setting["trueValue"].ToString()))
                        sb.AppendFormat(" truevalue='{0}'", setting["trueValue"].ToString());
                    if (setting.Keys.Contains("falseValue") && setting["falseValue"] != null && !string.IsNullOrEmpty(setting["falseValue"].ToString()))
                        sb.AppendFormat(" falsevalue='{0}'", setting["falseValue"].ToString());
                }
                else if (key == "showFilterRow" && dic[key].ToString().ToLower() == "true")
                {
                    sb.Append(" showFilterRow=false");//筛选默认关闭
                }
                else if (key == "format" && !String.IsNullOrEmpty(dic.GetValue("decimalPlaces")))
                {
                    //数字控件，设置格式的时候默认是保留2位小数，如果设置了小数位，则格式设置需要拼接小数位
                    sb.Append(String.Format(" format='{0}'", dic.GetValue("format") + dic.GetValue("decimalPlaces")));
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
                    else if (key.ToLower() == "showpager")
                    {
                        if (!isTreeGrid)
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

        public bool IsTreeGrid(string settings)
        {
            if (string.IsNullOrEmpty(settings))
                return false;
            var dic = JsonHelper.ToObject(settings);
            if (dic.GetValue("isTreeGrid") == "true")
                return true;
            else
                return false;
        }

        public string GetTreeGridSettings(string settings, List<Dictionary<string, object>> fields, bool isTreeGrid)
        {
            StringBuilder sb = new StringBuilder();
            if (isTreeGrid)
            {
                if (fields.Count > 0)
                {
                    var field = fields.FirstOrDefault(c => c["Visible"].ToString() == "true");
                    if (field != null)
                        sb.AppendFormat(" treeColumn='{0}'", field.GetValue("field"));
                }
                if (!string.IsNullOrEmpty(settings))
                {
                    var dic = JsonHelper.ToObject(settings);
                    if (string.IsNullOrEmpty(dic.GetValue("parentField")))
                        sb.Append(" parentField='ParentID'");
                    if (string.IsNullOrEmpty(dic.GetValue("idField")))
                        sb.Append(" idField='ID'");
                }
                sb.Append(" resultAsTree='false' ");
                sb.Append(" onload='onTreeGridLoad' ");
                sb.Append(" autoLoad='false' ");
            }
            return sb.ToString();
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
        public string ReplaceDicString(string sql, DataRow row = null, Dictionary<string, object> dic = null,
            Dictionary<string, DataTable> dtDic = null, bool validateCurrentUser = true)
        {
            if (string.IsNullOrEmpty(sql))
                return sql;

            var user = validateCurrentUser ? FormulaHelper.GetUserInfo() : new UserInfo();
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            string result = reg.Replace(sql, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');

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
                    return dic.GetValue(value);

                if (!string.IsNullOrEmpty(HttpContext.Current.Request[value]))
                    return HttpContext.Current.Request[value];

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
                    case Formula.Constant.CurrentUserDeptIDs:
                        return user.UserDeptIDs;
                    case Formula.Constant.CurrentUserOrgIDs:
                        return user.UserOrgIDs;
                    case Formula.Constant.CurrentUserPrjName:
                        return user.UserPrjName;
                    case "CurrentUserOrgFullName":
                        return user.UserFullOrgName;
                    case "CurrentUserCorpID":
                        return user.UserCompanyID;
                    case "CurrentUserCorpName":
                        return user.UserCompanyName;
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
                    case "CurrentUserOrgFullID":
                        return user.UserFullOrgID;
                    case "CurrentUserWorkNo":
                        return user.WorkNo;
                    default:
                        return "";
                }
            });

            return result;
        }

        public string ReplaceString(string sql, DataRow row = null, Dictionary<string, string> dic = null,
           Dictionary<string, DataTable> dtDic = null, bool validateCurrentUser = true)
        {
            if (string.IsNullOrEmpty(sql))
                return sql;

            var user = validateCurrentUser ? FormulaHelper.GetUserInfo() : new UserInfo();
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            string result = reg.Replace(sql, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');

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

                if (!string.IsNullOrEmpty(HttpContext.Current.Request[value]))
                    return HttpContext.Current.Request[value];

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
                    case Formula.Constant.CurrentUserDeptIDs:
                        return user.UserDeptIDs;
                    case Formula.Constant.CurrentUserOrgIDs:
                        return user.UserOrgIDs;
                    case Formula.Constant.CurrentUserPrjName:
                        return user.UserPrjName;
                    case "CurrentUserOrgFullName":
                        return user.UserFullOrgName;
                    case "CurrentUserCorpID":
                        return user.UserCompanyID;
                    case "CurrentUserCorpName":
                        return user.UserCompanyName;
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
                    case "CurrentUserOrgFullID":
                        return user.UserFullOrgID;
                    case "CurrentUserWorkNo":
                        return user.WorkNo;
                    default:
                        return "";
                }
            });

            return result;
        }
        #endregion

        #region Word导出

        public DataSet GetWordDataSource(string code, string formInstanceId, bool validateCurrentUser = true)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var word = GetWordDef(code, formInstanceId);


            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(word.ConnName);

            //word导出支持加密字段
            DataTable dtTmpl = sqlHelper.ExecuteDataTable(string.Format("SELECT * FROM ({0}) T WHERE 1=2", word.SQL));


            string sql = string.Format("select * from ({0}) a where ID='{1}'", word.SQL, formInstanceId);
            //列表定义支持加密字段
            if (System.Configuration.ConfigurationManager.AppSettings["FieldEncryption"] == "True")
            {
                StringBuilder sbField = new StringBuilder();
                foreach (DataColumn col in dtTmpl.Columns)
                {
                    if (col.DataType == typeof(byte[]))
                    {
                        sbField.AppendFormat(",{0}=convert(nvarchar(500), DecryptByKey({0}))", col.ColumnName);
                    }
                    else
                    {
                        sbField.AppendFormat(",{0}", col.ColumnName);
                    }
                }
                sql = string.Format("select {2} from ({0}) a where ID='{1}'", word.SQL, formInstanceId, sbField.ToString().Trim(','));
                //sqlserver列级加密
                sql = @"
OPEN SYMMETRIC KEY SymmetricByCert
DECRYPTION BY CERTIFICATE JinHuiCertificate 
" + sql;
            }


            DataTable dtForm = sqlHelper.ExecuteDataTable(sql);

            DataSet ds = new DataSet("dataSet1");
            if (word.Description == "FormWord")
            {
                ds = GetFormWordDataSource(code, formInstanceId, validateCurrentUser); //表单定义的Word导出数据    
                dtForm = ds.Tables["dtForm"];
            }
            else
            {
                dtForm.TableName = "dtForm";
                if (dtForm.Rows.Count == 0)
                    throw new Exception("不存在ID为：" + formInstanceId + "的记录！");
                ds.Tables.Add(dtForm);
            }
            var enumService = FormulaHelper.GetService<IEnumService>();

            if (!string.IsNullOrEmpty(word.Items))
            {

                var items = JsonHelper.ToObject<List<Dictionary<string, string>>>(word.Items);
                foreach (var item in items)
                {
                    string fieldCode = item["Code"];


                    if (item["ItemType"] == "Datetime")
                    {
                        object fieldValue = dtForm.Rows[0][fieldCode];
                        dtForm.Columns.Remove(fieldCode);
                        dtForm.Columns.Add(fieldCode);
                        if (fieldValue is DBNull || string.IsNullOrEmpty(fieldValue.ToString()))
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

                        IList<DicItem> enumData = GetEnum(word.ConnName,
                            !string.IsNullOrEmpty(word.TableNames) ? word.TableNames.Split(',')[0] : "", item["Code"],
                            settings.ContainsKey("data") ? settings["data"] : "");

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

                        sql = ReplaceString(formData["SQL"], dtForm.Rows[0], null, null, false);

                        //word导出子表支持加密字段
                        if (System.Configuration.ConfigurationManager.AppSettings["FieldEncryption"] == "True")
                        {
                            dtTmpl = sqlHelper.ExecuteDataTable(string.Format("SELECT * FROM ({0}) T WHERE 1=2", sql));
                            StringBuilder sbField = new StringBuilder();
                            foreach (DataColumn col in dtTmpl.Columns)
                            {
                                if (col.DataType == typeof(byte[]))
                                {
                                    sbField.AppendFormat(",{0}=convert(nvarchar(500), DecryptByKey({0}))", col.ColumnName);
                                }
                                else
                                {
                                    sbField.AppendFormat(",{0}", col.ColumnName);
                                }
                            }
                            sql = @"
OPEN SYMMETRIC KEY SymmetricByCert
DECRYPTION BY CERTIFICATE JinHuiCertificate 
" + sql;
                        }
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
                        if (ds.Tables.Contains(fieldCode))//如果报错表单定义
                            ds.Tables.Remove(fieldCode);
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
                                    string tableName = (!string.IsNullOrEmpty(word.TableNames) ? (word.TableNames.Split(',')[0] + "_") : "") + item["Code"];
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
            }

            #region word导出流程意见

            if (System.Configuration.ConfigurationManager.AppSettings["Flow_WordExportComment"] == "True")
            {
                var dtFlowComments = GetFlowComment(formInstanceId);
                if (ds.Tables.Contains(dtFlowComments.TableName)) //存在则移除
                    ds.Tables.Remove(dtFlowComments.TableName);
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
            if (dt.Rows.Count == 0)
                return dt;


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
                    var userOrgName = string.Empty;
                    var user = userService.GetUserInfoByID(row["TaskUserID"].ToString());
                    if (user != null)
                        userOrgName = user.UserOrgName;
                    row["TaskUserDept"] = userOrgName;
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
        /// <param name="formInstanceId"></param>
        /// <returns></returns>
        private DataSet GetFormWordDataSource(string code, string formInstanceId, bool validateCurrentUser = true)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var form = GetFormDef(code, formInstanceId);

            var enumService = FormulaHelper.GetService<IEnumService>();

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(form.ConnName);

            DataTable dtTmpl = sqlHelper.ExecuteDataTable(string.Format("SELECT * FROM {0} WHERE 1=2", form.TableName));

            string sql = string.Format("select * from {0} where ID='{1}'", form.TableName, formInstanceId);

            //列表定义支持加密字段
            if (System.Configuration.ConfigurationManager.AppSettings["FieldEncryption"] == "True")
            {
                StringBuilder sbField = new StringBuilder();
                foreach (DataColumn col in dtTmpl.Columns)
                {
                    if (col.DataType == typeof(byte[]))
                    {
                        sbField.AppendFormat(",{0}=convert(nvarchar(500), DecryptByKey({0}))", col.ColumnName);
                    }
                    else
                    {
                        sbField.AppendFormat(",{0}", col.ColumnName);
                    }
                }
                sql = string.Format("select {2} from {0} where ID='{1}'", form.TableName, formInstanceId, sbField.ToString().Trim(','));
                //sqlserver列级加密
                sql = @"
OPEN SYMMETRIC KEY SymmetricByCert
DECRYPTION BY CERTIFICATE JinHuiCertificate 
" + sql;
            }


            //string sql = string.Format("select * from {0} a where ID='{1}'", form.TableName, formInstanceId);
            DataTable dtForm = sqlHelper.ExecuteDataTable(sql);
            dtForm.TableName = "dtForm";
            if (dtForm.Rows.Count == 0)
                throw new Exception("不存在ID为：" + formInstanceId + "的记录！");

            DataSet ds = new DataSet("dataSet1");
            ds.Tables.Add(dtForm);

            //处理数据源
            var dataSourceDic = GetDefaultValueDic(form.DefaultValueSettings, null, false);

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
                    if (dtForm.Columns.Contains("_" + fieldCode) == false)
                        dtForm.Columns.Add("_" + fieldCode);
                    dtForm.Rows[0]["_" + fieldCode] = fileName;
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

                    if (dataSourceDic.ContainsKey(settings["data"]))
                    {
                        var dtEnum = dataSourceDic[settings["data"]];
                        var value = dtForm.Rows[0][fieldCode].ToString();
                        foreach (DataRow row in dtEnum.Rows)
                        {
                            if (row["value"].ToString() == value)
                                dtForm.Rows[0][fieldCode] = row["text"].ToString();
                        }
                    }
                    else
                    {
                        IList<DicItem> enumData = GetEnum(form.ConnName, form.TableName, fieldCode, settings["data"]);

                        var value = dtForm.Rows[0][fieldCode].ToString();
                        foreach (var d in enumData)
                        {
                            if (d.Value == value)
                                dtForm.Rows[0][fieldCode] = d.Text;
                        }
                    }

                    #endregion
                }
                else if (item["ItemType"] == "SubTable")
                {
                    var settings = JsonHelper.ToObject<Dictionary<string, object>>(item["Settings"]);
                    var formData = JsonHelper.ToObject<Dictionary<string, string>>(settings["formData"].ToString());
                    var listData = JsonHelper.ToObject<List<Dictionary<string, string>>>(settings["listData"].ToString());

                    //word导出子表支持加密字段
                    if (System.Configuration.ConfigurationManager.AppSettings["FieldEncryption"] == "True")
                    {
                        dtTmpl = sqlHelper.ExecuteDataTable(string.Format("SELECT * FROM {0}_{1} WHERE 1=2", form.TableName, fieldCode));
                        StringBuilder sbField = new StringBuilder();
                        foreach (DataColumn col in dtTmpl.Columns)
                        {
                            if (col.DataType == typeof(byte[]))
                            {
                                sbField.AppendFormat(",{0}=convert(nvarchar(500), DecryptByKey({0}))", col.ColumnName);
                            }
                            else
                            {
                                sbField.AppendFormat(",{0}", col.ColumnName);
                            }
                        }

                        sql = string.Format("select {3} from {0}_{1} where {0}ID = '{2}' order by sortIndex", form.TableName, fieldCode, formInstanceId, sbField.ToString().Trim(','));

                        sql = @"
OPEN SYMMETRIC KEY SymmetricByCert
DECRYPTION BY CERTIFICATE JinHuiCertificate 
" + sql;
                    }
                    else
                    {
                        sql = string.Format("select * from {0}_{1} where {0}ID = '{2}' order by sortIndex", form.TableName, fieldCode, formInstanceId);
                    }

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
                var dtFlowComments = GetFlowComment(formInstanceId);
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
            var listDef = entities.Set<S_UI_List>().SingleOrDefault(c => c.Code == code);
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

        public string CreateReleaseFormSql(string id)
        {
            var entities = FormulaHelper.GetEntities<BaseEntities>();
            var entity = entities.Set<S_UI_Form>().SingleOrDefault(c => c.ID == id);
            var items = JsonHelper.ToObject<List<FormItem>>(entity.Items ?? "[]");
            StringBuilder sb = new StringBuilder();
            sb.Append(CreateTable(entity.TableName, entity.Name));

            #region 处理移动端新添加的字段
            var mobileItems = JsonHelper.ToObject<List<Dictionary<string, object>>>(entity.MobileItems ?? "[]");
            if (mobileItems != null && mobileItems.Count > 0)
            {
                foreach (var item in mobileItems)
                {
                    var key = item.GetValue("Key");
                    if (!string.IsNullOrEmpty(key))
                    {
                        var valueType = item.GetValue("ValueType");
                        var settings = item.GetValue("Settings");
                        if (items.Where(c => c.Code == key).Count() <= 0)
                        {
                            items.Add(new FormItem
                            {
                                ID = FormulaHelper.CreateGuid(),
                                Code = key,
                                Name = item.GetValue("KeyName"),
                                FieldType = valueType == "date" ? "datetime" : "nvarchar(2000)",
                                ItemType = valueType,
                                Enabled = "true",
                                Visible = "true",
                                Settings = settings
                            });
                        }
                        else
                        {
                            //处理子表、行固定子表中新添加的字段
                            if (valueType == "table")
                            {
                                var tableItem = items.SingleOrDefault(c => c.Code == key);
                                var tableItems = JsonHelper.ToObject<List<Dictionary<string, object>>>(settings ?? "[]");
                                var subTableItems = getSubTableItem(tableItem.Settings);
                                foreach (var tItem in tableItems)
                                {
                                    var tCode = tItem.GetValue("Code");
                                    if (subTableItems.Where(c => c.Code == tCode).Count() <= 0)
                                    {
                                        subTableItems.Add(new FormItem
                                        {
                                            ID = FormulaHelper.CreateGuid(),
                                            Code = tCode,
                                            Name = tItem.GetValue("Name"),
                                            FieldType = tItem.GetValue("ValueType") == "date" ? "datetime" : "nvarchar(2000)",
                                            ItemType = tItem.GetValue("ValueType"),
                                            Enabled = "true",
                                            Visible = "true",
                                            Settings = tItem.GetValue("Settings")
                                        });
                                    }
                                }
                                var subTableSettings = JsonHelper.ToObject<Dictionary<string, object>>(tableItem.Settings);
                                subTableSettings.SetValue("listData", JsonHelper.ToJson(subTableItems));
                                tableItem.Settings = JsonHelper.ToJson(subTableSettings);
                            }
                            else if (valueType == "complexTable")
                            {
                                var tableItem = items.SingleOrDefault(c => c.Code == key);
                                var complexTableItemsObj = JsonHelper.ToObject<Dictionary<string, object>>(settings);
                                var list = complexTableItemsObj.GetValue("list");
                                if (!string.IsNullOrEmpty(list))
                                {
                                    var complexTableItems = JsonHelper.ToObject<List<Dictionary<string, object>>>(list ?? "[]");
                                    var subTableItems = getSubTableItem(tableItem.Settings);
                                    foreach (var ctItem in complexTableItems)
                                    {
                                        var ctCode = ctItem.GetValue("Code");
                                        if (subTableItems.Where(c => c.Code == ctCode).Count() <= 0)
                                        {
                                            subTableItems.Add(new FormItem
                                            {
                                                ID = FormulaHelper.CreateGuid(),
                                                Code = ctCode,
                                                Name = ctItem.GetValue("Name"),
                                                FieldType = ctItem.GetValue("ValueType") == "date" ? "datetime" : "nvarchar(2000)",
                                                ItemType = ctItem.GetValue("ValueType"),
                                                Enabled = "true",
                                                Visible = "true",
                                                Settings = ctItem.GetValue("Settings")
                                            });
                                        }
                                    }
                                    var subTableSettings = JsonHelper.ToObject<Dictionary<string, object>>(tableItem.Settings);
                                    subTableSettings.SetValue("listData", JsonHelper.ToJson(subTableItems));
                                    tableItem.Settings = JsonHelper.ToJson(subTableSettings);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            var foreignkeys = new List<Dictionary<string, object>>();
            if (!Config.Constant.IsOracleDb)
            {
                //此处先不处理Oracle 数据库
                //判定表中的所有外键数据，外键的数据类型一定设置为 nvarchar(50)，因为规范规定主键必须是nvarchar(50)
                //by Eric.Yang  2019-4-25
                var sqlDB = SQLHelper.CreateSqlHelper(entity.ConnName);
                var fkDt = sqlDB.ExecuteDataTable(String.Format(@"SELECT Field=(SELECT name FROM syscolumns WHERE colid=b.fkey AND id=b.fkeyid) ,
FKTable=object_name(b.rkeyid),
FKKeyField=(SELECT name FROM syscolumns WHERE colid=b.rkey AND id=b.rkeyid) 
FROM sysobjects a 
join sysforeignkeys b on a.id=b.constid 
join sysobjects c on a.parent_obj=c.id 
where a.xtype='f' AND c.xtype='U' 
and object_name(b.fkeyid)='{0}'", entity.TableName));
                foreignkeys = FormulaHelper.DataTableToListDic(fkDt);
            }

            foreach (var item in items)
            {
                if (!foreignkeys.Exists(c => c.GetValue("Field") == item.Code))
                {
                    //外键字段，不修改该字段，因为外键一定为nvarchar(50)，所以这里设置的数据类型可能不是nvarchar(50)
                    //在变更的过程中会报错，所以此次判定如果有外键，则不设置变更数据库   by Eric.Yang  2019-4-25
                    sb.Append(CreateField(entity.TableName, item.Code, item.FieldType, item.Name));
                }

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
                else if (item.ItemType == "ComboBox")
                {
                    var settings = JsonHelper.ToObject(item.Settings ?? "{}");
                    if (settings.ContainsKey("textName") && settings["textName"].ToString() != "")
                        sb.Append(CreateField(tableName, settings["textName"].ToString(), item.FieldType, item.Name + "名称"));
                }
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
        public DataTable GetFieldInfo(S_UI_Form form)
        {
            var itemList = form.Items;
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

        #region 表单打印

        public string FormToPrintHtml(string code, string formInstanceId)
        {
            if (string.IsNullOrEmpty(code))
                throw new BusinessException("缺少表单定义编号！");
            if (string.IsNullOrEmpty(formInstanceId))
                throw new BusinessException("缺少表单ID！");

            var entities = FormulaHelper.GetEntities<BaseEntities>();

            var formInfo = GetFormDef(code, formInstanceId);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
            var dt = sqlHelper.ExecuteDataTable(string.Format("select * from {0} where ID='{1}'", formInfo.TableName, formInstanceId));
            var formDataRow = dt.Rows[0];

            var items = JsonHelper.ToObject<List<FormItem>>(formInfo.Items);
            Regex reg = new Regex(uiRegStr);

            string layout = "";
            if (FormulaHelper.GetCurrentLGID() == "EN" && !string.IsNullOrEmpty(formInfo.LayoutEN))
                layout = formInfo.LayoutEN;
            else
                layout = formInfo.Layout;

            if (!string.IsNullOrWhiteSpace(formInfo.LayoutPrint))
                layout = formInfo.LayoutPrint;

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

                var fieldValue = "";
                if (dt.Columns.Contains(item.Code))
                    fieldValue = formDataRow[item.Code].ToString();//字段的值

                if (string.IsNullOrEmpty(item.ItemType))
                {
                    return fieldValue;
                }

                var itemSettings = JsonHelper.ToObject(item.Settings);
                if (item.Visible == "false")
                    return "";
                switch (item.ItemType)
                {
                    case "TextBox":
                        return string.Format("<div id='{1}'>{0}</div>", fieldValue, item.Code);
                    case "TextArea":
                        var height = "100px";
                        if (itemSettings.ContainsKey("style_height"))
                            height = itemSettings["style_height"].ToString();
                        return string.Format("<div class='textareaDiv' id='{2}' style='height:{1};'>{0}</div>", fieldValue, height, item.Code);
                    case "DatePicker":
                        if (fieldValue != "")
                            return string.Format("<div id='{1}'>{0}</div>", DateTime.Parse(fieldValue).ToString(itemSettings.ContainsKey("format") ? itemSettings["format"].ToString() : "yyyy-MM-dd"), item.Code);
                        else
                            return "";
                    case "CheckBoxList":
                    case "RadioButtonList":
                        return string.Format("<div id='{1}'>{0}</div>", FormToPrintHtmlCheckBoxList(itemSettings, fieldValue), item.Code);
                    case "ComboBox":
                        return string.Format("<div id='{1}'>{0}</div>", FormToPrintHtmlComboBox(itemSettings, fieldValue), item.Code);
                    case "ButtonEdit":
                        return string.Format("<div id='{1}'>{0}</div>", formDataRow[item.Code + "Name"].ToString(), item.Code);
                    case "SingleFile":
                    case "MultiFile":
                        return string.Format("<div id='{1}'>{0}</div>", FormToPrintHtmlFile(fieldValue), item.Code);
                    case "SubTable":
                        return string.Format("{0}", FormToPrintHtmlSubTable(formInfo, item, formInstanceId), item.Code);
                    case "AuditSign":
                        return string.Format("<div class='auditSignDiv' id='{1}'>{0}</div>", FormToPrintHtmlAuditSign(itemSettings, fieldValue), item.Code);
                    default:
                        return string.Format("<div id='{1}'>{0}</div>", fieldValue, item.Code);
                }

            });

            return result + @"
<script type='text/javascript'>
" + formInfo.WebPrintJS
            + @"
</script>
";
        }



        private string FormToPrintHtmlCheckBoxList(Dictionary<string, object> settings, string fieldValue)
        {
            StringBuilder sb = new StringBuilder();

            var arrFieldValue = fieldValue.Split(',');
            var data = settings["data"].ToString();
            if (data.StartsWith("["))
            {
                var list = JsonHelper.ToList(data);
                foreach (var item in list)
                {
                    string check = arrFieldValue.Contains(item["value"].ToString()) ? "✓" : "✗";
                    sb.AppendFormat(",{0}{1}", check, item["text"]);
                }
            }
            else
            {
                var dt = EnumBaseHelper.GetEnumTable(data);
                foreach (DataRow row in dt.Rows)
                {
                    string check = arrFieldValue.Contains(row["value"].ToString()) ? "✓" : "✗";
                    sb.AppendFormat(",{0}{1}", row["text"]);
                }
            }

            return sb.ToString().Trim(',');
        }

        private string FormToPrintHtmlComboBox(Dictionary<string, object> settings, string fieldValue)
        {
            StringBuilder sb = new StringBuilder();

            var arrFieldValue = fieldValue.Split(',');
            var data = settings["data"].ToString();
            if (data.StartsWith("["))
            {
                var list = JsonHelper.ToList(data);
                foreach (var item in list)
                {
                    if (arrFieldValue.Contains(item["value"].ToString()))
                        sb.AppendFormat(",{0}", item["text"]);
                }
            }
            else
            {
                var dt = EnumBaseHelper.GetEnumTable(data);
                foreach (DataRow row in dt.Rows)
                {
                    if (arrFieldValue.Contains(row["value"].ToString()))
                        sb.AppendFormat(",{0}", row["text"]);
                }
            }

            return sb.ToString().Trim(',');
        }

        private string FormToPrintHtmlFile(string fieldValue)
        {
            if (fieldValue.Contains(','))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in fieldValue.Split(','))
                {
                    sb.AppendFormat("<div>{0}</div>", item.Substring(item.IndexOf('_') + 1));
                }
                return sb.ToString();
            }
            else
            {
                return fieldValue.Substring(fieldValue.IndexOf('_') + 1);
            }
        }

        private string FormToPrintHtmlSubTable(S_UI_Form formInfo, FormItem formItem, string formInstanceId)
        {
            if (string.IsNullOrEmpty(formItem.Settings))
                return "";
            var dic = JsonHelper.ToObject(formItem.Settings);
            if (string.IsNullOrEmpty(dic.GetValue("listData")))
                return "";
            var list = JsonHelper.ToObject<List<FormItem>>(dic["listData"].ToString());
            if (list.Count == 0)
                return "";

            //获取子表数据
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(formInfo.ConnName);
            var dtSubTable = sqlHelper.ExecuteDataTable(string.Format("select * from {0}_{1} where {0}ID='{2}' order by SortIndex", formInfo.TableName, formItem.Code, formInstanceId));

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table class='subTable' width='100%' style='cellpadding:0px;cellspacing:0px;margin:0px;text-align:center;' border='1'>");
            //标题行
            sb.Append("<tr>");
            foreach (var item in list)
            {
                if (item.Visible == "false")
                {
                    continue;
                }
                sb.AppendFormat("<td width='{1}'><b>{0}</b></td>", item.Name, item.width);
            }
            sb.Append("</tr>");

            foreach (DataRow row in dtSubTable.Rows)
            {
                sb.AppendFormat("<tr>");
                foreach (var item in list)
                {
                    string fieldValue = row[item.Code].ToString();

                    var itemSettings = JsonHelper.ToObject(item.Settings);
                    if (item.Visible == "false")
                    {
                        continue;
                    }

                    if (item.ItemType == "DatePicker")
                    {
                        if (fieldValue != "")
                            fieldValue = DateTime.Parse(fieldValue).ToString(itemSettings.ContainsKey("format") ? itemSettings["format"].ToString() : "yyyy-MM-dd");
                    }
                    else if (item.ItemType == "ComboBox")
                    {
                        fieldValue = FormToPrintHtmlComboBox(itemSettings, fieldValue);
                    }
                    else if (item.ItemType == "ButtonEdit")
                    {
                        fieldValue = row[item.Code + "Name"].ToString();
                    }

                    sb.AppendFormat("<td>{0}</td>", fieldValue);
                }
                sb.AppendFormat("</tr>");
            }
            sb.AppendFormat("</table>");

            return sb.ToString();
        }

        private string FormToPrintHtmlAuditSign(Dictionary<string, object> settings, string fieldValue)
        {
            //旧会签模板(无标题)
            var auditSignTmpl_AuditSignTemplete = " <div  style='border:1px solid #828282'>"
                                    + "<table cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;" +
                                       "' width='100%'>$AuditItem</Table></div>";
            var auditSignTmpl_AuditSignTempleteItem = "<tr><td style='width:21%'><td style='width:3%'></td><td style='width:21%'></td>"
                + "<td style='width:21%'></td><td style='width:3%'></td><td style='width:21%'></td></tr>"
                + "<tr><td align='left' colspan='3' >意见：</td></tr>"
                + "<tr><td align='left' colspan='6'>$SignComment</td></tr>"
                + "<tr>"
                + "<td align='right'>负责人签字:</td><td align='center'>&nbsp;</td><td id='$ExecUserID' align='center'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\" width=\"80px\" height=\"30px\"></td>"
                + "<td align='right'>日期:</td><td align='center'>&nbsp;</td><td align='center'>$SignTime</td></tr>"
            + "<tr><td colspan='6'><hr></td></tr>"; //尾行必须要

            //新会签模板（带标题）
            var auditMultiSignTmpl_AuditSignTemplete = " <div  style='border:0px solid #828282'>"
                                    + "<table  cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;table-layout:fixed;" +
                                       "' width='100%'> <tr  style='display:none'><td width='60%'></td><td width='15%'></td><td width='10%'></td><td width='15%'></td></tr> $AuditItem</Table></div>";
            var auditMultiSignTmpl_AuditSignTempleteItem = "<tr><td align='left' colspan='4' >$StepName意见：</td></tr>"
                + "<tr><td align='left' colspan='4'>$SignComment</td></tr>"
                + "<tr>"
                + "<td align='right'>签字:&nbsp;&nbsp;&nbsp;</td><td  id='$ExecUserID' align='left'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\"  height=\"30px\"></td>"
                + "<td align='right'>日期:&nbsp;&nbsp;&nbsp;</td><td align='left'>$SignTime</td></tr>"
                + "<tr><td colspan='4'><hr></td></tr>"; //尾行必须要

            //单签模板（竖）
            var auditSignSingleTmpl_AuditSignTemplete = " <div  style='border:0px solid #828282'>"
                                    + "<table cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;" +
                                       "' width='100%'>$AuditItem</Table></div>";
            var auditSignSingleTmpl_AuditSignTempleteItem = "<tr><td align='right'>$SignTitle：</td><td align='left'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\" width=\"80px\" height=\"30px\"></td></tr>"
                + "<tr><td align='right'>日期：</td><td align='left'>$SignTime</td></tr>"
                + "<tr><td colspan='6'></td></tr>"; //尾行必须要

            //单签模板（横）
            var auditSignSingleHorizontalTmpl_AuditSignTemplete = " <div  style='border:0px solid #828282'>"
                                    + "<table cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;" +
                                       "' width='100%'>$AuditItem</Table></div>";
            var auditSignSingleHorizontalTmpl_AuditSignTempleteItem = "<tr><td align='right' style='width:20%'> 签字：</td><td align='left'  style='width:30%'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\" width=\"80px\" height=\"30px\"></td>"
                + "<td align='right' style='width:20%'>日期：</td><td align='left' style='width:30%'>$SignTime</td></tr>"
            + "<tr><td colspan='4'></td></tr>"; //尾行必须要

            //仅图片
            var auditSignOnlyUserTmpl_AuditSignTemplete = " <div  style='border:0px solid #828282'>"
                                    + "<table cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;" +
                                       "' width='100%'>$AuditItem</Table></div>";
            var auditSignOnlyUserTmpl_AuditSignTempleteItem = "<tr><td align='right'>$SignTitle</td><td align='left'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\" width=\"80px\" height=\"30px\"></td></tr>"
            + "<tr><td colspan='6'></td></tr>"; //尾行必须要

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("auditSignTmpl_AuditSignTemplete", auditSignTmpl_AuditSignTemplete);
            dic.Add("auditSignTmpl_AuditSignTempleteItem", auditSignTmpl_AuditSignTempleteItem);
            dic.Add("auditMultiSignTmpl_AuditSignTemplete", auditMultiSignTmpl_AuditSignTemplete);
            dic.Add("auditMultiSignTmpl_AuditSignTempleteItem", auditMultiSignTmpl_AuditSignTempleteItem);
            dic.Add("auditSignSingleTmpl_AuditSignTemplete", auditSignSingleTmpl_AuditSignTemplete);
            dic.Add("auditSignSingleTmpl_AuditSignTempleteItem", auditSignSingleTmpl_AuditSignTempleteItem);
            dic.Add("auditSignSingleHorizontalTmpl_AuditSignTemplete", auditSignSingleHorizontalTmpl_AuditSignTemplete);
            dic.Add("auditSignSingleHorizontalTmpl_AuditSignTempleteItem", auditSignSingleHorizontalTmpl_AuditSignTempleteItem);
            dic.Add("auditSignOnlyUserTmpl_AuditSignTemplete", auditSignOnlyUserTmpl_AuditSignTemplete);
            dic.Add("auditSignOnlyUserTmpl_AuditSignTempleteItem", auditSignOnlyUserTmpl_AuditSignTempleteItem);


            //获取签字模板名称
            var signTmpl = "auditSignTmpl";
            if (settings.ContainsKey("tmplName"))
                signTmpl = settings["tmplName"].ToString();

            var signTable = JsonHelper.ToList(fieldValue);
            if (signTable.Count == 0)
            {
                var dicRow = new Dictionary<string, object>();
                dicRow.Add("TaskUserID", "");
                dicRow.Add("TaskUserName", "");
                dicRow.Add("ExecUserID", "");
                dicRow.Add("ExecUserName", "");
                dicRow.Add("SignComment", "");
                dicRow.Add("SignTime", "");
                signTable.Add(dicRow);
            }

            //拼html
            StringBuilder sb = new StringBuilder();
            foreach (var item in signTable)
            {
                string rowHtml = dic[signTmpl + "_AuditSignTempleteItem"];
                rowHtml = rowHtml.Replace("$TaskUserID", item["TaskUserID"].ToString());
                rowHtml = rowHtml.Replace("$TaskUserName", item["TaskUserName"].ToString());
                rowHtml = rowHtml.Replace("$ExecUserID", item["ExecUserID"].ToString());
                rowHtml = rowHtml.Replace("$ExecUserName", item["ExecUserName"].ToString());
                rowHtml = rowHtml.Replace("$SignComment", item["SignComment"].ToString());
                rowHtml = rowHtml.Replace("$SignTime", item["SignTime"].ToString());
                sb.Append(rowHtml);
            }
            var html = dic[signTmpl + "_AuditSignTemplete"];
            html = html.Replace("$AuditItem", sb.ToString());

            return html;
        }

        #endregion
    }
}
