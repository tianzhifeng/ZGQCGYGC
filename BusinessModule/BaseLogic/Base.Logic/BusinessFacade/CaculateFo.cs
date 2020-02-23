using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

using Config;
using Config.Logic;
using Formula;
using Base.Logic.Domain;
using Base.Logic.BusinessFacade;
using System.Web;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using Formula.Helper;
using System.Diagnostics;

namespace Base.Logic
{
    public class CaculateFo
    {
        #region  私有变量
        Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
        Regex funcReg = new Regex("Func_[a-zA-Z0-9]+(\\()[!-\\'\\*-\\~\\t\\n\\s]+(\\)){1}");
        DataTable calDt = new DataTable();
        #endregion

        #region 公开属性
        UserInfo _user = null;
        /// <summary>
        /// 
        /// </summary>
        public UserInfo user
        {
            get
            {
                if (_user == null)
                {
                    _user = FormulaHelper.GetUserInfo();
                }
                return _user;
            }
        }

        DataTable _dt = null;
        /// <summary>
        /// 获取所有计算参数定义
        /// </summary>
        public DataTable AllParamDt
        {
            get
            {
                if (_dt == null)
                {
                    string sql = String.Format("select ID,Name,Code,Expression,ParamType,SQL,ConnName,IsCollectionRef,OrderBy from S_M_Parameter with(nolock)");
                    var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
                    _dt = db.ExecuteDataTable(sql);
                    _dt.PrimaryKey = new DataColumn[] { _dt.Columns["Code"] };
                }
                return _dt;
            }
        }



        Dictionary<string, object> _refParamCollection = new Dictionary<string, object>();
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> RefParamCollection
        {
            get
            {
                return _refParamCollection;
            }
        }
        #endregion

        #region 公共实例方法
        /// <summary>
        /// 校验计算参数定义的表达式是否正确
        /// </summary>
        /// <param name="param"></param>
        public void ValidateParam(Dictionary<string, object> param)
        {
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            var Expression = param.GetValue("Expression");
            string result = reg.Replace(Expression, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                return string.Empty;
            });
            MatchCollection mc = reg.Matches(Expression);
            for (int i = 0; i < mc.Count; i++)
            {
                var value = mc[i].Value.Trim('{', '}');
                var row = AllParamDt.Rows.Find(value);
                if (row == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException(String.Format("参数【{0}】定义错误，表达式中编号为【{1}】的参数不存在", param.GetValue("Name"), mc[i].Value));
                }
                else
                {
                    ValidateParam(FormulaHelper.DataRowToDic(row));
                }
            }
        }

        /// <summary>
        /// 根据计算项进行公式计算，并填充表单数据FormData
        /// </summary>
        /// <param name="calItem">计算项</param>
        /// <param name="formData">表单数据</param>
        public void CalculateCalItem(CalItem calItem, Dictionary<string, object> formData)
        {
            if (formData == null) throw new Formula.Exceptions.BusinessValidationException("表单数据源不能为空");
            if (calItem == null) throw new Formula.Exceptions.BusinessValidationException("计算项不能为空");
            if (calItem.IsSubTableField)
            {
                var subTableItemCode = calItem.FieldCode.Split('.')[0];
                var subItemCode = calItem.FieldCode.Split('.')[1];
                var subItemDataList = JsonHelper.ToList(formData.GetValue(subTableItemCode));
                foreach (var itemData in subItemDataList)
                {
                    if (this.ValidateCancelCaculate(calItem, formData, itemData))
                    {
                        continue;
                    }
                    var inputParamDic = new Dictionary<string, object>();
                    if (calItem.AdapterItems.Count > 0)
                    {
                        foreach (var adpItem in calItem.AdapterItems)
                        {
                            var subItemInputFields = adpItem.InputField.Trim('{', '}').Split('.');
                            if (subItemInputFields.Length == 2)
                            {
                                inputParamDic.SetValue(adpItem.NeedInputCode.Trim('{', '}'),
                                    String.IsNullOrEmpty(itemData.GetValue(subItemInputFields[1])) ? adpItem.DefaultValue : itemData.GetValue(subItemInputFields[1]));
                            }
                            else
                            {
                                inputParamDic.SetValue(adpItem.NeedInputCode.Trim('{', '}'),
                                    String.IsNullOrEmpty(formData.GetValue(adpItem.InputField.Trim('{', '}'))) ? adpItem.DefaultValue : formData.GetValue(adpItem.InputField.Trim('{', '}')));
                            }
                        }
                    }
                    var result = this.CaculateExpression(calItem.Expression, calItem.CalType, formData, itemData, inputParamDic, null,
                        FormulaHelper.CollectionToListDic<ValueConditionItem>(calItem.ValueConditionItem), calItem.CollectionValueField, calItem.DecimalPlaces);
                    itemData.SetValue(subItemCode, result);
                }
                formData.SetValue(subTableItemCode, JsonHelper.ToJson(subItemDataList));
            }
            else
            {
                if (this.ValidateCancelCaculate(calItem, formData))
                {
                    return;
                }
                var inputParamDic = new Dictionary<string, object>();
                foreach (var adpItem in calItem.AdapterItems)
                {
                    inputParamDic.SetValue(adpItem.NeedInputCode.Trim('{', '}'),
                              String.IsNullOrEmpty(formData.GetValue(adpItem.InputField.Trim('{', '}'))) ? adpItem.DefaultValue : formData.GetValue(adpItem.InputField.Trim('{', '}')));
                }
                var result = this.CaculateExpression(calItem.Expression, calItem.CalType, formData, null, inputParamDic, FormulaHelper.CollectionToListDic<SubTableAdpItem>(calItem.SubTableAdapters),
                    FormulaHelper.CollectionToListDic<ValueConditionItem>(calItem.ValueConditionItem), calItem.CollectionValueField, calItem.DecimalPlaces);
                formData.SetValue(calItem.FieldCode.Trim('{', '}'), result);
            }
        }

