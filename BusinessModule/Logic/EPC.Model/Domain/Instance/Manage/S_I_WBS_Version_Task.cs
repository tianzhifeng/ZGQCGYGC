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
using Formula.Exceptions;

namespace EPC.Logic.Domain
{
    public partial class S_I_WBS_Version_Task
    {
        S_I_WBS_Task _task;
        [NotMapped]
        [JsonIgnore]
        public S_I_WBS_Task TaskIns
        {
            get
            {
                if (_task == null)
                {
                    _task = this.S_I_WBS_Version.S_I_Engineering.S_I_WBS_Task.FirstOrDefault(c => c.ID == this.TaskID);
                }
                return _task;
            }
        }

        S_C_ScheduleDefine_Nodes _structNodeInfo;
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

    }
}
