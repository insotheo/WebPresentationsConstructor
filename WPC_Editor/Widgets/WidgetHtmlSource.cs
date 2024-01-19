namespace WPC_Editor.Widgets
{
    public class WidgetHtmlSource : Widget
    {
        public enum ContentTypeOfHtmlSource
        {
            file,
            text
        };

        public ContentTypeOfHtmlSource type;
        public string content;

        public WidgetHtmlSource()
        {
            type = ContentTypeOfHtmlSource.file;
            content = System.String.Empty;
            HTML_TAG = System.String.Empty;
            name = System.String.Empty;
            tag = "HTML Source";
        }

    }
}
