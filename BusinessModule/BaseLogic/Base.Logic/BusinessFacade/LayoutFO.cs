using Formula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using Config;
using Formula.Helper;

namespace Base.Logic
{
    public static class LayoutFO
    {
        private static DataTable GetFieldTable(string connName, string tableName)
        {
            string sql = string.Format(@"
SELECT syscolumns.name FieldCode,systypes.name Type,syscolumns.isnullable Nullable,
syscolumns.length
FROM syscolumns, systypes
WHERE syscolumns.xusertype = systypes.xusertype
AND syscolumns.id =
object_id('{0}')
", tableName);

            if (Config.Constant.IsOracleDb)
            {
                sql = string.Format("SELECT COLUMN_NAME FieldCode,DATA_TYPE Type,Nullable,DATA_LENGTH length FROM USER_TAB_COLUMNS WHERE TABLE_NAME = upper('{0}')", tableName);
            }


            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            return dt;
        }

        public static string CreateInsertSql(this Dictionary<string, string> dic, string connName, string tableName, string ID)
        {
            if (Config.Constant.IsOracleDb)
            {
                var dt = GetFieldTable(connName, tableName).AsEnumerable();
                var fields = dt.Select(c => c[0].ToString()).ToArray();


                StringBuilder sbField = new StringBuilder();
                StringBuilder sbValue = new StringBuilder();

                foreach (string key in dic.Keys)
                {
                    if (key == "ID")
                        continue;
                    if (!fields.Contains(key))
                        continue;

                    string value = dic[key];
                    value = GetValue(dt, key, value);

                    sbField.AppendFormat(",{0}", key);
                    sbValue.AppendFormat(",{0}", value);
                }

                var user = FormulaHelper.GetUserInfo();
                if (fields.Contains("CREATETIME") && !dic.Keys.Contains("CREATETIME"))
                {
                    sbField.AppendFormat(",{0}", "CREATETIME");
                    sbValue.AppendFormat(",to_date('{0}','yyyy-MM-dd hh24:mi:ss')", DateTime.Now);
                }
                if (fields.Contains("CREATEDATE") && !dic.Keys.Contains("CREATEDATE"))
                {
                    sbField.AppendFormat(",{0}", "CREATEDATE");
                    sbValue.AppendFormat(",to_date('{0}','yyyy-MM-dd hh24:mi:ss')", DateTime.Now);
                }
                if (fields.Contains("CREATEUSERID") && !dic.Keys.Contains("CREATEUSERID"))
                {
                    sbField.AppendFormat(",{0}", "CREATEUSERID");
                    sbValue.AppendFormat(",'{0}'", user.UserID);
                }
                if (fields.Contains("CREATEUSERNAME") && !dic.Keys.Contains("CREATEUSERNAME"))
                {
                    sbField.AppendFormat(",{0}", "CREATEUSERNAME");
                    sbValue.AppendFormat(",'{0}'", user.UserName);
                }
                if (fields.Contains("CREATEUSER") && !dic.Keys.Contains("CREATEUSER"))
                {
                    sbField.AppendFormat(",{0}", "CREATEUSER");
                    sbValue.AppendFormat(",'{0}'", user.UserName);
                }
                if (fields.Contains("ORGID") && !dic.Keys.Contains("ORGID"))
                {
                    sbField.AppendFormat(",{0}", "ORGID");
                    sbValue.AppendFormat(",'{0}'", user.UserOrgID);
                }
                if (fields.Contains("COMPANYID") && !dic.Keys.Contains("COMPANYID"))
                {
                    sbField.AppendFormat(",{0}", "COMPANYID");
                    sbValue.AppendFormat(",'{0}'", user.UserCompanyID);
                }

                if (fields.Contains("FLOWPHASE") && !dic.Keys.Contains("FLOWPHASE"))
                {
                    sbField.AppendFormat(",{0}", "FLOWPHASE");
                    sbValue.AppendFormat(",'{0}'", "Start");
                }

                if (fields.Contains("MODIFYTIME") && !dic.Keys.Contains("MODIFYTIME"))
                {
                    sbField.AppendFormat(",{0}", "MODIFYTIME");
                    sbValue.AppendFormat(",to_date('{0}','yyyy-MM-dd hh24:mi:ss')", DateTime.Now);
                }
                if (fields.Contains("MODIFYDATE") && !dic.Keys.Contains("MODIFYDATE"))
                {
                    sbField.AppendFormat(",{0}", "MODIFYDATE");
                    sbValue.AppendFormat(",to_date('{0}','yyyy-MM-dd hh24:mi:ss')", DateTime.Now);
                }
                if (fields.Contains("MODIFYUSERID") && !dic.Keys.Contains("MODIFYUSERID"))
                {
                    sbField.AppendFormat(",{0}", "MODIFYUSERID");
                    sbValue.AppendFormat(",'{0}'", user.UserID);
                }
                if (fields.Contains("MODIFYUSERNAME") && !dic.Keys.Contains("MODIFYUSERNAME"))
                {
                    sbField.AppendFormat(",{0}", "MODIFYUSERNAME");
                    sbValue.AppendFormat(",'{0}'", user.UserName);
                }
                if (fields.Contains("MODIFYUSER") && !dic.Keys.Contains("MODIFYUSER"))
                {
                    sbField.AppendFormat(",{0}", "MODIFYUSER");
                    sbValue.AppendFormat(",'{0}'", user.UserName);
                }

                string sql = string.Format(@"INSERT INTO {0} (ID{2}) VALUES ('{1}'{3});", tableName, ID, sbField, sbValue);

                return sql;
            }
            else
            {
                var dt = GetFieldTable(connName, tableName).AsEnumerable();
                var fields = dt.Select(c => c[0].ToString()).ToArray();


                StringBuilder sbField = new StringBuilder();
                StringBuilder sbValue = new StringBuilder();

                foreach (string key in dic.Keys)
                {
                    if (key == "ID")
                        continue;
                    if (!fields.Contains(key))
                        continue;

                    string value = dic[key];
                    value = GetValue(dt, key, value);

                    sbField.AppendFormat(",{0}", key);
                    sbValue.AppendFormat(",{0}", value);
                }

                var user = FormulaHelper.GetUserInfo();
                if (fields.Contains("CreateTime") && !dic.Keys.Contains("CreateTime"))
                {
                    sbField.AppendFormat(",{0}", "CreateTime");
                    sbValue.AppendFormat(",'{0}'", DateTime.Now);
                }
                if (fields.Contains("CreateDate") && !dic.Keys.Contains("CreateDate"))
                {
                    sbField.AppendFormat(",{0}", "CreateDate");
                    sbValue.AppendFormat(",'{0}'", DateTime.Now);
                }
                if (fields.Contains("CreateUserID") && !dic.Keys.Contains("CreateUserID"))
                {
                    sbField.AppendFormat(",{0}", "CreateUserID");
                    sbValue.AppendFormat(",'{0}'", user.UserID);
                }
                if (fields.Contains("CreateUserName") && !dic.Keys.Contains("CreateUserName"))
                {
                    sbField.AppendFormat(",{0}", "CreateUserName");
                    sbValue.AppendFormat(",'{0}'", user.UserName);
                }
                if (fields.Contains("CreateUser") && !dic.Keys.Contains("CreateUser"))
                {
                    sbField.AppendFormat(",{0}", "CreateUser");
                    sbValue.AppendFormat(",'{0}'", user.UserName);
                }
                if (fields.Contains("OrgID") && !dic.Keys.Contains("OrgID"))
                {
                    sbField.AppendFormat(",{0}", "OrgID");
                    sbValue.AppendFormat(",'{0}'", user.UserOrgID);
                }
                if (fields.Contains("CompanyID") && !dic.Keys.Contains("CompanyID"))
                {
                    sbField.AppendFormat(",{0}", "CompanyID");
                    sbValue.AppendFormat(",'{0}'", user.UserCompanyID);
                }

                if (fields.Contains("FlowPhase") && !dic.Keys.Contains("FlowPhase"))
                {
                    sbField.AppendFormat(",{0}", "FlowPhase");
                    sbValue.AppendFormat(",'{0}'", "Start");
                }

                if (fields.Contains("ModifyTime") && !dic.Keys.Contains("ModifyTime"))
                {
                    sbField.AppendFormat(",{0}", "ModifyTime");
                    sbValue.AppendFormat(",'{0}'", DateTime.Now);
                }
                if (fields.Contains("ModifyDate") && !dic.Keys.Contains("ModifyDate"))
                {
                    sbField.AppendFormat(",{0}", "ModifyDate");
                    sbValue.AppendFormat(",'{0}'", DateTime.Now);
                }
                if (fields.Contains("ModifyUserID") && !dic.Keys.Contains("ModifyUserID"))
                {
                    sbField.AppendFormat(",{0}", "ModifyUserID");
                    sbValue.AppendFormat(",'{0}'", user.UserID);
                }
                if (fields.Contains("ModifyUserName") && !dic.Keys.Contains("ModifyUserName"))
                {
                    sbField.AppendFormat(",{0}", "ModifyUserName");
                    sbValue.AppendFormat(",'{0}'", user.UserName);
                }
                if (fields.Contains("ModifyUser") && !dic.Keys.Contains("ModifyUser"))
                {
                    sbField.AppendFormat(",{0}", "ModifyUser");
                    sbValue.AppendFormat(",'{0}'", user.UserName);
                }

                string sql = string.Format(@"INSERT INTO {0} (ID{2}) VALUES ('{1}'{3})", tableName, ID, sbField, sbValue);

                return sql;
            }
        }

        public static string CreateUpdateSql(this Dictionary<string, string> dic, Dictionary<string, string> currentDic, string connName, string tableName, string ID)
        {
            if (Config.Constant.IsOracleDb)
            {
                var dt = GetFieldTable(connName, tableName).AsEnumerable();
                var fields = dt.Select(c => c[0].ToString()).ToArray();

                StringBuilder sb = new StringBuilder();
                foreach (string key in dic.Keys)
                {
                    if (key == "ID")
                        continue;
                    if (!fields.Contains(key) && !fields.Contains(key.ToUpper()))
                        continue;
                    if (key == "SERIALNUMBER")//流水号不能修改
                        continue;
                    sb.AppendFormat(",{0}={1}", key, GetValue(dt, key, dic[key]));
                }

                if (sb.ToString().Trim() == "")
                    return "";

                var user = FormulaHelper.GetUserInfo();
                if (fields.Contains("MODIFYTIME") && !dic.Keys.Contains("MODIFYTIME"))
                {
                    sb.AppendFormat(",MODIFYTIME=to_date('{0}','yyyy-MM-dd hh24:mi:ss')", DateTime.Now);
                }
                if (fields.Contains("MODIFYDATE") && !dic.Keys.Contains("MODIFYDATE"))
                {
                    sb.AppendFormat(",MODIFYDATE=to_date('{0}','yyyy-MM-dd hh24:mi:ss')", DateTime.Now);
                }
                if (fields.Contains("MODIFYUSERID") && !dic.Keys.Contains("MODIFYUSERID"))
                {
                    sb.AppendFormat(",MODIFYUSERID='{0}'", user.UserID);
                }
                if (fields.Contains("MODIFYUSERNAME") && !dic.Keys.Contains("MODIFYUSERNAME"))
                {
                    sb.AppendFormat(",MODIFYUSERNAME='{0}'", user.UserName);
                }
                if (fields.Contains("MODIFYUSER") && !dic.Keys.Contains("MODIFYUSER"))
                {
                    sb.AppendFormat(",MODIFYUSER='{0}'", user.UserName);
                }
                string sql = string.Format(@"UPDATE {0} SET {2} WHERE ID='{1}';", tableName, ID, sb.ToString().Trim(','));
                return sql;
            }
            else
            {
                var dt = GetFieldTable(connName, tableName).AsEnumerable();
                var fields = dt.Select(c => c[0].ToString()).ToArray();

                StringBuilder sb = new StringBuilder();
                foreach (string key in dic.Keys)
                {
                    if (key == "ID")
                        continue;
                    if (!fields.Contains(key))
                        continue;
                    if (key == "SerialNumber")//流水号不能修改
                        continue;
                    sb.AppendFormat(",{0}={1}", key, GetValue(dt, key, dic[key]));
                }

                if (sb.ToString().Trim() == "")
                    return "";

                var user = FormulaHelper.GetUserInfo();
                if (fields.Contains("ModifyTime") && !dic.Keys.Contains("ModifyTime"))
                {
                    sb.AppendFormat(",ModifyTime='{0}'", DateTime.Now);
                }
                if (fields.Contains("ModifyDate") && !dic.Keys.Contains("ModifyDate"))
                {
                    sb.AppendFormat(",ModifyDate='{0}'", DateTime.Now);
                }
                if (fields.Contains("ModifyUserID") && !dic.Keys.Contains("ModifyUserID"))
                {
                    sb.AppendFormat(",ModifyUserID='{0}'", user.UserID);
                }
                if (fields.Contains("ModifyUserName") && !dic.Keys.Contains("ModifyUserName"))
                {
                    sb.AppendFormat(",ModifyUserName='{0}'", user.UserName);
                }
                if (fields.Contains("ModifyUser") && !dic.Keys.Contains("ModifyUser"))
                {
                    sb.AppendFormat(",ModifyUser='{0}'", user.UserName);
                }
                string sql = string.Format(@"UPDATE {0} SET {2} WHERE ID='{1}'", tableName, ID, sb.ToString().Trim(','));
                return sql;
            }
        }

        private static bool CompareDicItem(Dictionary<string, string> dic1, Dictionary<string, string> dic2, string key)
        {
            if (dic1.ContainsKey(key) && dic2.ContainsKey(key))
            {
                return dic1[key] == dic2[key];
            }
            else if (!dic1.ContainsKey(key) && !dic2.ContainsKey(key))
            {
                return true;
            }
            else
                return false;
        }

        public static string CreateUpdateSql(this Dictionary<string, string> dic, string connName, string tableName, string ID)
        {
            if (Config.Constant.IsOracleDb)
            {
                var dt = GetFieldTable(connName, tableName).AsEnumerable();
                var fields = dt.Select(c => c[0].ToString()).ToArray();

                StringBuilder sb = new StringBuilder();
                foreach (string key in dic.Keys)
                {
                    if (key == "ID")
                        continue;
                    if (!fields.Contains(key) && !fields.Contains(key.ToUpper()))
                        continue;
                    if (key == "SERIALNUMBER")//流水号不能修改
                        continue;

                    sb.AppendFormat(",{0}={1}", key, GetValue(dt, key, dic[key]));
                }

                if (sb.ToString().Trim() == "")
                    return "";
                string sql = string.Format(@"UPDATE {0} SET {2} WHERE ID='{1}';", tableName, ID, sb.ToString().Trim(','));
                return sql;
            }
            else
            {
                var dt = GetFieldTable(connName, tableName).AsEnumerable();
                var fields = dt.Select(c => c[0].ToString()).ToArray();

                StringBuilder sb = new StringBuilder();
                foreach (string key in dic.Keys)
                {
                    if (key == "ID")
                        continue;
                    if (!fields.Contains(key))
                        continue;
                    if (key == "SerialNumber")//流水号不能修改
                        continue;

                    sb.AppendFormat(",{0}={1}", key, GetValue(dt, key, dic[key]));
                }

                if (sb.ToString().Trim() == "")
                    return "";
                string sql = string.Format(@"UPDATE {0} SET {2} WHERE ID='{1}';", tableName, ID, sb.ToString().Trim(','));
                return sql;
            }
        }

        private static string GetValue(EnumerableRowCollection<DataRow> fieldRows, string fieldCode, string value)
        {
            if (Config.Constant.IsOracleDb)
            {
                var field = fieldRows.SingleOrDefault(c => c.Field<string>("FIELDCODE").ToUpper() == fieldCode.ToUpper());
                if (string.IsNullOrEmpty(value))
                {
                    if (field["TYPE"].ToString() != "NVARCHAR2" && field["NULLABLE"].ToString() == "1")
                        value = "NULL";
                    else
                        value = "''";
                }
                else
                {
                    if (field["TYPE"].ToString().Contains("TIMESTAMP"))
                        value = string.Format("to_date('{0}','yyyy-MM-dd hh24:mi:ss')", DateTime.Parse(value));
                    else
                        value = "'" + value + "'";
                }

                return value;
            }
            else
            {
                if (string.IsNullOrEmpty(value))
                {
                    var field = fieldRows.SingleOrDefault(c => c.Field<string>("FieldCode") == fieldCode);
                    if (field["Type"].ToString() != "nvarchar" && field["Nullable"].ToString() == "1")
                        value = "NULL";
                    else
                        value = "''";
                }
                else
                {
                    value = "'" + value + "'";
                }

                return value;
            }
        }
    }
}
