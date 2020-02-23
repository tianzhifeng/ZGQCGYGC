var RGanttTaskMenu = function () {
    RGanttTaskMenu.superclass.constructor.call(this);

}


mini.extend(RGanttTaskMenu, mini.Menu, {
    _create: function () {
        RGanttTaskMenu.superclass._create.call(this);
        var menuItems = [
            { type: "menuitem", iconCls: "icon-ok", text: "完成", name: "split" },
            { type: "menuitem", iconCls: "icon-edit", text: "变更", name: "edit" },
            { type: "menuitem", iconCls: "", text: mini.Gantt.Deselect_Text, name: "deselect" }
        ];
        this.setItems(menuItems);

        this.split = mini.getbyName("split", this);
        this.edit = mini.getbyName("edit", this);
        this.deselect = mini.getbyName("deselect", this);

        this.split.on("click", this.__OnComplete, this);
        this.edit.on("click", this.__OnEdit, this);
        this.deselect.on("click", this.__OnDeSelect, this);


        this.on("opening", this.__OnOpening, this);
    },
    __OnOpening: function (e) {
        var gantt = this.owner;
        var tasks = gantt.getSelectedTasks();

        this.edit.hide();
        this.split.hide();
        if (tasks.length == 1) {
            this.split.show(); //仅当选中一个任务时，才显示拆分项
            this.edit.show(); //仅当选中一个任务时，才可以点击编辑项
        }
    },
    __OnComplete: function (e) {
        var gantt = this.owner;
        var task = gantt.getSelectedTask();
        if (task) {
            var id = task.UID;
            var url = "/Project/Extend/MileStoneExecute/MileStoneComplete?ID=" + id;
            openWindow(url, { width: 700, height: 400, onDestroy: function () {
                loadData();
            }
            });
        } else {
            alert("请选择一个任务");
        }
    },
    __OnEdit: function (e) {
        var gantt = this.owner;
        var task = gantt.getSelectedTask();
        if (task) {
            if (task.State == "Finish") { msgUI("已经完成的里程碑不能进行变更"); return; }
            var id = task.UID;
            var url = "/Project/Extend/MileStoneExecute/MileStoneChange?MileStoneID=" + id;
            openWindow(url, { width: 700, height: 400, onDestroy: function () {
                loadData();
            }
            });
        } else {
            alert("请选择一个任务");
        }
    },
    __OnDeSelect: function (e) {
        var gantt = this.owner;
        var tasks = gantt.getSelectedTasks();
        gantt.deselectTasks(tasks);
    }

});


var RGanttContextMenu = function () {
    RGanttContextMenu.superclass.constructor.call(this);

}
mini.extend(RGanttContextMenu, mini.Menu, {
    _create: function () {
        RGanttTaskMenu.superclass._create.call(this);

        var menuItems = [
            { type: "menuitem", icon: "icon-zoomin", text: mini.Gantt.ZoomIn_Text, name: "zoomin" },
            { type: "menuitem", icon: "icon-zoomout", text: mini.Gantt.ZoomOut_Text, name: "zoomout" }
        ];
        this.setItems(menuItems);

        this.zoomIn = mini.getbyName("zoomin", this);
        this.zoomOut = mini.getbyName("zoomout", this);

        this.zoomIn.on("click", this.__OnZoomIn, this);
        this.zoomOut.on("click", this.__OnZoomOut, this);
    },
    __OnZoomIn: function (e) {
        var gantt = this.owner;
        gantt.zoomIn();
    },
    __OnZoomOut: function (e) {
        var gantt = this.owner;
        gantt.zoomOut();
    }

});