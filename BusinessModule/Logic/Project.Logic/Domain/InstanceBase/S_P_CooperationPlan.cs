using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using Config;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
namespace Project.Logic.Domain
{
    /// <summary>
    /// 提资计划模型对象
    /// </summary>
    public partial class S_P_CooperationPlan
    {
        /// <summary>
        /// 保存提资计划
        /// </summary>
        public void Save(bool createMileStone = false)
        {
            //if (this.PlanFinishDate == null) throw new Formula.Exceptions.BusinessException("计划完成时间不能为空");
            this.OrPlanFinishDate = this.PlanFinishDate;
            if (String.IsNullOrEmpty(this.CooperationValue))
            {
                var subProjectNode = this.S_I_ProjectInfo.S_W_WBS.FirstOrDefault(d => d.ID == this.SchemeWBSID);
                if (subProjectNode == null) throw new Formula.Exceptions.BusinessException("关联的子项不存在，无法保存提资计划");
                this.CooperationValue = this.CooperationContent + "." + this.SchemeWBSID + "." + subProjectNode.PhaseCode + "." + this.OutMajorValue + ".Cooperation";
            }
            if (createMileStone)
            {
                var mileStone = this.S_I_ProjectInfo.S_P_MileStone.FirstOrDefault(d => d.ID == this.MileStoneID);
                if (mileStone == null)
                {
                    mileStone = new S_P_MileStone();
                    mileStone.MajorValue = this.OutMajorValue;
                    mileStone.MileStoneValue = this.CooperationValue;
                    mileStone.OutMajorValue = this.InMajorValue;
                    mileStone.PlanFinishDate = this.PlanFinishDate;
                    mileStone.MileStoneType = MileStoneType.Cooperation.ToString();
                    mileStone.WBSID = this.SchemeWBSID;
                    this.S_I_ProjectInfo.AddMileStone(mileStone);
                    this.MileStoneID = mileStone.ID;
                }
                mileStone.Name = this.CooperationContent;

                mileStone.Code = mileStone.MileStoneValue;
                mileStone.OutMajorValue = this.InMajorValue;
                mileStone.MajorValue = this.OutMajorValue;
                mileStone.PlanFinishDate = this.PlanFinishDate;
                mileStone.Save();
            }
        }

        /// <summary>
        /// 删除提资计划
        /// </summary>
        /// <param name="igroValidate">是否忽略已存在的提资记录（默认为FALSE）</param>
        /// <param name="igroValidate">是否关联WBS根节点（默认为FALSE）</param>
        public void Delete(bool igroValidate = false, bool linkRoot = false)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            if (!String.IsNullOrEmpty(this.MileStoneID) && !igroValidate)
            {
                var mileStone = this.S_I_ProjectInfo.S_P_MileStone.FirstOrDefault(d => d.ID == this.MileStoneID);
                if (mileStone != null && mileStone.State == MileStoneState.Finish.ToString()
                    && mileStone.PlanFinishDate.HasValue)
                {
                    throw new Formula.Exceptions.BusinessException("【" + this.CooperationContent + "】已经完成的互提计划不能删除");
                }
                if (mileStone != null)
                {
                    entities.S_P_MileStone.Delete(d => d.ID == this.MileStoneID);
                }
            }
            entities.S_P_CooperationPlan.Delete(d => d.ID == this.ID);
            if (!linkRoot)
                entities.S_W_WBS.Delete(c => c.ID == this.WBSID);
            this.onDelete();
        }

        partial void onDelete();
    }
}
