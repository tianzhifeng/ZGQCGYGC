using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocSystem.Logic
{
    public interface IControl
    {

        List<IControl> Controls { get; }

        string ID { get; set; }

        string Name { get; set; }

        int Width { get; set; }

        string WidthUnit { get; set; }

        string Style { get; set; }

        string InnerText { get; set; }

        string Render();

        void AddControl(IControl control);

        void SetAttribute(string attrName,string attrValue);

        string GetAttribute(string attrName);

    }
}
