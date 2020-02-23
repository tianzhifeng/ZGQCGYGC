using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Config;
using Formula;
using MvcAdapter;
using Formula.Helper;

namespace EPC.Logic.Domain
{
    public partial class S_M_BidOffer_CBS
    {
        #region 公共属性

        S_M_BidOffer_CBS _parent;
        /// <summary>
        /// 父节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_M_BidOffer_CBS Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this.S_M_BidOffer.S_M_BidOffer_CBS.FirstOrDefault(d => d.CBSID == this.CBSParentID);
                }
                return _parent;
            }
        }

        List<S_M_BidOffer_CBS> _ancestor;
        /// <summary>
        /// 所有上级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_M_BidOffer_CBS> Ancestor
        {
            get
            {
                if (_ancestor == null)
                {
                    _ancestor = this.S_M_BidOffer.S_M_BidOffer_CBS.Where(d => this.CBSFullID.StartsWith(d.CBSFullID)).OrderBy(d => d.CBSFullID).ToList();
                }
                return _ancestor;
            }
        }

        List<S_M_BidOffer_CBS> _children;
        /// <summary>
        /// 下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_M_BidOffer_CBS> Children
        {
            get
            {
                if (_children == null || _children.Count == 0)
                {
                    _children = this.S_M_BidOffer.S_M_BidOffer_CBS.Where(d => d.CBSParentID == this.CBSID).OrderBy(d => d.CBSFullID).ToList();
                }
                return _children;
            }
        }

        List<S_M_BidOffer_CBS> _allchildren;
        /// <summary>
        /// 所有下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_M_BidOffer_CBS> AllChildren
        {
            get
            {
                if (this._allchildren == null || this._allchildren.Count == 0)
                {
                    this._allchildren = this.S_M_BidOffer.S_M_BidOffer_CBS.Where(w => w.CBSFullID.StartsWith(this.CBSFullID) && w.CBSID != this.CBSID).OrderBy(d => d.CBSFullID).ToList();
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
                if(_CBSTemplateNode==null)
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
