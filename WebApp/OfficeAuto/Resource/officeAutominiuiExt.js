//永福extjs

function flowTraceByID(winSettings) {
    if (!winSettings||winSettings.ID == null)
        flowTrace();
    else {
        var settings = $.extend({ title: '流程跟踪', width: 1000 }, winSettings);
        openWindow('/MvcConfig/Workflow/Trace/Diagram?ID=' + winSettings.ID + '&FuncType=FlowTrace', settings);
    }
}


//设置表单元素为只读
function setFormItemDisabled(name, isDisable) {
    
    var Elements = name.split(',');
    for (var i = 0; i < Elements.length; i++) {
        var eleKey = Elements[i];
        if (eleKey == "") continue;
        
        var obj = mini.getbyName(eleKey);
        if (!obj)
            obj = mini.get(eleKey);
        if (obj) {
            if (obj.type == "datagrid") {
                obj.setAllowCellEdit(!isDisable);
                obj.setAllowCellSelect(!isDisable);
            }
            else {
                if (obj.setReadOnly) obj.setReadOnly(isDisable);
                if (obj.setEnabled) obj.setEnabled(!isDisable);
                if (obj.addCls) {
                    if(isDisable)
                        obj.addCls("asLabel");
                    else {
                        obj.removeCls("asLabel"); 
                    }
                }
            }
        }
        else {
            if (isDisable) {
                $("#" + eleKey).find("[name]").each(function (index) {
                    obj = mini.getbyName($(this).attr("name"));
                    if (obj.setReadOnly) obj.setReadOnly(true);
                    if (obj.addCls) obj.addCls("asLabel");
                });
            }
            else {
                $("#" + eleKey).find("[name]").each(function (index) {
                    obj = mini.getbyName($(this).attr("name"));
                    if (obj.setReadOnly) obj.setReadOnly(false);
                    if (obj.setEnabled) obj.setEnabled(true);
                    if (obj.removeCls) obj.removeCls("asLabel");         //增加asLabel外观
                });
            }
        }
        
    }
}
