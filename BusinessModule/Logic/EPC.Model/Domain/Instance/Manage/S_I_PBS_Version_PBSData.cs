using Formula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace EPC.Logic.Domain
{
    public partial class S_I_PBS_Version_PBSData
    {
        #region 公共属性

        /// <summary>
        /// 管理模式对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_Mode Mode
        {
            get
            {
                if (this.S_I_PBS_Version == null)
                    throw new Formula.Exceptions.BusinessValidationException("必须属于一个版本，否则无法获取管理模式对象");
                if (this.S_I_PBS_Version.S_I_Engineering == null)
                    throw new Formula.Exceptions.BusinessValidationException("必须属于一个工程，否则无法获取管理模式对象");
                return this.S_I_PBS_Version.S_I_Engineering.Mode;
            }
        }


        S_I_PBS_Version_PBSData _parent;
        /// <summary>
        /// 父节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_PBS_Version_PBSData Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this.S_I_PBS_Version.S_I_PBS_Version_PBSData.FirstOrDefault(d => d.PBSID == this.PBSParentID);
                }
                return _parent;
            }
        }

        List<S_I_PBS_Version_PBSData> _ancestor;
        /// <summary>
        /// 所有上级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_PBS_Version_PBSData> Ancestor
        {
            get
            {
                if (_ancestor == null)
                {
                    _ancestor = this.S_I_PBS_Version.S_I_PBS_Version_PBSData.Where(d => this.PBSFullID.StartsWith(d.PBSFullID)).OrderBy(d => d.PBSFullID).ToList();
                }
                return _ancestor;
            }
        }


        S_C_PBSStruct _structNodeInfo;

        /// <summary>
        /// 结构节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_PBSStruct StructNodeInfo
        {
            get
            {
                if (_structNodeInfo == null)
                    _structNodeInfo = this.Mode.S_C_PBSStruct.FirstOrDefault(d => d.ID == this.StructNodeID);
                return _structNodeInfo;
            }
        }

        List<S_I_PBS_Version_PBSData> _children;
        /// <summary>
        /// 下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_PBS_Version_PBSData> Children
        {
            get
            {
                if (_children == null || _children.Count == 0)
                {
                    _children = this.S_I_PBS_Version.S_I_PBS_Version_PBSData.Where(d => d.PBSParentID == this.PBSID).OrderBy(d => d.SortIndex).ToList();
                }
                return _children;
            }
        }

        List<S_I_PBS_Version_PBSData> _allchildren;
        /// <summary>
        /// 所有下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_PBS_Version_PBSData> AllChildren
        {
            get
            {
                if (this._allchildren == null || this._allchildren.Count == 0)
                {
                    this._allchildren = this.S_I_PBS_Version.S_I_PBS_Version_PBSData.Where(w => w.PBSFullID.StartsWith(this.PBSFullID) && w.ID != this.ID).OrderBy(d => d.SortIndex).ToList();
                }
                return _allchildren;
            }
        }

        [NotMapped]
        public bool CanAdd
        {
            get
            {
                if (this.StructNodeInfo == null)
                    return false;
                if (this.StructNodeInfo.Children == null || this.StructNodeInfo.Children.Count == 0)
                    return false;
                return true;
            }
        }

        [NotMapped]
        public bool CanDelete
        {
            get
            {
                if (this.NodeType == "Root")
                    return false;
                return true;
            }
        }

        [NotMapped]
        public bool CanParentAdd
        {
            get
            {
                if (this.StructNodeInfo == null)
                    return false;
                if (this.NodeType == "Root")
                    return false;
                return true;
            }
        }


        #endregion
    }
}
