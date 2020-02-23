Scrollbar.initAll();
var iframes='<div class="iframe-item" style="background-image:url(./images/bg.png);background-size: 100% 100%;">\
                        <div class="item-slider">\
                            <iframe src="./slider.html" frameborder="0"></iframe>\
                        </div>\
                        <div class="item-content"  data-scrollbar>\
                            <iframe src="./content.html" frameborder="0"></iframe>\
                        </div>\
                    </div>'

$(".tab-content-state .state-contaiber").not(".addstate").on("click",function(event){
    $(".nav-pills li",window.parent.document).removeClass("active");
    var $li=$('<li role="presentation" class="active"><a href="javascript:void(0)">东南大学九龙湖校区工程</a><span class="close-iframe glyphicon glyphicon-remove"></span></li>')
    $("#navss .nav-pills", window.parent.document).append($li);
    $("#iframe-container",window.parent.document).append(iframes);
    var indexs=$(".nav-pills",window.parent.document).find(".active").index();
    $("#iframe-container .iframe-item",window.parent.document).hide().eq(indexs).show();
    if( $("#iframe-container .iframe-item",window.parent.document).length>5){
        $(".nav-pills li",window.parent.document).eq(0).remove();
        $("#iframe-container .iframe-item",window.parent.document).eq(0).remove();
    }
});
$(".tab-content-state .state-content").on("mouseover",function(){
    $(this).find(".shade").stop().animate({top:"0px"})
}).on("mouseout",function(){
    $(this).find(".shade").stop().animate({top:"100%"})
});


