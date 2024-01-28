using System.IO;
using WPC_Editor.Widgets;
using WPC_Encrypting;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WPC_Editor.DataWorkerClasses
{
    public class DataLoader
    {
        public WidgetBodySaveData load(string loadPath)
        {
            if(!File.Exists(loadPath))
            {
                throw new Exception("Файл не существует");
            }
            string[] content = File.ReadAllLines(loadPath);
            string str = "";
            for (int i = 0; i < content.Length; i++)
            {
                str += DecryptingClass.decryptString(content[i]);
            }
            WidgetBodySaveData wbsd = null;
            wbsd = JsonConvert.DeserializeObject<WidgetBodySaveData>(str);
            //123
            Task.Delay(10).Wait();
            return wbsd;
        }

    }
}
