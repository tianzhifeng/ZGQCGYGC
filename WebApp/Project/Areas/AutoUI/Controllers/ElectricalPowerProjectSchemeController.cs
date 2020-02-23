using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Workflow.Logic;
using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Formula;
using Base.Logic.BusinessFacade;
using System.Data;
using System.Web.Routing;
using Config.Logic;
using Newtonsoft.Json;
using Formula.ImportExport;
using MvcAdapter;

namespace Project.Areas.AutoUI.Controllers
{
    public class ElectricalPowerProjectSchemeController : ProjectFormContorllor<T_SC_ElectricalPowerProjectScheme>
    {

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            string projectInfoID = this.GetQueryString("ProjectInfoID");
            if (string.IsNullOrEmpty(projectInfoID))
            {
                if (dt.Columns.Contains("ProjectInfoID"))
                    projectInfoID = dt.Rows[0]["ProjectInfoID"].ToString();
            }
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未找到指定的项目，请联系管理员");
            #region 里程碑数据源（由于流程中列表定义数据源因为要从url获取参数而失效，故在此写数据源）
            var db = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            string sql = @"select ID,MileStoneName,MileStoneCode,MileStoneType,Weight,SortIndex,OutMajors,InMajors,PhaseValue from dbo.S_T_MileStone
where ModeID in (select ID from S_T_ProjectMode where ModeCode='{0}') and ProjectClass like '%{1}%'  {2}
order by SortIndex ";
            var phaseStr = string.Empty;
            foreach (var item in projectInfo.PhaseValue.Split(','))
            {
                phaseStr += ("or PhaseValue like '%" + item + "%' ");
            }
            phaseStr = " and (" + phaseStr.TrimStart('o', 'r') + ")";
            var templateDt = db.ExecuteDataTable(String.Format(sql, projectInfo.ModeCode, projectInfo.ProjectClass, phaseStr));
            #endregion
            if (isNew)
            {
                string lastVersionID = this.Request["UpperVersion"];  //要升版的ID
                var lastVersion = this.GetEntityByID(lastVersionID);
                var defaultMileStoneDt = new List<Dictionary<string, object>>();

                if (projectInfo.ProjectMode.ExtentionObject.GetValue("Ext_MsDataIsFromLastVertion") == TrueOrFalse.True.ToString()
                    && lastVersion != null)
                {
                    #region 根据上一版数据生成里程碑列表
                    foreach (var item in lastVersion.T_SC_ElectricalPowerProjectScheme_MileStoneList.ToList())
                    {
                        var row = item.ToDic();
                        row.SetValue("ID", "");
                        defaultMileStoneDt.Add(row);
                    }
                    #endregion
                }
                else
                {
                    #region 根据里程碑定义获得默认的里程碑列表
                    var majors = dt.Rows[0]["Major"].ToString();
                    foreach (DataRow template in templateDt.Rows)
                    {
                        if (template["MileStoneType"].ToString() == MileStoneType.Major.ToString())
                        {
                            #region 专业级里程碑
                            foreach (var major in majors.Split(','))
                            {
                                if (String.IsNullOrEmpty(major)) continue;
                                string code = template["MileStoneName"].ToString() + "." + projectInfo.RootWBSID + "." + major + "." + template["MileStoneType"].ToString();
                                var row = this._createDefaultItem(template, code, lastVersion);
                                row.SetValue("Major", major);
                                defaultMileStoneDt.Add(row);
                            }
                            #endregion
                        }
                        else if (template["MileStoneType"].ToString() == MileStoneType.Cooperation.ToString())
                        {
                            #region 提资计划定义
                            if (template["OutMajors"].ToString() == "All")
                            {
                                foreach (var major in majors.Split(','))
                                {
                                    if (String.IsNullOrEmpty(major)) continue;
                                    var defInMajors = template["InMajors"].ToString();
                                    var InMajors = String.Join(",", majors.Split(',').Where(d => defInMajors.Contains(d)));
                                    if (defInMajors == "All")
                                        InMajors = String.Join(",", majors.Split(','));
                                    string code = template["MileStoneName"].ToString() + "." + projectInfo.RootWBSID + "." + major + "." + template["MileStoneType"].ToString();
                                    var row = this._createDefaultItem(template, code, lastVersion);
                                    row.SetValue("Major", major);
                                    row.SetValue("OutMajor", major);
                                    row.SetValue("InMajor", InMajors);
                                    var enmDef = EnumBaseHelper.GetEnumDef("Project.Major");
                                    var majorName = String.Join(",", enmDef.EnumItem.Where(d => InMajors.Split(',').Contains(d.Code)).Select(d => d.Name));
                                    row.SetValue("Remark", "接收专业：" + majorName);
                                    defaultMileStoneDt.Add(row);
                                }
                            }
                            else
                            {
                                var outMajors = template["OutMajors"].ToString().Split(',');
                                foreach (var major in majors.Split(','))
                                {
                                    if (String.IsNullOrEmpty(major)) continue;
                                    var defInMajors = template["InMajors"].ToString();
                                    var InMajors = String.Join(",", majors.Split(',').Where(d => defInMajors.Contains(d)));
                                    if (defInMajors == "All")
                                        InMajors = String.Join(",", majors.Split(','));
                                    if (!outMajors.Contains(major)) continue;
                                    string code = template["MileStoneName"].ToString() + "." + projectInfo.ID + "." + major + "." + template["MileStoneType"].ToString();
                                    var row = this._createDefaultItem(template, code, lastVersion);
                                    row.SetValue("Major", major);
                                    row.SetValue("OutMajor", major);
                                    row.SetValue("InMajor", InMajors);
                                    var enmDef = EnumBaseHelper.GetEnumDef("Project.Major");
                                    var majorName = String.Join(",", enmDef.EnumItem.Where(d => InMajors.Split(',').Contains(d.Code)).Select(d => d.Name));
                                    row.SetValue("Remark", "接收专业：" + majorName);
                                    defaultMileStoneDt.Add(row);
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            string code = template["MileStoneName"].ToString() + "." + projectInfo.ID + ".." + template["MileStoneType"].ToString();
                            var row = this._createDefaultItem(template, code, lastVersion);
                            row.SetValue("Major", "");
                            defaultMileStoneDt.Add(row);
                        }
                    }

                    #region 模板已经删除，但是实际已经完成的里程碑，需要再次加入里程碑子表数据和模板数据
                    var listCodes = defaultMileStoneDt.Select(a => a.GetValue("Code")).Where(a => !string.IsNullOrEmpty(a)).ToList();
                    string finishState = ProjectCommoneState.Finish.ToString();
                    var finishList = projectInfo.S_P_MileStone.Where(a => a.State == finishState && !listCodes.Contains(a.Code)).ToList();
                    foreach (var item in finishList)
                    {
                        var row = new Dictionary<string, object>();

                        row.SetValue("Name", item.Name);
                        row.SetValue("SortIndex", item.SortIndex ?? 0);
                        row.SetValue("PlanEndDate", item.PlanFinishDate);
                        row.SetValue("Weight", item.Weight);
                        row.SetValue("Code", item.Code);
                        row.SetValue("MileStoneType", item.MileStoneType);
                        row.SetValue("MileStoneID", item.ID);
                        row.SetValue("TemplateID", item.TemplateID);
                        row.SetValue("Major", item.MajorValue);
                        row.SetValue("OutMajor", item.MajorValue);
                        row.SetValue("InMajor", item.OutMajorValue);
                        row.SetValue("Remark", item.Description);
                        defaultMileStoneDt.Add(row);


                        var tmpRow = templateDt.NewRow();
                        tmpRow["ID"] = string.Empty;
                        tmpRow["MileStoneName"] = item.Name;
                        tmpRow["MileStoneCode"] = item.Code;
                        tmpRow["MileStoneType"] = item.MileStoneType;
                        if (item.Weight.HasValue)
                            tmpRow["Weight"] = item.Weight;
                        tmpRow["SortIndex"] = item.SortIndex ?? 0;
                        tmpRow["OutMajors"] = item.MajorValue;
                        tmpRow["InMajors"] = item.OutMajorValue;
                        tmpRow["PhaseValue"] = string.Empty;
                        templateDt.Rows.Add(tmpRow);

                    }

                    #endregion
                    #endregion
                    #region 手动添加的流程碑，需要再次加入里程碑子表数据和模板数据
                    var cusList = projectInfo.S_P_MileStone.Where(a => string.IsNullOrEmpty(a.TemplateID)).ToList();
                    foreach (var item in cusList)
                    {
                        var row = new Dictionary<string, object>();

                        row.SetValue("Name", item.Name);
                        row.SetValue("SortIndex", item.SortIndex ?? 0);
                        row.SetValue("PlanEndDate", item.PlanFinishDate);
                        row.SetValue("Weight", item.Weight);
                        row.SetValue("Code", item.Code);
                        row.SetValue("MileStoneType", item.MileStoneType);
                        row.SetValue("MileStoneID", item.ID);
                        row.SetValue("TemplateID", item.TemplateID);
                        row.SetValue("Major", item.MajorValue);
                        row.SetValue("OutMajor", item.MajorValue);
                        row.SetValue("InMajor", item.OutMajorValue);
                        row.SetValue("Remark", item.Description);
                        defaultMileStoneDt.Add(row);


                        var tmpRow = templateDt.NewRow();
                        tmpRow["ID"] = string.Empty;
                        tmpRow["MileStoneName"] = item.Name;
                        tmpRow["MileStoneCode"] = item.Code;
                        tmpRow["MileStoneType"] = item.MileStoneType;
                        if (item.Weight.HasValue)
                            tmpRow["Weight"] = item.Weight;
                        tmpRow["SortIndex"] = item.SortIndex ?? 0;
                        tmpRow["OutMajors"] = item.MajorValue;
                        tmpRow["InMajors"] = item.OutMajorValue;
                        tmpRow["PhaseValue"] = string.Empty;
                        templateDt.Rows.Add(tmpRow);

                    }
                    #endregion
                    defaultMileStoneDt = defaultMileStoneDt.OrderBy(a => Convert.ToDouble(a.GetValue("SortIndex"))).ToList();
                }
                if (dt.Rows.Count > 0 && dt.Columns.Contains("MileStoneList"))
                    dt.Rows[0]["MileStoneList"] = JsonHelper.ToJson(defaultMileStoneDt);

                #region 根据管理工时定义获默认管理工时
                if (dt.Rows.Count > 0 && dt.Columns.Contains("ManageWorkloadList") &&
                    (dt.Rows[0]["ManageWorkloadList"] == DBNull.Value || string.IsNullOrEmpty(dt.Rows[0]["ManageWorkloadList"].ToString())))
                {
                    sql = @"select * from S_C_ManageWorkloadDefine order by sortindex";
                    var tmpWorkloadDt = db.ExecuteDataTable(sql);
                    var defaultManageWorkloadDt = new List<Dictionary<string, object>>();
                    foreach (DataRow template in tmpWorkloadDt.Rows)
                    {
                        var row = new Dictionary<string, object>();
                        row.SetValue("ItemName", template["Name"].ToString());
                        var code = template["Code"].ToString();
                        row.SetValue("Item", code);
                        row.SetValue("SortIndex", template["SortIndex"].ToString());
                        var rate = Convert.ToDecimal(template["Rate"]);
                        decimal ProjectWorkload = 0m;
                        if (dt.Rows.Count > 0 && dt.Rows[0]["ProjectWorkload"] != DBNull.Value
                            && dt.Rows[0]["ProjectWorkload"] != null && !string.IsNullOrEmpty(dt.Rows[0]["ProjectWorkload"].ToString()))
                            ProjectWorkload = Convert.ToDecimal(dt.Rows[0]["ProjectWorkload"]);
                        var value = Math.Round(ProjectWorkload * rate / 100, 2);
                        if (lastVersion != null)
                        {
                            var lastItem = lastVersion.T_SC_ElectricalPowerProjectScheme_ManageWorkloadList.FirstOrDefault(d => d.Item == code);
                            if (lastItem != null)
                            {
                                rate = Convert.ToDecimal(lastItem.ManageWorkloadRate);
                                ProjectWorkload = lastVersion.ProjectWorkload ?? 0;
                                value = Math.Round(ProjectWorkload * rate / 100, 2);
                            }
                        }
                        row.SetValue("ManageWorkloadRate", rate);
                        row.SetValue("ManageWorkload", value);
                        defaultManageWorkloadDt.Add(row);
                    }
                    dt.Rows[0]["ManageWorkloadList"] = JsonHelper.ToJson(defaultManageWorkloadDt);
                }
                #endregion

                if (lastVersion != null)
                {
                    dt.Columns.Add("LastProjectWorkload");
                    dt.Rows[0]["LastProjectWorkload"] = Convert.ToDecimal(lastVersion.ProjectWorkload);
                }

                #region 卷册数据
                if (dt.Columns.Contains("TaskWorkList") && dt.Rows[0]["TaskWorkList"] != null
                    && !string.IsNullOrEmpty(dt.Rows[0]["TaskWorkList"].ToString()))
                {
                    var newList = new List<Dictionary<string, object>>();
                    var detailList = JsonHelper.ToObject<List<Dictionary<string, object>>>(dt.Rows[0]["TaskWorkList"].ToString());
                    var taskList = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => a.ProjectInfoID == projectInfo.ID).ToList();
                    foreach (var task in taskList)
                    {
                        var detail = detailList.FirstOrDefault(a => a.GetValue("TaskWorkID") == task.ID);
                        if (detail == null)
                        {
                            detail = new Dictionary<string, object>();
                            detail.SetValue("TaskWorkID", task.ID);
                        }

                        detail.SetValue("Name", task.Name);
                        detail.SetValue("Code", task.Code);
                        detail.SetValue("Major", task.MajorValue);
                        detail.SetValue("Phase", task.PhaseValue);
                        newList.Add(detail);
                    }
                    dt.Rows[0]["TaskWorkList"] = JsonHelper.ToJson(newList);
                }
                #endregion
            }
            if (!dt.Columns.Contains("MileStoneInfo")) dt.Columns.Add("MileStoneInfo");
            var dv = templateDt.DefaultView;
            dv.Sort = "SortIndex";
            dt.Rows[0]["MileStoneInfo"] = JsonHelper.ToJson(dv.ToTable());
            #region 专业已分配已结算数据
            if (dt.Rows.Count > 0 && dt.Columns.Contains("MajorList")
                && dt.Rows[0]["MajorList"] != DBNull.Value && dt.Rows[0]["MajorList"] != null
                && !string.IsNullOrEmpty(dt.Rows[0]["MajorList"].ToString()))
            {
                var majorList = JsonHelper.ToObject<List<Dictionary<string, object>>>(dt.Rows[0]["MajorList"].ToString());
                var nodeType = CBSNodeType.Major.ToString();
                var majorCBSList = projectInfo.S_C_CBS.Where(a => a.NodeType == nodeType).ToList();
                foreach (var majorDic in majorList)
                {
                    decimal? SummaryBudgetQuantity = null, SummaryCostQuantity = null;
                    var majorCBS = majorCBSList.FirstOrDefault(a => a.Code == majorDic.GetValue("MajorCode"));
                    if (majorCBS != null)
                    {
                        SummaryBudgetQuantity = majorCBS.SummaryBudgetQuantity;
                        SummaryCostQuantity = majorCBS.SummaryCostQuantity;
                    }
                    majorDic.SetValue("SummaryBudgetQuantity", SummaryBudgetQuantity);
                    majorDic.SetValue("SummaryCostQuantity", SummaryCostQuantity);
                }
                if (dt.Rows.Count > 0 && dt.Columns.Contains("MajorList"))
                    dt.Rows[0]["MajorList"] = JsonHelper.ToJson(majorList);
            }
            #endregion
            #region 管理工时已分配已结算数据
            if (dt.Rows.Count > 0 && dt.Columns.Contains("ManageWorkloadList")
                && dt.Rows[0]["ManageWorkloadList"] != DBNull.Value && dt.Rows[0]["ManageWorkloadList"] != null
                && !string.IsNullOrEmpty(dt.Rows[0]["ManageWorkloadList"].ToString()))
            {
                var manageList = JsonHelper.ToObject<List<Dictionary<string, object>>>(dt.Rows[0]["ManageWorkloadList"].ToString());
                var nodeType = CBSNodeType.Category.ToString();
                var nodeCode = CBSCategoryType.Manage.ToString();
                var manageNode = projectInfo.S_C_CBS.FirstOrDefault(a => a.NodeType == nodeType && a.Code == nodeCode);
                var manageCBSList = new List<S_C_CBS>();
                if (manageNode != null)
                    manageCBSList = manageNode.Children.ToList();
                foreach (var manageDic in manageList)
                {
                    decimal? SummaryCostQuantity = null;
                    var manageCBS = manageCBSList.FirstOrDefault(a => a.Code == manageDic.GetValue("Item"));
                    if (manageCBS != null)
                    {
                        SummaryCostQuantity = manageCBS.SummaryCostQuantity;
                    }
                    manageDic.SetValue("SummaryCostQuantity", SummaryCostQuantity);
                }
                if (dt.Rows.Count > 0 && dt.Columns.Contains("ManageWorkloadList"))
                    dt.Rows[0]["ManageWorkloadList"] = JsonHelper.ToJson(manageList);
            }
            #endregion
        }

