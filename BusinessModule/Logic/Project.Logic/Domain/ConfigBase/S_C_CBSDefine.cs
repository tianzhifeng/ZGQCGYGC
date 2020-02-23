using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_C_CBSDefine
    {
        public void AddChild()
        { }

        S_C_CBSDefine _Parent;
        public S_C_CBSDefine Parent
        {
            get
            {
                if (_Parent == null)
                {
                    var context = this.GetDbContext<BaseConfigEntities>();
                    _Parent = context.S_C_CBSDefine.SingleOrDefault(d => d.ID == this.ParentID);
                }
                return _Parent;
            }
        }

        public void Delete()
        {
            var context = this.GetDbContext<BaseConfigEntities>();
            context.S_C_CBSDefine.Delete(d => d.FullID.StartsWith(this.FullID));
        }

        public void Save()
        {
            if (String.IsNullOrEmpty(this.ID))
                this.ID = Formula.FormulaHelper.CreateGuid();
            if (String.IsNullOrEmpty(this.ParentID))
                this.FullID = this.ID;
            else
            {
                if (this.Parent != null)
                {
                    this.FullID = this.Parent.FullID + "." + this.ID;
                }
            }
        }
    }
}
