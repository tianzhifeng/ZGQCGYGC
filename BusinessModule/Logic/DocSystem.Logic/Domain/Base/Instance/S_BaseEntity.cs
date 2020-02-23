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
    public abstract class S_BaseEntity
    {
        public abstract string TableName { get; }

        #region 公共属性

        SQLHelper _instanceDB;
        /// <summary>
        /// 数据访问对象
        /// </summary>
        public SQLHelper InstanceDB
        {
            get { return this._instanceDB; }
            set { this._instanceDB = value; }
        }

        private S_DOC_Space _space;
        /// <summary>
        /// 档案配置空间
        /// </summary>
        public S_DOC_Space Space
        {
            get { return _space; }
            set
            {
                if (value != null)
                {
                    _space = value;
                    entity["SpaceID"] = value.ID;
                    if (_instanceDB == null)
                        _instanceDB = SQLHelper.CreateSqlHelper(value.SpaceKey, value.ConnectString); 
                }
            }
        }

        Dictionary<string, object> entity;
        /// <summary>
        /// 节点数据信息
        /// </summary>
        public Dictionary<string,object> DataEntity
        {
            get { return entity; }
            set { entity = value; }
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get
            {
                return this.DataEntity["ID"].ToString();
            }
            set
            {
                this.DataEntity["ID"] = value;
            }
        }

        /// <summary>
        /// 编目名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.DataEntity["Name"].ToString();
            }
            set
            {
                this.DataEntity["Name"] = value;
            }
        }

        private bool _isNewModel = false;
        /// <summary>
        /// 是否是新增数据
        /// </summary>
        public bool IsNewModel
        {
            get { return _isNewModel; }
            set { _isNewModel = value; }
        }

        public DateTime CreateTime
        {
            get
            {
                if (this.DataEntity.ContainsKey("CreateTime") &&
                    this.DataEntity["CreateTime"] != null)
                    return Convert.ToDateTime(this.DataEntity["CreateTime"]);
                else
                    return DateTime.Now;
            }
            set
            {
                this.DataEntity["CreateTime"] = value;
            }
        }

        #endregion

        /// <summary>
        /// 更新实体数据对象
        /// </summary>
        /// <param name="dataEntity"></param>
        public virtual void SetData(Dictionary<string,object> dataEntity)
        {
            foreach (var objDE in dataEntity)
            {
                if (objDE.Value != null && objDE.Value != DBNull.Value)
                {
                    if (objDE.Key.ToString() == "ID") continue;
                    this.DataEntity[objDE.Key.ToString()] = objDE.Value;
                }
            }
        }

        /// <summary>
        /// 保存至数据库
        /// </summary>
        public virtual void Save(bool withDefaultAttr = false)
        {
            if (this.IsNewModel)
            {
                if (!this.DataEntity.ContainsKey("CreateTime") || this.DataEntity["CreateTime"] == null)
                    this.DataEntity["CreateTime"] = DateTime.Now;
                this.entity.InsertDB(this.InstanceDB, TableName, this.ID);
            }
            else
                this.entity.UpdateDB(this.InstanceDB, TableName, this.ID);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        public virtual void Delete()
        {
            this.DataEntity.DeleteDB(this.InstanceDB, TableName, this.ID);
        }
    }
}
