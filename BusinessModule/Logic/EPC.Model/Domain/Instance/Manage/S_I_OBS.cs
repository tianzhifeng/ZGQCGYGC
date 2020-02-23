using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
using Formula.Helper;
using System.ComponentModel.DataAnnotations;


namespace EPC.Logic.Domain
{
    public partial class S_I_OBS
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
                if (this.S_I_Engineering == null)
                    throw new Formula.Exceptions.BusinessValidationException("WBS必须指定所属的项目对象，否则无法获取管理模式对象");
                return this.S_I_Engineering.Mode;
            }
        }

        S_I_OBS _parent;
        /// <summary>
        /// 父节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_OBS Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this.S_I_Engineering.S_I_OBS.FirstOrDefault(d => d.ID == this.ParentID);
                }
                return _parent;
            }
        }

        List<S_I_OBS> _ancestor;
        /// <summary>
        /// 所有上级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_OBS> Ancestor
        {
            get
            {
                if (_ancestor == null)
                {
                    _ancestor = this.S_I_Engineering.S_I_OBS.Where(d => this.FullID.StartsWith(d.FullID)).OrderBy(d => d.FullID).ToList();
                }
                return _ancestor;
            }
        }

        /// <summary>
        /// 根节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_OBS RootNode
        {
            get
            {
                return this.S_I_Engineering.S_I_OBS.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
            }
        }

        List<S_I_OBS> _children;
        /// <summary>
        /// 下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_OBS> Children
        {
            get
            {
                if (_children == null || _children.Count == 0)
                {
                    _children = this.S_I_Engineering.S_I_OBS.Where(d => d.ParentID == this.ID).OrderBy(d => d.SortIndex).ToList();
                }
                return _children;
            }
        }

        List<S_I_OBS> _allchildren;
        /// <summary>
        /// 所有下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_OBS> AllChildren
        {
            get
            {
                if (this._allchildren == null || this._allchildren.Count == 0)
                {
                    this._allchildren = this.S_I_Engineering.S_I_OBS.Where(w => w.FullID.StartsWith(this.FullID) && w.ID != this.ID).OrderBy(d => d.SortIndex).ToList();
                }
                return _allchildren;
            }
        }
        #endregion
    }
}
