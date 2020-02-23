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
    public partial class S_C_CBS_Budget
    {
        #region 公共属性

        List<S_C_CBS_Budget> _children;
        /// <summary>
        /// 获取CBSBudget子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_CBS_Budget> Children
        {
            get
            {
                if (_children == null)
                    _children = this.S_C_CBS.S_C_CBS_Budget.Where(d => d.ParentID == this.ID).ToList();
                return _children;
            }
        }

        List<S_C_CBS_Budget> _allchildren;
        /// <summary>
        /// 获取CBSBudget所有下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_CBS_Budget> AllChildren
        {
            get
            {
                if (_allchildren == null)
                    _allchildren = this.S_C_CBS.S_C_CBS_Budget.Where(d => d.FullID.StartsWith(this.FullID)).ToList();
                return _allchildren;
            }
        }

        List<S_C_CBS_Budget> _Ansestors;
        [NotMapped]
        [JsonIgnore]
        public List<S_C_CBS_Budget> Ansestors
        {
            get
            {
                if (_Ansestors == null)
                    _Ansestors = this.S_C_CBS.S_C_CBS_Budget.Where(d => this.FullID.StartsWith(d.FullID)).OrderBy(d => d.FullID).ToList();
                return _Ansestors;
            }
        }

        S_C_CBS_Budget _parent;
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_CBS_Budget Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this.S_C_CBS.S_C_CBS_Budget.SingleOrDefault(d => d.ID == this.ParentID);
                }
                return _parent;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 增加子节点
        /// </summary>
        /// <param name="child">子节点对象</param>
        public S_C_CBS_Budget AddChild(S_C_CBS_Budget child)
        {
            var user = FormulaHelper.GetUserInfo();
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Entry<S_C_CBS_Budget>(child).State != System.Data.EntityState.Added
                && entities.Entry<S_C_CBS_Budget>(child).State != System.Data.EntityState.Detached)
                throw new Formula.Exceptions.BusinessException("非新增状态的CBS对象，无法调用AddChild方法");
            if (String.IsNullOrEmpty(child.ID))
                child.ID = FormulaHelper.CreateGuid();
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            child.ProjectInfoID = this.ProjectInfoID;
            child.S_C_CBS = this.S_C_CBS;
            child.CBSID = this.S_C_CBS.ID;
            child.CBSFullID = this.S_C_CBS.FullID;

            if (child.UnitPrice == null)
                child.UnitPrice = this.UnitPrice;
            if (child.Quantity == null)
                child.Quantity = this.Quantity;
            if (child.TotalValue == null)
            {
                if (child.Quantity.HasValue && child.UnitPrice.HasValue)
                    child.TotalValue = child.Quantity.Value * child.UnitPrice.Value;
            }
            
            this.S_C_CBS.S_C_CBS_Budget.Add(child);
            this.Children.Add(child);
            this.AllChildren.Add(child);
            return child;
        }

        /// <summary>
        /// 删除节点（包含所有子节点）
        /// </summary>
        /// <param name="destroy"></param>
        public void Delete(bool destroy = false)
        {
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Entry(this).State == EntityState.Deleted) return;
            foreach (var item in this.AllChildren)
                item.DeleteSelf(destroy);
            this.DeleteSelf(destroy);
        }

        /// <summary>
        /// 删除自身节点
        /// </summary>
        /// <param name="destroy"></param>
        internal void DeleteSelf(bool destroy = false)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            if (!destroy)
            {
                //if (entities.S_C_CBS_Cost.Count(a => a.BudgetID == this.ID) > 0)
                if (this.SummaryCostQuantity.HasValue && this.SummaryCostQuantity.Value > 0)
                    throw new Formula.Exceptions.BusinessException("【" + this.Name + "】已经结算过，不允许删除");
            }
            entities.S_C_CBS_Budget.Delete(d => d.ID == this.ID);
        }
        
        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="cost"></param>
        /// <returns></returns>
        public S_C_CBS_Cost AddCost(S_C_CBS_Cost cost)
        {
            var user = FormulaHelper.GetUserInfo();
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Entry<S_C_CBS_Cost>(cost).State != System.Data.EntityState.Added
                && entities.Entry<S_C_CBS_Cost>(cost).State != System.Data.EntityState.Detached)
                throw new Formula.Exceptions.BusinessException("非新增状态的S_C_CBS_Cost对象，无法调用AddCost方法");
            if (String.IsNullOrEmpty(cost.ID))
                cost.ID = FormulaHelper.CreateGuid();
            cost.ProjectInfoID = this.ProjectInfoID;
            cost.CBSFullID = this.S_C_CBS.FullID;
            cost.CBSID = this.S_C_CBS.ID;
            cost.S_C_CBS = this.S_C_CBS;

            cost.BudgetID = this.ID;
            cost.BudgetFullID = this.FullID;

            if (cost.UnitPrice == null)
                cost.UnitPrice = this.UnitPrice;
            if (cost.Quantity == null)
                cost.Quantity = this.Quantity;
            if (cost.TotalValue == null)
            {
                if (cost.Quantity.HasValue && cost.UnitPrice.HasValue)
                    cost.TotalValue = cost.Quantity.Value * cost.UnitPrice.Value;
            }

            cost.CostDate = DateTime.Now;
            if (string.IsNullOrEmpty(cost.CostUser))
                cost.CostUser = user.UserID;
            if (string.IsNullOrEmpty(cost.CostUserName))
                cost.CostUserName = user.UserName;
            if (cost.BelongYear == null)
                cost.BelongYear = DateTime.Now.Year;
            if (cost.BelongQuarter == null)
                cost.BelongQuarter = (DateTime.Now.Month - 1) / 3 + 1;
            if (cost.BelongMonth == null)
                cost.BelongMonth = DateTime.Now.Month;
            if (string.IsNullOrEmpty(cost.BelongDept))
            {
                cost.BelongDept = this.S_C_CBS.S_I_ProjectInfo.ChargeDeptID;
                cost.BelongDeptName = this.S_C_CBS.S_I_ProjectInfo.ChargeDeptName;
            }
            if (string.IsNullOrEmpty(cost.UserDept))
            {
                cost.UserDept = user.UserOrgID;
                cost.UserDeptName = user.UserOrgName;
            }

            this.S_C_CBS.S_C_CBS_Cost.Add(cost);
            entities.S_C_CBS_Cost.Add(cost);
            return cost;
        }

        /// <summary>
        /// 汇总结算工时
        /// </summary>
        public void SummaryCost()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            foreach (var child in this.AllChildren)
            {
                child.SummaryCostQuantity = child.S_C_CBS.S_C_CBS_Cost.Where(a => a.BudgetFullID.StartsWith(child.FullID)).Sum(a => a.Quantity);
                child.SummaryCostValue = child.S_C_CBS.S_C_CBS_Cost.Where(a => a.BudgetFullID.StartsWith(child.FullID)).Sum(a => a.TotalValue);
            }
        }

        #endregion
    }
}
