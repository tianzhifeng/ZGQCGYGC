using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Logic.Domain;
using Formula.Exceptions;
using Config;
using Config.Logic;
using Formula;
namespace HR.Areas.AutoForm
{
    public class ForeignGoabroadapplyController : HRFormContorllor<T_Foreign_Goabroadapplybusiness>
    {
        //
        // GET: /AutoForm/ForeignGoabroadapply/

        protected override void AfterGetData(System.Data.DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                var row = dt.Rows[0];
                var ApplyUser = row["ApplyUser"].ToString();
                var ApplyDept = row["ApplyDept"].ToString();
                var BasesqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                var sql = "select a.*,b.UserID as Principal from  S_A_User a inner join S_A__OrgRoleUser b on a.DeptID=b.orgID inner join S_A_Role c on c.ID=b.RoleID where c.Code='部门正职' and  a.DeptID='" + ApplyDept + "'";
                var Userdt = BasesqlDB.ExecuteDataTable(sql);
                if (Userdt.Rows.Count > 0)
                {
                    var Principal = dt.Rows[0]["Principal"].ToString();
                    row["OwnPrincipal"] = Principal;
                }
            }
        }

        public JsonResult GetUserInfo()
        {
            var UserID = GetQueryString("UserID");
            var ApplyDept = GetQueryString("DeptID");
            var dic = new Dictionary<string, string>();
            var BasesqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var sql = "select a.*,b.UserID as Principal from  S_A_User a inner join S_A__OrgRoleUser b on a.DeptID=b.orgID inner join S_A_Role c on c.ID=b.RoleID where c.Code='部门正职' and  a.DeptID='" + ApplyDept + "'";
            var dt = BasesqlDB.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                var Principal = dt.Rows[0]["Principal"].ToString();
                dic.SetValue("OwnPrincipal", Principal);
            }
            return Json(dic);
        }
        protected override void OnFlowEnd(T_Foreign_Goabroadapplybusiness entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var list = entity.T_Foreign_Goabroadapplybusiness_Peerlist.ToList();

            var mod = new S_E_Peerlist();
            mod.ID = FormulaHelper.CreateGuid();
            mod.ApplyDate = entity.Applytime;
            mod.ApplyType = "因公出国";
            mod.T_Foreign_GoabroadapplybusinessID = entity.ID;
            mod.TaskName = entity.Taskname;
            mod.UserID = entity.ApplyUser;
            mod.UserName = entity.ApplyUserName;
            mod.DeptID = entity.Applydept;
            mod.DeptName = entity.ApplydeptName;
            mod.Code = FormulaHelper.GetUserInfoByID(entity.ApplyUser).Code;
            mod.Gocountry = entity.Gocountry;
            BusinessEntities.Set<S_E_Peerlist>().Add(mod);

            foreach (var item in list)
            {
                mod = new S_E_Peerlist();
                mod.ID = FormulaHelper.CreateGuid();
                mod.ApplyDate = entity.Applytime;
                mod.ApplyType = "因公出国";
                mod.T_Foreign_GoabroadapplybusinessID = entity.ID;
                mod.TaskName = entity.Taskname;
                mod.UserID = item.UserID;
                mod.UserName = item.Name;
                mod.DeptID = item.Dept;
                mod.DeptName = item.DeptName;
                mod.Code = item.Workid;
                mod.Gocountry = entity.Gocountry;
                BusinessEntities.Set<S_E_Peerlist>().Add(mod);
            }
            BusinessEntities.SaveChanges();

        }
    }
}
