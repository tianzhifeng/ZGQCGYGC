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
using System.Web;
using DocSystem.Logic.Domain;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DocSystem.Logic.Domain
{
    [Serializable]
    public partial class S_FileInfo : S_BaseEntity
    {
        #region 公共属性

        private S_DOC_File _configInfo;
        /// <summary>
        /// 编目配置信息
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_DOC_File ConfigInfo
        {
            get { return _configInfo; }
            set
            {
                _configInfo = value;
                if (value != null)
                {
                    _configInfo = value;
                    this.DataEntity["ConfigID"] = value.ID;
                }
            }
        }

        /// <summary>
        /// 所属编目ID
        /// </summary>
        [NotMapped]
        [JsonIgnore]
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
        /// 所属编目全路径ID
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public string FullNodeID
        {
            get
            {
                return this.DataEntity["FullNodeID"].ToString();
            }
            set
            {
                this.DataEntity["FullNodeID"] = value;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public string RelateID
        {
            get
            {
                return this.DataEntity["RelateID"].ToString();
            }
            set
            {
                this.DataEntity["RelateID"] = value;
            }
        }

        List<S_Attachment> _attachments;
        public List<S_Attachment> Attachments
        {
            get
            {
                if (_attachments == null || _attachments.Count == 0)
                {
                    _attachments = new List<S_Attachment>();
                    string sql = "select * from S_Attachment where FileID='" + this.ID + "'";
                    var table = this.InstanceDB.ExecuteDataTable(sql);
                    foreach (DataRow row in table.Rows)
                        _attachments.Add(new S_Attachment(row, this.Space.ID));
                }
                return _attachments;
            }
        }

        S_Attachment _cAttach;
        public S_Attachment CurrentAttachment
        {
            get
            {
                if (_cAttach == null)
                    _cAttach = this.Attachments.FirstOrDefault(d => d.CurrentVersion == "True");
                return _cAttach;
            }
        }

        #endregion

        #region 构造函数

        public S_FileInfo(string spaceID, string configID)
        {
            _initEntity();
            this.Space = DocConfigHelper.CreateConfigSpaceByID(spaceID);
            if (this.Space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + spaceID + "】档案配置对象，无法实例化编目节点对象");
            this.ConfigInfo = this.Space.S_DOC_File.FirstOrDefault(d => d.ID == configID);
            if (this.ConfigInfo == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + configID + "】编目定义对象，无法实例化编目节点对象");
            this.InstanceDB = SQLHelper.CreateSqlHelper(this.Space.SpaceKey, this.Space.ConnectString); //new DbAccess(this.Space.ConnectString);
            this.IsNewModel = true;
        }

        public S_FileInfo(DataRow row)
        {
            this._initEntity(); _validateColumns(row);
            DataAdapter.DataRowToHashTable(row, this.DataEntity);
            this.Space = DocConfigHelper.CreateConfigSpaceByID(this.DataEntity["SpaceID"].ToString());
            if (this.Space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + this.DataEntity["SpaceID"].ToString() + "】档案配置对象，无法实例化编目节点对象");
            this.ConfigInfo = this.Space.S_DOC_File.FirstOrDefault(d => d.ID == this.DataEntity["ConfigID"].ToString());
            if (this.ConfigInfo == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + this.DataEntity["ConfigID"].ToString() + "】编目定义对象，无法实例化编目节点对象");
            this.InstanceDB = SQLHelper.CreateSqlHelper(this.Space.SpaceKey, this.Space.ConnectString);
            if (Tool.IsNullOrEmpty(this.ID))
                this.IsNewModel = true;
        }

        public S_FileInfo(string ID, S_DOC_Space space)
        {
            this._initEntity();
            if (space == null)
                throw new Formula.Exceptions.BusinessException("档案配置对象未空，无法实例化编目节点对象");
            this.Space = space;
            this.InstanceDB = SQLHelper.CreateSqlHelper(this.Space.SpaceKey, this.Space.ConnectString);
            string sql = String.Format("select * from {0} where ID='{1}' ", TableName, ID);
            var dt = this.InstanceDB.ExecuteDataTable(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                _validateColumns(row);
                DataAdapter.DataRowToHashTable(row, this.DataEntity);
                this.ConfigInfo = this.Space.S_DOC_File.FirstOrDefault(d => d.ID == this.DataEntity["ConfigID"].ToString());
                if (this.ConfigInfo == null)
                    throw new Formula.Exceptions.BusinessException("未找到ID为【" + this.DataEntity["ConfigID"].ToString() + "】编目定义对象，无法实例化编目节点对象");

            }
            else
            {
                throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ID + "】的编目节点");

            }


        }

        #endregion

        #region 公共实例方法

        //进到下载借阅车时，根据配置生成名称
        public string CreateCarName()
        {
            var resultName = this.Name;
            if (this.ConfigInfo == null)
                return resultName;
            if (this.ConfigInfo.ExtentionObject.GetValue("Ext_Car_UseFullName").ToLower() == "true")
            {
                var fullName = string.Empty;
                var splitStr = this.ConfigInfo.ExtentionObject.GetValue("Ext_Car_FullNameSplit");
                if (string.IsNullOrEmpty(splitStr)) splitStr = " > ";
                var nameFormatStr = this.ConfigInfo.ExtentionObject.GetValue("Ext_Car_NameFormat");
                if (string.IsNullOrEmpty(nameFormatStr))
                    nameFormatStr = "{FullName} > {Name}";
                var node = S_NodeInfo.GetNode(this.NodeID, this.Space.ID);
                if (node != null)
                    fullName = string.Join(splitStr, node.Seniorities.Select(a => a.Name).ToList());
                resultName = nameFormatStr.Replace("{FullName}", fullName).Replace("{Name}", this.Name);
            }
            return resultName;
        }

        public override void Delete()
        {
            this.ClearAttachment();
            this.DataEntity.DeleteDB(this.InstanceDB, TableName, this.ID);
        }

        public virtual void MoveTo(string nodeID)
        {
            var node = S_NodeInfo.GetNode(nodeID, this.Space.ID);
            if (node == null)
                throw new Formula.Exceptions.BusinessException("未能找到ID为【" + nodeID + "】的编目信息，移动文件信息失败");
            if (this.IsNewModel) throw new Formula.Exceptions.BusinessException("新创建的文件对象，不能使用Move方法进行文件移动");
            this.FullNodeID = node.FullPathID;
            this.NodeID = nodeID;
            this.Save(true);
        }

        public void ClearAttachment()
        {
            string sql = " delete from S_Attachment where FileID='" + this.ID + "'";
            this.InstanceDB.ExecuteNonQuery(sql);
        }

        public void AddAttachment(S_Attachment attachment)
        {
            if (!attachment.IsNewModel) throw new Formula.Exceptions.BusinessException("附件对象不是新建对象，无法调用添加方法");
            if (this.IsNewModel)
            {
                this.Attachments.Add(attachment);
                attachment.Version = 0;
            }
            else
            {
                this.InstanceDB.ExecuteNonQuery(" update S_Attachment set CurrentVersion='False' where FileID='" + this.ID + "' ");
                attachment.FileID = this.ID;
                attachment.NodeID = this.NodeID;
                attachment.CurrentVersion = "True";
                attachment.Version = this.getMaxAttachVersion();
                attachment.Save();
                this.Attachments.Add(attachment);
            }
        }

        public void Publish()
        {
            string sql = "update {0} set State='{1}' where ID='{2}'";
            this.InstanceDB.ExecuteNonQuery(String.Format(sql, this.TableName, DocState.Published.ToString(), this.ID));
            CheckFulltextProp();
        }

        public void Recover()
        {
            string sql = "update {0} set State='{1}' where ID='{2}'";
            this.InstanceDB.ExecuteNonQuery(String.Format(sql, this.TableName, DocState.Normal.ToString(), this.ID));
        }

        public void Invalid()
        {
            string sql = "update {0} set State='{1}' where ID='{2}'";
            this.InstanceDB.ExecuteNonQuery(String.Format(sql, this.TableName, DocState.Invalid.ToString(), this.ID));
        }

        public void CheckFulltextProp()
        {
            //如果当前对象存在全文检索属性，则变更全文检索文件状态，以便重新插入Es
            if (this.ConfigInfo == null) throw new Formula.Exceptions.BusinessException("无法获取配置信息，保存失败");
            if (this.ConfigInfo.S_DOC_FileAttr.Any(d => d.FulltextProp == "True"))
                this.InstanceDB.ExecuteNonQuery(" update S_Attachment set State='Normal' where FileID='" + this.ID + "' and CurrentVersion='True'   ");
        }

        public override void Save(bool withDefaultAttr = true)
        {
            if (this.DataEntity.ContainsKey("DocIndexID"))  //作为hubbledotnet的检索键，不参与任何更新操作
                this.DataEntity.Remove("DocIndexID");
            if (this.IsNewModel)
            {
                if (withDefaultAttr) this.FillConfigDefaultAttr();
                if (String.IsNullOrEmpty(this.ID)) this.ID = FormulaHelper.CreateGuid();
                base.Save();
                foreach (var item in Attachments)
                {
                    this.InstanceDB.ExecuteNonQuery(" update S_Attachment set CurrentVersion='False' where FileID='" + this.ID + "' ");
                    item.FileID = this.ID;
                    item.NodeID = this.NodeID;
                    item.CurrentVersion = "True";
                    item.Version = this.getMaxAttachVersion();
                    item.Save();
                }
            }
            else
            {
                //if (withDefaultAttr) this.FillConfigDefaultAttr();
                base.Save();
                this.InstanceDB.ExecuteNonQuery(" update S_Attachment set NodeID='" + this.NodeID + "' where FileID='" + this.ID + "' ");
                CheckFulltextProp();
            }
        }

        public void UpdateAttachment(S_Attachment attachment)
        {
            if (attachment.IsNewModel) throw new Formula.Exceptions.BusinessException("附件对象新建对象，无法调用更新方法");
            attachment.Save();
            var attch = this.Attachments.FirstOrDefault(d => d.ID == attachment.ID);

        }

        public void FillConfigDefaultAttr()
        {
            if (this.ConfigInfo == null) throw new Formula.Exceptions.BusinessException("无法获取配置信息，保存失败");
            var attrs = this.ConfigInfo.S_DOC_FileAttr.Where(d => !String.IsNullOrEmpty(d.DefaultValue)).ToList();
            var CurrentUserInfo = FormulaHelper.GetUserInfo();
            foreach (var attr in attrs)
            {
                if (attr.DefaultValue.IndexOf("{") < 0)
                    this.DataEntity[attr.FileAttrField] = attr.DefaultValue;
                else
                {
                    if (attr.DefaultValue.IndexOf("{") >= 0)
                    {
                        var defaultStr = attr.DefaultValue.Replace("{", "").Replace("}", "");
                        if (defaultStr == "Now")
                            this.DataEntity[attr.FileAttrField] = DateTime.Now;
                        else if (defaultStr == "UserID")
                            this.DataEntity[attr.FileAttrField] = CurrentUserInfo.UserID; //user.UserID;
                        else if (defaultStr == "UserName")
                            this.DataEntity[attr.FileAttrField] = CurrentUserInfo.UserName;
                        if (!String.IsNullOrEmpty(this.DataEntity.GetValue("NodeID")))
                        {
                            var node = S_NodeInfo.GetNode(this.DataEntity.GetValue("NodeID"), this.Space.ID);
                            var defaultValue = attr.DefaultValue.Replace("{", "").Replace("}", "").Split(':');
                            if (defaultValue.Length > 1)
                            {

                                var nodePositon = defaultValue[0];
                                var defaultField = defaultValue[1];
                                if (nodePositon == "Root")
                                {
                                    if (node.RootNode.DataEntity.ContainsKey(defaultField))
                                        this.DataEntity[attr.FileAttrField] = node.RootNode.DataEntity[defaultField];
                                }
                                else if (nodePositon == "Node")
                                {
                                    if (node.DataEntity.ContainsKey(defaultField))
                                        this.DataEntity[attr.FileAttrField] = node.DataEntity[defaultField];
                                }
                                else if (nodePositon.Split('.').Length > 0)
                                {
                                    var pnode = _getParentNode(node, nodePositon.Split('.').Length);
                                    if (pnode.DataEntity.ContainsKey(defaultField))
                                        this.DataEntity[attr.FileAttrField] = node.DataEntity[defaultField];
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion

        #region 私有实例方法

        /// <summary>
        /// 初始化编目节点数据
        /// </summary>
        void _initEntity()
        {
            this.DataEntity = new Dictionary<string, object>();
            this.DataEntity["ID"] = "";
            this.DataEntity["Name"] = "";
            this.DataEntity["SpaceID"] = "";
            this.DataEntity["NodeID"] = "";
            this.DataEntity["FullNodeID"] = "";
            this.DataEntity["ConfigID"] = "";
            this.DataEntity["State"] = "Normal";
        }

        void _validateColumns(DataRow row)
        {
            if (!row.Table.Columns.Contains("SpaceID"))
                throw new Formula.Exceptions.BusinessException("未能找到SpaceID列，无法实例化节点对象");
            if (!row.Table.Columns.Contains("ConfigID"))
                throw new Formula.Exceptions.BusinessException("未能找到ConfigID列，无法实例化节点对象");
            if (!row.Table.Columns.Contains("NodeID"))
                throw new Formula.Exceptions.BusinessException("未能找到NodeID列，无法实例化节点对象");
            if (!row.Table.Columns.Contains("FullNodeID"))
                throw new Formula.Exceptions.BusinessException("未能找到FullNodeID列，无法实例化节点对象");
        }

        int getMaxAttachVersion()
        {
            if (this.Attachments.Count == 0) return 1;
            return this.Attachments.Max(d => d.Version) + 1;
        }

        private S_NodeInfo _getParentNode(S_NodeInfo parent, int len)
        {
            var node = parent;
            if (len == 1) return node;
            else
            {
                for (int i = 0; i < len - 2; i++)
                {
                    if (parent.Parent != null)
                        node = parent.Parent;
                }
            }
            return node;
        }
        #endregion

        #region 静态方法

        public static S_FileInfo GetFile(string ID, string spaceID)
        {
            var space = DocConfigHelper.CreateConfigSpaceByID(spaceID);
            if (space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + spaceID + "】档案配置对象，无法获取节点对象");

            string sql = "select * from S_FileInfo where ID='" + ID + "' ";
            var dt = SQLHelper.CreateSqlHelper(space.SpaceKey, space.ConnectString).ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new S_FileInfo(row);
            }
            else
            {
                throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ID + "】的编目节点");
            }



        }

        public static S_FileInfo GetFile(string spaceID, string configID, string whereStr)
        {
            var space = DocConfigHelper.CreateConfigSpaceByID(spaceID);
            if (space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + spaceID + "】档案配置对象，无法获取节点对象");

            string where = whereStr.Replace("where", "").Replace("WHERE", "");

            string sql = string.Empty;
            if (!String.IsNullOrEmpty(configID))
                sql = String.Format(" select * from S_FileInfo where ConfigID='{0}'", configID);
            else
                sql = " select * from S_FileInfo where 1=1 ";
            if (!String.IsNullOrEmpty(where))
                sql += " and " + where;

            var dt = SQLHelper.CreateSqlHelper(space.SpaceKey, space.ConnectString).ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new S_FileInfo(row);
            }
            else
            {
                return null;
            }



        }

        public static bool Exists(string spaceID, string configID, string whereStr)
        {
            var space = DocConfigHelper.CreateConfigSpaceByID(spaceID);
            if (space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + spaceID + "】档案配置对象，无法获取节点对象");
            string where = whereStr.Replace("where", "").Replace("WHERE", "");
            bool result = false;

            string sql = string.Empty;
            if (!String.IsNullOrEmpty(configID))
                sql = String.Format(" select ID from S_FileInfo where ConfigID='{0}'", configID);
            else
                sql = " select ID from S_FileInfo where 1=1 ";
            if (!String.IsNullOrEmpty(where))
                sql += " and " + where;
            var dt = SQLHelper.CreateSqlHelper(space.SpaceKey, space.ConnectString).ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }


            return result;
        }

        #endregion

        public override string TableName
        {
            get { return "S_FileInfo"; }
        }
    }
}
