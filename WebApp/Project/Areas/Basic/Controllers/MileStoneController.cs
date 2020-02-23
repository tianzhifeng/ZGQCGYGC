using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using MvcAdapter;
using Project.Logic.Domain;
using Project.Logic;
using Config.Logic;

namespace Project.Areas.Basic.Controllers
{
    public class MileStoneController : ProjectController<S_P_MileStone>
    {

        #region 里程碑计划管理

        public ActionResult MileStonePlanManageList()
        {
            var tab = new Tab();

            var stateCategory = CategoryFactory.GetCategory(typeof(MileStoneState), "State");
            stateCategory.SetDefaultItem(MileStoneState.Plan.ToString());
            tab.Categories.Add(stateCategory);

            var yearCategory = CategoryFactory.GetYearCategory("Year", 5);
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            tab.Categories.Add(yearCategory);

            var quarterCategory = CategoryFactory.GetQuarterCategory("Season");
            quarterCategory.SetDefaultItem((((DateTime.Now.Month - 1) / 3) + 1).ToString());
            tab.Categories.Add(quarterCategory);

            var monthCategory = CategoryFactory.GetMonthCategory("Month");
            tab.Categories.Add(monthCategory);

            var deptCategory = CategoryFactory.GetCategory("System.ManDept", "部门", "DeptID");
            deptCategory.SetDefaultItem();
            tab.Categories.Add(deptCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            return View();
        }

        public JsonResult GetPlanList(QueryBuilder qb)
        {
            string sql = @"select * from (select S_I_ProjectInfo.Name as ProjectInfoName,
S_I_ProjectInfo.Code as ProjectInfoCode,S_I_ProjectInfo.ChargeUserName,
S_P_MileStonePlan.* from S_P_MileStonePlan
left join S_I_ProjectInfo on S_P_MileStonePlan.ProjectInfoID = S_I_ProjectInfo.ID) MileStonePlanInfo";
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public void ValidateOperation(string MileStoneData)
        {
            var list = JsonHelper.ToList(MileStoneData);
            foreach (var item in list)
            {
                var plan = this.GetEntityByID<S_P_MileStonePlan>(item.GetValue("ID"));
                if (plan == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + item.GetValue("ID") + "】的里程碑信息，无法进行操作");
                if (plan.State == ProjectCommoneState.Finish.ToString())
                    throw new Formula.Exceptions.BusinessException("【" + plan.Name + "】已经完成，无法进行操作");
            }
        }

        public void DelayPlan(string MileStoneData, string NewPlanDate, string ChangeType)
        {
            var list = JsonHelper.ToList(MileStoneData);
            if (String.IsNullOrEmpty(NewPlanDate.Trim('\"'))) throw new Formula.Exceptions.BusinessException("必须指定延迟日期");
            foreach (var item in list)
            {
                var plan = this.GetEntityByID<S_P_MileStonePlan>(item.GetValue("ID"));
                if (plan == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + item.GetValue("ID") + "】的里程碑信息，无法进行变更操作");
                var date = Convert.ToDateTime(NewPlanDate.Trim('\"'));
                if (ChangeType == MileStoneChangeType.Delay.ToString())
                    plan.Delay(date);
                else
                    plan.Delay(date, true);
            }
            this.entities.SaveChanges();
        }

        public void Finish(string MileStoneData, string FinishDate)
        {
            var list = JsonHelper.ToList(MileStoneData);
            if (String.IsNullOrEmpty(FinishDate)) throw new Formula.Exceptions.BusinessException("必须指定延迟日期");

            //同步关联的收款项的里程碑信息：时间、状态
            var fo = new Basic.Controllers.MileStoneExecuteController();

            foreach (var item in list)
            {
                var plan = this.GetEntityByID<S_P_MileStonePlan>(item.GetValue("ID"));
                if (plan == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + item.GetValue("ID") + "】的里程碑信息，无法进行变更操作");
                var date = Convert.ToDateTime(FinishDate.Trim('\"'));
                plan.Finish(date);
                //fo.SyncReceiptObj(plan.S_P_MileStone);
            }
            this.entities.SaveChanges();
            //FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>().SaveChanges();
        }

        #endregion

        #region 里程碑策划

        public ActionResult MileStoneTab()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息，无法进行里程碑策划");
            var level = EnumBaseHelper.GetEnumDef(typeof(Project.Logic.MileStoneType)).EnumItem.ToList();
            ViewBag.MileStoneLevel = level;
            ViewBag.ProjectInfoID = projectInfoID;
            return View();
        }

