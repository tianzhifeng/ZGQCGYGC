using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_W_CooperationExe
    {
        public void Confirm()
        {
            var entities = Formula.FormulaHelper.GetEntities<ProjectEntities>();
            var userInfo = Formula.FormulaHelper.GetUserInfo();
            if (!String.IsNullOrEmpty(this.ReceiveState))
                throw new Formula.Exceptions.BusinessException("已经接收过或拒绝过得资料不能重复操作");
            this.ReceiveState = "已接收";
            this.ReceiveDate = DateTime.Now;
            this.ReceiveUser = userInfo.UserName;
            this.ReceiveUserID = userInfo.UserID;

            var reciverCount = entities.Set<S_W_CooperationExe>().Count(d => d.BatchID == this.BatchID && d.ReceiveState == "已接收" && d.ID != this.ID);
            var allCount = entities.Set<S_W_CooperationExe>().Count(d => d.BatchID == this.BatchID && d.ID != this.ID);
            if (reciverCount == allCount)
            {
                var form = entities.Set<T_EXE_MajorPutInfo>().Find(this.BatchID);
                if (form != null)
                {
                    form.RecieveDate = DateTime.Now;
                    foreach (var fetchItem in form.T_EXE_MajorPutInfo_FetchDrawingInfo.ToList())
                    {
                        var sharInfo = entities.S_D_ShareInfo.Find(fetchItem.ShareFileID);
                        if (sharInfo != null)
                        {
                            sharInfo.AuditState = "Pass";
                        }
                    }
                }
            }
            var mileStone = entities.S_P_MileStone.FirstOrDefault(d => d.ID == this.RelateMileStoneID);
            if (mileStone != null)
            {

                if (reciverCount == allCount)
                    mileStone.Finish(DateTime.Now);
            }
            var plan = entities.Set<S_P_CooperationPlan>().FirstOrDefault(d => d.ID == this.RelatePlanID);
            if (plan != null && plan.State != MileStoneState.Finish.ToString())
            {
                plan.FactFinishDate = DateTime.Now;
                plan.State = MileStoneState.Finish.ToString();
            }
        }

        public void Refuse()
        {
            var userInfo = Formula.FormulaHelper.GetUserInfo();
            if (!String.IsNullOrEmpty(this.ReceiveState))
                throw new Formula.Exceptions.BusinessException("已经接收过或拒绝过得资料不能重复操作");
            this.ReceiveState = "已拒绝";
            this.ReceiveDate = DateTime.Now;
            this.ReceiveUser = userInfo.UserName;
            this.ReceiveUserID = userInfo.UserID;
        }
    }
}