        private Dictionary<string, object> _createDefaultItem(DataRow template, string code, T_SC_ElectricalPowerProjectScheme lastVersion)
        {
            var row = new Dictionary<string, object>();
            row.SetValue("Name", template["MileStoneName"].ToString());
            row.SetValue("SortIndex", template["SortIndex"].ToString());
            if (lastVersion != null)
            {
                var lastItem = lastVersion.T_SC_ElectricalPowerProjectScheme_MileStoneList.FirstOrDefault(d => d.Code == code);
                if (lastItem != null)
                {
                    row.SetValue("PlanEndDate", lastItem.PlanEndDate);
                    row.SetValue("Weight", template["Weight"].ToString());
                }
            }
            if (String.IsNullOrEmpty(row.GetValue("Weight")))
                row.SetValue("Weight", template["Weight"].ToString());
            row.SetValue("Code", code);
            row.SetValue("MileStoneType", template["MileStoneType"].ToString());
            row.SetValue("TemplateID", template["ID"].ToString());
            return row;
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic["ID"]);
            if (entity == null) entity = new T_SC_ElectricalPowerProjectScheme();
            this.UpdateEntity(entity, dic);
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(entity.ProjectInfoID);
            #region 校验整体专业
            #region 校验成果
            string sql = @"select distinct MajorValue, S_W_WBS.Name MajorName from S_E_Product p
left join S_W_WBS on p.WBSID = S_W_WBS.ID 
where p.ProjectInfoID='{0}' and MajorValue not in ('{1}')";
            sql = string.Format(sql, entity.ProjectInfoID, entity.Major.Replace(",", "','"));
            var dt = this.ProjectSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                var msgMajor = "";
                foreach (DataRow item in dt.Rows)
                {
                    msgMajor += item["MajorName"].ToString() + ",";
                }
                throw new Formula.Exceptions.BusinessException("专业【" + msgMajor.TrimEnd(',') + "】已经上传了设计成果，必须选中。");
            }
            #endregion

