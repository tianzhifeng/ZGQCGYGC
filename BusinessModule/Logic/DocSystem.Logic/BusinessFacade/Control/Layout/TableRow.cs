using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocSystem.Logic
{
    public class TableRow:BaseControl
    {
        List<TableCell> _cells = new List<TableCell>();
        public List<TableCell> Cells
        {
            get { return _cells; }
        }


        public void MergeCell(int cellIndex, int mergeColCount)
        {
            if (cellIndex == 0) throw new Formula.Exceptions.BusinessException("不能合并第0个单元格");
            if (this.Cells.Count < cellIndex) throw new Formula.Exceptions.BusinessException("指定的单元格数超过了行的单元格总数");
            var cell = this.Cells[cellIndex - 1];
            cell.ColSpan = mergeColCount;
        }

        public override string Render()
        {
            string html = "<tr {0}>{1}</tr>";
            string attr = this.getAttr();
            string childControlHtml = this.renderChildControl();
            return String.Format(html, attr, childControlHtml);
        }

        protected override void onAddingControl(IControl control)
        {
            if (control is TableCell)
            {
                var cell = control as TableCell;
                this.Cells.Add(cell);
            }
        }
    }
}
