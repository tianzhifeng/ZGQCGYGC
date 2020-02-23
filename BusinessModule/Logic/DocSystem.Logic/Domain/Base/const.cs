using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DocSystem.Logic.Domain
{
    public enum TrueOrFalse
    {

        /// <summary>
        /// 是
        /// </summary>
        [Description("是")]
        True,

        /// <summary>
        /// 否
        /// </summary>
        [Description("否")]
        False,
    }

    public enum DocState
    {
        /// <summary>
        /// 整编中
        /// </summary>
        [Description("整编中")]
        Normal,

        /// <summary>
        /// 已发布
        /// </summary>
        [Description("已发布")]
        Published,

        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Invalid
    }
    public enum ListConfigType
    {
        [Description("编目")]
        Node,
        [Description("文件")]
        File
    }

    public enum AttrType
    {
        [Description("系统属性")]
        System,
        [Description("自定义属性")]
        Custom
    }

    public enum AttrDataType
    {
        [Description("整形")]
        Int,
        [Description("日期")]
        DateTime,
        [Description("字符50")]
        Varchar50,
        [Description("字符500")]
        Varchar500,
        [Description("字符MAX")]
        VarcharMAX,
        [Description("双字符50")]
        NVarchar50,
        [Description("双字符200")]
        NVarchar200,
        [Description("双字符500")]
        NVarchar500,
        [Description("双字符Max")]
        NVarcharMax,
        [Description("十进制数字")]
        Decimal
    }

    public enum ValidateType
    {
        [Description("无")]
        None,
        [Description("全局唯一")]
        Unique,
        [Description("同级唯一")]
        BortherUnique,
        [Description("同类别唯一")]
        TypeUnique
    }

    public enum QueryType
    {
        [Description("模糊匹配")]
        LK,
        [Description("等于")]
        EQ,
        [Description("大于等于")]
        FR,
        [Description("小于等于")]
        TO,
        [Description("包含")]
        IN,
        [Description("不等于")]
        UE,
        [Description("小于")]
        LT,
        [Description("大于")]
        GT,
        [Description("以...开始")]
        SW,
        [Description("以...结尾")]
        EW
    }

    public enum NodeState
    {
        Normal, Cancel
    }

    public enum ApplyType
    {
        [Description("借阅")]
        Borrow,
        [Description("下载")]
        Download
    }

    public enum BorrowState
    {
        [Description("未借阅")]
        NotBorrow,
        [Description("借阅车中")]
        InCar,
        [Description("审批中")]
        Flow,
        [Description("已借出")]
        HasBorrow
    }

    public enum NodeType
    {
        [Description("案卷")]
        Node,
        [Description("文件")]
        File
    }

    public enum FlowState
    {
        [Description("未提交")]
        New,
        [Description("审批中")]
        Flow,
        [Description("审批通过")]
        Finish

    }
    public enum BorrowType
    {
        [Description("直接借阅")]
        DirBorrow,
        [Description("审批借阅")]
        FlowBorrow
    }
    public class Field
    {
        public string FieldName
        { get; set; }

        public string DataType
        { get; set; }

        public string Required
        { get; set; }
    }
    //filestore文件转换状态
    public enum FsConvertResultStatus
    {
        Process,
        Success,
        Error
    }
}
