var SVG_NS = "http://www.w3.org/2000/svg";
var year_month = [], min_month, max_month, td_width = 0;//获取落点年月、最小月份、最大月份、TD的宽度
var workJson = window.parent.workJson;
$(function () {
    if (!isFirefoxAndChrome()) {
        msgUI("目前只支持Firefox与Chrome浏览器!");
        return;
    }
    var svg = $("#svg");
    if (workJson.nodes) {
        max_month = workJson.nodes[0].datatime;
        for (var i = 0; i < workJson.nodes.length; i++) {
            //比较大小   
            var temp = workJson.nodes[i].datatime;
            if (max_month < temp) {
                max_month = temp;
            } else {
                min_month = temp;
            }
            if (year_month.indexOf(temp.substring(0, 7)) < 0) {
                year_month.push(temp.substring(0, 7));
            }
        }

        var gap = getQueryString("gap");
        var dateType = getQueryString("dateType");
        var multiple = getQueryString("multiple");
        if (multiple == '') multiple = 1;
        ///////////////////////////////////////
        gTimeAll(gap, dateType, multiple); //日期绘制
        sCircleX(); //设置节点X坐标
        saveNode();
        gCircle(svg);//节点绘制
        gPath(svg); //路径绘制
        gLine(svg);//线绘制
        ///////////////////////////////////////
        window.parent.isShowLine();
    }
});


function makeSVG(tag, attributes) {
    var elem = document.createElementNS(SVG_NS, tag);
    for (var attribute in attributes) {
        var name = attribute;
        var value = attributes[attribute];
        if (typeof (value) != 'undefined' && value.toString().indexOf('NaN') < 0)
            elem.setAttribute(name, value);
    }
    return elem;
}

function getDayNumber(time) {
    var date = new Date(time);
    //获取年份 
    var year = date.getFullYear();
    //获取当前月份 
    var mouth = date.getMonth() + 1;
    //定义当月的天数； 
    var days;

    //当月份为二月时，根据闰年还是非闰年判断天数 
    if (mouth == 2) {
        days = year % 4 == 0 ? 29 : 28;
    }
    else if (mouth == 1 || mouth == 3 || mouth == 5 || mouth == 7 || mouth == 8 || mouth == 10 || mouth == 12) {
        //月份为：1,3,5,7,8,10,12 时，为大月.则天数为31； 
        days = 31;
    }
    else {
        //其它月份，天数为：30. 
        days = 30;
    }
    return days;
}


