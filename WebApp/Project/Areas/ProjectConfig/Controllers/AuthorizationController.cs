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
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Formula.Helper;
using Base.Logic.Domain;


namespace Project.Areas.ProjectConfig.Controllers
{
    public class AuthorizationController : BaseConfigController<S_T_SpaceAuth>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            string roleType = this.Request["roleType"];
            if (String.IsNullOrEmpty(roleType)) roleType = Project.Logic.RoleType.ProjectRoleType.ToString();
            if (roleType == Project.Logic.RoleType.ProjectRoleType.ToString())
            {
                var sql = "select * from S_D_RoleDefine";
                var db = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
                var list = db.ExecuteGridData(sql, qb);
                return Json(list);
            }
            else
            {
                var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
                qb.Add("Type", QueryMethod.Equal, roleType.Replace("Type", ""));
                var sql = "select * from (select *,Code as RoleCode,Name as RoleName,'" + roleType + "' as RoleType from S_A_Role) tmp";
                var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                var list = db.ExecuteGridData(sql, qb);
                return Json(list);
            }
        }

        public JsonResult GetSpaceList(string ModeID, string roleCode)
        {
            string sql = @"select S_T_SpaceDefine.*,
case when S_T_SpaceAuth.ID is null then 'False' else 'True' end as HasAuth
from S_T_SpaceDefine left join S_T_SpaceAuth  on S_T_SpaceAuth.SpaceID=S_T_SpaceDefine.ID
and RoleCode='{1}' where Type='Root' and ModeID='{0}'";
            var sqldb = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            var dt = sqldb.ExecuteDataTable(String.Format(sql, ModeID, roleCode));
            var data = new GridData(dt);
            return Json(data);
        }

        public JsonResult GetSpaceTree(string RootID,string roleCode)
        {
            string sql = @"select S_T_SpaceDefine.*,
case when S_T_SpaceAuth.AuthType <>'{3}' or S_T_SpaceAuth.AuthType is null then 'False' else 'True' end as FullControlAuth,
case when S_T_SpaceAuth.AuthType <>'{4}' or S_T_SpaceAuth.AuthType is null then 'False' else 'True' end as CurrentFullControlAuth,
case when S_T_SpaceAuth.AuthType <>'{2}' or S_T_SpaceAuth.AuthType is null then 'False' else 'True' end as ViewAuth 
from S_T_SpaceDefine left join S_T_SpaceAuth  
on S_T_SpaceAuth.SpaceID=S_T_SpaceDefine.ID
and RoleCode='{1}' where FullID like '{0}%' order by SortIndex";
            sql = String.Format(sql, RootID, roleCode, SpaceAuthType.View.ToString(), 
                SpaceAuthType.FullControl.ToString(),SpaceAuthType.CurrentFullControl.ToString());
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult RemoveAuth(string RoleCode, string SpaceDefineID, string RoleType)
        {
            var spaceDefine = this.entities.Set<S_T_SpaceDefine>().FirstOrDefault(d => d.ID == SpaceDefineID);
            if (spaceDefine == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + SpaceDefineID + "】的空间定义对象，无法移除权限");
            spaceDefine.ClearAuth(RoleCode);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SetAuth(string RoleCode, string SpaceDefineID, string RoleType,string authType="FullControl")
        {
            var spaceDefine = this.entities.Set<S_T_SpaceDefine>().FirstOrDefault(d => d.ID == SpaceDefineID);
            if (spaceDefine == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + SpaceDefineID + "】的空间定义对象，无法设置权限");
            if (RoleType == Project.Logic.RoleType.ProjectRoleType.ToString())
                spaceDefine.SetProjectRole(RoleCode, authType);
            else
                spaceDefine.SetSysRole(RoleCode, RoleType, authType);
            this.entities.SaveChanges();
            return Json("");
        }

    }
}
