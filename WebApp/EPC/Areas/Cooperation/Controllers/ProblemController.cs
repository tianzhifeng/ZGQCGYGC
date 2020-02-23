using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula.Helper;
using EPC.Logic;
using EPC.Logic.Domain;
using Formula;
using EPC.Areas.Cooperation.Models;
using Base.Logic.Domain;
using MvcAdapter;
using System.Data;

namespace EPC.Areas.Cooperation.Controllers
{
    public class ProblemController : EPCFormContorllor<S_C_RectifySheet_RectifyProblems>
    {
        public ActionResult ListView()
        {
            var tab = new Tab();
            var statusEnum = CategoryFactory.GetCategory("EPC.ProRectifiState", "状态", "RectifyState");
            var status = GetQueryString("Status").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (status.Length > 0)
                statusEnum.SetMultiDefaultItem(status.ToList());
            statusEnum.Multi = false;
            tab.Categories.Add(statusEnum);

            tab.IsDisplay = true;
            ViewBag.Tab = tab;

            return View();
        }


        public JsonResult GetList(QueryBuilder qb)
        {
            string engineeringInfo = GetQueryString("EngineeringInfo");
            string sql =string.Format(@"select * from S_C_RectifySheet_RectifyProblems where EngineeringInfo='{0}' and CreateCompanyID='' ", engineeringInfo);
            var data = this.EPCSQLDB.ExecuteGridData(sql, qb);
            return Json(data);
        }

        public ActionResult Edit()
        {
            string engineeringInfoID = GetQueryString("EngineeringInfo");
            string sql = "select Name as text,ID as value,Subcontractor,SubcontractorName from S_I_Section where EngineeringInfoID = '" + engineeringInfoID + "' ";
            var dt = this.EPCSQLDB.ExecuteDataTable(sql);
            string json = JsonHelper.ToJson(dt);
            ViewBag.SectionData = json;
            return View();
        }

        public JsonResult GetModel(string id)
        {
            var dic = new Dictionary<string, object>();
            bool isNew = true;
            if (!String.IsNullOrEmpty(id))
            {
                var sql = String.Format("select * from {0} where ID='{1}'", "S_C_RectifySheet_RectifyProblems", id);
                var dt = this.EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    dic = FormulaHelper.DataRowToDic(dt.Rows[0]);
                    isNew = false;
                }
                else
                {
                    throw new Formula.Exceptions.BusinessValidationException(String.Format("数据表【{0}】中没有找到ID为【{1}】的记录，无法读取数据", "S_C_RectifySheet_RectifyProblems", id));
                }
            }
            AfterGetData(dic, isNew, "");
            return Json(dic);
        }

        protected override void AfterGetData(Dictionary<string, object> dic, bool isNew, string upperVersionID)
        {
            if (isNew)
            {
                string engineeringInfo = GetQueryString("EngineeringInfo");
                if (string.IsNullOrEmpty(engineeringInfo))
                    throw new Formula.Exceptions.BusinessValidationException("请选择项目！");

                string sql = string.Format("select * from S_I_Engineering where ID='{0}' ", engineeringInfo);
                var dt = EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Formula.Exceptions.BusinessValidationException("所选项目不存在！");
                var engineering = FormulaHelper.DataRowToDic(dt.Rows[0]);

                dic.SetValue("EngineeringInfo", engineering.GetValue("ID"));
                dic.SetValue("EngineeringInfoName", engineering.GetValue("Name"));

                dic.SetValue("ProNature", "严重");
                dic.SetValue("CheckDate", DateTime.Now.ToString("yyyy-MM-dd"));
                dic.SetValue("CheckerName", CurrentUserInfo.UserName);
                dic.SetValue("ID", FormulaHelper.CreateGuid());
            }
        }