        /// <summary>
        /// 获取引用参数的内容（同时范围引用参数的集合，值，在确定引用参数返回范围时使用，例如子表数据信息变更等）
        /// </summary>
        /// <param name="paramKey">参数KEY</param>
        /// <param name="formData">输入表单上下文内容</param>
        /// <param name="paramInputData">表单定义配置匹配内容（优先于formData）</param>
        /// <returns></returns>
        public Dictionary<string, object> GetRefParamValue(string paramKey, Dictionary<string, object> formData = null, Dictionary<string, object> paramInputData = null)
        {
            var paramRow = this.AllParamDt.Rows.Find(paramKey);
            if (paramRow == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到相关的参数信息");
            }
            if (paramRow["ParamType"] == null || paramRow["ParamType"] == DBNull.Value || String.IsNullOrEmpty(paramRow["ParamType"].ToString()))
            {
                throw new Formula.Exceptions.BusinessValidationException("计算式没有设置类型，无法获取应用参数值");
            }
            if (paramRow["ParamType"].ToString() != ArgumentType.RefArg.ToString())
            {
                throw new Formula.Exceptions.BusinessValidationException("非引用参数不能调用次方法");
            }
            if (paramRow["ConnName"] == null || paramRow["ConnName"] == DBNull.Value || String.IsNullOrEmpty(paramRow["ConnName"].ToString()))
            {
                throw new Formula.Exceptions.BusinessValidationException("引用参数必须指定数据库链接");
            }
            if (paramRow["SQL"] == null || paramRow["SQL"] == DBNull.Value || String.IsNullOrEmpty(paramRow["SQL"].ToString()))
            {
                throw new Formula.Exceptions.BusinessValidationException("引用参数必须指定查询SQL");
            }
            var result = new Dictionary<string, object>();
            result.SetValue("ParamKey", paramKey);
            var db = SQLHelper.CreateSqlHelper(paramRow["ConnName"].ToString());
            var sql = paramRow["SQL"].ToString();
            sql = this.ReplaceString(sql, formData, paramInputData);
            var dt = db.ExecuteDataTable(sql);
            var resultValue = 0m;
            result.SetValue("ResultDt", dt);
            result.SetValue("ReslutList", FormulaHelper.DataTableToListDic(dt));
            result.SetValue("ReslutValue", resultValue);
            if (dt.Rows.Count == 1 && dt.Columns.Count == 1)
            {
                var obj = dt.Rows[0][0];
                if (decimal.TryParse(obj.ToString(), out resultValue))
                {
                    result.SetValue("ReslutValue", resultValue);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据表达式进行动态计算
        /// </summary>
        /// <param name="expression">计算表表达式</param>
        /// <param name="onlyCal">是否仅进行数学计算（如果为false，则表明表达式中有if ... else... 的逻辑判定）</param>
        /// <param name="formData">上下文环境数据</param>
        /// <param name="paramInputData">上下文输入参数数据（优先于formData）</param>
        /// <returns></returns>
        public object CaculateExpression(string expression, string calType = "Decimal", Dictionary<string, object> formData = null,
            Dictionary<string, object> itemData = null,
            Dictionary<string, object> paramInputData = null,
            List<Dictionary<string, object>> subTableAdapter = null,
            List<Dictionary<string, object>> refValueConditions = null, string collectionValueField = "", int decimalPlaces = 2)
        {
            object result = 0;
            if (formData == null || paramInputData == null)
            {
                if (reg.IsMatch(expression))
                {
                    throw new Formula.Exceptions.BusinessValidationException("无上下文环境的境况下，只能进行数值计算，表达式中不能含有其他变量");
                }
                try
                {
                    result = calDt.Compute(expression, "");
                }
                catch (Exception exp)
                {
                    LogWriter.Error(exp);
                    throw new Formula.Exceptions.BusinessValidationException(String.Format("计算式计算错误，请确认计算式【{0}】能被正确的计算，并不含非法字符", expression));
                }
            }
            else
            {
                var calExp = this.ResolveExpression(expression, formData, itemData, paramInputData, subTableAdapter);
                if (calType == CalType.Logic.ToString())
                {
                    #region 执行代码逻辑
                    try
                    {
                        #region 带有逻辑判定的编码，需要动态编译
                        Microsoft.JScript.Vsa.VsaEngine Engine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
                        result = Microsoft.JScript.Eval.JScriptEvaluate("function a (){ " + calExp + " } a();", Engine);
                        #endregion
                    }
                    catch (Exception exp)
                    {
                        LogWriter.Error(exp);
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("计算式逻辑执行错误，请确认计算式【{0}】能编写的JS语法正确，并确保有返回值", expression));
                    }
                    #endregion
                }
                else if (calType == CalType.Collection.ToString())
                {
                    #region 返回集合
                    if (refValueConditions != null && refValueConditions.Count > 0 && !String.IsNullOrEmpty(collectionValueField))
                    {
                        try
                        {
                            var data = JsonHelper.ToList(filterResultList(calExp, formData, paramInputData, refValueConditions));
                            return data[0].GetValue(collectionValueField);
                        }
                        catch
                        {
                            result = calExp;
                        }
                    }
                    else
                    {
                        result = calExp;
                    }
                    #endregion
                }
                else if (calType == CalType.String.ToString())
                {
                    #region 返回字符串
                    if (refValueConditions != null && refValueConditions.Count > 0 && !String.IsNullOrEmpty(collectionValueField))
                    {
                        try
                        {
                            var data = JsonHelper.ToList(filterResultList(calExp, formData, paramInputData, refValueConditions));
                            if (data.Count > 0)
                                return data[0].GetValue(collectionValueField);
                            else
                            {
                                return 0m;
                            }
                        }
                        catch
                        {
                            result = calExp;
                        }
                    }
                    else
                    {
                        result = calExp;
                    }
                    #endregion
                }
                else
                {
                    #region 数值计算
                    if (refValueConditions != null && refValueConditions.Count > 0 && !String.IsNullOrEmpty(collectionValueField))
                    {
                        try
                        {
                            var data = JsonHelper.ToList(filterResultList(calExp, formData, itemData, refValueConditions));
                            if (data.Count > 0)
                            {
                                var d = 0m;
                                decimal.TryParse(data[0].GetValue(collectionValueField), out d);
                                if (d > 0) return Math.Round(d, decimalPlaces);
                                return data[0].GetValue(collectionValueField);
                            }
                            else
                            {
                                return 0m;
                            }
                        }
                        catch
                        {
                            result = calExp;
                        }
                    }
                    else
                    {
                        try
                        {
                            Microsoft.JScript.Vsa.VsaEngine ve = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
                            object returnValue = Microsoft.JScript.Eval.JScriptEvaluate((object)calExp, ve);
                            if (returnValue.ToString() == "NaN") return 0;
                            result = Math.Round(Convert.ToDecimal(returnValue), decimalPlaces);

                        }
                        catch (Exception exp)
                        {
                            LogWriter.Error(exp);
                            throw new Formula.Exceptions.BusinessValidationException(String.Format("计算式逻辑执行错误，请确认计算式【{0}】能被正确计算，并确保有返回值", expression));
                        }
                    }
                    #endregion
                }
            }
            return result;
        }


        /// <summary>
        /// 计算表单定义中的所有需要计算的内容
        /// </summary>
        /// <param name="formDefineCode">表单定义编号</param>
        /// <param name="formData">表单上下文内容</param>
        ///<param name="paramInputData">上下文输入参数数据（优先于formData）</param>
        /// <returns></returns>
        public object Caculate(string formDefineCode, Dictionary<string, object> formData, Dictionary<string, object> paramInputData = null)
        {
            return 0;
        }

        /// <summary>
        /// 解析计算表达式至最终的数学公式
        /// </summary>
        /// <param name="expression">计算表达式</param>
        /// <param name="formData">表单上下文内容</param>
        /// <param name="paramInputData">上下文输入参数数据（优先于formData）</param>
        /// <returns></returns>
        public string ResolveExpression(string expression, Dictionary<string, object> formData,
            Dictionary<string, object> itemData,
            Dictionary<string, object> paramInputData,
            List<Dictionary<string, object>> subTableAdapter)
        {
            if (String.IsNullOrEmpty(expression))
                throw new Formula.Exceptions.BusinessValidationException("输入的计算表达式为空，不能进行表达式解析");
            var result = replaceFuncExpressionString(expression, formData, itemData, paramInputData, subTableAdapter);
            result = replaceExpressionString(result, formData, itemData, paramInputData, subTableAdapter);
            return result;
        }


        /// <summary>
        /// 解析计算表达式至最终的表达式（不在存在表达式嵌套）
        /// </summary>
        /// <param name="expression">计算表达式</param>
        /// <returns></returns>
        public string ResolveExpressionToExpression(string expression)
        {
            if (String.IsNullOrEmpty(expression))
                throw new Formula.Exceptions.BusinessValidationException("输入的计算表达式为空，不能进行表达式解析");
            string result = reg.Replace(expression, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                var row = this.AllParamDt.Rows.Find(value);
                if (row != null && row["ParamType"] != null && row["ParamType"] != DBNull.Value && row["ParamType"].ToString() == "CalArg")
                {
                    if (row["Expression"] == null || row["Expression"] == DBNull.Value || String.IsNullOrEmpty(row["Expression"].ToString()))
                        throw new Formula.Exceptions.BusinessValidationException("参数【" + row["Code"] + "】没有定义计算表达式，无法解析公式");
                    return "(" + ResolveExpressionToExpression(row["Expression"].ToString()) + ")";
                }
                else if (row != null && row["ParamType"] != null && row["ParamType"] != DBNull.Value && row["ParamType"].ToString() == "RefArg")
                {
                    return "{" + row["Code"].ToString() + "}";
                }
                else
                {
                    return "{" + value + "}";
                }
            });
            return result;
        }

        /// <summary>
        /// 获取表达式内所有需要输入匹配的参数
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="validateParam">是否强制验证参数是否存在</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetInputParamListFromExpression(string expression, bool validateParam = true)
        {
            var resExpression = this.ResolveExpressionToExpression(expression);
            var result = new List<Dictionary<string, object>>();
            var mc = reg.Matches(expression);
            for (int i = 0; i < mc.Count; i++)
            {
                var value = mc[i].Value.Trim('{', '}');
                var row = this.AllParamDt.Rows.Find(value);
                if (row == null)
                {
                    if (validateParam)
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有找编号为【{0}】的参数定义，请检查定义是否正确", value));
                    }
                    else
                    {
                        continue;
                    }
                }

                if (row["ParamType"] == null || row["ParamType"] == DBNull.Value || String.IsNullOrEmpty(row["ParamType"].ToString()))
                    continue;


                if (row["ParamType"].ToString() == ArgumentType.RefArg.ToString())
                {
                    var sql = row["SQL"] == null || row["SQL"] == DBNull.Value ? "" : row["SQL"].ToString();
                    if (String.IsNullOrEmpty(sql))
                        continue;
                    var sqlMC = reg.Matches(sql);
                    for (int m = 0; m < sqlMC.Count; m++)
                    {
                        var item = new Dictionary<string, object>();
                        if (result.Count(c => c.GetValue("NeedInputCode") == sqlMC[m].Value.Trim('{', '}')) == 0)
                        {
                            item.SetValue("Code", row["Code"]);
                            item.SetValue("Name", row["Name"]);
                            item.SetValue("NeedInputCode", sqlMC[m].Value.Trim('{', '}'));
                            item.SetValue("ParamType", row["ParamType"]);
                            item.SetValue("DefaultValue", 0);
                            result.Add(item);
                        }
                    }
                }
                else if ((row["ParamType"].ToString() == ArgumentType.InputArg.ToString()))
                {
                    if (result.Count(c => c.GetValue("NeedInputCode") == row["Code"].ToString()) == 0)
                    {
                        var item = new Dictionary<string, object>();
                        item.SetValue("Code", row["Code"]);
                        item.SetValue("Name", row["Name"]);
                        item.SetValue("NeedInputCode", row["Code"]);
                        item.SetValue("ParamType", row["ParamType"]);
                        item.SetValue("DefaultValue", 0);
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取引用参数的数据源的数据表字段
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetRefParamDataFields(string expression)
        {
            var resExpression = this.ResolveExpressionToExpression(expression);
            var result = new List<Dictionary<string, object>>();
            var mc = reg.Matches(expression);
            for (int i = 0; i < mc.Count; i++)
            {
                var value = mc[i].Value.Trim('{', '}');
                var row = this.AllParamDt.Rows.Find(value);
                if (row == null)
                {
                    continue;
                    //throw new Formula.Exceptions.BusinessValidationException(String.Format("没有找编号为【{0}】的取值参数定义，请检查定义是否正确", value));
                }
                if (row["ParamType"].ToString() == ArgumentType.RefArg.ToString())
                {
                    if (row["SQL"] == null || row["SQL"] == DBNull.Value || String.IsNullOrEmpty(row["SQL"].ToString()))
                        throw new Formula.Exceptions.BusinessValidationException("参数【" + row["Code"] + "】没有定义取值SQL，无法解析公式");
                    if (row["ConnName"] == null || row["ConnName"] == DBNull.Value || String.IsNullOrEmpty(row["ConnName"].ToString()))
                        throw new Formula.Exceptions.BusinessValidationException("参数【" + row["Code"] + "】没有定义取值的数据库链接，无法解析公式");
                    var db = SQLHelper.CreateSqlHelper(row["ConnName"].ToString());
                    var sql = "select * from (" + row["SQL"].ToString() + ") tableInfo where 1<>1 ";
                    var dt = db.ExecuteDataTable(sql);
                    foreach (DataColumn col in dt.Columns)
                    {
                        var colItem = new Dictionary<string, object>();
                        colItem.SetValue("value", col.ColumnName);
                        colItem.SetValue("text", col.ColumnName);
                        result.Add(colItem);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 校验计算项是否满足过滤掉件，如果满足过滤条件则不参与计算
        /// </summary>
        /// <param name="calItem">计算项</param>
        /// <param name="formData">表单数据</param>
        /// <param name="subTableItem">子表行数据</param>
        /// <returns>如果满足过滤条件返回true,否则返回false</returns>
        public bool ValidateCancelCaculate(CalItem calItem, Dictionary<string, object> formData, Dictionary<string, object> subTableItem = null)
        {
            bool cancelCal = false;
            #region 校验过滤数据
            if (calItem.Filters.Count > 0)
            {
                var resultList = new List<Dictionary<string, object>>();
                foreach (var filter in calItem.Filters)
                {
                    var conditionResult = false;
                    var result = new Dictionary<string, object>();
                    result.SetValue("Field", filter.Field);
                    result.SetValue("Group", filter.Group);

                    #region 过滤值取自表单或子表数据
                    var propertyValue = string.Empty;
                    if (filter.Field.Split('.').Length == 2)
                    {
                        var subTableItemCode = filter.Field.Trim('{', '}').Split('.')[0];
                        var subItemCode = filter.Field.Trim('{', '}').Split('.')[1];
                        if (subTableItem == null)
                        {
                            continue;
                        }
                        propertyValue = subTableItem.GetValue(subItemCode);
                    }
                    else
                    {
                        propertyValue = formData.GetValue(filter.Field.Trim('{', '}'));
                    }
                    #endregion

                    #region 校验条件定义
                    if (propertyValue == null)
                        continue;
                    var condiftionValue = filter.Value;
                    switch (filter.QueryMode)
                    {
                        default:
                        case "IsNotNull":
                            conditionResult = !String.IsNullOrEmpty(propertyValue);
                            break;
                        case "IsNull":
                            conditionResult = String.IsNullOrEmpty(propertyValue);
                            break;
                        case "In":
                            conditionResult = condiftionValue.Split(',').Contains(propertyValue);
                            break;
                        case "Like":
                            conditionResult = propertyValue.Contains(condiftionValue);
                            break;
                        case "GreaterThanOrEqual":
                            conditionResult = Convert.ToDecimal(propertyValue) >= Convert.ToDecimal(condiftionValue);
                            break;
                        case "LessThanOrEqual":
                            conditionResult = Convert.ToDecimal(propertyValue) <= Convert.ToDecimal(condiftionValue);
                            break;
                        case "Equal":
                            conditionResult = propertyValue.Trim() == condiftionValue.Trim();
                            break;
                        case "LessThan":
                            conditionResult = Convert.ToDecimal(propertyValue) < Convert.ToDecimal(condiftionValue);
                            break;
                        case "GreaterThan":
                            conditionResult = Convert.ToDecimal(propertyValue) > Convert.ToDecimal(condiftionValue);
                            break;
                    }
                    #endregion
                    result.SetValue("Result", conditionResult);
                    resultList.Add(result);
                }

                var groupInfoList = resultList.Select(d => d.GetValue("Group")).Distinct().ToList();
                foreach (var groupInfo in groupInfoList)
                {
                    if (resultList.Where(d => d.GetValue("Group") == groupInfo).Count(d => !Convert.ToBoolean(d["Result"])) == 0)
                    {
                        cancelCal = true; break;
                    }
                }
            }
            #endregion
            return cancelCal;
        }

        /// <summary>
        /// 替换字符串中的占位符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public string ReplaceString(string str, Dictionary<string, object> formData, Dictionary<string, object> adpData)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            var user = FormulaHelper.GetUserInfo(); //validateCurrentUser ? FormulaHelper.GetUserInfo() : new UserInfo();
            string result = reg.Replace(str, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                if (adpData != null && adpData.ContainsKey(value))
                    return adpData.GetValue(value);
                if (formData != null && formData.ContainsKey(value))
                    return formData.GetValue(value);

                #region 系统环境变量
                switch (value)
                {
                    case Formula.Constant.CurrentUserID:
                        return user.UserID;
                    case Formula.Constant.CurrentUserName:
                        return user.UserName;
                    case Formula.Constant.CurrentUserOrgID:
                        return user.UserOrgID;
                    case Formula.Constant.CurrentUserOrgCode:
                        return user.UserOrgCode;
                    case Formula.Constant.CurrentUserOrgName:
                        return user.UserOrgName;
                    case Formula.Constant.CurrentUserPrjID:
                        return user.UserPrjID;
                    case Formula.Constant.CurrentUserPrjName:
                        return user.UserPrjName;
                    case "CurrentUserOrgFullName":
                        return user.UserFullOrgName;
                    case "CurrentUserCorpID":
                        return user.UserCompanyID;
                    case "CurrentUserCorpName":
                        return user.UserCompanyName;
                    case "CurrentTime":
                        return DateTime.Now.ToString();
                    case "CurrentDate":
                        return DateTime.Now.Date.ToString("yyyy-MM-dd");
                    case "CurrentYear":
                        return DateTime.Now.Year.ToString();
                    case "CurrentMonth":
                        return DateTime.Now.Month.ToString();
                    case "CurrentQuarter":
                        return ((DateTime.Now.Month + 2) / 3).ToString();
                    case "CurrentUserOrgFullID":
                        return user.UserFullOrgID;
                    case "CurrentUserWorkNo":
                        return user.WorkNo;
                    default:
                        return "";
                }
                #endregion

            });

            return result;
        }
        #endregion

        #region 私有方法
        string filterResultList(string listData, Dictionary<string, object> formData, Dictionary<string, object> itemDic,
            List<Dictionary<string, object>> refValueConditions)
        {
            var result = new List<Dictionary<string, object>>();
            try
            {
                result = JsonHelper.ToList(listData);
            }
            catch
            {
                return listData;
            }
            if (refValueConditions == null || refValueConditions.Count == 0)
                return listData;

            if (refValueConditions.Count > 0)
            {
                foreach (var item in refValueConditions)
                {
                    var value = "";
                    if (String.IsNullOrEmpty(item.GetValue("FieldCode")) && !String.IsNullOrEmpty(item.GetValue("ConditionValue")))
                    {
                        value = item.GetValue("ConditionValue");
                    }
                    else if (item.GetValue("FieldCode").Trim('{', '}').Split('.').Length == 1)
                    {
                        if (formData == null) continue;
                        value = formData.GetValue(item.GetValue("FieldCode").Trim('{', '}'));
                    }
                    else
                    {
                        if (item == null) continue;
                        value = itemDic.GetValue(item.GetValue("FieldCode").Trim('{', '}').Split('.')[1]);
                    }
                    var conField = item.GetValue("Value");
                    if (String.IsNullOrEmpty(conField))
                        continue;
                    result = result.Where(c => c.GetValue(conField) == value).ToList();
                }
            }
            return JsonHelper.ToJson(result);
        }

        string replaceFuncExpressionString(string expression, Dictionary<string, object> formDic,
            Dictionary<string, object> itemData,
            Dictionary<string, object> adpaterData,
            List<Dictionary<string, object>> subTableAdapter)
        {
            var result = funcReg.Replace(expression, (Match m) =>
            {
                var funcName = "";
                if (m.Value.StartsWith("Func_"))
                {
                    funcName = m.Value.Substring(5, m.Value.IndexOf("(") - 5);
                }
                if (funcName == SystemFunction.DX.ToString())
                {
                    #region 数字转大写函数调用
                    var paramReg = new Regex("\\(([\\d\\D]*)*\\)");
                    var match = paramReg.Match(m.Value);
                    if (match == null || String.IsNullOrEmpty(match.Value))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有匹配要【{0}】函数需要输入的参数，请检查计算式配置", funcName));
                    }
                    var param = replaceExpressionString(match.Value.Trim('(', ')'), formDic, itemData, adpaterData, subTableAdapter);
                    if (String.IsNullOrEmpty(param))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("输入的参数有错误，无法调用【{0}】的参数，请检查计算式配置", funcName));
                    }
                    try
                    {
                        return CalSysFunction.ArabiaToChinese(param);
                    }
                    catch (Exception exp)
                    {
                        LogWriter.Error(exp);
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("调用函数【{0}】失败，请检查参数【{1}】是否正确", funcName, m.Value));
                    }
                    #endregion
                }
                else if (funcName == SystemFunction.SubString.ToString())
                {
                    #region 字符串SubString函数
                    var paramReg = new Regex("\\(([\\d\\D]*)*\\)");
                    var match = paramReg.Match(m.Value);
                    if (match == null || String.IsNullOrEmpty(match.Value))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有匹配要【{0}】函数需要输入的参数，请检查计算式配置", funcName));
                    }
                    var param = replaceExpressionString(match.Value, formDic, itemData, adpaterData, subTableAdapter);
                    if (String.IsNullOrEmpty(param))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("输入的参数有错误，无法调用【{0}】的参数，请检查计算式配置", funcName));
                    }
                    var paramList = param.Split(',');
                    if (paramList.Length < 2)
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("输入的参数必须有2个，无法调用【{0}】的参数，请检查计算式配置", funcName));
                    }
                    int startPos = 0; int endPos = 0;
                    if (!int.TryParse(paramList[1], out startPos))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("输入的第二个参数必须是整数，无法调用【{0}】的参数，请检查计算式配置", funcName));
                    }
                    if (paramList.Length > 2)
                    {
                        if (!int.TryParse(paramList[2], out endPos))
                        {
                            throw new Formula.Exceptions.BusinessValidationException(String.Format("输入的第三个参数必须是整数，无法调用【{0}】的参数，请检查计算式配置", funcName));
                        }
                    }
                    if (endPos > 0)
                    {
                        return paramList[0].Substring(startPos, endPos);
                    }
                    else
                    {
                        return paramList[0].Substring(startPos);
                    }
                    #endregion
                }
                else if (funcName == SystemFunction.IsNotNull.ToString())
                {
                    #region 不为空
                    var paramReg = new Regex("\\(([\\d\\D]*)*\\)");
                    var match = paramReg.Match(m.Value);
                    if (match == null || String.IsNullOrEmpty(match.Value))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有匹配要【{0}】函数需要输入的参数，请检查计算式配置", funcName));
                    }
                    var param = replaceExpressionString(match.Value.Trim('(', ')'), formDic, itemData, adpaterData, subTableAdapter);
                    return (!String.IsNullOrEmpty(param)).ToString().ToLower();
                    #endregion
                }
                else if (funcName == SystemFunction.IsNull.ToString())
                {
                    #region 为空判定
                    var paramReg = new Regex("\\(([\\d\\D]*)*\\)");
                    var match = paramReg.Match(m.Value);
                    if (match == null || String.IsNullOrEmpty(match.Value))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有匹配要【{0}】函数需要输入的参数，请检查计算式配置", funcName));
                    }
                    var param = replaceExpressionString(match.Value.Trim('(', ')'), formDic, itemData, adpaterData, subTableAdapter);
                    return (String.IsNullOrEmpty(param)).ToString().ToLower();
                    #endregion
                }
                else if (funcName == SystemFunction.Sum.ToString())
                {
                    #region Sum函数计算
                    var paramReg = new Regex("\\(([\\d\\D]*)*\\)");
                    var match = paramReg.Match(m.Value);
                    if (match == null || String.IsNullOrEmpty(match.Value))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有匹配要【{0}】函数需要输入的参数，请检查计算式配置", funcName));
                    }
                    var paramInfos = match.Value.Trim('(', ')').Split(',');
                    if (paramInfos.Length < 2) throw new Formula.Exceptions.BusinessValidationException(String.Format("函数计算错误，Sum函数必须拥有两个参数，请检查参数是否输入正确并以逗号分隔"));
                    var param = replaceExpressionString(paramInfos[0], formDic, itemData, adpaterData, subTableAdapter);

                    if (param.Trim() == "0")
                    {
                        //SUM集合时，当集合时空的时候，会返回0,
                        return "0";
                    }
                    if (String.IsNullOrEmpty(param))
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有找到集合【{0}】，函数【{1}】配置有错误，请检查对应配置信息", match.Value.Trim('(', ')'), funcName));
                    try
                    {
                        var list = JsonHelper.ToList(param);
                        if (list.Count == 0) return "0";
                        if (paramInfos.Length >= 3)
                        {
                            var filter = this.replaceExpressionString(paramInfos[2].Trim('"', '"'), formDic, itemData, null, null);
                            list = this.filterList(list, filter);
                        }
                        var key = paramInfos[1].Trim('"', '"').Trim('\'', '\'');
                        var value = list.Sum(c => String.IsNullOrEmpty(c.GetValue(key)) ? 0m : Convert.ToDecimal(c.GetValue(key)));
                        return value.ToString();
                    }
                    catch (Exception exp)
                    {
                        LogWriter.Error(exp, "集合JSON转换出现错误，请确认需要计数的对象是一个集合");
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("集合【{0}】JSON转换出现错误，函数【{1}】配置有错误，请确认需要计数的对象是一个集合", match.Value.Trim('(', ')'), funcName));
                    }
                    #endregion
                }
                else if (funcName == SystemFunction.Count.ToString())
                {
                    #region Count函数计算
                    var paramReg = new Regex("\\(([\\d\\D]*)*\\)");
                    var match = paramReg.Match(m.Value);
                    if (match == null || String.IsNullOrEmpty(match.Value))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有匹配要【{0}】函数需要输入的参数，请检查计算式配置", funcName));
                    }
                    var param = replaceExpressionString(match.Value.Trim('(', ')'), formDic, itemData, adpaterData, subTableAdapter);
                    if (String.IsNullOrEmpty(param))
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有找到集合【{0}】，函数【{1}】配置有错误，请检查对应配置信息", match.Value.Trim('(', ')'), funcName));
                    try
                    {
                        var list = JsonHelper.ToList(param);
                        var paramInfos = match.Value.Trim('(', ')').Split(',');
                        if (paramInfos.Length > 1)
                        {
                            var filter = this.replaceExpressionString(paramInfos[1].Trim('"', '"'), formDic, itemData, null, null);
                            list = this.filterList(list, filter);
                        }
                        var value = list.Count;
                        return value.ToString();
                    }
                    catch (Exception exp)
                    {
                        LogWriter.Error(exp, "集合JSON转换出现错误，请确认需要计数的对象是一个集合");
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("集合【{0}】JSON转换出现错误，函数【{1}】配置有错误，请确认需要计数的对象是一个集合", match.Value.Trim('(', ')'), funcName));
                    }
                    #endregion
                }
                else if (funcName == SystemFunction.Avg.ToString())
                {
                    #region AVG函数计算
                    var paramReg = new Regex("\\(([\\d\\D]*)*\\)");
                    var match = paramReg.Match(m.Value);
                    if (match == null || String.IsNullOrEmpty(match.Value))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有匹配要【{0}】函数需要输入的参数，请检查计算式配置", funcName));
                    }
                    var paramInfos = match.Value.Trim('(', ')').Split(',');
                    if (paramInfos.Length < 2) throw new Formula.Exceptions.BusinessValidationException(String.Format("函数计算错误，AVG函数必须拥有两个参数，请检查参数是否输入正确并以逗号分隔"));
                    var param = replaceExpressionString(paramInfos[0], formDic, itemData, adpaterData, subTableAdapter);
                    if (String.IsNullOrEmpty(param))
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有找到集合【{0}】，函数【{1}】配置有错误，请检查对应配置信息", match.Value.Trim('(', ')'), funcName));
                    try
                    {
                        var list = JsonHelper.ToList(param);
                        if (list.Count == 0) return "0";
                        if (paramInfos.Length >= 3)
                        {
                            var filter = this.replaceExpressionString(paramInfos[2].Trim('"', '"'), formDic, itemData, null, null);
                            list = this.filterList(list, filter);
                        }
                        var key = paramInfos[1].Trim('"', '"').Trim('\'', '\'');
                        var value = list.Average(c => String.IsNullOrEmpty(c.GetValue(key)) ? 0m : Convert.ToDecimal(c.GetValue(key)));
                        return value.ToString();
                    }
                    catch (Exception exp)
                    {
                        LogWriter.Error(exp, "集合JSON转换出现错误，请确认需要计数的对象是一个集合");
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("集合【{0}】JSON转换出现错误，函数【{1}】配置有错误，请确认需要计数的对象是一个集合", match.Value.Trim('(', ')'), funcName));
                    }
                    #endregion
                }
                else if (funcName == SystemFunction.NewID.ToString())
                {
                    #region GUID函数计算
                    return FormulaHelper.CreateGuid();
                    #endregion
                }
                else if (funcName == SystemFunction.CombineSelect.ToString())
                {
                    #region CombineSelect函数计算
                    var paramReg = new Regex("\\(([\\d\\D]*)*\\)");
                    var match = paramReg.Match(m.Value);
                    if (match == null || String.IsNullOrEmpty(match.Value))
                    {
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有匹配要【{0}】函数需要输入的参数，请检查计算式配置", funcName));
                    }
                    var paramInfos = match.Value.Trim('(', ')').Split(',');
                    if (paramInfos.Length < 3) throw new Formula.Exceptions.BusinessValidationException(String.Format("函数计算错误，CombineSelect函数必须拥有至少三个参数，请检查参数是否输入正确并以逗号分隔"));
                    var param = replaceExpressionString(paramInfos[0], formDic, itemData, adpaterData, subTableAdapter);

                    if (String.IsNullOrEmpty(param))
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("没有找到集合【{0}】，函数【{1}】配置有错误，请检查对应配置信息", match.Value.Trim('(', ')'), funcName));
                    try
                    {
                        var list = JsonHelper.ToList(param);
                        if (list.Count == 0) return "";
                        if (paramInfos.Length >= 4)
                        {
                            var filter = this.replaceExpressionString(paramInfos[3].Trim('"', '"'), formDic, itemData, null, null);
                            list = this.filterList(list, filter);
                        }
                        var key = paramInfos[1].Trim('"', '"').Trim('\'', '\'');
                        var splitParam = paramInfos[2].Trim('"', '"').Trim('\'', '\'');
                        var combinValues = list.Where(c => !String.IsNullOrEmpty(c.GetValue(key))).Select(c => c.GetValue(key)).ToList();
                        var value = String.Join(splitParam, combinValues);
                        return value.ToString();
                    }
                    catch (Exception exp)
                    {
                        LogWriter.Error(exp, "集合JSON转换出现错误，请确认需要计数的对象是一个集合");
                        throw new Formula.Exceptions.BusinessValidationException(String.Format("集合【{0}】JSON转换出现错误，函数【{1}】配置有错误，请确认需要计数的对象是一个集合", match.Value.Trim('(', ')'), funcName));
                    }
                    #endregion
                }
                else
                {
                    throw new Formula.Exceptions.BusinessValidationException(String.Format("目前尚不支持【{0}】函数", funcName));
                }
            });
            return result;
        }

        List<Dictionary<string, object>> filterList(List<Dictionary<string, object>> listData, string filter)
        {
            var result = new List<Dictionary<string, object>>();
            if (!String.IsNullOrEmpty(filter))
            {

                var dt = JsonHelper.ToObject<DataTable>(JsonHelper.ToJson(listData));
                var rows = dt.Select(filter);
                foreach (DataRow row in rows)
                {
                    result.Add(FormulaHelper.DataRowToDic(row));
                }
            }
            else
            {
                result = listData;
            }
            return result;
        }

        string replaceExpressionString(string expression, Dictionary<string, object> formDic,
            Dictionary<string, object> itemData,
            Dictionary<string, object> adpaterData,
            List<Dictionary<string, object>> subTableAdapter)
        {
            var result = reg.Replace(expression, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                var row = this.AllParamDt.Rows.Find(value);
                if (value.StartsWith("Field_"))
                {
                    var fieldKey = value.Substring(6);
                    if (fieldKey.Split('.').Length == 1)
                    {
                        if (formDic != null)
                            return String.IsNullOrEmpty(formDic.GetValue(value.Substring(6))) ? "0" : formDic.GetValue(value.Substring(6));
                        else
                            return "0";
                    }
                    else if (fieldKey.Split('.').Length == 2)
                    {
                        if (itemData != null && itemData.Keys.Contains(fieldKey.Split('.')[1]))
                            return String.IsNullOrEmpty(itemData.GetValue(fieldKey.Split('.')[1])) ? "0" : itemData.GetValue(fieldKey.Split('.')[1]);
                        else if (formDic != null)
                            return String.IsNullOrEmpty(formDic.GetValue(fieldKey.Split('.')[1])) ? "0" : formDic.GetValue(fieldKey.Split('.')[1]);
                        else
                            return "0";
                    }
                    else
                    {
                        return "0";
                    }
                }
                else if (row != null && row["ParamType"] != null && row["ParamType"] != DBNull.Value && row["ParamType"].ToString() == "CalArg")
                {
                    if (row["Expression"] == null || row["Expression"] == DBNull.Value || String.IsNullOrEmpty(row["Expression"].ToString()))
                        throw new Formula.Exceptions.BusinessValidationException("参数【" + row["Code"] + "】没有定义计算表达式，无法解析公式");
                    return replaceExpressionString(row["Expression"].ToString(), formDic, itemData, adpaterData, subTableAdapter);
                }
                else if (row != null && row["ParamType"] != null && row["ParamType"] != DBNull.Value && row["ParamType"].ToString() == "RefArg")
                {
                    var obj = RefParamCollection.GetObject(value);
                    if (obj == null || obj == DBNull.Value)
                    {
                        if (row["SQL"] == null || row["SQL"] == DBNull.Value || String.IsNullOrEmpty(row["SQL"].ToString()))
                            throw new Formula.Exceptions.BusinessValidationException("参数【" + row["Code"] + "】没有定义取值SQL，无法解析公式");
                        if (row["ConnName"] == null || row["ConnName"] == DBNull.Value || String.IsNullOrEmpty(row["ConnName"].ToString()))
                            throw new Formula.Exceptions.BusinessValidationException("参数【" + row["Code"] + "】没有定义取值的数据库链接，无法解析公式");
                        var db = SQLHelper.CreateSqlHelper(row["ConnName"].ToString());
                        var sql = this.ReplaceString(row["SQL"].ToString(), formDic, adpaterData);
                        if (row["OrderBy"] != null || row["OrderBy"] != DBNull.Value)
                        {
                            sql += " " + row["OrderBy"].ToString();
                        }
                        if (row["IsCollectionRef"].ToString() == true.ToString() || row["IsCollectionRef"].ToString() == "1")
                        {
                            obj = db.ExecuteDataTable(sql);
                        }
                        else
                        {
                            obj = db.ExecuteScalar(sql);
                            if (obj == null || obj == DBNull.Value) obj = 0m;
                        }
                        this.RefParamCollection.Add(value, obj);
                    }
                    if (row["IsCollectionRef"].ToString() == true.ToString() || row["IsCollectionRef"].ToString() == "1")
                    {
                        var dt = obj as DataTable;
                        if (subTableAdapter == null || subTableAdapter.Count == 0)
                        {
                            return JsonHelper.ToJson(dt);
                        }
                        #region 根据字段配置定义，适配匹配字段
                        var list = new List<Dictionary<string, object>>();
                        foreach (DataRow resultRow in dt.Rows)
                        {
                            var resultItem = new Dictionary<string, object>();
                            foreach (var adpItem in subTableAdapter)
                            {
                                var fieldCode = adpItem.GetValue("FieldCode");
                                var adpField = adpItem.GetValue("AdpField");
                                if (adpField.Split(',').Length > 1)
                                {
                                    var IDField = adpField.Split(',')[0];
                                    var NameField = adpField.Split(',')[1];
                                    if ((resultRow[IDField] == null || resultRow[IDField] == DBNull.Value || String.IsNullOrEmpty(resultRow[IDField].ToString()))
                                        && !String.IsNullOrEmpty(adpItem.GetValue("DefaultValue")))
                                    {
                                        if (adpItem.GetValue("DefaultValue").Split(',').Length > 1)
                                        {
                                            resultItem.SetValue(fieldCode, adpItem.GetValue("DefaultValue").Split(',')[0]);
                                            resultItem.SetValue(fieldCode + "Name", adpItem.GetValue("DefaultValue").Split(',')[1]);
                                        }
                                        else
                                        {
                                            resultItem.SetValue(fieldCode, adpItem.GetValue("DefaultValue"));
                                            resultItem.SetValue(fieldCode + "Name", adpItem.GetValue("DefaultValue"));
                                        }
                                    }
                                    else
                                    {
                                        if (String.IsNullOrEmpty(IDField) || !dt.Columns.Contains(IDField))
                                        {
                                            if (dt.Columns.Contains(fieldCode) && resultRow[fieldCode] != null && resultRow[fieldCode] != DBNull.Value)
                                            {
                                                resultItem.SetValue(fieldCode, resultRow[fieldCode]);
                                            }
                                            else
                                            {
                                                resultItem.SetValue(fieldCode, "");
                                            }
                                        }
                                        else
                                        {
                                            resultItem.SetValue(fieldCode, resultRow[IDField]);
                                        }

                                        if (String.IsNullOrEmpty(NameField) || !dt.Columns.Contains(NameField))
                                        {
                                            if (dt.Columns.Contains(fieldCode + "Name") && resultRow[fieldCode + "Name"] != null
                                                && resultRow[fieldCode + "Name"] != DBNull.Value)
                                            {
                                                resultItem.SetValue(fieldCode + "Name", resultRow[fieldCode + "Name"]);
                                            }
                                            else
                                            {
                                                resultItem.SetValue(fieldCode + "Name", "");
                                            }
                                        }
                                        else
                                        {
                                            resultItem.SetValue(fieldCode + "Name", resultRow[NameField]);
                                        }
                                    }
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(adpField) || !dt.Columns.Contains(adpField))
                                    {
                                        if (dt.Columns.Contains(fieldCode) && resultRow[fieldCode] != null && resultRow[fieldCode] != DBNull.Value)
                                        {
                                            resultItem.SetValue(fieldCode, resultRow[fieldCode]);
                                        }
                                        else
                                        {
                                            resultItem.SetValue(fieldCode, "");
                                        }
                                    }
                                    else
                                    {
                                        if ((resultRow[adpField] == null || resultRow[adpField] == DBNull.Value || String.IsNullOrEmpty(resultRow[adpField].ToString()))
                                     && !String.IsNullOrEmpty(adpItem.GetValue("DefaultValue")))
                                        {
                                            resultItem.SetValue(fieldCode, adpItem.GetValue("DefaultValue"));
                                        }
                                        else
                                        {
                                            resultItem.SetValue(fieldCode, resultRow[adpField]);
                                        }
                                    }
                                }
                            }
                            //ID 要设置为空，否则表单定义获取列表后，会无法保存
                            resultItem.SetValue("ID", "");
                            list.Add(resultItem);
                        }
                        #endregion
                        return JsonHelper.ToJson(list);
                    }
                    return obj.ToString();
                }
                else if (!String.IsNullOrEmpty(adpaterData.GetValue(value)))
                {
                    return adpaterData.GetValue(value);
                }
                else if (formDic != null || formDic.ContainsKey(value))
                    return formDic.GetValue(value);

                #region 系统环境变量
                switch (value)
                {
                    case Formula.Constant.CurrentUserID:
                        return user.UserID;
                    case Formula.Constant.CurrentUserName:
                        return user.UserName;
                    case Formula.Constant.CurrentUserOrgID:
                        return user.UserOrgID;
                    case Formula.Constant.CurrentUserOrgCode:
                        return user.UserOrgCode;
                    case Formula.Constant.CurrentUserOrgName:
                        return user.UserOrgName;
                    case Formula.Constant.CurrentUserPrjID:
                        return user.UserPrjID;
                    case Formula.Constant.CurrentUserPrjName:
                        return user.UserPrjName;
                    case "CurrentUserOrgFullName":
                        return user.UserFullOrgName;
                    case "CurrentUserCorpID":
                        return user.UserCompanyID;
                    case "CurrentUserCorpName":
                        return user.UserCompanyName;
                    case "CurrentTime":
                        return DateTime.Now.ToString();
                    case "CurrentDate":
                        return DateTime.Now.Date.ToString("yyyy-MM-dd");
                    case "CurrentYear":
                        return DateTime.Now.Year.ToString();
                    case "CurrentMonth":
                        return DateTime.Now.Month.ToString();
                    case "CurrentQuarter":
                        return ((DateTime.Now.Month + 2) / 3).ToString();
                    case "CurrentUserOrgFullID":
                        return user.UserFullOrgID;
                    case "CurrentUserWorkNo":
                        return user.WorkNo;
                    default:
                        return "";
                }
                #endregion
            });
            return result;
        }
        #endregion
    }


    public class CalSysFunction
    {
        /// <summary>
        /// 金额转为大写金额
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string ArabiaToChinese(string money)
        {
            string functionReturnValue = null;
            bool IsNegative = false; // 是否是负数
            if (money.Trim().Substring(0, 1) == "-")
            {
                // 是负数则先转为正数
                money = money.Trim().Remove(0, 1);
                IsNegative = true;
            }
            string strLower = null;
            string strUpart = null;
            string strUpper = null;
            int iTemp = 0;
            // 保留两位小数 123.489→123.49　　123.4→123.4
            money = Math.Round(double.Parse(money), 2).ToString();
            if (money.IndexOf(".") > 0)
            {

                if (money.IndexOf(".") == money.Length - 2)
                {
                    money = money + "0";
                }
            }
            else
            {
                money = money + ".00";
            }
            strLower = money;
            iTemp = 1;
            strUpper = "";
            while (iTemp <= strLower.Length)
            {
                switch (strLower.Substring(strLower.Length - iTemp, 1))
                {
                    case ".":
                        strUpart = "圆";
                        break;
                    case "0":
                        strUpart = "零";
                        break;
                    case "1":
                        strUpart = "壹";
                        break;
                    case "2":
                        strUpart = "贰";
                        break;
                    case "3":
                        strUpart = "叁";
                        break;
                    case "4":
                        strUpart = "肆";
                        break;
                    case "5":
                        strUpart = "伍";
                        break;
                    case "6":
                        strUpart = "陆";
                        break;
                    case "7":
                        strUpart = "柒";
                        break;
                    case "8":
                        strUpart = "捌";
                        break;
                    case "9":
                        strUpart = "玖";
                        break;
                }
                switch (iTemp)
                {
                    case 1:
                        strUpart = strUpart + "分";
                        break;
                    case 2:
                        strUpart = strUpart + "角";
                        break;
                    case 3:
                        strUpart = strUpart + "";
                        break;
                    case 4:
                        strUpart = strUpart + "";
                        break;
                    case 5:
                        strUpart = strUpart + "拾";
                        break;
                    case 6:
                        strUpart = strUpart + "佰";
                        break;
                    case 7:
                        strUpart = strUpart + "仟";
                        break;
                    case 8:
                        strUpart = strUpart + "万";
                        break;
                    case 9:
                        strUpart = strUpart + "拾";
                        break;
                    case 10:
                        strUpart = strUpart + "佰";
                        break;
                    case 11:
                        strUpart = strUpart + "仟";
                        break;
                    case 12:
                        strUpart = strUpart + "亿";
                        break;
                    case 13:
                        strUpart = strUpart + "拾";
                        break;
                    case 14:
                        strUpart = strUpart + "佰";
                        break;
                    case 15:
                        strUpart = strUpart + "仟";
                        break;
                    case 16:
                        strUpart = strUpart + "万";
                        break;
                    default:
                        strUpart = strUpart + "";
                        break;
                }
                strUpper = strUpart + strUpper;
                iTemp = iTemp + 1;
            }
            strUpper = strUpper.Replace("零拾", "零");
            strUpper = strUpper.Replace("零佰", "零");
            strUpper = strUpper.Replace("零仟", "零");
            strUpper = strUpper.Replace("零零零", "零");
            strUpper = strUpper.Replace("零零", "零");
            strUpper = strUpper.Replace("零角零分", "整");
            strUpper = strUpper.Replace("零分", "整");
            strUpper = strUpper.Replace("零角", "零");
            strUpper = strUpper.Replace("零亿零万零圆", "亿圆");
            strUpper = strUpper.Replace("亿零万零圆", "亿圆");
            strUpper = strUpper.Replace("零亿零万", "亿");
            strUpper = strUpper.Replace("零万零圆", "万圆");
            strUpper = strUpper.Replace("零亿", "亿");
            strUpper = strUpper.Replace("零万", "万");
            strUpper = strUpper.Replace("零圆", "圆");
            strUpper = strUpper.Replace("零零", "零");
            // 对壹圆以下的金额的处理
            if (strUpper.Substring(0, 1) == "圆")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "零")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "角")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "分")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "整")
            {
                strUpper = "零圆整";
            }
            functionReturnValue = strUpper;
            if (IsNegative == true)
            {
                return "负" + functionReturnValue;
            }
            else
            {
                return functionReturnValue;
            }
        }

        public static object SumValue(object listData, string sumField)
        {
            var data = new DataTable();
            if (listData is List<Dictionary<string, object>>)
            {
                data = ListToDataTable(listData as List<Dictionary<string, object>>);
            }
            else if (listData is String)
            {
                var list = JsonHelper.ToList(listData as string);
                data = ListToDataTable(list);
            }
            else if (listData is DataTable)
            {
                data = listData as DataTable;
            }
            else
            {
                throw new Formula.Exceptions.BusinessValidationException("集合数据只能是DataTable或字典集合或可转字典集合的JSON字符串");
            }
            try
            {
                return data.Compute("Sum(" + sumField + ")", "");
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp, String.Format("计算错误，进检查计算输入列【{0}】是数字格式", sumField));
            }
            return 0;
        }

        public static object AvgValue(object listData, string avgField)
        {
            var data = new DataTable();
            if (listData is List<Dictionary<string, object>>)
            {
                data = ListToDataTable(listData as List<Dictionary<string, object>>);
            }
            else if (listData is String)
            {
                var list = JsonHelper.ToList(listData as string);
                data = ListToDataTable(list);
            }
            else if (listData is DataTable)
            {
                data = listData as DataTable;
            }
            else
            {
                throw new Formula.Exceptions.BusinessValidationException("集合数据只能是DataTable或字典集合或可转字典集合的JSON字符串");
            }
            try
            {
                return data.Compute("Avg(" + avgField + ")", "");
            }
            catch (Exception exp)
            {
                LogWriter.Error(exp, String.Format("计算错误，进检查计算输入列【{0}】是数字格式", avgField));
            }
            return 0;
        }

        public static object CountValue(object listData)
        {
            var data = new DataTable();
            if (listData is List<Dictionary<string, object>>)
            {
                return (listData as List<Dictionary<string, object>>).Count;
                //data = ListToDataTable(listData as List<Dictionary<string, object>>);
            }
            else if (listData is String)
            {
                var list = JsonHelper.ToList(listData as string);
                return list.Count;
            }
            else if (listData is DataTable)
            {
                data = listData as DataTable;
                return data.Rows.Count;
            }
            else
            {
                throw new Formula.Exceptions.BusinessValidationException("集合数据只能是DataTable或字典集合或可转字典集合的JSON字符串");
            }
        }

        static DataTable ListToDataTable(List<Dictionary<string, object>> listData)
        {
            var result = new DataTable();
            foreach (var item in listData)
            {
                foreach (var key in item.Keys)
                {
                    if (!result.Columns.Contains(key))
                    {
                        result.Columns.Add(key);
                    }
                }
            }
            foreach (var item in listData)
            {
                var row = result.NewRow();
                foreach (var key in item.Keys)
                {
                    row[key] = item[key];
                }
                result.Rows.Add(row);
            }
            return result;
        }
    }

    public enum ArgumentType
    {
        [Description("输入参数")]
        InputArg,
        [Description("取值参数")]
        RefArg,
        [Description("计算参数")]
        CalArg,
        [Description("动态计算参数")]
        DynCalArg,
        [Description("数据源参数")]
        DSArg
    }

    public enum CalType
    {
        [Description("集合")]
        Collection,
        [Description("字符")]
        String,
        [Description("数字")]
        Decimal,
        [Description("逻辑编码")]
        Logic
    }

    public enum TriggerMethod
    {
        [Description("按钮计算")]
        Button,
        [Description("自动计算")]
        InputChange
    }

    public enum SystemFunction
    {
        [Description("数字转大写")]
        DX,
        [Description("字符截取")]
        SubString,
        [Description("为空")]
        IsNull,
        [Description("不为空")]
        IsNotNull,
        [Description("求和")]
        Sum,
        [Description("平均值")]
        Avg,
        [Description("计数")]
        Count,
        [Description("合并查询")]
        CombineSelect,
        [Description("GUID")]
        NewID
    }
}
