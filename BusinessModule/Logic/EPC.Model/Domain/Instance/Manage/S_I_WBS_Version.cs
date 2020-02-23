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
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace EPC.Logic.Domain
{
    public partial class S_I_WBS_Version
    {
        #region 公共属性
        S_I_WBS_Version_Node _root;
        [NotMapped]
        [JsonIgnore]
        public S_I_WBS_Version_Node RootNode
        {
            get
            {
                if (_root == null)
                {
                    _root = this.S_I_WBS_Version_Node.FirstOrDefault(c => String.IsNullOrEmpty(c.ParentID));
                }
                return _root;
            }
        }

        S_C_ScheduleDefine _scheduleDefine;
        [NotMapped]
        [JsonIgnore]
        public S_C_ScheduleDefine ScheduleDefine
        {
            get
            {
                if (_scheduleDefine == null)
                {
                    if (this.S_I_Engineering == null) return null;
                    if (this.S_I_Engineering.Mode == null) return null;
                    _scheduleDefine = this.S_I_Engineering.Mode.S_C_ScheduleDefine.FirstOrDefault(c => c.Code == this.ScheduleCode);
                }
                return _scheduleDefine;
            }
        }
        #endregion
    }
}