        protected override void BeforeSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            if (isNew)
            {
                dic.SetValue("RectifyState", "Register");//初始状态-- >“待整改”
                if (string.IsNullOrEmpty(dic.GetValue("OpenDate")))
                {
                    dic.SetValue("OpenDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                dic.SetValue("CreateCompanyID", string.Empty);

            }
            else
            {
                string sql = string.Format("select * from S_C_RectifySheet_RectifyProblems where ID='{0}' ", dic.GetValue("ID"));
                var dt = EPCSQLDB.ExecuteDataTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Formula.Exceptions.BusinessValidationException("此单不存在，请确认！");
                var problem = FormulaHelper.DataRowToDic(dt.Rows[0]);
                var currentState = problem.GetValue("RectifyState");

                string actionType = GetQueryString("actionType");
                switch (actionType)
                {
                    case "Rectify"://整改
                        if (currentState != "Register")
                            throw new Formula.Exceptions.BusinessValidationException("只有待整改的单才能整改，请确认！");

                        dic.SetValue("RectifyState", "Rectify");//状态-->“已整改”
                        if (string.IsNullOrEmpty(dic.GetValue("RectifyDate")))
                            throw new Formula.Exceptions.BusinessValidationException("请选择【整改时间】！");

                        dic.SetValue("RectifyUserID", CurrentUserInfo.UserID);
                        dic.SetValue("RectifyUserName", CurrentUserInfo.UserName);
                        break;

                    case "Close"://关闭
                        if (currentState != "Rectify")
                            throw new Formula.Exceptions.BusinessValidationException("只有状态为已整改的单才能关闭，请确认！");

                        dic.SetValue("RectifyState", "Closed");//状态-- >“已关闭”
                        if (string.IsNullOrEmpty(dic.GetValue("CloseDate")))
                            throw new Formula.Exceptions.BusinessValidationException("请选择【关闭时间】！");

                        dic.SetValue("CloseUserID", CurrentUserInfo.UserID);
                        dic.SetValue("CloseUserName", CurrentUserInfo.UserName);
                        break;

                    case "Reject"://重新整改
                        if (currentState != "Rectify")
                            throw new Formula.Exceptions.BusinessValidationException("只有状态为已整改的单才能设置重新整改，请确认！");

                        dic.SetValue("RectifyState", "Register");//状态-- >“待整改”
                        dic.SetValue("RectifyDate", string.Empty);//整改时间
                        dic.SetValue("RectifyUserID", string.Empty);
                        dic.SetValue("RectifyUserName", string.Empty);
                        break;

                    default:
                        if (currentState != "Register")
                            throw new Formula.Exceptions.BusinessValidationException("只有待整改的单才能修改，请确认！");
                        break;
                }

            }
        }

        protected override void AfterSave(Dictionary<string, string> dic, S_UI_Form formInfo, bool isNew)
        {
            LoggerHelper.InserLogger("S_C_RectifySheet_RectifyProblems", isNew ? EnumOperaType.Add : EnumOperaType.Update, dic.GetValue("ID"), dic.GetValue("EngineeringInfo"), dic.GetValue("Name"));
        }

        //删除
        public JsonResult DeleteProblem()
        {
            string id = GetQueryString("ID");
            if (string.IsNullOrEmpty(id))
                throw new Formula.Exceptions.BusinessValidationException("此单不存在，请确认！");
            string sql = string.Format("select * from S_C_RectifySheet_RectifyProblems where ID='{0}' ", id);
            var dt = EPCSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                throw new Formula.Exceptions.BusinessValidationException("此单不存在，请确认！");
            var problem = FormulaHelper.DataRowToDic(dt.Rows[0]);

            if (problem.GetValue("RectifyState") != "Register")
                throw new Formula.Exceptions.BusinessValidationException("只有待整改的单才能删除，请确认！");

            sql = string.Format("delete from S_C_RectifySheet_RectifyProblems where ID ='{0}' and RectifyState='Register' ", id);
            this.EPCSQLDB.ExecuteNonQuery(sql);
            LoggerHelper.InserLogger("S_C_RectifySheet_RectifyProblems", EnumOperaType.Delete, id, problem.GetValue("EngineeringInfo"), problem.GetValue("Name"));
            return Json("");
        }

        //批量删除
        [LoggerFilter(OperaType = EnumOperaType.Delete, Table = "S_C_RectifySheet_RectifyProblems", IDField = "ListData.ID", NameField = "ListData.Name")]
        public JsonResult DeleteProblems()
        {
            string listDataStr = GetQueryString("ListData");
            var dicList = JsonHelper.ToList(listDataStr);
            if (!dicList.Exists(d => d.GetValue("RectifyState") == "Register"))
                throw new Formula.Exceptions.BusinessValidationException("只有新增的联系单才能接收，请确认！");

            var idList = dicList.Select(a => a.GetValue("ID"));
            string sql = string.Format("delete from S_C_RectifySheet_RectifyProblems where ID in('{0}') and RectifyState='Register' ",
    string.Join("','", idList));
            this.EPCSQLDB.ExecuteNonQuery(sql);
            return Json("");
        }


        //重新整改
        public JsonResult Reject()
        {
            string id = GetQueryString("ID");
            if (string.IsNullOrEmpty(id))
                throw new Formula.Exceptions.BusinessValidationException("此单不存在，请确认！");
            string sql = string.Format("select * from S_C_RectifySheet_RectifyProblems where ID='{0}' ", id);
            var dt = EPCSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                throw new Formula.Exceptions.BusinessValidationException("此单不存在，请确认！");

            var problem = FormulaHelper.DataRowToDic(dt.Rows[0]);

            if (problem.GetValue("RectifyState") != "Rectify")
                throw new Formula.Exceptions.BusinessValidationException("只有状态为已整改的单才能设置重新整改，请确认！");

            sql = string.Format("update S_C_RectifySheet_RectifyProblems set RectifyState='Register',RectifyDate=null,RectifyUserID='',RectifyUserName='' where ID ='{0}' and RectifyState='Rectify' ", id);
            this.EPCSQLDB.ExecuteNonQuery(sql);

            LoggerHelper.InserLogger("S_C_RectifySheet_RectifyProblems", EnumOperaType.Update, id, problem.GetValue("EngineeringInfo"), problem.GetValue("Name"), "重新整改");

            return Json("");
        }

        //关闭
        public JsonResult Close()
        {
            string id = GetQueryString("ID");
            if (string.IsNullOrEmpty(id))
                throw new Formula.Exceptions.BusinessValidationException("此单不存在，请确认！");
            string sql = string.Format("select * from S_C_RectifySheet_RectifyProblems where ID='{0}' ", id);
            var dt = EPCSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                throw new Formula.Exceptions.BusinessValidationException("此单不存在，请确认！");

            var problem = FormulaHelper.DataRowToDic(dt.Rows[0]);

            if (problem.GetValue("RectifyState") != "Rectify")
                throw new Formula.Exceptions.BusinessValidationException("只有状态为已整改的单才能关闭，请确认！");

            sql = string.Format("update S_C_RectifySheet_RectifyProblems set RectifyState='Closed', CloseDate=GETDATE(), CloseUserID='{1}', CloseUserName='{2}' where ID ='{0}' and RectifyState='Rectify' ",
            id, CurrentUserInfo.UserID, CurrentUserInfo.UserName);
            this.EPCSQLDB.ExecuteNonQuery(sql);
            LoggerHelper.InserLogger("S_C_RectifySheet_RectifyProblems", EnumOperaType.Update, id, problem.GetValue("EngineeringInfo"), problem.GetValue("Name"), "关闭问题");
            return Json("");
        }


        public ActionResult Detail()
        {
            string problemID = GetQueryString("ID");
            string sql = string.Format("select * from S_C_RectifySheet_RectifyProblems where ID='{0}' ", problemID);
            var dt = EPCSQLDB.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                throw new Formula.Exceptions.BusinessValidationException("此单不存在，请确认！");

            var problem = FormulaHelper.DataRowToDic(dt.Rows[0]);

            ViewBag.ID = problem.GetValue("ID");
            ViewBag.Name = problem.GetValue("Name");
            ViewBag.RectifyState = problem.GetValue("RectifyState");
            ViewBag.EngineeringInfo = problem.GetValue("EngineeringInfo");
            ViewBag.ProType = problem.GetValue("ProType");
            ViewBag.JobContent = problem.GetValue("JobContent");
            ViewBag.CheckDate = string.IsNullOrEmpty(problem.GetValue("CheckDate")) ? string.Empty : Convert.ToDateTime(problem.GetValue("CheckDate")).ToString("yyyy-MM-dd");
            ViewBag.RectifyDate = string.IsNullOrEmpty(problem.GetValue("RectifyDate")) ? string.Empty : Convert.ToDateTime(problem.GetValue("RectifyDate")).ToString("yyyy-MM-dd");
            ViewBag.CloseDate = string.IsNullOrEmpty(problem.GetValue("CloseDate")) ? string.Empty : Convert.ToDateTime(problem.GetValue("CloseDate")).ToString("yyyy-MM-dd");
            ViewBag.LiablePerson = problem.GetValue("LiablePerson");
            ViewBag.CheckerName = problem.GetValue("CheckerName");

            ViewBag.BeforeImprovePictrue = problem.GetValue("BeforeImprovePictrue");
            ViewBag.AfterImprovePictrue = problem.GetValue("AfterImprovePictrue");

            ViewBag.BeforeImprovePictrueList = problem.GetValue("BeforeImprovePictrue").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ViewBag.AfterImprovePictrueList = problem.GetValue("AfterImprovePictrue").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return View();
        }

    }
}
