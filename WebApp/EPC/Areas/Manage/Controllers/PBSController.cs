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


namespace EPC.Areas.Manage.Controllers
{
    public class PBSController : EPCController<S_I_PBS_Version>
    {
        public ActionResult TreeList()
        {
            string engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的工程信息");
            var version = engineeringInfo.S_I_PBS_Version.OrderByDescending(c => c.ID).FirstOrDefault();
            var pushCount = engineeringInfo.S_I_PBS_Version.Count(c => c.FlowPhase == "End");
            bool flowEnd = true; bool isFirst = false;
            if (version == null)
            {
                //此时如果要编辑，则直接做升版预算操作
                flowEnd = true;
                isFirst = true;
                ViewBag.VersionID = "";
                ViewBag.FlowPhase = "";
                ViewBag.VersionNo = "0";
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
            ViewBag.First = isFirst;
            ViewBag.EngineeringInfoID = engineeringInfoID;
            ViewBag.PushCount = pushCount;
            return View();
        }

        public JsonResult GetVersionTreeList(QueryBuilder qb, string VersionID, string ShowType)
        {
            var version = this.GetEntity<S_I_PBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的结构策划版本");
            qb.PageSize = 0;
            qb.Add("VersionID", QueryMethod.Equal, VersionID);
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                qb.Add("ModifyState", QueryMethod.NotEqual, BomVersionModifyState.Normal.ToString());
            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不体现差异
                qb.Add("ModifyState", QueryMethod.NotEqual, BomVersionModifyState.Remove.ToString());
            }
            var data = this.entities.Set<S_I_PBS_Version_PBSData>().Where(qb).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in data)
            {
                var dic = FormulaHelper.ModelToDic<S_I_PBS_Version_PBSData>(item);
                var canSyn = item.StructNodeInfo.CanSynToWBS.HasValue &&
                    item.StructNodeInfo.CanSynToWBS.Value ? true.ToString().ToLower() : false.ToString().ToLower();
                dic.SetValue("CanSynToWBS", canSyn);
                result.Add(dic);
            }
            return Json(result);
        }

        public JsonResult UpgradPBS(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null)
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程信息，无法进行结构策划");
            engineeringInfo.UpGradePBSVersion();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetLastDetailInfo(string ID)
        {
            var detail = this.GetEntityByID<S_I_PBS_Version_PBSData>(ID);
            var result = new Dictionary<string, object>();
            if (detail != null)
            {

                var lastVersion = this.entities.Set<S_I_PBS_Version>().Where(d => d.EngineeringInfoID == detail.EngineeringInfoID
                    && d.FlowPhase == "End" && d.ID != detail.VersionID).OrderByDescending(c => c.ID).FirstOrDefault();
                if (lastVersion != null)
                {
                    var lastDetail = lastVersion.S_I_PBS_Version_PBSData.FirstOrDefault(c => c.PBSID == detail.PBSID);
                    if (lastDetail != null)
                    {
                        result = FormulaHelper.ModelToDic<S_I_PBS_Version_PBSData>(lastDetail);
                    }
                }
            }
            return Json(result);
        }

