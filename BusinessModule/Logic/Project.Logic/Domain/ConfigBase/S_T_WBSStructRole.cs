using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Formula.Helper;
using Formula;

namespace Project.Logic.Domain
{
    public partial class S_T_WBSStructRole
    {
        public void MoveUp()
        {
            if (this.SortIndex == 1) return;
            var preEty = this.S_T_WBSStructInfo.S_T_WBSStructRole.
                Where(d=>d.SortIndex<this.SortIndex).
                OrderByDescending(d => d.SortIndex).FirstOrDefault();
            if (preEty == null) return;
            var index = this.SortIndex;
            this.SortIndex = preEty.SortIndex;
            preEty.SortIndex = index;
        }

        public void MoveDown()
        {
            if (this.S_T_WBSStructInfo.S_T_WBSStructRole.Count + 1 == this.SortIndex) return;
            var afterEty = this.S_T_WBSStructInfo.S_T_WBSStructRole.
               Where(d => d.SortIndex > this.SortIndex).
               OrderBy(d => d.SortIndex).FirstOrDefault();
            if (afterEty == null) return;
            var index = this.SortIndex;
            this.SortIndex = afterEty.SortIndex;
            afterEty.SortIndex = index;
        }
    }
}
