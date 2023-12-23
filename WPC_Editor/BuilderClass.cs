using System;
using System.Collections.Generic;
using System.IO;
using WPC_Editor.Widgets;

namespace WPC_Editor
{
    class BuilderClass
    {
        private string cacheFolder;
        private string assetsFolder;

        public BuilderClass(string folder, string assetsFolder)
        {
            this.cacheFolder = folder;
            this.assetsFolder = assetsFolder;
        }

        public void Init(ref ConfigWorker config)
        {
            foreach (string file in config.usingStyles)
            {
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "styles", file.Trim())) &&
                    !File.Exists(Path.Combine(assetsFolder, file.Trim())))
                {
                    throw new Exception($"Файл \"{file.Trim()}\" не был найден!");
                }
            }
            foreach (string file in config.usingScripts)
            {
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "scripts", file.Trim())) &&
                    !File.Exists(Path.Combine(assetsFolder, file.Trim())))
                {
                    throw new Exception($"Файл \"{file.Trim()}\" не был найден!");
                }
            }
            if (!File.Exists(Path.Combine(cacheFolder, "INDEX.html")))
            {
                FileStream indexHTML = File.Create(Path.Combine(cacheFolder, "INDEX.html"));
                indexHTML.Close();
            }
            else
            {
                Directory.Delete(cacheFolder, true);
                Directory.CreateDirectory(cacheFolder);
                FileStream indexHTML = File.Create(Path.Combine(cacheFolder, "INDEX.html"));
                indexHTML.Close();
            }
        }

        public void fastBuild(List<WidgetsTreeItem> list, ref ConfigWorker config)
        {
            Directory.Delete(cacheFolder, true);
            Directory.CreateDirectory(cacheFolder);
            FileStream indexHTML = File.Create(Path.Combine(cacheFolder, "INDEX.html"));
            indexHTML.Close();

            string finalText = "<!DOCTYPE html>" +
                $"\n<html lang=\"{config.language}\">" +
                "\n<head>" +
                $"\n<title>{config.title}</title>" +
                $"\n<meta charset=\"{config.charset.ToLower()}\">";

            if (config.usingStyles.Count > 0)
            {
                foreach (var style in config.usingStyles)
                {
                    if (File.Exists(Path.Combine(assetsFolder, style)))
                    {
                        File.Copy(Path.Combine(assetsFolder, style), Path.Combine(cacheFolder, style));
                    }
                    else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "styles", style)))
                    {
                        File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "styles", style), Path.Combine(cacheFolder, style));
                    }
                    else
                    {
                        throw new Exception("Ошибка сборки! Файл не найден!");
                    }
                    finalText += $"\n<link rel=\"stylesheet\" href=\"{style}\">";
                }
            }
            if (config.usingScripts.Count > 0)
            {
                foreach (var script in config.usingScripts)
                {
                    if (File.Exists(Path.Combine(assetsFolder, script)))
                    {
                        File.Copy(Path.Combine(assetsFolder, script), Path.Combine(cacheFolder, script));
                    }
                    else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "styles", script)))
                    {
                        File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "styles", script), Path.Combine(cacheFolder, script));
                    }
                    else
                    {
                        throw new Exception("Ошибка сборки! Файл не найден!");
                    }
                    finalText += $"\n<script src=\"{script}\"></script>";
                }
            }

            finalText += "\n</head>\n<body>";
            foreach (WidgetsTreeItem el in list)
            {
                string line = "";

                var widget = el.widget;
                if (widget.HTML_TAG == "p")
                {
                    var text = widget as WidgetText;
                    string s = String.Empty;
                    if (widget.useStyle == false)
                    {
                        s = $"style=\"font-size: {text.fontSize.ToString()}pt; font-family: {text.fontFamily}; font-weight: {text.fontWeight}; color: {text.fontColorHEX.ToLower()};\"";
                    }
                    line = $"<{widget.HTML_TAG} id=\"{text.name}\" {s}>{(widget as WidgetText).content}</{widget.HTML_TAG}>";
                }

                finalText += "\n" + line;
            }
            finalText += "\n</body>\n</html>\n";

            File.WriteAllText(Path.Combine(cacheFolder, "INDEX.html"), finalText);
        }
    }
}
