using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Data;
using Formula;

namespace Project.Logic.Domain
{
    public partial class S_E_Product
    {
        /// <summary>
        /// 项目信息对象
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public S_I_ProjectInfo ProjectInfo
        {
            get
            {
                return this.S_W_WBS.S_I_ProjectInfo;
            }
        }

        /// <summary>
        /// 保存成果信息
        /// </summary>
        public void Save()
        {
            var entites = FormulaHelper.GetEntities<ProjectEntities>();

            this.Validate();

            if (entites.Entry(this).State == EntityState.Detached || entites.Entry(this).State == EntityState.Added)
            {
                var user = FormulaHelper.GetUserInfo();
                if (String.IsNullOrEmpty(this.WBSFullID))
                    this.WBSFullID = this.S_W_WBS.FullID;
                if (String.IsNullOrEmpty(this.FullID))
                    this.FullID = this.ID;
                if (String.IsNullOrEmpty(this.ProjectInfoID))
                    this.ProjectInfoID = this.S_W_WBS.ProjectInfoID;
                this.PrintState = "UnPrint";
                this.ArchiveState = TrueOrFalse.False.ToString();
                this.SignState = TrueOrFalse.False.ToString();
                this.IsCoSign = "";
                this.CoSignState = Project.Logic.CoSignState.NoSign.ToString();
                this.AuditState = Project.Logic.AuditState.Create.ToString();
                this.State = ProductState.Create.ToString();
                this.CreateDate = DateTime.Now;
                this.CreateUser = user.UserName;
                this.CreateUserID = user.UserID;
                this.SubmitDate = DateTime.Now;
                var majorNode = this.S_W_WBS.Seniorities.FirstOrDefault(d => d.WBSType == WBSNodeType.Major.ToString());
                if (majorNode != null)
                {
                    this.MajorValue = majorNode.WBSValue;
                    if (majorNode.Parent.WBSType == WBSNodeType.SubProject.ToString())
                    {
                        this.SubProjectInfo = majorNode.Parent.Name;
                    }
                }
                if (!this.Version.HasValue)
                    this.Version = 1;

                var version = new S_E_ProductVersion();
                FormulaHelper.UpdateModel(version, this);
                version.ID = FormulaHelper.CreateGuid();
                version.S_E_Product = this;
                version.ProductID = this.ID;
                this.S_E_ProductVersion.Add(version);
                entites.S_E_Product.Add(this);

            }
            else
            {
                UpdateVersison();
                SynMeetingSignProducts();
            }
        }

        /// <summary>
        /// 更新流程中会签单的成果数据
        /// </summary>
        public void SynMeetingSignProducts()
        {
            var entites = FormulaHelper.GetEntities<ProjectEntities>();
            var list = entites.Set<T_EXE_MettingSign_ResultList>().Where(a => a.T_EXE_MettingSign.FlowPhase != "End"
                && a.ProductID == this.ID).ToList();
            foreach (var item in list)
            {
                item.Name = this.Name;
                item.Code = this.Code;
                item.MainFile = this.MainFile;
                item.ProductVersion = this.Version;
            }
        }

        /// <summary>
        /// 同步更新成果当前版本的数据
        /// </summary>
        public void UpdateVersison()
        {
            var version = this.S_E_ProductVersion.FirstOrDefault(a => a.Version == this.Version);
            if (version != null)
            {
                var orgId = version.ID;
                FormulaHelper.UpdateModel(version, this);
                version.ID = orgId;
            }
        }

