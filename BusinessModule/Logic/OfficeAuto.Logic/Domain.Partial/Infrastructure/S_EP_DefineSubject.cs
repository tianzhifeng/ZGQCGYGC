using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace OfficeAuto.Logic.Domain
{
    public partial class S_EP_DefineSubject
    {

        /// <summary>
        /// 数据库访问对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public virtual SQLHelper DB
        {
            get
            {
                return SQLHelper.CreateSqlHelper(ConnEnum.OfficeAuto);
            }
        }

        Dictionary<string, object> _modelDic;
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public Dictionary<string, object> ModelDic
        {
            get
            {
                return _modelDic;
            }
        }

        public S_EP_DefineSubject(Dictionary<string, object> dic)
        {
            this._modelDic = dic;
            var type = this.GetType();
            var properties = type.GetProperties();
            foreach (var pi in properties)
            {
                if (!dic.ContainsKey(pi.Name))
                    continue;
                if (pi.PropertyType.FullName == "System.String")
                {
                    //为兼容Oracle，不能使用bool型，因此使用char(1)
                    string value = "";
                    if (!String.IsNullOrEmpty(dic.GetValue(pi.Name)))
                    {
                        value = dic.GetValue(pi.Name).Trim();
                    }
                    pi.SetValue(this, value, null);
                }
                else if (pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(Nullable<bool>))
                {
                    string value = dic.GetValue(pi.Name);
                    if (value.ToLower() == "true" || value == "1")
                        pi.SetValue(this, true, null);
                    else
                        pi.SetValue(this, false, null);
                }
                else if (pi.PropertyType.IsValueType)
                {
                    Object value = null;
                    Type t = System.Nullable.GetUnderlyingType(pi.PropertyType);
                    if (t == null)
                        t = pi.PropertyType;
                    MethodInfo mis = t.GetMethod("Parse", new Type[] { typeof(string) });
                    try
                    {
                        value = mis.Invoke(null, new object[] { dic.GetValue(pi.Name) });
                    }
                    catch
                    {
                        throw new Exception(string.Format("数据类型转换失败:将‘{0}’转换为{1}类型时.", dic.GetValue(pi.Name), t.Name));
                    }
                    pi.SetValue(this, value, null);
                }
            }
        }

        S_EP_DefineSubject _parent;
        public S_EP_DefineSubject ParentNode
        {
            get
            {
                if (_parent == null)
                {
                    string sql = "select * from S_EP_DefineSubject WITH(NOLOCK) where ID='{0}'";
                    var dt = this.DB.ExecuteDataTable(String.Format(sql, this.ModelDic.GetValue("ParentID")));
                    if (dt.Rows.Count > 0)
                    {
                        var dic = FormulaHelper.DataRowToDic(dt.Rows[0]);
                        _parent = new S_EP_DefineSubject(dic);
                    }
                }
                return _parent;
            }
        }

        public S_EP_DefineSubject AddChildNode()
        {
            var child = new Dictionary<string, object>();
            child.SetValue("ID", FormulaHelper.CreateGuid());
            child.SetValue("Name", "新建科目");
            return this.AddChildNode(child);
        }

        public S_EP_DefineSubject AddBrotherNode()
        {
            var child = new Dictionary<string, object>();
            child.SetValue("ID", FormulaHelper.CreateGuid());
            child.SetValue("Name", "新建科目");
            return this.AddBrotherNode(child);
        }

        public S_EP_DefineSubject AddBrotherNode(Dictionary<string, object> child)
        {
            if (this.ParentNode == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到父节点，无法新增科目");
            }
            var sortIndex = String.IsNullOrEmpty(this.ModelDic.GetValue("SortIndex")) ? 0m : Convert.ToDecimal(this.ModelDic.GetValue("SortIndex"));
            child.SetValue("SortIndex", sortIndex + 1);
            string sql = "update S_EP_DefineSubject set SortIndex= SortIndex+1 where ParentID='{0}' and SortIndex>{1}";
            this.DB.ExecuteNonQuery(String.Format(sql, this.ModelDic.GetValue("ParentID"), sortIndex));
            return this.ParentNode.AddChildNode(child);
        }

        public S_EP_DefineSubject AddChildNode(Dictionary<string, object> child)
        {
            if (String.IsNullOrEmpty(child.GetValue("ID")))
                child.SetValue("ID", FormulaHelper.CreateGuid());
            child.SetValue("ParentID", this.ModelDic.GetValue("ID"));
            child.SetValue("FullID", this.ModelDic.GetValue("FullID") + "." + child.GetValue("ID"));
            child.SetValue("NodeType", "Subject");
            child.SetValue("Level", child.GetValue("FullID").Split('.').Length);
            if (String.IsNullOrEmpty(child.GetValue("SortIndex")))
            {
                var sql = "select isnull(max(sortindex),0) from S_EP_DefineSubject with(nolock) where ParentID='{0}'";
                var maxSortIndex = this.DB.ExecuteScalar(String.Format(sql, this.ModelDic.GetValue("ID")));
                child.SetValue("SortIndex", Convert.ToDecimal(maxSortIndex) + 1);
            }
            child.InsertDB(this.DB, "S_EP_DefineSubject", child.GetValue("ID"));
            return new S_EP_DefineSubject(child);
        }

        public void Delete()
        {
            if (this.ModelDic.GetValue("NodeType") == "Root" || String.IsNullOrEmpty(this.ModelDic.GetValue("ParentID")))
                throw new Formula.Exceptions.BusinessValidationException("根节点不允许删除");
            string sql = "delete from S_EP_DefineSubject where FullID like '{0}%'";
            this.DB.ExecuteNonQuery(String.Format(sql, this.ModelDic.GetValue("FullID")));
        }

    }
}
