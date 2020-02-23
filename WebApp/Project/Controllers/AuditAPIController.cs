using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
    public class AuditAPIController : ApiController
    {
        [HttpPost]
        [ActionName("Submit")]
        public bool SubmitFlow(string id)
        {
            using (var entities = FormulaHelper.GetEntities<ProjectEntities>())
            {
                return true;
            }
        }


        [HttpPost]
        public string StartFlow()
        {
            using (var entities = FormulaHelper.GetEntities<ProjectEntities>())
            {
                return "";
            }
        
        }

        private string getCode(ProjectEntities entities)
        {
            return string.Empty;
        }
        
    }
}