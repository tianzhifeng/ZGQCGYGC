using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Logic.Domain
{
    public partial class CalItem
    {
        /// <summary>
        /// 被计算的字段
        /// </summary>
        public string FieldCode
        {
            get;
            set;
        }

        /// <summary>
        /// 计算公式
        /// </summary>
        public string Expression
        {
            get;
            set;
        }

        /// <summary>
        /// 是否自动触发计算
        /// </summary>
        public bool AutoTrigger
        {
            get;
            set;
        }

        /// <summary>
        /// 自动触发字段
        /// </summary>
        public string TriggerFields
        {
            get;
            set;
        }

        /// <summary>
        /// 计算返回方式
        /// </summary>
        public string CalType
        {
            get;
            set;
        }

        public int DecimalPlaces
        {
            get;
            set;
        }

        public string CalDefaultValue
        {
            get;
            set;
        }

        public string CollectionValueField
        {
            get;
            set;
        }


        public bool IsSubTableField
        {
            get
            {
                if (String.IsNullOrEmpty(this.FieldCode)) return false;
                return this.FieldCode.Split('.').Length > 1;
            }
        }

        List<CalAdapterItem> _adapterItems = new List<CalAdapterItem>();
        public List<CalAdapterItem> AdapterItems
        {
            get
            {
                return _adapterItems;
            }
        }

        List<FilterItem> _filters = new List<FilterItem>();
        public List<FilterItem> Filters
        {
            get
            {
                return _filters;
            }
        }

        List<SubTableAdpItem> _subTableAdapters = new List<SubTableAdpItem>();
        public List<SubTableAdpItem> SubTableAdapters
        {
            get
            {
                return _subTableAdapters;
            }
        }

        List<ValueConditionItem> _valueConditionItem = new List<ValueConditionItem>();
        public List<ValueConditionItem> ValueConditionItem
        {
            get
            {
                return _valueConditionItem;
            }
        }



    }

    public class CalAdapterItem
    {
        public string InputField
        {
            get;
            set;
        }

        public string NeedInputCode
        {
            get;
            set;
        }

        public string DefaultValue
        {
            get;
            set;
        }
    }

    public class SubTableAdpItem
    {
        public string FieldCode
        {
            get;
            set;
        }

        public string AdpField
        {
            get;
            set;
        }

        public string DefaultValue
        {
            get;
            set;
        }
    }

    public class ValueConditionItem
    {
        public string FieldCode
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public string QueryMode
        {
            get;
            set;
        }

        public string ConditionValue
        {
            get;
            set;
        }
    }

    public class FilterItem
    {
        public string Field
        {
            get;
            set;
        }

        public string QueryMode
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public string Group
        {
            get;
            set;
        }
    }

    public class TriggerFieldItem
    {
        /// <summary>
        /// 触发自动计算的字段名称
        /// </summary>
        public string FieldCode
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FieldName
        {
            get;
            set;
        }

        /// <summary>
        /// 被计算的字段，多个以逗号分隔
        /// </summary>
        public string CalItemCodes
        {
            get;
            set;
        }
    }
}
