using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_P_MileStonePlan
    {
        public void Delay(DateTime date, bool isNormal = false)
        {
            var entities = this.GetDbContext<ProjectEntities>();
            var plan = this.Clone<S_P_MileStonePlan>();
            if (this.S_P_MileStone == null) throw new Formula.Exceptions.BusinessException("计划未关联到里程碑数据，无法进行延期操作");
            if (isNormal)
            {
                this.S_P_MileStone.PlanFinishDate = date;
                plan.OrlPlanFinishDate = date;
            }
            plan.PlanFinishDate = date;
            plan.Year = date.Year;
            plan.Month = date.Month;
            plan.Season = (date.Month - 1) / 3 + 1;
            plan.State = MileStoneState.Plan.ToString();
            if (this.S_P_MileStone.PlanFinishDate < DateTime.Now.Date)
            {
                plan.Stauts = MileStoneShowStatus.DelayUndone.ToString();
            }
            else
            {
                plan.Stauts = MileStoneShowStatus.NormalUndone.ToString();
            }
            this.S_P_MileStone.S_P_MileStonePlan.Add(plan);
            this.State = MileStoneState.UnFinish.ToString();
            this.Stauts = MileStoneShowStatus.DelayUndone.ToString();

            var history = entities.S_P_MileStoneHistory.Create();
            history.ChangeTime = DateTime.Now;
            history.OrlPlanFinishDate = this.S_P_MileStone.OrlPlanFinishDate;
            history.PreviosPlanFinishDate = this.PlanFinishDate;
            history.PlanFinishDate = date;
            history.ID = Formula.FormulaHelper.CreateGuid();
            history.MileStoneID = this.MileStoneID;
            history.MileStoneType = this.S_P_MileStone.MileStoneType;
            history.Name = this.S_P_MileStone.Name;
            history.ProjectInfoID = this.ProjectInfoID;
            history.Weight = this.S_P_MileStone.Weight;
            history.WBSID = this.S_P_MileStone.WBSID;
            entities.S_P_MileStoneHistory.Add(history);
        }

        public void Finish(DateTime date)
        {
            if (this.S_P_MileStone == null) throw new Formula.Exceptions.BusinessException("计划未关联到里程碑数据，无法进行完成操作");
            if (this.S_P_MileStone.PlanFinishDate < date)
                this.Stauts = MileStoneShowStatus.DelayFinish.ToString();
            else
                this.Stauts = MileStoneShowStatus.NormalFinish.ToString();
            this.S_P_MileStone.State = MileStoneState.Finish.ToString();
            this.S_P_MileStone.FactFinishDate = date;
            this.State = MileStoneState.Finish.ToString();
        }
    }
}
