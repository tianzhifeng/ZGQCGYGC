using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum StorageType
    {
        [Description("公司仓库")]
        Company,
        [Description("项目仓库")]
        Project
    }

    public enum EquipmentAccountType
    {
        [Description("入库")]
        Entry,
        [Description("出库")]
        Out,
        [Description("退库")]
        Back,
        [Description("调拨")]
        Allot,
        [Description("报废")]
        Scrap,
        [Description("盘盈")]
        Profit,
        [Description("盘亏")]
        Deficit
    }

    public enum EntryType
    {
        [Description("采购")]
        Purchase,
        [Description("调拨")]
        Allot,
        [Description("盘盈")]
        Profit,
        [Description("其他")]
        Other
    }

    public enum InventoryState
    {
        [Description("未发布")]
        UnHandled,
        [Description("已发布")]
        Publish,
        [Description("处理中")]
        Processing,
        [Description("已处理")]
        Handled
    }
}
