using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Logic.Domain;
using Config.Logic;

namespace Project.Logic
{
    public class VirtualWBS
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();

        public VirtualWBS()
        {
            this.VirtualID = Formula.FormulaHelper.CreateGuid();
        }

        public VirtualWBS(S_W_WBS wbs, bool fillpty = false)
        {          
            if (wbs != null)
            {
                this.dic = wbs.ToDic();
                this.ID = wbs.ID;
                this.ParentID = "";
                this.Name = wbs.Name;
                this.WBSType = wbs.WBSType;
                this.Code = wbs.Code;
                this.WBSValue = wbs.WBSValue;
                this.FullID = wbs.FullID;
                if (fillpty)
                {
                    this.ChargeUserID = wbs.ChargeUserID;
                    this.ChargeUserName = wbs.ChargeUserName;
                    this.PlanStartDate = wbs.PlanStartDate;
                    this.PlanEndDate = wbs.PlanEndDate;
                }
            }
            this.VirtualID = Formula.FormulaHelper.CreateGuid();
        }

        public string ID
        {
            get
            {
                if (!dic.ContainsKey("ID")) return "";
                return dic["ID"] == null ? "" : dic["ID"].ToString();
            }
            set
            {
                dic["ID"] = value;
            }
        }

        public int VirtualLevel
        {
            get
            {
                if (!dic.ContainsKey("VirtualLevel")) return -1;
                return dic["VirtualLevel"] == null ? -1 : Convert.ToInt32(dic["VirtualLevel"]);
            }
            set
            {
                dic["VirtualLevel"] = value;
            }
        }

        public string VirtualID
        {
            get
            {
                if (!dic.ContainsKey("VirtualID")) return "";
                return dic["VirtualID"] == null ? "" : dic["VirtualID"].ToString();
            }
            set
            {
                dic["VirtualID"] = value;
            }
        }

        public string Name
        {
            get
            {
                if (!dic.ContainsKey("Name")) return "";
                return dic["Name"] == null ? "" : dic["Name"].ToString();
            }
            set
            {
                dic["Name"] = value;
            }
        }

        public string Code
        {
            get
            {
                if (!dic.ContainsKey("Code")) return "";
                return dic["Code"] == null ? "" : dic["Code"].ToString();
            }
            set
            {
                dic["Code"] = value;
            }
        }

        public string WBSValue
        {
            get
            {
                if (!dic.ContainsKey("WBSValue")) return "";
                return dic["WBSValue"] == null ? "" : dic["WBSValue"].ToString();
            }
            set
            {
                dic["WBSValue"] = value;
            }
        }

        public string ParentID
        {
            get
            {
                if (!dic.ContainsKey("ParentID")) return "";
                return dic["ParentID"] == null ? "" : dic["ParentID"].ToString();
            }
            set
            {
                dic["ParentID"] = value;
            }
        }

        public string WBSType
        {
            get
            {
                if (!dic.ContainsKey("WBSType")) return "";
                return dic["WBSType"] == null ? "" : dic["WBSType"].ToString();
            }
            set
            {
                dic["WBSType"] = value;
            }
        }

        public string ChargeUserID
        {
            get
            {
                if (!dic.ContainsKey("ChargeUserID")) return "";
                return dic["ChargeUserID"] == null ? "" : dic["ChargeUserID"].ToString();
            }
            set
            {
                dic["ChargeUserID"] = value;
            }
        }

        public string ChargeUserName
        {
            get
            {
                if (!dic.ContainsKey("ChargeUserName")) return "";
                return dic["ChargeUserName"] == null ? "" : dic["ChargeUserName"].ToString();
            }
            set
            {
                dic["ChargeUserName"] = value;
            }
        }

        public object PlanStartDate
        {
            get
            {
                if (!dic.ContainsKey("PlanStartDate")) return null;
                return dic["PlanStartDate"];
            }
            set
            {
                dic["PlanStartDate"] = value;
            }
        }

        public object PlanEndDate
        {
            get
            {
                if (!dic.ContainsKey("PlanEndDate")) return null;
                return dic["PlanEndDate"];
            }
            set
            {
                dic["PlanEndDate"] = value;
            }
        }

        public string FullID
        {
            get
            {
                if (!dic.ContainsKey("FullID")) return "";
                return dic["FullID"] == null ? "" : dic["FullID"].ToString();
            }
            set
            {
                dic["FullID"] = value;
            }
        }

        public VirtualWBS CreateChildNode(S_W_WBS wbs, bool fillPty = false)
        {
            if (wbs == null)
            {
                var child = new VirtualWBS();
                child.VirtualLevel = this.VirtualLevel + 1;
                child.ParentID = this.VirtualID;
                return child;
            }
            else
            {
                var child = new VirtualWBS(wbs, fillPty);
                child.VirtualLevel = this.VirtualLevel + 1;
                child.ParentID = this.VirtualID;
                return child;
            }
        }

        public VirtualWBS CreateChildNode(string name)
        {
            var child = new VirtualWBS();
            child.VirtualLevel = this.VirtualLevel + 1;
            child.ParentID = this.VirtualID;
            child.Name = name;
            return child;
        }

        public void SetValue(string name, object value)
        {
            this.dic.SetValue(name, value);
        }

        public string GetValue(string name)
        {

            return this.dic.GetValue(name);
        }

        public object GetObject(string name)
        {
            return this.dic.GetObject(name);
        }

        public Dictionary<string, object> ToDic()
        {
            return this.dic;
        }
    }
}
