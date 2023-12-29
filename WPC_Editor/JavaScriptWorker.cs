using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using MessageBoxesWindows;

namespace WPC_Editor
{
    class JavaScriptWorker
    {

        public static List<string> getAllEvents(ref ConfigWorker config, string assetsFolder)
        {
            List<string> events = new List<string>();
            string path = null;
            string funcNamePreficks = @"//_EDITOR::FUNCTIONS::TYPES::EVENT\s*function\s+(\w+)\s*\(";
            try
            {
                foreach(string file in config.usingScripts)
                {
                    if(File.Exists(Path.Combine(assetsFolder, file)))
                    {
                        path = Path.Combine(assetsFolder, file);
                    }
                    else if(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "scripts", file)))
                    {
                        path = Path.Combine(Directory.GetCurrentDirectory(), "scripts", file);
                    }
                    else
                    {
                        throw new Exception("Файл не найден!");
                    }
                    string programLines = File.ReadAllText(path);
                    MatchCollection matches = Regex.Matches(programLines, funcNamePreficks);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            string funcName = match.Groups[1].Value;
                            events.Add(funcName);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessBox.showError(ex.Message);
            }
            return events;
        }
    }
}
