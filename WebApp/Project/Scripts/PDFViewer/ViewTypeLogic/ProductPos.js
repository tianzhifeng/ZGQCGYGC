/* 说明
对象：
pdfPositionInfo：成果字段PDFSignPositionInfo相同结构的对象，保存时存到数据库
posDic：将pdfPositionInfo改造成易于使用的结构，只包含中心点位置信息
groupInfo：图章组合信息，包括图章ID，图章大小、从章等信息
tf：图幅
方法：
initPosition：页面初始化时，指定章的位置，需返回数组
*/
var pdfPositionInfo = {};
var posDic = {};
var groupInfo = {};
var tf = "A0";

var productID = getQueryString("ProductID");
var projectInfoID = getQueryString("ProjectInfoID");
var version = getQueryString("Version");

$(function () {
    if (funcType != "view") {
        $("#numPages").after('\
<a class="toolbarLabel productPos mini-button" id="savePosition" iconcls="icon-save" onclick="savePosition();">保存</a>\
<a class="toolbarLabel productPos mini-button" id="addTemplate" iconcls="icon-add" onclick="addTemplate();">套用模板</a>\
<a class="toolbarLabel productPos mini-button" id="addDiv" iconcls="icon-add" onclick="addStamp();">添加图章</a>\
<a class="toolbarLabel productPos mini-menubutton" id="addDiv" visible="false" iconcls="icon-add" menu="#popupMenu">添加签名域</a>\
<a class="toolbarLabel productPos mini-button" id="deleteDiv" iconcls="icon-remove" onclick="deleteDiv();">删除图章</a>\
<a class="toolbarLabel productPos mini-button" id="reSign" iconcls="mini-pager-reload" onclick="reSign();">重新盖章</a>\
<a class="toolbarLabel productPos mini-button" id="saveasTemplate" iconcls="icon-project" onclick="saveTempPosition(true);">存为模板</a>');
        $(".toolbar").after('\
<ul id="popupMenu" class="mini-contextmenu">\
<li text="添加图章" iconcls="icon-add" id="btnSelectSeal" onclick="addStamp()"></li>\
<li text="添加角色签名" iconcls="icon-add" id="btnUseTemplate" onclick="useTemplate()"></li>\
<li text="添加会签签名" iconcls="icon-add" id="btnView" onclick="viewPDF()" ></li>\
<li text="添加会签日期" iconcls="icon-add" id="btnView" onclick="viewPDF()" ></li>\
</ul>');
        mini.parse();
    }
    if (getQueryString("IsApply") != "true") {
        addExecuteParam("ProductID", productID);
        addExecuteParam("Version", version);
        execute("GetRelateInfo", {
            showLoading: true, async: false, onComplete: function (data) {
                pdfPositionInfo = data["Position"];
                groupInfo = data["GroupInfo"];
                config = getConfig(pdfPositionInfo["BorderConfigs"]);
                tf = data["TF"];
            }
        });
    }
    else if (window.parent && typeof (window.parent.getPosInfo) != "undefined") {
        setData(window.parent.getPosInfo());
    }
});

