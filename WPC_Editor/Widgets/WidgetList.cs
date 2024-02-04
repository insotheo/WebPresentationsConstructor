using MessageBoxesWindows;
using System.Collections.Generic;

namespace WPC_Editor.Widgets
{
    public class WidgetList : Widget
    {
        private new string HTML_TAG;

        public bool isNumeric;

        public string fontColor;
        public string fontFamily;
        public string fontWeight;
        public int fontSize;
        public string backgroundColor;
        public string borderRadius;
        public string margin;

        public List<Widget> content; //allow only { text, hyperlink, image, video, list, button }

        public WidgetList()
        {
            tag = "Список";
            name = $"List_{rndInd.Next(1, 99999)}";
            isNumeric = false;
            content = new List<Widget>();
            fontColor = "#000000";
            fontFamily = "Arial";
            fontWeight = "400";
            fontSize = 18;
            backgroundColor = "Transparent";
            borderRadius = "0";
            margin = "0 0 0 0";
        }

        public string getHTML_TAG()
        {
            if (isNumeric)
            {
                HTML_TAG = "ol";
            }
            else
            {
                HTML_TAG = "ul";
            }
            return HTML_TAG;
        }

        public void addContent(Widget widget)
        {
            if (widget is WidgetText || widget is WidgetImage || widget is WidgetVideo || widget is WidgetList || widget is WidgetButton || widget is WidgetSquareShape)
                content.Add(widget);
            else
            {
                MessBox.showInfo($"Элемент \"{widget.tag}\" не может являться элементом списка!");
            }
        }

        public void removeContent(Widget widget)
        {
            content.Remove(widget);
        }

        public bool doesContentExist(Widget widget)
        {
            return content.Contains(widget);
        }

    }
}
