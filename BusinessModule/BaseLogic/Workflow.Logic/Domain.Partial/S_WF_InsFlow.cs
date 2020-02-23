using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;
using Formula.Helper;
using Formula;

namespace Workflow.Logic.Domain
{
    public partial class S_WF_InsFlow
    {
        #region GetInsName

        /// <summary>
        /// 根据实例名模板获取实例名
        /// </summary>
        /// <param name="templete"></param>
        /// <returns></returns>
        public string GetInsName(string templete)
        {
            if (string.IsNullOrEmpty(templete))
                return "";
            Regex reg = new Regex("\\{[0-9a-zA-Z_\u4e00-\u9faf]*\\}");
            string result = reg.Replace(templete, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');

                if (value.ToLower() != "stepname" && value.ToLower() != "flowname")
                {
                    if (!FormDic.ContainsKey(value))
                        throw new Exception(string.Format("字段不存在：{0}", value));
                    string str = FormDic[value].ToString();
                    //去掉时间
                    str = str.Replace("0:00:00", "").Replace("00:00:00", "").Trim();
                    return str;
                }
                else
                    return m.Value;
            });
            return result;
        }

        #endregion

        #region FormDic

        private StringDictionary _formDic = null;
        [NotMapped]
        public StringDictionary FormDic
        {
            get
            {
                if (_formDic == null)
                {
                    string formData = FormulaHelper.ContextGetValueString("FormData");
                    string connName = this.S_WF_InsDefFlow.ConnName;
                    string tableName = this.S_WF_InsDefFlow.TableName;
                    string formInstanceID = this.FormInstanceID;
                    _formDic = GetFormDic(formData, connName, tableName, formInstanceID);
                }
                return _formDic;
            }
        }

        public static StringDictionary GetFormDic(string formData, string connName, string tableName, string formInstanceID)
        {
            StringDictionary _formDic = new StringDictionary();
            if (!string.IsNullOrEmpty(formData))
            {
                var objDic = JsonHelper.ToObject<Dictionary<string, object>>(formData);
                foreach (string key in objDic.Keys)
                {
                    _formDic.Add(key, objDic[key] == null ? "" : objDic[key].ToString());
                }
            }

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(connName);
            DataTable dt = sqlHelper.ExecuteDataTable(string.Format("select * from {0} where ID='{1}'", tableName, formInstanceID));
            if (dt.Rows.Count > 0)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.ToLower() == "serialnumber")
                    {
                        if (!_formDic.ContainsKey(col.ColumnName))
                            _formDic.Add(col.ColumnName, dt.Rows[0][col].ToString());
                        else
                            _formDic[col.ColumnName] = dt.Rows[0][col].ToString();
                    }
                    if (!_formDic.ContainsKey(col.ColumnName))
                        _formDic.Add(col.ColumnName, dt.Rows[0][col].ToString());
                }
            }
            return _formDic;
        }

        #endregion
    }
}
