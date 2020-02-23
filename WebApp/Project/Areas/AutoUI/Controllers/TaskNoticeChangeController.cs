using Config.Logic;
using EPC.Logic.Domain;
using Formula;
using Project.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic.Domain;

namespace Project.Areas.AutoUI.Controllers
{
    public class TaskNoticeChangeController : ProjectFormContorllor<T_CP_TaskNoticeChange>
    {
        //
        // GET: /AutoUI/TaskNoticeChang

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                string ProjectInfoID = GetQueryString("ProjectInfoID");
                if (!string.IsNullOrEmpty(ProjectInfoID))
                {
                    int maxNumber = 0;
                    ProjectEntities epcentity = FormulaHelper.GetEntities<ProjectEntities>();
                    var ProjectInfo = epcentity.Set<T_CP_TaskNotice>().FirstOrDefault(a => a.ID == ProjectInfoID);
                    var Projects = epcentity.Set<T_CP_TaskNoticeChange>().Where(c => c.ProjectInfoID == ProjectInfoID);

                    if (Projects != null && Projects.Count() > 0)
                    {
                        maxNumber = Projects.ToList().Max(c => Convert.ToInt32(c.VersionNumber));
                    }
                    maxNumber++;

                    if (ProjectInfo != null)
                    {
                        Type tp = ProjectInfo.GetType();
                        var ps = tp.GetProperties();
                        DataRow dtr = dt.Rows[0];
                        foreach (PropertyInfo item in ps)
                        {
                            string cname = item.Name;
                            string type = item.PropertyType.Name;
                            if (type != "ICollection`1" && cname != "ProjectInfoID" && cname != "VersionNumber" && dt.Columns.Contains(cname))
                            {
                                object value = item.GetValue(ProjectInfo, null);
                                if (cname == "ID")
                                {
                                    dtr.SetField("ProjectInfoID", value);
                                }
                                else
                                {
                                    dtr.SetField(cname, value);
                                }
                            }
                        }
                        dtr.SetField("VersionNumber", maxNumber);
                    }
                }
            }
        }
        protected override void OnFlowEnd(T_CP_TaskNoticeChange ProjectInof, S_WF_InsTaskExec taskExec, S_WF_InsDefRouting routing)
        {
            ProjectEntities epcentity = FormulaHelper.GetEntities<ProjectEntities>();
            string ProjectInfoID = ProjectInof.ProjectInfoID;
            var model = epcentity.Set<T_CP_TaskNotice>().FirstOrDefault(a => a.ID == ProjectInfoID);
            if (model != null)
            {
                model.FirstDesignManager = ProjectInof.FirstDesignManager;
                model.FirstDesignManagerName = ProjectInof.FirstDesignManagerName;
                model.FirstDesignManagerDept = ProjectInof.FirstDesignManagerDept;
                model.FirstDesignManagerDeptName = ProjectInof.FirstDesignManagerDeptName;
                model.SecondDesignManager = ProjectInof.SecondDesignManager;
                model.SecondDesignManagerName = ProjectInof.SecondDesignManagerName;
                model.ThirdDesignManager = ProjectInof.ThirdDesignManager;
                model.ThirdDesignManagerName = ProjectInof.ThirdDesignManagerName;
                model.DesignManagerAssistant = ProjectInof.DesignManagerAssistant;
                model.DesignManagerAssistantName = ProjectInof.DesignManagerAssistantName;
                epcentity.SaveChanges();
            }
        }

        public JsonResult ValidateChange(string ProjectInfoID)
        {

            var result = new Dictionary<string, object>();
            string sql = String.Format("select ID from T_CP_TaskNoticeChange where ProjectInfoID='{0}' and FlowPhase='{1}'", ProjectInfoID, "Start");
            var dt = this.ProjectSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                result.SetValue("ID", dt.Rows[0]["ID"]);
                return Json(result);
            }
            sql = String.Format("select Count(ID) from T_CP_TaskNoticeChange where ProjectInfoID='{0}' and FlowPhase='{1}'", ProjectInfoID, "Processing");
            var obj = Convert.ToInt32(this.ProjectSQLDB.ExecuteScalar(sql));
            if (obj > 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("设总任命单信息正在变更中，无法重复启动变更，请在变更结束后再启动变更");
            }
            return Json(result);
        }

    }
}