        public void Upgrade()
        {
            if (this.AuditState != Project.Logic.AuditState.Pass.ToString())
                throw new Exception("成果【" + this.Name.ToString() + "】正在校审中无法升版。");
            if (this.CoSignState == Project.Logic.CoSignState.Sign.ToString())
                throw new Exception("成果【" + this.Name.ToString() + "】正在会签中无法升版。");
            this.BatchID = "";
            this.AuditID = "";
            this.ChangeAuditID = "";
            this.AuditState = Project.Logic.AuditState.Create.ToString();
            this.AuditPassDate = null;
            this.ArchiveState = "False";
            this.ArchiveDate = null;
            this.SignState = "False";
            this.CoSignState = "NoSign";
            this.CoSignDate = null;
            this.PrintState = "UnPrint";
            this.Designer = "";
            this.DesignerName = "";
            this.Collactor = "";
            this.CollactorName = "";
            this.Auditor = "";
            this.AuditorName = "";
            this.Approver = "";
            this.ApproverName = "";
            this.AuditSignUser = null;
            this.CoSignUser = null;
            this.CounterSignAuditID = "";
            this.PDFSignPositionInfo = null;
            this.PDFAuditFiles = null;
            this.PlotSealGroup = "";
            this.PlotSealGroupName = "";
            this.SignPdfFile = "";
            this.FrameAllAttInfo = null;

            var version = new S_E_ProductVersion();
            FormulaHelper.UpdateModel(version, this);
            version.ID = FormulaHelper.CreateGuid();
            version.S_E_Product = this;
            version.ProductID = this.ID;
            var preVersion = this.S_E_ProductVersion.OrderByDescending(d => d.Version).FirstOrDefault();
            if (preVersion == null)
                version.Version = 1;
            else
                version.Version = preVersion.Version.HasValue ? Math.Floor(preVersion.Version.Value) + 1M : 1;
            this.S_E_ProductVersion.Add(version);
            this.Version = version.Version;
            SynMeetingSignProducts();
        }

        public void Change(string mainFile, string printFile, string attachments, string exploreFile)
        {
            if (this.S_W_WBS.WBSType == WBSNodeType.Work.ToString())
            {
                var task = this.S_W_WBS.S_W_TaskWork.FirstOrDefault();
                if (task != null && task.ChangeState != TaskWorkChangeState.ApplyFinish.ToString()
                    && task.ChangeState != TaskWorkChangeState.AuditStart.ToString())
                    throw new Formula.Exceptions.BusinessException("只有卷册变更申请完成，成果才能进行变更");
            }
            if (this.AuditState != Project.Logic.AuditState.Pass.ToString())
                throw new Formula.Exceptions.BusinessException("只有校审通过的成果才能变更");
            if (this.State != ProductState.Create.ToString())
                throw new Formula.Exceptions.BusinessException("只有未变更或未作废的成果才能变更");

            var version = new S_E_ProductVersion();
            FormulaHelper.UpdateModel(version, this);
            version.ID = FormulaHelper.CreateGuid();
            version.S_E_Product = this;
            version.ProductID = this.ID;
            version.Attachments = attachments;
            version.MainFile = mainFile;
            version.PdfFile = printFile;
            version.SwfFile = exploreFile;
            var preVersion = this.S_E_ProductVersion.OrderByDescending(d => d.Version).FirstOrDefault();
            if (preVersion == null)
                version.Version = 1;
            else
                version.Version = preVersion.Version.HasValue ? Math.Floor(preVersion.Version.Value) + 1M : 1;
            this.S_E_ProductVersion.Add(version);
            this.MainFile = version.MainFile;
            this.PdfFile = version.PdfFile;
            this.Attachments = version.Attachments;
            this.SwfFile = version.SwfFile;
            this.Version = version.Version;
            this.AuditState = Project.Logic.AuditState.Create.ToString();
            this.State = ProductState.Change.ToString();
            SynMeetingSignProducts();
        }

        public void RevertChange()
        {
            if (!this.Version.HasValue)
                throw new Formula.Exceptions.BusinessException("成果的版本号为空，无法撤销变更");
            if (this.State != ProductState.Change.ToString())
                throw new Formula.Exceptions.BusinessException("只有变更过的成果才能撤销变更");
            var orgVersionNo = Math.Floor(this.Version.Value);
            var orgVersion = this.S_E_ProductVersion.Where(a => a.Version.HasValue && a.Version.Value < orgVersionNo).OrderByDescending(a => a.Version).FirstOrDefault();
            if (orgVersion != null)
            {
                FormulaHelper.UpdateModel(this, orgVersion);
                this.ID = orgVersion.ProductID;

                var entites = this.GetDbContext<ProjectEntities>();
                entites.S_E_ProductVersion.Delete(a => a.ProductID == this.ID && a.Version.HasValue && a.Version.Value >= orgVersionNo);

                //子成果也撤销
                if (string.IsNullOrEmpty(this.ParentID))
                {
                    var children = this.S_W_WBS.S_E_Product.Where(a => a.FullID != null && a.FullID.Contains(this.ID) && a.ID != this.ID).ToList();
                    children.ForEach(a => a.RevertChange());
                }
            }
        }

