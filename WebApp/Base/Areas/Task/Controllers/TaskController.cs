using Base.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using Formula;
using Formula.Helper;
using System.Text;

namespace Base.Areas.Task.Controllers
{
    public class TaskController : BaseController<S_T_Task>
    {
        public override ActionResult Edit()
        {
            //第一级任务
            var id = Request["ID"];
            List<S_T_TaskExec> list = null;
            if (Request["IsExecutor"] == "True")//如果是任务执行页面，这只显示本任务，不显示平级任务
                list = entities.Set<S_T_TaskExec>().Where(c => c.TaskID == id && c.ExecUserID == FormulaHelper.UserID).ToList();
            else
                list = entities.Set<S_T_TaskExec>().Where(c => c.TaskID == id).ToList();

            //第二级任务
            var execIds = list.Select(c => c.ID).ToArray();
            var taskList = entities.Set<S_T_Task>().Where(c => execIds.Contains(c.TaskExecID));
            var taskIds = taskList.Select(c => c.ID).ToArray();
            var subList = entities.Set<S_T_TaskExec>().Where(c => taskIds.Contains(c.TaskID)).ToList();

            //合并
            list.AddRange(subList);

            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.AppendLine();
                sb.AppendFormat("<div title=\"{0}\" url='/Base/Task/TaskExec/Exec?execId={1}' refreshOnClick='true'></div>", item.ExecUserName, item.ID);
            }

            ViewBag.html = sb.ToString();
            return base.Edit();
        }

        public JsonResult GetMyTaskList(QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format(@"
select ID,Name,DeadLine,Urgency
,case when CompleteTime is null then '进行中' else '已完成' end TaskStatus
 from S_T_Task
where CreateUserID ='{0}'", FormulaHelper.UserID);
            var data = sqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetReceiveTaskList(QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format(@"
select S_T_Task.ID,S_T_Task.Name,S_T_Task.DeadLine,S_T_Task.Urgency
,S_T_Task.CompleteTime
,S_T_TaskExec.ExecTime
,case when S_T_TaskExec.ExecTime is null then '进行中' else '已完成' end TaskStatus
from S_T_Task
join S_T_TaskExec on S_T_TaskExec.TaskID=S_T_Task.ID and S_T_TaskExec.ExecUserID='{0}'
", FormulaHelper.UserID);
            var data = sqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public override JsonResult Delete()
        {
            //删除子任务
            var ids = Request["ListIDs"].Split(',');
            var execIds = entities.Set<S_T_TaskExec>().Where(c => ids.Contains(c.TaskID)).Select(c => c.ID).ToArray();
            entities.Set<S_T_Task>().Delete(c => execIds.Contains(c.TaskExecID));

            //删除任务
            entities.Set<S_T_Task>().Delete(c => ids.Contains(c.ID));

            entities.SaveChanges();
            return Json("");
        }

        public override JsonResult Save()
        {
            var entity = UpdateEntity<S_T_Task>();

            var arr = entity.ExecUserIDs.Split(',');
            var arrName = entity.ExecUserNames.Split(',');

            //先删除
            entities.Set<S_T_TaskExec>().Delete(c => c.TaskID == entity.ID && arr.Contains(c.ExecUserID) == false);
            //再增加
            for (var i = 0; i < arr.Length; i++)
            {
                var userId = arr[i];
                if (entities.Set<S_T_TaskExec>().SingleOrDefault(c => c.TaskID == entity.ID && c.ExecUserID == userId) == null)
                {
                    S_T_TaskExec exec = new S_T_TaskExec();
                    exec.ID = FormulaHelper.CreateGuid();
                    exec.TaskID = entity.ID;
                    exec.ExecUserID = arr[i];
                    exec.ExecUserName = arrName[i];
                    exec.CreateTime = DateTime.Now;
                    entity.S_T_TaskExec.Add(exec);
                }
            }

            entities.SaveChanges();

            return Json("");
        }

        public JsonResult ConfirmTask()
        {
            var entity = UpdateEntity<S_T_Task>();
            entity.CompleteTime = DateTime.Now; //完成整个任务


            var taskExecList = entities.Set<S_T_TaskExec>().Where(c => c.TaskID == entity.ID).ToArray();

            foreach (var exec in taskExecList)
            {
                exec.ExecTime = DateTime.Now;//执行当前任务


                //完成子任务
                var tasks = entities.Set<S_T_Task>().Where(c => c.TaskExecID == exec.ID).ToArray();
                foreach (var task in tasks)
                {
                    if (task.CompleteTime == null)
                    {
                        task.CompleteTime = DateTime.Now;
                    }
                }

                //完成子任务明细
                var taskIDs = tasks.Select(c => c.ID).ToArray();
                var taskExecs = entities.Set<S_T_TaskExec>().Where(c => taskIDs.Contains(c.TaskID)).ToArray();
                foreach (var taskExec in taskExecs)
                {
                    taskExec.ExecTime = DateTime.Now;
                }
            }

            entities.SaveChanges();


            return Json("");
        }

    }
}
