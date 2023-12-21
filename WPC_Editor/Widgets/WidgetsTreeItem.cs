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
        }

    }
}
