using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config;
using Formula.Helper;
using Formula;
using Config.Logic;
using System.Data;

namespace Project.Logic.Domain
{
    public partial class T_SC_DesignPlan
    {

        /// <summary>
        /// 流程结束的业务逻辑
        /// </summary>
        public void Push()
        {
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            //获取项目信息
            var projectInfo = projectEntities.Set<S_I_ProjectInfo>().Find(this.ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("根据当前设计进度表，未找到对应的项目信息");
            var rootWBS = projectInfo.WBSRoot;
            if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            var wbs = projectInfo.S_W_WBS.FirstOrDefault(d => d.ID == this.SubProjectWBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("不存在子项节点，无法提交计划");
            if (this.PlanStartDate.HasValue)
                wbs.PlanStartDate = this.PlanStartDate;
            if (this.PlanFinishDate.HasValue)
                wbs.PlanEndDate = this.PlanFinishDate;

            var detailMileStoneList = this.T_SC_DesignPlan_MilestoneList.ToList();
            #region 删除里程碑
            var selectCodes = detailMileStoneList.Select(a => a.Code).ToList();
            var deleteMileStoneList = projectInfo.S_P_MileStone.Where(a =>a.WBSID==this.SubProjectWBSID &&!selectCodes.Contains(a.Code)).ToList();
            foreach (var item in deleteMileStoneList)
                item.Delete();
            #endregion
            //将里程碑信息同步至项目里程碑表
            for (int i = 0; i < detailMileStoneList.Count; i++)
            {
                var item = detailMileStoneList.ToList()[i];
                var mileStone = projectEntities.S_P_MileStone.FirstOrDefault(d => d.Code == item.Code&&d.WBSID==this.SubProjectWBSID&&
                    d.ProjectInfoID==this.ProjectInfoID);
                if (mileStone == null)
                {
                    mileStone = new S_P_MileStone();
                    mileStone.ID = FormulaHelper.CreateGuid();
                    mileStone.Name =item.Name;
                    mileStone.Code = item.Code;
                    mileStone.MileStoneValue = item.Code;
                    mileStone.WBSID = this.SubProjectWBSID;
                    mileStone.ProjectInfoID = this.ProjectInfoID;
                    mileStone.OrlPlanFinishDate = item.PlanEndDate;
                    mileStone.S_I_ProjectInfo = projectInfo;
                }
                if (mileStone.State == ProjectCommoneState.Finish.ToString())
                    continue;
                mileStone.PlanFinishDate = item.PlanEndDate;
                mileStone.Weight = item.Weight;
                mileStone.MajorValue = item.Major;
                mileStone.MileStoneType = item.MileStoneType;
                mileStone.TemplateID = item.TemplateID;
                mileStone.Name = item.Name;
                if (projectInfo.ProjectMode.ExtentionObject.GetValue("Ext_MsDataIsFromLastVertion") == TrueOrFalse.True.ToString())
                    mileStone.SortIndex = Convert.ToInt32(item.SortIndex.HasValue ? Convert.ToDecimal(item.SortIndex) * 100 : i);
                else
                {
                    var template = projectInfo.ProjectMode.S_T_MileStone.FirstOrDefault(d => d.ID == item.TemplateID);
                    if (template != null)
                    {
                        mileStone.SortIndex = template.SortIndex;
                    }
                    if (!mileStone.SortIndex.HasValue)
                    {
                        mileStone.SortIndex = item.SortIndex.HasValue ? Convert.ToInt32(item.SortIndex) : i;
                    }
                }
                mileStone.Description = item.Remark;
                mileStone.Save();
                if (mileStone.MileStoneType == MileStoneType.Cooperation.ToString())
                {
                    mileStone.OutMajorValue = item.InMajor;
                    var cooperationPlan = projectEntities.S_P_CooperationPlan.FirstOrDefault(d => d.SchemeWBSID == this.SubProjectWBSID
                        && d.CooperationValue == mileStone.MileStoneValue);
                    if (cooperationPlan == null)
                    {
                        cooperationPlan = new S_P_CooperationPlan();
                        cooperationPlan.InMajorValue = item.InMajor;
                        cooperationPlan.OutMajorValue = item.OutMajor;
                        cooperationPlan.MileStoneID = mileStone.ID;
                        cooperationPlan.ID = FormulaHelper.CreateGuid();
                        cooperationPlan.CooperationContent = mileStone.Name;
                        cooperationPlan.CooperationValue = mileStone.MileStoneValue;
                        cooperationPlan.OrPlanFinishDate = item.PlanEndDate;
                    }
                    if (!cooperationPlan.OrPlanFinishDate.HasValue)
                        cooperationPlan.OrPlanFinishDate = item.PlanEndDate;
                    cooperationPlan.PlanFinishDate = item.PlanEndDate;
                    var subProject = projectEntities.S_W_WBS.Find(this.SubProjectWBSID);
                    if (subProject != null)
                    {
                        subProject.SaveCooperationPlan(cooperationPlan);
                    }
                }
            }
        }

    }
}
