using System.Collections.Generic;

namespace WPC_Editor.Widgets
{
    public class WidgetBodySaveData
    {
        public WidgetBody body;
        public List<WidgetSaveData> widgets;

        public WidgetBodySaveData(WidgetBody body, List<WidgetSaveData> widgets)
        {
            this.body = body;
            this.widgets = widgets;
        }
    }
}
