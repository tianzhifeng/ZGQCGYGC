using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

using Config.Logic;
using Config;
using Formula;
using Newtonsoft.Json;


namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_NodeStrcut
    {
        S_DOC_NodeStrcut _parent;
        [NotMapped]
        [JsonIgnore]
        public S_DOC_NodeStrcut Parent
        {
            get
            {
                if (_parent == null)
                {
                    if (this.S_DOC_Space == null) return null;
                    _parent = this.S_DOC_Space.S_DOC_NodeStrcut.FirstOrDefault(d => d.ID == this.ParentID);
                }
                return _parent;
            }
        }

        S_DOC_Node _nodeEntity;
        [NotMapped]
        [JsonIgnore]
        public S_DOC_Node NodeEntity
        {
            get
            {
                if (_nodeEntity == null)
                {
                    if (this.S_DOC_Space == null) return null;
                    _nodeEntity = this.S_DOC_Space.S_DOC_Node.FirstOrDefault(d => d.ID == this.NodeID);
                }
                return _nodeEntity;
            }
        }

        List<S_DOC_NodeStrcut> _children;
        [NotMapped]
        [JsonIgnore]
        public List<S_DOC_NodeStrcut> Children
        {
            get
            {
                if (_children == null)
                {
                    if (this.S_DOC_Space == null) return null;
                    _children = this.S_DOC_Space.S_DOC_NodeStrcut.Where(d => d.ParentID == this.ID).ToList();
                }
                return _children;
            }
        }

        List<S_DOC_NodeStrcut> _allchildren;
        [NotMapped]
        [JsonIgnore]
        public List<S_DOC_NodeStrcut> AllChildren
        {
            get
            {
                if (_allchildren == null && this.S_DOC_Space != null)
                    _allchildren = this.S_DOC_Space.S_DOC_NodeStrcut.Where(d => d.FullPathID.StartsWith(this.FullPathID)).ToList();
                return _allchildren;
            }
        }

        public S_DOC_Node GetNodeInfo()
        {
            var context = this.GetDocConfigContext();
            return context.S_DOC_Node.SingleOrDefault(d => d.ID.ToString() == this.NodeID);
            // return configSession.GetEntityByPrimaryKey<S_DOC_Node>(this.NodeID);
        }

        public void AddChild(S_DOC_NodeStrcut child)
        {
            if (this.Children.Exists(d => d.NodeID == child.NodeID))
                throw new Formula.Exceptions.BusinessException("同类型的节点不能重复添加");

            if (String.IsNullOrEmpty(child.NodeID))
                throw new Formula.Exceptions.BusinessException("必须指定节点类别ID");
            child.ParentID = this.ID;
            if (String.IsNullOrEmpty(child.ID))
                child.ID = FormulaHelper.CreateGuid();
            child.FullPathID = this.FullPathID + "." + child.ID;
            child.SpaceID = this.SpaceID;
            this.S_DOC_Space.S_DOC_NodeStrcut.Add(child);
        }

        public void Delete()
        {
            ClearChildren();
            var context = this.GetDocConfigContext();
            context.S_DOC_NodeStrcut.Remove(this);
        }

        public void ClearChildren()
        {
            var context = this.GetDocConfigContext();
            var children = this.AllChildren;
            if (children != null && children.Count > 0)
            {
                foreach (var item in children)
                    context.S_DOC_NodeStrcut.Remove(item);
            }
        }
    }
}
