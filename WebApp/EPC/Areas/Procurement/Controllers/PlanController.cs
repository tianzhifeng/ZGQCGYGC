using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using System.Data;

namespace EPC.Areas.Procurement.Controllers
{
    public class PlanController : EPCController<S_P_Plan>
    {
        public override ActionResult List()
        {
            var engineeringInfoID = this.GetQueryString("EngineeringInfoID");
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(engineeringInfoID);

            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程项目信息，无法编制采购计划");
            bool isFirstPlan = false;
            bool isFlowEnd = false;
            //先确定是否是首次编制采购 计划
            if (engineeringInfo.S_P_Plan.Count == 0)
            {
                isFirstPlan = true;
                ViewBag.PlanID = "";
                ViewBag.VersionNo = "0";
            }
            else
            {
                //判定是否有在编辑中的采购计划版本
                var plan = engineeringInfo.S_P_Plan.Where(c => c.FlowPhase != "End").FirstOrDefault();
                if (plan == null)
                {
                    plan = engineeringInfo.S_P_Plan.Where(c => c.FlowPhase == "End").OrderByDescending(c => c.ID).FirstOrDefault();
                    isFlowEnd = true;
                }
                ViewBag.PlanID = plan.ID;
                ViewBag.VersionNo = plan.VersionNumber;
                ViewBag.FlowPhase = plan.FlowPhase;
            }
            ViewBag.IsFlowEnd = isFlowEnd;
            ViewBag.IsFirstPlan = isFirstPlan;


            var belongEnum = new List<Dictionary<string, object>>();
            if (engineeringInfo.PBSRoot != null)
            {
                var list = engineeringInfo.PBSRoot.Children.OrderBy(c => c.SortIndex).ToList();
                foreach (var pbs in list)
                {
                    var dic = new Dictionary<string, object>();
                    dic.SetValue("text", pbs.Name);
                    dic.SetValue("value", pbs.ID);
                    belongEnum.Add(dic);
                }
            }
            ViewBag.PBSRelation = JsonHelper.ToJson(belongEnum);
            return View();
        }

        public JsonResult UpgradPlan(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) { throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程信息，无法启动采购计划编制"); }
            var plan = engineeringInfo.UpgradeProcurementPlan();
            this.entities.SaveChanges();
            return Json(plan);
        }

        public JsonResult GetPackageList(string planID, string ShowType)
        {
            var list = new List<S_P_Plan_Package>();
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                list = this.entities.Set<S_P_Plan_Package>().Where(c => c.PlanID == planID && c.ModifyState != "Normal").OrderBy(c => c.SortIndex).ToList();
            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不显示已经删除的数据
                list = this.entities.Set<S_P_Plan_Package>().Where(c => c.PlanID == planID && c.ModifyState != "Remove").OrderBy(c => c.SortIndex).ToList();
            }
            else
            {
                //显示全部数据，体现差异
                list = this.entities.Set<S_P_Plan_Package>().Where(c => c.PlanID == planID).OrderBy(c => c.SortIndex).ToList();
            }
            return Json(list);
        }

