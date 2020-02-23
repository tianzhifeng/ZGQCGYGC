using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
   public partial class T_SC_MajorDesignInput
    {
        /// <summary>
        /// 流程结束的反写数据的逻辑                                                                                   2
        /// </summary>
        public void Publish()
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            var currentUser = FormulaHelper.GetUserInfo();
            //获取字表数据
            var detailList = this.T_SC_MajorDesignInput_DesignInputList.ToList();
            foreach (var detail in detailList)
            {
                var doc = entities.S_D_InputDocument.Find(detail.DocID);
                if (doc == null) continue;
                doc.AuditState = CommonConst.finishAuditState.ToString();
            }
        }
    }
}
