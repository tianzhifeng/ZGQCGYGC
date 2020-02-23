using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Entity;
using System.Collections;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Config;
using Config.Logic;
using Project.Logic.Domain;

namespace Project.Logic
{
    public class BaseConfigFO
    {
        BaseConfigEntities entities = FormulaHelper.GetEntities<BaseConfigEntities>();
        ProjectEntities projectEntities = FormulaHelper.GetEntities<ProjectEntities>();

        public S_D_RoleDefine GetRoleDefine(string roleCode)
        {
            return entities.S_D_RoleDefine.FirstOrDefault(d => d.RoleCode == roleCode);
        }


        #region 静态方法

        public static S_T_EngineeringSpace GetEngineeringSpace(string code)
        {
            var mode = CacheHelper.Get(CommonConst.enginnerSpaceCacheKey + code); //获取缓存数据
            if (mode == null) //缓存未空则从数据库中重新获取
            {
                var configEntites = FormulaHelper.GetEntities<BaseConfigEntities>();
                var projectMode = configEntites.S_T_EngineeringSpace.Include("S_T_EngineeringSpaceRes")
                     .Include("S_T_EngineeringSpaceRes.S_T_EngineeringSpaceAuth")
                     .FirstOrDefault(d => d.Code == code);
                CacheHelper.Set(CommonConst.cacheKey + code, projectMode); //将数据库查询结果压入缓存中
                return projectMode;
            }
            else
            {
                return mode as S_T_EngineeringSpace;
            }
        }

        /// <summary>
        /// 根据管理模式编号，获取管理模式对象（如果缓存中存在则从缓存中和获取，同时一次性取出所有关联性配置定义数据）
        /// </summary>
        /// <param name="code">管理模式编号</param>
        /// <returns>管理模式对象</returns>
        public static S_T_ProjectMode GetMode(string code)
        {
            var mode = CacheHelper.Get(CommonConst.cacheKey + code); //获取缓存数据
            if (mode == null) //缓存未空则从数据库中重新获取
            {
                var configEntites = FormulaHelper.GetEntities<BaseConfigEntities>();
                var projectMode = configEntites.S_T_ProjectMode.Include("S_T_SpaceDefine")
                     .Include("S_T_SpaceDefine.S_T_SpaceAuth")
                     .Include("S_T_DBSDefine")
                     .Include("S_T_DataAuth")
                     .Include("S_T_WBSStructInfo")
                     .Include("S_T_WBSStructInfo.S_T_WBSStructRole")
                     .Include("S_T_MileStone")
                     .Include("S_T_ISODefine")
                     .Include("S_T_FlowTraceDefine")
                     .Include("S_T_ToDoListDefine")
                     .Include("S_T_ToDoListDefine.S_T_ToDoListDefineNode")
                     .FirstOrDefault(d => d.ModeCode == code);
                CacheHelper.Set(CommonConst.cacheKey + code, projectMode); //将数据库查询结果压入缓存中
                return projectMode;
            }
            else
            {
                return mode as S_T_ProjectMode;
            }
        }

        /// <summary>
        /// 根据WBS类型来获取WBS属性定义，并转换成枚举
        /// </summary>
        /// <param name="wbsType">WBS类型枚举</param>
        /// <returns>含有text,value 列的枚举DataTable</returns>
        public static DataTable GetWBSEnum(string wbsType, string projectClass = "")
        {
            var result = new DataTable();
            result.Columns.Add("text");
            result.Columns.Add("value");
            result.Columns.Add("NameEN");
            result.Columns.Add("BelongMode");
            var configEntites = FormulaHelper.GetEntities<BaseConfigEntities>();
            string type = wbsType.ToString();
            var list = configEntites.S_D_WBSAttrDefine.Where(d => d.Type == type).OrderBy(d => d.SortIndex).ToList();
            if (!string.IsNullOrEmpty(projectClass))
                list = list.Where(a => !string.IsNullOrEmpty(a.BelongMode) && a.BelongMode.Split(',').Contains(projectClass)).ToList();
            foreach (var item in list)
            {
                var row = result.NewRow();
                row["text"] = item.Name;
                row["value"] = item.Code;
                row["NameEN"] = item.NameEN;
                row["BelongMode"] = item.BelongMode;
                result.Rows.Add(row);
            }
            return result;
        }