        public JsonResult GetItemList(string PackageID, string ShowType)
        {
            string sql = @"select S_P_Plan_Package_Item.ID as RelationID,S_P_Plan_Package_Item.VersionNo as PlanVersion,
 S_P_Bom.*,ItemQuantity,ModifyState from S_P_Plan_Package_Item
inner join S_P_Bom on S_P_Plan_Package_Item.BomID=
S_P_Bom.ID  where PackageID='{0}'";
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                sql += " and S_P_Plan_Package_Item.ModifyState!='" + BomVersionModifyState.Normal.ToString() + "'";
            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不显示已经删除的数据
                sql += " and S_P_Plan_Package_Item.ModifyState!='" + BomVersionModifyState.Remove.ToString() + "'";
            }
            sql += " order by BomID ";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, PackageID));
            return Json(dt);
        }

        public JsonResult GetPackageItemList(string PackageID)
        {
            string sql = @"select dbo.S_P_Plan_Package_Item.ID as RelationID,
 S_P_Bom.*,ItemQuantity,ModifyState from S_P_Plan_Package_Item
inner join S_P_Bom on S_P_Plan_Package_Item.BomID=
S_P_Bom.ID  where PackageID='{0}' and S_P_Plan_Package_Item.ModifyState!='{1}'";
            sql += " order by BomID ";
            var dt = this.SqlHelper.ExecuteDataTable(String.Format(sql, PackageID, BomVersionModifyState.Remove.ToString()));
            return Json(dt);
        }

        public JsonResult AddPackage(string PlanID, string PrePackageID)
        {
            var plan = this.GetEntityByID(PlanID);
            if (plan == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的计划版本，无法新增采购包");
            var package = plan.AddEmptyPackage(PrePackageID);
            this.entities.SaveChanges();
            return Json(package);
        }

        public JsonResult ImportBidBom(string ListData, string EngineeringInfoID, string PlanID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) { throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程信息，无法导入采购包"); }
            var plan = this.GetEntityByID<S_P_Plan>(PlanID);
            var resultList = new List<S_P_Plan_Package>();
            if (plan == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到编辑中采购计划，请先进行升版操作后再进行导入");
            }
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var bomID = item.GetValue("BomID");
                if (plan.S_P_Plan_Package.Count(c => c.S_P_Plan_Package_Item.Count(d => d.BomID == bomID) > 0) > 0)
                {
                    continue;
                }
                var package = plan.AddEmptyPackage();
                package.Name = item.GetValue("Name");
                package.Code = item.GetValue("Code");
                var cbsID = item.GetValue("OfferCBSID");
                if (!String.IsNullOrEmpty(cbsID))
                {
                    var offerCBS = this.entities.Set<S_M_BidOffer_CBS>().Find(cbsID);
                    if (offerCBS != null && !String.IsNullOrEmpty(offerCBS.RelateID))
                    {
                        var pbsNode = engineeringInfo.S_I_PBS.FirstOrDefault(c => c.ID == offerCBS.RelateID);
                        if (pbsNode != null)
                        {
                            var subNode = pbsNode.Ancestor.FirstOrDefault(c => c.NodeType == "SubProject");
                            if (subNode != null)
                            {
                                package.SubProjectCode = subNode.Code;
                                package.SubProjectName = subNode.Name;
                                package.RelatePBSID = subNode.ID;
                            }
                        }
                    }
                }
                resultList.Add(package);
                var packageItem = new S_P_Plan_Package_Item();
                packageItem.ID = FormulaHelper.CreateGuid();
                packageItem.BomID = bomID;
                packageItem.ItemQuantity = String.IsNullOrEmpty(item.GetValue("Quantity")) ? 0 : Convert.ToDecimal(item.GetValue("Quantity"));
                packageItem.EngineeringInfoID = package.EngineeringInfoID;
                packageItem.ModifyState = BomVersionModifyState.Add.ToString();
                packageItem.PlanID = package.PlanID;
                packageItem.VersionNo = "";
                package.ItemCount = 1;
                package.ItemQuantity = packageItem.ItemQuantity;
                package.S_P_Plan_Package_Item.Add(packageItem);
            }
            this.entities.SaveChanges();
            return Json(resultList);
        }

        public JsonResult ImportPBom(string ListData, string EngineeringInfoID, string PlanID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) { throw new Formula.Exceptions.BusinessValidationException("未能找到指定的工程信息，无法导入采购包"); }
            var plan = this.GetEntityByID<S_P_Plan>(PlanID);
            var resultList = new List<S_P_Plan_Package>();
            if (plan == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到编辑中采购计划，请先进行升版操作后再进行导入");
            }

            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var bomID = item.GetValue("ID");
                if (plan.S_P_Plan_Package.Count(c => c.S_P_Plan_Package_Item.Count(d => d.BomID == bomID) > 0) > 0)
                {
                    continue;
                }
                var package = plan.AddEmptyPackage();
                package.Name = item.GetValue("Name");
                package.Code = item.GetValue("Code");
                var pbsID = item.GetValue("PBSNodeID");
                if (!String.IsNullOrEmpty(pbsID))
                {
                    var pbsNode = engineeringInfo.S_I_PBS.FirstOrDefault(c => c.ID == pbsID);
                    if (pbsNode != null)
                    {
                        var subNode = pbsNode.Ancestor.FirstOrDefault(c => c.NodeType == "SubProject");
                        if (subNode != null)
                        {
                            package.SubProjectCode = subNode.Code;
                            package.SubProjectName = subNode.Name;
                            package.RelatePBSID = subNode.ID;
                        }
                    }
                }
                resultList.Add(package);
                var packageItem = new S_P_Plan_Package_Item();
                packageItem.ID = FormulaHelper.CreateGuid();
                packageItem.BomID = bomID;
                packageItem.ItemQuantity = String.IsNullOrEmpty(item.GetValue("Quantity")) ? 0 : Convert.ToDecimal(item.GetValue("Quantity"));
                packageItem.EngineeringInfoID = package.EngineeringInfoID;
                packageItem.ModifyState = BomVersionModifyState.Add.ToString();
                packageItem.PlanID = package.PlanID;
                packageItem.VersionNo = "";
                package.ItemCount = 1;
                package.ItemQuantity = packageItem.ItemQuantity;
                package.S_P_Plan_Package_Item.Add(packageItem);
            }
            this.entities.SaveChanges();
            return Json(resultList);
        }

        public JsonResult RemovePackage(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var planPackage = this.GetEntity<S_P_Plan_Package>(item.GetValue("ID"));
                if (planPackage == null) continue;
                planPackage.Remove();
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult CombinePackage(string ListData, string CombineNodeID)
        {
            var result = new List<Dictionary<string, object>>();
            var combinPackage = this.GetEntityByID<S_P_Plan_Package>(CombineNodeID);
            if (combinPackage == null) throw new Formula.Exceptions.BusinessValidationException("未指定合并的节点信息，无法合并节点");
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var package = this.GetEntityByID<S_P_Plan_Package>(item.GetValue("ID"));
                if (package == null) continue;
                combinPackage.CombinePackage(package);
                result.Add(FormulaHelper.ModelToDic<S_P_Plan_Package>(package));
            }
            result.Add(FormulaHelper.ModelToDic<S_P_Plan_Package>(combinPackage));
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult SplitPackage(string ItemList, string PackageID)
        {
            var list = JsonHelper.ToList(ItemList);
            var package = this.GetEntityByID<S_P_Plan_Package>(PackageID);
            if (package == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的采购包，无法进行拆分操作");
            var newPackage = package.SplitToNewPackage(list);
            this.entities.SaveChanges();
            package.SumData();
            newPackage.SumData();
            this.entities.SaveChanges();
            return Json(newPackage);
        }

        public JsonResult SavePackage(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {
                var planPackage = this.GetEntity<S_P_Plan_Package>(item.GetValue("ID"));
                this.UpdateEntity<S_P_Plan_Package>(planPackage, item);
                if (!String.IsNullOrEmpty(planPackage.RelatePBSID))
                {
                    var pbsNode = this.GetEntityByID<S_I_PBS>(planPackage.RelatePBSID);
                    if (pbsNode == null)
                    {
                        throw new Formula.Exceptions.BusinessValidationException("没有找到指定的PBS结构");
                    }
                    planPackage.SubProjectCode = pbsNode.Code;
                    planPackage.SubProjectName = pbsNode.Name;
                }

                if (planPackage.ModifyState == BomVersionModifyState.Normal.ToString())
                    planPackage.ModifyState = BomVersionModifyState.Modify.ToString();

                //记录采购包下的设备材料记录条目数量
                planPackage.ItemCount = planPackage.S_P_Plan_Package_Item.Count;
                if (planPackage.S_P_Plan_Package_Item.Count > 0)
                {
                    //记录采购包下的设备材料的综合总数量
                    var quantity = planPackage.S_P_Plan_Package_Item.Sum(c => c.ItemQuantity);
                    planPackage.ItemQuantity = quantity.HasValue ? quantity.Value : 0;
                }
                else
                    planPackage.ItemQuantity = 0;
            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult AddPackageItem(string PackageID, string ListData)
        {
            var package = this.GetEntityByID<S_P_Plan_Package>(PackageID);
            if (package == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的采购包，无法绑定设备材料");
            }
            var result = new List<Dictionary<string, object>>();
            var list = JsonHelper.ToList(ListData);
            foreach (var item in list)
            {

                var pBomID = item.GetValue("ID");
                var pBom = this.GetEntityByID<S_P_Bom>(pBomID);
                //如果采购清单中不存在该内容，则不进行增加
                if (pBom == null) continue;
                var packageItem = package.S_P_Plan_Package_Item.FirstOrDefault(c => c.BomID == pBomID);
                if (packageItem == null)
                {
                    packageItem = new S_P_Plan_Package_Item();
                    packageItem.ID = FormulaHelper.CreateGuid();
                    packageItem.BomID = pBomID;
                    packageItem.ItemQuantity = String.IsNullOrEmpty(item.GetValue("RemainPlanQuantity")) ? 0 : Convert.ToDecimal(item.GetValue("RemainPlanQuantity"));
                    packageItem.EngineeringInfoID = package.EngineeringInfoID;
                    packageItem.ModifyState = BomVersionModifyState.Add.ToString();
                    packageItem.PlanID = package.PlanID;
                    packageItem.VersionNo = pBom.VersionNo;
                    package.S_P_Plan_Package_Item.Add(packageItem);
                    if (package.ModifyState == BomVersionModifyState.Normal.ToString())
                    {
                        package.ModifyState = BomVersionModifyState.Modify.ToString();
                    }
                }
                else
                {
                    var bomQuantity = pBom.Quantity.HasValue ? pBom.Quantity.Value : 0m;
                    var planQuantity = 0m;
                    packageItem.ItemQuantity = bomQuantity - planQuantity;
                    if (packageItem.ModifyState == BomVersionModifyState.Modify.ToString())
                        packageItem.ModifyState = BomVersionModifyState.Normal.ToString();
                    else if (packageItem.ModifyState == BomVersionModifyState.Remove.ToString())
                    {
                        packageItem.ModifyState = BomVersionModifyState.Normal.ToString();
                    }
                }
                var dic = FormulaHelper.ModelToDic<S_P_Plan_Package_Item>(packageItem);
                dic.SetValue("ParentID", package.ID);
                dic.SetValue("NodeType", "Detail");
            }
            package.ItemCount = package.S_P_Plan_Package_Item.Count(c => c.ModifyState != "Remove");
            if (package.S_P_Plan_Package_Item.Count(c => c.ModifyState != "Remove") > 0)
            {
                package.ItemQuantity = package.S_P_Plan_Package_Item.Where(c => c.ModifyState != "Remove").Sum(c => c.ItemQuantity);
            }
            this.entities.SaveChanges();

            //更新采购清单中的已计划数量
            foreach (var item in list)
            {
                var pBomID = item.GetValue("ID");
                var pBom = this.GetEntityByID<S_P_Bom>(pBomID);
                if (pBom == null) continue;
                pBom.PlanQuantity = this.entities.Set<S_P_Plan_Package_Item>().
                       Where(c => c.PlanID == package.PlanID && c.BomID == pBomID && c.ModifyState != "Remove").Sum(c => c.ItemQuantity);
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult RevertPlan(string PlanID)
        {
            var planInfo = this.GetEntityByID<S_P_Plan>(PlanID);
            if (planInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的计划版本");
            if (planInfo.FlowPhase == "End") { throw new Formula.Exceptions.BusinessValidationException("已经发布的计划不能进行撤销删除操作"); }
            this.entities.Set<S_P_Plan>().Remove(planInfo);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RevertPackage(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            var result = new List<Dictionary<string, object>>();
            foreach (var item in list)
            {
                var package = this.GetEntityByID<S_P_Plan_Package>(item.GetValue("ID"));
                if (package != null)
                {
                    package.Revert();
                    var dic = FormulaHelper.ModelToDic<S_P_Plan_Package>(package);
                    result.Add(dic);
                }
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult SaveItemData(string ItemData)
        {
            var dic = JsonHelper.ToObject(ItemData);
            var item = this.GetEntityByID<S_P_Plan_Package_Item>(dic.GetValue("RelationID"));
            if (item == null) return Json("");
            var pBom = this.GetEntityByID<S_P_Bom>(item.BomID);
            if (pBom == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的采购清单内容");
            var itemQuantity = String.IsNullOrEmpty(dic.GetValue("ItemQuantity")) ? 0 : Convert.ToDecimal(dic.GetValue("ItemQuantity"));
            if (item.ItemQuantity != itemQuantity)
            {
                //采购计划设备明细至允许修订数量
                if (item.ModifyState == BomVersionModifyState.Normal.ToString())
                {
                    item.ModifyState = BomVersionModifyState.Modify.ToString();
                }
                item.ItemQuantity = itemQuantity;
                if (item.S_P_Plan_Package.ModifyState == BomVersionModifyState.Normal.ToString())
                {
                    item.S_P_Plan_Package.ModifyState = BomVersionModifyState.Modify.ToString();
                }
            }
            var otherQuantity = this.entities.Set<S_P_Plan_Package_Item>().
                    Where(c => c.PlanID == item.PlanID && c.BomID == pBom.ID && c.ID != item.ID && c.ModifyState != "Remove").Sum(c => c.ItemQuantity);
            var quantity = otherQuantity.HasValue ? otherQuantity.Value : 0;
            quantity += item.ItemQuantity.HasValue ? item.ItemQuantity.Value : 0;
            //if (quantity > pBom.Quantity)
            //{
            //    throw new Formula.Exceptions.BusinessValidationException("计划采购总数量【" + quantity + "】不能超过确定的设备材料清单中的设备总数【" + pBom.Quantity + "】");
            //}

            this.entities.SaveChanges();

            if (pBom != null)
            {
                pBom.PlanQuantity = this.entities.Set<S_P_Plan_Package_Item>().
                     Where(c => c.PlanID == item.PlanID && c.BomID == pBom.ID && c.ModifyState != "Remove").Sum(c => c.ItemQuantity);
            }
            this.entities.SaveChanges();
            var result = new Dictionary<string, object>();
            result.SetValue("Item", item);
            result.SetValue("Package", item.S_P_Plan_Package);
            return Json(result);
        }

        public JsonResult RemoveItemData(string ItemData)
        {
            var dic = JsonHelper.ToObject(ItemData);
            var item = this.GetEntityByID<S_P_Plan_Package_Item>(dic.GetValue("RelationID"));
            if (item == null) return Json("");
            var pBom = this.GetEntityByID<S_P_Bom>(item.BomID);
            if (pBom == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的采购清单内容");
            var package = item.S_P_Plan_Package;
            if (item.ModifyState == BomVersionModifyState.Add.ToString())
                this.entities.Set<S_P_Plan_Package_Item>().Remove(item);
            else
                item.ModifyState = BomVersionModifyState.Remove.ToString();

            this.entities.SaveChanges();
            pBom.PlanQuantity = this.entities.Set<S_P_Plan_Package_Item>().
                  Where(c => c.PlanID == item.PlanID && c.BomID == pBom.ID && c.ModifyState != "Remove").Sum(c => c.ItemQuantity);
            if (package != null)
            {
                if (package.S_P_Plan_Package_Item.Count == 0)
                {
                    package.ItemQuantity = 0;
                }
                else
                {
                    package.ItemQuantity = package.S_P_Plan_Package_Item.Sum(c => c.ItemQuantity);
                }
                if (package.ModifyState == BomVersionModifyState.Normal.ToString())
                {
                    package.ModifyState = BomVersionModifyState.Modify.ToString();
                }
                package.ItemCount = package.S_P_Plan_Package_Item.Count(c => c.ModifyState != "Remove");

            }
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RevertItem(string ItemID)
        {
            var item = this.GetEntityByID<S_P_Plan_Package_Item>(ItemID);
            if (item == null) { throw new Formula.Exceptions.BusinessValidationException("您所选中的记录可能已经被删除，请刷新列表后重新操作"); }
            item.Revert();
            this.entities.SaveChanges();
            var result = new Dictionary<string, object>();
            result.SetValue("Package", item.S_P_Plan_Package);
            result.SetValue("Item", item);
            return Json(result);
        }

        public JsonResult MoveNode(string sourceID, string targetID, string dragAction)
        {
            var dragNode = this.GetEntityByID<S_P_Plan_Package>(sourceID);
            var targetNode = this.GetEntityByID<S_P_Plan_Package>(targetID);
            if (targetNode == null || dragNode == null) return Json("");
            if (dragAction.ToLower() == "after")
            {
                this.entities.Set<S_P_Plan_Package>().Where(c => c.PlanID == targetNode.PlanID
                    && c.SortIndex > targetNode.SortIndex).Update(c => c.SortIndex = c.SortIndex + 0.001);
                dragNode.SortIndex = targetNode.SortIndex + 0.001;
            }
            else if (dragAction.ToLower() == "before")
            {
                this.entities.Set<S_P_Plan_Package>().Where(c => c.PlanID == targetNode.PlanID && c.SortIndex < targetNode.SortIndex).Update(c => c.SortIndex = c.SortIndex - 0.001);
                dragNode.SortIndex = targetNode.SortIndex - 0.001;
            }
            this.entities.SaveChanges();
            return Json(dragNode);
        }

    }
}
