/**
 * 产品名称：Colla.
 * 创建用户： 秦佳.
 * 创建时间： 2015/12/23.
 * 创建原因：
 */
//显示合同明细页面
function showContractInfo(e){
    var id = $(e).find(".rememberCls").text();
    $monitor.getRawData("Contract/" + id + "/ReceiptRecord", function (info) {
        var receiptRecord = info;
        $monitor.go("contract.html", "Contract/" + id + "/Detail", id, function (info) {
            $("#" + id).find(".bindTitle").attr("way-scope", id);
            $("#" + id).find(".planReceipt").attr("way-repeat", id + ".PlanReceipt");
            $("#" + id).find(".receiptRecord").attr("way-repeat", id + ".ReceiptRecord");
            $("#" + id).find(".shoukuan").hide();
            if (info.ContractRMBAmount == 0) {
                info.receiptRate = "";
            } else {
                info.receiptRate = ((info.SummaryReceiptValue / info.ContractRMBAmount) * 100).toFixed(2) + '%';
            }
            
            $("#" + id).find(".complete-cls").css({ "width": info.receiptRate });

            $.each(receiptRecord, function (index, item) {
                if (item.Name == "") {
                    item.Name = "未对账";
                    item.Css = "record-none";
                }
            });

            way.set(id, { "contractInfo": info, "PlanReceipt": [], "ReceiptRecord": receiptRecord });
            $monitor.isHideLoading = false;
            $("#" + id).find(".daokuan").show();
            setTimeout(function () {
                $monitor.scroll();
                $monitor.hideLoading();
            }, 500);
        });
    });
}

//收款计划及到款记录切换
function changeReceiptList(e,name) {
    var cur=$(e);
    cur.addClass("active");
    cur.siblings().removeClass("active");
    $monitor.getRawData("Contract/"+$monitor.idName+"/"+name,function(info){
            if(name==="PlanReceipt"){
                $.each(info,function(index,item){
                    if(item.State=="UnReceipt"){
                        item.Css="is-edit";
                    }else{
                        item.Css="";
                    }
                });

                way.set($monitor.idName+"."+name,info);
                    var now = new Date();
                    setTimeout(function(){
                        $(".is-edit").find("input").each(function(){
                            $(this).mobiscroll().date({
                                theme: 'mobiscroll',
                                lang: 'zh',
                                display: 'bottom',
                                dateOrder: 'yymm',
                                dateFormat: 'yy年mm月',
                                startYear: now.getFullYear()-10,
                                endYear: now.getFullYear() + 10,
                                minWidth: 100
                            });
                        });
                    },300);
                    cur.parent().parent().parent().find("ul:eq("+cur.index()+")").css("display","block");
                    $("#"+$monitor.idName).find(".shoukuan").show();
                    $monitor.scroll();
            }else{
                $("#"+$monitor.idName).find(".daokuan").show();
                $monitor.scroll();
            }
    });
    cur.parent().parent().parent().find("ul").css("display","none");
    name!=="PlanReceipt"? cur.parent().parent().parent().find("ul:eq("+cur.index()+")").css("display","block"):null;
}

//修改计划收款日期
function updatePlanReceiptDate(e){
    var cur=$(e);
    var id=cur.parent().parent().siblings(".rememberCls").html();
    var date=cur.val().replace("年","-").replace("月","-01");
    $.ajax({
        type: "PUT",
        url: $monitor.apiEndpoint + "Contract/" + id + "?date=" + date,
        dataType: "json",
        success: function (result) {
            console.log(result);
        },
        error : function() {
            alert("网络连接异常！");
        }
    });
}
//显示tips信息
function ShowInfo(e){
    $monitor.alert(e.innerHTML);
    /*$monitor.clickTips($monitor.idName,e,"就是我哟");*/
}