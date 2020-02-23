using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula;
using MvcAdapter;
using Config;
using Formula.Helper;
using System.Data;
using Config.Logic;

namespace HR.Areas.AutoForm.Controllers
{
    public class Recruitment_SchoolrecruitmentinterviewController : HRFormContorllor<T_Recruitment_Schoolrecruitmentinterview>
    {
        // GET: /HR/AutoForm/Recruitment_Schoolrecruitmentinterview/

        public override bool ExecTaskExec(Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing, string nextExecUserIDs, string nextExecUserNames, string nextExecUserIDsGroup, string nextExecRoleIDs, string nextExecOrgIDs, string execComment)
        {
            string ID = taskExec.S_WF_InsTask.S_WF_InsFlow.FormInstanceID;

            var entity = BusinessEntities.Set<T_Recruitment_Schoolrecruitmentinterview>().Find(ID);
            //var sentity = BusinessEntities.Set<S_E_Recruitment_Schoolrecruitmentinterview>().FirstOrDefault(p => p.T_Recruitment_SchoolrecruitmentinterviewID == ID);

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

        protected override void OnFlowEnd(T_Recruitment_Schoolrecruitmentinterview entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var currentUser = FormulaHelper.GetUserInfo();

            var sentity = new S_E_Recruitment_Schoolrecruitmentinterview()
            {
                ID = FormulaHelper.CreateGuid(),
                CreateUserID = currentUser.UserID,
                CreateUser = currentUser.UserName,
                CreateDate = DateTime.Now,
                T_Recruitment_SchoolrecruitmentinterviewID = entity.ID,

                Major = string.IsNullOrEmpty(entity.Professional) ? entity.Professional1 : entity.Professional,
                Year = DateTime.Now.Year,
                Dept = entity.InterviewDept,
                DeptName = entity.InterviewDeptName,
                IsPass = entity.IsPass,

                Applicantname = entity.Applicantname,
                Sex = entity.Sex,
                Birth = entity.Birth,
                Graduateschoolmaster = entity.Graduateschoolmaster,
                Professional = entity.Professional,
                Graduateschool = entity.Graduateschool,
                Professional1 = entity.Professional1,
                Graduatetime = entity.Graduatetime,
                Is211School = entity.Is211School,
                Is985School = entity.Is985School,
                Referrer = entity.Referrer,
                Interviewtime = entity.Interviewtime,

            };

            BusinessEntities.Set<S_E_Recruitment_Schoolrecruitmentinterview>().Add(sentity);
            BusinessEntities.SaveChanges();
        }

        /// <summary>
        /// 获取所有专业
        /// </summary>
        /// <returns></returns>
        private DataTable GetMajors()
        {
            var baseSqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);

            var sql = @"SELECT b.code as Major, B.name as MajorName FROM S_M_EnumDef A LEFT JOIN S_M_EnumItem B on a.id = b.EnumDefID where A.code = 'HR.Professional' ";
            
            var dt = baseSqlDB.ExecuteDataTable(sql);

            return dt;
        }

        #region 校园招聘面试通过率
        /// <summary>
        /// 校园招聘面试通过率
        /// </summary>
        /// <returns></returns>
        public ActionResult Recruitment_SchoolInterviewPassRate()
        {
            var dt = GetMajors();

            ViewBag.MyColumns = dt.Rows;

            return View();
        }