        public JsonResult Revert(string VersionID)
        {
            var version = this.entities.Set<S_I_PBS_Version>().FirstOrDefault(c => c.ID == VersionID);
            if (version == null)
                throw new Formula.Exceptions.BusinessValidationException("没有可编辑的版本，无法进行撤销操作");
            if (version.FlowPhase == "End") { throw new Formula.Exceptions.BusinessValidationException("已经发布的版本不能进行撤销操作"); }
            this.entities.Set<S_I_PBS_Version>().Remove(version);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddVersionPBS(string NodeID, string VersionID, string AddMode)
        {
            var node = this.GetEntityByID<S_I_PBS_Version_PBSData>(NodeID);
            if (node == null) throw new Formula.Exceptions.BusinessValidationException("未能找到选中的节点，无法新增");
            var result = new Dictionary<string, object>();
            if (AddMode == "After")
            {
                var child = node.AddEmptyBrother();
                result = FormulaHelper.ModelToDic<S_I_PBS_Version_PBSData>(child);
            }
            else if (AddMode == "AddChild")
            {
                var child = node.AddEmptyChild();
                result = FormulaHelper.ModelToDic<S_I_PBS_Version_PBSData>(child);
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult DeleteNodes(string ListData, string VersionID)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var node = this.GetEntityByID<S_I_PBS_Version_PBSData>(item.GetValue("ID"));
                if (node == null || node.NodeType == "Root")
                    continue;
                node.ValidateDelete();
                if (node.ModifyState == BomVersionModifyState.Add.ToString())
                {
                    this.entities.Set<S_I_PBS_Version_PBSData>().Delete(c => c.PBSFullID.StartsWith(node.PBSFullID) && c.VersionID == VersionID);
                }
                else
                {
                    this.entities.Set<S_I_PBS_Version_PBSData>().Where(c => c.PBSFullID.StartsWith(node.PBSFullID) && c.VersionID == VersionID).
                        Update(c => c.ModifyState = BomVersionModifyState.Remove.ToString());
                }
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult SaveNodes(string ListData, string VersionID)
        {
            var list = JsonHelper.ToList(ListData);
            var version = this.GetEntityByID<S_I_PBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的预算版本信息，保存失败");
            foreach (var item in list)
            {
                if (item.GetValue("_state").ToLower() == "removed") continue;
                var detail = this.GetEntityByID<S_I_PBS_Version_PBSData>(item.GetValue("ID"));
                if (detail != null)
                {
                    this.UpdateEntity<S_I_PBS_Version_PBSData>(detail, item);
                    if (detail.ModifyState == BomVersionModifyState.Normal.ToString())
                        detail.ModifyState = BomVersionModifyState.Modify.ToString();
                    detail.ToWBS = item.GetValue("ToWBS");
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
                var detail = this.GetEntityByID<S_I_PBS_Version_PBSData>(item.GetValue("ID"));
                if (detail == null) continue;
                var versionID = detail.VersionID;  //记录下版本ID，以免恢复上一条记录时候，将版本ID一起改变
                //如果是新增的PBS记录，则直接删除
                if (detail.ModifyState == BomVersionModifyState.Add.ToString())
                {
                    var dic = FormulaHelper.ModelToDic<S_I_PBS_Version_PBSData>(detail);
                    this.entities.Set<S_I_PBS_Version_PBSData>().Remove(detail);                   
                    result.Add(dic);
                }
                else
                {
                    //非新增的PBS记录，要去找到上一个审批完成的版本，如果上一个审批完成的版本没有
                    //则获取S_I_PBS 初始化的PBS数据中的内容来恢复，如果都没有，则不恢复。
                    var engineeringInfo = this.GetEntityByID<S_I_Engineering>(detail.EngineeringInfoID);
                    var lastVersion = engineeringInfo.S_I_PBS_Version.Where(c => c.FlowPhase == "End").OrderByDescending(c => c.ID).FirstOrDefault();
                    if (lastVersion == null)
                    {
                        var pbs = this.GetEntityByID<S_I_PBS>(detail.PBSID);
                        if (pbs != null)
                        {
                            var pbsDic = FormulaHelper.ModelToDic<S_I_PBS>(pbs);
                            this.UpdateEntity<S_I_PBS_Version_PBSData>(detail, pbsDic);
                            detail.PBSID = pbs.ID;
                            detail.VersionID = versionID;
                        }
                    }
                    else
                    {
                        var lastDetail = lastVersion.S_I_PBS_Version_PBSData.FirstOrDefault(c => c.PBSID == detail.PBSID);
                        if (lastDetail != null)
                        {
                            var detailDic = FormulaHelper.ModelToDic<S_I_PBS_Version_PBSData>(lastDetail);
                            this.UpdateEntity<S_I_PBS_Version_PBSData>(detail, detailDic);
                            detail.VersionID = versionID;
                        }
                    }
                    detail.ModifyState = BomVersionModifyState.Normal.ToString();
                    var dic = FormulaHelper.ModelToDic<S_I_PBS_Version_PBSData>(detail);
                    result.Add(dic);
                }
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var sourceNode = this.GetEntityByID<S_I_PBS_Version_PBSData>(sourceID);
            if (sourceNode == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的设备，无法移动");
            if (dragAction.ToLower() == "add")
            {
                var target = this.GetEntityByID<S_I_PBS_Version_PBSData>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                sourceNode.PBSParentID = target.PBSID;
                sourceNode.PBSFullID = target.PBSFullID + "." + sourceNode.PBSID;
                if (target.StructNodeInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标节点的定义内容，无法移动");
                var childStruct = target.StructNodeInfo.Children.FirstOrDefault();
                if (childStruct == null) throw new Formula.Exceptions.BusinessValidationException("目标节点不允许增加新节点，无法移动");
                sourceNode.StructNodeID = childStruct.ID;
                sourceNode.NodeType = childStruct.NodeType;
                sourceNode.SortIndex = target.Children.Count + 1;
            }
            else if (dragAction.ToLower() == "before")
            {
                var target = this.GetEntityByID<S_I_PBS_Version_PBSData>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动");
                this.entities.Set<S_I_PBS_Version_PBSData>().Where(c => c.PBSParentID == target.PBSParentID && c.VersionID == sourceNode.VersionID
                   && c.SortIndex < target.SortIndex).Update(d => d.SortIndex = d.SortIndex - 0.001);
                if (target.Parent == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标的上级节点，移动失败");
                sourceNode.PBSParentID = target.PBSParentID;
                sourceNode.PBSFullID = target.Parent.PBSFullID + "." + sourceNode.PBSID;
                sourceNode.NodeType = target.NodeType;
                sourceNode.StructNodeID = target.StructNodeID;
                sourceNode.SortIndex = target.SortIndex - 0.001;
            }
            else if (dragAction.ToLower() == "after")
            {
                var target = this.GetEntityByID<S_I_PBS_Version_PBSData>(targetID);
                if (target == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的目标节点，无法移动设备");
                if (target.Parent == null) throw new Formula.Exceptions.BusinessValidationException("没有找到目标的上级节点，移动失败");
                this.entities.Set<S_I_PBS_Version_PBSData>().Where(c => c.PBSParentID == target.PBSParentID && c.VersionID == sourceNode.VersionID
                  && c.SortIndex > target.SortIndex).Update(d => d.SortIndex = d.SortIndex + 0.001);
                sourceNode.PBSParentID = target.PBSParentID;
                sourceNode.PBSFullID = target.Parent.PBSFullID + "." + sourceNode.PBSID;
                sourceNode.NodeType = target.NodeType;
                sourceNode.StructNodeID = target.StructNodeID;
                sourceNode.SortIndex = target.SortIndex + 0.001;
            }
            this.entities.SaveChanges();
            return Json(sourceNode);
        }

        public JsonResult ImportFromBidOffer(string VersionID)
        {
            var version = this.GetEntityByID<S_I_PBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的结构版本，无法导入");
            version.ImportFromBiddOffer();
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult ImportNodeTemplate(string NodeIDs,string VersionID)
        {
            var version = this.GetEntityByID<S_I_PBS_Version>(VersionID);
            if (version == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的结构版本，无法导入");
            var infraDbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
            var templateNodes = infraDbContext.Set<S_T_NodeTemplate_Detail>().Where(c => NodeIDs.Contains(c.ID)).OrderBy(c => c.FullID).ToList();
            var rootNode = version.S_I_PBS_Version_PBSData.FirstOrDefault(c => c.NodeType == "Root");
            if (rootNode == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到项目结构节点，请撤销版本后，并再次升版重试");
            }

            var structDic = new Dictionary<string, S_I_PBS_Version_PBSData>();
            structDic.SetValue(rootNode.StructNodeID, rootNode);
            S_I_PBS_Version_PBSData lastParent = null;
            double i = 1;
            foreach (var item in templateNodes)
            {
                var templateNode = rootNode.StructNodeInfo.AllChildren.FirstOrDefault(c => c.NodeType == item.NodeType);
                if (templateNode == null) continue;
                if (templateNode.Parent == null) continue;
                lastParent = structDic.GetValue(templateNode.Parent.ID);
                if (lastParent == null || lastParent.StructNodeInfo == null) continue;
                var childTemplateNode = lastParent.StructNodeInfo.Children.FirstOrDefault(c => c.NodeType == item.NodeType);
                if (childTemplateNode == null)
                {
                    continue;
                }
                if (lastParent.Children.Exists(c => c.Name == item.Name&&c.ModifyState!=BomVersionModifyState.Remove.ToString()))
                    continue;
                var childNode = lastParent.AddEmptyChild();
                childNode.Name = item.Name;
                childNode.Code = item.Code;
                childNode.ID = FormulaHelper.CreateGuid();
                childNode.NodeType = item.NodeType;
                childNode.StructNodeID = childTemplateNode.ID;
                childNode.SortIndex = item.SortIndex;
                lastParent.Children.Add(childNode);
                structDic.SetValue(childTemplateNode.ID, childNode);
            }
            this.entities.SaveChanges();
            return Json("");
        }
    }
}
