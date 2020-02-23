var url = LayoutDef.list.url;
var grid = new mini.DataGrid();
grid.set(LayoutDef.list);
grid.render(document.getElementById("divGrid"));
$("#table1").css("width", window.innerWidth - 40);
$("#table1").css("align", "center");
var kw = ($("#table1").width() - 246) / 3;
var t = ""; //控件代码
var w = 0; 
var table = document.getElementById("table1");

var array = new Array()
for (var j = 0; j < LayoutDef.form.columns.length; j++) {
    if (LayoutDef.form.columns[j].visible)
    {
        array.push(LayoutDef.form.columns[j]);
    }
}
function Gtd(w) {
    var td = "";
    if (w % 6 > 0) {
        td += "<td colspan='" + (w % 6 == 2 ? 4 : 2) + "'></td>";
    }
    return td;
}
for (var i = 0; i < array.length; i++) {
    var itemSettings = array[i];
    var c = itemSettings.cols;
    var colspan = !c ? 0 : c == 2 ? 3 : 5;

    var tr1 = "";
    var tr2 = "";
    w += (!c ? 2 : c == 2 ? 4 : 0);
    if (i == 0 && colspan != 5) {
        tr1 = "<tr>";
    } else if (i == 0 && colspan == 5) {
        t += Gtd(w);
        tr1 = "<tr>"; w = 0;
        tr2 = "</tr><tr>";
    } else if (colspan == 5 && i != 0) {
        t += Gtd(w);
        tr1 = "</tr><tr>"; w = 0;
        tr2 = "</tr><tr>";
    } else if (w % 6 == 2 && colspan == 3) {
        tr1 = "";
        tr2 = "</tr>";
    } else if (w % 6 == 4 && colspan == 3) {
        tr1 = "<tr>";
        tr2 = "";
    } else if (w % 6 > 1 && colspan == 0) {
        tr1 = "";
        tr2 = "";
    } else if (w % 6 == 0 && colspan != 5 && i != (array.length - 1)) {
        tr1 = "";
        tr2 = "</tr>";
    } else if (i == (array.length - 1)) {
        tr1 = "";
        tr2 = "</tr>";
    }

    t += tr1 + "<td class='tdtitle'>" + itemSettings.label + "</td><td id='" + itemSettings.name + "' style='width:" + kw + "px;'  colspan='" + colspan + "'></td>" + tr2;

}
$("#table1").html(t);

for (var i = 0; i < array.length; i++) {
    var itemSettings = array[i];
    var type = itemSettings.type;
    var item = eval("new mini." + type);
    var width = itemSettings.cols == 3 ? parseInt($("#table1").width() - 80) : itemSettings.cols == 2 ? parseInt(kw) * 2 + 83 : parseInt(kw);
    if (itemSettings.readOnly) {
        item.addCls("asLabel");
    }
    itemSettings.width = !itemSettings.width ? width : itemSettings.width;
    item.set(itemSettings);
    var txt = document.getElementById(itemSettings.name);
    item.render(txt);
}

function ExportExcel() {
    var rows = grid.getSelecteds();
    var columns = grid.getBottomColumns();
    var ids = "";
    for (var i = 0; i < rows.length; i++) {
        ids += "'" + rows[i].ID + "'" + ((i == rows.length - 1) ? "" : ",");
    }

    function getColumns(columns) {
        columns = columns.clone();
        for (var i = columns.length - 1; i >= 0; i--) {
            var column = columns[i];
            if (!column.field) {
                columns.removeAt(i);
            } else {
                var c = { header: column.header, field: column.field, enumKey: !column.enumKey ? "" : column.enumKey };
                columns[i] = c;
            }
        }
        return columns;
    }

    var columns = getColumns(columns);
    var json = mini.encode(columns);
    window.open(LayoutDef.list.expUrl + "&ids=" + ids + "&data=" + json);

}

function onChangeValue(e) {
    var id = e.getValue();

    for (var i = 0; i < LayoutDef.form.length; i++) {
        var itemSettings = LayoutDef.form[i];
        if (LayoutDef.form[i].linkColumn == e.name && LayoutDef.form[i].linkValue) {
            var data = LayoutDef.form[i].linkValue;
            $.ajax({
                url: "/MvcConfig/UI/Layout/onChangeValue?ConnName=Demo&ID=" + e.ID,
                data: { data: data },
                type: "post",
                success: function (text) {
                    itemSettings.setValue(text);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    msgUI(jqXHR.responseText);
                }
            });
        }
    }

}


function Calc(arr, e, str) {
    var result = [], isRepeated;
    var field = e.field, value = e.value;
    for (var i = 0; i < arr.length; i++) {
        isRepeated = false;
        for (var j = 0; j < result.length; j++) {
            if (arr[i] == result[j]) {
                isRepeated = true;
                break;
            }
        }
        if (!isRepeated) {
            result.push(arr[i]);
        }
    }
    for (var j = 0; j < result.length; j++) {
        var re = '/' + result[j] + "/gm";
        if (field == result[j]) {
            str = str.replace(eval(re), value);
        } else {
            var grid = e.sender;
            for (var i = 0; i < grid.columns.length - 1; i++) {
                var itemSettings = grid.columns[i];
                if (result[j] == itemSettings.field) {
                    var n = !eval('e.record.' + result[j]) ? 0 : eval('e.record.' + result[j]);
                    str = str.replace(eval(re), n);
                }
            }
        }
    }

    return eval(eval(str.replace(/[{^\}]/gm, '')));

}