        public void Invalid()
        {
            if (this.S_W_WBS.WBSType == WBSNodeType.Work.ToString())
            {
                var task = this.S_W_WBS.S_W_TaskWork.FirstOrDefault();
                if (task != null && task.ChangeState != TaskWorkChangeState.ApplyFinish.ToString()
                    && task.ChangeState != TaskWorkChangeState.AuditStart.ToString())
                    throw new Formula.Exceptions.BusinessException("只有卷册变更申请完成，成果才能进行作废");
            }
            if (this.AuditState != Project.Logic.AuditState.Pass.ToString())
                throw new Formula.Exceptions.BusinessException("只有校审通过的成果才能作废");
            if (this.State != ProductState.Create.ToString())
                throw new Formula.Exceptions.BusinessException("只有未变更或未作废的成果才能作废");
            this.State = ProductState.InInvalid.ToString();
            if (string.IsNullOrEmpty(this.ParentID))
            {
                var children = this.S_W_WBS.S_E_Product.Where(a => a.FullID != null && a.FullID.Contains(this.ID) && a.ID != this.ID).ToList();
                children.Update(a => a.State = this.State);
            }
        }

        public void RevertInvalid()
        {
            if (this.State != ProductState.InInvalid.ToString())
                throw new Formula.Exceptions.BusinessException("只有作废过的成果才能撤销作废");
            this.State = ProductState.Create.ToString();
            if (string.IsNullOrEmpty(this.ParentID))
            {
                var children = this.S_W_WBS.S_E_Product.Where(a => a.FullID != null && a.FullID.Contains(this.ID) && a.ID != this.ID).ToList();
                children.Update(a => a.State = this.State);
            }
        }

        public void Validate()
        {
            if (String.IsNullOrEmpty(this.WBSID))
                throw new Formula.Exceptions.BusinessException("必须为成果对象指定WBSID");
            if (this.S_W_WBS == null)
            {
                var wbs = this.GetDbContext<ProjectEntities>().Set<S_W_WBS>().SingleOrDefault(d => d.ID == this.WBSID);
                if (wbs == null) throw new Formula.Exceptions.BusinessException("未能找到ID为【" + this.WBSID + "】的WBS对象，添加成果失败");
                this.S_W_WBS = wbs;
            }
            var entities = this.GetDbContext<ProjectEntities>();
            if (entities.Set<S_E_Product>().Where(d => d.WBSID == this.WBSID && d.Code == this.Code && d.ID != this.ID).Count() > 0)
            {
                throw new Formula.Exceptions.BusinessException("已经存在图号为【" + this.Code + "】的成果，不能有重复图号");
            }
        }


        /// <summary>
        /// 删除成果信息
        /// </summary>
        public void Delete(bool validateMode = true)
        {
            var entites = FormulaHelper.GetEntities<ProjectEntities>();
            if (validateMode)
            {
                if (this.AuditState != Project.Logic.AuditState.Create.ToString()
                    && this.AuditState != Project.Logic.AuditState.Design.ToString()
                    && this.AuditState != Project.Logic.AuditState.Designer.ToString())
                {
                    throw new Formula.Exceptions.BusinessException("已经发起校审的成果不能删除。");
                }
                else if (this.State == ProductState.Change.ToString())
                    throw new Formula.Exceptions.BusinessException("已经变更的成果不能删除。");
                else if (this.State == ProductState.InInvalid.ToString() || this.State == ProductState.Invalid.ToString())
                    throw new Formula.Exceptions.BusinessException("已经作废的成果不能删除。");
                else if (this.Version != 1)
                {
                    throw new Formula.Exceptions.BusinessException("已经升版的成果不能删除。");
                }
            }
            entites.Set<S_E_Product>().Remove(this);
            onDelete();
        }

        partial void onDelete();
    }
}