function gTimeAll(gap, dateType, multiple) {
    function gTime(month) { //月日可变间距
        var days = [], range = [], s = 1;
        function isLastNode(d) {
            var ret = false;
            for (var j = 0; j < workJson.nodes.length; j++) {
                var date = new Date(workJson.nodes[j].datatime);
                if (d.getFullYear() == date.getFullYear() && d.getMonth() == date.getMonth() && d.getDate() == date.getDate()) {
                    ret = true;
                }
            }
            return ret;
        }
        for (var i = 1; i <= getDayNumber(month) ; i++) {
            var d = new Date(month + "-" + (i < 10 ? "0" + i : i));
            for (var j = 0; j < workJson.nodes.length; j++) {
                var date = new Date(workJson.nodes[j].datatime);
                if (d.getFullYear() == date.getFullYear() && d.getMonth() == date.getMonth() && days.indexOf(d.getDate()) < 0) {
                    if (d.getDate() == date.getDate() && s == 1) {
                        if (i > 3) {
                            if (days.indexOf("1~" + (i - 1)) < 0) {
                                days.push("1~" + (i - 1));
                                for (var o = 1; o <= i - 1; o++) {
                                    range.push(o);
                                }
                            }
                        } else {
                            for (var k = 1; k < i; k++) {
                                if (days.indexOf(k) < 0)
                                    days.push(k);
                            }
                        }
                        days.push(d.getDate()); s = i;
                    } else if (d.getDate() == date.getDate() && s > 1) {
                        if (i - s > 3) {
                            if (days.indexOf((s + 1) + "~" + (i - 2)) < 0) {
                                days.push((s + 1) + "~" + (i - 2));
                                for (var o = s + 1; o <= i - 2; o++) {
                                    range.push(o);
                                }
                            }
                        } else {
                            if (range.indexOf(i - 2) < 0 && days.indexOf(i - 2) < 0)
                                days.push(i - 2);
                        }
                        if (days.indexOf(i - 1) < 0)
                            days.push(i - 1);
                        days.push(d.getDate()); s = i;
                    }
                    else if (i == getDayNumber(month) && !isLastNode(d)) {
                        if (getDayNumber(month) - s < 3) {
                            for (var k = i; k <= getDayNumber(month) ; k++) {
                                if (days.indexOf(k) < 0)
                                    days.push(k);
                            }
                        } else {
                            if (days.indexOf((s + 1) + "~" + getDayNumber(month)) < 0) {
                                days.push((s + 1) + "~" + getDayNumber(month));
                                for (var o = s + 1; o <= getDayNumber(month) ; o++) {
                                    range.push(o);
                                }
                            }
                        }
                    }
                }

            }
        }
        return days;
    }

    //月日标准间距
    function gTimeAllDay(month) {
        var days = [];
        var fTime = new Date(workJson.nodes[0].datatime);
        var cTime = new Date(month);
        //if (fTime.getMonth() == cTime.getMonth() && fTime.getDate() > 5) {
        //    days.push('...');
        //    for (var i = fTime.getDate() ; i <= getDayNumber(month) ; i++) {
        //        days.push(i);
        //    }
        //} else if (cTime.getMonth() == new Date(max_month).getMonth()) {
        //    for (var i = 1 ; i <= getDayNumber(month) ; i++) {
        //        days.push(i);
        //        if (new Date(workJson.nodes[workJson.nodes.length - 1].datatime).getDate() == i) {
        //            days.push('...');
        //            break;
        //        }
        //    }
        //} else {
        for (var i = 1 ; i <= getDayNumber(month) ; i++) {
            days.push(i);
        }
        //}

        return days;
    }

    function gOmitTime(time) { //月日可变间距日跨度时用....
        if (time.toString().indexOf('~') >= 0) {
            return '...';
        } else {
            return time;
        }
    }

    var ttd = "", btd = "", count = 0, existCount = 0, lastMonth = new Date(year_month[0]); //count表示格子数
    var border_style = "border-right-color: #C0C0C0;border-right-style: solid;border-right-width: 1px;font-size: 12px;";
    if (dateType == window.parent.dateList[2].value) { //年月刻度
        function gYearMonths(year) { //年月可变间距
            var months = [], s = 1;
            for (var i = 0; i < year_month.length; i++) {
                var _year = year_month[i].substring(0, 4);
                var date = new Date(year_month[i]);
                if (_year == year) {
                    if (s == 1) {
                        if (date.getMonth() + 1 > 3) {
                            if (months.indexOf("1~" + (date.getMonth())) < 0) {
                                months.push("1~" + (date.getMonth()));
                            }
                        } else {
                            for (var k = 1; k < date.getMonth() + 1 ; k++) {
                                if (months.indexOf(k) < 0)
                                    months.push(k);
                            }
                        }
                        months.push(date.getMonth() + 1);
                    } else if (s > 1 && year_month[i] != max_month) {
                        if ((date.getMonth() + 1) - (lastMonth.getMonth() + 1) > 3) {
                            if (months.indexOf((lastMonth.getMonth() + 2) + "~" + (lastMonth.getMonth())) < 0) {
                                months.push((lastMonth.getMonth() + 2) + "~" + (lastMonth.getMonth()));
                            }
                        } else {
                            for (var k = lastMonth.getMonth() + 1; k < date.getMonth() + 1; k++) {
                                if (months.indexOf(k) < 0)
                                    months.push(k);
                            }
                        }
                        months.push(date.getMonth() + 1);
                    } else {
                        if (months.indexOf((lastMonth.getMonth() + 1) + "~" + (12 - (lastMonth.getMonth() + 1))) < 0) {
                            months.push((lastMonth.getMonth() + 1) + "~" + (12 - (lastMonth.getMonth() + 1)));
                        }
                    }
                    s += 1;
                    lastMonth = date;
                }
            }
            if (months[months.length - 1] < 10) {
                months.push((months[months.length - 1] + 1) + "~12");
            } else {
                for (var k = months[months.length - 1] + 1; k <= 12 ; k++) {
                    months.push(k);
                }
            }
            return months;
        }

        var years = [];
        for (var i = 0; i < year_month.length; i++) {
            var year = year_month[i].substring(0, 4);
            if (years.indexOf(year) <= -1) {
                years.push(year);
            }
        }
        if (gap == window.parent.gapList[0].value) {
            for (var i = 0; i < years.length; i++) {
                ttd += "<td style='" + border_style + "' colspan='12'>" + years[i] + "</td>";
                for (var j = 1; j < 13; j++) {
                    var id = years[i] + "-" + (j < 10 ? "0" + j : j);
                    btd += '<td style="' + border_style + '" id="' + id + '" name="tdName">' + j + '</td>';
                }
            }
            count = years.length * 12;
        } else {
            function gNodeWidth(year, month) {
                var isExist = false;
                for (var i = 0; i < workJson.nodes.length; i++) {
                    var date = new Date(workJson.nodes[i].datatime);
                    if (date.getFullYear() == year && (date.getMonth() + 1) == month) {
                        isExist = true;
                    }
                }
                return isExist;
            }

            for (var i = 0; i < years.length; i++)
                count += gYearMonths(years[i]).length;
            for (var i = 0; i < years.length; i++) {
                var time = gYearMonths(years[i]);
                ttd += "<td style='" + border_style + "' colspan='" + time.length + "'>" + years[i] + "</td>";
                for (var j = 0; j < time.length; j++) {
                    var style = "", text = time[j];
                    if (gap != window.parent.gapList[0].value) {
                        var isExist = gNodeWidth(years[i], time[j]);
                        if (!isExist) {
                            style = 'width:15px;';
                            text = '...';
                        } else {
                            existCount += 1;
                        }
                    }
                    var id = years[i] + "-" + (time[j].toString().length == 1 ? "0" + time[j] : time[j]);
                    btd += '<td id="' + id + '" name="tdName" style="' + border_style + style + '">' + text + '</td>';
                }
            }
        }
    } else if (dateType == window.parent.dateList[1].value) {
        function gAprilTime(month) {
            var days = [];
            days.push("上旬:" + year_month[i] + "-1~9");
            days.push("中旬:" + year_month[i] + "-10~19");
            days.push("下旬:" + year_month[i] + "-20~" + getDayNumber(month));
            return days;
        }
        function gNodeWidth(time) {
            var isExist = false;
            for (var i = 0; i < workJson.nodes.length; i++) {
                var date = new Date(workJson.nodes[i].datatime);
                var key = workJson.nodes[i].datatime.substring(0, 7) + (date.getDate() < 10 ? "-1~9" : date.getDate() >= 10 && date.getDate() < 20 ? "-10~19" : "-20~" + getDayNumber(workJson.nodes[i].datatime.substring(0, 7)))
                if (key == time)
                    isExist = true;
            }
            return isExist;
        }
        for (var i = 0; i < year_month.length; i++)
            count += gAprilTime(year_month[i]).length;

        for (var i = 0; i < year_month.length; i++) {
            var time = gAprilTime(year_month[i]);
            ttd += "<td style='" + border_style + "' colspan='" + time.length + "'>" + year_month[i] + "</td>";
            for (var j = 0; j < time.length; j++) {
                var style = "", text = time[j].split(':')[0];
                if (gap != window.parent.gapList[0].value) {
                    var isExist = gNodeWidth(time[j].split(':')[1]);
                    if (!isExist) {
                        style = 'width:15px;';
                        text = "...";
                    } else {
                        existCount += 1;
                    }
                }
                btd += '<td id="' + time[j].split(':')[1] + '" name="tdName" style="' + border_style + style + '">' + text + '</td>';
            }
        }
    } else {
        for (var i = 0; i < year_month.length; i++) {
            var time = gap == window.parent.gapList[0].value ? gTimeAllDay(year_month[i]) : gTime(year_month[i]);
            count += time.length;
            ttd += "<td style='" + border_style + "' colspan='" + time.length + "'>" + year_month[i] + "</td>";
            for (var j = 0; j < time.length; j++) {
                var id = year_month[i] + "-" + (time[j].toString().length == 1 ? "0" + time[j] : time[j]);
                btd += '<td style="' + border_style + '" id="' + id + '" name="tdName">' + gOmitTime(time[j]) + '</td>';
            }
        }
    }

    var maxWidth = getMaxTitleWidth();

    $("#topTime,#topTime2").html(ttd);
    $("#botTime,#botTime2").html(btd);

    if (dateType == window.parent.dateList[0].value) {
        td_width = parseInt($(document).width() * multiple / count) < 18 ? 18 : parseInt($(document).width() * multiple / count);
        $("#time,#table1,#time2,#table2").css("width", count * td_width);
    } else {
        var constant = dateType == window.parent.dateList[1].value ? 35 : 85;
        var width = parseInt($(document).width() * multiple / count) < constant ? constant : parseInt($(document).width() * multiple / count);
        $("#time,#table1,#time2,#table2").css("width", count * width - 5);
        if (existCount == 0) {
            td_width = (count * width - 5) / count;
        } else {
            td_width = ((count * width - 5) - (count - existCount) * 15) / existCount;
        }
    }

    if (existCount == 0) {
        $("table [name='tdName']").css("width", td_width);
    } else {
        $("table [name='tdName']").each(function (e, i) {
            //if (!$(this).attr("style")) {
                $(this).css("width", td_width);
            //}
        });
    }

    $("#main").css("width", $("#time").width() + 10);
    $("#svg").css("width", $("#time").width() - 2);
    $("#svg,#main").css("min-height", $(document).height() - 130);
    $("#btnSvgDiv").css("width", $("#time").width());
}


