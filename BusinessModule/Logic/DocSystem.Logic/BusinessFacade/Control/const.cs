using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DocSystem.Logic
{
    public enum Algin
    {
        [Description("居中对齐")]
        center,
        [Description("左对齐")]
        left,
        [Description("右对齐")]
        right
    }

    public enum ControlType
    {
        [Description("TextBox")]
        TextBox,

        [Description("TextArea")]
        TextArea,

        [Description("DatePicker")]
        DatePicker,

        [Description("Combobox")]
        Combobox,

        [Description("Combobox整行")]
        ComboboxFullRow,

        [Description("TextBox整行")]
        TextBoxFullRow,

        [Description("TextArea整行")]
        TextAreaFullRow,

        [Description("单附件上传")]
        SingleFile,

        [Description("多附件上传")]
        MultiFile,
        [Description("单附件上传整行")]
        SingleFileFullRow,

        [Description("多附件上传整行")]
        MultiFileFullRow,
    }
}
