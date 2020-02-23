using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;

namespace Comprehensive.Logic
{
    public abstract class BaseChart
    {
        List<Series> _seriesList = new List<Series>();
        public List<Series> SeriesList
        {
            get
            {
                return _seriesList;
            }
        }

        public Chart _chart;
        public Chart Chart
        {
            get
            {
                if (_chart == null)
                {
                    _chart = new Chart();
                }
                return _chart;
            }
        }

        public bool Is3D
        { get; set; }

        public SubTitle SubTitleInfo
        {
            get;
            set;
        }

        PlotOptions _plotOption;
        public virtual PlotOptions PlotOption
        {
            get
            {
                if (_plotOption == null)
                    _plotOption = new PlotOptions();
                return _plotOption;
            }
        }

        ToolTip _ToolTips;
        public ToolTip ToolTips
        {
            get
            {
                if (_ToolTips == null)
                    _ToolTips = new ToolTip();
                return _ToolTips;
            }
        }

        Title _titleInfo;
        public Title TitleInfo
        {
            get
            {
                if (_titleInfo == null) _titleInfo = new Title();
                return _titleInfo;
            }
        }

        public abstract Dictionary<string, object> Render();
    }
}
