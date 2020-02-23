using Formula;
using Formula.Helper;
using Project.Logic;
using Project.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config;
using Config.Logic;
using MvcAdapter;
using Base.Logic.Domain;

namespace Project.Areas.Basic.Controllers
{
    public class PackageSchemeController : ProjectFormContorllor<S_W_TaskWork>
    {
        public ActionResult Package()
        {
            var spaceCode = GetQueryString("SpaceCode");
            string projectInfoID = GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息");
            var majors = projectInfo.S_W_WBS.Where(a => a.WBSType == "Major").
                Select(a => new { text = a.Name, value = a.WBSValue }).ToList();

            var phase = new List<Dictionary<string, string>>();
            for (int i = 0; i < projectInfo.PhaseValue.Split(',').Count(); i++)
                phase.Add(new Dictionary<string, string>() {{ "text", projectInfo.PhaseName.Split(',')[i] }, { "value", projectInfo.PhaseValue.Split(',')[i] }  });

            var tab = new Tab();
            var majorCategory = CategoryFactory.GetCategoryByString(JsonHelper.ToJson(majors), "专业", "MajorValue");
            if (spaceCode != "Project") majorCategory.SetDefaultItem(spaceCode);
            majorCategory.Multi = false;
            tab.Categories.Add(majorCategory);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            var majorStruct = projectInfo.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Major.ToString());
            if (majorStruct == null) throw new Formula.Exceptions.BusinessException("项目不存在专业节点定义，无法进行专业策划");

            //数据字典定义，用于列表名字配置
            var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            var table = baseEntities.Set<S_M_Table>().FirstOrDefault(a => a.Code == "S_W_TaskWork");
            if (table != null)
            {
                var fieds = table.S_M_Field.Where(a => a.Name != "" && a.Name != null).Select(a => new { a.Name,a.Code}).ToList();
                ViewBag.HeaderNames = JsonHelper.ToJson(fieds);
            }
            else
                ViewBag.HeaderNames = "[]";


            var simpleMode = GetQueryString("SimpleMode");

            ViewBag.IsProject = spaceCode == "Project" ? "true" : "false";
            ViewBag.ProjectInfo = JsonHelper.ToJson(projectInfo);
            ViewBag.Major = JsonHelper.ToJson(majors);
            ViewBag.PhaseEnum = JsonHelper.ToJson(phase);
            ViewBag.SimpleMode = simpleMode == "True" ? true : false;
            return View();
        }

        public ActionResult ProjectPackageSelector()
        {
            //数据字典定义，用于列表名字配置
            var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            var table = baseEntities.Set<S_M_Table>().FirstOrDefault(a => a.Code == "S_W_TaskWork");
            if (table != null)
            {
                var fieds = table.S_M_Field.Where(a => a.Name != "" && a.Name != null).Select(a => new { a.Name, a.Code }).ToList();
                ViewBag.HeaderNames = JsonHelper.ToJson(fieds);
            }
            else
                ViewBag.HeaderNames = "[]";
            return View();
        }

        public JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            if (qb.SortField == "ID")
                qb.SortField = "MajorValue";

