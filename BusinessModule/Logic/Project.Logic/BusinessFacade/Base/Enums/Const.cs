using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic
{
    public class CommonConst
    {
        public const string cacheKey = "S_T_ProjectMode_";
        public const string enginnerSpaceCacheKey = "S_T_EngineeringSpace";
        public const string designSubmitActivityUrl = "/Project/Basic/Design/SubmitForm";

        public const string unAuditState = "未评审";
        public const string inAuditState = "评审中";
        public const string finishAuditState = "已评审";

        /// <summary>
        /// QBS定性类型的完成时间
        /// </summary>
        public const string QualitativeState = "CreateDate";
        /// <summary>
        /// QBS定量类型的总数
        /// </summary>
        public const string QuantifyNum = "TotalNum";
    }
}
