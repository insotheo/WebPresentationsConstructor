namespace WPC_Editor.Widgets
{
    public class WidgetImage : Widget
    {
        public WidgetImage()
        {
            HTML_TAG = "img";
            name = $"Image_{rndInd.Next(1, 99999)}";
            tag = "Фото";
        }
    }
}
