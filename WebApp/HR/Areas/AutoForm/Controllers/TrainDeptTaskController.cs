using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula;
/**
 * 部门培训通知
 * ***/
namespace HR.Areas.AutoForm.Controllers
{
    public class TrainDeptTaskController : HRFormContorllor<S_Train_DeptTask>
    {
        //
        // GET: /AutoForm/TrainDeptTask/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 计划收集情况跟进发送催办
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public JsonResult SendMsg(string IDs)
        {
            if (string.IsNullOrEmpty(IDs))
                throw new Formula.Exceptions.BusinessException("必须选中一行");
            var idArrt = IDs.Split(',');
            var list = BusinessEntities.Set<S_Train_DeptTask>().Where(p => idArrt.Contains(IDs));
            var msgTmp = @"截止{0}，{1}  {2}的培训计划仍未提交，部门培训负责人为: {3}，请催办部门培训负责人提交部门培训计划。";
            var title = "培训计划催办提醒";

            var msgService = FormulaHelper.GetService<IMessageService>();

            foreach (var item in list)
            {
                var sendMsg = string.Format(msgTmp, item.Deadline.Value.ToString("yyyy-MM-dd"), item.DeptName, item.DeptName, item.Trainyears, item.DepttrainleaderName);

                //发送消息给部门领导或者部门主管领导
                if (!string.IsNullOrEmpty(item.Depthead))
                    msgService.SendMsg(title, sendMsg, "", "", item.Depthead, item.DeptheadName);
                else if (!string.IsNullOrEmpty(item.Superiorleader))
                    msgService.SendMsg(title, sendMsg, "", "", item.Superiorleader, item.Superiorleader);
            }
            return Json("");
        }

        /// <summary>
        /// 结果填报情况跟进发送催办
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public JsonResult SendMsgResult(string IDs)
        {
            if (string.IsNullOrEmpty(IDs))
                throw new Formula.Exceptions.BusinessException("必须选中一行");
            var idArrt = IDs.Split(',');
            var list = BusinessEntities.Set<S_Train_DeptTask>().Where(p => idArrt.Contains(IDs));
            var msgTmp = @"截止{0}，{1}{2}的培训结果仍未全部提交，部门培训负责人为{3}，请催办部门培训负责人提交部门培训结果。";
            var title = "培训计划催办提醒";

            var msgService = FormulaHelper.GetService<IMessageService>();

            foreach (var item in list)
            {
                var sendMsg = string.Format(msgTmp, item.Deadline.Value.ToString("yyyy-MM-dd"), item.DeptName, item.DeptName, item.Trainyears, item.DepttrainleaderName);

                //发送消息给部门领导或者部门主管领导
                if (!string.IsNullOrEmpty(item.Depthead))
                    msgService.SendMsg(title, sendMsg, "", "", item.Depthead, item.DeptheadName);
                else if (!string.IsNullOrEmpty(item.Superiorleader))
                    msgService.SendMsg(title, sendMsg, "", "", item.Superiorleader, item.Superiorleader);
            }
            return Json("");
        }

        public JsonResult GetDeptTaskInfoByDeptID(string DeptID,string Year)
        {
            var entity = BusinessEntities.Set<S_Train_DeptTask>().Where(p => p.Dept == DeptID && p.Trainyears == Year).OrderByDescending(o => o.Deadline).FirstOrDefault();
          
            return Json(entity);
        }
    }
}
