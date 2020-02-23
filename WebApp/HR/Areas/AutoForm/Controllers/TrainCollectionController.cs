using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Config;
using System.Data;
/**
 * 公司级培训需求收集
 * **/
namespace HR.Areas.AutoForm.Controllers
{
    public class TrainCollectionController : HRFormContorllor<T_Trainmanagement_Trainneedcollection>
    {

        public JsonResult GetDeptManage(string DeptID)
        {
            string userID = "";
            string userName = "";
            if (!string.IsNullOrEmpty(DeptID))
            {
                var DeptMainRoleCode = "部门正职"; //部门正职角色编号

                var sql = @"select org.ID,org.Name orgName,u.ID UserID , u.Name UserName ,r.Code RoleCode , r.ID RoleID
from S_A_Org org 
left join  S_A__OrgRoleUser  orgRole  on org.ID = orgRole.OrgID 
left join S_A_Role r  on orgRole.RoleID = r.ID
left join S_A_User  u on  u.ID = orgRole.UserID 
where   org.ID ='{0}' and r.code= '{1}'  ";

                var baseDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                var dt = baseDB.ExecuteDataTable(string.Format(sql, DeptID, DeptMainRoleCode));


                foreach (DataRow dr in dt.Rows)
                {
                    userID += dr["UserID"] + ",";
                    userName += dr["UserName"] + ",";
                }
                if (userID != "")
                    userID = userID.TrimEnd(',');
                if (userName != "")
                    userName = userName.TrimEnd(',');
            }
            return Json(new { value = userID, text = userName });
        }

        protected override void OnFlowEnd(T_Trainmanagement_Trainneedcollection entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity == null)
                return;

            //流程结束并且同意的培训需求写入结果表
            var list = entity.T_Trainmanagement_Trainneedcollection_Trainneed.Where(p => p.Yesorno == "True").ToList();
            foreach (var item in list)
            {

                S_Train_CompanyTask task = new S_Train_CompanyTask();
    
                task.ID = Formula.FormulaHelper.CreateGuid();
                task.FormID = entity.ID;
                task.Trainyear = entity.Trainyear;
                task.Needdept = entity.Needdept;
                task.NeeddeptName = entity.NeeddeptName;
                task.Puttime = entity.Puttime;
                task.FormID = entity.ID;
                task.Trainproject = item.Trainproject;
                task.Trainobject = item.Trainobject;
                task.Content = item.Content;
                task.Traintype = item.Traintype;
                task.Trainclasses = item.Trainclasses;
                task.Time = item.Time;
                task.Personnumber = item.Personnumber;
                task.Days = item.Days;
                task.Undertakedept = item.Undertakedept;
                task.UndertakedeptName = item.UndertakedeptName;
                task.Remark = item.Remark;
                BusinessEntities.Set<S_Train_CompanyTask>().Add(task);
            
            }
            BusinessEntities.SaveChanges();
        }

    }
}
