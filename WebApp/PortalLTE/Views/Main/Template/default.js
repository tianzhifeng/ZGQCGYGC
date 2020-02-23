<div id="{|ID|}" class="box box-primary" style="padding-bottom:{|padding-bottom|}px">
    <div class="box-header">
        <h3 class="box-title">
            {|Title|}
        </h3>
        <div class="box-tools pull-right">
            <button id="more_{|MoreID|}" type="button" class="btn btn-box-tool" data-widget="ellipsis" onclick="more{|MoreID|}();">
                <i class="fa fa-ellipsis-v"></i>
            </button>
            <button type="button" class="btn btn-box-tool" data-widget="collapse">
                <i class="fa fa-minus"></i>
            </button>
            <button type="button" class="btn btn-box-tool" v-show="isLock()" data-widget="remove" onclick="$('#{|ID|}').data('hidden', 'true');dashboardChanged('{|ID|}');">
                <i class="fa fa-times"></i>
            </button>
        </div>
    </div>
    <div class="box-body" style="overflow:hidden;height:{|Height|}" id="List_{|LinkList|}">
        <ul class="list-group">
            <li class="list-item list-item-task" v-for="item in list">
                <div style="cursor: pointer;" v-on:click="openWindow(item);">
                    <div v-if="{|IsOldPortal|} || {|IsList|}" class="item-head nicescroll" style="cursor: pointer;">
                        <div class="taskno">
                            {{item.SortIndex}}
                        </div>
                        <div class="tasktitle nowrap">
                            {{item.Title}} <img v-if="isNew(item.CreateTime)" src="/PortalLTE/Images/new.png" />
                        </div>
                        <div class="date">
                            {{item.CreateTime | date }}
                        </div>
                    </div>
                    <div v-else class="item-head" style="cursor: pointer;">
                        <div class="new_time">
                            <div class="new_time_day">{{item.CreateTime | filterDay }}</div>
                            <div class="new_time_year">{{item.CreateTime | filterYearMonth }}</div>
                        </div>
                        <div class="new_task_right">
                            <div class="new_tasktitle nowrap">
                                    {{item.Title}} <img v-if="isNew(item.CreateTime)" src="/PortalLTE/Images/new.png" />
                            </div>
                            <div class="new_date-mark">
                                发布时间:{{item.CreateTime | filterDate }} 发布人:{{item.CreateUserName}} 
                            </div>
                            <div class="new_contenttext">{{item.ContentText}}</div>
                        </div>
                    </div>
                </div>
            </li>
        </ul>
    </div>
</div>
<script>
    var isShowDate = false;
    var List_{|LinkList|} = new Vue({
        el: '#List_{|LinkList|}',
        data: {
            list: [],
            list1: []
        },
        methods: {
            openWindow: function (item) {
                var fullUrl = "{|LinkUrl|}";
                var arr = fullUrl.match(/[^\{]+(?=\})/gm);
                if (arr) {
                    for (var i = 0; i < arr.length; i++) {
                        var re = '/{' + arr[i] + "}/gm";
                        if (item.ID) {
                            fullUrl = fullUrl.replace(eval(re), eval('item.' + arr[i]));
                        }
                    }
                }
                var opts = {
                    url: fullUrl,
                    title: "{|Title|}",
                    width: "80%",
                    height: "80%",
                    showMaxButton: true,
                    ondestroy: function (action) {

                    }
                };
                if (NoPopupLayer && NoPopupLayer != "true")
                    mini.open(opts);
                else
                    window.open(fullUrl);
            },
            isNew: function (input) {
                var date = new Date(input);
                var h = (new Date().getTime() - date.getTime()) / 1000 / 60 / 60;
                if (h < PortalNewsTime)
                    return true;
                return false;
            },
            isLock: function (item) {
                return getCache('isLockPortal') == 'false' || window.location.href.indexOf('/Base/UI/Portal/Portal') > 0;
            }
        },
        filters: {
            filterDate: function (input) {
                var date = new Date(input);
                var year = date.getFullYear();
                var month = date.getMonth() + 1;
                var day = date.getDate();
                month = month < 10 ? "0" + month : month;
                day = day < 10 ? "0" + day : day;
                var hour = date.getHours();
                var minute = date.getMinutes();
                return year + "-" + month + "-" + day + " " + hour + ":" + minute;
            },
            filterDay: function (input) {
                var date = new Date(input);
                var day = date.getDate();
                day = day < 10 ? "0" + day : day;
                return day;
            },
            filterYearMonth: function (input) {
                var date = new Date(input);
                var year = date.getFullYear();
                var month = date.getMonth() + 1;
                month = month < 10 ? "0" + month : month;
                return year + "-" + month;
            }
        }
    });
(function () {
    $.ajax({
        url: "/Base/UI/Portal/GetUrl?portalID={|ID|}",
        type: "post",
        dataType: "json",
        success: function (data) { 
            //增加新版报错分支
            if (data.errcode) {
                var msg = getErrorFromHtml(data.errmsg);
                msgUI(msg, 4);
                return;
            }
            if (data.length > 0) {
                List_{|LinkList|}._data.list = data;
                for(var i = 0;i < data.length;i++){
                    if(data[i].CreateTime != undefined || data[i].CreateTime != null){
                        isShowDate = true;
                    }
                }
            }
           
        },
        error: function (xhr, textStatus, errorThrown) {
 
        }
    });
})();

function more{|MoreID|}() {
    var url = "{|MoreUrl|}";
    var opts = {
        url: url,
        title: "{|Title|}",
        width: "80%",
        height: "80%",
        showMaxButton: true,
        ondestroy: function (action) {

        }
    };
    if (NoPopupLayer && NoPopupLayer != "true")
        mini.open(opts);
    else
        window.open(url);
}

</script>
