using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using HR.Logic;
using HR.Logic.Domain;
using System.Data;
using Formula;
using MvcAdapter;
using Newtonsoft.Json;
using Formula.ImportExport;
using System.Text;

namespace HR.Areas.UserAptitude.Controllers
{
    public class AptitudeController : HRController<S_D_UserAptitude>
    {
        public ActionResult MajorList()
        {
            var tab = new Tab();
            var majorCategory = CategoryFactory.GetCategory("Project.Major", "专业", "EngageMajor", true);
            var majorTable = EnumBaseHelper.GetEnumTable("Project.Major");
            if (majorTable.Rows.Count > 0)
                majorCategory.SetDefaultItem(majorTable.Rows[0]["value"].ToString());
            majorCategory.Multi = true;
            tab.Categories.Add(majorCategory);

            var businessTypeCategory = CategoryFactory.GetCategory("Market.BusinessType", "业务类型", "ProjectClass", true);
            var businessTypeTable = EnumBaseHelper.GetEnumTable("Market.BusinessType");
            if (businessTypeTable.Rows.Count > 0)
                businessTypeCategory.SetDefaultItem(businessTypeTable.Rows[0]["value"].ToString());
            businessTypeCategory.Multi = true;
            tab.Categories.Add(businessTypeCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            var aputitudeTable = EnumBaseHelper.GetEnumTable("HR.UserAptitude");
            ViewBag.ColumnTable = aputitudeTable;
            return View();
        }

        public ActionResult ManagerList()
        {
            var tab = new Tab();
            var businessTypeCategory = CategoryFactory.GetCategory("Market.BusinessType", "业务类型", "ProjectClass", false);
            var businessTypeTable = EnumBaseHelper.GetEnumTable("Market.BusinessType");
            if (businessTypeTable.Rows.Count > 0)
                businessTypeCategory.SetDefaultItem(businessTypeTable.Rows[0]["value"].ToString());
            businessTypeCategory.Multi = false;
            tab.Categories.Add(businessTypeCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            var aputitudeTable = EnumBaseHelper.GetEnumTable("HR.UserProjectAptitude");
            ViewBag.ColumnTable = aputitudeTable;
            return View();
        }

        public ActionResult MajorSimpleList()
        {
            var tab = new Tab();
            var businessTypeCategory = CategoryFactory.GetCategory("Market.BusinessType", "业务类型", "ProjectClass", true);
            var businessTypeTable = EnumBaseHelper.GetEnumTable("Market.BusinessType");
            if (businessTypeTable.Rows.Count > 0)
                businessTypeCategory.SetDefaultItem(businessTypeTable.Rows[0]["value"].ToString());
            businessTypeCategory.Multi = true;
            tab.Categories.Add(businessTypeCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            var aputitudeTable = EnumBaseHelper.GetEnumTable("HR.UserAptitude");
            ViewBag.ColumnTable = aputitudeTable;
            return View();
        }

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var enumService = FormulaHelper.GetService<IEnumService>();
            var queryTabData = this.Request["queryTabData"];
            var tabData = JsonHelper.ToList(queryTabData);
            string major = string.Empty, businessType = string.Empty;
            List<string> majors = new List<string>(), businessTypes = new List<string>();
            foreach (var item in tabData)
            {
                if (item.GetValue("queryfield") == "EngageMajor")
                    major = item.GetValue("value");
                else if (item.GetValue("queryfield") == "ProjectClass")
                    businessType = item.GetValue("value");
            }
            var majorCnd = qb.Items.FirstOrDefault(d => d.Field == "EngageMajor");
            if (majorCnd != null)
            {
                if (string.IsNullOrEmpty(major))
                    major = majorCnd.Value.ToString();
                qb.Items.Remove(majorCnd);
            }
            var businessTypeCnd = qb.Items.FirstOrDefault(d => d.Field == "ProjectClass");
            if (businessTypeCnd != null)
            {
                if (string.IsNullOrEmpty(businessType))
                    businessType = businessTypeCnd.Value.ToString();
                qb.Items.Remove(businessTypeCnd);
            }
            if (!string.IsNullOrEmpty(major))
                majors = major.Split(',').ToList();
            if (!string.IsNullOrEmpty(businessType))
                businessTypes = businessType.Split(',').ToList();

            var whereSqlList = new List<string>();
            var employeeSql = "select ID,UserID,Name,EngageMajor,DeptID,DeptName,'" + major + "' as Major,'" + businessType + "' as ProjectClass from T_Employee where UserID is not null and UserID <> '' and IsDeleted = '0'";
            foreach (var item in majors)
                if (!string.IsNullOrEmpty(item))
                    whereSqlList.Add("','+EngageMajor+',' like '%," + item + ",%'");
            if (whereSqlList.Count > 0)
                employeeSql += " and (" + string.Join(" or ", whereSqlList) + ")";
            var employeeDt = this.SqlHelper.ExecuteDataTable(employeeSql, qb);
            var aptitudeSql = "select * from S_D_UserAptitude where 1=1";
            if (!string.IsNullOrEmpty(major))
                aptitudeSql += " and Major in ('" + major.Replace(",", "','") + "')";
            if (!string.IsNullOrEmpty(businessType))
                aptitudeSql += "and ProjectClass in ('" + businessType.Replace(",", "','") + "')";
            var aptitudeList = this.SqlHelper.ExecuteList<S_D_UserAptitude>(aptitudeSql);

            var aputitudeEnums = EnumBaseHelper.GetEnumTable("HR.UserAptitude");
            var aptEnumList = new List<string>();
            foreach (DataRow item in aputitudeEnums.Rows)
            {
                employeeDt.Columns.Add(item["value"].ToString());
                employeeDt.Columns.Add(item["value"].ToString() + "Title");
                aptEnumList.Add(item["value"].ToString());
            }

            var numInTheory = majors.Count() * businessTypes.Count();
            foreach (DataRow user in employeeDt.Rows)
            {
                var aptitudes = aptitudeList.Where(a => a.UserID == user["UserID"].ToString());
                if (!string.IsNullOrEmpty(major))
                    aptitudes = aptitudes.Where(a => majors.Contains(a.Major));
                if (!string.IsNullOrEmpty(businessType))
                    aptitudes = aptitudes.Where(a => businessTypes.Contains(a.ProjectClass));
                foreach (var aptEnum in aptEnumList)
                {
                    var aptitudeLevels = aptitudes.Where(a => a.Position == aptEnum).OrderBy(a => a.ProjectClass).ThenBy(a => a.Major);
                    var levels = aptitudeLevels.Select(a => a.AptitudeLevel).Distinct();
                    if (aptitudeLevels.Count() > 0)
                    {
                        if (levels.Count() == 1 && aptitudeLevels.Count() == numInTheory)
                            user[aptEnum] = levels.FirstOrDefault();
                        else
                        {
                            user[aptEnum] = "multi";
                            var titleList = aptitudeLevels.Select(a => "{" + a.ProjectClass + "}-{" + a.Major + "}：{" + a.AptitudeLevel + "}");
                            user[aptEnum + "Title"] = string.Join("\n", titleList);
                            if (titleList.Count() != numInTheory)
                                user[aptEnum + "Title"] += "\n其他：无";
                        }
                    }
                }
            }
            GridData gridData = new GridData(employeeDt);
            gridData.total = qb.TotolCount;
            return Json(gridData);
        }

        public JsonResult GetProjectAptitudeList(QueryBuilder qb)
        {
            string sql = @"select * from (select UserID,UserName,DeptID,DeptName,ProjectClass,Major,Max(ProjectManager) as ProjectManager,Max(DesignManager) as DesignManager from (
SELECT UserID,UserName,DeptID,DeptName,AptitudeLevel,ProjectClass,Position,Major FROM S_D_UserAptitude
WHERE Major='Project') TableInfo
pivot(Max(AptitudeLevel) for Position in (ProjectManager,DesignManager)) as NewTable
group by UserID,UserName,DeptID,DeptName,ProjectClass,Major) tableInfo";
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult AddProjectUser(string UserInfo, string ProjectClass)
        {
            var list = JsonHelper.ToList(UserInfo);
            var aputitudeTable = EnumBaseHelper.GetEnumTable("HR.UserProjectAptitude");
            foreach (var item in list)
            {
                var userID = item.GetValue("ID");
                var userName = item.GetValue("Name");
                var userService = FormulaHelper.GetService<IUserService>();
                var userInfo = userService.GetUserInfoByID(userID);
                foreach (DataRow row in aputitudeTable.Rows)
                {
                    var position = row["value"].ToString();
                    var entity = this.entities.Set<S_D_UserAptitude>().FirstOrDefault(d => d.ProjectClass == ProjectClass
                        && d.Major == "Project" && d.UserID == userID && d.Position == position);
                    if (entity == null)
                    {
                        entity = this.entities.Set<S_D_UserAptitude>().Create();
                        entity.ID = FormulaHelper.CreateGuid();
                        entity.UserID = userID;
                        entity.UserName = userName;
                        entity.DeptID = userInfo.UserOrgID;
                        entity.DeptName = userInfo.UserOrgName;
                        entity.Position = position;
                        entity.ProjectClass = ProjectClass;
                        entity.Major = "Project";
                        entity.AptitudeLevel = 10;
                        this.entities.Set<S_D_UserAptitude>().Add(entity);
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteProjectUser(string ListData, string ProjectClass)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var userID = item.GetValue("UserID");
                this.entities.Set<S_D_UserAptitude>().Delete(d => d.UserID == userID && d.Major == "Project" && d.ProjectClass == ProjectClass);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveAptitude(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            var aputitudeTable = EnumBaseHelper.GetEnumTable("HR.UserAptitude");
            var sql = @"
if exists(select 1 from S_D_UserAptitude where UserID='{1}' and Major='{6}'and ProjectClass='{7}'and Position='{8}')
begin
update S_D_UserAptitude set AptitudeLevel='{9}' where UserID='{1}' and Major='{6}'and ProjectClass='{7}'and Position='{8}'
end
else
begin 
insert into S_D_UserAptitude values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9})
end";
            var deleteSql = "delete S_D_UserAptitude where UserID='{0}' and Major='{1}'and ProjectClass='{2}'and Position='{3}'";

            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                var employee = this.GetEntityByID<T_Employee>(item.GetValue("ID"));
                if (employee == null) throw new Formula.Exceptions.BusinessException("找不到ID为【" + item.GetValue("ID") + "】的T_Employee记录");
                var userID = item.GetValue("UserID");
                var majors = item.GetValue("Major").Split(',');
                var businessTypes = item.GetValue("ProjectClass").Split(',');
                foreach (var major in majors)
                {
                    foreach (var businessType in businessTypes)
                    {
                        foreach (DataRow row in aputitudeTable.Rows)
                        {
                            var id = FormulaHelper.CreateGuid();
                            var aptitudeLevel = item.GetValue(row["value"].ToString());
                            if (aptitudeLevel == "multi") continue;
                            if (string.IsNullOrEmpty(aptitudeLevel))
                                sb.AppendLine(string.Format(deleteSql, item.GetValue("UserID"), major, businessType, row["value"].ToString()));
                            else
                                sb.AppendLine(string.Format(sql, id, item.GetValue("UserID"), item.GetValue("Name"), item.GetValue("ID"), item.GetValue("DeptID")
                                    , item.GetValue("DeptName"), major, businessType, row["value"].ToString(), aptitudeLevel));

                        }
                    }
                    if (!(("," + employee.EngageMajor + ",").Contains("," + major + ",")))
                    {
                        if (string.IsNullOrEmpty(employee.EngageMajor))
                            employee.EngageMajor = major;
                        else
                            employee.EngageMajor += "," + major;
                    }
                }
            }
            if (sb.Length > 0)
            {
                this.SqlHelper.ExecuteNonQueryWithTrans(sb.ToString());
                this.entities.SaveChanges();
            }
            return Json("");
        }

        public JsonResult SaveProjectAptitude(string ListData, string ProjectClass)
        {
            var list = JsonHelper.ToList(ListData);
            var aputitudeTable = EnumBaseHelper.GetEnumTable("HR.UserProjectAptitude");
            foreach (var item in list)
            {
                var userID = item.GetValue("UserID");
                foreach (DataRow row in aputitudeTable.Rows)
                {
                    var position = row["value"].ToString();
                    var entity = this.entities.Set<S_D_UserAptitude>().FirstOrDefault(d => d.UserID == userID && d.ProjectClass == ProjectClass && d.Major == "Project"
                        && d.Position == position);
                    var AptitudeLevel = item.GetValue(position);
                    if (String.IsNullOrEmpty(AptitudeLevel))
                    {
                        if (entity != null) this.entities.Set<S_D_UserAptitude>().Remove(entity);
                    }
                    else
                    {
                        if (entity == null)
                        {
                            entity = this.entities.Set<S_D_UserAptitude>().Create();
                            entity.ID = FormulaHelper.CreateGuid();
                            entity.UserID = userID;
                            entity.UserName = item.GetValue("UserName");
                            entity.Position = position;
                            entity.Major = "Project";
                            entity.ProjectClass = ProjectClass;
                            entity.DeptID = item.GetValue("DeptID");
                            entity.DeptName = item.GetValue("DeptName");
                        }
                        entity.AptitudeLevel = Convert.ToInt32(AptitudeLevel); ;
                    }
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        #region EXCEL导入
        public JsonResult ValidateData()
        {
            var sql = "select * from T_Employee where UserID IS NOT NULL AND UserID <> '' and IsDeleted = 0";
            var employeeDt = this.SqlHelper.ExecuteDataTable(sql);
            var businessTypeDt = EnumBaseHelper.GetEnumTable("Market.BusinessType");
            var majorDt = EnumBaseHelper.GetEnumTable("Project.Major");
            var userAptitudeDt = EnumBaseHelper.GetEnumTable("HR.UserAptitude");
            var aptitudeLevelDt = EnumBaseHelper.GetEnumTable("System.AptitudeLevel");

            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "UserName")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = string.Format("人员姓名不能为空", e.Value);
                    }
                    else
                    {
                        var deptName = "";
                        if (e.Record.Table.Columns.Contains("DeptName"))
                            deptName = e.Record["DeptName"].ToString();
                        var users = employeeDt.Select("Name='" + e.Value + "'");
                        if (!string.IsNullOrEmpty(deptName))
                            users = employeeDt.Select("Name='" + e.Value + "'  and DeptName='" + deptName + "'");
                        if (users.Length == 0)
                        {
                            e.IsValid = false;
                            e.ErrorText = "T_Employee表中不存在部门为【" + deptName + "】，姓名为【" + e.Value + "】的员工";
                        }
                        else if (users.Length > 1)
                        {
                            e.IsValid = false;
                            e.ErrorText = "T_Employee表中部门为【" + deptName + "】，姓名为【" + e.Value + "】的员工记录超过1条";
                        }
                    }
                }
                else if (e.FieldName == "ProjectClass")
                {
                    if (!string.IsNullOrEmpty(e.Value))
                    {
                        var errorMessage = "";
                        var flag = false;
                        foreach (var pClass in e.Value.Split(','))
                        {
                            var businessTypes = businessTypeDt.Select("text='" + pClass + "'");
                            if (businessTypes.Length == 0)
                            {
                                flag = true;
                                errorMessage += pClass + "、";
                            }
                        }
                        if (flag)
                        {
                            e.IsValid = false;
                            e.ErrorText = "业务类型【" + errorMessage.TrimEnd('、') + "】不存在";
                        }
                    }
                }
                else if (e.FieldName == "Major")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "专业不能为空";
                    }
                    else
                    {
                        var errorMessage = "";
                        var flag = false;
                        foreach (var major in e.Value.Split(','))
                        {
                            var majors = majorDt.Select("text='" + major + "'");
                            if (majors.Length == 0)
                            {
                                flag = true;
                                errorMessage += major + "、";
                            }
                        }
                        if (flag)
                        {
                            e.IsValid = false;
                            e.ErrorText = "专业【" + errorMessage.TrimEnd('、') + "】不存在";
                        }
                    }
                }
                else if (e.FieldName == "Position")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "角色不能为空";
                    }
                    else
                    {
                        var errorMessage = "";
                        var flag = false;
                        foreach (var pos in e.Value.Split(','))
                        {
                            var userAptitudes = userAptitudeDt.Select("text='" + pos + "'");
                            if (userAptitudes.Length == 0)
                            {
                                flag = true;
                                errorMessage += pos + "、";
                            }
                        }
                        if (flag)
                        {
                            e.IsValid = false;
                            e.ErrorText = "角色【" + errorMessage.TrimEnd('、') + "】不存在";
                        }
                    }
                }
                else if (e.FieldName == "AptitudeLevel")
                {
                    if (string.IsNullOrWhiteSpace(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "等级不能为空";
                    }
                    else
                    {
                        var aptitudeLevels = aptitudeLevelDt.Select("text='" + e.Value + "'");
                        if (aptitudeLevels.Length == 0)
                        {
                            e.IsValid = false;
                            e.ErrorText = "等级【" + e.Value + "】不存在";
                        }
                    }
                }
            });
            return Json(errors);
        }

        public JsonResult SaveExcelData()
        {
            var employeeList = this.entities.Set<T_Employee>().Where(a => !string.IsNullOrEmpty(a.UserID) && a.IsDeleted == "0").ToList();
            var businessTypeDt = EnumBaseHelper.GetEnumTable("Market.BusinessType");
            var majorDt = EnumBaseHelper.GetEnumTable("Project.Major");
            var userAptitudeDt = EnumBaseHelper.GetEnumTable("HR.UserAptitude");
            var aptitudeLevelDt = EnumBaseHelper.GetEnumTable("System.AptitudeLevel");

            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var dicList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(tempdata["data"]);

            var resultDt = this.SqlHelper.ExecuteDataTable("SELECT * FROM S_D_UserAptitude WITH(NOLOCK) WHERE 1<>1");
            var employeeIDList = new List<string>();
            foreach (var dic in dicList)
            {
                var deptName = dic.GetValue("DeptName");
                var employees = employeeList.Where(a => a.Name == dic.GetValue("UserName"));
                if (!string.IsNullOrEmpty(deptName))
                    employees = employeeList.Where(a => a.Name == dic.GetValue("UserName") && a.DeptName == deptName);
                if (employees.Count() > 0)
                {
                    var employee = employees.FirstOrDefault();
                    employeeIDList.Add(employee.ID);

                    var businessTypes = dic.GetValue("ProjectClass");
                    var majors = dic.GetValue("Major");
                    var positions = dic.GetValue("Position");
                    var aptitudeLevel = "";
                    var aptitudeLevelRows = aptitudeLevelDt.Select("text='" + dic.GetValue("AptitudeLevel") + "'");
                    if (aptitudeLevelRows.Length > 0)
                        aptitudeLevel = aptitudeLevelRows[0]["value"].ToString();
                    foreach (var major in majors.Split(','))
                    {
                        var mValue = "";
                        var majorRows = majorDt.Select("text='" + major + "'");
                        if (majorRows.Length > 0)
                            mValue = majorRows[0]["value"].ToString();
                        foreach (var position in positions.Split(','))
                        {
                            var pValue = "";
                            var positionRows = userAptitudeDt.Select("text='" + position + "'");
                            if (positionRows.Length > 0)
                                pValue = positionRows[0]["value"].ToString();
                            foreach (var businessType in businessTypes.Split(','))
                            {
                                var bValue = "";
                                var businessTypeRows = businessTypeDt.Select("text='" + businessType + "'");
                                if (businessTypeRows.Length > 0)
                                    bValue = businessTypeRows[0]["value"].ToString();

                                var row = resultDt.NewRow();
                                row["ID"] = FormulaHelper.CreateGuid();
                                row["UserID"] = employee.UserID;
                                row["UserName"] = employee.Name;
                                row["HREmployeeID"] = employee.ID;
                                row["DeptID"] = employee.DeptID;
                                row["DeptName"] = employee.DeptName;
                                row["Major"] = mValue;
                                row["ProjectClass"] = bValue;
                                row["Position"] = pValue;
                                row["AptitudeLevel"] = aptitudeLevel;
                                resultDt.Rows.Add(row);
                            }
                        }

                        if (!(("," + employee.EngageMajor + ",").Contains("," + mValue + ",")))
                        {
                            if (string.IsNullOrEmpty(employee.EngageMajor))
                                employee.EngageMajor = major;
                            else
                                employee.EngageMajor += "," + major;
                        }
                    }
                }
            }
            if (employeeIDList.Count() > 0)
            {
                this.SqlHelper.ExecuteNonQueryWithTrans(string.Format("delete from S_D_UserAptitude where HREmployeeID in ('{0}')", string.Join("','", employeeIDList)));

                this.SqlHelper.BulkInsertToDB(resultDt, "S_D_UserAptitude");
                this.entities.SaveChanges();
            }
            return Json("Success");
        }
        #endregion
    }
}