function OnCellCommitEdit(e) {
    var grid = e.sender;
    var record = e.record;
    var field = e.field, value = e.value;

    for (var i = 0; i < grid.columns.length - 1; i++) {
        var itemSettings = grid.columns[i];
        if (!itemSettings) {
        } else {
            if (field == itemSettings.linkColumn && itemSettings.type != "ComboBox") {
                if (!itemSettings.editor || itemSettings.editor.type != "ComboBox") {
                    var data = itemSettings.linkValue;
                    var name = itemSettings.field;
                    loadAjax(grid, data, e, name, field, itemSettings);
                }
            }
            if (!itemSettings.expression) {
            } else {
                var str = itemSettings.expression;
                var arr = str.match(/[^\{]+(?=\})/gm);
                var number = Calc(arr, e, str);

                eval("grid.updateRow(record, {" + itemSettings.field + ":number});");
            }
        }
    }
}

function loadAjax(grid, data, e, name, field, itemSettings) {
    var arr = data.match(/[^\{]+(?=\})/gm);
    var para = data.match(/[^\[]+(?=\])/gm);
    if (arr) {
        for (var i = 0; i < arr.length; i++) {
            var re = '/{' + arr[i] + "}/gm";
            if (!e.records) {
                data = data.replace(eval(re), field == arr[i] ? e.value : eval('e.record.' + arr[i]));
            } else {
                data = data.replace(eval(re), field == arr[i] ? e.value : eval('e.records[0].' + arr[i]));
            }

        }
    }
    if (para) {
        for (var i = 0; i < para.length; i++) {
            var re = '[' + para[i] + "]";
            data = data.replace(re, getUrlParam('' + para[i] + ''));
        }
    }
    $.ajax({
        url: "/MvcConfig/UI/Layout/onChangeValue?ConnName=" + LayoutDef.list.connName,
        data: { data: data },
        type: "post",
        success: function (text) {
            if (field == itemSettings.linkColumn) {
                eval("grid.updateRow(e.record, {" + name + ":text});");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (field == itemSettings.linkColumn) {
                eval("grid.updateRow(e.record, {name :''});");
            }
        }
    });
}

function OnCellBeginEdit(e) {
    var grid = e.sender;
    var record = e.record;
    var field = e.field, value = e.value;
    var editor = e.editor;

    function Value(data) {
        if (data) {
            var arr = data.match(/[^\{]+(?=\})/gm);
            var para = data.match(/[^\[]+(?=\])/gm);
            if (arr) {
                for (var i = 0; i < arr.length; i++) {
                    var re = '/{' + arr[i] + "}/gm";
                    if (!e.records) {
                        data = data.replace(eval(re), field == arr[i] ? e.value : eval('e.record.' + arr[i]));
                    } else {
                        data = data.replace(eval(re), field == arr[i] ? e.value : eval('e.records[0].' + arr[i]));
                    }

                }
            }
            if (para) {
                for (var i = 0; i < para.length; i++) {
                    var re = '[' + para[i] + "]";
                    data = data.replace(re, getUrlParam('' + para[i] + ''));
                }
            }
        }
        return !data ? "" : data;
    }

    for (var i = 0; i < grid.columns.length; i++) {
        var itemSettings = grid.columns[i];
        if (!itemSettings) {
        } else {
            if (itemSettings.editor && itemSettings.editor.type == "ComboBox" && field == itemSettings.field) {
                //var id = record[itemSettings.linkColumn];
                if (id) {
                    var url = "/MvcConfig/UI/Layout/GetLinkData?ConnName=" + LayoutDef.list.connName + "&enumKey=" + itemSettings.enumKey + "&linkValue=" + Value(itemSettings.linkValue);
                    editor.setUrl(url);
                } else {
                    e.cancel = true;
                }
            }
        }
    }
}

$("#f_bottom").css("width", "850");
$(".mini-textbox&.mini-textarea&.mini-labelfield").css("width", "100%");
$(".gw-grid-toolbar&.mini-toolbar").removeClass("gw-grid-toolbar");
$(".mini-labelfield-label").addClass("label");

var subArr = [];
for (var i = 0; i < LayoutDef.tabs.length; i++) {
    var item = LayoutDef.tabs[i];
    var div = document.createElement("div");
    div.title = item.tabName;
    div.name = item.id;
    document.getElementById("tabs").appendChild(div);

    subArr.push(mini.clone(item));
    var subGrid = new mini.DataGrid();
    subGrid.set(item);
    subGrid.render(div);
}

var actTab;
var currentRow, rowValue;
var para = window.location.href.match(/[^\[]+(?=\])/gm);
function pageLoad() {
    var tabs = mini.get("tabs");
    tabs.on("activechanged", function (e) {
        actTab = e.tab;
        var settings = subArr[actTab._id - 2];
        $("_btn2").empty();
        if (settings) {
            $("#tabsButtons").show();
            loadButton(settings, '_btn2', 'tabs');
        } else { $("#tabsButtons").hide(); }
        loadTab(e);
    });

    if (!LayoutDef.list.searchColumn) {
        $("#key").hide();
    }
    if (!LayoutDef.list.expUrl) {
        $("#download").hide();
    }
    if (para) {
        url = getUrl(url, "");
        grid.columns.add();
        grid.setUrl(url)
        grid.reload();
    }

     loadButton(LayoutDef.list, '_btn', 'list');
     $("#queryForm").html(bindParameterHtml(LayoutDef.list.parameter));

}
function filter() {
    var parameter = LayoutDef.list.parameter;
    var form = new mini.Form("#queryForm");
    var data = form.getData();
    if (parameter != "") {
        for (var i = 0; i < parameter.length; i++) {
            var re = new RegExp("[?&]" + parameter[i].value + "=([^\\&]*)", "i");
            var r = re.exec(url);
            var value = eval("form.el.$LK$" + parameter[i].value).value;
            if (url.indexOf(parameter[i].value) >= 0) {
                if (r != null)
                    url = url.replace(r[0], (r[0].indexOf('?') >= 0 ? '?' : '&') + parameter[i].value + '=' + value);
            } else {
                url = url + (url.indexOf('?') >= 0 ? '&' : '?') + parameter[i].value + '=' + value;
            }
        }
    }
    hideWindow("queryWindow");
    grid.setUrl(url)
    grid.reload();
}
grid.on("rowclick", function (e) {
    currentRow = e.record;
    rowValue = e.record;
    loadTab(e);
});

function onListRenderer(e) {

    var grid = mini.get("list");
    var columns = grid.getColumns();
    var count = columns.length;
    var lists = "";
    for (var i = 0; i < count; i++) {
        if (columns[i].enumKey != undefined) {
            lists = eval(columns[i].enumKey);
        }
    }
    if (lists.length > 0) {
        for (var i = 0, l = lists.length; i < l; i++) {
            var g = lists[i];
            if (g.value == e.value) return g.text;
        }
    }
    return "";
}

function loadTab(e) {
    if (!actTab || !currentRow)
        return;
    var settings = subArr[actTab._id - 2];
    if (actTab._id == 1) {
        var form = new mini.Form("#table1");
        form.setData(currentRow);
        $("#tabsButtons").hide();
    }
    else {
        var grid = mini.get(settings.id);
        var url = getUrl(settings.url, e);
        grid.columns.add();
        grid.setUrl(url);
        grid.reload();
        $("#tabsButtons").show();
    }
}

var db = new mini.DataBinding();

db.bindForm("#table1", grid);

function addRow(t) {

    if (t == 1) {
        var newRowVal = "{fid:'" + (!currentRow ? "'" : currentRow.ID + "'") + defaultVal(grid) + "}";
        var newRow = eval('(' + newRowVal + ')');
        grid.addRow(newRow, 0);
        grid.deselectAll();
        grid.select(newRow);
    } else {
        var settings = subArr[actTab._id - 2];
        var grid_tabs = mini.get(settings.id);
        var newRowVal = "{fid:" + (!currentRow ? "''" : (currentRow.ID ? '\'' + currentRow.ID + '\'' : '\'\'')) + defaultVal(grid_tabs) + "}";
        var newRow = eval('(' + newRowVal + ')');
        grid_tabs.addRow(newRow, 0);
        grid_tabs.deselectAll();
        grid_tabs.select(newRow);
    }
}



function save(t) {
    var data; var postUrl;
    if (t == 1) {
        data = grid.getChanges();
        postUrl = grid.postUrl;
    } else {
        var settings = subArr[actTab._id - 2];
        var grid_tabs = mini.get(settings.id);
        data = grid_tabs.getChanges();
        postUrl = grid_tabs.postUrl;
    }
    var json = mini.encode(data);
    grid.loading("保存中，请稍后......");
    $.ajax({
        url: postUrl,
        data: { data: json, fid: grid.fid },
        type: "post",
        success: function (text) {
            msgUI("保存成功");
            grid.reload();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            msgUI(jqXHR.responseText);
        }
    });
}

function delRow(t) {
    if (t == 1) {
        var rows = grid.getSelecteds();
        if (rows.length > 0) {
            grid.removeRows(rows, true);
        }
    } else {
        var settings = subArr[actTab._id - 2];
        var grid_tabs = mini.get(settings.id);
        var rows = grid_tabs.getSelecteds();

        if (rows.length > 0) {
            grid_tabs.removeRows(rows, true);
        }
    }
}

function keySearch() {
    var keyBox = mini.get("key");
    var name = keyBox.getValue().toLowerCase();
    grid.filter(function (row) {
        var r = true;
        if (name && LayoutDef.list.searchColumn) {
            var c = eval("row." + LayoutDef.list.searchColumn);
            r = String(c).toLowerCase().indexOf(name) != -1;;
        }
        return r;
    });

}
