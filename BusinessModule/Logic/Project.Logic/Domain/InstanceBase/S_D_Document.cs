using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;

namespace Project.Logic.Domain
{
    public partial class S_D_Document
    {
        public void AddDocumentVersion()
        {
            var docVer = new S_D_DocumentVersion();
            docVer.ID = FormulaHelper.CreateGuid();
            docVer.DocumentID = this.ID;
            docVer.DBSID = this.DBSID;
            docVer.Name = this.Name;
            docVer.Code = this.Code;
            string fileType = "";
            if (string.IsNullOrEmpty(this.FileType) && !string.IsNullOrEmpty(this.MainFiles))
            {
                var index = this.MainFiles.LastIndexOf('.');
                if (index >= 0)
                    fileType = this.MainFiles.Substring(index);
                if (fileType.Contains('_'))
                    fileType = fileType.Split('_')[0];
            }
            docVer.FileType = fileType;
            docVer.MainFiles = this.MainFiles;
            docVer.PDFFile = this.PDFFile;
            docVer.PlotFile = this.PlotFile;
            docVer.XrefFile = this.XrefFile;
            docVer.DwfFile = this.DwfFile;
            docVer.TiffFile = this.TiffFile;
            docVer.SignPdfFile = this.SignPdfFile;
            docVer.Attachments = this.Attachments;
            docVer.Version = this.Version ?? "1";
            docVer.Description = this.Description;
            docVer.State = this.State;
            docVer.XrefFile = this.XrefFile;
            docVer.FontFile = this.FontFile;
            docVer.CreateUserID = this.CreateUserID;
            docVer.CreateUser = this.CreateUser;
            docVer.CreateDate = this.CreateDate;
            docVer.ModifyUserID = this.ModifyUserID;
            docVer.ModifyUser = this.ModifyUser;
            docVer.ModifyDate = this.ModifyDate;
            this.S_D_DocumentVersion.Add(docVer);
        }
    }
}
