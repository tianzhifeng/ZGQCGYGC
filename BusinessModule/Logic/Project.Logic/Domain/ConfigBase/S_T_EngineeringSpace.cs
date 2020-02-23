using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic.Domain
{
    public partial class S_T_EngineeringSpace
    {
        public void Save()
        {
            var root = this.S_T_EngineeringSpaceRes.FirstOrDefault(d => d.Type == SpaceNodeType.Root.ToString());
            if (root == null) {
                root = new S_T_EngineeringSpaceRes();
                root.ID = Formula.FormulaHelper.CreateGuid();
                root.FullID = root.ID;
                root.ParentID = "";
                this.S_T_EngineeringSpaceRes.Add(root);
            }
            root.Name = this.Name;
            root.SortIndex = 0;
            root.Code = this.Code;
            root.Type = SpaceNodeType.Root.ToString();
            
        }
    }
}