            #region 校验工时
            sql = string.Format(@"select * from S_C_CBS_Cost
            where CBSID in (
            select ID from S_C_CBS where ProjectInfoID='{0}' and NodeType='{1}' 
            and Code not in ('{2}') 
            )", dic.GetValue("ProjectInfoID"), CBSNodeType.Major.ToString(), dic.GetValue("Major").Replace(",", "','"));
            dt = this.ProjectSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                var msgMajor = "";
                foreach (DataRow item in dt.Rows)
                {
                    msgMajor += item["Name"].ToString() + ",";
                }
                throw new Formula.Exceptions.BusinessException("专业【" + msgMajor.TrimEnd(',') + "】已经结算了工时，必须选中");
            }

            #endregion

            string DeptLeaderValue = "", DeptLeaderText = "";//室主任字段
            #region 修改的专业工时必须大于已经下达的工时
            sql = string.Format(@"select * from S_C_CBS where ProjectInfoID='{0}'", dic.GetValue("ProjectInfoID"));
            var cbsList = this.ProjectSQLDB.ExecuteList<S_C_CBS>(sql);
            var majorCBSList = cbsList.Where(a => a.NodeType == CBSNodeType.Major.ToString()).ToList();
            var manageNode = cbsList.FirstOrDefault(a => a.NodeType == CBSNodeType.Category.ToString() && a.Code == CBSCategoryType.Manage.ToString());
            var manageCBSList = new List<S_C_CBS>();
            if (manageNode != null)
                manageCBSList = cbsList.Where(a => a.ParentID == manageNode.ID).ToList();

