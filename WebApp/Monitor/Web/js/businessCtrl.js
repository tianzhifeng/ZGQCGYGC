/**
 * 产品名称：MobileApp
 * 创建用户: 秦佳
 * 日期: 2015/12/10 10:38 PM
 * 创建原因: 经营首页控制器
 * 修改原因：
 * 修改时间:
 */
$monitor.getRawData("BusinessIndex",function(info,paramsData){
    $monitor.paramsData = info;
    //通用数据过滤器
    way.registerTransform("transform-integer-data", function(data) {
        return data!==undefined&&data!==null? data.toFixed(2).split('.')[0]:data;
    });
    way.registerTransform("transform-decimal-data", function(data) {
        return data!==undefined&&data!==null? '.'+data.toFixed(2).split('.')[1]:data;
    });

    if(info.Receipt.GrowthRate!=="--"){
        var cur =  $(".page-view").find(".receiptRate");
        if(info.Receipt.GrowthRate >= 0){
            cur.parent().addClass("up-color");
            cur.addClass("span-up-icon icon-up");
        }else{
            cur.parent().addClass(" down-color");
            cur.addClass("span-up-icon icon-down");
        }
        info.Receipt.GrowthRate = info.Receipt.GrowthRate+'%';
    }
    var transform1 = $("#receipt");
  if(info.Receipt.FinishRate>=100){
      transform1.css({"width" :  '100%',"background-color": '#66cc66'});
  }else {
      if(info.Receipt.FinishRate<100 && info.Receipt.FinishRate>=50){
          transform1.css({"width" :  info.Receipt.FinishRate+'%',"background-color": '#ffa128'});
      }else {
          transform1.css({"width" :  info.Receipt.FinishRate+'%',"background-color": '#f85255'});
      }
  }
    info.Receipt.FinishRate = info.Receipt.FinishRate+'%';
    if(info.Contract.GrowthRate!=="--"){
        var cur =  $(".page-view").find(".contractRate");
        if( info.Contract.GrowthRate >= 0){
            cur.parent().addClass("up-color");
            cur.addClass("span-up-icon icon-up");
        }else{
            cur.parent().addClass(" down-color");
            cur.addClass("span-up-icon icon-down");
        }
        info.Contract.GrowthRate =  info.Contract.GrowthRate+'%';
    }
    var transform2 = $("#contract");
    if(info.Contract.FinishRate>=100){
        transform2.css({"width" :  '100%',"background-color": '#66cc66'});
    }else {
        if(info.Contract.FinishRate<100 && info.Contract.FinishRate>=50){
            transform2.css({"width" :  info.Contract.FinishRate+'%',"background-color": '#ffa128'});
        }else {
            transform2.css({"width" :  info.Contract.FinishRate+'%',"background-color": '#f85255'});
        }
    }
    info.Contract.FinishRate =  info.Contract.FinishRate+'%';
     //数据绑定
    way.set("chartArray",[]);
    way.set("indexScope",info);

    way.registerTransform("dateFormatter", function(date) {
        return (date!==undefined)&&(date!==null)? date.split('T')[0]:date;
    });
    way.registerTransform("dateSpaceFormatter", function(date) {
        return (date!==undefined)&&(date!==null)? date.split(' ')[0]:date;
    });
    way.registerTransform("dateNewFormatter", function(date) {
        if(date){
            if(date==='9999-12-31'){
                return '--';
            }
            var array = date.split("-");
            return  array[0]+"年"+array[1]+"月";
        }else{
            return date;
        }
    });
});

$monitor.scroll();