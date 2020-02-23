using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;

namespace HR.Logic.Domain
{
    public partial class T_Employee
    {

        public void SynchPostTemplate()
        {
            var hrEntities = FormulaHelper.GetEntities<HREntities>();
            //根据岗位信息获取最新的模板数据
            var list = hrEntities.Set<S_S_PostLevelTemplate_PostList>()
                .Where(c => c.PostLevelCode == this.PostLevel && c.PostCode == this.Post)
                .OrderByDescending(c => c.BelongYear)
                .ThenByDescending(c => c.BelongMonth).ToList();
            if (list != null && list.Count > 0)
                this.PostTemplateID = list[0].S_S_PostLevelTemplateID;
        }

        /// <summary>
        /// 根据职务信息同步当前部门及兼职部门
        /// </summary>
        public void UpdateDeptByJob()
        {
            var hrEntities = FormulaHelper.GetEntities<HREntities>();
            var jobs = hrEntities.Set<T_EmployeeJob>().Where(a => a.EmployeeID == this.ID).ToList();
            var mainJob = jobs.FirstOrDefault(a => a.IsMain == "1");
            if (mainJob != null)
            {
                this.DeptID = mainJob.DeptID;
                this.DeptIDName = mainJob.DeptIDName;
                this.DeptName = this.DeptIDName;
            }
            var pJobs = jobs.Where(a => a.IsMain != "1").ToList();
            if (pJobs.Count > 0)
            {
                this.ParttimeDeptID = string.Join(",", pJobs.Select(a => a.DeptID).Distinct().ToArray());
                this.ParttimeDeptIDName = string.Join(",", pJobs.Select(a => a.DeptIDName).Distinct().ToArray());
                this.ParttimeDeptName = this.ParttimeDeptIDName;
            }
        }
    }
}
