using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAdapter;
using Config;
using System.Data;
using Formula;
using Base.Logic.Domain;
using Formula.Helper;
using Aspose.Words;
using System.Text.RegularExpressions;
using Base.Logic.BusinessFacade;
using Formula.Exceptions;
using Base.Logic.Model.UI.Form;
using System.Text;

namespace Base.Areas.UI.Controllers
{
    public class WordController : BaseController
    {
        #region 树和列表数据获取

        public JsonResult GetWordList(MvcAdapter.QueryBuilder qb)
        {
            if (!string.IsNullOrEmpty(Request["CategoryID"]))
            {
                string categoryID = Request["CategoryID"];
                var arr = entities.Set<S_M_Category>().Where(c => c.ID == categoryID || c.ParentID == categoryID).Select(c => c.ID).ToArray();
                string ids = string.Join(",", arr);
                qb.Add("CategoryID", QueryMethod.In, ids);
            }

            //word定义增加子公司权限
            if (System.Configuration.ConfigurationManager.AppSettings["CorpAuthEnabled"].ToLower() == "true"
                 && Request["listType"] == "subCompany")
                qb.Add("CompanyID", QueryMethod.Like, FormulaHelper.GetUserInfo().AdminCompanyID);

            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            //            string sql = @"
            //select * from(
            //select S_UI_Word.*,HasForm=case when S_UI_Form.Code is null then '0' else '1' end,S_UI_Form.ID FormID 
            //from S_UI_Word left join S_UI_Form on S_UI_Word.Code=S_UI_Form.Code where S_UI_Word.VersionEndDate is null
            //) tb1
            //";
            string sql = "select * from S_UI_Word where VersionEndDate is null";

            DataTable dt = sqlHelper.ExecuteDataTable(sql, qb);
            //是否存在word模板标识
            dt.Columns.Add("HasWord");
            foreach (DataRow row in dt.Rows)
            {
                var versionNum = row["VersionNum"].ToString();
                string tmplName = row["Code"].ToString() + ".docx";
                if (!string.IsNullOrEmpty(tmplName))
                {
                    string newTmplName = row["Code"].ToString() + "_" + versionNum + ".docx";
                    string path = HttpContext.Server.MapPath("/") + "WordTemplate/" + tmplName;
                    string newPath = HttpContext.Server.MapPath("/") + "WordTemplate/" + newTmplName;
                    if (System.IO.File.Exists(path) || System.IO.File.Exists(newPath))
                        row["HasWord"] = "1";
                }
            }

            //设置版本数量
            dt.Columns.Add("VersionCount");
            sql = string.Format("select Code,VersionCount=COUNT(1) from S_UI_Word where Code in('{0}') group by Code ", string.Join("','", dt.AsEnumerable().Select(c => c["Code"].ToString())));
            var dtVersionCount = sqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                row["VersionCount"] = dtVersionCount.AsEnumerable().SingleOrDefault(c => c["Code"].ToString() == row["Code"].ToString())["VersionCount"];
            }

            //设置是否有对应表单定义
            dt.Columns.Add("HasForm");
            sql = string.Format("select Code,VersionCount=COUNT(1) from S_UI_Form where Code in('{0}') group by Code ", string.Join("','", dt.AsEnumerable().Select(c => c["Code"].ToString())));
            var dtVersionFormCount = sqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                if (dtVersionFormCount.AsEnumerable().Count(c => c["Code"].ToString() == row["Code"].ToString()) > 0)
                    row["HasForm"] = 1;
                else
                    row["HasForm"] = 0;

            }


