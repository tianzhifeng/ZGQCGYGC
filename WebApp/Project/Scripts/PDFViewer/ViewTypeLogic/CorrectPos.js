
//var viewType = getQueryString("ViewType");
//var funcType = getQueryString("FuncType").toLowerCase();
//var currentWidth = 0;
//var currentHeight = 0;
//var currentScale = 1;
//initPosition 页面初始化时，指定章的位置，需返回数组
var pdfPositionInfo = {};
var groupInfo = {};
var config = [];

$(function () {
    if (funcType != "view") {
        $("#numPages").after('\
        <span id="saveCorrectPos" class="toolbarLabel correctPos icon-save" title="确定" onclick="saveCorrectPos();">确定</span>\
        ');
    }
});

function setData(data) {
    if (data && data.length) {
        var stamp = {}, follows = [];
        pdfPositionInfo["AuditRoles"] = [];
        pdfPositionInfo["PositionXs"] = [150];
        pdfPositionInfo["PositionYs"] = [100];
        for (var i = 0; i < data.length; i++) {
            if (data[i].IsMain == "true") {
                pdfPositionInfo["AuditRoles"].push(data[i]["BlockKey"]);
                stamp = {
                    BlockKey: data[i]["BlockKey"],
                    GroupID: "temp",
                    GroupName: "temp",
                    MainID: data[i]["PlotSeal"],
                    Name: data[i]["Name"],
                    Width: data[i]["Width"],
                    Height: data[i]["Height"]
                };
            }
            else {
                follows.push({
                    FollowID: data[i]["PlotSeal"],
                    Name: data[i]["Name"],
                    Width: data[i]["Width"],
                    Height: data[i]["Height"],
                    CorrectPosX: data[i]["CorrectPosX"],
                    CorrectPosY: data[i]["CorrectPosY"]
                });
            }
        }
        stamp["Follows"] = follows;
        groupInfo[stamp["BlockKey"]] = stamp;
    }
}

function initPosition() {
    var PDFPostions = [];
    for (var i = 0; i < pdfPositionInfo.AuditRoles.length; i++) {
        var signName = pdfPositionInfo.AuditRoles[i];
        var pos = { "Name": signName };
        var angle = 0, width = 0, height = 0;
        var groupID = "", groupName = "", correntX = 0, correntY = 0;
        var follows = [], sealID = "";

        angle = groupInfo[signName]["Angle"];
        width = groupInfo[signName]["Width"];
        height = groupInfo[signName]["Height"];
        groupID = groupInfo[signName]["GroupID"];
        groupName = groupInfo[signName]["GroupName"];
        sealID = groupInfo[signName]["MainID"];
        if (groupInfo[signName]["Follows"].length > 0) {
            var fs = groupInfo[signName]["Follows"];
            for (var j = 0; j < fs.length; j++) {
                follows.push({
                    SealID: fs[j]["FollowID"],
                    Name: fs[j]["Name"],
                    Left: mm2px(pdfPositionInfo.PositionXs[i] + fs[j]["CorrectPosX"] - fs[j]["Width"] / 2) * currentScale - 1,
                    Bottom: mm2px(pdfPositionInfo.PositionYs[i] + fs[j]["CorrectPosY"] - fs[j]["Height"] / 2) * currentScale - 1,
                    Width: mm2px(fs[j]["Width"]) * currentScale,
                    Height: mm2px(fs[j]["Height"]) * currentScale,
                    Angle: fs[j]["Angle"],
                    GroupID: groupID,
                    GroupName: groupName,
                    IsMain: false
                });
            }
        }

        PDFPostions.push({
            Name: signName,
            Left: mm2px(pdfPositionInfo.PositionXs[i] + correntX - width / 2) * currentScale - 1,
            Bottom: mm2px(pdfPositionInfo.PositionYs[i] + correntY - height / 2) * currentScale - 1,
            Width: mm2px(width) * currentScale,
            Height: mm2px(height) * currentScale,
            Angle: angle,
            GroupID: groupID,
            GroupName: groupName,
            Follows: follows,
            SealID: sealID,
            IsMain: true
        });
    }
    return PDFPostions;
}

function saveCorrectPos() {
    var RelativePos = [];
    var divs = $(".textLayer")[0].childNodes;
    var mainX = 0, mainY = 0;
    for (var i = 0; i < divs.length; i++) {
        var div = divs[i];
        if (div.dataset.isMain == "true") {
            mainX = parseFloat(div.dataset.left) + parseFloat(div.dataset.width) / 2 + 1;
            mainY = parseFloat(div.dataset.bottom) + parseFloat(div.dataset.height) / 2 + 1;
        }
    }
    for (var i = 0; i < divs.length; i++) {
        var div = divs[i];
        if (div.dataset.isMain != "true") {
            RelativePos.push({
                PlotSeal: div.dataset.sealID,
                CorrectX: px2mm((parseFloat(div.dataset.left) + parseFloat(div.dataset.width) / 2 + 1 - mainX) / currentScale),
                CorrectY: px2mm((parseFloat(div.dataset.bottom) + parseFloat(div.dataset.height) / 2 + 1 - mainY) / currentScale)
            });
        }
    }
    closeWindow(RelativePos);
}