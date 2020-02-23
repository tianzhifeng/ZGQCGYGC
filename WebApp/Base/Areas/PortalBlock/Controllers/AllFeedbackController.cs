using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Base.Logic.Domain;
using System.Web.Mvc;
using Newtonsoft.Json;
using Formula.ImportExport;

namespace Base.Areas.PortalBlock.Controllers
{
    public class AllFeedbackController : BaseController<S_H_AllFeedback>
    {
        #region Excel 批量导入
        public JsonResult VaildExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var orgs = entities.Set<S_A_Org>().ToList();
            var errors = excelData.Vaildate(e =>
            {
                if (false)
                {
                    
                }
            });

            return Json(errors);
        }

        public JsonResult BatchSave()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<S_H_AllFeedback>>(tempdata["data"]);

            entities.Configuration.AutoDetectChangesEnabled = false;

            var prjName = list.FirstOrDefault().ProjectName;

            var ids = list.Select(c => c.ID);

            entities.Set<S_H_AllFeedback>().Delete(c => ids.Contains(c.ID));

            foreach (var item in list)
            {
                entities.Set<S_H_AllFeedback>().Add(item);
            }            
           
            try
            {
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                Formula.LogWriter.Error(ex);
                throw ex;
            }
            entities.Configuration.AutoDetectChangesEnabled = true;
            return Json("Success");
        }

        #endregion
    }
}
