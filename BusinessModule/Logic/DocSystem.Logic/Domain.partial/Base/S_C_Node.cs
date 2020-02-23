using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_Node
    {
        public IControl GetEditForm()
        {
            var form = new Form();
            form.Style = "padding-left: 11px; padding-top: 5px;";
            var idHidden = new MiniHidden("ID"); form.AddControl(idHidden);
            var parentIDHidden = new MiniHidden("ParentID"); form.AddControl(parentIDHidden);
            foreach (var attr in this.S_DOC_NodeAttr.Where(d => d.Visible == "False").ToList())
            {
                var hidden = new MiniHidden(attr.AttrField); form.AddControl(hidden);
            }
            var table = ControlGenrator.CreateDefaultFormTable();
            bool changeRow = true;
            foreach (var attr in this.S_DOC_NodeAttr.Where(d => d.Visible == "True").OrderBy(d => d.AttrSort).ToList())
            {
                if (attr.IsFullRow)
                {
                    var row = new TableRow();
                    var cell = new TableCell();
                    cell.InnerText = attr.AttrName;
                    row.AddControl(cell);
                    var ctrlCell = new TableCell(); ctrlCell.ColSpan = 3;
                    ctrlCell.Style = "padding-right:40px;";
                    var control = attr.GetCtontrol();
                    ctrlCell.AddControl(control);
                    row.AddControl(ctrlCell);
                    table.AddControl(row); changeRow = true;
                }
                else
                {
                    var cell = new TableCell();
                    cell.InnerText = attr.AttrName;
                    var ctrlCell = new TableCell();
                    ctrlCell.Style = "padding-right:40px;";
                    var control = attr.GetCtontrol();
                    ctrlCell.AddControl(control);
                    if (changeRow)
                    {
                        var row = new TableRow();
                        row.AddControl(cell); row.AddControl(ctrlCell);
                        table.AddControl(row);
                        changeRow = false;
                    }
                    else
                    {
                        var row = table.Rows.LastOrDefault();
                        row.AddControl(cell); row.AddControl(ctrlCell);
                        changeRow = true;
                    }
                }
            }
            form.AddControl(table);
            return form;
        }

        public IControl GetQueryForm()
        {
            var form = new Form(); form.ID = "queryForm";
            var queryList = this.S_DOC_Space.S_DOC_ListConfig.FirstOrDefault(d => d.RelationID == this.ID).S_DOC_QueryParam.OrderBy(d => d.QuerySort).ToList(); 
            var table = ControlGenrator.CreateDefaultQueryFormTable();
            for (int i = 0; i < queryList.Count; i++)
            {
                var query = queryList[i];
                if (i % 3 == 0)
                {
                    var row = new TableRow();
                    table.AddControl(row);
                }
                var currentRow = table.Rows.LastOrDefault();
                var cell = new TableCell();
                cell.InnerText = query.AttrName;
                var contronCell = new TableCell();                
                contronCell.AddControl(query.GetCtontrol());
                currentRow.AddControl(cell); currentRow.AddControl(contronCell);
            }
            form.AddControl(table);
            return form;
        }

        //private string getQueryAttrName(S_DOC_QueryParam param)
        //{
        //    var result = string.Empty;
        //   // param.QueryType
        //    if (String.IsNullOrEmpty(param.AttrName))
        //        throw new Formula.Exceptions.BusinessException("");
        //    if (String.IsNullOrEmpty(param.QueryType))
        //        result = "$$" + param.AttrName;
        //    return result;
        //}

        //public string GetQuerySchema()
        //{
        //    var listCofig = this.S_DOC_Space.S_DOC_ListConfig.FirstOrDefault(d => d.RelationID == this.ID);
        //    var queryList = listCofig.S_DOC_QueryParam.OrderBy(d => d.QuerySort).ToList();
        //    var df = new DataForm(); 
        //    foreach (var item in queryList)
        //    {
        //        var di = df.AddItem(item.AttrField, "");
        //        di.SetAttr("InnerName", item.InnerField);
        //        di.SetAttr("Match", item.QueryType);
        //        if (item.InKey == TrueOrFalse.True.ToString() && (item.QueryType == QueryType.EQ.ToString() || item.QueryType == QueryType.LK.ToString()))
        //            di.SetAttr("InKey","true");
        //        if(item.QueryType==QueryType.LK.ToString())
        //            di.SetAttr("InSplit", "true");
        //    }
        //    return df.ToString();
        //}

        public IControl GetDataGrid(bool needBorrowColumn=false, bool needStateColumn = false, int stateColumnwidth = 80)
        {
            var dataGrid = new MiniDataGrid();
            var listCofig = this.S_DOC_Space.S_DOC_ListConfig.FirstOrDefault(d => d.RelationID == this.ID);
            var detailList = listCofig.S_DOC_ListConfigDetail.Where(d => d.Dispaly == "True").OrderBy(d => d.DetailSort).ToList();
            if (needStateColumn)
            {
                MiniGridColumn column = new MiniGridColumn();
                column.Field = "State";
                column.HeaderText = "状态";
                column.align = "center"; column.Allowsort = false; column.Width = stateColumnwidth;
                dataGrid.AddControl(column);
            }
            if (needBorrowColumn && this.CanBorrow == TrueOrFalse.True.ToString())
            {
                MiniGridColumn column = new MiniGridColumn();
                column.Field = "BorrowState";
                column.HeaderText = "是否借出";
                column.align = "center"; column.Allowsort = false; column.Width = 80;
                dataGrid.AddControl(column);
                MiniGridColumn column2 = new MiniGridColumn();
                column2.Field = "BorrowDetail";
                column2.HeaderText = "借阅记录";
                column2.align = "center"; column2.Allowsort = false; column2.Width = 80;
                dataGrid.AddControl(column2);
            }
            foreach (var item in detailList)
            {
                MiniGridColumn column = new MiniGridColumn();
                var nodeAttr = this.S_DOC_NodeAttr.FirstOrDefault(d => d.AttrField == item.AttrField);
                if (nodeAttr == null) continue;
                column.Field = item.AttrField;
                column.HeaderText = item.AttrName;
                column.align = item.Align;
                if (item.AllowSort == TrueOrFalse.False.ToString())
                    column.Allowsort = false;
                column.Width = Convert.ToInt32(item.Width);
                if (nodeAttr.DataType == AttrDataType.DateTime.ToString())
                    column.SetAttribute("dateformat", "yyyy-MM-dd");
                dataGrid.AddControl(column);
            }
            return dataGrid;
        }


    }
}
