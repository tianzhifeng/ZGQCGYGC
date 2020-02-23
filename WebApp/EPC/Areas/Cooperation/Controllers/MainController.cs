using Config;
using Config.Logic;
using EPC;
using EPC.Logic;
using Formula;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPC.Areas.Cooperation.Controllers
{
    public class MainController :  EPCController
    {
        private const int rightMainListMaxCount = 10;
        public ActionResult Index()
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            ViewBag.EngineeringInfo = engineeringInfoID;


            SetContactList(engineeringInfoID);//工作联系单
            SetProblemList(engineeringInfoID);//问题整改单
            SetDocuemtMainList(engineeringInfoID);
            SetVideoMainList(engineeringInfoID);
            return View();
        }

        //工作联系单
        private void SetContactList(string engineeringInfoID)
        {
            int total = 0;
            int add = 0;//新增
            int received = 0;//已接收
            int resolved = 0;//已解决
            string sql = string.Format("select Status,COUNT(ID) as qty from T_C_WorkingContact where EngineeringInfo='{0}' and MessageCompany=''  group by Status ", engineeringInfoID);
            var dt = SqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                total = Convert.ToInt32(dt.Compute("sum(qty)", ""));
                var row = dt.Select(" Status = 'Add' ");
                if (row.Length > 0)
                    add = Convert.ToInt32(row[0]["qty"]);
                row = dt.Select(" Status = 'Received' ");
                if (row.Length > 0)
                    received = Convert.ToInt32(row[0]["qty"]);
                row = dt.Select(" Status = 'Resolved' ");
                if (row.Length > 0)
                    resolved = Convert.ToInt32(row[0]["qty"]);
            }

            ViewBag.ContactTotal = total;//总计
            ViewBag.ContactAdd = add;//新增
            ViewBag.ContactReceived = received;//已接收
            ViewBag.ContactResolved = resolved;//已解决
            ViewBag.ContactAddList = new List<Dictionary<string, object>>();//待接收
            ViewBag.ContactCreate = new List<Dictionary<string, object>>();//我创建的

            sql = string.Format("select top 5 * from T_C_WorkingContact where EngineeringInfo='{0}' and Status='Add' ", engineeringInfoID);
            dt = SqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
                ViewBag.ContactAddList = FormulaHelper.DataTableToListDic(dt);

            sql = string.Format("select top 5 * from T_C_WorkingContact where EngineeringInfo='{0}' and MessageCompany='' ", engineeringInfoID);
            dt = SqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
                ViewBag.ContactCreate = FormulaHelper.DataTableToListDic(dt);


        }

        //问题整改单
        private void SetProblemList(string engineeringInfoID)
        {
            int total = 0;
            int register = 0;
            int rectify = 0;
            int closed = 0;
            string sql = string.Format("select RectifyState as Status,COUNT(ID) as qty from S_C_RectifySheet_RectifyProblems where EngineeringInfo='{0}' and CreateCompanyID='' group by RectifyState ", engineeringInfoID);
            var dt = SqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                total = Convert.ToInt32(dt.Compute("sum(qty)", ""));
                var row = dt.Select(" Status = 'Register' ");
                if (row.Length > 0)
                    register = Convert.ToInt32(row[0]["qty"]);
                row = dt.Select(" Status = 'Rectify' ");
                if (row.Length > 0)
                    rectify = Convert.ToInt32(row[0]["qty"]);
                row = dt.Select(" Status = 'Closed' ");
                if (row.Length > 0)
                    closed = Convert.ToInt32(row[0]["qty"]);
            }

            ViewBag.ProblemTotal = total;//总计发起
            ViewBag.ProblemRegister = register;//待整改
            ViewBag.Closed = closed;//已关闭
            ViewBag.ProblemUnclosed = new List<Dictionary<string, object>>();//待关闭
            ViewBag.ProblemCreate = new List<Dictionary<string, object>>();//我发起的

            sql = string.Format("select top 5 * from S_C_RectifySheet_RectifyProblems where EngineeringInfo='{0}' and CreateCompanyID='' and RectifyState='Rectify'  ", engineeringInfoID, CurrentUserInfo.UserOrgID);
            dt = SqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
                ViewBag.ProblemUnclosed = FormulaHelper.DataTableToListDic(dt);

            sql = string.Format("select top 5 * from S_C_RectifySheet_RectifyProblems where EngineeringInfo='{0}' and CreateCompanyID='' ", engineeringInfoID, CurrentUserInfo.UserOrgID);
            dt = SqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
                ViewBag.ProblemCreate = FormulaHelper.DataTableToListDic(dt);

        }

        private void SetDocuemtMainList(string engineeringInfoID)
        {
            {
                string stateFilter = "";// string.Format("and State != '{0}' ", EPCCooperation.Models.EnumDocumentState.Create.ToString());
                string userDefaultEnterSql = string.Format("select top " + rightMainListMaxCount + " * from S_I_CommonDocument where Participation = '{0}' and EngineeringInfoID = '{1}' {2} order by CreateDate desc", CurrentUserInfo.UserOrgID, engineeringInfoID, stateFilter);
                var userDefaultEnterTb = this.SqlHelper.ExecuteDataTable(userDefaultEnterSql);

                var tmpList = FormulaHelper.DataTableToListDic(userDefaultEnterTb);
                ViewBag.MyDocumentList = tmpList;
            }

            {
                string stateFilter = string.Format("and State = '{0}' ", EnumDocumentState.Review.ToString());
                string userDefaultEnterSql = string.Format("select top " + rightMainListMaxCount + " * from S_I_CommonDocument where EngineeringInfoID = '{0}' {1} order by CreateDate desc", engineeringInfoID, stateFilter);
                var userDefaultEnterTb = this.SqlHelper.ExecuteDataTable(userDefaultEnterSql);

                var tmpList = FormulaHelper.DataTableToListDic(userDefaultEnterTb);
                ViewBag.ReviewDocumentList = tmpList;
            }

            //本月已上传
            {
                string filter = "";// string.Format("and State != '{0}' ", EPCCooperation.Models.EnumDocumentState.Create.ToString());
                string sql = "select count(*) from S_I_CommonDocument where EngineeringInfoID = '" + engineeringInfoID + "' ";
                var countObj = this.SqlHelper.ExecuteScalar(string.Format(sql, filter));
                ViewBag.MonthDocumentCount = countObj;
            }

            //待审核
            {
                string filter = string.Format("and State = '{0}' ", EnumDocumentState.Review.ToString());
                string sql = "select count(*) from S_I_CommonDocument where EngineeringInfoID = '" + engineeringInfoID + "' {0}";
                var countObj = this.SqlHelper.ExecuteScalar(string.Format(sql, filter));
                ViewBag.MonthDocumentReviewCount = countObj;
            }
        }

        private int PicCountToShow = 10;
        private void SetVideoMainList(string engineeringinfoID)
        {
            //本月已上传
            {
                string sql = "select count(*) from S_C_ProjectVideo where EngineeringInfoID = '" + engineeringinfoID + "' ";
                var countObj = this.SqlHelper.ExecuteScalar(sql);
                ViewBag.MonthVideoCount = countObj;
            }
            //其中待发布
            {
                string sql = "select count(*) from S_C_ProjectVideo where EngineeringInfoID = '" + engineeringinfoID + "' and IsPublish != '1'";
                var countObj = this.SqlHelper.ExecuteScalar(sql);
                ViewBag.MonthVideoForPublishCount = countObj;
            }

            //待发布的
            {
                string picSql = "select top " + PicCountToShow + " * from S_C_ProjectVideo where EngineeringInfoID = '" + engineeringinfoID + "' and FileIDs != '' and FileIDs is not null and IsPublish = '0' order by CreateDate desc";
                var dt = this.SqlHelper.ExecuteDataTable(picSql);
                List<Dictionary<string, object>> dicList = NewMethod(dt);
                ViewBag.VideoFileForPublish = dicList;
            }

            //我上传的
            {
                string picSql = "select top " + PicCountToShow + " * from S_C_ProjectVideo where EngineeringInfoID = '" + engineeringinfoID + "' and FileIDs != '' and FileIDs is not null and CreateUserOrg = '" + CurrentUserInfo.UserOrgID + "' order by CreateDate desc";
                var dt = this.SqlHelper.ExecuteDataTable(picSql);
                List<Dictionary<string, object>> dicList = NewMethod(dt);
                ViewBag.MyVideoFile = dicList;
            }
        }

        private static List<Dictionary<string, object>> NewMethod(DataTable dt)
        {
            List<Dictionary<string, object>> dicList = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                string fileIDs = dr["FileIDs"].ToString();
                if (!string.IsNullOrEmpty(fileIDs))
                {
                    var fileIDArr = fileIDs.Split(',');
                    foreach (var fileID in fileIDArr)
                    {
                        Dictionary<string, object> fileDic = new Dictionary<string, object>();
                        fileDic.SetValue("Title", dr["Title"].ToString());
                        if (!string.IsNullOrEmpty(fileID))
                        {
                            var fileIDPre = fileID.Split('_');
                            if (fileIDPre.Length > 0)
                            {
                                fileDic.SetValue("FileID", fileIDPre[0]);
                            }
                        }
                        dicList.Add(fileDic);
                    }
                }
            }
            return dicList;
        }

        public ActionResult LogList()
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            ViewBag.EngineeringInfoID = engineeringInfoID;
            return View();
        }

        public ActionResult LogView()
        {
            string tableID = GetQueryString("TableID");
            string sql = "select * from S_A_ParticipantOperaLog where TableIDs like '%' + '{0}' + '%' order by CreateDate";
            var dt = this.SqlHelper.ExecuteDataTable(string.Format(sql, tableID));
            ViewBag.LogList = FormulaHelper.DataTableToListDic(dt);
            return View();
        }

        public JsonResult GetLogList(QueryBuilder qb)
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            string isMineFilterStr = " and CompanyID = '" + CurrentUserInfo.UserOrgID + "'";
            string sql = "select * from S_A_ParticipantOperaLog where EngineeringInfoID =  '{0}' {1} order by CreateDate";
            var res = this.SqlHelper.ExecuteGridData(string.Format(sql, engineeringInfoID, isMineFilterStr), qb);
            return Json(res);
        }

    }
}
