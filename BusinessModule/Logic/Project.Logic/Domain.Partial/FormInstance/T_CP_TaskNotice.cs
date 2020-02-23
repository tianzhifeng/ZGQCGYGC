using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Formula.Helper;

namespace Project.Logic.Domain
{
    public partial class T_CP_TaskNotice
    {

        /// <summary>
        /// 必要的验证逻辑
        /// </summary>
        public void Validate()
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            //if (string.IsNullOrEmpty(this.Phase))
            //    throw new Formula.Exceptions.BusinessException("选择项目阶段不可为空。");
            if (string.IsNullOrEmpty(this.SerialNumber))
                throw new Formula.Exceptions.BusinessException("项目编号不可为空。");
        }

        /// <summary>
        /// 多阶段只立一个项目
        /// </summary>
        /// <returns></returns>
        public S_I_ProjectInfo Push()
        {
            var context = this.GetDbContext<ProjectEntities>();
            string projectCode = this.SerialNumber;
            var projectInfo = new S_I_ProjectInfo
            {
                ID = string.IsNullOrEmpty(this.ProjectInfoID) ? FormulaHelper.CreateGuid() : this.ProjectInfoID,
                Name = this.ProjectInfo,
                Code = projectCode,
                PhaseValue = this.Phase,
                WorkContent = this.WorkContent,
                State = ProjectCommoneState.Plan.ToString(),
                ChargeUserName = this.ChargeUserName,
                ChargeUserID = this.ChargeUser,
                ChargeDeptID = this.ChargeDept,
                ChargeDeptName = this.ChargeDeptName,
                OtherDeptID = this.OtherDept,
                OtherDeptName = this.OtherDeptName,
                PlanStartDate = this.PlanStartDate,
                PlanFinishDate = this.PlanFinishDate,
                CustomerID = this.Customer,
                CustomerName = this.CustomerName,
                CustomerSub = this.CustomerSub,
                CustomerSubName = this.CustomerSubName,
                CompletePercent = 0,
                ProjectClass = this.ProjectClass,
                ProjectSpecialty = this.ProjectSpecialty,
                Country = this.Country,
                Province = this.Province,
                Area = this.Area,
                CustomerRequireInfoID = this.CustomerRequestReview,
                City = this.City,
                CoopUnitID = this.CoopUnitID,
                CoopUnitIDName = this.CoopUnitIDName,
                Long = this.Long,
                Lat = this.Lat,
                Address = this.Address,
                District = this.District
            };
            SyncExtention(projectInfo);
            if (!String.IsNullOrEmpty(this.BuildArea))
            {
                decimal d;
                if (Decimal.TryParse(this.BuildArea, out d))
                    projectInfo.Proportion = d;
            }

            var majorEnumTable = BaseConfigFO.GetWBSEnum(WBSNodeType.Major);
            if (!string.IsNullOrEmpty(this.Major))
            {
                var major = this.Major.Split(',').Select(
                        c => new
                        {
                            Name = majorEnumTable.Select("value='" + c + "'")[0]["text"],
                            Value = c,
                        }
                    );
                projectInfo.Major = JsonHelper.ToJson(major);
            }
            var phaseName = "";
            var phaseList = BaseConfigFO.GetWBSAttrList(WBSNodeType.Phase);
            var list = phaseList.Where(d => projectInfo.PhaseValue.Contains(d.Code)).ToList();
            foreach (var item in list)
            {
                phaseName += item.Name + ",";
            }
            projectInfo.PhaseName = phaseName.TrimEnd(',');
            projectInfo.MarketProjectInfoID = this.MarketProjectID;
            projectInfo.Build();
            projectInfo.ProjectLevel = String.IsNullOrEmpty(this.ProjectLevel) ? 10 : Convert.ToInt32(this.ProjectLevel);
            if (projectInfo.CBSRoot != null)
            {
                projectInfo.CBSRoot.Quantity = this.Workload ?? 0;
                projectInfo.CBSRoot.UnitPrice = this.WorkloadUnitPrice ?? 1;
                if (projectInfo.CBSRoot.Quantity.HasValue && projectInfo.CBSRoot.UnitPrice.HasValue)
                    projectInfo.CBSRoot.TotalPrice = projectInfo.CBSRoot.Quantity.Value * projectInfo.CBSRoot.UnitPrice.Value;
            }
            context.SaveChanges();
            if (String.IsNullOrEmpty(this.MultiProjMode) || this.MultiProjMode.ToLower() != "1")
                this.ProjectInfoID = projectInfo.ID;
            context.SaveChanges();
            return projectInfo;
        }

