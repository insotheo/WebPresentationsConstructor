using System;
using System.IO;
using System.Collections.Generic;

//using MessageBoxesWindows;

namespace WPC_Editor
{
    class ConfigWorker
    {
        public string title;
        public string language;
        private string creatorsUsername;
        private string creatorsMachineName;
        private string creatorsOS;
        private string is64Bit;
        public List<string> usingStyles;
        public List<string> usingScripts;
        public string appEditorVersion;

        public ConfigWorker(string configFilePath)
        {
            string[] configData = File.ReadAllLines(configFilePath);
            for(int i = 0; i < configData.Length; i++)
            {
                configData[i] = configData[i].Trim().Split(':')[1];
            }
            usingScripts = new List<string>();
            usingStyles = new List<string>();
            title = configData[0].Trim();
            language = configData[1].Trim();
            creatorsUsername = configData[2].Trim();
            creatorsMachineName = configData[3].Trim();
            creatorsOS = configData[4].Trim();
            is64Bit = configData[5].Trim();
            foreach(string style in configData[6].Trim().Split(';'))
            {
                usingStyles.Add(style);
            }
            foreach(string script in configData[7].Trim().Split(';'))
            {
                usingScripts.Add(script);
            }
            appEditorVersion = configData[8].Trim();
        }

        public bool isTheSamePC()
        {
            if(Environment.UserName == creatorsUsername &&
                Environment.MachineName == creatorsMachineName &&
                Environment.OSVersion.VersionString == creatorsOS &&
                Environment.Is64BitOperatingSystem.ToString() == is64Bit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