            var projectInfoID = GetQueryString("ProjectInfoID");
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到指定的项目。");
            Dictionary<string, object> data = new Dictionary<string, object>();
            string sql = @"select * from S_W_TaskWork where ProjectInfoID='"+projectInfoID+"'";
            var TaskWorks =this.BusinessEntities.Set<S_W_TaskWork>().Where(a => a.ProjectInfoID == projectInfoID).WhereToGridData(qb);
            qb.Items.RemoveWhere(d => d.Field != "MajorValue");
            var sumSchemed = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => a.ProjectInfoID == projectInfoID).Where(qb).Sum(a => a.Workload);
            decimal? sumWorkload = 0;
            decimal? sumPublished = 0;
            decimal? sumFinished = 0;
            var code = CBSCategoryType.Product.ToString();
            if (qb.Items.Count != 0)
                code = qb.Items[0].Value.ToString();

            var cbs = this.BusinessEntities.Set<S_C_CBS>().Where(a => a.Code == code && a.ProjectInfoID == projectInfoID);
            sumWorkload = cbs.Sum(a => a.Quantity);
            sumPublished = cbs.Sum(a => a.SummaryBudgetQuantity);
            sumFinished = cbs.Sum(a => a.SummaryCostQuantity);

            var obsUsers = projectInfo.S_W_OBSUser.Select(a => new { Major = a.MajorValue, RoleCode = a.RoleCode, UserID = a.UserID, UserName = a.UserName }).ToList();
            if (code != CBSCategoryType.Product.ToString()) obsUsers = obsUsers.Where(a => a.Major == code).ToList();

            data.Add("data", TaskWorks.data);
            data.Add("total", qb.TotolCount);
            data.Add("sumData", TaskWorks.sumData);
            data.Add("avgData", TaskWorks.avgData);
            data.Add("sumWorkload", sumWorkload == null ? 0 : sumWorkload);
            data.Add("sumSchemed", sumSchemed == null ? 0 : sumSchemed);
            data.Add("sumPublished", sumPublished == null ? 0 : sumPublished);
            data.Add("sumFinished", sumFinished == null ? 0 : sumFinished);
            data.Add("obsUsers", obsUsers);
            return Json(data);
        }

        public JsonResult GetAllPlanTasks(string ProjectInfoID,string SpaceCode)
        {
            var planState = TaskWorkState.Plan.ToString();
            var list = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => a.ProjectInfoID == ProjectInfoID && a.State == planState).ToList();
            if (SpaceCode != "Project")
                list = list.Where(a => a.MajorValue == SpaceCode).ToList();
            return Json(list);
        }

        public JsonResult PublishTask(string IDs)
        {
            var idAry = IDs.Split(',');
            var list = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => idAry.Contains(a.ID)).ToList();
            foreach (var item in list)
                item.Publish();
            this.BusinessEntities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveWork(string TaskInfo, string ProjectInfoID)
        {
            var list = JsonHelper.ToList(TaskInfo);
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            var taskWorkList = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => a.ProjectInfoID == ProjectInfoID).ToList();
            foreach (var item in list)
            {
                var major = item.GetValue("MajorValue");
                var parent = this.BusinessEntities.Set<S_W_WBS>().FirstOrDefault(
                    a => a.ProjectInfoID == ProjectInfoID && a.WBSValue == major);
                if (parent == null) throw new Formula.Exceptions.BusinessException("未获取到当前卷册的专业，保存失败。");
                S_W_TaskWork taskWork;
                if (String.IsNullOrEmpty(item.GetValue("ID")))
                {
                    taskWork = new S_W_TaskWork();
                    this.UpdateEntity<S_W_TaskWork>(taskWork, item);
                    if (taskWorkList.FirstOrDefault(a => a.Code == taskWork.Code && a.PhaseValue == item.GetValue("PhaseValue")) != null)
                        throw new Formula.Exceptions.BusinessException("相同阶段下已有编号为【" + taskWork.Code + "】的卷册，请重新策划。");
                    taskWork.State = TaskWorkState.Plan.ToString();
                    var TaskWorkWBS = parent.AddTaskWork(taskWork);
                    taskWork.InitRoleRate();
                }
                else
                {
                    taskWork = this.GetEntityByID<S_W_TaskWork>(item.GetValue("ID"));
                    this.UpdateEntity<S_W_TaskWork>(taskWork, item);

                    if (taskWorkList.FirstOrDefault(a => a.Code == taskWork.Code && a.PhaseValue == taskWork.PhaseValue && a.ID != taskWork.ID) != null)
                        throw new Formula.Exceptions.BusinessException("相同阶段下已有编号为【" + taskWork.Code + "】的卷册，请重新策划。");

                    if (taskWork.Workload < taskWork.WorkloadFinish)
                        throw new Formula.Exceptions.BusinessException("下达的工时不能小于已结算的工时。");

                    taskWork.Save();

                }
                taskWork.S_W_WBS.PhaseCode = taskWork.PhaseValue;
                //修改成果的阶段、专业、卷号
                foreach (var product in taskWork.S_W_WBS.S_E_Product.ToList())
                {
                    product.PhaseValue = taskWork.PhaseValue;
                    product.MajorValue = taskWork.MajorValue;
                    product.MonomerCode = taskWork.DossierCode;
                    product.MonomerInfo = taskWork.DossierName;
                    product.PackageCode = taskWork.Code;
                    product.PackageName = taskWork.Name;
                }
                if (taskWork.Version == null)
                    taskWork.Version = 1;
            }
            this.BusinessEntities.SaveChanges();
            new WBSFO().AutoCreateWBSEntity(projectInfo.ID);
            return Json("");
        }

        public JsonResult ImportPackage(string TaskInfo, string ProjectInfoID, string WithUser, string major)
        {
            var list = JsonHelper.ToList(TaskInfo);
            var parent = this.BusinessEntities.Set<S_W_WBS>().FirstOrDefault(
                a => a.ProjectInfoID == ProjectInfoID && a.WBSValue == major);
            if (parent == null) throw new Formula.Exceptions.BusinessException("未获取到当前工作包的专业，保存失败。");

            foreach (var item in list)
            {
                var task = new S_W_TaskWork();
                task.Name = item.GetValue("Name");
                task.Code = item.GetValue("Code");
                if (!String.IsNullOrEmpty(item.GetValue("Workload")))
                    task.Workload = Convert.ToDecimal(item.GetValue("Workload"));
                else
                    task.Workload = 0;
                task.PhaseValue = item.GetValue("PhaseCode");
                task.MajorValue = item.GetValue("MajorCode");
                if (WithUser == true.ToString())
                    task.FillWBSUser(parent);
                parent.AddTaskWork(task);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteTaskWork(string ListIDs)
        {
            var list = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => ListIDs.Contains(a.ID)).ToList();
            foreach (var item in list)
                item.Delete();
            this.BusinessEntities.SaveChanges();
            var nodeType = CBSNodeType.Root.ToString();
            var projectInfoID = GetQueryString("ProjectInfoID");
            var rootCBS = this.BusinessEntities.Set<S_C_CBS>().FirstOrDefault(a => a.NodeType == nodeType && a.ProjectInfoID == projectInfoID);
            if (rootCBS != null)
            {
                rootCBS.SummaryBudget();
                this.BusinessEntities.SaveChanges();
            }
            return Json("");
        }

        protected override void BeforeSaveSubTable(Dictionary<string, string> dic, string subTableName, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            decimal sumWorkload = 0;
            foreach (var detail in detailList)
            {
                sumWorkload += string.IsNullOrEmpty(detail.GetValue("Workload")) ? 0m : Convert.ToDecimal(detail.GetValue("Workload"));
            }
            var workload = string.IsNullOrEmpty(dic.GetValue("Workload")) ? 0m : Convert.ToDecimal(dic.GetValue("Workload"));
            if (workload > sumWorkload)
                throw new Formula.Exceptions.BusinessException("卷册的工时没有分配完。");
            else if (workload < sumWorkload)
                throw new Formula.Exceptions.BusinessException("分配的工时总数超过卷册的工时，请重新分配。");
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var taskWork = this.GetEntityByID<S_W_TaskWork>(dic.GetValue("ID"));
            var sum = taskWork.S_W_TaskWork_RoleRate.Sum(a => a.Rate).ToString().TrimEnd('0').TrimEnd('.') + "%";

            var d = taskWork.S_W_TaskWork_RoleRate.FirstOrDefault(a => a.Role == "Designer");
            var c = taskWork.S_W_TaskWork_RoleRate.FirstOrDefault(a => a.Role == "Collactor");
            var au = taskWork.S_W_TaskWork_RoleRate.FirstOrDefault(a => a.Role == "Auditor");
            var ap = taskWork.S_W_TaskWork_RoleRate.FirstOrDefault(a => a.Role == "Approver");
            var m = taskWork.S_W_TaskWork_RoleRate.FirstOrDefault(a => a.Role == "Mapper");
            var designStr = d == null ? "" : "设(" + d.Rate.ToString().TrimEnd('0').TrimEnd('.') + "%);";
            var collactStr = c == null ? "" : "校(" + c.Rate.ToString().TrimEnd('0').TrimEnd('.') + "%);";
            var auditStr = au == null ? "" : "审(" + au.Rate.ToString().TrimEnd('0').TrimEnd('.') + "%);";
            var approveStr = ap == null ? "" : "定(" + ap.Rate.ToString().TrimEnd('0').TrimEnd('.') + "%);";
            var mapperStr = m == null ? "" : "制(" + m.Rate.ToString().TrimEnd('0').TrimEnd('.') + "%);"; 
            
            var sumString = sum + " :" + (designStr == "设(%);" ? "" : designStr)
                        + (collactStr == "校(%);" ? "" : collactStr)
                        + (auditStr == "审(%);" ? "" : auditStr)
                        + (approveStr == "定(%);" ? "" : approveStr)
                        + (mapperStr == "制(%);" ? "" : mapperStr);
            taskWork.WorkloadDistribute = sumString;

            taskWork.Save();

            taskWork.RoleRate = JsonHelper.ToJson(taskWork.S_W_TaskWork_RoleRate.OrderBy(a => a.SortIndex));
            this.BusinessEntities.SaveChanges();
        }

        public JsonResult GetProjectInfo(QueryBuilder qb)
        {
            var grid = this.BusinessEntities.Set<S_I_ProjectInfo>().WhereToGridData(qb);
            return Json(grid);
        }

        public JsonResult GetProjectPackageList(string ProjectInfoID, string Majors, string Phases, QueryBuilder qb)
        {
            qb.Add("MajorValue", QueryMethod.In, Majors);
            qb.Add("PhaseValue", QueryMethod.In, Phases);
            var grid = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => a.ProjectInfoID == ProjectInfoID).WhereToGridData(qb);
            return Json(grid);
        }
    }
}
