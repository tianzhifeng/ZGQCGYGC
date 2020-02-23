using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using Formula;
using Formula.Helper;
using Config;
using Config.Logic;
using EPC.Logic;
using EPC.Logic.Domain;
using MvcAdapter;
using Newtonsoft.Json;
using Formula.ImportExport;
using Base.Logic.Domain;
using System.ComponentModel;
using Base.Logic.BusinessFacade;
using Formula.Exceptions;

namespace EPC.Areas.Documents.Controllers
{
    public class ParticipantDocumentController : EPCFormContorllor<S_I_CommonDocument>
    {
        public ActionResult List()
        {
            var level = String.IsNullOrEmpty(this.GetQueryString("Level").Trim()) ? "true" : this.GetQueryString("Level");
            ViewBag.DisplayLevel = level;
            return View();
        }

        public JsonResult GetFolderTree()
        {
            string engineeringInfoID = GetQueryString("EngineeringInfoID");
            var engineeringInfo = EPCEntites.Set<S_I_Engineering>().Find(engineeringInfoID);
            string sql = GetFolderTreeSql(GetQueryString("EngineeringInfoID"), engineeringInfo.Name, "FullID");
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
            string fileSql = string.Format("select S_I_CommonDocument.* from S_I_CommonDocument inner join ({0}) folder on S_I_CommonDocument.RelateObjID = folder.ID", folderSql);
            var res = this.EPCSQLDB.ExecuteGridData(fileSql, qb, false);
            return Json(res);
        }

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

        public JsonResult UpLoadDocument()
        {
            string listDataStr = GetQueryString("ListData");
            var dicList = JsonHelper.ToList(listDataStr);

            var idList = dicList.Select(a => a.GetValue("ID"));
            string sql = "update S_I_CommonDocument set State = '{0}', UpLoadDate = '{4}' where id in ('{1}') and (State = '{2}' or State = '{3}')";
            this.EPCSQLDB.ExecuteNonQuery(string.Format(sql,
               EnumDocumentState.Review.ToString(),
               string.Join("','", idList),
               EnumDocumentState.Create.ToString(),
               EnumDocumentState.Reject.ToString(),
               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            return Json("");
        }

        public JsonResult PassDocument()
        {
            string listDataStr = GetQueryString("ListData");
            var dicList = JsonHelper.ToList(listDataStr);

            var idList = dicList.Select(a => a.GetValue("ID"));
            string sql = "update S_I_CommonDocument set State = '{0}', PassDate = '{3}' where id in ('{1}') and (State = '{2}')";
            this.EPCSQLDB.ExecuteNonQuery(string.Format(sql,
               EnumDocumentState.Passed.ToString(),
               string.Join("','", idList),
               EnumDocumentState.Review.ToString(),
               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            return Json("");
        }

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
                         select '总体' as Name,ID,'{1}' as ParentID, 'WBS' as RelateObjType, '{0}'+ '.' + '{1}' + ID as FullID from S_I_WBS where EngineeringInfoID = '{0}'  and (ParentID = ''or ParentID is null)
                         Union
                         --标段目录
                         select Name, ID, '{1}' as ParentID, '{1}' as RelateObjType, '{0}'+ '.' + '{1}' + ID as FullID  from S_I_Section where EngineeringInfoID = '{0}' 
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
                         select Name, ID, '{2}' as ParentID, '{2}' as RelateObjType, '{0}' + '{2}' + ID as FullID from S_P_ContractInfo where SignDate is not null and ContractProperty = 'Procurement' and EngineeringInfoID = '{0}'
                         ) tmp3
                            ";

            if (!string.IsNullOrEmpty(orderBy))
            {
                sql = sql + " order by " + orderBy;
            }

            return string.Format(sql, engineeringInfoID, SectionZhuanXiangFolderKey, ProcurementContractZhuanXiangFolderKey,engineeringInfoName);
        }
    }
}
