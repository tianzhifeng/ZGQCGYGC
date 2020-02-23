using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Data;
using System.Data.Entity;
using System.Collections;
using System.Text;
using Formula;
using Formula.Helper;
using MvcAdapter;
using Project.Logic;
using Project.Logic.Domain;
using Config;

namespace Project.Controllers
{
    public class MileStoneAPIController : ApiController
    {

        public IEnumerable<S_P_MileStone> GetProjectMileStoneByCoustomer(string customerID)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            var projects = entities.S_I_ProjectInfo.Include("S_P_MileStone").Where(d => d.CustomerID == customerID &&
                d.S_P_MileStone.Count(p => p.MileStoneType == "First") > 0).ToList();
            var result = new List<S_P_MileStone>();
            foreach (var project in projects)
            {
                var milestones = project.S_P_MileStone.Where(d => d.MileStoneType == MileStoneType.Normal.ToString());
                result.AddRange(milestones);
            }
            return result;
        }

        public IEnumerable<S_P_MileStone> GetProjectMileStoneByProject(string projectID)
        {
            var entities = FormulaHelper.GetEntities<ProjectEntities>();
            return entities.S_P_MileStone.Where(p => p.ProjectInfoID == projectID).ToList();
        }
    }
}
