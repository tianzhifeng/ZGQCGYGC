using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.Collections;
using Formula.Helper;
using Formula;
using Config.Logic;
using Config;

namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_ListConfig
    {
        public void Delete()
        {
            var context = FormulaHelper.GetEntities<DocConfigEntities>();
            context.S_DOC_ListConfigDetail.Delete(d => d.ListConfigID == this.ID);
            context.S_DOC_QueryParam.Delete(d => d.ListConfigID == this.ID);
            context.S_DOC_ListConfig.Remove(this);
        }
    }
}
