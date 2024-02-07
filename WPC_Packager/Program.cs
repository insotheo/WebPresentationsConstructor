using WPC_BUILD_CLASS;
using Newtonsoft.Json;
using System.IO;
using System;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace WPC_Packager
{
    class Packager
    {
        private static string addTabsToHtmlSource(string source)
        {
            if(source != String.Empty)
            {
                const string pattern = @"(<[^<>]+>)";
                const string tab = "\t";
                string formattedHtml = Regex.Replace(source, pattern, m => Environment.NewLine + tab + m.Value);
                return formattedHtml;
            }
            else
            {
                return String.Empty;
            }
        }

        private static string removeCommentsFromJS(string source)
        {
            if(source != String.Empty)
            {
                string pattern = @"(//.*?$|/\*(.|\n)*?\*/)";
                string cleanJsCode = Regex.Replace(source, pattern, string.Empty, RegexOptions.Multiline);
                return cleanJsCode;
            }
            else
            {
                return String.Empty;
            }
        }

        static void Main()
        {
            Console.Title = "WebPresentationsConstructor -> Packaging...";
            string jsonString = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "build_data.json"));
            Console.WriteLine(jsonString);
            BUILD_DATA_CLASS data = JsonConvert.DeserializeObject<BUILD_DATA_CLASS>(jsonString);
            Console.WriteLine("Save path: " + data.pathToSave);
            string[] files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "projects", data.projectName, ".build"));
            foreach(string file in files)
            {
                switch (Path.GetExtension(file).Trim().ToUpper())
                {
                    case ".HTML":
                        File.Copy(file, Path.Combine(data.pathToSave, Path.GetFileName(file)));
                        Console.WriteLine($"{file} file copied");
                        if (data.makeHtmlSourceTab)
                        {
                            string htmlSource = File.ReadAllText(Path.Combine(data.pathToSave, Path.GetFileName(file)));
                            htmlSource = addTabsToHtmlSource(htmlSource);
                            File.WriteAllText(Path.Combine(data.pathToSave, Path.GetFileName(file)), htmlSource);
                            Console.WriteLine("HTML file tabs added");
                        }
                        break;

                    case ".CSS":
                        string stylePath = Path.Combine(data.pathToSave, Path.GetFileName(file));
                        if (data.sort)
                        {
                            stylePath = Path.Combine(data.pathToSave, "Styles", Path.GetFileName(file));
                        }
                        File.Copy(file, stylePath);
                        Console.WriteLine($"{file} file copied");
                        break;

                    case ".JS":
                        string scriptPath = Path.Combine(data.pathToSave, Path.GetFileName(file));
                        if (data.sort)
                        {
                            scriptPath = Path.Combine(data.pathToSave, "Scripts", Path.GetFileName(file));
                        }
                        File.Copy(file, scriptPath);
                        Console.WriteLine($"{file} file copied");
                        if (data.removeCommentsFromJS)
                        {
                            string source = File.ReadAllText(scriptPath);
                            source = removeCommentsFromJS(source);
                            File.WriteAllText(scriptPath, source);
                            Console.WriteLine("JS file comments removed");
                        }
                        break;

                    default:
                        string path = Path.Combine(data.pathToSave, Path.GetFileName(file));
                        if (data.sort)
                        {
                            path = Path.Combine(data.pathToSave, "Resources", Path.GetFileName(file));
                        }
                        File.Copy(file, path);
                        Console.WriteLine($"{file} file copied");
                        break;
                }
            }
            Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "projects", data.projectName, ".build"), true);
            if (data.zip)
            {
                ZipFile.CreateFromDirectory(data.pathToSave, data.pathToSave + ".zip");
                Console.WriteLine("Archive created from folder");
                Directory.Delete(data.pathToSave, true);
                Console.WriteLine($"Folder({data.pathToSave}) deleted");
            }
            Console.WriteLine("Done!");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Press any key to exit the app]");
            Console.ReadKey();
            return;
        }
    }
}
