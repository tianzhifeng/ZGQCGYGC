using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OfficeAuto.Logic
{

    /// <summary>
    /// 文档类型
    /// </summary>
    [Description("文档类型")]
    public enum enumDBSType
    {
        /// <summary>
        /// 个人
        /// </summary>
        [Description("个人")]
        Personal,
        /// <summary>
        /// 一级
        /// </summary>
        [Description("根")]
        Root,
        /// <summary>
        /// 公司
        /// </summary>
        [Description("公司")]
        Company,
        /// <summary>
        /// 部门
        /// </summary>
        [Description("部门")]
        Dept,
        /// <summary>
        /// 结构
        /// </summary>
        [Description("结构")]
        Struct,
    }

    /// <summary>
    /// 组或用户类型
    /// </summary>
    [Description("组或用户类型")]
    public enum enumGroupUserType
    {
        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        User,
        /// <summary>
        /// 组织
        /// </summary>
        [Description("组织")]
        Org,
        /// <summary>
        /// 组织角色
        /// </summary>
        [Description("组织角色")]
        OrgRole,
        /// <summary>
        /// 组织角色
        /// </summary>
        [Description("系统角色")]
        SysRole,
    }

    [Description("控制类型")]
    public enum enumAuthType
    {
        [Description("完全控制")]
        FullControl,
        [Description("只读权限")]
        ReadOnly,
        [Description("共享权限")]
        Share,
        [Description("无权限")]
        None
    }

    /// <summary>
    /// 接待住宿用房和包厢管理
    /// </summary>
    [Description("接待住宿用房和包厢管理")]
    public enum enumRoomType
    {
        /// <summary>
        /// 是
        /// </summary>
        [Description("住宿")]
        Stay,
        /// <summary>
        /// 否
        /// </summary>
        [Description("包厢")]
        Box,

    }

    /// <summary>
    /// 维修状态
    /// </summary>
    [Description("维修状态")]
    public enum enumAssetsRepairStatus
    {
        /// <summary>
        /// 编制
        /// </summary>
        [Description("编制")]
        BianZhi,
        /// <summary>
        ///  审批
        /// </summary>
        [Description(" 审批 ")]
        ShenPi,
        /// <summary>
        ///  审批
        /// </summary>
        [Description(" 打回 ")]
        DaHui,

        /// <summary>
        ///  审批
        /// </summary>
        [Description(" 待送修 ")]
        DaiSongXiu,

        /// <summary>
        ///  审批
        /// </summary>
        [Description(" 完成 ")]
        WanCheng,


    }

    /// <summary>
    /// 资产状态
    /// </summary>
    [Description("资产状态")]
    public enum enumAssetsStatus
    {
        /// <summary>
        /// 空闲
        /// </summary>
        [Description("空闲")]
        KongXian,
        /// <summary>
        ///  使用
        /// </summary>
        [Description(" 使用 ")]
        ShiYong,
        /// <summary>
        ///  维修
        /// </summary>
        [Description("  维修  ")]
        WeiXiu,

        /// <summary>
        ///  报废
        /// </summary>
        [Description(" 报废 ")]
        BaoFei,

        /// <summary>
        ///  对外投资
        /// </summary>
        [Description(" 对外投资 ")]
        DuiWaiTZ,


        /// <summary>
        ///  出售
        /// </summary>
        [Description(" 出售 ")]
        ChuShou,

        /// <summary>
        ///  出租出借
        /// </summary>
        [Description(" 出租出借 ")]
        ZuJie,

        /// <summary>
        ///  抵押担保
        /// </summary>
        [Description(" 抵押担保 ")]
        DiYa,
        /// <summary>
        ///  无偿调拨
        /// </summary>
        [Description(" 无偿调拨 ")]
        DiaoBo

    }


    /// <summary>
    /// 节假日安排车状态
    /// </summary>
    [Description("节假日安排车状态")]
    public enum CarPlanStatus
    {
        /// <summary>
        /// 登记
        /// </summary>
        [Description("登记")]
        Register,
        /// <summary>
        ///  已提交
        /// </summary>
        [Description("已提交")]
        Submit,
        /// <summary>
        ///  安排
        /// </summary>
        [Description("安排")]
        Plan
    }

    [Description("资料柜权限")]
    public enum DocumentCabinetsAuthType
    {
        [Description("完全控制")]
        FullControl,
        [Description("操作权限")]
        CanWrite,
        [Description("只读权限")]
        ReadOnly,
        [Description("无权限")]
        None
    }

    [Description("单身宿舍入住状态")]
    public enum AccommodationRegistState
    {
        [Description("已入住")]
        已入住,
        [Description("已退出")]
        已退出
    }
    [Description("单身宿舍结算状态")]
    public enum AccommodationAccountState
    {
        [Description("未提交")]
        未提交,
        [Description("已提交")]
        已提交,
        [Description("未结算")]
        未结算,
        [Description("已结算")]
        已结算
    }

    [Description("会议召开状态")]
    public enum ConferenceState
    {
        [Description("未召开")]
        未召开,
        [Description("已召开")]
        已召开
    }

    [Description("领导信箱使用状态")]
    public enum MailBoxState
    {
        [Description("正常使用")]
        正常使用,
        [Description("暂停使用")]
        暂停使用
    }

    public enum LoanAccountType
    {
        [Description("借款")]
        借款,
        [Description("还款")]
        还款
    }

    public enum ReimbursementClass
    {
        [Description("报销")]
        报销,
        [Description("冲账")]
        冲账
    }

    public enum BudgetType
    {
        [Description("党费预算")]
        Party,
        [Description("工会经费预算")]
        Union
    }
}