        /// <summary>
        /// 获取校园招聘面试通过率
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSchoolInterviewPassRate(QueryBuilder qb)
        {
            var year = DateTime.Now.Year.ToString();

            string yearQuery = this.Request["year"] ?? "";

            if (!string.IsNullOrEmpty(yearQuery))
            {
                year = yearQuery;
            }

            var linkSql = " where [Year] = '" + year + "'";


            //部门：提过需求的所有的部门
            var deptSql = @"SELECT Dept, DeptName FROM S_E_Recruitment_DeptRequirementReport " + linkSql + " GROUP BY Dept,DeptName";
            var deptDt = HRSQLDB.ExecuteDataTable(deptSql);
            //专业：所有的枚举
            var majorDt = GetMajors();
            //校招面试
            var sumSql = @"SELECT count(1) as Num, Major, Dept, IsPass FROM S_E_Recruitment_Schoolrecruitmentinterview " + linkSql + " GROUP BY Major, Dept,IsPass";
            var sumDt = HRSQLDB.ExecuteDataTable(sumSql);

            foreach (DataRow dr in majorDt.Rows)
            {
                if (!deptDt.Columns.Contains(dr["Major"].ToString()))
                {
                    deptDt.Columns.Add(dr["Major"].ToString());
                    deptDt.Columns.Add(dr["Major"].ToString() + "Pass");
                    deptDt.Columns.Add(dr["Major"].ToString() + "All");
                }
            }
            deptDt.Columns.Add("PassRate"); //面试通过率

            var result = new List<Dictionary<string, object>>();
            var json = JsonHelper.ToJson(deptDt);
            result = JsonHelper.ToList(json);

            foreach (var item in result)
            {
                double deptPass = 0;
                double deptAll = 0;

                foreach (DataRow dr in majorDt.Rows)
                {
                    var major = dr["Major"].ToString();

                    var passNumber = sumDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "' and Major='" + major + "' " + @" and IsPass = '通过' ");
                    var allNumber = sumDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "' and Major='" + major + "' ");

                    var pass = passNumber == DBNull.Value ? 0 : passNumber;
                    var all = allNumber == DBNull.Value ? 0 : allNumber;

                    deptPass += Convert.ToInt32(pass);
                    deptAll += Convert.ToInt32(all);

                    item.SetValue(major + "Pass", pass);
                    item.SetValue(major + "All", all);
                }

                double rate = 0;

                if (deptAll != 0)
                {
                    rate = (deptPass / deptAll) * 100;
                }

                item.SetValue("PassRate", rate.ToString("0.00"));
            }

