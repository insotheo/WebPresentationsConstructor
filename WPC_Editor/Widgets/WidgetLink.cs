namespace WPC_Editor.Widgets
{
    public class WidgetLink : WidgetText
    {
        public string href;

        public WidgetLink()
        {
            HTML_TAG = "a";
            name = $"Link_{rndInd.Next(1, 99999)}";
            content = "Перейти";
            href = "https://github.com";
            tag = "Ссылка";
            fontSize = 16;
            margin = "0 0 0 0";
            fontFamily = "Arial";
            fontColorHEX = "#0923a4";
            fontWeight = "300";
            backgroundColorHEX = "Transparent";
            backgroundRad = "0";
        }
    }
}
