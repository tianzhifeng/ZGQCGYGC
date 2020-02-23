using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.ComponentModel.DataAnnotations;
using Formula;
using Newtonsoft.Json;
using MvcAdapter;
using Config;
using Formula.Helper;
using Formula.DynConditionObject;
using Config.Logic;

namespace Project.Logic.Domain
{
    public partial class S_I_ProjectInfo
    {
        #region 公共属性

        S_I_ProjectGroup _groupInfo;
        /// <summary>
        /// 分组信息
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_ProjectGroup GroupInfo
        {
            get
            {
                if (_groupInfo == null)
                {
                    _groupInfo = this.GetDbContext<ProjectEntities>().S_I_ProjectGroup.FirstOrDefault(d => d.ID == this.GroupID);
                }
                return _groupInfo;
            }
        }

        S_I_ProjectGroup _grouprootInfo;
        /// <summary>
        /// 分组根节点信息
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_ProjectGroup GroupRootInfo
        {
            get
            {
                if (_grouprootInfo == null)
                {
                    _grouprootInfo = this.GetDbContext<ProjectEntities>().S_I_ProjectGroup.FirstOrDefault(d => d.ID == this.GroupRootID);
                }
                return _grouprootInfo;
            }
        }

        List<S_I_ProjectGroup> _groupInfoList;
        /// <summary>
        /// 分组信息
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public List<S_I_ProjectGroup> GroupInfoList
        {
            get
            {
                if (_groupInfoList == null)
                {
                    _groupInfoList = this.GroupInfo.AllParents;
                }
                return _groupInfoList;
            }
        }

        S_T_ProjectMode _mode;
        /// <summary>
        /// 管理模式
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_ProjectMode ProjectMode
        {
            get
            {
                if (_mode == null)
                    _mode = BaseConfigFO.GetMode(this.ModeCode);
                return _mode;
            }
        }


        [NotMapped]
        [JsonIgnore]
        public string RootWBSID
        {
            get
            {
                if (this.WBSRoot != null)
                    return this.WBSRoot.ID;
                else
                    return String.Empty;
            }
        }

        S_W_WBS _rootNode;
        /// <summary>
        /// WBS根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_W_WBS WBSRoot
        {
            get
            {
                if (_rootNode == null)
                    _rootNode = this.S_W_WBS.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
                return _rootNode;
            }
        }

        S_D_DBS _rootDBS;
        /// <summary>
        /// DBS根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_D_DBS DBSRoot
        {
            get
            {
                if (_rootDBS == null)
                    _rootDBS = this.S_D_DBS.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
                return _rootDBS;
            }
        }

        S_C_CBS _rootCBS;
        /// <summary>
        /// WBS根节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_C_CBS CBSRoot
        {
            get
            {
                if (_rootCBS == null)
                    _rootCBS = this.S_C_CBS.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
                return _rootCBS;
            }
        }

        /// <summary>
        /// Major的Value
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public string MajorValue
        {
            get
            {
                return string.Join(",", JsonHelper.ToList(this.Major).Select(c => c["Value"].ToString()));
            }
        }

        /// <summary>
        /// Major的Text
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public string MajorText
        {
            get
            {
                return string.Join(",", JsonHelper.ToList(this.Major).Select(c => c["Name"].ToString()));
            }
        }

        #endregion

        #region 公共实例方法

        /// <summary>
        /// 创建项目实体（立项），同时创建WBS根结构，和DBS结构
        /// </summary>
        public void Build()
        {
            onBuilding();
            this.ModeCode = this.GetProjectModeCode();
            if (string.IsNullOrEmpty(this.ModeCode))
                throw new Formula.Exceptions.BusinessException("项目启动时必须要指定项目的管理模式编码");
            if (this.ProjectMode == null)
                throw new Formula.Exceptions.BusinessException("未能找到编码为【" + this.ModeCode + "】的管理模式对象，无法启动项目对象");
            if (string.IsNullOrEmpty(this.State))
                this.State = ProjectCommoneState.Plan.ToString();
            if (this.CreateDate == DateTime.MinValue) this.CreateDate = DateTime.Now;
            if (string.IsNullOrEmpty(this.CreateUserID))
            {
                var userInfo = FormulaHelper.GetUserInfo();
                this.CreateUserID = userInfo.UserID;
                this.CreateUser = userInfo.UserName;
            }
            if (this.GetDbContext<ProjectEntities>().Entry(this).State == EntityState.Detached)
                this.GetDbContext<ProjectEntities>().S_I_ProjectInfo.Add(this);
            this.AddRootWBSNode();
            this.InitDBS();
            this.InitCBS();
            onBuildComplete();
        }

        /// <summary>
        /// 改变项目模式
        /// </summary>
        public void ReBuild()
        {
            onBuilding();
            this.ModeCode = this.GetProjectModeCode();
            _mode = BaseConfigFO.GetMode(this.ModeCode);
            if (string.IsNullOrEmpty(this.ModeCode))
                throw new Formula.Exceptions.BusinessException("未找到管理模式编码");
            if (this.ProjectMode == null)
                throw new Formula.Exceptions.BusinessException("未能找到编码为【" + this.ModeCode + "】的管理模式对象，无法启动项目对象");
            var userInfo = FormulaHelper.GetUserInfo();
            this.ModifyUserID = userInfo.UserID;
            this.ModifyUser = userInfo.UserName;
            this.ModifyDate = DateTime.Now;
            this.GetDbContext<ProjectEntities>().Set<S_D_DBS>().Delete(a => a.ProjectInfoID == this.ID);
            this.InitDBS();
            this.GroupInfo.Name = this.Name;
            this.GroupInfo.Code = this.Code;
            this.GroupInfo.ChargeUser = this.ChargeUserID;
            this.GroupInfo.ChargeUserName = this.ChargeUserName;
            this.GroupInfo.DeptID = this.ChargeDeptID;
            this.GroupInfo.DeptName = this.ChargeDeptName;
            this.GroupInfo.Province = this.Province;
            this.GroupInfo.City = this.City;
            onBuildComplete();
        }

        public void BuildNotice()
        {
            string title = "项目策划完成通知";
            string content = "项目【" + this.Name + "】已经完成策划流程已经完成或升版";
            var rbsCodes = FormulaHelper.GetEntities<ProjectEntities>().Set<S_W_RBS>().Where(a => a.ProjectInfoID == this.ID).Select(a => a.RoleCode).Distinct().ToArray();
            var codes = string.Join(",", rbsCodes);
            FormulaHelper.CreateFO<ProjectInfoFO>().SendNotice(this.ID, "", "", codes, "", "", title, content, "", "", "", "", ProjectNoticeType.Project.ToString(), "", "");
        }

