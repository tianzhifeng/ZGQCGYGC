/**
 * Created by 徐航 on 2015/12/25.
 */
function EnterProjectInfo(e){
    var id = $(e).find('.rememberCls').text();
    $monitor.go('prj-info.html','ProjectDetail/'+id+'/PrjInfo',id,function(info){
            $("#"+id).attr("projectId",id);
            $("#"+id).find(".prj-info").attr("way-scope",id+"projectInfo.baseInfo");
            $("#"+id).find(".prj-user").find('.itemRepeat').attr("way-repeat",id+"projectInfo.addressList");
            $("#"+id).find(".prjusers").hide();
            way.set(id+"projectInfo",{"baseInfo":info,"addressList":[]});
        setTimeout($monitor.scroll(),500);
        way.registerTransform("firstName", function(name) {
            if(name!=undefined&&name!=""){
                return name.substring(0,1);
            }else{
                return null;
            }
        });
    });
}

//menu 切换
function projectInfo(id,e){
    var cur=$(e);
    cur.addClass("active");
    cur.siblings("button").removeClass("active");
    if(id==='prjusers'){
        var flag = {'value':''};
        $("#"+$monitor.idName).find(".prjusers").show();
        searchUsers(flag);
    }else{
        $monitor.getRawData('ProjectDetail/'+$monitor.idName+'/PrjInfo',function(info){
                way.set(id+"projectInfo.baseInfo",info);
        });
    }
    $("#"+$monitor.idName).find("."+id).show();
    $("#"+$monitor.idName).find("."+id).siblings(".has-show").hide();
    $monitor.scroll();
}

//通讯录搜索
function  searchUsers(e){
    var condition = e.value;
    var id = $monitor.idName;
    $monitor.getRawData('ProjectDetail/'+id+'/PrjUsers?condition='+condition,function(info){
            way.set(id+"projectInfo.addressList",info);
            $monitor.scroll();
    });
}

//打电话或发邮件
function callUser(type,e){
    location.href=type + ":" +  e.innerHTML;
}
//显示tips信息
function showNumber(e){
    $monitor.alert(e.innerHTML);
    /*$monitor.clickTips($monitor.idName,e,"就是我哟");*/
}