using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Config.Logic;
/**
 * 人员岗位资质管理
 * **/
namespace HR.Areas.Qualification.Controllers
{
    public class UserQualificationManagementController : HRFormContorllor<T_Qualification_Postqualificationmanagement>
    {

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                dic.SetValue("IsApprove", "False");
            }
            else
            {
                var entity = BusinessEntities.Set<T_Qualification_Postqualificationmanagement>().Find(dic.GetValue("ID"));
                if (entity.IsApprove == "True")
                {
                    throw new Exception("当前岗位资格记录已经审批通过，不能修改");
                }
                if (string.IsNullOrEmpty(entity.T_Qualification_PostTotalID))
                    return;
                var postTotal = BusinessEntities.Set<T_Qualification_PostTotal>().Find(entity.T_Qualification_PostTotalID);
                if (postTotal != null)
                {
                    if (!string.IsNullOrEmpty(postTotal.FlowPhase) && (postTotal.FlowPhase == "End" || postTotal.FlowPhase == "Processing"))
                        throw new Exception("当前岗位资格记录已发起审批，不能修改");
                }
            }
        }
        /// <summary>
        /// 获取未审批的个人资质数据
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public JsonResult GetUserQualification(string UserID)
        {
            var entity = BusinessEntities.Set<T_Qualification_Postqualificationmanagement>().FirstOrDefault(p => p.Users == UserID && p.IsApprove != "True");
            if (entity == null)
            {
                var Qualification = BusinessEntities.Set<S_Qualification_Postqualificationmanagement>().FirstOrDefault(p => p.Users == UserID);
                if (Qualification != null)
                {
                    var dic = Qualification.ToDic();
                    dic.SetValue("ID", Formula.FormulaHelper.CreateGuid());
                    return Json(dic);
                }

                var emp = BusinessEntities.Set<T_Employee>().FirstOrDefault(p => p.UserID == UserID);
                if (emp != null)
                {
                    entity = new T_Qualification_Postqualificationmanagement();
                    entity.ID = Formula.FormulaHelper.CreateGuid();

                    var RegisteredqualificationList = BusinessEntities.Set<T_EmployeeQualification>().Where(p => p.EmployeeID == emp.ID && p.Type == "注册执业资格证书").Select(s => s.Certificatename).ToList();
                    var TitleList = BusinessEntities.Set<T_EmployeeQualification>().Where(p => p.EmployeeID == emp.ID && p.Type == "职称证书").Select(s => s.Certificatename).ToList();
                    entity.Number = emp.Code;
                    entity.Department = emp.DeptID;
                    entity.DepartmentName = emp.DeptIDName;
                    entity.Title = string.Join(",", TitleList);
                    entity.Registeredqualification = string.Join(",", RegisteredqualificationList);
                }

                return Json(entity);
            }
            else
            {
                return Json(entity);
            }
        }

    }
}
