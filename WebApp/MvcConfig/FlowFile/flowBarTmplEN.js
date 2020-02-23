﻿<style type='text/css'>
    .icon-delegate
{
    background: url(/Workflow/Scripts/Images/delegate.gif) no-repeat;
}
.icon-ask
{
    background: url(/Workflow/Scripts/Images/ask.gif) no-repeat;
}
.icon-circulate
{
    background: url(/Workflow/Scripts/Images/circulate.gif) no-repeat;
}
.icon-view
{
    background: url(/Workflow/Scripts/Images/view.gif) no-repeat;
}
.mini-menu-horizontal .mini-menu-inner
{
    padding: 6px;
    _padding-bottom: 7px;
    background-image: none;
    //background-color: #f8f8f8;
}
//.mini-menuitem-hover
//{
//    border: solid 1px #f8f8f8;
//    background-color: #f8f8f8;
//    background-image: none;
//    color: #f8f8f8;
//}
#comment .fonttd {
    font-family:'Microsoft YaHei';
    font-size:12px;
    white-space: nowrap;
    overflow: hidden; 
    text-overflow: ellipsis;
}
#comment table{
    table-layout:fixed;
    empty-cells:show; 
    border-collapse: collapse;
    margin:0 auto;
}
#flowAskMsg table td{
    height:20px;
}

#comment table.t1{
    border:1px solid #cad9ea;
    color:#666;
}
#comment table.t1 th {
    background-image: url(/Workflow/Scripts/Images/tablebg.gif);
    background-repeat:repeat-x;
    height:30px;
}

#comment table.t1 td,table.t1 th{
    border:1px solid #cad9ea;
    padding:0 1em 0;
}
#comment table.t1 tr.a1{
    background-color:#f5fafe;
}
#flowAskMsg table.t1 th:first-child {
    width:66px;
}
#comment table.t1 td:first-child {
    width:66px;
}

.scrollbar::-webkit-scrollbar{
    width: 3px;
    height: 12px;
    background-color: #f5f5f5;
}
.scrollbar::-webkit-scrollbar-track{
    -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,.3);
    border-radius: 10px;
    background-color: #f5f5f5;
}
.scrollbar::-webkit-scrollbar-thumb{
    height: 20px;
    border-radius: 10px;
    -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,.3);
    background-color: #555;
}
#flowSingleFile .mini-singlefile-border
{
    display: inline-block;
    height: 21px;
    overflow: hidden;
    vertical-align: middle;
    background: white;
    border: solid 1px #fff!important;
    width: auto;
    border-radius: 5px;
    padding: 5px;
    position: relative;
    padding-right: 22px;
    padding-left: 1px;
    display: block;
}
#flowSingleFile .mini-singlefile-viewer {
    display:none;
}
</style>
<div style='width: 100%; position: fixed; top: 0; left: 0; z-index: 999;' id='flowBar'>
    <ul id='flowMenubar' class='mini-menubar' style='width: 100%;'>
        <div property='toolbar'>
            <span id='divFormVersion' style='display:none;'>version：<div id='cmbVersion' class='mini-combobox' style='width:50px;' onvaluechanged='onVersionChanged'></div> </span>
            <span id='divFormStatus' style='display:none'>&nbsp;State：<span id='spanFormStatus'>Not Starting</span></span>&nbsp;
            <span name="urgency" class="mini-checkbox"  text="Urgent?"></span>&nbsp;&nbsp;
<a class='mini-button' onclick='showHelp()' iconcls='icon-help' plain='true'>help</a>
</div>
</ul>
</div>
<div style='width:100%;height:20px;display:none;' id="divAdvice"></div>

<div id="adviceWindow" class="mini-window" title="opinion" style="width: 420px; min-height: 130px; position: fixed; z-index: 999;" showmodal="true" allowresize="false" allowdrag="true">
    <div class="queryDiv">
        <form id="adviceForm" method="post">
            <input name='Advice' emptytext='Please Input Your Opinion' class='mini-textarea' style='width: 100%; height: 67px;' />
        </form>
        <div id='defaultAdvice' style='padding-top:2px;margin-left:5px;text-align:left;display:block'><span style='display:inline-block'>常用：&nbsp;&nbsp;</span><span id='defaultItem'></span></div>
        <div id="_flowButton">
        </div>
    </div>
