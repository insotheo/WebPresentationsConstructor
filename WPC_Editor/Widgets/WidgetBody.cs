namespace WPC_Editor.Widgets
{
    public class WidgetBody : Widget
    {
        public enum CommonType
        {
            color,
            photo
        }

        public enum PhotoType
        {
            link,
            image
        }

        public CommonType type;
        public PhotoType photoType;

        public static readonly string[] imageSize_eng = { "contain", "cover" };
        public static readonly string[] imageSize_rus = { "Содержание", "Покрытие" };

        public static readonly string[] imageRepeat_eng = { "repeat-x", "repeat", "no-repeat" };
        public static readonly string[] imageRepeat_rus = { "Повторять по горизонтали", "Повторять", "Не повторять" };

        //color properties
        public string color;

        //image properties
        public string imageHref;
        public string blurRadius;
        public string imageSize;
        public string imageRepeat;

        public WidgetBody()
        {
            name = "CanvasBody";
            tag = "body";
            HTML_TAG = "body";
            type = CommonType.color;
            photoType = PhotoType.link;

            color = "#ffffff";

            imageHref = "";
            blurRadius = "0";
            imageSize = imageSize_rus[1];
            imageRepeat = imageRepeat_rus[2];
        }
    }
}
