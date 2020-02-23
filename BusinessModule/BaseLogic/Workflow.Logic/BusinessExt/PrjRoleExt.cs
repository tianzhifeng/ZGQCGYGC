using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using System.Data;
using Config;
using System.Configuration;
using Formula.Exceptions;
using MvcAdapter;

namespace Workflow.Logic
{
    public class PrjRoleExt
    {
        public static string GetRoleUserIDs(string roles, string instanceID)
        {
            if (!string.IsNullOrEmpty(GetEngRoles(roles)))
                return GetEngUserIDs(roles, instanceID);
            else if (!string.IsNullOrEmpty(GetPrjRoles(roles)))
                return GetPrjRoleUserStr(FilterPrjRoleCode(roles), instanceID);
            return "";
        }

        private static string GetPrjRoleUserStr(string roleCodes, string prjID)
        {

            if (string.IsNullOrEmpty(roleCodes) || string.IsNullOrEmpty(prjID))
                return "";

            string sql = "select distinct UserID from S_W_OBSUser where ProjectInfoID='{0}' and RoleCode in('{1}') ";
            try
            {
                string majorWhere = "";
                if (prjID.Contains('|'))
                {
                    var majorValue = prjID.Split('|')[1];
                    prjID = prjID.Split('|')[0];
                    majorWhere = " and MajorValue in ('" + majorValue.Replace(",", "','") + "') ";
                }

                sql = string.Format(sql + majorWhere, prjID, roleCodes.Replace(",", "','").Replace(ConnEnum.InfrasBaseConfig.ToString(), ""));
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
                var dt = sqlHelper.ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    var idArr = dt.AsEnumerable().Select(c => c["UserID"].ToString()).ToArray();
                    return string.Join(",", idArr);
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                throw new FlowException(string.Format("sql语句执行出错:{0}", sql), ex);
            }
        }

        private static string FilterPrjRoleCode(string roleIDs)
        {
            if (string.IsNullOrEmpty(roleIDs))
                return null;

            string prjRoleCodes = "";
            Guid guid = new Guid();
            foreach (var id in roleIDs.Split(','))
            {
                if (Guid.TryParse(id, out guid)) //项目角色肯定不是Guid格式
                    continue;
                prjRoleCodes += "," + id;
            }
            return prjRoleCodes.Trim(',');
        }

        #region 流程项目选人集成

        public static DataTable GetScopeUserList(string orgIDs, string roleIDs, QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);

            string sql = @"
select dt.*,MajorName=majors.Name from (
select ID= UserID,Name=UserName,isnull(MajorValue,'') MajorValue,RoleName from S_W_OBSUser
where ProjectInfoID='{0}' {1} 
union
select ID= UserID,Name=UserName,isnull(MajorValue,'') MajorValue,RoleName from S_W_RBS 
where ProjectInfoID='{0}' {1} 
) dt 
left join {2}..S_D_WBSAttrDefine majors on dt.MajorValue=majors.Code and majors.Type='Major'
";

            string prjID = orgIDs;
            string roleCodes = roleIDs;
            if (!string.IsNullOrEmpty(roleCodes))
            {
                string projectBaseConfig = ConnEnum.InfrasBaseConfig.ToString();
                string infrastructure = ConnEnum.Infrastructure.ToString();
                roleCodes = roleCodes.Replace(projectBaseConfig, "").Replace(infrastructure, "");
            }

            if (!string.IsNullOrEmpty(roleCodes))
                roleCodes = string.Format("and RoleCode in('{0}')", roleCodes.Replace(",", "','"));

            string majorCode = "";
            if (prjID.Contains('|'))
            {
                majorCode = prjID.Split('|')[1];
                prjID = prjID.Split('|')[0];
            }
            if (!string.IsNullOrEmpty(majorCode))
            {
                majorCode = " and MajorValue in ('" + majorCode.Replace(",", "','") + "') ";
            }
            sql = string.Format(sql, prjID, majorCode + roleCodes, SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).DbName);
            var dt = sqlHelper.ExecuteDataTable(sql, qb);
            return dt;
        }

        #endregion


        #region 获取工程角色
        public static string GetEngRoles(string roleIDs)
        {
            string infrastructure = ConnEnum.Infrastructure.ToString();
            if (roleIDs.IndexOf(infrastructure) >= 0)
            {
                var engineering = Convert.ToString(System.Configuration.ConfigurationManager.ConnectionStrings["Engineering"]);
                if (!string.IsNullOrWhiteSpace(engineering))
                    return roleIDs.Replace(infrastructure, "");
            }
            return "";
        }
        #endregion

        #region 获取项目角色
        public static string GetPrjRoles(string roleIDs)
        {
            string projectBaseConfig = ConnEnum.InfrasBaseConfig.ToString();
            if (roleIDs.IndexOf(projectBaseConfig) >= 0)
            {
                var project = Convert.ToString(System.Configuration.ConfigurationManager.ConnectionStrings["Project"]);
                if (!string.IsNullOrWhiteSpace(project))
                    return roleIDs.Replace(projectBaseConfig, "");
            }
            //else
            //    return roleIDs;
            return "";
        }
        #endregion

        #region InEngRole用户是否在工程角色及角色中

        public static bool InEngRole(string userID, string roleIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            string sql = "select count(1) from S_I_OBS_User where UserID='{0}' and RoleCode in ('{1}')";
            sql = string.Format(sql, userID, roleIDs.Replace(",", "','"));
            object obj = sqlHelper.ExecuteScalar(sql);
            if (Convert.ToInt32(obj) > 0)
                return true;
            return false;
        }

        #endregion


        #region InPrjRole用户是否在工项目角色及角色中

        public static bool InPrjRole(string userID, string roleIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            string sql = "select count(1) from S_W_OBSUser where UserID='{0}' and RoleCode in ('{1}')";
            sql = string.Format(sql, userID, roleIDs.Replace(",", "','"));
            object obj = sqlHelper.ExecuteScalar(sql);
            if (Convert.ToInt32(obj) > 0)
                return true;
            return false;
        }

        #endregion

        #region 根据工程角色获取用户

        public static string GetEngUserIDs(string roleIDs, string engID)
        {

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            string sql = "select UserID from S_I_OBS_User where EngineeringInfoID='{1}' and RoleCode in ('{0}')";
            if (engID.Contains('|'))
            {
                engID = engID.Split('|')[0];
            }
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format(sql, roleIDs.Replace(",", "','").Replace(ConnEnum.Infrastructure.ToString(), ""), engID));
            if (dt.Rows.Count > 0)
            {
                var idArr = dt.AsEnumerable().Select(c => c["UserID"].ToString()).ToArray();
                return string.Join(",", idArr);
            }
            return "";
        }

        #endregion
    }
}
