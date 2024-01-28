namespace WPC_Editor.Widgets
{
    public class WidgetSaveData
    {
        public string name = "";
        public WidgetText text = new WidgetText();
        public WidgetLink link = new WidgetLink();
        public WidgetButton button = new WidgetButton();
        public WidgetImage image = new WidgetImage();
        public WidgetVideo video = new WidgetVideo();
        public WidgetGroup group = new WidgetGroup();
        public WidgetList list = new WidgetList();
        public WidgetHtmlSource html = new WidgetHtmlSource();
        public WidgetNextLine nl = new WidgetNextLine();
        public WidgetInput input = new WidgetInput();

        public WidgetSaveData(Widget widget)
        {
            name = widget.tag;
            switch (widget.tag)
            {
                case "Текст":
                    text = widget as WidgetText;
                    break;
                case "Ссылка":
                    link = widget as WidgetLink;
                    break;
                case "Кнопка":
                    button = widget as WidgetButton;
                    break;
                case "Фото":
                    image = widget as WidgetImage;
                    break;
                case "Видео":
                    video = widget as WidgetVideo;
                    break;
                case "Ввод":
                    input = widget as WidgetInput;
                    break;
                case "Группа":
                    group = widget as WidgetGroup;
                    break;
                case "Список":
                    list = widget as WidgetList;
                    break;
                case "HTML Source":
                    html = widget as WidgetHtmlSource;
                    break;
                case "Перенос":
                    nl = widget as WidgetNextLine;
                    break;
            }
        }

        public Widget getByName()
        {
            switch (name)
            {
                case "Текст":
                    return text;
                case "Ссылка":
                    return link;
                case "Кнопка":
                    return button;
                case "Фото":
                    return image;
                case "Видео":
                    return video;
                case "Ввод":
                    return input;
                case "Группа":
                    return group;
                case "Список":
                    return list;
                case "HTML Source":
                    return html;
                case "Перенос":
                    return nl;
                default:
                    return new Widget();
            }
        }
    }
}
