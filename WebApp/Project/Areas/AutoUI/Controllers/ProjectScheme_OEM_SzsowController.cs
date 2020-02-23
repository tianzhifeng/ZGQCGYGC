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
    public class ProjectScheme_OEM_SzsowController : ProjectFormContorllor<T_SC_SchemeForm_OEM_Szsow>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            var entity = this.GetEntityByID(dic["ID"]);
            if (entity == null) entity = new T_SC_SchemeForm_OEM_Szsow();
            var sublist = JsonHelper.ToList(dic.GetValue("SubProjectList"));
            foreach (var item in sublist)
            {
                if (String.IsNullOrEmpty(item.GetValue("Code")))
                    item.SetValue("Code", FormulaHelper.CreateGuid());
            }

            var phaseEnums = EnumBaseHelper.GetEnumDef("Project.Phase").EnumItem.ToList();
            string phaseExistError = string.Empty, phaseRepeatError = string.Empty, phaseStructError = string.Empty;
            string phaseError = string.Empty;

            #region 阶段校验
            //校验项目阶段必须在子项信息子表中的设计阶段出现
            //校验阶段+名称不能重复
            //不支持阶段下既有子项、又有专业的情况
            var projPhaseArr = dic.GetValue("PhaseValue").Split(',');
            foreach (var projPhase in projPhaseArr)
            {
                //校验项目阶段必须在子项信息子表中的设计阶段出现
                if (!sublist.Any(a => a.GetValue("PhaseValue") == projPhase))
                    phaseExistError += projPhase + ",";
                var list = sublist.Where(a => a.GetValue("PhaseValue") == projPhase).ToList();
                //校验阶段+名称不能重复
                phaseRepeatError = string.Empty;
                var group = list.GroupBy(a => a.GetValue("Name")).ToList();
                foreach (var item in group)
                {
                    if (item.Count() > 1)
                        phaseRepeatError += "阶段【" + phaseEnums.FirstOrDefault(a => a.Code == projPhase).Name + "】下子项【" + item.Key + "】不能重复。</br>";
                }
                if (!string.IsNullOrEmpty(phaseRepeatError))
                    phaseError += phaseRepeatError;
                //不支持阶段下既有子项、又有专业的情况
                phaseStructError = string.Empty;
                if (group.Any(a => string.IsNullOrEmpty(a.Key)) && group.Any(a => !string.IsNullOrEmpty(a.Key)))
                    phaseStructError += "阶段【" + phaseEnums.FirstOrDefault(a => a.Code == projPhase).Name + "】下不能同时子项为空和子项不为空的数据。</br>";
                if (!string.IsNullOrEmpty(phaseStructError))
                    phaseError += phaseStructError;
            }
            if (!string.IsNullOrEmpty(phaseExistError))
            {
                phaseExistError = String.Join(",", phaseEnums.Where(d => phaseExistError.TrimEnd(',').Split(',').Contains(d.Code)).Select(d => d.Name));
                phaseError += "必须在子项信息中添加阶段【" + phaseExistError + "】的数据。</br>";
            }
            if (!string.IsNullOrEmpty(phaseError))
                throw new Formula.Exceptions.BusinessException(phaseError);
            #endregion

            if (dic.ContainsKey("SubProjectList"))
                dic.SetValue("SubProjectList", JsonHelper.ToJson(sublist));

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
            //sll edit 2018-8-8
            //没有Name也视为要删除项
            //before  var newsubid = sublist.Select(t => t.GetValue("Code"));
            var newsubid = sublist.Where(a => !string.IsNullOrEmpty(a.GetValue("Name"))).Select(t => t.GetValue("Code"));
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


                    //sll add 2018-8-8
                    var subProjectWBSValue = "";
                    string errorExceptionPre = "";
                    //名称不为空，专业挂子项
                    if (!string.IsNullOrEmpty(subProject.GetValue("Name")))
                    {
                        subProjectWBSValue = subProject.GetValue("Code");
                        errorExceptionPre = "子项";
                    }
                    //名称为空, 专业挂阶段
                    else
                    {
                        subProjectWBSValue = subProject.GetValue("PhaseValue");
                        errorExceptionPre = "专业";
                    }


                    foreach (var majorNode in rbsList)
                    {
                        if (majorNode.GetValue("Valid") == "0")
                        {
                            var subProjectWBSNode = wbsList.FirstOrDefault(a => a.WBSValue == subProjectWBSValue);

                            //sll 2018-8-9 add 
                            //子项名称为空到有名称后但是wbs中没有该子项节点,此时略过不需要判断
                            if (subProjectWBSNode == null)
                                continue;

                            var code = majorNode.GetValue("MajorCode");
                            #region 校验成果
                            wbsList
                                .Where(w => w.ParentID == subProjectWBSNode.ID && w.WBSValue == code)
                                .ToList().ForEach(t =>
                                {
                                    if (t.HasProducts())
                                        throw new Formula.Exceptions.BusinessException(errorExceptionPre + "【" + subProject.GetValue("Name") + "】下的专业【" + t.Name + "】已经有成果图纸，不能删除。");
                                });
                            #endregion

                            #region 校验提资单
                            var hasPutInfo2 = putInfoList.Where(a => a.WBSID == subProjectWBSNode.ID && a.OutMajorValue == code).ToList();
                            if (hasPutInfo2.Count > 0)
                                throw new Formula.Exceptions.BusinessException(errorExceptionPre + "【" + subProject.GetValue("Name") + "】下的专业【" + majorNode.GetValue("MajorName") + "】发起过提资，不能删除。");
                            #endregion

                            #region 校验共享区
                            var hasShareInfo2 = shareInfoList.Where(a => a.SubProjectWBSID == subProjectWBSNode.ID && a.WBSValue == code).ToList();
                            if (hasShareInfo2.Count > 0)
                                throw new Formula.Exceptions.BusinessException(errorExceptionPre + "【" + subProject.GetValue("Name") + "】下的专业【" + majorNode.GetValue("MajorName") + "】拥有共享区文件，不能删除。");
                            #endregion

                            #region 校验专业里程碑
                            if (finishMileStones.Exists(a => a.WBSID == subProjectWBSNode.ID && (
                                (!string.IsNullOrEmpty(a.MajorValue) && a.MajorValue == code)
                                || (!string.IsNullOrEmpty(a.OutMajorValue) && a.OutMajorValue.Split(',').Contains(code))
                            )))
                                throw new Formula.Exceptions.BusinessException(errorExceptionPre + "【" + subProject.GetValue("Name") + "】下的专业【" + majorNode.GetValue("MajorName") + "】已经有相关里程碑完成，不能删除。");
                            #endregion
                        }
                    }
                }
            }
            #endregion
        }

        protected override void AfterGetData(DataTable dt, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                var projectInfoID = dt.Rows[0]["ProjectInfoID"].ToString();
                var sql = string.Format("select * from S_W_WBS with(nolock) where ProjectInfoID = '{0}'", projectInfoID);
                var wbsDT = this.ProjectSQLDB.ExecuteDataTable(sql);
                sql = string.Format("select * from S_W_RBS where ProjectInfoID = '{0}'", projectInfoID);
                var rbsDT = this.ProjectSQLDB.ExecuteDataTable(sql);
                var schemeSubProject = JsonHelper.ToList(dt.Rows[0]["SubProjectList"].ToString());
                var schemeMajorList = JsonHelper.ToList(dt.Rows[0]["MajorList"].ToString());
                var roleCodes = BaseConfigFO.GetRoleDefineList().Select(a => a.RoleCode).ToArray();
                if (schemeMajorList.Count() > 0)
                {
                    var attendRoles = schemeMajorList[0].Keys.Where(a => roleCodes.Contains(a)).ToArray();
                    foreach (var subProject in schemeSubProject)
                    {
                        var subProjectWBSID = subProject.GetValue("SubProjectWBSID");
                        //如果没有子项WBSID，则说明异构结构中，专业直接挂在了阶段节点下
                        if (String.IsNullOrEmpty(subProjectWBSID)) subProjectWBSID = subProject.GetValue("PhaseWBSID");                        
                        if (string.IsNullOrEmpty(subProject.GetValue("RBSJson")))
                        {
                            #region 没有RBSJson
                            //没有RBSJson，说明所有专业都参与
                            //没有RBSJson，需要判断是否修改过专业人员
                            var schemeToWrite = new List<Dictionary<string, object>>();
                            var flag = false;
                            for (int i = 0; i < schemeMajorList.Count(); i++)
                            {
                                var schemeMajor = schemeMajorList[i];
                                var rbsToWrite = new Dictionary<string, object>();
                                rbsToWrite.SetValue("SortIndex", i);
                                rbsToWrite.SetValue("MajorName", schemeMajor.GetValue("MajorName"));
                                rbsToWrite.SetValue("MajorCode", schemeMajor.GetValue("MajorCode"));
                                rbsToWrite.SetValue("Valid", "1");
                                rbsToWrite.SetValue("IsChange", false);

                                var majorWBS = wbsDT.AsEnumerable().Where(a => a["ParentID"].ToString() == subProjectWBSID
                                    && a["Code"].ToString() == schemeMajor.GetValue("MajorCode")).ToArray();
                                if (majorWBS.Length == 0) continue;
                                foreach (var aRole in attendRoles)
                                {
                                    var rbsList = rbsDT.AsEnumerable().Where(a => a["WBSID"].ToString() == majorWBS[0]["ID"].ToString()
                                        && a["RoleCode"].ToString() == aRole).ToArray();
                                    var userIDs = rbsList.Select(a => a["UserID"].ToString()).ToArray();
                                    var userNames = rbsList.Select(a => a["UserName"].ToString()).ToArray();
                                    rbsToWrite.SetValue(aRole, string.Join(",", userIDs));
                                    rbsToWrite.SetValue(aRole + "Name", string.Join(",", userNames));
                                    //判断aRole角色在rbs中和策划的专业人员是否相同
                                    var isSame = true;
                                    var schemeMajorRBS = schemeMajor.GetValue(aRole).Split(',');
                                    if (userIDs.Length == schemeMajorRBS.Length)
                                    {
                                        var q = (from a in userIDs join b in schemeMajorRBS on a equals b select a).ToArray();
                                        if (q.Length != userIDs.Length)
                                            isSame = false;
                                    }
                                    //schemeMajor.GetValue(aRole)会产生空字符串
                                    else if (!(userIDs.Length == 0 && schemeMajor.GetValue(aRole) == ""))
                                        isSame = false;
                                    if (!isSame)
                                    {
                                        flag = true;
                                        rbsToWrite.SetValue("IsChange", true);
                                    }
                                }
                                schemeToWrite.Add(rbsToWrite);
                            }
                            if (flag)
                                subProject.SetValue("RBSJson", JsonHelper.ToJson(schemeToWrite));
                            #endregion
                        }
                        else
                        {
                            #region 有RBSJson
                            //有RBSJson，替换人员
                            var schemeRBS = JsonHelper.ToList(subProject.GetValue("RBSJson"));
                            var tmpRBS = JsonHelper.ToList(JsonHelper.ToJson(schemeRBS));
                            for (int i = 0; i < schemeRBS.Count(); i++)
                            {
                                var sRBS = schemeRBS[i];
                                if (sRBS.GetValue("Valid") == "1")
                                {
                                    var majorWBS = wbsDT.AsEnumerable().Where(a => a["ParentID"].ToString() == subProjectWBSID
                                        && a["Code"].ToString() == sRBS.GetValue("MajorCode")).ToArray();
                                    if (majorWBS.Length == 0) continue;
                                    var roleDefines = sRBS.Keys.Where(a => roleCodes.Contains(a));
                                    foreach (var roleDefine in roleDefines)
                                    {
                                        var rbsList = rbsDT.AsEnumerable().Where(a => a["WBSID"].ToString() == majorWBS[0]["ID"].ToString()
                                            && a["RoleCode"].ToString() == roleDefine).ToArray();
                                        var userIDs = rbsList.Select(a => a["UserID"].ToString()).ToArray();
                                        var userNames = rbsList.Select(a => a["UserName"].ToString()).ToArray();
                                        var schemeUserIDs = tmpRBS[i].GetValue(roleDefine).Split(',');
                                        tmpRBS[i].SetValue(roleDefine, string.Join(",", userIDs));
                                        tmpRBS[i].SetValue(roleDefine + "Name", string.Join(",", userNames));
                                        var isSame = true;
                                        if (userIDs.Length == schemeUserIDs.Length)
                                        {
                                            var q = (from a in userIDs join b in schemeUserIDs on a equals b select a).ToArray();
                                            if (q.Length != userIDs.Length)
                                                isSame = false;
                                        }
                                        //schemeMajor.GetValue(aRole)会产生空字符串
                                        else if (!(userIDs.Length == 0 && tmpRBS[i].GetValue(roleDefine) == ""))
                                            isSame = false;
                                        if (!isSame)
                                            tmpRBS[i].SetValue("IsChange", true);
                                    }
                                }
                            }
                            subProject.SetValue("RBSJson", JsonHelper.ToJson(tmpRBS));
                            #endregion
                        }
                    }
                    dt.Rows[0]["SubProjectList"] = JsonHelper.ToJson(schemeSubProject);
                }
            }
        }

        //流程结束逻辑
        protected override void OnFlowEnd(T_SC_SchemeForm_OEM_Szsow entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            string formData = Request["FormData"];
            Dictionary<string, string> dic = JsonHelper.ToObject<Dictionary<string, string>>(formData);
            if (entity != null)
                entity.Push(dic);
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(entity.ProjectInfoID);
            //var marketEntities = Formula.FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>();
            //var marketProject = marketEntities.Set<Market.Logic.Domain.S_I_Project>().FirstOrDefault(d => d.ID == projectInfo.MarketProjectInfoID);
            //if (marketProject != null)
            //    marketProject.State = projectInfo.State;
            //marketEntities.SaveChanges();
            this.BusinessEntities.SaveChanges();
        }

        public JsonResult ValidateSubProjectRemove(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var name = item.GetValue("Name");
                var code = item.GetValue("Code");
                var phaseValue = item.GetValue("PhaseValue");
                if (string.IsNullOrEmpty(name))
                {
                    var type = WBSNodeType.Phase.ToString();
                    var wbs = this.BusinessEntities.Set<S_W_WBS>().FirstOrDefault(d => d.WBSValue == phaseValue && d.WBSType == type);
                    if (wbs == null) continue;
                    if (wbs.HasProducts())
                        throw new Formula.Exceptions.BusinessException("阶段【" + wbs.Name + "】已经有成果图纸，不能删除");
                }
                else
                {
                    var type = WBSNodeType.SubProject.ToString();
                    var wbs = this.BusinessEntities.Set<S_W_WBS>().FirstOrDefault(d => d.WBSValue == code && d.WBSType == type);
                    if (wbs == null) continue;
                    if (wbs.HasProducts())
                        throw new Formula.Exceptions.BusinessException("子项【" + name + "】已经有成果图纸，不能删除");
                }
            }
            return Json("");
        }

    }
}
