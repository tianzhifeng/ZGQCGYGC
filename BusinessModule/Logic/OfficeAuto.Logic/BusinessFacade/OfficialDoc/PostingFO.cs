using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Words;
using System.Web;
using System.IO;
using Aspose.Words.Drawing;
using OfficeAuto.Logic.Domain;
using Formula;
using Formula.Helper;
using Workflow.Logic.Domain;
using Base.Logic.Domain;

namespace OfficeAuto.Logic
{
    public class PostingFO
    {
        //已去除功能
        public static void AddPDFTask(string docID)
        {
            ////插入到PDFTask表
            //var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            //S_D_PDFTask model = baseEntities.Set<S_D_PDFTask>().FirstOrDefault(c => c.FileID == docID);
            //if (model == null)
            //{
            //    model = new S_D_PDFTask();
            //    model.ID = Guid.NewGuid().ToString();
            //    model.FileID = docID;
            //    model.FileType = SaveFormat.Doc.ToString();
            //    model.Status = "New";
            //    model.IsSplit = "0";
            //    baseEntities.Set<S_D_PDFTask>().Add(model);
            //    baseEntities.SaveChanges();
            //}
        }
        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="docID"></param>
        /// <returns></returns>
        public static string GetSWFFile(string docID)
        {

            //S_D_PDFTask model = FormulaHelper.GetEntities<BaseEntities>().Set<S_D_PDFTask>().FirstOrDefault(c => c.FileID == docID);
            //if (model != null && !string.IsNullOrEmpty(model.SWFFileID))
            //{
            //    string swfPath = System.Configuration.ConfigurationManager.AppSettings["OfficialDocSWF"] ?? "/MvcConfig/api/FileStoreAPI/";
            //    if (model.IsSplit == "1")
            //    {
            //        StringBuilder sb = new StringBuilder();
            //        sb.Append("{" + swfPath);
            //        sb.Append(Path.GetFileNameWithoutExtension(docID) + "[*,0].swf");
            //        sb.Append("," + model.PDFPageCount.ToString() + "}");
            //        return sb.ToString();
            //    }
            //    else
            //    {
            //        return swfPath + model.SWFFileID;

            //    }
            //}
            //else
            //{
            return string.Empty;
            //}
        }

        public static List<Dictionary<string, object>> GetTaskExec(string listData)
        {
            List<Dictionary<string, object>> rows = JsonHelper.ToObject<List<Dictionary<string, object>>>(listData);
            foreach (Dictionary<string, object> row in rows)
            {
                row["InsTaskID"] = row["TaskID"];
            }
            return rows;
        }

        public static void ResetStatus(string id)
        {
            var entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
            S_D_Posting model = entities.Set<S_D_Posting>().Find(id);
            if (model != null)
            {
                //model.Status = PostingStatus.Draft.ToString();
               // model.InsFlowID = null;
               // model.ExecutedSteps = null;
                entities.SaveChanges();
            }
        }

        public static void ChangeStatus(string id, string status)
        {
            var entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
            S_D_Posting model = entities.Set<S_D_Posting>().Find(id);
            if (model != null && !string.IsNullOrEmpty(status))
            {
                PostingStatus enumStatus = (PostingStatus)Enum.Parse(typeof(PostingStatus), status);
                if (enumStatus.ToString() != string.Empty)
                {
                    //model.Status = enumStatus.ToString();
                    //if (!string.IsNullOrEmpty(model.ExecutedSteps))
                    //{
                    //    string[] arr = model.ExecutedSteps.Split(',');
                    //    arr.RemoveWhere(c => c == enumStatus.ToString());
                    //    model.ExecutedSteps = string.Join(",", arr);
                    //}
                    entities.SaveChanges();
                }
            }
        }
    }
}
