using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_E_Product
    {
        /// <summary>
        /// 判断成果是否已经校审
        /// </summary>
        /// <returns></returns>
        public bool IsAudited()
        {
            if (this.AuditState != ProjectCommoneState.Create.ToString())
                return true;
            else
                return false;
        }

    }
}
