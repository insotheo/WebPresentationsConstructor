namespace WPC_Editor.Widgets
{
    public class WidgetText : Widget
    {
        public WidgetText()
        {
            HTML_TAG = "p";
            tag = "Текст";
            name = $"Text_{rndInd.Next(1, 99999)}";
        }
    }
}