        public S_I_ProjectInfo UpGrade()
        {
            var projectEntities = this.GetDbContext<ProjectEntities>();
            var projectInfo = projectEntities.S_I_ProjectInfo.FirstOrDefault(d => d.ID == this.ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("没有找到指定的项目信息，任务单无法升版");
            projectInfo.Name = this.ProjectInfo;
            projectInfo.Code = this.SerialNumber;
            projectInfo.Name = this.ProjectInfo;
            projectInfo.PhaseValue = this.Phase;
            var phaseName = "";
            var phaseList = BaseConfigFO.GetWBSAttrList(WBSNodeType.Phase);
            var list = phaseList.Where(d => projectInfo.PhaseValue.Contains(d.Code)).ToList();
            foreach (var item in list)
            {
                phaseName += item.Name + ",";
            }
            projectInfo.PhaseName = phaseName.TrimEnd(',');
            projectInfo.WorkContent = this.WorkContent;
            projectInfo.ChargeUserName = this.ChargeUserName;
            projectInfo.ChargeUserID = this.ChargeUser;
            projectInfo.ChargeDeptID = this.ChargeDept;
            projectInfo.ChargeDeptName = this.ChargeDeptName;
            projectInfo.OtherDeptID = this.OtherDept;
            projectInfo.OtherDeptName = this.OtherDeptName;
            projectInfo.PlanStartDate = this.PlanStartDate;
            projectInfo.PlanFinishDate = this.PlanFinishDate;
            projectInfo.CustomerID = this.Customer;
            projectInfo.CustomerName = this.CustomerName;
            projectInfo.CustomerSub = this.CustomerSub;
            projectInfo.CustomerSubName = this.CustomerSubName;
            projectInfo.ProjectClass = this.ProjectClass;
            projectInfo.ProjectSpecialty = this.ProjectSpecialty;
            projectInfo.Country = this.Country;
            projectInfo.Province = this.Province;
            projectInfo.CustomerRequireInfoID = this.CustomerRequestReview;
            projectInfo.City = this.City;
            projectInfo.CoopUnitID = this.CoopUnitID;
            projectInfo.CoopUnitIDName = this.CoopUnitIDName;
            SyncExtention(projectInfo);
            projectInfo.WBSRoot.Name = this.ProjectInfo;
            projectInfo.WBSRoot.PhaseCode = projectInfo.PhaseValue;
            if (!string.IsNullOrEmpty(this.ChargeUser))
                projectInfo.WBSRoot.SetUsers(ProjectRole.ProjectManager.ToString(), this.ChargeUser.Split(','), true, true);
            if (projectInfo.CBSRoot != null)
            {
                projectInfo.CBSRoot.Quantity = this.Workload ?? 0;
                projectInfo.CBSRoot.UnitPrice = this.WorkloadUnitPrice ?? 1;
                if (projectInfo.CBSRoot.Quantity.HasValue && projectInfo.CBSRoot.UnitPrice.HasValue)
                    projectInfo.CBSRoot.TotalPrice = projectInfo.CBSRoot.Quantity.Value * projectInfo.CBSRoot.UnitPrice.Value;
            }

            if (!String.IsNullOrEmpty(this.BuildArea))
                projectInfo.Proportion = Convert.ToDecimal(this.BuildArea);

            projectInfo.GroupInfo.Name = projectInfo.Name;
            projectInfo.GroupInfo.Code = projectInfo.Code;
            projectInfo.GroupInfo.ChargeUser = projectInfo.ChargeUserID;
            projectInfo.GroupInfo.ChargeUserName = projectInfo.ChargeUserName;
            projectInfo.GroupInfo.DeptID = projectInfo.ChargeDeptID;
            projectInfo.GroupInfo.DeptName = projectInfo.ChargeDeptName;
            projectInfo.GroupInfo.Province = projectInfo.Province;
            projectInfo.GroupInfo.City = projectInfo.City;
            projectInfo.GroupInfo.PhaseValue = projectInfo.PhaseValue;
            projectInfo.GroupInfo.PhaseContent = projectInfo.PhaseName;
            return projectInfo;
        }

        //同步扩展字段至项目
        public void SyncExtention(S_I_ProjectInfo project)
        {
            project.Extention1 = this.Extention1;
            project.Extention2 = this.Extention2;
            project.Extention3 = this.Extention3;
            project.Extention4 = this.Extention4;
            project.Extention5 = this.Extention5;
            project.Extention6 = this.Extention6;
            project.Extention7 = this.Extention7;
            project.Extention8 = this.Extention8;
            project.Extention9 = this.Extention9;
            project.Extention10 = this.Extention10;
        }

        public void Delete()
        {
            if (this.FlowPhase == "End" || !String.IsNullOrEmpty(this.ProjectInfoID)) throw new Formula.Exceptions.BusinessException("已经立项的任务单无法删除");
            this.GetDbContext<ProjectEntities>().T_CP_TaskNotice.Delete(d => d.ID == this.ID);
        }
    }
}
