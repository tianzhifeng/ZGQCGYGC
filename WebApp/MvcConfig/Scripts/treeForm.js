
var tree = new mini.Tree("tree");
var treeUrl = LayoutDef.tree.url;
tree.set(LayoutDef.tree);
tree.load(getUrl(treeUrl));
$(".mini-grid-rows-view$.mini-grid-hidden-y").css("height", $("#splitter").height() - 5);
function pageLoad() {
    loadData(LayoutDef.form.url + "&ID=" + tree.getAllChildNodes()[0]["ID"]);
    getForm();
}

function getForm() {
    $("#table1").css("width", $("#table1").width() - 15);
    var kw = ($("#table1").width() - 246) / 3;
    var t = ""; //控件代码
    var w = 0;
    var table = document.getElementById("table1");
    var array = new Array()
    for (var j = 0; j < LayoutDef.form.columns.length; j++) {
        if (LayoutDef.form.columns[j].visible) {
            array.push(LayoutDef.form.columns[j]);
        } else {
            t += "<tr><td style='width:80px;display:none'  id='" + LayoutDef.form.columns[j].name + "'></td></tr>";
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
        }  else if (w % 6 == 0 && colspan != 5 && i != (array.length - 1)) {
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
        itemSettings.width = !itemSettings.width ? width : itemSettings.width;
        if (itemSettings.readOnly) {
            item.addCls("asLabel");
        }
        item.set(itemSettings);
        var txt = document.getElementById(itemSettings.name);
        item.render(txt);
    }
}
var treeId;
tree.on("nodeselect", function (e) {
    treeId = e.node.ID;
    var url = getUrl(LayoutDef.form.url, e);
    loadData(url);
});

function onChangeValue(e) {
    var id = e.selected.id;
    if (!id)
        id = e.selected.value;
    for (var i = 0; i < LayoutDef.form.columns.length; i++) {
        var itemSettings = LayoutDef.form.columns[i];
        if (itemSettings.linkColumn == e.sender.id && itemSettings.linkValue && itemSettings.type != "ComboBox") {
            var data = itemSettings.linkValue;
            var txtName = itemSettings.name;
            loadAjax(txtName, data, id);
        } else if (itemSettings.linkColumn == e.sender.id && itemSettings.type == "ComboBox") {
            var positionCombo = mini.get(itemSettings.name);
            onComboChanged(id);
            function onComboChanged(id) {
                positionCombo.setValue("");
                var url = "/MvcConfig/UI/Layout/GetLinkData?tmplCode=" + LayoutDef.form.connName + "&data=" + itemSettings.linkValue + "&ID='" + id + "'";
                positionCombo.setUrl(url);

                positionCombo.select(0);
            }
        }
    }
}

function loadAjax(txtName, data, id) {
    $.ajax({
        url: "/MvcConfig/UI/Layout/onChangeValue?ConnName=" + LayoutDef.form.connName + "&ID=" + id,
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

function loadData(url) {
    var form = new mini.Form("#table1");
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

function save() {
    var form = new mini.Form("#table1");
    var data = form.getData();
    var json = "[" + mini.encode(data) + "]";
    var isNull = false;
    for (var i = 0; i < LayoutDef.form.columns.length; i++) {
        var txtName = LayoutDef.form.columns[i].name;
        if (LayoutDef.form.columns[i].required && $(":input[name='" + txtName + "']").val().length <= 0) {
            isNull = true;
            break;
        }
    }

    if (isNull) {
        msgUI("请把必填信息填写完整!");
    } else {
        $.ajax({
            url: LayoutDef.form.postUrl,
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
if (LayoutDef.tree.isHideSearch) {
    $("#txtSearch").hide();
}
$("#tree").css("height", $("#tree").height() - 40);
function search() {
    var key = mini.get("key").getValue();
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
