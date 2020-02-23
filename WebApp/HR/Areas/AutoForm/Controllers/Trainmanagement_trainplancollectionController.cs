using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using System.Web.ApplicationServices;
using System.Data;
using Config;
/**
 * 培训计划收集通知
 * **/
namespace HR.Areas.AutoForm.Controllers
{
    public class Trainmanagement_trainplancollectionController : HRFormContorllor<T_Trainmanagement_trainplancollection>
    {
        //
        // GET: /AutoForm/Trainmanagement_trainplancollection/

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 获取培训计划收集通知子表部门关联信息
        /// </summary>
        /// <param name="DetpIDs"></param>
        /// <returns></returns>
        public JsonResult GetDeptInfo(string DeptIDs)
        {
            if (string.IsNullOrEmpty(DeptIDs))
                throw new Formula.Exceptions.BusinessException("请选择部门");

            var DeptMainRoleCode = "部门正职"; //部门正职角色编号
            var DeptParentManageRoleCode = "上级主管领导"; //上级主管领导角色编号

            List<T_Trainmanagement_trainplancollection_Noticedept> list = new List<T_Trainmanagement_trainplancollection_Noticedept>();

            var sql = @"select org.ID,org.Name orgName,u.ID UserID , u.Name UserName ,r.Code RoleCode , r.ID RoleID
from S_A_Org org 
left join  S_A__OrgRoleUser  orgRole  on org.ID = orgRole.OrgID 
left join S_A_Role r  on orgRole.RoleID = r.ID and  r.Code  in({0})
left join S_A_User  u on  u.ID = orgRole.UserID 
where   org.ID in ({1})  ";

            var roleCodeWhere = string.Format("'{0}','{1}'", DeptMainRoleCode, DeptParentManageRoleCode);
            var orgIDWhere = "'" + DeptIDs.Replace(",", "','") + "'";

            var baseDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = baseDB.ExecuteDataTable(string.Format(sql, roleCodeWhere, orgIDWhere));

            foreach (DataRow item in dt.Rows)
            {
                var orgId = item["ID"].ToString();
                var entity = list.FirstOrDefault(p => p.Dept == orgId);
                if (entity == null)
                {
                    entity = new T_Trainmanagement_trainplancollection_Noticedept();
                    entity.Dept = item["ID"].ToString();
                    entity.DeptName = item["orgName"].ToString();
                    list.Add(entity);
                }

                //部门正职写
                if (item["RoleCode"].ToString() == DeptMainRoleCode && !string.IsNullOrEmpty(item["UserID"].ToString()))
                {
                    entity.Depttrainleader += string.IsNullOrEmpty(entity.Depttrainleader) ? item["UserID"].ToString() : "," + item["UserID"].ToString();
                    entity.DepttrainleaderName += string.IsNullOrEmpty(entity.DepttrainleaderName) ? item["UserName"].ToString() : "," + item["UserName"].ToString();
                    entity.Depthead += string.IsNullOrEmpty(entity.Depthead) ? item["UserID"].ToString() : "," + item["UserID"].ToString();
                    entity.DeptheadName += string.IsNullOrEmpty(entity.DeptheadName) ? item["UserName"].ToString() : "," + item["UserName"].ToString();
                }
                else if (item["RoleCode"].ToString() == DeptParentManageRoleCode && !string.IsNullOrEmpty(item["UserID"].ToString()))
                {
                    entity.Superiorleader += string.IsNullOrEmpty(entity.Superiorleader) ? item["UserID"].ToString() : "," + item["UserID"].ToString();
                    entity.SuperiorleaderName += string.IsNullOrEmpty(entity.SuperiorleaderName) ? item["UserName"].ToString() : "," + item["UserName"].ToString();
                }


            }


            return Json(list);
        }


        protected override void OnFlowEnd(T_Trainmanagement_trainplancollection entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {

            if (entity == null)
                return;
            var detailList = entity.T_Trainmanagement_trainplancollection_Noticedept.ToList();
            foreach (var item in detailList)
            {

                S_Train_DeptTask task = BusinessEntities.Set<S_Train_DeptTask>().FirstOrDefault(p => p.Dept == item.Dept && p.Trainyears == entity.Trainyears);
                if (task == null)
                {
                    task = new S_Train_DeptTask();
                    task.ID = Formula.FormulaHelper.CreateGuid();
                    task.Trainyears = entity.Trainyears;
                    task.Dept = item.Dept;
                    task.DeptName = item.DeptName;
                    task.IsSubmit = "False";
                    BusinessEntities.Set<S_Train_DeptTask>().Add(task);
                }
        
                task.Noticetime = entity.Noticetime;
                task.Deadline = entity.Deadline;
                task.Traincontent = entity.Traincontent;

             
                task.Depttrainleader = item.Depttrainleader;
                task.DepttrainleaderName = item.DepttrainleaderName;
                task.Depthead = item.Depthead;
                task.DeptheadName = item.DeptheadName;
                task.Superiorleader = item.Superiorleader;
                task.SuperiorleaderName = item.SuperiorleaderName;
               

            
            }
            BusinessEntities.SaveChanges();

        }

    }
}
