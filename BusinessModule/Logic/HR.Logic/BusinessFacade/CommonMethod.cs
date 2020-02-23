using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Formula;
using System.Data;
using System.ComponentModel;

namespace HR.Logic.BusinessFacade
{
    public class CommonMethod
    {
        public static string GetUserNamesByUserIDs(string UserIDs)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            UserIDs = "'" + UserIDs.Replace(",", "','") + "'";
            string strSql = string.Format("Select * From S_A_User Where ID in({0})", UserIDs);
            DataTable dt = sqlHelper.ExecuteDataTable(strSql);
            string UserNames = string.Join(",", dt.AsEnumerable().Select(c => c["Name"].ToString()).ToArray());
            return UserNames;
        }


        /// <summary>
        /// 根据部门ID获取部门下的用户ID
        /// </summary>
        /// <param name="orgId">部门ID</param>
        /// <returns>用户ID数组</returns>
        public static string[] GetUserIDsByOrgID(string orgId)
        {
            SQLHelper sh = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string strSql = string.Format(@"SELECT UserID FROM S_A__OrgUser WHERE OrgID='{0}'", orgId);
            DataTable dt = sh.ExecuteDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                string[] userIDs = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    userIDs[i] = dt.Rows[i]["UserID"].ToString();
                }

                return userIDs;
            }
            return new String[0];
        }

        /// <summary>
        /// 根据枚举值获取该枚举对应子枚举，子枚举分类中填写枚举值，以逗号隔开
        /// </summary>
        /// <param name="subEnumCode">子枚举编号</param>
        /// <param name="emumValue">枚举值</param>
        /// <returns></returns>
        public static List<object> GetSubEnum(string subEnumCode, string emumValue)
        {
            SQLHelper baseHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format("SELECT * FROM S_M_EnumItem WHERE EnumDefID IN (SELECT ID FROM S_M_EnumDef Where Code='{0}') AND Category like '%{1}%' Order by SortIndex", subEnumCode, emumValue);
            DataTable dt = baseHelper.ExecuteDataTable(sql);
            List<object> list = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new { text = dt.Rows[i]["Name"], value = dt.Rows[i]["Code"] });
            }
            return list;
        }
        
    }
}