            GridData data = new GridData(dt);
            data.total = qb.TotolCount;
            return Json(data);

        }

        #endregion

        #region 基本信息

        public ActionResult List()
        {
            var result = Formula.AuthCodeHelper.CheckConfigFuncLimited();
            if (!result)
            {
                this.Response.Clear();
                this.Server.Transfer("/MvcConfig/ConfigDenied.html");
            }
            return View();
        }

        public ActionResult Edit()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        public JsonResult GetModel(string id)
        {
            return JsonGetModel<S_UI_Word>(id);
        }

        public JsonResult Save()
        {
            var entity = UpdateEntity<S_UI_Word>();
            if (!(entity.VersionNum > 1))
                if (entities.Set<S_UI_Word>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                    throw new Exception(string.Format("Word导出编号重复，Word导出名称“{0}”，Word导出编号：“{1}”", entity.Name, entity.Code));

            var category = entities.Set<S_M_Category>().SingleOrDefault(c => c.ID == entity.CategoryID);
            entity.ConnName = category.Code;
            if (entity._state == EntityStatus.added.ToString())
            {
                entity.Items = "[]";
            }

            if (entity.VersionStartDate == null)
            {
                entity.VersionStartDate = entity.CreateTime;
                entity.VersionNum = 1;
            }

            entities.SaveChanges();
            return Json(new { ID = entity.ID });
        }

        public JsonResult Delete(string listIDs)
        {
            var lists = entities.Set<S_UI_Word>().Where(c => listIDs.Contains(c.ID));
            var codeList = lists.Select(c => c.Code).ToArray();
            entities.Set<S_UI_Word>().Delete(c => codeList.Contains(c.Code));
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 字段信息

        public ActionResult SettingsSubTable()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>().Where(c => !string.IsNullOrEmpty(c.ParentID)).Select(c => new { value = c.ID, text = c.Name }));
            return View();
        }

        public JsonResult GetItemList(string wordID)
        {
            return Json(entities.Set<S_UI_Word>().SingleOrDefault(c => c.ID == wordID).Items);
        }

        public JsonResult SaveItemList(string wordID, string itemList)
        {
            var word = entities.Set<S_UI_Word>().SingleOrDefault(c => c.ID == wordID);
            word.Items = itemList;
            var user = FormulaHelper.GetUserInfo();
            word.ModifyUserID = user.UserID;
            word.ModifyUserName = user.UserName;
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region 枚举选择

        public JsonResult GetEnumList(QueryBuilder qb)
        {
            var result = entities.Set<S_M_EnumDef>().WhereToGridData(qb);
            return Json(result);
        }

        #endregion

        #region 创建新版本

        public ActionResult VersionEdit()
        {
            ViewBag.EnumCategory = JsonHelper.ToJson(entities.Set<S_M_Category>());
            return View();
        }

        public JsonResult GetVersion(string wordID)
        {
            var wordInfo = entities.Set<S_UI_Word>().SingleOrDefault(c => c.ID == wordID);
            return Json(wordInfo);
        }

        public JsonResult SaveVersion(string wordID, string formData)
        {
            string newFormID = FormulaHelper.CreateGuid();

            var wordInfo = entities.Set<S_UI_Word>().SingleOrDefault(c => c.ID == wordID);

            var newWordInfo = new S_UI_Word();
            FormulaHelper.UpdateModel(newWordInfo, wordInfo);

            newWordInfo.ID = newFormID;
            newWordInfo.CreateTime = DateTime.Now;
            newWordInfo.ModifyTime = DateTime.Now;
            newWordInfo.VersionNum = entities.Set<S_UI_Word>().Count(c => c.Code == wordInfo.Code) + 1;
            //if (wordInfo.VersionNum != null)
            //    newWordInfo.VersionNum = wordInfo.VersionNum + 1;
            //else
            //    newWordInfo.VersionNum = entities.Set<S_UI_Word>().Count(c => c.Code == wordInfo.Code) + 1;

            var dic = JsonHelper.ToObject(formData);

            if (dic["VersionStartDate"].ToString() != "")
            {
                newWordInfo.VersionStartDate = DateTime.Parse(dic["VersionStartDate"].ToString());
                wordInfo.VersionEndDate = newWordInfo.VersionStartDate;
            }
            else
                throw new BusinessException("必须输入版本开始时间！");
            newWordInfo.VersionDesc = dic["VersionDesc"].ToString();

            entities.Set<S_UI_Word>().Add(newWordInfo);

            entities.SaveChanges();
            return Json(newWordInfo);
        }

        public JsonResult GetVersionList(string code, QueryBuilder qb)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var data = sqlHelper.ExecuteGridData(string.Format("select ID,Code,Name,ConnName,SQL, TableNames,ModifyTime,CategoryID,VersionNum,VersionStartDate,VersionEndDate from S_UI_Word where Code='{0}'", code), qb);
            return Json(data);
        }

        public JsonResult DeleteVersion(string code)
        {
            var wordInfoList = entities.Set<S_UI_Word>().Where(c => c.Code == code).OrderBy(c => c.VersionStartDate).ToList();
            if (wordInfoList.Count == 1)
                throw new BusinessException("最后一个不能删除！");
            var wordInfo = wordInfoList.Last();
            entities.Set<S_UI_Word>().Remove(wordInfo);
            wordInfoList.Remove(wordInfo);
            var newWordInfo = wordInfoList.Last();
            newWordInfo.VersionEndDate = null;
            entities.SaveChanges();
            return Json("");
        }

        #endregion

        #region  授权到子公司

        public JsonResult SetCompanyAuth(string objIds, string orgIds, string orgNames)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = "update S_UI_Word set CompanyID='{0}',CompanyName='{1}' where ID in('{2}')";
            sql = string.Format(sql, orgIds, orgNames, objIds.Replace(",", "','"));
            sqlHelper.ExecuteNonQuery(sql);
            return Json("");
        }

        #endregion

        public JsonResult UploadWord()
        {
            if (Request.Files.Count > 0)
            {
                string code = Request["TmplCode"];
                var define = entities.Set<S_UI_Word>().Where(c => c.Code == code).OrderByDescending(c => c.VersionNum).FirstOrDefault();
                int? versionNum = 1;
                if (define.VersionNum != null)
                    versionNum = define.VersionNum;

                string fileFullName = HttpContext.Server.MapPath("/") + "WordTemplate/" + define.Code + "_" + versionNum + ".docx";

                if (System.IO.File.Exists(fileFullName))
                    System.IO.File.Delete(fileFullName);

                Request.Files[0].SaveAs(fileFullName);

                var user = FormulaHelper.GetUserInfo();
                define.ModifyUserID = user.UserID;
                define.ModifyUserName = user.UserName;
                define.ModifyTime = DateTime.Now;
                entities.SaveChanges();
            }
            return Json("");
        }

        public FileResult DownloadWord(string tmplCode)
        {
            var define = entities.Set<S_UI_Word>().Where(c => c.Code == tmplCode).OrderByDescending(c => c.VersionStartDate).FirstOrDefault();
            if (define == null)
                throw new Exception("word模板没定义不能下载!");
            var versionNum = define.VersionNum.ToString();
            string filePath = HttpContext.Server.MapPath("/") + "WordTemplate/" + define.Code + "_" + versionNum + ".docx";
            string newFilePath = HttpContext.Server.MapPath("/") + "WordTemplate/" + define.Code + ".docx"; ;
            if (!System.IO.File.Exists(filePath) && System.IO.File.Exists(newFilePath))
                filePath = newFilePath;

            return File(filePath, "application/msword", define.Code + ".docx");
        }

        public JsonResult CreateWordTmpl(string TmplCode)
        {

            foreach (var code in TmplCode.Split(','))
            {
                var form = entities.Set<S_UI_Form>().Where(c => c.Code == code).OrderByDescending(c => c.VersionStartDate).FirstOrDefault();
                if (form == null)
                    throw new BusinessException(string.Format("表单定义不存在!"));
                if (string.IsNullOrWhiteSpace(form.Layout))
                    throw new BusinessException(string.Format("表单定义尚未完成"));
                var word = entities.Set<S_UI_Word>().Where(c => c.Code == code).OrderByDescending(c => c.VersionStartDate).FirstOrDefault();
                //if (word != null)
                //    throw new BusinessException(string.Format("Word导出定义已经存在：{0}", code));
                if (word != null)
                    continue;


                word = new S_UI_Word();
                word.ID = FormulaHelper.CreateGuid();
                word.Code = form.Code;
                word.Name = form.Name;
                word.ConnName = form.ConnName;
                word.TableNames = form.TableName;
                word.CategoryID = form.CategoryID;
                word.Description = "FormWord"; //根据此值判断是否由表单定义生成的Word导出
                word.SQL = string.Format("select * from {0}", form.TableName);

                word.VersionNum = form.VersionNum;
                word.VersionStartDate = form.VersionStartDate;
                word.VersionDesc = form.VersionDesc;

                entities.Set<S_UI_Word>().Add(word);

                var doc = CreateDoc(code);

                string path = HttpContext.Server.MapPath("/WordTemplate");
                int? versionNum = 1;
                if (form.VersionNum != null)
                    versionNum = form.VersionNum;
                path += "\\" + form.Code + "_" + versionNum + ".docx";
                doc.Save(path, Aspose.Words.SaveFormat.Docx);
                entities.SaveChanges();
            }

            return Json("");
        }

        public FileResult DownloadNewDoc(string tmplCode)
        {
            var doc = CreateDoc(tmplCode);
            System.IO.Stream stream = new System.IO.MemoryStream();
            doc.Save(stream, Aspose.Words.SaveFormat.Docx);
            stream.Position = 0;
            return File(stream, "application/msword", tmplCode + ".docx");
        }

        private Aspose.Words.Document CreateDoc(string code)
        {
            var baseEntities = FormulaHelper.GetEntities<BaseEntities>();
            var uiForm = baseEntities.Set<S_UI_Form>().Where(c => c.Code == code).OrderByDescending(c => c.VersionStartDate).FirstOrDefault();
            var formItems = JsonHelper.ToObject<List<Dictionary<string, string>>>(uiForm.Items);

            Aspose.Words.Document doc = new Aspose.Words.Document();
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            string layout = uiForm.Layout;

            Regex reg = new Regex("\\{[^\\{]*\\}", RegexOptions.IgnorePatternWhitespace);

            #region 处理layout中的子表

            layout = reg.Replace(layout, (Match m) =>
            {
                string text = m.Value.Trim('{', '}', ' ');
                var item = formItems.SingleOrDefault(c => c["Name"] == text);

                if (item == null)
                {
                    return m.Value;
                }
                else if (item["ItemType"] == "AuditSign")
                {
                    string tmplName = "auditSignTmpl";
                    string signTitle = "签字";
                    string width = "100%";
                    if (item.ContainsKey("Settings") && item["Settings"] != "")
                    {
                        var _dic = JsonHelper.ToObject<Dictionary<string, string>>(item["Settings"]);
                        if (_dic.ContainsKey("tmplName") && _dic["tmplName"] != "")
                            tmplName = _dic["tmplName"];
                        if (_dic.ContainsKey("signTitle") && _dic["signTitle"] != "")
                            signTitle = _dic["signTitle"];
                        if (_dic.ContainsKey("width") && _dic["width"] != "")
                            width = _dic["width"];
                    }

                    if (tmplName == "auditSignTmpl")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("{TableStart:" + item["Code"] + "}");
                        sb.Append("<span>意见：{Field:SignComment}</span>");
                        sb.Append("<div>" + signTitle + "：{Field:ExecUserID}日期：{Field:SignTime}</div>");
                        sb.Append("{TableEnd:" + item["Code"] + "}");
                        return sb.ToString();
                    }
                    else if (tmplName == "auditSignSingleTmpl")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("<table style=\"width:" + width + "\" cellspacing=\"0\" cellpadding=\"2\" border=\"0\">");
                        sb.AppendLine("<tr><td>");
                        sb.AppendLine(signTitle + "{" + item["Code"] + ":ExecUserID}");
                        sb.AppendLine("</td></tr><tr><td>");
                        sb.AppendLine("日期：{" + item["Code"] + ":SignTime}");
                        sb.AppendLine("</td></tr></table>");
                        return sb.ToString();
                    }
                    else
                    {
                        return "";
                    }

                }
                else if (item["ItemType"] == "SubTable")
                {
                    var _dic = JsonHelper.ToObject(item["Settings"]);
                    var subItemList = JsonHelper.ToObject<List<FormItem>>(_dic["listData"].ToString());
                    if (subItemList.Count == 0)
                        return m.Value;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<table style=\"width:100%\" bordercolor=\"#000000\" cellspacing=\"0\" cellpadding=\"2\" border=\"1\">");
                    sb.AppendLine("<tr>");
                    foreach (var subItem in subItemList)
                    {
                        sb.AppendFormat("<td width=\"{1}\">{0}</td>", subItem.Name, subItem.width);
                    }
                    sb.AppendLine("</tr>");
                    sb.AppendLine("<tr>");
                    for (int i = 0; i < subItemList.Count; i++)
                    {
                        var subItem = subItemList[i];

                        sb.Append("<td>");

                        if (i == 0)//子表开始
                            sb.Append("{TableStart:" + item["Code"] + "}");

                        sb.Append("{Field:" + subItem.Code + "}");

                        if (i == subItemList.Count - 1) //子表结束
                            sb.Append("{TableEnd:" + item["Code"] + "}");

                        sb.Append("</td>");
                    }
                    sb.AppendLine("</tr>");
                    sb.AppendLine("</table>");
                    return sb.ToString();
                }
                else
                {
                    return m.Value;
                }
            });


            #endregion

            #region 操作列表


            if (System.Configuration.ConfigurationManager.AppSettings["Flow_WordExportComment"] == "True")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<table style=\"width:100%\" bordercolor=\"#000000\" cellspacing=\"0\" cellpadding=\"2\" border=\"1\">");
                sb.Append(@"
<tr>
<td width='100'>环节名称</td>
<td width='150'>接收人部门</td>
<td width='60'>审批人</td>
<td width='100'>操作</td>
<td width='110'>审批时间</td>
<td>审批意见</td>
</tr>");
                sb.AppendLine("<tr>");
                sb.Append(@"
<td width='100'>{TableStart:FlowComments}{Field:StepName}</td>
<td width='150'>{Field:TaskUserDept}</td>
<td width='60'>{Field:ExecUserName}</td>
<td width='100'>{Field:ExecRoutingName}</td>
<td width='110'>{Field:ExecTime}</td>
<td>{Field:ExecComment}{TableEnd:FlowComments}</td>
");

                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");

                layout += sb.ToString();
            }

            #endregion

            layout = reg.Replace(layout, (match) =>
            {
                if (match.Value.Contains("TableStart:") || match.Value.Contains("TableEnd:"))
                    return match.Value;
                else
                    return "<span>" + match.Value + "</span>";
            });

            builder.InsertHtml(layout);

            doc.Range.Replace(reg, new ReplaceToField(formItems), false);
            return doc;
        }
    }


    public class ReplaceToField : IReplacingCallback
    {
        public List<Dictionary<string, string>> formItems = null;
        public ReplaceToField(List<Dictionary<string, string>> formItems)
        {
            this.formItems = formItems;
        }
        public ReplaceAction Replacing(ReplacingArgs e)
        {
            //获取当前节点            
            var node = e.MatchNode;
            //获取当前文档           
            Document doc = node.Document as Document;
            DocumentBuilder builder = new DocumentBuilder(doc);
            //将光标移动到指定节点      
            builder.MoveTo(node);
            var text = node.GetText().Trim('{', '}', ' ');

            var item = formItems.SingleOrDefault(c => c["Name"] == text);

            if (text.StartsWith("S:"))   //子表开始
            {
                builder.InsertField(@"MERGEFIELD " + "TableStart:" + text.Split(':')[1] + @" \* MERGEFORMAT");
                builder.InsertField(@"MERGEFIELD " + text.Split(':')[2] + @" \* MERGEFORMAT");
            }
            else if (text.StartsWith("E:"))//子表结束
            {
                builder.InsertField(@"MERGEFIELD " + text.Split(':')[2] + @" \* MERGEFORMAT");
                builder.InsertField(@"MERGEFIELD " + "TableEnd:" + text.Split(':')[1] + @" \* MERGEFORMAT");
            }
            else if (text.StartsWith("F:")) //子表字段
            {
                builder.InsertField(@"MERGEFIELD " + text.Split(':')[1] + @" \* MERGEFORMAT");

            }
            else if (text.StartsWith("TableStart:") || text.StartsWith("TableEnd:")) //子表开头和结束
            {
                builder.InsertField(@"MERGEFIELD " + text + @" \* MERGEFORMAT");
            }
            else if (text.StartsWith("Field:"))//子表字段
            {
                builder.InsertField(@"MERGEFIELD " + text.Split(':')[1] + @" \* MERGEFORMAT");
            }
            else if (item != null) //表单字段
            {
                if (item["ItemType"] == "ButtonEdit")
                    builder.InsertField(@"MERGEFIELD " + item["Code"] + "Name" + @" \* MERGEFORMAT");
                else
                    builder.InsertField(@"MERGEFIELD " + item["Code"] + @" \* MERGEFORMAT");
            }
            else
            {
                builder.InsertField(@"MERGEFIELD " + text + @" \* MERGEFORMAT");
            }

            return ReplaceAction.Replace;
        }
    }
}
