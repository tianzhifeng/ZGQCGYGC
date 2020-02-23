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
    public class PetrifactionProjectSchemeController : ProjectFormContorllor<T_SC_PetrifactionProjectScheme>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            #region 校验整体专业
            var projectInfoID = dic.GetValue("ProjectInfoID");
            var major = dic.GetValue("Major");
            #region 校验成果
            string sql = @"select distinct MajorValue, S_W_WBS.Name MajorName from S_E_Product p
left join S_W_WBS on p.WBSID = S_W_WBS.ID 
where p.ProjectInfoID='{0}' and MajorValue not in ('{1}')";
            sql = string.Format(sql, projectInfoID, major.Replace(",", "','"));
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
            sqlPutInfo = string.Format(sqlPutInfo, projectInfoID, major.Replace(",", "','"));
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
            sqlShareInfo = string.Format(sqlShareInfo, projectInfoID, major.Replace(",", "','"));
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
            var finishMileStones = this.BusinessEntities.Set<S_P_MileStone>().Where(a => a.ProjectInfoID == projectInfoID && a.State == finishState).ToList();
            List<string> msgMileStoneMajor = new List<string>();
            foreach (var item in finishMileStones)
            {
                if (string.IsNullOrEmpty(item.MajorValue) && string.IsNullOrEmpty(item.OutMajorValue))
                    continue;
                if (!major.Split(',').Contains(item.MajorValue))
                {
                    if (!msgMileStoneMajor.Contains(item.MajorValue))
                        msgMileStoneMajor.Add(item.MajorValue);
                }
                if (!string.IsNullOrEmpty(item.OutMajorValue))
                {
                    foreach (var _oMajorValue in item.OutMajorValue.Split(','))
                    {
                        if (string.IsNullOrEmpty(_oMajorValue)) continue;
                        if (!major.Split(',').Contains(_oMajorValue))
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
        }

        protected override void OnFlowEnd(T_SC_PetrifactionProjectScheme entity, Workflow.Logic.Domain.S_WF_InsTaskExec taskExec, Workflow.Logic.Domain.S_WF_InsDefRouting routing)
        {
            var projectInfo = this.BusinessEntities.Set<S_I_ProjectInfo>().Find(entity.ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("根据当前项目策划单，未找到对应的项目信息");
            List<Dictionary<string, object>> majorUserList = null;
            if (!string.IsNullOrEmpty(entity.MajorList))
                majorUserList = JsonHelper.ToList(entity.MajorList);
            projectInfo.SynchMajorData(entity.Major);
            //从RBS同步人员到obs
            projectInfo.ResetOBSUserFromRBS();
            //人员加到obs表，以便wbs策划时快速选人
            if (majorUserList != null)
            {
                var roleDefineList = BaseConfigFO.GetRoleDefineList();
                foreach (var majorNode in majorUserList)
                {
                    var roleList = roleDefineList.Where(a => majorNode.ContainsKey(a.RoleCode) && !string.IsNullOrEmpty(majorNode.GetValue(a.RoleCode)));
                    foreach (var roleDef in roleList)
                    {
                        foreach (var userID in majorNode.GetValue(roleDef.RoleCode).Split(','))
                        {
                            var user = FormulaHelper.GetService<IUserService>().GetUserInfoByID(userID);
                            projectInfo.SetOBSUser(user, roleDef.RoleCode, majorNode.GetValue("MajorCode"), majorNode.GetValue("MajorName"));
                        }
                    }
                }
            }


            //base.OnFlowEnd(entity, taskExec, routing);
        }
    }
}
