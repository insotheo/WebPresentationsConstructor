namespace WPC_Editor.Widgets
{
    public class WidgetInput : Widget
    {
        public static readonly string[] rus_types = { "текст", "пароль", "число", "диапазон", "дата", "время", "цвет" };
        public static readonly string[] types = { "text", "password", "number", "range", "date", "time", "color" };

        public string type;
        public string placeholder;
        public bool isReadonly;
        public string content;
        
        public int fontSize;
        public string fontFamily;
        public string fontWeight;
        public string fontColorHEX;
        public string backgroundColorHEX;
        public string backgroundRad;

        public WidgetInput()
        {
            HTML_TAG = "input";
            tag = "Ввод";
            name = $"Input_{rndInd.Next(1, 99999)}";
            type = rus_types[0];
            placeholder = rus_types[0];
            isReadonly = false;
            content = System.String.Empty;
            fontSize = 18;
            fontFamily = "Arial";
            fontColorHEX = "#000000";
            fontWeight = "400";
            backgroundColorHEX = "Transparent";
            backgroundRad = "0";
        }
    }
}