function initPosition() {
    var PDFPostions = [];
    //pdf盖章工具需要PaperWidth、PaperHeight
    //使用【套用模板】功能时，没有这些信息
    if (!pdfPositionInfo["FrameType"]) {
        pdfPositionInfo["FrameType"] = currentWidth > currentHeight ? "横框" : "竖框";
    }
    if (!pdfPositionInfo["FrameSize"]) {
        pdfPositionInfo["FrameSize"] = tf;
    }
    if (!pdfPositionInfo["PaperWidth"]) {
        pdfPositionInfo["PaperWidth"] = px2mm(currentWidth / currentScale);
    }
    if (!pdfPositionInfo["PaperHeight"]) {
        pdfPositionInfo["PaperHeight"] = px2mm(currentHeight / currentScale);
    }
    if (pdfPositionInfo["AuditRoles"]) {
        for (var i = 0; i < pdfPositionInfo.AuditRoles.length; i++) {
            var signName = pdfPositionInfo.AuditRoles[i];
            var type = "";
            if (pdfPositionInfo["AuditRolesType"] && pdfPositionInfo.AuditRolesType[i])
                type = pdfPositionInfo.AuditRolesType[i];
            var angle = 0, width = 0, height = 0;
            var groupID = "", groupName = "", correntX = 0, correntY = 0;
            var follows = [], sealID = "", category = "";

            if (signName.indexOf("章") > -1) {
                if (groupInfo[signName]) {
                    angle = groupInfo[signName]["Angle"];
                    width = groupInfo[signName]["Width"];
                    height = groupInfo[signName]["Height"];
                    groupID = groupInfo[signName]["GroupID"];
                    groupName = groupInfo[signName]["GroupName"];
                    sealID = groupInfo[signName]["MainID"];
                    category = "图章";
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
                }
            }
            else if (signName.indexOf("条码") > -1) {
                if (config["barcode"]) {
                    angle = config["barcode"]["Angle"];
                    height = config["barcode"]["Height"];
                    correntX = config["barcode"]["CorrectPosX"];
                    correntY = config["barcode"]["CorrectPosY"];
                    category = "条码";
                }
            }
            else if (signName.indexOf("二维码") > -1) {
                if (config["qrcode"]) {
                    angle = config["qrcode"]["Angle"];
                    width = config["qrcode"]["Width"];
                    height = config["qrcode"]["Height"];
                    correntX = config["qrcode"]["CorrectPosX"];
                    correntY = config["qrcode"]["CorrectPosY"];
                    category = "二维码";
                }
            }
            else if (type == "会签" || signName.indexOf("会签") > -1) {
                if (config["cosign"]) {
                    angle = config["cosign"]["Angle"];
                    width = config["cosign"]["Width"];
                    height = config["cosign"]["Height"];
                    correntX = config["cosign"]["CorrectPosX"];
                    correntY = config["cosign"]["CorrectPosY"];
                    category = "会签";
                }
            }
            else {
                if (config["sign"]) {
                    angle = config["sign"]["Angle"];
                    width = config["sign"]["Width"];
                    height = config["sign"]["Height"];
                    correntX = config["sign"]["CorrectPosX"];
                    correntY = config["sign"]["CorrectPosY"];
                    category = "签名";
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
                PosX: pdfPositionInfo.PositionXs[i],
                PosY: pdfPositionInfo.PositionYs[i],
                IsMain: true,
                Category: category
            });
        }
    }
    return PDFPostions;
}

function savePosition() {
    var divs = $(".textLayer")[0].childNodes;
    pdfPositionInfo["AuditRoles"] = [];
    pdfPositionInfo["AuditRolesType"] = [];
    pdfPositionInfo["PositionXs"] = [];
    pdfPositionInfo["PositionYs"] = [];
    pdfPositionInfo["BorderConfigs"] = [];
    var groupIDs = []; var groupNames = []; var keys = [];
    for (var i = 0; i < divs.length; i++) {
        var div = divs[i];
        if (div.dataset.isMain == "true") {
            if (div.dataset.groupID) {
                groupIDs.push(div.dataset.groupID);
                groupNames.push(div.dataset.groupName);
                keys.push(div.dataset.BlockKey);
            }
            pdfPositionInfo["AuditRoles"].push(div.dataset.BlockKey);
            pdfPositionInfo["PositionXs"].push(parseFloat(div.dataset.posX));
            pdfPositionInfo["PositionYs"].push(parseFloat(div.dataset.posY));
            pdfPositionInfo["AuditRolesType"].push(div.dataset.category);
        }
    }
    for (let key in config) {
        pdfPositionInfo["BorderConfigs"].push(config[key]);
    }

    msgUI("确认保存吗？", 2, function (data) {
        if (data == "ok") {
            addExecuteParam("ProductID", productID);
            addExecuteParam("Version", version);
            addExecuteParam("GroupIDs", groupIDs.join(','));
            addExecuteParam("GroupNames", groupNames.join(','));
            addExecuteParam("GroupKeys", keys.join(','));
            addExecuteParam("PdfPositionInfo", mini.encode(pdfPositionInfo));
            execute("SavePosition", {
                showLoading: true, refresh: false, onComplete: function (data) {
                    if (top["win"].savePosition)
                        top["win"].savePosition(productID, mini.encode(pdfPositionInfo), groupIDs.join(','), groupNames.join(','), keys.join(','));
                    msgUI("保存成功。");
                }, validateForm: false
            });
        }
    });
}

