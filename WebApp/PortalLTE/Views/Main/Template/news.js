<div id="{|ID|}" class="box box-primary" style="padding-bottom:{|padding-bottom|}px">
    <div class="box-header">
        <h3 class="box-title">{|Title|}</h3>
        <div class="box-tools pull-right">
            <button id="more_{|MoreID|}" type="button" class="btn btn-box-tool" data-widget="ellipsis" onclick="more{|MoreID|}();">
                <i class="fa fa-ellipsis-v"></i>
            </button>
            <button type="button" class="btn btn-box-tool" data-widget="collapse">
                <i class="fa fa-minus"></i>
            </button>
            <button type="button" class="btn btn-box-tool" data-widget="remove" onclick="$('#{|ID|}').data('hidden', 'true');dashboardChanged('{|ID|}');">
                <i class="fa fa-times"></i>
            </button>
        </div>
    </div>
    <div class="box-body" style="overflow:hidden;height:{|Height|}" id="List_{|LinkList|}">
        <ul class="list-group">
            <li class="list-item" v-for="item in list">
                <div style="cursor: pointer;" v-on:click="openWindow(item);">
                    <div class="item-head">
                        <div class="title nowrap item-mark">
                            {{item.Title}}
                        </div>
                        <div class="item-padding">
                            <div class="item-content">
                                {{item.Content}}
                            </div>
                        </div>
                        <div class="date date-mark">
                            {{item.CreateUserName}} {{item.CreateTime | date }}
                    </div>
              </div>
    </div>
</li>
<li class="list-item" v-for="item in list1">
    <div style="cursor: pointer;" v-on:click="openWindow(item);">
        <div class="item-head">
            <div class="title nowrap item-mark">
                {{item.Title}}
            </div>
        </div>
    </div>
</li>
</ul>
</div>
</div>
<script>
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
<script>
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
            if (NoPopupLayer && NoPopupLayer != "true")
                openWindow(fullUrl);
            else
                window.open(fullUrl);      
        }
    }
});


(function () {
    $.ajax({
        url: "/Base/UI/Portal/GetUrl?portalID={|ID|}",
        type: "post",
        dataType: "json",
        success: function (data) {
            if (data.length > 0) {
                List_{|LinkList|}._data.list = [data[0]];
                data.shift()
                List_{|LinkList|}._data.list1 = data;
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            alert(xhr.responseText);
        }
    });
})();

window.onload=function(){
    $.ajax({
        url: "GetPortal?id={|ID|}",
        type: "post",
        cache: false,
        dataType: "json",
        success: function (data) {
            if(data && (data.MoreUrl == undefined || data.MoreUrl == '')){
                $("#more_{|MoreID|}").hide(); 
            }
        },
        error: function (xhr, textStatus, errorThrown) {
        }
    });
};
</script>
