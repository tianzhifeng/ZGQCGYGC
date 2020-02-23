using System;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPivotGrid.Localization;
namespace ChineseLanguage
{
    public class Chinese
    {
        public Chinese()
        {
            Localizer.Active = new XtraEditors_CN();
            PivotGridLocalizer.Active = new PivotGridLocalizer_CN();            
        }
    }
    public class XtraEditors_CN : Localizer
    {
        public XtraEditors_CN()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }
        public override string GetLocalizedString(StringId id)
        {
            switch (id)
            {
                case StringId.TextEditMenuCopy: return "复制(&C)";
                case StringId.TextEditMenuCut: return "剪切(&T)";
                case StringId.TextEditMenuDelete: return "删除(&D)";
                case StringId.TextEditMenuPaste: return "粘贴(&P)";
                case StringId.TextEditMenuSelectAll: return "全选(&A)";
                case StringId.TextEditMenuUndo: return "撤消(&U)";
                case StringId.UnknownPictureFormat: return "未知图片格式";
                case StringId.DateEditToday: return "今天";
                case StringId.DateEditClear: return "清空";
                case StringId.DataEmpty: return "无图像";
                case StringId.ColorTabWeb: return "网页";
                case StringId.ColorTabSystem: return "系统";
                case StringId.ColorTabCustom: return "自定义";
                case StringId.CheckUnchecked: return "未选择";
                case StringId.CheckIndeterminate: return "不确定";
                case StringId.CheckChecked: return "已选择";
                case StringId.CaptionError: return "标题错误";
                case StringId.Cancel: return "取消";
                case StringId.CalcError: return "计算错误";
                case StringId.CalcButtonBack: return base.GetLocalizedString(id);
                case StringId.CalcButtonC: return base.GetLocalizedString(id);
                case StringId.CalcButtonCE: return base.GetLocalizedString(id); ;
                case StringId.CalcButtonMC: return base.GetLocalizedString(id);
                case StringId.CalcButtonMR: return base.GetLocalizedString(id);
                case StringId.CalcButtonMS: return base.GetLocalizedString(id);
                case StringId.CalcButtonMx: return base.GetLocalizedString(id);
                case StringId.CalcButtonSqrt: return base.GetLocalizedString(id);
                case StringId.OK: return "确定";
                case StringId.PictureEditMenuCopy: return "复制(&C)";
                case StringId.PictureEditMenuCut: return "剪切(&T)";
                case StringId.PictureEditMenuDelete: return "删除(&D)";
                case StringId.PictureEditMenuLoad: return "加载(&L)";
                case StringId.InvalidValueText: return "无效的值";
                case StringId.NavigatorRemoveButtonHint: return "删除";
                case StringId.NavigatorTextStringFormat: 
                    return "记录{0}/{1}";
                case StringId.None: return "";
                case StringId.NotValidArrayLength: return "无效的数组长度.";
            }
            return base.GetLocalizedString(id);
        }
    }


    public class PivotGridLocalizer_CN : PivotGridLocalizer
    {
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(PivotGridStringId id)
        {
            switch (id)
            {
                case PivotGridStringId.DataArea: return "合计区";
                case PivotGridStringId.FilterArea: return "过滤区";
                case PivotGridStringId.ColumnArea: return "列区";
                case PivotGridStringId.RowArea: return "行区";
                case PivotGridStringId.RowHeadersCustomization: return "拖动字段到行区";
                case PivotGridStringId.ColumnHeadersCustomization: return "拖动字段到列区";
                case PivotGridStringId.FilterHeadersCustomization: return "拖动字段到过滤区";
                case PivotGridStringId.DataHeadersCustomization: return "拖动字段到合计区";
                case PivotGridStringId.GrandTotal: return "总计";
                case PivotGridStringId.PopupMenuRefreshData: return "刷新";
                case PivotGridStringId.PopupMenuRemoveAllSortByColumn: return "移除列排序";
                case PivotGridStringId.PopupMenuMovetoBeginning: return "移至开始";
                case PivotGridStringId.PopupMenuMovetoEnd: return "移至最后";
                case PivotGridStringId.PopupMenuMovetoLeft: return "移至左侧";
                case PivotGridStringId.PopupMenuMovetoRight: return "移至右侧";   
            }
            return base.GetLocalizedString(id);
        }
    }
}
