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
using Formula.Helper;
using Config;


namespace Project.Areas.ProjectConfig.Controllers
{
    public class EngineeringSpaceResController : BaseConfigController<S_T_EngineeringSpaceRes>
    {

        protected override void BeforeSave(S_T_EngineeringSpaceRes entity, bool isNew)
        {
            if (isNew) {
                var parent = this.GetEntityByID<S_T_EngineeringSpaceRes>(entity.ParentID);
                if (parent == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + entity.ParentID + "】的定义节点，无法增加SpaceDefine子节点");
                parent.AddChild(entity);
            }
        }

        public JsonResult GetRoleList(QueryBuilder qb)
        {
            string roleType = this.Request["roleType"];
            if (String.IsNullOrEmpty(roleType)) roleType = Project.Logic.RoleType.ProjectRoleType.ToString();
            if (roleType == Project.Logic.RoleType.ProjectRoleType.ToString())
            {
                var list = entities.Set<S_D_RoleDefine>().WhereToGridData(qb);
                return Json(list);
            }
            else
            {
                var service = FormulaHelper.GetService<IRoleService>();
                var list = service.GetSysRoles().AsQueryable().Where(qb).ToList();
                var data = FormulaHelper.CollectionToListDic<Config.Role>(list);
                foreach (var item in data)
                {
                    item["RoleName"] = item["Name"].ToString();
                    item["RoleCode"] = item["Code"].ToString();
                    item["RoleType"] = Project.Logic.RoleType.SysRoleType.ToString();
                }
                var gridData = new GridData(data);
                return Json(gridData);
            }
        }

        public JsonResult GetSpaceTree(string SpaceID, string roleCode)
        {
            string sql = @"select S_T_EngineeringSpaceRes.*,
case when S_T_EngineeringSpaceAuth.AuthType <>'{3}' or S_T_EngineeringSpaceAuth.AuthType is null then 'False' else 'True' end as FullControlAuth,
case when S_T_EngineeringSpaceAuth.AuthType <>'{4}' or S_T_EngineeringSpaceAuth.AuthType is null then 'False' else 'True' end as CurrentFullControlAuth,
case when S_T_EngineeringSpaceAuth.AuthType <>'{2}' or S_T_EngineeringSpaceAuth.AuthType is null then 'False' else 'True' end as ViewAuth 
from S_T_EngineeringSpaceRes left join S_T_EngineeringSpaceAuth  
on S_T_EngineeringSpaceAuth.EngineeringSpaceResID=S_T_EngineeringSpaceRes.ID
and RoleCode='{1}' where S_T_EngineeringSpaceRes.SpaceID='{0}' order by SortIndex";
            sql = String.Format(sql, SpaceID, roleCode, SpaceAuthType.View.ToString(),
                SpaceAuthType.FullControl.ToString(), SpaceAuthType.CurrentFullControl.ToString());
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult RemoveAuth(string RoleCode, string SpaceDefineID, string RoleType)
        {
            var spaceDefine = this.entities.Set<S_T_EngineeringSpaceRes>().FirstOrDefault(d => d.ID == SpaceDefineID);
            if (spaceDefine == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + SpaceDefineID + "】的空间定义对象，无法移除权限");
            spaceDefine.ClearAuth(RoleCode);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SetAuth(string RoleCode, string SpaceDefineID, string RoleType, string authType = "FullControl")
        {
            var spaceDefine = this.entities.Set<S_T_EngineeringSpaceRes>().FirstOrDefault(d => d.ID == SpaceDefineID);
            if (spaceDefine == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + SpaceDefineID + "】的空间定义对象，无法设置权限");
            if (RoleType == Project.Logic.RoleType.ProjectRoleType.ToString())
                spaceDefine.SetProjectRole(RoleCode, authType);
            else
                spaceDefine.SetSysRole(RoleCode, authType);
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
