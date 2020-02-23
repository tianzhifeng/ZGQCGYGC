using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Config.Logic;
using Formula.Helper;

namespace Project.Logic.Domain
{
    public partial class T_EXE_PublishApply
    {
        /// <summary>
        /// 流程启动的反写数据的逻辑
        /// </summary>
        public void SetProductPrintState(string printState)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            if (!string.IsNullOrEmpty(printState))
            {
                //获取出图成果
                var products = this.T_EXE_PublishApply_Products.ToList();
                var pids = products.Select(a => a.ProductID).Distinct().ToList();
                var productList = entities.Set<S_E_Product>().Where(a => pids.Contains(a.ID)).ToList();
                var versionList = entities.Set<S_E_ProductVersion>().Where(a => pids.Contains(a.ProductID)).ToList();
                foreach (var product in products)
                {
                    var p = productList.FirstOrDefault(a => a.ID == product.ProductID && a.Version == product.ProductVersion);
                    if (p != null)
                    {
                        p.PrintState = printState;
                        p.SignState = printState;
                    }
                    var version = versionList.FirstOrDefault(a => a.ProductID == product.ProductID && a.Version == product.ProductVersion);
                    if (version != null)
                    {
                        version.PrintState = printState;
                        version.SignState = printState;
                    }
                }
            }
        }

        /// <summary>
        ///设置图纸数量规格
        /// </summary>
        public void SetPublishMainInfo()
        {
            //获取子表信息
            var detailList = this.T_EXE_PublishApply_Products.ToList();
            //获取A0数量
            if (detailList.Count > 0)
            {
                //this.A0 = detailList.Where(c => c.Specifications == "A0").Sum(c => c.Count);
                //this.A1 = detailList.Where(c => c.Specifications == "A1").Sum(c => c.Count);
                //this.A2 = detailList.Where(c => c.Specifications == "A2").Sum(c => c.Count);
                //this.A3 = detailList.Where(c => c.Specifications == "A3").Sum(c => c.Count);
                //this.A4 = detailList.Where(c => c.Specifications == "A4").Sum(c => c.Count);
            }
        }

        /// <summary>
        /// 设置出图信息
        /// </summary>
        public string SetPublishInfo()
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            var publishEntity = new S_E_PublishInfo();
            //publishEntity.A0 = this.A0;
            //publishEntity.A1 = this.A1;
            //publishEntity.A2 = this.A2;
            //publishEntity.A3 = this.A3;
            //publishEntity.A4 = this.A4;
            if (this.ApplyDate != null)
                publishEntity.PublishDate = Convert.ToDateTime(this.ApplyDate);
            if (publishEntity.PublishDate != null)
            {
                publishEntity.BelongYear = publishEntity.PublishDate.Year;
                publishEntity.BelongMonth = publishEntity.PublishDate.Month;
                publishEntity.BelongQuarter = (int)((publishEntity.PublishDate.Month - 1) / 3) + 1;
            }
            if (!string.IsNullOrEmpty(this.MajorCode))
            {
                var majors = BaseConfigFO.GetWBSAttrList(WBSNodeType.Major);
                var majorAttr = majors.SingleOrDefault(d => d.Code == this.MajorCode);
                if (majorAttr == null) throw new Formula.Exceptions.BusinessException("【" + this.MajorCode + "】专业未定义，无法进行出图操作");
                publishEntity.MajorValue = this.MajorCode;
                publishEntity.MajorName = majorAttr.Name;
            }
            publishEntity.ProjectInfoCode = this.ProjectCode;
            publishEntity.ProjectInfoID = this.ProjectInfoID;
            publishEntity.ProjectInfoName = this.ProjectName;
            //publishEntity.PublishType = this.FileType;
            publishEntity.RelateInfoID = this.ID;
            publishEntity.SummaryCost = this.RealCostAmount;

            var a0 = publishEntity.A0.HasValue ? publishEntity.A0.Value : 0m;
            var a1 = publishEntity.A1.HasValue ? publishEntity.A1.Value : 0m;
            var a2 = publishEntity.A2.HasValue ? publishEntity.A2.Value : 0m;
            var a3 = publishEntity.A3.HasValue ? publishEntity.A3.Value : 0m;
            var a4 = publishEntity.A4.HasValue ? publishEntity.A4.Value : 0m;

