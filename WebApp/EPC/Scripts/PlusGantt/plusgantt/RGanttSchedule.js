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

Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
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
    gantt.on("tasksync", function (e) {
        this._syncTask(e.task);
    }, this);

}
RGanttSchedule.prototype = {
    __OnTaskDragDrop: function (e) {
        e.cancel = true;
        var dragRecords = e.tasks, targetRecord = e.targetTask, action = e.action;
        this.gantt.moveTasks(dragRecords, targetRecord, action);
    },
    __OnCellBeginEdit: function (e) {
        if (funcType && funcType.toLowerCase() == "view" || (flowEnd && flowEnd == "True")) {
            e.cancel = true; return;
        }
        var task = e.record, field = e.field;
        if (task.CanEdit == "0") { e.cancel = true; return; }
        if (task.Milestone == 1 && (field == 'Start' || field == 'Duration')) { e.cancel = true; return; }
        if (field == "FactStart" || field == "FactEnd") { e.cancel = true; return; }
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

            var ancestors = gantt.getAncestorTasks(task);
            var miniDate; var maxDate;
            for (var i = 0; i < ancestors.length; i++) {
                var ancestor = ancestors[i];
                if (ancestor.IsLocked == "True" || ancestor.IsLocked == "1") {
                    if (!miniDate || (ancestor.Start && miniDate < ancestor.Start)) {
                        miniDate = ancestor.Start;
                    }
                    if (!maxDate || (ancestor.Finish && maxDate > ancestor.Finish)) {
                        maxDate = ancestor.Finish;
                    }
                }
            }

            switch (field) {
                case "Duration":
                    task.Duration = parseInt(value);
                    if (isNaN(task.Duration))
                    { task.Duration = 0; break; }
                    if (task.Start) {
                        task.Finish = maxTime(task.Start);
                        if (task.CALENDAR !== "Normal") {
                            task.Finish.setDate(task.Finish.getDate() + task.Duration - 1);
                        } else {
                            task.Finish = task.Finish.addDaysSkipDayOff(task.Duration - 1);
                        }
                        gantt.setTaskModified(task, "Finish");
                    }
                    break;
                case "Start":
                    if (miniDate && miniDate > e.value && e.value) {
                        msgUI("【" + task.Name + "】完成时间不能小于" + mini.formatDate(miniDate, "yyyy-MM-dd"));
                        gantt.refresh(); return;
                    }
                    if (task.Milestone) { return; }
                    task.Start = clearTime(value);
                    if (task.Start) {
                        task.Finish = maxTime(task.Start);
                        if (task.Duration > 0) {
                            if (task.CALENDAR !== "Normal") {
                                task.Finish.setDate(task.Finish.getDate() + task.Duration - 1);
                            } else {
                                task.Finish = task.Finish.addDaysSkipDayOff(task.Duration - 1);
                            }
                        }
                        else {
                            task.Duration = 1;
                        }
                        gantt.setTaskModified(task, "Finish");
                    }
                    break;
                case "Finish":
                    if (maxDate && maxDate < e.value && e.value) {
                        msgUI("【" + task.Name + "】完成时间不能大于" + mini.formatDate(maxDate, "yyyy-MM-dd"));
                        gantt.refresh(); return;
                    }
                    task.Finish = maxTime(value);
                    if (task.Milestone) {
                        task.Start = minTime(task.Finish);
                    }
                    else if (task.Finish && task.Start) {
                        var days = parseInt((task.Finish - task.Start) / (3600 * 24 * 1000));
                        if (task.CALENDAR === "Normal") {
                            days = task.Start.betweenWorkDay(task.Finish) - 1;
                        }
                        task.Duration = days + 1;
                        gantt.setTaskModified(task, "Duration");
                    }
                    break;
                default:
                    task[field] = value;
                    break;
            }
            gantt.refresh();
            this._syncTask(task);
            gantt.refresh();
        } catch (ex) {
            alert(ex.message);
        }
    },
    __OnItemDragStart: function (e) {
        if (funcType && funcType.toLowerCase() == "view" || (flowEnd && flowEnd == "True")) {
            e.cancel = true; return;
        }
        if (e.action == "start") {
            e.cancel = true;
        }
        if (e.action == "percentcomplete") {
            e.cancel = true;
        }
    },
    __OnItemDragComplete: function (e) {
        var action = e.action, value = e.value, task = e.item;
        //!!!处理条形图拖拽完成
        var gantt = this.gantt;
        if (action == "finish") {
            //校验是否有上层节点限制

            var ancestors = gantt.getAncestorTasks(task);
            var miniDate; var maxDate;
            for (var i = 0; i < ancestors.length; i++) {
                var ancestor = ancestors[i];
                if (ancestor.IsLocked == "True" || ancestor.IsLocked == "1") {
                    if (!miniDate || (ancestor.Start && miniDate < ancestor.Start)) {
                        miniDate = ancestor.Start;
                    }
                    if (!maxDate || (ancestor.Finish && maxDate > ancestor.Finish)) {
                        maxDate = ancestor.Finish;
                    }
                }
            }
            if (maxDate && maxDate < e.value && e.value) {
                msgUI("【" + task.Name + "】完成时间不能大于" + mini.formatDate(maxDate, "yyyy-MM-dd"));
                gantt.refresh(); return;
            }
            gantt.setTaskModified(task, "Finish");
            task.Finish = maxTime(value);
            if (task.Finish && task.Start) {
                var days = parseInt((task.Finish - task.Start) / (3600 * 24 * 1000));
                if (task.CALENDAR === "Normal") {
                    days = task.Start.betweenWorkDay(task.Finish) - 1;
                }
                task.Duration = days + 1;
                gantt.setTaskModified(task, "Duration");
            }

        }
        if (action == "percentcomplete") {
            gantt.setTaskModified(task, "PercentComplete");
            task.PercentComplete = value;
        }
        if (action == "move") {
            var ancestors = gantt.getAncestorTasks(task);
            var miniDate; var maxDate;
            for (var i = 0; i < ancestors.length; i++) {
                var ancestor = ancestors[i];
                if (ancestor.IsLocked == "True" || ancestor.IsLocked == "1") {
                    if (!miniDate || (ancestor.Start && miniDate < ancestor.Start)) {
                        miniDate = ancestor.Start;
                    }
                    if (!maxDate || (ancestor.Finish && maxDate > ancestor.Finish)) {
                        maxDate = ancestor.Finish;
                    }
                }
            }
            if (miniDate && miniDate > e.value && e.value) {
                msgUI("【" + task.Name + "】完成时间不能小于" + mini.formatDate(miniDate, "yyyy-MM-dd"));
                gantt.refresh(); return;
            }
            var finish = maxTime(e.value);
            finish.setDate(finish.getDate() + task.Duration - 1);
            if (maxDate && finish > maxDate) {
                msgUI("【" + task.Name + "】完成时间不能大于" + mini.formatDate(maxDate, "yyyy-MM-dd"));
                gantt.refresh(); return;
            }
            gantt.setTaskModified(task, "Start");
            task.Start = clearTime(value);
            if (task.Milestone) {
                task.Finish = maxTime(task.Start);
                gantt.setTaskModified(task, "Finish");
            }
            else if (task.Start) {
                task.Finish = maxTime(task.Start);
                if (task.CALENDAR != "Normal") {
                    task.Finish.setDate(task.Finish.getDate() + task.Duration - 1);
                } else {
                    task.Finish = task.Finish.addDaysSkipDayOff(task.Duration - 1);
                }
                gantt.setTaskModified(task, "Finish");
            }
        }
        gantt.refresh();
        this._syncTask(task);
        gantt.refresh();
    },

    _syncTask: function (task) {
        if (!task) return;
        var gantt = this.gantt;

        //同步任务的父任务工期
        function getDuration(pnode) {
            var duration = 0;
            if (pnode.Start && pnode.Finish) {
                var days = parseInt((pnode.Finish - pnode.Start) / (3600 * 24 * 1000));
                if (task.CALENDAR == "Normal") {
                    days = pnode.Start.betweenWorkDay(pnode.Finish) - 1;
                }
                duration = days + 1;
            }
            return duration;
        }

        //1)获取所有父级任务
        var ans = gantt.getAncestorTasks(task);
        ans.reverse();
        for (var i = 0, l = ans.length; i < l; i++) {
            var t = ans[i];
            //2)获取父任务下子任务的Duration之和
            var duration = getDuration(t);
            if (t.Duration != duration && t.IsLocked != "True") {
                t.Duration = duration;
                gantt.setTaskModified(t, "Duration");
            }
        }
    }
};


