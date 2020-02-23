using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocSystem.Logic
{
    public class Form : BaseControl
    {
        string _id = "dataForm";
        public override string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public override string Render()
        {
            string html = "<div {0}>{1}</div>";
            string attr = this.getAttr();
            string childControlHtml = this.renderChildControl();
            return String.Format(html, attr, childControlHtml);
        }
    }
}
