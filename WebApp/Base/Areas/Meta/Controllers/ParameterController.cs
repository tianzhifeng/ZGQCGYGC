using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using Base.Logic;
using Base.Logic.Domain;
using System.Data;
using System.ComponentModel;

namespace Base.Areas.Meta.Controllers
{
    public class ParameterController : BaseController<S_M_Parameter>
    {
        public ActionResult RefList()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            var tab = new Formula.Tab();
            var category = CategoryFactory.GetCategory(typeof(Base.Logic.ArgumentType), "参数类型", "ParamType", true,
                ArgumentType.CalArg.ToString() + "," + ArgumentType.DSArg.ToString() + "," + ArgumentType.DynCalArg.ToString());
            tab.Categories.Add(category);
            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public override ActionResult List()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            return View();
        }

        public override ActionResult Edit()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>().ToList());
            return View();
        }

        public ActionResult Settings()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>().ToList());
            return View();
        }

        public override JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            string sql = @"select * from S_M_Parameter
where (ParentID is null or ParentID='')
and ParamType in ('" + ArgumentType.CalArg.ToString() + "','" + ArgumentType.DynCalArg.ToString() + "')";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var data = db.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetSelectorList(MvcAdapter.QueryBuilder qb)
        {
            string sql = @"select * from S_M_Parameter
where (ParentID is null or ParentID='')
and ParamType in ('" + ArgumentType.CalArg.ToString() + "','" + ArgumentType.DynCalArg.ToString() + "')";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var data = db.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetRefList(MvcAdapter.QueryBuilder qb)
        {
            string sql = @"select S_M_Parameter.*, isnull(UseParam,0) as UseParam from S_M_Parameter with(nolock)
left join (select count(ID) as UseParam,Name,Code,ID from (
select Code,Name,ID from S_M_Parameter with(nolock)
where ParamType in ('InputArg','RefArg') and (ParentID is null or ParentID=''))
TableInfo inner join (select Expression from S_M_Parameter with(nolock) where ParamType='CalArg'
) CalParam on CalParam.Expression like '%{'+TableInfo.Code+'}%'
group by Name,Code,ID) UserCountTable on S_M_Parameter.ID=UserCountTable.ID
where ParamType in ('" + ArgumentType.InputArg.ToString() + "','" + ArgumentType.RefArg.ToString() + "') and (ParentID is null or ParentID='')";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var data = db.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetUserParamList(MvcAdapter.QueryBuilder qb)
        {
            string sql = @"select * from S_M_Parameter with(nolock) where Expression is not null
            and Expression<> '' and Expression like '%{" + this.GetQueryString("Code") + "}%'";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var data = db.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetParamTree(string ID)
        {
            var paramEntity = this.entities.Set<S_M_Parameter>().Find(ID);
            if (paramEntity == null) throw new Exception("没有找到指定ID的计算项，请检查参数是否正确");
            var result = new List<Dictionary<string, object>>();
            string sql = @"select ID,Name+'['+Code+']' as Name,Code,NodeType,FullID,ParentID,ParamType,[Expression],SortIndex from S_M_Parameter
where  (ParamType in ('" + ArgumentType.RefArg.ToString() + "','"
                        + ArgumentType.InputArg.ToString() + "','" + ArgumentType.CalArg + "') and ID<>'" + ID + "' and (ParentID ='' or ParentID is null)) or FullID like '" + paramEntity.FullID + "%'";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = db.ExecuteDataTable(sql);
            foreach (DataRow parameter in dt.Rows)
            {
                var param = FormulaHelper.DataRowToDic(parameter);
                if (param.GetValue("ID") == ID)
                {

                    continue;
                }
                else if (String.IsNullOrEmpty(param.GetValue("ParentID")))
                    param.SetValue("ParentID", param.GetValue("ParamType"));
                result.Add(param);
            }

            var enums = EnumBaseHelper.GetEnumDef(typeof(ArgumentType));
            foreach (var item in enums.EnumItem)
            {
                if (item.Code == ArgumentType.DynCalArg.ToString())
                    continue;
                else if (item.Code == ArgumentType.CalArg.ToString())
                {
                    var node = new Dictionary<string, object>();
                    node.SetValue("Name", "其他计算参数");
                    node.SetValue("Code", item.Code);
                    node.SetValue("ID", item.Code);
                    node.SetValue("NodeType", "ParamType");
                    node.SetValue("ParamType", item.Code);
                    node.SetValue("ParentID", "Root");
                    result.Add(node);
                }
                else
                {
                    var node = new Dictionary<string, object>();
                    node.SetValue("Name", item.Name);
                    node.SetValue("Code", item.Code);
                    node.SetValue("ID", item.Code);
                    node.SetValue("NodeType", "ParamType");
                    node.SetValue("ParamType", item.Code);
                    node.SetValue("ParentID", "Root");
                    result.Add(node);
                }
            }

            var rootNode = new Dictionary<string, object>();
            rootNode.SetValue("Name", "公用参数");
            rootNode.SetValue("Code", "Root");
            rootNode.SetValue("NodeType", "Root");
            rootNode.SetValue("ParamType", "Root");
            rootNode.SetValue("ID", "Root");
            rootNode.SetValue("ParentID", "");
            result.Add(rootNode);

            var funcNode = new Dictionary<string, object>();
            funcNode.SetValue("Name", "系统函数");
            funcNode.SetValue("Code", "Func");
            funcNode.SetValue("NodeType", "Func");
            funcNode.SetValue("ParamType", "Root");
            funcNode.SetValue("ID", "SysFuncRoot");
            funcNode.SetValue("ParentID", "");
            result.Add(funcNode);

            var emType = typeof(SystemFunction);
            foreach (var item in emType.GetFields())
            {
                if (item.FieldType.IsEnum)
                {
                    object[] arr = item.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    var text = arr.Length > 0 ? ((DescriptionAttribute)arr[0]).Description : item.Name;
                    var value = item.Name;
                    var funcSubNode = new Dictionary<string, object>();
                    funcSubNode.SetValue("Name", text);
                    funcSubNode.SetValue("Code", value);
                    funcSubNode.SetValue("NodeType", "Func");
                    funcSubNode.SetValue("ParamType", "Func");
                    funcSubNode.SetValue("ID", value);
                    funcSubNode.SetValue("ParentID", "SysFuncRoot");
                    result.Add(funcSubNode);
                }
            }

            var selfNode = new Dictionary<string, object>();
            selfNode.SetValue("Name", "本地参数");
            selfNode.SetValue("Code", paramEntity.Code);
            selfNode.SetValue("NodeType", "Self");
            selfNode.SetValue("ParamType", paramEntity.ParamType);
            selfNode.SetValue("ID", paramEntity.ID);
            selfNode.SetValue("ParentID", "");
            result.Add(selfNode);

            return Json(result);
        }

        public override JsonResult GetModel(string id)
        {
            var entity = this.GetEntity<S_M_Parameter>(id);
            bool isNew = false;
            if (entities.Entry<S_M_Parameter>(entity).State == System.Data.EntityState.Added || entities.Entry<S_M_Parameter>(entity).State == System.Data.EntityState.Detached)
                isNew = true;
            var dic = FormulaHelper.ModelToDic<S_M_Parameter>(entity);
            if (isNew)
            {
                dic.SetValue("ParamType", String.IsNullOrEmpty(this.GetQueryString("ParamType")) ? "" : this.GetQueryString("ParamType"));
                dic.SetValue("ParentID", this.GetQueryString("ParentID"));
            }
            else
            {
                var code = dic.GetValue("Code");
                if (code.StartsWith("Input_"))
                    code = code = code.Substring(6); //code.TrimStart(new char[] { 'I', 'n', 'p', 'u', 't', '_' });
                else if (code.StartsWith("Cal_"))
                    code = code.Substring(4);
                else if (code.StartsWith("Ref_"))
                    code = code = code.Substring(4);// code.TrimStart(new char[] { 'R', 'e', 'f', '_' });
                else if (code.StartsWith("DynCal_"))
                    code = code = code.Substring(7);//code.TrimStart(new char[] { 'D', 'y', 'n', 'C', 'a', 'l', '_' });
                else if (code.StartsWith("DS_"))
                    code = code = code.Substring(3);//code.TrimStart(new char[] { 'D', 'S', '_' });
                dic.SetValue("Code", code);
            }
            return Json(dic);
        }

        public override JsonResult Save()
        {
            var result = new Dictionary<string, object>();
            bool isNew = false;
            var entity = UpdateEntity<S_M_Parameter>();
            entity.Expression = this.Request["Expression"];
            if (this.entities.Entry<S_M_Parameter>(entity).State == System.Data.EntityState.Detached
                || this.entities.Entry<S_M_Parameter>(entity).State == System.Data.EntityState.Added)
                isNew = true;
            if (isNew)
            {
                entity.NodeType = "Paramter";
                entity.CalType = CalType.Decimal.ToString();       
                if (this.entities.Set<S_M_Parameter>().Count(c => c.Code == entity.Code) > 0)
                {
                    throw new Formula.Exceptions.BusinessValidationException("已经存在编号为【" + entity.Code + "】的参数，不能重复定义");
                }
                if (String.IsNullOrEmpty(entity.ParentID))
                {
                    entity.FullID = entity.ID;
                    var bortherList = this.entities.Set<S_M_Parameter>().Where(c => String.IsNullOrEmpty(c.ParentID)).Select(c => c.SortIndex).ToList();
                    entity.SortIndex = bortherList.Count > 0 ? bortherList.Max() + 1 : 0m;
                }
                else
                {
                    var bortherList = this.entities.Set<S_M_Parameter>().Where(c => c.ParentID == entity.ParentID).Select(c => c.SortIndex).ToList();
                    var parent = this.entities.Set<S_M_Parameter>().Find(entity.ParentID);
                    if (parent != null)
                    {
                        entity.FullID = parent.FullID + "." + entity.ID;
                        entity.SortIndex = 0;
                        entity.ParamType = parent.ParamType;
                    }
                    else
                    {
                        entity.FullID = entity.ID;
                        entity.SortIndex = bortherList.Count > 0 ? bortherList.Max() + 1 : 0m;
                    }
                }
                result = FormulaHelper.ModelToDic<S_M_Parameter>(entity);
            }
            if (!String.IsNullOrEmpty(entity.Expression))
            {
                var calFo = FormulaHelper.CreateFO<CaculateFo>();
                calFo.ValidateParam(FormulaHelper.ModelToDic<S_M_Parameter>(entity));
            }
            if (entity.ParamType == ArgumentType.InputArg.ToString())
            {
                if (entity.Code.StartsWith("Input_"))
                    entity.Code = "Input_" + entity.Code.TrimStart(new char[] { 'I', 'n', 'p', 'u', 't', '_' });
                else
                    entity.Code = "Input_" + entity.Code;
            }
            else if (entity.ParamType == ArgumentType.CalArg.ToString())
            {
                if (entity.Code.StartsWith("Cal_"))
                    entity.Code = "Cal_" + entity.Code.TrimStart(new char[] { 'C', 'a', 'l', '_' });
                else
                    entity.Code = "Cal_" + entity.Code;
            }
            else if (entity.ParamType == ArgumentType.RefArg.ToString())
            {
                if (entity.Code.StartsWith("Ref_"))
                    entity.Code = "Ref_" + entity.Code.TrimStart(new char[] { 'R', 'e', 'f', '_' });
                else
                    entity.Code = "Ref_" + entity.Code;
            }
            else if (entity.ParamType == ArgumentType.DynCalArg.ToString())
            {
                if (entity.Code.StartsWith("DynCal_"))
                    entity.Code = "DynCal_" + entity.Code.TrimStart(new char[] { 'D', 'y', 'n', 'C', 'a', 'l', '_' });
                else
                    entity.Code = "DynCal_" + entity.Code;
            }
            else if (entity.ParamType == ArgumentType.DSArg.ToString())
            {
                if (entity.Code.StartsWith("DS_"))
                    entity.Code = "DS_" + entity.Code.TrimStart(new char[] { 'D', 'y', 'n', 'C', 'a', 'l', '_' });
                else
                    entity.Code = "DS_" + entity.Code;
            }
            entities.SaveChanges();
            return Json(result);
        }

        public JsonResult RemoveParam(string ListIDs, string IsSelf)
        {
            foreach (var ID in ListIDs.Split(','))
            {
                var param = this.entities.Set<S_M_Parameter>().Find(ID);
                if (param == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到对应的参数对象，无法删除");
                }
                if (IsSelf == true.ToString().ToLower())
                {
                    this.entities.Set<S_M_Parameter>().Delete(c => c.FullID.StartsWith(param.FullID));
                }
                else
                {
                    var types = ArgumentType.CalArg.ToString() + "," + ArgumentType.DynCalArg.ToString();
                    if (this.entities.Set<S_M_Parameter>().Where(c => types.Contains(c.ParamType)
                        && !String.IsNullOrEmpty(c.Expression) && c.Expression.Contains("{" + param.Code + "}")).Count() > 0)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("【" + param.Name + "】尚有其他计算参数运用了当前参数，不能进行删除操作");
                    }
                    this.entities.Set<S_M_Parameter>().Delete(c => c.FullID.StartsWith(param.FullID));
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public override JsonResult GetTree()
        {
            var result = new List<Dictionary<string, object>>();
            var list = this.entities.Set<S_M_Parameter>().OrderBy(c => c.SortIndex).ToList();
            foreach (var parameter in list)
            {
                var param = FormulaHelper.ModelToDic<S_M_Parameter>(parameter);
                if (String.IsNullOrEmpty(parameter.ParentID))
                    param.SetValue("ParentID", parameter.ParamType);
                result.Add(param);
            }
            var enums = EnumBaseHelper.GetEnumDef(typeof(ArgumentType));
            foreach (var item in enums.EnumItem)
            {
                var node = new Dictionary<string, object>();
                node.SetValue("Name", item.Name);
                node.SetValue("Code", item.Code);
                node.SetValue("ID", item.Code);
                node.SetValue("NodeType", "ParamType");
                node.SetValue("ParamType", item.Code);
                node.SetValue("ParentID", "Root");
                result.Add(node);
            }
            var rootNode = new Dictionary<string, object>();
            rootNode.SetValue("Name", "计算项定义");
            rootNode.SetValue("Code", "Root");
            rootNode.SetValue("NodeType", "Root");
            rootNode.SetValue("ParamType", "Root");
            rootNode.SetValue("ID", "Root");
            rootNode.SetValue("ParentID", "");
            result.Add(rootNode);
            return Json(result);
        }
    }
}
