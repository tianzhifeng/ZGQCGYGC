using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DocSystem.Logic
{
    public class MiniGridColumn:BaseControl
    {
        string htmlTemplate = " <div {0}>{1}</div>";

        public MiniGridColumn()
        {
            this.Headeralign = Algin.center.ToString();
            this.align = Algin.center.ToString();
            this.Allowsort = true;
        }

        public string Field
        {
            get
            {
                if (this.Attributes.ContainsKey("field") && !Tool.IsNullOrEmpty(this.Attributes["field"]))
                    return this.Attributes["field"].ToString();
                else return "";
            }
            set { this.Attributes["field"] = value; }
        }

        public string Headeralign
        {
            get
            {
                if (this.Attributes.ContainsKey("headeralign") && !Tool.IsNullOrEmpty(this.Attributes["headeralign"]))
                    return this.Attributes["headeralign"].ToString();
                else return "";
            }
            set { this.Attributes["headeralign"] = value; }
        }

        public string align
        {
            get
            {
                if (this.Attributes.ContainsKey("align") && !Tool.IsNullOrEmpty(this.Attributes["align"]))
                    return this.Attributes["align"].ToString();
                else return "";
            }
            set { this.Attributes["align"] = value; }
        }

        public bool Allowsort
        {
            get
            {
                if (this.Attributes.ContainsKey("allowsort") && !Tool.IsNullOrEmpty(this.Attributes["allowsort"]))
                    return Convert.ToBoolean(this.Attributes["allowsort"].ToString());
                else return false;
            }
            set
            {
                if (value)
                    this.Attributes["allowsort"] = value.ToString().ToLower();
                else
                {
                    if (this.Attributes.ContainsKey("allowsort"))
                        this.Attributes.Remove("allowsort");
                }
            }
        }

        string _headerText = string.Empty;
        public string HeaderText
        {
            get { return _headerText; }
            set { _headerText = value; }
        }

        string _widthUnit = "px";
        public override string WidthUnit
        {
            get
            {
                return _widthUnit;
            }
            set
            {
                _widthUnit = value;
            }
        }

        public override string Render()
        {
            if (String.IsNullOrEmpty(this.Field))
                throw new Formula.Exceptions.BusinessException("必须指定 gridcolumn 控件的 field 属性");
            string attr = this.getAttr();
            if(this.Width==0)
                attr += " width=\"*\" ";
            string childControlHtml = this.HeaderText + " " + this.renderChildControl();
            return String.Format(htmlTemplate, attr, childControlHtml);
        }
    }
}
