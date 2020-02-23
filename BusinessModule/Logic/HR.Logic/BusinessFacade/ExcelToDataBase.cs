using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Config;
using System.Data.Entity;
using Formula;
using System.Configuration;

namespace HR.Logic.BusinessFacade
{
    public class ExcelToDataBase
    {
        #region Execl导入到数据库
        private long _totalCount = 0;
        private long _resultCount = 0;
        private string _error = "";

        /// <summary>
        /// Execl记录的总行数
        /// </summary>
        public long TotalCount
        {
            get { return _totalCount; }
        }

        /// <summary>
        /// 导入成功的记录数
        /// </summary>
        public long ResultCount
        {
            get { return _resultCount; }
        }

        public string Error
        {
            get { return _error; }
        }

        /// <summary>
        /// Execl导入到数据库
        /// </summary>
        /// <param name="Path">Execl路径</param>
        /// <param name="connEnum">Web.config配置文件中数据库信息的Name；例如：Hr</param>
        /// <param name="TableName">表的名称</param>
        /// <param name="Columns">表除去ID,CreateUserID,CreateUser,CreateDate,ModifyUserID,ModifyUser,ModifyDate之外的所有字段）,例如：C1,C2</param>
        /// <param name="Titles">Execl表头,例如:T1,T2,T3</param>
        /// <param name="CreateId">创建人Id</param>
        /// <param name="CreateName">创建人名字</param>
        /// <param name="datetimeFormatTitles">需要格式化的日期字段</param>
        /// <returns></returns>
        public DataTable Import(string Path, ConnEnum connEnum, string TableName, string Columns, string Titles, string CreateId, string CreateName, string datetimeFormatColumns, string MainTitle)
        {
            DataTable dtNew = new DataTable();
            try
            {
                DataTable dt = ReadExcel(Path, MainTitle);
                dtNew = BuildTable(connEnum, dt, TableName, Columns, Titles, CreateId, CreateName, datetimeFormatColumns);
                SqlBulkCopy SqlCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings[connEnum.ToString()].ConnectionString, SqlBulkCopyOptions.UseInternalTransaction);
                SqlCopy.SqlRowsCopied +=
                       new SqlRowsCopiedEventHandler(OnRowsCopied); //订阅复制完成后的方法,参数是 sqlbulk.NotifyAfter的值
                SqlCopy.NotifyAfter = dtNew.Rows.Count;
                SqlCopy.DestinationTableName = TableName;
                SqlCopy.WriteToServer(dtNew);
                _totalCount = dtNew.Rows.Count;
            }
            catch (Formula.Exceptions.BusinessException ex)
            {
                _error = ex.Message;
            }
            return dtNew;
        }

        private void OnRowsCopied(object sender, SqlRowsCopiedEventArgs args)
        {
            _resultCount = args.RowsCopied;
        }

        private DataTable ReadExcel(string Path, string MainTitle)
        {
            DataTable dt = new DataTable();
            string ext = Path.Split('.')[Path.Split('.').Length - 1];
            string excelStr = string.Empty;
            if (ext.ToLower() == "xls")
                excelStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + Path + "';Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";//execl 2003     
            else if (ext.ToLower() == "xlsx")
                excelStr = "Provider= Microsoft.Ace.OleDB.12.0;Data Source='" + Path + "';Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";//execl 2007以上  
            System.Data.OleDb.OleDbConnection cn = new System.Data.OleDb.OleDbConnection(excelStr);
            cn.Open();
            string SqlStr = " SELECT * FROM [sheet1$] ";

            #region  增加数据过滤条件、防止空数据被导入
            string whereStr = string.Empty;
            foreach (var title in MainTitle.Split(','))
            {
                if (string.IsNullOrEmpty(whereStr))
                    whereStr += " Where ";
                else
                    whereStr += " AND ";
                if (!string.IsNullOrEmpty(title))
                    whereStr += string.Format("  {0} IS NOT NULL ", title);
            }
            SqlStr += whereStr;
            #endregion

            System.Data.OleDb.OleDbDataAdapter dr = new System.Data.OleDb.OleDbDataAdapter(SqlStr, excelStr);
            dr.Fill(dt);
            cn.Close();
            return dt;
        }
        

        

