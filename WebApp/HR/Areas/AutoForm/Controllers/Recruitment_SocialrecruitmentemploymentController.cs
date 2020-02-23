using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula.Helper;
using MvcAdapter;
using Config;
using Config.Logic;
using Formula;

namespace HR.Areas.AutoForm.Controllers
{
    public class Recruitment_SocialrecruitmentemploymentController : HRFormContorllor<T_Recruitment_Socialrecruitmentemployment>
    {
        //
        // GET: /AutoForm/Recruitment_Socialrecruitmentemployment/

        public override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            string ID = taskExec.S_WF_InsTask.S_WF_InsFlow.FormInstanceID;

            var entity = BusinessEntities.Set<T_Recruitment_Socialrecruitmentemployment>().Find(ID);

            if (routing.Code == "Pass")
            {
                entity.IsPass = "通过";
                BusinessEntities.SaveChanges();
            }

            if (routing.Code == "NoPass")
            {
                entity.IsPass = "未通过";
                BusinessEntities.SaveChanges();
            }

            var isFlowComplete = base.ExecTaskExec(taskExec, routing, nextExecUserIDs, nextExecUserNames, nextExecUserIDsGroup, nextExecRoleIDs, nextExecOrgIDs, execComment);
            return isFlowComplete;
        }

        protected override void OnFlowEnd(T_Recruitment_Socialrecruitmentemployment entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var currentUser = FormulaHelper.GetUserInfo();

            var sentity = new S_E_Recruitment_Socialrecruitmentemployment()
            {
                ID = FormulaHelper.CreateGuid(),
                CreateUserID = currentUser.UserID,
                CreateUser = currentUser.UserName,
                CreateDate = DateTime.Now,
                T_Recruitment_SocialrecruitmentemploymentID = entity.ID,

                Major = entity.Professional,
                Year = DateTime.Now.Year,
                Dept = entity.Employmentdept,
                DeptName = entity.EmploymentdeptName,
                IsPass = entity.IsPass,

                Fillperson = entity.Fillperson,
                Filldate = entity.Filldate,
                Candidatename = entity.Candidatename,
                Position = entity.Position,
                FillpersonName = entity.FillpersonName,

                Monthlysalary = entity.Monthlysalary,
                Annualsalary = entity.Annualsalary,
                Pretest = entity.Pretest,
            };

            BusinessEntities.Set<S_E_Recruitment_Socialrecruitmentemployment>().Add(sentity);
            BusinessEntities.SaveChanges();

        }


        /// <summary>
        /// 社会招聘动态分析
        /// </summary>
        /// <returns></returns>
        public ActionResult Recruitment_SocialDynamic()
        {
            return View();
        }

        public JsonResult GetSociaDynamicList(QueryBuilder qb)
        {
            var year = DateTime.Now.Year.ToString();

            string yearQuery = this.Request["year"] ?? "";

            if (!string.IsNullOrEmpty(yearQuery))
            {
                year = yearQuery;
            }

            var linkSql = " where A.Year = '" + year + "'";

            //部门：提过需求的所有的部门
            var deptSql = @"SELECT Dept, DeptName FROM S_E_Recruitment_DeptRequirementReport_SZRCXQJH A " + linkSql + " GROUP BY Dept,DeptName";
            var deptDt = HRSQLDB.ExecuteDataTable(deptSql);

            var sumSql = @"SELECT count(1) as Num, Dept, IsPass FROM S_E_Recruitment_Socialrecruitmentemployment A " + linkSql + " GROUP BY Dept,IsPass";
            var sumDt = HRSQLDB.ExecuteDataTable(sumSql);

            var needSql = @"SELECT sum(Number) as Num, Dept FROM S_E_Recruitment_DeptRequirementReport_SZRCXQJH A " + linkSql + " GROUP BY Dept";
            var needDt = HRSQLDB.ExecuteDataTable(needSql);

            var entrySql = @"select count(1) as Num, A.Dept from  S_E_Recruitment_Socialrecruitmentemployment A " + linkSql + 
                @"and (select count(1) as num from T_Employee B where B.Interview = A.ID) != 0  GROUP BY A.dept";
            var entryDt = HRSQLDB.ExecuteDataTable(entrySql);

            deptDt.Columns.Add("Pass"); //面试通过人数
            deptDt.Columns.Add("All"); //面试总人数
            deptDt.Columns.Add("PassRate"); //面试通过率

            deptDt.Columns.Add("Need"); //招聘需求人数
            deptDt.Columns.Add("Entry"); //入职人数
            deptDt.Columns.Add("EntryRate"); //入职率
            deptDt.Columns.Add("FinishRate"); //招聘完成率

            var result = new List<Dictionary<string, object>>();
            var json = JsonHelper.ToJson(deptDt);
            result = JsonHelper.ToList(json);

            foreach (var item in result)
            {
                var passNumber = sumDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "'" + @" and IsPass = '通过' "); //面试通过人数
                var allNumber = sumDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "'"); //面试总人数

                var pass = passNumber == DBNull.Value ? 0 : passNumber;
                var all = allNumber == DBNull.Value ? 0 : allNumber;

                double passRate = Convert.ToInt32(all) != 0 ? Convert.ToDouble(pass) / Convert.ToDouble(all) * 100 : 0; //面试通过率=面试审批通过人数/面试审批总人数

                var needNumber = needDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "'"); //招聘需求人数
                var entryNumber = entryDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "'"); //入职人数

                var entry = entryNumber == DBNull.Value ? 0 : entryNumber;
                var need = needNumber == DBNull.Value ? 0 : needNumber;

                double entryRate = Convert.ToInt32(pass) != 0 ? Convert.ToDouble(entry) / Convert.ToDouble(pass) * 100 : 0; //入职率=入职人数/面试审批通过人数
                double finishRate = Convert.ToInt32(need) != 0 ? Convert.ToDouble(entry) / Convert.ToDouble(need) * 100 : 0; //招聘完成率=入职人数/招聘计划人数

                item.SetValue("Pass", pass);
                item.SetValue("All", all);
                item.SetValue("PassRate", passRate.ToString("0.00"));
                item.SetValue("Need", need);
                item.SetValue("Entry", entry);
                item.SetValue("EntryRate", entryRate.ToString("0.00"));
                item.SetValue("FinishRate", finishRate.ToString("0.00"));
            }

            var dataGrid = new GridData(result);
            dataGrid.total = qb.TotolCount;
            return Json(dataGrid);
        }
    }
}
