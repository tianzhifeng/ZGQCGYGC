using Project.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Config.Logic;
using Config;
using System.Data;
using Project.Logic;
using Formula.Helper;
using Formula;

namespace Project.Areas.AutoUI.Controllers
{
    public class DesignPlanPetrifactionController : ProjectFormContorllor<T_SC_DesignPlanPetrifaction>
    {
        protected override void AfterGetData(System.Data.DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                string lastVersionID = this.Request["UpperVersion"];  //要升版的ID
                var lastVersion = this.GetEntityByID(lastVersionID);

                string projectInfoID = this.GetQueryString("ProjectInfoID");
                var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("");
                var defaultMileStoneDt = new List<Dictionary<string, object>>();

                if (lastVersion != null)
                {
                    #region 根据上一版数据生成里程碑列表
                    foreach (var item in lastVersion.T_SC_DesignPlanPetrifaction_MilestoneList.ToList())
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
                    var enmDef = EnumBaseHelper.GetEnumDef("Project.Major");
                    var majors = enmDef.EnumItem.Select(a => new { Value = a.Code, Name = a.Name }).ToList();
                    if (!string.IsNullOrEmpty(projectInfo.Major))
                    {
                        majors.Clear();
                        var projectMajors = JsonHelper.ToList(projectInfo.Major);
                        foreach (var projectMajor in projectMajors)
                        {
                            majors.Add(new { Value = projectMajor.GetValue("Value"), Name = projectMajor.GetValue("Name") });
                        }
                    }
                    var db = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
                    string sql = @"select ID,MileStoneName,MileStoneCode,MileStoneType,Weight,SortIndex,OutMajors,InMajors,PhaseValue from dbo.S_T_MileStone
where ModeID in (select ID from S_T_ProjectMode where ModeCode='{0}') and ProjectClass like '%{1}%' and PhaseValue like '%{2}%'
order by SortIndex ";
                    var templateDt = db.ExecuteDataTable(String.Format(sql, projectInfo.ModeCode, projectInfo.ProjectClass, projectInfo.PhaseValue));
                    foreach (DataRow template in templateDt.Rows)
                    {
                        if (template["MileStoneType"].ToString() == MileStoneType.Major.ToString())
                        {
                            #region 专业级里程碑
                            foreach (var major in majors.Select(a => a.Value))
                            {
                                string code = template["MileStoneName"].ToString() + "." + projectInfo.RootWBSID + "." + projectInfo.PhaseValue + "." + major + "." + template["MileStoneType"].ToString();
                                var row = this._createDefaultItem(template, code, lastVersion);
                                row.SetValue("Major", major);
                                defaultMileStoneDt.Add(row);
                            }
                            #endregion
                        }
                        else if (template["MileStoneType"].ToString() == MileStoneType.Cooperation.ToString())
                        {
                            #region 提资计划定义
                            var outMajors = template["OutMajors"].ToString().Split(',');
                            var defInMajors = template["InMajors"].ToString();
                            foreach (var major in outMajors)
                            {
                                var InMajors = defInMajors;
                                if (defInMajors == "All")
                                    InMajors = String.Join(",", majors.Where(a => a.Value != major).Select(d => d.Value));
                                string code = template["MileStoneName"].ToString() + "." + projectInfo.RootWBSID + "." + projectInfo.PhaseValue + "." + major + "." + template["MileStoneType"].ToString();
                                var row = this._createDefaultItem(template, code, lastVersion);
                                row.SetValue("Major", major);
                                row.SetValue("OutMajor", major);
                                row.SetValue("InMajor", InMajors);
                                var majorName = String.Join(",", majors.Where(d => InMajors.Split(',').Contains(d.Value)).Select(d => d.Name));
                                row.SetValue("Remark", "接收专业：" + majorName);
                                defaultMileStoneDt.Add(row);
                            }
                            #endregion
                        }
                        else
                        {
                            string code = template["MileStoneName"].ToString() + "." + projectInfo.RootWBSID + "." + projectInfo.PhaseValue + "." + template["MileStoneType"].ToString();
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
                    #endregion
                    defaultMileStoneDt = defaultMileStoneDt.OrderBy(a => Convert.ToDouble(a.GetValue("SortIndex"))).ToList();
                }
                if (dt.Rows.Count > 0 && dt.Columns.Contains("MilestoneList"))
                    dt.Rows[0]["MilestoneList"] = JsonHelper.ToJson(defaultMileStoneDt);
            }
        }

        protected override void BeforeSaveDetail(Dictionary<string, string> dic, string subTableName, Dictionary<string, string> detail, List<Dictionary<string, string>> detailList, Base.Logic.Domain.S_UI_Form formInfo)
        {
            if (subTableName == "MilestoneList")
            {
                if (string.IsNullOrEmpty(detail.GetValue("Code")))
                {
                    string code = detail.GetValue("Name") + "." + dic.GetValue("ProjectInfoID") + "." + dic.GetValue("PhaseValue") + "." + detail.GetValue("Major") + "." + detail.GetValue("MileStoneType");
                    detail.SetValue("Code", code);
                }
            }
        }

        private Dictionary<string, object> _createDefaultItem(DataRow template, string code, T_SC_DesignPlanPetrifaction lastVersion)
        {
            var row = new Dictionary<string, object>();
            row.SetValue("Name", template["MileStoneName"].ToString());
            row.SetValue("SortIndex", template["SortIndex"].ToString());
            if (lastVersion != null)
            {
                var lastItem = lastVersion.T_SC_DesignPlanPetrifaction_MilestoneList.FirstOrDefault(d => d.Code == code);
                if (lastItem != null)
                {
                    row.SetValue("PlanEndDate", lastItem.PlanEndDate);
                    row.SetValue("Weight", template["Weight"].ToString());
                    row.SetValue("SortIndex", lastItem.SortIndex);
                }
            }
            if (String.IsNullOrEmpty(row.GetValue("Weight")))
                row.SetValue("Weight", template["Weight"].ToString());
            row.SetValue("Code", code);
            row.SetValue("MileStoneType", template["MileStoneType"].ToString());
            row.SetValue("MileStoneID", "");
            row.SetValue("TemplateID", template["ID"].ToString());
            return row;
        }

        protected override void OnFlowEnd(T_SC_DesignPlanPetrifaction entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            if (entity != null)
            {
                List<S_P_MileStone> deleteMileStoneList = new List<S_P_MileStone>();
                var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
                var projectInfo = projectEntities.Set<S_I_ProjectInfo>().Find(entity.ProjectInfoID);
                var detailMileStoneList = entity.T_SC_DesignPlanPetrifaction_MilestoneList.ToList();
                var selectCodes = detailMileStoneList.Select(a => a.Code).ToList();
                deleteMileStoneList = projectInfo.S_P_MileStone.Where(a => !selectCodes.Contains(a.Code)).ToList();

                entity.Push();
                this.BusinessEntities.SaveChanges();
                //同步关联的收款项的里程碑信息：时间、状态
                var fo = new Basic.Controllers.MileStoneExecuteController();
                //找到删除的里程碑的所有收款项，在这些收款项中去除当前里程碑，在同步这些收款项的里程碑信息
                //foreach (var item in deleteMileStoneList)
                //{
                //    fo.UpdateReceiptObjByDelMeliStoneID(item.ID);
                //}
                //foreach (var item in projectInfo.S_P_MileStone.ToList())
                //{
                //    fo.SyncReceiptObj(item);
                //}
                //FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>().SaveChanges();
            }
        }
    }
}
