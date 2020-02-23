/**
 * 创建用户： 徐航
 * 创建时间：2015/12/23.
 */

//加载页面
function showProjectSearchPage(idname){
    $monitor.go("prj-search.html", "",idname);
    var cur = $('#'+$monitor.idName);
    var his = cur.find('.history');
    his.hide();
    var history=new Array();
    if(localStorage.getItem("projectSearchHistory")!=null){
        history=localStorage.getItem("projectSearchHistory").split("*");
    }
    setTimeout(function(){
        if(history.length>0){
            cur.find('.for-trash').css({"color": '#387ef5'});
        }
        his.show();
    },500);
    way.set("projectSearchHistory",ArrayToObject(history));
}

//搜索结果
function searchProjectHistory(e){
    setTimeout(function(){
        var text=$(e).val();
        var info=[];
        if(text!=""){
            var info=getDataFromCache(text);
            if(info.length<=0){
                console.log("从服务器取数据");
                $monitor.getRawData("ProjectSearch?content="+text,function(info){
                    $('#'+$monitor.idName).find(".search-content").hide();
                    console.log(info);
                    saveProjectHistory(text,info);
                    //绑定内容
                    way.set("projectSearch",{"Customers":info.Customers,"Projects":info.Projects,"Users":info.Users});
                    $monitor.scroll();
                });
            }else{
                $('#'+$monitor.idName).find(".search-content").hide();
                console.log("从缓存取数据");
                console.log(info);
            }
        }
    },1);
}

//从最近搜索结果中取搜索值
function getDataFromCache(text){
    var info=[];
    var results=$monitor.projectSearchHistory;
    var len = results.length;
    console.log(results);
    for(var i=0;i<len;i++){
        if(text==results[i].key){
            info=results[i].value;
            break;
        }
    }
    return info;
}

//保存搜索历史
function saveProjectHistory(text,info){
    var flag=true;
    var history=new Array();
    if(localStorage.getItem("projectSearchHistory")!=null){
        history=localStorage.getItem("projectSearchHistory").split("*");
    }
    for(var i=0;i<history.length;i++){
        if(text==history[i]){
            flag=false;
            break;
        }
    }
    if(flag){
        //若搜索历史中不含有该字段，则保存搜索结果到本地
        history.push(text);
        localStorage.setItem("projectSearchHistory",history.join("*"));
        $monitor.projectSearchHistory.push({key:text,value:info});
    }else{
        //若搜索历史中含有该字段,看看临时中是否有该结果，若无保存临时结果
        var flag=false;
        var results=$monitor.projectSearchHistory;
        for(var i=0;i<results.length;i++){
            if(text==results[i].key){
                flag=true;
                break;
            }
        }
        if(!flag){
            $monitor.projectSearchHistory.push({key:text,value:info});
        }
    }
    way.set("projectSearchHistory",ArrayToObject(history));
}

//搜索历史结果
function searchProjectThisHistory(e){
    var cur=$(e);
    var input=cur.closest(".page").find("input");
    input.val(cur.html());
    $monitor.keyup(input[0]);
    searchProjectHistory(input[0]);
}

//清除搜索历史
function clearProjectSearchHistory(){
    localStorage.removeItem("projectSearchHistory");
    $monitor.projectSearchHistory=[];
    $('#'+$monitor.idName).find('.for-trash').css({"color": '#999999'});
    way.set("projectSearchHistory",new Array());
}

//数组绑定
function ArrayToObject(array){
    var result=new Array();
    $.each(array,function(index,value){
        result.push({"value":value})
    });
    return result;
}

//是否显示里程碑
function showSearchMilestone(e,event){
    var cur=$(e);
    var timeLine=cur.siblings(".prj-search-result");
    event.preventDefault();
    event.stopPropagation();
    var t=cur.find("i");
    if(t.hasClass("glyphicon-menu-up")){
        t.removeClass("glyphicon-menu-up").addClass("glyphicon-menu-down");
        setTimeout(function(){
            timeLine.hide();
            cur.find("span").text("查看");
            $monitor.scroll();
        },500);
    }else{
        //项目ID
        var id=cur.siblings(".rememberCls").text();
        cur.siblings(".prj-search-result").find("li").attr("way-repeat",id+"search-result-projectMileStone");
        $monitor.getRawData("ProjectOverview/"+id,function(info){
            way.set(id+"search-result-projectMileStone",info);
            /*        cur.siblings(".time-line").find('li:eq(0)').addClass("active");*/
        });
        t.removeClass("glyphicon-menu-down").addClass("glyphicon-menu-up");
        setTimeout(function(){
            timeLine.show();
            $monitor.scroll();
            $monitor.hideLoading();
            cur.find("span").text("收起");
        },500);
    }
}
//收起或展开搜索结果
function hideOrShow(e){
    var t=$(e).find("i");
    if(t.hasClass("glyphicon-menu-up")){
        t.removeClass("glyphicon-menu-up").addClass("glyphicon-menu-down");
        $(e).parent().siblings(".detail").hide();
    }else{
        t.removeClass("glyphicon-menu-down").addClass("glyphicon-menu-up");
        $(e).parent().siblings(".detail").show();
    }
}

//显示结果详细
function showProjectSearchResult(e,type){
    $monitor.isHideLoading=false;
    var id =$(e).find(".rememberCls").text();
    var searchText = $(e).find(".liCls").text();
    $('#'+$monitor.idName).find('input').blur();
    $monitor.go("prj-search-result.html","ProjectSearch"+"?type="+type+'&id='+id,"prjSearchResult"+id+type,function(info){
        $("#"+'prjSearchResult'+id+type).find(".bindParentData").attr("way-scope",id+type);
        way.set(id+type,{"type":(type==="Customers"? "客户":"负责人"),"searchText":searchText});
        setTimeout(function(){
            transformData(info,"prjSearchResult");
            $monitor.hideLoading();
            $monitor.scroll();
        },500);
    });
}