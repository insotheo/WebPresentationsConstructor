namespace WPC_Editor.Widgets
{
    public class WidgetNextLine : Widget
    {
        public int repeatTime;

        public WidgetNextLine()
        {
            HTML_TAG = "br";
            name = null;
            tag = "Перенос";
            repeatTime = 1;
        }

    }
}
