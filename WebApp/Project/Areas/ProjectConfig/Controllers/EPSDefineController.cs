using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Formula.Helper;
using Config;
using Config.Logic;

namespace Project.Areas.ProjectConfig.Controllers
{
    public class EPSDefineController : BaseConfigController<S_D_EPSDef>
    {
        public JsonResult InitStruct()
        {
            if (this.entities.Set<S_D_EPSDef>().Count() > 0)
                throw new Formula.Exceptions.BusinessException("已经有了EPS定义数据，无法进行初始化");
            var def = new S_D_EPSDef();
            def.ID = Formula.FormulaHelper.CreateGuid();
            def.Name = "工程";
            def.Type = EngineeringSpaceType.Engineering.ToString();
            def.ParentID = "";
            def.FullID = def.ID;
            def.GroupTable = "";
            def.GroupField = "";
            this.entities.Set<S_D_EPSDef>().Add(def);
            this.entities.SaveChanges();
            return Json("");
        }

        protected override void BeforeSave(S_D_EPSDef entity, bool isNew)
        {
            if (isNew)
            {
                var parent = this.GetEntityByID<S_D_EPSDef>(entity.ParentID);
                if (parent == null) throw new Formula.Exceptions.BusinessException("父节点未找到，无法增加");
                parent.AddChild(entity);
            }
        }

        public override JsonResult DeleteNode()
        {
            var ID = this.Request["ID"];
            var def = this.GetEntityByID<S_D_EPSDef>(ID);
            if (def == null) throw new Formula.Exceptions.BusinessException("未能找到指定的节点，删除失败");
            def.Delete();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult UpGrade(string ID)
        {
            var def = this.GetEntityByID<S_D_EPSDef>(ID);
            if (def == null) throw new Formula.Exceptions.BusinessException("未能找到指定的节点，上移失败");
            def.UpGrade();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ReloadEPS()
        {
            var sqlDB = SQLHelper.CreateSqlHelper(ConnEnum.Project);
            sqlDB.ExecuteNonQuery("delete from S_I_ProjectGroup");
            sqlDB.ExecuteNonQuery("delete from S_I_UserDefaultProjectInfo");
            var marketDB = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var dt = marketDB.ExecuteDataTable("select * from S_I_Engineering");
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var baseEntities = FormulaHelper.GetEntities<BaseConfigEntities>();
            var fo = Formula.FormulaHelper.CreateFO<EPSFO>();
            foreach (DataRow row in dt.Rows)
            {
                var group = new S_I_ProjectGroup();
                group.ID = Formula.FormulaHelper.CreateGuid();
                group.Name = row["Name"].ToString();
                group.Code = row["Code"].ToString();
                group.City = row["City"].ToString();
                group.Country = row["Country"].ToString();
                group.Province = row["Province"].ToString();
                group.ProjectClass = row["Class"].ToString();
                group.Investment = String.IsNullOrEmpty(row["Investment"].ToString()) ? 0M : Convert.ToDecimal(row["Investment"].ToString());
                group.Proportion = row["Proportion"].ToString();
                group.PhaseContent = row["PhaseContent"].ToString();
                group.Address = row["Address"].ToString();
                group.DeptID = row["MainDept"].ToString();
                if (baseEntities.S_T_EngineeringSpace.FirstOrDefault() != null)
                    group.EngineeringSpaceCode = baseEntities.S_T_EngineeringSpace.FirstOrDefault().Code;
                group.DeptName = row["MainDeptName"].ToString();
                group.RelateID = row["ID"].ToString();
                group.ChargeUser = row["ChargerUser"].ToString();
                group.ChargeUserName = row["ChargerUserName"].ToString();
                group.CreateDate = row["CreateDate"] == null || String.IsNullOrEmpty(row["CreateDate"].ToString()) ? DateTime.Now : Convert.ToDateTime(row["CreateDate"].ToString());
                fo.BuildEngineering(group);
                projectEntities.SaveChanges();
            }

            var projectList = projectEntities.S_I_ProjectInfo.ToList();
            var groupList = projectEntities.S_I_ProjectGroup.Where(d => d.Type == "Engineering").ToList();
            foreach (var project in projectList)
            {
               var engineerGroup =  groupList.FirstOrDefault(d => d.RelateID == project.EngineeringInfoID);
               if (engineerGroup == null) continue;
               engineerGroup.BindingProject(project);
               projectEntities.SaveChanges();
            }
            return Json("");
        }
    }
}
