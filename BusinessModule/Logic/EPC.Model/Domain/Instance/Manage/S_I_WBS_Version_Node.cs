using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config;
using Config.Logic;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
using Formula.Helper;
using System.ComponentModel.DataAnnotations;

namespace EPC.Logic.Domain
{
    public partial class S_I_WBS_Version_Node
    {
        S_C_ScheduleDefine_Nodes _structNodeInfo;
        /// <summary>
        /// 结构节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_ScheduleDefine_Nodes StructNodeInfo
        {
            get
            {
                if (_structNodeInfo == null)
                {
                    if (this.S_I_WBS_Version == null || this.S_I_WBS_Version.S_I_Engineering == null)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("没有关联计划版本，或计划版本未绑定工程");
                    }
                    else if (this.S_I_WBS_Version.S_I_Engineering.Mode == null)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("关联的计划模式为空");
                    }
                    var define = this.S_I_WBS_Version.S_I_Engineering.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == this.S_I_WBS_Version.ScheduleCode);
                    if (define == null)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("没有找到计划视图定义");
                    }
                    _structNodeInfo = define.S_C_ScheduleDefine_Nodes.FirstOrDefault(c => c.StructInfoID == this.StructInfoID);
                }
                return _structNodeInfo;
            }
        }

        S_I_WBS_Version_Node _parentNode;
        [NotMapped]
        [JsonIgnore]
        public S_I_WBS_Version_Node ParentNode
        {
            get
            {
                if (_parentNode == null)
                {
                    _parentNode = this.S_I_WBS_Version.S_I_WBS_Version_Node.FirstOrDefault(c => c.ID == this.ParentID);
                }
                return _parentNode;
            }
        }

        List<S_I_WBS_Version_Node> _ancestor;
        /// <summary>
        /// 所有上级节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_WBS_Version_Node> Ancestor
        {
            get
            {
                if (_ancestor == null)
                {
                    _ancestor = this.S_I_WBS_Version.S_I_WBS_Version_Node.Where(d => this.FullID.StartsWith(d.FullID)).OrderBy(d => d.FullID).ToList();
                }
                return _ancestor;
            }
        }

        /// <summary>
        /// 根节点对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_WBS_Version_Node RootNode
        {
            get
            {
                return this.S_I_WBS_Version.RootNode;
            }
        }
    }
}
