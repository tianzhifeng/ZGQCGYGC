using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula.Exceptions;
using Config;
using Config.Logic;
namespace HR.Areas.AutoForm.Controllers
{
    public class AttendanceLeaveApplyController : HRFormContorllor<T_AttendanceLeaveApply>
    {



        public JsonResult GetUserInfo()
        {
            var UserID = GetQueryString("UserID");
            var Employee = BusinessEntities.Set<T_Employee>().Where(c => c.UserID == UserID).FirstOrDefault();
            if (Employee == null) throw new BusinessException("员工不存在！");
            var dic = new Dictionary<string, string>();
            dic.SetValue("Number", Employee.Code);
            dic.SetValue("ApplyDept", Employee.DeptID);
            dic.SetValue("ApplyDeptName", Employee.DeptIDName);
            dic.SetValue("Counterfeitdecoy", Employee.Name);
            var Vacationdays = "0";
            var sql = "select sum(Actualdays) as Actualdays from T_AttendanceLeaveApply  where FlowPhase='End' and year(StartDate)=year(getdate()) and ApplyUser='" + UserID + "'";
            var dt = HRSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
                Vacationdays = dt.Rows[0]["Actualdays"].ToString();
            dic.SetValue("Vacationdays", Vacationdays);
            dic.SetValue("Workingyears", Employee.Lengthofservice);



            var ApplyUser = UserID;
            var ApplyDept = Employee.DeptID;
            sql = "select Character from S_A_Org where ID='" + ApplyDept + "'";
            var BasesqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var orgDt = BasesqlDB.ExecuteDataTable(sql);
            dic.SetValue("IsPrincipal", "0");
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
                        dic.SetValue("Principal", RoleDt.Rows[0]["UserID"].ToString());


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
                        dic.SetValue("Principal", userID);
                    else
                    {
                        RoleSql = "select  UserID from  S_A_Role a inner join S_A__OrgRoleUser b on a.ID=b.RoleID where a.Code='上级主管领导' and b.orgID='" + ApplyDept + "' ";
                        RoleDt = BasesqlDB.ExecuteDataTable(RoleSql);
                        if (RoleDt.Rows.Count > 0)
                        {
                            userID = RoleDt.Rows[0]["UserID"].ToString();
                            dic.SetValue("Principal", userID);
                            dic.SetValue("IsPrincipal", "1");
                        }
                    }
                }
            }


            return Json(dic);
        }

        protected override void AfterGetData(System.Data.DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                var row = dt.Rows[0];
                var ApplyUser = row["ApplyUser"].ToString();
                var ApplyDept = row["ApplyDept"].ToString();
                var sql = "select Character from S_A_Org where ID='" + ApplyDept + "'";
                var BasesqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                var orgDt = BasesqlDB.ExecuteDataTable(sql);
                row["IsPrincipal"] = "0";
                var Character = orgDt.Rows[0]["Character"].ToString();
                if (Character == "二级部门")
                {//如果当前人的党群部门是二级部门则送所长，如果当前人是所长送部门正职
                    var RoleSql = "select  UserID from  S_A_Role a inner join S_A__OrgRoleUser b on a.ID=b.RoleID where a.Code='所级正职领导' and b.orgID='" + ApplyDept + "' ";
                    var RoleDt = BasesqlDB.ExecuteDataTable(RoleSql);
                    if (RoleDt.Rows.Count > 0)
                    {

                        var userID = RoleDt.Rows[0]["UserID"].ToString();
                        if (userID != ApplyUser)
                            row["Director"] = userID;

                        RoleSql = @"select * from  S_A_Role a inner join S_A__OrgRoleUser b on a.ID=b.RoleID 
inner join (select ParentID from S_A_Org where ID='" + ApplyDept + @"' ) c on c.ParentID=b.orgID
where a.Code='部门正职' ";
                        RoleDt = BasesqlDB.ExecuteDataTable(RoleSql);
                        if (RoleDt.Rows.Count > 0)
                            row["Principal"] = RoleDt.Rows[0]["UserID"].ToString();


                    }

                }//员工属于院级部门
                else
                {
                    var RoleSql = "select  UserID from  S_A_Role a inner join S_A__OrgRoleUser b on a.ID=b.RoleID where a.Code='部门正职' and b.orgID='" + ApplyDept + "' ";
                    var RoleDt = BasesqlDB.ExecuteDataTable(RoleSql);
                    if (RoleDt.Rows.Count > 0)
                    {
                        var userID = RoleDt.Rows[0]["UserID"].ToString();
                        if (userID != ApplyUser)
                            row["Principal"] = userID;
                        else
                        {
                            RoleSql = "select  UserID from  S_A_Role a inner join S_A__OrgRoleUser b on a.ID=b.RoleID where a.Code='上级主管领导' and b.orgID='" + ApplyDept + "' ";
                            RoleDt = BasesqlDB.ExecuteDataTable(RoleSql);
                            if (RoleDt.Rows.Count > 0)
                            {
                                userID = RoleDt.Rows[0]["UserID"].ToString();
                                row["Principal"] = userID;
                                row["IsPrincipal"] = "1";
                            }
                        }
                    }
                }
            }
        }
    }
}
