/**
 * 创建用户： 徐航
 * 创建时间：2015/12/23.
 */

//加载页面
function showSearchPage(idname){
    $monitor.go("search.html", "",idname);
    var history=new Array();
    if(localStorage.getItem("contractSearchHistory")!=null){
        history=localStorage.getItem("contractSearchHistory").split("*");
    }
    way.set("contractSearchHistory",ArrayToObject(history));
}

//搜索结果
function searchContractHistory(e){
    setTimeout(function(){
        var text=$(e).val();
        var info=[];
        if(text!=""){
            var info=getDataFromCache(text);
            if(info.length<=0){
                console.log("从服务器取数据");
                $monitor.getRawData("ContractSearch?content="+text,function(info){
                    console.log(info);
                    saveHistory(text,info);
                    //绑定内容
                    way.set("contractSearch",{"Customers":info.Customers,"Contracts":info.Contracts,"Users":info.Users});
                    $monitor.scroll();
                });
            }else{
                console.log("从缓存取数据");
                console.log(info);
            }
            $(".search-content").hide();
        };
    },1);
}

//从最近搜索结果中取搜索值
function getDataFromCache(text){
    var info=[];
    var results=$monitor.contractSearchHistory;
    console.log(results);
    for(var i=0;i<results.length;i++){
        if(text==results[i].key){
            info=results[i].value;
            break;
        }
    }
    return info;
}

//保存搜索历史
function saveHistory(text,info){
    var flag=true;
    var history=new Array();
    if(localStorage.getItem("contractSearchHistory")!=null){
        history=localStorage.getItem("contractSearchHistory").split("*");
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
        localStorage.setItem("contractSearchHistory",history.join("*"));
        $monitor.contractSearchHistory.push({key:text,value:info});
    }else{
        //若搜索历史中含有该字段,看看临时中是否有该结果，若无保存临时结果
        var flag=false;
        var results=$monitor.contractSearchHistory;
        for(var i=0;i<results.length;i++){
            if(text==results[i].key){
                flag=true;
                break;
            }
        }
        if(!flag){
            $monitor.contractSearchHistory.push({key:text,value:info});
        }
    }
    way.set("contractSearchHistory",ArrayToObject(history));
}

//搜索历史结果
function searchThisHistory(e){
    var cur=$(e);
    var input=cur.closest(".page").find("input");
    input.val(cur.html());
    $monitor.keyup(input[0]);
    searchContractHistory(input[0]);
}

//清除搜索历史
function clearSearchHistory(){
    localStorage.removeItem("contractSearchHistory");
    $monitor.contractSearchHistory=[];
    way.set("contractSearchHistory",new Array());
}

//数组绑定
function ArrayToObject(array){
    var result=new Array();
    $.each(array,function(index,value){
        result.push({"value":value})
    })
    return result;
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
function showSearchResult(e,type){
    $monitor.isHideLoading=false;
    var id =$(e).find(".rememberCls").text();
    var searchText = $(e).find(".liCls").text();
    $monitor.go("search-result.html","ContractSearch/"+id+"?type="+type,"searchResult"+id+type,function(info){
        $("#searchResult"+id+type).find(".bindParentData").attr("way-scope",id+type);
        $("#searchResult"+id+type).find("li").attr("way-repeat",id+type+".contractList");
        way.set(id+type,{"type":(type==="Customers"? "客户":"负责人"),"searchText":searchText,"contractList":info});
        setTimeout(function(){
            $monitor.hideLoading();
            $monitor.scroll();
        },500);
    });
}