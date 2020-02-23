using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Config;
using Config.Logic;
using Formula.Helper;

namespace Base.Logic.Domain
{
    public partial class S_UI_Form
    {

        List<CalItem> _calculateItems = null;
        [NotMapped]
        [JsonIgnore]
        public List<CalItem> CalculateItems
        {
            get
            {
                if (_calculateItems == null)
                {
                    _calculateItems = new List<CalItem>();
                    var list = JsonHelper.ToList(this.CalItems);
                    foreach (var item in list)
                    {
                        var calItem = new CalItem();
                        calItem.Expression = item.GetValue("Expression");
                        calItem.FieldCode = item.GetValue("Code").Trim('{', '}');
                        calItem.TriggerFields = item.GetValue("TriggerFields");
                        calItem.CalDefaultValue = item.GetValue("CalDefaultValue");
                        calItem.CollectionValueField = item.GetValue("CollectionValueField");
                        calItem.DecimalPlaces = String.IsNullOrEmpty(item.GetValue("decimalPlaces")) ? 2 : Convert.ToInt32(item.GetValue("decimalPlaces"));
                        calItem.AutoTrigger = item.GetValue("TriggerMethod") == TriggerMethod.InputChange.ToString() ? true : false;
                        calItem.CalType = String.IsNullOrEmpty(item.GetValue("CalType")) ? CalType.Decimal.ToString() : item.GetValue("CalType");
                        if (!String.IsNullOrEmpty(item.GetValue("Details")))
                        {
                            var details = JsonHelper.ToList(item.GetValue("Details"));
                            foreach (var detail in details)
                            {
                                var adapter = new CalAdapterItem();
                                adapter.InputField = detail.GetValue("InputField");
                                adapter.NeedInputCode = detail.GetValue("NeedInputCode");
                                adapter.DefaultValue = detail.GetValue("DefaultValue");
                                calItem.AdapterItems.Add(adapter);
                            }
                        }
                        if (!String.IsNullOrEmpty(item.GetValue("Filter")))
                        {
                            var filters = JsonHelper.ToList(item.GetValue("Filter"));
                            foreach (var filter in filters)
                            {
                                var filterItem = new FilterItem();
                                filterItem.Field = filter.GetValue("Field");
                                filterItem.QueryMode = filter.GetValue("QueryMode");
                                filterItem.Value = filter.GetValue("Value");
                                filterItem.Group = filter.GetValue("Group");
                                calItem.Filters.Add(filterItem);
                            }
                        }
                        if (!String.IsNullOrEmpty(item.GetValue("ValueCondition")))
                        {
                            var valueConditionList = JsonHelper.ToList(item.GetValue("ValueCondition"));
                            foreach (var valueCondition in valueConditionList)
                            {
                                var valueItem = new ValueConditionItem();
                                valueItem.FieldCode = valueCondition.GetValue("FieldCode");
                                valueItem.QueryMode = valueCondition.GetValue("QueryMode");
                                valueItem.Value = valueCondition.GetValue("Value");
                                valueItem.ConditionValue = valueCondition.GetValue("ConditionValue");
                                calItem.ValueConditionItem.Add(valueItem);
                            }
                        }
                        if (!String.IsNullOrEmpty(item.GetValue("SubTableAdapter")))
                        {
                            var subTableAdps = JsonHelper.ToList(item.GetValue("SubTableAdapter"));
                            foreach (var adp in subTableAdps)
                            {
                                var adpItem = new SubTableAdpItem();
                                adpItem.FieldCode = adp.GetValue("FieldCode");
                                adpItem.AdpField = adp.GetValue("ParamField");
                                adpItem.DefaultValue = adp.GetValue("DefaultValue");
                                calItem.SubTableAdapters.Add(adpItem);
                            }
                        }
                        _calculateItems.Add(calItem);
                    }
                }
                return _calculateItems;
            }
        }



