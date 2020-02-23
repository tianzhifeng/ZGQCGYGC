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
    public class ProjectSchemeController : ProjectFormContorllor<T_SC_SchemeForm>
    {
        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            var enumService = FormulaHelper.GetService<IEnumService>();
            var UpperVersion = HttpContext.Request["UpperVersion"];
            if (isNew && String.IsNullOrEmpty(UpperVersion)) //新增时候并且不是升版的情况下才默认增加子项内容
            {
                if (dt.Rows.Count > 0 && dt.Columns.Contains("SubProjectList"))
                {
                    var subProjectList = new List<Dictionary<string, object>>();
                    if (dt.Columns.Contains("ProjectInfoID"))
                    {
                        var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(dt.Rows[0]["ProjectInfoID"].ToString());
                        if (projectInfo != null && projectInfo.ProjectMode.ExtentionObject.GetValue("Ext_TemporaryMode") == TrueOrFalse.True.ToString())
                        {
                            var phaseInfo = projectInfo.PhaseValue.Split(',');
                            var phaesNameInfo = projectInfo.PhaseName.Split(',');
                            for (int i = 0; i < phaseInfo.Length; i++)
                            {
                                var phaseValue = phaseInfo[i];
                                if (string.IsNullOrEmpty(phaseValue)) continue;
                                var phaseName = enumService.GetEnumText("Project.Phase", phaseValue);
                                var item = new Dictionary<string, object>();
                                item.SetValue("Name", phaseName);
                                item.SetValue("PhaseValue", phaseValue);
                                item.SetValue("Code", FormulaHelper.CreateGuid());
                                item.SetValue("Area", projectInfo.Proportion);
                                subProjectList.Add(item);
                            }
                        }
                    }
                    dt.Rows[0]["SubProjectList"] = JsonHelper.ToJson(subProjectList);
                }
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic["ID"]);
            if (entity == null) entity = new T_SC_SchemeForm();
            var sublist = JsonHelper.ToList(dic.GetValue("SubProjectList"));
            if (dic.ContainsKey("SubProjectList"))
            {
                foreach (var item in sublist)
                {
                    if (String.IsNullOrEmpty(item.GetValue("Code")))
                        item.SetValue("Code", FormulaHelper.CreateGuid());
                }
                dic.SetValue("SubProjectList", JsonHelper.ToJson(sublist));
            }
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

            #region 校验子项
            var newsubid = sublist.Select(t => t.GetValue("Code"));
            var enumSubProject = WBSNodeType.SubProject.ToString();
            var wbsList = this.BusinessEntities.Set<S_W_WBS>().Where(a => a.ProjectInfoID == entity.ProjectInfoID).ToList();
            var subWBSList = wbsList.Where(a => a.WBSType == enumSubProject).ToList();
            #region 校验成果
            subWBSList
                .Where(w => !newsubid.Contains(w.WBSValue))
                .ToList()
                .ForEach(t =>
                {
                    if (t.HasProducts())
                        throw new Formula.Exceptions.BusinessException("子项【" + t.Name + "】已经有成果图纸，不能删除。");
                });
            #endregion

            var WBSIDList = wbsList.Where(a => newsubid.Contains(a.WBSValue)).Select(a => a.ID).ToList();
            #region 校验提资单
            var putInfoList = this.BusinessEntities.Set<T_EXE_MajorPutInfo>().Where(a => a.ProjectInfoID == entity.ProjectInfoID).ToList();
            var hasPutInfo = putInfoList.Where(a => !WBSIDList.Contains(a.WBSID)).ToList();
            if (hasPutInfo.Count > 0)
            {
                var name = "";
                for (int i = 0; i < hasPutInfo.Count; i++)
                {
                    var pInfo = hasPutInfo[i];
                    if (!string.IsNullOrEmpty(pInfo.WBSID))
                    {
                        var _wbs = wbsList.FirstOrDefault(a => a.ID == pInfo.WBSID);
                        if (_wbs != null)
                        {
                            var subNode = subWBSList.FirstOrDefault(a => _wbs.FullID.Split('.').Contains(a.ID));
                            if (subNode != null)
                                name += subNode.Name + ",";
                        }
                    }
                }
                throw new Formula.Exceptions.BusinessException("子项【" + name.TrimEnd(',') + "】发起过提资，不能删除。");
            }
            #endregion

            #region 校验共享区
            var shareInfoList = this.BusinessEntities.Set<S_D_ShareInfo>().Where(a => a.ProjectInfoID == entity.ProjectInfoID).ToList();
            var hasShareInfo = shareInfoList.Where(a => !WBSIDList.Contains(a.SubProjectWBSID)).ToList();
            if (hasShareInfo.Count > 0)
            {
                var name = "";
                for (int i = 0; i < hasShareInfo.Count; i++)
                {
                    var sInfo = hasShareInfo[i];
                    if (!string.IsNullOrEmpty(sInfo.SubProjectWBSID))
                    {
                        var _wbs = wbsList.FirstOrDefault(a => a.ID == sInfo.SubProjectWBSID);
                        if (_wbs != null)
                        {
                            var subNode = subWBSList.FirstOrDefault(a => _wbs.FullID.Split('.').Contains(a.ID));
                            if (subNode != null)
                                name += subNode.Name + ",";
                        }
                    }
                }
                throw new Formula.Exceptions.BusinessException("子项【" + name.TrimEnd(',') + "】拥有共享区文件，不能删除。");
            }
            #endregion
            #endregion

            #region 校验子项下的专业
            foreach (var subProject in sublist)
            {
                if (!String.IsNullOrEmpty(subProject.GetValue("RBSJson")))
                {
                    var rbsList = JsonHelper.ToList(subProject.GetValue("RBSJson"));
                    var subProjectWBSValue = subProject.GetValue("Code");
                    var subProjectWBSNode = wbsList.FirstOrDefault(a => a.WBSValue == subProjectWBSValue);
                    if (subProjectWBSNode == null)
                        continue;
                    foreach (var majorNode in rbsList)
                    {
                        if (majorNode.GetValue("Valid") == "0")
                        {
                            var code = majorNode.GetValue("MajorCode");
                            #region 校验成果
                            wbsList
                                .Where(w => w.ParentID == subProjectWBSNode.ID && w.WBSValue == code)
                                .ToList().ForEach(t =>
                                {
                                    if (t.HasProducts())
                                        throw new Formula.Exceptions.BusinessException("子项【" + subProject.GetValue("Name") + "】下的专业【" + t.Name + "】已经有成果图纸，不能删除。");
                                });
                            #endregion

                            #region 校验提资单
                            var hasPutInfo2 = putInfoList.Where(a => a.WBSID == subProjectWBSNode.ID && a.OutMajorValue == code).ToList();
                            if (hasPutInfo2.Count > 0)
                                throw new Formula.Exceptions.BusinessException("子项【" + subProject.GetValue("Name") + "】下的专业【" + majorNode.GetValue("MajorName") + "】发起过提资，不能删除。");
                            #endregion

                            #region 校验共享区
                            var hasShareInfo2 = shareInfoList.Where(a => a.SubProjectWBSID == subProjectWBSNode.ID && a.WBSValue == code).ToList();
                            if (hasShareInfo2.Count > 0)
                                throw new Formula.Exceptions.BusinessException("子项【" + subProject.GetValue("Name") + "】下的专业【" + majorNode.GetValue("MajorName") + "】拥有共享区文件，不能删除。");
                            #endregion

                            #region 校验专业里程碑
                            if (finishMileStones.Exists(a => a.WBSID == subProjectWBSNode.ID && (
                                (!string.IsNullOrEmpty(a.MajorValue) && a.MajorValue == code)
                                || (!string.IsNullOrEmpty(a.OutMajorValue) && a.OutMajorValue.Split(',').Contains(code))
                            )))
                                throw new Formula.Exceptions.BusinessException("子项【" + subProject.GetValue("Name") + "】下的专业【" + majorNode.GetValue("MajorName") + "】已经有相关里程碑完成，不能删除。");
                            #endregion
                        }
                    }
                }
            }
            #endregion
        }


        //流程结束逻辑
        protected override void OnFlowEnd(T_SC_SchemeForm entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
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
                schemeDic.RemoveWhere(a=>ignorKeys.Contains(a.Key));
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
                //marketProject.ProjectScale = projectInfo.ProjectLevel.HasValue?projectInfo.ProjectLevel.Value.ToString():"";
            }
            if (entity != null)
            {
                entity.Push();
            }
            //if (entity != null)
            //    entity.Push(dic);
            //if (marketProject != null)
            //    marketProject.State = projectInfo.State;
            //marketEntities.SaveChanges();
            projectInfo.SynchMajorData(entity.Major);
            this.BusinessEntities.SaveChanges();
            projectInfo.BuildNotice();
        }

        public JsonResult ValidateSubProjectRemove(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var name = item.GetValue("Code");
                var type = WBSNodeType.SubProject.ToString();
                var wbs = this.BusinessEntities.Set<S_W_WBS>().FirstOrDefault(d => d.WBSValue == name && d.WBSType == type);
                if (wbs == null) continue;
                if (wbs.HasProducts())
                {
                    throw new Formula.Exceptions.BusinessException("子项【" + item.GetValue("Name") + "】已经有成果图纸，不能删除");
                }
            }
            return Json("");
        }

    }
}
