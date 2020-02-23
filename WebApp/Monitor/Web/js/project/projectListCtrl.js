/**
 * Created by 徐航 on 2015/12/25.
 */
//进入ProjectList 加载页面数据
var tempData={"HealthDegree":[],"ProjectTime":[]};

function EnterProjectList(){
    $monitor.go('prj-list.html',"ProjectOverview?condition=(p.State = 'Plan' OR p.State = 'Execute')",'ProjectList',function(info){
        $("#"+'ProjectList').find(".state-use").addClass("active");
        transformData(info,'');
       });
}

function transformData(info,value){
    tempData={"HealthDegree":[],"ProjectTime":[]};
    var cur = $("#"+$monitor.idName);
    if(value!=='prjSearchResult'){
        cur.find(".bindParentData").attr("way-scope",$monitor.idName+"projectInfo");
    }
    cur.find(".list-model").attr("way-repeat",$monitor.idName+"projectInfo.ProjectList").hide();
    //处理显示数据
    if(info.Data.length!==0){
        $.each(info.Data,function(n,value) {
            if(value.State===2){
                value.State="延期";
                value.StateCss="delay";
            }else{
                value.State="进行";
                value.StateCss="underway";
            }
            tempData.HealthDegree.push(value);
        });
    }
    var type=$("#orderFilter").text()=="健康度排序"?"HealthDegree":"ProjectTime";
    sortData(type);
    setTimeout(function(){
        cur.find(".list-model").show();
        $monitor.scroll();
    },500);
}
//按条件排序
function orderByCondition(type,e){
    $(e).addClass("active");
    $monitor.resetIdName(e);
    var parentId=$(e).parent().parent().attr("id");
    var currentEle = $("#"+$monitor.idName).find(".custom-select[data-id='"+ parentId +"'] span");
    currentEle.text($(e).html());
    //保存请求条件
    currentEle.attr("data-condition",type);
    $(e).siblings().removeClass("active");


//    var cur = $("#ProjectList");
    var cur = $("#"+$monitor.idName);
    cur.find(".bindParentData").attr("way-scope",$monitor.idName+"projectInfo");
    cur.find(".list-model").attr("way-repeat",$monitor.idName+"projectInfo.ProjectList").hide();
    sortData(type);
    $monitor.closefilter();

    cur.find(".list-model").show();
}

//数据排序
function sortData(type){
    if(type=="HealthDegree"){
        way.set($monitor.idName+"projectInfo",{"projectSum":tempData.HealthDegree.length,"ProjectList":tempData.HealthDegree});
    }else{
        var tempArray = tempData.HealthDegree;
        tempData.ProjectTime = [];
        //按照时间排序
        if( tempArray.length!==0){
            $.each(tempArray,function(n,value) {
                value.dateNum = Date.parse(value.CreateDate)/1000;
                tempData.ProjectTime.push(value);
            });
            tempData.ProjectTime.sort(sortTime);
        }else{
            tempData.ProjectTime = [];
        }
        way.set($monitor.idName+"projectInfo",{"projectSum":tempData.ProjectTime.length,"ProjectList":tempData.ProjectTime});
    }
}

function sortTime(a,b){
    return b.dateNum- a.dateNum;
}
//是否显示里程碑
function showMilestone(e,event){
    var cur=$(e);
    var timeLine=cur.siblings(".time-line");
    event.preventDefault();
    event.stopPropagation();
    //项目ID
    var id=cur.siblings(".rememberCls").text();
    cur.siblings(".time-line").find("li").attr("way-repeat",id+"projectMileStone");
    $monitor.getRawData("ProjectOverview/"+id,function(info){
        $.each(info,function(index,value){
            if(value.State=='已完成'){
                value.MistonState="miston-done";
            }else if(value.State=='延期'){
                value.MistonState="miston-delay";
            }else{
                value.MistonState="miston-doing";
            }
        });
        way.set(id+"projectMileStone",info);
/*        cur.siblings(".time-line").find('li:eq(0)').addClass("active");*/
    });
    var t=cur.find("i");
    if(t.hasClass("glyphicon-menu-up")){
        t.removeClass("glyphicon-menu-up").addClass("glyphicon-menu-down");
        setTimeout(function(){
            timeLine.hide();
            cur.find("span").text("查看");
            $monitor.scroll();
        },500);
    }else{
        t.removeClass("glyphicon-menu-down").addClass("glyphicon-menu-up");
        setTimeout(function(){
            timeLine.show();
            cur.find("span").text("收起");
            $monitor.scroll();
        },500);
    }
}

//选择或取消选择
function selectThisCondition(e){
    var cur=$(e);
    if(!cur.hasClass("active")){
        cur.addClass("active");
    }else{
        cur.removeClass("active");
    }

    //是否选择全部
    var flag=false;
    cur.parent().find("li").each(function(){
        flag|=$(this).hasClass("active");
    });
    cur.parent().parent().find("span").text(flag?"":"全部");
}

//重置选择
function unsetChoose(){
    $("#"+$monitor.idName).find(".un-prj-model").find("li").removeClass("active");
}

//收起或展开搜索结果
function hideOrShow(e){
    var t=$(e);
    if(t.hasClass("glyphicon-menu-up")){
        t.removeClass("glyphicon-menu-up").addClass("glyphicon-menu-down");
        $(e).parent().siblings("ul").hide();
    }else{
        t.removeClass("glyphicon-menu-down").addClass("glyphicon-menu-up");
        $(e).parent().siblings("ul").show();
    }
}

//确认选择
function confirmSelect(){
    var condition="";
        $("#"+$monitor.idName).find(".un-prj-model").each(function(){
            var con=[];
            var cur=$(this);
            cur.find("li").each(function(){
                if($(this).hasClass("active")){
                    con.push("'"+$(this).attr("data-id")+"'");
                    if($(this).attr("data-id")==='Plan'){
                        con.push("'"+"Execute"+"'");
                    }
                }
            });
            if(con.length>0){
                condition += cur.attr("data-id")+" in ("+con.join(",")+") and ";
            }
        });
        if(condition.indexOf("and")>=0){
            condition=condition.substring(0,condition.length-4);
        }
       /* if(condition!=""){
            condition="and " +condition;
        }*/
        console.log(condition);
        //删选条件：condition
    //接口
    $monitor.getRawData("ProjectOverview?condition="+condition,function(info){
        transformData(info,'');
    });
    $monitor.closefilter();
}

//index跳转
function goAndSelect(value){
    var content = value;
    var condition="";
    condition = "and PhaseValue in ('"+value+"')";
    $monitor.go('prj-list.html',"ProjectOverview?condition=(p.State = 'Plan' OR p.State = 'Execute')"+condition,value+'ProjectList',function(info){
       var cur =  $("#"+value+'ProjectList').find(".for-use");
           cur.find("li").each(function(index,value){
                if($(this).attr("data-id")===content){
                    $(this).addClass("active");
                }
            });
        transformData(info,value);
    });
}