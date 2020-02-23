using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcConfig.Areas.UI.Controllers;

using System.IO;
using Config;
using System.Data;
using Aspose.Words;
using Aspose.Words.Saving;
using Formula;
using Base.Logic.Domain;
using Formula.Helper;
using System.Text;
using Formula.ImportExport;
using Base.Logic.BusinessFacade;
using Aspose.Words.Drawing;
using Aspose.Words.Reporting;
using System.Net.Http.Headers;
using System.Web;

namespace MvcConfig.Controllers
{
    public class FormToWordAPIController : ApiController
    {
        /// <summary>
        /// Form to Word
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Export(string id)
        {
            string tmplCode = Request.RequestUri.ParseQueryString().Get("tmplCode");
            if (string.IsNullOrEmpty(tmplCode))
                throw new Exception("缺少参数TmplCode");

            SQLHelper sqlHeper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dtWordTmpl = sqlHeper.ExecuteDataTable(string.Format("select * from S_UI_Word where Code='{0}'", tmplCode));
            if (dtWordTmpl.Rows.Count == 0)
                throw new Exception("Word导出定义不存在");

            if (string.IsNullOrEmpty(id))
                throw new Exception("缺少参数ID");

            //string tmplName = dtWordTmpl.Rows[0]["Code"].ToString() + ".docx";

            ////var path = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            ////string tempPath = path.Substring(0, path.LastIndexOf('\\') + 1) + "WordTemplate/" + tmplName;// Server.MapPath("/") +

            //string tempPath = System.Web.HttpContext.Current.Server.MapPath("/") + "WordTemplate/" + tmplName;


            //word历史版本功能

            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            DataSet ds = uiFO.GetWordDataSource(tmplCode, id, false);//客户端转图访问时，没有当前用户，防止报错

            DataRow wordTmplRow = dtWordTmpl.Rows[0];
            DateTime date;
            if (ds.Tables[0].Columns.Contains("CreateTime"))
                date = DateTime.Parse(ds.Tables[0].Rows[0]["CreateTime"].ToString());
            else
                date = DateTime.Parse(ds.Tables[0].Rows[0]["CreateDate"].ToString());

            foreach (DataRow row in dtWordTmpl.Rows)
            {
                var _startDate = DateTime.Parse(row["VersionStartDate"].ToString());
                var _endDate = DateTime.MaxValue;
                if (row["VersionEndDate"].ToString() != "")
                    _endDate = DateTime.Parse(row["VersionEndDate"].ToString());

                if (date > _startDate && date < _endDate)
                {
                    wordTmplRow = row;
                    break;
                }
            }
            int? versionNum = 1;
            if (wordTmplRow["VersionNum"].ToString() != "")
                versionNum = int.Parse(wordTmplRow["VersionNum"].ToString());

            string tempPath = HttpContext.Current.Server.MapPath("/") + "WordTemplate/" + wordTmplRow["Code"].ToString() + "_" + versionNum + ".docx";
            //兼容以前没有版本时
            if (System.IO.File.Exists(tempPath) == false)
                tempPath = HttpContext.Current.Server.MapPath("/") + "WordTemplate/" + wordTmplRow["Code"].ToString() + ".docx";


     

            AsposeWordExporter export = new AsposeWordExporter();
            byte[] bytesArray = export.ExportWord(ds, tempPath, delegate (FieldMergingArgs e) {

                string strValues =e.FieldValue==null?"":e.FieldValue.ToString();
            if (HtmlStringHelper.CheckHtml(strValues))
                {
                    DocumentBuilder builder = new DocumentBuilder(e.Document);
                    builder.MoveToMergeField(e.FieldName);
                    builder.InsertHtml(strValues);
                }
            });
            string fileName = dtWordTmpl.Rows[0]["Name"].ToString();
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(bytesArray);
            result.Content.Headers.ContentLength = bytesArray.Length;
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fileName;

            return result;
        }
    }
}
