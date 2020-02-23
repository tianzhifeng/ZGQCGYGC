using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Comprehensive.Logic;
using Comprehensive.Logic.Domain;
using Comprehensive.Logic.BusinessFacade;
using Config;
using Config.Logic;
using System.Data;

namespace Comprehensive.Areas.Employee.Controllers
{
    public class EmployeeJobController : ComprehensiveFormController<S_HR_EmployeeJob>
    {
        public JsonResult GetJobEnumByDept(string DeptID)
        {
            List<object> list = new List<object>();
            string sql = string.Format("SELECT * From dbo.S_A_Org Where ParentID='{0}' and IsDeleted='0' and Type='{1}'", DeptID, OrgType.Post);
            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            DataTable dt = baseHelper.ExecuteDataTable(sql);

            list.Add(new { text = "普通员工", value = "GeneralEmployee" });
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new { text = row["Name"], value = row["ID"] });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var isMain = dic.GetValue("IsMain");
            var ID = dic.GetValue("ID");
            var employeeID = dic.GetValue("EmployeeID");
            if (isMain == "1")
            {
                var list = ComprehensiveDbContext.Set<S_HR_EmployeeJob>().Where(c => c.IsMain == isMain && c.ID != ID && c.IsDeleted == "0" && c.EmployeeID == employeeID).ToList();
                if (list.Count > 0)
                    throw new Formula.Exceptions.BusinessValidationException(string.Format("已存在主责部门职务【{0}:{1}】！", list.FirstOrDefault().DeptIDName, list.FirstOrDefault().JobName));
            }
            //base.BeforeSave(dic, formInfo, isNew);
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var employee = ComprehensiveDbContext.Set<S_HR_Employee>().Find(dic.GetValue("EmployeeID"));
            var fo = new EmployeeFo();
            fo.SyncDeptByJob(employee);
            //base.AfterSave(dic, formInfo, isNew);
        }
        
        public override JsonResult Delete()
        {
            string ids = Request["ListIDs"];
            string[] idArray = ids.Split(',');
            string employeeID = GetQueryString("EmployeeID");
            ComprehensiveDbContext.Set<S_HR_EmployeeJob>().Delete(c => idArray.Contains(c.ID));
            ComprehensiveDbContext.SaveChanges();

            var employee = ComprehensiveDbContext.Set<S_HR_Employee>().Find(employeeID);
            var fo = new EmployeeFo();
            fo.SyncDeptByJob(employee);

            return Json("");
        }

    }
}
