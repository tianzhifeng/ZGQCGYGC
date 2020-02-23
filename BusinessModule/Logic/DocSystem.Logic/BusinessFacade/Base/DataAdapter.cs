using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Data;
using Formula.Helper;
using Formula;
using Config.Logic;
using Config;
using DocSystem.Logic.Domain;

namespace DocSystem.Logic
{
    public static class DataAdapter
    {  
        /// <summary>
        /// DataRow转换成Hash对象
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Dictionary<string,object> DataRowToHashTable(DataRow row)
        {
            Dictionary<string, object> record = new Dictionary<string, object>();
            for (int j = 0; j < row.Table.Columns.Count; j++)
            {
                object cellValue = row[j];
                if (cellValue.GetType() == typeof(DBNull))
                    cellValue = null;
                record[row.Table.Columns[j].ColumnName] = cellValue;
            }
            return record;
        }

        /// <summary>
        /// DataRow转换成Hash对象
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Dictionary<string, object> DataRowToHashTable(DataRow row, Dictionary<string, object> record)
        {
            for (int j = 0; j < row.Table.Columns.Count; j++)
            {
                object cellValue = row[j];
                if (cellValue.GetType() == typeof(DBNull))
                    cellValue = null;
                record[row.Table.Columns[j].ColumnName] = cellValue;
            }
            return record;
        }
    }
}
