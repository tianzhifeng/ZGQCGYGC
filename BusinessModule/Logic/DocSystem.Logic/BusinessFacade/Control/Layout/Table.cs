using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocSystem.Logic
{
    public class Table : BaseControl
    {
        int _cellspacing = 2;
        public int CellSpacing 
        {
            get { return  _cellspacing; }
            set { _cellspacing = value; }
        }

        int _cellpadding = 0;
        public int CellPadding 
        {
            get { return _cellpadding; }
            set { _cellpadding = value; }
        }

        int _border = 0;
        public int Border
        {
            get { return _border; }
            set { _border = value; }
        }

        List<TableRow> _rows = new List<TableRow>();
        public List<TableRow> Rows
        {
            get { return _rows; }
        }

        int _width = 100;
        public override int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public override string Render()
        {
            string html = "<table {0}>{1}</table>";
            string attr = this.getAttr();
            attr += " border=\"" + this.Border.ToString() + "\" cellpadding=\"" + CellPadding.ToString() + "\" cellspacing=\"" + CellSpacing.ToString() + "\"";
            string childControlHtml = this.renderChildControl();
            return String.Format(html, attr, childControlHtml);
        }

        protected override void onAddingControl(IControl control)
        {
            if (control is TableRow)
            {
                var row = control as TableRow;
                this.Rows.Add(row);
            }
        }
    }
}
