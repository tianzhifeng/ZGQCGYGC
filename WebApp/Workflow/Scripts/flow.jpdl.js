(function ($) {
    var myflow = $.myflow;

    $.extend(true, myflow.config.rect, {
        attr: {
            r: 8,
            fill: '#ffffff',
            stroke: '#000000',
            "stroke-width": 1
        }
    });

    $.extend(true, myflow.config.props.props, {
        name: { name: 'name', label: '名称', value: '新建流程', editor: function () { return new myflow.editors.inputEditor(); } },
        key: { name: 'key', label: '标识', value: '', editor: function () { return new myflow.editors.inputEditor(); } },
        desc: { name: 'desc', label: '描述', value: '', editor: function () { return new myflow.editors.inputEditor(); } }
    });


    $.extend(true, myflow.config.tools.states, {
        start: {
            showType: 'image&text',
            type: 'start',
            name: { text: '' },
            text: { text: '开始' },
            img: { src: '/Workflow/Scripts/Images/start.png', width: 48, height: 48 },
            attr: { width: 48, heigth: 48 },
            props: {
                text: { name: 'text', label: '显示', value: '', editor: function () { return new myflow.editors.textEditor(); }, value: '开始' },
                temp1: { name: 'temp1', label: '文本', value: '', editor: function () { return new myflow.editors.inputEditor(); } },
                temp2: { name: 'temp2', label: '选择', value: '', editor: function () { return new myflow.editors.selectEditor([{ name: 'aaa', value: 1 }, { name: 'bbb', value: 2 }]); } }
            }
        },
        'start-active': {
            showType: 'image&text', type: 'start-active',
            name: { text: '<<start-active>>' },
            text: { text: '审批中' },
            img: { src: '/Workflow/Scripts/Images/start.png', width: 48, height: 48 },
            props: {
                text: { name: 'text', label: '显示', value: '', editor: function () { return new myflow.editors.textEditor(); }, value: '错误' },
                temp1: { name: 'temp1', label: '文本', value: '', editor: function () { return new myflow.editors.inputEditor(); } },
                temp2: { name: 'temp2', label: '选择', value: '', editor: function () { return new myflow.editors.selectEditor([{ name: 'aaa', value: 1 }, { name: 'bbb', value: 2 }]); } }
            }
        },
        end: {
            showType: 'image&text', type: 'end',
            name: { text: '<<end>>' },
            text: { text: '结束' },
            img: { src: '/Workflow/Scripts/Images/end.png', width: 48, height: 48 },
            attr: { width: 50, heigth: 63 },
            props: {
                text: { name: 'text', label: '显示', value: '', editor: function () { return new myflow.editors.textEditor(); }, value: '结束' },
                temp1: { name: 'temp1', label: '文本', value: '', editor: function () { return new myflow.editors.inputEditor(); } },
                temp2: { name: 'temp2', label: '选择', value: '', editor: function () { return new myflow.editors.selectEditor([{ name: 'aaa', value: 1 }, { name: 'bbb', value: 2 }]); } }
            }
        },
        'end-state': {
            showType: 'image&text', type: 'state',
            name: { text: '<<end>>' },
            text: { text: '结束' },
            img: { src: '/Workflow/Scripts/Images/end-state.png', width: 48, height: 48 },
            attr: { width: 50, heigth: 63 },
            props: {
                text: { name: 'text', label: '显示', value: '', editor: function () { return new myflow.editors.textEditor(); }, value: '结束' },
                temp1: { name: 'temp1', label: '文本', value: '', editor: function () { return new myflow.editors.inputEditor(); } },
                temp2: { name: 'temp2', label: '选择', value: '', editor: function () { return new myflow.editors.selectEditor([{ name: 'aaa', value: 1 }, { name: 'bbb', value: 2 }]); } }
            }
        },
        state: {
            showType: 'image&text', type: 'state',
            name: { text: '<<state>>' },
            text: { text: '状态' },
            img: { src: '/Workflow/Scripts/Images/task_empty.png', width: 48, height: 70 },
            props: {
                text: { name: 'text', label: '显示', value: '', editor: function () { return new myflow.editors.textEditor(); }, value: '状态' },
                temp1: { name: 'temp1', label: '文本', value: '', editor: function () { return new myflow.editors.inputEditor(); } },
                temp2: { name: 'temp2', label: '选择', value: '', editor: function () { return new myflow.editors.selectEditor([{ name: 'aaa', value: 1 }, { name: 'bbb', value: 2 }]); } }
            }
        },
        active: {
            showType: 'image&text', type: 'active',
            name: { text: '<<active>>' },
            text: { text: '审批中' },
            img: { src: '/Workflow/Scripts/Images/task.png', width: 48, height: 48 },
            props: {
                text: { name: 'text', label: '显示', value: '', editor: function () { return new myflow.editors.textEditor(); }, value: '分支' },
                temp1: { name: 'temp1', label: '文本', value: '', editor: function () { return new myflow.editors.inputEditor(); } },
                temp2: { name: 'temp2', label: '选择', value: '', editor: function () { return new myflow.editors.selectEditor('select.json'); } }
            }
        },
        history: {
            showType: 'image&text', type: 'history',
            name: { text: '<<history>>' },
            text: { text: '历史' },
            img: { src: '/Workflow/Scripts/Images/approve.png', width: 48, height: 48 },
            props: {
                text: { name: 'text', label: '显示', value: '', editor: function () { return new myflow.editors.textEditor(); }, value: '合并' },
                temp1: { name: 'temp1', label: '文本', value: '', editor: function () { return new myflow.editors.inputEditor(); } },
                temp2: { name: 'temp2', label: '选择', value: '', editor: function () { return new myflow.editors.selectEditor('select.json'); } }
            }
        },
        task: {
            showType: 'image&text', type: 'task',
            name: { text: '<<task>>' },
            text: { text: '任务' },
            img: { src: '/Workflow/Scripts/Images/not-approve.png', width: 48, height: 48 },
            props: {
                text: { name: 'text', label: '显示', value: '', editor: function () { return new myflow.editors.textEditor(); }, value: '任务' },
                assignee: { name: 'assignee', label: '用户', value: '', editor: function () { return new myflow.editors.inputEditor(); } },
                form: { name: 'form', label: '表单', value: '', editor: function () { return new myflow.editors.inputEditor(); } },
                desc: { name: 'desc', label: '描述', value: '', editor: function () { return new myflow.editors.inputEditor(); } }
            }
        }
    });
})(jQuery);