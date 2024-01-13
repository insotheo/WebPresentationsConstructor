using System.Collections.Generic;

namespace WPC_Editor.Widgets
{
    public class WidgetList : Widget
    {
        private new string HTML_TAG;

        public bool isNumeric;

        public List<Widget> content; //allow only { text, hyperlink, image, video, list, button }

        public WidgetList()
        {
            tag = "Список";
            name = $"List_{rndInd.Next(1, 99999)}";
            isNumeric = false;
            content = new List<Widget>();

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
            content.Add(widget);
        }

        public void removeContent(Widget widget)
        {
            content.Remove(widget);
        }

    }
}
