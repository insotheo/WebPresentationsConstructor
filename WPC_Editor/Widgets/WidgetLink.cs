namespace WPC_Editor.Widgets
{
    public class WidgetLink : WidgetText
    {
        public string href;

        public WidgetLink()
        {
            HTML_TAG = "a";
            name = $"Link_{rndInd.Next(10000, 999999)}";
            content = "Перейти";
            href = "https://github.com";
            tag = "Ссылка";
            fontSize = 16;
            fontFamily = "Arial";
            fontColorHEX = "#0923a4";
            fontWeight = "300";
            backgroundColorHEX = "Transparent";
            backgroundRad = "0";
        }
    }
}
