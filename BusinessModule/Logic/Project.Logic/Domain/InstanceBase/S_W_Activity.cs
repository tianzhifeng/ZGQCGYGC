using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula.Helper;
using Config.Logic;
namespace Project.Logic.Domain
{
    public partial class S_W_Activity
    {
        public void Delete()
        {
 
        }

        public void Finish()
        {
            this.State = ProjectCommoneState.Finish.ToString();
            this.FinishDate = DateTime.Now;
        }

        public void SetParam(string name, string value)
        {
            Dictionary<String, object> dic;
            if (!String.IsNullOrEmpty(this.Params))
                dic = JsonHelper.ToObject(this.Params);
            else
                dic = new Dictionary<string, object>();
            dic.SetValue(name, value);
            this.Params = JsonHelper.ToJson(dic);
        }

        //public List<AuditStep> GetSteps()
        //{
        //    var result = new List<AuditStep>();
        //    if (String.IsNullOrEmpty(this.DefSteps))
        //        return result;
        //    var list = JsonHelper.ToList(this.DefSteps);
        //    foreach (var item in list)
        //    {
        //        var step = new AuditStep(item);
        //        result.Add(step);
        //    }
        //    return result;
        //}

        //public AuditStep GetCurrentStep()
        //{
        //    var steps = this.GetSteps();
        //    return steps.FirstOrDefault(d => d.StepKey.ToString() == this.ActivityKey);
        //}

        //public AuditStep GetNextStep()
        //{
        //    if (String.IsNullOrEmpty(this.NextStep))
        //        return null;
        //    var auditStep = new AuditStep(this.NextStep);
        //    return auditStep;
        //}
    }
}
