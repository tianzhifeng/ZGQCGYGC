using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Project.Logic.Domain
{
    /// <summary>
    /// 里程碑信息
    /// </summary>
    public partial class S_P_MileStone
    {
        #region 公共属性

        /// <summary>
        /// 里程碑展现状态(标识里程碑式延期，延期完成，正常，还是延期未完成)
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public string ShowStatus
        {
            get
            {
                if (this.State == ProjectCommoneState.Finish.ToString())
                {
                    if (this.PlanFinishDate.HasValue
                        && this.FactFinishDate.HasValue
                        && this.FactFinishDate.Value > this.PlanFinishDate.Value)
                        return MileStoneShowStatus.DelayFinish.ToString();
                    else
                        return MileStoneShowStatus.NormalFinish.ToString();
                }
                else
                {
                    //没完成 也没有计划时间的
                    if (!this.PlanFinishDate.HasValue)
                        return MileStoneShowStatus.NotPlan.ToString();
                    else if (DateTime.Now.Date > this.PlanFinishDate.Value)
                        return MileStoneShowStatus.DelayUndone.ToString();
                    else
                        return MileStoneShowStatus.NormalUndone.ToString();
                }
            }
        }

        #endregion

        /// <summary>
        /// 保存里程碑
        /// </summary>
        public void Save()
        {
            var user = FormulaHelper.GetUserInfo();
            var entites = this.GetDbContext<ProjectEntities>();
            if (String.IsNullOrEmpty(this.ProjectInfoID)) throw new Formula.Exceptions.BusinessException("必须为里程碑指定项目ID");
            if (this.PlanFinishDate != null)
                this.OrlPlanFinishDate = this.PlanFinishDate;
            if (!String.IsNullOrEmpty(this.WBSID))
            {
                var wbs = entites.S_W_WBS.FirstOrDefault(d => d.ID == this.WBSID);
                if (wbs != null)
                {
                    if (wbs.WBSType == WBSNodeType.Major.ToString())
                    {
                        this.MajorValue = wbs.WBSValue;
                    }
                }
            }

            SetDefaultValue();

            if (this.S_I_ProjectInfo == null)
            {
                this.S_I_ProjectInfo = entites.S_I_ProjectInfo.Find(this.ProjectInfoID);
            }
            if (this.MileStoneType == Project.Logic.MileStoneType.Normal.ToString())
            {
                this.DeptID = this.S_I_ProjectInfo.ChargeDeptID;
                this.DeptName = this.S_I_ProjectInfo.ChargeDeptName;
                SychToPlan();
            }
           
            if (entites.Entry<S_P_MileStone>(this).State == System.Data.EntityState.Added || entites.Entry<S_P_MileStone>(this).State == System.Data.EntityState.Detached)
            { 
                entites.S_P_MileStone.Add(this);              
                this.CreateDate = DateTime.Now;
                this.CreateUser = user.UserName;
                this.CreateUserID = user.UserID;
            }
            else
            {
                this.ModifyDate = DateTime.Now;
                this.ModifyUser = user.UserName;
                this.ModifyUserID = user.UserID;
            }
        }

        /// <summary>
        /// 变更里程碑
        /// </summary>
        /// <param name="newDateTime">新的计划完成日期</param>
        public void ChangeMileStone(DateTime newDateTime)
        {
            this.PlanFinishDate = newDateTime;
            SetDefaultValue();
        }

        /// <summary>
        /// 变更里程碑
        /// </summary>
        /// <param name="day">天数</param>
        /// <param name="fillEmpty"></param>
        public void ChangeMileStone(int day, bool fillEmpty = true)
        {
            if (this.PlanFinishDate == null)
            {
                if (fillEmpty)
                    this.PlanFinishDate = DateTime.Now.AddDays(day);
                else
                    throw new Formula.Exceptions.BusinessException("计划完成时间不能为空");
            }
            else
                this.PlanFinishDate = this.PlanFinishDate.Value.AddDays(day);
            SetDefaultValue();
        }

        /// <summary>
        /// 关联收款项
        /// </summary>
        public void RelateTo(string receiptID)
        { }

        /// <summary>
        /// 删除里程碑
        /// </summary>
        public void Delete()
        {
            var entites = this.GetDbContext<ProjectEntities>();
            if (this.State == ProjectCommoneState.Finish.ToString())
                throw new Formula.Exceptions.BusinessException("里程碑【" + this.Name + "】已经完成，无法删除");
            if (this.MileStoneType == Project.Logic.MileStoneType.Cooperation.ToString())
            {
                var cooperationPlan = entites.S_P_CooperationPlan.FirstOrDefault(d => d.MileStoneID == this.ID);
                if (cooperationPlan != null)
                {
                    entites.S_W_WBS.Delete(d => d.ID == cooperationPlan.WBSID);
                }
            }
            entites.S_P_MileStone.Delete(d => d.ID == this.ID);
        }

        public void Finish(DateTime finishDate)
        {
            this.FactFinishDate = finishDate;
            //没有策划把完成时间作为计划时间
            if (!this.PlanFinishDate.HasValue)
                this.PlanFinishDate = finishDate;
            this.State = ProjectCommoneState.Finish.ToString();
            if (this.Weight.HasValue )
            {
                var progress = this.S_I_ProjectInfo.CompletePercent == null ? 0M : Convert.ToDecimal(this.S_I_ProjectInfo.CompletePercent);
                var maxWeight = this.S_I_ProjectInfo.S_P_MileStone.Where(d=>d.ID != this.ID).Max(d => d.Weight);
                if (maxWeight < this.Weight)
                    maxWeight = this.Weight;
                if (progress < maxWeight)
                    this.S_I_ProjectInfo.CompletePercent = maxWeight;

                var mileStones = this.S_I_ProjectInfo.S_P_MileStone.Where(d => d.ID != this.ID && d.Weight <= this.Weight && d.WBSID == this.WBSID).ToList();
                foreach (var item in mileStones)
                {
                    if (!item.FactFinishDate.HasValue)
                        item.FactFinishDate = finishDate;
                    item.State = MileStoneState.Finish.ToString();
                }
            }

            var plans = this.S_P_MileStonePlan.Where(d => d.State == MileStoneState.Plan.ToString());
            foreach (var item in plans)
            {
                item.State = MileStoneState.Finish.ToString();
                if (item.PlanFinishDate >= finishDate)
                    item.Stauts = MileStoneShowStatus.NormalFinish.ToString();
                else
                    item.Stauts = MileStoneShowStatus.DelayFinish.ToString();
            }            

            //更新提资计划状态
            var cooplist = this.S_I_ProjectInfo.S_P_CooperationPlan.Where(a => a.MileStoneID == this.ID).ToList();
            foreach (var coopPlan in cooplist)
            {
                coopPlan.State = ProjectCommoneState.Finish.ToString();
                coopPlan.FactFinishDate = DateTime.Now;
            }
            //更新提资wbs状态
            var wbsids = cooplist.Select(a => a.WBSID).ToList();
            var projectType = WBSNodeType.Project.ToString();
            var wbsList =  this.S_I_ProjectInfo.S_W_WBS;
            var coopNodes = wbsList.Where(a => wbsids.Contains(a.ID) && a.WBSType != projectType).ToList();
            foreach (var coopNode in coopNodes)
            {
                coopNode.State = ProjectCommoneState.Finish.ToString();
                coopNode.FactEndDate = DateTime.Now;
            }
            var relateWBSs = wbsList.Where(a => !string.IsNullOrEmpty(a.RelateMileStone) && a.RelateMileStone.Contains(this.ID)).ToList();
            foreach (var relateWBS in relateWBSs)
            {
                relateWBS.State = ProjectCommoneState.Finish.ToString();
                relateWBS.FactEndDate = DateTime.Now;
            }
        }

        public void Revert()
        {
            this.State = MileStoneState.Plan.ToString();
            this.FactFinishDate = null;
            var mileStoneList = this.S_I_ProjectInfo.S_P_MileStone.
                Where(d => d.MileStoneType == Project.Logic.MileStoneType.Normal.ToString() && d.State == MileStoneState.Finish.ToString() && d.ID != this.ID).ToList();
            var weight = mileStoneList.Max(d => d.Weight);
            if (weight.HasValue)
                this.S_I_ProjectInfo.CompletePercent = weight;
            else
                this.S_I_ProjectInfo.CompletePercent = 0;
            var plan = this.S_P_MileStonePlan.OrderByDescending(d => d.ID).FirstOrDefault();
            if (plan != null)
            {
                plan.State = MileStoneState.Plan.ToString();
                if (this.PlanFinishDate >= DateTime.Now.AddDays(-1))
                    plan.Stauts = MileStoneShowStatus.NormalUndone.ToString();
                else
                    plan.Stauts = MileStoneShowStatus.DelayUndone.ToString();
            }
            //更新提资计划状态
            var cooplist = this.S_I_ProjectInfo.S_P_CooperationPlan.Where(a => a.MileStoneID == this.ID).ToList();
            foreach (var coopPlan in cooplist)
            {
                coopPlan.State = null;
                coopPlan.FactFinishDate = null;
            }
            //更新提资wbs状态
            var wbsids = cooplist.Select(a => a.WBSID).ToList();
            var coopNodes = this.S_I_ProjectInfo.S_W_WBS.Where(a => wbsids.Contains(a.ID)).ToList();
            foreach (var coopNode in coopNodes)
            {
                coopNode.State = ProjectCommoneState.Plan.ToString();
                coopNode.FactEndDate = null;
            }
        }

        #region 私有方法

        private void SychToPlan()
        {
            var entities = this.GetDbContext<ProjectEntities>();
            if (this.State == MileStoneState.Finish.ToString()) return;
            var plan = this.S_P_MileStonePlan.FirstOrDefault(d => d.State != ProjectCommoneState.Finish.ToString());
            if (plan == null)
            {
                plan = entities.S_P_MileStonePlan.Create();
                plan.ID = FormulaHelper.CreateGuid();
                plan.ProjectInfoID = this.ProjectInfoID;             
                plan.MileStoneID = this.ID;
                plan.S_P_MileStone = this;
                this.S_P_MileStonePlan.Add(plan);
            }
            plan.DeptID = this.DeptID;
            plan.DeptName = this.DeptName;
            plan.Name = this.Name;
            plan.Code = this.Code;
            plan.Description = this.Description;
            plan.MajorValue = this.MajorValue;
            plan.MileStoneType = this.MileStoneType;
            plan.MileStoneValue = this.MileStoneValue;
            plan.SortIndex = this.SortIndex;
            plan.WBSID = this.WBSID;
            plan.PlanFinishDate = this.PlanFinishDate;
            plan.OrlPlanFinishDate = this.PlanFinishDate;
            plan.Month = this.Month;
            plan.Year = this.Year;
            plan.Season = this.Season;
            plan.State = this.State;
            if (this.PlanFinishDate != null && this.PlanFinishDate < DateTime.Now && this.State != ProjectCommoneState.Finish.ToString())
                plan.Stauts = MileStoneShowStatus.DelayUndone.ToString();
            else if (this.PlanFinishDate == null)
                plan.Stauts = MileStoneShowStatus.NotPlan.ToString();
            else if (this.State == ProjectCommoneState.Finish.ToString() && this.PlanFinishDate != null && this.PlanFinishDate < DateTime.Now.AddDays(-1))
                plan.Stauts = MileStoneShowStatus.DelayFinish.ToString();
            else if (this.State == ProjectCommoneState.Finish.ToString() && this.PlanFinishDate != null && this.PlanFinishDate >= DateTime.Now.AddDays(-1))
                plan.Stauts = MileStoneShowStatus.NormalFinish.ToString();
            else if (this.State != ProjectCommoneState.Finish.ToString() && this.PlanFinishDate != null && this.PlanFinishDate >= DateTime.Now.AddDays(-1))
                plan.Stauts = MileStoneShowStatus.NormalUndone.ToString();
            else
                plan.Stauts = MileStoneShowStatus.NotPlan.ToString();
        }

        private void SetDefaultValue()
        {
            if (String.IsNullOrEmpty(this.Necessity))
                this.Necessity = false.ToString();
            var user = FormulaHelper.GetUserInfo();
            if (String.IsNullOrEmpty(this.MileStoneType)) this.MileStoneType = Project.Logic.MileStoneType.Normal.ToString();
            if (this.PlanFinishDate != null)
            {
                this.Year = this.PlanFinishDate.Value.Year;
                this.Month = this.PlanFinishDate.Value.Month;
                this.Season = (this.PlanFinishDate.Value.Month - 1) / 3 + 1;
            }
            else
            {
                this.Year = null;
                this.Month = null;
                this.Season = null;
            }
            var entites = this.GetDbContext<ProjectEntities>();
            if (entites.Entry<S_P_MileStone>(this).State == System.Data.EntityState.Added || entites.Entry<S_P_MileStone>(this).State == System.Data.EntityState.Detached)
            {
                this.State = MileStoneState.Plan.ToString();
                this.CreateDate = DateTime.Now;
                this.CreateUser = user.UserName;
                this.CreateUserID = user.UserID;
            }
            else
            {
                this.ModifyDate = DateTime.Now;
                this.ModifyUser = user.UserName;
                this.ModifyUserID = user.UserID;
            }
        }

        #endregion

    }
}
