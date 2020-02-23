using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Formula;
using Project.Logic.Domain;
using Config.Logic;
using Formula.Exceptions;
using Project.Logic;

namespace Project.Areas.AutoList.Controllers
{
    public class MileStoneEditListController : ProjectEditListContorllor
    {
        protected override void BeforeSaveDelete(List<Dictionary<string, string>> list, Base.Logic.Domain.S_UI_List listInfo)
        {
            base.BeforeSaveDelete(list, listInfo);
            var idAry = list.Select(a => a.GetValue("ID"));
            var mileStoneList = this.BusinessEntities.Set<S_P_MileStone>().Where(a => idAry.Contains(a.ID)).ToList();
            var planList = this.BusinessEntities.Set<S_P_CooperationPlan>().Where(a => idAry.Contains(a.MileStoneID)).ToList();
            foreach (var mileStone in mileStoneList)
            {
                //if (mileStone.State == ProjectCommoneState.Finish.ToString())
                //    throw new Formula.Exceptions.BusinessException("里程碑【" + mileStone.Name + "】已经完成，无法删除");
                if (mileStone.MileStoneType == Project.Logic.MileStoneType.Cooperation.ToString())
                {
                    var cooperationPlan = planList.FirstOrDefault(d => d.MileStoneID == mileStone.ID);
                    if (cooperationPlan != null)
                    {
                        var wbs = this.BusinessEntities.Set<S_W_WBS>().FirstOrDefault(a => a.ID == cooperationPlan.WBSID);
                        if (wbs != null)
                            wbs.Delete(false);
                    }
                }
            }
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> detail, List<Dictionary<string, string>> list, Base.Logic.Domain.S_UI_List listInfo, bool isNew)
        {
            base.BeforeSaveDetail(detail, list, listInfo, isNew);
            if (string.IsNullOrEmpty(detail.GetValue("Code")))
            {
                //string code = detail.GetValue("Name") + "." + detail.GetValue("ProjectInfoID") + "." + detail.GetValue("PhaseValue") + "." + detail.GetValue("MileStoneType") + "." + detail.GetValue("Major");
                detail.SetValue("Code", detail.GetValue("ID"));
                detail.SetValue("MileStoneValue", detail.GetValue("ID"));
            }
        }

        protected override void AfterSave(List<Dictionary<string, string>> list, List<Dictionary<string, string>> deleteList, Base.Logic.Domain.S_UI_List listInfo)
        {
            base.AfterSave(list, deleteList, listInfo);

            var wbsid = GetQueryString("NodeWBSID");
            var wbs = this.BusinessEntities.Set<S_W_WBS>().FirstOrDefault(a => a.ID == wbsid);
            if (wbs == null)
                throw new BusinessException("未获得WBS节点！");
            var mileStoneList = this.BusinessEntities.Set<S_P_MileStone>().Where(a => a.WBSID == wbsid).ToList();// wbs.S_I_ProjectInfo.S_P_MileStone.Where(a => a.WBSID == wbs.ID).ToList();
            var planList = this.BusinessEntities.Set<S_P_CooperationPlan>().Where(a => a.SchemeWBSID == wbsid).ToList();// wbs.S_I_ProjectInfo.S_P_CooperationPlan.Where(a => a.SchemeWBSID == wbs.ID).ToList();
            foreach (var mileStone in mileStoneList)
            {
                mileStone.Save();
                if (mileStone.MileStoneType == Project.Logic.MileStoneType.Cooperation.ToString())
                {
                    var cooperationPlan = planList.FirstOrDefault(d => d.MileStoneID == mileStone.ID);
                    if (cooperationPlan == null)
                    {
                        cooperationPlan = new S_P_CooperationPlan();
                        cooperationPlan.InMajorValue = mileStone.OutMajorValue;
                        cooperationPlan.OutMajorValue = mileStone.MajorValue;
                        cooperationPlan.MileStoneID = mileStone.ID;
                        cooperationPlan.ID = FormulaHelper.CreateGuid();
                        cooperationPlan.CooperationValue = mileStone.MileStoneValue;
                        cooperationPlan.OrPlanFinishDate = mileStone.PlanFinishDate;
                    }
                    cooperationPlan.CooperationContent = mileStone.Name;
                    if (!cooperationPlan.OrPlanFinishDate.HasValue)
                        cooperationPlan.OrPlanFinishDate = mileStone.PlanFinishDate;
                    cooperationPlan.PlanFinishDate = mileStone.PlanFinishDate;
                    wbs.SaveCooperationPlan(cooperationPlan);
                }
            }
            this.BusinessEntities.SaveChanges();
        }

        public string CheckDelete(string IDs)
        {
            var error = string.Empty;
            var idAry = IDs.Split(',').ToArray();
            var mileStoneList = this.BusinessEntities.Set<S_P_MileStone>().Where(a => idAry.Contains(a.ID)).ToList();
            foreach (var mileStone in mileStoneList)
            {
                if (mileStone.S_P_MileStone_ProductDetail.Count > 0)
                {
                    error += string.Format("【{0}】下已经有文件</br>", mileStone.Name);
                    continue;
                }
                if (mileStone.State == ProjectCommoneState.Finish.ToString())
                    error += string.Format("【{0}】已经完成</br>", mileStone.Name);
            }
            return error;
        }
    }
}
