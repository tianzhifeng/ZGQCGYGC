//保存初始化节点、线坐标
var nodeTightPos = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var nodeVirTightPos = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var nodePrecedencePos = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var nodePos = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var pathMove = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var pathPos = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var isCalcNode = {
    Set: function (key, value) { delete this[key]; this[key] = value; },
    Get: function (key) { return this[key] },
    Remove: function (key) { delete this[key] }
};
var level = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Remove: function (key) { delete this[key] }
};
var pathType = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] }
};
var circlePos = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var nodeTights = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var nodePrecedences = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var cacheLevel = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var cacheNodePos = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var cachePathType = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};
var overlapNodes = {
    Set: function (key, value) { this[key] = value },
    Get: function (key) { return this[key] },
    Contains: function (key) { return this.Get(key) == null ? false : true },
    Remove: function (key) { delete this[key] }
};


var Y = 32;
var circle_Width = 8;//节点的直径
var circle_y = 0; //节点Y轴位置
var relation = window.parent.relation;
//获取元素绝对位置
function getAbsPosition(element) {
    var abs = { x: 0, y: 0 }

    //如果浏览器兼容此方法
    if (document.documentElement.getBoundingClientRect) {
        //注意，getBoundingClientRect()是jQuery对象的方法
        //如果不用jQuery对象，可以使用else分支。
        if (element) {
            abs.x = parseInt(element.getBoundingClientRect().left - 11);
            abs.y = parseInt(element.getBoundingClientRect().top);
        }
        abs.x += parseInt(//window.screenLeft +
                    document.documentElement.scrollLeft -
                    document.documentElement.clientLeft);
        abs.y += parseInt(//window.screenTop +
                    document.documentElement.scrollTop -
                    document.documentElement.clientTop);

    }

        //如果浏览器不兼容此方法
    else {
        while (element != document.body) {
            abs.x += parseInt(element.offsetLeft);
            abs.y += parseInt(element.offsetTop);
            element = parseInt(element.offsetParent);
        }

        //计算想对位置
        abs.x += parseInt(//window.screenLeft +
               document.body.clientLeft - document.body.scrollLeft);
        abs.y += parseInt(//window.screenTop +
               document.body.clientTop - document.body.scrollTop);

    }

    return abs;
}

function textSize(fontSize, text) { //计算文字的宽度
    var span = document.createElement("span");
    var result = {};
    result.width = span.offsetWidth;
    result.height = span.offsetWidth;
    span.style.visibility = "hidden";
    document.body.appendChild(span);
    if (typeof span.textContent != "undefined")
        span.textContent = text;
    else span.innerText = text;
    result.width = span.offsetWidth - result.width;
    result.height = span.offsetHeight - result.height;
    span.parentNode.removeChild(span);
    return result;
}

function gPathPos(begin, end, type) {

    var bX, eX, fromX, toX;
    for (var i = 0; i < workJson.nodes.length; i++) {
        if (workJson.nodes[i].code == begin) {
            bX = fromX = circlePos.Get('circle' + workJson.nodes[i].id);//getAbsPosition(document.getElementById(gNodePos(workJson.nodes[i].datatime))).x;
            //bX = workJson.nodes[i].code == 1 ? bX - circle_Width : bX + td_width - circle_Width;
        }
        if (workJson.nodes[i].code == end) {
            eX = toX = circlePos.Get('circle' + workJson.nodes[i].id);//getAbsPosition(document.getElementById(gNodePos(workJson.nodes[i].datatime))).x;
            //eX = workJson.nodes[i].code == 1 ? eX + td_width : eX + td_width - circle_Width;
        }
    }
    var fx = bX; //开始路径X坐标
    var fy = nodePos.Get(begin) + circle_Width; //开始路径Y坐标
    var tx = eX - circle_Width * 2; //结束路径X坐标
    var ty = nodePos.Get(end); //结束路径Y坐标

    var line = "";
    if (type == 2) { //直线
        line = 'M' + (fx + circle_Width) + ' ' + (fy - circle_Width) + ' L' + (fx + circle_Width) + ' ' + (fy - circle_Width) + ' L' + (tx < (fx + circle_Width) ? (fx + circle_Width) : tx) + ' ' + ty;
    } else if (type == 1) {//向上折线
        line = 'M' + fx + ' ' + (fy - circle_Width * 2) + ' V' + ty + ' L' + tx + ' ' + ty;
    } else if (type == 4) {//向右往上折线
        line = 'M' + (fx + circle_Width) + ' ' + (fromX == toX ? (fy - circle_Width * 2) : (fy - circle_Width)) + ' L' + (tx + circle_Width * 2) + ' ' + (fromX == toX ? (fy - circle_Width * 2) : (fy - circle_Width)) + ' V' + (ty + circle_Width * 2);
    } else if (type == 5) {//向右往下折线
        line = 'M' + (fx + circle_Width) + ' ' + (fromX == toX ? (fy + circle_Width * 2) : (fy - circle_Width)) + ' L' + (tx + circle_Width * 2) + ' ' + (fromX == toX ? (fy + circle_Width * 2) : (fy - circle_Width)) + ' V' + (ty - circle_Width * 2);
    } else if (type == 6) {//Z线 M200 188 V150 L318 150 V62
        var one = nodePos.Get(begin);
        var two = nodePos.Get(end);
        if (Math.abs(two - one) > Y) {
            line = 'M' + fx + ' ' + (one > two ? (fy - circle_Width * 2) : fy) +
                   ' V' + (one > two ? one - Y : one + Y) +
                   ' L' + (tx + circle_Width * 2) + ' ' + (one > two ? one - Y : one + Y) +
                   ' V' + (one > two ? (two + circle_Width + circle_Width / 2) : two - circle_Width - circle_Width / 2);
        } else if (Math.abs(two - one) == Y) {
            //M 200 188 V 175 L 318 175 V 162
            line = 'M' + fx + ' ' + (one > two ? (fy - circle_Width * 2) : fy) +
                   ' V' + (one > two ? one - Y / 2 : two - Y / 2) +
                   ' L' + (tx + circle_Width * 2) + ' ' + (one > two ? one - Y / 2 : two - Y / 2) +
                   ' V' + (one > two ? (two + circle_Width + circle_Width / 2) : two - circle_Width - circle_Width / 2);
        }
    } else if (type == 7) {//斜线
        line = 'M' + (fx + circle_Width) + ' ' + (fy - circle_Width - 2) + ' L' + (tx + 4) + ' ' + (ty + circle_Width / 2);
    } else {
        line = 'M' + fx + ' ' + (ty < fy ? (fy - circle_Width * 2) : fy) + ' V' + ty + ' L' + tx + ' ' + ty;
    }
    return line;
}


function setJoin(b, e) { return b + "." + e; }

function XYPos(line) {
    var str = $.trim(line).replace(/[A-Z]/gi, "").split(' ');
    if (line.indexOf('M') >= 0 && line.indexOf('L') >= 0 && line.indexOf('V') < 0) //直线
    {
        var a = {}, b = {};
        a = { x: str[0], y: str[1] };
        if (line.split('L').length > 2) {
            b = { x: str[4], y: str[5] };
        } else {
            b = { x: str[2], y: str[3] };
        }
        return { t: 'L', a: a, b: b }
    } else if (line.indexOf('M') >= 0 && line.indexOf('L') >= 0 && line.indexOf('V') >= 0 && line.indexOf('V') == line.lastIndexOf('V')) //折线
    {
        var a = {}, b = {}; var a2 = {}, b2 = {};
        if (line.indexOf('V') < line.indexOf('L')) {
            a = { x: str[0], y: str[1] };
            b = { x: str[0], y: str[2] };
            a2 = { x: str[0], y: str[2] };
            b2 = { x: str[3], y: str[4] };
        } else {
            a = { x: str[0], y: str[1] };
            b = { x: str[2], y: str[3] };
            a2 = { x: str[2], y: str[3] };
            b2 = { x: str[2], y: str[4] };
        }
        return { t: 'V', a: a, b: b, a2: a2, b2: b2 }
    } else {//Z线 M200 188 V150 L318 150 V62
        var a = {}, b = {}; var a2 = {}, b2 = {}; var a3 = {}, b3 = {};
        a = { x: str[0], y: str[1] };
        b = { x: str[0], y: str[2] };
        a2 = { x: str[0], y: str[2] };
        b2 = { x: str[3], y: str[4] };
        a3 = { x: str[3], y: str[4] };
        b3 = { x: str[3], y: str[5] };
        return { t: 'V2', a: a, b: b, a2: a2, b2: b2, a3: a3, b3: b3 }
    }
}

