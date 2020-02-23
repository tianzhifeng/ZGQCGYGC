using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;

namespace EPC.Areas.Manage.Controllers
{
    public class ToDoListController : EPCController<S_T_ToDoList>
    {
        public JsonResult ValidateExecute(string ToDoListID)
        {
            var toDoListEty = this.GetEntityByID<S_T_ToDoList>(ToDoListID);
            if (toDoListEty == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的待办事项，无法执行") ;
            toDoListEty.ValidateExe();
            return Json("");
        }

        public JsonResult SetFinish(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var toDoListEty = this.GetEntityByID<S_T_ToDoList>(item.GetValue("ID"));
                if (toDoListEty == null) continue;
                toDoListEty.SetFinish();
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
