using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using Config.Logic;

namespace Base.Logic.Domain
{
    public partial class S_A_User
    {
        public void UpdateSignImgFromFileStore(string fileId)
        {
            byte[] bytes = FileStoreHelper.GetFile(fileId);
            var obj = this.S_A_UserImg.SingleOrDefault(c => c.UserID == this.ID);
            if (obj == null)
            {
                obj = new S_A_UserImg();
                obj.ID = FormulaHelper.CreateGuid();
                obj.UserID = this.ID;
                this.S_A_UserImg.Add(obj);
            }
            obj.SignImg = bytes;
        }

        public void UpdatePictureFromFileStore(string fileId)
        {
            byte[] bytes = FileStoreHelper.GetFile(fileId);
            var obj = this.S_A_UserImg.SingleOrDefault(c => c.UserID == this.ID);
            if (obj == null)
            {
                obj = new S_A_UserImg();
                obj.ID = FormulaHelper.CreateGuid();
                obj.UserID = this.ID;
                this.S_A_UserImg.Add(obj);
            }
            obj.Picture = bytes;
        }
    }
}
