
//initPosition 页面初始化时，指定章的位置，需返回数组
//onDrawCanvas 初始化画布前
//盖章模板预览
var tempGroups = [];

$(function () {
    if (funcType != "view") {
        $("#numPages").after('\
<a class="toolbarLabel tempPos mini-button" id="saveTempPosition" iconcls="icon-save" onclick="saveTempPosition(false);">保存</a>\
<a class="toolbarLabel tempPos mini-button" id="addTempStamp" iconcls="icon-add" onclick="addStamp();">添加图章</a>\
<a class="toolbarLabel tempPos mini-button" id="deleteTempDiv" iconcls="icon-remove" onclick="deleteDiv();">删除图章</a>');
        mini.parse();
    }
});

function onDrawCanvas(canvasWrapper) {
    canvasWrapper.classList.add('blur');
}

function setData(data) {
    if (data) {
        if (data["GroupList"]) tempGroups = data["GroupList"];
        if (data["BorderConfig"]) config = getConfig(data["BorderConfig"]);
    }
}

function initPosition() {
    var pos = [];
    for (var i = 0; i < tempGroups.length; i++) {
        var group = tempGroups[i];
        var poxX = mm2px(group["PositionXs"]) * currentScale;//左下为圆心
        var poxY = mm2px(group["PositionYs"]) * currentScale;
        var stamp = {};
        if (group["Category"] != "图章")
            stamp = fillSign(group);
        else {
            addExecuteParam("GroupID", group["GroupID"]);
            execute("GetPlotSealGroupInfo", {
                async: false, showLoading: true, onComplete: function (newSealGroupInfo) {
                    stamp = fillGroupSign(newSealGroupInfo);
                    stamp["Category"] = "图章";
                    for (var i = 0; i < stamp.Follows.length; i++) {
                        //RelativeX 以main为原点的坐标系
                        stamp.Follows[i].Left = poxX + stamp.Follows[i].RelativeX - stamp.Follows[i]["Width"] / 2 - 1;
                        stamp.Follows[i].Bottom = poxY + stamp.Follows[i].RelativeY - stamp.Follows[i]["Height"] / 2 - 1;
                    }
                }
            });
        }
        stamp.PosX = group["PositionXs"];
        stamp.PosY = group["PositionYs"];
        stamp.Left = poxX - stamp["Width"] / 2 - 1;
        stamp.Bottom = poxY - stamp["Height"] / 2 - 1;
        pos.push(stamp);
    }
    return pos;
}