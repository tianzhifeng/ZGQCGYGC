using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Formula.Helper;
using Config.Logic;
using Config;

namespace DocSystem.Logic.Domain
{
    public partial class T_Borrow
    {
        public void SetDetailData(string ListData)
        {
            if (!string.IsNullOrEmpty(ListData))
            {
                var list = JsonHelper.ToList(ListData);
                var entities = FormulaHelper.GetEntities<DocConstEntities>();
                foreach (var item in list)
                {
                    T_Borrow_FileInfo FileInfo = new T_Borrow_FileInfo();
                    FileInfo.ID = FormulaHelper.CreateGuid();
                    FileInfo.T_BorrowID = this.ID;
                    FileInfo.FileID = item.GetValue("FileID");
                    FileInfo.FileName = item.GetValue("FileName");
                    FileInfo.CarInfoID = item.GetValue("ID");
                    entities.Set<T_Borrow_FileInfo>().Add(FileInfo);

                }
            }
        }

        /// <summary>
        /// 借阅流程的业务逻辑
        /// </summary>
        public void Push()
        {
            var entities = FormulaHelper.GetEntities<DocConstEntities>();
            this.PassDate = DateTime.Now;
            this.LendDate = DateTime.Now;
            this.BorrowState = ItemState.Finish.ToString();
            this.SynchS_BorrowDetail();
            this.SynchCarInfoState(ItemState.Finish.ToString());
        }

        public void SyncInstanceDataState()
        {
            var details = this.T_Borrow_FileInfo.ToList();
            var spaceList = new List<S_DOC_Space>();
            foreach (var item in details)
            {
                var space = spaceList.FirstOrDefault(a => a.ID == item.SpaceID);
                if (space == null)
                {
                    space = DocConfigHelper.CreateConfigSpaceByID(item.SpaceID);
                    if (space == null) continue;
                    spaceList.Add(space);
                }
                if (!String.IsNullOrEmpty(item.FileID))
                {
                    var file = new S_FileInfo(item.FileID, space);
                    if (file == null) continue;
                    file.DataEntity.SetValue("BorrowState", BorrowReturnState.Borrow.ToString());
                    file.DataEntity.SetValue("BorrowUserID", this.CreateUserID);
                    file.DataEntity.SetValue("BorrowUserName", this.CreateUserName);
                    file.Save(false);
                }
                else
                {
                    var node = new S_NodeInfo(item.NodeID, space);
                    if (node == null) continue;
                    node.DataEntity.SetValue("BorrowState", BorrowReturnState.Borrow.ToString());
                    node.DataEntity.SetValue("BorrowUserID", this.CreateUserID);
                    node.DataEntity.SetValue("BorrowUserName", this.CreateUserName);
                    node.Save(false);
                }
            }
        }

        public void SynchCarInfoState(string state)
        {
            var entities = FormulaHelper.GetEntities<DocConstEntities>();
            var details = this.T_Borrow_FileInfo.ToList();
            foreach (var detail in details)
            {
                var orginleEntity = entities.Set<S_CarInfo>().Find(detail.CarInfoID);
                orginleEntity.State = state;
            }

        }

        public void SynchS_BorrowDetail()
        {
            var entities = FormulaHelper.GetEntities<DocConstEntities>();
            var list = this.T_Borrow_FileInfo.ToList();
            foreach (var item in list)
            {                
                S_BorrowDetail entity = new S_BorrowDetail();
                entity.ID = FormulaHelper.CreateGuid();
                entity.LendDate = this.LendDate;
                entity.LendUserID = this.CreateUserID;
                entity.LendUserName = this.CreateUserName;
                entity.Name = item.FileName;
                entity.SpaceID = item.SpaceID;
                if (!String.IsNullOrEmpty(item.FileID))
                {
                    entity.DetailType = NodeType.File.ToString();
                    entity.RelateID = item.FileID;
                }
                else
                {
                    entity.DetailType = NodeType.Node.ToString();
                    entity.RelateID = item.NodeID;
                }
                entity.CreateUserID = this.CreateUserID;
                entity.CreateUserName = this.CreateUserName;
                entity.BorrowID = this.ID;
                entity.CreateDate = DateTime.Now;
                entity.ConfigID = item.ConfigID;
                entity.BorrowState = BorrowReturnState.Borrow.ToString();
                if (!string.IsNullOrEmpty(this.LendDate.ToString()))
                {
                    entity.BorrowExpireDate = Convert.ToDateTime(this.LendDate).AddDays(7);
                }                
                entities.Set<S_BorrowDetail>().Add(entity);
                AddBorrowLog(item);
            }
        }

        public void AddBorrowLog(T_Borrow_FileInfo item)
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
