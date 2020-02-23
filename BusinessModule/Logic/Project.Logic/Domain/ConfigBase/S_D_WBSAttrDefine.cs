using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;

namespace Project.Logic.Domain
{
    public partial class S_D_WBSAttrDefine
    {
        /// <summary>
        /// 为WBS属性定义关联部门信息
        /// </summary>
        /// <param name="DeptID">部门ID</param>
        /// <param name="DeptName">部门名称</param>
        public void SetRelationDept(string DeptID,string DeptName)
        {
            var entity = this.S_D_WBSAttrDeptInfo.FirstOrDefault(d => d.DeptID == DeptID);
            if (entity == null)
            {
                entity = new S_D_WBSAttrDeptInfo();
                entity.DeptID = DeptID;
                entity.DeptName = DeptName;
                entity.ID = FormulaHelper.CreateGuid();
                this.S_D_WBSAttrDeptInfo.Add(entity);
            }
        }

        /// <summary>
        /// 为WBS属性定义关联部门信息
        /// </summary>
        /// <param name="wbsAttrDept">部门关联对象</param>
        public void SetRelationDept(S_D_WBSAttrDeptInfo wbsAttrDept)
        {
            if (wbsAttrDept == null) throw new Formula.Exceptions.BusinessException("空的部门关联信息无法关联到WBS定义");
            if (this.S_D_WBSAttrDeptInfo.FirstOrDefault(d => d.DeptID == wbsAttrDept.DeptID) == null)
            {
                if(String.IsNullOrEmpty(wbsAttrDept.ID))
                    wbsAttrDept.ID = FormulaHelper.CreateGuid();
                this.S_D_WBSAttrDeptInfo.Add(wbsAttrDept);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Delete()
        {
            var context = this.GetDbContext<BaseConfigEntities>();
            context.S_D_WBSAttrDeptInfo.Delete(d => d.WBSAttrDefineID == this.ID);
        }
    }
}
