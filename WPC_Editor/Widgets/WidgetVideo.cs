namespace WPC_Editor.Widgets
{
    public class WidgetVideo : Widget
    {
        public bool isLoop;
        public bool showControls;
        public string height;
        public string width;
        public string src;

        public WidgetVideo()
        {
            HTML_TAG = "video";
            tag = "Видео";
            name = $"Video_{rndInd.Next(1, 99999)}";
            isLoop = false;
            showControls = true;
            height = "100";
            width = "100";
            src = "";
            useStyle = false;
        }
    }
}
