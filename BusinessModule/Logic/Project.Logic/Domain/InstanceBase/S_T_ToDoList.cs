using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config;
using Formula;
using Formula.Helper;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;

namespace Project.Logic.Domain
{
    public partial class S_T_ToDoList
    {
        public void ValidateExe()
        {
            var infransContext = FormulaHelper.GetEntities<ProjectEntities>();
            var projectConfigContext = FormulaHelper.GetEntities<BaseConfigEntities>();
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var projectInfoDt = db.ExecuteDataTable("select * from S_I_ProjectInfo where ID='" + this.ProjectInfoID + "'");

            var defineNode = projectConfigContext.S_T_ToDoListDefineNode.Find(this.DefineNodeID);
            var canExec = true;
            string hint = "未能满足执行条件设置，无法执行待办事项";
            if (defineNode != null && projectInfoDt.Rows.Count > 0)
            {
                if (!String.IsNullOrEmpty(defineNode.ExecCondition))
                {
                    var execConditionList = JsonHelper.ToList(defineNode.ExecCondition);
                    if (execConditionList.Count > 0)
                    {
                        canExec = false;
                        var groups = execConditionList.Select(c => c["GroupName"]).Distinct().ToList();
                        foreach (var groupName in groups)
                        {
                            if (canExec)
                            {
                                continue;
                            }
                            var items = execConditionList.Where(c => c["GroupName"] == groupName).ToList();
                            foreach (var condition in execConditionList)
                            {
                                if (String.IsNullOrEmpty(condition["SQLCondition"].ToString()))
                                    continue;
                                var sql = condition["SQLCondition"].ToString();
                                sql = ReplaceString(sql, projectInfoDt.Rows[0]);
                                var dt = db.ExecuteDataTable(sql);
                                string logicType = String.IsNullOrEmpty(condition["LogicType"].ToString()) ? "Exist" : condition["LogicType"].ToString();
                                if (logicType == "Exist")
                                {
                                    if (dt.Rows.Count == 0)
                                    {
                                        canExec = false;
                                        if (!String.IsNullOrEmpty(condition["Hint"].ToString()))
                                        {
                                            hint = condition["Hint"].ToString();
                                        }
                                        continue;
                                    }
                                    else
                                    {
                                        canExec = true;
                                    }
                                }
                                else
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        canExec = false;
                                        if (!String.IsNullOrEmpty(condition["Hint"].ToString()))
                                        {
                                            hint = condition["Hint"].ToString();
                                        }
                                        continue;
                                    }
                                    else
                                    {
                                        canExec = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!canExec)
            {
                throw new Formula.Exceptions.BusinessValidationException(hint);
            }
        }

        public void SetFinish()
        {
            var projectContext = FormulaHelper.GetEntities<ProjectEntities>();
            var projectConfigContext = FormulaHelper.GetEntities<BaseConfigEntities>();

            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var projectInfoDt = db.ExecuteDataTable("select * from S_I_ProjectInfo where ID='" + this.ProjectInfoID + "'");

            var defineNode = projectConfigContext.S_T_ToDoListDefineNode.Find(this.DefineNodeID);

            var canClose = true;
            string hint = "未能满足关闭条件设置，无法关闭待办事项";
            if (defineNode != null && projectInfoDt.Rows.Count > 0)
            {
                if (!String.IsNullOrEmpty(defineNode.CloseCondition))
                {
                    var closeConditionList = JsonHelper.ToList(defineNode.CloseCondition);
                    if (closeConditionList.Count > 0)
                    {
                        canClose = false;
                        var groups = closeConditionList.Select(c => c["GroupName"]).Distinct().ToList();
                        foreach (var groupName in groups)
                        {
                            if (canClose)
                            {
                                continue;
                            }
                            var items = closeConditionList.Where(c => c["GroupName"] == groupName).ToList();
                            foreach (var condition in closeConditionList)
                            {
                                if (String.IsNullOrEmpty(condition["SQLCondition"].ToString()))
                                    continue;
                                var sql = condition["SQLCondition"].ToString();
                                sql = ReplaceString(sql, projectInfoDt.Rows[0]);
                                var dt = db.ExecuteDataTable(sql);
                                string logicType = String.IsNullOrEmpty(condition["LogicType"].ToString()) ? "Exist" : condition["LogicType"].ToString();
                                if (logicType == "Exist")
                                {
                                    if (dt.Rows.Count == 0)
                                    {
                                        canClose = false;
                                        if (!String.IsNullOrEmpty(condition["Hint"].ToString()))
                                        {
                                            hint = condition["Hint"].ToString();
                                        }
                                        continue;
                                    }
                                    else
                                    {
                                        canClose = true;
                                    }
                                }
                                else
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        canClose = false;
                                        if (!String.IsNullOrEmpty(condition["Hint"].ToString()))
                                        {
                                            hint = condition["Hint"].ToString();
                                        }
                                        continue;
                                    }
                                    else
                                    {
                                        canClose = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!canClose)
            {
                throw new Formula.Exceptions.BusinessValidationException(hint);
            }
            this.State = ProjectCommoneState.Finish.ToString();
        }



        /// <summary>
        /// 替换{}内容为当前地址栏参数或当前人信息
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public string ReplaceString(string sql, DataRow row = null, Dictionary<string, string> dic = null, Dictionary<string, DataTable> dtDic = null)
        {
            if (string.IsNullOrEmpty(sql))
                return sql;

            var user = FormulaHelper.GetUserInfo();
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            string result = reg.Replace(sql, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');

                if (!string.IsNullOrEmpty(HttpContext.Current.Request[value]))
                    return HttpContext.Current.Request[value];

                if (dtDic != null && dtDic.Count > 0)
                {
                    var arr = value.Split('.');
                    if (arr.Length == 1)
                    {
                        if (dtDic.ContainsKey(value)) //默认值为整个表
                            return JsonHelper.ToJson(dtDic[value]);
                    }
                    else if (arr.Length == 2) //默认子编号名.字段名
                    {
                        if (dtDic.ContainsKey(arr[0]))
                        {
                            var dt = dtDic[arr[0]];
                            if (dt.Rows.Count > 0 && dt.Columns.Contains(arr[1]))
                            {
                                return dt.Rows[0][arr[1]].ToString();
                            }
                        }
                    }

                }
                if (row != null && row.Table.Columns.Contains(value))
                    return row[value].ToString();
                if (dic != null && dic.ContainsKey(value))
                    return dic[value];

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
                    default:
                        return "";
                }
            });

            return result;
        }


    }
}
