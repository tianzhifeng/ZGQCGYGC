<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PivotGrid.aspx.cs" Inherits="DevPivotGrid.PivotGrid" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title><link href='/CommonWebResource/Theme/Default/MiniUI/miniui.css' rel='stylesheet' type='text/css' />
    <link href='/CommonWebResource/Theme/Default/MiniUI/icons.css' rel='stylesheet' type='text/css' />
    <link href='/CommonWebResource/Theme/Default/MiniUI/miniextend.css' rel='stylesheet' type='text/css' />
    <script src="/CommonWebResource/CoreLib/Basic/jQuery/jquery-1.6.2.min.js" type="text/javascript"></script>
    <script src="/CommonWebResource/CoreLib/MiniUI/miniui.js"></script>
    <script src="/PortalLTE/PortalLTEInc.js" type="text/javascript"></script>
    <script type="text/javascript">
        function showDetail(sender, e) {
            var rowIndex = e.RowIndex;
            var colIndex = e.ColumnIndex;
            var url = "/DevPivotGrid/PivotDetail.aspx?TmplCode=" + getQueryString("TmplCode") + "&colIndex=" + colIndex + "&rowIndex=" + rowIndex;
            if (getQueryString("ID"))//自由透视表
                url = "/DevPivotGrid/PivotDetail.aspx?ID=" + getQueryString("ID") + "&colIndex=" + colIndex + "&rowIndex=" + rowIndex;

            var op = 'toolbar=no, menubar=no, scrollbars=yes,top=100, resizable=yes, location=no, status=no';
            op += ',height=' + 600;
            op += ',width=' + 1000;
            op += ',top=' + 50;
            op += ',left=' + 100;
            var win = window.open(url, '', op);
        }
    </script>
