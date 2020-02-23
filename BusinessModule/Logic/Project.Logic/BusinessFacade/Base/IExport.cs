using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Logic
{
    public interface IFormExport
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="tmpCode"></param>
        /// <returns></returns>
        string ExportPDF(string fileName, string formID, string tmpCode);
    }
}
