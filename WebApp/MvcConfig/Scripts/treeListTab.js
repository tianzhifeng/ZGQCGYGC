
var tree = new mini.Tree("tree");
var treeUrl = LayoutDef.tree.url;
tree.set(LayoutDef.tree);
tree.load(getUrl(treeUrl));
$(".gw-grid-toolbar&.mini-toolbar").removeClass("gw-grid-toolbar");
$(".mini-grid-rows-view$.mini-grid-hidden-y").css("height", $("#divs").height() - 5);
var url = LayoutDef.list.url;
var grid = new mini.DataGrid();
grid.set(LayoutDef.list);
grid.render(document.getElementById("divList"))
$("#rdiv").css("width", $("#divs").width() - 162);


var treeId, rowValue;
tree.on("nodeselect", function (e) {
    treeId = e.node.ID;
    url = getUrl(url, e);
    grid.columns.add();
    grid.setUrl(url)
    grid.reload();
});

function getForm(form) {
    $("#table" + actTab._id).css("width", $("#table" + actTab._id).width() - 15);
    var kw = ($("#table" + actTab._id).width() - 246) / 3;
    var t = ""; //控件代码
    var w = 0;
    var table = document.getElementById("table" + actTab._id);
    var array = new Array();
    for (var j = 0; j < form.columns.length; j++) {
        if (form.columns[j].visible) {
            array.push(form.columns[j]);
        } else {
            t += "<tr><td style='width:80px;display:none'  id='" + form.columns[j].name + actTab._id + "'></td></tr>";
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
        var colspan = !c || c == 1 ? 0 : c == 2 ? 3 : 5;

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

        t += tr1 + "<td class='tdtitle'>" + itemSettings.label + "</td><td id='" + itemSettings.name + actTab._id + "' style='width:" + kw + "px;'  colspan='" + colspan + "'></td>" + tr2;

    }

    $("#table" + actTab._id).html(t);

    for (var i = 0; i < array.length; i++) {
        var itemSettings = array[i];
        var type = itemSettings.type;
        var item = eval("new mini." + type);
        var width = itemSettings.cols == 3 ? parseInt($("#table" + actTab._id).width() - 80) : itemSettings.cols == 2 ? parseInt(kw) * 2 + 83 : parseInt(kw);
        itemSettings.width = !itemSettings.width ? width : itemSettings.width;
        if (itemSettings.readOnly) {
            item.addCls("asLabel");
        }
        item.set(itemSettings);
        var txt = document.getElementById(itemSettings.name + actTab._id);
        item.render(txt);
    }
    for (var j = 0; j < form.columns.length; j++) {
        if (!form.columns[j].visible) {
            var itemSettings = form.columns[j];
            var type = itemSettings.type;
            var item = eval("new mini." + type);
            item.set(itemSettings);
            var txt = document.getElementById(itemSettings.name + actTab._id);
            item.render(txt);
        }
    }
}
function loadData(e) {
    var settings = subArr[actTab._id - 1];
    if (settings.winType == 'form') {
        var form = new mini.Form("#table" + actTab._id);
        $.ajax({
            url: getUrl(settings.url, e),
            type: "post",
            success: function (text) {
                var json = mini.encode(text.data).substr(1, mini.encode(text.data).length - 2);
                var data = mini.decode(json);
                form.setData(data);
            }
        });
    }
}


var currentRow;
grid.on("rowclick", function (e) {
    currentRow = e;
    rowValue = e.record;
    loadTab(e);
    loadData(e);
});

var actTab;
function pageLoad() {
    var tabs = mini.get("tabs");
    tabs.on("activechanged", function (e) {
        actTab = e.tab;
        var settings = subArr[actTab._id - 1];
        $("#_btn2").empty();
        if (settings.winType == 'form') {
            tabs.getTabBodyEl(actTab).innerHTML = '<table id="table' + actTab._id + '" width="100%" border="0" cellspacing="1" cellpadding="0" style="margin: 10px;"></table>';
            getForm(settings);
        }
        loadButton(settings, '_btn2', 'tabs');
        if (currentRow) {
            loadTab(currentRow);
            loadData(currentRow);
        }
    });
    if (!LayoutDef.list.searchColumn) {
        $("#key").hide();
    }
    if (!LayoutDef.list.expUrl) {
        $("#download").hide();
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

function loadTab(e) {
    if (!actTab || !currentRow)
        return;

    var settings = subArr[actTab._id - 1];
    if (settings.winType != 'form') {
        var grid = mini.get(settings.id);
        var url = getUrl(settings.url, e);
        grid.setUrl(url);
        grid.reload();
    }
}

var subArr = [];

for (var i = 0; i < LayoutDef.tabs.length; i++) {
    var item = LayoutDef.tabs[i];
    var div = document.createElement("div");
    div.title = item.tabName;
    div.name = item.id;
    document.getElementById("tabs").appendChild(div);

    subArr.push(mini.clone(item));
    if (LayoutDef.tabs[i].winType == 'tabs') {
        var subGrid = new mini.DataGrid();
        subGrid.set(item);
        subGrid.render(div);
    }
}

function addRow(t) {
    var settings = subArr[actTab._id - 1];
    var grid_tabs = mini.get(settings.id);
    var gridRow = t == 1 ? eval(grid) : grid_tabs;
    var newRowVal = "{fid:" + (t == 1 ? treeId : (currentRow ? '\'' + currentRow.record.ID + '\'' : '\'\'')) + defaultVal(gridRow) + "}";
    var newRow = eval('(' + newRowVal + ')');
    gridRow.addRow(newRow, 0);
    gridRow.deselectAll();
    gridRow.select(newRow);

}


function delRow(t) {
    var settings = subArr[actTab._id - 1];
    var grid_tabs = mini.get(settings.id);
    var gridRow = t == 1 ? eval(grid) : grid_tabs;
    var rows = gridRow.getSelecteds();
    if (rows.length > 0) {
        gridRow.removeRows(rows, true);
    }
}


function save(t) {
    var settings = subArr[actTab._id - 1];
    if (settings.winType == 'form' && t > 1) {
        saveForm();
    } else {
        saveList(t);
    }
}

function saveForm() {
    var settings = subArr[actTab._id - 1];
    var form = new mini.Form("#table" + actTab._id);
    var data = form.getData();
    var json = "[" + mini.encode(data) + "]";
    var isNull = false;
    for (var i = 0; i < settings.columns.length; i++) {
        var txtName = settings.columns[i].name;
        if (settings.columns[i].required && $(":input[name='" + txtName + "']").val().length <= 0) {
            isNull = true;
            break;
        }
    }

    if (isNull) {
        msgUI("请把必填信息填写完整!");
    } else {
        $.ajax({
            url: settings.postUrl,
            data: { data: json },
            type: "post",
            success: function (text) {
                msgUI("保存成功");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                msgUI(jqXHR.responseText);
            }
        });
    }
}

function saveList(t) {
    var settings = subArr[actTab._id - 1];
    var grid_tabs = mini.get(settings.id);
    var gridRow = t == 1 ? eval(grid) : grid_tabs;
    var data = gridRow.getChanges();
    var postUrl = gridRow.postUrl;
    var json = mini.encode(data);
    $.ajax({
        url: postUrl,
        data: { data: json, fid: gridRow.fid },
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

if (LayoutDef.tree.isHideSearch) {
    $("#txtSearch").hide();
}
$("#tree").css("height", $("#tree").height() - 40);
function search() {
    var key = mini.get("keySearch").getValue();
    if (key == "") {
        tree.clearFilter();
    } else {
        key = key.toLowerCase();
        tree.filter(function (node) {
            var fieid = LayoutDef.tree.textField;
            var text = eval('node.' + fieid);
            var ret = text ? text.toLowerCase() : "";
            if (ret.indexOf(key) != -1) {
                return true;
            }
        });
    }
}
function onKeyEnter(e) {
    search();
}
