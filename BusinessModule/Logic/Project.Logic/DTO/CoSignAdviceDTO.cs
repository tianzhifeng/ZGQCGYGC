using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic
{
    /// <summary>
    /// 会签确认对象
    /// </summary>
    public class CoSignAdviceDTO
    {
        public string ProductID { get; set; }
        public string ProductCode { get; set; }
        /// <summary>
        /// 会签专业
        /// </summary>
        public string CoSignMajorCode { get; set; }
        public string CoSignMajorName { get; set; }

        /// <summary>
        /// 会签人
        /// </summary>
        public string CoSignUserName { get; set; }
        public string CoSignUserID { get; set; }

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
