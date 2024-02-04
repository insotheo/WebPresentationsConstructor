namespace WPC_Editor.Widgets
{
    public class WidgetSquareShape : Widget
    {
        public readonly string classHTML = "Shape";

        public string skew;
        public string width;
        public string height;
        public string radius;
        public string color;
        public string margin;

        public WidgetSquareShape()
        {
            tag = "Форма";
            HTML_TAG = "div";
            name = $"Shape_{rndInd.Next(0, 9999999)}";
            useStyle = false;
            radius = "0";
            skew = "-20";
            width = "60";
            height = "40";
            color = "#2d2d2d";
            margin = "0 0 0 0";
        }
    }
}
