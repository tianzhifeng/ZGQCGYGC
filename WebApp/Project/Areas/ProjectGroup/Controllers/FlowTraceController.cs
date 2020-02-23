
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Formula;
using Project.Logic;
using Project.Logic.Domain;
using Config.Logic;
using Config;
using Formula.Helper;
using System.Data;
using System.Text.RegularExpressions;

namespace Project.Areas.ProjectGroup.Controllers
{
    public class FlowTraceController : ProjectController
    {
        public ActionResult ProjectList()
        {
            var tab = new Tab();
            var deptCategory = CategoryFactory.GetCategory("Market.ManDept", "责任部门", "ChargeDeptID");
            deptCategory.SetDefaultItem();
            deptCategory.Multi = false;
            tab.Categories.Add(deptCategory);

            var phaseCategory = CategoryFactory.GetCategory("Project.Phase", "设计阶段", "PhaseValue");
            phaseCategory.SetDefaultItem();
            phaseCategory.Multi = false;
            tab.Categories.Add(phaseCategory);
            var RelativeArea = CategoryFactory.GetCategory("Base.ProjectClass", "业务类型", "ProjectClass");
            RelativeArea.SetDefaultItem();
            RelativeArea.Multi = false;
            tab.Categories.Add(RelativeArea);

            var state = CategoryFactory.GetCategory(typeof(Project.Logic.ProjectCommoneState), "项目状态", "State", true);
            state.SetDefaultItem();
            state.Multi = true;
            tab.Categories.Add(state);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetList(MvcAdapter.QueryBuilder qb)
        {
            var data = this.entities.Set<S_I_ProjectInfo>().WhereToGridData(qb);
            return Json(data);
        }

        public JsonResult GetTreeList(string ProjectInfoID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null)
            {
                throw new Formula.Exceptions.BusinessException("未能找到指定的项目信息");
            }
            var defineList = projectInfo.ProjectMode.S_T_FlowTraceDefine.OrderBy(d => d.SortIndex).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var rootDefine in defineList.Where(d => String.IsNullOrEmpty(d.ParentID)).ToList())
            {
                var item = Formula.FormulaHelper.ModelToDic<S_T_FlowTraceDefine>(rootDefine);
                result.Add(item);
                if (rootDefine.Type == FlowTraceDefineNodeType.FlowNode.ToString())
                {
                    var list = _createFormItem(rootDefine, ProjectInfoID);
                    item.SetValue("FlowCount", list.Count(d => d["Type"].ToString() == "Form"));
                    item.SetValue("FinishCount", list.Count(d => d["Type"].ToString() == "Form" && d["FlowPhase"].ToString() == "End"));
                    item.SetValue("UnFinishCount", list.Count(d => d["Type"].ToString() == "Form" && d["FlowPhase"].ToString() != "End"));
                    result.AddRange(list);
                }
                else
                {
                    _createNode(rootDefine, result, defineList, ProjectInfoID);
                }
            }
            var data = new Dictionary<string, object>();
            data.SetValue("data", result);
            return Json(result);
        }

        void _createNode(S_T_FlowTraceDefine parentNode, List<Dictionary<string, object>> result, List<S_T_FlowTraceDefine> defineList, string ProjectInfoID)
        {
            var children = defineList.Where(d => d.ParentID == parentNode.ID).OrderBy(d => d.SortIndex).ToList();
            foreach (var child in children)
            {
                var item = Formula.FormulaHelper.ModelToDic<S_T_FlowTraceDefine>(child);
                item.SetValue("SearchFieldInfo", parentNode.Name + item.GetValue("Name"));
                result.Add(item);
                if (child.Type == FlowTraceDefineNodeType.FlowNode.ToString())
                {
                    var list = _createFormItem(child, ProjectInfoID);
                    item.SetValue("FlowCount", list.Count(d => d["Type"].ToString() == "Form"));
                    item.SetValue("FinishCount", list.Count(d => d["Type"].ToString() == "Form" && d["FlowPhase"].ToString() == "End"));
                    item.SetValue("UnFinishCount", list.Count(d => d["Type"].ToString() == "Form" && d["FlowPhase"].ToString() != "End"));
                    result.AddRange(list);
                }
                else
                {
                    _createNode(child, result, defineList, ProjectInfoID);
                }
            }
        }

