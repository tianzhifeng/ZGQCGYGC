using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

using System.Collections;
using Newtonsoft.Json;


namespace DocSystem.Logic.Domain
{
    public partial class S_DOC_FileAttr
    {
        [NotMapped]
        [JsonIgnore]
        public bool IsFullRow
        {
            get
            {
                var result = false;
                if (this.InputType == ControlType.ComboboxFullRow.ToString() ||
                    this.InputType == ControlType.TextAreaFullRow.ToString() ||
                    this.InputType == ControlType.TextBoxFullRow.ToString())
                    result = true;
                return result;
            }
        }

        public void MoveUp()
        {
            var preAttr = this.S_DOC_File.S_DOC_FileAttr.Where(d => d.AttrSort < this.AttrSort).OrderByDescending(d => d.AttrSort).FirstOrDefault();
            if (preAttr == null) return;
            int sort = this.AttrSort;
            this.AttrSort = preAttr.AttrSort;
            preAttr.AttrSort = sort;
        }

        public void MoveDown()
        {
            var aftAttr = this.S_DOC_File.S_DOC_FileAttr.Where(d => d.AttrSort > this.AttrSort).OrderBy(d => d.AttrSort).FirstOrDefault();
            if (aftAttr == null) return;
            int sort = this.AttrSort;
            this.AttrSort = aftAttr.AttrSort;
            aftAttr.AttrSort = sort;
        }
    }
}
