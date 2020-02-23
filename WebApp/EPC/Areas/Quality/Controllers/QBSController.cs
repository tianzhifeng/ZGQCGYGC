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

namespace EPC.Areas.Quality.Controllers
{
    public class QBSController : EPCController<S_Q_QBS_Version_QBSData>
    {
        public ActionResult TreeList()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");

            var version = engineeringInfo.S_Q_QBS_Version.OrderByDescending(c => c.ID).FirstOrDefault();
            bool flowEnd = true; bool isFirst = false;
            if (version == null)
            {
                //此时如果要编辑，则直接做升版BOM操作
                flowEnd = true;
                ViewBag.VersionID = "";
                ViewBag.FlowPhase = "";
                ViewBag.VersionNo = "0";
                isFirst = true;
            }
            else
            {
                if (version.FlowPhase != "End")
                    flowEnd = false;
                ViewBag.FlowPhase = version.FlowPhase;
                ViewBag.VersionID = version.ID;
                ViewBag.VersionNo = version.VersionNumber;
            }
            ViewBag.FlowEnd = flowEnd;
            ViewBag.EngineeringInfoID = engineeringInfoID;
            var pushCount = engineeringInfo.S_Q_QBS_Version.Count(c => c.FlowPhase == "End");
            ViewBag.PushCount = pushCount;
            ViewBag.First = isFirst;
            //默认展现所有（树节点展开至PBS定义的最下层）

            var list = engineeringInfo.Mode.S_C_QBSStruct.OrderBy(c => c.FullID).ToList();
            var enumNodeType = new List<Dictionary<string, object>>();
            for (int i = 0; i < list.Count - 1; i++)
            {
                var item = list[i];
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", i + 1);
                dic.SetValue("text", item.Name);
                dic.SetValue("sortindex", item.SortIndex);
                enumNodeType.Add(dic);
                if (i == list.Count - 2)
                {
                    ViewBag.ExpandLevel = i + 1;
                }
            }
            ViewBag.NodeTypeEnum = JsonHelper.ToJson(enumNodeType);
            return View();
        }

        public ActionResult TreeListSelector()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");

            var version = engineeringInfo.S_Q_QBS_Version.Where(a=>a.FlowPhase == "End").OrderByDescending(c => c.ID).FirstOrDefault();
            if (version != null)
            {
                ViewBag.VersionID = version.ID;
                ViewBag.VersionNo = version.VersionNumber;
            }