        /// <summary>
        /// 根据模式结构校验子项
        /// </summary>
        /// <param name="formDic"></param>
        /// <param name="hasSubProject"></param>
        public void ValidateMode(Dictionary<string, string> formDic, bool hasSubProject)
        {
            //根据dic获得模式
            var modeCode = this.GetProjectModeCode(formDic);
            if (string.IsNullOrEmpty(modeCode))
                throw new Formula.Exceptions.BusinessException("未找到管理模式编码");
            var mode = BaseConfigFO.GetMode(modeCode);
            var subProjectCode = WBSNodeType.SubProject.ToString();
            if (hasSubProject && !mode.S_T_WBSStructInfo.Any(a => a.ChildCode == subProjectCode))
                throw new Formula.Exceptions.BusinessException("管理模式【" + mode.Name + "】中没有子项节点");
        }

        /// <summary>
        /// 修改项目信息（如是新增项目对象，请使用Build方法）
        /// </summary>
        public void Save()
        {
            if (String.IsNullOrEmpty(this.ModeCode))
                throw new Formula.Exceptions.BusinessException("项目启动时必须要指定项目的管理模式编码");
            if (this.ProjectMode == null)
                throw new Formula.Exceptions.BusinessException("未能找到编码为【" + this.ModeCode + "】的管理模式对象，无法启动项目对象");
            if (this.GetDbContext<ProjectEntities>().Entry(this).State == EntityState.Detached
                || this.GetDbContext<ProjectEntities>().Entry(this).State == EntityState.Added)
                throw new Formula.Exceptions.BusinessException("新建的项目对象请使用Build方法进行立项操作，Save方法只用作项目信息变更时调用");
            this.DBSRoot.Name = this.Name;
            if (this.CBSRoot != null)
            {
                this.CBSRoot.Code = this.Code;
                this.CBSRoot.Name = this.Name;
            }
            this.WBSRoot.Name = this.Name;
            this.WBSRoot.ProjectCode = this.Code;
            this.WBSRoot.Code = this.Name;
            this.WBSRoot.ChargeUserID = this.ChargeUserID;
            this.WBSRoot.ChargeUserName = this.ChargeUserName;
            this.WBSRoot.Save();
        }

        public void SetOBSUser(UserInfo user, string roleCode, string majorValue = "", string majorName = "", bool resetFormRBS = false)
        {
            var entiites = this.GetDbContext<ProjectEntities>();
            var obsUser = this.S_W_OBSUser.FirstOrDefault(d => d.UserID == user.UserID && d.MajorValue == majorValue
            && d.RoleCode == roleCode);
            var configFO = FormulaHelper.CreateFO<BaseConfigFO>();
            var roleDefine = configFO.GetRoleDefine(roleCode);
            if (roleDefine == null) throw new Formula.Exceptions.BusinessException("没有找到指定的角色定义");
            if (obsUser == null)
            {
                obsUser = new S_W_OBSUser();
                obsUser.ID = FormulaHelper.CreateGuid();
                obsUser.RoleCode = roleDefine.RoleCode;
                obsUser.RoleName = roleDefine.RoleName;
                obsUser.MajorValue = majorValue;
                obsUser.MajorName = majorName;
                obsUser.UserID = user.UserID;
                obsUser.UserName = user.UserName;
                obsUser.DeptID = user.UserOrgID;
                obsUser.DeptName = user.UserOrgName;
                obsUser.IsCloud = "F";
                this.S_W_OBSUser.Add(obsUser);
            }
            if (resetFormRBS)
            {
                entiites.SaveChanges();
                var rbsUserList = new List<String>();
                var removeList = new List<S_W_OBSUser>();
                if (String.IsNullOrEmpty(majorValue))
                {
                    rbsUserList = entiites.S_W_RBS.Where(d => d.ProjectInfoID == this.ID && d.RoleCode == roleCode
                    && string.IsNullOrEmpty(d.MajorValue)).Select(d => d.UserID).ToList();
                    removeList = this.S_W_OBSUser.Where(d => d.RoleCode == roleCode && String.IsNullOrEmpty(majorValue)
                        && !rbsUserList.Contains(d.UserID)).ToList();
                }
                else
                {
                    rbsUserList = entiites.S_W_RBS.Where(d => d.ProjectInfoID == this.ID && d.RoleCode == roleCode
                    && d.MajorValue == majorValue).Select(d => d.UserID).ToList();
                    removeList = this.S_W_OBSUser.Where(d => d.RoleCode == roleCode && d.MajorValue == majorValue
                       && !rbsUserList.Contains(d.UserID)).ToList();
                }
                foreach (var item in removeList)
                {
                    entiites.Set<S_W_OBSUser>().Remove(item);
                }
            }
        }



        public void ResetOBSUserFromRBS()
        {
            var entiites = this.GetDbContext<ProjectEntities>();
            entiites.SaveChanges();
            entiites.S_W_OBSUser.Delete(d => d.ProjectInfoID == this.ID);
            var enumService = FormulaHelper.GetService<IEnumService>();
            var majorEnums = enumService.GetEnumDataSource("Project.Major").ToList();
            var obsList = entiites.S_W_RBS.Where(d => d.ProjectInfoID == this.ID).Select(d => new
            {
                UserID = d.UserID,
                UserName = d.UserID,
                RoleCode = d.RoleCode,
                RoleName = d.RoleName,
                MajorValue = d.MajorValue
            }).Distinct().ToList();
            var list = this.S_W_OBSUser.ToList();
            foreach (var item in obsList)
            {
                var userInfo = FormulaHelper.GetUserInfoByID(item.UserID);
                var majorName = string.Empty;
                if (majorEnums.Any(a => a.Value == item.MajorValue))
                    majorName = majorEnums.FirstOrDefault(a => a.Value == item.MajorValue).Text;
                this.SetOBSUser(userInfo, item.RoleCode, item.MajorValue, majorName);
            }
        }

