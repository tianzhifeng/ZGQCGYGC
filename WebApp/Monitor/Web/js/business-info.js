//进入实际到款统计
function showReceipt(idname) {
    //实际收款统计页面
    var paramsData = $monitor.paramsData.Receipt;
    paramsData.Title = "当年实际收款";
    $monitor.go("busi-info.html", "BusinessOverview?type=Receipt",idname, function (info) {
        paramsData.GrowthRate =  paramsData.GrowthRate.split('%')[0];
        if(paramsData.GrowthRate!=="--"){
            var cur =  $("#"+$monitor.idName).find(".contractRate");
            paramsData.GrowthRate =  paramsData.GrowthRate.split('%')[0];
            if( paramsData.GrowthRate >= 0){
                cur.parent().addClass("up-color");
                cur.addClass("span-up-icon icon-up");
            }else{
                cur.parent().addClass(" down-color");
                cur.addClass("span-up-icon icon-down");
            }
            paramsData.GrowthRate = paramsData.GrowthRate+'%';
        }
        var transform3 = $("#"+$monitor.idName).find(".parameter");
        paramsData.FinishRate =  paramsData.FinishRate.split('%')[0];
        if(paramsData.FinishRate>=100){
            transform3.css({"width" :  '100%',"background-color": '#66cc66'});
        }else {
            if(paramsData.FinishRate<100 && paramsData.FinishRate>=50){
                transform3.css({"width" :  paramsData.FinishRate+'%',"background-color": '#ffa128'});
            }else {
                transform3.css({"width" :  paramsData.FinishRate+'%',"background-color": '#f85255'});
            }
        }
        paramsData.FinishRate =  paramsData.FinishRate+'%';
        $("#"+idname).find(".chart-info-div").attr("way-scope",idname+"receipt-params");
        //初始化并配置图表
        chatConfig(idname,info,paramsData);
        //数据绑定
        way.set("showReceipt", info);
        way.set(idname+"receipt-params", paramsData);
        var array = [],contractYears=[];
        var n = info.single[0].data.length;
        for(;n>0;n--){
            var obj = {
                    month: n+"月",
                    five: info.single[0].data[n-1],
                    four: info.single[1].data[n-1],
                    three: info.single[2].data[n-1],
                    two: info.single[3].data[n-1],
                    one: info.single[4].data[n-1],
                    rate:rateGenerator(info.single[1].data[n-1],info.single[0].data[n-1])
                };
            array.push(obj);
        }
        for (var i = 0; i < info.single.length; i++) {
            contractYears.push(info.single[i].year + "年");
        }
        way.set("timeLineData.List", array);
        way.set("contractYears", contractYears);
        $monitor.scroll();
    });
}
//进入合同额统计
function showContract(idname) {
    //已签合同统计页面
    var paramsData = $monitor.paramsData.Contract;
    paramsData.Title = "当年已签合同额";
    $monitor.go("busi-info.html", "BusinessOverview?type=Contract",idname,function (info) {
        $("#"+idname).find(".chart-info-div").attr("way-scope",idname+"receipt-params");
        if(paramsData.GrowthRate!=="--"){
            var cur =  $("#"+$monitor.idName).find(".contractRate");
            paramsData.GrowthRate =  paramsData.GrowthRate.split('%')[0];
            if( paramsData.GrowthRate >= 0){
                cur.parent().addClass("up-color");
                cur.addClass("span-up-icon icon-up");
            }else{
                cur.parent().addClass(" down-color");
                cur.addClass("span-up-icon icon-down");
            }
            paramsData.GrowthRate = paramsData.GrowthRate+'%';
            paramsData.FinishRate = paramsData.FinishRate+'%';
        }
        //进度条
        var transform3 = $("#"+$monitor.idName).find(".parameter");
        paramsData.FinishRate =  paramsData.FinishRate.split('%')[0];
        if(paramsData.FinishRate>=100){
            transform3.css({"width" :  '100%',"background-color": '#66cc66'});
        }else {
            if(paramsData.FinishRate<100 && paramsData.FinishRate>=50){
                transform3.css({"width" :  paramsData.FinishRate+'%',"background-color": '#ffa128'});
            }else {
                transform3.css({"width" :  paramsData.FinishRate+'%',"background-color": '#f85255'});
            }
        }
        //初始化并配置图表
        chatConfig(idname,info,paramsData);
        //数据绑定
        way.set("showContract", info);
        way.set(idname+"receipt-params", paramsData);
        var contractArray = [],contractYears=[];
        var n = info.single[0].data.length;
        for(;n>0;n--){
            var obj = {
                month: n+"月",
                five: info.single[0].data[n-1],
                four: info.single[1].data[n-1],
                three: info.single[2].data[n-1],
                two: info.single[3].data[n-1],
                one: info.single[4].data[n-1],
                rate:rateGenerator(info.single[1].data[n-1],info.single[0].data[n-1])
            };
            contractArray.push(obj);
        }

        for (var i = 0; i < info.single.length; i++) {
            contractYears.push(info.single[i].year + "年");
        }
        way.set("timeLineData.List", contractArray);
        way.set("contractYears", contractYears);
        $monitor.scroll();
    });
}

