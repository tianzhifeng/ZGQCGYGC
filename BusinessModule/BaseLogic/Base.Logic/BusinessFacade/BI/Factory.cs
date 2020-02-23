using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config.Logic;
namespace Base.Logic
{
    public class BIComponentFactory
    {
        public static IBIComponent CreateComponent(string componentType, string BlockDefJson)
        {
            IBIComponent component = null;
            switch (componentType)
            {
                case "ColumnGrid":
                    component = new ColumnGrid(BlockDefJson);
                    break;
                case "ComplexTargetColumn":
                    break;
                case "ComplexTargetPie":
                    break;
                case "ComplexTextBlock":
                    component = new ComplexTextBlock(BlockDefJson);
                    break;
                case "FileGrid":
                    component = new FileGrid(BlockDefJson);
                    break;
                case "Grid":
                    component = new Grid(BlockDefJson);
                    break;
                case "Navigation":
                    component = new Navigation(BlockDefJson);
                    break;
                case "NoticeGrid":
                    component = new NoticeGrid(BlockDefJson);
                    break;
                case "PieGrid":
                    component = new PieGrid(BlockDefJson);
                    break;
                case "SimpleTitleGrid":
                    component = new SimpleTitleGrid(BlockDefJson);
                    break;
                case "StandardColumn":
                    component = new StandardColumn(BlockDefJson);
                    break;
                case "StandardPie":
                    component = new StandardPie(BlockDefJson);
                    break;
                case "StandardText":
                    break;
                case "TargetColumn":
                    component = new TargetColumn(BlockDefJson);
                    break;
                case "TargetPie":
                    component = new TargetPie(BlockDefJson);
                    break;
                case "Tab":
                    component = new Tab(BlockDefJson);
                    break;
                default:
                case "TitleBlock":
                    component = new TitleBlock(BlockDefJson);
                    break;

            }
            return component;
        }
    }
}
