using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic
{
    public class AuditSignUserDTO
    {
        public string ProductID { get; set; }
        public string ProductCode { get; set; }
        /// <summary>
        /// 流程环节
        /// </summary>
        public string StepKey { get; set; }

        /// <summary>
        /// 校审人
        /// </summary>
        public string SignUserName { get; set; }
        public string SignUserID { get; set; }

        /// <summary>
        /// 意见
        /// </summary>
        public string Advice { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? SignDate { get; set; }
    }
}
