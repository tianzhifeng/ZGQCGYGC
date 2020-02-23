using Formula;
using HR.Logic.Domain;
using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class ProjectController : OfficeAutoFormContorllor<T_Technology_Projectapplicationform>
    {
        //
        // GET: /AutoForm/Project/

        public ActionResult Tab()
        {
            var strID = GetQueryString("ProjectID");
            var strType = GetQueryString("FuncType");
            ViewBag.TabHtml = new StringBuilder(@"
<div title='课题立项' url='/MvcConfig/UI/Form/PageView?TmplCode=Technology_Projectapplicationform&ID="+ strID + @"&FuncType="+ strType + @"' showCloseButton='true'>课题立项</div>
<div title='人员安排' url='/MvcConfig/UI/List/PageView?TmplCode=Technology_PersonnelPlacement&ProjectID=" + strID + @"' showCloseButton='true'>人员安排</div>
<div title='课题进度' url='/MvcConfig/UI/List/PageView?TmplCode=Technology_ProjectProgress&ProjectID=" + strID + @"' showCloseButton='true'>课题进度</div>
<div title='延期申请' url='/MvcConfig/UI/List/PageView?TmplCode=Technology_ProjectDelayApply&ProjectID=" + strID + @"' showCloseButton='true'>延期申请</div>
<div title='专家评审' url='/MvcConfig/UI/List/PageView?TmplCode=Technology_ExpertReview&ProjectID=" + strID + @"' showCloseButton='true'>专家评审</div>
<div title='费用计划审批' url='/MvcConfig/UI/List/PageView?TmplCode=Technology_CostPlan&ProjectID=" + strID + @"' showCloseButton='true'>费用计划审批</div>
<div title='合同审批' url='/MvcConfig/UI/List/PageView?TmplCode=Technology_ContractApproval&ProjectID=" + strID +  @"' showCloseButton='true'>合同审批</div>
<div title='付款申请' url='/MvcConfig/UI/List/PageView?TmplCode=Technology_PaymentApply&ProjectID=" + strID +  @"' showCloseButton='true'>付款申请</div>
<div title='成果提交' url='/MvcConfig/UI/List/PageView?TmplCode=Technology_ResultCommit&ProjectID=" + strID + @"' showCloseButton='true'>成果提交</div>");
            return View();
        }

        public JsonResult GetUserInfoById()
        {
            var id = GetQueryString("ID");

#if DEBUG
            var hrEntity = FormulaHelper.GetEntities<HREntities>();
            var userInfo = hrEntity.Set<T_Employee>().FirstOrDefault(p => p.UserID == id);
            if (userInfo == null||string.IsNullOrWhiteSpace(userInfo.EngageMajor))
                return Json(new { EngageMajor = "测试专业" });
            return Json(new { EngageMajor = userInfo.EngageMajor });

#else

            var hrEntity =   FormulaHelper.GetEntities<HREntities>();
          var userInfo =   hrEntity.Set<T_Employee>().FirstOrDefault(p => p.UserID == id);
            if (userInfo == null)
                return Json("");
            return Json(new { EngageMajor = userInfo.EngageMajor });

#endif

        }

        //获取课程进度版本号
        public JsonResult GetProcessCount()
        {
            var id = GetQueryString("ProjectID");

           var iCount=  BusinessEntities.Set<T_Technology_ProjectProgress>().Count(x => x.ProjectID == id);
            return Json(new { Count = iCount });
        }

        //获取专家评审中的核准金额
        public JsonResult GetProjectFunds()
        {
            var id = GetQueryString("ProjectID");
          var expert=   BusinessEntities.Set<T_Technology_ExpertReview>().Where(x => x.ProjectID == id).OrderByDescending(x => x.CreateDate).FirstOrDefault();
            if (expert == null)
                return Json(new { Funds = "0" });

            return Json(new { Funds = expert.ProjectFunds });
        }

        /// <summary>
        /// 获取技术委员会主任信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTechnicalcommittee()
        {
            return Json(BusinessEntities.Set<T_Technicalcommittee_Technicalperson>().Where(x => x.Position == "主任").Select(x => new { DeptID=x.ID, Dept = x.Dept, Name = x.NameName,ID=x.Name }));

        }

        /// <summary>
        /// 获取付款计划
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPaymentPlan()
        {
            var id = GetQueryString("ContractID");
            return Json(BusinessEntities.Set<T_Technology_ContractApproval_PaymentPlan>().Where(x => x.T_Technology_ContractApprovalID == id).ToList());
        }

        /// <summary>
        /// 根据用户ID获取标准化人员信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStandardPeopleById()
        {
            var id = GetQueryString("ID");

            var standard = BusinessEntities.Set<T_Standardengineer_Standardpeoplemanage>().FirstOrDefault(x => x.Standardpeoplename == id);
            if (standard == null)
                return Json("");

            return Json(standard);

        }
        /// <summary>
        /// 获取标准化管理版本号
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStandardVersion()
        {
            return Json(new { Count= BusinessEntities.Set<T_Standardengineer_Standardpeoplemanage>().Count() });
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUserDetailById()
        {
            var id = GetQueryString("ID");
            var hrEntity = FormulaHelper.GetEntities<HREntities>();
            var userInfo = hrEntity.Set<T_Employee>().FirstOrDefault(p => p.UserID == id);
            if (userInfo == null)
                return Json("");

            //该字段暂存年龄
            if (userInfo.Birthday != null)
                userInfo.OldName = Math.Ceiling((DateTime.Now - Convert.ToDateTime(userInfo.Birthday)).TotalDays / 365).ToString();
            else
                userInfo.OldName = "0";
            return Json(userInfo);
        }

        //获取报奖项目总数
        public JsonResult GetAwardItemCount()
        {
            return Json(new { Count = BusinessEntities.Set<T_Win_AwardItem>().Count() });
        }

        /// <summary>
        /// 获取所有报奖项目不含当前专家所在部门
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAwardItems()
        {
            return Json(BusinessEntities.Set<T_Win_DeclareManager>().Where(x => x.FlowPhase == "End" && x.CreateUserID != CurrentUserInfo.UserID).ToList());

        }

        public ActionResult AwardTab()
        {
            var strID = GetQueryString("DeclareManagerID");
            ViewBag.TabHtml = new StringBuilder(@"
<div title='得分情况' url='/MvcConfig/UI/Form/PageView?TmplCode=Win_ProjectScore&DeclareID=" + strID + @"' showCloseButton='true'>得分情况</div>
<div title='专家评分' url='/MvcConfig/UI/List/PageView?TmplCode=Win_ExpertGrade&DeclareID=" + strID + @"' showCloseButton='true'>专家评分</div>");
            return View();
        }
        /// <summary>
        /// 计算所有项目得分并保存
        /// </summary>
        /// <returns></returns>
        public void CalcScore()
        {
            var lstGrade = BusinessEntities.Set<T_Win_ExpertGrade_AwardItems>().ToList();
            var lstID = lstGrade.GroupBy(x => x.T_Win_ExpertGradeID).Select(x=>x.Key).ToList();
            lstID.ForEach(id =>
            {
                //项目下的所有评分
                var lstItem = BusinessEntities.Set<T_Win_ExpertGrade_AwardItems>().Where(x => x.T_Win_ExpertGradeID == id).OrderByDescending(x => x.Grade).ToList();
                var scoreModel = BusinessEntities.Set<T_Win_ProjectScore>().FirstOrDefault(x => x.ProjectID == id);
                if (lstItem != null && lstItem.Count > 0)
                {
                    scoreModel.HightScore = lstItem[0].Grade;
                    scoreModel.LowScore= lstItem[lstItem.Count-1].Grade;
                    scoreModel.ExpertCount = lstItem.Count.ToString();

                    lstItem.Remove(lstItem[0]);
                    lstItem.Remove(lstItem[lstItem.Count - 1]);

                    scoreModel.AverageScore = ((lstItem.Sum(x => Convert.ToInt32(x.Grade)) / lstItem.Count) * 1.00).ToString();

                   
                    BusinessEntities.SaveChanges();
                }
                
           



            });
        }

        /// <summary>
        /// 获取合同下已支付金额
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPaymentAmount()
        {
            var number = GetQueryString("ContractNumber");
            var totalAmount= BusinessEntities.Set<T_Technology_PaymentApply>().Where(x => x.ContractNumber == number&&x.FlowPhase=="End").Sum(x => Convert.ToDecimal(x.PaymentAmount));
            return Json(new { amount = totalAmount });
        }
        /// <summary>
        /// 根据部门获取部门技术负责人
        /// </summary>
        /// <returns></returns>
        public ActionResult GetResponsibilityByDeptID()
        {
            var id = GetQueryString("DeptID");
            var baseEntity = FormulaHelper.GetEntities<BaseEntities>();
            var role = baseEntity.Set<S_A_Role>().FirstOrDefault(x => x.Name == "部门技术主管");

            if (role == null)
                return Json("");

            var userRelation = baseEntity.Set<S_A__OrgRoleUser>().FirstOrDefault(x => x.OrgID == id && x.RoleID == role.ID);

            if (userRelation == null || string.IsNullOrWhiteSpace(userRelation.UserID))
                return Json("");

            var user = baseEntity.Set<S_A_User>().FirstOrDefault(x => x.ID == userRelation.UserID);

            if (user == null)
                return Json("");

            return Json(new { ID = user.ID, Name = user.Name });

        }

    }
}
