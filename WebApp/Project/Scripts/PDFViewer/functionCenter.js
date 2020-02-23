/* 说明
对象：
startX、startY：点击div时记录的clientX、clientY，用于移动时的计算
moveSwitch：是否是选中div时鼠标移动的标志
currentDivs：当前选中div的数组，如果是出图章组合就包含组合内的所有div
currentLefts、currentBottoms：当前选中div的left、bottom，用于移动时的计算
currentPosXs、currentPosYs：当前选中div的中心点位置，保存时从div上记录的中心点位置同步到数据库
newSign：是否是新增加的div的标志
config：签名、会签、二维码、条码的配置信息
方法：
initPosition：页面初始化时，指定章的位置，需返回数组
*/
var viewType = getQueryString("ViewType");
var funcType = getQueryString("FuncType").toLowerCase();
document.write('<script src="/Project/Scripts/PDFViewer/ViewTypeLogic/' + viewType + '.js" type="text/javascript"></script>');

function getSignPositions(cwidth, cheight, scale) {
    var PDFPostions = [];
    currentScale = scale;
    currentWidth = cwidth;
    currentHeight = cheight;
    if (typeof (initPosition) != "undefined")
        PDFPostions = initPosition();
    ($(".textLayer")[0]).onmousedown = function (e) {
        if (newSign) {
            newSign.Left = e.offsetX - newSign["Width"] / 2 - 1;
            newSign.Bottom = currentHeight - (e.offsetY + newSign["Height"] / 2 + 1);
            newSign.PosX = px2mm(e.offsetX / currentScale);
            newSign.PosY = px2mm((currentHeight - e.offsetY) / currentScale);
            for (var i = 0; i < newSign.Follows.length; i++) {
                newSign.Follows[i].Left = e.offsetX + newSign.Follows[i].RelativeX - newSign.Follows[i]["Width"] / 2 - 1;
                newSign.Follows[i].Bottom = currentHeight - (e.offsetY + (-newSign.Follows[i].RelativeY) + newSign.Follows[i]["Height"] / 2 + 1);
            }
            appendSign(newSign);
            newSign = false;
        }
    }
    return PDFPostions;
}

var startX;
var startY;
var moveSwitch = false;
var currentDivs = [];
var currentLefts = [];
var currentBottoms = [];
var currentPosXs = [];
var currentPosYs = [];

var newSign = false;
var config = {};

document.onmousemove = function (e) {
    if (moveSwitch && funcType != "view") {
        var x = e.clientX;
        var y = e.clientY;
        var distanceX = x - startX;
        var distanceY = startY - y;
        $.each(currentDivs, function (i, item) {
            item.style.left = (distanceX + currentLefts[i]) + "px";
            item.style.bottom = (distanceY + currentBottoms[i]) + "px";
            item.dataset.left = distanceX + currentLefts[i];
            item.dataset.bottom = distanceY + currentBottoms[i];
            if (item.dataset.posX) {
                item.dataset.posX = px2mm(distanceX / currentScale) + parseFloat(currentPosXs[i]);
                item.dataset.posY = px2mm(distanceY / currentScale) + parseFloat(currentPosYs[i]);
            }
        });
    }
}

document.onmouseup = function (e) {
    $.each(currentDivs, function (i, item) {
        item.style.border = "1px solid"
    });
    currentLefts = [];
    currentBottoms = [];
    currentPosXs = [];
    currentPosYs = [];
}

document.onkeyup = function (event) {
    var e = event || window.event;
    if (funcType != "view") {
        var keyCode = e.keyCode || e.which;
        switch (keyCode) {
            //case 37:
            //    moveDiv(-1, 0);
            //    break;
            //case 38:
            //    moveDiv(0, 1);
            //    break;
            //case 39:
            //    moveDiv(1, 0);
            //    break;
            //case 40:
            //    moveDiv(0, -1);
            //    break;
            case 46:
                deleteDiv();
                break;
            default:
                break;
        }
    }
}

