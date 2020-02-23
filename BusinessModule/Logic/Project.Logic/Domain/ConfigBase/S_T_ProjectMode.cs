using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Formula;
using Formula.Helper;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Project.Logic.Domain
{
    public partial class S_T_ProjectMode
    {
        /// <summary>
        /// 结构项目节点
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_T_WBSStructInfo RootStructInfoNode
        {
            get
            {
                return this.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == WBSNodeType.Project.ToString());
            }
        }
        
        /// <summary>
        /// 扩展属性
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public Dictionary<string, object> ExtentionObject
        {
            get
            {
                if (string.IsNullOrEmpty(this.ExtentionJson))
                    return new Dictionary<string, object>();
                else
                    return JsonHelper.ToObject(this.ExtentionJson);
            }
        }

        /// <summary>
        ///  删除管理模式
        /// </summary>
        public void Delete()
        {
            var context = this.GetDbContext<BaseConfigEntities>();
            //WBS结构配置
            foreach (var stInfo in this.S_T_WBSStructInfo.ToList())
                stInfo.Delete();
            //DBS结构配置
            var dbsRoot = this.S_T_DBSDefine.FirstOrDefault(d => d.DBSType == DBSType.Root.ToString());
            if (dbsRoot != null)
                dbsRoot.Delete();
            //空间配置
            var spaceDefine = this.S_T_SpaceDefine.Where(c => c.Type == "Root").ToList();
            if (spaceDefine != null)
            {
                foreach (var space in spaceDefine)
                {
                    space.Delete();
                }
            }
            //里程碑定义
            context.S_T_MileStone.Delete(d => d.ModeID == this.ID);

            context.S_T_ProjectMode.Delete(d => d.ID == this.ID);
        }

        /// <summary>
        /// 发布管理模式（即清空缓存，重新从数据库中读取新的管理模式数据）
        /// </summary>
        public void Pubish()
        {
            Formula.Helper.CacheHelper.Remove(CommonConst.cacheKey + this.ModeCode);
        }

        /// <summary>
        /// 增加DBS模板根节点
        /// </summary>
        public void AddRootDBSDefine()
        {
            if (this.S_T_DBSDefine.FirstOrDefault(d => d.DBSType == DBSType.Root.ToString()) != null)
                return;
            var dbsDefine = new S_T_DBSDefine();
            dbsDefine.ID = FormulaHelper.CreateGuid();
            dbsDefine.DBSType = DBSType.Root.ToString();
            dbsDefine.DBSCode = DBSType.Root.ToString();
            dbsDefine.FullID = dbsDefine.ID;
            dbsDefine.Level = dbsDefine.FullID.Split('.').Length;
            dbsDefine.Name = "DBS根目录";
            dbsDefine.InheritAuth = true.ToString();
            this.S_T_DBSDefine.Add(dbsDefine);
        }

        /// <summary>
        /// 增加WBS结构对象
        /// </summary>
        /// <param name="nodeType">WBS节点类别</param>
        public void AddWBSSturct(WBSNodeType nodeType)
        {
            string wbsNodeType = nodeType.ToString();
            if (this.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == wbsNodeType) != null)
                return;
            var structInfo = new S_T_WBSStructInfo();
            structInfo.ID = FormulaHelper.CreateGuid();
            structInfo.Code = wbsNodeType;
            structInfo.Name = EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), wbsNodeType);
            this.S_T_WBSStructInfo.Add(structInfo);
        }

        /// <summary>
        /// 增加WBS结构对象
        /// </summary>
        /// <param name="nodeType">WBS节点类别</param>
        public void AddWBSSturct(string nodeType)
        {
            if (this.S_T_WBSStructInfo.FirstOrDefault(d => d.Code == nodeType) != null)
                return;
            var structInfo = new S_T_WBSStructInfo();
            structInfo.ID = FormulaHelper.CreateGuid();
            structInfo.Code = nodeType;
            structInfo.Name = EnumBaseHelper.GetEnumDescription(typeof(WBSNodeType), nodeType);
            this.S_T_WBSStructInfo.Add(structInfo);
        }

        /// <summary>
        /// 增加Space空间定义根
        /// </summary>
        /// <param name="define">根节点定义对象</param>
        public void AddRootSpaceDefine(S_T_SpaceDefine define)
        {
            Project.Logic.Domain.S_T_SpaceDefine.AddRoot(define);
        }

        /// <summary>
        /// 获取所有里程碑定义
        /// </summary>
        /// <returns></returns>
        public List<S_T_MileStone> GetProjectMileStone()
        {
            return this.S_T_MileStone.ToList();
        }

        /// <summary>
        /// 复制项目模式
        /// </summary>
        public void DoCopy()
        {
            var context = this.GetDbContext<BaseConfigEntities>();

            var newModel = this.Clone<S_T_ProjectMode>();
            newModel.ID = FormulaHelper.CreateGuid();
            newModel.ModeCode = newModel.ModeCode + "_Copy";
            newModel.Name = newModel.Name + "_Copy";
            newModel.Description = "";
            context.Set<S_T_ProjectMode>().Add(newModel);

            //空间配置
            var rootSpaceList = this.S_T_SpaceDefine.Where(c => c.Type == "Root").ToList();
            if (rootSpaceList != null)
            {
                foreach (var rootSpace in rootSpaceList)
                {
                    var newRootSpace = rootSpace.Clone<S_T_SpaceDefine>();
                    newRootSpace.ID = FormulaHelper.CreateGuid();
                    newRootSpace.FullID = newRootSpace.ID;
                    newRootSpace.ModeID = newModel.ID;

                    context.Set<S_T_SpaceDefine>().Add(newRootSpace);

                    CopySpaceDefine(rootSpace, newRootSpace, context);
                }
            }

            //WBS结构配置
            CopyWBSStructInfo(this.ID, newModel.ID, context);

            //DBS结构配置
            var rootDBSDefineList = this.S_T_DBSDefine.Where(c => c.DBSType == "Root").ToList();
            if (rootDBSDefineList != null)
            {
                foreach (var rootDBSDefine in rootDBSDefineList)
                {
                    var newRootDSBDefine = rootDBSDefine.Clone<S_T_DBSDefine>();
                    newRootDSBDefine.ID = FormulaHelper.CreateGuid();
                    newRootDSBDefine.ModeID = newModel.ID;
                    newRootDSBDefine.FullID = newRootDSBDefine.ID;

                    context.Set<S_T_DBSDefine>().Add(newRootDSBDefine);

                    CopyDBSDefine(rootDBSDefine, newRootDSBDefine, context);
                }
            }

            //里程碑配置
            CopyMilestone(this.ID, newModel.ID, context);

            //质量控制模板
            var rootQBSTemplateList = this.S_T_QBSTemplate.Where(c => c.ParentID == "").ToList();
            if (rootQBSTemplateList != null)
            {
                foreach (var rootQBSTemplate in rootQBSTemplateList)
                {
                    var newRootQBSTemplate = rootQBSTemplate.Clone<S_T_QBSTemplate>();
                    newRootQBSTemplate.ID = FormulaHelper.CreateGuid();
                    newRootQBSTemplate.ModeID = newModel.ID;
                    newRootQBSTemplate.FullID = newRootQBSTemplate.ID;

                    context.Set<S_T_QBSTemplate>().Add(newRootQBSTemplate);

                    CopyQBSTemplate(rootQBSTemplate, newRootQBSTemplate, context);
                }
            }

            //ISO表单定义
            CopyISODefine(this.ID, newModel.ID, context);

            //流程跟踪定义
            var rootFlowTraceDefineList = this.S_T_FlowTraceDefine.Where(c => c.ParentID == "").ToList();
            if (rootFlowTraceDefineList != null)
            {
                foreach (var rootFlowTraceDefine in rootFlowTraceDefineList)
                {
                    var newFlowTraceDefine = rootFlowTraceDefine.Clone<S_T_FlowTraceDefine>();
                    newFlowTraceDefine.ID = FormulaHelper.CreateGuid();
                    newFlowTraceDefine.ModeID = newModel.ID;
                    newFlowTraceDefine.FullID = newFlowTraceDefine.ID;

                    context.Set<S_T_FlowTraceDefine>().Add(newFlowTraceDefine);

                    CopyFlowTraceDefine(rootFlowTraceDefine, newFlowTraceDefine, context);
                }
            }

            //代办事项定义
            var todoListDefines = this.S_T_ToDoListDefine.ToList();
            foreach (var ld in todoListDefines)
            {
                var newLD = ld.Clone<S_T_ToDoListDefine>();
                newLD.ID = FormulaHelper.CreateGuid();
                newLD.ModeID = newModel.ID;
                context.S_T_ToDoListDefine.Add(newLD);
            }
            
        }
        /// <summary>
        /// 复制空间配置及控件授权
        /// </summary>
        /// <param name="rootSpace"></param>
        /// <param name="newRootSpace"></param>
        /// <param name="context"></param>
        private void CopySpaceDefine(S_T_SpaceDefine rootSpace, S_T_SpaceDefine newRootSpace, BaseConfigEntities context)
        {
            var childSpace = rootSpace.Children;
            foreach (var space in childSpace)
            {
                var newSpace = space.Clone<S_T_SpaceDefine>();
                newSpace.ID = FormulaHelper.CreateGuid();
                newSpace.ParentID = newRootSpace.ID;
                newSpace.FullID = newRootSpace.FullID + "." + newSpace.ID;
                newSpace.ModeID = newRootSpace.ModeID;
                context.Set<S_T_SpaceDefine>().Add(newSpace);

                //空间权限
                var spaceAuth = context.Set<S_T_SpaceAuth>().Where(c => c.SpaceID == space.ID);
                if (spaceAuth != null)
                {
                    foreach (var auth in spaceAuth)
                    {
                        var newAuth = auth.Clone<S_T_SpaceAuth>();
                        newAuth.SpaceID = newSpace.ID;
                        context.Set<S_T_SpaceAuth>().Add(newAuth);
                    }
                }

                if (space.Children != null)
                {
                    CopySpaceDefine(space, newSpace, context);
                }

            }
        }
        /// <summary>
        /// 复制WBS结构配置
        /// </summary>
        /// <param name="modelID"></param>
        /// <param name="newModelID"></param>
        /// <param name="context"></param>
        private void CopyWBSStructInfo(string modelID, string newModelID, BaseConfigEntities context)
        {
            var structList = context.Set<S_T_WBSStructInfo>().Where(c => c.ModeID == modelID).ToList();
            foreach (var structInfo in structList)
            {
                var newSturctInfo = structInfo.Clone<S_T_WBSStructInfo>();
                newSturctInfo.ID = FormulaHelper.CreateGuid();
                newSturctInfo.ModeID = newModelID;
                context.S_T_WBSStructInfo.Add(newSturctInfo);
                if (structInfo.S_T_WBSStructRole != null)
                {
                    foreach (var wbsStructRole in structInfo.S_T_WBSStructRole)
                    {
                        var newWBSStructRole = wbsStructRole.Clone<S_T_WBSStructRole>();
                        newWBSStructRole.ID = FormulaHelper.CreateGuid();
                        newWBSStructRole.WBSStructID = newSturctInfo.ID;
                        context.S_T_WBSStructRole.Add(newWBSStructRole);
                    }
                }
            }
        }
        /// <summary>
        /// 复制DBS结构配置及权限
        /// </summary>
        /// <param name="rootDBS"></param>
        /// <param name="newRootDBS"></param>
        /// <param name="context"></param>
        private void CopyDBSDefine(S_T_DBSDefine rootDBS, S_T_DBSDefine newRootDBS, BaseConfigEntities context)
        {
            var childDBS = rootDBS.Children;
            foreach (var dbs in childDBS)
            {
                var newDBS = dbs.Clone<S_T_DBSDefine>();
                newDBS.ID = FormulaHelper.CreateGuid();
                newDBS.ParentID = newRootDBS.ID;
                newDBS.FullID = newRootDBS.FullID + "." + newDBS.ID;
                newDBS.ModeID = newRootDBS.ModeID;
                context.Set<S_T_DBSDefine>().Add(newDBS);

                //DBS权限
                var dbsAuth = context.Set<S_T_DBSSecurity>().Where(c => c.DBSDefineID == dbs.ID);
                if (dbsAuth != null)
                {
                    foreach (var auth in dbsAuth)
                    {
                        var newAuth = auth.Clone<S_T_DBSSecurity>();
                        newAuth.ID = FormulaHelper.CreateGuid();
                        newAuth.DBSDefineID = newDBS.ID;
                        context.Set<S_T_DBSSecurity>().Add(newAuth);
                    }
                }

                if (dbs.Children != null)
                {
                    CopyDBSDefine(dbs, newDBS, context);
                }

            }
        }
        /// <summary>
        /// 复制里程碑配置
        /// </summary>
        /// <param name="modelID"></param>
        /// <param name="newModelID"></param>
        /// <param name="context"></param>
        private void CopyMilestone(string modelID, string newModelID, BaseConfigEntities context)
        {
            var milestoneList = context.Set<S_T_MileStone>().Where(c => c.ModeID == modelID).ToList();
            foreach (var milestone in milestoneList)
            {
                var newMilestone = milestone.Clone<S_T_MileStone>();
                newMilestone.ID = FormulaHelper.CreateGuid();
                newMilestone.ModeID = newModelID;
                context.S_T_MileStone.Add(newMilestone);
            }
        }
        /// <summary>
        /// 复制ISO配置
        /// </summary>
        /// <param name="modelID"></param>
        /// <param name="newModelID"></param>
        /// <param name="context"></param>
        private void CopyISODefine(string modelID, string newModelID, BaseConfigEntities context)
        {
            var isoList = context.S_T_ISODefine.Where(c => c.ModeID == modelID).ToList();
            foreach (var iso in isoList)
            {
                var newISO = iso.Clone<S_T_ISODefine>();
                newISO.ID = FormulaHelper.CreateGuid();
                newISO.ModeID = newModelID;
                context.S_T_ISODefine.Add(newISO);
            }
        }

        private void CopyQBSTemplate(S_T_QBSTemplate rootNode, S_T_QBSTemplate newRootNode, BaseConfigEntities context)
        {
            var childNodes = this.S_T_QBSTemplate.Where(d => d.ParentID == rootNode.ID).ToList(); 
            foreach (var node in childNodes)
            {
                var newNode = node.Clone<S_T_QBSTemplate>();
                newNode.ID = FormulaHelper.CreateGuid();
                newNode.ParentID = newRootNode.ID;
                newNode.FullID = newRootNode.FullID + "." + newNode.ID;
                newNode.ModeID = newRootNode.ModeID;
                context.Set<S_T_QBSTemplate>().Add(newNode);

                if (this.S_T_QBSTemplate.Where(d => d.ParentID == node.ID).Count() > 0)
                {
                    CopyQBSTemplate(node, newNode, context);
                }

            }
        }

        private void CopyFlowTraceDefine(S_T_FlowTraceDefine rootNode, S_T_FlowTraceDefine newRootNode, BaseConfigEntities context)
        {
            var childNodes = this.S_T_FlowTraceDefine.Where(d => d.ParentID == rootNode.ID).ToList();
            foreach (var node in childNodes)
            {
                var newNode = node.Clone<S_T_FlowTraceDefine>();
                newNode.ID = FormulaHelper.CreateGuid();
                newNode.ParentID = newRootNode.ID;
                newNode.FullID = newRootNode.FullID + "." + newNode.ID;
                newNode.ModeID = newRootNode.ModeID;
                context.Set<S_T_FlowTraceDefine>().Add(newNode);

                if (this.S_T_FlowTraceDefine.Where(d => d.ParentID == node.ID).Count() > 0)
                {
                    CopyFlowTraceDefine(node, newNode, context);
                }

            }
        }
    }
}
