using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_NodeAttr
    {
        public IControl GetCtontrol(bool fromQuery = false)
        {
            IControl control = ControlGenrator.GenrateMiniControl(this.InputType);
            control.Name = this.AttrField;
            if (this.IsEnum == TrueOrFalse.True.ToString())
            {
                string enumKey = this.EnumKey;
                if (enumKey.Split('.').Length > 1)
                    enumKey = enumKey.Split('.')[1];
                control.SetAttribute("data", enumKey);
                if (this.MultiSelect == "True")
                {
                    control.SetAttribute("multiSelect", "true");
                }
                if (!String.IsNullOrEmpty(this.TextFieldName))
                {
                    control.SetAttribute("textName", this.TextFieldName);
                }
            }          
            if (!fromQuery)
            {
                if (!String.IsNullOrEmpty(this.VType))
                    control.SetAttribute("vtype", this.VType);
                if (this.Required == TrueOrFalse.True.ToString())
                    control.SetAttribute("required", "true");
                if (this.Disabled == TrueOrFalse.True.ToString())
                    control.SetAttribute("enabled", "false");
            }
            return control;
        }
    }
}
