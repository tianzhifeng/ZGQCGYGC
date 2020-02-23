﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Logic.Model.UI.Form
{
    public class FormItem
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string textName { get; set; }
        public string Name { get; set; }
        public string NameEN { get; set; }
        public string FieldType { get; set; }
        public string ItemType { get; set; }
        public string Enabled { get; set; }
        public string Visible { get; set; }
        public string readOnly { get; set; }
        public string Unique { get; set; }
        public string DefaultValue { get; set; }
        public string Group { get; set; }
        public string Settings { get; set; }

        //用于子表汇总列
        public string SummaryType { get; set; }
        public string width { get; set; }
        public string align { get; set; }

        //用于子表的格式
        public string ColumnSettings { get; set; }

        public string help { get; set; }


        //计算公式相关属性
        public string Param { get; set; }

        public string ParamName { get; set; }

        public string Expression { get; set; }

        public string ExpressionSettings
        {
            get;
            set;
        }

    }
}