function gNodePos(time) {
    var dateType = getQueryString("dateType");
    if (window.parent.dateList[2].value == dateType) {
        return time.substring(0, 7);
    } else if (window.parent.dateList[1].value == dateType) {
        if (new Date(time).getDate() <= 9)
            return time.substring(0, 7) + "-1~9";
        else if (new Date(time).getDate() > 9 && new Date(time).getDate() <= 19)
            return time.substring(0, 7) + "-10~19";
        else
            return time.substring(0, 7) + "-20~" + getDayNumber(time);
    } else {
        return time;
    }
}

//获取节点的X坐标
function sCircleX() {
    for (var i = 0; i < workJson.nodes.length; i++) {
        var x = getAbsPosition(document.getElementById(gNodePos(workJson.nodes[i].datatime))).x + td_width;
        gCircleX(i, x, td_width);
    }
}
function gCircleX(i, x, td_width) {
    var dateType = getQueryString("dateType");
    var isOne = workJson.nodes[i].code == 1;
    if (window.parent.dateList[1].value == dateType) {
        var day = new Date(workJson.nodes[i].datatime).getDate();
        var posX = 0;
        if (day <= 5 || (day >= 10 && day <= 15) || (day >= 20 && day <= 25)) {
            posX = x - td_width;
        } else {
            if (isOne)
                posX = x - td_width + (td_width / 2);
            else
                posX = x - (td_width / 2);
        }
    } else if (window.parent.dateList[2].value == dateType) {
        //td_width += 20;
        var day = new Date(workJson.nodes[i].datatime).getDate();
        var posX = 0, w1_3 = td_width / 3;
        if (day < 10) {
            if (isOne)
                posX = (x - td_width) + w1_3 / 2;
            else
                posX = x - (w1_3 * 2 + w1_3 / 2);
        } else if (day >= 10 && day < 20) {
            if (isOne)
                posX = (x - td_width) + td_width / 2;
            else
                posX = x - td_width / 2;
        } else {
            if (isOne)
                posX = (x - td_width) + w1_3 * 2 + w1_3 / 2;
            else
                posX = x - w1_3 / 2;
        }
    } else {
        posX = isOne ? x - td_width : x;
    }
    var key = "circle" + workJson.nodes[i].id;
    if (circlePos.Contains(key))
        circlePos.Remove(key);
    circlePos.Set(key, posX);
}

