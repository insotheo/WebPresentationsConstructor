using System.Collections.Generic;

namespace WPC_Editor.Widgets
{
    public class WidgetGroup : Widget
    {
        public List<Widget> kids;

        public static readonly string[] justifying = { "flex-start", "flex-end", "center", "space-between" };
        public static readonly string[] justifying_rus = { "к началу", "к концу", "к центру", "раномерно" };

        public static readonly string[] flexDir = { "row", "column", "row-reverse", "column-reverse" }; 
        public static readonly string[] flexDir_rus = { "ряд", "столбец", "отражённый ряд", "отражённый столбец" };

        public static readonly string[] position = { "relative", "fixed", "sticky" };
        public static readonly string[] position_rus = { "относительный", "поверх всего", "наклейка" };

        public static readonly string[] positionVector = { "top", "bottom", "left", "right" };
        public static readonly string[] positionVector_rus = { "верхний", "нижний", "левый", "правый" };

        public string backgroundColorHEX;
        public string radius;
        public string justifyContent;
        public string margin;
        public bool isFlex;
        public string flexDirection;
        public string pos; //position
        public string posVector;
        public string posVectorPx;

        public WidgetGroup()
        {
            HTML_TAG = "div";
            tag = "Группа";
            name = $"Group_{rndInd.Next(1, 99999)}";
            kids = new List<Widget>();
            useStyle = false;
            isFlex = true;
            backgroundColorHEX = "Transparent";
            radius = "0";
            margin = "0 0 0 0";
            justifyContent = justifying_rus[0];
            flexDirection = flexDir_rus[0];
            pos = position_rus[0];
            posVector = positionVector_rus[0];
            posVectorPx = "0";
        }

        public void addKid(Widget kid)
        {
            kids.Add(kid);
        }

        public void removeKid(Widget kid)
        {
            kids.Remove(kid);
        }
    }
}
