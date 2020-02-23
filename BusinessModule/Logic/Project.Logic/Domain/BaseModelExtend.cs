using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Entity;
using Formula;
using System.Web;
using System.Reflection;
using Formula.Helper;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Project.Logic.Domain
{
    public static class BaseModelExtend
    {
        public static T GetDbContext<T>(this BaseModel model) where T : DbContext, new()
        {
            return FormulaHelper.GetEntities<T>();
        }

        public static Dictionary<string, object> ToDic(this BaseModel model, string fields = "")
        {
            var type = model.GetType();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            PropertyInfo[] arrPtys = type.GetProperties();
            if (!String.IsNullOrEmpty(fields))
            {
                var fieldsArray = fields.Split(',');
                arrPtys = arrPtys.Where(d => fieldsArray.Contains(d.Name)).ToArray();
            }
            foreach (PropertyInfo destPty in arrPtys)
            {
                if (destPty.CanRead == false)
                    continue;
                if (destPty.PropertyType.Name == "ICollection`1")
                    continue;
                if ((destPty.PropertyType.IsClass && destPty.PropertyType.Name != "String") || destPty.PropertyType.IsArray || destPty.PropertyType.IsInterface)
                    continue;
                object value = destPty.GetValue(model, null);
                dic.Add(destPty.Name, value);
            }
            return dic;
        }

        public static string ToJson(this BaseModel model)
        {
            var type = model.GetType();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            PropertyInfo[] arrPtys = type.GetProperties();
            foreach (PropertyInfo destPty in arrPtys)
            {
                if (destPty.CanRead == false)
                    continue;
                if (destPty.PropertyType.Name == "ICollection`1")
                    continue;
                if ((destPty.PropertyType.IsClass && destPty.PropertyType.Name != "String") || destPty.PropertyType.IsArray || destPty.PropertyType.IsInterface)
                    continue;
                object value = destPty.GetValue(model, null);
                dic.Add(destPty.Name, value);
            }
            IsoDateTimeConverter dt = new IsoDateTimeConverter();
            dt.DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
            return JsonConvert.SerializeObject(dic, dt);
        }

        public static List<PropertyInfo> GetProperties(this BaseModel model)
        {
            Type type = model.GetType();
            var propInfos = type.GetProperties().Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime)
              || p.PropertyType == typeof(double) || p.PropertyType == typeof(float) || p.PropertyType == typeof(decimal)
              || p.PropertyType.IsValueType).Where(d => d.PropertyType != typeof(bool)).ToList();
            return propInfos;
        }

        public static PropertyInfo GetProperty(this BaseModel model, string propertyName, bool canReturnNull = false)
        {
            Type type = model.GetType();
            PropertyInfo[] propInfos = type.GetProperties();
            PropertyInfo propInfo = propInfos.FirstOrDefault
                (p => p.Name == propertyName);
            if (propInfo == null)
                if (canReturnNull)
                    return null;
                else
                    throw new Formula.Exceptions.BusinessException("没有找到相应的属性，操作失败！");
            return propInfo;
        }

        public static void SetProperty(this BaseModel model, string name, object value)
        {
            var property = model.GetProperty(name, true);
            if (property == null) return;
            property.SetValue(model, value, null);
        }

        public static string GetPropertyString(this BaseModel model, string name)
        {
            var property = model.GetProperty(name, true);
            if (property == null) return string.Empty;
            object obj = property.GetValue(model, null);
            if (obj == null || obj == DBNull.Value)
                return string.Empty;
            else
                return obj.ToString();
        }

        public static object GetPropertyValue(this BaseModel model, string name)
        {
            var property = model.GetProperty(name, true);
            if (property == null) return null;
            return property.GetValue(model, null);
        }

        public static T Clone<T>(this BaseModel model) where T : BaseModel, new()
        {
            var proplist = model.GetProperties();
            var result = new T();
            foreach (var item in proplist)
                result.SetProperty(item.Name, model.GetPropertyValue(item.Name));
            if (proplist.FirstOrDefault(d => d.Name == "ID").PropertyType == typeof(string))
                result.SetProperty("ID", FormulaHelper.CreateGuid());
            return result;
        }
    }
}
