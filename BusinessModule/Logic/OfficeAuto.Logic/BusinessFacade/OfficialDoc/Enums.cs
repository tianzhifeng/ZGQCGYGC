using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OfficeAuto.Logic
{
    [Description("收文状态")]
    public enum IncomingStatus
    {
        [Description("登记")]
        Register,
        [Description("拟办")]
        Propose,
        [Description("批办")]
        Approve,
        [Description("承办")]
        Undertake,
        [Description("完成")]
        Complete,
        [Description("登记人处理")]
        Transfer,
    }

    [Description("收文密级")]
    public enum IncomingMiJi
    {
        [Description("一般")]
        YiBan,
        [Description("秘密")]
        MiMi,
        [Description("机密")]
        JiMi,
        [Description("绝密")]
        JueMi,
    }

    [Description("收文紧急")]
    public enum IncomingHuanJi
    {
        [Description("一般")]
        YiBan,
        [Description("急件")]
        JiJian,
    }

    [Description("发文状态")]
    public enum PostingStatus
    {
        [Description("拟稿")]
        Draft,
        [Description("核稿")]
        Audit,
        [Description("部门会签")]
        DeptCountersign,
        [Description("中转")]
        Transfer,
        [Description("预审稿")]
        WorksSecretary,
        [Description("审稿")]
        Review,
        [Description("领导会签")]
        LeaderCountersign,
        [Description("签发")]
        LeaderIssue,
        [Description("校对套红")]
        TaoHong,
        [Description("登记印发")]
        Print,
        [Description("完成")]
        Complete,
        [Description("终止")]
        Stop,
    }

    [Description("发文缓急")]
    public enum PostingHuanJi
    {
        [Description("一般")]
        YiBan,
        [Description("急件")]
        JiJian,
        [Description("特急")]
        TeJi,
    }

    [Description("发文密级")]
    public enum PostingMiJi
    {
        [Description("一般")]
        YiBan,
        [Description("秘密")]
        MiMi,
        [Description("机密")]
        JiMi,
        [Description("绝密")]
        JueMi,
    }

    [Description("批语类型")]
    public enum CommentType
    {
        [Description("收文")]
        Incoming,
        [Description("发文")]
        Posting,
    }

    [Description("传真主题")]
    public enum FaxTheme
    {
        [Description("紧急")]
        JinJi,
        [Description("请审阅")]
        QingShenYue,
        [Description("请批注")]
        QingPiZhu,
        [Description("请回复")]
        QingHuiFu,
        [Description("请传阅")]
        QingChuanYue,
    }

    [Description("发传真状态")]
    public enum SendFaxStatus
    {
        [Description("填写")]
        Register,
        [Description("审批中")]
        Processing,
        [Description("已通过")]
        End,
    }

    [Description("便函流程状态")]
    public enum MemoStatus
    {
        [Description("填写")]
        Start,
        [Description("审批中")]
        Processing,
        [Description("已通过")]
        End,
    }
}
