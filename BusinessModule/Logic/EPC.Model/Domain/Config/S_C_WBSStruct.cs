using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace EPC.Logic.Domain
{
    public partial class S_C_WBSStruct
    {
        #region 公开属性

        S_C_WBSStruct _Parent;
        /// <summary>
        /// 父节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_WBSStruct Parent
        {
            get
            {
                if (_Parent == null)
                {
                    _Parent = this.S_C_Mode.S_C_WBSStruct.FirstOrDefault(d => d.ID == this.ParentID);
                }
                return _Parent;
            }
        }

        List<S_C_WBSStruct> _Ancestors;
        /// <summary>
        /// 上级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_WBSStruct> Ancestors
        {
            get
            {
                if (_Ancestors == null)
                {
                    var dbContext = this.GetDbContext<InfrastructureEntities>();
                    _Ancestors = dbContext.S_C_WBSStruct.Where(d => this.FullID.StartsWith(d.FullID) && d.ID != this.ID).ToList();
                }
                return _Ancestors;
            }
        }


        List<S_C_WBSStruct> _Children;
        /// <summary>
        /// 子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_WBSStruct> Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = this.S_C_Mode.S_C_WBSStruct.Where(d => d.ParentID == this.ID).ToList();
                }
                return _Children;
            }
        }

        List<S_C_WBSStruct> _AllChildren;
        /// <summary>
        /// 下级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_WBSStruct> AllChildren
        {
            get
            {
                if (_AllChildren == null)
                {
                    _AllChildren = this.S_C_Mode.S_C_WBSStruct.Where(d => d.FullID.StartsWith(this.FullID) && d.ID != this.ID).ToList();
                }
                return _AllChildren;
            }
        }

        #endregion
    }
}
