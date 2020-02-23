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
    public partial class S_NodeInfo : S_BaseEntity
    {
        #region 公共属性

        private S_DOC_Node _configInfo;
        [NotMapped]
        [JsonIgnore]
        /// <summary>
        /// 编目配置信息
        /// </summary>
        public S_DOC_Node ConfigInfo
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

        List<S_FileInfo> _files;
        [NotMapped]
        [JsonIgnore]
        /// <summary>
        /// 文件集合
        /// </summary>
        public List<S_FileInfo> FileInfos
        {
            get
            {
                if (_files == null)
                {
                    _files = new List<S_FileInfo>();
                    string sql = "select * from S_FileInfo where NodeID='" + this.ID + "'";
                    var table = this.InstanceDB.ExecuteDataTable(sql);
                    foreach (DataRow row in table.Rows)
                        _files.Add(new S_FileInfo(row));
                }
                return _files;
            }
        }

        S_NodeInfo _parentNode;
        [NotMapped]
        [JsonIgnore]
        /// <summary>
        /// 父节点
        /// </summary>
        public S_NodeInfo Parent
        {
            get
            {
                if (_parentNode == null)
                {
                    if (String.IsNullOrEmpty(this.DataEntity.GetValue("ParentID"))) return null;
                    _parentNode = new S_NodeInfo(this.DataEntity["ParentID"].ToString(), this.Space);
                }
                return _parentNode;
            }
            set
            {
                if (value != null)
                {
                    _parentNode = value;
                    DataEntity["ParentID"] = _parentNode.ID;
                }
            }
        }

        /// <summary>
        /// 编目全路径ID
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public string FullPathID
        {
            get
            {
                return this.DataEntity["FullPathID"].ToString();
            }
            set
            {
                this.DataEntity["FullPathID"] = value;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public string RelateID
        {
            get
            {
                return this.DataEntity.GetValue("RelateID");
            }
            set
            {
                this.DataEntity["RelateID"] = value;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public double SortIndex
        {
            get
            {
                if (!string.IsNullOrEmpty(this.DataEntity.GetValue("SortIndex")))
                    return Convert.ToDouble(this.DataEntity.GetValue("SortIndex"));
                else
                    return 0;
            }
            set
            {
                this.DataEntity["SortIndex"] = value;
            }
        }

        List<S_Attachment> _attachments;
        [NotMapped]
        [JsonIgnore]
        public List<S_Attachment> Attachments
        {
            get
            {
                if (_attachments == null)
                {
                    _attachments = new List<S_Attachment>();
                    string sql = "select * from S_Attachment where NodeID='" + this.ID + "'";
                    var table = this.InstanceDB.ExecuteDataTable(sql);
                    foreach (DataRow row in table.Rows)
                        _attachments.Add(new S_Attachment(row, this.Space.ID));
                }
                return _attachments;
            }
        }

        S_Attachment _cAttach;
        [NotMapped]
        [JsonIgnore]
        public S_Attachment CurrentAttachment
        {
            get
            {
                if (_cAttach == null)
                    _cAttach = this.Attachments.FirstOrDefault(d => d.CurrentVersion == "True");
                return _cAttach;
            }
        }

        List<S_NodeInfo> _children;
        [NotMapped]
        [JsonIgnore]
        public List<S_NodeInfo> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<S_NodeInfo>();
                    string sql = "select * from S_NodeInfo where ParentID='" + this.ID + "'";
                    var table = this.InstanceDB.ExecuteDataTable(sql);
                    foreach (DataRow row in table.Rows)
                        _children.Add(new S_NodeInfo(row));
                }
                return _children;
            }
        }

        List<S_NodeInfo> _neighbors;
        [NotMapped]
        [JsonIgnore]
        public List<S_NodeInfo> Neighbors
        {
            get
            {
                if (_neighbors == null)
                {
                    _neighbors = new List<S_NodeInfo>();
                    if (String.IsNullOrEmpty(this.DataEntity.GetValue("ParentID"))) return _neighbors;
                    string sql = "select * from S_NodeInfo where ParentID='" + this.DataEntity.GetValue("ParentID") + "' and ID!='" + this.ID + "'";
                    var treeConfig = this.ConfigInfo.S_DOC_Space.S_DOC_TreeConfig.FirstOrDefault();
                    if (treeConfig != null)
                        sql += treeConfig.GetOrderByStr();
                    var table = this.InstanceDB.ExecuteDataTable(sql);
                    foreach (DataRow row in table.Rows)
                        _neighbors.Add(new S_NodeInfo(row));
                }
                return _neighbors;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public List<S_NodeInfo> AllChildren
        {
            get
            {
                var allChildren = new List<S_NodeInfo>();
                string sql = "select * from S_NodeInfo where FullPathID like '" + this.FullPathID + "%'";
                var table = this.InstanceDB.ExecuteDataTable(sql);
                foreach (DataRow row in table.Rows)
                    allChildren.Add(new S_NodeInfo(row));
                return allChildren;
            }
        }

        /// <summary>
        /// 所有上级节点（含自身）
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_NodeInfo> Seniorities
        {
            get
            {
                var _seniorities = new List<S_NodeInfo>();
                string sql = "select * from S_NodeInfo where '" + this.FullPathID + "' like FullPathID+'%' order by FullPathID";
                var table = this.InstanceDB.ExecuteDataTable(sql);
                foreach (DataRow row in table.Rows)
                    _seniorities.Add(new S_NodeInfo(row));
                return _seniorities;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public S_NodeInfo RootNode
        {
            get
            {
                if (this.IsRoot)
                    return this;
                else
                    return this.Parent.RootNode;

            }
        }

        [NotMapped]
        [JsonIgnore]
        public bool IsRoot
        {
            get
            {
                if (this.Parent != null)
                    return false;
                else
                    return true;
                //if (this.ConfigInfo.Structs.Exists(d => d.Parent.NodeID == "Root"))
                //    return true;
                //else
                //    return false;
            }
        }

        #endregion

        #region 构造函数

        public S_NodeInfo(string spaceID, string configID)
        {
            _initEntity();
            this.Space = DocConfigHelper.CreateConfigSpaceByID(spaceID);
            if (this.Space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + spaceID + "】档案配置对象，无法实例化编目节点对象");
            this.ConfigInfo = this.Space.S_DOC_Node.FirstOrDefault(d => d.ID == configID);
            if (this.ConfigInfo == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + configID + "】编目定义对象，无法实例化编目节点对象");
            this.InstanceDB = SQLHelper.CreateSqlHelper(this.Space.SpaceKey,this.Space.ConnectString);
            this.IsNewModel = true;
        }

        public S_NodeInfo(DataRow row)
        {
            this._initEntity(); _validateColumns(row);
            DataAdapter.DataRowToHashTable(row, this.DataEntity);
            this.Space = DocConfigHelper.CreateConfigSpaceByID(this.DataEntity["SpaceID"].ToString());
            if (this.Space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + this.DataEntity["SpaceID"].ToString() + "】档案配置对象，无法实例化编目节点对象");
            this.ConfigInfo = this.Space.S_DOC_Node.FirstOrDefault(d => d.ID == this.DataEntity["ConfigID"].ToString());
            if (this.ConfigInfo == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + this.DataEntity["ConfigID"].ToString() + "】编目定义对象，无法实例化编目节点对象");
            this.InstanceDB = SQLHelper.CreateSqlHelper(this.Space.SpaceKey,this.Space.ConnectString);
            if (Tool.IsNullOrEmpty(this.ID))
                this.IsNewModel = true;
        }

        public S_NodeInfo(string ID, S_DOC_Space space)
        {
            this._initEntity();
            if (space == null)
                throw new Formula.Exceptions.BusinessException("档案配置对象未空，无法实例化编目节点对象");
            this.Space = space;
            this.InstanceDB = SQLHelper.CreateSqlHelper(this.Space.SpaceKey,this.Space.ConnectString);
            string sql = String.Format("select * from {0} where ID='{1}' ", TableName, ID);
            var dt = this.InstanceDB.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                _validateColumns(row);
                DataAdapter.DataRowToHashTable(row, this.DataEntity);
                this.ConfigInfo = this.Space.S_DOC_Node.FirstOrDefault(d => d.ID == this.DataEntity["ConfigID"].ToString());
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
                var splitStr = this.ConfigInfo.ExtentionObject.GetValue("Ext_Car_FullNameSplit");
                if (string.IsNullOrEmpty(splitStr)) splitStr = " > ";
                resultName = string.Join(splitStr, this.Seniorities.Select(a => a.Name).ToList());
            }
            return resultName;
        }

        /// <summary>
        /// 删除编目节点对象
        /// </summary>
        public override void Delete()
        {
            foreach (var child in this.AllChildren)
                child.remove();
            this.remove();
        }

        protected void remove()
        {
            this.ClearFiles();
            this.DataEntity.DeleteDB(this.InstanceDB, TableName, this.ID);
        }

        /// <summary>
        /// 清楚文件
        /// </summary>
        public void ClearFiles()
        {
            string sql = string.Empty;
            sql += " delete from S_Attachment where NodeID='" + this.ID + "';";
            sql += " delete from S_FileInfo where NodeID='" + this.ID + "'";
            this.InstanceDB.ExecuteDataTable(sql);
        }

        public void CheckFulltextProp()
        {
            //如果当前对象存在全文检索属性，则变更全文检索文件状态，以便重新插入Es
            if (this.ConfigInfo == null) throw new Formula.Exceptions.BusinessException("无法获取配置信息，保存失败");
            if (this.ConfigInfo.S_DOC_NodeAttr.Any(d => d.FulltextProp == "True"))
            {
                var allNodeIDs = this.AllChildren.Select(a => a.ID);
                var ids = string.Join("','", allNodeIDs);
                this.InstanceDB.ExecuteNonQuery(" update S_Attachment set State='Normal' where NodeID in ('" + ids + "') and CurrentVersion='True'   ");
            }
        }

        public override void Save(bool withDefaultAttr = true)
        {
            this.ValidateDataAttr(this.IsNewModel);
            if (this.IsNewModel)
            {
                if (this.IsRoot)
                {
                    if (String.IsNullOrEmpty(this.ID))
                        this.ID = FormulaHelper.CreateGuid();
                    this.DataEntity["FullPathID"] = this.ID;
                    if (!this.DataEntity.ContainsKey("SpaceID") || Tool.IsNullOrEmpty(this.DataEntity["SpaceID"]))
                        throw new Formula.Exceptions.BusinessException("未指定档案空间ID，无法新增编目节点数据");
                    this.DataEntity["State"] = "Normal";
                    if (this.DataEntity.ContainsKey("SortIndex") && Tool.IsNullOrEmpty(this.DataEntity["SortIndex"]))
                        this.SortIndex = 0;
                    this.DataEntity.InsertDB(this.InstanceDB, TableName, this.ID);
                }
                else
                {
                    if (this.Parent == null)
                    {
                        if (!this.DataEntity.ContainsKey("SpaceID") || Tool.IsNullOrEmpty(this.DataEntity["SpaceID"]))
                            throw new Formula.Exceptions.BusinessException("未指定档案空间ID，无法新增编目节点数据");
                        if (String.IsNullOrEmpty(this.ID))
                            this.ID = FormulaHelper.CreateGuid();
                        this.DataEntity["FullPathID"] = this.ID;
                        this.DataEntity["State"] = "Normal";
                        if (this.DataEntity.ContainsKey("SortIndex") && Tool.IsNullOrEmpty(this.DataEntity["SortIndex"]))
                            this.SortIndex = 0;
                        this.DataEntity.InsertDB(this.InstanceDB, TableName, this.ID);
                    }
                    else
                        this.Parent.AddChild(this, withDefaultAttr);
                }
            }
            else
            {
                //if (withDefaultAttr) this.FillConfigDefaultAttr();
                this.DataEntity.UpdateDB(this.InstanceDB, TableName, this.ID);
                CheckFulltextProp();
            }
        }

        public void AddChild(S_NodeInfo node, bool withDefaultAttr = false)
        {
            if (String.IsNullOrEmpty(node.ID))
                node.ID = FormulaHelper.CreateGuid();
            if (!this.ConfigInfo.StructNode.Children.Exists(d => d.NodeID == node.ConfigInfo.ID)
                && this.ConfigInfo.IsFreeNode != TrueOrFalse.True.ToString())
                throw new Formula.Exceptions.BusinessException("【" + this.ConfigInfo.Name + "】下不能增加【" + node.ConfigInfo.Name + "】节点");
            node.DataEntity["ParentID"] = this.ID;
            node.DataEntity["FullPathID"] = this.FullPathID + "." + node.ID;
            node.DataEntity["SpaceID"] = this.Space.ID;
            node.DataEntity["State"] = "Normal";
            if (node.DataEntity.ContainsKey("SortIndex"))
            {
                var maxIndex = 0;
                if (this.Children.Count > 0)
                    this.Children.Max(a => a.SortIndex);
                node.SortIndex = ++maxIndex;
            }
            if (withDefaultAttr)
                node.FillConfigDefaultAttr();
            node.ValidateDataAttr(true);
            node.DataEntity.InsertDB(this.InstanceDB, this.TableName, node.ID);
            this.Children.Add(node);
            this.AllChildren.Add(node);
        }

        public void AddChildren(List<S_NodeInfo> nodes)
        {
            foreach (var node in nodes)
                AddChild(node);
        }

        public void AddAttachment(S_Attachment attachment)
        {
            if (!attachment.IsNewModel) throw new Formula.Exceptions.BusinessException("附件对象不是新建对象，无法调用添加方法");
            attachment.NodeID = this.ID;
            attachment.CurrentVersion = "True";
            attachment.Version = this.getMaxAttachVersion();
            attachment.Save();
            this.Attachments.Add(attachment);
        }

        public void UpdateAttachment(S_Attachment attachment)
        {
            if (attachment.IsNewModel) throw new Formula.Exceptions.BusinessException("附件对象新建对象，无法调用更新方法");
            attachment.Save();
            var attch = this.Attachments.FirstOrDefault(d => d.ID == attachment.ID);

        }

        public void AddFile(S_FileInfo file, bool withDefaultAttr = false)
        {
            file.NodeID = this.ID;
            file.FullNodeID = this.FullPathID;
            if (!file.IsNewModel) throw new Formula.Exceptions.BusinessException("文件对象不是新建对象，无法调用添加方法");
            file.Save(withDefaultAttr);
            this.FileInfos.Add(file);
        }

        public void AddFiles(List<S_FileInfo> files)
        {
            foreach (var file in files)
                AddFile(file);
        }

        public void Publish()
        {
            string sql = "update S_NodeInfo set State='{0}' where FullPathID like '{1}%';";
            sql += "update S_FileInfo set State='{0}' where FullNodeID like '{1}%';";
            this.InstanceDB.ExecuteNonQuery(String.Format(sql, DocState.Published.ToString(), this.FullPathID));
            CheckFulltextProp();
        }

        public void Recover()
        {
            string sql = "update S_NodeInfo set State='{0}' where FullPathID like '{1}%';";
            sql += "update S_FileInfo set State='{0}' where FullNodeID like '{1}%';";
            this.InstanceDB.ExecuteNonQuery(String.Format(sql, DocState.Normal.ToString(), this.FullPathID));
        }

        public void Invalid()
        {
            string sql = "update S_NodeInfo set State='{0}' where FullPathID like '{1}%';";
            sql += "update S_FileInfo set State='{0}' where FullNodeID like '{1}%';";
            this.InstanceDB.ExecuteNonQuery(String.Format(sql, DocState.Invalid.ToString(), this.FullPathID));
        }

        public void FillConfigDefaultAttr()
        {
            if (this.ConfigInfo == null) throw new Formula.Exceptions.BusinessException("无法获取配置信息，保存失败");
            var attrs = this.ConfigInfo.S_DOC_NodeAttr.Where(d => !String.IsNullOrEmpty(d.DefaultValue)).ToList();
            foreach (var attr in attrs)
            {
                if (attr.DefaultValue.IndexOf("{") < 0)
                    this.DataEntity[attr.AttrField] = attr.DefaultValue;
                else
                {
                    if (attr.DefaultValue.IndexOf("{") >= 0)
                    {
                        var CurrentUserInfo = FormulaHelper.GetUserInfo();
                       // var user = UserState.CreateUserState(HttpContext.Current.User.Identity.Name);
                        var defaultStr = attr.DefaultValue.Replace("{", "").Replace("}", "");
                        if (defaultStr == "Now")
                            this.DataEntity[attr.AttrField] = DateTime.Now;
                        else if (defaultStr == "UserID")
                            this.DataEntity[attr.AttrField] = CurrentUserInfo.UserID;
                        else if (defaultStr == "UserName")
                            this.DataEntity[attr.AttrField] = CurrentUserInfo.UserName;
                        if (!String.IsNullOrEmpty(this.DataEntity.GetValue("ParentID")))
                        {
                            var defaultValue = attr.DefaultValue.Replace("{", "").Replace("}", "").Split(':');
                            if (defaultValue.Length > 1)
                            {
                                var nodePositon = defaultValue[0];
                                var defaultField = defaultValue[1];
                                if (nodePositon == "Root")
                                {
                                    if (this.RootNode.DataEntity.ContainsKey(defaultField))
                                        this.DataEntity[attr.AttrField] = this.RootNode.DataEntity[defaultField];
                                }
                                else
                                {
                                    S_NodeInfo node = _getParentNode(this.Parent, nodePositon.Split('.').Length);
                                    if (node.DataEntity.ContainsKey(defaultField))
                                        this.DataEntity[attr.AttrField] = node.DataEntity[defaultField];
                                }
                            }
                            else
                            {
                                var defaultField = defaultValue[0];
                                if (this.DataEntity.ContainsKey(defaultField))
                                    this.DataEntity[attr.AttrField] = this.DataEntity[defaultField];
                            }
                        }
                    }
                }
            }
        }

        public void SynchronizeAttr()
        {
            var sycChildren = this.AllChildren.Where(d => d.ConfigInfo.S_DOC_NodeAttr.Count(c =>!String.IsNullOrEmpty(c.DefaultValue)
                && c.DefaultValue.StartsWith("{"))>0).ToList();
            foreach (var item in sycChildren)
            {
                item.Save(true);
            }

            //var files = this.InstanceDB.ExecuteDataTable(" SELECT * FROM  S_FileInfo WHERE FULLNODEID LIKE '" + this.FullPathID + "%'");

        }

        public void ValidateDataAttr(bool isNew = true)
        {
            var valiateAttr = this.ConfigInfo.S_DOC_NodeAttr.Where(d => d.ValidateType != ValidateType.None.ToString()).ToList();
            var uniqueList = valiateAttr.Where(d => d.ValidateType == ValidateType.Unique.ToString()).ToList();
            var typeUniqueList = valiateAttr.Where(d => d.ValidateType == ValidateType.TypeUnique.ToString()).ToList();
            var bortherUniqueList = valiateAttr.Where(d => d.ValidateType == ValidateType.BortherUnique.ToString()).ToList();
            string sql = " SELECT COUNT(0) FROM " + this.TableName + " WHERE 1=1";
            string whereStr = string.Empty;

            string attrNames = string.Empty;
            foreach (var item in uniqueList)
            {
                whereStr += item.AttrField + " = '" + this.DataEntity.GetValue(item.AttrField) + "' OR ";
                attrNames += item.AttrName + ",";
            }

            if (!String.IsNullOrEmpty(whereStr))
            {
                whereStr = whereStr.Trim().TrimEnd(new char[] { 'R', 'O' });
                if (!isNew) whereStr += ") AND ID !='" + this.ID + "'";
                if (Convert.ToInt32(this.InstanceDB.ExecuteScalar(sql + "AND (" + whereStr)) > 0)
                    throw new Formula.Exceptions.BusinessException("请确认属性【" + attrNames.TrimEnd(',') + "】中，不存在重复的内容");
            }

            attrNames = string.Empty; whereStr = string.Empty;
            foreach (var item in typeUniqueList)
            {
                whereStr += item.AttrField + " = '" + this.DataEntity.GetValue(item.AttrField) + "' OR ";
                attrNames += item.AttrName + ",";
            }
            if (!String.IsNullOrEmpty(whereStr))
            {
                whereStr = whereStr.Trim().TrimEnd(new char[] { 'R', 'O' });
                whereStr += ") AND CONFIGID='" + this.ConfigInfo.ID + "' ";
                if (!isNew) whereStr += " AND ID !='" + this.ID + "'";
                if (Convert.ToInt32(this.InstanceDB.ExecuteScalar(sql + "AND (" + whereStr)) > 0)
                    throw new Formula.Exceptions.BusinessException("请确认属性【" + attrNames.TrimEnd(',') + "】中，在" + this.ConfigInfo.Name + "范围内不存在重复的内容");
            }

            attrNames = string.Empty; whereStr = string.Empty;
            foreach (var item in bortherUniqueList)
            {
                whereStr += item.AttrField + " = '" + this.DataEntity.GetValue(item.AttrField) + "' OR ";
                attrNames += item.AttrName + ",";
            }
            if (!String.IsNullOrEmpty(whereStr))
            {
                whereStr = whereStr.Trim().TrimEnd(new char[] { 'R', 'O' });
                whereStr += ") AND PARENTID='" + this.DataEntity.GetValue("ParentID") + "' ";
                if (!isNew) whereStr += " AND ID !='" + this.ID + "'";
                if (Convert.ToInt32(this.InstanceDB.ExecuteScalar(sql + "AND (" + whereStr)) > 0)
                    throw new Formula.Exceptions.BusinessException("请确认属性【" + attrNames.TrimEnd(',') + "】中，在同一父节点下不存在重复的内容");
            }
        }

        public void MoveUp(string targetNodeID)
        {
            if (!this.DataEntity.ContainsKey("SortIndex"))
                return;
            if (string.IsNullOrEmpty(targetNodeID))
            {
                this.SortIndex++;
                this.Save(false);
            }
            else
            {
                var targert = this.Neighbors.FirstOrDefault(a=>a.ID==targetNodeID);
                if(targert==null)
                    throw new Formula.Exceptions.BusinessException("只能移动至同级节点");
                var targetIndex = this.Neighbors.IndexOf(targert);
                this.Neighbors.Insert(++targetIndex, this);
                var sb = new StringBuilder();
                string sql = "update S_NodeInfo set SortIndex='{0}' where ID='{1}'";
                var _index = 1;
                foreach (var item in this.Neighbors)
                {
                    sb.AppendFormat(sql, _index++, item.ID);
                    sb.AppendLine();
                }
                this.InstanceDB.ExecuteNonQuery(sb.ToString());
            }
        }

        public void MoveDown(string targetNodeID)
        {
            if (!this.DataEntity.ContainsKey("SortIndex"))
                return;
            if (string.IsNullOrEmpty(targetNodeID))
            {
                this.SortIndex--;
                this.Save(false);
            }
            else
            {
                var targert = this.Neighbors.FirstOrDefault(a => a.ID == targetNodeID);
                if (targert == null)
                    throw new Formula.Exceptions.BusinessException("只能移动至同级节点");
                var targetIndex = this.Neighbors.IndexOf(targert);
                this.Neighbors.Insert(targetIndex, this);
                var sb = new StringBuilder();
                string sql = "update S_NodeInfo set SortIndex='{0}' where ID='{1}'";
                var _index = 1;
                foreach (var item in this.Neighbors)
                {
                    sb.AppendFormat(sql, _index++, item.ID);
                    sb.AppendLine();
                }
                this.InstanceDB.ExecuteNonQuery(sb.ToString());
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
            this.DataEntity["ParentID"] = "";
            this.DataEntity["FullPathID"] = "";
            this.DataEntity["ConfigID"] = "";
            this.DataEntity["CreateTime"] = DateTime.Now;
            this.DataEntity["State"] = "";
            this.DataEntity["SortIndex"] = 0;
        }

        void _validateColumns(DataRow row)
        {
            if (!row.Table.Columns.Contains("SpaceID"))
                throw new Formula.Exceptions.BusinessException("未能找到SpaceID列，无法实例化节点对象");
            if (!row.Table.Columns.Contains("ConfigID"))
                throw new Formula.Exceptions.BusinessException("未能找到ConfigID列，无法实例化节点对象");
            if (!row.Table.Columns.Contains("ParentID"))
                throw new Formula.Exceptions.BusinessException("未能找到ParentID列，无法实例化节点对象");
            if (!row.Table.Columns.Contains("FullPathID"))
                throw new Formula.Exceptions.BusinessException("未能找到FullPathID列，无法实例化节点对象");
        }

        int getMaxAttachVersion()
        {
            if (this.Attachments.Count == 0) return 1;
            return this.Attachments.Max(d => d.Version) + 1;
        }

        S_NodeInfo _getParentNode(S_NodeInfo parent, int len)
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

        /// <summary>
        /// 创建空的编目节点对象
        /// </summary>
        /// <param name="spaceID"></param>
        /// <param name="configID"></param>
        /// <returns></returns>
        public static S_NodeInfo CreateEmptyNode(string spaceID, string configID)
        {
            return new S_NodeInfo(spaceID, configID);
        }

        public static S_NodeInfo GetNode(string ID, string spaceID)
        {
            var space = DocConfigHelper.CreateConfigSpaceByID(spaceID);
            if (space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + spaceID + "】档案配置对象，无法获取节点对象");
           // var db = SQLHelper.CreateSqlHelper(ConnEnum.DocConfig);
            var db = SQLHelper.CreateSqlHelper(space.SpaceKey, space.ConnectString);
            string sql = "select * from S_NodeInfo where ID='" + ID + "' ";
            var dt = db.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new S_NodeInfo(row);
            }
            else
            {
                throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ID + "】的编目节点");

            }


        }

        public static S_NodeInfo GetNode(string spaceID, string configID, string whereStr)
        {
            var space = DocConfigHelper.CreateConfigSpaceByID(spaceID);
            if (space == null)
                throw new Formula.Exceptions.BusinessException("未找到ID为【" + spaceID + "】档案配置对象，无法获取节点对象");
            string where = whereStr.Replace("where", "").Replace("WHERE", "");
            var db = SQLHelper.CreateSqlHelper(space.SpaceKey, space.ConnectString);
            string sql = string.Empty;
            if (!String.IsNullOrEmpty(configID))
                sql = String.Format(" select * from S_NodeInfo where ConfigID='{0}'", configID);
            else
                sql = " select * from S_NodeInfo where 1=1 ";
            if (!String.IsNullOrEmpty(where))
                sql += " and " + where;
            var dt = db.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new S_NodeInfo(row);
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
            var db = SQLHelper.CreateSqlHelper(space.SpaceKey, space.ConnectString);

            string sql = String.Format(" select ID from S_NodeInfo where ConfigID='{0}'", configID);
            if (!String.IsNullOrEmpty(where))
                sql += " and " + where;
            var dt = db.ExecuteDataTable(sql);
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
            get { return "S_NodeInfo"; }
        }
    }
}