StatusColumn = function (optons) {
    return mini.copyTo({
        name: "Status",
        width: 20,
        header: '<div class="mini-gantt-taskstatus"></div>',
        formatDate: function (date) {
            return date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
        },
        renderer: function (e) {
            var record = e.record;
            var s = "";
            if (record.PercentComplete == 100) {
                var t = record.Finish ? "任务完成于 " + this.formatDate(record.Finish) : "";
                s += '<div class="mini-gantt-finished" title="' + t + '"></div>';
            }
            if (record.Summary && record.FixedDate) {

                var t = "此任务固定日期，从开始日期 " + this.formatDate(record.Start)
                        + " 到完成日期 " + this.formatDate(record.Finish);
                s += '<div class="mini-gantt-constraint3" title=\'' + t + '\'></div>';
            } else if (record.ConstraintType >= 2 && mini.isDate(record.ConstraintDate)) {
                var ct = mini.Gantt.ConstraintType[record.ConstraintType];
                if (ct) {
                    var ctype = ct.Name;
                    var t = "此任务有 " + ct.Name + " 的限制，限制日期 " + this.formatDate(record.ConstraintDate);
                    s += '<div class="mini-gantt-constraint' + record.ConstraintType + '" title=\'' + t + '\'></div>';
                }
            }
            if (record.Milestone) {
                s += '<div class="mini-gantt-milestone-red" title="里程碑"></div>';
            }
            if (record.Notes) {
                var t = '备注：' + record.Notes;
                s += '<div class="mini-gantt-notes" title="' + t + '"></div>';
            }
            if (record.Conflict == 1) {
                var t = "此任务排程有冲突，如有必要，请适当调整";
                s += '<div class="mini-gantt-conflict" title="' + t + '"></div>';
            }
            return s;
        }
    }, optons);
}