//共用图表配置
function chatConfig(idname,info,paramsData) {
    //图表加载
    var busiform = echarts.init($("#"+idname).find(".busi-form")[0]);
    var chartArray = way.get("chartArray");
    if(chartArray.indexOf(idname)===-1){
        chartArray.push(idname,busiform);
    }
    //图标数据生成
    //表单
    var formOption = {
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            data: [info.single[0].year, info.single[1].year, info.single[2].year, info.single[3].year, info.single[4].year],
            x: 'right',
            itemWidth: 7,
            itemGap: 3,
            selected: {
                lastOne: false,
                last: false
            }
        },
        color: [
            '#c12e34', '#e6b600', '#0098d9', '#2b821d',
            '#005eaa', '#339ca8', '#cda819', '#32a487'
        ],
        calculable: false,
        grid: {
            x: 36,
            x2: 10,
            y: 30,
            y2: 33
        },
        xAxis: [
            {
                type: 'category',
                data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
            }
        ],
        yAxis: [
            {
                name:'单位:万元',
                type: 'value',
                formatter:function(value){
                    return value%100;
                }
            }
        ],
        series: [
            {
                name: info.single[4].year,
                type: 'bar',
                data: info.single[4].data
            },
            {
                name: info.single[3].year,
                type: 'bar',
                data: info.single[3].data
            },
            {
                name: info.single[2].year,
                type: 'bar',
                data: info.single[2].data
            },
            {
                name: info.single[1].year,
                type: 'bar',
                data: info.single[1].data
            },
            {
                name: info.single[0].year,
                type: 'bar',
                data: info.single[0].data
            }
        ]
    };
    busiform.setOption(formOption);

    var num = ($(window).width()>380)? 3:2;
    //手势操作切换（支持触摸）
    var touchDevice = "ontouchstart" in window;
    var startEvt, moveEvt, endEvt;
    //选择不同事件
    if (touchDevice) {
        startEvt = "touchstart";
        moveEvt = "touchmove";
        endEvt = "touchend";
    } else {
        startEvt = "mousedown";
        moveEvt = "mousemove";
        endEvt = "mouseup";
    }

    var temp = $("#"+idname).find(".time-model");
    temp[0].addEventListener(startEvt, startEvtHandler, false);
    temp[0].addEventListener(moveEvt, moveEvtHandler, false);
    temp[0].addEventListener(endEvt, endEvtHandler, false);

    //按下之后移动30px之后就认为swipe开始
    var SWIPE_DISTANCE = 30;
    //swipe最大经历时间
    var SWIPE_TIME = 500;
    var pt_pos;
    var ct_pos;
    var pt_time;
    var pt_up_time;
    var pt_up_pos;
    //获取swipe的方向
    var getSwipeDirection = function (p2, p1) {
        var dx = p2.x - p1.x;
        var dy = -p2.y + p1.y;
        var angle = Math.atan2(dy, dx) * 180 / Math.PI;

        if (angle < 45 && angle > -45) return "right";
        if (angle >= 45 && angle < 135) return "top";
        if (angle >= 135 || angle < -135) return "left";
        if (angle >= -135 && angle <= -45) return "bottom";
    };

    function getTouchPos(e) {
        var t = e.touches;
        if (t && t[0]) {
            return { x: t[0].clientX, y: t[0].clientY };
        }
        return { x: e.clientX, y: e.clientY };
    }

    function startEvtHandler(e) {
        // e.stopPropagation();
        var touches = e.touches;
        if (!touches || touches.length == 1) {//鼠标点击或者单指点击
            pt_pos = ct_pos = getTouchPos(e);
            pt_time = Date.now();
        }
    }

    function moveEvtHandler(e) {
        // e.stopPropagation();
        e.preventDefault();
        ct_pos = getTouchPos(e);
    }

    function endEvtHandler(e) {
        // e.stopPropagation();
        var dir;
        pt_up_pos = ct_pos;
        pt_up_time = Date.now();
        if (getDist(pt_pos, pt_up_pos) > SWIPE_DISTANCE && pt_up_time - pt_time < SWIPE_TIME) {
            dir = getSwipeDirection(pt_up_pos, pt_pos);

            var width = temp.find("li").width();

            var firstleft = temp.find("li:eq(0)")[0].style.left;
            var index;
            if (firstleft != "" && firstleft != "0px") {
                index = Math.round(parseFloat(firstleft) / width);
            } else {
                index = 0;
            }

            if (dir === "right") {
                if (index != 0) {
                    temp.find("li").each(function () {
                        $(this).animate({ "left": (index + 1) * width + "px" }, 500);
                    })
                    $("#"+idname).find(".line").find(".before").show();
                    $("#"+idname).find(".line").find(".after").show();
                } else {
                    $("#"+idname).find(".line").find(".before").hide();
                }
            } else if (dir === "left") {
                if (Math.abs(index) != temp.find("li").length - num) {
                    temp.find("li").each(function () {
                        $(this).animate({ "left": (index - 1) * width + "px"}, 500);
                    })
                    $("#"+idname).find(".line").find(".after").show();
                    $("#"+idname).find(".line").find(".before").show();
                } else {
                    $("#"+idname).find(".line").find(".after").hide();
                }
            }
        }
    }

    //计算两点之间距离
    var getDist = function (p1, p2) {
        if (!p1 || !p2) return 0;
        return Math.sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
    };

}

