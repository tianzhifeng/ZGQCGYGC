using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Comprehensive.Logic.Domain;
using Base.Logic.Domain;
using Formula;

namespace Comprehensive.Logic.BusinessFacade
{
    public class EmployeeFo
    {
        BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
        ComprehensiveEntities entities = FormulaHelper.GetEntities<ComprehensiveEntities>();

        #region 根据职务信息同步Employee的部门、兼职部门

        /// <summary>
        /// 根据职务信息同步Employee的部门、兼职部门
        /// </summary>
        /// <param name="employee"></param>
        public void SyncDeptByJob(S_HR_Employee employee)
        {
            if (employee == null)
                return;
            //主责部门
            var mainJob = entities.Set<S_HR_EmployeeJob>().FirstOrDefault(c => c.IsMain == "1" && c.IsDeleted == "0" && c.EmployeeID == employee.ID);
            if (mainJob != null)
            {
                employee.Dept = mainJob.DeptID;
                employee.DeptName = mainJob.DeptIDName;
            }
            //兼职部门
            var allJobs = this.entities.Set<S_HR_EmployeeJob>().Where(c => c.EmployeeID == employee.ID && c.IsMain == "0" && c.IsDeleted == "0").ToList();
            string deptIds = string.Empty;
            string deptNames = string.Empty;
            if (allJobs != null && allJobs.Count > 0)
            {
                var tempdeptIds = allJobs.Select(c => c.DeptID).Distinct();
                var tempdeptNames = allJobs.Select(c => c.DeptIDName).Distinct();
                if (tempdeptIds != null && tempdeptIds.Count() > 0)
                    deptIds = string.Join(",", tempdeptIds);
                if (tempdeptNames != null && tempdeptNames.Count() > 0)
                    deptNames = string.Join(",", tempdeptNames);
            }
            if (!string.IsNullOrEmpty(deptIds))
            {
                employee.ParttimeDept = deptIds;
                employee.ParttimeDeptName = deptNames;
            }
            entities.SaveChanges();
        }

        #endregion

        #region 同步系统用户
        //新添雇员加入系统用户
        public void EmployeeAddToUser(S_HR_Employee employee, string Password)
        {
            if (employee == null || employee.IsDeleted == "1")
                return;
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            //添加新用户
            S_A_User user = new S_A_User()
            {
                ID = FormulaHelper.CreateGuid(),
                Code = employee.Code,
                Name = employee.Name,
                WorkNo = employee.Code,
                Address = employee.Address,
                Email = employee.Email,
                Password = Password,
                Sex = employee.Sex,
                InDate = employee.JoinCompanyDate,
                Phone = employee.OfficePhone,
                MobilePhone = employee.MobilePhone,
                IsDeleted = "0"
            };
            baseEntities.Set<S_A_User>().Add(user);

            //反写userID
            employee.UserID = user.ID;

            //添加用户图片
            S_A_UserImg userImg = new S_A_UserImg()
            {
                ID = FormulaHelper.CreateGuid(),
                UserID = user.ID,
                Picture = employee.Portrait,
                SignImg = employee.SignImage
            };
            baseEntities.Set<S_A_UserImg>().Add(userImg);

            SyncUserDeptByEmployee(employee, user);

            entities.SaveChanges();
            baseEntities.SaveChanges();

        }

        //更新雇员加入系统用户
        public void EmployeeUpdateToUser(S_HR_Employee employee)
        {
            S_A_User user = baseEntities.Set<S_A_User>().Where(t => t.ID == employee.UserID).FirstOrDefault();
            user.WorkNo = employee.Code;
            user.Code = employee.Code;
            user.Name = employee.Name;
            user.IsDeleted = employee.IsDeleted;

            if (employee.Address != null)
                user.Address = employee.Address;
            if (employee.Email != null)
                user.Email = employee.Email;
            if (employee.OfficePhone != null)
                user.Phone = employee.OfficePhone;
            if (employee.MobilePhone != null)
                user.MobilePhone = employee.MobilePhone;


            //修改用户图片
            S_A_UserImg userImg = null;
            List<S_A_UserImg> userImgList = baseEntities.Set<S_A_UserImg>().Where(c => c.UserID == user.ID).ToList();
            if (userImgList.Count > 0)
                userImg = userImgList.FirstOrDefault();
            else
            {
                userImg = new S_A_UserImg()
                {
                    ID = FormulaHelper.CreateGuid(),
                    UserID = user.ID
                };
                baseEntities.Set<S_A_UserImg>().Add(userImg);
            }

            if (employee.Portrait != null)
                userImg.Picture = employee.Portrait;
            if (employee.SignImage != null)
                userImg.SignImg = employee.SignImage;

            SyncUserDeptByEmployee(employee, user);

            baseEntities.SaveChanges();

        }
        
