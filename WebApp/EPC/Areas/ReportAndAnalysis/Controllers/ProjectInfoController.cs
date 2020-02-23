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
using Formula.Exceptions;
using Config;
using Config.Logic;
using MvcAdapter;
using EPC.Logic.Domain;

namespace EPC.Areas.ReportAndAnalysis.Controllers
{
    public class ProjectInfoController : EPCController
    {
        //protected override void BeforeDelete(List<S_I_Project> entityList)
        //{
        //    foreach (var item in entityList)
        //        item.Delete();
        //}

        public JsonResult GetpushTaskForMPList(string ID)
        {
            string sql = "select * from dbo.T_CP_TaskNotice where MarketID='" + ID + "'";
            var dt = SQLHelper.CreateSqlHelper(ConnEnum.Engineering).ExecuteDataTable(sql);
            return Json(dt);
        }

        public JsonResult ValidateData(string ID)
        {
            return Json("");
        }

        //public JsonResult ValidationInfo(string ID)
        //{
        //    var project = this.GetEntityByID(ID);
        //    if (project == null) throw new Formula.Exceptions.BusinessValidationException("未能找到ID为【】的项目信息，无法进行任务单下达操作");
        //    project.ValidateTaskPush();
        //    return Json("");
        //}

        public JsonResult GetInvoiceList(QueryBuilder qb, string ProjectInfoID)
        {
            string sql = @"SELECT  *
                FROM    ( SELECT    Relation.* ,
                                    S_M_ContractInfo_ReceiptObj.ProjectInfo ,
                                    S_M_ContractInfo_ReceiptObj.ProjectInfoName ,
                                    ContractCode ,
                                    ContractName ,
                                    PayerUnit ,
                                    S_M_Invoice.ID ,
                                    InvoiceCode ,
                                    InvoiceType ,
                                    InvoiceDate
                          FROM      ( SELECT    SUM(RelationValue) AS InvoiceValue ,
                                                S_M_ReceiptObjID ,
                                                S_M_InvoiceID,
								                S_M_ContractInfoID
                                      FROM      dbo.S_M_Invoice_ReceiptObjRelation
                                      GROUP BY  S_M_ReceiptObjID ,
                                                S_M_InvoiceID,
								                S_M_ContractInfoID
                                    ) Relation
                                    LEFT JOIN S_M_Invoice ON Relation.S_M_InvoiceID = S_M_Invoice.ID
                                    LEFT JOIN S_M_ContractInfo_ReceiptObj ON S_M_ContractInfo_ReceiptObj.ID = Relation.S_M_ReceiptObjID
					                LEFT joiN
					                 (SELECT ID AS contractid,SerialNumber AS contractcode,Name AS contractname,PartyBName AS PayerUnit  FROM dbo.S_M_ContractInfo)CO
                                    ON CO.contractid = Relation.S_M_ContractInfoID
                        ) tableInfo where ProjectInfo='" + ProjectInfoID + "'";
            qb.PageSize = 0;
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetReceiptList(QueryBuilder qb, string ProjectInfoID)
        {
            string sql = @"select  * from (
                            select S_M_Receipt.ReceiptValue AS Amount,S_M_Receipt.CreateDate AS CreateDate,S_M_Receipt.ID as ID,
                            relation.ReceiptValue,ContractName,ContractCode,
                            sc.ProjectInfo from (select Sum(RelationValue) as ReceiptValue,ReceiptObjID,S_M_ReceiptID from S_M_Receipt_PlanRelation
                            group by ReceiptObjID,S_M_ReceiptID) relation
                            left join S_M_ContractInfo_ReceiptObj on S_M_ContractInfo_ReceiptObj.ID=relation.ReceiptObjID
                            left join S_M_Receipt on S_M_Receipt.ID = relation.S_M_ReceiptID
                            LEFT JOIN(SELECT SerialNumber AS ContractCode,Name AS ContractName,ProjectInfo,ID AS contractid FROM dbo.S_M_ContractInfo)sc ON S_M_ContractInfo_ReceiptObj.S_M_ContractInfoID = sc.contractid
                            ) tableInfo where ProjectInfo='" + ProjectInfoID + "'";
            qb.PageSize = 0;
            var data = this.SqlHelper.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public JsonResult GetList(QueryBuilder qb)
        {
            this.FillQueryBuilderFilter<S_M_CustomerInfo>(qb);
            var qbItem_date = qb.Items.FirstOrDefault(a => a.Field == "Date");
            if (qbItem_date != null)
            {
                qb.Add("CreateDate", QueryMethod.GreaterThanOrEqual, GetQueryString("Date"));
                qb.Items.Remove(qbItem_date);
            }

            string sql = @"select * from (SELECT pinfo.ID,pinfo.Name,pinfo.SerialNumber,pinfo.ProjectScale,pinfo.ProjectClass,
c.Industry,pinfo.CustomerInfo,pinfo.CustomerInfoName,
pinfo.FlowPhase,pinfo.ChargerDeptName,pinfo.ChargerDept,pinfo.ChargerUserName
,pinfo.ChargerUser,pinfo.BusinessChargerName
,pinfo.BusinessCharger,pinfo.OtherDeptName,pinfo.OtherDept,c.Country,isnull(c.City,'') City
,pinfo.State,pinfo.Remark,pinfo.Attachment,pinfo.CreateDate,isnull(c.Province,'') Province FROM dbo.S_I_Engineering pinfo
left join dbo.S_M_CustomerInfo c on pinfo.CustomerInfo = c.ID) aa ";
            GridData grid = this.SqlHelper.ExecuteGridData(sql, qb);

            return Json(grid);

        }

