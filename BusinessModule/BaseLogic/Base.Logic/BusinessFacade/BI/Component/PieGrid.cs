using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Logic
{
    public class PieGrid : BaseComponent
    {
        public PieGrid(string BlockDefJson)
            : base(BlockDefJson)
        {
        }


        public override Dictionary<string, object> Render(string parameters = "", bool IsMobile = false)
        {
            this.FillDataSource(parameters);
            var result = new Dictionary<string, object>();
            return result;
        }
    }
}
