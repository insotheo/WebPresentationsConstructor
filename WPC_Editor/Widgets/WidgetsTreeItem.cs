using System.Collections.Generic;

namespace WPC_Editor.Widgets
{
    public class WidgetsTreeItem
    {
        public Widget widget;

        public string listText
        {
            get { return $"[{widget.name}]{widget.tag}"; }
            set { listText = value; }
        }

        public List<WidgetsTreeItem> widgetsOfScene { get; set; }

        public WidgetsTreeItem(Widget widget)
        {
            this.widget = widget;
            widgetsOfScene = new List<WidgetsTreeItem>();
            if(widget is WidgetGroup)
            {
                widgetsOfScene = setKids((widget as WidgetGroup).kids);
            }
            if(widget is WidgetList)
            {
                widgetsOfScene = setKids((widget as WidgetList).content);
            }
            if(widget is WidgetMarquee)
            {
                widgetsOfScene = setKids((widget as WidgetMarquee).elements);
            }
        }

        private List<WidgetsTreeItem> setKids(List<Widget> kids)
        {
            List<WidgetsTreeItem> newKids = new List<WidgetsTreeItem>();
            foreach (var item in kids)
            {
                newKids.Add(new WidgetsTreeItem(item));
            }
            return newKids;
        }

    }
}
