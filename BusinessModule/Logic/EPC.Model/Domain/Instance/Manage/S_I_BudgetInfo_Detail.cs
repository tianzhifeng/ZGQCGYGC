using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
using Formula.Helper;
using System.ComponentModel.DataAnnotations;

namespace EPC.Logic.Domain
{
    public  partial class S_I_BudgetInfo_Detail
    {

        S_T_CBSNodeTemplate _structNodeInfo;
        /// <summary>
        /// 结构节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_CBSNodeTemplate StructNodeInfo
        {
            get
            {
                var dbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
                if (_structNodeInfo == null)
                    _structNodeInfo = dbContext.S_T_CBSNodeTemplate.FirstOrDefault(c => c.ID == this.CBSDefineID);
                return _structNodeInfo;
            }
        }

        List<S_I_BudgetInfo_Detail> _ancestors;
        [NotMapped]
        [JsonIgnore]
        public List<S_I_BudgetInfo_Detail> Ancestors
        {
            get
            {
                if (_ancestors == null)
                {
                    _ancestors = this.S_I_BudgetInfo.S_I_BudgetInfo_Detail.Where(c => this.CBSFullID.StartsWith(c.CBSFullID) && c.CBSFullID != this.CBSFullID).
                        OrderBy(c => c.CBSFullID).ThenBy(c => c.SortIndex).ToList();
                }
                return _ancestors;
            }
        }
    }
}