        List<TriggerFieldItem> _calTriggerFields = null;
        [NotMapped]
        [JsonIgnore]
        public List<TriggerFieldItem> CalTriggerFields
        {
            get
            {
                if (_calTriggerFields == null)
                {
                    _calTriggerFields = new List<TriggerFieldItem>();
                    foreach (var item in this.CalculateItems)
                    {
                        var fieldList = item.TriggerFields.Split(',');
                        foreach (var field in fieldList)
                        {
                            var triggerField = _calTriggerFields.FirstOrDefault(c => c.FieldCode == field.Trim('{', '}'));
                            if (triggerField == null)
                            {
                                triggerField = new TriggerFieldItem();
                                triggerField.FieldCode = field.Trim('{', '}');
                                triggerField.CalItemCodes = item.FieldCode.Trim('{', '}');
                                _calTriggerFields.Add(triggerField);
                            }
                            else
                            {
                                if (!triggerField.CalItemCodes.Split(',').Contains(item.FieldCode))
                                {
                                    triggerField.CalItemCodes += "," + item.FieldCode.Trim('{', '}');
                                }
                            }
                        }
                    }

                    #region
                    //var list = JsonHelper.ToList(this.CalItems);
                    //foreach (var item in list)
                    //{
                    //    if(String.IsNullOrEmpty(  item.GetValue("Details"))
                    //}

                    //_autoCalInputFields = new List<Dictionary<string, object>>();

                    //var items = JsonHelper.ToObject<List<CalItem>>(this.CalItems);


                    //_autoCalInputFields = new List<Dictionary<string, object>>();
                    //var formItems = JsonHelper.ToObject<List<Base.Logic.Model.UI.Form.FormItem>>(this.Items);

                    //#region 普通字段（非子表）
                    //var calItems = formItems.Where(c => c.ItemType != "SubTable" && !String.IsNullOrEmpty(c.Expression)).ToList();
                    //foreach (var item in calItems)
                    //{
                    //    if (!String.IsNullOrEmpty(item.ExpressionSettings))
                    //    {
                    //        var expSettings = JsonHelper.ToObject(item.ExpressionSettings);
                    //        if (expSettings.GetValue("TriggerMethod") == TriggerMethod.InputChange.ToString())
                    //        {
                    //            if (!String.IsNullOrEmpty(expSettings.GetValue("TriggerFields")))
                    //            {
                    //                foreach (var TriggerField in expSettings.GetValue("TriggerFields").Split(','))
                    //                {
                    //                    var inputField = TriggerField.Trim('{', '}');
                    //                    var fieldDic = _autoCalInputFields.FirstOrDefault(c => c.GetValue("FieldCode") == inputField && c.GetValue("CalField") == item.Code);
                    //                    if (fieldDic == null)
                    //                    {
                    //                        fieldDic = new Dictionary<string, object>();
                    //                        fieldDic.SetValue("FieldCode", inputField);
                    //                        fieldDic.SetValue("CalField", item.Code);
                    //                        fieldDic.SetValue("Priority", String.IsNullOrEmpty(expSettings.GetValue("Priority")) ? 0 :
                    //                            Convert.ToInt32(expSettings.GetValue("Priority")));
                    //                        _autoCalInputFields.Add(fieldDic);
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //#endregion

                    //#region 子表字段
                    //var subTableList = formItems.Where(c => c.ItemType == "SubTable").ToList();
                    //foreach (var subTableItem in subTableList)
                    //{
                    //    if (!String.IsNullOrEmpty(subTableItem.Expression))
                    //    {
                    //        if (!String.IsNullOrEmpty(subTableItem.ExpressionSettings))
                    //        {
                    //            var expSettings = JsonHelper.ToObject(subTableItem.ExpressionSettings);
                    //            if (expSettings.GetValue("TriggerMethod") == TriggerMethod.InputChange.ToString())
                    //            {
                    //                foreach (var TriggerField in expSettings.GetValue("TriggerFields").Split(','))
                    //                {
                    //                    var inputField = TriggerField.Trim('{', '}');
                    //                    var fieldDic = _autoCalInputFields.FirstOrDefault(c => c.GetValue("FieldCode") == inputField && c.GetValue("CalField") == subTableItem.Code);
                    //                    if (fieldDic == null)
                    //                    {
                    //                        fieldDic = new Dictionary<string, object>();
                    //                        fieldDic.SetValue("FieldCode", inputField);
                    //                        fieldDic.SetValue("CalField", subTableItem.Code);
                    //                        fieldDic.SetValue("Priority", String.IsNullOrEmpty(expSettings.GetValue("Priority")) ? 0 : Convert.ToInt32(expSettings.GetValue("Priority")));
                    //                        _autoCalInputFields.Add(fieldDic);
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //    if (String.IsNullOrEmpty(subTableItem.Settings))
                    //        continue;
                    //    var subItemSettings = JsonHelper.ToObject(subTableItem.Settings);
                    //    var listData = subItemSettings.GetValue("listData");
                    //    if (String.IsNullOrEmpty(listData)) continue;
                    //    var subTableItems = JsonHelper.ToObject<List<Base.Logic.Model.UI.Form.FormItem>>(listData);
                    //    var calSubTableItems = subTableItems.Where(c => !String.IsNullOrEmpty(c.Expression)).ToList();
                    //    foreach (var calSubItem in calSubTableItems)
                    //    {
                    //        if (String.IsNullOrEmpty(calSubItem.ExpressionSettings)) continue;
                    //        var expSettings = JsonHelper.ToObject(calSubItem.ExpressionSettings);
                    //        if (expSettings.GetValue("TriggerMethod") == TriggerMethod.InputChange.ToString())
                    //        {
                    //            foreach (var TriggerField in expSettings.GetValue("TriggerFields").Split(','))
                    //            {
                    //                var subTableItemCode = subTableItem.Code + "." + calSubItem.Code;
                    //                var inputField = TriggerField.Trim('{', '}');
                    //                var fieldDic = _autoCalInputFields.FirstOrDefault(c => c.GetValue("FieldCode") == inputField && c.GetValue("CalField") == subTableItemCode);
                    //                if (fieldDic == null)
                    //                {
                    //                    fieldDic = new Dictionary<string, object>();
                    //                    fieldDic.SetValue("FieldCode", inputField);
                    //                    fieldDic.SetValue("CalField", subTableItemCode);
                    //                    fieldDic.SetValue("Priority", String.IsNullOrEmpty(expSettings.GetValue("Priority")) ? 0 : Convert.ToInt32(expSettings.GetValue("Priority")));
                    //                    _autoCalInputFields.Add(fieldDic);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //#endregion
                    #endregion
                }
                return _calTriggerFields;
            }
        }
    }
}