function calcPos(a, b, c, d) {
    a = { x: parseFloat(a.x), y: parseFloat(a.y) },
    b = { x: parseFloat(b.x), y: parseFloat(b.y) },
    c = { x: parseFloat(c.x), y: parseFloat(c.y) },
    d = { x: parseFloat(d.x), y: parseFloat(d.y) };
    /** 1 解线性方程组, 求线段交点. **/
    var denominator = (b.y - a.y) * (d.x - c.x) - (a.x - b.x) * (c.y - d.y);
    //平行或共线  
    if (a.y == c.y && b.y == d.y) {
        if (c.x < b.x && b.x <= d.x || a.x < d.x && d.x <= b.x) {
            return true;
        }
    }

    // 线段所在直线的交点坐标 (x , y)       
    var x = ((b.x - a.x) * (d.x - c.x) * (c.y - a.y)
                + (b.y - a.y) * (d.x - c.x) * a.x
                - (d.y - c.y) * (b.x - a.x) * c.x) / denominator;
    var y = -((b.y - a.y) * (d.y - c.y) * (c.x - a.x)
                + (b.x - a.x) * (d.y - c.y) * a.y
                - (d.x - c.x) * (b.y - a.y) * c.y) / denominator;

    /** 2 判断交点是否在两条线段上 **/
    if (
        // 交点在线段1上   
        ((x - a.x) * (x - b.x) <= 0 && (y - a.y) * (y - b.y) <= 0
        // 且交点也在线段2上   
         && (x - c.x) * (x - d.x) <= 0 && (y - c.y) * (y - d.y) <= 0)
        || //或者十字相交
        (a.x == b.x && a.y < b.y && c.x < a.x && d.x > a.x && a.y < c.y && c.y < b.y
            ||
         c.x == d.x && c.y < d.y && a.x < c.x && b.x > c.x && c.y < a.y && a.y < d.y
        )
        ) {

        // 返回交点p   
        return true
    }
    //否则不相交   
    return false
}