        private DataTable BuildTable(ConnEnum connEnum, DataTable dt, string tableName, string Columns, string Titles, string CreateId, string CreateName, string datetimeFormatColumns)
        {
            DataTable dtNew = new DataTable();
            string sql = "select * from " + tableName + " where 1 = 0";
            dtNew = SQLHelper.CreateSqlHelper(connEnum).ExecuteDataTable(sql);
            string[] Col = Columns.Split(',');
            string[] Tit = Titles.Split(',');
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dtNew.NewRow();
                row["ID"] = FormulaHelper.CreateGuid();
                for (int j = 0; j < Col.Length; j++)
                {
                    var count = 0;
                    foreach (var column in datetimeFormatColumns.Split(','))
                    {
                        if (column == Col[j])
                        {
                            count++;
                            break;
                        }
                    }
                    if (count > 0)
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[i][Tit[j]].ToString()))
                            row[Col[j]] = ConvertToDateTime(dt.Rows[i][Tit[j]].ToString());
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(dt.Rows[i][Tit[j]].ToString()))
                            row[Col[j]] = dt.Rows[i][Tit[j]].ToString();
                    }
                }
                row["CreateUserID"] = CreateId;
                row["CreateUser"] = CreateName;
                row["CreateDate"] = DateTime.Now.ToString();
                row["ModifyUserID"] = CreateId;
                row["ModifyUser"] = CreateName;
                row["ModifyDate"] = DateTime.Now.ToString();
                dtNew.Rows.Add(row);
            }
            return dtNew;
        }


        /// <summary>
        /// 建表
        /// </summary>
        /// <param name="connEnum">连接字符串</param>
        /// <param name="dt">数据源</param>
        /// <param name="tableName">要插入的表名</param>
        /// <param name="titles">标题</param>
        /// <param name="createId">创建人ID</param>
        /// <param name="createName">创建人</param>
        /// <returns>要复制到数据库的表</returns>
        private DataTable buildTable(ConnEnum connEnum, DataTable dt, string tableName, string titles, string createId, string createName)
        {
            SQLHelper shBase = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            SQLHelper shProject = SQLHelper.CreateSqlHelper(connEnum);
            string sql = "select * from " + tableName + " where 1 = 0";
            DataTable dtNew = shProject.ExecuteDataTable(sql);
            string[] tit = titles.Split(',');
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                var workNo = dt.Rows[i]["工号"].ToString();
                //if (String.IsNullOrWhiteSpace(workNo))
                //{
                //    continue;
                //}

                const string sqlBase = "SELECT ID,Name FROM S_A_User WHERE WorkNo='{0}'";
                var dtUser = shBase.ExecuteDataTable(String.Format(sqlBase, workNo));
                object userId;
                object userName;
                if (dtUser != null && dtUser.Rows.Count > 0)
                {
                    var curRow = dtUser.Rows[0];
                    userId = curRow["ID"];
                    userName = curRow["Name"];
                }
                else
                {
                    continue;
                }

                foreach (string t in tit)
                {
                    string qualifyType = "";
                    switch (t)
                    {
                        case "项目负责人":
                            if (!String.IsNullOrWhiteSpace(dt.Rows[i]["项目负责人"].ToString()))
                            {
                                qualifyType = "ProjectManager";
                            }
                            break;
                        case "专业负责人":
                            if (!String.IsNullOrWhiteSpace(dt.Rows[i]["专业负责人"].ToString()))
                            {
                                qualifyType = "MajorPrinciple";
                            }
                            break;
                        case "审定人":
                            if (!String.IsNullOrWhiteSpace(dt.Rows[i]["审定人"].ToString()))
                            {
                                qualifyType = "Approver";
                            }
                            break;
                        case "审核人":
                            if (!String.IsNullOrWhiteSpace(dt.Rows[i]["审核人"].ToString()))
                            {
                                qualifyType = "Auditor";
                            }
                            break;
                        case "校对人":
                            if (!String.IsNullOrWhiteSpace(dt.Rows[i]["校对人"].ToString()))
                            {
                                qualifyType = "Collactor";
                            }
                            break;
                        case "设计人":
                            if (!String.IsNullOrWhiteSpace(dt.Rows[i]["设计人"].ToString()))
                            {
                                qualifyType = "Designer";
                            }
                            break;
                        default:
                            continue;
                    }

                    if (qualifyType != "")
                    {
                        const string sqlOnly = "SELECT COUNT(ID) FROM S_Z_PROJECTUSERQUALIFY WHERE USERID = '{0}' AND QUALIFYTYPE='{1}'";
                        object obj = shProject.ExecuteScalar(String.Format(sqlOnly, userId, qualifyType));

                        if (obj != DBNull.Value && Convert.ToInt32(obj) == 0)
                        {
                            var flag = true;
                            for (int j = 0; j < dtNew.Rows.Count; j++)
                            {
                                var curRow = dtNew.Rows[j];
                                if (curRow["USERID"].ToString() == userId.ToString() && curRow["QUALIFYTYPE"].ToString() == qualifyType)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                DataRow row = dtNew.NewRow();
                                row["ID"] = FormulaHelper.CreateGuid();
                                row["USERID"] = userId;
                                row["USERNAME"] = userName;
                                row["QUALIFYTYPE"] = qualifyType;
                                row["CREATEUSERID"] = createId;
                                row["CREATEUSER"] = createName;
                                row["CREATEDATE"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                                row["SORTINDEX"] = 0;
                                row["ISVALIDATE"] = "是";
                                dtNew.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            return dtNew;
        }


        public DateTime ConvertToDateTime(object obj)
        {
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        #endregion
    }
}