            var majorDetailList = JsonHelper.ToObject<List<Dictionary<string, string>>>(dic["MajorList"]);
            string msgMajorsBudget = "", msgMajorsCost = "";
            var sumMajorWorkload = 0m;
            foreach (var item in majorDetailList)
            {
                if (!string.IsNullOrEmpty(item.GetValue("DeptLeader")))
                {
                    DeptLeaderValue += item.GetValue("DeptLeader") + ",";
                    DeptLeaderText += item.GetValue("DeptLeaderName") + ",";
                }

                var itemWorkload = 0m;
                if (!string.IsNullOrEmpty(item.GetValue("MajorWorkload")))
                    itemWorkload = Convert.ToDecimal(item.GetValue("MajorWorkload"));
                sumMajorWorkload += itemWorkload;
                var majorCBS = majorCBSList.FirstOrDefault(a => a.Code == item.GetValue("MajorCode"));
                if (majorCBS == null)
                    continue;
                if (itemWorkload < (majorCBS.SummaryBudgetQuantity.HasValue ? majorCBS.SummaryBudgetQuantity.Value : 0))
                    msgMajorsBudget += item["MajorName"].ToString() + ",";
                if (itemWorkload < (majorCBS.SummaryCostQuantity.HasValue ? majorCBS.SummaryCostQuantity.Value : 0))
                    msgMajorsCost += item["MajorName"].ToString() + ",";
            }
            if (!string.IsNullOrEmpty(msgMajorsBudget))
                throw new Formula.Exceptions.BusinessException("专业【" + msgMajorsBudget.TrimEnd(',') + "】的下达工时不能小于已经分配到卷册的工时");
            if (!string.IsNullOrEmpty(msgMajorsCost))
                throw new Formula.Exceptions.BusinessException("专业【" + msgMajorsCost.TrimEnd(',') + "】的下达工时不能小于已经结算的工时");

