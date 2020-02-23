using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Config;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
using Formula.Helper;
using Config.Logic;
using Base.Logic.BusinessFacade;


namespace EPC.Logic.Domain
{
    public partial class S_I_WBS
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

        /// <summary>
        /// 是否为枚举型节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool IsEnumNode
        {
            get
            {
                if (this.StructNodeInfo == null) return false;
                if (this.StructNodeInfo.IsEnum == "1")
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 结构节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_WBSStruct StructNodeInfo
        {
            get
            {
                return this.Mode.S_C_WBSStruct.FirstOrDefault(d => d.ID == this.StructInfoID);
            }
        }


        S_I_WBS _parent;
        /// <summary>
        /// 父节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_WBS Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this.S_I_Engineering.S_I_WBS.FirstOrDefault(d => d.ID == this.ParentID);
                }
                return _parent;
            }
        }

        List<S_I_WBS> _ancestor;
        /// <summary>
        /// 所有上级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_WBS> Ancestor
        {
            get
            {
                if (_ancestor == null)
                {
                    _ancestor = this.S_I_Engineering.S_I_WBS.Where(d => this.FullID.StartsWith(d.FullID)).OrderBy(d => d.FullID).ToList();
                }
                return _ancestor;
            }
        }

        /// <summary>
        /// 根节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_WBS RootNode
        {
            get
            {
                return this.S_I_Engineering.WBSRoot;
            }
        }


        /// <summary>
        /// 下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_WBS> Children
        {
            get
            {
                return this.S_I_Engineering.S_I_WBS.Where(d => d.ParentID == this.ID).OrderBy(d => d.SortIndex).ToList();
            }
        }

        /// <summary>
        /// 所有下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_WBS> AllChildren
        {
            get
            {
                return this.S_I_Engineering.S_I_WBS.Where(w => w.FullID.StartsWith(this.FullID) && w.ID != this.ID).OrderBy(d => d.SortIndex).ToList();
            }
        }




        #endregion

    }
}
