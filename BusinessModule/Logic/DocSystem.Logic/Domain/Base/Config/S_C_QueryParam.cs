using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_QueryParam
    {
        public void MoveUp()
        {
            var preParam = this.S_DOC_ListConfig.S_DOC_QueryParam.Where(d => d.QuerySort < this.QuerySort).OrderByDescending(d => d.QuerySort).FirstOrDefault();
            if (preParam == null) return;
            int sort = this.QuerySort;
            this.QuerySort = preParam.QuerySort;
            preParam.QuerySort = sort;
        }

        public void MoveDown()
        {
            var aftParam = this.S_DOC_ListConfig.S_DOC_QueryParam.Where(d => d.QuerySort > this.QuerySort).OrderBy(d => d.QuerySort).FirstOrDefault();
            if (aftParam == null) return;
            int sort = this.QuerySort;
            this.QuerySort = aftParam.QuerySort;
            aftParam.QuerySort = sort;
        }
    }
}
