

function loadData(onLoad) {
    var task = gantt.getSelected();
    if (!versionID) return;
    addExecuteParam("VersionID", versionID);
    gantt.loading();
    execute("GetGantteTree", {
        formId: "none",
        refresh: false, onComplete: function (data) {
            var list = mini.arrayToTree(data, "children", "UID", "ParentTaskUID");
            gantt.loadTasks(list);
            gantt.unmask();
            if (task) {
                $.each(data,
                    function (i, v) {
                        if (v.ID === task.ID) {
                            gantt.select(v);
                            gantt.scrollIntoView(v);
                            return false;
                        }
                    });
            } else {
                if (data.length) {
                    gantt.select(data[0]);
                    gantt.fire('taskclick', { task: data[0] });
                }
            }
            gantt.filter(function (tk) {
                if (tk.Visible != 1) return false;
                return true;
            });
            if (onLoad) {
                onLoad(list);
            }
        }, validateForm: false
    });
}

function zoomIn() {
    gantt.zoomIn();
}

function zoomOut() {
    gantt.zoomOut();
}

function initGantt() {
    var gantt = new PlusGantt();
    gantt.setStyle("width:100%;height:100%;border:0px;");
    new RGanttSchedule(gantt);
    gantt.setAllowResize(false);
    gantt.setMultiSelect(true);
    gantt.setAllowDragDrop(true);
    //gantt.setViewModel("track");
    gantt.setTopTimeScale("year");
    gantt.setBottomTimeScale("month");
    gantt.setTimeLines([{ date: new Date(), text: "今日", style: "width:2px;background:red;" }]);
    var firstLevelVisible = true;
    gantt.setColumns([
        { header: "", field: "ID", width: 25, cellCls: "mini-indexcolumn", align: "center", allowDrag: false, allowMove: false, visible: false },
        { header: "", field: "NODETYPE", width: 15, cellCls: "mini-checkcolumn", align: "center", allowDrag: false, allowMove: false, visible: false },
         new StatusColumn(),
        {
            header: "作业代码", field: "Code", width: 60, headeralign: "center", visible: false,
            editor: {
                type: "textbox"
            }
        },
        {
            header: "作业名称", field: "Name", width: 250, name: "taskname", headeralign: "center",
            editor: {
                type: "textbox"
            }
        },
        {
            header: "负责人", field: "ChargerUserName", width: 60, headeralign: "center",
            editor: {
                type: "textbox"
            }
        },
        {
            header: "计划开始", field: "Start", width: 70, dateformat: "yyyy-MM-dd", align: "center", editor: {
                type: "datepicker"
            }
        },
        {
            header: "计划完成", field: "Finish", width: 70, dateformat: "yyyy-MM-dd", align: "center"
            , editor: {
                type: "datepicker"
            }
        },
         {
             header: "工期", field: "Duration", width: 35, align: "right", visible: firstLevelVisible,
             editor: {
                 type: "textbox"
             }
         },
          {
              header: "权重", field: "Weight", width: 35, align: "right",
              editor: {
                  type: "textbox"
              }
          },
         {
             header: "实际开始", field: "FactStart", width: 60, dateformat: "yyyy-MM-dd", align: "center", visible: firstLevelVisible,
         },
          {
              header: "实际完成", field: "FactFinish", width: 60, dateformat: "yyyy-MM-dd", align: "center",
          }
    ]);
    //设置节点列
    gantt.setTreeColumn("taskname");
    return gantt;
}

function changeTopTimeScale(value) {
    gantt.setTopTimeScale(value)
}

function changeBottomTimeScale(value) {
    gantt.setBottomTimeScale(value)
}

function changeTopTimeAndBottomTimeScale(value) {
    var TopTime = value.split(",")[0];
    var BottomTime = value.split(",")[1];
    gantt.setTopTimeScale(TopTime);
    gantt.setBottomTimeScale(BottomTime);

}

function changeCollapse(value) {
    gantt.expandAll();
    gantt.collapseLevel(value);
}