            #endregion
            dic.SetValue("DeptLeader", DeptLeaderValue.TrimEnd(','));
            dic.SetValue("DeptLeaderName", DeptLeaderText.TrimEnd(','));

            #region 校验提资单
            var sqlPutInfo = @"select distinct OutMajorValue, S_W_WBS.Name MajorName from T_EXE_MajorPutInfo p
left join S_W_WBS on p.WBSID = S_W_WBS.ParentID and p.OutMajorValue = S_W_WBS.WBSValue 
where p.ProjectInfoID='{0}' and OutMajorValue not in ('{1}')";
            sqlPutInfo = string.Format(sqlPutInfo, entity.ProjectInfoID, entity.Major.Replace(",", "','"));
            var dtPutInfo = this.ProjectSQLDB.ExecuteDataTable(sqlPutInfo);
            if (dtPutInfo.Rows.Count > 0)
            {
                var msgMajor = "";
                foreach (DataRow item in dtPutInfo.Rows)
                {
                    msgMajor += item["MajorName"].ToString() + ",";
                }
                throw new Formula.Exceptions.BusinessException("专业【" + msgMajor.TrimEnd(',') + "】发起过提资，必须选中。");
            }
            #endregion

            #region 校验共享区
            var sqlShareInfo = @"select distinct p.WBSValue, S_W_WBS.Name MajorName from S_D_ShareInfo p
left join S_W_WBS on p.SubProjectWBSID = S_W_WBS.ParentID and p.WBSValue = S_W_WBS.WBSValue 
where p.ProjectInfoID='{0}' and p.WBSValue not in ('{1}')";
            sqlShareInfo = string.Format(sqlShareInfo, entity.ProjectInfoID, entity.Major.Replace(",", "','"));
            var dtShareInfo = this.ProjectSQLDB.ExecuteDataTable(sqlShareInfo);
            if (dtShareInfo.Rows.Count > 0)
            {
                var msgMajor = "";
                foreach (DataRow item in dtShareInfo.Rows)
                {
                    msgMajor += item["MajorName"].ToString() + ",";
                }
                throw new Formula.Exceptions.BusinessException("专业【" + msgMajor.TrimEnd(',') + "】拥有共享区文件，必须选中。");
            }
            #endregion

            #region 校验专业里程碑
            string finishState = ProjectCommoneState.Finish.ToString();
            var finishMileStones = this.BusinessEntities.Set<S_P_MileStone>().Where(a => a.ProjectInfoID == entity.ProjectInfoID && a.State == finishState).ToList();
            List<string> msgMileStoneMajor = new List<string>();
            foreach (var item in finishMileStones)
            {
                if (string.IsNullOrEmpty(item.MajorValue) && string.IsNullOrEmpty(item.OutMajorValue))
                    continue;
                if (!entity.Major.Split(',').Contains(item.MajorValue))
                {
                    if (!msgMileStoneMajor.Contains(item.MajorValue))
                        msgMileStoneMajor.Add(item.MajorValue);
                }
                if (!string.IsNullOrEmpty(item.OutMajorValue))
                {
                    foreach (var _oMajorValue in item.OutMajorValue.Split(','))
                    {
                        if (string.IsNullOrEmpty(_oMajorValue)) continue;
                        if (!entity.Major.Split(',').Contains(_oMajorValue))
                        {
                            if (!msgMileStoneMajor.Contains(_oMajorValue))
                                msgMileStoneMajor.Add(_oMajorValue);
                        }
                    }
                }
            }
            if (msgMileStoneMajor.Count > 0)
            {
                var enmDef = EnumBaseHelper.GetEnumDef("Project.Major");
                throw new Formula.Exceptions.BusinessException("专业【" + String.Join(",", enmDef.EnumItem.Where(d => msgMileStoneMajor.Contains(d.Code)).Select(d => d.Name)) + "】已经有相关里程碑完成，必须选中。");
            }
            #endregion
            #endregion

