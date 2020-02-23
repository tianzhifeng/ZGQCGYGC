using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Logic.Domain;
using Formula;

namespace Project.Areas.Engineering.Controllers
{
    public class YellowPageController : ProjectController
    {
        public ActionResult Index()
        {
            var tab = new Tab();
            var roleCategory = CategoryFactory.GetCategory("Project.OBSRole", "角色", "RoleCode");
            roleCategory.SetDefaultItem();
            roleCategory.Multi = false;
            tab.Categories.Add(roleCategory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            ViewBag.GroupInfoID = this.GetQueryString("GroupInfoID");
            return View();
        }

        public ActionResult List()
        {
            string groupInfoID = this.Request["GroupInfoID"];
            var prjList = this.entities.Set<S_I_ProjectInfo>().Where(d => d.GroupRootID == groupInfoID).ToList();
            List<string> prjIDs = new List<string>();
            var obsList = new List<S_W_OBSUser>();
            if (prjList.Count != 0)
            {
                prjIDs = prjList.Select(d => d.ID).ToList();
                obsList = this.entities.Set<S_W_OBSUser>().Where(d => prjIDs.Contains(d.ProjectInfoID)).ToList();
            }
            var userList = new List<UserList>();
            if (obsList.Count != 0)
            {
                foreach (var user in obsList)
                {
                    var userInfo = new UserList();
                    userInfo.ID = user.UserID;
                    userInfo.UserName = user.UserName;
                    if (!userList.Select(d => d.ID).Contains(user.UserID))
                    {
                        var userBaseInfo = FormulaHelper.GetUserInfoByID(userInfo.ID);
                        if (userBaseInfo != null)
                        {
                            userInfo.Phone = userBaseInfo.UserPhone;
                            userInfo.DeptID = userBaseInfo.UserOrgID;
                            userInfo.DeptName = userBaseInfo.UserOrgName;
                            userInfo.RoleCode = user.RoleCode;
                            userInfo.RoleName = user.RoleName;
                            userList.Add(userInfo);
                        }
                    }
                    else
                    {
                        var curUser = userList.FirstOrDefault(d => d.ID == user.UserID);
                        if (!curUser.RoleCode.Contains(user.RoleCode))
                        {
                            curUser.RoleCode += "," + user.RoleCode;
                            curUser.RoleName += "," + user.RoleName;
                        }
                    }
                }
            }
            string userName = this.Request["EmployeeName"];
            if (!string.IsNullOrEmpty(userName))
            {
                userList = userList.Where(d => d.UserName.Contains(userName)).ToList();
            }

            var resList = new List<UserList>();
            string tabCondition = this.Request["TabCondition"];
            if (!string.IsNullOrEmpty(tabCondition))
            {
                userList.RemoveWhere(m => !tabCondition.Split(',').Any(n => m.RoleCode.Contains(n)));
            }

            ViewBag.EngineeringUsers = userList;
            return View();
        }

        public class UserList
        {
            public string ID { set; get; }
            public string UserName { set; get; }
            public string RoleCode { set; get; }
            public string RoleName { set; get; }
            public string Phone { set; get; }
            public string DeptID { set; get; }
            public string DeptName { set; get; }
        }

    }

}
