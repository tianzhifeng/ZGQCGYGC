using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocSystem.Logic
{
    public class MiniFileMulti : BaseControl
    {
        string htmlTemplate = "<input  {0} class=\"mini-multifile\" />";

        public MiniFileMulti()
        {
            this.Required = false;
        }

        public MiniFileMulti(string name)
            : this()
        {
            this.Name = name;
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

        public string Src
        {
            get
            {
                return this.Attributes["src"].ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    this.Attributes["src"] = "System";
                else
                    this.Attributes["src"] = value;
            }
        }

        string _style = "border-bottom: 0; padding: 0px; height:150px;width: 100%;";
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

        public override string Render()
        {
            if (String.IsNullOrEmpty(this.Name))
                throw new Formula.Exceptions.BusinessException("必须指定 multifile 控件的 name 属性");
            string attr = this.getAttr();
            return String.Format(htmlTemplate, attr);
        }
    }
}
