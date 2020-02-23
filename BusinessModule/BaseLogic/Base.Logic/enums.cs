using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Base.Logic
{

    [Description("角色类型")]
    public enum RoleType
    {
        [Description("系统角色")]
        SysRole,
        [Description("组织角色")]
        OrgRole,
    }

    [Description("资源类型")]
    public enum ResType
    {
        [Description("分类")]
        Menu,
        [Description("数据对象")]
        Data,
        [Description("按钮对象")]
        Button,
        [Description("页面对象")]
        Page,
        [Description("业务对象")]
        Code,
        [Description("字段可见")]
        Field,
        [Description("字段编辑")]
        FieldEdit,
    }

    [Description("数据过滤类型")]
    public enum DataFilterType
    {
        [Description("全部数据")]
        All,
        [Description("用户当前部门数据")]
        OrgID,
        [Description("用户当前部门及子部门数据")]
        OrgAndSubOrgID,
        [Description("用户当前部门及上级部门数据")]
        Dept,
        [Description("用户所属部门数据")]
        DeptIDs,
        [Description("用户所属部门及子部门数据")]
        DeptAndSubDeptID,
        [Description("本公司数据")]
        Company,
        [Description("本项目数据")]
        PrjID,
        [Description("本人数据")]
        CreateUserID,
    }

    [Description("枚举类型")]
    public enum EnumType
    {
        [Description("普通枚举")]
        Normal,
        [Description("表枚举")]
        Table,
    }

    [Description("图表类型")]
    public enum GraphType
    {
        [Description("柱状图")]
        bar,
        [Description("柱状图3D")]
        bar_3D,
        [Description("折线图")]
        line,
        [Description("饼图")]
        pie,
        [Description("饼图3D")]
        pie_3D,
        [Description("仪表盘")]
        gauge,
        [Description("速度仪")]
        speedometer,
        [Description("列表")]
        table,
        [Description("静态块")]
        block,
        [Description("文本块")]
        text,
    }

    [Description("汇总方式")]
    public enum SummaryType
    {
        [Description("汇总")]
        sum,
    }

    [Description("数字格式化")]
    public enum NumberFormat
    {
        [Description("数字格式")]
        thousands,
        [Description("货币格式")]
        currency,
        [Description("百分比格式")]
        percentile,
    }

    [Description("提醒模式")]
    public enum AlarmMode
    {
        [Description("系统消息")]
        Msg,
        [Description("系统提醒")]
        Alarm,
        [Description("电子邮件")]
        Email
    }

    [Description("提醒周期")]
    public enum AlarmFrequency
    {
        [Description("每天一次")]
        EveryDay = 1,
        [Description("每周一次")]
        EveryWeek = 7,
        [Description("每两周一次")]
        EveryTowWeek = 14,
        [Description("每月一次")]
        EveryMonth = 30,
        [Description("每季度一次")]
        EverySeason = 90,
    }

    [Description("列表查询控件")]
    public enum ListItemType 
    {
        [Description("单行文本框")]
        TextBox,
        [Description("日期选择框")]
        DatePicker,
        [Description("多选框列表")]
        CheckBoxList,
        [Description("单选框列表")]
        RadioButtonList,
        [Description("组合下拉框")]
        ComboBox,
        [Description("弹出选择框")]
        ButtonEdit,
        [Description("复选框")]
        CheckBox
    }

    [Description("表单查询控件")]
    public enum FormItemType
    {
        [Description("单行文本框")]
        TextBox,
        [Description("多行文本框")]
        TextArea,
        [Description("富文本编辑框")]
        UEditor,
        [Description("日期选择框")]
        DatePicker,
        [Description("多选框列表")]
        CheckBoxList,
        [Description("单选框列表")]
        RadioButtonList,
        [Description("组合下拉框")]
        ComboBox,
        [Description("弹出选择框")]
        ButtonEdit,
        [Description("单附件上传")]
        SingleFile,
        [Description("多附件上传")]
        MultiFile,
        [Description("流程签字框")]
        AuditSign,
        [Description("复选框")]
        CheckBox,
        [Description("数字输入框")]
        Spinner,
        [Description("链接选择框")]
        LinkEdit,
        [Description("子表")]
        SubTable
    }

    [Description("类型")]
    public enum ImportItemType
    {
        [Description("文本")]
        Text,
        [Description("日期")]
        Date,
        [Description("枚举")]
        Enum,
        [Description("转换")]
        Convert
    }
}
