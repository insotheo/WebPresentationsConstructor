namespace WPC_Editor.Widgets
{
    public class WidgetButton : Widget
    {
        public string content;
        public string onclick;
        public string arguments;

        public int fontSize;
        public string fontFamily;
        public string fontWeight;
        public string fontColorHEX;
        public string backgroundColorHEX;
        public string borderColorHEX;
        public string borderRadius;
        public string cursor;

        public WidgetButton()
        {
            HTML_TAG = "button";
            tag = "Кнопка";
            name = $"Button_{rndInd.Next(1, 99999)}";

        }
    }
}
