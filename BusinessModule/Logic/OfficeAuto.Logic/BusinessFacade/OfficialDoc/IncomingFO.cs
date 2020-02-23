using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto.Logic
{
    public class IncomingFO
    {
        public static bool ValidateUniqueCode(string id,string code)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(code))
            {
                var entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
                if (entities.Set<S_D_Incoming>().Where(c => c.ID != id && c.Code == code).Count() > 0)
                    return false;
            }
            return true;
        }

        public static void ResetStatus(string id)
        {

        }

        public static void ChangeStatus(string id, string status)
        {

        }
    }
}
