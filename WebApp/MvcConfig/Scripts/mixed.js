var tabUrl = LayoutDef.tabs[0].url;
var tree = new mini.Tree("tree");
tree.set(LayoutDef.tree);
tree.load(LayoutDef.tree.url);

$(".gw-grid-toolbar&.mini-toolbar").removeClass("gw-grid-toolbar");
if (LayoutDef.tree.isHideSearch) {
    $("#txtSearch").hide();
}

var subArr = [];
var grid = new mini.DataGrid();;
function pageLoad() {
    loadTreeList();
    if (!LayoutDef.tree.isHideTopBtn) {
        $("#splitter").css("height", $("#splitter").height() - 25);
    } else {
        $("#topBtn").hide();
    }
}


function loadTreeList() {
    if (LayoutDef.tabs[0].isHide) {
        var tabs = mini.get("tabs");
        tabs.removeTab(0);
    }

    grid.set(LayoutDef.tabs[0]);
    grid.render(document.getElementById("gridTreeList"));
}


function getTitle(id, code, tabs, skipUrl, e) {
    $.ajax({
        url: LayoutDef.tree.nameUrl + "&keyCode=" + code + "&UID=" + user.UserOrgID + "&ID=" + treeId,
        data: { data: "" },
        type: "post",
        success: function (text) {
            var json = eval("(" + text + ")");
            tab = {};
            tab._nodeid = id;
            tab.name = id;
            tab.title = json.name;

            //这里拼接了url，实际项目，应该从后台直接获得完整的url地址
            if (json.permission == 0) {
                tab.url = "/MvcConfig/UI/Layout/PageView?TmplCode=" + code + getUrl(skipUrl, e);
                tabs.addTab(tab);
                tabs.activeTab(tab);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            msgUI(jqXHR.responseText);
        }
    });
}

var treeId;
tree.on("nodeclick", function (e) {
    treeId = e.node.ID;
    var url = tabUrl + "&id=" + treeId;
    grid.columns.add();
    grid.setUrl(url)
    grid.reload();

    var openWinKey;
    var tabs = mini.get("tabs");
    var skipUrl = LayoutDef.tree.skipUrl;

    if (!LayoutDef.tree.isOutPage) {
        $.ajax({
            url: getUrl(LayoutDef.tree.treeUrl, e),
            data: { data: "" },
            type: "post",
            success: function (text) {
                if (!text.data[0].OpenWinKey) {
                } else {
                    openWinKey = text.data[0].OpenWinKey.split(",");
                    for (var i = 0; i < openWinKey.length; i++) {
                        var id = i + 1;
                        var tab0 = tabs.getTab(0);
                        if (LayoutDef.tabs[0].isHide) {
                            tabs.removeAll();
                        } else {
                            tabs.removeAll(tab0);
                        }
                        getTitle(id, openWinKey[i], tabs, skipUrl, e);
                    }
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                msgUI(jqXHR.responseText);
            }
        });

    } else {
        tab = {};
        tab._nodeid = id;
        tab.name = id;
        tab.title = LayoutDef.tree.nameUrl;

        tabs.removeAll();
        tab.url = getUrl(LayoutDef.tree.treeUrl, e);
        tabs.addTab(tab);
        tabs.activeTab(tab);
    }

});

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

function addRow() {
    var fid = LayoutDef.tabs[0].fid;
    var newRow = "";
    var node = tree.getSelectedNode();
    if (node) {
        eval("newRow = { " + fid + ": " + node.ID + " }")
    }
    //else {
    //    newRow = { pid: "" }
    //}
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



function onButtonEdit(e) {
    var btnEdit = this;

    mini.open({
        url: "/MvcConfig/Auth/Org/Selector?SelectMode=Multi",
        title: "组织树",
        width: 400,
        height: 600,
        ondestroy: function (action) {

            if (action) {
                var ids = [], texts = [];
                for (var i = 0; i < action.length; i++) {
                    ids.push(action[i].ID);
                    texts.push(action[i].Name);
                }
                btnEdit.setValue(ids.join(","));
                btnEdit.setText(texts.join(","));
            }
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
            if (LayoutDef.tree.isAsyn) {
                var node = tree.getSelectedNode();
                if (node) {
                    tree.loadNode(node);
                }
            } else {
                tree.load(LayoutDef.tree.url);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            msgUI(jqXHR.responseText);
        }
    });
}

