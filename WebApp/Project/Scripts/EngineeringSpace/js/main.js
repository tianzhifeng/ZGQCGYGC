Scrollbar.initAll();

/////
var iframeMsg;
$(".nav-chose .home span").on("click",function(){
    $(".nav-pills li").eq(0).trigger("click")
})

$(document).on("keyup",function(e){
    if(e.keyCode==13){
        if($("#search input").is(":focus")){
            $("#Index-container").animate({"bottom":"0%"},500,function(){
            });
            $("#Index-container .up-sj").css("right","108px");
            $(".navs div").removeClass("nav-actives");
        }
    }
    $("#Index-container .up-sj").hide();
})
//选择页码后样式
$(".pages-btn-group li a").on("click",function(){

    $(this).addClass("actives").parent().siblings().find("a").removeClass("actives")
});

//回到首页/尾页
function firstLast(obj,num){
    $(obj).on("click",function(){
        $(".chose-page .pages-container li").find("a").removeClass("actives");
        $(".chose-page .pages-container li").eq(num).find("a").addClass("actives");
    })
};
firstLast(".first-page",0);
firstLast(".last-page",Number($(".chose-page .pages-container li").length-1));
//下一页
$(".chose-page .next-page").on("click",function(){
    var $actives=$(".chose-page .pages-container .actives");
    if($actives.parent().index()==$(".chose-page .pages-container li").length-1){
        return;
    }
    $actives.removeClass("actives").parent().next().find("a").addClass("actives");
});
//上一页
$(".chose-page .prev-page").on("click",function(){
    var $actives=$(".chose-page .pages-container .actives");
    if($actives.parent().index()==0){
        return;
    }
    $actives.removeClass("actives").parent().prev().find("a").addClass("actives");
});


///////////////////////////miniui 数据
mini.parse();
var treegrid = mini.get("treegrid");


treegird.on("drawcell",function(e){
    var record = e.record,
        column = e.column,
        field = e.field,
        value = e.value;
    if (field == "Enter") {
        ///在这里控制工程还是项目，如果是工程  事件为moves（）；项目为movepro();
        e.cellHtml = "<image src='./images/arrow.png' class='enter-arrow pull-right' onclick='moves()' style='margin-right:6px'/>"
    }
    Scrollbar.initAll();
});

///////////////////////////////////dom操作
var iframes='<div class="iframe-item"  style="background-image:url(./images/bg.png);background-size: 100% 100%;">\
                                <div class="item-slider">\
                                    <iframe src="./slider.html" frameborder="0"></iframe>\
                                </div>\
                                <div class="item-content"  data-scrollbar>\
                                    <iframe src="./content.html" frameborder="0"></iframe>\
                                </div>\
                            </div>'

var iframespro='<div class="iframe-item"  style="background-image:url(./images/bg.png);background-size: 100% 100%;">\
                                <div class="item-slider">\
                                    <iframe src="./slider-pro.html" frameborder="0"></iframe>\
                                </div>\
                                <div class="item-content"  data-scrollbar>\
                                    <iframe src="./content-pro.html" frameborder="0"></iframe>\
                                </div>\
                            </div>'


function moves(){
    $(".navs .nav-sj").removeClass("nav-actives")
    var $lis=$('<li role="presentation" class="active"><a href="javascript:void(0)">新工程</a><span class="close-iframe glyphicon glyphicon-remove"></span></li>')
    $(".nav-pills").empty().append($lis);
    //清空  ，然后创建
    $("#iframe-container").empty().append(iframes)
    $("#Index-container").animate({"bottom":"100%"},500,function(){
//                        $("#indexOne").css("overflow","auto");
    });
    $(".chose-room").attr("onoff","false");
}

function movespro(){
    $(".navs .nav-sj").removeClass("nav-actives")
    var $lis=$('<li role="presentation" class="active"><a href="javascript:void(0)">新工程</a><span class="close-iframe glyphicon glyphicon-remove"></span></li>')
    $(".nav-pills").empty().append($lis);
    //清空  ，然后创建
    $("#iframe-container").empty().append(iframespro)
    $("#Index-container").animate({"bottom":"100%"},500,function(){
//                        $("#indexOne").css("overflow","auto");
    });

}