            var dataGrid = new GridData(result);
            dataGrid.total = qb.TotolCount;
            return Json(dataGrid);

        }

        #endregion

        #region 校园招聘入职率
        /// <summary>
        /// 校园招聘入职率
        /// </summary>
        /// <returns></returns>
        public ActionResult Recruitment_SchoolInterviewEntryRate()
        {

            var dt = GetMajors();

            ViewBag.MyColumns = dt.Rows;

            return View();
        }

        public JsonResult GetSchoolInterviewEntryRateList(QueryBuilder qb)
        {
            var year = DateTime.Now.Year.ToString();

            string yearQuery = this.Request["year"] ?? "";

            if (!string.IsNullOrEmpty(yearQuery))
            {
                year = yearQuery;
            }

            var linkSql = " where A.Year = '" + year + "'";

            //部门：提过需求的所有的部门
            var deptSql = @"SELECT Dept, DeptName FROM S_E_Recruitment_DeptRequirementReport A " + linkSql + " GROUP BY Dept,DeptName";
            var deptDt = HRSQLDB.ExecuteDataTable(deptSql);
            //专业：所有的枚举
            var majorDt = GetMajors();
            
            var sumSql = @"SELECT count(1) as Num, Major, Dept, IsPass FROM S_E_Recruitment_Schoolrecruitmentinterview A " + linkSql + " GROUP BY Major, Dept,IsPass";
            var sumDt = HRSQLDB.ExecuteDataTable(sumSql);

            var entrySql = @"select count(1) as Num, A.Dept,A.Major from  S_E_Recruitment_Schoolrecruitmentinterview A " + linkSql + 
@"and (select count(1) as num from T_Employee B where B.Interview = A.ID) != 0  GROUP BY A.dept, A.major";
            var entryDt = HRSQLDB.ExecuteDataTable(entrySql);


            foreach (DataRow dr in majorDt.Rows)
            {
                if (!deptDt.Columns.Contains(dr["Major"].ToString()))
                {
                    deptDt.Columns.Add(dr["Major"].ToString());
                    deptDt.Columns.Add(dr["Major"].ToString() + "Pass");
                    deptDt.Columns.Add(dr["Major"].ToString() + "Entry");
                }
            }

            deptDt.Columns.Add("EntryRate"); //入职率

            var result = new List<Dictionary<string, object>>();
            var json = JsonHelper.ToJson(deptDt);
            result = JsonHelper.ToList(json);

            foreach (var item in result)
            {
                foreach (DataRow dr in majorDt.Rows)
                {
                    var major = dr["Major"].ToString();

                    var passNumber = sumDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "' and Major='" + major + "' " + @" and IsPass = '通过' ");
                    var entryNumber = entryDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "' and Major='" + major + "' ");

                    var pass = passNumber == DBNull.Value ? 0 : passNumber;
                    var entry = entryNumber == DBNull.Value ? 0 : entryNumber;

                    item.SetValue(major + "Pass", pass);
                    item.SetValue(major + "Entry", entry);
                }

                var deptPassNumber = sumDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + @"' and IsPass = '通过' ");
                var entryPassNumber = entryDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "' ");

                var deptPass = deptPassNumber == DBNull.Value ? 0 : deptPassNumber;
                var entryPass = entryPassNumber == DBNull.Value ? 0 : entryPassNumber;

                double entryRate = Convert.ToInt32(deptPass) != 0 ? entryRate = Convert.ToDouble(entryPass) / Convert.ToDouble(deptPass) * 100 : 0; //入职率=入职人数/面试审批通过人数

                item.SetValue("EntryRate", entryRate.ToString("0.00"));
            }


            var dataGrid = new GridData(result);
            dataGrid.total = qb.TotolCount;
            return Json(dataGrid);
        } 


        #endregion

        #region 校园招聘完成率
        /// <summary>
        /// 校园招聘完成率
        /// </summary>
        /// <returns></returns>
        public ActionResult Recruitment_SchoolInterviewFinishRate()
        {
            var dt = GetMajors();

            ViewBag.MyColumns = dt.Rows;

            return View();
        }

        public JsonResult GetSchoolInterviewFinishRateList(QueryBuilder qb)
        {
            var year = DateTime.Now.Year.ToString();

            string yearQuery = this.Request["year"] ?? "";

            if (!string.IsNullOrEmpty(yearQuery))
            {
                year = yearQuery;
            }

            var linkSql = " where A.Year = '" + year + "'";


            //部门：提过需求的所有的部门
            var deptSql = @"SELECT Dept, DeptName FROM S_E_Recruitment_DeptRequirementReport A " + linkSql + " GROUP BY Dept,DeptName";
            var deptDt = HRSQLDB.ExecuteDataTable(deptSql);
            //专业：所有的枚举
            var majorDt = GetMajors();
       
            var entrySql = @"select count(1) as Num, A.Dept,A.Major from  S_E_Recruitment_Schoolrecruitmentinterview A " + linkSql +
@"and (select count(1) as num from T_Employee B where B.Interview = A.ID) != 0  GROUP BY A.dept, A.major";
            var entryDt = HRSQLDB.ExecuteDataTable(entrySql);

            var needSql = @"SELECT sum(Number) as Num, major, dept FROM S_E_Recruitment_DeptRequirementReport A " + linkSql + " GROUP BY major, dept";
            var needDt = HRSQLDB.ExecuteDataTable(needSql);

            foreach (DataRow dr in majorDt.Rows)
            {
                if (!deptDt.Columns.Contains(dr["Major"].ToString()))
                {
                    deptDt.Columns.Add(dr["Major"].ToString());
                    deptDt.Columns.Add(dr["Major"].ToString() + "Entry");
                    deptDt.Columns.Add(dr["Major"].ToString() + "Need");
                }
            }
            deptDt.Columns.Add("FinishRate"); //招聘完成率

            var result = new List<Dictionary<string, object>>();
            var json = JsonHelper.ToJson(deptDt);
            result = JsonHelper.ToList(json);

            foreach (var item in result)
            {
                foreach (DataRow dr in majorDt.Rows)
                {
                    var major = dr["Major"].ToString();

                    var entryNumber = entryDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "' and Major='" + major + "' ");
                    var needNumber = needDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "' and Major='" + major + "' ");

                    item.SetValue(major + "Entry", entryNumber == DBNull.Value ? 0 : entryNumber);
                    item.SetValue(major + "Need", needNumber == DBNull.Value ? 0 : needNumber);
                }

                var entryDeptNumber = entryDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "' ");
                var needDeptNumber = needDt.Compute("sum(Num)", " Dept='" + item.GetValue("Dept") + "' ");

                var entryDept = entryDeptNumber == DBNull.Value ? 0 : entryDeptNumber;
                var needDept = needDeptNumber == DBNull.Value ? 0 : needDeptNumber;

                double finishRate = Convert.ToInt32(needDept) != 0 ? Convert.ToDouble(entryDept) / Convert.ToDouble(needDept) * 100 : 0; //招聘完成率=入职人数/招聘计划人数

                item.SetValue("FinishRate", finishRate.ToString("0.00"));
            }

            var dataGrid = new GridData(result);
            dataGrid.total = qb.TotolCount;
            return Json(dataGrid);
        } 

        #endregion

  
    }
}