</div>

<div id="commentWindow" class="mini-window" title="Reply Message" style="width: 420px; min-height: 130px; position: fixed; z-index: 999;" showmodal="true" allowresize="false" allowdrag="true">
    <input id="nodeID" style="display: none;" />
    <div>
        <form id="commentForm" method="post">
            <div><input name='_Comment' emptytext='Please Input Your Message' class='mini-textarea' style='width: 100%; height: 67px;' /></div>
            <div style="height: 25px; text-align: left; border-bottom: solid 1px #cccccc;padding-top:8px;padding-bottom:8px;">
                <div style="text-overflow:ellipsis;overflow:hidden;white-space:nowrap; height: 28px; padding-left:5px; line-height: 28px; width: 200px; float: left;" >
                    <div class="flow-addImgCss">
                        <img src="/CommonWebResource/RelateResource/image/customctrl/user.png" style="padding-top:6px;"/></div>
                    <div class="flow-addCss"><a id="commentUser" ids="" style="color: #3c8dbc;cursor: pointer;" onclick="addCommentUser()">CC.</a></div>
                </div>
                <div style=" height: 28px; line-height: 28px; width: 200px; float: left;" >
                    <input name="flowSingleFile" id="flowSingleFile" class="mini-singlefile" autocomplete='off' />
                </div>
            </div>
        </form>
        <div id="_flowCommentButton" style="text-align:center;padding:10px;">
            <a id="submitComment" class='mini-button mini-button-plain' onclick="saveComment()" style='margin-right: 20px;' hidefocus=''><span class='mini-button-text  mini-button-icon icon-ok'>Sumbit</span></a>
            <a id="clearComment" class='mini-button mini-button-plain' hidefocus='' onclick='clearComment()'><span class='mini-button-text  mini-button-icon icon-undo'>Clear</span></a>
            <a id="clearUserComment" style="display: none;" class='mini-button mini-button-plain' hidefocus='' onclick="clearComment('user')"><span class='mini-button-text  mini-button-icon icon-undo'>Clear CC.</span></a>
            <a id="clearFileComment" style="display: none;" class='mini-button mini-button-plain' hidefocus='' onclick="clearComment('file')"><span class='mini-button-text  mini-button-icon icon-undo'>Clear attachment</span></a>
        </div>
    </div>
</div>
<script type="text/javascript">
    $(function(){
        bindFlowComment();
        bindDefaultAdvice(jQuery("#defaultItem")[0], function(){
            var __advice = $("textarea[name='Advice']");
            __advice.unbind("blur");
            mini.getbyName("Advice").setValue(this.innerText.replace('－',''));
        });
        $('.formDiv').css('padding-top','35px');
    });
    function clearAdvice(obj) {
        var form = new mini.Form("#adviceForm");
        form.clear();
    }
    function flowApprove(button, bType) {
        if (FlowCurrntStep && FlowCurrntStep.HideAdvice == "1"){
            eval(button);
        }else{
            var click = bType == 1 ? "onclick=\'" + button + "\;closed()'" : 'onclick=\"' + button + '\;closed()"';
            var html = "<a class='mini-button mini-button-plain' style='margin-right: 20px;' hidefocus='' "+click+" href='javascript:void(0)'><span class='mini-button-text  mini-button-icon icon-ok'>"
                + "submit</span></a> <a class='mini-button mini-button-plain' hidefocus='' onclick='clearAdvice()' href='javascript:void(0)'><span class='mini-button-text  mini-button-icon icon-undo'>clear</span></a>";
            document.getElementById("_flowButton").innerHTML = html;
            var window = mini.get('adviceWindow');
            if (!window) { msgUI("未找到指定的window窗体！", 4); return; }
            window.onscroll=scroll;
            window.queryWindowId = window.id;
            window.show();
        }
    }
    function closed(){
        var window = mini.get('adviceWindow');
        if (!window) { msgUI("未找到指定的window窗体！", 4); return; }
        window.queryWindowId = window.id;
        window.hide();
    }
</script>