        public static List<Dictionary<string, object>> GetWBSEnumDic(string wbsType, string radio = "T")
        {
            var result = new List<Dictionary<string, object>>();
            var configEntites = FormulaHelper.GetEntities<BaseConfigEntities>();
            string type = wbsType.ToString();
            var list = configEntites.S_D_WBSAttrDefine.Where(d => d.Type == type).OrderBy(d => d.SortIndex).ToList();
            foreach (var item in list)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("text", item.Name);
                dic.SetValue("value", item.Code);
                dic.SetValue("radio", radio);
                result.Add(dic);
            }
            return result;
        }


        /// <summary>
        /// 根据WBS类型来获取WBS属性定义，并转换成枚举
        /// </summary>
        /// <param name="wbsType">WBS类型枚举</param>
        /// <returns>含有text,value 列的枚举DataTable</returns>
        public static DataTable GetWBSEnum(WBSNodeType wbsType)
        {
            return GetWBSEnum(wbsType.ToString());
        }

        /// <summary>
        /// 根据WBS类型来获取WBS属性定义对象列表(无缓存，实时数据附带关联的部门信息)
        /// </summary>
        /// <param name="wbsType">WBS类型枚举</param>
        /// <returns>WBS属性集合</returns>
        public static List<S_D_WBSAttrDefine> GetWBSAttrList(string wbsType)
        {
            var configEntites = FormulaHelper.GetEntities<BaseConfigEntities>();
            var list = configEntites.S_D_WBSAttrDefine
                .Include("S_D_WBSAttrDeptInfo")
                .Where(d => d.Type == wbsType).OrderBy(d => d.SortIndex).ToList();
            return list;
        }

        /// <summary>
        /// 根据WBS类型来获取WBS属性定义对象列表(无缓存，实时数据附带关联的部门信息)
        /// </summary>
        /// <param name="wbsType">WBS类型枚举</param>
        /// <returns>WBS属性集合</returns>
        public static List<S_D_WBSAttrDefine> GetWBSAttrList(WBSNodeType wbsType)
        {
            return GetWBSAttrList(wbsType.ToString());
        }

        /// <summary>
        /// 根据部门获取该部门所关联的WBS定义信息
        /// </summary>
        /// <param name="wbsType">wbs节点类别</param>
        /// <param name="DeptID">部门ID</param>
        /// <returns>返回S_D_WBSAttrDefine集合</returns>
        public static List<S_D_WBSAttrDefine> GetWBSAttrFromDept(WBSNodeType wbsType, string DeptID)
        {
            var entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var result = entities.S_D_WBSAttrDeptInfo.Where(d => d.DeptID == DeptID).
                Select(d => d.S_D_WBSAttrDefine).Where(d => d.Type == wbsType.ToString());
            return result.ToList();

        }

        /// <summary>
        /// 根据部门获取该部门所关联的WBS定义信息
        /// </summary>
        /// <param name="wbsType">wbs节点类别</param>
        /// <param name="DeptID">部门ID</param>
        /// <returns>返回DataTable</returns>
        public static DataTable GetWBSAttrTableFromDept(WBSNodeType wbsType, string DeptID)
        {
            string sql = @"select distinct S_D_WBSAttrDefine.* from dbo.S_D_WBSAttrDefine
left join dbo.S_D_WBSAttrDeptInfo on  S_D_WBSAttrDefine.ID=S_D_WBSAttrDeptInfo.WBSAttrDefineID
where S_D_WBSAttrDefine.Type='{0}' and S_D_WBSAttrDeptInfo.DeptID='{1}' ";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.InfrasBaseConfig);
            var result = db.ExecuteDataTable(sql);
            return result;
        }

        /// <summary>
        /// 获取岗位角色集合
        /// </summary>
        /// <returns>角色集合</returns>
        public static List<S_D_RoleDefine> GetRoleDefineList()
        {
            var entities = FormulaHelper.GetEntities<BaseConfigEntities>();
            return entities.S_D_RoleDefine.ToList();
        }

        public static DataTable GetRoleDefineEnum()
        {
            var result = new DataTable();
            result.Columns.Add("text");
            result.Columns.Add("value");
            var list = GetRoleDefineList();
            foreach (var item in list)
            {
                var row = result.NewRow();
                row["text"] = item.RoleName;
                row["value"] = item.RoleCode;
                result.Rows.Add(row);
            }
            return result;
        }

        #endregion
    }
}
