using System.IO;
using WPC_Editor.Widgets;
using System.Collections.Generic;
using WPC_Encrypting;
using Newtonsoft.Json;

namespace WPC_Editor.DataWorkerClasses
{
    public class DataSaver
    {

        public void save(WidgetBody body, List<WidgetsTreeItem> widgets, string path)
        {
            List<WidgetSaveData> widgs = new List<WidgetSaveData>();
            foreach (WidgetsTreeItem item in widgets)
            {
                widgs.Add(new WidgetSaveData(item.widget));
            }
            WidgetBodySaveData data = new WidgetBodySaveData(body, widgs);
            string jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented);
            string[] finData = jsonStr.Split('\n');
            for(int i = 0; i < finData.Length; i++)
            {
                finData[i] = EncryptingClass.encryptString(finData[i]);
            }
            File.WriteAllLines(path, finData);
        }

    }
}
