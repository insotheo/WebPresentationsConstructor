using System;
using System.IO;
using System.Collections.Generic;
using MessageBoxesWindows;

namespace WPC_Editor
{
   public class ConfigWorker
   {
        public string title;
        public string language;
        public List<string> usingStyles;
        public List<string> usingScripts;
        public readonly string appEditorVersion;
        public string charset;

        private string creatorsUsername;
        private string creatorsMachineName;
        private string creatorsOS;
        private string is64Bit;

        private string path;

        public ConfigWorker(string configFilePath)
        {
            path = configFilePath;
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
                usingStyles.Add(style.Trim());
            }
            foreach(string script in configData[7].Trim().Split(';'))
            {
                usingScripts.Add(script.Trim());
            }
            appEditorVersion = configData[8].Trim();
            charset = configData[9].Trim();
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

        public void updateInformationAboutTheCreator()
        {
            this.creatorsUsername = Environment.UserName;
            this.creatorsMachineName = Environment.MachineName;
            this.creatorsOS = Environment.OSVersion.VersionString;
            this.is64Bit = Environment.Is64BitOperatingSystem.ToString();
            if (!isTheSamePC())
            {
                throw new Exception("Не получилось изменить config-файл!");
            }
            else
            {
                MessBox.showInfo("Успешно!");
            }
        }

        public void overwriteFile()
        {
            string scriptsLine = String.Empty, stylesLine = String.Empty;
            if (usingScripts.Count > 0)
            {
                for (int i = 0; i < usingScripts.Count; i++)
                {
                    if (i + 1 == usingScripts.Count)
                    {
                        scriptsLine += usingScripts[i];
                    }
                    else
                    {
                        scriptsLine += usingScripts[i] + " ; ";
                    }
                }
            }
            if (usingStyles.Count > 0)
            {
                for (int i = 0; i < usingStyles.Count; i++)
                {
                    if (i + 1 == usingStyles.Count)
                    {
                        stylesLine += usingStyles[i];
                    }
                    else
                    {
                        stylesLine += usingStyles[i] + " ; ";
                    }
                }
            }
            string ans = $"title: {title}" +
                $"\nlanguage: {language}" +
                $"\ncreatorsUsername: {creatorsUsername}" +
                $"\ncreatorsMachineName: {creatorsMachineName}" +
                $"\ncreatorsOS: {creatorsOS}" +
                $"\nis64Bit: {is64Bit}" +
                $"\nusingStyles: {stylesLine}" +
                $"\nusingScripts: {scriptsLine}" +
                $"\nappEditorVersion: {appEditorVersion}" +
                $"\ncharset: {charset}";

            File.WriteAllText(path, ans);
        }
   }
}
