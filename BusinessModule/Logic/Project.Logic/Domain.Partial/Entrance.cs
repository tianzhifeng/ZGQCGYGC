using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Project.Logic.Domain;

namespace Project.Logic
{
    public class Entrance
    {
        public Entrance()
        {
            this.ID = Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string SpaceDefineID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string SpaceDefineCode { get; set; }
        public int SortIndex { get; set; }
        public bool IsDefault { get; set; }
        public string LinkUrl { get; set; }
        public string Auth { get; set; }


    }
}
