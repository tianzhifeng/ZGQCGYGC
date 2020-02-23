using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config.Logic;
using Config;
/**
 * 培训情况统计报表
 * **/
namespace HR.Areas.Report.Controllers
{
    public class TrainReportController : HRController
    {
        //
        // GET: /Report/TrainReport/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetTrainPerformList(QueryBuilder qb)
        {
            var Years = GetQueryString("Years");
            if (string.IsNullOrEmpty(Years))
                Years = DateTime.Now.Year.ToString();

            var sql = string.Format(@" 
select  ''ID,isnull(a.Trainyear,b.Trainyear) Trainyear, isnull(a.Undertakedept,b.Putdept)  DeptID,
		 isnull(a.UndertakedeptName,b.PutdeptName)  DeptName, 
		 (isnull(a.CompleteProjectCount,0)+ isnull(b.CompleteProjectCount_Dept,0) + isnull(a.outPlanProjectCount,0)+ isnull(b.outPlanProjectCount_Dept,0))/ (isnull(a.PlanProjectCount,0)+isnull(b.PlanProjectCount_Dept,0)) TotalCountScale,
		 (isnull(a.Accumulathours,0)+ isnull(b.Accumulathours_Dept,0) + isnull(a.outAccumulathours,0)+ isnull(b.outAccumulathours_Dept,0))/ (isnull(a.PlanProjectHours,0)+isnull(b.PlanProjectHours_Dept,0)) TotalHoursScal,
 --公司级
  --项目数
  a.PlanProjectCount,a.CompleteProjectCount,(isnull(a.CompleteProjectCount,0)/isnull(a.PlanProjectCount,1) )CompleteScale,  a.outPlanProjectCount,(isnull(a.outPlanProjectCount,0)+ isnull(a.CompleteProjectCount,0))/ isnull(a.PlanProjectCount,1) TotalCompleteScale,
  --工时
  a.PlanProjectHours,a.Accumulathours,isnull(a.Accumulathours,0)/ isnull(a.PlanProjectHours,1) CompleteHoursScale , a.outAccumulathours,(isnull(a.Accumulathours,0)+ isnull(a.outAccumulathours,0))/ isnull(a.PlanProjectHours,1) TotalCompleteHoursScale,
 --部门级
  --项目数
  b.PlanProjectCount_Dept,b.CompleteProjectCount_Dept,(isnull(b.CompleteProjectCount_Dept,0)/isnull(b.PlanProjectCount_Dept,1)) CompleteScale_Dept  , b.outPlanProjectCount_Dept, (isnull(b.CompleteProjectCount_Dept,0)+isnull(b.outPlanProjectCount_Dept,0))/ isnull(b.PlanProjectCount_Dept,1) TotalCompleteScale_Dept,

  --工时数
  b.PlanProjectHours_Dept,b.Accumulathours_Dept, isnull(b.Accumulathours_Dept,0)/isnull(b.PlanProjectHours_Dept,1) CompleteHoursScale_Dept,b.outAccumulathours_Dept,(isnull(b.Accumulathours_Dept,0)+ isnull(b.outAccumulathours_Dept,0))/isnull(b.PlanProjectHours_Dept,1) TotalCompleteHoursScale_Dept
   from (
--公司级
select Trainyear,Undertakedept,UndertakedeptName,
	sum(case when isnull(InOrOut,'In') ='In' then 1 else 0 end ) PlanProjectCount,
    sum(case when  IsComplete ='True' and isnull(InOrOut,'In') ='In'   then 1 else 0 end ) CompleteProjectCount,
	sum(case when isnull(InOrOut,'In') ='Out' then 1 else 0 end ) outPlanProjectCount,
	sum(case when isnull(InOrOut,'In') ='In' then Totalclasshours else 0 end ) PlanProjectHours,
	sum(case when isnull(InOrOut,'In') ='In' then Accumulathours  else 0 end    ) Accumulathours ,
	sum(case when isnull(InOrOut,'In') ='Out' then Accumulathours  else 0 end ) outAccumulathours
 from [dbo].[S_Train_CompPlan] 
where Trainyear= '{0}'
group by  Trainyear,Undertakedept,UndertakedeptName
) a
full  join 
(
--部门级
select Trainyears Trainyear,Putdept,PutdeptName, 
sum(case when isnull(InOrOut,'In') ='In' then 1 else 0 end ) PlanProjectCount_Dept,
	sum(case when  IsComplete ='True' and isnull(InOrOut,'In') ='In'  then 1 else 0 end ) CompleteProjectCount_Dept,
	sum(case when isnull(InOrOut,'In') ='Out' then 1 else 0 end ) outPlanProjectCount_Dept,
	sum(case when isnull(InOrOut,'In') ='In' then [Time] else 0 end ) PlanProjectHours_Dept,
	sum(case when isnull(InOrOut,'In') ='In' then UseWorkTime  else 0 end  ) Accumulathours_Dept ,
	sum(case when isnull(InOrOut,'In') ='Out' then UseWorkTime  else 0 end ) outAccumulathours_Dept
  from [dbo].[S_Train_DeptPlan]  
 where  Trainyears= '{0}'
group by Trainyears,Putdept,PutdeptName 
) b on a.Undertakedept = b.Putdept 
 ", Years);

            var dt = SqlHelper.ExecuteGridData(sql, qb);

            return Json(dt);
        }

        public JsonResult GetTrainCoverageAreaList(QueryBuilder qb)
        {
            var Years = GetQueryString("Years");
            if (string.IsNullOrEmpty(Years))
                Years = DateTime.Now.Year.ToString();

            var sql = string.Format(@" 
select  ''ID,isnull(a.Trainyear,b.Trainyear) Trainyear, isnull(a.Undertakedept,b.Putdept)  DeptID,
		 isnull(a.UndertakedeptName,b.PutdeptName)  DeptName, c.UserCount ,
          (a.Accumulathours+a.outAccumulathours+b.Accumulathours_Dept+b.outAccumulathours_Dept) Hours,

 (a.Accumulathours+a.outAccumulathours+b.Accumulathours_Dept+b.outAccumulathours_Dept)/ c.UserCount HoursScale,
--公司级
  --工时
a.Accumulathours+a.outAccumulathours totalHours,
   (a.Accumulathours+a.outAccumulathours)/c.UserCount UserScale,
  --部门级
  --工时数
b.Accumulathours_Dept+b.outAccumulathours_Dept totalHours_Dept,
   (b.Accumulathours_Dept+b.outAccumulathours_Dept)/c.UserCount UserScale_Dept
   from (
--公司级
select Trainyear,Undertakedept,UndertakedeptName,
	sum(case when isnull(InOrOut,'In') ='In' then Accumulathours  else 0 end    ) Accumulathours ,
	sum(case when isnull(InOrOut,'In') ='Out' then Accumulathours  else 0 end ) outAccumulathours
 from [dbo].[S_Train_CompPlan] 
where Trainyear= '{0}'
group by  Trainyear,Undertakedept,UndertakedeptName
) a
full  join 
(
--部门级
select Trainyears Trainyear,Putdept,PutdeptName, 
	sum(case when isnull(InOrOut,'In') ='In' then UseWorkTime  else 0 end  ) Accumulathours_Dept ,
	sum(case when isnull(InOrOut,'In') ='Out' then UseWorkTime  else 0 end ) outAccumulathours_Dept
  from [dbo].[S_Train_DeptPlan]  
 where  Trainyears= '{0}'
group by Trainyears,Putdept,PutdeptName 
) b on a.Undertakedept = b.Putdept
left  join (
select DeptID,DeptName,count(1)UserCount  from T_Employee
where isnull(DeptID,'') <> ''  
 group by DeptID,DeptName  
)c  on c.DeptID = a.Trainyear or c.DeptID = b.Putdept

 ", Years);

            var dt = SqlHelper.ExecuteGridData(sql, qb);


            return Json(dt);
        }
    }
}
