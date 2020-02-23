using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;

namespace Project.Areas.Selector.Controllers
{
    public class OBSUserSelectController : ProjectController<S_W_OBSUser>
    {

        public ActionResult OBSUserSelect()
        {
            string wbsType = this.Request["WBSType"];
            string showMajorPrincple = this.Request["ShowMajorPrincple"];
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目对象");
            var mode = BaseConfigFO.GetMode(projectInfo.ModeCode);
            if (mode == null) throw new Formula.Exceptions.BusinessException("编码为【" + projectInfo.ModeCode + "】的管理模式定义不存在");
            var wbsStructNode = mode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == wbsType);
            if (wbsStructNode == null) throw new Formula.Exceptions.BusinessException("未找到WBS节点定义，请确认【" + projectInfo.ModeCode + "】管理模式中存在【" + wbsType + "】的WBS定义");
            var roleDefines = wbsStructNode.S_T_WBSStructRole.Where(d => d.RoleCode != ProjectRole.ProjectManager.ToString()).OrderBy(d => d.SortIndex).ToList();
            if (showMajorPrincple == false.ToString())
                roleDefines = roleDefines.Where(d => d.RoleCode != ProjectRole.MajorPrinciple.ToString()).ToList();
            ViewBag.RoleDefines = roleDefines;
            return View();
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            string sql = @"select distinct UserID,UserName,RoleCode,RoleName,MajorValue from S_W_OBSUser  where ProjectInfoID='{0}'";
            qb.SortField = "UserName";
            var sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);

            if (!String.IsNullOrEmpty(this.GetQueryString("MajorValue")))
            {
                qb.Add("MajorValue", QueryMethod.Equal, this.GetQueryString("MajorValue"));
            }

            var dt = sqlHelper.ExecuteDataTable(String.Format(sql, projectInfoID), (SearchCondition)qb);
            var data = (from DataRow vRow in dt.Rows
                        select new
                        {
                            UserID = vRow["UserID"].ToString(),
                            UserName = vRow["UserName"].ToString(),
                            MajorValue = vRow["MajorValue"].ToString()
                        }).Distinct();
            var result = new List<Dictionary<string, object>>();
            var majorDefList = BaseConfigFO.GetWBSAttrList(WBSNodeType.Major);
            foreach (var user in data)
            {
                var item = new Dictionary<string, object>();
                item.SetValue("UserID", user.UserID);
                item.SetValue("UserName", user.UserName);
                if (!string.IsNullOrEmpty(user.MajorValue))
                {
                    item.SetValue("MajorValue", user.MajorValue);
                    var major = majorDefList.FirstOrDefault(d => d.Code == user.MajorValue);
                    if (major != null)
                        item.SetValue("MajorName", major.Name);
                }
                var roleRows = dt.Select("UserID='" + user.UserID + "' and MajorValue='" + user.MajorValue + "'");
                string roleCodes = string.Empty; string roleNames = string.Empty;
                foreach (var roleRow in roleRows)
                {
                    roleCodes += roleRow["RoleCode"].ToString() + ",";
                    roleNames += roleRow["RoleName"].ToString() + ",";
                }
                item.SetValue("RoleCode", roleCodes.TrimEnd(','));
                item.SetValue("RoleName", roleNames.TrimEnd(','));
                result.Add(item);
            }
            return Json(result);
        }
    }
}