        /// <summary>
        /// 同步删除的人员到系统用户表
        /// </summary>
        /// <param name="employee"></param>
        public void EmployeeDeleteToUser(S_HR_Employee employee)
        {
            S_A_User user = baseEntities.Set<S_A_User>().Where(t => t.ID == employee.UserID).FirstOrDefault();
            if (user != null)
            {
                user.DeleteTime = DateTime.Now;
                user.IsDeleted = "1";
                var retiredList = entities.Set<S_HR_EmployeeRetired>().Where(c => c.EmployeeID == employee.ID).OrderBy("RetiredDate", false);
                if (retiredList.Count() > 0)
                {
                    user.OutDate = retiredList.FirstOrDefault().RetiredDate;
                }
                baseEntities.SaveChanges();
            }
        }
        /// <summary>
        /// 同步删除的人员到系统用户表
        /// </summary>
        /// <param name="EmployeeIds"></param>
        public void EmployeeDeleteToUser(string EmployeeIds)
        {
            var EList = entities.Set<S_HR_Employee>().Where(c => EmployeeIds.Contains(c.ID)).ToList();
            if (EList == null || EList.Count == 0)
                return;
            int count = 0;
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            foreach (var employee in EList)
            {
                var user = baseEntities.Set<S_A_User>().Where(t => t.ID == employee.UserID).FirstOrDefault();
                if (user != null)
                {
                    count++;
                    user.DeleteTime = DateTime.Now;
                    user.IsDeleted = "1";

                    var retiredList = entities.Set<S_HR_EmployeeRetired>().Where(c => c.EmployeeID == employee.ID).OrderBy("RetiredDate", false);
                    if (retiredList.Count() > 0)
                    {
                        user.OutDate = retiredList.FirstOrDefault().RetiredDate;
                    }
                }
            }
            if (count > 0)
                baseEntities.SaveChanges();
        }

        /// <summary>
        /// 恢复系统用户的账号状态
        /// </summary>
        /// <param name="EmployeeIds">员工基本信息表id</param>
        public void ResetSysUserState(string EmployeeIds)
        {
            if (string.IsNullOrWhiteSpace(EmployeeIds))
                return;
            string[] UserArr = entities.Set<S_HR_Employee>().Where(c => EmployeeIds.Contains(c.ID)).ToList().Select(t => t.UserID).ToArray();
            if (UserArr == null || UserArr.Length == 0)
                return;
            string UserIds = string.Join(",", UserArr);
            BaseEntities baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            baseEntities.Set<S_A_User>().Where(t => UserIds.Contains(t.ID)).Update(t =>
            {
                t.DeleteTime = null;
                t.IsDeleted = "0";
                t.OutDate = null;
            });
            baseEntities.SaveChanges();
        }

        #endregion
        #region 根据Employee的部门、兼职部门同步系统账户的部门
        /// <summary>
        /// 根据Employee的部门、兼职部门同步系统账户的部门
        /// </summary>
        /// <param name="employee"></param>
        public void SyncUserDeptByEmployee(S_HR_Employee employee, S_A_User user, bool needSaveChange = false)
        {
            //主责部门设置为当前部门
            S_A_Org org = null;
            if (employee.Dept != null)
                org = baseEntities.Set<S_A_Org>().Where(c => c.ID == employee.Dept && c.IsDeleted == "0").FirstOrDefault();

            if (org == null)
                org = baseEntities.Set<S_A_Org>().Where(c => c.ParentID == null || c.ParentID == "").FirstOrDefault();

            List<string> orgIDList = new List<string>();//所属部门列表
            if (org != null)
            {
                user.DeptID = org.ID;
                user.DeptName = org.Name;
                user.DeptFullID = org.FullID;

                string[] orgArray = org.FullID.Split('.');
                foreach (string orgID in orgArray)
                {
                    //该组织关系已经添加过则继续下一组织
                    if (orgIDList.Contains(orgID))
                        continue;
                    else
                        orgIDList.Add(orgID);
                }

            }

            //部门设置为所属部门
            if (!string.IsNullOrEmpty(employee.ParttimeDept))
            {
                foreach (var item in employee.ParttimeDept.Split(','))
                {
                    S_A_Org pOrg = baseEntities.Set<S_A_Org>().Where(c => c.ID == item && c.IsDeleted == "0").FirstOrDefault(); ;
                    if (pOrg == null)
                        continue;
                    string[] pOrgArray = pOrg.FullID.Split('.');
                    foreach (string orgID in pOrgArray)
                    {
                        //该组织关系已经添加过则继续下一组织
                        if (orgIDList.Contains(orgID))
                            continue;
                        else
                            orgIDList.Add(orgID);
                    }
                }
            }

            //差异更新，删除或增加
            var deleteList = baseEntities.Set<S_A__OrgUser>().Where(a => a.UserID == user.ID).ToList();
            foreach (var orgid in orgIDList)
            {
                var exist = deleteList.FirstOrDefault(a => a.OrgID == orgid);
                if (exist != null)
                    deleteList.Remove(exist);
                else
                    baseEntities.Set<S_A__OrgUser>().Add(new S_A__OrgUser() { OrgID = orgid, UserID = user.ID });
            }
            foreach (var item in deleteList)
                baseEntities.Set<S_A__OrgUser>().Remove(item);

            if (needSaveChange)
                baseEntities.SaveChanges();
        }

        #endregion
    }
}
