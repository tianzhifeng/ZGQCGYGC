$(".gw-grid-toolbar&.mini-toolbar").removeClass("gw-grid-toolbar");
var gridList = new mini.DataGrid();
var grid = new mini.DataGrid();
var gridUrl = LayoutDef.list.url;
var gridUrl2 = LayoutDef.list2.url;
var para = window.location.href.match(/[^\[]+(?=\])/gm);
function pageLoad() {
    getLists();
    if (typeof (beforeLoad) != "undefined")
        beforeLoad();
    if (para) {
        var url = getUrl(gridUrl, "");
        grid.columns.add();
        grid.setUrl(url)
        grid.reload();
    }
    if (!LayoutDef.list.searchColumn) {
        $("#key").hide();
    }
    if (!LayoutDef.list.expUrl) {
        $("#download").hide();
    }

    loadButton(LayoutDef.list, '_btn', 'list');
    loadButton(LayoutDef.list2, '_btn2', 'list');

    $("#queryForm").html(bindParameterHtml(LayoutDef.list.parameter));
}


function getLists() {
    grid.set(LayoutDef.list);
    grid.render(document.getElementById("divList"))
    gridList.set(LayoutDef.list2);
    gridList.render(document.getElementById("divList2"))

}

function filter() {
    var parameter = LayoutDef.list.parameter;
    var url = getUrl(gridUrl, "");
    var form = new mini.Form("#queryForm");
    var data = form.getData();
    var paras = {};
    if (parameter != "") {
        for (var i = 0; i < parameter.length; i++) {
            var re = new RegExp("[?&]" + parameter[i].value + "=([^\\&]*)", "i");
            var r = re.exec(url);
            var value = eval("form.el.$LK$" + parameter[i].value).value;
            if (parameter[i].value != null)
                paras["$IL$" + parameter[i].value] = value;
            if (url.indexOf(parameter[i].value) >= 0) {
                if (r != null)
                    url = url.replace(r[0], (r[0].indexOf('?') >= 0 ? '?' : '&') + parameter[i].value + '=' + value);
            } else {
                url = url + (url.indexOf('?') >= 0 ? '&' : '?') + parameter[i].value + '=' + value;
            }
        }
    }
    hideWindow("queryWindow");
    if (grid != undefined)
        grid.load({ quickQueryFormData: mini.encode(paras) });
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
                var c = { header: column.header, field: column.field, enumKey: column.enumKey };
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


var currentRow, rowValue;
grid.on("rowclick", function (e) {
    currentRow = e.record;
    rowValue = e.record;
    var url = getUrl(gridUrl2, e);
    gridList.columns.add();
    gridList.setUrl(url);
    gridList.reload();

});
gridList.on("rowclick", function (e) {
    rowValue = e.record;
});

function addRow(t) {
    var currentRowID;
    if (!para == false) {
        if (para.length > 0 && t == 1) {
            currentRowID = para;
        } else {
            currentRowID = !currentRow ? "" : currentRow.ID;
        }
    } else {
        currentRowID = !currentRow ? "" : currentRow.ID;
    }
    var gridRow = t == 1 ? eval(grid) : gridList;
    var newRowVal = "{fid:" + (currentRowID ? '\'' + currentRowID + '\'' : '\'\'') + defaultVal(gridRow) + "}";
    var newRow = eval('(' + newRowVal + ')');
    gridRow.addRow(newRow, 0);
    gridRow.deselectAll();
    gridRow.select(newRow);
}


function delRow(t) {
    var gridRow = t == 1 ? eval(grid) : gridList;
    var rows = gridRow.getSelecteds();
    if (rows.length > 0) {
        gridRow.removeRows(rows, true);
    }
}

function save(t) {
    var gridRow = t == 1 ? eval(grid) : gridList;
    var data = gridRow.getChanges();
    var postUrl = gridRow.postUrl;
    var json = mini.encode(data);
    $.ajax({
        url: postUrl,
        data: { data: json, fid: !para > 0 ? gridList.fid : gridRow.fid },
        type: "post",
        success: function (text) {
            msgUI("保存成功");
            gridRow.reload();
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
