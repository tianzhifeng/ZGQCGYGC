using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Formula.Helper;
using Config.Logic;
namespace DocSystem.Logic.Domain
{
    public partial class T_Download
    {
        public void Download(S_FileInfo fileInfo, string SpaceID, string ArchiveType,bool CheckCar = false)
        {
            var user = FormulaHelper.GetUserInfo();
            var entities = FormulaHelper.GetEntities<DocConstEntities>();
            if (fileInfo.Space == null) throw new Formula.Exceptions.BusinessException("没有获取到申请文件的配置空间信息，无法进行申请下载");
            if (fileInfo.ConfigInfo == null) throw new Formula.Exceptions.BusinessException("没有获取到申请文件的文件配置类别信息，无法进行申请下载");
            var carItem = entities.Set<S_CarInfo>().FirstOrDefault(d => d.FileID == fileInfo.ID
                   && d.State != "Finish" && d.UserID == user.UserID && d.Type == "DownLoad");
            if (carItem == null)
            {
                carItem = new S_CarInfo();
                carItem.ID = FormulaHelper.CreateGuid();
                carItem.NodeID = fileInfo.NodeID;
                carItem.Type = ItemType.DownLoad.ToString();
                carItem.UserID = user.UserID;
                carItem.UserName = user.UserName;
                carItem.CreateDate = DateTime.Now;
                carItem.FileID = fileInfo.ID;
                carItem.SpaceID = SpaceID;
                carItem.ConfigID = fileInfo.ConfigInfo.ID;
                carItem.Name = fileInfo.CreateCarName();//fileInfo.Name;
                carItem.State = ItemState.New.ToString();
                carItem.CreateUser = user.UserName;
                carItem.CreateUserID = user.UserID;
                var attchments = new List<string>();
                ArchiveType = "MainFile,Attachments," + ArchiveType;
                foreach (var item in ArchiveType.Split(','))
                {
                    if (!string.IsNullOrEmpty(fileInfo.CurrentAttachment.DataEntity.GetValue(item)))
                        attchments.Add(fileInfo.CurrentAttachment.DataEntity.GetValue(item));
                }
                carItem.Attachments = string.Join(",", attchments);
                entities.Set<S_CarInfo>().Add(carItem);
            }
            else if (CheckCar)
            {
                throw new Formula.Exceptions.BusinessException("该文件已在下载车中，请前往下载车申请下载");
            }
        }

        /// <summary>
        /// 借阅流程的业务逻辑
        /// </summary>
        public void Push()
        {
            var entities = FormulaHelper.GetEntities<DocConstEntities>();
            this.PassDate = DateTime.Now;
            this.DownloadState = ItemState.Finish.ToString();
            this.SynchS_DownloadDetail();
            this.SynchCarInfoState(ItemState.Finish.ToString());
        }

        public void SynchCarInfoState(string state)
        {
            var entities = FormulaHelper.GetEntities<DocConstEntities>();
            var details = this.T_Download_FileInfo.ToList();
            foreach (var detail in details)
            {
                var orginleEntity = entities.Set<S_CarInfo>().Find(detail.CarInfoID);
                orginleEntity.State = state;
            }

        }

        public void SynchS_DownloadDetail()
        {
            var user = FormulaHelper.GetUserInfo();
            var entities = FormulaHelper.GetEntities<DocConstEntities>();
            var list = this.T_Download_FileInfo.ToList();
            foreach (var item in list)
            {
                S_DownloadDetail entity = new S_DownloadDetail();
                entity.ID = FormulaHelper.CreateGuid();
                entity.Name = item.FileName;
                entity.FileID = item.FileID;
                entity.CreateUserID = this.CreateUserID;
                entity.CreateUserName = this.CreateUserName;
                entity.SpaceID = item.SpaceID;
                entity.DownloadID = this.ID;
                entity.UserDeptID = user.UserOrgID;
                entity.UserDeptName = user.UserOrgName;
                entity.CreateDate = DateTime.Now;
                entity.ConfigID = item.ConfigID;
                entity.DownloadState = ItemState.Finish.ToString();
                entity.DownloadExpireDate = DateTime.Now.AddDays(7);
                entity.PassDate = this.PassDate;
                entity.Attachments = item.Attachments;
                entities.Set<S_DownloadDetail>().Add(entity);
                AddBorrowLog(item);
            }
        }

        public void AddBorrowLog(T_Download_FileInfo item)
        {
            var entities = FormulaHelper.GetEntities<DocConstEntities>();
            var user = FormulaHelper.GetUserInfo();
            S_DocumentLog log = entities.S_DocumentLog.Create();
            log.ID = FormulaHelper.CreateGuid();
            log.LogType = "Borrow";
            log.Name = item.FileName;
            log.NodeID = item.NodeID;
            log.SpaceID = item.SpaceID;
            log.ConfigID = item.ConfigID;
            log.CreateDate = DateTime.Now;
            log.CreateUserID = user.UserID;
            log.CreateUserName = user.UserName;
            log.FileID = item.FileID;
            log.FullNodeID = "";
            entities.S_DocumentLog.Add(log);
        }
    }
}
