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

namespace Project.Areas.AutoUI.Controllers
{
    public class SingleProjectSchemeController : ProjectFormContorllor<T_SC_SingleProjectScheme>
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
                    foreach (var item in lastVersion.T_SC_SingleProjectScheme_MileStoneList.ToList())
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
            }
            if (!dt.Columns.Contains("MileStoneInfo")) dt.Columns.Add("MileStoneInfo");
            var dv = templateDt.DefaultView;
            dv.Sort = "SortIndex";
            dt.Rows[0]["MileStoneInfo"] = JsonHelper.ToJson(dv.ToTable());
        }

        private Dictionary<string, object> _createDefaultItem(DataRow template, string code, T_SC_SingleProjectScheme lastVersion)
        {
            var row = new Dictionary<string, object>();
            row.SetValue("Name", template["MileStoneName"].ToString());
            row.SetValue("SortIndex", template["SortIndex"].ToString());
            if (lastVersion != null)
            {
                var lastItem = lastVersion.T_SC_SingleProjectScheme_MileStoneList.FirstOrDefault(d => d.Code == code);
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
            if (entity == null) entity = new T_SC_SingleProjectScheme();
            this.UpdateEntity(entity, dic);
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

            #region 校验里程碑编号

            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(entity.ProjectInfoID);
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


        protected override void OnFlowEnd(T_SC_SingleProjectScheme entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
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
            if (entity!=null)
            {
                var selectCodes = entity.T_SC_SingleProjectScheme_MileStoneList.Select(a => a.Code).ToList();
                deleteMileStoneList = projectInfo.S_P_MileStone.Where(a => !selectCodes.Contains(a.Code)).ToList();
                entity.Push(dic);
            }
            //if (marketProject != null)
            //    marketProject.State = ProjectCommoneState.Execute.ToString();
            //marketEntities.SaveChanges();
            this.BusinessEntities.SaveChanges();

            ////同步关联的收款项的里程碑信息：时间、状态
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

    }
}
