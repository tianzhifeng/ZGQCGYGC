using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
/**
 * 公司级培训结果审批
 * **/
namespace HR.Areas.AutoForm.Controllers
{
    public class Train_CompanytrainresultsController : HRFormContorllor<T_Trainmanagement_Companytrainresults>
    {


        protected override void OnFlowEnd(T_Trainmanagement_Companytrainresults entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity == null)
            {
                return;
            }
           
            var compPlan = BusinessEntities.Set<S_Train_CompPlan>().Find(entity.Trainproject);
            if (compPlan == null)
            {
                //生成计划外培训数据
                compPlan = new S_Train_CompPlan();
                compPlan.ID = Formula.FormulaHelper.CreateGuid();
                compPlan.Trainyear = entity.Trainyears;
                compPlan.Fillname = entity.Fillname;
                compPlan.FillnameName = entity.FillnameName;
                compPlan.Trainproject = entity.TrainprojectName;
                compPlan.InOrOut = "Out";//计划外
                compPlan.Undertakedept = entity.Udertakedept;
                compPlan.UndertakedeptName = entity.UdertakedeptName;

                BusinessEntities.Set<S_Train_CompPlan>().Add(compPlan);

                entity.Trainproject = compPlan.ID;
            }

            //累计学时
            if (compPlan.Accumulathours == null)
            {
                compPlan.Accumulathours = 0;
            }
            compPlan.Accumulathours += entity.Trainhours;
            //进度=100 标记为完成
            if (entity.Progress == 100)
                compPlan.IsComplete = "True";

            BusinessEntities.SaveChanges();

        }
    }
}