        /// <summary>
        /// 发布WBS
        /// </summary>
        public void PublishWBS()
        {
            //var entities = this.GetDbContext<ProjectEntities>();
            //var userInfo = FormulaHelper.GetUserInfo();
            //var json = JsonHelper.ToJson(this.S_W_WBS.ToList());
            //var currentVersion = new S_W_WBSVersion();
            //var lastVersion = entities.S_W_WBSVersion.FirstOrDefault(d => d.ProjectInfoID == this.ID && d.CurrentVersion == "T");
            //currentVersion.ProjectInfoID = this.ID;
            //currentVersion.ID = FormulaHelper.CreateGuid();
            //currentVersion.WBSList = json;
            //currentVersion.CreateUser = userInfo.UserName;
            //currentVersion.CreateUserID = userInfo.UserID;
            //currentVersion.CreateDate = DateTime.Now;
            //currentVersion.CurrentVersion = "T";
            //if (lastVersion == null)
            //{
            //    currentVersion.Version = 1.0M;
            //    currentVersion.BaseVersion = "T";
            //}
            //else
            //{
            //    lastVersion.CurrentVersion = "F";
            //    currentVersion.Version = lastVersion.Version + 0.1M;
            //    currentVersion.BaseVersion = "F";
            //}

            //entities.S_W_WBSVersion.Add(currentVersion);

            //var log = new S_W_WBS_Log();
            //log.ID = FormulaHelper.CreateGuid();
            //log.ProjectInfoID = this.ID;
            //log.WBSType = WBSNodeType.Project.ToString();
            //log.WBSID = this.WBSRoot.ID;
            //log.OptionType = "publish";
            //log.OptionContent = "发布新版";
            //log.CreateDate = DateTime.Now;
            //log.CreateUserID = userInfo.UserID;
            //log.CreateUserName = userInfo.UserName;
            //entities.S_W_WBS_Log.Add(log);
            SetMajorSpace();
        }

        public void SetMajorSpace()
        {
            var majorList = this.GetMajorList().Select(c => new { Name = c.Name, Value = c.WBSValue }).Distinct().ToList();
            var majorAttrs = BaseConfigFO.GetWBSAttrList(WBSNodeType.Major);
            var majors = new List<Dictionary<string, object>>();
            foreach (var attr in majorAttrs)
            {
                if (majorList.Count(c => c.Value == attr.Code) == 0)
                    continue;
                var dic = new Dictionary<string, object>();
                dic.SetValue("Name", attr.Name);
                dic.SetValue("Value", attr.Code);
                dic.SetValue("SortIndex", attr.SortIndex);
                majors.Add(dic);
            }
            this.Major = JsonHelper.ToJson(majors);
        }

        /// <summary>
        /// 删除项目实体（附带删除所有的相关数据）
        /// </summary>
        /// <param name="destory">是否销毁所有相关结构数据</param>
        public void Delete(bool destory = false)
        {
            var entities = this.GetDbContext<ProjectEntities>();
            onDeleteing();
            if (destory)
            {
                if (this.WBSRoot != null)
                    this.WBSRoot.Delete(true);
                if (this.DBSRoot != null)
                    this.DBSRoot.Delete(false);
                if (this.CBSRoot != null)
                    this.CBSRoot.Delete(true);
                entities.S_P_MileStone.Delete(d => d.ProjectInfoID == this.ID);
                entities.S_P_CooperationPlan.Delete(d => d.ProjectInfoID == this.ID);
                entities.S_D_Input.Delete(d => d.ProjectInfoID == this.ID);
                entities.S_R_Risk.Delete(d => d.ProjectInfoID == this.ID);
                entities.S_W_Activity.Delete(d => d.ProjectInfoID == this.ID);
                entities.S_I_UserFocusProjectInfo.Delete(d => d.ProjectInfoID == this.ID);
                entities.S_I_UserDefaultProjectInfo.Delete(d => d.ProjectInfoID == this.ID);
                entities.S_AE_Mistake.Delete(d => d.ProjectInfoID == this.ID);
            }
            entities.S_I_ProjectInfo.Delete(d => d.ID == this.ID);
            onDeleteComplete();
        }

        /// <summary>
        /// 对指定的WBS节点增加子节点
        /// </summary>
        /// <param name="parent">指定的WBS父节点</param>
        /// <param name="wbsNode">WBS子节点</param>
        public void AddWBSNode(S_W_WBS parent, S_W_WBS wbsNode)
        {
            if (parent.ProjectInfoID != this.ID) throw new Formula.Exceptions.BusinessException("不属于该项目的节点，无法通过该项目实体对象创建WBS");
            parent.AddChild(wbsNode);
        }

        /// <summary>
        /// 获取指定人员在项目上的所有角色
        /// </summary>
        /// <param name="userID">人员ID</param>
        /// <returns>角色列表</returns>
        public List<S_W_OBSUser> GetUserRoles(string userID)
        {
            var entities = this.GetDbContext<ProjectEntities>();
            var result = entities.S_W_OBSUser.Where(d => d.UserID == userID && d.ProjectInfoID == this.ID).ToList();
            return result;
        }

        /// <summary>
        /// 根据用户获取该用户在本项目上的角色
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns>OBS用户对象集合</returns>
        public List<S_W_OBSUser> GetUserOBSRoles(string userID)
        {
            var entities = this.GetDbContext<ProjectEntities>();
            var rbs = entities.S_W_OBSUser.Where(d => d.UserID == userID && d.ProjectInfoID == this.ID).ToList();
            return rbs;
        }


        /// <summary>
        /// 获取指定人员在项目上的所有角色编码
        /// </summary>
        /// <param name="userID">人员ID</param>
        /// <returns>角色编码字符串，用逗号分隔</returns>
        public string GetUserRoleCodes(string userID)
        {
            var rolesCodes = "";
            var list = this.GetUserRoles(userID);
            foreach (var item in list)
                rolesCodes += item.RoleCode + ",";
            return rolesCodes.TrimEnd(',');
        }

        /// <summary>
        /// 获取项目内的所有专业（根据专业编码Distinct），以泛型Dictionary集合返回
        /// </summary>
        /// <returns>返回的专业名称为Name，专业编码为 Value，排序号为 SortIndex</returns>
        public List<Dictionary<string, object>> GetMajors(string majorValues = "", string exceptMajorValues = "")
        {
            var majors = this.GetWBSList(WBSNodeType.Major).
                Select(d => new { Name = d.Name, Value = d.WBSValue, SortIndex = d.SortIndex }).Distinct().OrderBy(d => d.SortIndex).ToList();
            var result = new List<Dictionary<string, object>>();
            string checkMajorValues = "";
            foreach (var item in majors)
            {
                if (!String.IsNullOrEmpty(majorValues))
                    if (!majorValues.Contains(item.Value)) continue;
                if (!String.IsNullOrEmpty(checkMajorValues) && checkMajorValues.Contains(item.Value))
                    continue;
                if (String.IsNullOrEmpty(exceptMajorValues))
                    if (exceptMajorValues.Contains(item.Value)) continue;
                checkMajorValues += "," + item.Value;
                var dic = new Dictionary<string, object>();
                dic.SetValue("Name", item.Name);
                dic.SetValue("Value", item.Value);
                dic.SetValue("SortIndex", item.SortIndex);
                result.Add(dic);
            }
            return result;
        }

