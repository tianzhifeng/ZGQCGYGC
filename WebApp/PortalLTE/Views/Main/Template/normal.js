<section class="nologin-normal" id="List_{|LinkList|}" v-cloak>
    <div class="">
        <ul class="portal-head">
            <li style="float:left; margin-right:40px;cursor: pointer;" v-for="item in list1" :class="{portal_active : active == item.CatalogKey}" v-on:click="loadNormal(item);" >{{item.CatalogName}}</li>
            <li class="portal-head-more" v-on:click="moreNormal();">{{userIsLogin ? '更多...' : '' }}</li>
        </ul>
        <ul class="list-group protal list-hover01" id="{|LinkList|}_ul" onmouseout="current_{|LinkList|}()">
            <li v-for="item in list"  class="list-item list-item-task new_list_item {{item.LastClass}}" :id="item.ID" onmouseover="onMouseOverMenu_{|LinkList|}(this, {|LinkList|})" onmouseout="onMouseOutMenu_{|LinkList|}(this, {|LinkList|})">
                <div style="cursor: pointer;" v-on:click="openWindow(item);">
                    <div class="item-head" style="cursor: pointer;">
                        <div class="new_time">
                            <div class="new_time_day">{{item.CreateTime | filterDay }}</div>
                            <div class="new_time_year">{{item.CreateTime | filterYearMonth }}</div>
                        </div>
                        <div class="new_task_right">
                            <div class="new_tasktitle nowrap" id="title-{|LinkList|}">
                                    {{item.Title}} <img v-if="isNew(item.CreateTime)" src="/PortalLTE/Images/new.png" />
                            </div>
                            <div class="new_date-mark" id="date-mark-{|LinkList|}">
                                <div class="release"><img src="/PortalLTE/Images/index-01time.png">{{item.CreateTime | filterDate }}</div>
                                <div class="read"><img src="/PortalLTE/Images/index-03view.png">阅读:{{item.ReadCount}}</div> 
                            </div>
                            <div class="new_contenttext display nowrap" id="contenttext-{|LinkList|}"><img src="/PortalLTE/Images/index-04text.png">{{item.ContentText}}</div>
                        </div>
                    </div>                
                </div>
            </li>
             <img id="_img_{|LinkList|}" style="display:none;left: 40%;position: relative;top:100px;" src="/PortalLTE/Images/no-msg.png">
         </ul>
		 <div class="normal-dot" id="dot_{|LinkList|}">
             <a href="javascript:;" v-for="item in pageList" onclick="loadNormal{|LinkList|}({{item}})"></a>
             <a class="normal-dot-more" onclick="normalMore()"></a>
         </div>

     </div>
