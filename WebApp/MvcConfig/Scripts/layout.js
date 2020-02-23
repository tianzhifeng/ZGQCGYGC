function onActionRenderer(e) {
    var url = e.column.actionUrl;
    if (e.column.iconcls && e.column.displayContent && e.column.displayContent == "buttonIcon") {
        return "<a plain='true' class='" + e.column.iconcls + "' href='javascript:Open(\"" + url + "\",\"" + e.column.btnName + "\",\"" + e.column.winType + "\");'></a>";
    } else if (e.column.displayContent && e.column.displayContent == "buttonName") {
        return "<a href='javascript:Open(\"" + url + "\",\"" + e.column.btnName + "\",'','',\"" + e.column.winType + "\");'>" + e.column.btnName + "</a>";
    } else {
        return "<a href='javascript:Open(\"" + url + "\",\"" + e.column.btnName + "\",'','',\"" + e.column.winType + "\");'>" + e.value + "</a>";
    }
}

function onActionOpen(url, value, width, height) {
    mini.open({
        url: url,
        title: value,
        width: !width ? screen.width - 200 : width,
        height: !height ? screen.height - 200 : height,
    });

}
//获取url中的参数
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}

function Open(url, value, type, width, height) {
    var arr = url.match(/[^\{]+(?=\})/gm);
    var para = url.match(/[^\[]+(?=\])/gm);
    if (type == 'tabs') {
        var settings = subArr[actTab._id - 1];
        var _grid = mini.get(settings.id);
        rowValue = _grid.getSelecteds()[0];
    }
    if (arr && (!rowValue)) {
        msgUI("请选择一条记录！");
        return;
    }
    if (arr) {
        for (var i = 0; i < arr.length; i++) {
            var re = '/{' + arr[i] + "}/gm";
            if (rowValue) {
                url = url.replace(eval(re), eval('rowValue.' + arr[i]));
            }
        }
    }
    if (para) {
        for (var i = 0; i < para.length; i++) {
            var re = '[' + para[i] + "]";
            url = url.replace(re, getUrlParam('' + para[i] + ''));
        }
    }
    mini.open({
        url: url,
        title: value,
        width: !width ? screen.width - 200 : width,
        height: !height ? screen.height - 200 : height,
    });

}

function buttonUrl(e, btn, type) {
    if (e.method) {
        if (e.method.indexOf('/') >= 0) {
            return "Open('" + e.method + "','" + e.popupTitle + "', '" + type + "', '" + (e.popupWidth ? e.popupWidth : 1000) + "','" + (e.popupHeight ? e.popupHeight : screen.height - 200) + "')";
        } else {
            if (e.id.indexOf('btnAdd') >= 0 || e.id.indexOf('btnDel') >= 0 || e.id.indexOf('btnSave') >= 0) {
                var method = e.method.substr(0, e.method.length - 2);
                return btn == '_btn' ? method + "(1)" : method + "(2)";
            } else {
                return e.method;
            }
        }
    } else { return ""; }
}

function loadButton(e, btn, type) {
    var str = "";
    e = e.button;
    if (e) {   
        for (var i = 0; i < e.length; i++) {
            if (e[i].visible && !e[i].field) {
                str += '<a class="mini-button mini-button-plain" iconcls="' + (e[i].iconcls && (!e[i].displayContent || e[i].displayContent == "buttonIcon") ? e[i].iconcls : "") + '" style="float:left;min-width:18px; height:20px; line-height:20px; padding-left:1px;" plain="true" onclick="' + buttonUrl(e[i], btn, type) + '" id="' + e[i].id + '">' + e[i].name + "</a>"
            }
        }
        $("#" + btn).html(str);
    }
}

function getUrl(url, e) {
    var arr = url.match(/[^\{]+(?=\})/gm);
    var para = url.match(/[^\[]+(?=\])/gm);
    if (arr) {
        for (var i = 0; i < arr.length; i++) {
            var re = '/{' + arr[i] + "}/gm";
            if (!e.records) {
                url = url.replace(eval(re), eval('e.record.' + arr[i]));
            } else {
                url = url.replace(eval(re), eval('e.records[0].' + arr[i]));
            }
        }
    }
    if (para) {
        for (var i = 0; i < para.length; i++) {
            var re = '[' + para[i] + "]";
            url = url.replace(re, getUrlParam('' + para[i] + ''));
        }
    }
    return url;
}

function onListRenderer(e) {
    var lists = eval(e.column.enumKey);
    if (lists.length > 0) {
        for (var i = 0, l = lists.length; i < l; i++) {
            var g = lists[i];
            if (g.value == e.value) return g.text;
        }
    }
    return "";
}

function defaultVal(grid) {
    var newRow = "";
    function SQLVal(para, val) {
        if (para) {
            for (var i = 0; i < para.length; i++) {
                var re = '[' + para[i] + "]";
                val = val.replace(re, getUrlParam('' + para[i] + ''));
            }
        }
        return val;
    }
    var array = new Array()
    for (var i = 0; i < grid.getColumns().length; i++) {
        if (grid.getColumns()[i]) {
            array.push(grid.getColumns()[i]);
        } 
    }
    for (var i = 0; i < array.length; i++) {
        var sqlval = array[i].SQLVal;
        if (sqlval) {
            var para = sqlval.match(/[^\[]+(?=\])/gm);
            if (para) {
                newRow += "," + array[i].field + ":" + "'" + SQLVal(para, sqlval) + "'";
            } else if (!para) {
                $.ajax({
                    url: "/MvcConfig/UI/Layout/SQLVal?para=" + sqlval,
                    type: "post",
                    cache: false,
                    async: false,
                    success: function (text) {
                        newRow += "," + array[i].field + ":" + "'" + text + "'";
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        var msg = getErrorFromHtml(jqXHR.responseText);
                        msgUI(msg);
                    }
                });
            }
        }
    }
    return newRow;

}

function bindParameterHtml(json) {
    if (json != null) {
        var html = "<table id='_table'>";
        function getHtml(val) {
            if (val.type != null && typeof (val.data) != "undefined") {
                return 'textField="' + val.textField + '" valueField="' + val.valueField + '" data="' + val.data + '"';
            } else { return "";}
        }
        for (var i = 0; i < json.length; i++) {
            html += '<tr><td width="10%" align="center">' + json[i].text + '</td><td width="23%" align="left">'
                + '<div name="$LK$' + json[i].value + '" style="width: 75%;" ' + getHtml(json[i]) + ' class="' + json[i].type + '"></div></td></tr>';
        }
        html += "</table>";
    } else {
        $("#_paraQuery").hide();
    }
    return html;
}