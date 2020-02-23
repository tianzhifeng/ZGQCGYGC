// 初始化一个甘特图，在页面加载完成时需要调用
/*
function CreateGWGantt(dom, url, settings) {

    var settings = settings || {};

    // 创建甘特图
    var gantt = new gwGantt();
    gantt.setStyle("width:100%;height:500px;");
    gantt.setBorderStyle("border:1");

    if (typeof dom === "string") {
        gantt.render(document.getElementById(dom));
    }
    else {
        gantt.render(dom);
    }

    if (settings.columns) {
        gantt.setColumns(settings.columns);
    }

    if (settings.showContextMenu) {
        // 加入右键菜单
        var ganttMenu = new GanttMenu();
        gantt.setContextMenu(ganttMenu);

        //监听菜单的opening事件，此事件在菜单显示前激发。可以控制菜单项的显示和可操作。
        if (settings.onMenuBeforeOpen) {
            ganttMenu.on("beforeopen", settings.onMenuBeforeOpen);
        }
    }

    if (settings.readonly) {
        gantt.setReadOnly(true);
    }

    // 加载数据到甘特图上
    function loadgwGanttTasks(json) {
        if (typeof json === "string") {
            data = mini.decode(json);
        }
        else {
            data = json;
        }

        // 加载数据
        gantt.loadTasks(data);

        // 隐藏加载中的遮盖层
        gantt.unmask();

        //createBaseline();
        //折叠全部
        //gantt.collapseAll();
    }

    function createBaseline() {
        var tasklist = gantt.getTaskList();
        for (var i = 0, l = tasklist.length; i < l; i++) {
            var task = tasklist[i];
            if (!task.Start || !task.Finish) continue;

            var baseline0 = {
                Start: mini.parseDate(task.FactStart),
                Finish: mini.parseDate(task.FactEnd)
            };

            task.Baseline = [];
            task.Baseline.push(baseline0);
        }
        gantt.refresh();
    }

    //    if (settings.onDrawCell) {
    //        gantt.on("drawcell", settings.onDrawCell);
    //    }

    if (typeof url != "undefined") {
        // 显示加载中的遮盖层
        gantt.loading();

        // 远程请求Json数据的地址
        if (typeof url == "string") {
            // 发起异步请求数据，并将数据加载到甘特图上
            $.ajax({
                url: url,
                type: "POST",
                cache: false,
                success: loadgwGanttTasks
            });
        }
        else { // 可以为Json的数据对象
            loadgwGanttTasks(url);
        }
    }

    return gantt;
}
*/

window.gwGantt = function (settings) {

    var s = $.extend({}, gwGantt.DefaultSettings, settings);
    this.gantt = new PlusGantt();

    // 1. 初始化：设置相关属性
    this.__InitGantt(s);

    // 3. 注册事件：处理单元格编辑，和条形图拖拽事件
    this.__RegEvent(this.gantt);

    // 4. 加载数据
    this.__LoadData(s);

    return this.gantt;
};

gwGantt.DefaultSettings = {
    columns: {},
    allowDragDrop: true,
    style: "width:100%;height:100%;",
    domId: document.body, // 甘特图容器ID，既可以是字符串（ID），也可以是dom对象
    treeColumn: '',
    borderStyle: 'border:1',
    readOnly: false,
    contextMenu: null, // 右键菜单，可以字符串（右键菜单ID），也可以菜单对象
    onContextMenuBeforeOpen: null,
    url: null
};

