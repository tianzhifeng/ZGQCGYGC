using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DocSystem.Logic
{
    public class MiniDataGrid : BaseControl
    {
        string htmlTemplate = " <div {0} class=\"mini-datagrid\"><div property=\"columns\"><div type=\"checkcolumn\"></div>{1}</div></div> ";
        string htmlTemplateWithIndex = " <div {0} class=\"mini-datagrid\"><div property=\"columns\"><div type=\"checkcolumn\"></div><div type=\"indexcolumn\"  headerAlign=\"center\" >序号</div>{1}</div></div> ";
        private List<MiniGridColumn> _columns = new List<MiniGridColumn>();
        public List<MiniGridColumn> Columns
        {
            get { return _columns; }
        }

        public MiniDataGrid(string id)
            : this()
        {
            this.ID = id;
        }

        public MiniDataGrid()
        {
            this.Attributes["allowresize"] = false;
            this.Attributes["multiselect"] = true;
            this.Attributes["idfield"] = "ID";
        }

        string _id = "dataGrid";
        public override string ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        string _style = "width: 100%; height: 100%; ";
        public override string Style
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

        public string Url
        {
            get
            {
                if (this.Attributes.ContainsKey("url") && !Tool.IsNullOrEmpty(this.Attributes["url"]))
                    return this.Attributes["url"].ToString();
                else return "";
            }
            set { this.Attributes["url"] = value; }
        }

        public bool Allowresize
        {
            get
            {
                if (this.Attributes.ContainsKey("allowresize") && !Tool.IsNullOrEmpty(this.Attributes["allowresize"]))
                    return Convert.ToBoolean(this.Attributes["allowresize"].ToString());
                else return false;
            }
            set { this.Attributes["allowresize"] = value; }
        }

        public bool Multiselect
        {
            get
            {
                if (this.Attributes.ContainsKey("multiselect") && !Tool.IsNullOrEmpty(this.Attributes["multiselect"]))
                    return Convert.ToBoolean(this.Attributes["multiselect"].ToString());
                else return false;
            }
            set { this.Attributes["multiselect"] = value.ToString().ToLower(); }
        }

        public string BorderStyle
        {
            get
            {
                if (this.Attributes.ContainsKey("borderstyle") && !Tool.IsNullOrEmpty(this.Attributes["borderstyle"]))
                    return this.Attributes["borderstyle"].ToString();
                else return "";
            }
            set { this.Attributes["borderstyle"] = value; }
        }

        public override string Render()
        {
            if (String.IsNullOrEmpty(this.ID))
                throw new Formula.Exceptions.BusinessException("必须指定 datagrid 控件的 id 属性");
            if (String.IsNullOrEmpty(this.Url))
                throw new Formula.Exceptions.BusinessException("必须指定 datagrid 控件的 Url 属性");
            string attr = this.getAttr();
            string childControlHtml = this.renderChildControl();
            return String.Format(htmlTemplate, attr, childControlHtml);
        }

        public string Render(bool isShowIndex = false)
        {
            if (String.IsNullOrEmpty(this.ID))
                throw new Formula.Exceptions.BusinessException("必须指定 datagrid 控件的 id 属性");
            if (String.IsNullOrEmpty(this.Url))
                throw new Formula.Exceptions.BusinessException("必须指定 datagrid 控件的 Url 属性");
            string attr = this.getAttr();
            string childControlHtml = this.renderChildControl();
            if (isShowIndex)
                return String.Format(htmlTemplateWithIndex, attr, childControlHtml);
            else
                return String.Format(htmlTemplate, attr, childControlHtml);
        }

        protected override void onAddingControl(IControl control)
        {
            if (control is MiniGridColumn)
            {
                var column = control as MiniGridColumn;
                this.Columns.Add(column);
            }
        }
    }
}