        /// <summary>
        /// 获取项目内的所有阶段（根据阶段编码Distinct），以泛型Dictionary集合返回
        /// </summary>
        /// <returns>返回的阶段名称为Name，阶段编码为 Value，排序号为 SortIndex</returns>
        public List<Dictionary<string, object>> GetPhaseCodes()
        {
            var phases = this.GetWBSList(WBSNodeType.Phase).Select(d => new { Name = d.Name, Value = d.WBSValue, SortIndex = d.SortIndex }).Distinct().OrderBy(d => d.SortIndex).ToList(); ;
            var result = new List<Dictionary<string, object>>();
            foreach (var item in phases)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("Name", item.Name);
                dic.SetValue("Value", item.Value);
                dic.SetValue("SortIndex", item.SortIndex);
                result.Add(dic);
            }
            return result;
        }

        /// <summary>
        /// 获取专业WBS集合（以WBS存在，不做DISTINCT）
        /// </summary>
        /// <returns>专业WBS列表</returns>
        public List<S_W_WBS> GetMajorList()
        {
            return this.GetWBSList(WBSNodeType.Major);
        }

        /// <summary>
        /// 获取阶段WBS集合（以WBS存在，不做DISTINCT）
        /// </summary>
        /// <returns>阶段WBS列表</returns>
        public List<S_W_WBS> GetPhaseList()
        {
            return this.GetWBSList(WBSNodeType.Phase);
        }

        /// <summary>
        /// 获取指定WBS节点类型的WBS结合
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        /// <returns>WBS节点集合</returns>
        public List<S_W_WBS> GetWBSList(WBSNodeType nodeType)
        {
            return this.GetWBSList(nodeType.ToString());
        }

        /// <summary>
        /// 获取指定WBS节点类型的WBS结合
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        /// <returns>WBS节点集合</returns>
        public List<S_W_WBS> GetWBSList(string nodeType)
        {
            var entites = FormulaHelper.GetEntities<ProjectEntities>();
            return entites.S_W_WBS.Where(d => d.ProjectInfoID == this.ID && d.WBSType == nodeType).OrderBy(d => d.FullID).ToList();
        }

