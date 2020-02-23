<section class="col-xs-{|row|}" style="height:470px;color: #333;" id="List_{|LinkList|}">
    <div class="box-body" style="overflow:hidden;height: 100%;padding: 20px 10px 0px 10px;">
        <ul style="list-style:none;line-height: 60px;height: 60px;-webkit-padding-start:23px;font-size: 18px;font-weight: bold;color: #888;">
            <li style="float:left; margin-right:40px;cursor: pointer;" onclick="loadUndoTask();" :class="{portal_active : active == true}" >我的待办任务</li>
            <li style="float:left; margin-right:10px;cursor: pointer;" onclick="loadDoneTask();" :class="{portal_active : active == false}" >我的已办任务</li>
            <li style="float:right;cursor: pointer;font-size: 12px;color: #0e3d6e;" v-on:click="moreNormal();">更多...</li>
        </ul>
        <ul class="list-group protal list-hover01" style="margin-bottom: 0px!important;height: 331px;border-bottom: 1px solid #fff;border-top: 1px solid #fff;">
             <li class="list-item list-item-task" v-for="item in list" v-on:click="openWindow(item);" style="cursor: pointer; border-bottom: 1px solid #fff;height: 55px;line-height: 45px;">
                 <div style="float:left;width:90px;">
                     {{!item.ExecTime ? item.CreateTime : item.ExecTime | formatDate }}
                 </div>
                 <div class="nowrap">
                     {{item.TaskName}}
                 </div>
             </li>
             <img id="_img_{|LinkList|}" style="display:none;left: 40%;position: relative;top:100px;" src="/PortalLTE/Images/no-msg.png">
         </ul>
     </div>
</section>
<script>
    var isUndo = true;              
    var List_{|LinkList|} = new Vue({
        el: '#List_{|LinkList|}',
        data: {
            list: loadUndoTask(),
            active: isUndo
    },
    methods: {
        openWindow: function (task) {
            if (task.FormWidth == "") task.FormWidth = "80%";
            if (task.FormHeight == "") task.FormHeight = "80%";
            var url = task.FormUrl;
            if (url.indexOf('?') >= 0)
                url += "&ID=" + task.ID + "&TaskExecID=" + task.TaskExecID;
            else
                url += "?ID=" + task.ID + "&TaskExecID=" + task.TaskExecID;
            var opts = {
                url: url,
                title: task.TaskName,
                width: task.FormWidth,
                height: task.FormHeight,
                showMaxButton: true,
                ondestroy: function (action) {
                }
            };
            if (NoPopupLayer && NoPopupLayer != "true")
                mini.open(opts);
            else
                window.open(url);
        },
        moreNormal: function () {
            var url = '/MvcConfig/WorkFlow/Task/MyUndoList';
            if(!isUndo)
                url = '/MvcConfig/WorkFlow/Task/MyDoneList';
            var opts = {
                url: url,
                title: isUndo ? '我的待办任务': '我的已办任务',
                width: "80%",
                height: "80%",
                showMaxButton: true,
                ondestroy: function (action) {

                }
            };
            if (NoPopupLayer && NoPopupLayer != "true")
                mini.open(opts);
            else
                window.open(url);
        }
    },
    filters:{
            formatDate:function (val) {
                if(val)
                    return val.substring(0, 10).replace('-', '.').replace('-', '.');
                else
                    return '';
            }
    }
    });

    function loadUndoTask() {
        isUndo = true;
        $.ajax({
            url: "/PortalLTE/Main/UndoTask",
            type: "get",
            cache: false,
            dataType: "json",
            success: function (data) {
                var list = $.parseJSON(data);
                if(list.length > 6)
                    list = list.slice(0,6);
                List_{|LinkList|}._data.list = list;
                List_{|LinkList|}._data.active = isUndo;
                if(list.length > 0)
                    $("#_img_{|LinkList|}").hide();
                else
                    $("#_img_{|LinkList|}").show();
            },
            error: function (xhr, textStatus, errorThrown) {
                $("#_img_{|LinkList|}").show();
                List_{|LinkList|}._data.active = isUndo;
            }
        });
    }

    function loadDoneTask() {
        isUndo = false;
        $.ajax({
            url: "/PortalLTE/Main/DoneTask",
            type: "get",
            cache: false,
            dataType: "json",
            success: function (data) {
                var list = $.parseJSON(data);
                if(list.length > 6)
                    list = list.slice(0,6);
                List_{|LinkList|}._data.list = list;
                List_{|LinkList|}._data.active = isUndo;
                if(list.length > 0)
                    $("#_img_{|LinkList|}").hide();
                else
                    $("#_img_{|LinkList|}").show();
            },
            error: function (xhr, textStatus, errorThrown) {
                $("#_img_{|LinkList|}").show();
                List_{|LinkList|}._data.active = isUndo;
            }
        
        });
    }


</script>
