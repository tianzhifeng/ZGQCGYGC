/**
 * 产品名称：Web.
 * 创建用户： 秦佳.
 * 创建时间： 2015/12/21.
 * 创建原因：
 */
function showCurrentPage(idname){
    //详情页面
    $monitor.isHideLoading=false;
    var paramsData = $monitor.paramsData[idname];
    $monitor.go("current.html", "BusinessDetail?condition="+ idname +"&type=Customer",idname, function (info) {
        $("#"+$monitor.idName).find(".imgCls").addClass("icon-customer");
        if($monitor.idName.indexOf("ActualReceipt")>0||$monitor.idName=="CurrentSurplusContract"||$monitor.idName=="CurrentAccountReceivable"){
            $("#"+idname).find(".model-1 li:eq(2)").hide();
        }
        if ($monitor.idName.indexOf("Contract") > 0 ) {
            $("#" + idname).find(".model-1 li:eq(1)").hide();
        }
        way.set("superName",{"name":"笔"});
        showIcon();
        //添加绑定作用域(注：作用域不能写在页面层，否则repeat加载不出数据)
        $("#"+idname).find(".list-model").attr("way-repeat",idname + ".currentData").hide();
        $("#"+idname).find(".contract-model").attr("way-repeat",idname + ".contractData").hide();
        $("#"+idname).find(".bindParentData").attr("way-scope",idname);

        paramsData.currentData=info;
        way.set(idname,{"indexParams": paramsData,"currentData":info});
        //数据绑定
        setTimeout(function(){
            $("#"+idname).find(".list-model").show();
            $monitor.scroll();
            $monitor.hideLoading();
        },500);
    });
}

//按条件查询
function filterByCondition(type,e){
    $(e).addClass("active");
    $monitor.resetIdName(e);
    var currentEle = $("#"+$monitor.idName);
    currentEle.find(".custom-select span").text($(e).html());
    currentEle.find(".custom-select span").attr("data-id",type);
    $(e).siblings().removeClass("active");
    $monitor.getRawData("BusinessDetail?condition="+ $monitor.idName +"&type="+type,function(info){
        //数据绑定
        if($(e).text()!=="不分组"){
            way.set($monitor.idName,{"currentData": info});
            $("#"+$monitor.idName).find(".list-model").show();
            var currentImg = $("#"+$monitor.idName).find(".imgCls");
            currentImg.removeClass();
            switch ($(e).text()){
                case "按部门":
                    way.set("superName",{"name":"个客户"});
                    currentImg.addClass("imgCls span-change-icon icon-department");break;
                case "按客户":
                    way.set("superName",{"name":"笔"});
                    currentImg.addClass("imgCls span-change-icon icon-customer");break;
                case "按责任人":
                    currentImg.addClass("imgCls span-change-icon icon-master");break;
            }
        }else{
            way.set($monitor.idName,{"contractData": info});
            $("#"+$monitor.idName).find(".contract-model").show();
        }
        $monitor.scroll();
        //---------------
        $monitor.hideLoading();
    });
    $monitor.closefilter();
}

//显示合同明细
function showContractDetail(e){
    var cur=$(e);
    $monitor.resetIdName(e);
    var id=cur.siblings(".rememberCls").text();
    var type=$("#"+$monitor.idName).find(".custom-select span").attr("data-id");
    var repeatItem = cur.siblings(".show-hide-cls").find(".contract-item-cls");
    repeatItem.attr("way-repeat",$monitor.idName+id );
    var newTimestamp = Date.parse(new Date());
    var oldTimestamp=parseInt(repeatItem.attr("date-timestamp"));
    var token =(newTimestamp-oldTimestamp)/(1000*60);
    if(!(token<1)){
        repeatItem.attr("date-timestamp",newTimestamp );
        $monitor.getRawData("BusinessDetail?condition="+ $monitor.idName +"&type="+type+"&id="+id,function(info){
            $(e).hide();
            console.log(info);
            //数据绑定
            way.set($monitor.idName+id,info);
            cur.siblings(".show-hide-cls").show();
            $monitor.scroll();
        });
    }else{
        $(e).hide();
        cur.siblings(".show-hide-cls").show();
        $monitor.scroll();
        //-----------
        $monitor.hideLoading();
    }
   }

//隐藏合同明细
function closeContractDetail(e){
    var cur=$(e).parent();
    cur.hide();
    cur.siblings(".contract-tips-cls").show();
    $monitor.scroll();
}

//显示不同的icon
function showIcon(){
    //选择显示图标
    var currentType = $monitor.idName;
    var currentView = $("#"+$monitor.idName);
    var currentDom;
    if(currentType.indexOf("ActualReceipt")!==-1){
        currentDom = currentView.find(".icon-invoice");
        currentDom.hide();
        currentDom.siblings(".icon-user").hide();
        currentDom.siblings(".icon-signTime").hide();
    }else {
        if(currentType.indexOf("PlanReceipt")!==-1){
            currentView.find(".hide-you").hide();
            currentDom = currentView.find(".icon-lastTime");
            currentDom.hide();
            currentDom.siblings(".icon-invoice").hide();
            currentDom.siblings(".icon-num").hide();
            currentDom.siblings(".icon-signTime").hide();
            currentDom.siblings(".icon-user").hide();
        }else {
            if(currentType.indexOf("Contract")!==-1){
                currentView.find(".hide-you").hide();
                currentDom = currentView.find(".icon-invoice");
                currentDom.hide();
                currentDom.siblings(".icon-lastTime").hide();
                currentDom.siblings(".icon-num").hide();
                currentDom.siblings(".icon-user").hide();
            }else{
                currentView.find(".hide-you").hide();
                currentDom = currentView.find(".icon-signTime");
                currentDom.hide();
                currentDom.siblings(".icon-lastTime").hide();
                currentDom.siblings(".icon-num").hide();
                currentDom.siblings(".icon-user").hide();
            }
        }
    }

}

//显示tips信息
function showYou(e){
    $monitor.alert(e.innerHTML);
    /*$monitor.clickTips($monitor.idName,e,"就是我哟");*/
}