function gCircle(svg) {
    var g = makeSVG("g", { id: "nodes", "stroke-width": "1", zIndex: "1" });
    var gt = makeSVG("g", { id: "nodeTexts", "stroke-width": "1", zIndex: "2" });

    for (var i = 0; i < workJson.nodes.length; i++) {
        var y = getAbsPosition(document.getElementById(gNodePos(workJson.nodes[i].datatime))).y;
        var color = workJson.nodes[i].type && workJson.nodes[i].type == "main" ? "red" : "blue"; //节点颜色
        circle_y = nodePos.Get(workJson.nodes[i].code) ? nodePos.Get(workJson.nodes[i].code) : 30; //workJson.nodes[i].y ? workJson.nodes[i].y : 30;
        var tWidth = textSize(circle_Width, workJson.nodes[i].code).width;
        var node = makeSVG("circle", { id: workJson.nodes[i].id, cx: circlePos.Get("circle" + workJson.nodes[i].id), cy: circle_y, r: circle_Width, "stroke-width": "1.1", stroke: color, fill: "none" });
        var text = makeSVG("text", { x: circlePos.Get("circle" + workJson.nodes[i].id) - tWidth + (tWidth < 8 ? 4 : tWidth >= 8 && tWidth <= 15 ? 8 : 13), y: circle_y + circle_Width / 2 - 1, fill: color, opacity: "1", style: "color: #606060; cursor: default; font-size: 8px;font-weight:bold;" });
        if (i == workJson.nodes.length - 1) {
            $("#svgDiv").css("min-height", maxLevel * Y + 30);
            $("#svg").css("min-height", maxLevel * Y + 30);
            var lh = maxLevel * Y + 230;
            $("#main").css("min-height", lh < $(document).height() ? $(document).height() - 20 : lh - 30);
            $("#bottomTime").attr('y', lh < $(document).height() ? $(document).height() - 90 : lh - 100);
        }
        text.textContent = workJson.nodes[i].code;
        g.appendChild(node);
        gt.appendChild(text);
    }
    svg.append(g);
    svg.append(gt);
}

