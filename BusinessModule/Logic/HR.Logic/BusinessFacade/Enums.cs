using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace HR.Logic.BusinessFacade
{
    [Description("历程类型")]
    public enum enumCourseType
    {
        [Description("职称变化")]
        AcademicTitle,
        [Description("岗位变化")]
        WorkPost,
        [Description("项目经历")]
        WorkPerformance,
    }

    [Description("员工状态")]
    public enum EmployeeState
    {
        [Description("在职")]
        Incumbency,
        [Description("离职")]
        Dimission,
        [Description("退休")]
        Retire,
        [Description("返聘")]
        ReEmploy,
        [Description("返聘离职")]
        ReEmployDimission,
    }

    [Description("用工形式")]
    public enum EmploymentWay
    {
        [Description("正式员工")]
        正式员工,
        [Description("外聘员工")]
        外聘员工
    }

    [Description("资质借用归还状态")]
    public enum QualificationReturnState
    {
        [Description("未归还")]
        未归还,
        [Description("已归还")]
        已归还
    }


    /// <summary>
    /// 该枚举在多处用于控制逻辑，修改时请注意
    /// </summary>
    [Description("考评类型")]
    public enum ProCheckCategory
    {
        [Description("个人考评")]
        Personal,
        [Description("部门考评")]
        Dept,

    }
    /// <summary>
    /// 工资发放状态
    /// </summary>
    [Description("工资发放状态")]
    public enum WagesGrantState
    {
        [Description("未发放")]
        UnGrant,
        [Description("已发放")]
        Granted,

    }

    /// <summary>
    /// TrueOrFalse
    /// </summary>
    [Description("TrueOrFalse")]
    public enum TrueOrFalse
    {
        [Description("True")]
        True,
        [Description("False")]
        False,

    }

    /// <summary>
    /// 证书状态
    /// </summary>
    [Description("证书状态")]
    public enum CertificateState
    {
        [Description("未借出")]
        未借出,
        [Description("已借出")]
        已借出,
        [Description("已遗失")]
        已遗失,
        [Description("已作废")]
        已作废
    }
}
