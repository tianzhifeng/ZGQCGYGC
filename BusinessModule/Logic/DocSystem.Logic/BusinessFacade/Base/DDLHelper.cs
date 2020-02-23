using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocSystem.Logic.Domain;

namespace DocSystem.Logic
{
    public class DDLHelper
    {
        public static string GetNodeFieldDDL(Field field)
        {
            var tableName = "S_NodeInfo";
            StringBuilder UpdateTbSql = new StringBuilder();
            string fieldSql = " [" + field.FieldName + "] ";
            if (field.DataType == AttrDataType.Decimal.ToString())
                fieldSql += " decimal(18,2) ";
            else
                fieldSql += " " + GetFieldDataTypeDDL(field.DataType) + " ";

            //判断字段在数据库中是否存在
            UpdateTbSql.Append("  if exists (" +
                " select * from dbo.syscolumns " +
                " where [name] = '" + field.FieldName + "' " +
                " and [id]=object_id(N'[dbo].[" + tableName + "]')  " +
                " and OBJECTPROPERTY(id, N'IsUserTable') = 1 ) ");
            UpdateTbSql.Append(" begin ");
            UpdateTbSql.Append(" ALTER TABLE [dbo].[" + tableName + "] ALTER COLUMN  " + fieldSql + " ");
            UpdateTbSql.Append(" end " + " else begin ");
            UpdateTbSql.Append(" ALTER TABLE [dbo].[" + tableName + "] ADD " + fieldSql + " end  ");
            return UpdateTbSql.ToString();
        }

        public static string GetFileFieldDDL(Field field)
        {
            var tableName = "S_FileInfo";
            StringBuilder UpdateTbSql = new StringBuilder();
            string fieldSql = " [" + field.FieldName + "] ";
            fieldSql += " " + GetFieldDataTypeDDL(field.DataType) + " ";

            //判断字段在数据库中是否存在
            UpdateTbSql.Append("  if exists (" +
                " select * from dbo.syscolumns " +
                " where [name] = '" + field.FieldName + "' " +
                " and [id]=object_id(N'[dbo].[" + tableName + "]')  " +
                " and OBJECTPROPERTY(id, N'IsUserTable') = 1 ) ");
            UpdateTbSql.Append(" begin ");
            UpdateTbSql.Append(" ALTER TABLE [dbo].[" + tableName + "] ALTER COLUMN  " + fieldSql + " ");
            UpdateTbSql.Append(" end " + " else begin ");
            UpdateTbSql.Append(" ALTER TABLE [dbo].[" + tableName + "] ADD " + fieldSql + " end  ");
            return UpdateTbSql.ToString();
        }

        public static string GetFieldDataTypeDDL(string dataType)
        {
            string result = string.Empty;
            if (dataType == AttrDataType.NVarchar50.ToString())
                result = " nvarchar(50) ";
            else if (dataType == AttrDataType.NVarchar200.ToString())
                result = " nvarchar(200) ";
            else if (dataType == AttrDataType.NVarchar500.ToString())
                result = " nvarchar(500) ";
            else if (dataType == AttrDataType.NVarcharMax.ToString())
                result = " nvarchar(max) ";
            else if (dataType == AttrDataType.Varchar50.ToString())
                result = " varchar(50) ";
            else if (dataType == AttrDataType.Varchar500.ToString())
                result = " varchar(500) ";
            else if (dataType == AttrDataType.VarcharMAX.ToString())
                result = " varchar(max) ";
            else if (dataType == AttrDataType.Int.ToString())
                result = " int ";
            else if (dataType == AttrDataType.Decimal.ToString())
                result = " decimal(18,2) ";
            else if (dataType == AttrDataType.DateTime.ToString())
                result = " DateTime ";
            else
                result = " nvarchar(500) ";
            return result;
        }

        public static string GetDefaultDDL()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(" if not exists (select * from dbo.sysobjects " +
       "where id = object_id(N'[dbo].[HBTrigger_FileInfo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)   ");
            sql.AppendLine(" begin ");
            sql.Append(
            @"CREATE TABLE [dbo].[HBTrigger_FileInfo](
	[Serial] [bigint] IDENTITY(1,1) NOT NULL,
	[Id] [int] NOT NULL,
	[Opr] [char](16) NULL,
	[Fields] [nvarchar](4000) NULL,
PRIMARY KEY CLUSTERED 
(
	[Serial] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]"); sql.AppendLine(" end ");

            sql.AppendLine(" if not exists (select * from dbo.sysobjects " +
        "where id = object_id(N'[dbo].[S_Attachment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)   ");
            sql.AppendLine(" begin ");
            sql.AppendLine(@"  CREATE TABLE [dbo].[S_Attachment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FileID] [varchar](50) NULL,
    [FileType] [varchar](50) NOT NULL,
	[NodeID] [varchar](50) NULL,
	[Attachments] [nvarchar](500) NULL,
    [ThumbNail] [nvarchar](500) NULL,
	[Version] [int] NOT NULL,
	[State] [varchar](50) NOT NULL,
	[CurrentVersion] [varchar](50) NOT NULL,
    [SWFFile] [nvarchar](500) NULL,
    [MainFile] [nvarchar](500) NULL,
    [PDFFile] [nvarchar](500) NULL,
    [PlotFile] [nvarchar](500) NULL,
    [XrefFile] [nvarchar](500) NULL,
    [DwfFile] [nvarchar](500) NULL,
    [TiffFile] [nvarchar](500) NULL,
    [SignPdfFile] [nvarchar](500) NULL,
    [BMPState] [nvarchar](50) NULL,
    [PDFState] [nvarchar](50) NULL,
    [SWFState] [nvarchar](50) NULL,
    [CreateDate] [datetime] NULL,
    [CreateUser] [nvarchar](200) NULL,
    [CreateUserName] [nvarchar](200) NULL,
 CONSTRAINT [PK_S_Attachment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[S_Attachment]  WITH CHECK ADD  CONSTRAINT [FK_S_Attachment_S_FileInfo] FOREIGN KEY([FileID])
REFERENCES [dbo].[S_FileInfo] ([ID])

ALTER TABLE [dbo].[S_Attachment] CHECK CONSTRAINT [FK_S_Attachment_S_FileInfo]

ALTER TABLE [dbo].[S_Attachment]  WITH CHECK ADD  CONSTRAINT [FK_S_Attachment_S_NodeInfo] FOREIGN KEY([NodeID])
REFERENCES [dbo].[S_NodeInfo] ([ID])

ALTER TABLE [dbo].[S_Attachment] CHECK CONSTRAINT [FK_S_Attachment_S_NodeInfo]");
            sql.AppendLine(" end ");
            return sql.ToString();
        }
    }
}
