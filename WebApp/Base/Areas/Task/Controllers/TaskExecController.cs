using Base.Logic.Domain;
using Formula;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Base.Areas.Task.Controllers
{
    public class TaskExecController : BaseController<S_T_TaskExec>
    {
        public override JsonResult GetModel(string execId)
        {
            var exec = entities.Set<S_T_TaskExec>().SingleOrDefault(c => c.ID == execId);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("ID", exec.ID);
            dic.Add("TaskID", exec.TaskID);
            dic.Add("ExecUserID", exec.ExecUserID);
            dic.Add("ExecUserName", exec.ExecUserName);
            dic.Add("ExecTime", exec.ExecTime);
            dic.Add("ExecDescription", exec.ExecDescription);
            dic.Add("ExecAttachment", exec.ExecAttachment);
            dic.Add("ExecScore", exec.ExecScore);
            dic.Add("CreateTime", exec.CreateTime);
            //增加其它属性
            var task = exec.S_T_Task;
            dic.Add("TaskExecID", task.TaskExecID); //用于判断是否为第一级任务
            dic.Add("Name", task.Name);//用于界面显示
            dic.Add("DeadLine", task.DeadLine);       //用于界面显示

            var subItems = entities.Set<S_T_Task>().Where(c => c.TaskExecID == execId).Select(c => new
            {
                ID = c.ID,
                Name = c.Name,
                ExecUserIDs = c.ExecUserIDs,
                ExecUserNames = c.ExecUserNames,
                Urgency = c.Urgency,
                DeadLine = c.DeadLine,
                CompleteTime = c.CompleteTime,
                ExecTime = c.S_T_TaskExec.FirstOrDefault().ExecTime
            });


            dic.Add("subItems", JsonHelper.ToJson(subItems));//用于界面显示

            return Json(dic);
        }

        public override JsonResult Save()
        {
            var entity = UpdateEntity<S_T_TaskExec>();

            var formDic = JsonHelper.ToObject(Request["formData"]);
            var list = JsonHelper.ToList(formDic["subItems"].ToString());

            var user = FormulaHelper.GetUserInfo();

            //先删除已经不存在的
            var arrSubTaskIDs = list.Where(c => c.ContainsKey("ID")).Select(c => c["ID"].ToString()).ToArray();
            entities.Set<S_T_Task>().Delete(c => c.TaskExecID == entity.ID && arrSubTaskIDs.Contains(c.ID) == false);

            foreach (Dictionary<string, object> dic in list)
            {
                S_T_Task sub = null;
                if (dic.ContainsKey("ID") && dic["ID"].ToString() != "")
                {
                    string _id = dic["ID"].ToString();
                    sub = entities.Set<S_T_Task>().SingleOrDefault(c => c.ID == _id);
                }
                else
                {
                    sub = new S_T_Task();                   
                    UpdateEntity(sub, dic);
                    entities.Set<S_T_Task>().Add(sub);
                    sub.ID = FormulaHelper.CreateGuid();
                }

                sub.TaskExecID = entity.ID;                
                sub.CreateTime = DateTime.Now;
                sub.UpdateTime = DateTime.Now;
                //子任务不显示在我发起的任务中
                //sub.CreateUserID = user.UserID;
                //sub.CreateUserName = user.UserName;

                //发送任务
                //子任务只有一个执行人
                S_T_TaskExec exec = entities.Set<S_T_TaskExec>().SingleOrDefault(c => c.TaskID == sub.ID && c.ExecUserID == sub.ExecUserIDs);
                if (exec == null)
                {
                    exec = new S_T_TaskExec();
                    exec.ID = FormulaHelper.CreateGuid();
                    exec.TaskID = sub.ID;
                    exec.ExecUserID = sub.ExecUserIDs;
                    exec.ExecUserName = sub.ExecUserNames;
                    exec.CreateTime = DateTime.Now;
                    sub.S_T_TaskExec.Add(exec);
                }
            }

            entities.SaveChanges();
            return Json("");
        }

        /// <summary>
        /// 确认子任务
        /// </summary>
        /// <param name="subTaskIDs"></param>
        /// <param name="execId"></param>
        /// <returns></returns>
        public JsonResult ConfirmSubTask(string subTaskIDs, string execId)
        {
            foreach (var subTaskId in subTaskIDs.Split(','))
            {
                var subItem = entities.Set<S_T_Task>().SingleOrDefault(c => c.ID == subTaskId);
                subItem.CompleteTime = DateTime.Now;
                subItem.S_T_TaskExec.Update(c => c.ExecTime = DateTime.Now);
            }
            entities.SaveChanges();
            return Json(entities.Set<S_T_Task>().Where(c => c.TaskExecID == execId).Select(c => new
            {
                ID = c.ID,
                Name = c.Name,
                ExecUserIDs = c.ExecUserIDs,
                ExecUserNames = c.ExecUserNames,
                Urgency = c.Urgency,
                DeadLine = c.DeadLine,
                CompleteTime = c.CompleteTime,
                ExecTime = c.S_T_TaskExec.FirstOrDefault().ExecTime
            }));
        }

        public JsonResult RedoSubTask(string subTaskIDs, string execId)
        {
            //子项任务的重做(子项列表上的重做按钮)
            if (!string.IsNullOrEmpty(subTaskIDs))
            {
                var arr = subTaskIDs.Split(',');
                entities.Set<S_T_TaskExec>().Where(c => arr.Contains(c.TaskID)).Update(c => c.ExecTime = null);
                entities.Set<S_T_Task>().Where(c => arr.Contains(c.ID)).Update(c => c.CompleteTime = null);
                entities.SaveChanges();

                return Json(entities.Set<S_T_Task>().Where(c => c.TaskExecID == execId).Select(c => new
                {
                    ID = c.ID,
                    Name = c.Name,
                    ExecUserIDs = c.ExecUserIDs,
                    ExecUserNames = c.ExecUserNames,
                    Urgency = c.Urgency,
                    DeadLine = c.DeadLine,
                    CompleteTime = c.CompleteTime,
                    ExecTime = c.S_T_TaskExec.FirstOrDefault().ExecTime
                }));
            }
            else  //工具栏的重做按钮
            {
                var exec = entities.Set<S_T_TaskExec>().SingleOrDefault(c => c.ID == execId);
                exec.ExecTime = null;
                exec.S_T_Task.CompleteTime = null;
                entities.SaveChanges();
                return Json("");
            }
         
        }


        public JsonResult CompleteTask(string execId)
        {
            var exec = UpdateEntity<S_T_TaskExec>();
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

            entities.SaveChanges();
            return Json("");
        }



    }
}