            var toA1 = a0 * 2 + a1 + a2 * 0.5m + a3 * 0.25m + a4 * 0.125m;
            publishEntity.ToA1 = this.ToA1;
            publishEntity.UniPrice = this.Price;
            publishEntity.ID = FormulaHelper.CreateGuid();
            entities.Set<S_E_PublishInfo>().Add(publishEntity);
            SetPublishInfoDetail(publishEntity.ID);
            return publishEntity.ID;
        }

        public void SetProductInfo()
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            var products = this.T_EXE_PublishApply_Products.ToList();
            var pids = products.Select(a => a.ProductID).Distinct().ToList();
            var productList = entities.Set<S_E_Product>().Where(a => pids.Contains(a.ID)).ToList();
            var versionList = entities.Set<S_E_ProductVersion>().Where(a => pids.Contains(a.ProductID)).ToList();
            foreach (var product in products)
            {
                var printCount = entities.Set<T_EXE_PublishApply_Products>().Where(d => d.ProductID == product.ProductID).Count();
                var p = productList.FirstOrDefault(a => a.ID == product.ProductID && a.Version == product.ProductVersion);
                if (p != null)
                {
                    p.PlotSealGroup = product.PlotSealGroup;
                    p.PlotSealGroupName = product.PlotSealGroupName;
                    p.PrintCount = printCount;
                }
                var version = versionList.FirstOrDefault(a => a.ProductID == product.ProductID && a.Version == product.ProductVersion);
                if (version != null)
                {
                    version.PlotSealGroup = product.PlotSealGroup;
                    version.PlotSealGroupName = product.PlotSealGroupName;
                    version.PrintCount = printCount;
                }
            }
        }

        private void SetPublishInfoDetail(string S_E_PublishInfoID)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            var products = this.T_EXE_PublishApply_Products.ToList();
            var pids = products.Select(a => a.ProductID).Distinct().ToList();
            var productList = entities.Set<S_E_Product>().Where(a => pids.Contains(a.ID)).ToList();
            var versionList = entities.Set<S_E_ProductVersion>().Where(a => pids.Contains(a.ProductID)).ToList();
            foreach (var applyProduct in products)
            {
                S_E_PublishInfoDetail detail = new S_E_PublishInfoDetail();
                detail.S_E_PublishInfoID = S_E_PublishInfoID;
                detail.ProductID = applyProduct.ID;
                detail.ProductCode = applyProduct.ProductCode;
                detail.ProductName = applyProduct.ProductName;
                detail.ProductVersion = applyProduct.ProductVersion;

                detail.ProjectInfoName = this.ProjectName;
                detail.ProjectCode = this.ProjectCode;
                detail.ProjectManager = this.ProjectManager;
                detail.MajorCode = this.MajorCode;
                detail.StepName = this.StepName;
                detail.SerialNumber = this.SerialNumber;
                //detail.PublishDate = this.PublishDate;
                S_I_ProjectInfo proj = entities.Set<S_I_ProjectInfo>().Find(this.ProjectInfoID);
                if (proj == null)
                {
                    throw new Formula.Exceptions.BusinessException("未找到ID为" + this.ProjectInfoID + "的S_I_ProjectInfo表");
                }
                detail.ChargeDeptName = proj.ChargeDeptName;

                var p = productList.FirstOrDefault(a => a.ID == applyProduct.ProductID && a.Version == applyProduct.ProductVersion);
                if (p != null)
                {
                    detail.PdfFile = p.PdfFile;
                    detail.PlotFile = p.PlotFile;
                }
                else
                {
                    var version = versionList.FirstOrDefault(a => a.ProductID == applyProduct.ProductID && a.Version == applyProduct.ProductVersion);
                    if (version != null)
                    {
                        {
                            detail.PdfFile = version.PdfFile;
                            detail.PlotFile = version.PlotFile;
                        }
                    }
                }
                //TODO 
                //detail.DesingerName
                detail.Printed = false;
                detail.ID = FormulaHelper.CreateGuid();
                entities.Set<S_E_PublishInfoDetail>().Add(detail);
            }
        }

        public void SetProductSignUser()
        {
            IUserService service = FormulaHelper.GetService<IUserService>();
            var auditStateArray = Enum.GetNames(typeof(Project.Logic.AuditState));
            var projectEntities = FormulaHelper.GetEntities<ProjectEntities>();
            var allProducts = projectEntities.Set<S_E_Product>().Where(a => a.ProjectInfoID == this.Project).ToList();
            foreach (var p in this.T_EXE_PublishApply_Products)
            {
                if (p.CanSetUser.HasValue && p.CanSetUser.Value == 1)
                {
                    var pdic = p.ToDic();
                    var product = allProducts.FirstOrDefault(a => a.ID == p.ProductID);
                    if (product != null)
                    {
                        List<AuditUserInfo> list = new List<AuditUserInfo>();
                        foreach (var item in auditStateArray)
                        {
                            var value = pdic.GetValue(item);
                            if (!string.IsNullOrEmpty(value))
                            {
                                var users = value.Split(',');
                                foreach (var user in users)
                                {
                                    var u = service.GetUserInfoByID(user);
                                    if (u != null)
                                    {
                                        AuditUserInfo userInfo = new AuditUserInfo(item, user, u.UserName);
                                        userInfo.SignDate = DateTime.Now;
                                        list.Add(userInfo);
                                    }
                                }
                            }
                        }
                        var designers = list.Where(a => a.ActivityKey == AuditState.Design.ToString()
                            || a.ActivityKey == AuditState.Designer.ToString());
                        var collactors = list.Where(a => a.ActivityKey == AuditState.Collact.ToString()
                            || a.ActivityKey == AuditState.Collactor.ToString());
                        var auditors = list.Where(a => a.ActivityKey == AuditState.Audit.ToString()
                            || a.ActivityKey == AuditState.Auditor.ToString());
                        var approvers = list.Where(a => a.ActivityKey == AuditState.Approve.ToString()
                            || a.ActivityKey == AuditState.Approver.ToString());
                        product.AuditSignUser = JsonHelper.ToJson(list);
                        product.Designer = string.Join(",", designers.Select(a => a.UserID));
                        product.DesignerName = string.Join(",", designers.Select(a => a.UserName));
                        product.Collactor = string.Join(",", collactors.Select(a => a.UserID));
                        product.CollactorName = string.Join(",", collactors.Select(a => a.UserName));
                        product.Auditor = string.Join(",", auditors.Select(a => a.UserID));
                        product.AuditorName = string.Join(",", auditors.Select(a => a.UserName));
                        product.Approver = string.Join(",", approvers.Select(a => a.UserID));
                        product.ApproverName = string.Join(",", approvers.Select(a => a.UserName));

                        product.UpdateVersison();
                    }
                }
            }
        }
    }
}
