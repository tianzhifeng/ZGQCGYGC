using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;
using Formula;
using Formula.Helper;

namespace Project.Logic.Domain
{
    public partial class S_W_StandardWorkTime
    {
        public void CreateDetailInfo()
        {
            //创建设计人工日
            if (!string.IsNullOrEmpty(this.DesignerUserID))
                this.SynchWorkDaysByUserIDs(this.DesignerUserID, AuditRoles.Designer.ToString());
            //创建审核人工日
            if (!string.IsNullOrEmpty(this.CollactorUserID))
                this.SynchWorkDaysByUserIDs(this.CollactorUserID, AuditRoles.Collactor.ToString());
            //创建审核人工日
            if (!string.IsNullOrEmpty(this.AuditorUserID))
                this.SynchWorkDaysByUserIDs(this.AuditorUserID, AuditRoles.Auditor.ToString());
            //创建审定人工日
            if (!string.IsNullOrEmpty(this.ApproverUserID))
                this.SynchWorkDaysByUserIDs(this.ApproverUserID, AuditRoles.Approver.ToString());
            //创建制图人工日
            if (!string.IsNullOrEmpty(this.MapperUserID))
                this.SynchWorkDaysByUserIDs(this.MapperUserID, AuditRoles.Mapper.ToString());
        }

        public void ValidateStandardWorkDay()
        {
            var totalWorkDay = this.S_W_StandardWorkTimeDetail.Sum(c => c.WorkDay);
            var totalValue = 0.0;
            if (totalWorkDay != null)
                totalValue = Convert.ToDouble(totalWorkDay.ToString());

                if (totalValue != this.WorkDay)
                    throw new Formula.Exceptions.BusinessException("图号为【" + this.TaskWorkCode + "】的套图的人员总工日总和(" + totalWorkDay + ")，不等于该套图的标准工日(" + this.WorkDay + ")，保存信息失败。");

        }

        public void SynchWorkDaysByUserIDs(string userIDs, string roleCode)
        {
            var userIds = userIDs.Split(',');
            foreach (var userId in userIds)
                SynchWorkDaysBySingleUserId(userId, roleCode);
        }

        public void SynchWorkDaysBySingleUserId(string userId, string roleCode, float workDay = 0)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            var currentUserInfo = FormulaHelper.GetUserInfo();
            if (!this.IsUserExistedOfRoleCode(userId, roleCode))
            {
                var user = FormulaHelper.GetUserInfoByID(userId);
                S_W_StandardWorkTimeDetail detail = entities.Set<S_W_StandardWorkTimeDetail>().Create();
                detail.ID = FormulaHelper.CreateGuid();
                detail.UserID = user.UserID;
                detail.UserName = user.UserName;
                detail.WBSID = this.WBSID;
                detail.TaskWorkID = this.TaskWorkID;
                detail.ProjectInfoID = this.ProjectInfoID;
                detail.RoleCode = roleCode;
                var roleName = EnumBaseHelper.GetEnumDescription(typeof(Project.Logic.AuditRoles), roleCode);
                detail.RoleName = roleName;
                detail.WorkDay = workDay;
                detail.CreateDate = DateTime.Now;
                if (currentUserInfo != null)
                {
                    detail.CreateUser = currentUserInfo.UserName;
                    detail.CreateUserID = currentUserInfo.UserID;
                }
                this.S_W_StandardWorkTimeDetail.Add(detail);
            }
            else
            {
                var detail = entities.Set<S_W_StandardWorkTimeDetail>().SingleOrDefault(c => c.FormID == this.ID && c.UserID == userId && c.RoleCode == roleCode);
                detail.WorkDay = workDay;
                detail.ModifyDate = DateTime.Now;
                if (currentUserInfo != null)
                {
                    detail.ModifyUser = currentUserInfo.UserName;
                    detail.ModifyUserID = currentUserInfo.UserID;
                }
            }
        }

        public bool IsUserExistedOfRoleCode(string userId, string roleCode)
        {
            var details = this.S_W_StandardWorkTimeDetail.ToList();
            var existedDetails = details.Where(c => c.RoleCode == roleCode && c.UserID == userId).ToList();
            if (existedDetails != null && existedDetails.Count > 0)
                return true;
            else
                return false;
        }

    }
}
