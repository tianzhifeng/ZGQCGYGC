using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum EquipmentMaterialCategoryType
    {
        /// <summary>
        /// 分类
        /// </summary>
        [Description("分类")]
        Category,
        /// <summary>
        /// 设备材料
        /// </summary>
        [Description("设备材料")]
        Entity
    }

    public enum BomType
    {
        /// <summary>
        /// 设备
        /// </summary>
        [Description("设备")]
        Equipment,

        /// <summary>
        /// 材料
        /// </summary>
        [Description("材料")]
        Material
    }

    public enum BomVersionPhase
    {
        [Description("预算")]
        预算版本,
        [Description("设计")]
        设计版本
    }

    public enum BomVersionModifyState
    {
        [Description("未修改")]
        Normal,
        [Description("新增")]
        Add,
        [Description("修改")]
        Modify,
        [Description("删除")]
        Remove
    }
}
