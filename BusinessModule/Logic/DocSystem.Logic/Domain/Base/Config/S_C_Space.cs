using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config.Logic;
using Config;
using System.Data;
using System.Collections;
using Formula;
using Formula.Helper;

namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_Space
    {
        public void Delete()
        {
            var context = this.GetDocConfigContext();
            foreach (var file in this.S_DOC_File.ToList())
                file.Delete();
            foreach (var node in S_DOC_Node.ToList())
                node.Delete();
            foreach (var listconfig in this.S_DOC_ListConfig.ToList())
                listconfig.Delete();
            foreach (var structNode in this.S_DOC_NodeStrcut.ToList())
                structNode.Delete();
            foreach (var treeconfig in this.S_DOC_TreeConfig.ToList())
                context.S_DOC_TreeConfig.Remove(treeconfig);
            context.S_DOC_Space.Remove(this);
            //docConfigSession.CreateEntityRepository<S_DOC_TreeConfig>().Delete(treeconfig);
            //docConfigSession.CreateEntityRepository<S_DOC_Space>().Delete(this);            
        }

        public void Save()
        {
            var root = this.S_DOC_NodeStrcut.FirstOrDefault(d => d.SpaceID == this.ID && d.NodeID == "Root");
            if (root == null)
            {
                root = new S_DOC_NodeStrcut();
                root.ID = FormulaHelper.CreateGuid();
                root.NodeID = "Root";
                root.FullPathID = root.ID;
                root.Name = this.Name;
                this.S_DOC_NodeStrcut.Add(root);
            }
            var treeConfig = this.S_DOC_TreeConfig.FirstOrDefault();
            if (treeConfig == null)
            {
                treeConfig = new S_DOC_TreeConfig();
                treeConfig.ID = FormulaHelper.CreateGuid();
                treeConfig.TreeDisplay = "Name";
                var sortList = new List<Dictionary<string, object>>();
                var sortTable = new Dictionary<string, object>();
                sortTable["SortField"] = "ID";
                sortTable["SortDir"] = "asc";
                sortList.Add(sortTable);
                treeConfig.TreeSort = JsonHelper.ToJson(sortList);
                this.S_DOC_TreeConfig.Add(treeConfig);
            }
        }
    }


}
