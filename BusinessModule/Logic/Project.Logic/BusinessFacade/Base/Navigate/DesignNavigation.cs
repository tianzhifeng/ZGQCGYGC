using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic
{
    /// <summary>
    /// 
    /// </summary>
    public  class DesignNavigation
    {
        private int _sortIndex;
        /// <summary>
        /// 排序号
        /// </summary>
        public int SortIndex
        {
            get { return _sortIndex; }
            set { _sortIndex = value; }
        }

        private string _title;
        /// <summary>
        /// 标题栏
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        string imgUrl = "";
        /// <summary>
        /// 大图标地址
        /// </summary>
        public string ImgUrl
        {
            get { return imgUrl; }
            set { imgUrl = value; }
        }

        private string btnClass;
        /// <summary>
        /// 
        /// </summary>
        public string BtnAreaClass
        {
            get { return btnClass; }
            set { btnClass = value; }
        }


        private List<DesignNavigationButton> btnList = new List<DesignNavigationButton>();
        /// <summary>
        /// 按钮集合
        /// </summary>
        public List<DesignNavigationButton> BtnList
        {
            get { return btnList; }
            set { btnList = value; }
        }        

        private DesignNavigationContent _content;
        /// <summary>
        /// 内容文本
        /// </summary>
        public DesignNavigationContent Content
        {
            get { return _content; }
            set { _content = value; }
        }

        private List<DesignNavigationLink> linkAction = new List<DesignNavigationLink>();
        /// <summary>
        /// 链接
        /// </summary>
        public List<DesignNavigationLink> Link
        {
            get { return linkAction; }
            set { linkAction = value; }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class DesignNavigationContent
    {
        List<Dictionary<string, object>> _content = new List<Dictionary<string, object>>();
        public List<Dictionary<string, object>> Content
        {
            get { return _content; }
        }        
    }

    /// <summary>
    /// 
    /// </summary>
    public class DesignNavigationLink
    {
        private string text;
        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private string linkUrl;
        /// <summary>
        ///     
        /// </summary>
        public string LinkUrl
        {
            get { return linkUrl; }
            set { linkUrl = value; }
        }

        private string onClick;
        /// <summary>
        /// 
        /// </summary>
        public string OnClick
        {
            get { return onClick; }
            set { onClick = value; }
        }
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class DesignNavigationButton
    {
        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private string onClick;
        public string OnClick
        {
            get { return onClick; }
            set { onClick = value; }
        }
    }

    public class DeisgnNavigationConst
    {
        public const string HTMLTemplate = @"<table class='gw_pm_main_table' width='100%' border='0' cellspacing='0' cellpadding='0'>
                    <tr>
                        <td colspan='2' class='gw_pm_main_title'>
                            {0}
                        </td>
                        <td width='120' rowspan='3' align='center' class='gw_pm_button'>
                            <span class='gw_pm_buttono'>
                                <a href='#' onclick='onclick'>{2}</a></span>
                        </td>
                    </tr>
                    <tr>
                        <td width='88' rowspan='2' valign='top'>
                            <img src='/Project/Scripts/DesignNavigate/icon/010.png' width='88' height='70' />
                        </td>
                        <td valign='middle' class='gw_pm_main_sum'>
                            {1}
                        </td>
                    </tr>
                    <tr>
                        <td valign='top' class='gw_pm_main_link'>
                            &nbsp;
                        </td>
                    </tr>
                </table>";
 
    }
}
