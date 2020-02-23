using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;

namespace Project.Logic.Domain
{
    public partial class T_EXE_MajorPutInfo
    {
        /// <summary>
        /// 互提资料保存
        /// </summary>
        public void Publish(bool isFinish = false)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            S_P_CooperationPlan coopPlan = null;
            if (!string.IsNullOrEmpty(this.CoopPlanID))
            {
                foreach (var planID in this.CoopPlanID.Split(','))
                {
                    coopPlan = entities.Set<S_P_CooperationPlan>().FirstOrDefault(c => c.ID == planID);
                    if (coopPlan != null)
                    {
                        foreach (var inMajor in coopPlan.InMajorValue.Split(','))
                        {
                            var exe = CreateExe(inMajor, coopPlan);
                            entities.Set<S_W_CooperationExe>().Add(exe);
                        }

                        if (this.PassAuditState.ToLower() == "true")
                        {
                            coopPlan.State = ProjectCommoneState.Finish.ToString();
                            coopPlan.FactFinishDate = DateTime.Now;
                            if (!String.IsNullOrEmpty(coopPlan.MileStoneID))
                            {
                                var mileStone = entities.S_P_MileStone.Find(coopPlan.MileStoneID);
                                if (mileStone != null)
                                {
                                    mileStone.Finish(DateTime.Now);
                                }
                            }
                        }
                    }
                }
                coopPlan = entities.Set<S_P_CooperationPlan>().FirstOrDefault(c => c.ID == this.CoopPlanID);
                if (coopPlan != null && this.PassAuditState.ToLower() == "true")
                {
                    coopPlan.State = ProjectCommoneState.Finish.ToString();
                    coopPlan.FactFinishDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(coopPlan.MileStoneID))
                    {
                        var mileStone = entities.S_P_MileStone.Find(coopPlan.MileStoneID);
                        if (mileStone != null)
                        {
                            mileStone.Finish(DateTime.Now);
                        }
                    }
                }
            }
            else
            {
                foreach (var inMajor in this.InMajorValue.Split(','))
                {
                    var exe = CreateExe(inMajor);
                    entities.Set<S_W_CooperationExe>().Add(exe);
                }
            }
            var detailList = this.T_EXE_MajorPutInfo_FetchDrawingInfo.ToList();

            #region 共享文件
            var userInfo = FormulaHelper.GetUserInfoByID(this.CreateUserID);
            foreach (var item in detailList)
            {
                var shareInfo = entities.S_D_ShareInfo.Find(item.ShareFileID);
                if (shareInfo == null)
                {
                    shareInfo = entities.S_D_ShareInfo.Create();
                    shareInfo.ID = FormulaHelper.CreateGuid();
                    item.ShareFileID = shareInfo.ID;
                    shareInfo.FileName = item.FileName;
                    shareInfo.Code = item.Code;
                    shareInfo.Annex = item.MainFile;
                    shareInfo.CreateDate = DateTime.Now;
                    shareInfo.CreateUser = userInfo.UserName;
                    shareInfo.CreateUserID = userInfo.UserID;
                    shareInfo.ModifyDate = DateTime.Now;
                    shareInfo.ModifyUser = userInfo.UserName;
                    shareInfo.ModifyUserID = userInfo.UserID;
                    shareInfo.OrgID = userInfo.UserOrgID;
                    shareInfo.CompanyID = userInfo.UserCompanyID;
                    shareInfo.RegisterName = userInfo.UserName;
                    shareInfo.Register = userInfo.UserID;
                    shareInfo.PhaseValue = this.Phase;
                    shareInfo.ProjectInfoID = this.ProjectInfoID;
                    shareInfo.SubProjectWBSID = this.RelateWBSID;
                    shareInfo.FormID = this.ID;
                    shareInfo.Source = item.Source;
                    shareInfo.SourceType = item.SourceType;
                    shareInfo.Type = item.Type;
                    shareInfo.WBSValue = this.OutMajorValue;

                    entities.S_D_ShareInfo.Add(shareInfo);
                }
                if (this.PassAuditState.ToLower() == "true")
                    shareInfo.AuditState = "Pass";

                shareInfo.FormID = this.ID;
                if (coopPlan != null)
                {
                    shareInfo.CooperationPlan = coopPlan.ID;
                    shareInfo.CooperationPlanName = coopPlan.CooperationContent;
                }
            }
            #endregion
        }

        private S_W_CooperationExe CreateExe(string inMajor, S_P_CooperationPlan coopPlan = null)
        {
            var cooperationExe = new S_W_CooperationExe();
            cooperationExe.ID = FormulaHelper.CreateGuid();
            cooperationExe.Name = this.CooperationContent;
            cooperationExe.InMajor = inMajor;
            cooperationExe.OutMajor = this.OutMajorValue;
            cooperationExe.PicNo = this.PictureNum;
            cooperationExe.ProjectInfoID = this.ProjectInfoID;
            if (coopPlan != null)
            {
                cooperationExe.RelateMileStoneID = coopPlan.MileStoneID;
                cooperationExe.RelatePlanID = coopPlan.ID;
                cooperationExe.RelateWBSID = coopPlan.WBSID;
            }
            else
                cooperationExe.RelateWBSID = this.WBSID;
            if (this.PassAuditState.ToLower() == "true")
            {
                cooperationExe.ReceiveState = "已接收";
                cooperationExe.ReceiveDate = DateTime.Now;
            }
            cooperationExe.BatchID = this.ID;
            cooperationExe.FetchOutDate = DateTime.Now;
            cooperationExe.FetchOutUser = this.RegisterName;
            cooperationExe.FetchOutUserID = this.Register;
            cooperationExe.Files = this.Annex;
            cooperationExe.DrawingNo = this.AnnexNum;
            return cooperationExe;
        }
    }
}
