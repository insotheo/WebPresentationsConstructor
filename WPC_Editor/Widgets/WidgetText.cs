namespace WPC_Editor.Widgets
{
    public class WidgetText : Widget
    {
        public string content;
        public int fontSize;
        public string fontFamily;
        public string fontWeight;
        public string fontColorHEX;
        public string backgroundColorHEX;
        public string backgroundRad;

        public WidgetText()
        {
            HTML_TAG = "p";
            tag = "Текст";
            name = $"Text_{rndInd.Next(1, 99999)}";
            content = "Привет, Мир!";
            fontSize = 18;
            fontFamily = "Arial";
            fontColorHEX = "#000000";
            fontWeight = "400";
            backgroundColorHEX = "Transparent";
            backgroundRad = "0";
        }
    }
}
