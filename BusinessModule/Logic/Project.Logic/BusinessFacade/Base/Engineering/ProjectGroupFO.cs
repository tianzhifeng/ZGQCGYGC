using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Entity;
using System.Collections;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Config;
using Config.Logic;
using Project.Logic.Domain;

namespace Project.Logic
{
    public partial class ProjectGroupFO
    {
        ProjectEntities instanceEnitites = FormulaHelper.GetEntities<ProjectEntities>();
        BaseConfigEntities configEntities = FormulaHelper.GetEntities<BaseConfigEntities>();

        /// <summary>
        /// 获得工程的菜单入口
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="GroupID">工程ID</param>
        /// <returns>入口集合</returns>
        public List<S_T_EngineeringSpaceRes> GetSpaceDefine(string userID, string GroupID)
        {
            var userService = FormulaHelper.GetService<IUserService>();
            var group = instanceEnitites.S_I_ProjectGroup.FirstOrDefault(d => d.ID == GroupID);
            if (group == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + GroupID + "】的项目信息对象，无法获取空间入口信息");
            if (group.Type != Project.Logic.Domain.EngineeringSpaceType.Engineering.ToString())
                throw new Formula.Exceptions.BusinessException("只有工程节点才能获取功能菜单");
            if (group.EngineeringSpace == null) throw new Formula.Exceptions.BusinessException("项目【" + group.Name + "】的管理模式对象为空，请为项目指定管理模式");

            var roles = userService.GetRoleCodesForUser(userID, string.Empty);
            var roleList = group.GetUserOBSTable(userID);
            foreach (DataRow item in roleList.Rows)
            {
                roles += "," + item["RoleCode"].ToString();
            }
            var result = group.GetSpaceDefineByRoleKeys(roles.TrimStart(','));
            return result.OrderBy(d => d.SortIndex).ToList();
        }


        /// <summary>
        /// 设置用户默认工程（最多设置10个）
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="projectInfoID">项目ID</param>
        public void SetUserProjecGrouptList(string userID, string GroupID)
        {
            var group = instanceEnitites.S_I_ProjectGroup.Find(GroupID);
            if (group == null) return;

            var sb = new StringBuilder();
            var deleteSql = @"
delete from S_I_UserDefaultProjectInfo where UserID='" + userID + "' and EngineeringInfoID='" + GroupID + @"' and ProjectInfoID='' 
delete from S_I_UserDefaultProjectInfo
where UserID='" + userID + @"' 
and ID not in (
select top 9 ID from S_I_UserDefaultProjectInfo
where UserID='" + userID + "' order by ID desc)";
            sb.AppendLine(deleteSql);
            var insertSql = string.Format(@"insert into S_I_UserDefaultProjectInfo (ID,UserID,ProjectInfoID,EngineeringInfoID) 
                Values('{0}','{1}','{2}','{3}')", FormulaHelper.CreateGuid(), userID, "", GroupID);
            sb.AppendLine(insertSql);
            SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteNonQuery(sb.ToString());

        }

    }
}
