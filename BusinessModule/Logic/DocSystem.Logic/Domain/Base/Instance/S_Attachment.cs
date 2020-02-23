using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using Formula.Helper;
using Formula;
using Config.Logic;
using Config;
using DocSystem.Logic.Domain;

namespace DocSystem.Logic.Domain
{
    [Serializable]
    public partial class S_Attachment : S_BaseEntity
    {
        #region 公共属性

        /// <summary>
        /// 编目节点ID
        /// </summary>
        public string NodeID
        {
            get
            {
                return this.DataEntity["NodeID"].ToString();
            }
            set
            {
                this.DataEntity["NodeID"] = value;
            }
        }

        /// <summary>
        /// 文件节点ID
        /// </summary>
        public string FileID
        {
            get
            {
                return this.DataEntity["FileID"].ToString();
            }
            set
            {
                this.DataEntity["FileID"] = value;
            }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version
        {
            get
            {
                return Convert.ToInt32(this.DataEntity["Version"].ToString());
            }
            set
            {
                this.DataEntity["Version"] = value;
            }
        }

        /// <summary>
        /// 是否当前版本
        /// </summary>
        public string CurrentVersion
        {
            get
            {
                return this.DataEntity["CurrentVersion"].ToString();
            }
            set
            {
                this.DataEntity["CurrentVersion"] = value;
            }
        }

        /// <summary>
        /// 文件类别
        /// </summary>
        public string FileType
        {
            get
            {
                return this.DataEntity.GetValue("FileType");
            }
            set
            {
                this.DataEntity["FileType"] = value;
            }
        }

        S_FileInfo _fileInfo;
        public S_FileInfo FileInfo
        {
            get
            {
                if (_fileInfo == null)
                    if (!String.IsNullOrEmpty(this.FileID))
                        _fileInfo = new S_FileInfo(this.FileID, this.Space);
                return _fileInfo;
            }
        }

        S_NodeInfo _nodeInfo;
        public S_NodeInfo NodeInfo
        {
            get
            {
                if (_nodeInfo == null)
                    if (!String.IsNullOrEmpty(this.NodeID))
                        _nodeInfo = new S_NodeInfo(this.NodeID, this.Space);
                return _nodeInfo;
            }
        }

        #endregion

        #region 构造函数

        public S_Attachment(string spaceID)
        {
            _initEntity();
            this.Space = DocConfigHelper.CreateConfigSpaceByID(spaceID);
            if (this.Space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + spaceID + "】档案配置对象，无法实例化附件对象");
            this.InstanceDB = SQLHelper.CreateSqlHelper(this.Space.SpaceKey, this.Space.ConnectString);
            this.IsNewModel = true;
        }

        public S_Attachment(DataRow row, string spaceID)
        {
            this._initEntity(); _validateColumns(row);
            DataAdapter.DataRowToHashTable(row, this.DataEntity);
            this.Space = DocConfigHelper.CreateConfigSpaceByID(spaceID);
            if (this.Space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + this.DataEntity["SpaceID"].ToString() + "】档案配置对象，无法实例化附件对象");
            this.InstanceDB = SQLHelper.CreateSqlHelper(this.Space.SpaceKey, this.Space.ConnectString);
            if (Tool.IsNullOrEmpty(this.ID))
                this.IsNewModel = true;
        }

        public S_Attachment(string ID, S_DOC_Space space)
        {
            this._initEntity();
            if (space == null)
                throw new Formula.Exceptions.BusinessException("档案配置对象未空，无法实例化附件对象");
            this.Space = space;
            this.InstanceDB = SQLHelper.CreateSqlHelper(this.Space.SpaceKey, this.Space.ConnectString);
            string sql = String.Format("select * from {0} where ID='{1}' ", TableName, ID);
            var dt = this.InstanceDB.ExecuteDataTable(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                DataAdapter.DataRowToHashTable(row, this.DataEntity);
            }
            else
            {
                throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ID + "】的编目节点");
              
            }

        }

        #endregion

        public override void Save(bool withDefaultAttr = false)
        {
            string mainFile = this.DataEntity.GetValue("MainFile");
            if (!String.IsNullOrEmpty(mainFile))
            {
                string extFileName = mainFile.Split('_')[1];
                extFileName = extFileName.Substring(extFileName.LastIndexOf(".") + 1);
                this.FileType = _getFileType(extFileName);
            }
            if (this.IsNewModel)
            {
                if (!this.DataEntity.ContainsKey("CreateTime") || this.DataEntity["CreateTime"] == null)
                    this.DataEntity["CreateTime"] = DateTime.Now;
                this.DataEntity.InsertDB(this.InstanceDB, TableName, "",true);
            }
            else
                this.DataEntity.UpdateDB(this.InstanceDB, TableName, this.ID);
        }

        public override string TableName
        {
            get { return "S_Attachment"; }
        }

        /// <summary>
        /// 初始化编目节点数据
        /// </summary>
        void _initEntity()
        {
            this.DataEntity = new Dictionary<string, object>();
            this.DataEntity["ID"] = "";
            this.DataEntity["FileID"] = "";
            this.DataEntity["FileType"] = "Document";
            this.DataEntity["NodeID"] = "";
            this.DataEntity["Attachments"] = "";
            this.DataEntity["ThumbNail"] = "";
            this.DataEntity["Version"] = "";
            this.DataEntity["State"] = "Normal";
            this.DataEntity["CurrentVersion"] = "";
            this.DataEntity["SWFFile"] = "";
            this.DataEntity["MainFile"] = "";
            this.DataEntity["PDFFile"] = "";
        }

        void _validateColumns(DataRow row)
        {
            if (!row.Table.Columns.Contains("FileID"))
                throw new Formula.Exceptions.BusinessException("未能找到FileID列，无法实例化附件对象");
            if (!row.Table.Columns.Contains("NodeID"))
                throw new Formula.Exceptions.BusinessException("未能找到NodeID列，无法实例化附件对象");
            if (!row.Table.Columns.Contains("MainFile"))
                throw new Formula.Exceptions.BusinessException("未能找到MainFile列，无法实例化附件对象");
            if (!row.Table.Columns.Contains("Attachments"))
                throw new Formula.Exceptions.BusinessException("未能找到Attachments列，无法实例化附件对象");
        }

        string _getFileType(string extName)
        {
            string result = "Document";
            switch (extName.ToLower().Trim())
            {
                case "xlsx":
                case "xls":
                    result = "Excel";
                    break;
                case "doc":
                case "docx":
                    result = "Word";
                    break;
                case "dwg":
                    result = "Dwg";
                    break;
                case "pdf":
                    result = "Pdf";
                    break;
                case "jpg":
                case "gif":
                case "png":
                case "tiff":
                case "bmp":
                    result = "Image";
                    break;
            }
            return result;
        }
    }
}
