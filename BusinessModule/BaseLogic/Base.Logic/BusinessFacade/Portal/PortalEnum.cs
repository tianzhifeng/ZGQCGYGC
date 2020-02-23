using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Base.Logic
{
    [Description("Portal类型")]
    public enum PortalType
    {
        [Description("现门户块")]
        Now,
        [Description("连接页面")]
        Link,
        [Description("高级定义")]
        Default,
    }

    [Description("新门户Portal类型")]
    public enum NewPortalType
    {
        [Description("普通新闻")]
        Normal,
        [Description("图片新闻")]
        Image
    }

    [Description("默认块")]
    public enum DefaultPortal
    {
        [Description("我的任务")]
        board1,
        [Description("我的消息")]
        board7,
        [Description("待办任务")]
        Task,
        [Description("公司新闻")]
        board6,
        [Description("图片新闻")]
        board2,
        [Description("个人头像")]
        board5,
        [Description("设计任务")]
        board3,
        [Description("通知公告")]
        board8,
        [Description("项目公告")]
        board9,
        [Description("部门通讯录")]
        MailList,
        [Description("友情链接")]
        board10,
        [Description("天气预报")]
        board4
    }

    [Description("显示类型")]
    public enum DisplayType
    {
        [Description("列表(序号)")]
        List,
        [Description("列表(摘要)")]
        Block,
        [Description("列表(头像)")]
        Head
    }
}
