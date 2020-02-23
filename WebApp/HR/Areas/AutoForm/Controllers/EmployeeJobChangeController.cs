using HR.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using Config;
using Config.Logic;
using Workflow.Logic;
using System.Text;
using Formula.Helper;
using Formula;

namespace HR.Areas.AutoForm.Controllers
{
    public class EmployeeJobChangeController : HRFormContorllor<T_EmployeeJobChange>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            //base.BeforeSave(dic, formInfo, isNew);
            var currentListStr = dic.GetValue("CurrentJobList");
            var mainCount = 0;
            if (!string.IsNullOrEmpty(currentListStr))
            {
                var list = JsonHelper.ToList(currentListStr);
                foreach (var item in list)
                {
                    if (item.GetValue("NeedDelete") == "1") continue;
                    if (item.GetValue("IsMain") == "1") mainCount++;
                }
            }
            var changeListStr = dic.GetValue("ChangeJobList");
            if (!string.IsNullOrEmpty(changeListStr))
            {
                var list = JsonHelper.ToList(changeListStr);
                foreach (var item in list)
                    if (item.GetValue("IsMain") == "1") mainCount++;
            }
            if (mainCount != 1)
                throw new Formula.Exceptions.BusinessException("必须有且只能有一个主责部门");
        }

        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            T_Employee employee = BusinessEntities.Set<T_Employee>().Find(dic.GetValue("EmployeeID"));
            if (employee == null)
                throw new Formula.Exceptions.BusinessException("未找到员工对象");
            StringBuilder sql = new StringBuilder();
            var currentListStr = dic.GetValue("CurrentJobList");
            if (!string.IsNullOrEmpty(currentListStr))
            {
                var list = JsonHelper.ToList(currentListStr);
                foreach (var item in list)
                {
                    if (item.GetValue("NeedDelete") == "1")
                        sql.AppendLine(" delete T_EmployeeJob where id='" + item.GetValue("EmployeeJobID") + "'");
                }
            }
            var changeListStr = dic.GetValue("ChangeJobList");
            if (!string.IsNullOrEmpty(changeListStr))
            {
                var dtField = DbHelper.GetFieldTable(formInfo.ConnName, "T_EmployeeJob").AsEnumerable();
                var list = JsonHelper.ToObject<List<Dictionary<string, string>>>(changeListStr);
                foreach (var item in list)
                {
                    item.SetValue("EmployeeID", employee.ID);
                    item.RemoveWhere(a => !dtField.Select(b => b.Field<string>("FIELDCODE").ToUpper()).Contains(a.Key.ToUpper()));
                    sql.AppendLine(item.CreateInsertSql(this.HRSQLDB.DbName, "T_EmployeeJob", FormulaHelper.CreateGuid(), dtField));
                }
            }
            if (sql.ToString() != "")
                if (Config.Constant.IsOracleDb)
                {
                    this.HRSQLDB.ExecuteNonQuery(string.Format(@"
                        begin
                        {0}
                        end;", sql));
                }
                else
                {
                    this.HRSQLDB.ExecuteNonQuery(sql.ToString());
                }

            var updateEmployeeBaseDept = GetQueryString("UpdateEmployeeBaseDept");
            if (updateEmployeeBaseDept == "1")
            {
                employee.UpdateDeptByJob();
                this.BusinessEntities.SaveChanges();
            }
        }

    }
}
