using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Entity;
using Formula;
using Config;
using Config.Logic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace EPC.Logic.Domain
{
    public partial class S_T_CBSNodeTemplate
    {
        #region 公开属性

        S_T_CBSNodeTemplate _Parent;
        /// <summary>
        /// 父节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_CBSNodeTemplate Parent
        {
            get
            {
                if (_Parent == null)
                {
                    _Parent = this.S_T_CBSDefine.S_T_CBSNodeTemplate.FirstOrDefault(d => d.ID == this.ParentID);
                }
                return _Parent;
            }
        }

        List<S_T_CBSNodeTemplate> _Ancestors;
        /// <summary>
        /// 上级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_CBSNodeTemplate> Ancestors
        {
            get
            {
                if (_Ancestors == null)
                {
                    var dbContext = this.GetDbContext<InfrastructureEntities>();
                    _Ancestors = dbContext.S_T_CBSNodeTemplate.Where(d => this.FullID.StartsWith(d.FullID) && d.ID != this.ID).ToList();
                }
                return _Ancestors;
            }
        }


        List<S_T_CBSNodeTemplate> _Children;
        /// <summary>
        /// 子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_CBSNodeTemplate> Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = this.S_T_CBSDefine.S_T_CBSNodeTemplate.Where(d => d.ParentID == this.ID).ToList();
                }
                return _Children;
            }
        }

        List<S_T_CBSNodeTemplate> _AllChildren;
        /// <summary>
        /// 下级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_T_CBSNodeTemplate> AllChildren
        {
            get
            {
                if (_AllChildren == null)
                {
                    _AllChildren = this.S_T_CBSDefine.S_T_CBSNodeTemplate.Where(d => d.FullID.StartsWith(this.FullID) && d.ID != this.ID).ToList();
                }
                return _AllChildren;
            }
        }

        #endregion
    }
}