function moveDiv(left, bottom) {
    $.each(currentDivs, function (i, item) {
        item.style.left = (currentLefts[i] + left) + "px";
        item.style.bottom = (currentBottoms[i] + bottom) + "px";
        item.dataset.left = currentLefts[i] + left;
        item.dataset.bottom = currentBottoms[i] + bottom;
        currentLefts[i] += left;
        currentBottoms[i] += bottom;
    });
}

function getConfig(configArray) {
    var c = {};
    if (configArray)
        for (var i = 0; i < configArray.length; i++) {
            c[configArray[i]["Category"].toLowerCase()] = configArray[i];
        }
    return c;
}

function mouseDown(e) {
    e.stopPropagation();
    $.each(currentDivs, function (i, item) {
        item.style.border = "1px solid"
    });
    currentDivs = [];
    currentLefts = [];
    currentBottoms = [];
    currentPosXs = [];
    currentPosYs = [];

    e = e ? e : window.event;
    var currentDiv = e.target;
    if (viewType == "CorrectPos") {
        currentDiv.style.border = "1px solid #00188c";
        currentDivs.push(currentDiv);
        currentLefts.push(parseFloat(currentDiv.dataset.left));
        currentBottoms.push(parseFloat(currentDiv.dataset.bottom));
    }
    else {
        if (currentDiv.dataset.groupID) {
            var divs = $(".textLayer")[0].childNodes;
            $.each(divs, function (i, item) {
                if (item.dataset.groupID == currentDiv.dataset.groupID) {
                    item.style.border = "1px solid #00188c";
                    currentDivs.push(item);
                    currentLefts.push(parseFloat(item.dataset.left));
                    currentBottoms.push(parseFloat(item.dataset.bottom));
                    if (item.dataset.posX) {
                        currentPosXs.push(parseFloat(item.dataset.posX));
                        currentPosYs.push(parseFloat(item.dataset.posY));
                    }
                    else {
                        currentPosXs.push(0);
                        currentPosYs.push(0);
                    }

                }
            });
        }
        else {
            currentDiv.style.border = "1px solid #00188c";
            currentDivs.push(currentDiv);
            currentLefts.push(parseFloat(currentDiv.dataset.left));
            currentBottoms.push(parseFloat(currentDiv.dataset.bottom));
            currentPosXs.push(parseFloat(currentDiv.dataset.posX));
            currentPosYs.push(parseFloat(currentDiv.dataset.posY));
        }
    }
    startX = e.clientX;
    startY = e.clientY;
    moveSwitch = true;
}

function mouseUp(e) {
    e.stopPropagation();
    moveSwitch = false;
}


var dpi = [];
function mm2px(mm) {
    if (dpi.length == 0)
        dpi = getDPI();
    var pixel = parseFloat(mm) / 25.4 * dpi[0];
    return (Math.round(pixel * 100000000) / 100000000)
}

function px2mm(px) {
    if (dpi.length == 0)
        dpi = getDPI();
    var mm = parseFloat(px) * 25.4 / dpi[0];
    return (Math.round(mm * 100000000) / 100000000)
}

function getDPI() {
    var arrDPI = new Array();
    if (window.screen.deviceXDPI != undefined) {
        arrDPI[0] = window.screen.deviceXDPI;
        arrDPI[1] = window.screen.deviceYDPI;
    } else {
        var tmpNode = document.createElement("DIV");
        tmpNode.style.cssText = "width:1in;height:1in;position:absolute;left:0px;top:0px;z-index:99;visibility:hidden";
        document.body.appendChild(tmpNode);
        arrDPI[0] = parseInt(tmpNode.offsetWidth);
        arrDPI[1] = parseInt(tmpNode.offsetHeight);
        tmpNode.parentNode.removeChild(tmpNode);
    }
    return arrDPI;
}