            ViewBag.EngineeringInfoID = engineeringInfoID;
            var list = engineeringInfo.Mode.S_C_QBSStruct.OrderBy(c => c.FullID).ToList();
            var enumNodeType = new List<Dictionary<string, object>>();
            for (int i = 0; i < list.Count - 1; i++)
            {
                var item = list[i];
                var dic = new Dictionary<string, object>();
                dic.SetValue("value", i + 1);
                dic.SetValue("nodeTypeName", item.NodeType);
                dic.SetValue("text", item.Name);
                dic.SetValue("sortindex", item.SortIndex);
                enumNodeType.Add(dic);
                if (i == list.Count - 2)
                {
                    ViewBag.ExpandLevel = i + 1;
                }
            }
            ViewBag.NodeTypeEnum = JsonHelper.ToJson(enumNodeType);
            return View();
        }

        public JsonResult GetVersionTreeList(QueryBuilder qb, string VersionID, string ShowType)
        {
            var result = new List<Dictionary<string, object>>();
            var version = this.GetEntityByID<S_Q_QBS_Version>(VersionID);
            if (version == null) return Json(result);
            var data = this.entities.Set<S_I_Section>().Where(d => d.EngineeringInfoID == version.EngineeringInfoID).OrderBy(c => c.SortIndex).ToList();

            var versionList = new List<S_Q_QBS_Version_QBSData>();
            qb.SortField = "SortIndex"; qb.SortOrder = "asc";
            qb.PageSize = 0;
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                qb.Add("ModifyState", QueryMethod.NotEqual, BomVersionModifyState.Normal.ToString());
                versionList = this.entities.Set<S_Q_QBS_Version_QBSData>().Where(c => c.VersionID == version.ID).Where(qb).ToList();

            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不体现差异
                qb.Add("ModifyState", QueryMethod.NotEqual, BomVersionModifyState.Remove.ToString());
                versionList = this.entities.Set<S_Q_QBS_Version_QBSData>().Where(c => c.VersionID == version.ID).Where(qb).ToList();
            }
            else
            {
                //显示全部数据，体现差异
                versionList = this.entities.Set<S_Q_QBS_Version_QBSData>().Where(c => c.VersionID == version.ID).Where(qb).ToList();

            }
            var rootStruct = version.S_I_Engineering.Mode.S_C_QBSStruct.FirstOrDefault(c => c.NodeType == "Root");
            if (rootStruct == null) throw new Formula.Exceptions.BusinessValidationException("QBS结构定义中没有根节点");
            foreach (var item in data)
            {
                var dic = FormulaHelper.ModelToDic<S_I_Section>(item);
                var details = versionList.Where(c => c.SectionID == item.ID).ToList();
                dic.SetValue("QBSID", item.ID);
                dic.SetValue("NodeType", "Root");
                dic.SetValue("StructNodeID", rootStruct.ID);
                dic.SetValue("FullID", item.ID);
                dic.SetValue("ParentID", "");
                dic.SetValue("CanAdd", true);
                dic.SetValue("CanParentAdd", false);
                dic.SetValue("CanDelete", false);
                result.Add(dic);
                foreach (var detail in details)
                {
                    var detailDic = FormulaHelper.ModelToDic<S_Q_QBS_Version_QBSData>(detail);
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

        public JsonResult GetLastDetailInfo(string ID)
        {
            var detail = this.GetEntityByID<S_Q_QBS_Version_QBSData>(ID);
            var result = new Dictionary<string, object>();
            if (detail != null)
            {

                var lastVersion = this.entities.Set<S_Q_QBS_Version>().Where(d => d.EngineeringInfoID == detail.EngineeringInfoID
                    && d.FlowPhase == "End" && d.ID != detail.VersionID).OrderByDescending(c => c.ID).FirstOrDefault();
                if (lastVersion != null)
                {
                    var lastDetail = lastVersion.S_Q_QBS_Version_QBSData.FirstOrDefault(c => c.QBSID == detail.QBSID);
                    if (lastDetail != null)
                    {
                        result = FormulaHelper.ModelToDic<S_Q_QBS_Version_QBSData>(lastDetail);
                    }
                }
            }
            return Json(result);
        }

        public JsonResult UpgradQBS(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null)
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程信息，无法进行结构策划");
            engineeringInfo.UpGradeQBSVersion();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult Revert(string VersionID)
        {
            var version = this.entities.Set<S_Q_QBS_Version>().FirstOrDefault(c => c.ID == VersionID);
            if (version == null)
                throw new Formula.Exceptions.BusinessValidationException("没有可编辑的版本，无法进行撤销操作");
            if (version.FlowPhase == "End") { throw new Formula.Exceptions.BusinessValidationException("已经发布的版本不能进行撤销操作"); }
            this.entities.Set<S_Q_QBS_Version>().Remove(version);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddVersionQBS(string NodeID, string NodeType, string VersionID, string AddMode)
        {
            var version = this.GetEntityByID<S_Q_QBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的版本信息，请确认版本信息是否存在");
            var result = new Dictionary<string, object>();
            if (AddMode == "After")
            {
                if (NodeType.ToLower() == "root") throw new Formula.Exceptions.BusinessValidationException("分部分项策划不能新增标段");
                var node = this.GetEntityByID<S_Q_QBS_Version_QBSData>(NodeID);
                if (node == null) throw new Formula.Exceptions.BusinessValidationException("未能找到选中的节点，无法新增");
                var child = node.AddEmptyBrother();
                result = FormulaHelper.ModelToDic<S_Q_QBS_Version_QBSData>(child);
            }
            else if (AddMode == "AddChild")
            {
                if (NodeType.ToLower() == "root")
                {
                    var section = this.GetEntityByID<S_I_Section>(NodeID);
                    var child = section.AddEmptyVersionNode(version);
                    result = FormulaHelper.ModelToDic<S_Q_QBS_Version_QBSData>(child);
                }
                else
                {
                    var node = this.GetEntityByID<S_Q_QBS_Version_QBSData>(NodeID);
                    if (node == null) throw new Formula.Exceptions.BusinessValidationException("未能找到选中的节点，无法新增");
                    var child = node.AddEmptyChild();
                    result = FormulaHelper.ModelToDic<S_Q_QBS_Version_QBSData>(child);
                }
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult DeleteNodes(string ListData, string VersionID)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var node = this.GetEntityByID<S_Q_QBS_Version_QBSData>(item.GetValue("ID"));
                if (node == null || node.NodeType == "Root")
                    continue;
                node.ValidateDelete();
                if (node.ModifyState == BomVersionModifyState.Add.ToString())
                {
                    this.entities.Set<S_Q_QBS_Version_QBSData>().Delete(c => c.QBSFullID.StartsWith(node.QBSFullID) && c.VersionID == VersionID);
                }
                else
                {
                    this.entities.Set<S_Q_QBS_Version_QBSData>().Where(c => c.QBSFullID.StartsWith(node.QBSFullID) && c.VersionID == VersionID).
                        Update(c => c.ModifyState = BomVersionModifyState.Remove.ToString());
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveNodes(string ListData, string VersionID)
        {
            var list = JsonHelper.ToList(ListData);
            var version = this.GetEntityByID<S_Q_QBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的版本信息，保存失败");
            foreach (var item in list)
            {
                if (item.GetValue("_state").ToLower() == "removed") continue;
                var detail = this.GetEntityByID<S_Q_QBS_Version_QBSData>(item.GetValue("ID"));
                if (detail != null)
                {
                    this.UpdateEntity<S_Q_QBS_Version_QBSData>(detail, item);
                    if (detail.ModifyState == BomVersionModifyState.Normal.ToString())
                        detail.ModifyState = BomVersionModifyState.Modify.ToString();
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RevertNode(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            var result = new List<Dictionary<string, object>>();
            foreach (var item in list)
            {
                var detail = this.GetEntityByID<S_Q_QBS_Version_QBSData>(item.GetValue("ID"));
                if (detail == null) continue;
                var versionID = detail.VersionID;  //记录下版本ID，以免恢复上一条记录时候，将版本ID一起改变
                //如果是新增的QBS记录，则直接删除
                if (detail.ModifyState == BomVersionModifyState.Add.ToString())
                {
                    var dic = FormulaHelper.ModelToDic<S_Q_QBS_Version_QBSData>(detail);
                    this.entities.Set<S_Q_QBS_Version_QBSData>().Remove(detail);
                    result.Add(dic);
                }
                else
                {
                    //非新增的QBS记录，要去找到上一个审批完成的版本，如果上一个审批完成的版本没有
                    //则获取S_Q_QBS 初始化的QBS数据中的内容来恢复，如果都没有，则不恢复。
                    var engineeringInfo = this.GetEntityByID<S_I_Engineering>(detail.EngineeringInfoID);
                    var lastVersion = engineeringInfo.S_Q_QBS_Version.Where(c => c.FlowPhase == "End").OrderByDescending(c => c.ID).FirstOrDefault();
                    if (lastVersion == null)
                    {
                        var qbs = this.GetEntityByID<S_Q_QBS>(detail.QBSID);
                        if (qbs != null)
                        {
                            var qbsDic = FormulaHelper.ModelToDic<S_Q_QBS>(qbs);
                            this.UpdateEntity<S_Q_QBS_Version_QBSData>(detail, qbsDic);
                            detail.QBSID = qbs.ID;
                            detail.VersionID = versionID;
                        }
                    }
                    else
                    {
                        var lastDetail = lastVersion.S_Q_QBS_Version_QBSData.FirstOrDefault(c => c.QBSID == detail.QBSID);
                        if (lastDetail != null)
                        {
                            var detailDic = FormulaHelper.ModelToDic<S_Q_QBS_Version_QBSData>(lastDetail);
                            this.UpdateEntity<S_Q_QBS_Version_QBSData>(detail, detailDic);
                            detail.VersionID = versionID;
                        }
                    }
                    detail.ModifyState = BomVersionModifyState.Normal.ToString();
                    var dic = FormulaHelper.ModelToDic<S_Q_QBS_Version_QBSData>(detail);
                    result.Add(dic);
                }
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var sourceNode = this.GetEntityByID<S_Q_QBS_Version_QBSData>(sourceID);
            if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的设备，无法移动");
            if (dragAction.ToLower() == "add")
            {
                var target = this.GetEntityByID<S_Q_QBS_Version_QBSData>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                sourceNode.QBSParentID = target.QBSID;
                sourceNode.QBSFullID = target.QBSFullID + "." + sourceNode.QBSID;
                if (target.StructNodeInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标节点的定义内容，无法移动");
                var childStruct = target.StructNodeInfo.Children.FirstOrDefault();
                if (childStruct == null) throw new Formula.Exceptions.BusinessValidationException("目标节点不允许增加新节点，无法移动");
                sourceNode.StructNodeID = childStruct.ID;
                sourceNode.NodeType = childStruct.NodeType;
                sourceNode.SortIndex = target.Children.Count + 1;
            }
            else if (dragAction.ToLower() == "before")
            {
                var target = this.GetEntityByID<S_Q_QBS_Version_QBSData>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                this.entities.Set<S_Q_QBS_Version_QBSData>().Where(c => c.QBSParentID == target.QBSParentID && c.VersionID == sourceNode.VersionID
                   && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 0.001);
                if (target.Parent == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标的上级节点，移动失败");
                sourceNode.QBSParentID = target.QBSParentID;
                sourceNode.QBSFullID = target.Parent.QBSFullID + "." + sourceNode.QBSID;
                sourceNode.NodeType = target.NodeType;
                sourceNode.StructNodeID = target.StructNodeID;
                sourceNode.SortIndex = target.SortIndex - 0.001;
            }
            else if (dragAction.ToLower() == "after")
            {
                var target = this.GetEntityByID<S_Q_QBS_Version_QBSData>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");
                if (target.Parent == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标的上级节点，移动失败");
                this.entities.Set<S_Q_QBS_Version_QBSData>().Where(c => c.QBSParentID == target.QBSParentID && c.VersionID == sourceNode.VersionID
                  && c.SortIndex > target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 0.001);
                sourceNode.QBSParentID = target.QBSParentID;
                sourceNode.QBSFullID = target.Parent.QBSFullID + "." + sourceNode.QBSID;
                sourceNode.NodeType = target.NodeType;
                sourceNode.StructNodeID = target.StructNodeID;
                sourceNode.SortIndex = target.SortIndex + 0.001;
            }
            this.entities.SaveChanges();
            return Json(sourceNode);
        }

        public JsonResult GetQBSList(string EngineeringInfoID, QueryBuilder qb, string ShowAll)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息");
            qb.SortField = "SortIndex";
            qb.SortOrder = "asc";
            qb.PageSize = 0;
            var qbsList = this.entities.Set<S_Q_QBS>().Where(c => c.EngineeringInfoID == engineeringInfo.ID).Where(qb).ToList();
            var sectionList = engineeringInfo.S_I_Section.OrderBy(c => c.SortIndex).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in sectionList)
            {
                var dic = FormulaHelper.ModelToDic<S_I_Section>(item);
                var details = qbsList.Where(c => c.SectionID == item.ID).ToList();
                if (details.Count == 0 && ShowAll.ToLower() == "false")
                {
                    continue;
                }
                dic.SetValue("NodeType", "Root");
                result.Add(dic);
                foreach (var detail in details)
                {
                    var detailDic = FormulaHelper.ModelToDic<S_Q_QBS>(detail);
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

        public JsonResult ValidateQBS(string ListData)
        {
            var list = JsonHelper.ToList(ListData);

            foreach (var item in list)
            {
                var qbs = this.GetEntityByID<S_Q_QBS>(item.GetValue("ID"));
                if (qbs == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有找到【" + item.GetValue("Name") + "】质量策划节点，请确认选择的内容是质量节点");
                }
                if (qbs.Children.Count(c => c.State != ProjectState.Finish.ToString()) > 0)
                {
                    throw new Formula.Exceptions.BusinessValidationException("【" + qbs.Name + "】的子节点未全部验收通过，无法进行验收操作");
                }
                if (qbs.State == ProjectState.Finish.ToString())
                {

                    throw new Formula.Exceptions.BusinessValidationException("【" + qbs.Name + "】的已经通过验收，无法进行重复验收操作");
                }
            }
            return Json("");
        }
    }
}