var linePos = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};

function gPath(svg) {
    var g = makeSVG("g", { id: "paths", "stroke-width": 0.6, zIndex: "1", "stroke-opacity": 0.8 });
    var gt = makeSVG("g", { id: "pathTexts", "stroke-width": "1", zIndex: "2" });

    function gColor(i) //获取颜色
    {
        return workJson.lines[i].type && workJson.lines[i].type == "main" || workJson.lines[i].type && workJson.lines[i].type == "d&m" ? "red" : "blue";
    }
    function gText(i, tx, fx, ty) { //获取路径上的文字
        function gX(title) {
            return fx + (tx - fx) / 2 - textSize(tx - fx, title).width / 3;
        }
        var from = workJson.lines[i].from;
        var to = workJson.lines[i].to;
        var name = workJson.lines[i].name, time = workJson.lines[i].time;
        var isOverlap = pathType.Get(setJoin(from, to)) == l2s && level.Get(to) > level.Get(from);
        var titleY = ty - 5;
        if (isOverlap)
            titleY = ty - Y / 2;
        var textTitle = makeSVG("text", { x: gX(name), y: titleY, fill: gColor(i), opacity: "1", style: "color: #606060; cursor: default; font-size: 10px;" });
        textTitle.textContent = name + (isOverlap ? '(' + time + ')' : '');
        gt.appendChild(textTitle);
        if (!isOverlap) {
            var textTime = makeSVG("text", { x: gX(time), y: ty + 13, fill: gColor(i), opacity: "1", style: "color: #606060; cursor: default; font-size: 10px;" });
            textTime.textContent = time;
            gt.appendChild(textTime);
        }
        svg.append(gt);
    }

    function saveLinePos(form, pos) //保存路径的坐标点
    {
        for (var i = 0; i < workJson.nodes.length; i++) {
            if (workJson.nodes[i].id == form) {
                var nodeY = pos - circle_Width / 2;
                if (linePos.Get(gNodePos(workJson.nodes[i].datatime)) && linePos.Get(gNodePos(workJson.nodes[i].datatime)) < pos) {
                }
                else {
                    linePos.Set(gNodePos(workJson.nodes[i].datatime), nodeY < pos ? nodeY : pos);
                }
            }
        }
    }

    function isExistPN(from, to) //是否存在路径和节点
    {
        function gFrom(node) {
            function gFTime() {
                for (var i = 0; i < workJson.nodes.length; i++) {
                    if (workJson.nodes[i].id == from) {
                        return gNodePos(workJson.nodes[i].datatime);
                    }
                }
            }
            for (var i = 0; i < workJson.nodes.length; i++) {
                var x1 = getAbsPosition(document.getElementById(gNodePos(workJson.nodes[i].datatime))).x + td_width / 2;
                var x2 = getAbsPosition(document.getElementById(node.datatime)).x + td_width / 2;
                if (gNodePos(workJson.nodes[i].datatime) == gFTime() && x1 == x2) {
                    return true;
                }
            }
        }

        for (var i = 0; i < workJson.nodes.length; i++) {
            if (workJson.nodes[i].id == to) {

                for (var j = 0; j < workJson.nodes.length; j++) {
                    if (workJson.nodes[i].y == workJson.nodes[j].y && workJson.nodes[j].datatime < gNodePos(workJson.nodes[i].datatime)) {
                        if (gFrom(workJson.nodes[j])) {
                            return true;
                        }
                    }
                }
            }
        }

    }

    function gNodeY(val) {
        return level.Get(val) * Y;
    }

    function isOverlap(node) {
        return typeof(overlapNodes.Get(node)) != 'undefined';
    }
    for (var i = 0; i < workJson.lines.length; i++) {
        var from = workJson.lines[i].from;
        var to = workJson.lines[i].to;
        var fx = circlePos.Get("circle" + from);//getAbsPosition(document.getElementById(workJson.lines[i].from)).x + circle_Width; //开始路径X坐标标
        var tx = circlePos.Get("circle" + to); //getAbsPosition(document.getElementById(workJson.lines[i].to)).x - circle_Width / 2 - 4; //结束路径X坐标
        var ty = gNodeY(to); //结束路径Y坐标
        saveLinePos(from, ty);
        var arrowUrl = "url(#idArrow)";
        if (isOverlap(setJoin(from, to)))
            arrowUrl = '';
        var path = makeSVG("path", { id: "path" + i, d: pathPos.Get(setJoin(from, to)), stroke: gColor(i), style: workJson.lines[i].type && workJson.lines[i].type == "dashed" || workJson.lines[i].type && workJson.lines[i].type == "d&m" ? "stroke-dasharray:3" : "", "stroke-width": 0.9, fill: "none", "marker-end": arrowUrl });
        g.appendChild(path);
        if (workJson.lines[i].type && workJson.lines[i].type == "dashed" || workJson.lines[i].type && workJson.lines[i].type == "d&m") { }
        else {
            gText(i, tx, fx, ty);
        }
    }
    svg.append(g);
}

