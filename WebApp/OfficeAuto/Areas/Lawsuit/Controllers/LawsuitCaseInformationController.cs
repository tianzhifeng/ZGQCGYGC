using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;
using MvcAdapter;

namespace OfficeAuto.Areas.Lawsuit.Controllers
{
    public class LawsuitCaseInformationController : OfficeAutoFormContorllor<T_Lawsuit_CaseInformation>
    {
        // GET: /Lawsuit/LawsuitCaseInformation/


        public JsonResult GetList(QueryBuilder qb)
        {
            var data = BusinessEntities.Set<T_Lawsuit_CaseInformation>().WhereToGridData(qb);

            return Json(data);
        }

        public JsonResult GetCaseFileList(QueryBuilder qb)
        {
            var caseID = GetQueryString("CaseID");
            var data = BusinessEntities.Set<T_Lawsuit_CaseFile>().Where(p => p.CaseID == caseID).WhereToGridData(qb);

            return Json(data);
        }

        public JsonResult DelCase() 
        {
            var ids = GetQueryString("IDs");

            if (string.IsNullOrEmpty(ids))
            {
                return Json("");
            }

            var arr = ids.Split(',');

            foreach (var id in arr)
            {
                var caseInfo = BusinessEntities.Set<T_Lawsuit_CaseInformation>().Find(id);

                if (caseInfo != null)
                {
                    BusinessEntities.Set<T_Lawsuit_CaseInformation>().Remove(caseInfo);

                    var fileList = BusinessEntities.Set<T_Lawsuit_CaseFile>().Where(p => p.CaseID == caseInfo.ID);

                    foreach (var file in fileList)
                    {
                        BusinessEntities.Set<T_Lawsuit_CaseFile>().Remove(file);
                    }
                }
            }
            BusinessEntities.SaveChanges();

            return Json("");
        }

        public JsonResult DelFile()
        {
            var ids = GetQueryString("IDs");

            if (string.IsNullOrEmpty(ids))
            {
                return Json("");
            }

            var arr = ids.Split(',');

            foreach (var id in arr)
            {
                var file = BusinessEntities.Set<T_Lawsuit_CaseFile>().Find(id);

                if (file != null)
                {
                    BusinessEntities.Set<T_Lawsuit_CaseFile>().Remove(file);
                }
            }
            BusinessEntities.SaveChanges();
            return Json("");
        }

    }
}
