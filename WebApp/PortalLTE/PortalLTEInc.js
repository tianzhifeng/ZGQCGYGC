function IsUserNewPortal() {
    return typeof (IsUseNewPortal) != 'undefined' && IsUseNewPortal.toLocaleLowerCase() == 'true';
}
function GetPortalKeys() {
    if (typeof (PortalKeys) != 'undefined') {
        return PortalKeys;
    }
    return "";
}
var skinType = get("skin");
var cssType = "/PortalLTE/portalLTERW.css";
var cssPortal = "/PortalLTE/Scripts/Custom/index2018.css";
if (IsUserNewPortal()) {
    cssType = "/PortalLTE/portalLTE2018BL.css";
    if (skinType == "skin_transparent_yellow") {
        cssType = "/PortalLTE/portalLTE2018BY.css";
        cssPortal = "/PortalLTE/Scripts/Custom/index2018BY.css";
    }
    if ((typeof (userInfo) == 'undefined' || userInfo == '') && typeof (user) == 'undefined') {
        cssPortal = "/PortalLTE/Scripts/Custom/portal.css";
        if (skinType == "skin_transparent_yellow")
            cssPortal = "/PortalLTE/Scripts/Custom/portalBY.css";
    }
}
else {
    if (skinType == "skin-black")
        cssType = "/PortalLTE/portalLTEBW.css";
    else if (skinType == "skin-blue")
        cssType = "/PortalLTE/portalLTEBL.css";
}

if (IsUserNewPortal()) {
    $("<link>")
    .attr({
        rel: "stylesheet",
        type: "text/css",
        href: cssPortal
    })
    .appendTo("head");
}
$("<link>")
.attr({
    rel: "stylesheet",
    type: "text/css",
    href: cssType
})
.appendTo("head");
//解决ie11 f12调试崩溃问题
if (typeof (rm) == "undefined") {
    rm = "";
}

if (navigator.userAgent.indexOf('Trident') > -1 && navigator.userAgent.indexOf("rv:11.0") > -1) {
    rm = Math.random();
}
/*******************************************************缓存*******************************************************/
//读取cookies
function getPageCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))
        return (arr[2]);
    else
        return null;
}

function getCache(key) {
    if (window.localStorage) {
        return localStorage.getItem(key);
    } else {
        return getPageCookie(key);
    }
}

function get(name) {
    return getCache(name);
}

document.write("<link href='/PortalLTE/Scripts/dist/css/font-awesome.min.css?" + rm+ "' rel='stylesheet' type='text/css' />");
document.write("<link href='/PortalLTE/Scripts/dist/css/ionicons.min.css' type='text/css' />");