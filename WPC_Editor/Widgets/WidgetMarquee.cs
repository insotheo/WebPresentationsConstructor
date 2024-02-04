using MessageBoxesWindows;
using System.Collections.Generic;

namespace WPC_Editor.Widgets
{
    public class WidgetMarquee : Widget
    {
        public static readonly string[] behaviorOptions = { "scroll", "slide", "alternate" }; 
        public static readonly string[] behaviorOptions_rus = { "бесконечность", "до столкновения", "отражение" };

        public static readonly string[] direction = { "right", "left", "up", "down" };
        public static readonly string[] direction_rus = { "вправо", "влево", "вверх", "вниз" };

        public List<Widget> elements;

        public string behavior;
        public string dir;
        public string loop;
        public string scrollAmount;
        public string backgroundColor;
        public string margin;

        public WidgetMarquee()
        {
            HTML_TAG = "marquee";
            tag = "Прокрутка";
            useStyle = false;
            name = $"Marquee_{rndInd.Next(1, 99999)}";
            elements = new List<Widget>();
            behavior = behaviorOptions_rus[0];
            dir = direction_rus[0];
            loop = "-1";
            backgroundColor = "Transparent";
            scrollAmount = "6";
            margin = "0 0 0 0";
        }

        public void addElement(Widget widget)
        {
            if (widget is WidgetText || widget is WidgetImage || widget is WidgetSquareShape)
                elements.Add(widget);
            else
            {
                MessBox.showInfo($"Элемент \"{widget.tag}\" не может являться дочерним элементом прокрутки!");
            }
        }

        public void removeElement(Widget widget)
        {
            elements.Remove(widget);
        }

        public bool doesElementExist(Widget widget)
        {
            return elements.Contains(widget);
        }

    }
}
