using Formula;
using Formula.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_C_CBS
    {
        #region 公共属性

        List<S_C_CBS> _children;
        /// <summary>
        /// 获取CBS子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_CBS> Children
        {
            get
            {
                if (_children == null)
                    _children = this.S_I_ProjectInfo.S_C_CBS.Where(d => d.ParentID == this.ID).ToList();
                return _children;
            }
        }

        List<S_C_CBS> _allchildren;
        /// <summary>
        /// 获取CBS所有下级子节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_C_CBS> AllChildren
        {
            get
            {
                if (_allchildren == null)
                    _allchildren = this.S_I_ProjectInfo.S_C_CBS.Where(d => d.FullID.StartsWith(this.FullID)).ToList();
                return _allchildren;
            }
        }

        List<S_C_CBS> _Ansestors;
        [NotMapped]
        [JsonIgnore]
        public List<S_C_CBS> Ansestors
        {
            get
            {
                if (_Ansestors == null)
                    _Ansestors = this.S_I_ProjectInfo.S_C_CBS.Where(d => this.FullID.StartsWith(d.FullID)).OrderBy(d => d.FullID).ToList();
                return _Ansestors;
            }
        }

        S_C_CBS _parent;
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_CBS Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this.S_I_ProjectInfo.S_C_CBS.SingleOrDefault(d => d.ID == this.ParentID);
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
        public S_C_CBS AddChild(S_C_CBS child)
        {
            var user = FormulaHelper.GetUserInfo();
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Entry<S_C_CBS>(child).State != System.Data.EntityState.Added
                && entities.Entry<S_C_CBS>(child).State != System.Data.EntityState.Detached)
                throw new Formula.Exceptions.BusinessException("非新增状态的CBS对象，无法调用AddChild方法");
            if (String.IsNullOrEmpty(child.ID))
                child.ID = FormulaHelper.CreateGuid();
            if (String.IsNullOrEmpty(child.NodeType))
                child.NodeType = CBSNodeType.CBS.ToString();
            child.ParentID = this.ID;
            child.FullID = this.FullID + "." + child.ID;
            child.ProjectInfoID = this.ProjectInfoID;
            child.S_I_ProjectInfo = this.S_I_ProjectInfo;
            if (child.UnitPrice == null)
                child.UnitPrice = this.UnitPrice;
            if (child.Quantity == null)
                child.Quantity = this.Quantity;
            if (child.TotalPrice == null)
            {
                if (child.Quantity.HasValue && child.UnitPrice.HasValue)
                    child.TotalPrice = child.Quantity.Value * child.UnitPrice.Value;
            }
            child.CreateDate = DateTime.Now;
            if (string.IsNullOrEmpty(child.CreateUserID))
                child.CreateUserID = user.UserID;
            if (string.IsNullOrEmpty(child.CreateUser))
                child.CreateUser = user.UserName;
            this.S_I_ProjectInfo.S_C_CBS.Add(child);
            this.Children.Add(child);
            this.AllChildren.Add(child);
            return child;
        }
                
        /// <summary>
        /// 汇总子节点的工作量，并计算金额
        /// </summary>
        public void SummayQuantity()
        {
            this.Quantity = this.Children.Sum(a => a.Quantity);
            if (this.Quantity == null)
                this.Quantity = 0m;
            this.TotalPrice = this.Quantity.Value * this.UnitPrice.Value;
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
                //if (this.S_C_CBS_Cost.Count > 0)
                if (this.SummaryCostQuantity.HasValue && this.SummaryCostQuantity.Value > 0)
                    throw new Formula.Exceptions.BusinessException("【" + this.Name + "】已经结算过，不允许删除");
            }
            entities.S_C_CBS.Delete(d => d.ID == this.ID);
        }

        /// <summary>
        /// 增加卷册
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public S_C_CBS_Budget AddBudget(S_C_CBS_Budget budget)
        {
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Entry<S_C_CBS_Budget>(budget).State != System.Data.EntityState.Added
                && entities.Entry<S_C_CBS_Budget>(budget).State != System.Data.EntityState.Detached)
                throw new Formula.Exceptions.BusinessException("非新增状态的S_C_CBS_Budget对象，无法调用AddBudget方法");
            if (String.IsNullOrEmpty(budget.ID))
                budget.ID = FormulaHelper.CreateGuid();
            budget.ProjectInfoID = this.ProjectInfoID;
            budget.CBSFullID = this.FullID;
            budget.CBSID = this.ID;
            budget.S_C_CBS = this;
            if (string.IsNullOrEmpty(budget.ParentID))
            {
                budget.ParentID = string.Empty;
                budget.FullID = budget.ID;
            }
            if (budget.UnitPrice == null)
                budget.UnitPrice = this.UnitPrice;
            if (budget.Quantity == null)
                budget.Quantity = this.Quantity;
            if (budget.TotalValue == null)
            {
                if (budget.Quantity.HasValue && budget.UnitPrice.HasValue)
                    budget.TotalValue = budget.Quantity.Value * budget.UnitPrice.Value;
            }
            this.S_C_CBS_Budget.Add(budget);
            entities.S_C_CBS_Budget.Add(budget);
            return budget;
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
            cost.CBSFullID = this.FullID;
            cost.CBSID = this.ID;
            cost.S_C_CBS = this;

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
                cost.BelongDept = this.S_I_ProjectInfo.ChargeDeptID;
                cost.BelongDeptName = this.S_I_ProjectInfo.ChargeDeptName;
            }
            if (string.IsNullOrEmpty(cost.UserDept))
            {
                cost.UserDept = user.UserOrgID;
                cost.UserDeptName = user.UserOrgName;
            }

            this.S_C_CBS_Cost.Add(cost);
            entities.S_C_CBS_Cost.Add(cost);
            return cost;
        }

        /// <summary>
        /// 汇总卷册额定工时
        /// </summary>
        public void SummaryBudget()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            foreach (var cbs in this.AllChildren)
            {
                //只汇总卷册的 不汇总角色
                cbs.SummaryBudgetQuantity = entities.S_C_CBS_Budget.Where(a => string.IsNullOrEmpty(a.ParentID)
                    && a.CBSFullID.StartsWith(cbs.FullID)).Sum(a => a.Quantity);
                cbs.SummaryBudgetPrice = entities.S_C_CBS_Budget.Where(a => string.IsNullOrEmpty(a.ParentID)
                    && a.CBSFullID.StartsWith(cbs.FullID)).Sum(a => a.TotalValue);
            }
        }

        /// <summary>
        /// 汇总结算工时
        /// </summary>
        public void SummaryCost()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            foreach (var cbs in this.AllChildren)
            {
                cbs.SummaryCostQuantity = entities.S_C_CBS_Cost.Where(a => a.CBSFullID.StartsWith(cbs.FullID)).Sum(a => a.Quantity);
                cbs.SummaryCostPrice = entities.S_C_CBS_Cost.Where(a => a.CBSFullID.StartsWith(cbs.FullID)).Sum(a => a.TotalValue);
            }
        }
        #endregion
    }
}
