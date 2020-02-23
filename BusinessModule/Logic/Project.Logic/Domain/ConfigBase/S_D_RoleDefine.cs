using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial  class S_D_RoleDefine
    {
        public void Delete()
        {
            var context = this.GetDbContext<BaseConfigEntities>();
            var structRoleDefine = context.S_T_WBSStructRole.Where(d => d.RoleCode == this.RoleCode).ToList();
            foreach (var item in structRoleDefine)
                context.S_T_WBSStructRole.Delete(d => d.ID == item.ID);
            context.S_D_RoleDefine.Delete(d => d.ID == this.ID);
        }
    }
}
