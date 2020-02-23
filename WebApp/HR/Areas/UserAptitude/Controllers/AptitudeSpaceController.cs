using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula;
using Config;
using Config.Logic;
using System.Data;
using Project.Logic;
using MvcAdapter;

namespace HR.Areas.UserAptitude.Controllers
{
    public class AptitudeSpaceController : HRController<S_P_Performance>
    {
        public ActionResult TabIndex()
        {
            var tab = new Tab();
            var positionalTitlesCategory = CategoryFactory.GetCategory("HR.PositionalTitle", "职称", "PositionalTitles");
            positionalTitlesCategory.SetDefaultItem();
            positionalTitlesCategory.Multi = true;
            tab.Categories.Add(positionalTitlesCategory);

            var jobCategory = CategoryFactory.GetCategory("HR.Post", "职务", "JobNames");
            jobCategory.SetDefaultItem();
            jobCategory.Multi = true;
            tab.Categories.Add(jobCategory);

            var typeCategory = CategoryFactory.GetCategory("System.ProjectClass", "承担项目类型", "ProjectClass");
            typeCategory.SetDefaultItem();
            typeCategory.Multi = true;
            tab.Categories.Add(typeCategory);

            var phaseCategory = CategoryFactory.GetCategory("Project.Phase", "承担项目阶段", "PhaseName");
            phaseCategory.SetDefaultItem();
            phaseCategory.Multi = true;
            tab.Categories.Add(phaseCategory);

            var scaleCategory = CategoryFactory.GetCategory("System.AptitudeLevel", "承担项目规模", "ProjectLevel");
            scaleCategory.SetDefaultItem();
            scaleCategory.Multi = true;
            tab.Categories.Add(scaleCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            return View();
        }

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var whereStr = "";
            var projectClass = this.GetQueryString("ProjectClass");
            var phaseName = this.GetQueryString("PhaseName");
            var projectLevel = this.GetQueryString("ProjectLevel");
            if (!string.IsNullOrEmpty(projectClass))
                whereStr += " and ProjectClass='" + projectClass + "' ";
            if (!string.IsNullOrEmpty(phaseName))
                whereStr += " and PhaseName='" + phaseName + "' ";
            if (!string.IsNullOrEmpty(projectLevel))
                whereStr += " and ProjectLevel='" + projectLevel + "' ";


            var sql = string.Format(@"select distinct emp.ID,emp.Name,emp.Code,emp.PositionalTitles,emp.Educational,emp.EngageMajor,emp.UserID
	,QualificationName=stuff((select distinct ','+QualificationName 
						from T_EmployeeQualification 
						where EmployeeID=emp.ID 
						for xml path('')) ,1,1,'') 
	,JobNames=stuff((select distinct ','+JobName 
			from T_EmployeeJob 
			where EmployeeID=emp.ID
			and IsDeleted=0
			for xml path('')) ,1,1,'')
from T_Employee emp
left join S_P_Performance_JoinPeople pfj
	on emp.UserID=pfj.MajorPrinciple
left join S_P_Performance pf
	on pf.ID=pfj.S_P_PerformanceID
where emp.IsDeleted=0 {0}", whereStr);

            var db = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(db);
        }

        public JsonResult GetPerformanceList(MvcAdapter.QueryBuilder qb)
        {
            var userID = this.GetQueryString("UserID");
            var sql = string.Format(@"select * from (
select  sp.*,ProjectRole=stuff((select distinct ','+ProjectRole
							FROM (select ProjectInfoID,ChargeUser,MajorPrinciple,Designer,Collactor,Auditor,Approver 
								from S_P_Performance pf left join S_P_Performance_JoinPeople pfj 
								on pf.ID=pfj.S_P_PerformanceID) tmp
							unpivot(UserIDs FOR ProjectRole in(ChargeUser,MajorPrinciple,Designer,Collactor,Auditor,Approver))AS up
							where UserIDs like '%{0}%'
							and ProjectInfoID=sp.ProjectInfoID
						for xml path('')) ,1,1,'') 
from S_P_Performance sp) tmp
where ProjectRole!='' and ProjectRole is not null", userID);

            var dt = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(dt);
        }

    }
}
