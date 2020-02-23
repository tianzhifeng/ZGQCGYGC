
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

using Formula;
using Formula.Helper;

namespace EPC.Logic.Domain
{
    public partial class S_C_Mode
    {
        public static S_C_Mode GetMode(string code)
        {
            var mode = CacheHelper.Get("S_C_Mode" + code); //获取缓存数据
            if (mode == null) //缓存未空则从数据库中重新获取
            {
                var configEntites = FormulaHelper.GetEntities<InfrastructureEntities>();
                var projectMode = configEntites.S_C_Mode
                     .Include("S_C_WBSStruct")
                     .Include("S_C_MileStoneTemplate")
                     .Include("S_C_ScheduleDefine")
                     .FirstOrDefault(d => d.Code == code);
                CacheHelper.Set("S_C_Mode" + code, projectMode); //将数据库查询结果压入缓存中
                return projectMode;
            }
            else
            {
                return mode as S_C_Mode;
            }
        }
    }
}
