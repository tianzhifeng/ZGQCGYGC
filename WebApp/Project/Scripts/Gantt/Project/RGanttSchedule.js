/*
GanttSchedule：甘特图功能逻辑模块
描述：监听处理单元格编辑，以及条形图拖拽事件。
*/

function clearTime(date) {
    if (!date) return null;
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}
function maxTime(date) {
    if (!date) return null;
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), 23, 59, 59);
}

function GetDateDiff(startTime, endTime, diffType) {
    diffType = diffType.toLowerCase();
    var sTime = clearTime(startTime);  //开始时间
    var eTime = clearTime(endTime);  //结束时间
    var divNum = 1;
    switch (diffType) {
        case "second":
            divNum = 1000;
            break;
        case "minute":
            divNum = 1000 * 60;
            break;
        case "hour":
            divNum = 1000 * 60 * 60;
            break;
        case "day":
            divNum = 1000 * 60 * 60 * 24;
            break;
    }
    return parseInt((eTime.getTime() - sTime.getTime()) / parseInt(divNum));
}

function minTime(date) {
    if (!date) return null;
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), 00, 00, 00);
}

RGanttSchedule = function (gantt) {
    this.gantt = gantt;
    //处理单元格编辑，和条形图拖拽事件
    gantt.on("cellbeginedit", this.__OnCellBeginEdit, this);
    gantt.on("aftercellcommitedit", this.__OnCellCommitEdit, this);
    gantt.on("itemdragstart", this.__OnItemDragStart, this);
    gantt.on("itemdragcomplete", this.__OnItemDragComplete, this);
    gantt.on("taskdragdrop", this.__OnTaskDragDrop, this);
}
RGanttSchedule.prototype = {
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
                case "Start":
                    task.Start = clearTime(value);
                    if (task.Milestone != 1) {
                        if (task.Start && task.Finish) {
                            if (task.Start >= task.Finish) {
                                task.Start = minTime(task.Finish);
                            }
                            gantt.setTaskModified(task, "Finish");
                        }
                    }
                    else {
                        task.Finish = maxTime(task.Start);
                    }
                    break;
                case "Finish":
                    task.Finish = maxTime(value);
                    gantt.setTaskModified(task, "Finish");
                    if (task.Milestone != 1) {
                        if (task.Finish && task.Start) {
                            if (task.Finish <= task.Start) {
                                task.Finish = maxTime(task.Start);
                            }
                        }
                    }
                    else {
                        task.Start = minTime(task.Finish);
                    }
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
        var task = e.item;
        if (e.action == "percentcomplete") {
            e.cancel = true;
        }
        else if (e.action == "Start" && task.MileStone == 1)
        { e.cancel = true; }
        else if (e.action == "Start" && task.MileStone == 1)
        { e.cancel = true; }
        task.OrlStart = task.Start;
    },
    __OnItemDragComplete: function (e) {
        var action = e.action, value = e.value, task = e.item;
        //!!!处理条形图拖拽完成
        var gantt = this.gantt;
        if (action == "start") {
            if (task.MileStone != 1) {
                gantt.setTaskModified(task, "Start");
                task.Start = minTime(value);
            }
        }

        if (action == "finish") {
            if (task.MileStone != 1) {
                gantt.setTaskModified(task, "Finish");
                task.Finish = maxTime(value);
            }
        }
        if (action == "move") {
            gantt.setTaskModified(task, "Start");
            task.Start = clearTime(value);
            if (task.Start) {
                if (task.Milestone != 1 && task.Duration && task.Duration > 0) {
                    task.Finish = maxTime(task.Start);
                    task.Finish.setDate(task.Start.getDate() + task.Duration - 1);
                    gantt.setTaskModified(task, "Finish");
                }
                else if (task.Milestone != 1 && task.Start && task.OrlStart) {
                    var day = GetDateDiff(task.OrlStart, task.Start, "Day");
                    task.Finish.setDate(task.Finish.getDate() + day);
                    gantt.setTaskModified(task, "Finish");
                }
                else if (task.Milestone && task.Milestone == 1) {
                    task.Finish = maxTime(task.Start);
                    gantt.setTaskModified(task, "Finish");
                }
            }
        }

        gantt.refresh();
    }
};