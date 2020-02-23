using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DocSystem.Logic
{
    public class TableCell :BaseControl
    {
        int _ColSpan = 0;
        public int ColSpan
        {
            get { return _ColSpan; }
            set { _ColSpan = value; }
        }

        public override string Render()
        {
            string html = "<td {0}>{1}</td>";
            string attr = this.getAttr();
            if (_ColSpan > 0)
                attr += " colspan=\"" + _ColSpan.ToString() + "\" ";
           
            string childControlHtml = this.renderChildControl();
            return String.Format(html, attr, childControlHtml);
        }
    }
}