        /// <summary>
        /// 根据项目角色编码获取该项目上的入口集合
        /// </summary>
        /// <param name="roleList">用户OBS角色集合</param>
        /// <returns>入口集合</returns>
        public List<Entrance> GetEntraceByProjectRoles(List<S_W_OBSUser> roleList)
        {
            string projectRoleKeys = "";
            foreach (var item in roleList)
            {
                projectRoleKeys += item.RoleCode + ",";
            }
            projectRoleKeys = projectRoleKeys.TrimEnd(',');
            string sql = @"select S_T_SpaceDefine.*,S_T_SpaceAuth.AuthType from S_T_SpaceAuth 
left join S_T_SpaceDefine  on S_T_SpaceAuth.SpaceID=S_T_SpaceDefine.ID where RoleCode in ('{0}') and Type='Root' and ModeID='{1}' order by SortIndex";
            sql = string.Format(sql, projectRoleKeys.Replace(",", "','"), this.ProjectMode.ID);
            var dataTable = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql);
            var result = new List<Entrance>();
            foreach (DataRow row in dataTable.Rows)
            {
                var name = row["Name"].ToString();
                if (FormulaHelper.GetCurrentLGID() == "EN" && dataTable.Columns.Contains("NameEN"))
                {
                    var nameEN = row["NameEN"].ToString();
                    if (!string.IsNullOrEmpty(nameEN))
                        name = nameEN;
                }
                if (row["DefineType"].ToString() == SpaceDefineType.Static.ToString())
                {
                    var entrance = this.createEntrace(row["Code"].ToString(), name,
                        Convert.ToInt32(row["SortIndex"].ToString()), row);
                    this.FillEntranceResult(entrance, result);
                }
                else
                {
                    var propertyValue = FormulaHelper.GetProperty(this, row["DynamicDataFiled"].ToString());
                    if (propertyValue == null) continue;
                    var list = JsonHelper.ToList(propertyValue.ToString());
                    if (roleList.Exists(d => String.IsNullOrEmpty(d.MajorValue)))  //项目级的角色默认拥有所有专业的入口选项菜单
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            int sortIndex = Convert.ToInt32(row["SortIndex"].ToString());
                            var majorValue = item["Value"].ToString();
                            var majorName = item["Name"].ToString();
                            if (FormulaHelper.GetCurrentLGID() == "EN")
                            {
                                if (item != null && item.Keys.Contains("NameEN"))
                                {
                                    var majorNameEN = Convert.ToString(item["NameEN"]);
                                    if (!string.IsNullOrEmpty(majorNameEN))
                                        majorName = majorNameEN;
                                }
                            }
                            var entrance = this.createEntrace(item["Value"].ToString(), majorName, sortIndex + i, row);
                            this.FillEntranceResult(entrance, result, true);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < list.Count; i++)   //专业级的角色人员只能看到本专业的入口
                        {
                            var item = list[i];
                            int sortIndex = Convert.ToInt32(row["SortIndex"].ToString());
                            var majorValue = item["Value"].ToString();
                            if (roleList.Exists(d => d.MajorValue == majorValue))
                            {
                                var majorName = item["Name"].ToString();
                                if (FormulaHelper.GetCurrentLGID() == "EN")
                                {
                                    if (item != null && item.Keys.Contains("NameEN"))
                                    {
                                        var majorNameEN = item["NameEN"].ToString();
                                        if (!string.IsNullOrEmpty(majorNameEN))
                                            majorName = majorNameEN;
                                    }
                                }
                                var entrance = this.createEntrace(item["Value"].ToString(), majorName, sortIndex + i, row);
                                this.FillEntranceResult(entrance, result, true);
                            }
                        }
                    }
                }
            }
            return result.OrderBy(d => d.SortIndex).ToList();
        }

        /// <summary>
        /// 根据系统角色编码获取该项目上的入口集合
        /// </summary>
        /// <param name="sysRoleCodes">系统角色编码（多角色以逗号分隔）</param>
        /// <returns>入口集合</returns>
        public List<Entrance> GetEntraceBySysRoles(string sysRoleCodes)
        {
            string sql = @"select S_T_SpaceDefine.*,S_T_SpaceAuth.AuthType from S_T_SpaceAuth 
left join S_T_SpaceDefine  on S_T_SpaceAuth.SpaceID=S_T_SpaceDefine.ID where RoleCode in ('{0}') and Type='Root' and ModeID='{1}'";
            sql = string.Format(sql, sysRoleCodes.Replace(",", "','"), this.ProjectMode.ID);
            var dataTable = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig).ExecuteDataTable(sql);
            var result = new List<Entrance>();

            foreach (DataRow row in dataTable.Rows)
            {
                var name = row["Name"].ToString();
                if (FormulaHelper.GetCurrentLGID() == "EN")
                {
                    var nameEN = row["NameEN"].ToString();
                    if (!string.IsNullOrEmpty(nameEN))
                        name = nameEN;
                }
                if (row["DefineType"].ToString() == SpaceDefineType.Static.ToString())
                {
                    var entrance = this.createEntrace(row["Code"].ToString(), name,
                      Convert.ToInt32(row["SortIndex"].ToString()), row);
                    this.FillEntranceResult(entrance, result);
                }
                else
                {
                    var propertyValue = FormulaHelper.GetProperty(this, row["DynamicDataFiled"].ToString());
                    if (propertyValue == null) continue;
                    var list = JsonHelper.ToList(propertyValue.ToString());
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        int sortIndex = Convert.ToInt32(row["SortIndex"].ToString());
                        var propertyName = item["Name"].ToString();
                        if (FormulaHelper.GetCurrentLGID() == "EN")
                        {
                            if (item != null && item.Keys.Contains("NameEN"))
                            {
                                var nameEN = item["NameEN"];
                                var propertyNameEN = nameEN != null && !string.IsNullOrEmpty(nameEN.ToString()) ? nameEN.ToString() : propertyName;
                                if (!string.IsNullOrEmpty(propertyNameEN))
                                    propertyName = propertyNameEN;
                            }
                        }
                        var entrance = this.createEntrace(item["Value"].ToString(), propertyName, sortIndex + i, row);
                        this.FillEntranceResult(entrance, result, true);
                    }
                }
            }
            return result.OrderBy(d => d.SortIndex).ToList();
        }

        /// <summary>
        /// 根据定义入口对象获取指定项目角色和指定编码的所有导航菜单入口
        /// </summary>
        /// <param name="defineRootID">空间定义根节点ID</param>
        /// <param name="roleList">项目角色集合</param>
        /// <param name="relateCode">关联编码</param>
        /// <returns>导航菜单入口集合（空间定义集合）</returns>
        public List<S_T_SpaceDefine> GetSpaceDefineByProjectRoles(string defineRootID, List<S_W_OBSUser> roleList, string relateCode)
        {
            var result = new List<S_T_SpaceDefine>();
            var roleCodes = roleList.Select(a => a.RoleCode).ToList();
            var list = this.ProjectMode.S_T_SpaceDefine.Where(d => d.FullID.StartsWith(defineRootID)).ToList();
            foreach (var item in list)
            {
                foreach (var role in roleList)
                {
                    var spaceAuth = item.S_T_SpaceAuth.FirstOrDefault(d => d.RoleCode == role.RoleCode);
                    if (spaceAuth != null)
                    {
                        var siItem = result.FirstOrDefault(d => d.ID == item.ID);
                        if (siItem == null) { result.Add(item); siItem = item; }
                        if (spaceAuth.AuthType == SpaceAuthType.FullControl.ToString())
                        {
                            siItem.AuthType = SpaceAuthType.FullControl.ToString();
                            break;
                        }
                        else if (spaceAuth.AuthType == SpaceAuthType.CurrentFullControl.ToString())
                        {
                            if (relateCode == role.MajorValue)
                            {
                                siItem.AuthType = SpaceAuthType.FullControl.ToString();
                                break;
                            }
                            else
                                siItem.AuthType = SpaceAuthType.View.ToString();
                        }
                        else
                            siItem.AuthType = SpaceAuthType.View.ToString();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根据定义入口对象获取指定系统角色和指定编码的所有导航菜单入口
        /// </summary>
        /// <param name="defineRootID">空间定义根节点ID</param>
        /// <param name="sysRoleKeys">系统角色Code</param>
        /// <returns>导航菜单入口集合（空间定义集合）</returns>
        public List<S_T_SpaceDefine> GetSpaceDefineBySysRoles(string defineRootID, string sysRoleKeys)
        {
            var result = new List<S_T_SpaceDefine>();
            var list = this.ProjectMode.S_T_SpaceDefine.Where(d => d.FullID.StartsWith(defineRootID)).ToList();
            foreach (var item in list)
            {
                if (FormulaHelper.GetCurrentLGID() == "EN")
                {
                    if (!string.IsNullOrEmpty(item.NameEN))
                        item.Name = item.NameEN;
                }
                foreach (var role in sysRoleKeys.Split(','))
                {
                    var spaceAuth = item.S_T_SpaceAuth.FirstOrDefault(d => d.RoleCode == role);
                    if (spaceAuth != null)
                    {
                        var siItem = result.FirstOrDefault(d => d.ID == item.ID);
                        if (siItem == null) { result.Add(item); siItem = item; }
                        if (spaceAuth.AuthType == SpaceAuthType.FullControl.ToString())
                        {
                            siItem.AuthType = SpaceAuthType.FullControl.ToString();
                            break;
                        }
                        else
                            siItem.AuthType = SpaceAuthType.View.ToString();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 为项目增加里程碑
        /// </summary>
        /// <param name="mileStone">进度里程碑对象</param>
        public void AddMileStone(S_P_MileStone mileStone)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            if (entities.Entry(mileStone).State != EntityState.Added && entities.Entry(mileStone).State != EntityState.Detached)
                throw new Formula.Exceptions.BusinessException("非新增状态下的里程碑不能调用AddMileStone方法");
            if (String.IsNullOrEmpty(mileStone.ID))
                mileStone.ID = FormulaHelper.CreateGuid();
            if (String.IsNullOrEmpty(mileStone.WBSID))
                mileStone.WBSID = this.WBSRoot.ID;
            if (String.IsNullOrEmpty(mileStone.State))
                mileStone.State = ProjectCommoneState.Plan.ToString();
            if (mileStone.SortIndex == null)
                mileStone.SortIndex = this.S_P_MileStone.Count * 100;
            if (String.IsNullOrEmpty(mileStone.MileStoneType))
                mileStone.MileStoneType = MileStoneType.Normal.ToString();
            mileStone.ProjectInfoID = this.ID;
            mileStone.S_I_ProjectInfo = this;
            mileStone.Save();
        }

        /// <summary>
        /// 初始化资料输入模板
        /// </summary>
        public void InitDeisgnInputTemplate(bool isBuild = false)
        {
            var db = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            var phaseValues = this.PhaseValue.Split(',');
            var phaseWhere = "";
            foreach (var item in phaseValues)
            {
                phaseWhere += "  PhaseValues like '%" + item + "%' or";
            }
            string sql = "select * from S_D_DesignInputDefine where ProjectClass like '%{0}%' ";
            if (String.IsNullOrEmpty(phaseWhere))
            {
                sql += " and ({1})";
            }
            sql = String.Format(sql, this.ProjectClass, phaseWhere.TrimEnd(new char[] { 'r', 'o' }));
            var dt = db.ExecuteDataTable(sql);
            foreach (DataRow item in dt.Rows)
            {
                var name = item["Name"].ToString();
                S_D_Input input = null;
                if (!isBuild)
                    this.S_D_Input.FirstOrDefault(d => d.InfoName == name);
                if (input == null)
                {
                    input = new S_D_Input();
                    input.ID = FormulaHelper.CreateGuid();
                    input.InfoName = name;
                    this.S_D_Input.Add(input);
                }
                input.InputType = item["Class"].ToString();
                input.DocType = item["DocType"].ToString();
                input.DBSCode = item["DocType"].ToString();
                input.Catagory = "Project";
                input.SortIndex = item["SortIndex"] == null || String.IsNullOrEmpty(item["SortIndex"].ToString()) ? 0 : Convert.ToInt32(item["SortIndex"].ToString());
                input.InputTypeIndex = item["InputTypeIndex"] == null || String.IsNullOrEmpty(item["InputTypeIndex"].ToString()) ? 0 : Convert.ToInt32(item["InputTypeIndex"].ToString());
                input.State = CommonConst.unAuditState;
                input.EngineeringInfoID = this.GroupID;
            }
        }


        #endregion

        #region 内部方法

        internal void InitCBS()
        {
            //var configContext = this.GetDbContext<BaseConfigEntities>();
            var cbsInfo = new S_C_CBS();
            cbsInfo.ID = FormulaHelper.CreateGuid();
            cbsInfo.ParentID = string.Empty;
            cbsInfo.FullID = cbsInfo.ID;
            cbsInfo.CBSType = string.Empty;// CBSType.LabourExpense.ToString();
            cbsInfo.NodeType = CBSNodeType.Root.ToString();
            cbsInfo.Name = this.Name;
            cbsInfo.Code = this.Code;
            cbsInfo.CreateDate = DateTime.Now;
            cbsInfo.CreateUser = this.CreateUser;
            cbsInfo.CreateUserID = this.CreateUserID;
            cbsInfo.ProjectInfoID = this.ID;
            cbsInfo.S_I_ProjectInfo = this;
            this.S_C_CBS.Add(cbsInfo);
        }

        /// <summary>
        /// 创建DBS结构
        /// </summary>
        internal void InitDBS()
        {
            if (this.ProjectMode == null)
                throw new Formula.Exceptions.BusinessException("未能找到编号为【" + this.ModeCode + "】的管理模式模板，无法创建DBS目录");
            var user = FormulaHelper.GetUserInfo();
            var dbsRootDefine = this.ProjectMode.S_T_DBSDefine.FirstOrDefault(d => d.DBSType == DBSType.Root.ToString());
            if (dbsRootDefine == null) return;
            var dbs = new S_D_DBS();
            dbs.ID = FormulaHelper.CreateGuid();
            dbs.Name = this.Name;
            dbs.DBSType = DBSType.Root.ToString();
            dbs.DBSCode = dbsRootDefine.DBSCode;
            dbs.CreateDate = DateTime.Now;
            dbs.CreateUser = user.UserName;
            dbs.CreateUserID = user.UserID;
            dbs.FullID = dbs.ID;
            dbs.InheritAuth = true.ToString();
            dbs.S_I_ProjectInfo = this;
            dbs.ConfigDBSID = dbsRootDefine.ID;
            this.S_D_DBS.Add(dbs);
            foreach (var security in dbsRootDefine.S_T_DBSSecurity.ToList())
            {
                var securityInstance = new S_D_DBSSecurity();
                securityInstance.ID = FormulaHelper.CreateGuid();
                securityInstance.AuthType = security.AuthType;
                securityInstance.RoleCode = security.RoleCode;
                securityInstance.RoleName = security.RoleName;
                securityInstance.RoleType = security.RoleType;
                dbs.S_D_DBSSecurity.Add(securityInstance);
            }

            CreateDBS(dbsRootDefine, dbs, this.ProjectMode.S_T_DBSDefine.ToList());
        }

        /// <summary>
        /// 根据DBSConfig同步DBS
        /// </summary>
        public void SyncDBS()
        {
            if (this.ProjectMode == null)
                throw new Formula.Exceptions.BusinessException("未能找到编号为【" + this.ModeCode + "】的管理模式模板，无法同步DBS目录");
            var user = FormulaHelper.GetUserInfo();
            var dbsList = this.S_D_DBS.ToList();
            //对于项目添加的自定义目录不进行删除，其他都根据DBSconfig来更新
            var dbsRootDefine = this.ProjectMode.S_T_DBSDefine.FirstOrDefault(d => d.DBSType == DBSType.Root.ToString());
            var dbsRoot = dbsList.FirstOrDefault(a => a.ConfigDBSID == dbsRootDefine.ID);
            if (dbsRoot == null)
            {
                dbsRoot = new S_D_DBS();
                dbsRoot.ID = FormulaHelper.CreateGuid();
                dbsRoot.CreateDate = DateTime.Now;
                dbsRoot.CreateUser = user.UserName;
                dbsRoot.CreateUserID = user.UserID;
                dbsRoot.FullID = dbsRoot.ID;
                dbsRoot.S_I_ProjectInfo = this;
                dbsRoot.ProjectInfoID = this.ID;
                dbsRoot.ConfigDBSID = dbsRootDefine.ID;
                dbsList.Add(dbsRoot);
                this.S_D_DBS.Add(dbsRoot);
            }
            dbsRoot.Name = this.Name;
            dbsRoot.DBSType = DBSType.Root.ToString();
            dbsRoot.DBSCode = dbsRootDefine.DBSCode;
            dbsRoot.InheritAuth = true.ToString();

            foreach (var item in dbsRoot.S_D_DBSSecurity.ToList())
                this.GetDbContext<ProjectEntities>().S_D_DBSSecurity.Remove(item);
            foreach (var security in dbsRootDefine.S_T_DBSSecurity.ToList())
            {
                var securityInstance = new S_D_DBSSecurity();
                securityInstance.ID = FormulaHelper.CreateGuid();
                securityInstance.AuthType = security.AuthType;
                securityInstance.RoleCode = security.RoleCode;
                securityInstance.RoleName = security.RoleName;
                securityInstance.RoleType = security.RoleType;
                dbsRoot.S_D_DBSSecurity.Add(securityInstance);
            }
            this._SyncDBS(dbsRootDefine, dbsRoot, this.ProjectMode.S_T_DBSDefine.ToList(), dbsList, user);
        }

        private void _SyncDBS(S_T_DBSDefine parentDefine, S_D_DBS parentDBS, List<S_T_DBSDefine> defines, List<S_D_DBS> dbsList, UserInfo user)
        {
            if (parentDefine.MappingType == DBSMappingType.ISO.ToString() || parentDBS.MappingType == DBSMappingType.Product.ToString())
            {
                //更新所有根据归档子表生成的目录的归档文件夹
                var configStructList = JsonHelper.ToList(parentDefine.ProductStruct);
                foreach (var configStruct in configStructList)
                {
                    var autoFolderType = configStruct.GetValue("FieldName");
                    var autoFolderList = dbsList.Where(a => a.ConfigDBSID == parentDefine.ID + "." + autoFolderType).ToList();
                    foreach (var autoFolder in autoFolderList)
                    {
                        autoFolder.ArchiveFolder = configStruct.GetValue("ArchiveFolder");
                        autoFolder.ArchiveFolderName = configStruct.GetValue("ArchiveFolderName");
                    }
                }
            }
            var children = defines.Where(d => d.ParentID == parentDefine.ID).ToList();
            foreach (var childDefine in children)
            {
                var dbsChild = dbsList.FirstOrDefault(a => a.ConfigDBSID == childDefine.ID);
                if (dbsChild == null)
                {
                    dbsChild = new S_D_DBS();
                    dbsChild.ID = FormulaHelper.CreateGuid();
                    dbsChild.CreateDate = DateTime.Now;
                    dbsChild.CreateUser = user.UserName;
                    dbsChild.CreateUserID = user.UserID;
                    dbsChild.FullID = parentDBS.FullID + "." + dbsChild.ID;
                    dbsChild.S_I_ProjectInfo = this;
                    dbsChild.ProjectInfoID = parentDBS.ProjectInfoID;
                    dbsChild.ParentID = parentDBS.ID;
                    dbsList.Add(dbsChild);
                    this.S_D_DBS.Add(dbsChild);
                }

                dbsChild.DBSCode = childDefine.DBSCode;
                dbsChild.Name = childDefine.Name;
                dbsChild.DBSType = childDefine.DBSType;
                dbsChild.MappingNodeUrl = childDefine.MappingNodeUrl;
                dbsChild.MappingType = childDefine.MappingType;
                dbsChild.ConfigDBSID = childDefine.ID;
                dbsChild.ArchiveFolder = childDefine.ArchiveFolder;
                dbsChild.ArchiveFolderName = childDefine.ArchiveFolderName;
                dbsChild.InheritAuth = childDefine.InheritAuth;

                foreach (var item in dbsChild.S_D_DBSSecurity.ToList())
                    this.GetDbContext<ProjectEntities>().S_D_DBSSecurity.Remove(item);
                foreach (var security in childDefine.S_T_DBSSecurity.ToList())
                {
                    var securityInstance = new S_D_DBSSecurity();
                    securityInstance.ID = FormulaHelper.CreateGuid();
                    securityInstance.AuthType = security.AuthType;
                    securityInstance.RoleCode = security.RoleCode;
                    securityInstance.RoleName = security.RoleName;
                    securityInstance.RoleType = security.RoleType;
                    dbsChild.S_D_DBSSecurity.Add(securityInstance);
                }
                //映射目录 同步归档目录
                if (childDefine.DBSType == DBSType.Mapping.ToString())
                {
                    if (!string.IsNullOrEmpty(childDefine.ProductStruct))
                    {
                        var structList = JsonHelper.ToList(childDefine.ProductStruct);
                        foreach (var item in structList)
                        {
                            var _configDbsID = childDefine.ID + "." + item.GetValue("FieldName");
                            dbsList.Where(a => a.ConfigDBSID == _configDbsID).ToList().ForEach(a =>
                            {
                                a.ArchiveFolder = item.GetValue("ArchiveFolder");
                                a.ArchiveFolderName = item.GetValue("ArchiveFolderName");
                            });
                        }
                    }
                }
                this._SyncDBS(childDefine, dbsChild, defines, dbsList, user);
            }
        }

        /// <summary>
        /// 新增WBS根节点
        /// </summary>
        internal void AddRootWBSNode()
        {
            if (this.ProjectMode == null)
                throw new Formula.Exceptions.BusinessException("未能找到编号为【" + this.ModeCode + "】的项目管理模式对象，无法为项目增加WBS节点");
            if (this.WBSRoot != null)
                throw new Formula.Exceptions.BusinessException("项目已经存在WBS根节点，无法重复添加根节点");
            var wbsNode = new S_W_WBS();
            if (wbsNode == null)
                throw new Formula.Exceptions.BusinessException("空的WBS对象，无法添加");
            if (String.IsNullOrEmpty(wbsNode.ID))
                wbsNode.ID = this.ID;
            wbsNode.ProjectInfoID = this.ID;
            wbsNode.Name = this.Name;
            wbsNode.Code = this.Code;
            wbsNode.WBSValue = this.Code;
            wbsNode.ChargeUserID = this.ChargeUserID;
            wbsNode.ChargeUserName = this.ChargeUserName;
            wbsNode.WBSDeptID = this.ChargeDeptID;
            wbsNode.WBSDeptName = this.ChargeDeptName;
            wbsNode.WBSType = WBSNodeType.Project.ToString();
            wbsNode.State = ProjectCommoneState.Plan.ToString();
            var stNode = this.ProjectMode.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == wbsNode.WBSType);
            if (stNode == null) throw new Formula.Exceptions.BusinessException("管理模式中未能找到指定的WBS结构定义，无法新增节点");
            wbsNode.WBSStructCode = stNode.Code;
            wbsNode.FullID = wbsNode.ID;
            wbsNode.Level = wbsNode.FullID.Split('.').Length;
            wbsNode.State = ProjectCommoneState.Plan.ToString();
            wbsNode.PlanStartDate = this.PlanStartDate;
            wbsNode.PlanEndDate = this.PlanFinishDate;
            this.S_W_WBS.Add(wbsNode);
            wbsNode.S_I_ProjectInfo = this;
            wbsNode.SortIndex = wbsNode.Level * 1000;
            wbsNode.PhaseCode = this.PhaseValue;
            wbsNode.SetDefaultValue();
            wbsNode.SetWBSCodes();
        }

        internal string GetProjectModeCode(Dictionary<string, string> formDic = null)
        {
            var baseEntities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var allModes = baseEntities.Set<S_T_ProjectMode>().OrderBy(d => d.Priority).ToList();
            var modeCode = string.Empty;

            foreach (var item in allModes)
            {
                if (String.IsNullOrEmpty(item.Condition)) continue;
                var conditions = JsonHelper.ToList(item.Condition);
                bool result = false;
                var resultList = new List<ProjectCondition>();
                foreach (var condition in conditions)
                {
                    var conditionResult = false;
                    #region 校验条件定义
                    var propertyValue = string.Empty;
                    if (formDic == null)
                        propertyValue = this.GetPropertyString(condition.GetValue("FieldName"));
                    else
                        propertyValue = formDic.GetValue(condition.GetValue("FieldName"));
                    if (propertyValue == null)
                        continue;
                    var condiftionValue = condition.GetValue("Value");
                    switch (condition.GetValue("QueryMode"))
                    {
                        default:
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
                    condition.SetValue("Result", conditionResult);
                    resultList.Add(new ProjectCondition
                    {
                        FieldName = condition.GetValue("FieldName"),
                        GroupName = condition.GetValue("Group"),
                        Result = conditionResult
                    });
                }
                var groupInfoList = resultList.Select(d => d.GroupName).Distinct().ToList();
                foreach (var groupInfo in groupInfoList)
                {
                    if (resultList.Where(d => d.GroupName == groupInfo).Count(d => !d.Result) == 0)
                    {
                        result = true; break;
                    }
                }
                if (result)
                {
                    modeCode = item.ModeCode;
                    break;
                }
            }
            if (String.IsNullOrEmpty(modeCode))
            {
                var defaultMode = allModes.FirstOrDefault(d => d.IsDefault == "True");
                if (defaultMode != null)
                    modeCode = defaultMode.ModeCode;
            }
            return modeCode;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entrance"></param>
        /// <param name="result"></param>
        void FillEntranceResult(Entrance entrance, List<Entrance> result, bool Dynamic = false)
        {
            if (!Dynamic)
            {
                if (entrance.Auth == SpaceAuthType.FullControl.ToString())
                {
                    result.RemoveWhere(d => d.SpaceDefineID == entrance.SpaceDefineID);
                    result.Add(entrance);
                }
                else if (entrance.Auth == SpaceAuthType.CurrentFullControl.ToString())
                {
                    if (result.Exists(d => d.SpaceDefineID == entrance.SpaceDefineID && (d.Auth == SpaceAuthType.FullControl.ToString()
                        || d.Auth == SpaceAuthType.CurrentFullControl.ToString())))
                        return;
                    result.RemoveWhere(d => d.SpaceDefineID == entrance.SpaceDefineID);
                    result.Add(entrance);
                }
                else
                {
                    if (result.Exists(d => d.SpaceDefineID == entrance.SpaceDefineID))
                        return;
                    result.Add(entrance);
                }
            }
            else
            {
                if (entrance.Auth == SpaceAuthType.FullControl.ToString())
                {
                    result.RemoveWhere(d => d.Code == entrance.Code);
                    result.Add(entrance);
                }
                else if (entrance.Auth == SpaceAuthType.CurrentFullControl.ToString())
                {
                    if (result.Exists(d => d.Code == entrance.Code && (d.Auth == SpaceAuthType.FullControl.ToString()
                        || d.Auth == SpaceAuthType.CurrentFullControl.ToString())))
                        return;
                    result.RemoveWhere(d => d.Code == entrance.Code);
                    result.Add(entrance);
                }
                else
                {
                    if (result.Exists(d => d.Code == entrance.Code))
                        return;
                    result.Add(entrance);
                }
            }
        }

        /// <summary>
        /// 创建入口Entrace对象
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="name">入口名称</param>
        /// <param name="sortIndex">排序号</param>
        /// <param name="row">DataRow对象，描述入口定义信息</param>
        /// <returns>Entrace对象</returns>
        Entrance createEntrace(string code, string name, int sortIndex, DataRow row)
        {
            var entrance = new Entrance();
            entrance.SpaceDefineID = row["ID"].ToString();
            entrance.Code = code;
            entrance.Name = name;
            if (!String.IsNullOrEmpty(row["LinkUrl"].ToString()))
                entrance.LinkUrl = row["LinkUrl"].ToString() + "?ProjectInfoID=" + this.ID + "&Code=" + row["Code"].ToString() + "&AuthType=" + row["AuthType"].ToString();
            else
                entrance.LinkUrl = "";
            entrance.SortIndex = sortIndex;
            entrance.SpaceDefineCode = row["Code"].ToString();
            entrance.IsDefault = false;
            entrance.Auth = row["AuthType"].ToString();
            return entrance;
        }

        /// <summary>
        /// 根据定义创建DBS节点
        /// </summary>
        /// <param name="parentDefine">父目录定义</param>
        /// <param name="parentDBS">父目录</param>
        /// <param name="defines">目录定义集合</param>
        void CreateDBS(S_T_DBSDefine parentDefine, S_D_DBS parentDBS, List<S_T_DBSDefine> defines)
        {
            var children = defines.Where(d => d.ParentID == parentDefine.ID).ToList();
            foreach (var childDefine in children)
            {
                var childDBS = parentDBS.AddChild(childDefine);
                this.CreateDBS(childDefine, childDBS, defines);
            }
        }


        #endregion

        #region 分布扩展方法

        partial void onBuilding();

        partial void onBuildComplete();

        partial void onDeleteing();

        partial void onDeleteComplete();

        #endregion

    }

    public struct ProjectCondition
    {
        public string GroupName { get; set; }
        public bool Result { get; set; }
        public string FieldName { get; set; }
    }
}
