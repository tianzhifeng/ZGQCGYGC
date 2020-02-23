using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using EPC.Areas.Cooperation.Models;
using Base.Logic.Domain;
using MvcAdapter;
using System.Data;
using Formula.Exceptions;

namespace EPC.Areas.Cooperation.Controllers
{
    public class DocumentController : EPCFormContorllor<S_I_CommonDocument>
    {
        public ActionResult List()
        {
            var level = String.IsNullOrEmpty(this.GetQueryString("Level").Trim()) ? "true" : this.GetQueryString("Level");
            ViewBag.DisplayLevel = level;
            return View();
        }

        public ActionResult ViewList()
        {
            ViewBag.State = GetQueryString("State");
            ViewBag.EngineeringInfoID = GetQueryString("EngineeringInfoID");
            ViewBag.JustMonth = GetQueryString("JustMonth");
            return View();
        }

        public override ActionResult PageView()
        {
            return View();
        }

        public ActionResult ReviewList()
        {
            ViewBag.EngineeringInfoID = GetQueryString("EngineeringInfoID");
            return View();
        }

        public JsonResult GetFolderTree()
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            string engineeringInfoSql = "select Name from S_I_Engineering where ID = '" + engineeringInfoID + "'";
            var nameRes = this.EPCSQLDB.ExecuteScalar(engineeringInfoSql);
            string sql = GetFolderTreeSql(engineeringInfoID, nameRes.ToString(), "FullID");
            var res = this.EPCSQLDB.ExecuteDataTable(sql);
            return Json(res);
        }

        public JsonResult GetDocumentList(QueryBuilder qb)
        {
            string folderID = GetQueryString("FolderID");
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            string folderSql = GetFolderTreeSql(engineeringInfoID);
            if (!string.IsNullOrEmpty(folderID))
                folderSql = string.Format("select * from ({0}) tmp123 where tmp123.FullID like '%' + '{1}' + '%'", folderSql, folderID);//经过筛选后的目录
            string fileSql = string.Format("select S_I_CommonDocument.* from S_I_CommonDocument inner join ({0}) folder on S_I_CommonDocument.RelateObjID = folder.ID ", folderSql);
            var res = this.EPCSQLDB.ExecuteGridData(fileSql, qb, false);
            return Json(res);
        }

        public JsonResult GetMonthDocumentListByState(QueryBuilder qb)
        {
            string state = GetQueryString("State");
            string justMonth = GetQueryString("JustMonth");
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            string filter = "";// string.Format("and State = '{0}' ", state);
            if (!string.IsNullOrEmpty(state))
                filter = string.Format("and State = '{0}' ", state);

            string monthFilter = "";
            if (justMonth == "true")
            {
                monthFilter = " and datediff(month,CreateDate,getdate())=0 ";
            }

            string sql = string.Format("select * from S_I_CommonDocument where EngineeringInfoID = '{0}' {1}", engineeringInfoID, filter + monthFilter);

            var res = this.EPCSQLDB.ExecuteGridData(sql, qb);
            return Json(res);
        }

        public JsonResult GetReviewDocumentList(QueryBuilder qb)
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            string engineeringInfoSql = "select Name from S_I_Engineering where ID = '" + engineeringInfoID + "'";
            var nameRes = this.EPCSQLDB.ExecuteScalar(engineeringInfoSql).ToString();

            string documentSql = string.Format("select * from S_I_CommonDocument where EngineeringInfoID = '{0}' and State = 'Review' ", engineeringInfoID);

            string folderSql = GetFolderTreeSql(engineeringInfoID, nameRes);
            string sql = string.Format("select tmp11.*, tmp22.FullID, '' as FullName from ({0}) tmp11 left join ({1}) tmp22 on tmp11.RelateObjID = tmp22.ID", documentSql, folderSql);

            var documentGridData = this.EPCSQLDB.ExecuteGridData(sql, qb, false);
            var documentDt = documentGridData.data as DataTable;
            var folderDt = this.EPCSQLDB.ExecuteDataTable(folderSql);
            foreach (DataRow dr in documentDt.Rows)
            {
                string fullID = dr["FullID"].ToString();

                var rows = folderDt.Select(string.Format(" ID in ('{0}')", fullID.Replace(".", "','")));

                dr["FullName"] = string.Join(">", rows.Select(a => a["Name"]));
            }


