
$(".gw-grid-toolbar&.mini-toolbar").removeClass("gw-grid-toolbar");


var para = window.location.href.match(/[^\[]+(?=\])/gm);
function pageLoad() {
    getForm();
    loadData();

}

function loadData() {
    var form = new mini.Form("#table1");
    var url = getUrl(LayoutDef.form[0].url, "");
    $.ajax({
        url: url,
        type: "post",
        success: function (text) {
            var json = mini.encode(text.data).substr(1, mini.encode(text.data).length - 2);
            var data = mini.decode(json);
            form.setData(data);
        }
    });
}

function getForm() {
    $("#table1").css("width", $("#divs").width() - 20);
    var kw = ($("#table1").width() - 240) / 3;
    var t = ""; //控件代码
    var w = 0;
    var table = document.getElementById("form1");
    var array = new Array()
    for (var j = 0; j < LayoutDef.form[0].columns.length; j++) {
        if (LayoutDef.form[0].columns[j].visible) {
            array.push(LayoutDef.form[0].columns[j]);
        } else {
            t += "<tr><td style='width:80px;display:none'  id='" + LayoutDef.form[0].columns[j].name + "'></td></tr>";
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
}

function onChangeValue(e) {
    var id = e.selected.id;
    if (!id)
        id = e.selected.value;
    for (var i = 0; i < LayoutDef.form[0].columns.length; i++) {
        var itemSettings = LayoutDef.form[0].columns[i];
        if (itemSettings.linkColumn == e.sender.id && itemSettings.linkValue && itemSettings.type != "ComboBox") {
            var data = itemSettings.linkValue;
            var txtName = itemSettings.name;
            loadAjax(txtName, data, id);
        } else if (itemSettings.linkColumn == e.sender.id && itemSettings.type == "ComboBox") {
            var positionCombo = mini.get(itemSettings.name);
            onComboChanged(id);
            function onComboChanged(id) {
                positionCombo.setValue("");
                var url = "/MvcConfig/UI/Layout/GetLinkData?tmplCode=" + LayoutDef.form[0].connName + "&data=" + itemSettings.linkValue + "&ID='" + id + "'";
                positionCombo.setUrl(url);

                positionCombo.select(0);
            }
        }
    }
}

function loadAjax(txtName, data, id) {
    $.ajax({
        url: "/MvcConfig/UI/Layout/onChangeValue?ConnName=" + LayoutDef.form[0].connName + "&ID=" + id,
        data: { data: data },
        type: "post",
        success: function (text) {
            $(":input[name='" + txtName + "']").val(text);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            msgUI(errorThrown);
        }
    });
}


function addRow() {
    var newRow = { fid: para };

    grid.addRow(newRow, 0);
    grid.deselectAll();
    grid.select(newRow);
}


function delRow() {
    var rows = grid.getSelecteds();
    if (rows.length > 0) {
        grid.removeRows(rows, true);
    }
}

function saveForm() {

    var form = new mini.Form("#table1");
    var data = form.getData();
    var json = "[" + mini.encode(data) + "]";

    var isNull = false;
    //for (var i = 0; i < LayoutDef.form[0].columns.length; i++) {
    //    var txtName = LayoutDef.form[0].columns[i].name;
    //    if (LayoutDef.form[0].columns[i].required && $(":input[name='" + txtName + "']").val().length <= 0) {
    //        isNull = true;
    //        break;
    //    }
    //}
    $.ajax({
        url: LayoutDef.form[0].postUrl,
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

function saveList() {
    var data = grid.getChanges();
    var postUrl = grid.postUrl;
    var json = mini.encode(data);

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