</section>
<script>
    var normal_{|LinkList|}_ID = '{|ID|}', current_{|LinkList|}_ID = '', current_{|LinkList|}_pageIndex = 1;              
    var List_{|LinkList|} = new Vue({
        el: '#List_{|LinkList|}',
        data: {
            list: loadNormal{|LinkList|}(),
            list1: {|TabListData|},
            active: '',
            pageList:[],
            userIsLogin: typeof(userInfo) != "undefined" && userInfo != ''
        },
        methods: {
            openWindow: function (item) {
                var list = this.list1;
                for (var i = 0; i < list.length; i++) {
                    if(normal_{|LinkList|}_ID == list[i].CatalogKey){
                        var url = '/Base/PortalBlock/PublicInformation/PublicView?ID={ID}&FuncType=view&CatalogKey={CatalogKey}&BlockType=Portal&IsPass=true';
                        if(url){
                            url = url.replace('{ID}', item.ID);
                            url = url.replace('{CatalogKey}', list[i].CatalogKey);
                        }
                        var opts = {
                            url: url,
                            title: list[i].CatalogName,
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
                }
            },
            loadNormal: function (item) {
                normal_{|LinkList|}_ID = item.CatalogKey;
                loadNormal{|LinkList|}();
            },
            moreNormal: function () {
                var list = this.list1;
                for (var i = 0; i < list.length; i++) {
                    if(normal_{|LinkList|}_ID == list[i].CatalogKey){
                        var url = '/Base/PortalBlock/PublicInformation/ListView?CatalogKey=' + list[i].CatalogKey + '&BlockType=Portal&IsPass=true';
                        var opts = {
                            url: url,
                            title: list[i].CatalogName,
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
                }
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
        filters:{
            filterDate: function (input) {
                var date = new Date(input);
                var year = date.getFullYear();
                var month = date.getMonth() + 1;
                var day = date.getDate();
                month = month < 10 ? "0" + month : month;
                day = day < 10 ? "0" + day : day;
                var hour = date.getHours();
                var minute = date.getMinutes();
                if(minute < 10)
                    minute = "0" + minute;
                var datetime = new Date();
                
                if(date.toDateString() == datetime.toDateString())
                    return "今天:" + hour + ":" + minute;
                else if(datetime.getDay() - date.getDay() == 1)
                    return "昨天:" + hour + ":" + minute;
                else
                    return year + "-" + month + "-" + day;
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
                return year.toString().substr(2,3) + "-" + month;
            }
        }
    });

function getNormalCount{|LinkList|}(){
    var count = 0;
    if(NoLoginPortalDic)
    {
        for (var i = 0; i < NoLoginPortalDic.length; i++) { 
            if(NoLoginPortalDic[i].Key && NoLoginPortalDic[i].Key.indexOf(normal_{|LinkList|}_ID) >= 0){
                if(NoLoginPortalDic[i].Count)
                    count = NoLoginPortalDic[i].Count;
            }
        }
    }   
    return parseInt(count);
}

function loadNormal{|LinkList|}(pageIndex, pageSize){
            if(!pageIndex)
                pageIndex = 1;
            if(!pageSize)
                pageSize = 5;
            var count = getNormalCount{|LinkList|}();

            $.ajax({
                url: "GetPublicInformCatalog",
                data: { catalogKey: normal_{|LinkList|}_ID, pageIndex: pageIndex, pageSize:pageSize},
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
                        var list = JSON.parse(data);
                        if(list.length < pageSize)
                            count = list.length;
                        if(count == 0 && list.length > 0)
                            count = list[0]["TableCount"];
                    }
                    for (var l = 0; l < list.length; l++) {
                        for (var key in list[l]) {
                            if(key == "Content"){             
                                var img = getContent{|LinkList|}(list[l][key]);
                                list[l][key] = img + list[l]["ContentText"];
                            }else
                                list[l][key] = list[l][key];
                        }
                        if(pageSize == 5 && l == 4)
                            list[l]["LastClass"] = "li-last-child";
                        else
                            list[l]["LastClass"] = "";
                    }
                  
                    List_{|LinkList|}._data.list = list;
                    if(list.length > 0){
                        $("#_img_{|LinkList|}").hide();
                    }else{
                        $("#_img_{|LinkList|}").show();
                    }    
                    
                    List_{|LinkList|}._data.active = normal_{|LinkList|}_ID;

                    if(pageIndex && pageIndex > 1){
                        current_{|LinkList|}_pageIndex = pageIndex;
                    }
                    if(count > pageSize && pageIndex == 1){
                        var pageList = new Array();
                        for (var i = 1; i <= Math.ceil(count / 5); i++) {
                            pageList.push(i);
                        }
                        List_{|LinkList|}._data.pageList = pageList;
                    }
                    var dots = $("#dot_{|LinkList|}").find('a').not('.normal-dot-more');
                    for (var i = 0; i < dots.length; i++) {
                            if(i + 1 == pageIndex){
                                $(dots[i]).addClass("cur"); 
                            }else{
                                $(dots[i]).removeClass("cur");
                            }
                        }                   
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert(xhr.responseText);
                }
            });
}

function getContent{|LinkList|}(content){
    var img = "";
    var newContent= content.replace(/<img [^>]*src=['"]([^'"]+)[^>]*>/gi,function(match,capture){

        //capture,返回每个匹配的字符串
        if(capture != '' && capture.indexOf('http') < 0 && capture.indexOf('KindEditor') >= 0){
            img = "/Base/Scripts/" + capture;
        }
        img = "<img src='"+capture+"' style='width:185px;height:100px;float: right;margin-left: 20px;'/>";
        return capture;
    });
    return img;
}

List_{|LinkList|}.$watch('list',function(val){
    if(List_{|LinkList|}.list.length > 0){
        current_{|LinkList|}_ID  = List_{|LinkList|}.list[0].ID;
    }
    first_{|LinkList|}();
    if(current_{|LinkList|}_pageIndex == 1){
        $("#dot_{|LinkList|}").children(":first").addClass("cur");
    }

});

function current_{|LinkList|}(){
    first_{|LinkList|}();
}

function first_{|LinkList|}(){
    $("#{|LinkList|}_ul").children(":first").addClass("list-over-bg");
}

function reset_{|LinkList|}(){
    $("#{|LinkList|}_ul").children(":first").removeClass("list-over-bg"); 
}

function onMouseOverMenu_{|LinkList|}(event, id){
    current_{|LinkList|}_ID  = event.id;
    reset_{|LinkList|}();
}

function onMouseOutMenu_{|LinkList|}(event, id){
    reset_{|LinkList|}(); 
}

function normalMore(){
    msgUI("请登录后查看更多的信息!");
}
</script>
