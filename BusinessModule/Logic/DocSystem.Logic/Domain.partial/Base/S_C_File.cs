using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_File
    {
        public IControl GetEditForm(bool showAttachment = true, bool isUpVersion = false)
        {
            var form = new Form();
            form.Style = "padding-left: 11px; padding-top: 5px;";
            var idHidden = new MiniHidden("ID"); form.AddControl(idHidden);
            var nodeIDHidden = new MiniHidden("NodeID"); form.AddControl(nodeIDHidden);
            foreach (var attr in this.S_DOC_FileAttr.Where(d => d.Visible == "False").ToList())
            {
                var hidden = new MiniHidden(attr.FileAttrField);
                form.AddControl(hidden);
            }
            var AllType = new string[] { "MainFile", "PdfFile", "PlotFile", "XrefFile", "DwfFile", "TiffFile", "SignPdfFile", "Attachments" };
            var archiveType = System.Configuration.ConfigurationManager.AppSettings["ArchiveType"];
            var ArchiveType = (string.IsNullOrEmpty(archiveType) ? "PdfFile" : archiveType).Split(',').ToList();
            var ShowType = new List<string>();
            foreach (var type in AllType)
            {
                if (showAttachment && (type == "MainFile" || type == "Attachments" || ArchiveType.Contains(type)))
                    ShowType.Add(type);
                else
                {
                    var hidden = new MiniHidden(type);
                    form.AddControl(hidden);
                }
            }

            var table = ControlGenrator.CreateDefaultFormTable();
            bool changeRow = true;
            foreach (var attr in this.S_DOC_FileAttr.Where(d => d.Visible == "True").OrderBy(d => d.AttrSort).ToList())
            {
                if (attr.IsFullRow)
                {
                    var row = new TableRow();
                    var cell = new TableCell();
                    cell.InnerText = attr.FileAttrName;
                    row.AddControl(cell);
                    var ctrlCell = new TableCell(); ctrlCell.ColSpan = 3;
                    ctrlCell.Style = "padding-right:40px;";
                    var control = attr.GetCtontrol(isUpVersion);
                    ctrlCell.AddControl(control);
                    row.AddControl(ctrlCell);
                    table.AddControl(row); changeRow = true;
                }
                else
                {
                    var cell = new TableCell();
                    cell.InnerText = attr.FileAttrName;
                    var ctrlCell = new TableCell();
                    var control = attr.GetCtontrol(isUpVersion);
                    ctrlCell.Style = "padding-right:40px;";
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
            if (showAttachment)
                AddAttachmentsControl(table, ShowType);

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
                cell.Style = " text-align: center; ";
                var contronCell = new TableCell();
                contronCell.AddControl(query.GetCtontrol());
                currentRow.AddControl(cell); currentRow.AddControl(contronCell);
            }
            form.AddControl(table);
            return form;
        }

        public IControl GetAdviceQueryForm()
        {
            return null;
        }

        public string GetQuerySchema()
        {
            var listCofig = this.S_DOC_Space.S_DOC_ListConfig.FirstOrDefault(d => d.RelationID == this.ID);
            var queryList = listCofig.S_DOC_QueryParam.OrderBy(d => d.QuerySort).ToList();
            var df = new DataForm();
            foreach (var item in queryList)
            {
                var di = df.AddItem(item.AttrField, "");
                di.SetAttr("InnerName", item.InnerField);
                di.SetAttr("Match", item.QueryType);
                if (item.InKey == TrueOrFalse.True.ToString() && (item.QueryType == QueryType.EQ.ToString() || item.QueryType == QueryType.LK.ToString()))
                    di.SetAttr("InKey", "true");
                if (item.QueryType == QueryType.LK.ToString())
                    di.SetAttr("InSplit", "true");
            }
            return df.ToString();
        }

        public IControl GetDataGrid(bool needBorrowColumn = false, bool needStateColumn = false, int stateColumnwidth = 80)
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
            if (needBorrowColumn && this.CanBorrow == "True")
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
                var nodeAttr = this.S_DOC_FileAttr.FirstOrDefault(d => d.FileAttrField == item.AttrField);
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

        protected void AddAttachmentsControl(Table table, List<string> types)
        {
            foreach (var file in types)
            {
                var fileRow = new TableRow();
                var fileCell = new TableCell(); var fileCtrlCell = new TableCell();
                fileCtrlCell.ColSpan = 3;
                fileCtrlCell.Style = "padding-right:40px;";
                fileCell.InnerText = "文件";
                if (file == "MainFile") { fileCell.InnerText = "文件"; }
                else if (file == "PdfFile") { fileCell.InnerText = "PDF文件"; }
                else if (file == "PlotFile") { fileCell.InnerText = "Plot文件"; }
                else if (file == "XrefFile") { fileCell.InnerText = "引用文件"; }
                else if (file == "DwfFile") { fileCell.InnerText = "Dwf文件"; }
                else if (file == "TiffFile") { fileCell.InnerText = "Tiff文件"; }
                else if (file == "SignPdfFile") { fileCell.InnerText = "盖章PDF文件"; }
                else if (file == "Attachments") { fileCell.InnerText = "其它附件"; }
                if (file == "Attachments")
                {
                    var fControl = new MiniFileMulti(file);
                    fControl.Src = "DocSystem";
                    fileCtrlCell.AddControl(fControl);
                }
                else
                {
                    var fControl = new MiniFileSingle(file);
                    fControl.Src = "DocSystem";
                    fileCtrlCell.AddControl(fControl);
                }
                fileRow.AddControl(fileCell); fileRow.AddControl(fileCtrlCell);
                table.AddControl(fileRow);
            }
        }

        public string GetQueryFields()
        {
            string result = @"S_FileInfo.ID,S_FileInfo.Name,S_FileInfo.NodeID,S_FileInfo.SpaceID,S_FileInfo.ConfigID,
S_FileInfo.FullNodeID,S_FileInfo.CreateTime,S_FileInfo.State,S_FileInfo.BorrowState,S_FileInfo.BorrowUserID,S_FileInfo.BorrowUserName,";
            var attrList = this.S_DOC_FileAttr.ToList();
            foreach (var item in attrList)
            {
                if (result.IndexOf("S_FileInfo." + item.FileAttrField) >= 0)
                    continue;
                result += "S_FileInfo." + item.FileAttrField + ",";
            }
            return result.TrimEnd(',');
        }
    }
}