$("body").on("click",".close-iframe",function(event){
    $("#iframe-container .iframe-item").eq($(this).parent().index()).remove();
    if(!$(this).parent().hasClass("active")){
        $(this).parent().remove();
        return;
    }
    if($(this).parent().index()==0){
        $(this).parent().next().trigger("click");
        $(this).parent().remove();
    }else{
        $(this).parent().prev().trigger("click");
        $(this).parent().remove();
    }
    var len=$("#navss .nav-pills li").length;

    if(len==0){
        $(".control-img").hide();
    }
    event.stopPropagation();
})

$("body").on("click","#navss .nav-pills li",function(){
    $(this).addClass("active").siblings().removeClass("active");
    $("#iframe-container .iframe-item").hide().eq($(this).index()).show();

});

// change
$(".control-img").on("click",function(){
    for(var i=0;i<$("#iframe-container .iframe-item").length;i++){
        if($("#iframe-container .iframe-item").eq(i).css("display")=="block"){

            $(".control-img").hide();

            $("#iframe-container .iframe-item").eq(i).find(".item-content").animate({
                "padding-left":"259px"
            },300);

            $("#iframe-container .iframe-item").eq(i).find(".item-slider").animate({
                left:"0px"
            },300);
        }
    }
})
// change
$("#search .form-control").on("mouseover",function(){
    $(this).stop();
    $(this).animate({width:"259px"},500)
}).on("mouseout",function(){
    if($(this).is(":focus")){
        return;
    }
    $(this).stop();
    $(this).animate({width:"0px"},500)
}).on("blur",function(){
    $(this).stop();
    $(this).animate({width:"0px"},500)
})



$(".mini-tabs-firstSpace").on("click",function(){
    $("#slider-container").show();
});



//add
$(".chose-room").on("click",function(event){

    if($(this).attr("onoff")=="false"){
        $("#Index-container").animate({"bottom":"0%"},300);
        $(this).attr("onoff","true");
    }else{
       $("#Index-container").animate({"bottom":"100%"},300);
        $(this).attr("onoff","false");
    }
    $("#Index-container .up-sj").show();
    event.stopPropagation()
})



$(".show-prosess").on("click",function(){
    if($(".chose-profess").attr("off")=="false"){
        $(".chose-profess").show();
        $(".chose-profess").attr("off","true");
    }else{
        $(".tab-content-right").css("margin-left","308px");
        $(".tab-content-right").css("left","0px")
        $(".chose-profess").hide();
        $(".chose-profess").attr("off","false")
    }
})


$(".chose-profess ul li").on("click",function(){
    $(".active-pro").text( $(this).text()).css("background-color", $(this).find("div").css("background-color"));
    $(this).siblings().find("div").removeClass("active");
    $(this).find("div").addClass("active");
});


$(".drop-nav").on("click",function(){
    if($("#Index-container").css("bottom")=="0px"){
        return false;
    }
    if($(".drop-nav").attr("onoff")=="true"){
        $(this).css({"background-color":"#2d89d6"});
        $(this).find("span").css({"color":"#fff"});
        $(".url-active").text("当前工程："+$(".nav-pills .active a").text());
        $("#navss").animate({"top":"-2px"});
        $(".drop-nav").attr("onoff","false").find("span").addClass("icon-double-angle-down").removeClass("icon-double-angle-up");
        $("#iframe-container").animate({
            "margin-top":"-42px",
            "height":$("#iframe-container").height()+42+"px"
        })
        //change
        $(".control-img").animate({
            top:"54px"
        })
        //change
    }else{
        $(this).css({"background-color":"transparent"});
        $(this).find("span").css({"color":"black"});
        $(".url-active").text("");
        $("#navss").animate({"top":"41px"});
        $(".drop-nav").attr("onoff","true").find("span").addClass("icon-double-angle-up").removeClass("icon-double-angle-down");
        $("#iframe-container").animate({
            "margin-top":"0px",
            "height":"100%"
        })
        $(".control-img").animate({
            top:"94px"
        })

    }
});
//change这段注释掉
// $(document).on("click",function(){
//     $("#Index-container").animate({"bottom":"100%"},300);
//     $(this).find("div").removeClass("nav-actives");
// })
//change
$(".chose-page,.page-set,#treegrid").on("click",function(e){
    e.stopPropagation();
})
//add
$(".todoListtop ul li").on("click",function(){
    $(this).addClass("active").siblings().removeClass("active");
})
//add
