using System.Collections.Generic;

namespace WPC_Editor.Widgets
{
    public class WidgetSaveData
    {
        public string name = "";
        public WidgetText text;
        public WidgetLink link;
        public WidgetButton button;
        public WidgetImage image;
        public WidgetVideo video;
        public WidgetGroup group;
        public WidgetList list;
        public WidgetHtmlSource html;
        public WidgetNextLine nl ;
        public WidgetInput input;

        public List<WidgetSaveData> savedKids;

        public WidgetSaveData(Widget widget)
        {
            if (widget != null)
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
                        savedKids = saveKids(group.kids);
                        break;
                    case "Список":
                        list = widget as WidgetList;
                        savedKids = saveKids(list.content);
                        break;
                    case "HTML Source":
                        html = widget as WidgetHtmlSource;
                        break;
                    case "Перенос":
                        nl = widget as WidgetNextLine;
                        break;
                }
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

        public List<WidgetSaveData> saveKids(List<Widget> list)
        {
            List<WidgetSaveData> kids = new List<WidgetSaveData>();
            foreach(Widget widget in list)
            {
                kids.Add(new WidgetSaveData(widget));
            }
            return kids;
        }

        public List<Widget> getKids()
        {
            if(savedKids != null)
            {
                List<Widget> kids = new List<Widget>();
                foreach(var kid in savedKids)
                {
                    kids.Add(kid.getByName());
                }
                return kids;
            }
            else
            {
                return null;
            }
        }
    }
}