//切换单月统计和累计统计
function changeMenuIndex(index,e){
    var idName = $monitor.idName;
    var temp = $(e);
    var info;
    if(index === 0 ){
        temp.addClass("active");
        temp.siblings("button").removeClass("active");
        info =  way.get(idName).single;
    }else{
        temp.addClass("active");
        temp.siblings("button").removeClass("active");
        info =  way.get(idName).cumulation;
    }
    var changeArray = [];
    var n = info[0].data.length;
    for(;n>0;n--){
        var obj = {
            month: n+"月",
            five: info[0].data[n-1],
            four: info[1].data[n-1],
            three: info[2].data[n-1],
            two: info[3].data[n-1],
            one: info[4].data[n-1],
            rate:rateGenerator(info[1].data[n-1],info[0].data[n-1])
        };
        changeArray.push(obj);
    }
    way.set("timeLineData.List",changeArray);
    var newOption =  [
            {
                name: info[4].year,
                type: 'bar',
                data: info[4].data
            },
            {
                name: info[3].year,
                type: 'bar',
                data: info[3].data
            },
            {
                name: info[2].year,
                type: 'bar',
                data: info[2].data
            },
            {
                name: info[1].year,
                type: 'bar',
                data: info[1].data
            },
            {
                name: info[0].year,
                type: 'bar',
                data: info[0].data
            }
        ];
    var chartArray = way.get("chartArray");
    chartArray[chartArray.indexOf(idName)+1].setSeries(newOption,false);
    $monitor.scroll();
}

function rateGenerator(last,now){
    if(last===0){
        return '--';
    }else{
        if(now===0){
            return 0
        }else{
            return Number ((Number((now-last)/last))*100).toFixed(2)+'%';
        }
    }
}

