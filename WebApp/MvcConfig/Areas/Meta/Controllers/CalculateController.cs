using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Config;
using Config.Logic;
using Formula;
using Formula.Helper;
using Base.Logic;
using Base.Logic.Domain;


namespace MvcConfig.Areas.Meta.Controllers
{
    public class CalculateController : BaseController
    {
        CaculateFo _fo = null;
        public CaculateFo CalFo
        {
            get
            {
                if (this._fo == null)
                {
                    _fo = FormulaHelper.CreateFO<CaculateFo>();
                }
                return _fo;
            }
        }

        public JsonResult CalculateFormExpressionWithItem(string TriggerField, string FormItemCode, string FormCode, string FormData)
        {
            if (String.IsNullOrEmpty(FormData))
                throw new Formula.Exceptions.BusinessValidationException("输入的上下文数据内容不能为空");
            var formDic = JsonHelper.ToObject(FormData);
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var defineDt = db.ExecuteDataTable(String.Format("SELECT * FROM S_UI_Form with(nolock) where Code='{0}'", FormCode));
            if (defineDt.Rows.Count == 0)
            {
                throw new Formula.Exceptions.BusinessValidationException(String.Format("没有找到编号为【{0}】的表单定义数据，请确认定义存在", FormCode));
            }
            var defineForm = defineDt.Rows[0];
            if (defineForm["CalItems"] == null || defineForm["CalItems"] == DBNull.Value || String.IsNullOrEmpty(defineForm["CalItems"].ToString()))
                throw new Formula.Exceptions.BusinessValidationException("没有定义字段信息，无法进行公式计算");
            var form = new S_UI_Form();
            FormulaHelper.UpdateEntity<S_UI_Form>(form, FormulaHelper.DataRowToDic(defineForm));
            var calculateItems = form.CalculateItems;
            if (!String.IsNullOrEmpty(TriggerField))
            {
                calculateItems = calculateItems.Where(c => c.TriggerFields.Contains(TriggerField)).ToList();
            }
            var formItemCodes = FormItemCode.Split(',');
            foreach (var formItemCode in formItemCodes)
            {
                var calItems = calculateItems.Where(c => c.FieldCode == formItemCode).ToList();
                if (calItems.Count == 0) continue;
                //当设置为空时，或设置了默认值计算的计算项，才能进行计算
                foreach (var item in calItems)
                {
                    if (!String.IsNullOrEmpty(item.CalDefaultValue) && !item.CalDefaultValue.Split(',').Contains("FormCal"))
                        continue;
                    this.CalFo.CalculateCalItem(item, formDic);
                }            
            }
            return Json(formDic);
        }

        public JsonResult CalculateFormExpression(string FormCode, string FormData)
        {
            if (String.IsNullOrEmpty(FormData))
                throw new Formula.Exceptions.BusinessValidationException("输入的上下文数据内容不能为空");
            var formDic = JsonHelper.ToObject(FormData);
            var db = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var defineDt = db.ExecuteDataTable(String.Format("SELECT * FROM S_UI_Form with(nolock) where Code='{0}'", FormCode));
            if (defineDt.Rows.Count == 0)
            {
                throw new Formula.Exceptions.BusinessValidationException(String.Format("没有找到编号为【{0}】的表单定义数据，请确认定义存在", FormCode));
            }
            var defineForm = defineDt.Rows[0];
            if (defineForm["CalItems"] == null || defineForm["CalItems"] == DBNull.Value || String.IsNullOrEmpty(defineForm["CalItems"].ToString()))
                throw new Formula.Exceptions.BusinessValidationException("没有定义字段信息，无法进行公式计算");

            var form = new S_UI_Form();
            FormulaHelper.UpdateEntity<S_UI_Form>(form, FormulaHelper.DataRowToDic(defineForm));

            foreach (var calItem in form.CalculateItems)
            {
                //当设置为空时，或设置了默认值计算的计算项，才能进行计算
                if (!String.IsNullOrEmpty(calItem.CalDefaultValue) && !calItem.CalDefaultValue.Split(',').Contains("FormCal"))
                    continue;
                this.CalFo.CalculateCalItem(calItem, formDic);
            }
            return Json(formDic);
        }

        public JsonResult CalculateExpression(string expression, string FormData, string calType)
        {
            object result = null;
            if (!String.IsNullOrEmpty(FormData))
            {
                var formDic = JsonHelper.ToObject(FormData);
                result = this.CalFo.CaculateExpression(expression, calType, formDic);
            }
            else
            {
                result = this.CalFo.CaculateExpression(expression, calType);
            }
            if (result != null)
                return Json(new { Result = result });
            else
                return Json("");
        }

    }
}
