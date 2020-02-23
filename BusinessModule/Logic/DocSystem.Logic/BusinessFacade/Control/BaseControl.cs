using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DocSystem.Logic
{
    public abstract class BaseControl : IControl
    {
        #region 属性

        string _id = string.Empty;
        public virtual string ID
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

        string _name = string.Empty;
        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        string _heightUnit = "px";
        public virtual string HeightUnit
        {
            get
            {
                return _heightUnit;
            }
            set
            {
                _heightUnit = value;
            }
        }

        int _height = 0;
        public virtual int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        int _width = 0;
        public virtual int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        string _style = string.Empty;
        public virtual string Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
            }
        }

        string _widthUnit = "%";
        public virtual string WidthUnit
        {
            get
            {
                return _widthUnit;
            }
            set
            {
                _widthUnit = value;
            }
        }

        string _innerText = string.Empty;
        public virtual string InnerText
        {
            get { return _innerText; }
            set { _innerText = value; }
        }

        List<IControl> _controls = new List<IControl>();
        public List<IControl> Controls
        {
            get { return _controls; }
        }

        #endregion

        protected Hashtable Attributes = new Hashtable();

        public abstract string Render();

        public virtual void AddControl(IControl control)
        {
            this.Controls.Add(control);
            onAddingControl(control);
        }

        protected string renderChildControl()
        {
            StringBuilder result = new StringBuilder();
            result.Append(this.InnerText);
            foreach (var item in this.Controls)
            {
                result.Append(item.Render());
            }
            return result.ToString();
        }

        protected string getAttr()
        {
            string attr = string.Empty;
            if (!String.IsNullOrEmpty(this.ID))
                attr += " id=\"" + this.ID + "\" ";
            if (!String.IsNullOrEmpty(this.Name))
                attr += " name=\"" + this.Name + "\" ";
            if (!String.IsNullOrEmpty(this.Style))
                attr += " style=\"" + this.Style + "\" ";
            if (this.Width > 0)
                attr += " width=\"" + this.Width.ToString() + this.WidthUnit.ToString() + "\" ";
            if (this.Height > 0)
                attr += " height=\"" + this.Height.ToString() + this.HeightUnit.ToString() + "\" ";
            foreach (DictionaryEntry item in this.Attributes)
                attr += " " + item.Key.ToString() + "=\"" + item.Value.ToString() + "\"";
            return attr;
        }

        protected virtual void onAddingControl(IControl control)
        { }

        public virtual void SetAttribute(string attrName, string attrValue)
        {
            if (Attributes.ContainsKey(attrName))
            {
                Attributes.Remove(attrName);
                Attributes[attrName] = attrValue;
            }
            else
                Attributes[attrName] = attrValue;
        }

        public virtual string GetAttribute(string attrName)
        {
            if (!Attributes.ContainsKey(attrName)) return "";
            if (Tool.IsNullOrEmpty(Attributes[attrName])) return "";
            return Attributes[attrName].ToString();
        }
    }
}
