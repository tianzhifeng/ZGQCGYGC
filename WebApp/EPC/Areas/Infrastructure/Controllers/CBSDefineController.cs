using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using MvcAdapter;
using EPC.Logic.Domain;
using EPC.Logic;
using Formula;
using System.Text;


namespace EPC.Areas.Infrastructure.Controllers
{
    public class CBSDefineController : InfrastructureController<S_T_CBSDefine>
    {
        protected override void BeforeSave(S_T_CBSDefine entity, bool isNew)
        {
            var root = entity.S_T_CBSNodeTemplate.FirstOrDefault(d => String.IsNullOrEmpty(d.ParentID));
            if (root == null)
            {
                root = new S_T_CBSNodeTemplate();
                root.ID = FormulaHelper.CreateGuid();
                root.ParentID = "";
                root.FullID = root.ID;
                root.CBSName = entity.Name;
                root.CBSCode = entity.Code;
                root.DefineType = "Static";
                root.NodeType = "Root";
                root.SortIndex = 0;
                root.CBSDefineID = entity.ID;
                root.CanAdd = "1";
                root.CanEdit = "0";
                root.CanDelete = "0";
                entity.S_T_CBSNodeTemplate.Add(root);
            }
        }

        public ActionResult Config()
        {
            var pbsEnum = EnumBaseHelper.GetEnumDef("Base.PBSType");
            var enumList = new List<Dictionary<string, object>>();

            var cbsdic = new Dictionary<string, object>();
            cbsdic.SetValue("value", "CBS");
            cbsdic.SetValue("text", "费用科目");
            cbsdic.SetValue("sortindex", 0);
            enumList.Add(cbsdic);

            foreach (var item in pbsEnum.EnumItem.ToList())
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", item.Code);
                dic.SetValue("text", item.Name);
                dic.SetValue("sortindex", item.SortIndex);
                enumList.Add(dic);
            }
            ViewBag.CBSNodeTypeEnum = JsonHelper.ToJson(enumList);
            return View();
        }

        public JsonResult GetDefineTree(String CBSDefineID)
        {
            var data = this.entities.Set<S_T_CBSNodeTemplate>().Where(d => d.CBSDefineID == CBSDefineID).OrderBy(d => d.SortIndex).ToList();
            return Json(data);
        }

        public JsonResult AddEmptyNode(string ParentID)
        {
            var parent = this.GetEntityByID<S_T_CBSNodeTemplate>(ParentID);
            if (parent == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的父节点，无法增加");
            }
            var emptyNode = new S_T_CBSNodeTemplate();
            emptyNode.CBSName = "新建节点";
            emptyNode.CBSCode = "NewCode";
            emptyNode.DefineType = SpaceDefineType.Static.ToString();
            emptyNode.NodeType = "CBS";
            emptyNode.ID = FormulaHelper.CreateGuid();
            parent.AddChild(emptyNode);
            this.entities.SaveChanges();
            return Json(emptyNode);
        }

        public JsonResult SaveCBSTemplate(string CBSInfo, string CBSDefineID)
        {
            var list = JsonHelper.ToList(CBSInfo);
            foreach (var item in list)
            {
                var node = this.GetEntityByID<S_T_CBSNodeTemplate>(item.GetValue("ID"));
                if (node == null) continue;
                this.UpdateEntity<S_T_CBSNodeTemplate>(node, item);
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult DeleteCBSTemplate(string CBSInfo)
        {
            var list = JsonHelper.ToList(CBSInfo);
            foreach (var item in list)
            {
                var id = item.GetValue("ID");
                if (String.IsNullOrEmpty(id)) continue;
                var node = this.GetEntityByID<S_T_CBSNodeTemplate>(id);
                if (node == null) continue;
                if (String.IsNullOrEmpty(node.ParentID)) throw new Formula.Exceptions.BusinessValidationException("项目节点不允许删除");
                node.Delete();
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult Push(string CbsDefineID)
        {
            var templateNodes = this.entities.Set<S_T_CBSNodeTemplate>().Where(c => c.CBSDefineID == CbsDefineID).ToList();
            var sqlBuilder = new StringBuilder();

            foreach (var item in templateNodes)
            {
                var sql = "update S_M_BidOffer_CBS set CanAdd='{0}',CanDelete='{1}',CanEdit='{2}' where CBSDefineID='{3}';";
                sqlBuilder.AppendLine(String.Format(sql
                    , String.IsNullOrEmpty(item.BidAdd) ? "0" : item.BidAdd
                    , String.IsNullOrEmpty(item.BidDelete) ? "0" : item.BidDelete
                    , String.IsNullOrEmpty(item.BidEdit) ? "0" : item.BidEdit
                    , item.ID));

                sql = "update S_I_BudgetInfo_Detail set CanAdd='{0}',CanDelete='{1}',CanEdit='{2}' where CBSDefineID='{3}';";
                sqlBuilder.AppendLine(String.Format(sql
                  , String.IsNullOrEmpty(item.CanAdd) ? "0" : item.CanAdd
                  , String.IsNullOrEmpty(item.CanDelete) ? "0" : item.CanDelete
                  , String.IsNullOrEmpty(item.CanEdit) ? "0" : item.CanEdit
                    , item.ID));

                if (item.DefineType == SpaceDefineType.Static.ToString())
                {
                    sql = "update S_I_CBS set Code='{0}' where CBSDefineID='{1}';";
                    sqlBuilder.AppendLine(String.Format(sql, item.CBSCode, item.ID));
                }
            }

            var epcDB = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            epcDB.ExecuteNonQuery(sqlBuilder.ToString());
            return Json("");
        }
    }
}
