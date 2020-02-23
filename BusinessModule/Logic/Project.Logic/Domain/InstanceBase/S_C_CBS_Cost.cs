using Formula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_C_CBS_Cost
    {

        #region 公共方法
                
        /// <summary>
        /// 删除结算数据
        /// </summary>
        /// <param name="destroy"></param>
        public void Delete()
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            entities.S_C_CBS_Cost.Delete(d => d.ID == this.ID);
        }

        #endregion
    }
}
