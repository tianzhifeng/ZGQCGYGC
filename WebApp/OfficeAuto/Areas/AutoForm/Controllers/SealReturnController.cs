using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto.Areas.AutoForm.Controllers
{
    public class SealReturnController : OfficeAutoFormContorllor<T_Seal_Borrow>
    {
        public void ReturnSeal(string id)
        {
            var borrowInfo = this.GetEntityByID<T_Seal_Borrow>(id);
            if (borrowInfo == null) throw new Formula.Exceptions.BusinessException("该借用流程不存在，请重新确认或联系管理员！");

            borrowInfo.BorrowState = "已归还";
            borrowInfo.ReturnDate = DateTime.Now;
            this.BusinessEntities.SaveChanges();

            string sql = @"update T_Seal_SealInfo set State='正常',Borrower='',BorrowerName='',BorrowDate=null where ID in (
select SealID from T_Seal_Borrow_Detail where T_Seal_BorrowID = '" + borrowInfo.ID + "')";
            this.SQLDB.ExecuteNonQuery(sql);

        }
    }
}
