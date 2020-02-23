var $monitor={
	link:"",
	api:"",
	version:"",
	isAnimation:true,
    paramsData:"",
	//时间戳
	timestamp:1000*60,
	//页面ID
	idName:"",
	//版本号
	version:"",
	//是否自动隐藏loading
	isHideLoading:true,
	//临时存储合同搜索历史（刷新会清空）
	contractSearchHistory:[],
    projectSearchHistory:[],
    curIScrollArray:[],
    //后台api地址
	apiEndpoint:"http://113.59.104.129:8053/api/",
//    apiEndpoint:"http://10.10.1.31:8057/api/",
	//页面跳转
	go:function(link,api,id,callback){
		if(id==""||id==undefined){
			id=link.replace(".","");
		}

		var page= $(".page-views").find("li#"+id);
		var timestamp = Date.parse(new Date());
		$monitor.link=link+"?id="+$monitor.getVersion();
		$monitor.api=api;
		$monitor.idName=id;

		if(page.length==0){
			var newnode=$(".page-views").append('<li class="page" id="'+ id +'" date-timestamp="'+ timestamp +'"><div class="page-view"></div></li>').find("li:nth-last-child(1)");
			var nodehtml=newnode.find(".page-view");
			nodehtml.load($monitor.link,function(){
				newnode.addClass("md-show");
				$monitor.addAnimation(newnode[0],callback);
			});
		}else{
			var oldtimestamp=parseInt(page.attr("date-timestamp"));
			var dt=(timestamp-oldtimestamp)/$monitor.timestamp;

			//一分钟内不重新加载数据
			if(dt>=1){
				page.attr("date-timestamp",timestamp); //重置时间戳
				page.find(".page-view").load(link,function(){
					page.addClass("md-show");
					$monitor.addAnimation(page[0],callback);
				});
			}else{
				$monitor.closefilter();
				page.addClass("md-show");
			}
		}
	},
    //加载数据
    getRawData:function(api,bindFunction){
		$monitor.showLoading();
        //发送数据请求
        $.ajax({
            type: "get",
            url: $monitor.apiEndpoint + api,
            data: "",
            dataType: "json",
            success: function (result) {
                //将请求到的数据及上级页面传参通过callback函数绑定到页面
                if(result.status && bindFunction){
                    bindFunction(result.info);
                }else{
                    alert(result.info);
                }
				if($monitor.isHideLoading){
					$monitor.hideLoading();
				}
            },
            error : function() {
				$monitor.hideLoading();
                alert("网络连接异常！");
            }
        });
    },
	//导入数据
	loadData:function(e,callback){
        if($monitor.api != undefined && $monitor.api!=""){
            $monitor.getRawData($monitor.api,callback);
        }
		$monitor.clearAnimation(e);
	},
	//添加动画
	addAnimation:function(e,callback){
/*		$monitor.scroll();*/
		e.addEventListener("webkitTransitionEnd", $monitor.loadData(e,callback),false);
	},
	//清除动画
	clearAnimation:function(e,callback){
		e.removeEventListener("webkitTransitionEnd",$monitor.loadData);
	},
	//显示菜单
	showMenu:function(){
		var t=$(".page-views");
		if(t.hasClass("md-show")){
			t.removeClass("md-show");
		}else{
			t.addClass("md-show");
		}
	},
	//返回
	goback:function(e){
		$(e).closest("li").removeClass("md-show");
	},
	//显示清除按钮
	keyup:function(e){
		$(e).siblings(".glyphicon-remove-sign").show();
	},
	//清除搜索值（返回搜索首页）
	clearInputValue:function(e){
		var t=$(e);
		t.siblings("input").val("");
		t.hide();
		t.closest(".page-view").find(".search-content").show();
	},
	//显示过滤条件
	showFilter:function(e){
		var cur=$(e);
		var id=cur.attr("data-id");
		var filter=$(".filter-content#"+id);
        var idName = $monitor.idName;
		if(cur.hasClass("active")){
			cur.removeClass("active");
			filter.removeClass("md-show");
		}else{
			cur.addClass("active");
			cur.siblings(".custom-select").removeClass("active");
			filter.addClass("md-show");
			filter.siblings(".filter-content").removeClass("md-show");
            if(filter.find('.footer').length<=0){
                filter.click($monitor.closefilter);
            }
		}
	},
	//关闭过滤条件
	closefilter:function(){
		$(".custom-select").removeClass("active");
		$(".filter-content").removeClass("md-show");
	},
	//重新获取页面ID
	resetIdName:function(e){
		$monitor.idName=$(e).closest(".page").attr("id");
	},
	//显示loading
	showLoading:function() {
		$(".loading").addClass("active");
	},
	//隐藏loading
	hideLoading:function(){
		$monitor.isHideLoading=true;
		$(".loading").removeClass("active");
	},
	scroll:function(){
		var cur=$("#"+$monitor.idName).find(".scroll");
		if(cur.html()==undefined){
			cur=$(".scroll");
		}
        var myScroll;
        var length = $monitor.curIScrollArray.length;
        for(var n=0;n<length;n++){
            if($monitor.curIScrollArray[n].idName === $monitor.idName){
                 myScroll = $monitor.curIScrollArray[n].scroll;
                 break;
            }
        }
		cur.each(function(){
            if($(this).find(".iScrollVerticalScrollbar").length!==0){
                myScroll.refresh();
            }else{
                myScroll = new IScroll($(this)[0], {
                    scrollbars: true,
                    click:true,
                    mouseWheel: true,
                    interactiveScrollbars: true,
                    shrinkScrollbars: 'scale',
					fadeScrollbars: true,
					preventDefaultException: { tagName: /^(P|B|H1|H2|DIV|A|INPUT|TEXTAREA|BUTTON|SELECT)$/ }
                });
                $monitor.curIScrollArray.push({"idName":$monitor.idName,"scroll":myScroll});
            }
		})
	},
	loadFile:function(re){
		var version=$monitor.getVersion();
		for(var i=0;i<re.css.length;i++){
	        var oCss = document.createElement("link"); 
	        oCss.setAttribute("rel", "stylesheet"); 
	        oCss.setAttribute("type", "text/css");  
	        oCss.setAttribute("href", re.css[i]+"?id="+version);
	        document.getElementsByTagName("head")[0].appendChild(oCss);//绑定
		}
		for(var i=0;i<re.js.length;i++){
			var oJs = document.createElement('script');        
	        oJs.setAttribute("type","text/javascript");
	        oJs.setAttribute("src", re.js[i]+"?id="+version);
	        document.getElementsByTagName("head")[0].appendChild(oJs);//绑定
	    }
	},
    clickTips : function (name,ele,tips){
        var self = $(ele);
        var sw = self.get(0).switch;
        if( !sw ) {
            sw = true;
            var content = tips;
            var htmlDom = $("<div class='tooltips'>")
                .addClass("yellow")
                .html("<p class='content'></p>"
                    + "<p class='triangle-front'></p>"
                    + "<p class='triangle-back'></p>");
            htmlDom.find("p.content").html( content );
        }
          self.append( htmlDom );
            var left = self.offset().left - htmlDom.outerWidth()/2 + self.outerWidth()/2;
            var top = self.offset().top - htmlDom.outerHeight() - parseInt(htmlDom.find(".triangle-front").css("border-width"));
            htmlDom.css({"left":left,"top":top - 10,"display":"block"});
            htmlDom.stop().animate({ "top" : top ,"opacity" : 1},300, function(){
            setTimeout(function(){
                    htmlDom.stop().animate({"top":top - 10 ,"opacity" : 0},300,function(){
                        htmlDom.remove();
                        sw = false;
                    })
                },300000000)
            });
    },
    alert:function(name){
	    var iframe = document.createElement("IFRAME");
	    iframe.style.display="none";
	    iframe.setAttribute("src", 'data:text/plain,');
	    document.documentElement.appendChild(iframe);
	    window.frames[0].window.alert(name);
	    iframe.parentNode.removeChild(iframe);
	},
	getVersion:function(){
		var version;
		if($monitor.version!=""){
			version=$monitor.version;
		}else{
			version= Date.parse(new Date());
		}
		return version;
	}
};

//防止微信自带界面拖拽冲突
$(document).ready(function(){
	document.body.addEventListener('touchmove' , function(e){
	　　  var e=e||window.event;
	　　  e.preventDefault();
	},{ passive: false });
});
