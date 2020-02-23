using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace DocSystem.Logic
{
    public static class Tool
    {
        public static bool IsNullOrEmpty(object str)
        {
            if (str == null)
                return true;
            if (String.IsNullOrEmpty(str.ToString()))
                return true;
            return false;
        }

 
        /// <summary>
        /// 根据Hash表更新实体对象
        /// </summary>
        /// <typeparam name="TEntity">实体对象类型</typeparam>
        /// <param name="obj">实体对象</param>
        /// <param name="table">Hash表数据</param>
        /// <returns>实体对象</returns>
        public static TEntity UpdateHashtableInstance<TEntity>(TEntity obj, Dictionary<string,object> table) where TEntity : class
        {
            var properties = obj.GetType().GetProperties();
            foreach (var pi in properties)
            {
                if (pi.Name == "ID")
                    continue;
                if (!table.ContainsKey(pi.Name)) continue;
                var value = table[pi.Name];
                if (value == null || value == DBNull.Value)
                    continue;
                if (pi.PropertyType.FullName == "System.String")
                    pi.SetValue(obj, value.ToString(), null);
                else
                {
                    if (pi.PropertyType.IsGenericType)
                    {
                        if (Tool.IsNullOrEmpty(value))
                        {
                            pi.SetValue(obj, null, null);
                            continue;
                        }
                        if (pi.PropertyType.FullName.IndexOf("Int32") > 0)
                            value = Convert.ToInt32(value);
                        else if (pi.PropertyType.FullName.IndexOf("Decimal") > 0)
                            value = Convert.ToDecimal(value);
                        else if (pi.PropertyType.FullName.IndexOf("Double") > 0)
                            value = Convert.ToDouble(value);
                        else if (pi.PropertyType.FullName.IndexOf("DateTime") > 0)
                            value = Convert.ToDateTime(value);
                        pi.SetValue(obj, value, null);
                    }
                    else
                    {
                        MethodInfo mis = pi.PropertyType.GetMethod("Parse", new Type[] { typeof(string) });
                        if (mis != null && !String.IsNullOrEmpty(value.ToString()))
                            pi.SetValue(obj, mis.Invoke(null, new object[] { value.ToString() }), null);
                    }
                }

            }
            return (TEntity)obj;
        }
    }
}