        public ActionResult MajorMileStoneList()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息，无法进行里程碑策划");
            var majors = projectInfo.GetMajors();
            ViewBag.ProjectInfoID = projectInfoID;
            if (majors.Count == 0) throw new Formula.Exceptions.BusinessException("未进行专业策划的项目，无法策划专业里程碑");
            ViewBag.DefaultMajorCode = majors.FirstOrDefault().GetValue("Value");
            ViewBag.Majors = majors;
            ViewBag.RootNode = JsonHelper.ToJson(projectInfo.WBSRoot);
            return View();
        }

        public ActionResult MileStonePlanList()
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息，无法进行里程碑策划");
            var level = EnumBaseHelper.GetEnumDef(typeof(Project.Logic.MileStoneType)).EnumItem.ToList();
            ViewBag.MileStoneLevel = level;
            ViewBag.RootNode = JsonHelper.ToJson(projectInfo.WBSRoot);
            string sql = @"select S_C_ManageContract_ReceiptObj.ID,S_C_ManageContract_ReceiptObj.Name,S_C_ManageContract.Name as 
ContractName,PlanFinishDate,ReceiptValue  from S_C_ManageContract_ReceiptObj
left join.S_C_ManageContract on S_C_ManageContract_ReceiptObj.S_C_ManageContractID = S_C_ManageContract.ID
where ProjectInfo = '{0}'";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var dt = db.ExecuteDataTable(String.Format(sql, projectInfo.MarketProjectInfoID));
            ViewBag.ReceiptObject = JsonHelper.ToJson(dt);
            return View();
        }

        public JsonResult GetMajorMileStoneList(QueryBuilder qb, string MajorCode, string ProjectInfoID)
        {
            string sql = string.Format("SElect * from S_P_MileStone where MajorValue='{0}' and MileStoneType='{1}' and ProjectInfoID='{2}' ", MajorCode, MileStoneType.Normal.ToString(), ProjectInfoID);
            GridData data = SQLHelper.CreateSqlHelper(ConnEnum.Project).ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult ImportMileStoneData(string MileStoneData, string ProjectInfoID, string WBSID, string MajorValue)
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
                wbs.ImportMileStoneDefine(mileStoneDefine, MajorValue);
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteMileStone(string MileStoneData)
        {
            var list = JsonHelper.ToList(MileStoneData);
            var deleteMileStoneIDList = new List<string>(); 
            foreach (var item in list)
            {
                var ID = item.GetValue("ID");
                if (String.IsNullOrEmpty(ID))
                    continue;
                var mileStone = this.GetEntityByID<S_P_MileStone>(ID);
                if (mileStone == null) continue;

                if (mileStone.State == MileStoneState.Finish.ToString())
                    throw new Formula.Exceptions.BusinessException("里程碑【" + mileStone.Name + "】已经完成，无法删除");
                this.entities.Set<S_P_MileStone>().Delete(d => d.ID == ID);
                deleteMileStoneIDList.Add(ID);
            }
            this.entities.SaveChanges();

            //同步关联的收款项的里程碑信息：时间、状态
            //var fo = new Basic.Controllers.MileStoneExecuteController();
            //找到删除的里程碑的所有收款项，在这些收款项中去除当前里程碑，在同步这些收款项的里程碑信息
            //foreach (var item in deleteMileStoneIDList)
            //{
            //    fo.UpdateReceiptObjByDelMeliStoneID(item);
            //}
            //FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>().SaveChanges();

            return Json("");
        }

        public override JsonResult GetList(QueryBuilder qb)
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目对象");
            string sql = "SELECT *,'' as ReceiptObjectID FROM S_P_MileStone ";
            this.FillQueryBuilderFilter(qb);
            var dt = this.SqlHelper.ExecuteDataTable(sql, qb);

            //里程碑关联收款项
            sql = "select * from dbo.S_C_ManageContract_ReceiptObj where ProjectInfo='" + projectInfo.MarketProjectInfoID + "' ";
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            var receiptObjectDT = db.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                var receiptObjs = receiptObjectDT.Select(" MileStoneID='" + row["ID"] + "'");
                var ids = string.Empty;
                foreach (var item in receiptObjs)
                    ids += item["ID"] + ",";
                ids = ids.TrimEnd(',');
                row["ReceiptObjectID"] = ids;
            }
            var data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);
        }

        public JsonResult MileStoneSave(string MileStoneData, string ProjectInfoID, string WBSID)
        {
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象，无法保存里程碑信息");
            var list = JsonHelper.ToList(MileStoneData);
            var marketDB = SQLHelper.CreateSqlHelper(ConnEnum.Market);
            int Weight = 0;

            //同步关联的收款项的里程碑信息：时间、状态
            var fo = new Basic.Controllers.MileStoneExecuteController();

            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.GetValue("Weight")))
                {
                    if (Weight < Convert.ToInt32(item.GetValue("Weight")))
                        Weight = Convert.ToInt32(item.GetValue("Weight"));
                }
                if (item.GetValue("_state") == "removed") continue;
                S_P_MileStone mileStone;
                mileStone = this.GetEntityByID<S_P_MileStone>(item.GetValue("ID"));
                if (mileStone == null)
                {
                    mileStone = this.CreateEmptyEntity<S_P_MileStone>();
                }
                this.UpdateEntity(mileStone, item);
                mileStone.Save();

                //fo.SyncReceiptObj(mileStone);

                var receiptObjID = item.GetValue("ReceiptObjectID");
                if (!String.IsNullOrEmpty(receiptObjID))
                {
                    string sql = "update S_C_ManageContract_ReceiptObj set MileStoneID='{0}',MileStoneState='{1}' WHERE ID in ('{2}') ";
                    marketDB.ExecuteNonQuery(String.Format(sql, mileStone.ID, mileStone.State, receiptObjID.Replace(",", "','")));
                }
                else
                {
                    string sql = "update S_C_ManageContract_ReceiptObj set MileStoneID='',MileStoneState='' WHERE MileStoneID='{0}'  ";
                    marketDB.ExecuteNonQuery(String.Format(sql, mileStone.ID));
                }
            }
            if (Weight>100)
                throw new Formula.Exceptions.BusinessException("权重不能大于100%");
            entities.SaveChanges();
            //FormulaHelper.GetEntities<Market.Logic.Domain.MarketEntities>().SaveChanges();
            return Json("");
        }

        #endregion

        #region 获取里程碑模板导入数据信息

        public JsonResult GetMileStoneTemplateList(QueryBuilder qb)
        {
            string belongMajors = this.Request["BelongMajors"];
            string mileStoneType = this.Request["MileStoneType"];
            string projectInfoID = this.Request["ProjectInfoID"];
            var projectInfo = this.GetEntityByID<S_I_ProjectInfo>(projectInfoID);
            if (projectInfo == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + projectInfoID + "】的项目信息对象");
            if (projectInfo.ProjectMode == null) throw new Formula.Exceptions.BusinessException("未能找到编号为【" + projectInfo.ProjectMode + "】的管理模式对象，无法导入里程碑");
            List<S_T_MileStone> result;
            IQueryable<S_T_MileStone> query;
            if (String.IsNullOrEmpty(belongMajors))
                query = projectInfo.ProjectMode.S_T_MileStone.AsQueryable().Where(d => d.MileStoneType == mileStoneType).Where((SearchCondition)qb);
            else
                query = projectInfo.ProjectMode.S_T_MileStone.AsQueryable().Where(d => d.MileStoneType == mileStoneType && (
                    String.IsNullOrEmpty(d.OutMajors) || d.OutMajors.Contains(belongMajors))).Where((SearchCondition)qb);
            result = query.ToList();
            var gridData = new GridData(result);
            return Json(gridData);
        }

        #endregion

    }
}
