using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;

namespace FixedAssets.Logic.Domain
{
    public partial class T_EmployeeWorkPost
    {

        //public void SynchPostInfo()
        //{
        //    var hrEntities = FormulaHelper.GetEntities<HREntities>();
        //    T_Employee employee = hrEntities.Set<T_Employee>().Find(this.EmployeeID);
        //    if (employee == null)
        //        throw new Formula.Exceptions.BusinessException("获取用户信息失败。");
        //    this.IsTheNewest = TrueOrFalse.True.ToString();
        //    var falseState = TrueOrFalse.False.ToString();
        //    hrEntities.Set<T_EmployeeWorkPost>().Where(c => c.EmployeeID == this.EmployeeID && c.ID != this.ID).Update(c => c.IsTheNewest = falseState);
        //    employee.Post = this.Post;
        //    employee.PostLevel = this.PostLevel;
        //}

    }
}
