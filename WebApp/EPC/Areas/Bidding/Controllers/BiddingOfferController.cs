using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using Newtonsoft.Json;
using Formula.ImportExport;

namespace EPC.Areas.Bidding.Controllers
{
    public class BiddingOfferController : EPCFormContorllor<S_M_BidOffer>
    {
        protected override void BeforeSave(Dictionary<string, string> dic, Base.Logic.Domain.S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                throw new Formula.Exceptions.BusinessValidationException("禁止直接新增版本数据，请重新操作");
            }
        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            string ID = this.GetQueryString("ID");
            ViewBag.FormID = ID;
            var deviceTab = new Tab();
            var catagory = CategoryFactory.GetCategory("Base.BOMMajor", "设备材料分类", "MajorCode");
            catagory.Multi = false;
            deviceTab.Categories.Add(catagory);
            deviceTab.IsDisplay = true;
            ViewBag.DeviceTab = deviceTab;
        }

        public JsonResult ReloadForm(string ID)
        {
            var dt = this.EPCSQLDB.ExecuteDataTable("select * from S_M_BidOffer where ID='" + ID + "'");
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return Json(FormulaHelper.DataRowToDic(row));
            }
            else
                return Json("");
        }

        public JsonResult CreateBidOffer(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息，无法报价");
            var bidOffer = engineeringInfo.CreateBidOffer();
            this.EPCEntites.SaveChanges();
            return Json(bidOffer);
        }

        public JsonResult UpgradBidOffer(string EngineeringInfoID)
        {
            var engineeringInfo = this.GetEntityByID<S_I_Engineering>(EngineeringInfoID);
            if (engineeringInfo == null) throw new Formula.Exceptions.BusinessValidationException("未能找到指定的项目信息，无法报价");
            var bidOffer = engineeringInfo.UpgradeBidOffer();
            bidOffer.SummaryTotalValue();
            this.EPCEntites.SaveChanges();
            return Json(bidOffer);
        }

        public JsonResult GetAllTree(string BiddingOfferID)
        {
            var data = this.EPCEntites.Set<S_M_BidOffer_CBS>().Include("S_M_BidOffer_CBS_Detail").Where(d => d.BidOfferID == BiddingOfferID).OrderBy(d => d.SortIndex).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in data)
            {
                var dic = FormulaHelper.ModelToDic<S_M_BidOffer_CBS>(item);
                dic.SetValue("TypeInfo", "CBS");
                result.Add(dic);
                foreach (var detail in item.S_M_BidOffer_CBS_Detail.ToList())
                {
                    var detailDic = FormulaHelper.ModelToDic<S_M_BidOffer_CBS_Detail>(detail);
                    detailDic.SetValue("TypeInfo", "Detail");
                    detailDic.SetValue("NodeType", "Detail");
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

        public JsonResult AddSubNode(string ParentID, string MajorCode, string ValidateMajorCode)
        {
            var parentNode = this.GetEntityByID<S_M_BidOffer_CBS>(ParentID);
            var result = new Dictionary<string, object>();
            if (parentNode == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("请选择一个节点");
            }
            if (parentNode.CBSTemplateNode != null && parentNode.CBSTemplateNode.Children.Count == 0)
            {
                var detail = parentNode.AddEmptyDetail();
                if (ValidateMajorCode.ToLower() == "true")
                {
                    var enumItems = EnumBaseHelper.GetEnumDef("Base.BOMMajor");
                    if (enumItems == null) throw new Formula.Exceptions.BusinessValidationException("未能找到 设备专业分类枚举 [Base.BOMMajor]");
                    var enumItem = enumItems.EnumItem.FirstOrDefault(c => c.Code == MajorCode);
                    if (enumItem != null)
                    {
                        detail.MajorCode = MajorCode;
                        detail.MajorName = enumItem.Name;
                    }
                }
                result = FormulaHelper.ModelToDic<S_M_BidOffer_CBS_Detail>(detail);
                result.SetValue("TypeInfo", "Detail");
                result.SetValue("NodeType", "Detail");
            }
            else
            {
                var child = parentNode.AddEmptyChild();
                result = FormulaHelper.ModelToDic<S_M_BidOffer_CBS>(child);
                if (String.IsNullOrEmpty(child.NodeType))
                {
                    result.SetValue("TypeInfo", "CBS");
                    result.SetValue("NodeType", "CBS");
                }
                else
                {
                    result.SetValue("TypeInfo", child.NodeType);
                }
            }
            this.EPCEntites.SaveChanges();
            return Json(result);
        }

        public JsonResult DeleteNode(string NodeInfo)
        {
            var infrastructureEntities = FormulaHelper.GetEntities<InfrastructureEntities>();
            var list = JsonHelper.ToList(NodeInfo);
            foreach (var item in list)
            {
                if (item.GetValue("TypeInfo") == "Detail")
                {
                    var id = item.GetValue("ID");
                    this.EPCEntites.Set<S_M_BidOffer_CBS_Detail>().Delete(d => d.ID == id);
                }
                else
                {
                    var node = this.GetEntityByID<S_M_BidOffer_CBS>(item.GetValue("ID"));
                    var cbsDefine = infrastructureEntities.Set<S_T_CBSNodeTemplate>().FirstOrDefault(a => a.ID == node.CBSDefineID);
                    if (cbsDefine.BidDelete == "1")
                        node.Delete();
                    else
                        throw new Formula.Exceptions.BusinessValidationException("该节点不允许删除");
                }
            }
            this.EPCEntites.SaveChanges();
            return Json("");
        }

        public JsonResult SaveNodes(string NodeInfo, string BidOfferID)
        {
            var bidOffer = this.GetEntityByID(BidOfferID);
            if (bidOffer == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的报价对象");
            var list = JsonHelper.ToList(NodeInfo);
            foreach (var item in list)
            {
                decimal a = 0;
                if (!string.IsNullOrEmpty(item.GetValue("Quantity")))
                {
                    if (!decimal.TryParse(item.GetValue("Quantity"), out a))
                        throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】的数量必须为数字");
                    else if (a < 0)
                        throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】的数量必须不小于0");
                }

                if (!string.IsNullOrEmpty(item.GetValue("UnitPrice")))
                {
                    if (!decimal.TryParse(item.GetValue("UnitPrice"), out a))
                        throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】的单价必须为数字");
                    else if (a < 0)
                        throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】的单价必须不小于0");
                }

                if (!string.IsNullOrEmpty(item.GetValue("TotalValue")))
                {
                    if (!decimal.TryParse(item.GetValue("TotalValue"), out a))
                        throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】的合价必须为数字");
                    else if (a < 0)
                        throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】的合价必须不小于0");
                }

                if (String.IsNullOrEmpty(item.GetValue("Name")))
                    throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】的名称属性不能为空");

                if (item.GetValue("TypeInfo") == "Detail")
                {
                    var detail = this.GetEntityByID<S_M_BidOffer_CBS_Detail>(item.GetValue("ID"));
                    if (detail == null) continue;
                    this.UpdateEntity<S_M_BidOffer_CBS_Detail>(detail, item);
                    if (String.IsNullOrEmpty(detail.MajorCode) && detail.S_M_BidOffer_CBS.CBSType == "Device")
                    {
                        throw new Formula.Exceptions.BusinessValidationException("设备【" + detail.Name + "】的专业分类属性不能为空");
                    }
                }
                else
                {
                    var node = this.GetEntityByID<S_M_BidOffer_CBS>(item.GetValue("ID"));
                    if (node == null) continue;
                    this.UpdateEntity<S_M_BidOffer_CBS>(node, item);
                }
            }
            bidOffer.SummaryTotalValue();
            this.EPCEntites.SaveChanges();
            return Json("");
        }

        public JsonResult GetDeviceTree(string BiddingOfferID, string MajorCode)
        {
            var bidOffer = this.GetEntityByID(BiddingOfferID);
            var data = this.EPCEntites.Set<S_M_BidOffer_CBS>().Include("S_M_BidOffer_CBS_Detail").Where(d => d.BidOfferID == BiddingOfferID
                && d.CBSType == "Device").OrderBy(d => d.SortIndex).ToList();
            var result = new List<Dictionary<string, object>>();
            foreach (var item in data)
            {
                var dic = FormulaHelper.ModelToDic<S_M_BidOffer_CBS>(item);
                dic.SetValue("TypeInfo", "CBS");
                result.Add(dic);
                var detailList = item.S_M_BidOffer_CBS_Detail;
                if (!String.IsNullOrEmpty(MajorCode) && MajorCode.ToLower() != "all")
                    detailList = item.S_M_BidOffer_CBS_Detail.Where(d => d.MajorCode == MajorCode).ToList();
                foreach (var detail in detailList.ToList())
                {
                    var detailDic = FormulaHelper.ModelToDic<S_M_BidOffer_CBS_Detail>(detail);
                    detailDic.SetValue("TypeInfo", "Detail");
                    detailDic.SetValue("NodeType", "Detail");
                    result.Add(detailDic);
                }
            }
            return Json(result);
        }

        public JsonResult GetConstructionTree(string BiddingOfferID)
        {
            var data = this.EPCEntites.Set<S_M_BidOffer_CBS>().Include("S_M_BidOffer_CBS_Detail").Where(d => d.BidOfferID == BiddingOfferID && d.CBSType == "Construction").
                OrderBy(d => d.SortIndex).ToList();
            var result = new List<Dictionary<string, object>>();

            foreach (var item in data)
            {
                var dic = FormulaHelper.ModelToDic<S_M_BidOffer_CBS>(item);//Construction 结构cbs
                dic.SetValue("TypeInfo", "CBS");
                result.Add(dic);

                int subCBSCountofConstructionCBS = EPCEntites.Set<S_M_BidOffer_CBS>().Count(a => a.CBSFullID.Contains(item.CBSID) && a.CBSFullID != item.CBSID);
                //如果首次添加数据,默认添加pbs节点
                if (subCBSCountofConstructionCBS == 0)
                {
                    S_M_BidOffer bidOffer = EPCEntites.Set<S_M_BidOffer>().Find(BiddingOfferID);
                    if (bidOffer == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的报价,无法载入pbs");
                    var pbsList = EPCEntites.Set<S_I_PBS>().Where(a => a.EngineeringInfoID == bidOffer.EngineeringInfoID && a.NodeType == "SubProject").ToList();

                    foreach (var pbs in pbsList)
                    {
                        var subCBS = item.AddEmptyChild();
                        subCBS.Name = pbs.Name;
                        subCBS.Code = pbs.Code;

                        var detailDic = FormulaHelper.ModelToDic<S_M_BidOffer_CBS>(subCBS);
                        detailDic.SetValue("TypeInfo", "CBS");
                        detailDic.SetValue("NodeType", "CBS");
                        result.Add(detailDic);
                    }
                    EPCEntites.SaveChanges();
                }
                else
                {
                    foreach (var detail in item.S_M_BidOffer_CBS_Detail.ToList())
                    {
                        var detailDic = FormulaHelper.ModelToDic<S_M_BidOffer_CBS_Detail>(detail);
                        detailDic.SetValue("TypeInfo", "Detail");
                        detailDic.SetValue("NodeType", "Detail");
                        result.Add(detailDic);
                    }
                }
            }

            return Json(result);
        }

        #region
        public JsonResult ImportNodeTemplate(string NodeIDs, string BidInfoID)
        {
            var bid = this.GetEntityByID<S_M_BidOffer>(BidInfoID);
            if (bid == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的报价就，无法导入");
            var deviceRoot = bid.S_M_BidOffer_CBS.Where(d => d.CBSType == "Device").OrderBy(d => d.CBSFullID).FirstOrDefault();
            if (deviceRoot == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到投标报价记录中的设备费用节点，无法导入！");
            }
            if (deviceRoot.CBSTemplateNode == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到设备材料费用的定义科目信息，导入失败");
            }

            var deviceTemplate = deviceRoot.CBSTemplateNode;
            var structDic = new Dictionary<string, S_M_BidOffer_CBS>();
            structDic.SetValue(deviceTemplate.ID, deviceRoot);
            S_M_BidOffer_CBS lastParent = null;

            var infraDbContext = FormulaHelper.GetEntities<InfrastructureEntities>();
            var templateNodes = infraDbContext.Set<S_T_NodeTemplate_Detail>().Where(c => NodeIDs.Contains(c.ID)).OrderBy(c => c.FullID).ToList();
            for (int i = 0; i < templateNodes.Count; i++)
            {
                var item = templateNodes[i];
                var nodeType = item.NodeType;
                if (nodeType != "Detail")
                {
                    var templateNode = deviceTemplate.AllChildren.FirstOrDefault(c => c.NodeType == nodeType);
                    if (templateNode == null) continue;
                    if (templateNode.Parent == null) continue;
                    lastParent = structDic.GetValue(templateNode.Parent.ID);
                    if (lastParent == null || lastParent.CBSTemplateNode == null) continue;
                    var childTemplateNode = lastParent.CBSTemplateNode.Children.FirstOrDefault(c => c.NodeType == nodeType);
                    if (childTemplateNode == null)
                    {
                        continue;
                    }
                    if (lastParent.Children.Exists(c => c.Name == item.Name))
                        continue;
                    var name = item.Name;
                    var childNode = new S_M_BidOffer_CBS();
                    childNode.Name = name;
                    childNode.Code = item.Code;
                    childNode.ID = FormulaHelper.CreateGuid();
                    childNode.NodeType = nodeType;
                    childNode.CBSDefineID = childTemplateNode.ID;
                    childNode.SortIndex = item.SortIndex;
                    lastParent.AddChild(childNode);
                    structDic.SetValue(childTemplateNode.ID, childNode);
                }
            }
            this.EPCEntites.SaveChanges();
            return Json("");
        }
        #endregion


        #region Excel导入

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var BidOfferID = this.Request["BidOfferID"];
            var bid = this.GetEntityByID<S_M_BidOffer>(BidOfferID);
            var enumItems = EnumBaseHelper.GetEnumDef("Base.BOMMajor");
            var errors = excelData.Vaildate(e =>
            {
                if (bid == null)
                {
                    e.IsValid = false;
                    e.ErrorText = "没有找到投标报价记录，无法导入EXCEL";
                }
                else
                {
                    if (e.FieldName == "NodeType" && String.IsNullOrEmpty(e.Value))
                    {
                        if (e.Record["MajorName"] == null || e.Record["MajorName"] == DBNull.Value ||
                            String.IsNullOrEmpty(e.Record["MajorName"].ToString()))
                        {
                            e.IsValid = false;
                            e.ErrorText = "专业分类不能为空";
                        }
                    }
                    if (e.FieldName == "MajorName" && !String.IsNullOrEmpty(e.Value))
                    {
                        if (enumItems.EnumItem.Count(c => c.Name == e.Value) == 0)
                        {
                            e.IsValid = false;
                            e.ErrorText = "没有找到对应的专业分类";
                        }
                    }
                    if (e.FieldName == "Name" && String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "名称不能为空";
                    }
                }
            });
            return Json(errors);
        }

        public JsonResult SaveExcelData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(tempdata["data"]);
            var BidOfferID = this.Request["BidOfferID"];
            var bid = this.GetEntityByID<S_M_BidOffer>(BidOfferID);
            if (bid == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到投标报价记录，无法导入EXCEL");
            }
            var deviceRoot = bid.S_M_BidOffer_CBS.Where(d => d.CBSType == "Device").OrderBy(d => d.CBSFullID).FirstOrDefault();
            if (deviceRoot == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到投标报价记录中的设备费用节点，无法导入！");
            }
            var BOMMajor = EnumBaseHelper.GetEnumDef("Base.BOMMajor");
            if (BOMMajor == null) throw new Formula.Exceptions.BusinessValidationException("没有找到对应的采购专业分类枚举【Base.BOMMajor】");
            var enumItems = BOMMajor.EnumItem.ToList();

            if (deviceRoot.CBSTemplateNode == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("没有找到设备材料费用的定义科目信息，导入失败");
            }
            var deviceTemplate = deviceRoot.CBSTemplateNode;
            var structDic = new Dictionary<string, S_M_BidOffer_CBS>();
            structDic.SetValue(deviceTemplate.ID, deviceRoot);
            S_M_BidOffer_CBS lastParent = null;
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var nodeType = getNodeType(item);
                if (nodeType != "Detail")
                {
                    var templateNode = deviceTemplate.AllChildren.FirstOrDefault(c => c.NodeType == nodeType);
                    if (templateNode == null) continue;
                    if (templateNode.Parent == null) continue;
                    lastParent = structDic.GetValue(templateNode.Parent.ID);
                    if (lastParent == null || lastParent.CBSTemplateNode == null) continue;
                    var childTemplateNode = lastParent.CBSTemplateNode.Children.FirstOrDefault(c => c.NodeType == nodeType);
                    if (childTemplateNode == null)
                    {
                        continue;
                    }
                    var name = item.GetValue("Name");
                    var childNode = new S_M_BidOffer_CBS();
                    this.UpdateEntity<S_M_BidOffer_CBS>(childNode, item);
                    childNode.ID = FormulaHelper.CreateGuid();
                    childNode.NodeType = nodeType;
                    childNode.CBSDefineID = childTemplateNode.ID;
                    lastParent.AddChild(childNode);
                    structDic.SetValue(childTemplateNode.ID, childNode);
                }
                else
                {
                    var templateNode = deviceTemplate.AllChildren.LastOrDefault();
                    if (templateNode == null) continue;
                    var cbsNode = structDic.GetValue(templateNode.ID);
                    if (cbsNode == null) continue;
                    var majorName = item.GetValue("MajorName");
                    var codeItem = enumItems.FirstOrDefault(c => c.Name == majorName);
                    if (codeItem == null) { throw new Formula.Exceptions.BusinessValidationException("没有找到对应的专业内容【" + majorName + "】"); }
                    var majorCode = codeItem.Code;
                    item.SetValue("MajorCode", majorCode);
                    var detail = new S_M_BidOffer_CBS_Detail();
                    FormulaHelper.UpdateEntity<S_M_BidOffer_CBS_Detail>(detail, item);
                    detail.BidOfferID = bid.ID;
                    detail.ID = FormulaHelper.CreateGuid();
                    detail.CBSParentID = cbsNode.CBSID;
                    detail.BomID = FormulaHelper.CreateGuid();
                    detail.OfferCBSFullID = cbsNode.CBSFullID;
                    detail.OfferCBSID = cbsNode.ID;
                    detail.FullID = detail.ID;
                    detail.SortIndex = i;
                    cbsNode.S_M_BidOffer_CBS_Detail.Add(detail);
                }
            }
            this.EPCEntites.SaveChanges();
            return Json("Success");
        }

        private string getNodeType(Dictionary<string, object> dic)
        {
            var enumDefine = EnumBaseHelper.GetEnumDef("Base.PBSType");
            if (dic.ContainsKey("NodeType") && String.IsNullOrEmpty(dic.GetValue("NodeType")))
            {
                return "Detail";
            }
            else if (!String.IsNullOrEmpty(dic.GetValue("NodeType")))
            {
                var name = dic.GetValue("NodeType");
                var item = enumDefine.EnumItem.FirstOrDefault(c => c.Name == name);
                if (item != null)
                    return item.Code;
                else
                    return "Detail";
            }
            else
            {
                return "Detail";
            }
        }
        #endregion

    }
}
