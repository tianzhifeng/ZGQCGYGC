using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EPC.Logic
{
    public enum FolderType
    {
        /// <summary>
        /// 文件目录
        /// </summary>
        [Description("文件目录")]
        Folder,
        /// <summary>
        /// 文件映射目录
        /// </summary>
        [Description("文件映射目录")]
        FileMappingFolder,
        /// <summary>
        /// 数据映射目录
        /// </summary>
        [Description("数据映射目录")]
        DataMappingFolder
    }
}
