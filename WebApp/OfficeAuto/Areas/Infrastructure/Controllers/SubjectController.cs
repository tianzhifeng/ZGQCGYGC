using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using OfficeAuto.Logic.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeAuto.Areas.Infrastructure.Controllers
{
    public class SubjectController : OfficeAutoController
    {

        protected string TableName
        {
            get
            {
                return "S_EP_DefineSubject";
            }
        }

        public JsonResult GetSubjectTree(QueryBuilder qb)
        {
            string sql = "select * from {0}  WITH(NOLOCK) ";
            qb.PageSize = 0;

            DataTable dtTmpl = this.SqlHelper.ExecuteDataTable(String.Format("select * from {0}  WITH(NOLOCK) where 1!=1", this.TableName));
            foreach (string key in Request.QueryString.Keys)
            {
                if (string.IsNullOrEmpty(key))
                    continue;
                if ("ID,FullID,FULLID,TmplCode,IsPreView,_winid,_t".Split(',').Contains(key) || key.StartsWith("$"))
                    continue;
                if (dtTmpl.Columns.Contains(key))
                    qb.Add(key, QueryMethod.In, Request[key]); ;
            }
            
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, this.TableName), qb);
            return Json(dt);
        }


        public ActionResult TreeList()
        {
            var maxLevel = this.SqlHelper.ExecuteScalar(String.Format("select isnull(Max(Level),0) from {0} WITH(NOLOCK)", this.TableName));
            ViewBag.ExpandLevel = maxLevel;
            var root = this.SqlHelper.ExecuteDataTable(String.Format(" select ID from {0}  WITH(NOLOCK) where ParentID is null or ParentID=''", this.TableName));
            if (root.Rows.Count == 0)
            {
                var rootDic = new Dictionary<string, object>();
                rootDic.SetValue("ID", FormulaHelper.CreateGuid());
                rootDic.SetValue("ParentID", string.Empty);
                rootDic.SetValue("FullID", rootDic.GetValue("ID"));
                rootDic.SetValue("Level", rootDic.GetValue("FullID").Split('.').Length);
                rootDic.SetValue("Name", "根科目");
                rootDic.SetValue("NodeType", "Root");
                rootDic.SetValue("SortIndex", "0");
                rootDic.InsertDB(this.SqlHelper, this.TableName, rootDic.GetValue("ID"));
            }
            return View();
        }

        public JsonResult AddNode(string NodeID, string AddMode)
        {
            var result = new Dictionary<string, object>();
            Action action = () =>
            {
                var node = this.GetDataDicByID(this.TableName, NodeID);
                if (node == null)
                    throw new Formula.Exceptions.BusinessValidationException("没有找到ID为【" + NodeID + "】的科目信息，无法新增科目");
                var subjectNode = new S_EP_DefineSubject(node);
                if (AddMode.ToLower() == "after")
                {
                    result = subjectNode.AddBrotherNode().ModelDic;
                }
                else
                {
                    result = subjectNode.AddChildNode().ModelDic;
                }
            };
            this.ExecuteAction(action);
            return Json(result);
        }

        public JsonResult SaveNodes(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            Action action = () =>
            {
                foreach (var item in list)
                {
                    item.SetValue("ModifyDate", DateTime.Now);
                    item.SetValue("ModifyTime", DateTime.Now);
                    item.SetValue("ModifyUser", this.CurrentUserInfo.UserName);
                    item.SetValue("ModifyUserName", this.CurrentUserInfo.UserName);
                    item.SetValue("ModifyUserID", this.CurrentUserInfo.UserID);
                    item.UpdateDB(this.SqlHelper, this.TableName, item.GetValue("ID"));
                }
                #region 重新刷新FULLCODE
                var dt = this.SqlHelper.ExecuteDataTable("SELECT * FROM S_EP_DefineSubject WITH(NOLOCK) ORDER BY FULLID");
                foreach (DataRow row in dt.Rows)
                {
                    if (row["FullID"].ToString() == row["ID"].ToString())
                    {
                        //根节点 FULLCODE 等于CODE本身
                        this.SqlHelper.ExecuteNonQuery(String.Format("UPDATE S_EP_DefineSubject SET FULLCODE='{0}' WHERE ID='{1}'", row["Code"], row["ID"]));
                    }
                    else
                    {
                        var ancestors = dt.Select("'" + row["FULLID"].ToString() + "' LIKE FULLID+'%'", "FULLID");
                        var fullCode = "";
                        foreach (DataRow ancestor in ancestors)
                        {
                            fullCode += ancestor["Code"].ToString() + ".";
                        }
                        this.SqlHelper.ExecuteNonQuery(String.Format("UPDATE S_EP_DefineSubject SET FULLCODE='{0}' WHERE ID='{1}'", fullCode.TrimEnd('.'), row["ID"]));
                    }
                }
                #endregion
                
            };
                        this.ExecuteAction(action);
            return Json("");
        }

        public JsonResult DeleteNodes(string Ids)
        {
            Action action = () =>
            {
                var nodeIDs = Ids.Split(',');
                foreach (var id in nodeIDs)
                {
                    var node = this.GetDataDicByID(this.TableName, id);
                    if (node == null) continue;
                    var subjectNode = new S_EP_DefineSubject(node);
                    subjectNode.Delete();
                }
            };
            this.ExecuteAction(action);
            return Json("");
        }

        protected Dictionary<string, object> GetDataDicByID(string tableName, string id, bool withNolock = false)
        {
            string sql = withNolock ? String.Format("select * from {0} {2} where ID='{1}'", tableName, id, "with(nolock)")
                : String.Format("select * from {0} {2} where ID='{1}'", tableName, id, "");
            var dt = this.SqlHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return FormulaHelper.DataRowToDic(dt.Rows[0]);
            }
        }

    }
}
