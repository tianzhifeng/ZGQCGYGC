using EPC.Logic.Domain;
using Config.Logic;
using Formula;
using Formula.Helper;
using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Formula.ImportExport;
using EPC.Logic;

namespace EPC.Areas.Construction.Controllers
{
    public class BOQController : EPCController<S_C_BOQ>
    {
        public override ActionResult List()
        {
            ViewBag.IsEdit = GetQueryString("IsEdit");

            string contractInfoID = this.GetQueryString("ContractInfoID");
            var contractInfo = this.GetEntityByID<S_P_ContractInfo>(contractInfoID);
            if (contractInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的分包合同信息");

            bool flowEnd = true;
            var version = entities.Set<S_C_BOQ_Version>()
                .Where(a => a.ContractInfoID == contractInfoID)
                .OrderByDescending(a => a.ID).FirstOrDefault();

            //没有发布任何版本
            if (version == null)
            {
                ViewBag.VersionID = "";
                ViewBag.VersionNo = "0";
                flowEnd = true;
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
            return View();
        }

        //界面dataGrid显示数据
        public JsonResult GetBOQList(QueryBuilder qb, string VersionID, string ShowType)
        {
            var boqVersionList = GetSearchList(qb, VersionID, ShowType);
            return Json(boqVersionList);
        }

        //导出excel而定义的函数
        public JsonResult GetVersionList(QueryBuilder qb, string VersionID, string ShowType)
        {
            var boqVersionList = GetSearchList(qb, VersionID, ShowType);
            return Json(new { data = boqVersionList });
        }

        public JsonResult DetectDelete(string detailData)
        {
            List<Dictionary<string, object>> dics = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(detailData);
            foreach (var dic in dics)
            {
                string boqID = dic.GetValue("BOQID");
                S_C_BOQ boq = entities.Set<S_C_BOQ>().SingleOrDefault(a => a.ID == boqID);

                if (boq != null && boq.Locked)
                {
                    throw new Formula.Exceptions.BusinessValidationException("【" + dic.GetValue("Name") + "】清单已被锁定(应用于计量单中)不能删除");
                }
            }

            return Json("");
        }

        public JsonResult ThisSaveList(string listData, string versionID)
        {
            var list = JsonHelper.ToList(listData);

            PriceValidate(list, versionID);

            foreach (var item in list)
            {
                string dataStateFromUICtrl = item.GetValue("_state").ToLower();
                var data = this.GetEntityByID<S_C_BOQ_Version_Detail>(item.GetValue("ID"));

                if (dataStateFromUICtrl == "removed")
                {
                    if (data != null)
                    {
                        //删除的数据就是该未发布版(暂存)新增数据
                        //则直接删除数据库数据
                        if (data.ModifyState == "Add")
                        {
                            string id = item.GetValue("ID");
                            entities.Set<S_C_BOQ_Version_Detail>().Delete(a => a.ID == id);
                        }
                        else
                        {
                            string id = item.GetValue("BOQID");
                            S_C_BOQ boq = entities.Set<S_C_BOQ>().SingleOrDefault(a => a.ID == id);
                            if (boq != null && boq.Locked)
                            {
                                throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】清单已被锁定(应用于计量单中)不能删除");
                            }
                            data.ModifyState = "Remove";
                        }
                    }
                }
                else if (dataStateFromUICtrl == "modified")
                {
                    if (data != null)
                    {
                        string id = item.GetValue("BOQID");
                        S_C_BOQ boq = entities.Set<S_C_BOQ>().SingleOrDefault(a => a.ID == id);
                        if (boq != null && boq.Locked)
                        {
                            decimal tmp = 0;
                            if (decimal.TryParse(item.GetValue("Quantity"), out tmp))
                            {
                                if (tmp < boq.CheckQuantityTotal)
                                {
                                    throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】清单已经计量" + boq.CheckQuantityTotal + boq.Unit + ",因此不能比这个数值小");
                                }
                            }

                            if (decimal.TryParse(item.GetValue("Price"), out tmp))
                            {
                                if (tmp < boq.CheckPriceTotal)
                                {
                                    throw new Formula.Exceptions.BusinessValidationException("【" + item.GetValue("Name") + "】清单已经计价" + boq.CheckPriceTotal + "元,因此不能比这个数值小");
                                }
                            }
                        }
                        this.UpdateEntity<S_C_BOQ_Version_Detail>(data, item);
                        //Add的部分始终保持为add的state,不能混为修改项
                        //只找normal的state变为modify
                        if (data.ModifyState == "Normal")
                            data.ModifyState = "Modify";
                    }
                }
                else if (dataStateFromUICtrl == "added")
                {
                    S_C_BOQ_Version_Detail detail = new S_C_BOQ_Version_Detail();
                    detail.ID = FormulaHelper.CreateGuid();
                    detail.BOQID = detail.ID;
                    detail.VersionID = versionID;

                    var details = this.entities.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == versionID).ToList();
                    if (details.Count == 0)
                        detail.SortIndex = 0;
                    else
                    {
                        var maxSortIndex = details.Max(c => c.SortIndex);
                        detail.SortIndex = maxSortIndex + 0.001;
                    }

                    detail.Name = item.GetValue("Name");
                    detail.Code = item.GetValue("Code");
                    detail.Unit = item.GetValue("Unit");
                    detail.UnitPrice = 0;
                    detail.Quantity = 0;
                    detail.Price = 0;
                    decimal tmp = 0;
                    if (decimal.TryParse(item.GetValue("UnitPrice"), out tmp))
                        detail.UnitPrice = tmp;
                    if (decimal.TryParse(item.GetValue("Quantity"), out tmp))
                        detail.Quantity = tmp;
                    if (decimal.TryParse(item.GetValue("Price"), out tmp))
                        detail.Price = tmp;
                    detail.Property = item.GetValue("Property");
                    detail.Remark = item.GetValue("Remark");
                    detail.CreateDate = DateTime.Now;
                    detail.CreateUserID = CurrentUserInfo.UserID;
                    detail.CreateUser = CurrentUserInfo.UserName;
                    detail.ModifyState = "Add";
                    entities.Set<S_C_BOQ_Version_Detail>().Add(detail);
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult InsertDetail(string versionID, string targetID) 
        {
            //首次
            if (string.IsNullOrEmpty(targetID))
            {
                if (entities.Set<S_C_BOQ_Version_Detail>().Any(a => a.VersionID == versionID))
                {
                    throw new Formula.Exceptions.BusinessValidationException("没有指定插入位置的ID");
                }
                else
                {
                    S_C_BOQ_Version_Detail newDetail = new S_C_BOQ_Version_Detail();
                    newDetail.ID = FormulaHelper.CreateGuid();
                    newDetail.BOQID = newDetail.ID;
                    newDetail.Name = "新增清单";
                    newDetail.VersionID = versionID;
                    newDetail.Quantity = 0;
                    newDetail.SortIndex = 1;
                    newDetail.CreateDate = DateTime.Now;
                    newDetail.CreateUser = CurrentUserInfo.UserName;
                    newDetail.CreateUserID = CurrentUserInfo.UserID;
                    newDetail.ModifyState = "Add";
                    entities.Set<S_C_BOQ_Version_Detail>().Add(newDetail);
                }
            }
            //插入
            else
            {
                var targetDetail = entities.Set<S_C_BOQ_Version_Detail>().SingleOrDefault(a => a.ID == targetID);
                if (targetDetail == null)
                {
                    throw new Formula.Exceptions.BusinessValidationException("未能找到指定的数据清单，无法再后面插入新行");
                }

                var targetDetailNext = entities.Set<S_C_BOQ_Version_Detail>()
                    .Where(a => a.SortIndex > targetDetail.SortIndex && a.VersionID == targetDetail.VersionID).OrderBy(a => a.SortIndex).FirstOrDefault();

                double newDetailSortIndex = targetDetail.SortIndex + 1;
                if (targetDetailNext != null)
                {
                    newDetailSortIndex = (targetDetailNext.SortIndex + targetDetail.SortIndex) / 2;
                }
                S_C_BOQ_Version_Detail newDetail = new S_C_BOQ_Version_Detail();
                newDetail.ID = FormulaHelper.CreateGuid();
                newDetail.BOQID = newDetail.ID;
                newDetail.Name = "新增清单";
                newDetail.VersionID = targetDetail.VersionID;
                newDetail.Quantity = 0;
                newDetail.SortIndex = newDetailSortIndex;
                newDetail.CreateDate = DateTime.Now;
                newDetail.CreateUser = CurrentUserInfo.UserName;
                newDetail.CreateUserID = CurrentUserInfo.UserID;
                newDetail.ModifyState = "Add";
                entities.Set<S_C_BOQ_Version_Detail>().Add(newDetail);
            }
           
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult Move(string targetID, bool UpOrDown)
        {
            var targetDetail = entities.Set<S_C_BOQ_Version_Detail>().SingleOrDefault(a => a.ID == targetID);
            if (targetDetail == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未能找到指定的数据清单，无法移动");
            }

            if (UpOrDown)
            {
                var targetDetailPre = entities.Set<S_C_BOQ_Version_Detail>()
                .Where(a => a.SortIndex < targetDetail.SortIndex && a.VersionID == targetDetail.VersionID).OrderByDescending(a => a.SortIndex).FirstOrDefault();

                if (targetDetailPre != null)
                {
                    double tmpSortIndex = targetDetailPre.SortIndex;
                    targetDetailPre.SortIndex = targetDetail.SortIndex;
                    targetDetail.SortIndex = tmpSortIndex;
                }
            }
            else
            {
                var targetDetailNext = entities.Set<S_C_BOQ_Version_Detail>()
                   .Where(a => a.SortIndex > targetDetail.SortIndex && a.VersionID == targetDetail.VersionID).OrderBy(a => a.SortIndex).FirstOrDefault();

                if (targetDetailNext != null)
                {
                    double tmpSortIndex = targetDetailNext.SortIndex;
                    targetDetailNext.SortIndex = targetDetail.SortIndex;
                    targetDetail.SortIndex = tmpSortIndex;
                }
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult Revert(string VersionID)
        {
            var version = this.entities.Set<S_C_BOQ_Version>().FirstOrDefault(c => c.ID == VersionID);
            if (version == null)
                throw new Formula.Exceptions.BusinessValidationException("没有可编辑的版本，无法进行撤销操作");
            this.entities.Set<S_C_BOQ_Version_Detail>().Delete(a => a.VersionID == VersionID);
            this.entities.Set<S_C_BOQ_Version>().Remove(version);
            this.entities.SaveChanges();
            return Json("");
        }

        public JsonResult RevertNode(string ListData)
        {
            var list = JsonHelper.ToList(ListData);
            var result = new List<Dictionary<string, object>>();
            foreach (var item in list)
            {
                var detail = entities.Set<S_C_BOQ_Version_Detail>().Find(item.GetValue("ID"));
                if (detail == null) continue;
                var versionID = detail.VersionID;  //记录下版本ID，以免恢复上一条记录时候，将版本ID一起改变
                //如果是新增的记录，则直接删除
                if (detail.ModifyState == "Add")
                {
                    entities.Set<S_C_BOQ_Version_Detail>().Remove(detail);
                    var dic = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(detail);
                    result.Add(dic);
                }
                else
                {
                    string contractInfoID = this.GetQueryString("ContractInfoID");
                    //非新增的记录，要去找到上一个审批完成的版本，如果上一个审批完成的版本没有
                    //则获取初始化的BOM数据中的内容来恢复，如果都没有，则不恢复。
                    var lastVersion = entities.Set<S_C_BOQ_Version>().Where(a => a.FlowPhase == "End" && a.ContractInfoID == contractInfoID).OrderByDescending(a => a.ID).FirstOrDefault();
                    if (lastVersion == null)
                    {
                        var boq = this.GetEntityByID<S_C_BOQ>(detail.BOQID);
                        if (boq != null)
                        {
                            var boqDic = FormulaHelper.ModelToDic<S_C_BOQ>(boq);
                            this.UpdateEntity<S_C_BOQ_Version_Detail>(detail, boqDic);
                            detail.BOQID = boq.ID;
                            detail.VersionID = versionID;
                        }
                    }
                    else
                    {
                        var lastDetail = entities.Set<S_C_BOQ_Version_Detail>().FirstOrDefault(c => c.BOQID == detail.BOQID);
                        if (lastDetail != null)
                        {
                            var detailDic = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(lastDetail);
                            this.UpdateEntity<S_C_BOQ_Version_Detail>(detail, detailDic);
                            detail.VersionID = versionID;
                        }
                    }
                    detail.ModifyState = "Normal";
                    var dic = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(detail);
                    result.Add(dic);
                }
            }
            this.entities.SaveChanges();
            return Json(result);
        }

        public JsonResult UpgradeBOQ(string ContractInfoID)
        {
            string contractInfoID = this.GetQueryString("ContractInfoID");
            var contractInfo = this.GetEntityByID<S_P_ContractInfo>(contractInfoID);
            if (contractInfo == null) throw new Formula.Exceptions.BusinessValidationException("没有找到指定的分包合同信息");

            var versionQuery = entities.Set<S_C_BOQ_Version>().Where(a => a.ContractInfoID == contractInfoID);
            if (versionQuery.Count(c => c.FlowPhase != "End") > 0)
            {
                throw new Formula.Exceptions.BusinessValidationException("尚存在未发布的BOQ清单数据，无法再次升版");
            }

            var userInfo = FormulaHelper.GetUserInfo();
            string bomVersionPhase = BomVersionPhase.设计版本.ToString();
            var lastVersion = versionQuery.Where(c => c.FlowPhase == "End" && c.BOQType == bomVersionPhase).
                OrderByDescending(c => c.ID).FirstOrDefault();

            var newVersion = new S_C_BOQ_Version();
            newVersion.ID = FormulaHelper.CreateGuid();
            newVersion.CreateDate = DateTime.Now;
            newVersion.EngineeringInfoID = contractInfo.EngineeringInfoID;
            newVersion.CreateUser = userInfo.UserName;
            newVersion.CreateUserID = userInfo.UserID;
            newVersion.ContractInfoID = contractInfo.ID;
            newVersion.FlowPhase = "Start";
            newVersion.FlowInfo = "";
            if (lastVersion != null)
            {
                //var versionNum = String.IsNullOrEmpty(lastVersion.VersionNumber) || !Regex.IsMatch(lastVersion.VersionNumber, @"[1-9]\d*$") ? 0 :
                //    Convert.ToInt32(lastVersion.VersionNumber);
                var versionNum = lastVersion.VersionNumber;
                newVersion.VersionNumber = versionNum + 1;
                var list = entities.Set<S_C_BOQ_Version_Detail>().Where(c => c.ModifyState != "Remove" && c.VersionID == lastVersion.ID).ToList();
                foreach (var item in list)
                {
                    var detail = item.Clone<S_C_BOQ_Version_Detail>();
                    detail.ModifyState = "Normal";
                    detail.VersionID = newVersion.ID;
                    entities.Set<S_C_BOQ_Version_Detail>().Add(detail);
                }
            }
            else
            {
                newVersion.VersionNumber = 1;
            }
            newVersion.VersionName = "第【" + newVersion.VersionNumber + "】版分包合同清单";
            newVersion.BOQType = BomVersionPhase.设计版本.ToString();
            entities.Set<S_C_BOQ_Version>().Add(newVersion);
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult StandardImport(string listData)
        {
            var vID = GetQueryString("VersionID");
            S_C_BOQ_Version version = entities.Set<S_C_BOQ_Version>().Find(vID);
            if (version == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到ID为" + vID + "的S_C_BOQ_Version");
            }

            List<Dictionary<string, object>> dics = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(listData);

            var details = this.entities.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == vID).ToList();
            var maxindex = details.Count() == 0 ? 0 : details.Max(a => a.SortIndex);
            foreach (var item in dics)
            {
                S_C_BOQ_Version_Detail detail = new S_C_BOQ_Version_Detail();
                detail.ID = FormulaHelper.CreateGuid();
                detail.BOQID = detail.ID;
                detail.VersionID = version.ID;

                string code = item.GetValue("Code");
                //if (entities.Set<S_C_BOQ_Version_Detail>()
                //    .Any(a => a.Code == code))
                //{
                //    throw new Formula.Exceptions.BusinessValidationException("编号【" + code + "】重复");
                //}

                detail.Code = code;
                detail.Name = item.GetValue("Name");
                detail.Property = item.GetValue("Property");
                detail.Unit = item.GetValue("Unit");
                detail.Quantity = 0;
                detail.Price = 0;
                detail.UnitPrice = 0;
                detail.Remark = item.GetValue("Remark");

                detail.CreateDate = DateTime.Now;
                detail.CreateUserID = CurrentUserInfo.UserID;
                detail.CreateUser = CurrentUserInfo.UserName;
                detail.ModifyState = "Add";
                detail.SortIndex = maxindex + 0.001;
                maxindex = detail.SortIndex;

                entities.Set<S_C_BOQ_Version_Detail>().Add(detail);
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult ValidateData()
        {
            var reader = new System.IO.StreamReader(HttpContext.Request.InputStream);
            string data = reader.ReadToEnd();
            var tempdata = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            var excelData = JsonConvert.DeserializeObject<ExcelData>(tempdata["data"]);

            var vID = GetQueryString("VersionID");
            S_C_BOQ_Version quantity = entities.Set<S_C_BOQ_Version>().Find(vID);
            if (quantity == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到ID为" + vID + "的S_C_BOQ_Version");
            }

            List<string> tmpCodes = new List<string>();
            var errors = excelData.Vaildate(e =>
            {
                if (e.FieldName == "Code")
                {
                    if (String.IsNullOrEmpty(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "清单编号不能为空";
                    }
                    else if (tmpCodes.Contains(e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "excel表中清单编号重复";
                    }
                    else if (entities.Set<S_C_BOQ_Version_Detail>().Any(a => a.VersionID == vID && a.Code == e.Value))
                    {
                        e.IsValid = false;
                        e.ErrorText = "清单编号已存在";
                    }
                    else
                    {
                        tmpCodes.Add(e.Value);
                    }
                }
                else if (e.FieldName == "Name" && String.IsNullOrEmpty(e.Value))
                {
                    e.IsValid = false;
                    e.ErrorText = "名称不能为空";
                }
                else if (e.FieldName == "UnitPrice")
                {
                    decimal tmp = 0;
                    if (!String.IsNullOrEmpty(e.Value) && !decimal.TryParse(e.Value, out tmp))
                    {
                        e.IsValid = false;
                        e.ErrorText = "单价格式不对";
                    }
                }
                else if (e.FieldName == "Quantity")
                {
                    decimal tmp = 0;
                    if (!String.IsNullOrEmpty(e.Value) && !decimal.TryParse(e.Value, out tmp))
                    {
                        e.IsValid = false;
                        e.ErrorText = "数量格式不对";
                    }
                }
                else if (e.FieldName == "Price")
                {
                    decimal tmp = 0;
                    if (!String.IsNullOrEmpty(e.Value) && !decimal.TryParse(e.Value, out tmp))
                    {
                        e.IsValid = false;
                        e.ErrorText = "合价格式不对";
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

            var vID = GetQueryString("VersionID");
            S_C_BOQ_Version version = entities.Set<S_C_BOQ_Version>().Find(vID);
            if (version == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("未找到ID为" + vID + "的S_C_BOQ_Version");
            }

            var groups = list.GroupBy(a => a.GetValue("Code")).Select(g => new { g.Key, Counts = g.Count() }).ToList();
            if (groups.Count(a => a.Counts > 1) > 0)
                throw new Formula.Exceptions.BusinessValidationException("编号【" + groups.FirstOrDefault(a => a.Counts > 1).Key + "】重复");

            var details = this.entities.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == vID).ToList();
            var maxindex = details.Count() == 0 ? 0 : details.Max(a => a.SortIndex);
            foreach (var item in list)
            {
                string code = item.GetValue("Code");
                if (entities.Set<S_C_BOQ_Version_Detail>()
                    .Any(a => a.Code == code))
                {
                    throw new Formula.Exceptions.BusinessValidationException("编号【" + item.GetValue("Code") + "】重复");
                }

                S_C_BOQ_Version_Detail detail = new S_C_BOQ_Version_Detail();
                detail.ID = FormulaHelper.CreateGuid();
                detail.BOQID = detail.ID;
                detail.VersionID = version.ID;
                detail.Code = item.GetValue("Code");
                detail.Name = item.GetValue("Name");
                detail.Property = item.GetValue("Property");
                detail.Unit = item.GetValue("Unit");
                decimal tmp = 0;
                if (decimal.TryParse(item.GetValue("UnitPrice"), out tmp))
                    detail.UnitPrice = tmp;
                if (decimal.TryParse(item.GetValue("Quantity"), out tmp))
                    detail.Quantity = tmp;
                if (decimal.TryParse(item.GetValue("Price"), out tmp))
                    detail.Price = tmp;
                else
                    detail.Price = detail.UnitPrice * detail.Quantity;

                detail.Remark = item.GetValue("Remark");

                detail.CreateDate = DateTime.Now;
                detail.CreateUserID = CurrentUserInfo.UserID;
                detail.CreateUser = CurrentUserInfo.UserName;
                detail.ModifyState = "Add";
                detail.SortIndex = maxindex + 0.001;
                maxindex = detail.SortIndex;

                entities.Set<S_C_BOQ_Version_Detail>().Add(detail);
            }

            entities.SaveChanges();
            return Json("Success");
        }

        public JsonResult GetLatestBoqs(string boqIdList, bool bAllowNotExist)
        {
            List<string> ids = JsonConvert.DeserializeObject<List<string>>(boqIdList);
            var boqs = entities.Set<S_C_BOQ>().Where(a => ids.Contains(a.ID)).ToList();
            List<string> tmps = ids.Where(a => boqs.Any(b => b.ID != a)).ToList();
            if (!bAllowNotExist && tmps.Count > 0)
            {
                string tmp = "";
                foreach (string str in tmps)
                {
                    tmp += str + " ";
                }
                throw new Formula.Exceptions.BusinessValidationException("id为" + tmp + "工程量清单已不存在");
            }
            return Json(boqs);
        }

        public JsonResult GetLastDetailInfo(string ID, string VersionID)
        {
            var detail = this.GetEntityByID<S_C_BOQ_Version_Detail>(ID);
            var currDetailVersion = entities.Set<S_C_BOQ_Version>().Find(detail.VersionID);
            var result = new Dictionary<string, object>();
            if (detail != null && currDetailVersion != null)
            {
                var lastVersion = entities.Set<S_C_BOQ_Version>()
                    .Where(d => d.ContractInfoID == currDetailVersion.ContractInfoID && d.FlowPhase == "End" && d.VersionNumber < currDetailVersion.VersionNumber)
                    .OrderByDescending(c => c.VersionNumber).FirstOrDefault();

                if (lastVersion == null)
                {
                    var boq = this.GetEntityByID<S_C_BOQ>(detail.BOQID);
                    if (boq != null)
                    {
                        result = FormulaHelper.ModelToDic<S_C_BOQ>(boq);
                    }
                }
                else
                {
                    var lastDetail = entities.Set<S_C_BOQ_Version_Detail>().FirstOrDefault(a => a.BOQID == detail.BOQID && a.VersionID == lastVersion.ID);
                    if (lastDetail != null)
                    {
                        result = FormulaHelper.ModelToDic<S_C_BOQ_Version_Detail>(lastDetail);
                    }
                }
            }
            return Json(result);
        }

        public void PriceValidate(List<Dictionary<string, object>> list, string versionID)
        {
            string contractInfoID = this.GetQueryString("ContractInfoID");

            //合同金额
            S_P_ContractInfo contractInfo = entities.Set<S_P_ContractInfo>().Find(contractInfoID);
            if (contractInfo == null)
            {
                throw new Formula.Exceptions.BusinessValidationException("id为" + contractInfoID + "的合同已不存在");
            }

            decimal priceTotal = 0;
            List<string> ids = new List<string>();
            foreach (var item in list)
            {
                ids.Add(item.GetValue("ID"));
                string dataStateFromUICtrl = item.GetValue("_state").ToLower();

                decimal tmp = 0;
                decimal.TryParse(item.GetValue("Price"), out tmp);
                if (dataStateFromUICtrl == "removed")
                {
                    tmp = -tmp;
                }
                priceTotal += tmp;
            }

            //找出不在list(变更项)的数据
            decimal dbPriceTotal = entities.Set<S_C_BOQ_Version_Detail>().Where(a => !ids.Contains(a.ID) && a.VersionID == versionID).ToList().Sum(a => a.Price ?? 0);

            if ((priceTotal + dbPriceTotal) > (contractInfo.ContractAmount ?? 0))
            {
                throw new Formula.Exceptions.BusinessValidationException("清单总价已经超出了合同额" + contractInfo.ContractAmount + "元");
            }
        }

        private List<S_C_BOQ_Version_Detail> GetSearchList(QueryBuilder qb, string VersionID, string ShowType)
        {
            var boqVersionList = entities.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == VersionID);
            if (boqVersionList == null)
            {
                return new List<S_C_BOQ_Version_Detail>();
            }

            qb.SortField = "SortIndex"; qb.SortOrder = "asc";
            qb.PageSize = 0;
            if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "diff")
            {
                //只显示差异数据
                qb.Add("ModifyState", QueryMethod.NotEqual, "Normal");
                boqVersionList = this.entities.Set<S_C_BOQ_Version_Detail>().Where(a => a.VersionID == VersionID).Where(qb);

            }
            else if (!String.IsNullOrEmpty(ShowType) && ShowType.ToLower() == "new")
            {
                //只显示最新版本的数据，不体现差异
                qb.Add("ModifyState", QueryMethod.NotEqual, "Remove");
                boqVersionList = this.entities.Set<S_C_BOQ_Version_Detail>().Where(c => c.VersionID == VersionID).Where(qb);
            }
            else
            {
                //显示全部数据，体现差异
                boqVersionList = this.entities.Set<S_C_BOQ_Version_Detail>().Where(c => c.VersionID == VersionID).Where(qb);

            }

            return boqVersionList.ToList();
        }
    }
}
