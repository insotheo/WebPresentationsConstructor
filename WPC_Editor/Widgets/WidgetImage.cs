namespace WPC_Editor.Widgets
{
    public class WidgetImage : Widget
    {

        public string href;
        public char contentType; //'f' - file; 'l' - link;
        public string height;
        public string width;
        public string radius;
        public string rotationAngle;

        public WidgetImage()
        {
            useStyle = false;
            HTML_TAG = "img";
            name = $"Image_{rndInd.Next(1, 99999)}";
            tag = "Фото";
            contentType = 'l';
            width = "100";
            height = "100";
            radius = "0";
            rotationAngle = "0";
        }
    }
}
