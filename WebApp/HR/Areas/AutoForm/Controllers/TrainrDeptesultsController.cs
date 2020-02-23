using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
/**
 * 部门培训结果
 * **/
namespace HR.Areas.AutoForm.Controllers
{
    public class TrainrDeptesultsController : HRFormContorllor<T_Trainmanagement_Depttrainresults>
    {
        //
        // GET: /AutoForm/TrainrDeptesults/

        public ActionResult Index()
        {
            return View();
        }

        protected override void OnFlowEnd(T_Trainmanagement_Depttrainresults entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {

            if (entity == null)
            {
                return;
            }

            //更新人力下达给部门培训任务的信息
            var task = BusinessEntities.Set<S_Train_DeptTask>().FirstOrDefault(p => p.Dept == entity.Udertakedept && p.Trainyears == entity.Trainyears);
            if (task == null)
                return;
            if (entity.Planornot == "True")
                task.SubmitInPlanHours += 1;
            else
                task.SubmitOutPlanCount += 1;
            if (string.IsNullOrEmpty(entity.Trainproject))
                return;

            var deptPlan = BusinessEntities.Set<S_Train_DeptPlan>().Find(entity.Trainproject);
            if (deptPlan == null) {
                //生成计划外培训数据
                deptPlan = new S_Train_DeptPlan();
                deptPlan.ID = Formula.FormulaHelper.CreateGuid();
                deptPlan.Trainyears = entity.Trainyears;
                deptPlan.Trainperson = entity.Fillname;
                deptPlan.TrainpersonName = entity.FillnameName;
                deptPlan.Trainproject = entity.TrainprojectName;
                deptPlan.InOrOut = "Out";//计划外
                deptPlan.Putdept = entity.Udertakedept;
                deptPlan.PutdeptName = entity.UdertakedeptName;

                BusinessEntities.Set<S_Train_DeptPlan>().Add(deptPlan);

                entity.Trainproject = deptPlan.ID;
            
            }
               
            //累计学时
            if (deptPlan.UseWorkTime == null)
            {
                deptPlan.UseWorkTime = 0;
            }

            //进度=100 标记为完成
            if (entity.Progress == 100)
            {
                
            deptPlan.IsComplete = "True";
            }
            deptPlan.UseWorkTime += entity.Trainhours;
            BusinessEntities.SaveChanges();

        }
    }
}