        #region 项目群综合控制
        /// <summary>
        ///  项目一览表
        /// </summary>
        public JsonResult GetProjectList(QueryBuilder qb, string year, string month)
        {
            string sql = @"SELECT S_I_Engineering.*,
                        ISNULL(sc.ContractRMBValue,0) AS ContractRMBValue,
                        ISNULL(sccount.ReceiptValueCount,0) AS ReceiptValue,
                        ISNULL(sccount.ReceiptValueCount/ContractRMBValue*100,0) AS ReceiptRate,
						ISNULL(receiptMonth.ReceiptValue,0) AS currentReceiptValue,
                        workhours.ActualHours,cbs.Budget
                        FROM dbo.S_I_Engineering
                        LEFT JOIN (SELECT EngineeringInfoID,SUM(Budget) AS Budget FROM S_I_CBS WHERE ParentID='' and NodeType='Root' GROUP BY EngineeringInfoID)cbs  ON S_I_Engineering.ID = cbs.EngineeringInfoID
                        LEFT JOIN (SELECT SUM(WorkHourValue) AS ActualHours,ProjectID FROM [Arch_HR].[dbo].S_W_UserWorkHour
                        GROUP BY ProjectID
                        )workhours ON S_I_Engineering.ID = workhours.ProjectID
                        LEFT JOIN(SELECT SUM(ContractRMBValue) AS ContractRMBValue ,ProjectInfo FROM dbo.S_M_ContractInfo GROUP BY ProjectInfo)sc ON S_I_Engineering.ID = sc.ProjectInfo
						LEFT JOIN (SELECT SUM(ReceiptValue) AS ReceiptValueCount,ProjectInfo FROM S_M_Receipt GROUP BY ProjectInfo)sccount ON sccount.ProjectInfo = S_I_Engineering.ID
                        Inner JOIN ( SELECT SUM(ReceiptValue)ReceiptValue,ProjectInfo,BelongYear,BelongMonth FROM
						( SELECT ReceiptValue,ProjectInfo,YEAR(ReceiptDate) AS BelongYear,MONTH(ReceiptDate) AS BelongMonth FROM dbo.S_M_Receipt)sr 
						WHERE sr.BelongYear='{0}' AND sr.BelongMonth='{1}'
						GROUP BY ProjectInfo,BelongYear,BelongMonth)receiptMonth
                        ON S_I_Engineering.ID = receiptMonth.ProjectInfo";
            sql = string.Format(sql, year, month);
            var data = this.SqlHelper.ExecuteDataTable(sql, qb);
            return Json(data);

        }