function gLine(svg) {
    var g = makeSVG("g", { id: "lines", "stroke-width": "2", zIndex: "1" });
    var array = new Array();
    for (var i = 0; i < workJson.nodes.length; i++) {
        var x = circlePos.Get("circle" + workJson.nodes[i].id);
        //var y = linePos.Get(gNodePos(workJson.nodes[i].datatime)) ? linePos.Get(gNodePos(workJson.nodes[i].datatime)) : $("#" + workJson.nodes[i].id).attr("cy") - td_width / 2; //到节点上
        if (array.indexOf(x) < 0) {
            var lh = maxLevel * Y + 130;
            var y = lh < $(document).height() ? $(document).height() - 180 : lh - 90; //全屏
            var line = makeSVG("line", { id: "lineL" + i, x1: x, y1: 0, x2: x, y2: y, "stroke-width": "1", style: "stroke-dasharray:6;stroke:#e0e0e0;" });
            g.appendChild(line);
            array.push(x);
        }
    }
    svg.append(g);
}


function getMaxTitleWidth() {
    if (workJson.lines.length <= 0) return;
    var max = textSize('', workJson.lines[0].name).width;
    for (var i = 0; i < workJson.lines.length; i++) {
        //比较大小   
        var temp = textSize('', workJson.lines[i].name).width;
        if (max < temp) {
            max = temp;
        }
    }
    return max;
}

function isFirefoxAndChrome() {
    var userAgent = navigator.userAgent;
    return userAgent.indexOf("Firefox") > -1 || userAgent.indexOf("Chrome") > -1 && userAgent.indexOf("Safari") > -1;
}

function svgExport(type) {
    // 下载后的图片名
    var filename = (new Date()).getTime();
    var title = $("#title").html();
    if (title != '')
        filename = title;
    var canvas = $('#svg-wrap svg[id="main"]')[0];
    try {
        saveSvgAsPng(canvas, filename + '.png');
    } catch (err) {
    }

}