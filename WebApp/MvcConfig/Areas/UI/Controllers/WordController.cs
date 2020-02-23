using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

namespace MvcConfig.Areas.UI.Controllers
{
    public class WordController : Controller
    {
        public FileResult Export(string tmplCode, string id)
        {
            if (string.IsNullOrEmpty(tmplCode))
                throw new Exception("缺少参数TmplCode");

            SQLHelper sqlHeper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dtWordTmpl = sqlHeper.ExecuteDataTable(string.Format("select TOP 1 * from S_UI_Word where Code='{0}' ORDER BY VersionNum DESC", tmplCode));
            if (dtWordTmpl.Rows.Count == 0)
                throw new Exception("Word导出定义不存在");


            #region 预览时的自动ID
            if (Request["TopID"] == "true" && string.IsNullOrEmpty(id))
            {
                SQLHelper sqlHelperWord = SQLHelper.CreateSqlHelper(dtWordTmpl.Rows[0]["ConnName"].ToString());
                string sql = string.Format("select top 1 ID from ({0}) a", dtWordTmpl.Rows[0]["SQL"]);

                if (Config.Constant.IsOracleDb)
                {
                    sql = string.Format("select ID from({0}) a where rownum=1", dtWordTmpl.Rows[0]["SQL"]);
                }

                try
                {
                    var obj = sqlHelperWord.ExecuteScalar(sql);
                    if (obj != null)
                        id = obj.ToString();
                }
                catch
                {
                    // Response.Write("SQL错误，或者表不存在！");
                    Response.Redirect("/MvcConfig/Error.html?errMsg=SQL错误，或者表不存在！");
                    Response.End();
                    return null;
                }

                if (string.IsNullOrEmpty(id))
                {
                    Response.Write("表中没有数据，无法导出Word。");
                    Response.End();
                    return null;
                }
            }

            #endregion

            if (string.IsNullOrEmpty(id))
                throw new Exception("缺少参数ID");

            //string tmplName = dtWordTmpl.Rows[0]["Code"].ToString() + ".docx";

            //string tempPath = Server.MapPath("/") + "WordTemplate/" + tmplName;




            UIFO uiFO = FormulaHelper.CreateFO<UIFO>();
            DataSet ds = uiFO.GetWordDataSource(tmplCode, id);

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

            string tempPath = HttpContext.Server.MapPath("/") + "WordTemplate/" + wordTmplRow["Code"].ToString() + "_" + versionNum + ".docx";
            //兼容以前没有版本时
            if (System.IO.File.Exists(tempPath) == false)
                tempPath = HttpContext.Server.MapPath("/") + "WordTemplate/" + wordTmplRow["Code"].ToString() + ".docx";

            AsposeWordExporter export = new AsposeWordExporter();
            byte[] result = null;
            if (Request["pdf"] == "true")
                result = export.ExportPDF(ds, tempPath, delegate (FieldMergingArgs e)
                {
                    if (e.FieldValue != null)
                    {
                        string strValues = e.FieldValue.ToString();
                        if (HtmlStringHelper.CheckHtml(strValues))
                        {
                            DocumentBuilder builder = new DocumentBuilder(e.Document);
                            builder.MoveToMergeField(e.FieldName);
                            builder.InsertHtml(strValues);
                        }
                    }
                });
            else
                result = export.ExportWord(ds, tempPath, delegate (FieldMergingArgs e)
                {
                    if (e.FieldValue != null)
                    {
                        string strValues = e.FieldValue.ToString();
                        if (HtmlStringHelper.CheckHtml(strValues))
                        {
                            DocumentBuilder builder = new DocumentBuilder(e.Document);
                            builder.MoveToMergeField(e.FieldName);
                            builder.InsertHtml(strValues);
                        }
                    }
                });
            MemoryStream docStream = new MemoryStream(result);

            string realFileName = dtWordTmpl.Rows[0]["Name"].ToString();
            if (wordTmplRow["WordNameTmpl"].ToString() != "")
            {
                realFileName = uiFO.ReplaceString(wordTmplRow["WordNameTmpl"].ToString(), ds.Tables[0].Rows[0]);
            }


            if (!String.IsNullOrEmpty(Request["filename"]))
            {
                realFileName = Request["filename"];
            }
            var explorerName = HttpContext.Request.Browser.Browser.ToUpper();
            if (explorerName == "IE" || explorerName == "INTERNETEXPLORER" || HttpContext.Request.UserAgent.ToString().IndexOf("rv:11") > 0)
            {
                realFileName = HttpUtility.UrlEncode(realFileName, System.Text.Encoding.UTF8);
                realFileName = realFileName.Replace("+", "%20");
            }

            if (Request["pdf"] == "true")
                return base.File(docStream.ToArray(), "application/pdf", realFileName + ".pdf");
            else
                return base.File(docStream.ToArray(), "application/msword", realFileName + ".doc");

        }


    }
}