        List<Dictionary<string, object>> _createFormItem(S_T_FlowTraceDefine formDefine, string ProjectInfoID)
        {
            if (formDefine == null) throw new Formula.Exceptions.BusinessException("");
            var reslut = new List<Dictionary<string, object>>();
            var db = SQLHelper.CreateSqlHelper(formDefine.ConnName);
            if (String.IsNullOrWhiteSpace(formDefine.TableName)) return reslut;
            var sql = "select * from " + formDefine.TableName + " where ProjectInfoID='" + ProjectInfoID + "'";
            var dt = db.ExecuteDataTable(sql);
            if (String.IsNullOrEmpty(formDefine.NameFieldInfo) && !dt.Columns.Contains("Name"))
                throw new Formula.Exceptions.BusinessException("【" + formDefine.Name + "】定义配置中，数据源SQL必须定义NAME列");
            if (!dt.Columns.Contains("ID")) { throw new Formula.Exceptions.BusinessException("【" + formDefine.Name + "】定义配置中，数据源SQL必须定义ID列"); }
            foreach (DataRow row in dt.Rows)
            {
                var formItem = new Dictionary<string, object>();
                formItem.SetValue("ParentID", formDefine.ID);
                formItem.SetValue("ID", row["ID"].ToString());
                if (string.IsNullOrEmpty(formDefine.NameFieldInfo.Trim()))
                    formItem.SetValue("Name", row["Name"].ToString());
                else
                {
                    var enumDefList = new List<Dictionary<string, object>>();
                    if (!String.IsNullOrEmpty(formDefine.EnumFieldInfo))
                        enumDefList = JsonHelper.ToList(formDefine.EnumFieldInfo);
                    string name = this.replaceNameString(formDefine.NameFieldInfo, FormulaHelper.DataRowToDic(row), enumDefList);
                    formItem.SetValue("Name", name);
                    formItem.SetValue("SearchFieldInfo", formDefine.Name + name);
                }
                #region 设置流程节点的属性
                formItem.SetValue("Type", "Form");
                if (dt.Columns.Contains("VersionNumber"))
                {
                    formItem.SetValue("VersionNumber", row["VersionNumber"]);
                }
                if (dt.Columns.Contains("FlowPhase"))
                {
                    formItem.SetValue("FlowPhase", row["FlowPhase"]);
                }
                if (dt.Columns.Contains("StepName"))
                {
                    formItem.SetValue("StepName", row["StepName"]);
                }
                if (!String.IsNullOrWhiteSpace(formDefine.LinkFormUrl))
                {
                    var url = "";
                    if (formDefine.LinkFormUrl.IndexOf("?") >= 0)
                    {
                        url = formDefine.LinkFormUrl + "&ID=" + row["ID"].ToString() + "&FuncType=View";
                    }
                    else
                    {
                        url = formDefine.LinkFormUrl + "?ID=" + row["ID"].ToString() + "&FuncType=View";
                    }
                    formItem.SetValue("LinkUrl", url);
                }
                #endregion

                #region  设置流程下的各环节审批信息
                if (dt.Columns.Contains("FlowInfo"))
                {
                    string flowInfo = row["FlowInfo"].ToString();
                    if (!String.IsNullOrEmpty(flowInfo))
                    {
                        var flowInfoDic = JsonHelper.ToObject(flowInfo);
                        formItem.SetValue("CurrentUser", flowInfoDic.GetValue("CurrentStepUser"));
                        formItem.SetValue("CurrentUserID", flowInfoDic.GetValue("CurrentStepUserID"));
                        formItem.SetValue("StartUser", flowInfoDic.GetValue("StartUser"));
                        formItem.SetValue("StartUserID", flowInfoDic.GetValue("StartUserID"));
                        formItem.SetValue("StartDate", flowInfoDic.GetValue("StartDate"));
                        formItem.SetValue("EndDate", flowInfoDic.GetValue("EndDate"));
                        var taskExeList = JsonHelper.ToList(flowInfoDic.GetValue("DetailInfo"));
                        foreach (var taskInfo in taskExeList)
                        {
                            var taskDic = new Dictionary<string, object>();
                            taskDic.SetValue("Name", taskInfo.GetValue("StepName"));
                            taskDic.SetValue("CurrentUser", taskInfo.GetValue("TaskUserNames"));
                            taskDic.SetValue("CurrentUserID", taskInfo.GetValue("TaskUserIDs"));
                            taskDic.SetValue("StartDate", taskInfo.GetValue("StartDate"));
                            taskDic.SetValue("EndDate", taskInfo.GetValue("EndDate"));
                            taskDic.SetValue("FlowPhase", taskInfo.GetValue("Status"));
                            taskDic.SetValue("Status", taskInfo.GetValue("Status"));
                            taskDic.SetValue("ParentID", formItem.GetValue("ID"));
                            taskDic.SetValue("SearchFieldInfo", formItem.GetValue("SearchFieldInfo"));
                            taskDic.SetValue("Type", "Task");
                            if (String.IsNullOrEmpty(taskInfo.GetValue("ID")))
                            {
                                taskDic.SetValue("ID", taskInfo.GetValue("StepName"));
                            }
                            else
                            {
                                taskDic.SetValue("ID", taskInfo.GetValue("ID"));
                            }
                            reslut.Add(taskDic);
                        }
                    }
                #endregion
                }
                reslut.Add(formItem);
            }
            return reslut;
        }

        string replaceNameString(string regString, Dictionary<string, object> data, List<Dictionary<string, object>> EnumDefine)
        {
            Regex reg = new Regex("\\{[0-9a-zA-Z_\\.]*\\}");
            string result = reg.Replace(regString, (Match m) =>
            {
                string value = m.Value.Trim('{', '}');
                var sValue = data.GetValue(value);
                var define = EnumDefine.FirstOrDefault(d => d["FieldName"].ToString() == value);
                if (define != null)
                {
                    var enumValue = "";
                    var EnumService = FormulaHelper.GetService<IEnumService>();
                    foreach (var item in sValue.Split(','))
                    {
                        enumValue += EnumService.GetEnumText(define.GetValue("EnumKey"), item) + ",";
                    }
                    return enumValue.TrimEnd(',');
                }
                return sValue;
            });
            return result;
        }

    }
}
