using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Base.Logic
{
    public interface IBIComponent
    {
        Dictionary<string, object> BlockDef { get; set; }

        Dictionary<string, DataTable> DataSource { get; set; }

        string ID
        {
            get;
            set;
        }

        Dictionary<string, object> Render(string parameters = "", bool IsMobile = false);
        GridData GetGridPageData(QueryBuilder qb, string sql, string ConnName);
    }
}
