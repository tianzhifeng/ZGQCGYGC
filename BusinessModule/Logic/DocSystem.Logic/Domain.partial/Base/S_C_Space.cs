using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Config;

namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_Space
    {
        const string conStrTemplate = "Data Source={0};User ID={1};PWD={2};Initial Catalog={3};Persist Security Info=False;Pooling=true;Min Pool Size=50;Max Pool Size=500;MultipleActiveResultSets=true";

        string _connectString = string.Empty;
        public string ConnectString
        {
            get
            {
                if (String.IsNullOrEmpty(_connectString))
                {
                    _connectString = String.Format(conStrTemplate, this.Server, this.UserName, this.Pwd, this.DbName);
                }
                return _connectString;
            }
        }

        public void SaveDataBase()
        {
            if (!this.IsExistDocSpace())
            {
                string createDataBaseSql = " Create DataBase " + this.DbName;
                var result = this.GetDocConfigSqlHelper().ExecuteNonQuery(createDataBaseSql); 
            }
            var instanceDB = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append(" use " + this.DbName + " ");
                sqlBuilder.Append(NodeConfigDDL());
                instanceDB.ExecuteNonQuery(sqlBuilder.ToString());
                sqlBuilder.Clear(); sqlBuilder.Append(" use " + this.DbName + " ");
                sqlBuilder.Append(FileConfigDDL());
                instanceDB.ExecuteNonQuery(sqlBuilder.ToString());
                sqlBuilder.Clear(); sqlBuilder.Append(" use " + this.DbName + " ");
                sqlBuilder.Append(DDLHelper.GetDefaultDDL());
                instanceDB.ExecuteNonQuery(sqlBuilder.ToString());
        }

        string[] NodeDefaultField = new string[] { "ID", "Name", "SpaceID", "ParentID", "FullPathID", "ConfigID"
            , "RelateID", "State", "CreateTime", "BorrowState", "BorrowUserID", "BorrowUserName","SortIndex" }; 

        public string NodeConfigDDL()
        {
            string tbName = "S_NodeInfo";
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(" if not exists (select * from dbo.sysobjects " +
				"where id = object_id(N'[dbo].[" + tbName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ");
            StringBuilder CreateTbSql = new StringBuilder();
            StringBuilder UpdateTbSql = new StringBuilder();
            CreateTbSql.AppendLine(" CREATE TABLE [dbo].[" + tbName + "] ( ");
            CreateTbSql.AppendLine(" [ID] [varchar](50) NOT NULL,");
            CreateTbSql.AppendLine(" [Name] [nvarchar](500) NOT NULL, ");
            CreateTbSql.AppendLine(" [SpaceID] [varchar](50) NOT NULL, ");
            CreateTbSql.AppendLine(" [ParentID] [varchar](500) NULL, ");
            CreateTbSql.AppendLine(" [FullPathID] [varchar](500) NOT NULL, ");
            CreateTbSql.AppendLine(" [ConfigID] [varchar](50) NOT NULL, ");
            CreateTbSql.AppendLine(" [RelateID] [varchar](500) NULL, ");
            CreateTbSql.AppendLine(" [State] [nvarchar](50) NOT NULL, ");
            CreateTbSql.AppendLine(" [CreateTime] [datetime] NULL, ");
            CreateTbSql.AppendLine(" [BorrowState] [nvarchar](50) NULL, ");
            CreateTbSql.AppendLine(" [BorrowUserID] [nvarchar](50) NULL, ");
            CreateTbSql.AppendLine(" [BorrowUserName] [nvarchar](50) NULL, ");
            CreateTbSql.AppendLine(" [SortIndex] [float] NULL, ");
            var list = this.ToNodeFieldList();
            foreach (var item in list)
            {
                if (!NodeDefaultField.Contains(item.FieldName))
                    CreateTbSql.AppendLine(" [" + item.FieldName + "] " + DDLHelper.GetFieldDataTypeDDL(item.DataType) + "  NULL, ");
            }
            UpdateTbSql.AppendLine(GetNodeConfigAlterDDL());
            CreateTbSql.AppendLine(@" CONSTRAINT [PK_S_NodeInfo] PRIMARY KEY CLUSTERED  ( [ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] ");

            if (String.IsNullOrEmpty(UpdateTbSql.ToString().Trim()))
            { sql.Append(" begin " + CreateTbSql.ToString() + " end "); }
            else
            {
                sql.Append(" begin " + CreateTbSql.ToString() + " end else ");
                sql.Append(" begin " + UpdateTbSql.ToString() + " end ");
            }
            return sql.ToString();
        }

        string[] FIleDefaultField = new string[] { "ID","DocIndexID", "Name","NodeID", "SpaceID", "ConfigID", "RelateID", "FullNodeID"
            , "ContentText", "State", "CreateTime", "BorrowState", "BorrowUserID", "BorrowUserName" }; 

        public string FileConfigDDL()
        {
            string tbName = "S_FileInfo";
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(" if not exists (select * from dbo.sysobjects " +
                "where id = object_id(N'[dbo].[" + tbName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) ");
            StringBuilder CreateTbSql = new StringBuilder();
            StringBuilder UpdateTbSql = new StringBuilder();
            CreateTbSql.AppendLine(" CREATE TABLE [dbo].[" + tbName + "] ( ");
            CreateTbSql.AppendLine(" [ID] [varchar](50) NOT NULL,");
            CreateTbSql.AppendLine(" [DocIndexID] [int] IDENTITY(1,1) NOT NULL,");
            CreateTbSql.AppendLine(" [Name] [nvarchar](500) NOT NULL, ");
            CreateTbSql.AppendLine(" [NodeID] [varchar](50) NOT NULL, ");
            CreateTbSql.AppendLine(" [SpaceID] [varchar](50) NOT NULL, ");
            CreateTbSql.AppendLine(" [ConfigID] [varchar](50) NOT NULL, ");
            CreateTbSql.AppendLine(" [RelateID] [varchar](500) NULL, ");
            CreateTbSql.AppendLine(" [FullNodeID] [varchar](500) NOT NULL, ");
            CreateTbSql.AppendLine(" [ContentText] [text] NULL, ");
            CreateTbSql.AppendLine(" [CreateTime] [datetime] NULL, ");
            CreateTbSql.AppendLine(" [State] [nvarchar](50) NOT NULL, ");
            CreateTbSql.AppendLine(" [BorrowState] [nvarchar](50) NULL, ");
            CreateTbSql.AppendLine(" [BorrowUserID] [nvarchar](50) NULL, ");
            CreateTbSql.AppendLine(" [BorrowUserName] [nvarchar](50) NULL, ");   
            var list = this.ToFileFieldList();
            foreach (var item in list)
            {
                if (!FIleDefaultField.Contains(item.FieldName))
                    CreateTbSql.AppendLine(" [" + item.FieldName + "] " + DDLHelper.GetFieldDataTypeDDL(item.DataType) + "  NULL, ");
            }
            UpdateTbSql.AppendLine(GetFileConfigAlterDDL());
            CreateTbSql.AppendLine(@" CONSTRAINT [PK_S_FileInfo] PRIMARY KEY CLUSTERED  ( [ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] ");

            CreateTbSql.AppendLine(@"
ALTER TABLE [dbo].[S_FileInfo]  WITH CHECK ADD  CONSTRAINT [FK_S_FileInfo_S_NodeInfo] FOREIGN KEY([NodeID])
REFERENCES [dbo].[S_NodeInfo] ([ID])
ALTER TABLE [dbo].[S_FileInfo] CHECK CONSTRAINT [FK_S_FileInfo_S_NodeInfo]");

            if (String.IsNullOrEmpty(UpdateTbSql.ToString().Trim()))
            { sql.Append(" begin " + CreateTbSql.ToString() + " end "); }
            else
            {
                sql.Append(" begin " + CreateTbSql.ToString() + " end else ");
                sql.Append(" begin " + UpdateTbSql.ToString() + " end ");
            }
            return sql.ToString();
        }

        private string GetNodeConfigAlterDDL()
        {
            StringBuilder result = new StringBuilder();
            foreach (var item in this.ToNodeFieldList())
                result.AppendLine(DDLHelper.GetNodeFieldDDL(item));
            return result.ToString();
        }

        private string GetFileConfigAlterDDL()
        {
            StringBuilder result = new StringBuilder();
            foreach (var item in this.ToFileFieldList())
                result.AppendLine(DDLHelper.GetFileFieldDDL(item));
            return result.ToString();
        }

        private bool IsExistDocSpace()
        {

            //试图调用文档空间数据库
            string sqlString = "use master " +
                "select * from sysdatabases where [name] ='" + this.DbName + "'";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            DataTable dt = db.ExecuteDataTable(sqlString);
            var result = false;
            //判断当前的数据库名称是否存在
            if (dt != null && dt.Rows.Count > 0)
                result = true;
            return result;
        }

        List<Field> _nodefields;
        public List<Field> ToNodeFieldList()
        {
            if (_nodefields == null)
            {
                _nodefields = new List<Field>();
                var configNodes = this.S_DOC_Node.ToList();
                foreach (var nodeConfig in configNodes)
                {
                    var configAttrList = nodeConfig.S_DOC_NodeAttr.Where(d => d.AttrType == AttrType.Custom.ToString()).OrderBy(d => d.AttrSort).ToList();
                    foreach (var item in configAttrList)
                    {
                        if (_nodefields.Exists(d => d.FieldName == item.AttrField))
                            continue;
                        var field = new Field();
                        field.DataType = item.DataType;
                        field.Required = item.Required;
                        field.FieldName = item.AttrField;
                        _nodefields.Add(field);
                    }
                }
            }
            return _nodefields;
        }

        List<Field> _filefields;
        public List<Field> ToFileFieldList()
        {
            if (_filefields == null)
            {
                _filefields = new List<Field>();
                var fileNodes = this.S_DOC_File.ToList();
                foreach (var nodeConfig in fileNodes)
                {
                    var configAttrList = nodeConfig.S_DOC_FileAttr.Where(d => d.AttrType == AttrType.Custom.ToString()).OrderBy(d => d.AttrSort).ToList();
                    foreach (var item in configAttrList)
                    {
                        if (_filefields.Exists(d => d.FieldName == item.FileAttrField))
                            continue;
                        var field = new Field();
                        field.DataType = item.DataType;
                        field.Required = item.Required;
                        field.FieldName = item.FileAttrField;
                        _filefields.Add(field);
                    }
                }
            }
            return _filefields;
        }

    }
}