            return Json(documentGridData);
        }

        [LoggerFilter(OperaType = EnumOperaType.Delete, Table = "S_I_CommonDocument", IDField = "ListData.ID", NameField = "ListData.Name", Remark = "批量删除")]
        public JsonResult DeleteDocument()
        {
            string listDataStr = GetQueryString("ListData");
            var dicList = JsonHelper.ToList(listDataStr);
            foreach (var dic in dicList)
            {
                if (dic.GetValue("State") == EnumDocumentState.Review.ToString())
                {
                    throw new BusinessException("【" + dic.GetValue("Name") + "】审核中无法删除");
                }
                else if (dic.GetValue("State") == EnumDocumentState.Passed.ToString())
                {
                    throw new BusinessException("【" + dic.GetValue("Name") + "】已通过审核无法删除");
                }
            }
            var idList = dicList.Select(a => a.GetValue("ID"));
            string sql = string.Format("delete from S_I_CommonDocument where ID in ('{0}')", string.Join("','", idList));
            this.EPCSQLDB.ExecuteNonQuery(sql);
            return Json("");
        }

        [LoggerFilter(OperaType = EnumOperaType.Update, Table = "S_I_CommonDocument", IDField = "ListData.ID", NameField = "ListData.Name", Remark = "提交审核")]
        public JsonResult UpLoadDocument()
        {
            string listDataStr = GetQueryString("ListData");
            var dicList = JsonHelper.ToList(listDataStr);

            var idList = dicList.Select(a => a.GetValue("ID"));
            string sql = "update S_I_CommonDocument set State = '{0}', UpLoadDate = '{4}',UpLoadUserID = '{5}',UpLoadUser = '{6}' where id in ('{1}') and (State = '{2}' or State = '{3}')";
            this.EPCSQLDB.ExecuteNonQuery(string.Format(sql,
               EnumDocumentState.Review.ToString(),
               string.Join("','", idList),
               EnumDocumentState.Create.ToString(),
               EnumDocumentState.Reject.ToString(),
               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
               CurrentUserInfo.UserID,
               CurrentUserInfo.UserName));
            return Json("");
        }

        [LoggerFilter(OperaType = EnumOperaType.Update, Table = "S_I_CommonDocument", IDField = "ListData.ID", NameField = "ListData.Name", Remark = "通过")]
        public JsonResult PassDocument()
        {
            string listDataStr = GetQueryString("ListData");
            var dicList = JsonHelper.ToList(listDataStr);

            var idList = dicList.Select(a => a.GetValue("ID"));
            string sql = "update S_I_CommonDocument set State = '{0}', PassDate = '{3}',PassUserID = '{4}',PassUser = '{5}' where id in ('{1}') and (State = '{2}')";
            foreach (var dic in dicList.Where(a => a.GetValue("IsPublish") == "1"))
            {
                sql += string.Format(" update S_I_CommonDocument set IsPublish = '1' where id = '{0}' ", dic.GetValue("ID"));
            }

            this.EPCSQLDB.ExecuteNonQuery(string.Format(sql,
               EnumDocumentState.Passed.ToString(),
               string.Join("','", idList),
               EnumDocumentState.Review.ToString(),
               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
               CurrentUserInfo.UserID,
               CurrentUserInfo.UserName));
            return Json("");
        }

        [LoggerFilter(OperaType = EnumOperaType.Update, Table = "S_I_CommonDocument", IDField = "ListData.ID", NameField = "ListData.Name", Remark = "驳回")]
        public JsonResult RejectDocument()
        {
            string listDataStr = GetQueryString("ListData");
            var dicList = JsonHelper.ToList(listDataStr);

            var idList = dicList.Select(a => a.GetValue("ID"));
            string sql = "update S_I_CommonDocument set State = '{0}' where id in ('{1}') and (State = '{2}')";
            this.EPCSQLDB.ExecuteNonQuery(string.Format(sql,
               EnumDocumentState.Reject.ToString(),
               string.Join("','", idList),
               EnumDocumentState.Review.ToString()));
            return Json("");
        }

        public JsonResult SetPublishDocument()
        {
            string idStr = GetQueryString("ID");
            string isPublish = GetQueryString("IsPublish");
            string sql = "update S_I_CommonDocument set IsPublish = '{0}' where id = '{1}'";
            this.EPCSQLDB.ExecuteNonQuery(string.Format(sql, isPublish, idStr));
            return Json("");
        }


        protected override void AfterSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            LoggerHelper.InserLogger("S_I_CommonDocument", isNew ? EnumOperaType.Add : EnumOperaType.Update, dic.GetValue("ID"), dic.GetValue("EngineeringInfoID"), dic.GetValue("Name"));
        }

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                dic.SetValue("State", EnumDocumentState.Passed.ToString());
                dic.SetValue("UpLoadDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                dic.SetValue("PassDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                dic.SetValue("ParticipationType", ParticipationType.General.ToString());
                dic.SetValue("CreateUserID", CurrentUserInfo.UserID);
                dic.SetValue("CreateUserName", CurrentUserInfo.UserName);
                dic.SetValue("Participation", CurrentUserInfo.UserOrgID);
                dic.SetValue("ParticipationName", CurrentUserInfo.UserOrgName);
                dic.SetValue("EngineeringInfoID", GetQueryString("EngineeringInfoID"));
                dic.SetValue("RelateObjType", GetQueryString("RelateObjType"));
                dic.SetValue("RelateObjID", GetQueryString("FolderID"));
                dic.SetValue("ID", FormulaHelper.CreateGuid());

            }
        }

        private const string SectionZhuanXiangFolderKey = "Section";//标段专项目录key
        private const string ProcurementContractZhuanXiangFolderKey = "ProcurementContract";//采购合同专项目录key
        private string GetFolderTreeSql(string engineeringInfoID, string engineeringInfoName = "", string orderBy = "")
        {
            string sql = @"--根节点
                         select  '{3}' as Name, '{0}' as ID, '' as ParentID, '' as RelateObjType, '{0}' as FullID
                         Union
                         --通用文档
                         select * from (
                         --通用文档目录
                         select '通用文档' Name,ID, '{0}' as ParentID, '' as RelateObjType, '{0}'+ '.' + FullID FullID  from S_D_Folder where EngineeringInfoID = '{0}' and ParentID = ''
                         Union
                         --通用文档所有子目录
                         select Name,ID, ParentID, 'DBS' as RelateObjType, '{0}'+ '.' + FullID FullID from S_D_Folder where EngineeringInfoID = '{0}' and ParentID != ''
                         ) tmp1
                         Union
                         --专项文档(业主与监理)
                         select * from (
                         --专项文档目录
                         select '专项文档(业主与监理)' as Name, '{1}' as ID,  '{0}' as ParentID, '' as RelateObjType, '{0}'+ '.' + '{1}' as FullID 
                         Union
                         --总体目录(WBS根节点)
                         select '总体' as Name,ID,'{1}' as ParentID, 'WBS' as RelateObjType, '{0}'+ '.' + '{1}' + '.' + ID as FullID from S_I_WBS where EngineeringInfoID = '{0}'  and (ParentID = ''or ParentID is null)
                         Union
                         --标段目录
                         select Name, ID, '{1}' as ParentID, '{1}' as RelateObjType, '{0}'+ '.' + '{1}' + '.' + ID as FullID  from S_I_Section where EngineeringInfoID = '{0}' 
                         Union
                         --标段下wbs子表目录 --wbsrootid替换为sectionid
                         select * from (
                         select Name,WBSID as ID, S_I_SectionID as ParentID, 'WBS' as RelateObjType, '{0}' + '.' + '{1}' + '.' + replace(FullID, left(FullID,36),S_I_SectionID) FullID
                         from S_I_Section_RelateWBS where len(FullID) = (select top 1 len(FullID) from S_I_Section_RelateWBS where EngineeringInfoID = '{0}' order by len(FullID)) and EngineeringInfoID = '{0}' 
                         union
                         select Name,WBSID as ID, ParentID, 'WBS' as RelateObjType, '{0}' + '.' + '{1}' + '.' + replace(FullID, left(FullID,36),S_I_SectionID) FullID
                         from S_I_Section_RelateWBS where len(FullID) > (select top 1 len(FullID) from S_I_Section_RelateWBS  where EngineeringInfoID = '{0}' order by len(FullID)) and EngineeringInfoID = '{0}' 
                         ) tmp4 
                         ) tmp2
                         Union
                         --专项文档(采购分包)
                         select * from (
                         select '专项文档(采购分包)' as Name, '{2}' as ID,  '{0}' as ParentID, '' as RelateObjType, '{0}'+ '.' + '{2}' as FullID 
                         Union
                         select Name, ID, '{2}' as ParentID, '{2}' as RelateObjType, '{0}' + '.' + '{2}' + '.' + ID as FullID from S_P_ContractInfo where SignDate is not null and ContractProperty = 'Procurement' and EngineeringInfoID = '{0}'
                         ) tmp3
                            ";

            if (!string.IsNullOrEmpty(orderBy))
            {
                sql = sql + " order by " + orderBy;
            }

            return string.Format(sql, engineeringInfoID, SectionZhuanXiangFolderKey, ProcurementContractZhuanXiangFolderKey, engineeringInfoName);
        }

        public JsonResult GetModel(string id)
        {
            var dic = new Dictionary<string, object>();
            bool isNew = true;
            if (!String.IsNullOrEmpty(id))
            {
                var sql = String.Format("select * from {0} where ID='{1}'", "S_I_CommonDocument", id);
                var dt = this.EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    dic = FormulaHelper.DataRowToDic(dt.Rows[0]);
                    isNew = false;
                }
                else
                {
                    throw new Formula.Exceptions.BusinessValidationException(String.Format("数据表【{0}】中没有找到ID为【{1}】的记录，无法读取数据", "S_I_CommonDocument", id));
                }
            }
            AfterGetData(dic, isNew, "");
            return Json(dic);
        }


    }
}