function fillGroupSign(groupInfo) {
    var sign = {
        Name: groupInfo["BlockKey"],
        Left: 0,
        Bottom: 0,
        Width: mm2px(groupInfo["Width"]) * currentScale,
        Height: mm2px(groupInfo["Height"]) * currentScale,
        Angle: groupInfo["Angle"],
        GroupID: groupInfo["GroupID"],
        GroupName: groupInfo["GroupName"],
        Follows: [],
        SealID: groupInfo["MainID"],
        PosX: 0,
        PosY: 0,
        IsMain: true,
        Category: groupInfo["Category"]
    };
    if (groupInfo["Follows"] && groupInfo["Follows"].length > 0) {
        var fs = groupInfo["Follows"];
        for (var j = 0; j < fs.length; j++) {
            sign.Follows.push({
                SealID: fs[j]["FollowID"],
                Name: fs[j]["Name"],
                Width: mm2px(fs[j]["Width"]) * currentScale,
                Height: mm2px(fs[j]["Height"]) * currentScale,
                Angle: fs[j]["Angle"],
                GroupID: groupInfo["GroupID"],
                GroupName: groupInfo["GroupName"],
                IsMain: false,
                RelativeX: mm2px(fs[j]["CorrectPosX"]) * currentScale,
                RelativeY: mm2px(fs[j]["CorrectPosY"]) * currentScale,
                Left: 0,
                Bottom: 0
            });
        }
    }
    return sign;
}

function fillSign(groupInfo) {
    var sign = {};
    var c = {};
    if (groupInfo["Category"] == "条码") c = config["barcode"];
    if (groupInfo["Category"] == "二维码") c = config["qrcode"];
    if (groupInfo["Category"] == "签名") c = config["sign"];
    if (groupInfo["Category"] == "会签") c = config["cosign"];
    if (c)
        sign = {
            Name: groupInfo["BlockKey"],
            Left: 0,
            Bottom: 0,
            Width: mm2px(c["Width"]) * currentScale,
            Height: mm2px(c["Height"]) * currentScale,
            Angle: c["Angle"],
            GroupID: "",
            GroupName: "",
            Follows: [],
            SealID: "",
            PosX: 0,
            PosY: 0,
            IsMain: true,
            Category: groupInfo["Category"]
        };
    return sign;
}

function appendSign(sign) {
    var textLayer = $(".textLayer")[0];
    var textDiv = document.createElement('div');
    textDiv.dataset.width = (sign.Width - 2);
    textDiv.dataset.height = (sign.Height - 2);
    textDiv.dataset.left = sign.Left;
    textDiv.dataset.bottom = sign.Bottom;
    textDiv.dataset.angle = sign.Angle;
    textDiv.dataset.category = sign.Category;

    textDiv.style.width = textDiv.dataset.width + "px";
    textDiv.style.height = textDiv.dataset.height + "px";
    textDiv.style.left = textDiv.dataset.left + 'px';
    textDiv.style.bottom = textDiv.dataset.bottom + 'px';
    textDiv.style.fontFamily = "serif";
    textDiv.style.fontSize = (sign.Height - 3) + "px";
    textDiv.style.color = "red";
    textDiv.style.border = "1px solid";
    textDiv.style.cursor = "move";
    textDiv.style.overflow = "hidden";
    if (sign.Angle && sign.Angle !== 0) {
        textDiv.style.transform = "rotate(" + (0 - sign.Angle) + "deg)";
        textDiv = calAngle(textDiv, sign.Angle);
    }
    if (sign.PosX) {
        textDiv.dataset.posX = sign.PosX;
        textDiv.dataset.posY = sign.PosY;
    }
    if (sign.GroupID) {
        var src = "/Project/AutoUI/PlotSealInfo/GetSealPic?ID=" + sign.SealID;
        textDiv.style.backgroundImage = "url(" + src + ")";
        textDiv.style.backgroundSize = "100% 100%";
        textDiv.dataset.groupID = sign.GroupID;
        textDiv.dataset.groupName = sign.GroupName;
        textDiv.dataset.sealID = sign.SealID;
        if (sign.Follows && sign.Follows.length > 0) {
            for (var i = 0; i < sign.Follows.length; i++) {
                appendSign(sign.Follows[i]);
            }
        }
    }
    else {
        textDiv.textContent = sign.Name;
    }
    textDiv.dataset.isMain = sign.IsMain;
    textDiv.dataset.BlockKey = sign.Name;
    textDiv.title = sign.Name;

    textDiv.onmousedown = mouseDown;
    textDiv.onmouseup = mouseUp;
    textLayer.appendChild(textDiv);
}

