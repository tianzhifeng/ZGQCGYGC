using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace EPC.Logic.Domain
{
    public partial class S_C_ScheduleDefine_Nodes
    {
        #region 公开属性

        S_C_ScheduleDefine_Nodes _Parent;
        /// <summary>
        /// 父节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_ScheduleDefine_Nodes Parent
        {
            get
            {
                if (_Parent == null)
                {
                    _Parent = this.S_C_ScheduleDefine.S_C_ScheduleDefine_Nodes.FirstOrDefault(d => d.StructInfoID == this.ParentID);
                }
                return _Parent;
            }
        }

        List<S_C_ScheduleDefine_Nodes> _Ancestors;
        /// <summary>
        /// 上级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_ScheduleDefine_Nodes> Ancestors
        {
            get
            {
                if (_Ancestors == null)
                {
                    _Ancestors = this.S_C_ScheduleDefine.S_C_ScheduleDefine_Nodes.Where(c => this.FullID.StartsWith(c.FullID) && c.ID != this.ID).OrderBy(c=>c.FullID).ToList();
                }
                return _Ancestors;
            }
        }


        List<S_C_ScheduleDefine_Nodes> _Children;
        /// <summary>
        /// 子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_ScheduleDefine_Nodes> Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = this.S_C_ScheduleDefine.S_C_ScheduleDefine_Nodes.Where(d => d.ParentID == this.StructInfoID).ToList();
                }
                return _Children;
            }
        }

        List<S_C_ScheduleDefine_Nodes> _AllChildren;
        /// <summary>
        /// 下级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_ScheduleDefine_Nodes> AllChildren
        {
            get
            {
                if (_AllChildren == null)
                {
                    _AllChildren = this.S_C_ScheduleDefine.S_C_ScheduleDefine_Nodes.Where(d => d.FullID.StartsWith(this.FullID) && d.ID != this.ID).ToList();
                }
                return _AllChildren;
            }
        }

        #endregion
    }
}