</head>
<body style="">
    <form id="form1" runat="server">
    <%=enumHtml %>
    <table width='100%' style='border-style: solid; border-width: 1px; border-color: #bfc0c9; background-color: #f5f5f7; border-bottom-width: 0px'>
        <tr>
            <td style="vertical-align: top">
                <%=queryHtml %>
            </td>
            <td width="80px" style="vertical-align: bottom;">
                <a class='mini-button' onclick='doFilter();' iconcls='icon-search' plain='true'>过滤</a>
                <a class="mini-button" plain="true" onclick="mini.get('queryWindow').show();" iconcls="icon-setting" style="margin-top: 5px; z-index: 100;">设置</a>
            </td>
        </tr>
    </table>
    <div>
        <div style="position: absolute; height: 37px; width: 100%; margin: 0 auto; border-style: solid; border-width: 1px; border-color: #bfc0c9; background-color: rgb(235,236,239);
            box-sizing: border-box; text-align: right; vertical-align: bottom; z-index: -10;">
        </div>
        <div style="position: absolute; height: 100%; width: 100%; border-style: solid; border-width: 1px; border-color: #bfc0c9; box-sizing: border-box; background-color: rgb(241,244,249);
            text-align: right; vertical-align: middle; z-index: -100;">
        </div>
        <input runat="server" id="ColumnIndex" type="hidden" enableviewstate="true" />
        <input runat="server" id="RowIndex" type="hidden" enableviewstate="true" />
        <dx:ASPxPivotGrid ID="ASPxPivotGrid1" runat="server" ClientInstanceName="PivotGrid" OptionsCustomization-AllowFilter="false" OptionsView-ShowColumnTotals="false"
            OptionsView-ShowRowTotals="false" Styles-CellStyle-Font-Size="9pt" Styles-AreaStyle-Font-Size="9pt" Styles-ColumnAreaStyle-Font-Size="9pt" Styles-RowAreaStyle-Font-Size="9pt"
            Styles-DataAreaStyle-Font-Size="9pt" Styles-HeaderStyle-Font-Size="9pt" Font-Size="9pt" Styles-FieldValueStyle-Font-Size="9pt" OptionsView-ShowFilterHeaders="False"
            OptionsView-ShowDataHeaders="False" OptionsPager-EllipsisMode="None" OptionsPager-Position="Bottom" OptionsPager-Summary-Visible="false" Styles-CellStyle-Font-Underline="true"
            Styles-CellStyle-ForeColor="Blue" OptionsPager-RowsPerPage="30" OptionsView-DataHeadersDisplayMode="Popup">
            <Styles>
                <CellStyle Cursor="pointer" />
            </Styles>
            <OptionsFilter NativeCheckBoxes="False" />
        </dx:ASPxPivotGrid>
    </div>
    </form>
    <div id="queryWindow" class="mini-window" title="查询设置" style="width: 690px; height: 400px;">
        <div class="mini-tabs" activeindex="0" style="width: 100%; height: 100%;">
            <div title="过滤">
                <div class='mini-toolbar' style='height: 25px; border-bottom: 0px;'>
                    <table>
                        <tr>
                            <td style='text-align: left;'>
                                <a class='mini-button' iconcls='icon-add' onclick='addFilterRow();' plain="true">添加</a>
                                <a class='mini-button' iconcls='icon-remove' onclick='delRow("FilterItems");' plain="true">移除</a>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="mini-fit">
                    <div id='FilterItems' multiselect='true' style='width: 100%; height: 100%' class='mini-datagrid' allowcellvalid='true' multiselect='true' allowcelledit='true'
                        allowcellselect='true' allowunselect='false' showpager="false">
                        <div property='columns'>
                            <div type='checkcolumn'>
                            </div>
                            <div field='FieldName' displayfield="Caption" header='过滤字段' width="150">
                            </div>
                            <div field='QueryType' header='筛选方式' width="100" renderer="onQueryTypeRender" autoshowpopup="true" vtype="required;">
                                <input property='editor' class='mini-combobox' style='width: 100%' multiselect="false" valuefield="value" textfield="text" data="queryType" onitemclick="commitGridEdit('FilterItems');" />
                            </div>
                            <div field='CtrlType' header='控件类型' width="80" renderer="onCtrlTypeRender" autoshowpopup="true" vtype="required;">
                                <input property='editor' class='mini-combobox' style='width: 100%' multiselect="false" valuefield="value" textfield="text" data="ctrlType" onitemclick="commitGridEdit('FilterItems');" />
                            </div>
                            <div field='FieldValue' header='筛选值' width="*">
                                <input property='editor' class='mini-textbox' style="width: 100%;" />
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <table style="width: 100%">
                        <tr>
                            <td width="100px">
                                <a class="mini-button" onclick="reset();" iconcls="icon-refer">恢复默认</a>
                            </td>
                            <td style="text-align: center; vertical-align: middle; margin: 5px;">
                                <a class="mini-button" onclick="doQuery();" iconcls="icon-save">保存</a>
                                <a class="mini-button" onclick="mini.get('queryWindow').hide();" iconcls="icon-close">取消</a>
                            </td>
                            <td width="100px">
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div title="统计">
                <div class='mini-toolbar' style='height: 25px; border-bottom: 0px;'>
                    <table>
                        <tr>
                            <td style='text-align: left;'>
                                <a class='mini-button' iconcls='icon-add' onclick='addDataRow();' plain="true">添加</a>
                                <a class='mini-button' iconcls='icon-remove' onclick='delRow("DataItems");' plain="true">移除</a>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="mini-fit">
                    <div id='DataItems' multiselect='true' style='width: 100%; height: 100%' class='mini-datagrid' allowcellvalid='true' multiselect='true' allowcelledit='true'
                        allowcellselect='true' allowunselect='false' showpager="false">
                        <div property='columns'>
                            <div type='checkcolumn'>
                            </div>
                            <div field='FieldName' displayfield="Caption" header='数值字段' width="150">
                            </div>
                            <div field='SumType' header='统计方式' width="100" renderer="onSumTypeRender" autoshowpopup="true">
                                <input property='editor' class='mini-combobox' style='width: 100%' multiselect="false" valuefield="value" textfield="text" data="sumType" onitemclick="commitGridEdit('DataItems');" />
                            </div>
                            <div width="*">
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <table style="width: 100%">
                        <tr>
                            <td width="100px">
                                <a class="mini-button" onclick="reset();" iconcls="icon-refer">恢复默认</a>
                            </td>
                            <td style="text-align: center; vertical-align: middle; margin: 5px;">
                                <a class="mini-button" onclick="doQuery();" iconcls="icon-save">保存</a>
                                <a class="mini-button" onclick="mini.get('queryWindow').hide();" iconcls="icon-close">取消</a>
                            </td>
                            <td width="100px">
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div title="行标签">
                <div class='mini-toolbar' style='height: 25px; border-bottom: 0px;'>
                    <table>
                        <tr>
                            <td style='text-align: left;'>
                                <a class='mini-button' iconcls='icon-add' onclick='addRowArea();' plain="true">添加</a>
                                <a class='mini-button' iconcls='icon-remove' onclick='delRow("RowItems");' plain="true">移除</a>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="mini-fit">
                    <div id='RowItems' multiselect='true' style='width: 100%; height: 100%' class='mini-datagrid' allowcellvalid='true' multiselect='true' allowcelledit='true'
                        allowcellselect='true' allowunselect='false' showpager="false">
                        <div property='columns'>
                            <div type='checkcolumn'>
                            </div>
                            <div field='FieldName' displayfield="Caption" header='行标签字段' width="150">
                            </div>
                            <div width="*">
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <table style="width: 100%">
                        <tr>
                            <td width="100px">
                                <a class="mini-button" onclick="reset();" iconcls="icon-refer">恢复默认</a>
                            </td>
                            <td style="text-align: center; vertical-align: middle; margin: 5px;">
                                <a class="mini-button" onclick="doQuery();" iconcls="icon-save">保存</a>
                                <a class="mini-button" onclick="mini.get('queryWindow').hide();" iconcls="icon-close">取消</a>
                            </td>
                            <td width="100px">
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div title="列标签">
                <div class='mini-toolbar' style='height: 25px; border-bottom: 0px;'>
                    <table>
                        <tr>
                            <td style='text-align: left;'>
                                <a class='mini-button' iconcls='icon-add' onclick='addColumnArea();' plain="true">添加</a>
                                <a class='mini-button' iconcls='icon-remove' onclick='delRow("ColumnItems");' plain="true">移除</a>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="mini-fit">
                    <div id='ColumnItems' multiselect='true' style='width: 100%; height: 100%' class='mini-datagrid' allowcellvalid='true' multiselect='true' allowcelledit='true'
                        allowcellselect='true' allowunselect='false' showpager="false">
                        <div property='columns'>
                            <div type='checkcolumn'>
                            </div>
                            <div field='FieldName' displayfield="Caption" header='列标签字段' width="150">
                            </div>
                            <div width="*">
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <table style="width: 100%">
                        <tr>
                            <td width="100px">
                                <a class="mini-button" onclick="reset();" iconcls="icon-refer">恢复默认</a>
                            </td>
                            <td style="text-align: center; vertical-align: middle; margin: 5px;">
                                <a class="mini-button" onclick="doQuery();" iconcls="icon-save">保存</a>
                                <a class="mini-button" onclick="mini.get('queryWindow').hide();" iconcls="icon-close">取消</a>
                            </td>
                            <td width="100px">
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <div id="selectWin" class="mini-window" title="选择字段" style="width: 300px; height: 400px;">
        <div class='mini-toolbar' style='height: 25px; border-bottom: 0px;'>
            <table>
                <tr>
                    <td style='text-align: left;'>
                        <a class='mini-button' iconcls='icon-ok' onclick='selectField();' plain="true">选择</a>
                        <a class='mini-button' iconcls='icon-cancel' onclick='mini.get("selectWin").hide();' plain="true">取消</a>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </div>
        <div class="mini-fit">
            <div id='fieldGrid' multiselect='true' style='width: 100%; height: 100%' class='mini-datagrid' multiselect='true' showpager="false" allowrowselect="true"
                allowunselect="true">
                <div property='columns'>
                    <div type='checkcolumn'>
                    </div>
                    <div field='FieldName' header='字段编号' width="150">
                    </div>
                    <div field='Caption' header='字段名称' width="*">
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">        
        var userFilterItems=<%=DevPivotGrid.JsonHelper.ToJson(userFilterItems) %>;
        var userDataItems=<%=DevPivotGrid.JsonHelper.ToJson(userDataItems) %>;
        var userRowItems=<%=DevPivotGrid.JsonHelper.ToJson(userRowItems) %>;
        var userColumnItems=<%=DevPivotGrid.JsonHelper.ToJson(userColumnItems) %>;

        var filterItems=<%=DevPivotGrid.JsonHelper.ToJson(filterItems) %>;
        var dataItems=<%=DevPivotGrid.JsonHelper.ToJson(dataItems) %>;
        var rowItems=<%=DevPivotGrid.JsonHelper.ToJson(rowItems) %>;
        var columnItems=<%=DevPivotGrid.JsonHelper.ToJson(columnItems) %>;
    </script>
    <script type="text/javascript">
        var sumType = [{ value: "Count", text: "计数" }, { value: "Sum", text: "求和" }, { value: "Min", text: "最小值" }, { value: "Max", text: "最大值" }, { value: "Average", text: "平均值"}];
        var queryType = [{ value: "in", text: "包含" }, { value: "like", text: "类似于" }, { value: ">", text: "大于" }, { value: "<", text: "小于" }, { value: ">=", text: "大于等于" }, { value: "<=", text: "小于等于" }, { value: "between", text: "介于"}];
        var ctrlType = [{ value: "text", text: "文本框" }, { value: "date", text: "日期框" }, { value: "dropdown", text: "下拉框"}];

        var textQueryType = [{ value: "like", text: "类似于" }, { value: ">", text: "大于" }, { value: "<", text: "小于" }, { value: ">=", text: "大于等于" }, { value: "<=", text: "小于等于"}];
        var dateQueryType = [{ value: "between", text: "介于" }, { value: "betweenWeek", text: "本周" }, { value: "betweenMonth", text: "本月" }, { value: "betweenQuarter", text: "本季" }, { value: "betweenYear", text: "本年"}];
        var dropdownQueryType = [{ value: "in", text: "包含"}];

        //获取枚举文本
        function getEnumText(enumValues, en) {

            if (enumValues == null)
                return "";

            var vals = [];
            if (enumValues.split)
                vals = enumValues.split(',');
            else
                vals[0] = enumValues.toString();

            for (var i = 0; i < vals.length; i++) {
                if (vals[i] == "")
                    continue;
                for (var j = 0; j < en.length; j++) {
                    if (en[j]["value"].toString() == vals[i]) {
                        vals[i] = en[j]["text"];
                    }
                }
            }

            return vals.join(',');
        }

        function onQueryTypeRender(e) {
            return getEnumText(e.value, queryType);
        }

        function onSumTypeRender(e) {
            return getEnumText(e.value, sumType);
        }

        function onCtrlTypeRender(e) {
            return getEnumText(e.value, ctrlType);
        }

        function commitGridEdit(gridId) {
            var grid = mini.get(gridId);
            grid.commitEdit();
        }
    
    </script>
    <script type="text/javascript">
        //获取地址栏参数,如果参数不存在则返回空字符串
        function getQueryString(key, url) {
            if (typeof (url) == "undefined")
                url = window.location.search;
            var re = new RegExp("[?&]" + key + "=([^\\&]*)", "i");
            var a = re.exec(url);
            if (a == null) return "";
            return a[1];
        }
        var gridId;
        function addFilterRow() {
            gridId = "FilterItems";
            mini.get("fieldGrid").setData(filterItems);
            mini.get("selectWin").show();
        }
        function addDataRow() {
            gridId = "DataItems";
            mini.get("fieldGrid").setData(dataItems);
            mini.get("selectWin").show();
        }
        function addRowArea() {
            gridId = "RowItems";
            mini.get("fieldGrid").setData(rowItems);
            mini.get("selectWin").show();
        }
        function addColumnArea() {
            gridId = "ColumnItems";
            mini.get("fieldGrid").setData(columnItems);
            mini.get("selectWin").show();
        }
        function selectField() {
            var grid = mini.get(gridId);
            var fieldGrid = mini.get("fieldGrid");
            grid.addRows(mini.clone(fieldGrid.getSelecteds()));
            mini.get("selectWin").hide();
        }
        function delRow(gridId) {
            var grid = mini.get(gridId);
            var rows = grid.getSelecteds();
            grid.removeRows(rows, true);
        }

        function doFilter() {
            var filterGrid = mini.get("FilterItems");
            var FilterItems = filterGrid.getData();
            for (var i = 0; i < FilterItems.length; i++) {
                var fieldName = FilterItems[i]["FieldName"];
                FilterItems[i]["QueryType"] = mini.get("f_" + i.toString()).getValue();
                if (FilterItems[i]["QueryType"].indexOf("between") >= 0) {
                    FilterItems[i]["QueryType"] = "between";
                    FilterItems[i]["FieldValue"] = "";
                    if (mini.get("v_" + i.toString()).getValue())
                        FilterItems[i]["FieldValue"] = mini.get("v_" + i.toString()).getValue().format("yyyy-MM-dd");
                    FilterItems[i]["FieldValue"] += ",";
                    if (mini.get("v1_" + i.toString()).getValue())
                        FilterItems[i]["FieldValue"] += mini.get("v1_" + i.toString()).getValue().format("yyyy-MM-dd");
                }
                else {
                    FilterItems[i]["FieldValue"] = mini.get("v_" + i.toString()).getValue();
                }
            }
            filterGrid.setData(FilterItems);
            doQuery();
        }

        function doQuery() {
            var filterGrid = mini.get("FilterItems");
            filterGrid.commitEdit();
            var FilterItems = filterGrid.getData();
            var DataItems = mini.get("DataItems").getData();
            var RowItems = mini.get("RowItems").getData();
            var ColumnItems = mini.get("ColumnItems").getData();
            var data = { FilterItems: mini.encode(FilterItems), DataItems: mini.encode(DataItems), RowItems: mini.encode(RowItems), ColumnItems: mini.encode(ColumnItems) };

            var url = "PivotGrid.aspx?TmplCode=" + getQueryString("TmplCode") + "&method=doQuery";
            if (getQueryString("ID"))//自由透视表
                url = "PivotGrid.aspx?ID=" + getQueryString("ID") + "&method=doQuery";

            $.ajax({
                url: url,
                data: data,
                type: "post",
                cache: false,
                success: function (text) {
                    //增加新版报错分支
                    if (text && typeof (text) == "string" && text.indexOf("{\"errcode\"") == 0) {
                        var fail = jQuery.parseJSON(text);
                        var msg = getErrorFromHtml(fail.errmsg);
                        msgUI(msg, 4);
                        return;
                    }
                    window.location.reload();
                }
            });
        }
    </script>
    <script type="text/javascript">
        mini.parse();
        mini.get("FilterItems").setData(userFilterItems);
        mini.get("DataItems").setData(userDataItems);
        mini.get("RowItems").setData(userRowItems);
        mini.get("ColumnItems").setData(userColumnItems);
        function reset() {
            var url = "PivotGrid.aspx?TmplCode=" + getQueryString("TmplCode") + "&method=reset";
            $.ajax({
                url: url,
                data: {},
                type: "post",
                cache: false,
                success: function (text) {
                    //增加新版报错分支
                    if (text && typeof (text) == "string" && text.indexOf("{\"errcode\"") == 0) {
                        var fail = jQuery.parseJSON(text);
                        var msg = getErrorFromHtml(fail.errmsg);
                        msgUI(msg, 4);
                        return;
                    }
                    window.location = "PivotGrid.aspx?TmplCode=" + getQueryString("TmplCode");
                }
            });
        }
    </script>
    <script type="text/javascript">
        function onFilterTypeChanged(e) {
            var id = e.id.replace("f_", "v_");
            var id1 = e.id.replace("f_", "v1_");
            var c = mini.get(id);
            var c1 = mini.get(id1);
            if (e.value == "betweenWeek") {
                c.setValue(getWeekStartDate());
                c1.setValue(getWeekEndDate());
            }
            else if (e.value == "betweenMonth") {
                c.setValue(getMonthStartDate());
                c1.setValue(getMonthEndDate());
            }
            else if (e.value == "betweenQuarter") {
                c.setValue(getQuarterStartDate());
                c1.setValue(getQuarterEndDate());
            }
            else if (e.value == "betweenYear") {
                c.setValue(nowYear + "-1-1");
                c1.setValue(nowYear + "-12-31");
            }
        }
    </script>
    <script type="text/javascript">
        //日期格式化
        Date.prototype.format = function (format) {
            var o = {
                "M+": this.getMonth() + 1, //month
                "d+": this.getDate(), //day
                "h+": this.getHours(), //hour
                "m+": this.getMinutes(), //minute
                "s+": this.getSeconds(), //second
                "q+": Math.floor((this.getMonth() + 3) / 3), //quarter
                "S": this.getMilliseconds() //millisecond
            }
            if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
 (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o) if (new RegExp("(" + k + ")").test(format))
                format = format.replace(RegExp.$1,
 RegExp.$1.length == 1 ? o[k] :
 ("00" + o[k]).substr(("" + o[k]).length));
            return format;
        }
    </script>
    <script type="text/javascript">

        /** 
        * 获取本周、本季度、本月、上月的开端日期、停止日期 
        */
        var now = new Date(); //当前日期 
        var nowDayOfWeek = now.getDay(); //今天本周的第几天 
        var nowDay = now.getDate(); //当前日 
        var nowMonth = now.getMonth(); //当前月 
        var nowYear = now.getYear(); //当前年 
        nowYear += (nowYear < 2000) ? 1900 : 0; // 

        var lastMonthDate = new Date(); //上月日期 
        lastMonthDate.setDate(1);
        lastMonthDate.setMonth(lastMonthDate.getMonth() - 1);
        var lastYear = lastMonthDate.getYear();
        var lastMonth = lastMonthDate.getMonth();

        //格局化日期：yyyy-MM-dd 
        function formatDate(date) {
            var myyear = date.getFullYear();
            var mymonth = date.getMonth() + 1;
            var myweekday = date.getDate();

            if (mymonth < 10) {
                mymonth = "0" + mymonth;
            }
            if (myweekday < 10) {
                myweekday = "0" + myweekday;
            }
            return (myyear + "-" + mymonth + "-" + myweekday);
        }

        //获得某月的天数 
        function getMonthDays(myMonth) {
            var monthStartDate = new Date(nowYear, myMonth, 1);
            var monthEndDate = new Date(nowYear, myMonth + 1, 1);
            var days = (monthEndDate - monthStartDate) / (1000 * 60 * 60 * 24);
            return days;
        }

        //获得本季度的开端月份 
        function getQuarterStartMonth() {
            var quarterStartMonth = 0;
            if (nowMonth < 3) {
                quarterStartMonth = 0;
            }
            if (2 < nowMonth && nowMonth < 6) {
                quarterStartMonth = 3;
            }
            if (5 < nowMonth && nowMonth < 9) {
                quarterStartMonth = 6;
            }
            if (nowMonth > 8) {
                quarterStartMonth = 9;
            }
            return quarterStartMonth;
        }

        //获得本周的开端日期 
        function getWeekStartDate() {
            var weekStartDate = new Date(nowYear, nowMonth, nowDay - nowDayOfWeek);
            return formatDate(weekStartDate);
        }

        //获得本周的停止日期 
        function getWeekEndDate() {
            var weekEndDate = new Date(nowYear, nowMonth, nowDay + (6 - nowDayOfWeek));
            return formatDate(weekEndDate);
        }

        //获得本月的开端日期 
        function getMonthStartDate() {
            var monthStartDate = new Date(nowYear, nowMonth, 1);
            return formatDate(monthStartDate);
        }

        //获得本月的停止日期 
        function getMonthEndDate() {
            var monthEndDate = new Date(nowYear, nowMonth, getMonthDays(nowMonth));
            return formatDate(monthEndDate);
        }

        //获得上月开端时候 
        function getLastMonthStartDate() {
            var lastMonthStartDate = new Date(nowYear, lastMonth, 1);
            return formatDate(lastMonthStartDate);
        }

        //获得上月停止时候 
        function getLastMonthEndDate() {
            var lastMonthEndDate = new Date(nowYear, lastMonth, getMonthDays(lastMonth));
            return formatDate(lastMonthEndDate);
        }

        //获得本季度的开端日期 
        function getQuarterStartDate() {

            var quarterStartDate = new Date(nowYear, getQuarterStartMonth(), 1);
            return formatDate(quarterStartDate);
        }

        //或的本季度的停止日期 
        function getQuarterEndDate() {
            var quarterEndMonth = getQuarterStartMonth() + 2;
            var quarterStartDate = new Date(nowYear, quarterEndMonth, getMonthDays(quarterEndMonth));
            return formatDate(quarterStartDate);
        } 
    </script>
</body>
</html>