gwGantt.prototype = {
    __RegEvent: function (gantt) {
        gantt.on("cellbeginedit", this.__OnCellBeginEdit, this);
        gantt.on("aftercellcommitedit", this.__OnCellCommitEdit, this);
        gantt.on("itemdragstart", this.__OnItemDragStart, this);
        gantt.on("itemdragcomplete", this.__OnItemDragComplete, this);
        gantt.on("taskdragdrop", this.__OnTaskDragDrop, this);
    },
    __InitGantt: function (s) {
        this.gantt.setColumns(s.columns);
        this.gantt.setTreeColumn(s.treeColumn);

        this.gantt.setReadOnly(s.readOnly);
        this.gantt.setAllowDragDrop(s.allowDragDrop);
        this.gantt.setStyle(s.style);
        this.gantt.setBorderStyle(s.borderStyle);

        if (s.contextMenu)
            this.gantt.setContextMenu(typeof s.contextMenu === "string" ? mini.get(s.contextMenu) : s.contextMenu);

        this.gantt.render(typeof s.domId === "string" ? document.getElementById(s.domId) : s.domId);
    },
    __LoadData: function (s) {
        if (typeof s.url != "undefined") {
            // 显示加载中的遮盖层
            this.gantt.loading();

            // 远程请求Json数据的地址
            if (typeof s.url == "string") {
                // 发起异步请求数据，并将数据加载到甘特图上
                $.ajax({
                    url: s.url,
                    type: "POST",
                    cache: false,
                    success: this.__LoadGanttTasks
                });
            }
            else { // 可以为Json的数据对象
                this.__LoadGanttTasks(s.url);
            }
        }
    },
    __LoadGanttTasks: function (json) {  // 加载数据到甘特图上
        if (typeof json === "string") {
            data = mini.decode(json);
        }
        else {
            data = json;
        }
        if (this.gantt) {
            // 加载数据
            this.gantt.loadTasks(data);
            // 隐藏加载中的遮盖层
            this.gantt.unmask();
        }
        else {
            // 加载数据
            gantt.loadTasks(data);
            // 隐藏加载中的遮盖层
            gantt.unmask();
        }
        //折叠全部
        //this.gantt.collapseAll();
    },
    __OnTaskDragDrop: function (e) {
        e.cancel = true;
        var dragRecords = e.tasks, targetRecord = e.targetTask, action = e.action;

        this.gantt.moveTasks(dragRecords, targetRecord, action);
    },
    __OnCellBeginEdit: function (e) {
        var task = e.record, field = e.field;
        if (task.Summary) {
            if (field == 'Start' || field == 'Finish' || field == 'Duration') {
                e.cancel = true;
            }
        }
    },
    __OnCellCommitEdit: function (e) {
        e.cancel = true;
        var task = e.record, field = e.field, value = e.value, oldValue = task[field];

        if (mini.isEquals(oldValue, value)) return;

        try {

            //!!!处理单元格编辑提交
            var gantt = this.gantt;

            //标记任务的属性被修改
            gantt.setTaskModified(task, field);

            if (e.column.displayField) {
                task[e.column.displayField] = e.text;
            }

            switch (field) {
                case "Duration":
                    task.Duration = value;

                    if (task.Start) {
                        task.Finish = maxTime(task.Start);
                        task.Finish.setDate(task.Finish.getDate() + task.Duration - 1);

                        gantt.setTaskModified(task, "Finish");
                    }

                    break;
                case "Start":
                    task.Start = clearTime(value);

                    if (task.Start) {
                        task.Finish = maxTime(task.Start);
                        task.Finish.setDate(task.Start.getDate() + task.Duration - 1);

                        gantt.setTaskModified(task, "Finish");
                    }

                    break;
                case "Finish":
                    task.Finish = maxTime(value);

                    if (task.Finish && task.Start) {
                        var days = parseInt((task.Finish - task.Start) / (3600 * 24 * 1000));
                        task.Duration = days + 1;

                        gantt.setTaskModified(task, "Duration");
                    }

                    break;
                case "PredecessorLink":
                    gantt.setLinks(task, value);
                    return;
                    break;
                default:
                    task[field] = value;

                    break;
            }

            gantt.refresh();

        } catch (ex) {
            alert(ex.message);
        }
    },
    __OnItemDragStart: function (e) {
        if (e.action == "start") {
            e.cancel = true;
        }
    },
    __OnItemDragComplete: function (e) {
        var action = e.action, value = e.value, task = e.item;

        //!!!处理条形图拖拽完成
        var gantt = this.gantt;


        if (action == "finish") {
            gantt.setTaskModified(task, "Finish");

            task.Finish = maxTime(value);
            if (task.Finish && task.Start) {
                var days = parseInt((task.Finish - task.Start) / (3600 * 24 * 1000));
                task.Duration = days + 1;

                gantt.setTaskModified(task, "Duration");
            }
        }
        if (action == "percentcomplete") {
            gantt.setTaskModified(task, "PercentComplete");

            task.PercentComplete = value;

        }
        if (action == "move") {
            gantt.setTaskModified(task, "Start");

            task.Start = clearTime(value);

            if (task.Start) {
                task.Finish = maxTime(task.Start);
                task.Finish.setDate(task.Start.getDate() + task.Duration - 1);

                gantt.setTaskModified(task, "Finish");
            }
        }

        gantt.refresh();
    }
};