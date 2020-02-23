using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DocSystem.Logic
{
    public class MiniComboBox : BaseControl
    {
        string htmlTemplate = "<input {0} class=\"mini-combobox\"  />";

        public MiniComboBox()
        {
            this.Attributes["textfield"] = "text";
            this.Attributes["valuefield"] = "value";
            this.AllowInput = true;
            this.Required = false;
        }

        public MiniComboBox(string name)
            : this()
        {
            this.Name = name;
        }

        string _style = " width: 100% ";
        public override string Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
            }
        }

        public string VType
        {
            get
            {
                if (this.Attributes.ContainsKey("vtype") && !Tool.IsNullOrEmpty(this.Attributes["vtype"]))
                    return this.Attributes["vtype"].ToString();
                else return "";
            }
            set { this.Attributes["vtype"] = value; }
        }

        public string TextField
        {
            get
            {
                if (this.Attributes.ContainsKey("textfield") && !Tool.IsNullOrEmpty(this.Attributes["textfield"]))
                    return this.Attributes["textfield"].ToString();
                else return "";
            }
            set { this.Attributes["textfield"] = value; }
        }

        public string ValueField
        {
            get
            {
                if (this.Attributes.ContainsKey("valuefield") && !Tool.IsNullOrEmpty(this.Attributes["valuefield"]))
                    return this.Attributes["valuefield"].ToString();
                else return "";
            }
            set { this.Attributes["valuefield"] = value; }
        }

        public bool AllowInput
        {
            get
            {
                if (this.Attributes.ContainsKey("allowinput") && !Tool.IsNullOrEmpty(this.Attributes["allowinput"]))
                    return Convert.ToBoolean(this.Attributes["allowinput"].ToString());
                else return false;
            }
            set { this.Attributes["allowinput"] = value; }
        }

        public string Url
        {
            get {
                if (this.Attributes.ContainsKey("url") && !Tool.IsNullOrEmpty(this.Attributes["url"]))
                    return this.Attributes["url"].ToString();
                else return "";
            }
            set { this.Attributes["url"] = value; }
        }

        public bool Required
        {
            get
            {
                if (this.Attributes.ContainsKey("required") && !Tool.IsNullOrEmpty(this.Attributes["required"]))
                    return Convert.ToBoolean(this.Attributes["required"].ToString());
                else return false;
            }
            set
            {
                if (value)
                    this.Attributes["required"] = value;
                else
                {
                    if (this.Attributes.ContainsKey("required"))
                        this.Attributes.Remove("required");
                }
            }
        }

        public override string Render()
        {
            if (String.IsNullOrEmpty(this.Name))
                throw new Formula.Exceptions.BusinessException("必须指定 combobox 控件的 name 属性");           
            string attr = this.getAttr();
            if (this.Required)
                attr += " required=\"true\" ";
            //if (String.IsNullOrEmpty(Url)) throw new Formula.Exceptions.BusinessException("未指定枚举的查询地址，无法绘制combobox控件");
            if (String.IsNullOrEmpty(this.TextField)) throw new Formula.Exceptions.BusinessException("未指定Text字段，无法绘制combobox控件");
            if (String.IsNullOrEmpty(this.ValueField)) throw new Formula.Exceptions.BusinessException("未指定Value字段，无法绘制combobox控件");            
            return String.Format(htmlTemplate, attr);
        }
    }
}
