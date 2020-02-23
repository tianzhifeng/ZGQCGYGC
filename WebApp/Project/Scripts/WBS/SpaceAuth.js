

function setAuth() {
    var authType = getQueryString("AuthType");
    if (authType == "View") {
//        $(".mini-toolbar").hide();
        $(".mini-button").hide();
        $(".mini-button:contains('详细查询')").show();
        $(".mini-menubutton").hide();
        $(".mini-datagrid").attr("allowcelledit", "false");
        $(".mini-datagrid").attr("allowcellselect", "false");
        $(".mini-treegrid").attr("allowcelledit", "false");
        $(".mini-treegrid").attr("allowcellselect", "false");
    }
}
