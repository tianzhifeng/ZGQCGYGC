using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_ListConfigDetail
    {
        public void MoveUp()
        {
            var preAttr = this.S_DOC_ListConfig.S_DOC_ListConfigDetail.Where(d => d.DetailSort < this.DetailSort).OrderByDescending(d => d.DetailSort).FirstOrDefault();
            if (preAttr == null) return;
            int sort = this.DetailSort;
            this.DetailSort = preAttr.DetailSort;
            preAttr.DetailSort = sort;
        }

        public void MoveDown()
        {
            var aftAttr = this.S_DOC_ListConfig.S_DOC_ListConfigDetail.Where(d => d.DetailSort > this.DetailSort).OrderBy(d => d.DetailSort).FirstOrDefault();
            if (aftAttr == null) return;
            int sort = this.DetailSort;
            this.DetailSort = aftAttr.DetailSort;
            aftAttr.DetailSort = sort;
        }
    }
}
