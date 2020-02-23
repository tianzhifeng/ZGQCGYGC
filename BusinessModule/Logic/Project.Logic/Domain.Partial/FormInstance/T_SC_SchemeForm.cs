using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Config;
using Formula.Helper;
using Formula;
using Config.Logic;
using System.Web;

namespace Project.Logic.Domain
{
    public partial class T_SC_SchemeForm
    {

        public void Push()
        {
            var allUser = FormulaHelper.GetService<IUserService>().GetAllUsers();
            var userInfo = FormulaHelper.GetUserInfo();
            var fo = FormulaHelper.CreateFO<ProjectInfoFO>();
            var roleDefine = BaseConfigFO.GetRoleDefineList();
            string formData = HttpContext.Current.Request["FormData"];
            var dic = JsonHelper.ToObject(formData);
            var DB = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            var newRbsDt = DB.ExecuteDataTable("SELECT * FROM S_W_RBS WITH(NOLOCK) WHERE 1<>1");
            var wbsDt = DB.ExecuteDataTable("SELECT 'false' as AddState,* FROM S_W_WBS WITH(NOLOCK) WHERE PROJECTINFOID='" + this.ProjectInfoID + "'");
            var monomerDt = DB.ExecuteDataTable("SELECT * FROM S_W_Monomer WHIT(NOLOCK) WHERE PROJECTINFOID='" + this.ProjectInfoID + "'");
            var projectInfoDt = DB.ExecuteDataTable(String.Format("SELECT * FROM S_I_PROJECTINFO WHERE ID='{0}'", this.ProjectInfoID));
            if (projectInfoDt.Rows.Count == 0) { throw new Formula.Exceptions.BusinessException("根据当前项目策划单，未找到对应的项目信息"); }
            var projectRow = projectInfoDt.Rows[0];
            var rootWBS = wbsDt.AsEnumerable().FirstOrDefault(c => c["WBSType"].ToString() == "Project");
            if (rootWBS == null) { throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。"); }
            var configBaseEntities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var modeCode = projectRow["ModeCode"].ToString();
            var mode = configBaseEntities.S_T_ProjectMode.FirstOrDefault(c => c.ModeCode == modeCode);
            if (mode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到对应的项目模式，策划失败");
            var structList = mode.S_T_WBSStructInfo.ToList();

            var sqlCommand = new StringBuilder();
            var newMonomerDT = monomerDt.Clone();

            #region 设置项目的RBS
            //重设项目经理
            if (!String.IsNullOrEmpty(this.ChargeUser))
            {
                var pmRoleDefine = roleDefine.FirstOrDefault(c => c.RoleCode == ProjectRole.ProjectManager.ToString());
                if (pmRoleDefine == null) throw new Formula.Exceptions.BusinessException("没有项目负责人的角色定义，无法设置项目负责人。");
                sqlCommand.AppendLine(String.Format("DELETE FROM S_W_RBS WHERE WBSID='{0}' AND ROLECODE='{1}'", rootWBS["ID"], ProjectRole.ProjectManager.ToString()));
                foreach (var userID in this.ChargeUser.Split(','))
                {
                    var user = allUser.SingleOrDefault(c => c.UserID == userID);
                    if (user == null) continue;
                    addRbsDt(userInfo, newRbsDt, rootWBS, pmRoleDefine);
                }
            }
            var roleList = BaseConfigFO.GetRoleDefineList().Where(a => dic.ContainsKey(a.RoleCode) && !string.IsNullOrEmpty(dic.GetValue(a.RoleCode)));
            foreach (var roleDef in roleList)
            {
                sqlCommand.AppendLine(String.Format("DELETE FROM S_W_RBS WHERE WBSID='{0}' AND ROLECODE='{1}'", rootWBS["ID"], roleDef.RoleCode));
                foreach (var userID in this.ChargeUser.Split(','))
                {
                    var user = allUser.SingleOrDefault(c => c.UserID == userID);
                    if (user == null) continue;
                    addRbsDt(user, newRbsDt, rootWBS, roleDef);
                }
            }
            #endregion

            wbsDt.PrimaryKey = new DataColumn[] { wbsDt.Columns["ID"] };
            var subwbsList = wbsDt.AsEnumerable().Where(c => c["ParentID"] != DBNull.Value && c["ParentID"].ToString() == rootWBS["ID"].ToString());

            #region 设置WBS
            if (this.T_SC_SchemeForm_SubProjectList.ToList().Count == 0)   //如果没有策划任何子项，则默认建立一个子项，子项名称为项目阶段
            {
                var subProjectListItem = new T_SC_SchemeForm_SubProjectList();
                subProjectListItem.ID = FormulaHelper.CreateGuid(); ;
                subProjectListItem.Name = projectRow["PhaseName"] == null || projectRow["PhaseName"] == DBNull.Value ? "" : projectRow["PhaseName"].ToString();
                subProjectListItem.Code = projectRow["PhaseValue"] == null || projectRow["PhaseValue"] == DBNull.Value ? "" : projectRow["PhaseValue"].ToString();
                subProjectListItem.Unit = "";
                subProjectListItem.PhaseValue = projectRow["PhaseValue"] == null || projectRow["PhaseValue"] == DBNull.Value ? "" : projectRow["PhaseValue"].ToString();
                this.T_SC_SchemeForm_SubProjectList.Add(subProjectListItem);
                this.SubProjectList = JsonHelper.ToJson(this.T_SC_SchemeForm_SubProjectList);
            }
            var subProjectStr = WBSNodeType.SubProject.ToString();
            var subPrjWBSNodes = wbsDt.AsEnumerable().Where(a => a["WBSType"].ToString() == subProjectStr).ToList();
            //需要删除的子项
            var subPrjIDWBSIDs = T_SC_SchemeForm_SubProjectList.Select(a => a.WBSID).ToArray();
            var delSubProjects = subPrjWBSNodes.Where(a => !subPrjIDWBSIDs.Contains(a["ID"].ToString()));
            foreach (var item in delSubProjects)
            {
                sqlCommand.AppendLine("delete from S_W_WBS where FullID like '" + item["FullID"].ToString() + "%'");
            }
            foreach (var subProject in this.T_SC_SchemeForm_SubProjectList.OrderBy(d => d.SortIndex).ToList())
            {
                if (String.IsNullOrEmpty(subProject.WBSID))
                {
                    subProject.WBSID = FormulaHelper.CreateGuid();
                }
                var subWBSNode = wbsDt.Rows.Find(subProject.WBSID);  //subwbsList.FirstOrDefault(d => d["ID"].ToString() == subProject.WBSID);
                if (subWBSNode == null)
                {
                    subWBSNode = wbsDt.NewRow();
                    subWBSNode["ID"] = subProject.WBSID;
                    subWBSNode["Name"] = subProject.Name;
                    subWBSNode["WBSType"] = WBSNodeType.SubProject.ToString();
                    subWBSNode["WBSValue"] = subProject.Code;
                    subWBSNode["PhaseCode"] = subProject.PhaseValue;
                    subWBSNode["ExtField1"] = subProject.Area.HasValue ? subProject.Area.ToString() : "";
                    subWBSNode["ExtField2"] = subProject.Unit;
                    fo.AddWBSChildWithAdo(rootWBS, subWBSNode, wbsDt, structList);
                }
                else
                {
                    sqlCommand.AppendLine(String.Format("update S_W_WBS set Name='{0}', PhaseCode='{1}',ExtField1='{2}',ExtField2='{3}' where ID ='{4}'",
                        subProject.Name,
                        subProject.PhaseValue,
                        subProject.Area.HasValue ? subProject.Area.ToString() : "",
                        subProject.Unit,
                        subWBSNode["ID"]
                        ));
                    var allChildren = wbsDt.AsEnumerable().Where(c => c["FullID"].ToString().StartsWith(subWBSNode["FullID"].ToString()) && c["ID"].ToString() != subWBSNode["ID"].ToString());
                    foreach (DataRow item in allChildren)
                    {
                        sqlCommand.AppendLine(String.Format("update S_W_WBS set PhaseCode='{0}' where ID ='{1}'", subProject.PhaseValue, item["ID"]));
                    }
                }
                #region 新增单体
                var wonomerList = monomerDt.AsEnumerable().Where(c => c["WBSID"].ToString() == subWBSNode["ID"].ToString()).ToList();
                if (String.IsNullOrEmpty(subProject.Unit))
                {
                    var wonomer = wonomerList.FirstOrDefault(d => d["Name"].ToString() == subProject.Name);
                    if (wonomer == null)
                    {
                        wonomer = newMonomerDT.NewRow();
                        wonomer["ID"] = FormulaHelper.CreateGuid();
                        wonomer["WBSID"] = subWBSNode["ID"].ToString();
                        wonomer["ProjectInfoID"] = this.ProjectInfoID;
                        wonomer["Name"] = subProject.Name;
                        wonomer["CreateDate"] = DateTime.Now;
                        wonomer["CreateUser"] = userInfo.UserName;
                        wonomer["CreateUserID"] = userInfo.UserID;
                        wonomer["SchemeFormSubID"] = this.ID;
                        wonomer["Code"] = String.IsNullOrEmpty(subProject.Code) ? subProject.Name : subProject.Code;
                        newMonomerDT.Rows.Add(wonomer);
                    }
                }
                else
                {
                    foreach (var item in subProject.Unit.Replace("，", ",").Split(','))
                    {
                        var wonomer = wonomerList.FirstOrDefault(d => d["Name"].ToString() == item);
                        if (wonomer == null)
                        {
                            wonomer = newMonomerDT.NewRow();
                            wonomer["ID"] = FormulaHelper.CreateGuid();
                            wonomer["WBSID"] = subWBSNode["ID"].ToString();
                            wonomer["ProjectInfoID"] = this.ProjectInfoID;
                            wonomer["Name"] = item;
                            wonomer["CreateDate"] = DateTime.Now;
                            wonomer["CreateUser"] = userInfo.UserName;
                            wonomer["CreateUserID"] = userInfo.UserID;
                            wonomer["SchemeFormSubID"] = this.ID;
                            wonomer["Code"] = item;
                            newMonomerDT.Rows.Add(wonomer);
                        }
                        else
                        {
                            wonomerList.Remove(wonomer);
                        }
                    }
                }
                sqlCommand.AppendLine("delete from S_W_Monomer where ID in ('" + String.Join(",", wonomerList.Select(c => c["ID"].ToString())).Replace(",", "','") + "')");

                #endregion

                setMajorNodes(subWBSNode, wbsDt, newRbsDt, roleDefine, subProject.RBSJson, sqlCommand, structList, allUser);
            }
            #endregion

            var newWBSDt = wbsDt.Clone();
            var addesRows = wbsDt.AsEnumerable().Where(c => c["AddState"] != null && c["AddState"] != DBNull.Value && c["AddState"].ToString() == "true").ToList();
            foreach (var item in addesRows)
            {
                newWBSDt.ImportRow(item);
            }
            newWBSDt.Columns.Remove("AddState");
            DB.BulkInsertToDB(newWBSDt, "S_W_WBS");
            DB.BulkInsertToDB(newMonomerDT, "S_W_Monomer");
            DB.BulkInsertToDB(newRbsDt, "S_W_RBS");
            fo.ResetOBSUserFromRBSWithAdo(this.ProjectInfoID);
            if (!String.IsNullOrEmpty(sqlCommand.ToString()))
                DB.ExecuteNonQuery(sqlCommand.ToString());
        }


        void setMajorNodes(DataRow subProjectNode, DataTable wbsDt, DataTable rbsDt, List<S_D_RoleDefine> roleDefineList, string rbsList, StringBuilder sqlCommand,
            List<S_T_WBSStructInfo> structList, IList<UserInfo> allUser)
        {
            var fo = FormulaHelper.CreateFO<ProjectInfoFO>();
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            if (subProjectNode == null) throw new Formula.Exceptions.BusinessException("传入的WBS节点为空。");
            var majorList = "";
            if (!string.IsNullOrEmpty(rbsList))
                majorList = rbsList;
            else
                majorList = this.MajorList;
            #region 删除不需要的专业
            var delMajors = wbsDt.AsEnumerable().Where(c => c["ParentID"] != DBNull.Value &&
                c["ParentID"].ToString() == subProjectNode["ID"].ToString()
                && !this.Major.Contains(c["WBSValue"].ToString())).ToList();  //parentNode.Children.Where(d => !this.Major.Contains(d.WBSValue)).ToList();
            foreach (var item in delMajors)
            {
                fo.DeleteWBSNodeWithADO(item, wbsDt, "", sqlCommand);
            }
            #endregion

            if (!String.IsNullOrEmpty(majorList))
            {
                var majorNodes = JsonHelper.ToList(majorList);
                var majorAttrList = BaseConfigFO.GetWBSAttrList(WBSNodeType.Major);
                var majorWBSType = WBSNodeType.Major.ToString();
                foreach (var majorNode in majorNodes)
                {
                    if (majorNode.GetValue("Valid") == "0")
                    {
                        var thisMajorValue = majorNode.GetValue("MajorCode");
                        var thisMajorNode = wbsDt.AsEnumerable().FirstOrDefault(c => c["ParentID"] != DBNull.Value
                            && c["ParentID"] != null && c["ParentID"].ToString() == subProjectNode["ID"].ToString() &&
                            c["WBSValue"].ToString() == thisMajorValue);
                        if (thisMajorNode != null)
                        {
                            fo.DeleteWBSNodeWithADO(thisMajorNode, wbsDt, this.ProjectInfoID, sqlCommand);
                        }
                        continue;
                    }
                    var value = majorNode.GetValue("MajorCode");
                    var majorAttr = majorAttrList.SingleOrDefault(c => c.Code == value);
                    if (majorAttr == null) continue;
                    var childNode = wbsDt.AsEnumerable().FirstOrDefault(c => c["ParentID"] != DBNull.Value
                          && c["ParentID"] != null && c["ParentID"].ToString() == subProjectNode["ID"].ToString() &&
                          c["WBSValue"].ToString() == value);
                    if (!this.Major.Split(',').Contains(value))
                    {
                        if (childNode != null)
                        {
                            fo.DeleteWBSNodeWithADO(childNode, wbsDt, this.ProjectInfoID, sqlCommand);
                        }
                        continue;
                    }
                    if (childNode == null)
                    {
                        childNode = wbsDt.NewRow();
                        childNode["ID"] = FormulaHelper.CreateGuid();
                        childNode["WBSType"] = majorWBSType;
                        childNode["Name"] = majorAttr.Name;
                        childNode["WBSValue"] = majorAttr.Code;
                        childNode["Code"] = majorAttr.WBSCode;
                        childNode["SortIndex"] = majorAttr.SortIndex;
                        childNode["ChargeUserID"] = majorNode.GetValue("MajorPrinciple"); ;
                        childNode["ChargeUserName"] = majorNode.GetValue("MajorPrincipleName");
                        fo.AddWBSChildWithAdo(subProjectNode, childNode, wbsDt, structList);
                    }
                    else
                    {
                        childNode["SortIndex"] = majorAttr.SortIndex;
                        sqlCommand.AppendLine(String.Format("update S_W_WBS set ChargeUserID='{0}',ChargeUserName='{1}' where ID='{2}'",
                            majorNode.GetValue("MajorPrinciple"), majorNode.GetValue("MajorPrincipleName"), childNode["ID"]));
                    }
                    var roleList = roleDefineList.Where(a => majorNode.ContainsKey(a.RoleCode) && !string.IsNullOrEmpty(majorNode.GetValue(a.RoleCode)));
                    foreach (var roleDef in roleList)
                    {
                        var userIDs = majorNode.GetValue(roleDef.RoleCode).Split(',');
                        foreach (var userID in userIDs)
                        {
                            var userInfo = allUser.SingleOrDefault(c => c.UserID == userID);
                            if (userInfo == null) continue;
                            addRbsDt(userInfo, rbsDt, childNode, roleDef);
                        }
                    }
                }
            }
        }

        void addRbsDt(UserInfo userInfo, DataTable rbsDt, DataRow wbsRow, S_D_RoleDefine roleDefine)
        {
            if (userInfo == null) return;
            var rbsNewRow = rbsDt.NewRow();
            rbsNewRow["WBSID"] = wbsRow["ID"];
            rbsNewRow["WBSCode"] = wbsRow["Code"];
            rbsNewRow["WBSType"] = wbsRow["WBSType"];
            if (wbsRow["WBSType"].ToString() == WBSNodeType.Major.ToString())
            {
                rbsNewRow["MajorValue"] = wbsRow["WBSValue"];
            }
            rbsNewRow["ProjectInfoID"] = wbsRow["ProjectInfoID"];
            rbsNewRow["RoleCode"] = roleDefine.RoleCode;
            rbsNewRow["RoleName"] = roleDefine.RoleName;
            rbsNewRow["UserID"] = userInfo.UserID;
            rbsNewRow["UserName"] = userInfo.UserName;
            rbsNewRow["UserDeptID"] = userInfo.UserOrgID;
            rbsNewRow["UserDeptName"] = userInfo.UserOrgName;
            rbsDt.Rows.Add(rbsNewRow);
        }

        /// <summary>
        /// 流程结束反写业务逻辑
        /// </summary>
        public void Push(Dictionary<string, string> dic)
        {

            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            //获取项目信息
            var projectInfo = projectEntities.Set<S_I_ProjectInfo>().Find(this.ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("根据当前项目策划单，未找到对应的项目信息");
            var rootWBS = projectInfo.WBSRoot;
            if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            List<Dictionary<string, object>> majorUserList = null;

            if (this.PlanStartDate.HasValue)
                projectInfo.PlanStartDate = this.PlanStartDate;
            if (this.PlanFinishDate.HasValue)
                projectInfo.PlanFinishDate = this.PlanFinishDate;

            if (!String.IsNullOrEmpty(this.Code))
                projectInfo.Code = this.Code;

            //此处批量清空专业人员信息；
            //string[] roles = new string[] { ProjectRole.ProjectManager.ToString(),
            //     ProjectRole.MajorPrinciple.ToString(),ProjectRole.MajorEngineer.ToString(),
            //ProjectRole.Designer.ToString(),ProjectRole.Collactor.ToString(),
            //ProjectRole.Auditor.ToString(),ProjectRole.Approver.ToString()};
            //projectEntities.Set<S_W_RBS>().Delete(a => roles.Contains(a.RoleCode) && a.ProjectInfoID == projectInfo.ID);
            projectEntities.Set<S_W_RBS>().Delete(a => a.ProjectInfoID == projectInfo.ID);

            //将项目负责人,项目副经理,主管副总工,计划工程师,质量工程师,文档工程师写进RBS
            this.SynchRBSUser(dic, rootWBS);

            if (!string.IsNullOrEmpty(this.MajorList))
                majorUserList = JsonHelper.ToList(this.MajorList);
            projectInfo.SynchMajorData(this.Major);

            //从RBS同步人员到obs
            projectInfo.ResetOBSUserFromRBS();

            //根据子项信息和专业信息同步WBS节点
            this.SynchSubProjectWBSNode(dic, rootWBS);
            projectInfo.State = ProjectCommoneState.Execute.ToString();

        }

        /// <summary>
        /// 同步项目负责人,项目副经理,主管副总工,计划工程师,质量工程师,文档工程师写进RBS
        /// </summary>
        public void SynchRBSUser(Dictionary<string, string> dic, S_W_WBS rootWBS = null)
        {
            if (rootWBS == null)
            {
                var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
                //获取项目信息
                var projectInfo = projectEntities.Set<S_I_ProjectInfo>().Find(this.ProjectInfoID);
                if (projectInfo == null) throw new Formula.Exceptions.BusinessException("根据当前项目策划单，未找到对应的项目信息");
                rootWBS = projectInfo.WBSRoot;
                if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            }
            if (rootWBS.WBSType != WBSNodeType.Project.ToString()) throw new Formula.Exceptions.BusinessException("非根节点WBS，不可将项目主要干系人信息同步至RBS");
            if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            //将项目负责人,项目副经理,主管副总工,计划工程师,质量工程师,文档工程师写进RBS
            rootWBS.SetUsers(ProjectRole.ProjectManager.ToString(), this.ChargeUser.Split(','), true, true);//项目负责人
            //c_hua 2018/10/08 根据角色定义与表单字段动态写入RBS
            var roleList = BaseConfigFO.GetRoleDefineList().Where(a => dic.ContainsKey(a.RoleCode) && !string.IsNullOrEmpty(dic.GetValue(a.RoleCode)));
            foreach (var roleDef in roleList)
                rootWBS.SetUsers(roleDef.RoleCode, dic.GetValue(roleDef.RoleCode).Split(','), true, true);
        }

        /// <summary>
        /// 根据子项和专业列表同步WBS表
        /// </summary>
        public void SynchSubProjectWBSNode(Dictionary<string, string> dic, S_W_WBS rootWBS = null)
        {
            var userInfo = FormulaHelper.GetUserInfo();
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var projectInfo = projectEntities.Set<S_I_ProjectInfo>().Find(this.ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("根据当前项目策划单，未找到对应的项目信息");
            if (rootWBS == null)
            {
                //获取项目信息             
                rootWBS = projectInfo.WBSRoot;
                if (rootWBS == null) throw new Formula.Exceptions.BusinessException("未获取到当前项目的WBS根节点。");
            }

            var roleDefineList = BaseConfigFO.GetRoleDefineList();
            var enumSubProject = WBSNodeType.SubProject.ToString();
            var subwbses = rootWBS.Children.ToList();
            if (this.T_SC_SchemeForm_SubProjectList.ToList().Count == 0)   //如果没有策划任何子项，则默认建立一个子项，子项名称为项目阶段
            {
                foreach (var item in projectInfo.PhaseValue.Split(','))
                {

                }
                var subProjectListItem = new T_SC_SchemeForm_SubProjectList();
                subProjectListItem.ID = FormulaHelper.CreateGuid(); ;
                subProjectListItem.Name = projectInfo.PhaseName;
                subProjectListItem.Code = projectInfo.PhaseValue;
                subProjectListItem.Unit = "";
                subProjectListItem.PhaseValue = projectInfo.PhaseValue;
                this.T_SC_SchemeForm_SubProjectList.Add(subProjectListItem);
                this.SubProjectList = JsonHelper.ToJson(this.T_SC_SchemeForm_SubProjectList);
            }

            foreach (var subProject in this.T_SC_SchemeForm_SubProjectList.OrderBy(d => d.SortIndex).ToList())
            {
                var subWBSNode = subwbses.FirstOrDefault(d => d.WBSValue == subProject.Code && d.WBSType == WBSNodeType.SubProject.ToString());
                if (subWBSNode == null)
                {
                    subWBSNode = new S_W_WBS();
                    subWBSNode.ID = FormulaHelper.CreateGuid();
                    subWBSNode.WBSType = WBSNodeType.SubProject.ToString();
                    subWBSNode.Name = subProject.Name;
                    subWBSNode.Code = "";
                    subWBSNode.WBSValue = subProject.Code;
                    subWBSNode.PhaseCode = subProject.PhaseValue;
                    subWBSNode.ExtField1 = subProject.Area.HasValue ? subProject.Area.ToString() : "";
                    subWBSNode.ExtField2 = subProject.Unit;
                    rootWBS.AddChild(subWBSNode);
                }
                else
                {
                    subWBSNode.Name = subProject.Name;
                    subWBSNode.PhaseCode = subProject.PhaseValue;
                    subWBSNode.ExtField1 = subProject.Area.HasValue ? subProject.Area.ToString() : "";
                    subWBSNode.ExtField2 = subProject.Unit;
                    subWBSNode.Save();
                    subwbses.Remove(subWBSNode);
                }
                #region 新增单体
                var wonomerList = subWBSNode.S_W_Monomer.ToList();
                if (String.IsNullOrEmpty(subProject.Unit))
                {
                    var wonomer = wonomerList.FirstOrDefault(d => d.Name == subProject.Name);
                    if (wonomer == null)
                    {
                        wonomer = new S_W_Monomer();
                        wonomer.ID = FormulaHelper.CreateGuid();
                        wonomer.Name = subProject.Name;
                        wonomer.ProjectInfoID = this.ProjectInfoID;
                        wonomer.Code = "";
                        wonomer.CreateDate = DateTime.Now;
                        wonomer.CreateUser = userInfo.UserName;
                        wonomer.CreateUserID = userInfo.UserID;
                        wonomer.SchemeFormSubID = this.ID;
                        subWBSNode.S_W_Monomer.Add(wonomer);
                    }
                }
                else
                {

                    foreach (var item in subProject.Unit.Replace("，", ",").Split(','))
                    {
                        var wonomer = subWBSNode.S_W_Monomer.FirstOrDefault(d => d.Name == item);
                        if (wonomer == null)
                        {
                            wonomer = new S_W_Monomer();
                            wonomer.ID = FormulaHelper.CreateGuid();
                            wonomer.Name = item;
                            wonomer.ProjectInfoID = this.ProjectInfoID;
                            wonomer.Code = "";
                            wonomer.CreateDate = DateTime.Now;
                            wonomer.CreateUser = userInfo.UserName;
                            wonomer.CreateUserID = userInfo.UserID;
                            wonomer.SchemeFormSubID = this.ID;
                            subWBSNode.S_W_Monomer.Add(wonomer);
                        }
                        else
                        {
                            wonomerList.Remove(wonomer);
                        }

                    }
                }
                foreach (var delWonomer in wonomerList)   //移除不需要的单体
                {
                    projectEntities.S_W_Monomer.Remove(delWonomer);
                }

                #endregion

                SynchMajorWBSNode(roleDefineList, subWBSNode, subProject.RBSJson);
                //this.InitMileStone(subWBSNode);
            }
            subwbses.ForEach(t => t.Delete());  //删除多余的子项
        }

        /// <summary>
        /// 根据专业同步WBS
        /// </summary>
        /// <param name="parentNode"></param>
        public void SynchMajorWBSNode(List<S_D_RoleDefine> roleDefineList, S_W_WBS parentNode, string rbsList = "")
        {
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            if (parentNode == null) throw new Formula.Exceptions.BusinessException("传入的WBS节点为空。");
            var majorList = "";
            if (!string.IsNullOrEmpty(rbsList))
                majorList = rbsList;
            else
                majorList = this.MajorList;

            #region 删除不需要的专业
            var delMajors = parentNode.Children.Where(d => !this.Major.Contains(d.WBSValue)).ToList();
            foreach (var item in delMajors)
            {
                item.Delete();
                parentNode.Children.Remove(item);
            }
            #endregion

            if (!string.IsNullOrEmpty(majorList))
            {
                var majorNodes = JsonHelper.ToList(majorList);
                var majorAttrList = BaseConfigFO.GetWBSAttrList(WBSNodeType.Major);
                var majorWBSType = WBSNodeType.Major.ToString();
                foreach (var majorNode in majorNodes)
                {
                    if (majorNode.GetValue("Valid") == "0")
                    {
                        var thisMajorValue = majorNode.GetValue("MajorCode");
                        var thisMajorNode = parentNode.Children.SingleOrDefault(c => c.WBSValue == thisMajorValue);
                        if (thisMajorNode != null)
                            thisMajorNode.Delete();
                        continue;
                    }
                    var value = majorNode.GetValue("MajorCode");
                    var majorAttr = majorAttrList.SingleOrDefault(c => c.Code == value);
                    if (majorAttr == null) continue;
                    var childNode = parentNode.Children.SingleOrDefault(c => c.WBSValue == value);
                    if (!this.Major.Split(',').Contains(value))
                    {
                        if (childNode != null)
                            childNode.Delete();
                        continue;
                    }
                    if (childNode == null)
                    {
                        childNode = projectEntities.Set<S_W_WBS>().Create();
                        if (string.IsNullOrEmpty(childNode.ID))
                            childNode.ID = FormulaHelper.CreateGuid();
                        childNode._state = EntityState.Added.ToString();
                        childNode.WBSType = majorWBSType;
                        childNode.Name = majorAttr.Name;
                        childNode.WBSValue = majorAttr.Code;
                        childNode.Code = majorAttr.WBSCode;
                    }
                    childNode.SortIndex = majorAttr.SortIndex;
                    childNode.ChargeUserID = majorNode.GetValue("MajorPrinciple");
                    childNode.ChargeUserName = majorNode.GetValue("MajorPrincipleName");
                    if (childNode._state == EntityState.Added.ToString())
                        parentNode.AddChild(childNode);

                    #region 同步设校审人员信息
                    /*
                    //同步设置专业总工
                    var userIDs = majorNode.GetValue("MajorEngineer");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.MajorEngineer.ToString(), userIDs.Split(','), true, false, true, true);
                    //为WBS节点设定专业负责人
                    userIDs = majorNode.GetValue("MajorPrinciple");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.MajorPrinciple.ToString(), userIDs.Split(','), true, false, true, true);
                    //为WBS节点设定设计人
                    userIDs = majorNode.GetValue("Designer");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.Designer.ToString(), userIDs.Split(','), true, false, true, true);
                    //为WBS节点设定校核人
                    userIDs = majorNode.GetValue("Collactor");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.Collactor.ToString(), userIDs.Split(','), true, false, true, true);
                    //为WBS节点设定审核人
                    userIDs = majorNode.GetValue("Auditor");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.Auditor.ToString(), userIDs.Split(','), true, false, true, true);
                    //为WBS节点设定审定人
                    userIDs = majorNode.GetValue("Approver");
                    if (!String.IsNullOrEmpty(userIDs))
                        childNode.SetUsers(ProjectRole.Approver.ToString(), userIDs.Split(','), true, false, true, true);
                     */
                    var roleList = roleDefineList.Where(a => majorNode.ContainsKey(a.RoleCode) && !string.IsNullOrEmpty(majorNode.GetValue(a.RoleCode)));
                    foreach (var roleDef in roleList)
                        childNode.SetUsers(roleDef.RoleCode, majorNode.GetValue(roleDef.RoleCode).Split(','), true, false, true, true);
                    #endregion
                }
            }
        }

    }
}
