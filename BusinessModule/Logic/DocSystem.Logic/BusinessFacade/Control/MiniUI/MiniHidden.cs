using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocSystem.Logic
{
    public class MiniHidden : BaseControl
    {
        public MiniHidden()
        { }

        public MiniHidden(string name)
        {
            this.Name = name;
        }

        public override string Render()
        {
            if(String.IsNullOrEmpty( this.Name))
                throw new Formula.Exceptions.BusinessException("必须指定 hidden 控件的 name 属性");
            string html = "<input {0} class=\"mini-hidden\" />";
            string attr = this.getAttr();
            return String.Format(html, attr);
        }
    }
}