            #region 校验卷册
            if (!string.IsNullOrEmpty(dic.GetValue("TaskWorkList")))
            {
                string sameNames = string.Empty, hasProductNames = string.Empty,stateNames = string.Empty;
                var alllist = this.BusinessEntities.Set<S_W_TaskWork>().Where(a => a.ProjectInfoID == projectInfo.ID).ToList();
                var taskWorkList = JsonHelper.ToList(dic.GetValue("TaskWorkList"));
                var selectids = taskWorkList.Select(a => a.GetValue("TaskWorkID")).ToList();
                var deleteList = alllist.Where(a => !selectids.Contains(a.ID)).ToList();
                foreach (var item in deleteList)
                {
                    if (item.S_W_WBS.S_E_Product.Count > 0)
                        hasProductNames += item.Name + ",";
                    if (item.State != TaskWorkState.Create.ToString() && item.State != TaskWorkState.Plan.ToString())
                        stateNames += item.Name + ",";
                    alllist.Remove(item);
                } if (!string.IsNullOrEmpty(hasProductNames))
                    throw new Formula.Exceptions.BusinessException("卷册【" + hasProductNames.TrimEnd(',') + "】已经上传了设计成果，无法删除。");
                 if (!string.IsNullOrEmpty(stateNames))
                    throw new Formula.Exceptions.BusinessException("卷册【" + stateNames.TrimEnd(',') + "】已经发起卷册任务，无法删除。");
                foreach (var item in taskWorkList)
                {
                    var taskID = item.GetValue("TaskWorkID");
                    var task = alllist.FirstOrDefault(a => a.ID == taskID);
                    if (task==null)
                    {
                        if (string.IsNullOrEmpty(taskID))
                            taskID = FormulaHelper.CreateGuid();
                        task = new S_W_TaskWork();
                        task.ID = taskID;
                        task.ProjectInfoID = projectInfo.ID;
                        item.SetValue("TaskWorkID", taskID);
                        alllist.Add(task);
                    }
                    task.PhaseValue = string.IsNullOrEmpty(item.GetValue("Phase")) ? projectInfo.PhaseValue : item.GetValue("Phase");
                    task.MajorValue = item.GetValue("Major");
                    task.Code = item.GetValue("Code");
                    task.Name = item.GetValue("Name");
                    //编号重复校验
                    if (alllist.Any(a => a.Code == task.Code && a.PhaseValue == task.PhaseValue && a.ID != task.ID))
                        sameNames += task.Name + ",";
                }
                if (!string.IsNullOrEmpty(sameNames))
                    throw new Formula.Exceptions.BusinessException("相同阶段下，卷册【" + sameNames.TrimEnd(',') + "】编号重复。");

                dic.SetValue("TaskWorkList", JsonHelper.ToJson<List<Dictionary<string, object>>>(taskWorkList));
            }
            #endregion

            if (!string.IsNullOrEmpty(dic.GetValue("ManageWorkloadList")))
            {
                #region 管理工时不能大于已结算的工时
                var msgManageCost = "";
                var sumManangeWorkload = 0m;
                var manageDetailList = JsonHelper.ToObject<List<Dictionary<string, string>>>(dic["ManageWorkloadList"]);
                foreach (var item in manageDetailList)
                {
                    var itemWorkload = 0m;
                    if (!string.IsNullOrEmpty(item.GetValue("ManageWorkload")))
                        itemWorkload = Convert.ToDecimal(item.GetValue("ManageWorkload"));
                    sumManangeWorkload += itemWorkload;
                    var manageCBS = manageCBSList.FirstOrDefault(a => a.Code == item.GetValue("Item"));
                    if (manageCBS == null)
                        continue;
                    if (itemWorkload < (manageCBS.SummaryCostQuantity.HasValue ? manageCBS.SummaryCostQuantity.Value : 0))
                        msgManageCost += item["ItemName"].ToString() + ",";
                }
                if (!string.IsNullOrEmpty(msgManageCost))
                    throw new Formula.Exceptions.BusinessException("管理工时【" + msgManageCost.TrimEnd(',') + "】的下达工时不能小于已经结算的工时");
                #endregion
                #region 工时总数校验 管理工时+专业工时+预留工时不能大于下达工时
                var reserveWorkload = 0m;
                if (!string.IsNullOrEmpty(dic.GetValue("ReserveWorkload")))
                    reserveWorkload = Convert.ToDecimal(dic.GetValue("ReserveWorkload"));
                var projectWorkload = 0m;
                if (!string.IsNullOrEmpty(dic.GetValue("ProjectWorkload")))
                    projectWorkload = Convert.ToDecimal(dic.GetValue("ProjectWorkload"));
                if (projectWorkload < reserveWorkload + sumManangeWorkload + sumMajorWorkload)
                    throw new Formula.Exceptions.BusinessException("管理工时【" + sumManangeWorkload + "】+专业工时【" + sumMajorWorkload + "】+预留工时【" + reserveWorkload + "】已经超过下达工时【" + projectWorkload + "】");

                #endregion
            }

            #region 校验里程碑编号