function addTemplate() {
    openWindow("/MvcConfig/UI/List/PageView?TmplCode=PlotSealTemplateSingleSelector&PrjID=" + projectInfoID, {
        title: "选择模板", addQueryString: false, onDestroy: function (data) {
            if (data != "close" && data && data.length > 0) {
                var divs = $(".textLayer")[0].childNodes;
                for (var i = 0; i < divs.length; i++) {
                    var div = divs[i];
                    var _parentElement = div.parentNode;
                    _parentElement.removeChild(div);
                    i--;
                }
                addExecuteParam("TemplateID", data[0].ID);
                execute("GetTemplate", {
                    showLoading: true, addQueryString: false, onComplete: function (result) {
                        var groups = result.GroupList;
                        config = getConfig(result.BorderConfig);
                        for (var i = 0; i < groups.length; i++) {
                            var group = groups[i];
                            var poxX = mm2px(group["PositionXs"]) * currentScale;//左下为圆心
                            var poxY = mm2px(group["PositionYs"]) * currentScale;
                            var stamp = {};
                            if (group["Category"] != "图章") 
                                stamp = fillSign(group, config);
                            else
                                stamp = fillGroupSign(group);
                            stamp.PosX = group["PositionXs"];
                            stamp.PosY = group["PositionYs"];
                            stamp.Left = poxX - stamp["Width"] / 2 - 1;
                            stamp.Bottom = poxY - stamp["Height"] / 2 - 1;
                            for (var j = 0; j < stamp.Follows.length; j++) {
                                //RelativeX 以main为原点的坐标系
                                stamp.Follows[j].Left = poxX + stamp.Follows[j].RelativeX - stamp.Follows[j]["Width"] / 2 - 1;
                                stamp.Follows[j].Bottom = poxY + stamp.Follows[j].RelativeY - stamp.Follows[j]["Height"] / 2 - 1;
                            }
                            appendSign(stamp);
                        }
                    }
                });
            }
        }
    });
}

function addStamp() {
    openWindow("/MvcConfig/UI/List/PageView?TmplCode=PlotSealGroupSingleSelector", {
        onDestroy: function (data) {
            if (data != "close" && data && data.length > 0) {
                addExecuteParam("GroupID", data[0].ID);
                execute("GetPlotSealGroupInfo", {
                    showLoading: true, onComplete: function (newSealGroupInfo) {
                        var sign = fillGroupSign(newSealGroupInfo);
                        newSign = sign;
                    }
                });
            }
        }
    });
}

function reSign() {
    msgUI("确认重新盖章吗？", 2, function (data) {
        if (data == "ok") {
            var list = [{ ProductID: productID, Version: version }];
            addExecuteParam("Products", mini.encode(list));
            execute("ReSign", {
                showLoading: true, refresh: false, onComplete: function (data) {
                    msgUI("保存成功。");
                }, validateForm: false
            });
        }
    });
}

function setData(data) {
    if (data && data.IsApply) {
        pdfPositionInfo = data["PDFSignPositionInfo"];
        if (!pdfPositionInfo)
            pdfPositionInfo = {};
        else
            config = getConfig(pdfPositionInfo["BorderConfigs"]);

        addExecuteParam("GroupIDs", data["PlotSealGroup"]);
        execute("GetPlotSealGroupInfos", {
            showLoading: true, async: false, onComplete: function (rtn) {
                groupInfo = rtn["GroupInfo"];
                var leftBottom = 100;
                if (!pdfPositionInfo.AuditRoles || !pdfPositionInfo.AuditRoles.length) {
                    pdfPositionInfo["AuditRoles"] = []; pdfPositionInfo["PositionXs"] = []; pdfPositionInfo["PositionYs"] = [];
                }
                for (var g in groupInfo) {
                    if (pdfPositionInfo["AuditRoles"].indexOf(g) < 0) {
                        pdfPositionInfo["AuditRoles"].push(g);
                        pdfPositionInfo["PositionXs"].push(leftBottom);
                        pdfPositionInfo["PositionYs"].push(leftBottom);
                        leftBottom += 50;
                    }
                }
            }
        });
    }
}