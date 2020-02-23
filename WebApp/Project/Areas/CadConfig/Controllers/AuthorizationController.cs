
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Project.Logic.Domain;
using MvcAdapter;
using Config;
using Config.Logic;
using Formula;
using Base.Logic.Domain;
using Project.Logic;
using Formula.Helper;
using System.Data;

namespace Project.Areas.CadConfig.Controllers
{
    public class AuthorizationController : BaseConfigController<S_CAD_AreaAuth>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            qb.PageSize = 0;
            var sql = @"select * from (select r.SortIndex,r.id,r.RoleCode,r.RoleName,r.RoleType,a.HasCRUDAuth,a.HasPrintAuth,
(case when a.AuthType is null or a.AuthType ='' or a.AuthType='User' then 'True' else 'False' end) [User],
(case when a.AuthType='Major' then 'True' else 'False' end) Major,
(case when a.AuthType='Project' then 'True' else 'False' end) Project from S_D_RoleDefine r
left join S_CAD_AreaAuth a on r.RoleCode=a.RoleCode) dt";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            var grid = db.ExecuteGridData(sql, qb);
            return Json(grid);
        }

        public JsonResult SetAuth(string Data)
        {
            var dic = JsonHelper.ToObject(Data);
            var RoleCode = dic.GetValue("RoleCode");
            var auth = this.entities.Set<S_CAD_AreaAuth>().FirstOrDefault(a => a.RoleCode == RoleCode);
            if (auth == null)
            {
                auth = new S_CAD_AreaAuth();
                auth.ID = FormulaHelper.CreateGuid();
                auth.RoleCode = RoleCode;
                auth.RoleType = Project.Logic.RoleType.ProjectRoleType.ToString();
                this.entities.Set<S_CAD_AreaAuth>().Add(auth);
            }
            auth.AuthType = CADAreaAuthType.User.ToString();
            if (dic.GetValue("Project")=="True")
                auth.AuthType = CADAreaAuthType.Project.ToString();
            else if (dic.GetValue("Major") == "True")
                auth.AuthType = CADAreaAuthType.Major.ToString();
            auth.HasCRUDAuth = dic.GetValue("HasCRUDAuth");
            auth.HasPrintAuth = dic.GetValue("HasPrintAuth");
            this.entities.SaveChanges();
            return Json("");
        }

    }
}
