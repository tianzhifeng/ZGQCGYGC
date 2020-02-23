using System;
using System.Linq;
using Config;
using Formula;
using OfficeAuto.Logic.Domain;
using System.Collections.Generic;
using Config.Logic;

namespace OfficeAuto.Logic
{
    public class DocumentCabinetsFO
    {
        static OfficeAutoEntities instanceEnitites = FormulaHelper.GetEntities<OfficeAutoEntities>();
        /// <summary>
        /// 获取节点的最高权限
        /// </summary>
        /// <param name="documentInfoId">节点ID</param>
        /// <returns>最高权限</returns>
        public static string GetHighestAuthorityString(string documentInfoId)
        {
            var user = FormulaHelper.GetUserInfo();
            var entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
            const string sql = "SELECT RoleID AS RoleCode FROM S_A__OrgRoleUser WHERE UserID='{0}' UNION SELECT OrgID AS RoleCode FROM S_A__OrgUser WHERE UserID='{0}' UNION SELECT RoleID AS RoleCode FROM S_A__RoleUser WHERE UserID='{0}'";
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Base).ExecuteDataTable(String.Format(sql, user.UserID));
            var roleCodeStr = user.UserID;
            if (dt.Rows.Count > 0)
            {
                var roleArgs = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    roleArgs[i] = dt.Rows[i]["RoleCode"].ToString();
                }
                roleCodeStr += "," + String.Join(",", roleArgs);
            }

            var highestAuth = DocumentCabinetsAuthType.None.ToString();
            var authorityList =
                entities.Set<S_F_DocumentFileAuthority>().Where(i => i.DocumentInfoID == documentInfoId && roleCodeStr.Contains(i.RoleCode)).ToList();
            if (authorityList.Any(i => i.AuthType == DocumentCabinetsAuthType.FullControl.ToString()))
            {
                highestAuth = DocumentCabinetsAuthType.FullControl.ToString();
                return highestAuth;
            }

            if (authorityList.Any(i => i.AuthType == DocumentCabinetsAuthType.CanWrite.ToString()))
            {
                highestAuth = DocumentCabinetsAuthType.CanWrite.ToString();
                return highestAuth;
            }

            if (authorityList.Any(i => i.AuthType == DocumentCabinetsAuthType.ReadOnly.ToString()))
            {
                highestAuth = DocumentCabinetsAuthType.ReadOnly.ToString();
                return highestAuth;
            }

            return highestAuth;
        }

        /// <summary>
        /// 继承权限
        /// </summary>
        /// <param name="documentInfoId">被继承权限的节点ID</param>
        public static void InheritAuthority(string documentInfoId)
        {
            var entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
            var list = entities.Set<S_F_DocumentInfo>().Where(i => i.ParentID == documentInfoId && i.IsInherit == 1).ToList();
            foreach (S_F_DocumentInfo documentInfo in list)
            {
                S_F_DocumentInfo info = documentInfo;
                //删除原有权限设置
                entities.Set<S_F_DocumentFileAuthority>().Delete(i => i.DocumentInfoID == info.ID);
                entities.SaveChanges();

                //设置继承的权限
                var listAuth = entities.Set<S_F_DocumentFileAuthority>().Where(i => i.DocumentInfoID == documentInfoId);
                foreach (S_F_DocumentFileAuthority authority in listAuth)
                {
                    var newItem = new S_F_DocumentFileAuthority
                    {
                        ID = FormulaHelper.CreateGuid(),
                        DocumentInfoID = info.ID,
                        AuthType = authority.AuthType,
                        RoleType = authority.RoleType,
                        RoleCode = authority.RoleCode,
                        IsParentAuth = 1
                    };
                    entities.Set<S_F_DocumentFileAuthority>().Add(newItem);
                }
                entities.SaveChanges();

                //递归处理子节点
                InheritAuthority(info.ID);
            }
        }
    }
}
