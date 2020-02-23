using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Data;
using Config;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
using Formula.Helper;


namespace EPC.Logic.Domain
{
    public partial class S_I_CBS
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


        S_T_CBSNodeTemplate _structNodeInfo;
        /// <summary>
        /// 结构节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_CBSNodeTemplate StructNodeInfo
        {
            get
            {
                var dbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
                if (_structNodeInfo == null)
                    _structNodeInfo = dbContext.S_T_CBSNodeTemplate.FirstOrDefault(c => c.ID == this.CBSDefineID);
                return _structNodeInfo;
            }
        }

        S_I_CBS _parent;
        /// <summary>
        /// 父节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_CBS Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this.S_I_Engineering.S_I_CBS.FirstOrDefault(d => d.ID == this.ParentID);
                }
                return _parent;
            }
        }

        List<S_I_CBS> _ancestor;
        /// <summary>
        /// 所有上级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_CBS> Ancestor
        {
            get
            {
                if (_ancestor == null)
                {
                    _ancestor = this.S_I_Engineering.S_I_CBS.Where(d => this.FullID.StartsWith(d.FullID)).OrderBy(d => d.FullID).ToList();
                }
                return _ancestor;
            }
        }

        /// <summary>
        /// 根节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_CBS RootNode
        {
            get
            {
                return this.S_I_Engineering.CBSRoot;
            }
        }

        List<S_I_CBS> _children;
        /// <summary>
        /// 下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_CBS> Children
        {
            get
            {
                if (_children == null || _children.Count == 0)
                {
                    _children = this.S_I_Engineering.S_I_CBS.Where(d => d.ParentID == this.ID).OrderBy(d => d.SortIndex).ToList();
                }
                return _children;
            }
        }

        List<S_I_CBS> _allchildren;
        /// <summary>
        /// 所有下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_CBS> AllChildren
        {
            get
            {
                if (this._allchildren == null || this._allchildren.Count == 0)
                {
                    this._allchildren = this.S_I_Engineering.S_I_CBS.Where(w => w.FullID.StartsWith(this.FullID) && w.ID != this.ID).OrderBy(d => d.SortIndex).ToList();
                }
                return _allchildren;
            }
        }

        S_T_CBSNodeTemplate _CBSTemplateNode;
        [NotMapped]
        [JsonIgnore]
        public S_T_CBSNodeTemplate CBSTemplateNode
        {
            get
            {
                if (_CBSTemplateNode == null)
                {
                    var dbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
                    _CBSTemplateNode = dbContext.S_T_CBSNodeTemplate.FirstOrDefault(d => d.ID == this.CBSDefineID);
                }
                return _CBSTemplateNode;
            }
        }
        #endregion
    }
}
