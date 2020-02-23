using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocSystem.Logic
{
    public class MiniLayout : BaseControl
    {
        string _id = "mylayout";
        public override string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }



        public override string Render()
        {
            throw new NotImplementedException();
        }
    }
}
