using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Formula.Helper;
using System.Collections;
using Formula;
using Config.Logic;
using Config;
using Newtonsoft.Json;

namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_Node
    {
        string[] SystemAttrField = new string[] { "Name" };
        string[] SystemAttrName = new string[] { "名称" };

        /// <summary>
        /// 扩展属性
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public Dictionary<string, object> ExtentionObject
        {
            get
            {
                if (string.IsNullOrEmpty(this.ExtentionJson))
                    return new Dictionary<string, object>();
                else
                    return JsonHelper.ToObject(this.ExtentionJson);
            }
        }

        S_DOC_NodeStrcut _structNode;
        [NotMapped]
        [JsonIgnore]
        public S_DOC_NodeStrcut StructNode
        {
            get
            {
                if (_structNode == null && this.S_DOC_Space != null)
                    _structNode = this.S_DOC_Space.S_DOC_NodeStrcut.FirstOrDefault(d => d.NodeID == this.ID);
                return _structNode;
            }
        }

        List<S_DOC_NodeStrcut> _structs;
        [NotMapped]
        [JsonIgnore]
        public List<S_DOC_NodeStrcut> Structs
        {
            get
            {
                if (_structs == null && this.S_DOC_Space != null)
                    _structs = this.S_DOC_Space.S_DOC_NodeStrcut.Where(d => d.NodeID == this.ID).ToList();
                return _structs;
            }
        }

        public void Delete()
        {
            var context = this.GetDocConfigContext();
            context.S_DOC_NodeAttr.Delete(d => d.NodeID == this.ID);
            context.S_DOC_FileNodeRelation.Delete(d => d.NodeID == this.ID);
            var structList = GetStructList();
            foreach (var item in structList)
                item.Delete();
            var listConfigs = this.GetListConfigs();
            foreach (var listconfig in listConfigs)
                listconfig.Delete();
        }

        public List<S_DOC_NodeStrcut> GetStructList()
        {
            var context = this.GetDocConfigContext();
            var structNodes = context.S_DOC_NodeStrcut.Where(d => d.NodeID == this.ID).ToList();
            return structNodes;
        }

        public void Add()
        {
            var listConfig = new S_DOC_ListConfig();
            listConfig.ID = FormulaHelper.CreateGuid();
            listConfig.RelationID = this.ID;
            listConfig.QueryKeyText = "请输入名称或编号";
            listConfig.Type = ListConfigType.Node.ToString();
            listConfig.SpaceID = this.SpaceID;
            this._InitSystemAtt(listConfig);
            var context = this.GetDocConfigContext();
            context.S_DOC_ListConfig.Add(listConfig);
           
        }

        public void Save()
        {
                   
            var list = this.ListConfig();
            if (list == null)
            {
                var context = this.GetDocConfigContext();
                list = new S_DOC_ListConfig();
                list.ID = FormulaHelper.CreateGuid();
                list.RelationID = this.ID;
                list.QueryKeyText = "请输入名称或编号";
                list.Type = ListConfigType.File.ToString();
                list.SpaceID = this.SpaceID;
                context.S_DOC_ListConfig.Add(list);
            }
            _InitSystemAtt(list);
        }

        public List<S_DOC_ListConfig> GetListConfigs()
        {
            var context = this.GetDocConfigContext();
            return context.S_DOC_ListConfig.Where(d => d.RelationID == this.ID && d.SpaceID == this.SpaceID).ToList();
        }

        S_DOC_ListConfig _listConfig;
        public S_DOC_ListConfig ListConfig()
        {


            if (_listConfig == null)
            {
                var context = this.GetDocConfigContext();
                _listConfig = context.S_DOC_ListConfig.FirstOrDefault(d => d.RelationID == this.ID && d.SpaceID == this.SpaceID);
            }
            return _listConfig;
        }

        public void SaveNodeAttr(List<Dictionary<string, object>> Attrlist)
        {
            var listconfig = this.ListConfig();
            foreach (var item in Attrlist)
            {
                var context = this.GetDocConfigContext();
                if (!item.ContainsKey("AttrField") || Tool.IsNullOrEmpty(item["AttrField"])) continue;
                string fileName = item.GetValue("AttrField");
                S_DOC_NodeAttr entity;
                string ID = item.GetValue("ID");
                if (!String.IsNullOrEmpty(ID))
                    entity = context.S_DOC_NodeAttr.SingleOrDefault(d => d.ID == ID);
                else
                {                   
                    entity = this.S_DOC_NodeAttr.FirstOrDefault(d => d.AttrField == fileName);
                    if (entity != null) throw new Formula.Exceptions.BusinessException("已经存在字段名称为【"+fileName+"】的属性，不能重复添加");
                }
                if (item.GetValue("IsEnum") == TrueOrFalse.True.ToString()
                    &&string.IsNullOrEmpty(item.GetValue("EnumKey")))
                    throw new Formula.Exceptions.BusinessException("枚举字段【" + item.GetValue("AttrName") + "】必须指定枚举Key");
                if (entity == null)
                {
                    entity = new S_DOC_NodeAttr();
                    entity.ID = FormulaHelper.CreateGuid();
                    entity.AttrSort = this.S_DOC_NodeAttr.Count + 1;
                    entity.SpaceID = this.SpaceID;
                    if (String.IsNullOrEmpty(entity.AttrType))
                        entity.AttrType = Logic.Domain.AttrType.Custom.ToString();
                    if (String.IsNullOrEmpty(entity.Visible))
                        entity.Visible = TrueOrFalse.True.ToString();
                    if (String.IsNullOrEmpty(entity.Disabled))
                        entity.Disabled = TrueOrFalse.False.ToString();
                    if (String.IsNullOrEmpty(entity.FulltextProp))
                        entity.FulltextProp = TrueOrFalse.False.ToString();
                    this.S_DOC_NodeAttr.Add(entity);
                }
                Tool.UpdateHashtableInstance<S_DOC_NodeAttr>(entity, item);
  
            }
        }

        public void AddListConfigDetail(S_DOC_NodeAttr item)
        {
            var entity = this.ListConfig().S_DOC_ListConfigDetail.FirstOrDefault(d => d.AttrField == item.AttrField);
            if (entity == null)
            {
                entity = new S_DOC_ListConfigDetail();
                entity.AttrName = item.AttrName;
                entity.AttrField = item.AttrField;
                entity.AllowSort = TrueOrFalse.True.ToString();
                entity.Align = Algin.center.ToString();
                entity.Dispaly = TrueOrFalse.True.ToString();
                entity.Width = 150;
                if (this.ListConfig().S_DOC_ListConfigDetail.Count > 0)
                    entity.DetailSort = this.ListConfig().S_DOC_ListConfigDetail.Max(d => d.DetailSort) + 1;
                else
                    entity.DetailSort = 1;
                this.ListConfig().S_DOC_ListConfigDetail.Add(entity);
            }
        }

        public void AddFileNode(string fileID)
        {
            var entity = this.S_DOC_FileNodeRelation.FirstOrDefault(d => d.FileID == fileID);
            if (entity == null)
            {
                entity = new S_DOC_FileNodeRelation();
                entity.FileID = fileID;
                entity.Sort = this.S_DOC_FileNodeRelation.Count + 1;
                this.S_DOC_FileNodeRelation.Add(entity);
            }
        }

        public void AddFileNode(S_DOC_File file)
        {
            var entity = this.S_DOC_FileNodeRelation.FirstOrDefault(d => d.FileID == file.ID);
            if (entity == null)
            {
                entity = new S_DOC_FileNodeRelation();
                entity.FileID = file.ID;
                entity.Sort = this.S_DOC_FileNodeRelation.Count + 1;
                this.S_DOC_FileNodeRelation.Add(entity);
            }
        }

        public void DeleteFileNode(string fileID)
        {
            var context = this.GetDocConfigContext();
            var entity = this.S_DOC_FileNodeRelation.FirstOrDefault(d => d.FileID == fileID);
            if (entity != null)
                context.S_DOC_FileNodeRelation.Remove(entity);
          
        }

        public void DeleteFileNode(S_DOC_File file)
        {
            var context = this.GetDocConfigContext();
            var entity = this.S_DOC_FileNodeRelation.FirstOrDefault(d => d.FileID == file.ID);
            if (entity != null)
                context.S_DOC_FileNodeRelation.Remove(entity);
          
        }

        public void SaveListConfigDetail(List<Dictionary<string, object>> detailList)
        {
            var context = this.GetDocConfigContext();
            if (this.ListConfig() == null) throw new Formula.Exceptions.BusinessException("指定的节点【" + this.ID + "】未能找到");
            foreach (var item in detailList)
            {
                if (!item.ContainsKey("AttrField") || Tool.IsNullOrEmpty(item["AttrField"])) continue;
          
                string fileName = item.GetValue("AttrField");

                S_DOC_ListConfigDetail entity;
                string ID = item.GetValue("ID");
                if (!string.IsNullOrEmpty(ID))
                {
                    int id = Convert.ToInt32(ID);
                    entity = context.S_DOC_ListConfigDetail.FirstOrDefault(d => d.ID == id);// configSession.GetEntityByPrimaryKey<S_DOC_ListConfigDetail>(table["ID"].ToString());
                }
                else
                    entity = this.ListConfig().S_DOC_ListConfigDetail.FirstOrDefault(d => d.AttrField == fileName);
                if (entity == null)
                {
                    entity = new S_DOC_ListConfigDetail();
                    entity.AllowSort = TrueOrFalse.True.ToString();
                    entity.Align = Algin.center.ToString();
                    entity.Dispaly = TrueOrFalse.True.ToString();
                    entity.Width = 150;
                    if (this.ListConfig().S_DOC_QueryParam.Count > 0)
                        entity.DetailSort = this.ListConfig().S_DOC_QueryParam.Max(d => d.QuerySort) + 1;
                    else
                        entity.DetailSort = 1;
                    this.ListConfig().S_DOC_ListConfigDetail.Add(entity);
                }
                Tool.UpdateHashtableInstance<S_DOC_ListConfigDetail>(entity, item);
            }
        }

        public void AddQueryParam(S_DOC_NodeAttr item)
        {
            var entity = this.ListConfig().S_DOC_QueryParam.FirstOrDefault(d => d.AttrField == item.AttrField);
            if (entity == null)
            {
                entity = new S_DOC_QueryParam();
                entity.AttrField = item.AttrField;
                entity.AttrName = item.AttrName;
                entity.InKey = TrueOrFalse.False.ToString();
                entity.InnerField = item.AttrField;
                entity.InAdvancedQuery = TrueOrFalse.True.ToString();
                entity.QueryType = QueryType.LK.ToString();
                entity.DataType = item.DataType;
                if (this.ListConfig().S_DOC_QueryParam.Count > 0)
                    entity.QuerySort = this.ListConfig().S_DOC_QueryParam.Max(d => d.QuerySort) + 1;
                else
                    entity.QuerySort = 1;
                this.ListConfig().S_DOC_QueryParam.Add(entity);
            }
        }

        public void SaveQueryParam(List<Dictionary<string, object>> detailList)
        {
            var context = this.GetDocConfigContext();
            if (this.ListConfig() == null) throw new Formula.Exceptions.BusinessException("指定的节点【" + this.ID + "】未能找到");
            foreach (var item in detailList)
            {
                if (!item.ContainsKey("AttrField") || Tool.IsNullOrEmpty(item["AttrField"])) continue;
                string fileName = item.GetValue("AttrField");
                S_DOC_QueryParam entity;
                string ID = item.GetValue("ID");
                if (!string.IsNullOrEmpty(ID))
                {
                    int id = Convert.ToInt32(ID);
                    entity = context.S_DOC_QueryParam.FirstOrDefault(d => d.ID == id); //configSession.GetEntityByPrimaryKey<S_DOC_QueryParam>(table["ID"].ToString());
                }
                else
                    entity = this.ListConfig().S_DOC_QueryParam.FirstOrDefault(d => d.AttrField == fileName);
                if (entity == null)
                {
                    entity = new S_DOC_QueryParam();
                    entity.AttrField = item.GetValue("AttrField");
                    entity.AttrName = item.GetValue("AttrName");
                    entity.InKey = TrueOrFalse.False.ToString();
                    entity.InnerField = item.GetValue("AttrField");
                    entity.QueryType = QueryType.LK.ToString();
                    if (this.ListConfig().S_DOC_QueryParam.Count > 0)
                        entity.QuerySort = this.ListConfig().S_DOC_QueryParam.Max(d => d.QuerySort) + 1;
                    else
                        entity.QuerySort = 1;
                    this.ListConfig().S_DOC_QueryParam.Add(entity);
                }
                Tool.UpdateHashtableInstance<S_DOC_QueryParam>(entity, item);
            }
        }

        private void _InitSystemAtt(S_DOC_ListConfig listConfig)
        {
            for (int i = 0; i < this.SystemAttrField.Length; i++)
            {
                var systemAttr = this.S_DOC_NodeAttr.FirstOrDefault(d => d.AttrField == SystemAttrField[i]);
                if (systemAttr != null)
                    continue;
                systemAttr = new S_DOC_NodeAttr();
                systemAttr.AttrField = SystemAttrField[i];
                systemAttr.AttrName = SystemAttrName[i];
                systemAttr.AttrType = AttrType.System.ToString();
                systemAttr.DataType = AttrDataType.NVarchar200.ToString();
                systemAttr.ID = FormulaHelper.CreateGuid();
                systemAttr.InputType = ControlType.TextBox.ToString();
                systemAttr.IsEnum = TrueOrFalse.False.ToString();
                systemAttr.ValidateType = ValidateType.BortherUnique.ToString();
                systemAttr.Required = TrueOrFalse.True.ToString();
                systemAttr.Disabled = TrueOrFalse.False.ToString();
                systemAttr.Visible = TrueOrFalse.True.ToString();
                systemAttr.VType = "maxLength:500";
                systemAttr.SpaceID = this.SpaceID;
                this.S_DOC_NodeAttr.Add(systemAttr);
                var listDetail = listConfig.S_DOC_ListConfigDetail.FirstOrDefault(d => d.AttrField == SystemAttrField[i]);
                if (listDetail == null)
                {
                    listDetail = new S_DOC_ListConfigDetail();
                    listDetail.AttrField = SystemAttrField[i];
                    listDetail.AttrName = SystemAttrName[i];
                    listDetail.Dispaly = TrueOrFalse.True.ToString();
                    listDetail.AllowSort = TrueOrFalse.True.ToString();
                    listDetail.Align = Algin.center.ToString();
                    listDetail.Width = 150;
                    listDetail.DetailSort = listConfig.S_DOC_ListConfigDetail.Count + 1;
                    listConfig.S_DOC_ListConfigDetail.Add(listDetail);
                }
            }
        }


    }
}
