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
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;
using Config.Logic;

namespace Project.Areas.Gantt.Controllers
{
    public class MileStoneGanttController : ProjectController<S_P_MileStone>
    {
        public override JsonResult GetTree()
        {
            var fo = FormulaHelper.CreateFO<ProjectInfoFO>();
            string projectInfoID = this.Request["ProjectInfoID"];
            string includeWBS = this.Request["IncludeWBS"];
            bool includewbs = false;
            if (includeWBS == "True") includewbs = true;
            var result = fo.GetMileStoneGantt(projectInfoID, includewbs);
            return Json(result);
        }

        public JsonResult SaveMileStoneList(string NodeList,string ProjectInfoID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ProjectInfoID + "】的项目信息对象，无法保存里程碑");
            var list = JsonHelper.ToList(NodeList);
            foreach (var item in list)
            {
                if (item.GetValue("Milestone") == "1")
                {
                    S_P_MileStone mileStone = this.GetEntityByID<S_P_MileStone>(item.GetValue("MileStoneID"));
                    if (mileStone == null)
                    {
                        mileStone = new S_P_MileStone();                        
                        projectInfo.AddMileStone(mileStone);                   
                    }
                    this.UpdateEntity<S_P_MileStone>(mileStone, item);
                    mileStone.PlanFinishDate = Convert.ToDateTime(item.GetValue("Finish"));
                    mileStone.Save();
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteMileStone(string NodeList)
        {
            var list = JsonHelper.ToList(NodeList);
            foreach (var item in list)
            {
                if (item.GetValue("Milestone") == "1")
                {
                    string ID =item.GetValue("MileStoneID");
                    S_P_MileStone mileStone = this.GetEntityByID<S_P_MileStone>(ID);
                    if (mileStone != null && mileStone.Necessity == true.ToString())
                        throw new Formula.Exceptions.BusinessException("里程碑【" + mileStone.Name + "】为项目必要里程碑，无法删除");
                    if (mileStone != null)
                        this.entities.Set<S_P_MileStone>().Delete(d => d.ID == ID);
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportMileStoneData(string MileStoneData, string ProjectInfoID, string WBSID)
        {
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(ProjectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + ProjectInfoID + "】的项目信息对象，无法导入里程碑");
            var baseConfigEneites = FormulaHelper.GetEntities<BaseConfigEntities>();
            var mileStoneList = JsonHelper.ToList(MileStoneData);
            S_W_WBS wbs;
            if (String.IsNullOrEmpty(WBSID))
                wbs = projectInfo.WBSRoot;
            else
                wbs = entities.Set<S_W_WBS>().FirstOrDefault(d => d.ID == WBSID);
            if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + WBSID + "】的WBS节点，无法导入里程碑信息");
            foreach (var item in mileStoneList)
            {
                string id = item.GetValue("ID");
                var mileStoneDefine = baseConfigEneites.S_T_MileStone.FirstOrDefault(d => d.ID == id);
                //wbs.ImportMileStoneDefine(mileStoneDefine);
            }
            entities.SaveChanges();
            return Json("");
        }
    }
}
