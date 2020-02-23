<section class="nologin-section" id="List_{|LinkList|}">
    <div class="index-image">
        <div class="nologin-image-detail">
            <div class="nologin-image-detail-i main-i-active" v-for="item in curItems" id="{{item.ID}}" onclick="openImage_{|LinkList|}(this)">
                <img class="nologin-image" :src="getImageUrl(item)" style="height:350px;width:716px;"/>
            </div>
            <div class="nologin-detail-title nowrap">{{curTitle}}</div>
            <div class="nologin-detail-count"><span class="lbl1">共</span><span class="lbl2">{{curItems.length}}</span><span class="lbl3">张</span></div>
        </div>
        <div class="nologin-image-div">
                <ul class="list-group list-hover02" style="list-style:none;">
                    <li class="nologin-image-li" v-for="item in list" :onclick="item.Switch">
                        <div class="nologin-image-title" id="list_{{item.ID}}"><span>{{item.Title}}</span></div>
				        <img class="nologin-image" :src="getImageUrl(item)" />
                    </li>
                </ul>
        </div>
    </div>

</section> 
<script>             
    var List_{|LinkList|} = new Vue({
        el: '#List_{|LinkList|}',
        data: {
            list: null,
            curItems:[],
            curTitle: ""
        },
            methods: {
                moreNormal: function () {
                    var url = '/Base/PortalBlock/NewsImage/Gallerys';
                    var opts = {
                        url: url,
                        title: '图片新闻',
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
                },
                getImageUrl: function (item) {
                    if(item.IsMis == 1)
                        return "/Base/PortalBlock/NewsImage/GetPic?ID=" + item.NewsImageID;
                    else
                        return "/PortalLTE/Images/no-news.jpg"
                }
            }
        });

    function openImage_{|LinkList|}(event) {
        var active = $(".nologin-image-detail").find(".main-i-active");
        if(active.length > 0){
            var url = '/base/PortalBlock/NewsImage/Gallery?ID=' +active[0].id;
            var opts = {
                url: url,
                title: '图片新闻',
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


    function filterDate(input) {
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
        return year + "-" + month + "-" + day + " " + hour + ":" + minute;
    }

    function loadNormal{|LinkList|}(){
        setTimeout(function(){
            $.ajax({
                url: "GetImageNews",
                type: "get",
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
                        var width = $(window).width() - 100;
                        //var maxNum = Math.floor(width / 260);
                        //if(list.length >= maxNum)
                        //    list = list.slice(0, maxNum);
                        for (var l = 0; l < list.length; l++) {
                            for (var key in list[l]) {
                                list[l][key] = list[l][key];
                                if(key == "CreateTime"){
                                    list[l][key] = filterDate(list[l][key]);
                                }
                                if(key == "NewsImageIDs"){
                                    if(list[l][key] && list[l][key].split(',').length > 0){
                                        list[l]["NewsImageID"] = list[l][key].split(',')[0];
                                    }
                                }
                                list[l]['Switch'] = "clickImage('" + list[l].ID + "')";
                            }
                            if(l == 0){
                                var images = new Array();
                                var newImages = list[0].NewsImageIDs;
                                if(newImages && newImages != '')
                                    newImages = newImages.split(',');
                                for (var i = 0; i < newImages.length; i++) {
                                    if(newImages[i] != '')
                                        images.push({ ID: list[0].ID, NewsImageID: newImages[i], IsMis: 1});
                                }
                                List_{|LinkList|}.curTitle = list[0].Title;
                                List_{|LinkList|}.curItems = images;
                            }
                        }
                        List_{|LinkList|}._data.list = list;
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert(xhr.responseText);
                }
            });
        }, 1500);
         }
         (function () {
             loadNormal{|LinkList|}('{|ID|}');
         })();


         List_{|LinkList|}.$watch('list',function(val){
             var list = List_{|LinkList|}.list;
             if(list.length > 0){
                 switchImage(list[0].ID, false); 
                 noLoginImage(0);
             }
         });
        
         var index_{|LinkList|}=-1,timer_{|LinkList|};

         function clickImage(id){
             switchImage(id, true); 
             var lis = $(".nologin-image-div .nologin-image-title");
             var $id = "list_" + id;
             if(lis.length > 0){
                 for (var i = 0; i < lis.length; i++) {
                     if(lis[i].id == $id){
                         clearInterval(timer_{|LinkList|});
                         index_{|LinkList|} = 0;
                         noLoginImage(0, id);
                     }
                 }
             }           
         }

    
         function switchImage(id, isClick){
             var lis = $(".nologin-image-div .nologin-image-title");
             var $id = "list_" + id;
             if(lis.length > 0){
                 for (var i = 0; i < lis.length; i++) {
                     if(lis[i].id == $id){
                         var list = List_{|LinkList|}.list;
                         for (var j = 0; j < list.length; j++) {
                             if(list[j].ID == id){
                                 if(isClick){
                                     var images = new Array();
                                     var newImages = list[j].NewsImageIDs;
                                     if(newImages && newImages != '')
                                         newImages = newImages.split(',');
                                     for (var n = 0; n < newImages.length; n++) {
                                         if(newImages[n] != '')
                                             images.push({ ID: list[j].ID, NewsImageID: newImages[n], IsMis: 1});
                                     }
                                     List_{|LinkList|}.curTitle = list[j].Title;
                                     List_{|LinkList|}.curItems = images;
                                 }
                                 break;
                             }
                         }
                         
                         $("#" + lis[i].id).addClass("nologin-image-click-bg");
                         $("#" + lis[i].id + " span").hide();
                     }else{
                         $("#" + lis[i].id).removeClass("nologin-image-click-bg");
                         $("#" + lis[i].id + " span").show();
                     }
                 }
             }       
         }

         function noLoginImage(index, id){      
             timerF(index, id);			
             function timerF (index, id) {
                 var list = List_{|LinkList|}.curItems;
                 if(typeof(index) != 'undefined')
                     index_{|LinkList|} = index;
              
                 timer_{|LinkList|}=setInterval(function(){
                     index_{|LinkList|}++;
                     if (index_{|LinkList|}>=list.length) {
                         index_{|LinkList|}=0;
                     }
                     slide(index_{|LinkList|},id);
                 },3000)
             }
			
             function slide (index, id) {
                 if(index >= 0){
                     var list = $(".index-image .nologin-image-detail-i").eq(index);
                     if(!id)
                         id = list.attr("id");
                     if(id){
                         index_{|LinkList|} = index;
                     }
                     list.addClass("main-i-active");
                     list.siblings().removeClass("main-i-active");
                 }
             }
			
             //鼠标移到图片上禁止滚动，鼠标移开开始滚动
             $(".nologin-image-detail-i").hover(function () {
                 clearInterval(timer_{|LinkList|});
             },function () {
                 //timerF(index_{|LinkList|});
             })
  
         }
</script>