        public ActionResult ProjectInfo()
        {
            var tab = new Tab();

            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 5, 1, false);
            yearCategory.Multi = false;
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            tab.Categories.Add(yearCategory);

            var monthCategory = CategoryFactory.GetMonthCategory("BelongMonth", false);
            monthCategory.Multi = false;
            monthCategory.SetDefaultItem(DateTime.Now.Month.ToString());
            tab.Categories.Add(monthCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            ViewBag.DefaultYear = DateTime.Now.Year.ToString();
            ViewBag.DefaultMonth = DateTime.Now.Month.ToString();

            return View();
        }

        /// <summary>
        ///  项目工时统计
        /// </summary>
        public JsonResult GetProjectWorkHours(QueryBuilder qb, string year, string month)
        {
            string sql = @"SELECT S_I_Engineering.*,
                        workhours.ActualHoursCount,
						currentWorkHour.currentWorkHourValue
                        FROM dbo.S_I_Engineering
                        LEFT JOIN (SELECT SUM(WorkHourValue) AS ActualHoursCount,ProjectID FROM [Arch_HR].[dbo].S_W_UserWorkHour
                        GROUP BY ProjectID
                        )workhours ON S_I_Engineering.ID = workhours.ProjectID
						INNER JOIN(
						SELECT SUM(WorkHourValue) AS currentWorkHourValue,ProjectID,BelongYear,BelongMonth  FROM [Arch_HR].[dbo].S_W_UserWorkHour
						WHERE BelongYear='{0}' AND BelongMonth='{1}'
						GROUP BY ProjectID,BelongYear,BelongMonth			
						)currentWorkHour
						ON currentWorkHour.ProjectID = S_I_Engineering.ID";
            sql = string.Format(sql, year, month);
            var data = this.SqlHelper.ExecuteDataTable(sql, qb);
            return Json(data);

        }

        public ActionResult ProjectWorkHourInfo()
        {
            var tab = new Tab();

            var yearCategory = CategoryFactory.GetYearCategory("BelongYear", 5, 1, false);
            yearCategory.Multi = false;
            yearCategory.SetDefaultItem(DateTime.Now.Year.ToString());
            tab.Categories.Add(yearCategory);

            var monthCategory = CategoryFactory.GetMonthCategory("BelongMonth", false);
            monthCategory.Multi = false;
            monthCategory.SetDefaultItem(DateTime.Now.Month.ToString());
            tab.Categories.Add(monthCategory);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;
            ViewBag.DefaultYear = DateTime.Now.Year.ToString();
            ViewBag.DefaultMonth = DateTime.Now.Month.ToString();

            return View();
        }

        #endregion

        #region 工程Gis地图更新坐标信息
        /// <summary>
        ///  更新工程坐标信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lat">纬度</param>
        /// <param name="lng">经度</param>
        public void UpdateGisInfo(string id, string lat,string lng,string engineerAddress)
        {
            var entity = FormulaHelper.GetEntities<EPCEntities>();
            S_I_Engineering engineeringInfo = entity.Set<S_I_Engineering>().FirstOrDefault(x => x.ID == id);
            if (engineeringInfo != null)
            {
                engineeringInfo.Lat = lat;
                engineeringInfo.Long = lng;
                engineeringInfo.EngineerAddress = engineerAddress;

                T_I_EngineeringBuild tEBulidInfo = entity.Set<T_I_EngineeringBuild>().FirstOrDefault(x => x.ID == engineeringInfo.BuildFormID);
                if (tEBulidInfo != null)
                {
                    tEBulidInfo.Lat = lat;
                    tEBulidInfo.Long = lng;
                    tEBulidInfo.EngineerAddress = engineerAddress;
                }
                entity.SaveChanges();
            }

        }
        #endregion


    }
}
