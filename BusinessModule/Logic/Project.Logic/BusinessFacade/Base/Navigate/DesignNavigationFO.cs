using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Formula;

namespace Project.Logic
{
    public class DesignNavigationFO
    {
        public List<DesignNavigation> CreateNavigationList()
        {
            var user = FormulaHelper.GetUserInfo();
           
            var list = new List<DesignNavigation>();
            var design = this.CreateCreateNavigation("", "", "");
            list.Add(design);
            return list;
        }

        public DesignNavigation CreateCreateNavigation(string title, string content, string btns)
        {
            var nav = new DesignNavigation();
            nav.Title = title;
            if (!String.IsNullOrEmpty(btns))
            {
                foreach (var item in btns.Split(',').ToList())
                {
                    var navBtn = new DesignNavigationButton();
                    navBtn.Text = item;                  
                }
            }
            if (nav.BtnList.Count > 1)
                nav.BtnAreaClass = "";
            else
                nav.BtnAreaClass = "";
            return nav;
        }
    }
}