            var db = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            sql = @"select ID,MileStoneName,MileStoneCode,MileStoneType,Weight,SortIndex,OutMajors,InMajors,PhaseValue from dbo.S_T_MileStone
where ModeID in (select ID from S_T_ProjectMode where ModeCode='{0}') and ProjectClass like '%{1}%'  {2}
order by SortIndex ";
            var phaseStr = string.Empty;
            foreach (var item in projectInfo.PhaseValue.Split(','))
            {
                phaseStr += ("or PhaseValue like '%" + item + "%' ");
            }
            phaseStr = " and (" + phaseStr.TrimStart('o', 'r') + ")";
            var templateDt = db.ExecuteDataTable(String.Format(sql, projectInfo.ModeCode, projectInfo.ProjectClass, phaseStr));
            //当前端去掉专业，再次勾选专业后，对应的里程碑编号会为空
            var msList = JsonHelper.ToList(dic.GetValue("MileStoneList"));
            foreach (var milestone in msList)
            {
                if (string.IsNullOrEmpty(milestone.GetValue("Code")))
                {
                    DataRow tmpRow = null;
                    var tmpid = milestone.GetValue("TemplateID");
                    if (!string.IsNullOrEmpty(tmpid) && templateDt.Select("ID='" + tmpid + "'").Length > 0)
                        tmpRow = templateDt.Select("ID='" + tmpid + "'")[0];
                    else
                    {
                        var msName = milestone.GetValue("Name");
                        if (!string.IsNullOrEmpty(msName) && templateDt.Select("MileStoneName='" + msName + "'").Length > 0)
                            tmpRow = templateDt.Select("MileStoneName='" + msName + "'")[0];
                    }
                    if (tmpRow != null)
                    {
                        var msType = milestone.GetValue("MileStoneType");
                        string code = tmpRow["MileStoneName"].ToString() + "." + projectInfo.ID + "." + projectInfo.PhaseValue + "." + tmpRow["MileStoneType"].ToString();
                        if (msType == MileStoneType.Major.ToString())
                            code = tmpRow["MileStoneName"].ToString() + "." + projectInfo.ID + "." + projectInfo.PhaseValue + "." + milestone.GetValue("Major") + "." + tmpRow["MileStoneType"].ToString();
                        else if (msType == MileStoneType.Cooperation.ToString())
                            code = tmpRow["MileStoneName"].ToString() + "." + projectInfo.ID + "." + projectInfo.PhaseValue + "." + milestone.GetValue("Major") + "." + tmpRow["MileStoneType"].ToString();
                        milestone.SetValue("Code", code);
                    }
                    else
                    {
                        milestone.SetValue("Code", milestone.GetValue("Name") + "." + projectInfo.ID + "." + projectInfo.PhaseValue + "." + milestone.GetValue("Major"));
                    }
                }
            }
            dic.SetValue("MileStoneList", JsonHelper.ToJson<List<Dictionary<string, object>>>(msList));
            #endregion
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "MileStoneList")
            {
                if (string.IsNullOrEmpty(detail.GetValue("Code")))
                {
                    string code = detail.GetValue("Name") + "." + dic.GetValue("ProjectInfoID") + "." + dic.GetValue("PhaseValue") + "." + detail.GetValue("Major") + "." + detail.GetValue("MileStoneType");
                    detail.SetValue("Code", code);
                }
            }
        }

        //室主任策划环节只传变更行
        protected override void AfterSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic["ID"]);
            var sublist = entity.T_SC_ElectricalPowerProjectScheme_MajorList.ToList();
            if (sublist.Count > 0)
                entity.MajorList = JsonHelper.ToJson<List<T_SC_ElectricalPowerProjectScheme_MajorList>>(entity.T_SC_ElectricalPowerProjectScheme_MajorList.ToList());
            else
                entity.MajorList = "[]";
            this.BusinessEntities.SaveChanges();
            //base.AfterSave(dic, formInfo, isNew);
        }

        protected override void OnFlowEnd(T_SC_ElectricalPowerProjectScheme entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            string formData = Request["FormData"];
            Dictionary<string, string> dic = JsonHelper.ToObject<Dictionary<string, string>>(formData);
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(entity.ProjectInfoID);
            //var marketEntities = Formula.FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            //var marketProject = marketEntities.Set<Market.Logic.Domain.S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
            if (projectInfo.ProjectMode.ExtentionObject.GetValue("Ext_TemporaryMode") == TrueOrFalse.True.ToString())
            {
                var schemeDic = JsonHelper.ToObject<Dictionary<string, object>>(formData);
                var ignorKeys = new string[] { "CreateUserID", "CreateUser", "CreateDate", "State", "Major" };
                schemeDic.RemoveWhere(a => ignorKeys.Contains(a.Key));
                FormulaHelper.UpdateEntity(projectInfo, schemeDic);
                projectInfo.ReBuild();
                //marketProject.Phase = projectInfo.PhaseValue;
                //marketProject.Name = projectInfo.Name;
                //marketProject.Code = projectInfo.Code;
                //marketProject.Phase = projectInfo.PhaseValue;
                //marketProject.ProjectClass = projectInfo.ProjectClass;
                //marketProject.Customer = projectInfo.CustomerID;
                //marketProject.CustomerName = projectInfo.CustomerName;
                //marketProject.CreateDate = DateTime.Now;
                //marketProject.EngineeringInfo = projectInfo.EngineeringInfoID;
                //marketProject.ChargerDept = projectInfo.ChargeDeptID;
                //marketProject.ChargerDeptName = projectInfo.ChargeDeptName;
                //marketProject.ChargerUser = projectInfo.ChargeUserID;
                //marketProject.ChargerUserName = projectInfo.ChargeUserName;
                //marketProject.Country = projectInfo.Country;
                //marketProject.Province = projectInfo.Province;
                //marketProject.City = projectInfo.City;
                //marketProject.ProjectScale = projectInfo.ProjectLevel.HasValue ? projectInfo.ProjectLevel.Value.ToString() : "";
            }
            List<S_P_MileStone> deleteMileStoneList = new List<S_P_MileStone>();
            if (entity != null)
            {
                var selectCodes = entity.T_SC_ElectricalPowerProjectScheme_MileStoneList.Select(a => a.Code).ToList();
                deleteMileStoneList = projectInfo.S_P_MileStone.Where(a => !selectCodes.Contains(a.Code)).ToList();
                entity.Push(dic);
            }
            //if (marketProject != null)
            //    marketProject.State = ProjectCommoneState.Execute.ToString();
            //marketEntities.SaveChanges();
            this.BusinessEntities.SaveChanges();

            if (!string.IsNullOrEmpty(entity.TaskWorkList))
            {
                new WBSFO().AutoCreateWBSEntity(entity.ProjectInfoID);
            }

            //同步关联的收款项的里程碑信息：时间、状态
            //var fo = new Basic.Controllers.MileStoneExecuteController();
            ////找到删除的里程碑的所有收款项，在这些收款项中去除当前里程碑，在同步这些收款项的里程碑信息
            //foreach (var item in deleteMileStoneList)
            //{
            //    fo.UpdateReceiptObjByDelMeliStoneID(item.ID);
            //}
            //foreach (var item in projectInfo.S_P_MileStone.ToList())
            //{
            //    fo.SyncReceiptObj(item);
            //}
            //marketEntities.SaveChanges();
        }

        public JsonResult ValidateExist(string ID)
        {
            var entity = this.GetEntityByID(ID);
            if (entity != null)
                return Json(new { isExist = true });
            else
                return Json(new { isExist = false });
        }
        
        public JsonResult GetDetailTaskWorkList(string ID)
        {
            var data= this.BusinessEntities.Set<T_SC_ElectricalPowerProjectScheme_TaskWorkList>()
                .Where(a => a.T_SC_ElectricalPowerProjectSchemeID == ID).OrderBy(a=>a.SortIndex);
            return Json(data);
        }

        #region Excel导入

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);
            var enumService = FormulaHelper.GetService<IEnumService>();
            var majorEnums = enumService.GetEnumDataSource("Project.Major").ToList();
            var phaseEnums = enumService.GetEnumDataSource("Project.Phase").ToList();

            var id = this.GetQueryString("ID");
            if (string.IsNullOrEmpty(id))
                throw new Formula.Exceptions.BusinessException("未获得当前表单ID");

            var entity = this.GetEntityByID(id);
            if (entity == null)
                throw new Formula.Exceptions.BusinessException("未获得当前表单对象");

            //验证数据库字段
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "Name" && string.IsNullOrWhiteSpace(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("卷册名称不能为空", e.Value);
                }
                if (e.FieldName == "Code" && string.IsNullOrWhiteSpace(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = string.Format("卷册编号不能为空", e.Value);
                }
                if (e.FieldName == "MajorName")
                {
                    //专业有策划
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        var majorEnum = majorEnums.FirstOrDefault(a => a.Text == e.Value);
                        if (majorEnum == null)
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("专业不存在", e.Value);
                        }
                        else
                        {
                            if (!entity.Major.Split(',').Contains(majorEnum.Value))
                            {
                                e.IsValid = false;
                                e.ErrorText = string.Format("专业未勾选", e.Value);
                            }
                        }
                    }
                }
                if (e.FieldName == "PhaseName")
                {
                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        var phaseEnum = phaseEnums.FirstOrDefault(a => a.Text == e.Value);
                        if (phaseEnum == null)
                        {
                            e.IsValid = false;
                            e.ErrorText = string.Format("阶段不存在", e.Value);
                        }
                        else
                        {
                            if (!entity.PhaseValue.Split(',').Contains(phaseEnum.Value))
                            {
                                e.IsValid = false;
                                e.ErrorText = string.Format("本项目不包含此专业", e.Value);
                            }
                        }
                    }
                }
            });

            return Json(errors);
        }

        public JsonResult SaveExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var currentUser = FormulaHelper.GetUserInfo();
            var enumService = FormulaHelper.GetService<IEnumService>();
            var majorEnums = enumService.GetEnumDataSource("Project.Major").ToList();
            var phaseEnums = enumService.GetEnumDataSource("Project.Phase").ToList();
            var id = this.GetQueryString("ID");
            if (string.IsNullOrEmpty(id))
                throw new Formula.Exceptions.BusinessException("未获得当前表单ID");

            var entity = this.GetEntityByID(id);
            if (entity == null)
                throw new Formula.Exceptions.BusinessException("未获得当前表单对象");

            var detailList = entity.T_SC_ElectricalPowerProjectScheme_TaskWorkList.ToList();
            var dicList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(tempdata["data"]);
            foreach (var item in dicList)
            {
                var detail = detailList.FirstOrDefault(a => a.Code == item.GetValue("Code"));
                if (detail == null)
                {
                    detail = new T_SC_ElectricalPowerProjectScheme_TaskWorkList();
                    detail.ID = FormulaHelper.CreateGuid();
                    detail.TaskWorkID = FormulaHelper.CreateGuid();
                    detail.T_SC_ElectricalPowerProjectSchemeID = entity.ID;
                    detailList.Add(detail);
                    entity.T_SC_ElectricalPowerProjectScheme_TaskWorkList.Add(detail);
                }
                detail.Major = majorEnums.FirstOrDefault(a => a.Text == item.GetValue("MajorName")).Value;
                detail.Phase = phaseEnums.FirstOrDefault(a => a.Text == item.GetValue("PhaseName")).Value;
                UpdateEntity<T_SC_ElectricalPowerProjectScheme_TaskWorkList>(detail, item);
                if (!detail.SortIndex.HasValue)
                    detail.SortIndex = (detailList.Max(a => a.SortIndex) ?? 0) + 1;
            }
            this.BusinessEntities.SaveChanges();
            return Json("Success");
        }

        #endregion
    }
}