function isAcross(line) {
    /*
    -------------------------------------------------------------------------以下三种是常用的
    M39 88 V50 L97 50   Y轴88比V垂直50高所以是向上折线
    M51 100 L51 100 L97 100  都是100是直线
    M39 112 V150 L136 150   V比112大是向下折线
    -------------------------------------------------------------------------以下是备用
    M39 150  L136 150 V50 向右往上折线
    M39 50  L136 50 V150 向右往下折线
    M253 62 V75 L303 75 V82 Z线只用于虚工作
    -------------------------------------------------------------------------以下是当避免不了交叉时用斜线连接
    M 0 0 L 10 10
    -------------------------------------------------------------------------
    */
    if (line == undefined) return true;
    var pos = XYPos(line);
    if (pos.t == 'L') {
        return findPath(pos.a, pos.b);
    } else if (pos.t == 'V') {
        return findPath(pos.a, pos.b) || findPath(pos.a2, pos.b2);
    } else {
        return findPath(pos.a, pos.b) || findPath(pos.a2, pos.b2) || findPath(pos.a3, pos.b3);
    }

    function findPath(a, b) {
        var isTrue = false;
        for (var i = 0; i < Object.keys(pathPos).length; i++) {
            var node = Object.keys(pathPos)[i];
            if (node.split('.').length > 1) {
                var path = pathPos.Get(node);
                if (path.length > 0) {
                    var p = XYPos(path);
                    if (p.t == 'L') {
                        if (calcPos(a, b, p.a, p.b)) {
                            isTrue = true;
                        }
                    } else {
                        if (calcPos(a, b, p.a, p.b) && calcPos(a, b, p.a2, p.b2)) {
                            isTrue = true;
                        }
                    }
                }
            }
        }


        for (var i = 1; i <= Object.keys(nodePos).length; i++) {
            var x = 0;
            for (var j = 0; j < workJson.nodes.length; j++) {
                x = circlePos.Get('circle' + workJson.nodes[j].id);
                if (i == workJson.nodes[j].code) {
                    var c = { x: x, y: nodePos.Get(i) }, d = { x: x, y: nodePos.Get(i) };
                    if (calcPos(a, b, c, d)) {
                        isTrue = true;
                    }
                }
            }
        }

        return isTrue;
    }

}
var maxLevel = 0;
var l1s = 1, l2s = 2, l3s = 3, l4s = 4, l5s = 5, l6s = 6, l7s = 7;
function saveNode() {
    for (var i = 0; i < relation.length; i++) {
        {
            nodePrecedencePos.Set(relation[i].Number, relation[i].Precedence);
            nodeTightPos.Set(relation[i].Number, relation[i].Tight);
        }
    }
    for (var i = 0; i < workJson.lines.length; i++) {
        var type = workJson.lines[i].type;
        if (type == "dashed" || type == "d&m") {
            var nodeID = workJson.lines[i].from;
            if (nodeVirTightPos.Contains(nodeID)) {
                var bak = nodeVirTightPos.Get(nodeID);
                nodeVirTightPos.Remove(nodeID);
                nodeVirTightPos.Set(nodeID, !bak ? workJson.lines[i].to : bak + ',' + workJson.lines[i].to);
            } else {
                nodeVirTightPos.Set(nodeID, workJson.lines[i].to);
            }
        }
    }

    var level_map = new Array(), node_map = new Array();
    function setNodeY(begin, end, nType, pType) {
        level.Remove(end);
        var l = nType == l1s ? level.Get(begin) : level.Get(begin) + 1;
        level.Set(end, l);
        delete level_map[end];
        level_map[end] = l;
        updateLevel(end);
        if (!isAcross(gPathPos(begin, end, pType)) && !getNodePAndTIsAcross(end)) {
            return true;
        } else {
            restoreNode();
            return false;
        }
    }

    function isExistVirTight(tight) {
        var isExist = false;
        for (var i = 0; i < Object.keys(nodeVirTightPos).length; i++) {
            if (!isNaN(Object.keys(nodeVirTightPos)[i])) {
                if (nodeVirTightPos.Get(Object.keys(nodeVirTightPos)[i]) == tight) {
                    isExist = true;
                }
            }
        }
        return isExist;
    }


    function gPrecedenceWork(nodeID) {
        for (var i = 0; i < relation.length; i++) {
            if (relation[i].Number == nodeID) {
                return relation[i].PrecedenceWork;
            }
        }
    }

    function setPathPos(begin, end, type) {
        var pos = gPathPos(nodes[i].code, end, type);
        if (!isAcross(pos) && pos.indexOf('NaN') < 0) {
            pathPos.Set(setJoin(begin, end), pos);
            isCalcNode.Set(setJoin(begin, type), true);
        }
    }

    function updateLevel(nodeID) {
        for (var i = 0; i < Object.keys(level).length; i++) {
            var key = Object.keys(level)[i];
            var l = level.Get(key);
            if (!isNaN(key) && key != nodeID) { level_map[key] = l }
            if (!isNaN(key) && key != nodeID) {
                if (l >= level.Get(nodeID) && key != 1) {
                    level.Remove(key);
                    level.Set(key, l + 1);
                }
            }
        }
        for (var i = 0; i < Object.keys(nodePos).length; i++) {
            var key = Object.keys(nodePos)[i];
            if (!isNaN(key)) { node_map[key] = nodePos.Get(key) }
            if (!isNaN(key)) {
                nodePos.Remove(key);
                nodePos.Set(key, level.Get(key) * Y);
            }
        }

        for (var i = 0; i < Object.keys(pathPos).length; i++) {
            var path = Object.keys(pathPos)[i].split('.');
            if (!isNaN(path[0])) {
                var pos = gPathPos(path[0], path[1], pathType.Get(Object.keys(pathPos)[i]));
                if (pos && pos.indexOf('NaN') < 0) {
                    pathPos.Set(Object.keys(pathPos)[i], pos);
                }
            }
        }
    }
    function restoreNode() {
        for (var i = 0; i < Object.keys(nodePos).length; i++) {
            var key = Object.keys(nodePos)[i];
            if (!isNaN(key) && node_map[key]) {
                nodePos.Remove(key);
                nodePos.Set(key, node_map[key]);
                level.Remove(key);
                level.Set(key, node_map[key] / Y);
            }
        }
        for (var i = 0; i < Object.keys(pathPos).length; i++) {
            var path = Object.keys(pathPos)[i].split('.');
            if (!isNaN(path[0])) {
                var pos = gPathPos(path[0], path[1], pathType.Get(Object.keys(pathPos)[i]));
                if (pos && pos.indexOf('NaN') < 0) {
                    pathPos.Set(Object.keys(pathPos)[i], pos);
                }
            }
        }
    }
    function getLevel(tig, nodeID) {
        for (var i = 0; i < relation.length; i++) {
            if (relation[i].Number == nodeID) {
                var tight = relation[i].Tight.split(',');
                if (tight.length > 0) {
                    var min = 0, max = level.Get(tight[0]);
                    for (var j = 0; j < tight.length - 1; j++) {
                        var temp = level.Get(tight[j]);
                        if (max < temp) {
                            max = temp;
                        } else {
                            min = temp;
                        }
                    }
                    if ((tig % 2) == 0) { return min != undefined ? min : 1 } else { return max != undefined ? max : 1 };
                }
            }
        }
    }

    function getNodePAndTIsAcross(nodeID) {
        for (var i = 0; i < Object.keys(pathPos).length; i++) {
            var path = Object.keys(pathPos)[i].split('.');
            if (!isNaN(path[1]) && path[1] == nodeID) {
                if (isAcross(pathPos.Get(Object.keys(pathPos)[i]))) {
                    return true;
                }
            }
            if (!isNaN(path[0]) && path[0] == nodeID) {
                if (isAcross(pathPos.Get(Object.keys(pathPos)[i]))) {
                    return true;
                }
            }
        }
    }

    function setPos(begin, end) {
        var realAll = new Array();
        function gMax() {
            var maxNum = 1;
            for (var i = 0; i < realAll.length; i++) {
                var temp = nodeTightPos.Get(realAll[i]).length;
                if (maxNum < temp) {
                    maxNum = temp;
                } else {
                    maxNum = temp;
                }
            }
        }
        function findTight(nodeID) {
            if (realAll.indexOf(nodeID) < 0) {
                realAll.push(nodeID);
            }
            var last = nodeTightPos.Get(nodeID).split(',');
            for (var i = 0; i < last.length; i++) {
                if (last && last[i] != "") {
                    findTight(last[i]);
                }
            }
        }
        function findFirst(nodeID) {
            if (realAll.indexOf(nodeID) < 0) {
                realAll.push(nodeID);
            }
            var first = nodePrecedencePos.Get(nodeID);
            if (first && first != 1 && first != "") {
                findFirst(first);
            }
        }
        findFirst(begin);
        findTight(begin); //alert(gMax());
        level.Remove(begin);
        if (nodeTightPos.Get(begin) == 1) {
            level.Set(begin, level.Get(end));
            updateLevel(begin);
        } else {
            level.Set(begin, level.Get(begin));
        }
        if (!isAcross(gPathPos(begin, end, l6s))) {
            return true;
        } else {
            restoreNode();
            return false;
        }
    }
    var tights = new Array(), nodeLevel = new Array();
    function getNodeTights(tight) {
        for (var i = 0; i < workJson.lines.length; i++) {
            if (workJson.lines[i].from == tight) {
                if (!tights[tight]) {
                    tights[tight] = tight + ',' + workJson.lines[i].to;
                } else {
                    tights[tight] += ',' + workJson.lines[i].to;
                }
            }
        }
    }
    function getAffinity() {
        var arr1 = new Array(), arr3 = new Array(), arr5 = new Array(), index = 2;
        for (var i = 0; i < Object.keys(tights).length; i++) {
            arr1 = tights[Object.keys(tights)[i]].split(',');
            for (var s = 0; s < arr1.length; s++) {
                for (var j = 0; j < Object.keys(tights).length; j++) {
                    if (Object.keys(tights)[i] != Object.keys(tights)[j] && tights[Object.keys(tights)[j]].indexOf(arr1[s]) >= 0) {
                        arr3[Object.keys(tights)[i] + ',' + arr1[s]] = arr1[s];
                    }
                }
            }
        }
        function gCount(n) {
            var c = 0;
            for (var i = 0; i < Object.keys(arr3).length; i++) {
                if (arr3[Object.keys(arr3)[i]] == n) {
                    c = c + 1;
                }
            }
            return c;
        }
        for (var i = 0; i < Object.keys(arr3).length; i++) {
            var num = arr3[Object.keys(arr3)[i]];
            if (arr5.indexOf(num) < 0 && gCount(num) > 1) {
                arr5[num] = gCount(num);
            }
        }
        for (var i = Object.keys(arr5).length - 1; i >= 0; i--) {
            if (arr5[Object.keys(arr5)[i]] > 2) {
                var pre = gPrecedenceWork(Object.keys(arr5)[i]);
                nodeLevel[pre] = 2;
                for (var j = 0; j < Object.keys(arr3).length; j++) {
                    if (arr3[Object.keys(arr3)[j]] == Object.keys(arr5)[i]
                        && Object.keys(arr3)[j].indexOf(pre) < 0) {
                        if (index == 2) {
                            nodeLevel[Object.keys(arr3)[j].split(',')[0]] = 3;
                            index = index + 1;
                        } else if (index == 3) {
                            nodeLevel[Object.keys(arr3)[j].split(',')[0]] = 1;
                            index = index + 1;
                        } else {
                            nodeLevel[Object.keys(arr3)[j].split(',')[0]] = index;
                            index = index + 1;
                        }
                    }
                }
            } else if (arr5[Object.keys(arr5)[i]] == 2) {
                for (var j = 0; j < Object.keys(arr3).length; j++) {
                    if (arr3[Object.keys(arr3)[j]] == Object.keys(arr5)[i]) {
                        nodeLevel[Object.keys(arr3)[j].split(',')[0]] = index;
                        index = index - 1;
                    }
                }
            } else {
                for (var j = 0; j < Object.keys(arr3).length; j++) {
                    if (arr3[Object.keys(arr3)[j]] == Object.keys(arr5)[i]) {
                        nodeLevel[Object.keys(arr3)[j].split(',')[0]] = index;
                        index = index + 1;
                    }
                }
            }
        }
    }

    var nodes = workJson.nodes;
    for (var i = 0; i < nodes.length; i++) {
        var tight = nodeTightPos.Get(nodes[i].code).split(',');

        //如果是节点1则优先处理
        if (nodes[i].code == 1) {
            nodePos.Set(nodes[i].code, tight.length == 1 ? Y : Y * 2);
            level.Set(nodes[i].code, Y * 2 / Y);
            //只有一个紧后
            /*给每个插入后的未确定的范围循环，初始是从0开始*/
            for (var unfixed = 0; unfixed < tight.length; unfixed++) {
                /*设置当前范围的最小值和其索引*/
                var min = 0;
                var minIndex = 0;
                for (var m = 0; m < tight.length; m++) {
                    for (var n = 0; n < nodes.length; n++) {
                        if (nodes[n].code == tight[m] && nodes[n].type == 'main') {
                            min = tight[m];
                            minIndex = m;
                        }
                    }
                }
                /*将最小值插入到unfixed，并且把它所在的原有项替换成*/
                tight.splice(unfixed, 0, min);
                tight.splice(minIndex + 1, 1);
                break;
            }
            if (tight.length == 1) {
                nodePos.Set(tight, Y);
                level.Set(tight, l1s);
                pathPos.Set(setJoin(nodes[i].code, tight), gPathPos(nodes[i].code, tight, l2s));
                pathMove.Set(tight[0], l2s);
                pathType.Set(setJoin(nodes[i].code, tight), l2s);
            } else { //多紧后              
                for (var j = 0; j < tight.length; j++) { getNodeTights(tight[j]) }
                getAffinity();
                for (var j = 0; j < tight.length; j++) {
                    if (nodeLevel[tight[j]]) {
                        level.Set(tight[j], nodeLevel[tight[j]]);
                        nodePos.Set(tight[j], level.Get(tight[j]) * Y);
                        if (!isAcross(gPathPos(nodes[i].code, tight[j], l2s)) && !isCalcNode.Get(setJoin(nodes[i].code, l2s))) {
                            pathPos.Set(setJoin(nodes[i].code, tight[j]), gPathPos(nodes[i].code, tight[j], l2s));
                            isCalcNode.Set(setJoin(nodes[i].code, l2s), true);
                            pathMove.Set(tight[j], l2s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l2s);
                        } else if (!isAcross(gPathPos(nodes[i].code, tight[j], l3s)) && !isCalcNode.Get(setJoin(nodes[i].code, l3s))) {
                            pathPos.Set(setJoin(nodes[i].code, tight[j]), gPathPos(nodes[i].code, tight[j], l3s));
                            isCalcNode.Set(setJoin(nodes[i].code, l3s), true);
                            pathMove.Set(tight[j], l3s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l3s);
                        } else if (!isAcross(gPathPos(nodes[i].code, tight[j], l1s)) && !isCalcNode.Get(setJoin(nodes[i].code, l1s))) {
                            pathPos.Set(setJoin(nodes[i].code, tight[j]), gPathPos(nodes[i].code, tight[j], l1s));
                            isCalcNode.Set(setJoin(nodes[i].code, l1s), true);
                            pathMove.Set(tight[j], l1s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l1s);
                        }
                    }
                }
                for (var j = 0; j < tight.sort().length; j++) {
                    if (!nodeLevel[tight[j]]) {
                        level.Remove(tight[j]);
                        level.Set(tight[j], 1);
                        nodePos.Set(tight[j], level.Get(tight[j]) * Y);
                        updateLevel(tight[j]);
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l1s);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l1s);
                        var pos = gPathPos(nodes[i].code, tight[j], l1s);
                        if (pos.indexOf('NaN') < 0) {
                            pathPos.Set(setJoin(nodes[i].code, tight[j]), pos);
                            isCalcNode.Set(setJoin(nodes[i].code, l1s), true);
                        }
                    }
                }
            }
        } else {
            for (var j = 0; j < tight.length; j++) {
                if (tight[j] != "") { //存在紧后工作
                    if (pathMove.Contains(tight[j])) { //节点已存在
                        var s = pathMove.Get(tight[j]).toString();
                        var one = nodePos.Get(nodes[i].code);
                        var two = nodePos.Get(tight[j]);
                        if (s.indexOf(l2s) < 0 && one == two && !isAcross(gPathPos(nodes[i].code, tight[j], l2s))) {
                            setPathPos(nodes[i].code, tight[j], l2s);
                            pathMove.Remove(tight[j]);
                            pathMove.Set(tight[j], s += ',' + l2s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l2s);
                        } else if (s.indexOf(l3s) < 0 && !isAcross(gPathPos(nodes[i].code, tight[j], l3s))) {
                            setPathPos(nodes[i].code, tight[j], l3s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l3s);
                            pathMove.Remove(tight[j]);
                            pathMove.Set(tight[j], s += ',' + l3s);
                        } else if (s.indexOf(l3s) < 0 && one > two && !isAcross(gPathPos(nodes[i].code, tight[j], l4s))) {
                            setPathPos(nodes[i].code, tight[j], l4s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l4s);
                            pathMove.Remove(tight[j]);
                            pathMove.Set(tight[j], s += ',' + l3s);
                        } else if (s.indexOf(l3s) < 0 && one < two && !isAcross(gPathPos(nodes[i].code, tight[j], l5s))) {
                            setPathPos(nodes[i].code, tight[j], l5s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l5s);
                            pathMove.Remove(tight[j]);
                            pathMove.Set(tight[j], s += ',' + l3s);
                        } else if (s.indexOf(l1s) < 0 && !isAcross(gPathPos(nodes[i].code, tight[j], l1s))) {
                            setPathPos(nodes[i].code, tight[j], l1s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l1s);
                            pathMove.Remove(tight[j]);
                            pathMove.Set(tight[j], s += ',' + l1s);
                        } else if (s.indexOf(l1s) < 0 && one < two && !isAcross(gPathPos(nodes[i].code, tight[j], l5s))) {
                            setPathPos(nodes[i].code, tight[j], l5s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l5s);
                            pathMove.Remove(tight[j]);
                            pathMove.Set(tight[j], s += ',' + l1s);
                        } else if (s.indexOf(l1s) < 0 && one > two && nodePrecedencePos.Get(tight[j]).split(',').length > 1) {
                            setPathPos(nodes[i].code, tight[j], l4s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l4s);
                            pathMove.Remove(tight[j]);
                            pathMove.Set(tight[j], s += ',' + l1s);
                        } else {
                            level.Remove(tight[j]);
                            level.Set(tight[j], getLevel(tight[j], nodes[i].code));
                            updateLevel(tight[j]);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l1s);
                            pathMove.Set(tight[j], !s ? s : s + ',' + l1s);
                            var pos = gPathPos(nodes[i].code, tight[j], l1s);
                            if (pos.indexOf('NaN') < 0) {
                                pathPos.Set(setJoin(nodes[i].code, tight[j]), pos);
                                isCalcNode.Set(setJoin(nodes[i].code, l1s), true);
                            }
                        }
                    } else {
                        nodePos.Set(tight[j], nodePos.Get(gPrecedenceWork(tight[j])));
                        if (!isAcross(gPathPos(nodes[i].code, tight[j], l2s))) {
                            level.Set(tight[j], level.Get(nodes[i].code));
                            pathMove.Set(tight[j], l2s);
                            setPathPos(nodes[i].code, tight[j], l2s);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l2s);
                        } else if (setNodeY(nodes[i].code, tight[j], l3s, l3s)) {
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l3s);
                            pathMove.Set(tight[j], l3s);
                            setPathPos(nodes[i].code, tight[j], l3s);
                        } else if (setNodeY(nodes[i].code, tight[j], l3s, l4s)) {
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l4s);
                            pathMove.Set(tight[j], l3s);
                            setPathPos(nodes[i].code, tight[j], l4s);
                        } else if (setNodeY(nodes[i].code, tight[j], l3s, l5s)) {
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l5s);
                            pathMove.Set(tight[j], l3s);
                            setPathPos(nodes[i].code, tight[j], l5s);
                        } else if (setNodeY(nodes[i].code, tight[j], l1s, l1s)) {
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l1s);
                            pathMove.Set(tight[j], l1s);
                            setPathPos(nodes[i].code, tight[j], l1s);
                        } else if (setNodeY(nodes[i].code, tight[j], l1s, l4s)) {
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l4s);
                            pathMove.Set(tight[j], l1s);
                            setPathPos(nodes[i].code, tight[j], l4s);
                        } else if (setNodeY(nodes[i].code, tight[j], l1s, l5s)) {
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l5s);
                            pathMove.Set(tight[j], l1s);
                            setPathPos(nodes[i].code, tight[j], l5s);
                        } else {
                            level.Remove(tight[j]);
                            level.Set(tight[j], getLevel(tight[j], nodes[i].code));
                            updateLevel(tight[j]);
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l1s);
                            pathMove.Set(tight[j], l1s);
                            var pos = gPathPos(nodes[i].code, tight[j], l1s);
                            if (pos.indexOf('NaN') < 0) {
                                pathPos.Set(setJoin(nodes[i].code, tight[j]), pos);
                                isCalcNode.Set(setJoin(nodes[i].code, l1s), true);
                            }
                        }
                    }
                }

            }
        }
    }
    //虚工作处理
    for (var i = 0; i < nodes.length; i++) {
        if (nodeVirTightPos.Get(nodes[i].code)) {
            var tight = nodeVirTightPos.Get(nodes[i].code).split(',');
            for (var j = 0; j < tight.length; j++) {
                if (pathMove.Get(tight[j]) != undefined) {
                    var s = pathMove.Get(tight[j]).toString();
                    var one = nodePos.Get(nodes[i].code);
                    var two = nodePos.Get(tight[j]);


                    if (pathPos.Contains(nodes[i].code + '.' + tight[j]) || (s.indexOf(l2s) < 0 && one == two && !isAcross(gPathPos(nodes[i].code, tight[j], l2s)))) {
                        setPathPos(nodes[i].code, tight[j], l2s);
                        pathMove.Remove(tight[j]);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l2s);
                        if (!pathPos.Contains(nodes[i].code + '.' + tight[j]))
                            pathType.Set(setJoin(nodes[i].code, tight[j]), l2s);
                    } else if (s.indexOf(l3s) < 0 && !isAcross(gPathPos(nodes[i].code, tight[j], l3s))) {
                        setPathPos(nodes[i].code, tight[j], l3s);
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l3s);
                        pathMove.Remove(tight[j]);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l3s);
                    } else if (s.indexOf(l3s) < 0 && one > two && !isAcross(gPathPos(nodes[i].code, tight[j], l4s))) {
                        setPathPos(nodes[i].code, tight[j], l4s);
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l4s);
                        pathMove.Remove(tight[j]);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l3s);
                    } else if (s.indexOf(l3s) < 0 && one < two && !isAcross(gPathPos(nodes[i].code, tight[j], l5s))) {
                        setPathPos(nodes[i].code, tight[j], l5s);
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l5s);
                        pathMove.Remove(tight[j]);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l3s);
                    } else if (s.indexOf(l1s) < 0 && !isAcross(gPathPos(nodes[i].code, tight[j], l1s))) {
                        setPathPos(nodes[i].code, tight[j], l1s);
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l1s);
                        pathMove.Remove(tight[j]);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l1s);
                    } else if (s.indexOf(l1s) < 0 && one < two && !isAcross(gPathPos(nodes[i].code, tight[j], l5s))) {
                        setPathPos(nodes[i].code, tight[j], l5s);
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l5s);
                        pathMove.Remove(tight[j]);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l1s);
                    } else if (s.indexOf(l1s) < 0 && one > two && !isAcross(gPathPos(nodes[i].code, tight[j], l4s))) {
                        setPathPos(nodes[i].code, tight[j], l4s);
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l4s);
                        pathMove.Remove(tight[j]);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l1s);
                    } else if (setNodeY(nodes[i].code, tight[j], l3s, l3s)) {
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l3s);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l3s);
                        setPathPos(nodes[i].code, tight[j], l3s);
                    } else if (setNodeY(nodes[i].code, tight[j], l3s, l4s)) {
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l4s);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l3s);
                        setPathPos(nodes[i].code, tight[j], l4s);
                    } else if (setNodeY(nodes[i].code, tight[j], l3s, l5s)) {
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l5s);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l3s);
                        setPathPos(nodes[i].code, tight[j], l5s);
                    } else if (setNodeY(nodes[i].code, tight[j], l1s, l1s)) {
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l1s);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l1s);
                        setPathPos(nodes[i].code, tight[j], l1s);
                    } else if (setNodeY(nodes[i].code, tight[j], l1s, l4s)) {
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l4s);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l1s);
                        setPathPos(nodes[i].code, tight[j], l4s);
                    } else if (setNodeY(nodes[i].code, tight[j], l1s, l5s)) {
                        pathType.Set(setJoin(nodes[i].code, tight[j]), l5s);
                        pathMove.Set(tight[j], !s ? s : s + ',' + l1s);
                        setPathPos(nodes[i].code, tight[j], l5s);
                    } else {
                        var pos = gPathPos(nodes[i].code, tight[j], one > two ? l4s : l5s);
                        pathType.Set(setJoin(nodes[i].code, tight[j]), one > two ? l4s : l5s);
                        pathMove.Set(tight[j], !s ? s : s + ',' + one > two ? l4s : l5s);
                        pathPos.Set(setJoin(nodes[i].code, tight[j]), pos);
                        isCalcNode.Set(setJoin(nodes[i].code, tight[j]), true);
                    }
                }
            }
        }
    }
    for (var i = 0; i < nodes.length; i++) {
        var tight = nodeTightPos.Get(nodes[i].code).split(',');
        if (tight.length == 1 && nodes[i].code == 1) {
            nodePos.Set(nodes[i].code, level.Get(tight[0]) * Y);
            level.Set(nodes[i].code, level.Get(tight[0]));
            pathPos.Set(setJoin(nodes[i].code, tight[0]), gPathPos(nodes[i].code, tight[0], l2s));
            pathMove.Set(nodes[i].code, l2s);
            pathType.Set(setJoin(nodes[i].code, tight[0]), l2s);
        } else if (tight.length == 2 && nodes[i].code == 1) {
            pathPos.Set(setJoin(nodes[i].code, tight[1]), gPathPos(nodes[i].code, tight[1], l1s));
        }
        break;
    }

    /**********************************以上初始化算法*************************************/

    /*********************************优化算法************************************/

    var op = getQueryString("optimize");
    //处理紧前
    if (eval(op))
        handlePrecedence();
    function handlePrecedence() {
        var precedenceNodes = {
            Set: function (key, value) { this[key] = value },
            Get: function (key) { return this[key] },
            Contains: function (key) { return this.Get(key) == null ? false : true },
            Remove: function (key) { delete this[key] }
        };
        for (var i = 0; i < Object.keys(level).length; i++) {
            var node = Object.keys(level)[i];
            if (!isNaN(node)) {
                var count = 0;
                var precedences = new Array();
                var preNodes = new Array();
                for (var j = 0; j < Object.keys(pathType).length; j++) {
                    var path = Object.keys(pathType)[j];
                    if (!isNaN(path)) {
                        var sNode = path.split('.')[1];
                        if (node == sNode) {
                            count = count + 1;
                            if (count > 1)
                                preNodes.push(node);
                        }
                    }
                }
                if (preNodes.length > 0) {
                    for (var p = 0; p < preNodes.length; p++) {
                        for (var j = 0; j < Object.keys(pathType).length; j++) {
                            var path = Object.keys(pathType)[j];
                            if (!isNaN(path)) {
                                var sNode = path.split('.');
                                if (preNodes[p] == sNode[1]) {
                                    if (precedences.indexOf(sNode[0]) < 0)
                                        precedences.push(sNode[0]);
                                }
                            }
                        }
                    }
                    precedenceNodes.Set(node, precedences.join(','));
                }
            }
        }

        for (var i = 0; i < Object.keys(precedenceNodes).length; i++) {
            var node = Object.keys(precedenceNodes)[i];
            if (!isNaN(node)) {
                var l = Object.keys(level);
                var paths = precedenceNodes.Get(node).split(',');
                if (paths.length > 0) {
                    var sl = getMaxPrecedenceLevel(node);
                    for (var j = 0; j < paths.length; j++) {
                        var across = searchAcrossNode(setJoin(paths[j], node));
                        if (across.length > 0) {
                            var mainLevel = level.Get(node);
                            if (level.Get(paths[j]) != level.Get(node)) {
                                var precedences = getSameLevelPrecedences(paths[j]);
                                if (mainLevel > level.Get(paths[j])) {
                                    switchLevel(precedences, sl);
                                } else if (mainLevel < level.Get(paths[j])) {
                                    switchLevel(precedences, sl + 1);
                                }
                            }
                        }
                    }
                }
            }
        }
        //查找同级紧前
        function getSameLevelPrecedences(node) {
            function getNode(n) {
                for (var i = 0; i < nodes.length; i++) {
                    if (nodes[i].code == node) {
                        return nodes[i];
                    }
                }
            }
            var precedences = new Array();
            precedences.push(getNode(node));
            for (var i = Object.keys(nodePrecedencePos).length - 1; i >= 0; i--) {
                var precedencNode = Object.keys(nodePrecedencePos)[i];
                if (!isNaN(precedencNode)) {
                    var nl = level.Get(node);
                    if (node == precedencNode) {
                        var nextNode = nodePrecedencePos.Get(precedencNode);
                        if (level.Get(nextNode) == nl) {
                            node = nextNode;
                            precedences.push(getNode(nextNode));
                        }
                    }
                }
            }
            return precedences;
        }

        //查找当前节点紧前的最大层级
        function getMaxPrecedenceLevel(node) {
            var maxLevel = 1;
            for (var i = Object.keys(nodePrecedencePos).length - 1; i >= 0; i--) {
                var precedencNode = Object.keys(nodePrecedencePos)[i];
                if (!isNaN(precedencNode)) {
                    var nl = level.Get(node);
                    if (node == precedencNode) {
                        var nextNode = nodePrecedencePos.Get(precedencNode);
                        for (var j = 0; j < Object.keys(precedenceNodes).length; j++) {
                            var pNode = Object.keys(precedenceNodes)[j];
                            if (!isNaN(pNode)) {
                                if (precedencNode == pNode) {
                                    var l = level.Get(precedencNode);
                                    if (l > maxLevel)
                                        maxLevel = l;

                                    var pNodes = precedenceNodes.Get(pNode).split(',').sort(function (a, b) { return b - a; });
                                    nextNode = pNodes[0];
                                }
                            }
                        }

                        node = nextNode;
                        if (level.Get(nextNode) > maxLevel)
                            maxLevel = level.Get(nextNode);
                    }
                }
            }
            return maxLevel;
        }
    }

    getMaxLevel();
    function getMaxLevel() {
        for (var i = 0; i < Object.keys(level).length; i++) {
            if (!isNaN(Object.keys(level)[i])) {
                var lev = level.Get(Object.keys(level)[i]);
                if (maxLevel < lev) {
                    maxLevel = lev;
                }
            }
        }

        for (var i = 0; i < Object.keys(level).length; i++) {
            var node = Object.keys(level)[i];
            if (!isNaN(node)) {
                cacheLevel.Set(node, level.Get(node));
            }
        }
        for (var i = 0; i < Object.keys(nodePos).length; i++) {
            var node = Object.keys(nodePos)[i];
            if (!isNaN(node)) {
                cacheNodePos.Set(node, nodePos.Get(node));
            }
        }
        for (var i = 0; i < Object.keys(pathType).length; i++) {
            var node = Object.keys(pathType)[i];
            if (!isNaN(node)) {
                cachePathType.Set(node, pathType.Get(node));
            }
        }
    }

    ////优化计算主路径算法
    ////mainOptimize();
    //function mainOptimize() {
    //    //查找所有的主路径节点
    //    var mainNodes = new Array();
    //    for (var i = 0; i < nodes.length; i++) {
    //        if (nodes[i].type == 'main')
    //            mainNodes.push(nodes[i]);
    //    }
    //    var sl = 1;
    //    if (maxLevel >= 3) {
    //        if (maxLevel == 3)
    //            sl = 2;
    //        else
    //            sl = Math.ceil(maxLevel / 2);
    //    }
    //    switchLevel(mainNodes, sl);
    //}

    //切换节点层级时，把之前这个层级及下层级的节点往下移动一层
    function switchLevel(ns, sl) {
        if (ns.length > 0) {
            //查找sl这个层级及下的所有节点进行处理，但排除ns中的节点
            var subnodes = new Array();
            for (var i = 0; i < Object.keys(level).length; i++) {
                if (!isNaN(Object.keys(level)[i])) {
                    var node = i + 1;
                    if (level.Get(node) >= sl) {
                        var isExist = false;
                        for (var j = 0; j < ns.length; j++) {
                            var code = ns[j].code;
                            if (code == node) {
                                isExist = true;
                                break;
                            }
                        }
                        if (!isExist) {
                            subnodes.push(node);
                        }
                    }
                }
            }
            //处理当前ns节点为sl层级
            for (var i = 0; i < ns.length; i++) {
                var node = ns[i].code;
                level.Set(node, sl);
                nodePos.Set(node, sl * Y);
            }
            //处理ns下的层级
            for (var i = 0; i < subnodes.length; i++) {
                var node = subnodes[i];
                var l = level.Get(node) + 1;
                level.Set(node, l);
                nodePos.Set(node, l * Y);
            }

            //更新所有线路
            updatePath();
        }
    }

    function updatePath() {
        for (var i = 0; i < Object.keys(pathPos).length; i++) {
            var node = Object.keys(pathPos)[i];
            if (!isNaN(node)) {
                //清空路径
                pathPos.Set(node, '');
            }
        }
        for (var i = 0; i < Object.keys(pathPos).length; i++) {
            var node = Object.keys(pathPos)[i];
            if (!isNaN(node)) {
                //替换原来的线路
                var path = getUpdatePath(node);
                pathPos.Set(node, path);
            }
        }
    }

    //判断当前路径是否是虚工作
    function isVirWork(path) {
        var isExist = false;
        for (var i = 0; i < Object.keys(nodeVirTightPos).length; i++) {
            var node = Object.keys(nodeVirTightPos)[i];
            if (!isNaN(node)) {
                var value = nodeVirTightPos.Get(node);
                if (value != '') {
                    var tights = value.split(',');
                    for (var j = 0; j < tights.length; j++) {
                        if (path == setJoin(node, tights[j])) {
                            isExist = true;
                            break;
                        }
                    }
                }
            }
        }
        return isExist;
    }

    function getUpdatePath(node) {
        var from = node.split('.')[0];
        var to = node.split('.')[1];
        var line = gPathPos(from, to, l2s); //默认是直线
        /*
            1.如果from与to是同一层级则是直线l2s
            2.如果from比to低(如:form=2,to=5)优先为向下折线l3s、之后是向右往下折线l5s、再后是Z线l6s(只有虚工作)、最后是斜线l7s(只有虚工作)
            3.如果from比to高(如:form=5,to=2)优先为向上折线l1s、之后是向右往上折线l4s、再后是Z线l6s(只有虚工作)、最后是斜线l7s(只有虚工作)
        */
        var type = l2s; //路径的类型
        var fromLevel = level.Get(from);
        var toLevel = level.Get(to);
        if (fromLevel < toLevel) { //处理2.
            if (!isAcross(gPathPos(from, to, l3s)))
                line = gPathPos(from, to, type = l3s);
            else if (!isAcross(gPathPos(from, to, l5s)))
                line = gPathPos(from, to, type = l5s);
            else {
                line = gPathPos(from, to, type = l3s);
            }
        } else if (fromLevel > toLevel) { //处理3.
            isVirWork(node);
            if (!isAcross(gPathPos(from, to, l1s)))
                line = gPathPos(from, to, type = l1s);
            else if (!isAcross(gPathPos(from, to, l4s)))
                line = gPathPos(from, to, type = l4s);
            else {
                line = gPathPos(from, to, type = l1s);
            }
        }
        pathType.Set(node, type);
        return line;
    }

    //获取所有节点的所有紧后节点(不包含主路径)
    function getTights() {
        //递归找当前节点的所有紧后
        var tights = new Array();
        function isMain(nodeID) {
            var isMain = false;
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i].type == 'main' && nodes[i].id == nodeID) {
                    isMain = true;
                    break;
                }
            }
            return isMain;
        }
        function findTight(tightID) {
            if (tights.indexOf(tightID) < 0) {
                if (!isMain(tightID))
                    tights.push(tightID);
            }
            var last = nodeTightPos.Get(tightID).split(',');
            for (var i = 0; i < last.length; i++) {
                if (last && last[i] != "") {
                    findTight(last[i]);
                }
            }
        }
        for (var i = 0; i < Object.keys(level).length; i++) {
            var node = Object.keys(level)[i];
            if (!isNaN(node)) {
                tights = new Array();
                findTight(node);
                nodeTights.Set(node, tights.join(','));
            }
        }
    }

    //查找所有节点的紧前(不包含主路径)
    function getPrecedences() {
        for (var i = 0; i < Object.keys(level).length; i++) {
            var node = Object.keys(level)[i];
            if (!isNaN(node)) {
                var precedences = new Array();
                for (var j = 0; j < Object.keys(nodeTights).length; j++) {
                    var tNode = Object.keys(nodeTights)[j];
                    if (!isNaN(tNode)) {
                        var tValue = nodeTights.Get(tNode);
                        if (typeof (tValue) != 'undefined') {
                            tValue = tValue.split(',');
                            if (tValue.indexOf(node) >= 0) {
                                precedences.push(tNode);
                            }
                        }
                    }
                }
                for (var j = 0; j < Object.keys(nodeVirTightPos).length; j++) {
                    var tNode = Object.keys(nodeTights)[j];
                    if (!isNaN(tNode)) {
                        var tValue = nodeVirTightPos.Get(tNode);
                        if (typeof (tValue) != 'undefined') {
                            tValue = tValue.split(',');
                            if (tValue.indexOf(node) >= 0) {
                                precedences.push(tNode);
                            }
                        }
                    }
                }
                nodePrecedences.Set(node, precedences.join(','));
            }
        }
    }


    //如果是直线,且路径上有文字超出两个节点的宽度或两个节点相交时换成折线显示
    /*for (var i = 0; i < Object.keys(pathPos).length; i++) {
        var node = Object.keys(pathPos)[i];
        if (!isNaN(node)) {
            var from = node.split('.')[0];
            var to = node.split('.')[1];

            var fromLevel = level.Get(from);
            var toLevel = level.Get(to);
            if (fromLevel == toLevel) {
                var fromX = parseInt(circlePos.Get('circle' + from));
                var toX = parseInt(circlePos.Get('circle' + to));
                if (fromX == toX) {
                    //查找这个层级下的所有节点进行处理
                    var subnodes = new Array();
                    subnodes.push(to);
                    for (var i = 0; i < Object.keys(level).length; i++) {
                        if (!isNaN(Object.keys(level)[i])) {
                            var levelNode = i + 1;
                            if (level.Get(levelNode) > toLevel) {
                                subnodes.push(levelNode);
                            }
                        }
                    }

                    //处理ns下的层级
                    for (var i = 0; i < subnodes.length; i++) {
                        var subNode = subnodes[i];
                        var l = level.Get(subNode) + 1;
                        level.Set(subNode, l);
                        nodePos.Set(subNode, l * Y);
                    }

                    //更新所有线路
                    updatePath();
                }
            }
        }
    }*/




    var currentIndex = 1;
    //console.log(new Date().getMinutes() + "-" + new Date().getSeconds().toString());
    if (eval(op)) {
        getTights();
        getPrecedences();
        loop();
    }
    //console.log(new Date().getMinutes() + "-" + new Date().getSeconds().toString());
    //循环交叉的节点找到最优的路径
    function loop() {
        var across = searchAcrossNode();
        for (var i = 0; i < across.length; i++) {
            var node = across[i];
            var splitNode = node.split('.');
            var tights = nodeTights.Get(splitNode[1]);
            var type = pathType.Get(node);
            var isAcross = false; //是否还交叉
            var record = across.length; //记录交叉记录
            var tight = tights.split(',');
            //如果是下面的线,则从最大的层数maxLevel上添加一层递减到1层找出最优的路径
            var curLoopLevel = 0; //当前循环的层级
            if (type == l3s || type == l4s) {
                curLoopLevel = maxLevel + 1;
                if (type == l4s) {
                    var precedences = nodePrecedences.Get(splitNode[0]);
                    if (typeof (precedences) != 'undefined')
                        tight = precedences.split(',');
                }
            } else if (type == l1s || type == l5s) {
                curLoopLevel = 1;
                if (type == l5s) {
                    var precedences = nodePrecedences.Get(splitNode[0]);
                    if (typeof (precedences) != 'undefined')
                        tight = precedences.split(',');
                }
            }

            loopNode();
            function loopNode() {
                if (tight.length > 0) {
                    var otl = level.Get(splitNode[1]); //查找第一个节点的层级
                    if (type == l4s || type == l5s)
                        otl = level.Get(splitNode[0]);
                    var loopNodes = new Array();
                    for (var j = 0; j < tight.length; j++) {
                        //交叉是否同层级
                        var tl = level.Get(tight[j]);
                        if (tl == otl) {
                            for (var n = 0; n < nodes.length; n++) {
                                if (nodes[n].code == tight[j])
                                    loopNodes.push(nodes[n]);
                            }
                        }
                    }
                    if (loopNodes.length > 0) { //相同移动
                        switchLevel(loopNodes, curLoopLevel);
                        across = searchAcrossNode(); //搜索交叉的节点
                        if (type == l3s || type == l4s) {
                            if (record <= across.length && curLoopLevel != 1) {
                                resetWork();
                                curLoopLevel = curLoopLevel - 1;
                                loopNode();
                            }
                        } else if (type == l1s || type == l5s) {
                            if (record <= across.length && curLoopLevel != maxLevel) {
                                resetWork();
                                curLoopLevel = curLoopLevel + 1;
                                loopNode();
                            }
                        }
                    }
                }
            }
        }

        var currentAcross = searchAcrossNode(); //搜索交叉的节点
        if (currentAcross.length > 0 && currentIndex <= currentAcross.length) {
            currentIndex = currentIndex + 1;
            loop();
        }
    }

    function resetWork() {
        for (var i = 0; i < Object.keys(cacheLevel).length; i++) {
            var node = Object.keys(cacheLevel)[i];
            if (!isNaN(node)) {
                level.Set(node, cacheLevel.Get(node));
            }
        }
        for (var i = 0; i < Object.keys(cacheNodePos).length; i++) {
            var node = Object.keys(cacheNodePos)[i];
            if (!isNaN(node)) {
                nodePos.Set(node, cacheNodePos.Get(node));
            }
        }
        for (var i = 0; i < Object.keys(cachePathType).length; i++) {
            var node = Object.keys(cachePathType)[i];
            if (!isNaN(node)) {
                pathType.Set(node, cachePathType.Get(node));
            }
        }
    }

    //搜索交叉的节点
    function searchAcrossNode(aNode) {
        var cacheAcross = new Array();
        //折线是否重叠
        function isOverlap(APos, BPos, type) {
            if (type == l3s)
                return APos.a.x == BPos.a.x && APos.a.x == APos.b.x && BPos.a.x == BPos.b.x;
            else if (type == l4s)
                return APos.a2.x == BPos.a2.x && APos.a2.x == APos.b2.x && BPos.a2.x == BPos.b2.x;
        }
        function aJoin(a, b) {
            return a + ':' + b;
        }
        //查找是否交叉
        function findAcross(pos, n, t) {
            var across = "";
            //查找线路是否相交
            for (var i = 0; i < Object.keys(pathPos).length; i++) {
                var node = Object.keys(pathPos)[i];
                if (node.split('.').length > 1 && n != node && typeof (node) != 'undefined') {
                    var path = pathPos.Get(node);
                    var type = pathType.Get(node);
                    if (path.length > 0) {
                        var p = XYPos(path);
                        //if (node == "32.35" && n == "33.38" || n == "32.35" && node == "33.38")
                        //    debugger
                        if (p.t == 'L' && pos.t == 'L') { //两条都是直线
                            var one = node.split('.');
                            var two = n.split('.');
                            if (calcPos(pos.a, pos.b, p.a, p.b) && (one[0] != two[0] || one[1] != two[1])) {
                                across = aJoin(n, node); break;
                            }
                        } else if (p.t == 'V' && pos.t == 'V') { //两条都是折线
                            if (type == t && type == l3s && !isOverlap(pos, p, l3s)
                                && (calcPos(pos.a2, pos.b2, p.a, p.b) || calcPos(pos.a, pos.b, p.a2, p.b2))) { //两条都是向下折线l3s,向下的直线部分不重叠时
                                across = aJoin(n, node); break;
                            } else if (type == t && type == l4s && !isOverlap(pos, p, l4s)
                                && (calcPos(pos.a, pos.b, p.a2, p.b2) || calcPos(pos.a2, pos.b2, p.a, p.b))) {//两条都是向右往上折线l4s,向上的直线部分不重叠时
                                across = aJoin(n, node); break;
                            } else if (type == t && type == l5s && !isOverlap(pos, p, l4s)
                                && (calcPos(pos.a, pos.b, p.a2, p.b2) || calcPos(pos.a2, pos.b2, p.a, p.b))) {//两条都是向右往下折线l5s,向上的直线部分不重叠时
                                across = aJoin(n, node); break;
                            } else if (type == t && type == l1s && !isOverlap(pos, p, l3s)
                                && (calcPos(pos.a2, pos.b2, p.a, p.b) || calcPos(pos.a, pos.b, p.a2, p.b2))) {//两条都是向上折线l1s,向上的直线部分不重叠时
                                across = aJoin(n, node); break;
                            }
                        } else if (p.t == 'V' && pos.t == 'L') { //传过来是直线,比较的是折线
                            if (calcPos(pos.a, pos.b, p.a, p.b) || calcPos(pos.a, pos.b, p.a2, p.b2)) {
                                across = aJoin(n, node); break;
                            }
                        } else if (p.t == 'L' && pos.t == 'V') { // 传过来是折线,比较的是直线
                            if (calcPos(pos.a, pos.b, p.a, p.b) || calcPos(pos.a2, pos.b2, p.a, p.b)) {
                                across = aJoin(n, node); break;
                            }
                        }
                    }
                }
            }

            //查找是否与节点相交
            for (var i = 0; i < Object.keys(nodePos).length; i++) {
                var node = Object.keys(nodePos)[i];
                if (!isNaN(node)) {
                    var x = circlePos.Get('circle' + node);
                    var y = nodePos.Get(node);
                    if (t == l3s && pos.a.x == x && pos.a.y < y && y < pos.b.y) { //如果是向下折线l3s,如果在向下折线上则相交
                        across = aJoin(n, node); break;
                    } else if (t == l1s && pos.a.x == x && pos.b.y < y && y < pos.a.y) { //如果是向上折线l1s,如果在向上折线上则相交
                        across = aJoin(n, node); break;
                    } /*else if (t == l4s && pos.a2.x == x && pos.b2.y < y && y < pos.a2.y) { //如果是向右往上折线l4s,如果在向右往上折线上则相交
                        across = aJoin(n, node); break;
                    } else if (t == l5s && pos.a2.x == x && pos.a2.y < y && y < pos.b2.y) { //如果是向右往下折线l5s,如果在向右往下折线上则相交
                        across = aJoin(n, node); break;
                    }*/
                }
            }

            return across;
        }

        //查找所有的路径
        var across = new Array();
        if (typeof (aNode) == 'undefined') {
            for (var i = 0; i < Object.keys(pathPos).length; i++) {
                var node = Object.keys(pathPos)[i];
                if (!isNaN(node)) {
                    if (typeof (node) != 'undefined') {
                        var line = pathPos.Get(node);
                        if (line == undefined) return true;
                        var type = pathType.Get(node);
                        var pos = XYPos(line);
                        //if (node == '18.25')
                        //    debugger
                        var result = findAcross(pos, node, type);
                        if (result != '') {
                            cacheAcross.push(result);
                        }
                    }
                }
            }
        } else {
            var line = pathPos.Get(aNode);
            if (line == undefined) return true;
            var type = pathType.Get(aNode);
            var pos = XYPos(line);
            var result = findAcross(pos, aNode, type);
            if (result != '') {
                cacheAcross.push(result);
            }
        }
        var maxRepeat = 1; //最大重复
        function gCount(node) {
            var count = 1;
            for (var i = 0; i < cacheAcross.length; i++) {
                var line = cacheAcross[i].split(':')[1];
                if (line == node)
                    count = count + 1;
            }
            if (count > maxRepeat)
                maxRepeat = count;
            return aJoin(node, count);
        }

        //过滤掉不重复的,优化排序交叉最多的路径
        var filterAcross = new Array();
        for (var i = 0; i < cacheAcross.length; i++) {
            var isExist = false;
            var line1 = cacheAcross[i].split(':')[0];
            for (var j = 0; j < cacheAcross.length; j++) {
                var line2 = cacheAcross[j].split(':')[1];
                if (line1 == line2) {
                    isExist = true;
                    filterAcross.push(gCount(line1));
                    break;
                }
            }
            if (!isExist)
                filterAcross.push(gCount(line1));
        }

        for (var i = maxRepeat; i > 0; i--) {
            for (var j = 0; j < filterAcross.length; j++) {
                var ca = filterAcross[j].split(':');
                if (i == parseInt(ca[1]))
                    across.push(ca[0]);
            }
        }
        return across;
    }

    //处理上下间距
    narrowYPos();
    function narrowYPos() {
        var YPos = new Array();
        //取Y坐标集合
        for (var i = 0; i < Object.keys(level).length; i++) {
            var node = Object.keys(level)[i];
            if (!isNaN(node)) {
                var val = level.Get(node);
                if (YPos.indexOf(val) < 0) {
                    YPos.push(parseInt(val));
                }
            }
        }
        function sortNumber(a, b) {
            return a - b;
        }
        YPos = YPos.sort(sortNumber);
        var pre = 1;
        var gaps = new Array(); //要更新的层级
        for (var i = 0; i < YPos.length; i++) {
            if (YPos[i] > pre) {
                pre = pre + 1;
                if (YPos[i] != pre)
                    gaps.push(setJoin(YPos[i], pre));
            }
        }
        //更新层级
        if (gaps.length > 0) {
            for (var i = 0; i < gaps.length; i++) {
                var oldLevel = gaps[i].split('.')[0];
                var newLevel = parseInt(gaps[i].split('.')[1]);
                for (var j = 0; j < Object.keys(level).length; j++) {
                    var node = Object.keys(level)[j];
                    if (!isNaN(node)) {
                        var val = level.Get(node);
                        if (oldLevel == val) {
                            level.Set(node, newLevel);
                            nodePos.Set(node, newLevel * Y);
                        }
                    }
                }
            }
            //更新所有线路
            updatePath();
        }

        ////查找紧后大于1个节点时的间距
        //for (var i = 0; i < Object.keys(nodeTightPos).length; i++) {
        //    var node = Object.keys(nodeTightPos)[i];
        //    if (!isNaN(node)) {
        //        debugger
        //        var value = nodeTightPos.Get(node);
        //        if (value != '') {
        //            var nodes = value.split(',');
        //            if (nodes.length > 1) {
        //                for (var j = 0; j < nodes.length; j++) {

        //                }
        //            }
        //        }
        //    }
        //}
    }



    //处理拆线重叠时把重叠部分去掉，优先是实线
    removeOverlapLine();
    function removeOverlapLine() {
        var overlapLines = new Array();  //重叠的所有路径集合
        var overlaps = new Array();  //重叠的节点集合
        function isOverlap(node, node2) { //折线是否重叠
            var pos = XYPos(pathPos.Get(node));
            var type = pathType.Get(node);
            var pos2 = XYPos(pathPos.Get(node2));
            var type2 = pathType.Get(node2);
            var n1 = node.split('.');
            var n2 = node2.split('.');
            if (parseFloat(pos.a.x) == parseFloat(pos2.a.x) && n1[0] == n2[0]
                && (parseFloat(pos2.b.y) > parseFloat(pos.b.y) || parseFloat(pos2.b.y) < parseFloat(pos.b.y)) && type == type2 && type == l3s) { //向下折线l3s
                if (overlaps.indexOf(setJoin(0, n2[0])) < 0) overlaps.push(setJoin(0, n2[0]));
                return true;
            } else if (parseFloat(pos.a2.x) == parseFloat(pos2.a2.x) && n1[1] == n2[1]
                && (parseFloat(pos2.a2.y) > parseFloat(pos.a2.y) || parseFloat(pos2.b2.y) < parseFloat(pos.b2.y)) && type == type2 && type == l4s) { //向右往上折线l4s
                if (overlaps.indexOf(setJoin(1, n2[1])) < 0) overlaps.push(setJoin(1, n2[1]));
                return true;
            } else if (parseFloat(pos.a.x) == parseFloat(pos2.a.x) && n1[0] == n2[0]
                && (parseFloat(pos2.b.y) > parseFloat(pos.b.y) || parseFloat(pos2.b.y) < parseFloat(pos.b.y)) && type == type2 && type == l1s) { //向上折线l1s
                if (overlaps.indexOf(setJoin(0, n2[0])) < 0) overlaps.push(setJoin(0, n2[0]));
                return true;
            } else if (parseFloat(pos.a2.x) == parseFloat(pos2.a2.x) && n1[1] == n2[1]
                && (parseFloat(pos2.a2.y) > parseFloat(pos.a2.y) || parseFloat(pos2.a2.y) < parseFloat(pos.a2.y)) && type == type2 && type == l5s) { //向右往下折线l5s
                if (overlaps.indexOf(setJoin(1, n2[1])) < 0) overlaps.push(setJoin(1, n2[1]));
                return true;
            }
            return false;
        }
        for (var i = 0; i < Object.keys(pathPos).length; i++) {
            var node = Object.keys(pathPos)[i];
            if (!isNaN(node)) {
                var overlap = new Array();
                var pos = XYPos(pathPos.Get(node));
                if (pos.t == "V") {
                    for (var j = 0; j < Object.keys(pathPos).length; j++) {
                        var node2 = Object.keys(pathPos)[j];
                        if (!isNaN(node2)) {
                            var pos2 = XYPos(pathPos.Get(node2));
                            if (pos2.t == "V") {
                                if (isOverlap(node, node2)) {
                                    overlap.push(node2);
                                }
                            }
                        }
                    }
                }
                if (overlap.length > 0) {
                    overlap.push(node);
                    overlapLines.push(overlap.join(','));
                }
            }
        }

        var lines = new Array(); //过滤重复
        
        overlapLines = overlapLines.sort(function (a, b) { return b.length - a.length; })
        for (var i = 0; i < overlaps.length; i++) {
            var index = overlaps[i].split('.')[0];
            var value = overlaps[i].split('.')[1];
            var arr = new Array(), arr2 = new Array();
            for (var j = 0; j < overlapLines.length; j++) {
                var split = overlapLines[j].split(',');
                for (var s = 0; s < split.length; s++) {
                    if (split[s].split('.')[index] == value) {
                        //if (split[s] == '69.156')
                        //    debugger
                        if (arr.indexOf(split[s]) < 0) {
                            var type = pathType.Get(split[s]);
                            if (arr.length == 0)
                                arr.push(split[s]);
                            else {
                                if (pathType.Get(arr[0]) == type)
                                    arr.push(split[s]);
                                else
                                    arr2.push(split[s]);
                            }
                        }
                    }
                }
            }
            if (arr.length > 0)
                lines.push(arr.join(','));
            if (arr2.length > 0)
                lines.push(arr2.join(','));
        }

        //优先主路径、再是实线、最后虚线
        for (var i = 0; i < lines.length; i++) {
            var mainLine = ""; //主要路径
            var solidMaxLevel = 1, solidMinLevel = 100000000; //实线
            var dottedMaxLevel = 1, dottedMinLevel = 100000000; //虚线
            var maxLevel = 1, minLevel = 1; //最大层级
            var solids = new Array(), dotteds = new Array();
            var ls = lines[i].split(',');
            var overlapNode = 1;//重叠节点
            if (ls[0].split('.')[0] == ls[1].split('.')[0])
                overlapNode = ls[0].split('.')[0];
            else
                overlapNode = ls[0].split('.')[1];
            //找主路径
            for (var l = 0; l < ls.length; l++) {
                var type = pathType.Get(ls[l]);
                for (var j = 0; j < workJson.lines.length; j++) {
                    var from = workJson.lines[j].from;
                    var to = workJson.lines[j].to;
                    var node = setJoin(from, to);
                    if (node == ls[l]) {
                        if (workJson.lines[j].type == 'main' || workJson.lines[j].type == 'd&m') {
                            mainLine = pathPos.Get(node);
                            break;
                        }
                    }
                }

                //不存在主路径找最大实线或虚线
                var n = ls[l].replace(overlapNode, '').replace('.', '');
                if (mainLine == "") {
                    for (var j = 0; j < workJson.lines.length; j++) {
                        var from = workJson.lines[j].from;
                        var to = workJson.lines[j].to;
                        var node = setJoin(from, to);
                        if (node == ls[l]) {
                            if (workJson.lines[j].type == '') {
                                var n = node.replace(overlapNode, '').replace('.', '');
                                if (type == l3s || type == l4s) {
                                    if (solidMaxLevel < level.Get(n)) {
                                        solidMaxLevel = level.Get(n);
                                    }
                                } else {
                                    if (solidMinLevel > level.Get(n)) {
                                        solidMinLevel = level.Get(n);
                                    }
                                }
                                solids.push(node);
                                break;
                            }
                            if (workJson.lines[j].type == 'dashed') {
                                if (type == l3s || type == l4s) {
                                    if (dottedMaxLevel < level.Get(n)) {
                                        dottedMaxLevel = level.Get(n);
                                    }
                                } else {
                                    if (dottedMinLevel > level.Get(n)) {
                                        dottedMinLevel = level.Get(n);
                                    }
                                }
                                dotteds.push(node);
                                break;
                            }
                        }
                    }
                }

                if (type == l3s || type == l4s) {
                    if (maxLevel < level.Get(n)) {
                        maxLevel = level.Get(n);
                    }
                } else {
                    if (minLevel > level.Get(n)) {
                        minLevel = level.Get(n);
                    }
                }
            }
 
            //得到主路径
            if (mainLine == '') {
                if (solids.length > 0) {
                    for (var s = 0; s < solids.length; s++) {
                        var n = solids[s].replace(overlapNode, '').replace('.', '');
                        var solidLevel = level.Get(n);
                        var targetLevel = 1;
                        var type = pathType.Get(solids[s]);
                        if (type == l3s || type == l4s)
                            targetLevel = solidMaxLevel;
                        else
                            targetLevel = solidMinLevel;
                        if (solidLevel == targetLevel) {
                            mainLine = pathPos.Get(solids[s]);
                            break;
                        }
                    }
                } else if (dotteds.length > 0) {
                    for (var s = 0; s < dotteds.length; s++) {
                        var n = dotteds[s].replace(overlapNode, '').replace('.', '');
                        var dottedLevel = level.Get(n);
                        var targetLevel = 1;
                        var type = pathType.Get(dotteds[s]);
                        if (type == l3s || type == l4s)
                            targetLevel = dottedMaxLevel;
                        else
                            targetLevel = dottedMinLevel;
                        if (dottedLevel == targetLevel) {
                            mainLine = pathPos.Get(dotteds[s]);
                            break;
                        }
                    }
                }
            }

            //处理线路
            for (var l = 0; l < ls.length; l++) {
                var path = pathPos.Get(ls[l]);
                if (mainLine != path) {
                    var n = ls[l].replace(overlapNode, '').replace('.', '');
                    var pl = level.Get(n);
                    var type = pathType.Get(ls[l]);
                    var pos = XYPos(path);
                    if (type == l3s || type == l4s) {
                        if (pl < maxLevel) {
                            var finalPath = 'M' + pos.a2.x + ' ' + pos.a2.y + ' L' + pos.b2.x + ' ' + pos.b2.y;
                            if (type == l4s) {
                                finalPath = 'M' + pos.a.x + ' ' + pos.a.y + ' L' + pos.b.x + ' ' + pos.b.y;
                                overlapNodes.Set(ls[l], finalPath);
                            }
                            pathPos.Set(ls[l], finalPath);
                        }
                    } else {
                        if (pl > maxLevel) {
                            var finalPath = 'M' + pos.a.x + ' ' + pos.a.y + ' L' + pos.b.x + ' ' + pos.b.y;
                            if (type == l1s)
                                finalPath = 'M' + pos.a2.x + ' ' + pos.a2.y + ' L' + pos.b2.x + ' ' + pos.b2.y;
                            else
                                overlapNodes.Set(ls[l], finalPath);
                            pathPos.Set(ls[l], finalPath);
                        }
                    }
                } 
                
            }
            
        }
    }
}

