using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DocSystem.Logic
{
    public class MiniTextArea : BaseControl
    {
        string htmlTemplate = "<input {0} class=\"mini-textarea\" />";

        public MiniTextArea()
        {
            this.Required = false;
        }

        public MiniTextArea(string name):this()
        {
            this.Name = name;
        }

        string _style = " width: 100%;height:100px ";
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
                throw new Formula.Exceptions.BusinessException("必须指定 textarea 控件的 name 属性");
            string attr = this.getAttr();
            return String.Format(htmlTemplate, attr);
        }
    }
}
