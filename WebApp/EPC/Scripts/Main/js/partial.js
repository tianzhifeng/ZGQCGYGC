$(function () {
    if ($(".rightMain").length > 0)
        $(".rightMain").niceScroll();
    if ($(".mini-fit").length > 0 && $(".mini-fit").niceScroll != undefined)
        $(".mini-fit").niceScroll();

    //rightMain无数据时的显示处理
    $.each($(".rightMain ul"), function (index, item) {
        //没有数据则替换为空数据提醒
        if ($(item).find("li").length == 0) {
            var emptyText = $(item).attr('emptyText');
            if (!emptyText) {
                emptyText = '暂无数据';
            }

            $(item).replaceWith("<div class='emptyData'>" + emptyText + "</div>");
        }
    })

    $.each($(".rightTabContent ul"), function (index, item) {
        //没有数据则替换为空数据提醒
        if ($(item).find("li").length == 0) {
            var emptyText = $(item).attr('emptyText');
            if (!emptyText) {
                emptyText = '暂无数据';
            }
            var display = $(item).css('display');
            $(item).replaceWith("<div class='emptyData' style='display: " + display + "'>" + emptyText + "</div>");
        }
    })

    $.each($(".rightMain table"), function (index, item) {
        //没有数据则替换为空数据提醒
        if ($(item).find("tr").length == 0) {
            var emptyText = $(item).attr('emptyText');
            if (!emptyText) {
                emptyText = '暂无数据';
            }

            $(item).replaceWith("<div class='emptyData'>" + emptyText + "</div>");
        }
    })

    //rightTabContent无数据时的显示处理
    $.each($(".rightTabContent table"), function (index, item) {
        //没有数据则替换为空数据提醒
        if ($(item).find("tr").length == 0) {
            var emptyText = $(item).attr('emptyText');
            if (!emptyText) {
                emptyText = '暂无数据';
            }
            var display = $(item).css('display');
            $(item).replaceWith("<div class='emptyData' style='display: " + display + "'>" + emptyText + "</div>");
        }
    })

    //ulPicGroup无数据时的显示处理
    $.each($(".ulPicGroup ul"), function (index, item) {
        //没有数据则替换为空数据提醒
        if ($(item).find("li").length == 0) {
            var emptyText = $(item).attr('emptyText');
            if (!emptyText) {
                emptyText = '暂无数据';
            }
            //$(item).css('overflow', 'hidden');
            $(item).replaceWith("<div class='emptyPic'>" + emptyText + "</div>");
        }
    })
})

var picGroupHasMove = false;
window.onload = function () {
    var oDiv = document.getElementsByClassName('ulPicGroup')[0];
    if (!oDiv) return;

    var oUl = oDiv.getElementsByTagName('ul')[0];
    if (!oUl) return;
    var lis = oUl.getElementsByTagName('li');

    //计算ulPicGroup 下ul的width;
    var ulWidth = 0
    for (var index = 0; index < lis.length; index++) {
        if (lis[index])
            ulWidth += lis[index].offsetWidth;
    }

    var liPadding = 10;
    oUl.style.width = ulWidth + liPadding + 'px';

    drag(oUl);

    function drag(obj) {

        obj.onmousedown = function (ev) {
            var ev = ev || event;
            picGroupHasMove = false;

            var startPosX = ev.clientX;
            var startLeft = this.offsetLeft;

            if (obj.setCapture) {
                obj.setCapture();
            }

            document.onmousemove = function (ev) {
                var ev = ev || event;

                var containerWidth = obj.parentElement.offsetWidth;
                var endPosX = ev.clientX;
                var moveX = (endPosX - startPosX)
                if (moveX == 0) return;

                //左侧不能脱节
                var leftAfterMove = startLeft + moveX;
                if (leftAfterMove >= 0 || (containerWidth > obj.offsetWidth)) {
                    leftAfterMove = 0;
                }
                    //右侧不能脱节
                else if (leftAfterMove < (containerWidth - obj.offsetWidth)) {
                    leftAfterMove = containerWidth - obj.offsetWidth;
                }
                obj.style.left = leftAfterMove + 'px';
                picGroupHasMove = true;
            }

            document.onmouseup = function () {
                document.onmousemove = document.onmouseup = null;
                //picGroupHasMove = false;
                //释放全局捕获 releaseCapture();
                if (obj.releaseCapture) {
                    obj.releaseCapture();
                }
            }

            return false;

        }

    }
}

//查看图片
function $onViewImages(fieldIDs) {
    if (picGroupHasMove) return;
    openWindow('/Image/ImgView?fieldIDs=' + fieldIDs, {
        title: "查看图片", width: "100%", height: "100%", addQueryString: false, onDestroy: function (data) {

        }
    });

}

function $onViewLog(id) {
    openWindow("/EPC/Cooperation/Main/LogView?TableID=" + id, { title: "操作日志" });
}