function calAngle(div, angle) {
    switch (angle) {
        case 90:
            div.dataset.left = (parseFloat(div.dataset.left) + (parseFloat(div.dataset.width) / 2 - parseFloat(div.dataset.height) / 2));
            div.dataset.bottom = (parseFloat(div.dataset.bottom) - (parseFloat(div.dataset.width) / 2 + parseFloat(div.dataset.height) / 2));
            div.style.left = div.dataset.left + "px";
            div.style.bottom = div.dataset.bottom + "px";
            break;
        case 180:
            div.dataset.left = (parseFloat(div.dataset.left) + parseFloat(div.dataset.width));
            div.dataset.bottom = (parseFloat(div.dataset.bottom) - parseFloat(div.dataset.height));
            div.style.left = div.dataset.left + "px";
            div.style.bottom = div.dataset.bottom + "px";
            break;
        case 270:
        case -90:
            div.dataset.left = (parseFloat(div.dataset.left) + (parseFloat(div.dataset.width) / 2 + parseFloat(div.dataset.height) / 2));
            div.dataset.bottom = (parseFloat(div.dataset.bottom) + (parseFloat(div.dataset.width) / 2 - parseFloat(div.dataset.height) / 2));
            div.style.left = div.dataset.left + "px";
            div.style.bottom = div.dataset.bottom + "px";
            break;
        default:
            break;
    }
    return div;
}


function deleteDiv() {
    if (currentDivs.length <= 0) return;
    msgUI("确认删除吗？", 2, function (data) {
        if (data == "ok") {
            for (var i = 0; i < currentDivs.length; i++) {
                var currentDiv = currentDivs[i];
                var _parentElement = currentDiv.parentNode;
                if (_parentElement) {
                    _parentElement.removeChild(currentDiv);
                    currentDiv = null;
                }
            }
            currentLefts = [];
            currentBottoms = [];
            currentDivs = [];
        }
    })
}


function saveTempPosition(isNew) {
    var divs = $(".textLayer")[0].childNodes;
    var posData = { GroupList: [], BorderConfig: [] };
    for (var i = 0; i < divs.length; i++) {
        var div = divs[i];
        if (div.dataset.isMain == "true") {
            var pos = {};
            pos["GroupID"] = div.dataset.groupID;
            pos["GroupName"] = div.dataset.groupName;
            pos["PositionXs"] = parseFloat(div.dataset.posX);//左下为圆心
            pos["PositionYs"] = parseFloat(div.dataset.posY);
            pos["BlockKey"] = div.dataset.BlockKey;
            pos["Category"] = div.dataset.category;
            posData.GroupList.push(pos);
        }
    }
    for (let key in config) {
        var cs = config[key];
        delete cs["ID"];
        posData.BorderConfig.push(cs);
    }
    if (isNew) {
        var projectinfoID = getQueryString("ProjectInfoID");
        var fileid = getQueryString('FileID');
        var url = "/MvcConfig/UI/Form/PageView?TmplCode=PlotSealTemplate&ProjectInfoID=" + projectinfoID + "&FileID=" + fileid;
        openWindow(url, {
            data: posData, addQueryString: false,
            title: "盖章模板", height: 430, width: 600, onDestroy: function (data) {

            }
        });
    }
    else
        closeWindow(posData);
}