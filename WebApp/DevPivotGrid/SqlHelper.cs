using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Reflection;


namespace CommonLibrary
{
    /// <summary>
    /// 数据库操作工具类
    /// </summary>
    public class SqlHelper
    {
        #region 静态方法

        public static T RowToObj<T>(DataRow row, T obj = null) where T : class,new()
        {
            if (obj == null)
                obj = new T();

            foreach (var pi in typeof(T).GetProperties())
            {
                if (!row.Table.Columns.Contains(pi.Name))
                    continue;
                if (row[pi.Name] != System.DBNull.Value)
                    pi.SetValue(obj, row[pi.Name], null);
            }

            return obj;
        }

        public static List<T> TableToList<T>(DataTable dt) where T : class,new()
        {
            List<T> list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(RowToObj<T>(row));
            }
            return list;
        }

        #endregion

        #region 扩展

        #region 静态构造方法

        private static Dictionary<string, object> _dic = new Dictionary<string, object>();
        public static SqlHelper CreateSqlHelper(string connName)
        {
            string key = string.Format("Conn_{0}", connName);

            SqlHelper sqlHelper = null;
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items[key] != null)
                    return HttpContext.Current.Items[key] as SqlHelper;

                if (System.Configuration.ConfigurationManager.ConnectionStrings[connName] == null)
                    throw new Exception(string.Format("配置文件中不包含数据库连接字符串：{0}", connName));

                sqlHelper = new SqlHelper(ConfigurationManager.ConnectionStrings[connName].ConnectionString);
                HttpContext.Current.Items[key] = sqlHelper;
            }
            else
            {
                if (_dic.Keys.Contains(key))
                    return _dic[key] as SqlHelper;

                sqlHelper = new SqlHelper(ConfigurationManager.ConnectionStrings[connName].ConnectionString);
                _dic[key] = sqlHelper;
            }

            return sqlHelper;
        }


        #endregion

        #endregion

        #region 常量
        // 数据库连接字符串
        private string connStr = "";
        private string _dbName = "";

        public string ConnStr
        {
            get
            {
                return connStr;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string ShortName
        {
            get
            {
                return Name.Remove(0, Name.LastIndexOf('_') + 1);
            }
        }

        public string DbName
        {
            get
            {
                if (_dbName == "")
                {
                    if (connStr == "")
                        _dbName = ""; ;
                    SqlConnection conn = new SqlConnection(connStr);
                    _dbName = conn.Database;
                }
                return _dbName;
            }
        }
        #endregion

        #region 构造函数
        public SqlHelper(string connStr)
        {
            this.connStr = connStr;
        }
        #endregion

        #region SQL无返回值可操作的方法

        //带错误输出，无返回值的SQL执行（默认是Text）
        public string ExecuteNonQuery(string cmdText)
        {
            string retVal = "0";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    retVal = cmd.ExecuteNonQuery().ToString();
                }
                catch (Exception exp)
                {
                    //retVal = exp.Message;
                    throw exp;
                }
                finally
                {
                    conn.Close();
                }
            }
            return retVal;
        }

        public string ExecuteNonQueryWithTrans(string cmdText)
        {
            string retVal = "0";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            cmd.Transaction = trans;
                            retVal = cmd.ExecuteNonQuery().ToString();
                            trans.Commit();
                        }
                        catch { trans.Rollback(); throw; }
                    }
                }
                catch (Exception exp)
                {
                    //retVal = exp.Message;
                    throw exp;
                }
                finally
                {
                    conn.Close();
                }
            }
            return retVal;
        }

        #endregion

        #region SQL有返回值可操作的方法

        //有返回值的SQL执行
        public object ExecuteScalar(string cmdText)
        {
            object retVal = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    retVal = cmd.ExecuteScalar();
                }
                catch (Exception exp)
                {
                    //retVal = (object)exp.Message;
                    throw exp;
                }
                finally
                {
                    conn.Close();
                }
            }

            return retVal;
        }

        #endregion

        #region 返回DataTable可操作的方法

        public DataTable ExecuteDataTable(string cmdText)
        {
            DataTable dt = new DataTable();

            SqlConnection conn = new SqlConnection(connStr);
            SqlDataAdapter apt = new SqlDataAdapter(cmdText, conn);
            apt.SelectCommand.CommandType = CommandType.Text;
            try
            {
                apt.Fill(dt);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }

        public DataTable ExecuteDataTable(string cmdText, int start = 0, int end = 0)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            int dtflag = 0;

            SqlConnection conn = new SqlConnection(connStr);
            SqlDataAdapter apt = new SqlDataAdapter(cmdText, conn);
            apt.SelectCommand.CommandType = CommandType.Text;

            try
            {
                if (end <= 0)
                {
                    apt.Fill(dt);
                }
                else
                {
                    apt.Fill(ds, start, end, "ThisTable");
                    dtflag = 1;
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            if (dtflag == 1)
            {
                dt = ds.Tables["ThisTable"];
            }
            return dt;
        }


        #endregion
    }
}
