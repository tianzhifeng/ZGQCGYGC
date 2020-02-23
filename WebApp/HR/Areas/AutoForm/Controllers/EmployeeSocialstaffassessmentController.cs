using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Config;
using Formula.Exceptions;
using Config;
using Config.Logic;


namespace HR.Areas.AutoForm.Controllers
{
    public class EmployeeSocialstaffassessmentController : HRFormContorllor<T_Employee_Socialstaffassessment>
    {
        // GET: /AutoForm/EmployeeSocialstaffassessment/

        public JsonResult GetDirector(string UserID, string DeptID)
        {
            var Employee = BusinessEntities.Set<T_Employee>().Where(c => c.UserID == UserID).FirstOrDefault();
            if (Employee == null) throw new BusinessException("员工不存在！");

            var dic = new Dictionary<string, string>();

            var ApplyUser = UserID;
            var ApplyDept = DeptID;
            var sql = "select Character from S_A_Org where ID='" + ApplyDept + "'";
            var BasesqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var orgDt = BasesqlDB.ExecuteDataTable(sql);

            var Character = orgDt.Rows[0]["Character"].ToString();
            if (Character == "二级部门")
            {//如果当前人的党群部门是二级部门则送所长，如果当前人是所长送部门正职
                var RoleSql = "select  UserID from  S_A_Role a inner join S_A__OrgRoleUser b on a.ID=b.RoleID where a.Code='所级正职领导' and b.orgID='" + ApplyDept + "' ";
                var RoleDt = BasesqlDB.ExecuteDataTable(RoleSql);
                if (RoleDt.Rows.Count > 0)
                {

                    var userID = RoleDt.Rows[0]["UserID"].ToString();
                    if (userID != ApplyUser)
                        dic.SetValue("Director", userID);

                    RoleSql = @"select * from  S_A_Role a inner join S_A__OrgRoleUser b on a.ID=b.RoleID 
inner join (select ParentID from S_A_Org where ID='" + ApplyDept + @"' ) c on c.ParentID=b.orgID
where a.Code='部门正职' ";
                    RoleDt = BasesqlDB.ExecuteDataTable(RoleSql);
                    if (RoleDt.Rows.Count > 0)
                        dic.SetValue("Director", RoleDt.Rows[0]["UserID"].ToString());


                }

            }
            else
            {
                var RoleSql = "select  UserID from  S_A_Role a inner join S_A__OrgRoleUser b on a.ID=b.RoleID where a.Code='部门正职' and b.orgID='" + ApplyDept + "' ";
                var RoleDt = BasesqlDB.ExecuteDataTable(RoleSql);
                if (RoleDt.Rows.Count > 0)
                {
                    var userID = RoleDt.Rows[0]["UserID"].ToString();
                    if (userID != ApplyUser)
                        dic.SetValue("Director", userID);
                    else
                    {
                        RoleSql = "select  UserID from  S_A_Role a inner join S_A__OrgRoleUser b on a.ID=b.RoleID where a.Code='上级主管领导' and b.orgID='" + ApplyDept + "' ";
                        RoleDt = BasesqlDB.ExecuteDataTable(RoleSql);
                        if (RoleDt.Rows.Count > 0)
                        {
                            userID = RoleDt.Rows[0]["UserID"].ToString();
                            dic.SetValue("Director", userID);

                        }
                    }
                }
            }

            var nameSql = "select Name from S_A_User where id = '" + dic.GetValue("Director") + "'";
            var nameDt = BasesqlDB.ExecuteDataTable(nameSql);
            var directorName = "";
            if (nameDt.Rows.Count > 0)
            {
                directorName = nameDt.Rows[0]["Name"].ToString();
            }

            dic.SetValue("DirectorName", directorName);

            return Json(dic);
        }

    }
}
